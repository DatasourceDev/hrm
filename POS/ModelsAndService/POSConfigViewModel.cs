using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using POS.Common;
using SBSModel.Models;
using SBSModel.Common;


namespace POS.Models
{
    public class ConfigurationViewModel : ModelBase
    {
        public bool isCashier { get; set; }
        public bool isSupervisor { get; set; }

        public List<Branch> branchlist { get; set; }

        public string[] rightsSup { get; set; }

        [LocalizedDisplayName("Terminal", typeof(Resources.ResourcePOS))]
        public Nullable<int> Terminal_ID { get; set; }

        [LocalizedDisplayName("Cashier", typeof(Resources.ResourcePOS))]
        public string Cashier_Name { get; set; }

        public Nullable<int> Cashier_ID { get; set; }

        [LocalizedDisplayName("Branch", typeof(Resources.ResourcePOS))]
        public Nullable<int> Branch_ID { get; set; }

        [MaxLength(150)]
        [LocalizedDisplayName("Name", typeof(Resources.ResourcePOS))]
        public string Name { get; set; }

        [MaxLength(150)]
        [LocalizedDisplayName("IPAddress", typeof(Resources.ResourcePOS))]
        public string IP_Address { get; set; }

        [MaxLength(100)]
        [LocalizedDisplayName("Device", typeof(Resources.ResourcePOS))]
        public string Mac_Address { get; set; }

        [MaxLength(150)]
        [LocalizedDisplayName("TerminalName", typeof(Resources.ResourcePOS))]
        public string Terminal_Name { get; set; }

        [MaxLength(500)]
        [LocalizedDisplayName("HostName", typeof(Resources.ResourcePOS))]
        public string Host_Name { get; set; }

        //------receipt config
        public List<ComboViewModel> dateformatlist { get; set; }
        public List<ComboViewModel> paperSizelist { get; set; }

        public Nullable<int> Receipt_Conf_ID { get; set; }

        [MaxLength(50)]
        [LocalizedDisplayName("Prefix", typeof(Resources.ResourcePOS))]
        public string Prefix { get; set; }

        [LocalizedDisplayName("DateFormat", typeof(Resources.ResourcePOS))]
        public string Date_Format { get; set; }

        [MaxLength(50)]
        [LocalizedDisplayName("Suffix", typeof(Resources.ResourcePOS))]
        public string Suffix { get; set; }

        [LocalizedDisplayName("NumLenght", typeof(Resources.ResourcePOS))]
        public Nullable<int> Num_Lenght { get; set; }

        public Nullable<int> Ref_Count { get; set; }

        public string Sample { get; set; }

        public DateTime Current_Date { get; set; }

        [MaxLength(1000)]
        [LocalizedDisplayName("ReceiptHeader", typeof(Resources.ResourcePOS))]
        public string Receipt_Header { get; set; }

        [MaxLength(1000)]
        [LocalizedDisplayName("ReceiptFooter", typeof(Resources.ResourcePOS))]
        public string Receipt_Footer { get; set; }

        [LocalizedDisplayName("PaperSize", typeof(Resources.ResourcePOS))]
        public string Paper_Size { get; set; }

        [LocalizedDisplayName("IsByBranch", typeof(Resources.ResourcePOS))]
        public bool Is_By_Branch { get; set; }

        [MaxLength(50)]
        [LocalizedDisplayName("PrinterIPAddress", typeof(Resources.ResourcePOS))]
        public string Printer_IP_Address { get; set; }
        [LocalizedDisplayName("IsWebPRNT", typeof(Resources.ResourcePOS))]
        public bool Is_WebPRNT { get; set; }

        //----- Surcharge
        public Nullable<int> Tax_ID { get; set; }
        [LocalizedDisplayName("IncludeSurcharge", typeof(Resources.ResourcePOS))]
        public bool Surcharge_Include { get; set; }

        [LocalizedDisplayName("SurchargePercen", typeof(Resources.ResourcePOS))]
        public Nullable<decimal> Surcharge_Percen { get; set; }

        [LocalizedDisplayName("IncludeServiceCharge", typeof(Resources.ResourcePOS))]
        public bool Service_Charge_Include { get; set; }

        [LocalizedDisplayName("ServiceCharge", typeof(Resources.ResourcePOS))]
        public Nullable<decimal> Service_Charge_Percen { get; set; }

        public Nullable<bool> Is_Uploaded { get; set; }
        public Nullable<bool> Is_Latest { get; set; }
    }

    public class MemberConfigurationViewModel : ModelBase
    {
        public List<ComboViewModel> DiscountTypeList { get; set; }

        public bool isCashier { get; set; }
        public bool isSupervisor { get; set; }


        public int Member_Configuration_ID { get; set; }
        public Nullable<int> Company_ID { get; set; }

         [LocalizedDisplayName("MemberDiscount", typeof(Resources.ResourcePOS))]
        public Nullable<decimal> Member_Discount { get; set; }
        public string Member_Discount_Type { get; set; }

         [LocalizedDisplayName("BirthdayDiscount", typeof(Resources.ResourcePOS))]
        public Nullable<decimal> Birthday_Discount { get; set; }
        public string Birthday_Discount_Type { get; set; }

