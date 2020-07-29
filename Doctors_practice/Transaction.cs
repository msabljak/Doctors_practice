using Apache.NMS;
using Doctors_practice.Models;
using Doctors_practice.Models.Patient;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice
{
    public class Transaction
    {
        private IPatientClient _client;
        private IPatientRepository _repository;
        private DBConnectionInfo _dBConnectionInfo;
        private MessengerConnectionInfo _messengerConnectionInfo;

        public Transaction(IPatientClient client, IPatientRepository patientRepository)
        {
            _client = client;
            _repository = patientRepository;
        }

        public bool ExecuteTransaction(PatientDTO patient, string destination, string message)
        {
            bool failedDBCommand = false;
            bool failedMessengerCommand = false;

            try
            {
                _dBConnectionInfo = _repository.PrepareAdd(patient);
                try
                {
                    _messengerConnectionInfo = _client.SendTransactionalMessage(destination, message);
                }
                catch
                {
                    failedMessengerCommand = true;
                }
            }
            catch
            {
                failedDBCommand = true;
            }
            if (failedDBCommand == true || failedMessengerCommand == true)
            {
                _client.RollbackTransactionalMessage(_messengerConnectionInfo.Connection, _messengerConnectionInfo.Session);
                _repository.RollbackAdd(_dBConnectionInfo.Connection, _dBConnectionInfo.Transaction);
                return false;
            }
            else
            {
                _client.CommitTransactionalMessage(_messengerConnectionInfo.Connection, _messengerConnectionInfo.Session);
                _repository.CommitAdd(_dBConnectionInfo.Connection, _dBConnectionInfo.Transaction);
                return true;
            }
        }
    }
}
