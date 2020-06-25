using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using CivinTecAccessManager;

namespace UnitTestCivinTec
{
    [TestClass]
    public class UnitTestCivinTec
    {
        private string iPAddress = "180.255.25.48";
        [TestMethod]
        public void TestConnection()
        {
            clsAccessManager cls = new clsAccessManager();
            bool bFlag = false;
            bFlag = cls.getConnection(iPAddress);
            if(bFlag == true)
            {
                System.Diagnostics.Trace.WriteLine("Connected successfully.");                
            }                
            else
            {
                System.Diagnostics.Trace.WriteLine("Connection failed.");
            }                            
        }

        [TestMethod]
        public void getUsers()
        {
            clsAccessManager cls = new clsAccessManager();
            bool bFlag = false;
            bFlag = cls.getConnection(iPAddress);
            if (bFlag == true)
            {
                System.Diagnostics.Trace.WriteLine("Connected successfully.");
                List<User> UserList = new List<User>();
                UserList = cls.getUsers();
                if (UserList.Count > 0)
                {
                    foreach (User u in UserList)
                    {
                        System.Diagnostics.Trace.WriteLine("PIN :" + u.Pin.ToString());
                        System.Diagnostics.Trace.WriteLine("Name:" + u.LastName + "," + u.MiddleName + " " + u.FirstName);
                    }
                }
                else
                { System.Diagnostics.Trace.WriteLine("There is no users found to display."); }

            }
            else
            {
                System.Diagnostics.Trace.WriteLine("Connection failed.");
            }
        }

        [TestMethod]
        public void getTransactions()
        {
            clsAccessManager cls = new clsAccessManager();
            bool bFlag = false;
            bFlag = cls.getConnection(iPAddress);
            if (bFlag == true)
            {
                System.Diagnostics.Trace.WriteLine("Connected successfully.");
                List<Transaction> TranList = new List<Transaction>();
                TranList = cls.getTransactions(0);
                if (TranList.Count > 0)
                {
                    //foreach (Transaction u in TranList)
                    //{
                    //    System.Diagnostics.Trace.WriteLine("User Name: " + u.objUser.LastName + " , PIN :" + u.Pin.ToString());
                    //    System.Diagnostics.Trace.WriteLine("Type     : " + u.Type + ", TypeName :" + u.TypeName);
                    //    System.Diagnostics.Trace.WriteLine("Job Type :" + u.JobCode + ":" + u.JobCodeDesc + " >> " + u.DateAndTime);
                    //}
                    System.Diagnostics.Trace.WriteLine("ID >> From :  " + TranList[0].ID + " To :" + TranList[TranList.Count-1].ID);
                    System.Diagnostics.Trace.WriteLine("Trans Count:  " + TranList.Count );
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
