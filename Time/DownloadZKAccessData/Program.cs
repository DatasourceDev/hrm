using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKAccessManager;
using DownloadZKAccessData.Service;
using DownloadZKAccessData.Models;
using SBSModel.Common;

namespace DownloadZKAccessData
{
    class Program
    {
        static TimeService svc = null;   
        static void Main(string[] args)
        {
            System.Diagnostics.Debugger.Break();
            svc = new TimeService();
            List<Time_Device> lstDev = svc.LstZKTimeDeviceFromDB();
            if (lstDev.Count > 0)
            {
                foreach (var d in lstDev)
                {
                    if (d.IP_Address != "bcckl2.dyndns.org")
                        continue;
                    //string iPAddress = "192.168.0.115";
                    d.IP_Address = "210.186.135.100";
                    int iPort = 0; // set default port
                    if (d.Port != null)
                        iPort = d.Port.Value;

                    Console.WriteLine("Checking Users ...");
                    Console.WriteLine("Start : " + DateTime.Now);
                    List<User> lstUsers = getAllUsersFromDevice(d.IP_Address, iPort);
                    if (lstUsers.Count > 0)
                    {                        
                        ServiceResult sr = svc.UpdateZKUsersToDB(d.Device_ID, lstUsers);
                        Console.WriteLine(sr.Msg);
                    }
                    Console.WriteLine("End : " + DateTime.Now);

                    Console.WriteLine("Checking Transactions ...");
                    Console.WriteLine("Start : " + DateTime.Now);                    
                    List<Transaction> lstTrans = getTransactionsFromDevice(d.IP_Address, iPort);
                    if (lstTrans.Count > 0)
                    {
                        ServiceResult sr = svc.UpdateTimeTransactionToDB(d.Company_ID, d.Device_ID, lstTrans);
                        Console.WriteLine(sr.Msg);
                    }
                    Console.WriteLine("End : " + DateTime.Now);    
                }
            }    
        }
        static public List<Transaction> getTransactionsFromDevice(string iPAddress, int Port)
        {
            clsZKAccessManager cls = new clsZKAccessManager();
            List<Transaction> TranList = new List<Transaction>();            
            bool bFlag = cls.getConnection(iPAddress, Port);
            if (bFlag == true)
            {
                System.Diagnostics.Trace.WriteLine("Connected successfully.");
                TranList = cls.getTransactions(iPAddress, Port);
            }
            else
            {
                System.Diagnostics.Trace.WriteLine("Connection failed.");
            }
            cls.Disconnect();
            return TranList;
        }
        static public List<User> getAllUsersFromDevice(string iPAddress, int Port)
        {
            // If you got dll loading error when you initiate ZKAccessManager class, please make sure below facts.
            // - Make sure clsZKAccessManager's Platform Target should be "x64" in build properties
            // - Make sure SBSModel's Platform Target should be "x64" in build properties. 
            // - After build, you removed the existing ref of ZK and SBSModel. Then add new build ref again.
            // - Please do not forget for SBSModel to set it back to "Any CPU" after mapped.
            clsZKAccessManager cls = new clsZKAccessManager();
            List<User> userList = new List<User>();
            bool bFlag = cls.getConnection(iPAddress, Port);
            if (bFlag == true)
            {
                System.Diagnostics.Trace.WriteLine("Connected successfully.");
                userList = cls.getAllUsers(iPAddress, Port);
            }
            else
            {
                System.Diagnostics.Trace.WriteLine("Connection failed.");
            }
            cls.Disconnect();
            return userList;
        }
    }
}
