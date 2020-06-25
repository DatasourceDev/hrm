using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBSModel.Models
{
    #region Global
    public class _Ctl_Table_Syn
    {
        public string Table_Name { get; set; }
        public Nullable<int> Index { get; set; }
        public string Update_By { get; set; }
        public string Update_On { get; set; }
    }

    public class _Company_Details
    {

        public int Company_ID { get; set; }

        public string Name { get; set; }

        public string Currency_Code { get; set; }

        public string Address { get; set; }

        public string Country { get; set; }

        public string State { get; set; }

        public string Zip_Code { get; set; }

        public string Tagline { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }

        public string Email { get; set; }

        public string Website { get; set; }

        public string Registry { get; set; }

        public string Effective_Date { get; set; }

        public string Tax_No { get; set; }

        public string Company_Status { get; set; }

        public string Registration_Date { get; set; }

        public string GST_Registration { get; set; }

        public string APIUsername { get; set; }

        public string APIPassword { get; set; }

        public string APISignature { get; set; }

        public string Business_Type { get; set; }

        public Nullable<int> No_Of_Table { get; set; }

        public string Product_Table_Prefix { get; set; }

        public string Receipt_Date_Format { get; set; }

        public string Receipt_Prefix { get; set; }

        public string Receipt_Suffix { get; set; }
        public Nullable<int> Receipt_Num_Lenght { get; set; }

        public string Receipt_Header { get; set; }

        public string Receipt_Footer { get; set; }
        public Nullable<bool> Surcharge_Include { get; set; }
        public Nullable<decimal> Surcharge_Percen { get; set; }
        public Nullable<decimal> Member_Discount { get; set; }

        public string Member_Discount_Type { get; set; }
        public Nullable<decimal> Birthday_Discount { get; set; }

        public string Birthday_Discount_Type { get; set; }
        public string Update_On { get; set; }

        public Nullable<bool> Include_Service_Charge { get; set; }
        public Nullable<decimal> Service_Charge_Percen { get; set; }

        public Nullable<bool> Include_GST { get; set; }
        public Nullable<decimal> GST_Percen { get; set; }
        public string Tax_Type { get; set; }
    }

    public class _Branch
    {
        public int Branch_ID { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public string Branch_Code { get; set; }
        public string Branch_Name { get; set; }
        public string Branch_Desc { get; set; }
        public string Update_On { get; set; }
    }

    public class _Brand
    {
        public int Brand_ID { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public string Brand_Code { get; set; }
        public string Brand_Name { get; set; }
        public string Brand_Description { get; set; }
        public string Update_On { get; set; }
    }

    public class _User_Profile
    {
        public int Profile_ID { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public string Name { get; set; }
        public string User_Status { get; set; }
        public string PWD { get; set; }
        public string Email_Address { get; set; }
        public string Image { get; set; }
        public string Update_On { get; set; }
    }

    public class _Global_Lookup_Def
    {
        public int Def_ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Record_Status { get; set; }
        public string Update_On { get; set; }
    }

    public class _Global_Lookup_Data
    {
        public int Lookup_Data_ID { get; set; }
        public Nullable<int> Def_ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Record_Status { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public string Update_On { get; set; }
    }

    public partial class _Tax_Surcharge
    {
        public int Tax_Surcharge_ID { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public Nullable<int> Tax_ID { get; set; }
        public Nullable<int> Tax_Title { get; set; }
        public Nullable<decimal> Tax { get; set; }
        public string Record_Status { get; set; }
        public string Update_On { get; set; }
    }

    public class _Member
    {
        public int Member_ID { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public string Member_Card_No { get; set; }
        public string Member_Name { get; set; }
        public string NRIC_No { get; set; }
        public string Phone_No { get; set; }
        public string Email { get; set; }
        public string DOB { get; set; }
        public string Member_Status { get; set; }
        public Nullable<decimal> Member_Discount { get; set; }
        public string Member_Discount_Type { get; set; }
        public Nullable<decimal> Credit { get; set; }
        public string Create_By { get; set; }
        public string Create_On { get; set; }
        public string Update_By { get; set; }
        public string Update_On { get; set; }
    }
    #endregion
    
    #region Product

    //public partial class _Attribute
    //{
    //    public int Product_ID { get; set; }
    //    public string Product_Code { get; set; }
    //    public string Product_Name { get; set; }
    //    public byte[] Image { get; set; }
    //    public Nullable<decimal> Selling_Price { get; set; }
        
    //    public List<Product_Attribute> Product_Attribute { get; set; }
    //    //public List<Product_Attribute_Map> Product_Attribute_Map { get; set; }    
    //    //public List<Product_Image> Product_Image { get; set; }
    //    //public List<Product_Image_Attribute> Product_Image_Attribute { get; set; }
    //    //public List<Product_Price> Product_Price { get; set; }
    //    //public List<Product_Attribute_Map_Price> Product_Attribute_Map_Price { get; set; }
    //}


    public partial class _Product_Category
    {
        public int Product_Category_ID { get; set; }
        public string Category_Name { get; set; }
        public string Record_Status { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public Nullable<int> Category_Level { get; set; }
        public Nullable<int> Category_Parent_ID { get; set; }
        public string Update_On { get; set; }

    }

    public partial class _Product
    {
        public int Product_ID { get; set; }
        public Nullable<int> Company_ID { get; set; }
        public string Product_Code { get; set; }
        public string Product_Name { get; set; }
        public Nullable<decimal> Selling_Price { get; set; }
        public Nullable<decimal> Discount_Price { get; set; }
        public string Description { get; set; }
        public Nullable<bool> Product_Service { get; set; }
        public string Type { get; set; }
        public string Record_Status { get; set; }
        public Nullable<int> Branch_ID { get; set; }
        public Nullable<int> Product_Category_L1 { get; set; }
        public Nullable<int> Product_Category_L2 { get; set; }
        public Nullable<int> Product_Category_L3 { get; set; }
        public Nullable<decimal> Reorder_Qty { get; set; }
        public Nullable<bool> Sellable { get; set; }
        public string Extra_Info_Description { get; set; }
        public Nullable<int> Brand_ID { get; set; }
        public Nullable<int> Location_L1 { get; set; }
        public Nullable<int> Location_L2 { get; set; }
        public Nullable<int> Location_L3 { get; set; }
        public string Image { get; set; }
        public string Update_On { get; set; }

        public List<_Product_Attribute> Product_Attribute { get; set; }
        public List<_Product_Attribute_Map> Product_Attribute_Map { get; set; }
    }

    public partial class _Product_Attribute
    {
        public int Attribute_ID { get; set; }
        public Nullable<int> Product_ID { get; set; }
        public string Attribute_Name { get; set; }
        public List<_Product_Attribute_Value> Product_Attribute_Value { get; set; }
    }

    public partial class _Product_Attribute_Value
    {
        public int Attribute_Value_ID { get; set; }
        public string Attribute_Value { get; set; }
        public Nullable<int> Attribute_ID { get; set; }

    }

    public partial class _Product_Attribute_Map
    {

        public int Map_ID { get; set; }
        public Nullable<int> Attr1 { get; set; }
        public Nullable<int> Attr2 { get; set; }
        public Nullable<int> Attr3 { get; set; }
        public Nullable<int> Attr4 { get; set; }
        public Nullable<int> Attr5 { get; set; }
        public Nullable<decimal> Costing_Price { get; set; }
        public Nullable<decimal> Selling_Price { get; set; }
        public Nullable<bool> Record_Status { get; set; }
        public Nullable<int> Product_ID { get; set; }
    }

    #endregion

    #region POS
    public class _POS_Receipt
    {
        public Nullable<int> Company_ID { get; set; }
        public Nullable<int> Profile_ID { get; set; } //Cashier
        public Nullable<int> Cashier_ID { get; set; } //Cashier
        public Nullable<int> User_Authentication_ID { get; set; }

        public int Action { get; set; }
        public Nullable<int> Receipt_ID { get; set; }
        public string Receipt_Local_ID { get; set; }
        public Nullable<int> Shift_ID { get; set; }
        public string Shift_Local_ID { get; set; }
        public string Terminal_Local_ID { get; set; }

        public Nullable<int> Payment_Type { get; set; }
        public Nullable<int> Total_Qty { get; set; }
        public Nullable<decimal> Total_Amount { get; set; }
        public Nullable<decimal> Total_Discount { get; set; }
        public Nullable<decimal> Net_Amount { get; set; }
        public Nullable<decimal> Cash_Payment { get; set; }

        public Nullable<decimal> Surcharge_Amount { get; set; }
        public Nullable<decimal> Surcharge_Percen { get; set; }

        public Nullable<decimal> GST_Percen { get; set; }
        public Nullable<decimal> Total_GST_Amount { get; set; }

        public Nullable<decimal> Service_Charge_Percen { get; set; }
        public Nullable<decimal> Service_Charge_Amount { get; set; }

        public Nullable<decimal> Voucher_Amount { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public string Discount_Type { get; set; }

        public Nullable<decimal> Changes { get; set; }
        public Nullable<int> Card_Type { get; set; }
        public Nullable<int> Card_Branch { get; set; }


        public Nullable<int> Branch_ID { get; set; }
        public Nullable<int> Terminal_ID { get; set; }
        public Nullable<int> Terminal_Info_ID { get; set; }
        public Nullable<int> Member_ID { get; set; }

        public string Approval_Code { get; set; }
        public string Discount_Reason { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
        public string Customer_Name { get; set; }
        public string Contact_No { get; set; }
        public string NRIC_No { get; set; }
        public List<_POS_Products_Rcp> Products { get; set; }
        public List<_POS_Receipt_Payment> Payments { get; set; }

        public Nullable<int> Promotion_ID { get; set; }
        public Nullable<int> Cashier { get; set; }
        public string Card_Branch_Name { get; set; }
        public string Card_Type_Name { get; set; }
        public string Receipt_Date { get; set; }

        public string Terminal { get; set; }

        public string Receipt_Time { get; set; }
        public string Receipt_No { get; set; }
        public string Receipt_Local_No { get; set; }
        public string Branch { get; set; }

        public Nullable<decimal> Member_Discount { get; set; }
        public string Member_Discount_Type { get; set; }
        public Nullable<bool> Is_Birthday_Discount { get; set; }

        public string Create_By { get; set; }

        public string Create_On { get; set; }

        public string Update_By { get; set; }

        public string Update_On { get; set; }

    }

    public class _POS_Products_Rcp
    {
        public int ID { get; set; }
        public Nullable<int> Receipt_ID { get; set; }
        public Nullable<int> Product_ID { get; set; }
        public Nullable<int> Qty { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<int> Product_Color_ID { get; set; }
        public Nullable<int> Product_Size_ID { get; set; }
        public string Product_Code { get; set; }
        public string Product_Name { get; set; }
        public string Product_Color { get; set; }
        public string Product_Size { get; set; }
        public Nullable<decimal> GST { get; set; }
        public Nullable<decimal> Total_GST_Amount { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public string Discount_Type { get; set; }
        public string Create_By { get; set; }
        public string Create_On { get; set; }
        public string Update_By { get; set; }
        public string Update_On { get; set; }

    }

    public class _POS_Receipt_Payment 
    {
        public int Receipt_Payment_ID { get; set; }
        public Nullable<int> Receipt_ID { get; set; }
        public Nullable<int> Payment_Type { get; set; }
        public Nullable<decimal> Payment_Amount { get; set; }
        public string Approval_Code { get; set; }
        public Nullable<int> Card_Type { get; set; }
        public Nullable<int> Card_Branch { get; set; }

        public Nullable<decimal> Surcharge_Amount { get; set; }
        public Nullable<decimal> Surcharge_Percen { get; set; }

        public string Create_By { get; set; }
        public string Create_On { get; set; }
        public string Update_By { get; set; }
        public string Update_On { get; set; }
    }

    public class _POS_Shift
    {
        public Nullable<int> Shift_ID { get; set; }
        public string Shift_Local_ID { get; set; }
        public string Terminal_Local_ID { get; set; }
        public Nullable<int> Terminal_ID { get; set; }

        public Nullable<int> Company_ID { get; set; }

        public Nullable<int> Branch_ID { get; set; }
        public string Branch_Name { get; set; }

        public string Open_Time { get; set; }


        public string Close_Time { get; set; }

        public string Email_Address { get; set; }

        public string Effective_Date { get; set; }

        public Nullable<decimal> Total_Amount { get; set; }

        public string Status { get; set; }
        public string Create_By { get; set; }

        public string Create_On { get; set; }

        public string Update_By { get; set; }

        public string Update_On { get; set; }

    }

    public class _POS_Terminal
    {
        public Nullable<int> Company_ID { get; set; }

        public Nullable<int> Terminal_ID { get; set; }
        public string Terminal_Local_ID { get; set; }
        public Nullable<int> Branch_ID { get; set; }


        public Nullable<int> Cashier_ID { get; set; }

        public string Terminal_Name { get; set; }

        public string Host_Name { get; set; }

        public string Mac_Address { get; set; }

        public string Create_By { get; set; }

        public string Create_On { get; set; }

        public string Update_By { get; set; }

        public string Update_On { get; set; }
    }
    #endregion



}
