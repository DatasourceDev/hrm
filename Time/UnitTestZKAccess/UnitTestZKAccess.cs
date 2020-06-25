using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ZKAccessManager;

namespace UnitTestZKAccess
{
    [TestClass]
    public class UnitTestZKAccess
    {
        string iPAddress = "bcckl2.dyndns.org"; //"192.168.0.115";
        int Port = 4370;
        [TestMethod]
        public void TestConnection()
        {
            clsZKAccessManager cls = new clsZKAccessManager();
            bool bFlag = false;
            bFlag = cls.getConnection(iPAddress, Port);
            if (bFlag == true)
            {
                System.Diagnostics.Trace.WriteLine("Connected successfully.");
            }
            else
            {
                System.Diagnostics.Trace.WriteLine("Connection failed.");
            }           
        }

        [TestMethod]
        public void getTransactions()
        {
            clsZKAccessManager cls = new clsZKAccessManager();            
            bool bFlag = cls.getConnection(iPAddress, Port);
            if (bFlag == true)
            {
                System.Diagnostics.Trace.WriteLine("Connected successfully.");
                List<Transaction> TranList = new List<Transaction>();
                TranList = cls.getTransactions(iPAddress, Port);
                if (TranList.Count > 0)
                {
                    foreach (Transaction u in TranList)
                    {
                        System.Diagnostics.Trace.WriteLine("ID: " + u.ID);
                        System.Diagnostics.Trace.WriteLine("User Name: " + u.objUser.Name);                        
                        System.Diagnostics.Trace.WriteLine("Job Type :" + u.JobCode + ":" + u.JobCodeDesc + " >> " + u.DateAndTime);
                    }
                }
                else
                { System.Diagnostics.Trace.WriteLine("There is no transactions found to display."); }

            }
            else
            {
                System.Diagnostics.Trace.WriteLine("Connection failed.");
            }
        }
    }
}
