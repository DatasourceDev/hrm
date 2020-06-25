using POSSyn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SBSModel.Models;
using SBSModel.Common;
using RNCryptor;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;


namespace POSSyn.Controllers
{
    public class ControllerBase : Controller
    {
        protected List<_Company_Details> LstCompany(Nullable<int> pCompanyID, Nullable<DateTime> pUpdateon = null)
        {
            var _companies = new List<_Company_Details>();
            var synService = new POSMasterSyncService();
            var company = synService.GetCompany(pCompanyID, pUpdateon);
            if (company != null)
            {
                var detail = company.Company_Details.FirstOrDefault();

                var com = new _Company_Details()
                {
                    Company_ID = detail.Company_ID,
                    Address = detail.Address,
                    APIPassword = detail.APIPassword,
                    APISignature = detail.APISignature,
                    APIUsername = detail.APIUsername,
                    Business_Type = detail.Business_Type,
                    Company_Status = detail.Company_Status,

                    Effective_Date = DateUtil.ToDisplayDateTime(detail.Effective_Date),
                    Email = detail.Email,
                    Fax = detail.Fax,
                    GST_Registration = detail.GST_Registration,
                    Name = detail.Name,
                    Phone = detail.Phone,
                    Registration_Date = DateUtil.ToDisplayDateTime(detail.Registration_Date),
                    Registry = detail.Registry,
                    Tagline = detail.Tagline,
                    Tax_No = detail.Tax_No,
                    Website = detail.Website,
                    Zip_Code = detail.Zip_Code,
                    Update_On = DateUtil.ToDisplayDateTime(company.Update_On),
                };

                if (detail.Country1 != null)
                {
                    com.Country = detail.Country1.Description;
                }
                if (detail.State1 != null)
                {
                    com.State = detail.State1.Descrition;
                }
                if (detail.Currency != null)
                {
                    com.Currency_Code = detail.Currency.Currency_Code;
                }
                if (company.POS_Receipt_Configuration != null && company.POS_Receipt_Configuration.FirstOrDefault() != null)
                {
                    var rc = company.POS_Receipt_Configuration.FirstOrDefault();
                    com.Receipt_Date_Format = rc.Date_Format;
                    com.Receipt_Footer = rc.Receipt_Footer;
                    com.Receipt_Header = rc.Receipt_Header;
                    com.Receipt_Num_Lenght = rc.Num_Lenght;
                    com.Receipt_Prefix = rc.Prefix;
                    com.Receipt_Suffix = rc.Suffix;
                    

                    if (rc.Update_On > DateUtil.ToDate(com.Update_On))
                        com.Update_On = DateUtil.ToDisplayDateTime(rc.Update_On);
                }

                if (company.Product_Table != null && company.Product_Table.FirstOrDefault() != null)
                {
                    var pt = company.Product_Table.FirstOrDefault();
                    com.Product_Table_Prefix = pt.Prefix;
                    com.No_Of_Table = pt.No_Of_Table;

                    if (pt.Update_On > DateUtil.ToDate(com.Update_On))
                        com.Update_On = DateUtil.ToDisplayDateTime(pt.Update_On);
                }
                if (company.Member_Configuration != null && company.Member_Configuration.FirstOrDefault() != null)
                {
                    var mc = company.Member_Configuration.FirstOrDefault();
                    com.Member_Discount = mc.Member_Discount;
                    com.Member_Discount_Type = mc.Member_Discount_Type;
                    com.Birthday_Discount = mc.Birthday_Discount;
                    com.Birthday_Discount_Type = mc.Birthday_Discount_Type;

                    if (mc.Update_On > DateUtil.ToDate(com.Update_On))
                        com.Update_On = DateUtil.ToDisplayDateTime(mc.Update_On);
                }

                if (company.Taxes != null && company.Taxes.FirstOrDefault() != null)
                {
                    var tax = company.Taxes.FirstOrDefault();
                    if (tax != null)
                    {
                        com.Include_Service_Charge = tax.Include_Service_Charge;
                        com.Service_Charge_Percen = tax.Service_Charge_Percen;
                        var gst = tax.Tax_GST.Where(w => w.Record_Status == RecordStatus.Active && w.Is_Default == true).FirstOrDefault();
                        if (gst != null)
                        {
                            com.Include_GST = tax.Include_GST;
                            com.GST_Percen = gst.Tax;
                            com.Tax_Type = gst.Tax_Type;
                        }

                        com.Surcharge_Include = tax.Include_Surcharge;
                        com.Surcharge_Percen = tax.Surcharge_Percen;
                        
                    }

                }
                ObjectUtil.BindDefault(com);

                _companies.Add(com);
            }


            return _companies;
        }

