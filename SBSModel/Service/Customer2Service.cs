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
   public class CustomerCriteria : CriteriaBase
   {
      public Nullable<int> Customer_ID { get; set; }
      public string Customer_No { get; set; }
      public string Customer_Name { get; set; }
      public string Customer_No_Dup { get; set; }

      public Nullable<int> Billing_Country_ID { get; set; }
      public Nullable<int> Billing_State_ID { get; set; }

   }

   public class Customer2Service
   {
      public Customer GetCustomer(Nullable<int> pCustomerID)
      {
         using (var db = new SBS2DBContext())
         {
            return db.Customers.Where(w => w.Customer_ID == pCustomerID && w.Record_Status != RecordStatus.Delete).FirstOrDefault();
         }
      }

      public List<Customer> LstCustomer(CustomerCriteria criteria)
      {

         using (var db = new SBS2DBContext())
         {
            var cust = db.Customers
              .Where(w => w.Company_ID == criteria.Company_ID && w.Record_Status != RecordStatus.Delete);
            if (criteria != null)
            {
               if (criteria.Customer_ID.HasValue && criteria.Customer_ID.Value > 0)
                  cust = cust.Where(w => w.Customer_ID == criteria.Customer_ID);
               if (!string.IsNullOrEmpty(criteria.Customer_No))
                  cust = cust.Where(w => w.Customer_No.Contains(criteria.Customer_No));
               if (!string.IsNullOrEmpty(criteria.Customer_Name))
                  cust = cust.Where(w => w.Customer_Name.Contains(criteria.Customer_Name));
               if (!string.IsNullOrEmpty(criteria.Customer_No_Dup))
                  cust = cust.Where(w => w.Customer_No == criteria.Customer_No_Dup);
               if (!string.IsNullOrEmpty(criteria.Record_Status))
                  cust = cust.Where(w => w.Record_Status == criteria.Record_Status);
               if (criteria.Billing_Country_ID.HasValue && criteria.Billing_Country_ID.Value > 0)
                  cust = cust.Where(w => w.Billing_Country_ID == criteria.Billing_Country_ID);
               if (criteria.Billing_State_ID.HasValue && criteria.Billing_State_ID.Value > 0)
                  cust = cust.Where(w => w.Billing_State_ID == criteria.Billing_State_ID);
            }
            return cust.OrderBy(o => o.Customer_Name).ToList();
         }

      }

      public ServiceResult UpdateCustomer(Customer pCustomer)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               var current = db.Customers.Where(w => w.Customer_ID == pCustomer.Customer_ID).FirstOrDefault();
               if (current == null)
                  return new ServiceResult { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Msg = Resource.Customer + " " + Resource.Not_Found_Msg, Field = Resource.Customer_Details };

               db.Entry(current).CurrentValues.SetValues(pCustomer);
               db.SaveChanges();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Customer };
            }

         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Customer };
         }
      }

      public ServiceResult InsertCustomer(Customer pCustomer)
      {
         try
         {
            using (var db = new SBS2DBContext())
            {
               db.Customers.Add(pCustomer);
               db.SaveChanges();
               return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Customer };
            }
         }
         catch
         {
            return new ServiceResult() { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Customer };
         }
      }

   }
}
