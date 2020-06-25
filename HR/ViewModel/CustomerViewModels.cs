using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using HR.Common;
using HR.Models;
using SBSModel.Models;
using SBSModel.Common;
using SBSResourceAPI;

namespace HR.Models
{
   public class CustomerViewModels : ModelBase
   {
      public List<Customer> CustomerList { get; set; }
      public List<ComboViewModel> Customerlst { get; set; }
      public List<ComboViewModel> Statuslst { get; set; }
      public List<ComboViewModel> Countrylst { get; set; }
      public List<ComboViewModel> Statelst { get; set; }

      [LocalizedDisplayName("Customer_Name", typeof(Resource))]
      public string Customer_Name_Search { get; set; }

      public Nullable<int> Company_ID { get; set; }
      public Nullable<int> Country_ID { get; set; }

      [LocalizedDisplayName("Customer_Name", typeof(Resource))]
      public int Customer_ID { get; set; }

      [LocalizedRequired]
      [LocalizedValidMaxLength(150)]
      [LocalizedDisplayName("Customer_No", typeof(Resource))]
      public string Customer_No { get; set; }

      [LocalizedRequired]
      [LocalizedValidMaxLength(300)]
      [LocalizedDisplayName("Customer_Name", typeof(Resource))]
      public string Customer_Name { get; set; }

      [LocalizedDisplayName("Person_In_Charge", typeof(Resource))]
      public string Person_In_Charge { get; set; }

      [LocalizedValidationEmail]
      [LocalizedValidMaxLength(50)]
      //Edit by sun 29-09-2106
      //[LocalizedRequired]
      [LocalizedDisplayName("Email_Address", typeof(Resource))]
      public string Email { get; set; }

      [LocalizedValidMaxLength(100)]
      [LocalizedDisplayName("Mobile_Phone", typeof(Resource))]
      public string Mobile_Phone { get; set; }

      [LocalizedRequired]

      [LocalizedValidMaxLength(100)]
      [LocalizedDisplayName("Office_Phone", typeof(Resource))]
      public string Office_Phone { get; set; }
      public string Website { get; set; }

      [LocalizedValidMaxLength(500)]
      [LocalizedDisplayName("Address", typeof(Resource))]
      public string Billing_Address { get; set; }

      [LocalizedValidMaxLength(500)]
      [LocalizedDisplayName("Street", typeof(Resource))]
      public string Billing_Street { get; set; }

      [LocalizedDisplayName("City", typeof(Resource))]
      public string Billing_City { get; set; }

      [LocalizedDisplayName("Country", typeof(Resource))]
      public Nullable<int> Billing_Country_ID { get; set; }

      [LocalizedDisplayName("State", typeof(Resource))]
      public Nullable<int> Billing_State_ID { get; set; }

      [LocalizedDisplayName("Postal_Code", typeof(Resource))]
      public string Billing_Postal_Code { get; set; }

      [LocalizedDisplayName("Status", typeof(Resource))]
      public string Record_Status { get; set; }
   }
}