using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Yogagiv
{
    public class ApplicationLogic
    {
        public SqlConnection Connection { get; set; }

        public void UpdateData(string i_Name, string i_Month, string i_Date, string i_Paid)
        {
            if (!customerExists(i_Name)) // add customer to database
            {
                string query1 = string.Format("insert into Customers (Name, NumOfClass) values('{0}', 1)",
                    i_Name);
                new SqlCommand(query1, Connection).ExecuteNonQuery();
            }

            string query2 = string.Format("select NumOfClass from Customers where Name='{0}'",i_Name); // find num Of class
            SqlCommand numOfClassCommand = new SqlCommand(query2,Connection);
            var reader = numOfClassCommand.ExecuteReader();
            reader.Read();
            int numOfClass = reader.GetInt32(0);
            reader.Close();
            if (!customerMonthExists(i_Name, i_Month)) // update month
            {
                string query3 = string.Format("insert into Month (Name, Month) values('{0}','{1}')",
                  i_Name, i_Month);
                SqlCommand command = new SqlCommand(query3,Connection);
                command.ExecuteNonQuery();
            }

            string query4; // update class
            if (!string.IsNullOrEmpty(i_Paid))
            {
                query4 = string.Format("update Month set [{0}]='{1}', Paid={2} where Name= '{3}' and Month='{4}'",
                        numOfClass, i_Date, i_Paid, i_Name, i_Month);
            }
            else
            {
                query4 = string.Format("update Month set [{0}]='{1}' where Name= '{2}' and Month='{3}'",
                        numOfClass, i_Date, i_Name, i_Month);
            }

            new SqlCommand(query4, Connection).ExecuteNonQuery();
            string query5 = string.Format("update Customers set NumOfClass+=1 where Name='{0}'",
                i_Name); // increase num of classes
            new SqlCommand(query5, Connection).ExecuteNonQuery();
        }

        private bool customerMonthExists(string i_Name, string i_Month)
        {
            string query = string.Format("select * from Month where Name='{0}' and Month='{1}'",
                    i_Name, i_Month);
            if (new SqlCommand(query, Connection).ExecuteScalar() != null)
                return true;
            return false;
        }

        private bool customerExists(string i_Name)
        {
            string query = string.Format("select * from Customers where Name= '{0}'", i_Name);
            if (new SqlCommand(query, Connection).ExecuteScalar() != null) // uset exists
                return true;
            return false;
        }
    }
}
