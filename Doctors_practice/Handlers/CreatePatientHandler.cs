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
                    EventData eventData = eventProducer.CreateEventData(_patient, "Patient-Created");
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
                    
                    var message = await _client.SendMessageAsync("EmailQueue", "PatientCreated");
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
                        EventData eventData = eventProducer.CreateEventData(_patient, "Patient-Deleted");
                        eventProducer.SendEvent($"Patient-{_patient.ID}", eventData);
                    }
                    tableInsertStatus = false;
                }
            }
            if (tableInsertStatus==false)
            {
                return null;
            }
            return _patient;
        }
    }
}
