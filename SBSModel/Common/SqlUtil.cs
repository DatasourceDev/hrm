using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace SBSModel.Common
{
    public class SqlUtil
    {
        public static bool EnumerateSQLInstances()
        {

            string correctNamespace = GetCorrectWmiNameSpace();

            if (string.Equals(correctNamespace, string.Empty))
            {

                return false;

            }

            string query = string.Format("select * from SqlServiceAdvancedProperty where SQLServiceType = 1 and PropertyName = 'instanceID'");

            ManagementObjectSearcher getSqlEngine = new ManagementObjectSearcher(correctNamespace, query);

            if (getSqlEngine.Get().Count == 0)
            {

                return false;

            }

            Console.WriteLine("SQL Server database instances discovered :");

            string instanceName = string.Empty;

            string serviceName = string.Empty;

            string version = string.Empty;

            string edition = string.Empty;

            Console.WriteLine("Instance Name \t ServiceName \t Edition \t Version \t");

            foreach (ManagementObject sqlEngine in getSqlEngine.Get())
            {

                serviceName = sqlEngine["ServiceName"].ToString();

                instanceName = GetInstanceNameFromServiceName(serviceName);

                version = GetWmiPropertyValueForEngineService(serviceName, correctNamespace, "Version");

                edition = GetWmiPropertyValueForEngineService(serviceName, correctNamespace, "SKUNAME");

                Console.Write("{0} \t", instanceName);

                Console.Write("{0} \t", serviceName);

                Console.Write("{0} \t", edition);

                Console.WriteLine("{0} \t", version);

            }

            return true;

        }


        public static string GetCorrectWmiNameSpace()
        {

            String wmiNamespaceToUse = "root\\Microsoft\\sqlserver";

            List<string> namespaces = new List<string>();

            try
            {

                // Enumerate all WMI instances of

                // __namespace WMI class.

                ManagementClass nsClass =

                    new ManagementClass(

                    new ManagementScope(wmiNamespaceToUse),

                    new ManagementPath("__namespace"),

                    null);

                foreach (ManagementObject ns in

                    nsClass.GetInstances())
                {

                    namespaces.Add(ns["Name"].ToString());

                }

            }

            catch (ManagementException e)
            {

                Console.WriteLine("Exception = " + e.Message);

            }

            if (namespaces.Count > 0)
            {
                if (namespaces.Contains("ComputerManagement12"))
                {

                    //use katmai+ namespace

                    wmiNamespaceToUse = wmiNamespaceToUse + "\\ComputerManagement12";

                }

                else if (namespaces.Contains("ComputerManagement10"))
                {

                    //use katmai+ namespace

                    wmiNamespaceToUse = wmiNamespaceToUse + "\\ComputerManagement10";

                }

                else if (namespaces.Contains("ComputerManagement"))
                {

                    //use yukon namespace

                    wmiNamespaceToUse = wmiNamespaceToUse + "\\ComputerManagement";

                }

                else
                {

                    wmiNamespaceToUse = string.Empty;

                }

            }

            else
            {

                wmiNamespaceToUse = string.Empty;

            }

            return wmiNamespaceToUse;

        }

        public static string GetInstanceNameFromServiceName(string serviceName)
        {

            if (!string.IsNullOrEmpty(serviceName))
            {

                if (string.Equals(serviceName, "MSSQLSERVER", StringComparison.OrdinalIgnoreCase))
                {

                    return serviceName;

                }

                else
                {

                    return serviceName.Substring(serviceName.IndexOf('$') + 1, serviceName.Length - serviceName.IndexOf('$') - 1);

                }

            }

            else
            {

                return string.Empty;

            }

        }

        public static string GetWmiPropertyValueForEngineService(string serviceName, string wmiNamespace, string propertyName)
        {

            string propertyValue = string.Empty;

            string query = String.Format("select * from SqlServiceAdvancedProperty where SQLServiceType = 1 and PropertyName = '{0}' and ServiceName = '{1}'", propertyName, serviceName);

            ManagementObjectSearcher propertySearcher = new ManagementObjectSearcher(wmiNamespace, query);

            foreach (ManagementObject sqlEdition in propertySearcher.Get())
            {

                propertyValue = sqlEdition["PropertyStrValue"].ToString();

            }

            return propertyValue;

        }
    }
}
