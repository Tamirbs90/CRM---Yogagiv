using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yogagiv
{
    public class Customer
    {
        public string Name { get; set; }
        public int NumOfClass { get; set; }

        public Customer(string i_Name)
        {
            Name = i_Name;
            NumOfClass = 1;
        }


} 
}
