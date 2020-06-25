using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Web.Administration;
using System.DirectoryServices;

namespace SBSModel.Common
{
    public class IISUtil
    {
        public enum FrameworkVersion
        {
            /// <span class="code-SummaryComment"><summary></span>
            /// .NET Framework 1.0
            /// <span class="code-SummaryComment"></summary></span>
            Fx10,

            /// <span class="code-SummaryComment"><summary></span>
            /// .NET Framework 1.1
            /// <span class="code-SummaryComment"></summary></span>
            Fx11,

            /// <span class="code-SummaryComment"><summary></span>
            /// .NET Framework 2.0
            /// <span class="code-SummaryComment"></summary></span>
            Fx20,

            /// <span class="code-SummaryComment"><summary></span>
            /// .NET Framework 3.0
            /// <span class="code-SummaryComment"></summary></span>
            Fx30,

            /// <span class="code-SummaryComment"><summary></span>
            /// .NET Framework 3.5 (Orcas)
            /// <span class="code-SummaryComment"></summary></span>
            Fx35,
        }

        /// <span class="code-SummaryComment"><summary></span>
        /// Specifies the Internet Information Services (IIS) versions
        /// <span class="code-SummaryComment"></summary></span>
        public enum InternetInformationServicesVersion
        {
            /// <span class="code-SummaryComment"><summary></span>
            /// Internet Information Services 4
            /// <span class="code-SummaryComment"></summary></span>
            /// <span class="code-SummaryComment"><remarks>Shipped in NT Option Pack for Windows NT 4</remarks></span>
            IIS4,

            /// <span class="code-SummaryComment"><summary></span>
            /// Internet Information Services 5
            /// <span class="code-SummaryComment"></summary></span>
            /// <span class="code-SummaryComment"><remarks>Shipped in Windows 2000 Server and Windows XP Professional</remarks></span>
            IIS5,
            IIS51,

            /// <span class="code-SummaryComment"><summary></span>
            /// Internet Information Services 6
            /// <span class="code-SummaryComment"></summary></span>
            /// <span class="code-SummaryComment"><remarks>Shipped in Windows Server 2003</remarks></span>
            IIS6,

            /// <span class="code-SummaryComment"><summary></span>
            /// Internet Information Services 7
            /// <span class="code-SummaryComment"></summary></span>
            /// <span class="code-SummaryComment"><remarks>Shipped in Windows Vista</remarks></span>
            IIS7,
            IIS8,
        }

        /// <span class="code-SummaryComment"><summary></span>
        /// Specifies the Internet Information Services (IIS) versions
        /// <span class="code-SummaryComment"></summary></span>
        public enum InternetInformationServicesComponent
        {
            /// <span class="code-SummaryComment"><summary></span>
            /// Internet Information Services Common Files
            /// <span class="code-SummaryComment"></summary></span>
            Common,

            /// <span class="code-SummaryComment"><summary></span>
            /// Active Server Pages (ASP) for Internet Information Services
            /// <span class="code-SummaryComment"></summary></span>
            ASP,

            /// <span class="code-SummaryComment"><summary></span>
            /// File Transfer Protocol (FTP) service
            /// <span class="code-SummaryComment"></summary></span>
            FTP,

            /// <span class="code-SummaryComment"><summary></span>
            /// Internet Information Services Manager
            /// <span class="code-SummaryComment"></summary></span>
            InetMgr,

            /// <span class="code-SummaryComment"><summary></span>
            /// Internet Data Connector
            /// <span class="code-SummaryComment"></summary></span>
            InternetDataConnector,

            /// <span class="code-SummaryComment"><summary></span>
            /// Network News Transfer Protocol (NNTP) service
            /// <span class="code-SummaryComment"></summary></span>
            NNTP,

            /// <span class="code-SummaryComment"><summary></span>
            /// Server-Side Includes
            /// <span class="code-SummaryComment"></summary></span>
            ServerSideIncludes,

            /// <span class="code-SummaryComment"><summary></span>
            /// Simple Mail Transfer Protocol (SMTP) service
            /// <span class="code-SummaryComment"></summary></span>
            SMTP,

            /// <span class="code-SummaryComment"><summary></span>
            /// Web Distributed Authoring and Versioning (WebDAV) publishing
            /// <span class="code-SummaryComment"></summary></span>
            WebDAV,

