using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Authentication.Models
{
   public class Alpha7_User
   {
      public string PresetName { get; set; }

      public String Id { get; set; }

      public String FirstName { get; set; }

      public String LastName { get; set; }

      public String Email { get; set; }

      public String CompanyName { get; set; }

      public String Country { get; set; }

      public String SsoSession { get; set; }

      public DateTime CreatedAt { get; set; }

      public DateTime UpdatedAt { get; set; }
   }

   public class Alpha7_Group
   {
      public string PresetName { get; set; }

      public String Id { get; set; }

      public DateTime CreatedAt { get; set; }

      public DateTime UpdatedAt { get; set; }

      public String Status { get; set; }

      public String Name { get; set; }

      public Boolean HasCreditCard { get; set; }

      public DateTime FreeTrialEndAt { get; set; }

      public String Email { get; set; }

      public String Currency { get; set; }

      public String TimezoneAsString { get; set; }

      public String Country { get; set; }

      public String City { get; set; }
   }
}