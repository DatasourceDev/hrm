using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using SBSModel.Models;
using SBSModel.Common;
using System.Data.Entity.Validation;
using SBSResourceAPI;

namespace SBSModel.Models
{
   public class FrontService
    {
       public ServiceResult InsertNewletters(Subscriber subscriber )
       {
           try
           {
               using (var db = new SBS2DBContext())
               {
                   db.Subscribers.Add(subscriber);
                   db.SaveChanges();
                   return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE)};
               }
           }
           catch
           {
               return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR)};
           }
       }

       public Subscriber GetSubscriber(string pEmail)
       {
          using (var db = new SBS2DBContext())
          {
             return db.Subscribers.Where(w => w.Subscriber_Email.Equals(pEmail)).FirstOrDefault();
          }
       }

    }
}
