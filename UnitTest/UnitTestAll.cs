﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cerealizer;
using System.Collections.Generic;

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
            testSub.ID = 4;

            Cerealizer<SupObj> subTest = new Cerealizer<SupObj>(testSup);
            subTest["Neighborhood"] = "Test";
            subTest["Home"] = "ThisIsText";
            subTest["Address"] = "4374";
            subTest["myObj"] = testSub;
            subTest["ID"] = 1;

            System.Data.SqlClient.SqlConnection testConn = new System.Data.SqlClient.SqlConnection("Data Source=(LocalDB)\\v11.0;AttachDbFilename=|DataDirectory|\\Test.db.mdf;Integrated Security=True;Connect Timeout=30");

            //Reflux.FormatTable(subTest, testConn);

            //subTest["ID"] = Reflux.InsertIntoTable(subTest, testConn);
            //List<object> temp = Reflux.SelectFromTable(new Cerealizer<SubObj>(), testConn, new System.Collections.Generic.Dictionary<string, object> { });
            List<object> temp = Reflux.SelectFromTable(new Cerealizer<SupObj>(), testConn, new System.Collections.Generic.Dictionary<string, object> { });
            //SubObj temp = (SubObj)Reflux.SelectFromTable(new Cerealizer<SubObj>(), testConn, new System.Collections.Generic.Dictionary<string, object> { })[0];
            //SupObj temp = (SupObj)Reflux.SelectFromTable(typeof(SupObj), testConn);
            Console.WriteLine(((SupObj)temp[0]).Neighborhood);

            testSub.Name = "Chuck Norris";
            subTest["Address"] = "New Address";

            //Reflux.UpdateTable(subTest, testConn, new System.Collections.Generic.Dictionary<string, object> { { "ID", 1 } });
            ((SupObj)temp[0]).Neighborhood = "OH YEAH!";

            Reflux.UpdateTable(new Cerealizer<SupObj>(((SupObj)temp[0])), testConn);

            Console.WriteLine(((SupObj)temp[0]).Neighborhood);
            //Cerealizer<SubObj> newTemp = new Cerealizer<SubObj>(temp);
            //Reflux.DeleteFromTable(new Cerealizer<SupObj>(((SupObj)temp[0])), testConn, true);
            //SubObj temp1 = (SubObj)Reflux.SelectFromTable(new Cerealizer<SubObj>(), testConn, new System.Collections.Generic.Dictionary<string, object> { })[0];
            //Reflux.DeleteFromTable(newTemp, testConn, true);

            //SupObj temp1 = (SupObj)Reflux.SelectFromTable(typeof(SupObj), testConn, new System.Collections.Generic.Dictionary<string, object> { })[0];

            //Console.WriteLine(temp1.myObj.Name);

            //Reflux.DeleteFromTable(subTest, testConn, new System.Collections.Generic.Dictionary<string, object> { { "ID", 1 } }, true);

        }
    }
}