        protected List<_Branch> LstBranch(Nullable<int> pCompanyID, Nullable<DateTime> pUpdateon = null)
        {
            var bService = new BranchService();
            var _branches = new List<_Branch>();
            foreach (var row in bService.LstBranch(pCompanyID, pUpdateon))
            {

                var brand = new _Branch()
                {
                    Company_ID = row.Company_ID,
                    Branch_ID = row.Branch_ID,
                    Branch_Code = row.Branch_Code,
                    Branch_Desc = row.Branch_Desc,
                    Branch_Name = row.Branch_Name,
                    Update_On = DateUtil.ToDisplayDateTime(row.Update_On),
                };

                ObjectUtil.BindDefault(brand);
                _branches.Add(brand);
            }

            return _branches;
        }

        protected List<_Brand> LstBrand(Nullable<int> pCompanyID, Nullable<DateTime> pUpdateon = null)
        {
            var bService = new BrandService();
            var _brands = new List<_Brand>();
            foreach (var row in bService.LstBrand(pCompanyID, pUpdateon))
            {

                var brand = new _Brand()
                {
                    Company_ID = row.Company_ID,
                    Brand_ID = row.Brand_ID,
                    Brand_Name = row.Brand_Name,
                    Brand_Description = row.Brand_Description,
                    Update_On = DateUtil.ToDisplayDateTime(row.Update_On),
                };

                ObjectUtil.BindDefault(brand);
                _brands.Add(brand);
            }

            return _brands;
        }

        protected List<_User_Profile> LstUser(Nullable<int> pCompanyID, Nullable<DateTime> pUpdateon = null)
        {
            var _users = new List<_User_Profile>();

            var uService = new UserService();
            foreach (var row in uService.LstUser(pCompanyID, pUpdateon))
            {

                var user = new _User_Profile()
                {
                    Company_ID = row.Company_ID,
                    Email_Address = row.User_Authentication.Email_Address,
                    Name = AppConst.GetUserName(row),
                    Profile_ID = row.Profile_ID,
                    User_Status = row.User_Status,
                    PWD = row.User_Authentication.PWD,
                    Update_On = DateUtil.ToDisplayDateTime(row.Update_On),
                };

                if (row.User_Profile_Photo != null)
                {
                    var image = row.User_Profile_Photo.FirstOrDefault();
                    if (image != null)
                    {
                        user.Image = Convert.ToBase64String(image.Photo);
                    }
                }

                ObjectUtil.BindDefault(user);
                _users.Add(user);
            }
            return _users;
        }
        protected List<_Global_Lookup_Def> LstLookupDef(Nullable<int> pCompanyID, Nullable<DateTime> pUpdateon = null)
        {
            var _defs = new List<_Global_Lookup_Def>();
            var gService = new GlobalLookupService();
            foreach (var row in gService.LstLookupDef(pUpdateon))
            {
                var def = new _Global_Lookup_Def()
                {
                    Def_ID = row.Def_ID,
                    Name = row.Name,
                    Description = row.Description,
                    Record_Status = row.Record_Status,
                    Update_On = DateUtil.ToDisplayDateTime(row.Update_On),
                };

                ObjectUtil.BindDefault(def);
                _defs.Add(def);
            }

            return _defs;
        }
        protected List<_Global_Lookup_Data> LstLookupData(Nullable<int> pCompanyID, Nullable<DateTime> pUpdateon = null)
        {
            var _datas = new List<_Global_Lookup_Data>();

            var gService = new GlobalLookupService();
            foreach (var row in gService.LstLookUpData(pUpdateon))
            {
                var data = new _Global_Lookup_Data()
                {
                    Company_ID = row.Company_ID,
                    Def_ID = row.Def_ID,
                    Name = row.Name,
                    Description = row.Description,
                    Record_Status = row.Record_Status,
                    Lookup_Data_ID = row.Lookup_Data_ID,
                    Update_On = DateUtil.ToDisplayDateTime(row.Update_On),
                };

                ObjectUtil.BindDefault(data);
                _datas.Add(data);
            }
            return _datas;
        }

