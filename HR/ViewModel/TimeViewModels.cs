//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.ComponentModel.DataAnnotations;

//using System.Web.UI.WebControls;
//using System.Net.Mail;
//using System.Security.Cryptography;
//using System.Text;
//using System.Data.Entity;

//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.EntityFramework;

//using HR.Models;
//using SBSModel.Models;
//using SBSModel.Common;
//using SBSResourceAPI;
//using CivinTecAccessManager;
//using SBSTimeModel.Models;
////using ZKAccessManager;


//namespace HR.Models
//{

//   public class TimeConfViewModel : ModelBase
//   {
//      public List<ComboViewModel> cBranchlist { get; set; }

//      public List<Time_Device> deviceList { get; set; }

//      public List<ComboViewModel> cBrandlist { get; set; }

//      [LocalizedDisplayName("Device")]
//      public Nullable<int> Device_ID { get; set; }

//      [LocalizedDisplayName("Branch")]
//      public Nullable<int> Device_Branch_ID { get; set; }

//      [LocalizedDisplayName("Brand_Name")]
//      public string Brand_Name { get; set; }

//      [LocalizedValidMaxLength(150)]
//      [LocalizedDisplayName("IP_Address")]
//      public string Device_IP { get; set; }

//      [LocalizedValidMaxLength(150)]
//      [LocalizedDisplayName()]
//      public string Device_No { get; set; }

//      [LocalizedDisplayName()]
//      public Nullable<int> Device_Port { get; set; }

//      [LocalizedValidMaxLength(150)]
//      [LocalizedDisplayName("Username")]
//      public string Device_User_Name { get; set; }

//      public TimeConfMapViewModel[] Map_Rows { get; set; }

//      [LocalizedValidMaxLength(300)]
//      [LocalizedDisplayName("Password")]
//      public string Device_Password { get; set; }

//      /************* Device Mapping ***************/

//      [LocalizedDisplayName("Device")]
//      public Nullable<int> Search_Device_ID { get; set; }

//      [LocalizedDisplayName("Device")]
//      public Nullable<int> Map_Device_ID { get; set; }


//      public List<ComboViewModel> cDevicelist { get; set; }
//      public List<ComboViewModel> cEmplist { get; set; }
//      public List<CivinTecAccessManager.User> cEmpDevicelist { get; set; }
//      //public List<ZKAccessManager.User> cZKEmpDevicelist { get; set; }

//   }

//   public class TimeConfMapViewModel
//   {
//      public List<ComboViewModel> cEmplist { get; set; }

//      public int Index { get; set; }
//      public string Row_Type { get; set; }

//      public Nullable<int> Device_ID { get; set; }

//      [LocalizedDisplayName("Device_Employee_Mapping")]
//      public Nullable<int> Employee_Map_ID { get; set; }

//      [LocalizedDisplayName("Employee")]
//      public Nullable<int> Employee_Profile_ID { get; set; }

//      [LocalizedDisplayName()]
//      public Nullable<int> Device_Employee_Pin { get; set; }

//      [LocalizedDisplayName()]
//      public string Device_Employee_Name { get; set; }

//      [LocalizedDisplayName("Email")]
//      public string Employee_Email { get; set; }

//      public List<CivinTecAccessManager.User> cEmpDevicelist { get; set; }
//      //public List<ZKAccessManager.User> cZKEmpDevicelist { get; set; }
//   }

//   public class TimeArrangementViewModel : ModelBase
//   {
//      public List<Time_Arrangement> argList { get; set; }

//      public List<ComboViewModel> cDayOfweeklist { get; set; }
//      public List<ComboViewModel> cBranchlist { get; set; }
//      public List<ComboViewModel> cDepartmentlist { get; set; }
//      public List<ComboViewModel> cEmplist { get; set; }

//      [LocalizedDisplayName()]
//      public string search_Effective_Date { get; set; }
//      public string display_Effective_Date { get; set; }

//      [LocalizedDisplayName("Branch")]
//      public Nullable<int> search_Branch_ID { get; set; }

//      [LocalizedDisplayName("Department")]
//      public Nullable<int> search_Department_ID { get; set; }

//      public Nullable<int> Arrangement_ID { get; set; }
//      public Nullable<int> Employee_Profile_ID { get; set; }

//      public string Employee_Name { get; set; }

//      [LocalizedRequired]
//      [LocalizedDisplayName("Branch")]
//      public Nullable<int> Branch_ID { get; set; }

//      [LocalizedValidMaxLength(500)]
//      [LocalizedDisplayName()]
//      public string Remark { get; set; }

//      [LocalizedDisplayName()]
//      public Nullable<bool> Repeat { get; set; }

//      [LocalizedDisplayName()]
//      public string Day_Of_Week { get; set; }

//      [LocalizedDisplayName()]
//      public string[] Day_Of_Weeks { get; set; }

//      [LocalizedRequired]
//      [LocalizedValidDate]
//      [LocalizedDisplayName()]
//      public string Effective_Date { get; set; }

//      [LocalizedRequired]
//      [LocalizedDisplayName("Time")]
//      public string Time_From { get; set; }

//      [LocalizedRequired]
//      [LocalizedDisplayName("Time")]
//      public string Time_To { get; set; }



//   }

//   public class TimeArgPerHour
//   {
//      public int hour { get; set; }
//      public Time_Arrangement Arg { get; set; }
//   }


//   public class TimeTransactionViewModel : ModelBase
//   {
//      public List<Time_Transaction> transList { get; set; }
//      public List<ComboViewModel> cBranchlist { get; set; }
//      public List<ComboViewModel> cEmplist { get; set; }
//      public List<ComboViewModel> cDepartmentlist { get; set; }
//      public List<ComboViewModel> cDevicelist { get; set; }

//      public bool do_migrate { get; set; }

//      [LocalizedDisplayName("Time")]
//      public string search_From { get; set; }

//      [LocalizedDisplayName("To")]
//      public string search_To { get; set; }

//      [LocalizedDisplayName("Branch")]
//      public Nullable<int> search_Branch_ID { get; set; }

//      [LocalizedDisplayName("Department")]
//      public Nullable<int> search_Department_ID { get; set; }

//      [LocalizedDisplayName("Employee")]
//      public Nullable<int> search_Employee_Profile_ID { get; set; }

//      [LocalizedDisplayName("Device")]
//      public Nullable<int> search_Device_ID { get; set; }

//      public Working_Days workdays { get; set; }

//      public string Branch_Name { get; set; }
//      public string Brand_Name { get; set; }

//   }
//   public class TimeSummaryViewModel : ModelBase
//   {
//      public List<Time_Transaction> transList { get; set; }
//      public List<ComboViewModel> cBranchlist { get; set; }
//      public List<ComboViewModel> cEmplist { get; set; }
//      public List<ComboViewModel> cDepartmentlist { get; set; }
//      public List<ComboViewModel> cDevicelist { get; set; }

//      public bool do_migrate { get; set; }

//      [LocalizedDisplayName("Time")]
//      public string search_From { get; set; }

//      [LocalizedDisplayName("To")]
//      public string search_To { get; set; }

//      [LocalizedDisplayName("Branch")]
//      public Nullable<int> search_Branch_ID { get; set; }

//      [LocalizedDisplayName("Department")]
//      public Nullable<int> search_Department_ID { get; set; }

//      [LocalizedDisplayName("Employee")]
//      public Nullable<int> search_Employee_Profile_ID { get; set; }

//      [LocalizedDisplayName("Device")]
//      public Nullable<int> search_Device_ID { get; set; }

//      public Working_Days workdays { get; set; }

//      public string Branch_Name { get; set; }

//   }
//}
