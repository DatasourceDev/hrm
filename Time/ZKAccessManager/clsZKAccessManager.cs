using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zkemkeeper;
namespace ZKAccessManager
{
    public class clsZKAccessManager
    {
        //192.168.0.115 
        //string gIPAddr = "192.168.0.115";

        public zkemkeeper.CZKEMClass axCZKEM1 = new zkemkeeper.CZKEMClass();
        bool bIsConnected = false;

        public bool Disconnect()
        {
            axCZKEM1.Disconnect();
            bIsConnected = false;
            return true;
        }
        public bool getConnection(string IPAddress, int Port)
        {            
            bIsConnected = axCZKEM1.Connect_Net(IPAddress, Port);
            if (bIsConnected == true)
            {
                Console.WriteLine(IPAddress + " : Device is connected");
                return true;
            }
            else
            {
                Console.WriteLine(IPAddress + " :Device is not connected");
                return false;
            }
        }
        public List<Transaction> getTransactions(string IPAddress, int Port)
        {
            int iMachineNumber = Port;
            string sdwEnrollNumber = "";
            //int idwTMachineNumber = 0;
            //int idwEMachineNumber = 0;
            int idwVerifyMode = 0;
            int idwInOutMode = 0;
            int idwYear = 0;
            int idwMonth = 0;
            int idwDay = 0;
            int idwHour = 0;
            int idwMinute = 0;
            int idwSecond = 0;
            int idwWorkcode = 0;

            int idwErrorCode = 0;
            int iGLCount = 0;
            int iIndex = 0;

            List<Transaction> TranList = new List<Transaction>();
             axCZKEM1.EnableDevice(1, false);//disable the device            
            if (axCZKEM1.ReadGeneralLogData(iMachineNumber))//read all the attendance records to the memory
            {
                while (axCZKEM1.SSR_GetGeneralLogData(iMachineNumber, out sdwEnrollNumber, out idwVerifyMode,
                           out idwInOutMode, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute, out idwSecond, ref idwWorkcode))//get records from the memory
                {
                    iGLCount++;
                    Transaction t = new Transaction();
                    t.ID = iGLCount;
                    t.DateAndTime = Convert.ToDateTime(idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString() + " " + idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString());
                    t.JobCode = Convert.ToInt16(idwInOutMode);
                    t.Type = Convert.ToInt16(idwVerifyMode);
                    //t.objUser = getUserByEnrollNumber(ref axCZKEM1, Convert.ToInt32(sdwEnrollNumber));
                    t.Pin = Convert.ToInt16(sdwEnrollNumber);
                    //Console.WriteLine(sdwEnrollNumber);
                    //Console.WriteLine(idwVerifyMode.ToString());
                    //Console.WriteLine(idwInOutMode.ToString());
                    //Console.WriteLine(idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString() + " " + idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString());
                    //Console.WriteLine(idwWorkcode.ToString());
                    TranList.Add(t);
                }                
            }
            else
            {                
                axCZKEM1.GetLastError(ref idwErrorCode);

                if (idwErrorCode != 0)
                {
                    Console.WriteLine("Reading data from terminal failed,ErrorCode: " + idwErrorCode.ToString(), "Error");
                }
                else
                {
                    Console.WriteLine("No data from terminal returns!", "Error");
                }
            }
            axCZKEM1.EnableDevice(iMachineNumber, true);//enable the device
            return TranList;
        }
        public List<User> getAllUsers(string IPAddress, int Port)
        {
            int iMachineNumber = Port;
            string sdwEnrollNumber = "";
            string sName = "";
            string sPassword = "";
            int iPrivilege = 0;
            bool bEnabled = false;

            int idwFingerIndex;
            string sTmpData = "";
            int iTmpLength = 0;
            int iFlag = 0;

            List<User> lstUser = new List<User>();
            axCZKEM1.EnableDevice(1, false);//disable the device
            axCZKEM1.ReadAllUserID(iMachineNumber);//read all the user information to the memory
            while (axCZKEM1.SSR_GetAllUserInfo(iMachineNumber, out sdwEnrollNumber, out sName, out sPassword, out iPrivilege, out bEnabled))//get all the users' information from the memory
            {
                for (idwFingerIndex = 0; idwFingerIndex < 10; idwFingerIndex++)
                {
                    if (axCZKEM1.GetUserTmpExStr(iMachineNumber, sdwEnrollNumber, idwFingerIndex, out iFlag, out sTmpData, out iTmpLength))//get the corresponding templates string and length from the memory
                    {                        
                        User u = new User();
                        if(sName == "")
                            u.Name = "User-" + sdwEnrollNumber.ToString();
                        else
                            u.Name = sName;
                        u.Level = Convert.ToByte(iPrivilege.ToString());
                        u.Enabled = bEnabled;
                        u.Pin = Convert.ToInt32(sdwEnrollNumber);
                        lstUser.Add(u);
                    }
                }
            }
            axCZKEM1.EnableDevice(iMachineNumber, true);
            return lstUser;
        }
        public User getUserByEnrollNumber(ref zkemkeeper.CZKEMClass axCZKEM1, int EnrollNo)
        {
            int iMachineNumber = 4370;
            string sdwEnrollNumber = "";
            string sName = "";
            string sPassword = "";
            int iPrivilege = 0;
            bool bEnabled = false;

            int idwFingerIndex;
            string sTmpData = "";
            int iTmpLength = 0;
            int iFlag = 0;
            User u = new User();
            axCZKEM1.EnableDevice(1, false);//disable the device
            axCZKEM1.ReadAllUserID(iMachineNumber);//read all the user information to the memory                
            while (axCZKEM1.SSR_GetAllUserInfo(iMachineNumber, out sdwEnrollNumber, out sName, out sPassword, out iPrivilege, out bEnabled))//get all the users' information from the memory
            {
                for (idwFingerIndex = 0; idwFingerIndex < 10; idwFingerIndex++)
                {
                    if (axCZKEM1.GetUserTmpExStr(iMachineNumber, sdwEnrollNumber, idwFingerIndex, out iFlag, out sTmpData, out iTmpLength))//get the corresponding templates string and length from the memory
                    {
                        if (EnrollNo.ToString() == sdwEnrollNumber)
                        {

                            if (sName == "")
                                u.Name = "User-" + sdwEnrollNumber.ToString();
                            else
                                u.Name = sName;
                            u.Level = Convert.ToByte(iPrivilege.ToString());
                            u.Enabled = bEnabled;
                            u.Pin = Convert.ToInt32(sdwEnrollNumber);
                            break;
                        }

                    }
                }
            }
            axCZKEM1.EnableDevice(iMachineNumber, true);
            return u;
        }
    }
}