            /// <span class="code-SummaryComment"><summary></span>
            /// World Wide Web (WWW) service
            /// <span class="code-SummaryComment"></summary></span>
            WWW,

            /// <span class="code-SummaryComment"><summary></span>
            /// Remote administration (HTML)
            /// <span class="code-SummaryComment"></summary></span>
            RemoteAdmin,

            /// <span class="code-SummaryComment"><summary></span>
            /// Internet Server Application Programming Interface (ISAPI) for
            /// Background Intelligent Transfer Service (BITS) server extensions
            /// <span class="code-SummaryComment"></summary></span>
            BitsISAPI,

            /// <span class="code-SummaryComment"><summary></span>
            /// Background Intelligent Transfer Service (BITS) server extensions
            /// <span class="code-SummaryComment"></summary></span>
            Bits,

            /// <span class="code-SummaryComment"><summary></span>
            /// FrontPage server extensions
            /// <span class="code-SummaryComment"></summary></span>
            FrontPageExtensions,

            /// <span class="code-SummaryComment"><summary></span>
            /// Internet printing
            /// <span class="code-SummaryComment"></summary></span>
            InternetPrinting,

            /// <span class="code-SummaryComment"><summary></span>
            /// ActiveX control and sample pages for hosting Terminal Services
            /// client connections over the web
            /// <span class="code-SummaryComment"></summary></span>
            TSWebClient,
        }

        public static bool IISIsInstalled(InternetInformationServicesVersion version)
        {
            return InternetInformationServicesDetection.IsInstalled(version);
        }

        #region class InternetInformationServicesDetection
        /// <summary>
        /// Provides support for determining if a specific version of the .NET
        /// Framework runtime is installed and the service pack level for the
        /// runtime version.
        /// </summary>
        public static class InternetInformationServicesDetection
        {
            #region events

            #endregion

            #region class-wide fields

            const string IISRegKeyName = "Software\\Microsoft\\InetStp";
            const string IISRegKeyValue = "MajorVersion";
            const string IISRegKeyMinorVersionValue = "MinorVersion";
            const string IISComponentRegKeyName = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Setup\\Oc Manager\\Subcomponents";
            const string Netfx11RegKeyName = "Software\\Microsoft\\ASP.NET\\1.1.4322.0";
            const string Netfx20RegKeyName = "Software\\Microsoft\\ASP.NET\\2.0.50727.0";
            const string NetRegKeyValue = "DllFullPath";

            #endregion

            #region private and internal properties and methods

            #region properties

            #endregion

            #region methods

            #region GetRegistryValue
            private static bool GetRegistryValue<T>(RegistryHive hive, string key, string value, RegistryValueKind kind, out T data)
            {
                bool success = false;
                data = default(T);

                using (RegistryKey baseKey = RegistryKey.OpenRemoteBaseKey(hive, String.Empty))
                {
                    if (baseKey != null)
                    {
                        using (RegistryKey registryKey = baseKey.OpenSubKey(key, RegistryKeyPermissionCheck.ReadSubTree))
                        {
                            if (registryKey != null)
                            {
                                try
                                {
                                    // If the key was opened, try to retrieve the value.
                                    RegistryValueKind kindFound = registryKey.GetValueKind(value);
                                    if (kindFound == kind)
                                    {
                                        object regValue = registryKey.GetValue(value, null);
                                        if (regValue != null)
                                        {
                                            data = (T)Convert.ChangeType(regValue, typeof(T), CultureInfo.InvariantCulture);
                                            success = true;
                                        }
                                    }
                                }
                                catch (IOException)
                                {
                                    // The registry value doesn't exist. Since the
                                    // value doesn't exist we have to assume that
                                    // the component isn't installed and return
                                    // false and leave the data param as the
                                    // default value.
                                }
                            }
                        }
                    }
                }
                return success;
            }
            #endregion

            #region IsIISInstalled functions

            #region IsIIS4Installed
            private static bool IsIIS4Installed()
            {
                bool found = false;
                int regValue = 0;

                if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyValue, RegistryValueKind.DWord, out regValue))
                {
                    if (regValue >= 4)
                    {
                        found = true;
                    }
                }

                return found;
            }
            #endregion

