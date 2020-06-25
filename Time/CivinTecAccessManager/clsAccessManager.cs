using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Timers;
using System.Data;
using System.ComponentModel;


using CN8TcpComm;
using CN8TcpComm.Data;

namespace CivinTecAccessManager
{
    public class clsAccessManager
    {
        //192.168.0.122
        CN8Client client = new CN8Client();
        public bool getConnection(string IPAddress)
        {
            client.ResponseFile = "rfWAD8MEG0aQpEjURjMoyg==";
            client.Connect(IPAddress);
            if (!client.IsConnected)
            {
                //client.Disconnect();
                return false;
            }
            else
            {
                //client.Disconnect();
                return true;
            }

        }
        public List<User> getUsers()
        {
            List<User> Users = new List<User>();
            if (client.IsConnected)
            {
                CN8DataUsers pins = client.GetAllPins();

                foreach (int p in pins.Pins.ToList())
                {
                    CN8DataUser dUser = client.GetUser((uint)(p));
                    if (dUser.Code == CN8.ReturnCode.OK && dUser.User != null)
                    {
                        User u = new User();
                        u.Enabled = dUser.User.Enabled;
                        u.FAR = dUser.User.FAR;
                        u.FirstName = dUser.User.FirstName;
                        u.LastName = dUser.User.LastName;
                        u.MiddleName = dUser.User.MiddleName;
                        u.Pin = dUser.User.Pin;
                        u.Level = dUser.User.Level;
                        u.Type = dUser.User.Type;
                        Users.Add(u);
                    }
                }
            }
            //client.Disconnect();
            return Users;
        }
        private Transaction generateTransactionObj(CN8Transaction p)
        {
            Transaction t = new Transaction();
            string JC = "F" + (p.JobCode - 240).ToString();
            t.TypeName = CN8.GetTransactionTypeName((CN8.TransactionType)p.Type);
            t.JobCode = p.JobCode;
            CN8DataString res = client.GetJobCode(ushort.Parse(JC, System.Globalization.NumberStyles.HexNumber));
            t.JobCodeDesc = res.Text;
            t.CardID = p.CardID;
            t.DateAndTime = p.DateAndTime;
            t.ID = p.ID;
            t.Pin = p.Pin;
            t.Type = p.Type;

            CN8DataUser dUser = client.GetUser((uint)(p.Pin));
            if (dUser.Code == CN8.ReturnCode.OK && dUser.User != null)
            {
                User u = new User();
                u.FirstName = dUser.User.FirstName;
                u.LastName = dUser.User.LastName;
                u.MiddleName = dUser.User.MiddleName;
                u.Level = dUser.User.Level;
                u.Type = dUser.User.Type;
                u.FAR = dUser.User.FAR;
                u.Pin = dUser.User.Pin;                
                t.objUser = u;
            }
            return t;
        }
        public List<Transaction> getTransactions()
        {
            List<Transaction> transactions = new List<Transaction>();
            if (client.IsConnected)
            {
                CN8DataTransactions dTrans = client.GetAllTransactions();
                dTrans.Transactions.ToList().ForEach(delegate(CN8Transaction p)
                {
                    Transaction t = generateTransactionObj(p);            
                    transactions.Add(t);
                });
            }
            return transactions;
        }
        public List<Transaction> getTransactions(int TransID)
        {
            List<Transaction> transactions = new List<Transaction>();
            Decimal LastTransID = 0;
            Decimal ActualLastTransID = 0;

            if (client.IsConnected)
            {
                CN8DataInt32 res = client.GetLastTransactionID();
                if (res.Code == CN8.ReturnCode.OK)
                {
                    ActualLastTransID = (Decimal)(uint)res.Value;
                    if (TransID < ActualLastTransID)
                    {
                        LastTransID = TransID;
                    }
                    else { return transactions; }
                }

                if (TransID == 0)
                {
                    CN8DataTransactions dTrans = client.GetAllTransactions();
                    dTrans.Transactions.ToList().ForEach(delegate(CN8Transaction p)
                    {
                        LastTransID = p.ID;
                        if (p.JobCode > 240)
                        {
                            Transaction t = generateTransactionObj(p);                            
                            transactions.Add(t);
                        }
                    });
                }
                
                while (LastTransID < ActualLastTransID)
                {
                    CN8DataTransactions dTrans = client.GetTransactions((uint)LastTransID+1);
                    dTrans.Transactions.ToList().ForEach(delegate(CN8Transaction p)
                    {
                        LastTransID = p.ID;
                        if (p.JobCode > 240)
                        {
                            Transaction t = generateTransactionObj(p);                            
                            transactions.Add(t);
                        }
                    });
                }
            }
            return transactions;
        }
    }
}
