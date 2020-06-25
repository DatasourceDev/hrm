using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBSModel.Common
{
   public  class LogUtil
   {
      public static void WriteLog(string Message, string name)
      {
         StreamWriter sw = null;
         try
         {
            //if (!Directory.Exists(@"c:\sbs\Job_Scheduled"))
            //   Directory.CreateDirectory(@"c:\sbs\Job_Scheduled");

            //var path = @"c:\sbs\Job_Scheduled\" + "\\" + name + ".txt";

            //sw = new StreamWriter(path, true);
            //sw.WriteLine(DateTime.Now.ToString("MM_dd_yyyy_HH_mm") + ": " + Message);
            //sw.Flush();
            //sw.Close();

            Console.WriteLine();   
            Console.Write(DateTime.Now.ToString("MM_dd_yyyy_HH_mm") + ": " + Message);     
         }
         catch
         {
         }
      }
   }
}
