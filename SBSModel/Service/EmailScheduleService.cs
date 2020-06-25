using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using SBSModel.Common;
using SBSWorkFlowAPI.Models;
using System.Data.Entity.Core.Objects;
using SBSResourceAPI;
using System.Diagnostics;
using SBSModel.Models;
namespace SBSModel.Models
{
   public class ScheddulerService
   {
      public List<Email_Notification> LstEmailNotification()
      {
         using (var db = new SBS2DBContext())
         {
            return db.Email_Notification
              .Include(i => i.Email_Attachment)
              .Where(w => w.Status != ScheduledAction.Sent).ToList();

         }
      }

      public Email_Notification GetEmailNotification(Nullable<int> pEmailScheduledID)
      {
         using (var db = new SBS2DBContext())
         {
            return db.Email_Notification
               //.Include(w => w.Company)
               .Where(w => w.Email_ID == pEmailScheduledID).FirstOrDefault();
         }
      }

      //public bool UpdateEmailNotification(Email_Notification pEmailNotification)
      //{
      //   try
      //   {
      //      using (var db = new SBS2DBContext())
      //      {
      //         var current = db.Email_Notification.Where(w => w.Email_ID == pEmailNotification.Email_ID).FirstOrDefault();
      //         if (current == null)
      //            return false;

      //         db.Entry(current).CurrentValues.SetValues(pEmailNotification);
      //         db.SaveChanges();
      //         return true;
      //      }
      //   }
      //   catch
      //   {
      //      return false;
      //   }
      //}

      public bool InsertEmailNotification(Email_Notification pEmailScheduled)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               if (pEmailScheduled.Email_Attachment != null && pEmailScheduled.Email_Attachment.Count > 0)
               {
                  foreach (var row in pEmailScheduled.Email_Attachment)
                  {
                     var guid = Guid.NewGuid();
                     while (db.Email_Attachment.Where(w => w.Email_Attachment_ID == guid).FirstOrDefault() != null)
                        guid = Guid.NewGuid();

                     row.Email_Attachment_ID = guid;
                  }
               }

               db.Email_Notification.Add(pEmailScheduled);
               db.SaveChanges();
               return true;
            }
         }
         catch(Exception ex)
         {
            return false;
         }
      }

      public bool UpdateEmailNotification(List<Email_Notification> pEmailNotifications)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               if (pEmailNotifications != null && pEmailNotifications.Count > 0)
               {
                  foreach (var row in pEmailNotifications)
                  {
                     var current = db.Email_Notification.Where(w => w.Email_ID == row.Email_ID).FirstOrDefault();
                     if (current != null)
                     {
                        db.Entry(current).CurrentValues.SetValues(row);
                     }
                  }
                  db.SaveChanges();
               }
               return true;
            }
         }
         catch
         {
            return false;
         }
      }
   }
}
