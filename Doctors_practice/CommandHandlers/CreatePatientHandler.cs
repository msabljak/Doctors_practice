using Apache.NMS;
using Doctors_practice.Commands;
using Doctors_practice.Models;
using Doctors_practice.Models.Patient;
using EventStore.ClientAPI;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventStoreImplementation;

namespace Doctors_practice.Handlers
{
    public class CreatePatientHandler : IRequestHandler<CreatePatientCommand, PatientDTO>
    {
        private IPatientRepository _patientRepository;
        private IPatientClient _client;
        private IEventStore _eventStore;
        private PatientDTO _patient;
        private bool _workerSucceeded;

        public CreatePatientHandler(IPatientRepository patientRepository, IPatientClient client, IEventStore eventStore)
        {
            _patientRepository = patientRepository;
            _client = client;
            _eventStore = eventStore;
        }
        public async Task<PatientDTO> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
        {
            bool tableInsertStatus;
            try
            {
                _patient = await _patientRepository.AddAsync(request.PatientDTO);
                using (EventProducer eventProducer = _eventStore.CreateEventProducer())
                {
                    EventData eventData = eventProducer.CreateEventData(_patient, "Patient-Creation-Pending");
                    eventProducer.SendEvent($"Patient-{_patient.ID}", eventData);
                }
                tableInsertStatus = true;
            }
            catch (Exception)
            {
                tableInsertStatus = false;
            }
            if (tableInsertStatus == true)
            {
                try
                {
                    PatientMessage patientMessage = new PatientMessage("PatientCreated", _patient);
                    var message = await _client.SendObjectMessageAsync("EmailQueue", patientMessage);
                    using (EventProducer eventProducer = _eventStore.CreateEventProducer())
                    {
                        EventData eventData = eventProducer.CreateEventData(message, "PatientCreatedConfirmation-Sent");
                        eventProducer.SendEvent($"AMQMessages-Patient-{_patient.ID}", eventData);
                    }
                }
                catch (Exception)
                {
                    await _patientRepository.DeleteAsync(_patient.ID);
                    using (EventProducer eventProducer = _eventStore.CreateEventProducer())
                    {
                        EventData eventData = eventProducer.CreateEventData(_patient, "Patient-Creation-Rollback");
                        eventProducer.SendEvent($"Patient-{_patient.ID}", eventData);
                    }
                    tableInsertStatus = false;
                }
            }
            using (EventConsumer eventConsumer = _eventStore.CreateEventConsumer($"AMQMessages-Patient-{_patient.ID}","EmailCheck"))
            {
                bool eventFound = false;
                while (eventFound == false)
                {
                    var events = await eventConsumer.ReadStreamEventsForwardAsync();
                    if (eventConsumer.FindEventType(events, "EmailConfirmation-Sent"))
                    {
                        eventFound = true;
                        _workerSucceeded = true;
                    }
                    else if (eventConsumer.FindEventType(events, "EmailConfirmation-Failed"))
                    {
                        eventFound = true;
                        _workerSucceeded = false;
                    }
                }
            }
            if (_workerSucceeded == false)
            {
                await _patientRepository.DeleteAsync(_patient.ID);
                using (EventProducer eventProducer = _eventStore.CreateEventProducer())
                {
                    EventData eventData = eventProducer.CreateEventData(_patient, "Patient-Creation-Rollback");
                    eventProducer.SendEvent($"Patient-{_patient.ID}", eventData);
                }
                tableInsertStatus = false;
            }
            if (tableInsertStatus==false)
            {
                return null;
            }
            using (EventProducer eventProducer = _eventStore.CreateEventProducer())
            {
                EventData eventData = eventProducer.CreateEventData(_patient, "Patient-Creation-Committed");
                eventProducer.SendEvent($"Patient-{_patient.ID}", eventData);
            }
            return _patient;
        }
    }
}
