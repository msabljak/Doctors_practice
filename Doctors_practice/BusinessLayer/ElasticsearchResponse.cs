using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doctors_practice.BusinessLayer
{
    public class ElasticsearchResponse
    {
        private int _id;

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        private string _objectType;

        public string ObjectType
        {
            get { return _objectType; }
            set { _objectType = value; }
        }

        public ElasticsearchResponse()
        {

        }

        public ElasticsearchResponse(int id, string objectType)
        {
            _id = id;
            _objectType = objectType;
        }

    }
}
