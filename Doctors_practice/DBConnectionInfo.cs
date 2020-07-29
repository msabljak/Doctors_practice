using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice
{
    public class DBConnectionInfo
    {
        public SqlTransaction Transaction { get; set; }
        public SqlConnection Connection { get; set; }

        public DBConnectionInfo(SqlTransaction transaction, SqlConnection connection)
        {
            Transaction = transaction;
            Connection = connection;
        }
    }
}