        protected List<_Tax_Surcharge> LstTaxSurcharge(TaxCriteria cri)
        {
            var _datas = new List<_Tax_Surcharge>();
            var cbService = new ComboService();
            var iService = new InventoryService();
            var taxService = new TaxServie();
            var taxs = taxService.LstTax(cri);
            if (taxs.Object != null)
            {
                var tax = ((List<Tax>)taxs.Object).FirstOrDefault();
                if (tax != null)
                {
                   var cardtypelist = cbService.LstLookup(ComboType.Credit_Card_Type, cri.Company_ID);
                    foreach (var row in tax.Tax_Surcharge)
                    {
                        if (row.Record_Status == RecordStatus.Active)
                        {
                            var data = new _Tax_Surcharge()
                            {
                                Record_Status = row.Record_Status,
                                Tax_ID = row.Tax_Preference_ID,
                                Tax_Surcharge_ID = row.Tax_Surcharge_ID,
                                Tax_Title = row.Tax_Title,
                                Tax = row.Tax,
                                Company_ID = row.Tax1.Company_ID,
                                Update_On = DateUtil.ToDisplayDateTime(row.Update_On),

                            };
                            var cardtypeID = data.Tax_Title.HasValue ? data.Tax_Title.Value.ToString() : "";
                            var cardtype = cardtypelist.Where(w => w.Value == cardtypeID).FirstOrDefault();
                            if (cardtype != null)
                            {
                                if (cardtype.Text == CreditCardType.MasterCard)
                                {
                                    data.Tax_Title = 1;
                                }
                                else if (cardtype.Text == CreditCardType.Visa)
                                {
                                    data.Tax_Title = 2;
                                }
                                else if (cardtype.Text == CreditCardType.AMEX)
                                {
                                    data.Tax_Title = 3;
                                }
                                else if (cardtype.Text == CreditCardType.DinersClub)
                                {
                                    data.Tax_Title = 4;
                                }
                                else if (cardtype.Text == CreditCardType.JCB)
                                {
                                    data.Tax_Title = 5;
                                }
                                else
                                {
                                    data.Tax_Title = 0;
                                }
                            }
                            ObjectUtil.BindDefault(data);
                            _datas.Add(data);
                        }
                      
                    }
                }

            }

            return _datas;
        }

