using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Yogagiv
{
    public class AppLogic
    {
        public SqlConnection Connection { get; set; }

        public List<Customer> Customers { get; set; }

        public AppLogic()
        {
            Customers = new List<Customer>();
        }


    }
}