         [LocalizedDisplayName("Prefix", typeof(Resources.ResourcePOS))]
        public string Prefix { get; set; }

         [LocalizedDisplayName("NumLenght", typeof(Resources.ResourcePOS))]
        public Nullable<int> Num_Lenght { get; set; }

         [LocalizedDisplayName("RefCount", typeof(Resources.ResourcePOS))]
        public Nullable<int> Ref_Count { get; set; }
        //public string Create_By { get; set; }
        //public Nullable<System.DateTime> Create_On { get; set; }
        //public string Update_By { get; set; }
        //public Nullable<System.DateTime> Update_On { get; set; }
        //public virtual Company Company { get; set; }
    }

    public class TerminalViewModel : ModelBase
    {
        public Nullable<int> Company_ID { get; set; }

         [LocalizedDisplayName("Terminal", typeof(Resources.ResourcePOS))]
        public Nullable<int> Terminal_ID { get; set; }

        [LocalizedDisplayName("Branch", typeof(Resources.ResourcePOS))]
        public Nullable<int> Branch_ID { get; set; }


         [LocalizedDisplayName("Cashier", typeof(Resources.ResourcePOS))]
        public Nullable<int> Cashier_ID { get; set; }

        [MaxLength(150)]
        [LocalizedDisplayName("TerminalName", typeof(Resources.ResourcePOS))]
        public string Terminal_Name { get; set; }

        [MaxLength(500)]
        [LocalizedDisplayName("HostName", typeof(Resources.ResourcePOS))]
        public string Host_Name { get; set; }

        [MaxLength(100)]
        [LocalizedDisplayName("MacAddress", typeof(Resources.ResourcePOS))]
        public string Mac_Address { get; set; }


    }

    public class ReceiptConfigViewModel : ModelBase
    {
        public List<ComboViewModel> dateformatlist { get; set; }
        public List<ComboViewModel> paperSizelist { get; set; }

        public Nullable<int> Receipt_Conf_ID { get; set; }

        [Required]
        [MaxLength(50)]
        [LocalizedDisplayName("Prefix", typeof(Resources.ResourcePOS))]
        public string Prefix { get; set; }

        [Required]
        [LocalizedDisplayName("DateFormat", typeof(Resources.ResourcePOS))]
        public string Date_Format { get; set; }

        [Required]
        [MaxLength(50)]
        [LocalizedDisplayName("Suffix", typeof(Resources.ResourcePOS))]
        public string Suffix { get; set; }

        [Required]
        [LocalizedDisplayName("NumLenght", typeof(Resources.ResourcePOS))]
        public int Num_Lenght { get; set; }

        public string Sample { get; set; }

        public DateTime Current_Date { get; set; }

        [MaxLength(1000)]
        [LocalizedDisplayName("ReceiptHeader", typeof(Resources.ResourcePOS))]
        public string Receipt_Header { get; set; }

        [MaxLength(1000)]
        [LocalizedDisplayName("ReceiptFooter", typeof(Resources.ResourcePOS))]
        public string Receipt_Footer { get; set; }

        [Required]
        [LocalizedDisplayName("PaperSize", typeof(Resources.ResourcePOS))]
        public string Paper_Size { get; set; }

        [LocalizedDisplayName("IsByBranch", typeof(Resources.ResourcePOS))]
        public bool Is_By_Branch { get; set; }
    }

    public class ShiftViewModel : ModelBase
    {

        public List<Branch> branchlist { get; set; }
        public List<POS_Shift> shiftlist { get; set; }

        [LocalizedDisplayName("Shift", typeof(Resources.ResourcePOS))]
        public Nullable<int> Shift_ID { get; set; }
        public Nullable<int> Terminal_ID { get; set; }
        public string Shift_Local_ID { get; set; }
        public string Terminal_Local_ID { get; set; }
        public Nullable<int> Profile_ID { get; set; }

        public Nullable<int> Company_ID { get; set; }

        [Required]
        [LocalizedDisplayName("Branch", typeof(Resources.ResourcePOS))]
        public Nullable<int> Branch_ID { get; set; }


        [LocalizedDisplayName("OpenTime", typeof(Resources.ResourcePOS))]
        public string Open_Time { get; set; }


        [LocalizedDisplayName("CloseTime", typeof(Resources.ResourcePOS))]
        public string Close_Time { get; set; }

        [LocalizedDisplayName("EmailAddress", typeof(Resources.ResourcePOS))]
        public string Email_Address { get; set; }

        [LocalizedDisplayName("EffectiveDate", typeof(Resources.ResourcePOS))]
        public string Effective_Date { get; set; }

        [LocalizedDisplayName("TotalAmount", typeof(Resources.ResourcePOS))]
        public Nullable<decimal> Total_Amount { get; set; }

        [LocalizedDisplayName("Status", typeof(Resources.ResourcePOS))]
        public string Status { get; set; }

        public string Sales_Report_Data { get; set; }
        public string Terminal_IP_Address { get; set; }
        public int Action { get; set; }

        public Nullable<bool> Is_Uploaded { get; set; }
        public Nullable<bool> Is_Latest { get; set; }
    }
}