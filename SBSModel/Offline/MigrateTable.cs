using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBSModel.Offline
{
    public class MigrateTable
    {
        public static string[] ProductMasterTable = {   
                                         "Product_Attribute",
                                         "Product_Attribute_Value",
                                         "Product_Attribute_Map", 
                                         "Product_Tag",
                                         "Product_Price",
                                         "Product_Attribute_Map_Price",
                                         "Product_Image",
                                         "Product_Image_Attribute",
                                         "Kit",
                                         "Bom",
                                         "Unit_Of_Measurement"};

        public static string[] TransactionTable = {   
                                         "POS_Terminal", 
                                         "POS_Shift",
                                         "POS_Receipt",
                                         "POS_Receipt_Payment",                                         
                                         "POS_Products_Rcp",
                                         "Inventory_Transaction"
                                        };

         public static string[] Tables = {   
                                         "Company",
                                         "Branch",
                                         "Brand",
                                         "User_Authentication",
                                         "User_Profile",
                                         "User_Profile_Photo",
                                         "User_Role",
                                         "SBS_Module",
                                         "SBS_Module_Detail",
                                         "Page",
                                         "Page_Role",
                                         "Access_Right",
                                         "Access_Page",
                                         "User_Assign_Role",
                                         "Global_Lookup_Def",
                                         "Global_Lookup_Data",
                                         "Module",
                                         "Subscription",
                                         "User_Assign_Module",                                        
                                         "Product_Category",
                                         "Product_Table",
                                         "Product",                                       
                                         "POS_Terminal",
                                         "POS_Receipt_Configuration",
                                         "POS_Shift",
                                         "Member",
                                         "POS_Receipt",                                       
                                         "POS_Products_Rcp",                                     
                                         "Inventory_Transaction",                                      
                                         "AspNetUsers",    
                                         "Country",
                                         "State",
                                         "Currency",
                                         "Company_Details",
                                         "Tag",                  
                                         "POS_Receipt_Payment",
                                         "Member_Configuration",
                                         "Promotion",
                                         "Promotion_Branch",
                                         "Promotion_Product",
                                         "Promotion_Spacial",
                                         "Tax",
                                         "Tax_Surcharge",
                                         "Tax_GST",
                                         "Inventory_Location",
                                         "Vendor",
                                         "Product_Attribute",
                                         "Product_Attribute_Value",
                                         "Product_Attribute_Map", 
                                         "Product_Tag",
                                         "Product_Price",
                                         "Product_Attribute_Map_Price",
                                         "Product_Image",
                                         "Product_Image_Attribute",
                                         "Kit",
                                         "Bom",
                                         "Unit_Of_Measurement",
                                         "SBS_No_Pattern"
 };

    }
}
