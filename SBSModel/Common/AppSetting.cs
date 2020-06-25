using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Diagnostics;

namespace SBSModel.Common
{
    public class AppSetting
    {
     
        public static bool POS_OFFLINE_CLIENT
        {
            get
            {
                try
                {
                    //IniFile objIniFile = new IniFile("AppSetting.ini");
                    //return Convert.ToBoolean(objIniFile.GetString("POS", "POS_OFFLINE_CLIENT", "false"));
                    try { return Convert.ToBoolean( ConfigurationManager.AppSettings["POS_OFFLINE_CLIENT"].ToString()); }
                    catch { return false; }
                }
                catch { return false; }
            }
        }

       
        public static string SERVER_NAME
        {
            get
            {
                try { return ConfigurationManager.AppSettings["SERVER_NAME"].ToString(); }
                catch { return ""; }
            }
        }

        public static string SBSTmpAPI
        {
            get
            {
                try { return ConfigurationManager.AppSettings["SBSTmpAPI"].ToString(); }
                catch { return ""; }
            }
        }

        public static string IsStaging
        {
            get
            {
                try { return ConfigurationManager.AppSettings["IsStaging"].ToString(); }
                catch { return ""; }
            }
        }
        public static string IsDemo
        {
            get
            {
                try { return ConfigurationManager.AppSettings["IsDemo"].ToString(); }
                catch { return ""; }
            }
        }
        public static string IsLocal
        {
            get
            {
                try { return ConfigurationManager.AppSettings["IsLocal"].ToString(); }
                catch { return ""; }
            }
        }

        public static string IsLive
        {
            get
            {
                try { return ConfigurationManager.AppSettings["IsLive"].ToString(); }
                catch { return ""; }
            }
        }

        public static string SMTP_SERVER
        {
            get
            {
                try { return ConfigurationManager.AppSettings["SMTP_SERVER"].ToString(); }
                catch { return ""; }
            }
        }

        public static string SMTP_PORT
        {
            get
            {
                try { return ConfigurationManager.AppSettings["SMTP_PORT"].ToString(); }
                catch { return ""; }
            }
        }

        public static string SMTP_USERNAME
        {
            get
            {
                try { return ConfigurationManager.AppSettings["SMTP_USERNAME"].ToString(); }
                catch { return ""; }
            }
        }

        public static string SMTP_PASSWORD
        {
            get
            {
                try { return ConfigurationManager.AppSettings["SMTP_PASSWORD"].ToString(); }
                catch { return ""; }
            }
        }

        public static bool SMTP_SSL
        {
            get
            {
                try { return Convert.ToBoolean(ConfigurationManager.AppSettings["SMTP_SSL"]); }
                catch { return false; }
            }
        }

        public static string TIME_DEVICE_IP
        {
            get
            {
                try { return ConfigurationManager.AppSettings["TIME_DEVICE_IP"].ToString(); }
                catch { return ""; }
            }
        }

        public static string WSVR_URL
        {
            get
            {
                try { return ConfigurationManager.AppSettings["WSVR_URL"].ToString(); }
                catch { return ""; }
            }
        }

        public static string Version_Control
        {
           get
           {
              try { return ConfigurationManager.AppSettings["Version_Control"].ToString(); }
              catch { return ""; }
           }
        }
    }
}