            #region IsIIS5Installed
            private static bool IsIIS5Installed()
            {
                bool found = false;
                int regValue = 0;

                if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyValue, RegistryValueKind.DWord, out regValue))
                {
                    if (regValue >= 5)
                    {
                        // We know that we have either 5.0 or 5.1, so check the MinorVersion value.
                        int minorVersion = -1;
                        if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyMinorVersionValue, RegistryValueKind.DWord, out minorVersion))
                        {
                            if (minorVersion == 0)
                            {
                                found = true;
                            }
                        }
                    }
                }

                return found;
            }
            #endregion

            #region IsIIS51Installed
            private static bool IsIIS51Installed()
            {
                bool found = false;
                int regValue = 0;

                if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyValue, RegistryValueKind.DWord, out regValue))
                {
                    if (regValue >= 5)
                    {
                        // We know that we have either 5.0 or 5.1, so check the MinorVersion value.
                        int minorVersion = -1;
                        if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyMinorVersionValue, RegistryValueKind.DWord, out minorVersion))
                        {
                            if (minorVersion == 1)
                            {
                                found = true;
                            }
                        }
                    }
                }

                return found;
            }
            #endregion

            #region IsIIS6Installed
            private static bool IsIIS6Installed()
            {
                bool found = false;
                int regValue = 0;

                if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyValue, RegistryValueKind.DWord, out regValue))
                {
                    if (regValue >= 6)
                    {
                        found = true;
                    }
                }

                return found;
            }
            #endregion

            #region IsIIS7Installed
            private static bool IsIIS7Installed()
            {
                bool found = false;
                int regValue = 0;

                if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyValue, RegistryValueKind.DWord, out regValue))
                {
                    if (regValue >= 7)
                    {
                        found = true;
                    }
                }

                return found;
            }
            #endregion

            #region IsIIS8Installed
            private static bool IsIIS8Installed()
            {
                bool found = false;
                int regValue = 0;

                if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyValue, RegistryValueKind.DWord, out regValue))
                {
                    if (regValue >= 8)
                    {
                        found = true;
                    }
                }

                return found;
            }
            #endregion

            #endregion

            #region IsIISComponentInstalled
            private static bool IsIISComponentInstalled(string subcomponent)
            {
                bool found = false;
                int regValue = 0;

                if (GetRegistryValue(RegistryHive.LocalMachine, IISComponentRegKeyName, subcomponent, RegistryValueKind.DWord, out regValue))
                {
                    if (regValue == 1)
                    {
                        found = true;
                    }
                }

                return found;
            }
            #endregion

            #region IsAspNetRegistered functions

            #region IsAspNet10Registered
            // TODO: Determine how to detect ASP.NET 1.0
            private static bool IsAspNet10Registered()
            {
                return false;
            }
            #endregion

            #region IsAspNet11Registered
            private static bool IsAspNet11Registered()
            {
                string regValue = string.Empty;
                return (GetRegistryValue(RegistryHive.LocalMachine, Netfx11RegKeyName, NetRegKeyValue, RegistryValueKind.String, out regValue));
            }
            #endregion

            #region IsAspNet20Registered
            private static bool IsAspNet20Registered()
            {
                string regValue = string.Empty;
                return (GetRegistryValue(RegistryHive.LocalMachine, Netfx20RegKeyName, NetRegKeyValue, RegistryValueKind.String, out regValue));
            }
            #endregion

            #endregion

            #endregion

            #endregion

            #region public properties and methods

            #region properties

            #endregion

            #region methods

            #region IsInstalled

            #region IsInstalled(InternetInformationServicesVersion iisVersion)
            /// <summary>
            /// Determines if the specified version of Internet Information 
            /// Services (IIS) is installed on the local computer.
            /// </summary>
            /// <param name="iisVersion">One of the 
            /// <see cref="InternetInformationServicesVersion"/> values.</param>
            /// <returns><see langword="true"/> if the specified Internet
            /// Information Services version is installed; otherwise
            /// <see langword="false"/>.</returns>
            public static bool IsInstalled(InternetInformationServicesVersion iisVersion)
            {
                bool ret = false;

                switch (iisVersion)
                {
                    case InternetInformationServicesVersion.IIS4:
                        ret = IsIIS4Installed();
                        break;

                    case InternetInformationServicesVersion.IIS5:
                        ret = IsIIS5Installed();
                        break;

                    case InternetInformationServicesVersion.IIS51:
                        ret = IsIIS51Installed();
                        break;

                    case InternetInformationServicesVersion.IIS6:
                        ret = IsIIS6Installed();
                        break;

                    case InternetInformationServicesVersion.IIS7:
                        ret = IsIIS7Installed();
                        break;
                    case InternetInformationServicesVersion.IIS8:
                        ret = IsIIS8Installed();
                        break;

                    default:
                        break;
                }

                return ret;
            }
            #endregion

            #region IsInstalled(InternetInformationServicesComponent subcomponent)
            /// <summary>
            /// Determines if the specified Internet Information Services (IIS)
            /// subcomponent is installed on the local computer.
            /// </summary>
            /// <param name="subcomponent">One of the 
            /// <see cref="InternetInformationServicesComponent"/> values.</param>
            /// <returns><see langword="true"/> if the specified Internet
            /// Information Services subcomponent is installed; otherwise
            /// <see langword="false"/>.</returns>
            public static bool IsInstalled(InternetInformationServicesComponent subcomponent)
            {
                bool ret = false;

                switch (subcomponent)
                {
                    case InternetInformationServicesComponent.Common:
                        ret = IsIISComponentInstalled("iis_common");
                        break;

                    case InternetInformationServicesComponent.ASP:
                        ret = IsIISComponentInstalled("iis_asp");
                        break;

                    case InternetInformationServicesComponent.FTP:
                        ret = IsIISComponentInstalled("iis_ftp");
                        break;

                    case InternetInformationServicesComponent.InetMgr:
                        ret = IsIISComponentInstalled("iis_inetmgr");
                        break;

                    case InternetInformationServicesComponent.InternetDataConnector:
                        ret = IsIISComponentInstalled("iis_internetdataconnector");
                        break;

                    case InternetInformationServicesComponent.NNTP:
                        ret = IsIISComponentInstalled("iis_nntp");
                        break;

                    case InternetInformationServicesComponent.ServerSideIncludes:
                        ret = IsIISComponentInstalled("iis_serversideincludes");
                        break;

                    case InternetInformationServicesComponent.SMTP:
                        ret = IsIISComponentInstalled("iis_smtp");
                        break;

                    case InternetInformationServicesComponent.WebDAV:
                        ret = IsIISComponentInstalled("iis_webdav");
                        break;

                    case InternetInformationServicesComponent.WWW:
                        ret = IsIISComponentInstalled("iis_www");
                        break;

                    case InternetInformationServicesComponent.RemoteAdmin:
                        ret = IsIISComponentInstalled("sakit_web");
                        break;

                    case InternetInformationServicesComponent.BitsISAPI:
                        ret = IsIISComponentInstalled("BitsServerExtensionsISAPI");
                        break;

                    case InternetInformationServicesComponent.Bits:
                        ret = IsIISComponentInstalled("BitsServerExtensionsManager");
                        break;

                    case InternetInformationServicesComponent.FrontPageExtensions:
                        ret = IsIISComponentInstalled("fp_extensions");
                        break;

                    case InternetInformationServicesComponent.InternetPrinting:
                        ret = IsIISComponentInstalled("inetprint ");
                        break;

                    case InternetInformationServicesComponent.TSWebClient:
                        ret = IsIISComponentInstalled("TSWebClient ");
                        break;

                    default:
                        break;
                }


                return ret;
            }
            #endregion

            #endregion

            #region IsAspRegistered
            /// <summary>
            /// Determines if ASP is registered with Internet Information
            /// Services (IIS) on the local computer.
            /// </summary>
            /// <returns><see langword="true"/> if ASP is registered; otherwise
            /// <see langword="false"/>.</returns>
            public static bool IsAspRegistered()
            {
                return IsInstalled(InternetInformationServicesComponent.ASP);
            }
            #endregion

            #region IsAspNetRegistered
            /// <summary>
            /// Determines if the version of ASP.NET is registered with Internet
            /// Information Services (IIS) on the local computer.
            /// </summary>
            /// <param name="frameworkVersion">One of the
            /// <see cref="FrameworkVersion"/> values.</param>
            /// <returns><see langword="true"/> if the specified ASP.NET version
            /// is registered; otherwise <see langword="false"/>.</returns>
            public static bool IsAspNetRegistered(FrameworkVersion frameworkVersion)
            {
                bool ret = false;

                switch (frameworkVersion)
                {
                    case FrameworkVersion.Fx10:
                        ret = IsAspNet10Registered();
                        break;

                    case FrameworkVersion.Fx11:
                        ret = IsAspNet11Registered();
                        break;

                    case FrameworkVersion.Fx20:
                    case FrameworkVersion.Fx30:
                    case FrameworkVersion.Fx35:
                        ret = IsAspNet20Registered();
                        break;

                    default:
                        break;
                }
                return ret;
            }
            #endregion

            #endregion

            #endregion
        }
        #endregion



        #region Application1
        public static void CreateAppPool(string metabasePath, string appPoolName)
        {
            //  metabasePath is of the form "IIS://<servername>/W3SVC/AppPools"
            //    for example "IIS://localhost/W3SVC/AppPools" 
            //  appPoolName is of the form "<name>", for example, "MyAppPool"
            Console.WriteLine("\nCreating application pool named {0}/{1}:", metabasePath, appPoolName);

            try
            {
                if (metabasePath.EndsWith("/W3SVC/AppPools"))
                {
                    DirectoryEntry newpool;
                    DirectoryEntry apppools = new DirectoryEntry(metabasePath);
                    newpool = apppools.Children.Add(appPoolName, "IIsApplicationPool");
                    newpool.CommitChanges();
                    Console.WriteLine(" Done.");
                }
                else
                    Console.WriteLine(" Failed in CreateAppPool; application pools can only be created in the */W3SVC/AppPools node.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed in CreateAppPool with the following exception: \n{0}", ex.Message);
            }
        }

        public static void CreateVDir(string metabasePath, string vDirName, string physicalPath)
        {
            //  metabasePath is of the form "IIS://<servername>/<service>/<siteID>/Root[/<vdir>]"
            //    for example "IIS://localhost/W3SVC/1/Root" 
            //  vDirName is of the form "<name>", for example, "MyNewVDir"
            //  physicalPath is of the form "<drive>:\<path>", for example, "C:\Inetpub\Wwwroot"
            Console.WriteLine("\nCreating virtual directory {0}/{1}, mapping the Root application to {2}:",
                metabasePath, vDirName, physicalPath);

            try
            {
                DirectoryEntry site = new DirectoryEntry(metabasePath);
                string className = site.SchemaClassName.ToString();
                if ((className.EndsWith("Server")) || (className.EndsWith("VirtualDir")))
                {
                    DirectoryEntries vdirs = site.Children;
                    DirectoryEntry newVDir = vdirs.Add(vDirName, (className.Replace("Service", "VirtualDir")));
                    newVDir.Properties["Path"][0] = physicalPath;
                    newVDir.Properties["AccessScript"][0] = true;
                    // These properties are necessary for an application to be created.
                    newVDir.Properties["AppFriendlyName"][0] = vDirName;
                    newVDir.Properties["AppIsolated"][0] = "1";
                    newVDir.Properties["AppRoot"][0] = "/LM" + metabasePath.Substring(metabasePath.IndexOf("/", ("IIS://".Length)));

                    newVDir.CommitChanges();

                    Console.WriteLine(" Done.");
                }
                else
                    Console.WriteLine(" Failed. A virtual directory can only be created in a site or virtual directory node.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed in CreateVDir with the following exception: \n{0}", ex.Message);
            }
        }





        public static void AssignVDirToAppPool(string metabasePath, string appPoolName)
        {
            //  metabasePath is of the form "IIS://<servername>/W3SVC/<siteID>/Root[/<vDir>]"
            //    for example "IIS://localhost/W3SVC/1/Root/MyVDir" 
            //  appPoolName is of the form "<name>", for example, "MyAppPool"
            Console.WriteLine("\nAssigning application {0} to the application pool named {1}:", metabasePath, appPoolName);

            try
            {
                DirectoryEntry vDir = new DirectoryEntry(metabasePath);
                string className = vDir.SchemaClassName.ToString();
                if (className.EndsWith("VirtualDir"))
                {
                    object[] param = { 0, appPoolName, true };
                    vDir.Invoke("AppCreate3", param);
                    vDir.Properties["AppIsolated"][0] = "2";
                    Console.WriteLine(" Done.");
                }
                else
                    Console.WriteLine(" Failed in AssignVDirToAppPool; only virtual directories can be assigned to application pools");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed in AssignVDirToAppPool with the following exception: \n{0}", ex.Message);
            }
        }

        #endregion


    }
}
