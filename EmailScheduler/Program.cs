using SBSModel.Common;
using SBSModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailScheduler
{
   class Program
   {
      static void Main(string[] args)
      {
         Scheduler();
      }

      public static void Scheduler()
      {
         var logName = "Send_Email_Log";
         LogUtil.WriteLog("Start Job Scheduler", logName);
         var currentdate = StoredProcedure.GetCurrentDate(); ;
         var SchService = new ScheddulerService();
         var emails = SchService.LstEmailNotification();
         if (emails != null && emails.Count > 0)
         {
            LogUtil.WriteLog("Total Job: " + emails.Count, logName);
            foreach (var erow in emails)
            {
               ServiceResult result = new ServiceResult();
               result = EmailTemplete.sendEmailNotification(erow.Receiver, erow.Subject, erow.Message, erow.CC, erow.Email_Attachment.ToList());
               erow.Update_By = "System";
               erow.Update_On = currentdate;
               if (result.Code == ERROR_CODE.SUCCESS)
               {
                  erow.Status = ScheduledAction.Sent;
                  erow.Exception_Message = null;
               }
               else
               {
                  erow.Status = ScheduledAction.Fail;
                  erow.Exception_Message = result.Msg;
               }
               LogUtil.WriteLog("Status: " + erow.Status + " >>> Module: " + erow.Module + " >>> Send Email: " + erow.Receiver, logName);
            }
            SchService.UpdateEmailNotification(emails);
         }
         else
            LogUtil.WriteLog("Total Job: 0", logName);

         LogUtil.WriteLog("End Job Scheduler", logName);
         LogUtil.WriteLog("********************************", logName);
      }
   }
}