        protected List<_Member> LstMember(MemberCriteria cri)
        {
            var _members = new List<_Member>();

            var mService = new MemberService();
            foreach (var row in mService.LstMember(cri))
            {
                var mem = new _Member()
                {
                    Company_ID = row.Company_ID,
                    Credit = row.Credit,
                    DOB = DateUtil.ToDisplayDate(row.DOB),
                    Email = row.Email,
                    Member_Card_No = row.Member_Card_No,
                    Member_Discount = row.Member_Discount,
                    Member_Discount_Type = row.Member_Discount_Type,
                    Member_ID = row.Member_ID,
                    Member_Name = row.Member_Name,
                    Member_Status = row.Member_Status,
                    NRIC_No = row.NRIC_No,
                    Phone_No = row.Phone_No,
                    Create_By = row.Create_By,
                    Update_By = row.Update_By,
                    Create_On = DateUtil.ToDisplayDateTime(row.Create_On),
                    Update_On = DateUtil.ToDisplayDateTime(row.Update_On),
                };

                ObjectUtil.BindDefault(mem);
                _members.Add(mem);
            }

            return _members;
        }
        protected List<_Product_Category> LstCatgory(CategoryCriteria cri)
        {
            var _cats = new List<_Product_Category>();
            var iService = new InventoryService();


            foreach (var row in iService.LstCategory(cri))
            {
                var cat = new _Product_Category()
                {
                    Company_ID = row.Company_ID,
                    Category_Level = row.Category_Level,
                    Category_Name = row.Category_Name,
                    Category_Parent_ID = row.Category_Parent_ID,
                    Product_Category_ID = row.Product_Category_ID,
                    Record_Status = row.Record_Status,
                    Update_On = DateUtil.ToDisplayDateTime(row.Update_On),
                };

                ObjectUtil.BindDefault(cat);
                _cats.Add(cat);
            }

            return _cats;
        }
        protected List<_Product> LstProduct(ProductCriteria cri)
        {
            var _products = new List<_Product>();
            var iService = new InventoryService();


            foreach (var prow in (List<Product>)iService.LstProduct(cri).Object)
            {
                var productImage = "";
                if (prow.Product_Image != null)
                {
                    var image = prow.Product_Image.Where(w => w.Is_Main == true).FirstOrDefault();
                    if (image != null && image.Image != null)
                    {
                        productImage = Convert.ToBase64String(image.Image);
                    }
                }

                var product = new _Product()
                {
                    Company_ID = prow.Company_ID,
                    Record_Status = prow.Record_Status,
                    Branch_ID = prow.Branch_ID,
                    Brand_ID = prow.Brand_ID,
                    Description = prow.Description,
                    Discount_Price = prow.Discount_Price,
                    Extra_Info_Description = prow.Extra_Info_Description,
                    Location_L1 = prow.Location_L1,
                    Location_L2 = prow.Location_L2,
                    Location_L3 = prow.Location_L3,
                    Product_Category_L1 = prow.Product_Category_L1,
                    Product_Category_L2 = prow.Product_Category_L2,
                    Product_Category_L3 = prow.Product_Category_L3,
                    Product_Code = prow.Product_Code,
                    Product_ID = prow.Product_ID,
                    Product_Name = prow.Product_Name,
                    Reorder_Qty = prow.Reorder_Qty,
                    Sellable = prow.Sellable,
                    Selling_Price = prow.Selling_Price,
                    Type = prow.Type,
                    Update_On = DateUtil.ToDisplayDateTime(prow.Update_On),
                    Image = productImage,
                    Product_Attribute = new List<_Product_Attribute>(),
                    Product_Attribute_Map = new List<_Product_Attribute_Map>()
                };

                if (cri.QueryChild)
                {
                    if (prow.Product_Attribute != null)
                    {
                        foreach (var arow in prow.Product_Attribute)
                        {
                            var attr = new _Product_Attribute()
                            {
                                Attribute_ID = arow.Attribute_ID,
                                Attribute_Name = arow.Attribute_Name,
                                Product_ID = arow.Product_ID,
                                Product_Attribute_Value = new List<_Product_Attribute_Value>()
                            };


                            if (arow.Product_Attribute_Value != null)
                            {
                                foreach (var vrow in arow.Product_Attribute_Value)
                                {
                                    var value = new _Product_Attribute_Value()
                                    {
                                        Attribute_ID = vrow.Attribute_ID,
                                        Attribute_Value = vrow.Attribute_Value,
                                        Attribute_Value_ID = vrow.Attribute_Value_ID
                                    };
                                    ObjectUtil.BindDefault(value);
                                    attr.Product_Attribute_Value.Add(value);
                                }
                            }

                            ObjectUtil.BindDefault(attr);
                            product.Product_Attribute.Add(attr);
                        }
                    }
                    if (prow.Product_Attribute_Map != null)
                    {
                        foreach (var mrow in prow.Product_Attribute_Map)
                        {
                            var map = new _Product_Attribute_Map()
                            {
                                Attr1 = mrow.Attr1,
                                Attr2 = mrow.Attr2,
                                Attr3 = mrow.Attr3,
                                Attr4 = mrow.Attr4,
                                Attr5 = mrow.Attr5,
                                Costing_Price = mrow.Costing_Price,
                                Map_ID = mrow.Map_ID,
                                Product_ID = prow.Product_ID,
                                Record_Status = mrow.Record_Status,
                                Selling_Price = mrow.Selling_Price

                            };
                            ObjectUtil.BindDefault(map);
                            product.Product_Attribute_Map.Add(map);
                        }
                    }
                }


                ObjectUtil.BindDefault(product);
                _products.Add(product);
            }

            return _products;
        }


        protected List<_POS_Terminal> LstTerminal(Nullable<int> pCompanyID, string pMacAddress = "", Nullable<DateTime> pUpdateon = null)
        {
            var tService = new POSConfigService();
            var _terminals = new List<_POS_Terminal>();
            foreach (var row in tService.LstTerminal(pCompanyID, pMacAddress, pUpdateon))
            {
                var terminal = new _POS_Terminal()
                {
                    Branch_ID = row.Branch_ID,
                    Cashier_ID = row.Cashier_ID,
                    Company_ID = row.Company_ID,
                    Host_Name = row.Host_Name,
                    Mac_Address = row.Mac_Address,
                    Terminal_ID = row.Terminal_ID,
                    Terminal_Name = row.Terminal_Name,
                    Terminal_Local_ID = row.Terminal_Local_ID,
                    Create_By = row.Create_By,
                    Update_By = row.Update_By,
                    Create_On = DateUtil.ToDisplayDateTime(row.Create_On),
                    Update_On = DateUtil.ToDisplayDateTime(row.Update_On),
                };
                ObjectUtil.BindDefault(terminal);
                _terminals.Add(terminal);
            }
            return _terminals;
        }

