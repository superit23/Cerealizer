using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cerealizer;

namespace UnitTest
{
    class SubObj
    {
        public string Name
        { get; set; }

        public string Text
        { get; set; }

        public int Age
        { get; set; }

        [Exclude]
        public string ShouldNotAppear
        { get; set; }

        [PrimaryKey]
        public int ID
        { get; set; }
    }
}
