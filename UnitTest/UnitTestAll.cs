using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cerealizer;

namespace UnitTest
{
    [TestClass]
    public class UnitTestAll
    {
        [TestMethod]
        public void TestMethod1()
        {
            SupObj testSup = new SupObj();
            SubObj testSub = new SubObj();

            testSub.Name = "George Clooney";
            testSub.Age = 10;
            testSub.Text = "ALLO!";
            testSub.ShouldNotAppear = "I WON'T EVEN SEE THIS!";

            Cerealizer<SupObj> subTest = new Cerealizer<SupObj>(testSup);
            subTest["Neighborhood"] = "Test";
            subTest["Home"] = "ThisIsText";
            subTest["Address"] = "4374";
            subTest["myObj"] = testSub;

            System.Data.SqlClient.SqlConnection testConn = new System.Data.SqlClient.SqlConnection("Data Source=(LocalDB)\\v11.0;AttachDbFilename=|DataDirectory|\\Test.db.mdf;Integrated Security=True;Connect Timeout=30");

            //Reflux.FormatTable(subTest, testConn);

           //Reflux.InsertIntoTable(subTest, testConn);
            SupObj temp = (SupObj)Reflux.SelectFromTable(typeof(SupObj), testConn, new System.Collections.Generic.Dictionary<string, object> { })[0];

            Console.WriteLine(temp.myObj.Name);



        }
    }
}
