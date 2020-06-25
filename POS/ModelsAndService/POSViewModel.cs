using POS.Common;
using SBSModel.Common;
using SBSModel.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace POS.Models {

    public class POSReportViewModel : ModelBase {

        public List<POS_Receipt> receipts { get; set; }

        [LocalizedDisplayName("StartDate", typeof(Resources.ResourcePOS))]
        public string Start_Date { get; set; }

        [LocalizedDisplayName("EndDate", typeof(Resources.ResourcePOS))]
        public string End_Date { get; set; }

        [LocalizedDisplayName("Search", typeof(Resources.ResourceMain))]
        public string TextSearch { get; set; }
    }

    public class POSQoHReportViewModel : ModelBase {

        public List<vwQuantityOnHand> inventory_qoh { get; set; }

        [LocalizedDisplayName("StartDate", typeof(Resources.ResourcePOS))]
        public string Start_Date { get; set; }

        [LocalizedDisplayName("EndDate", typeof(Resources.ResourcePOS))]
        public string End_Date { get; set; }

        [LocalizedDisplayName("Search", typeof(Resources.ResourceMain))]
        public string TextSearch { get; set; }

        public int CategoryID { get; set; }

        public List<Product_Category> CategoryList { get; set; }

    }

    public class POSViewModel : ModelBase {
        public POSViewModel() {
            this.rights = new string[] { };
            this.result = new ServiceResult();
        }
        //Add by sun 13-11-2015
        public List<ComboViewModel> CategoryLVList { get; set; }
        public Nullable<int> Category_LV { get; set; }
        public string TextSearch { get; set; }
        public bool Ch_Category_Di { get; set; }

        //Add by sun 19-08-2015
        public bool Visa { get; set; }
        public bool Master { get; set; }
        public bool AMEX { get; set; }
        public bool DinersClub { get; set; }
        public bool JCB { get; set; }

        public List<Product> products { get; set; }

        public POSAction Action { get; set; }
        public int Page_Action { get; set; }

        public Nullable<int> Receipt_ID { get; set; }
        public int SID { get; set; }
        public Nullable<int> Product_Category_ID { get; set; }

        [LocalizedDisplayName("TableNo", typeof(Resources.ResourcePOS))]
        public string Table_No { get; set; }

        public Nullable<int> Payment_Type { get; set; }

        public string Receipt_No { get; set; }
        public string Receipt_Date { get; set; }

        [Required]
        [LocalizedDisplayName("TotalQty", typeof(Resources.ResourcePOS))]
        public int Total_Qty { get; set; }

        [Required]
        [LocalizedDisplayName("Subtotal", typeof(Resources.ResourcePOS))]
        public decimal Total_Amount { get; set; }

        [LocalizedDisplayName("TotalDiscount", typeof(Resources.ResourcePOS))]
        public decimal Total_Discount { get; set; }

        [Required]
        [LocalizedDisplayName("Total", typeof(Resources.ResourcePOS))]
        public decimal Net_Amount { get; set; }

        [LocalizedDisplayName("Cash", typeof(Resources.ResourcePOS))]
        public decimal Cash_Payment { get; set; }

        [LocalizedDisplayName("CreditDebit", typeof(Resources.ResourcePOS))]
        public decimal Credit_Card_Payment { get; set; }

        [LocalizedDisplayName("AMEX", typeof(Resources.ResourcePOS))]
        public decimal AMEX_Payment { get; set; }

        [LocalizedDisplayName("DinersClub", typeof(Resources.ResourcePOS))]
        public decimal Diners_Club_Payment { get; set; }

        [LocalizedDisplayName("MasterCard", typeof(Resources.ResourcePOS))]
        public decimal Master_Card_Payment { get; set; }

        [LocalizedDisplayName("Visa", typeof(Resources.ResourcePOS))]
        public decimal Visa_Payment { get; set; }

        [LocalizedDisplayName("JCB", typeof(Resources.ResourcePOS))]
        public decimal JCB_Payment { get; set; }

        [LocalizedDisplayName("Nets", typeof(Resources.ResourcePOS))]
        public decimal Nets_Payment { get; set; }

        [MaxLength(50)]
        [LocalizedDisplayName("ApprovalCode", typeof(Resources.ResourcePOS))]
        public string Approval_Code { get; set; }
        public decimal Voucher_Amount { get; set; }



        [LocalizedDisplayName("GroupDiscount", typeof(Resources.ResourcePOS))]
        public decimal Discount { get; set; }

        [LocalizedDisplayName("DiscountReason", typeof(Resources.ResourcePOS))]
        public string Discount_Reason { get; set; }

        public string Discount_Type { get; set; }

        [LocalizedDisplayName("Changes", typeof(Resources.ResourcePOS))]
        public decimal Changes { get; set; }

        [LocalizedDisplayName("RemainingBalance", typeof(Resources.ResourcePOS))]
        public decimal Remaining_Balance { get; set; }

        [LocalizedDisplayName("CardType", typeof(Resources.ResourcePOS))]
        public Nullable<int> Card_Type { get; set; }

        public string Card_Type_Name { get; set; }

        [LocalizedDisplayName("CardBrand", typeof(Resources.ResourcePOS))]
        public Nullable<int> Card_Branch { get; set; }

        public POSProductViewModel[] Product_Rows { get; set; }
       


        //Added by NayThway on 30-May-2015
        public bool useGST { get; set; }
        public Nullable<decimal> valGST { get; set; }
        public decimal Total_GST_Amount { get; set; }

        public string Terminal_Name { get; set; }
        public Nullable<int> Terminal_Info_ID { get; set; }
        public Nullable<int> Location_ID { get; set; }
        public Nullable<int> Company_ID { get; set; }

        public List<Product_Category> productCategory { get; set; }
        public Product_Table productTable { get; set; }

        public List<ComboViewModel> banks { get; set; }
        public List<ComboViewModel> cardTypes { get; set; }

        public POS_Receipt receipt { get; set; }
        public POS_Receipt_Configuration receipt_config { get; set; }

        [LocalizedDisplayName("Terminal", typeof(Resources.ResourcePOS))]
        public int sTerminal { get; set; }

        [LocalizedDisplayName("Cashier", typeof(Resources.ResourcePOS))]
        public int sCashier { get; set; }

        [LocalizedDisplayName("Date", typeof(Resources.ResourcePOS))]
        public string sDate { get; set; }

        //public List<ComboViewModel> terminallist { get; set; }
        //public List<ComboViewModel> cashierlist { get; set; }

        public string Pos_Select_Mode { get; set; }
        public string Text1 { get; set; }
        public string Text2 { get; set; }
        public string Text3 { get; set; }

        //----- Surcharge
        [LocalizedDisplayName("Include", typeof(Resources.ResourcePOS))]
        public bool Surcharge_Include { get; set; }

        [LocalizedDisplayName("SurchargePercen", typeof(Resources.ResourcePOS))]
        public Nullable<decimal> Surcharge_Percen { get; set; }

        public Nullable<decimal> Surcharge_Master { get; set; }
        public Nullable<decimal> Surcharge_Visa { get; set; }
        public Nullable<decimal> Surcharge_AMEX { get; set; }
        public Nullable<decimal> Surcharge_Diner { get; set; }
        public Nullable<decimal> Surcharge_JCB { get; set; }

        public Nullable<decimal> Surcharge_Amount { get; set; }
        public bool Service_Charge_Include { get; set; }
        public Nullable<decimal> Service_Charge_Rate { get; set; }
        public decimal Service_Charge_Amount { get; set; }
        public string Business_Type { get; set; }

        public string Remark { get; set; }
        public List<POS_Receipt_Payment> Payments { get; set; }

        [LocalizedDisplayName("BackOrder", typeof(Resources.ResourcePOS))]
        public bool Back_Order { get; set; }

        [LocalizedDisplayName("CombinePayment", typeof(Resources.ResourcePOS))]
        public bool Combine_Payment { get; set; }

        public string Status { get; set; }

        //Added by Nay on 07-Oct-2015
        public string Customer_Name { get; set; }
        public string Contact_No { get; set; }
        public string Customer_Email { get; set; }
        public string searchProduct { get; set; }
        public string ReceiptData { get; set; }
        public string Terminal_IP_Address { get; set; }

        public string Currency_Code { get; set; }
        public List<Member> MemberList { get; set; }

        public string Text_Search_Member { get; set; }

        public Nullable<int> Select_Member_ID { get; set; }
        public bool Select_Birthday { get; set; }

        [LocalizedDisplayName("RedeemCredits", typeof(Resources.ResourcePOS))]
        public Nullable<decimal> Redeem_Credits { get; set; }

        [LocalizedDisplayName("MemberDiscount", typeof(Resources.ResourcePOS))]
        public Nullable<decimal> Member_Discount { get; set; }

        public string Member_Discount_Type { get; set; }

        [LocalizedDisplayName("BirthdayDiscount", typeof(Resources.ResourcePOS))]
        public Nullable<decimal> Birthday_Discount { get; set; }

        [LocalizedDisplayName("BirthdayDiscountType", typeof(Resources.ResourcePOS))]
        public string Birthday_Discount_Type { get; set; }

        public bool Is_Text_Search_Member { get; set; }

        [LocalizedDisplayName("MemberDiscount", typeof(Resources.ResourcePOS))]
        public Nullable<decimal> Select_Member_Discount { get; set; }

        [LocalizedDisplayName("MemberDiscountType", typeof(Resources.ResourcePOS))]
        public string Select_Member_Discount_Type { get; set; }

        public List<Product> RelatedProducts { get; set; }

        public List<ComboViewModel> CategoryList { get; set; }
        public bool QuickAddProduct { get; set; }
    }

    public class POSMemberViewModel : ModelBase {
        public List<Member> memberList { get; set; }

        public Nullable<int> Member_ID { get; set; }

        public Nullable<int> Company_ID { get; set; }

        [LocalizedDisplayName("MemberCardNo", typeof(Resources.ResourcePOS))]
        public string Member_Card_No { get; set; }

        [LocalizedDisplayName("MemberName", typeof(Resources.ResourcePOS))]
        public string Member_Name { get; set; }

        [LocalizedDisplayName("NRICNo", typeof(Resources.ResourcePOS))]
        public string NRIC_No { get; set; }

        [LocalizedDisplayName("PhoneNo", typeof(Resources.ResourcePOS))]
        public string Phone_No { get; set; }

        [LocalizedDisplayName("Email", typeof(Resources.ResourcePOS))]
        public string Email { get; set; }

        [LocalizedDisplayName("DOB", typeof(Resources.ResourcePOS))]
        public string DOB { get; set; }

        [LocalizedDisplayName("Gender", typeof(Resources.ResourcePOS))]
        public string Gender { get; set; }

        [LocalizedDisplayName("Address", typeof(Resources.ResourcePOS))]
        public string Address { get; set; }

        [LocalizedDisplayName("MemberStatus", typeof(Resources.ResourcePOS))]
        public string Member_Status { get; set; }

        public bool Select_Birthday { get; set; }

        [LocalizedDisplayName("MemberDiscount", typeof(Resources.ResourcePOS))]
        public Nullable<decimal> Select_Discount { get; set; }

        [LocalizedDisplayName("MemberDiscountType", typeof(Resources.ResourcePOS))]
        public string Select_Discount_Type { get; set; }

        [LocalizedDisplayName("MemberDiscount", typeof(Resources.ResourcePOS))]
        public Nullable<decimal> Member_Discount { get; set; }

        [LocalizedDisplayName("MemberDiscountType", typeof(Resources.ResourcePOS))]
        public string Member_Discount_Type { get; set; }

        public string Currency_Code { get; set; }

        [LocalizedDisplayName("Credits", typeof(Resources.ResourcePOS))]
        public Nullable<decimal> Credit { get; set; }

        [LocalizedDisplayName("BirthdayDiscount", typeof(Resources.ResourcePOS))]
        public Nullable<decimal> Birthday_Discount { get; set; }

        [LocalizedDisplayName("BirthdayDiscountType", typeof(Resources.ResourcePOS))]
        public string Birthday_Discount_Type { get; set; }

        public List<POS_Receipt> purchaseList { get; set; }

        public bool Is_Payment { get; set; }

        //Search Fields
        public string Search_Name { get; set; }
        public string Search_NRIC { get; set; }
        public string Search_Email { get; set; }
        public string Search_Card_No { get; set; }
        public string Search_Free_Text { get; set; }
        public string Search_Date_From { get; set; }
        public string Search_Date_To { get; set; }
        public string Search_Status { get; set; }
        public string Search_Receipt_No { get; set; }
        public string Search_Terminal { get; set; }
        public string Search_Amount { get; set; }
    }

    public class POSMemberBirthdayViewModel : ModelBase {
        [LocalizedDisplayName("BirthdayMonth", typeof(Resources.ResourcePOS))]
        public string Search_Birthday_Month { get; set; }
        [LocalizedDisplayName("Search", typeof(Resources.ResourceMain))]
        public string Text_Search { get; set; }
        public List<Member> Members { get; set; }
    }

    public class POSCustomerPurchasesViewModel : ModelBase {
        public int Category_Id { get; set; }
        [LocalizedDisplayName("Search", typeof(Resources.ResourceMain))]
        public string Text_Search { get; set; }
        public List<Product_Category> Category_List { get; set; }
        public List<CustomerPurchaseViewModel> Customers { get; set; }
    }

    public class CustomerPurchaseViewModel : ModelBase {
        public int? Member_Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Contact_No { get; set; }
        public Nullable<DateTime> Purchase_Date { get; set; }
    }

    public class POSDlgType {
        public static string Qty = "Qty";
        public static string Topup = "Topup";
        public static string Discount = "Discount";
        public static string OpenBill = "OpenBill";
    }

    public class POSNumberViewModel {
        public string Type { get; set; }
        public string Display_Label_ID { get; set; }

    }


    public class POSProductViewModel {
        public int Index { get; set; }
        public Nullable<int> ID { get; set; }
        public Nullable<int> Receipt_ID { get; set; }
        public Nullable<int> Product_ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Product_Name { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public decimal GST { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public string Discount_Type { get; set; }
        public string Image { get; set; }
        public List<Product_Attribute> Product_Attribute { get; set; }
        public string Row_Type { get; set; }
        public string Receipt_Local_ID { get; set; }
        public string Receipt_Product_Local_ID { get; set; }
        public bool Quick_Add_Product { get; set; }

        public Nullable<int> Attr1 { get; set; }
        public Nullable<int> Attr2 { get; set; }
        public Nullable<int> Attr3 { get; set; }
        public Nullable<int> Attr4 { get; set; }
        public Nullable<int> Attr5 { get; set; }
        public Nullable<int> Map_ID { get; set; }
    }

    public class POSProductColor {
        public int Product_Color_ID { get; set; }
        public string Color { get; set; }
    }

    public class POSProductSize {
        public int Product_Size_ID { get; set; }
        public string Size { get; set; }
    }
}