        protected List<_POS_Shift> LstShift(Nullable<int> pCompanyID, string pMacAddress = "", Nullable<DateTime> pUpdateon = null)
        {
            var tService = new POSConfigService();
            var _shifts = new List<_POS_Shift>();
            foreach (var row in tService.LstTerminal(pCompanyID, pMacAddress, pUpdateon))
            {
                var shifts = tService.LstShift(pCompanyID, row.Terminal_ID, pUpdateon);
                foreach (var srow in shifts)
                {
                    var shift = new _POS_Shift()
                    {
                        Shift_ID = srow.Shift_ID,
                        Company_ID = srow.Company_ID,
                        Branch_ID = srow.Branch_ID,
                        Terminal_ID = srow.Terminal_ID,
                        Branch_Name = (srow.Branch != null ? srow.Branch.Branch_Name : ""),
                        Open_Time = DateUtil.ToDisplayDateTime(srow.Open_Time),
                        Close_Time = DateUtil.ToDisplayDateTime(srow.Close_Time),
                        Effective_Date = DateUtil.ToDisplayDateTime(srow.Effective_Date),
                        Total_Amount = srow.Total_Amount,
                        Status = srow.Status,
                        Email_Address = srow.Create_By,
                        Create_By = srow.Create_By,
                        Create_On = DateUtil.ToDisplayDate(srow.Create_On),
                        Update_By = srow.Update_By,
                        Update_On = DateUtil.ToDisplayDate(srow.Update_On),
                    };
                    ObjectUtil.BindDefault(shift);
                    _shifts.Add(shift);
                }

            }
            return _shifts;
        }
        protected _POS_Receipt GetReceipt(POS_Receipt rrow)
        {
            var shift = rrow.POS_Shift;
            if (shift == null)
                return null;

            var terminal = rrow.POS_Shift.POS_Terminal;
            if (terminal == null)
                return null;

            var branch = terminal.Branch;
            if (branch == null)
                return null;

            var rcp = new _POS_Receipt()
            {
                Cash_Payment = rrow.Cash_Payment,
                Cashier = rrow.Cashier,
                Changes = rrow.Changes,
                Company_ID = rrow.Company_ID,
                Discount = rrow.Discount,
                Discount_Type = rrow.Discount_Type,
                Net_Amount = rrow.Net_Amount,
                Payment_Type = rrow.Payment_Type,
                Promotion_ID = 0,
                Receipt_Date = DateUtil.ToDisplayDate2(rrow.Receipt_Date),
                Receipt_Time = DateUtil.ToDisplayTime(rrow.Receipt_Date),
                Receipt_ID = rrow.Receipt_ID,
                Receipt_No = rrow.Receipt_No,
                Terminal_ID = rrow.POS_Shift.Terminal_ID,
                Total_Amount = rrow.Total_Amount,
                Total_Discount = rrow.Total_Discount,
                Total_Qty = rrow.Total_Qty,
                Voucher_Amount = 0,
                Terminal = terminal.Terminal_Name,
                Branch = branch.Branch_Name,
                Status = rrow.Status,
                Remark = rrow.Remark,
                Create_By = rrow.Create_By,
                Create_On = DateUtil.ToDisplayDate(rrow.Create_On),
                Update_By = rrow.Update_By,
                Update_On = DateUtil.ToDisplayDate(rrow.Update_On),
                Customer_Name = rrow.Customer_Name,
                Contact_No = rrow.Contact_No,
                NRIC_No = rrow.NRIC_No,
                Member_ID = rrow.Member_ID,
                Service_Charge_Amount = rrow.Service_Charge,
                Service_Charge_Percen = rrow.Service_Charge_Rate,
                GST_Percen = rrow.GST_Percen,
                Total_GST_Amount = rrow.Total_GST_Amount,
                Surcharge_Amount = rrow.Surcharge_Amount,
                Surcharge_Percen = rrow.Surcharge_Percen
            };

            ObjectUtil.BindDefault(rcp);
            return rcp;
        }
        protected List<_POS_Receipt> LstReceipt(Nullable<int> pCompanyID, string pMacAddress = "", Nullable<DateTime> pUpdateon = null)
        {
            var tService = new POSConfigService();
            var pService = new POSService();
            var _receipts = new List<_POS_Receipt>();
            foreach (var row in tService.LstTerminal(pCompanyID, pMacAddress, pUpdateon))
            {
                foreach (var srow in tService.LstShift(pCompanyID, row.Terminal_ID, pUpdateon))
                {
                    var cri = new POSReciptCriteria();
                    cri.Company_ID = pCompanyID;
                    cri.Shift_ID = srow.Shift_ID;

                    foreach (var rrow in pService.LstPOSReceipt(cri))
                    {
                        var rcp = GetReceipt(rrow);
                        if (rcp != null)
                            _receipts.Add(rcp);
                    }

                }

            }
            return _receipts;
        }
    }
}