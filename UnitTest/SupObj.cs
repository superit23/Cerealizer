using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cerealizer;

namespace UnitTest
{
    class SupObj
    {
        public string Neighborhood
        { get; set; }

        public string Home
        { get; set; }

        public string Address
        { get; set; }

        [Exclude]
        public string SSN
        { get; set; }

        public SubObj myObj
        { get; set; }

        [PrimaryKey]
        public int ID
        { get; set; }
    }
}
