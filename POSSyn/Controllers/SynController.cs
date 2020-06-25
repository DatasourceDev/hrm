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


    public class SynController : ControllerBase
    {
        public SynController()
            : this(new UserManager<SBSModel.Models.ApplicationUser>(new UserStore<SBSModel.Models.ApplicationUser>(new SBS2DBContext())))
        {
        }

        public SynController(UserManager<SBSModel.Models.ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<SBSModel.Models.ApplicationUser> UserManager { get; private set; }

        //
        // GET: /Syn/
        [HttpPost]
        public JsonResult SynTb(List<_Ctl_Table_Syn> pTables, string pUserName, string pPassword, string pMacAddress)
        {
            User_Profile profile = null;
            if (!string.IsNullOrEmpty(pUserName) && !string.IsNullOrEmpty(pPassword))
            {
                var uService = new UserService();
                profile = uService.getUserProfile(pUserName, pPassword);
               if (profile == null)
                    return Json(new { Valid = false }, JsonRequestBehavior.AllowGet);
            }

            if (profile == null)
                return Json(new { Valid = false }, JsonRequestBehavior.AllowGet);

            if (pTables == null)
                return Json(new { Valid = false }, JsonRequestBehavior.AllowGet);

            var _companies = new List<_Company_Details>();
            var _branches = new List<_Branch>();
            var _brands = new List<_Brand>();
            var _users = new List<_User_Profile>();
            var _defs = new List<_Global_Lookup_Def>();
            var _datas = new List<_Global_Lookup_Data>();
            var _members = new List<_Member>();
            var _cats = new List<_Product_Category>();
            var _products = new List<_Product>();
            var _terminals = new List<_POS_Terminal>();
            var _shifts = new List<_POS_Shift>();
            var _receipts = new List<_POS_Receipt>();
            var _taxSurcharges = new List<_Tax_Surcharge>();

            pTables = pTables.OrderBy(o => o.Index).ToList();
            foreach (var tb in pTables)
            {
                var updateon = DateUtil.ToDate(tb.Update_On);
                if (tb.Table_Name == Syn_Table_Name.Company)
                {
                    _companies = LstCompany(profile.Company_ID, updateon);
                }
                else if (tb.Table_Name == Syn_Table_Name.Brand)
                {
                    _brands = LstBrand(profile.Company_ID, updateon);
                }
                else if (tb.Table_Name == Syn_Table_Name.Branch)
                {
                    _branches = LstBranch(profile.Company_ID, updateon);
                }
                else if (tb.Table_Name == Syn_Table_Name.User_Profile)
                {
                    _users = LstUser(profile.Company_ID, updateon);
                }
                else if (tb.Table_Name == Syn_Table_Name.Global_Lookup_Def)
                {
                    _defs = LstLookupDef(profile.Company_ID, updateon);
                }
                else if (tb.Table_Name == Syn_Table_Name.Global_Lookup_Data)
                {
                    _datas = LstLookupData(profile.Company_ID, updateon);

                }
                else if (tb.Table_Name == Syn_Table_Name.Member)
                {
                    var cri = new MemberCriteria();
                    cri.Company_ID = profile.Company_ID;
                    cri.Update_On = updateon;
                    _members = LstMember(cri);
                }
                else if (tb.Table_Name == Syn_Table_Name.Product_Category)
                {
                    var cri = new CategoryCriteria();
                    cri.Company_ID = profile.Company_ID;
                    cri.Update_On = updateon;
                    _cats = LstCatgory(cri);
                }
                else if (tb.Table_Name == Syn_Table_Name.Product)
                {
                    var cri = new ProductCriteria();
                    cri.Company_ID = profile.Company_ID;
                    cri.Update_On = updateon;
                    _products = LstProduct(cri);
                }
                else if (tb.Table_Name == Syn_Table_Name.Tax_Surcharge)
                {
                    var cri = new TaxCriteria();
                    cri.Company_ID = profile.Company_ID;
                    cri.Update_On = updateon;
                    _taxSurcharges = LstTaxSurcharge(cri);
                }
                else if (tb.Table_Name == Syn_Table_Name.POS_Terminal)
                {
                    _terminals = LstTerminal(profile.Company_ID,pMacAddress, updateon);
                }
                else if (tb.Table_Name == Syn_Table_Name.POS_Shift)
                {
                    _shifts = LstShift(profile.Company_ID, pMacAddress, updateon);
                }
                else if (tb.Table_Name == Syn_Table_Name.POS_Receipt)
                {
                    _receipts = LstReceipt(profile.Company_ID, pMacAddress, updateon);
                }
            }



            return Json(new
            {
                Valid = true,
                Company = _companies,
                Branch = _branches,
                Brand = _brands,
                User_Profile = _users,
                Lookup_Def = _defs,
                Lookup_Data = _datas,
                Member = _members,
                Category = _cats,
                Product = _products,
                POS_Terminal = _terminals,
                Tax_Surcharge = _taxSurcharges
            }, JsonRequestBehavior.AllowGet);
        }

        /*        
        delete from Inventory_Transaction
        delete from POS_Receipt_Payment
        delete from POS_Products_Rcp
        delete from POS_Receipt
        delete from POS_Shift
        delete from POS_Terminal
        */
        [HttpPost]
        public JsonResult UploadTb(List<_POS_Terminal> Terminal, List<_POS_Shift> Shift, List<_POS_Receipt> Receipt)
        {
            var currentdate = StoredProcedure.GetCurrentDate();
            var pService = new POSService();

            var _rcpResult = new List<_POS_Receipt_Result>();
            var _tResult = new List<_POS_Terminal_Result>();
            var _sResult = new List<_POS_Shift_Result>();


            var _terminals = new List<POS_Terminal>();
            var _shifts = new List<POS_Shift>();
            var _receipts = new List<POS_Receipt>();

            if (Terminal != null)
            {
                foreach (var row in Terminal)
                {
                    if (string.IsNullOrEmpty(row.Terminal_Local_ID) || row.Terminal_Local_ID == "(null)")
                        continue;

                    ObjectUtil.BindDefault(row, true);
                    var t = new POS_Terminal();
                    t.Company_ID = row.Company_ID;
                    t.Terminal_Local_ID = row.Terminal_Local_ID;
                    t.Branch_ID = row.Branch_ID;
                    t.Host_Name = row.Host_Name;
                    t.Mac_Address = row.Mac_Address;
                    t.Terminal_Name = row.Terminal_Name;
                    t.Update_By = row.Update_By;
                    t.Update_On = currentdate;

                    var tService = new POSConfigService();
                    if (row.Terminal_ID.HasValue && row.Terminal_ID.Value > 0)
                    {
                        //update
                        t.Terminal_ID = row.Terminal_ID.Value;
                        _terminals.Add(t);
                    }
                    else
                    {
                        //insert
                        _terminals.Add(t);
                    }
                }
            }

            if (Shift != null)
            {
                var cService = new POSConfigService();
                foreach (var row in Shift)
                {
                    ObjectUtil.BindDefault(row, true);
                    var s = new POS_Shift();
                    s.Open_Time = DateUtil.ToDate(row.Open_Time);
                    s.Effective_Date = DateUtil.ToDate(row.Effective_Date);
                    s.Total_Amount = row.Total_Amount;
                    s.Status = row.Status;
                    s.Terminal_ID = row.Terminal_ID;
                    s.Terminal_Local_ID = row.Terminal_Local_ID;
                    s.Shift_Local_ID = row.Shift_Local_ID;
                    s.Company_ID = row.Company_ID;
                    s.Update_By = row.Update_By;
                    s.Update_On = currentdate;
                    if (row.Shift_ID.HasValue && row.Shift_ID.Value > 0)
                    {
                        //update
                        s.Close_Time = DateUtil.ToDate(row.Close_Time);
                        s.Create_By = row.Create_By;
                        s.Create_On = currentdate;
                        s.Status = row.Status;
                        s.Total_Amount = row.Total_Amount;
                        s.Shift_ID = row.Shift_ID.Value;
                        _shifts.Add(s);

                    }
                    else
                    {
                        //insert
                        _shifts.Add(s);
                    }
                }
            }
            if (Receipt != null)
            {
                var cbService = new ComboService();
                var cardtypelist = new List<ComboViewModel>();
                Nullable<int> currComID = 0;
                foreach (var row in Receipt)
                {
                    if (currComID != row.Company_ID)
                    {
                        cardtypelist = cbService.LstLookup(ComboType.Credit_Card_Type, row.Company_ID);
                    }
                    currComID = row.Company_ID;

                    ObjectUtil.BindDefault(row, true);
                    var rcp = new POS_Receipt();
                    rcp.POS_Products_Rcp = new List<POS_Products_Rcp>();
                    rcp.POS_Receipt_Payment = new List<POS_Receipt_Payment>();

                    if (row.Receipt_ID.HasValue && row.Receipt_ID.Value > 0)
                        rcp = pService.GetPOSReceipt(row.Receipt_ID.Value);

                    rcp.Company_ID = row.Company_ID;
                    rcp.Cashier = row.Cashier_ID;
                    rcp.Cash_Payment = row.Cash_Payment;
                    rcp.Changes = row.Changes;
                    rcp.Shift_ID = row.Shift_ID;
                    rcp.Discount = row.Discount;
                    rcp.Discount_Type = row.Discount_Type;
                    rcp.Status = row.Status;
                    rcp.Receipt_Local_ID = row.Receipt_Local_ID;
                    rcp.Net_Amount = row.Net_Amount;
                    rcp.Receipt_Date = currentdate;
                    rcp.Total_Amount = row.Total_Amount;
                    rcp.Total_Discount = row.Total_Discount;
                    rcp.Total_Qty = row.Total_Qty;
                    rcp.Payment_Type = row.Payment_Type;
                    rcp.Remark = row.Remark;
                    rcp.Customer_Name = row.Customer_Name;
                    rcp.Contact_No = row.Contact_No;
                    rcp.NRIC_No = row.NRIC_No;
                    rcp.Member_ID = row.Member_ID;
                    rcp.Member_Discount = row.Member_Discount;
                    rcp.Member_Discount_Type = row.Member_Discount_Type;
                    rcp.Is_Birthday_Discount = row.Is_Birthday_Discount;
                    rcp.Receipt_Local_No = row.Receipt_Local_No;
                    rcp.Shift_Local_ID = row.Shift_Local_ID;
                    rcp.Update_By = row.Update_By;
                    rcp.Update_On = DateUtil.ToDate(row.Update_On);
                    rcp.Surcharge_Percen = row.Surcharge_Percen;
                    rcp.Surcharge_Amount = row.Surcharge_Amount;
                    rcp.Service_Charge = row.Service_Charge_Amount;
                    rcp.Service_Charge_Rate = row.Service_Charge_Percen;
                    rcp.Total_GST_Amount = row.Total_GST_Amount;
                    rcp.GST_Percen = row.GST_Percen;
                    rcp.Is_Uploaded = true;
                    rcp.Is_Latest = true;
                    var cService = new POSConfigService();
                    if (row.Status == ReceiptStatus.BackOrder)
                    {
                        rcp.Remark = row.Remark;
                        rcp.Customer_Name = row.Customer_Name;
                        rcp.Contact_No = row.Contact_No;
                        rcp.NRIC_No = row.NRIC_No;
                    }

                    if (row.Products != null)
                    {
                        foreach (var prow in row.Products)
                        {
                            ObjectUtil.BindDefault(prow, true);
                            var product = new POS_Products_Rcp();
                            product.Discount = prow.Discount;
                            product.Discount_Type = prow.Discount_Type;
                            product.GST = prow.GST;
                            product.Create_By = prow.Create_By;
                            product.Create_On = DateUtil.ToDate(prow.Create_On);
                            product.Update_By = prow.Update_By;
                            product.Update_On = DateUtil.ToDate(prow.Update_On);
                            product.ID = prow.ID;
                            product.Price = prow.Price;
                            product.Product_Color_ID = prow.Product_Color_ID;
                            product.Product_ID = prow.Product_ID;
                            product.Product_Name = prow.Product_Name;
                            product.Product_Size_ID = prow.Product_Size_ID;
                            product.Qty = prow.Qty;
                            product.Receipt_ID = prow.Receipt_ID;
                            product.Total_GST_Amount = prow.Total_GST_Amount;
                            product.Is_Uploaded = true ;
                            product.Is_Latest = true;
                            rcp.POS_Products_Rcp.Add(product);

                        }

                    }

                    if (row.Payments != null)
                    {
                        foreach (var prow in row.Payments)
                        {
                            ObjectUtil.BindDefault(prow, true);
                            var payment = new POS_Receipt_Payment();
                            if(prow.Card_Type == 1)
                            {
                               // Master
                               var cardtype = cardtypelist.Where(w => w.Text == CreditCardType.MasterCard).FirstOrDefault();
                               if (cardtype != null)
                                    payment.Card_Type = NumUtil.ParseInteger(cardtype.Value);
                            }
                            else if(prow.Card_Type == 2)
                            {
                                //Visa
                                var cardtype = cardtypelist.Where(w => w.Text == CreditCardType.Visa).FirstOrDefault();
                                if (cardtype != null)
                                    payment.Card_Type = NumUtil.ParseInteger(cardtype.Value);
                            }
                            else if (prow.Card_Type == 3)
                            {
                                //AMEX
                                var cardtype = cardtypelist.Where(w => w.Text == CreditCardType.AMEX).FirstOrDefault();
                                if (cardtype != null)
                                    payment.Card_Type = NumUtil.ParseInteger(cardtype.Value);
                            }
                            else if (prow.Card_Type == 4)
                            {
                                //Diner
                                var cardtype = cardtypelist.Where(w => w.Text == CreditCardType.DinersClub).FirstOrDefault();
                                if (cardtype != null)
                                    payment.Card_Type = NumUtil.ParseInteger(cardtype.Value);
                            }
                            else if (prow.Card_Type == 5)
                            {
                                //JCB
                                var cardtype = cardtypelist.Where(w => w.Text == CreditCardType.JCB).FirstOrDefault();
                                if (cardtype != null)
                                    payment.Card_Type = NumUtil.ParseInteger(cardtype.Value);
                            }
                            else
                            {
                                payment.Card_Type = null;
                            }
                            payment.Approval_Code = prow.Approval_Code;
                            payment.Card_Branch = prow.Card_Branch;
                            payment.Payment_Amount = prow.Payment_Amount;
                            payment.Payment_Type = prow.Payment_Type;
                            payment.Receipt_ID = prow.Receipt_ID;
                            payment.Receipt_Payment_ID = prow.Receipt_Payment_ID;
                            payment.Surcharge_Amount = prow.Surcharge_Amount;
                            payment.Surcharge_Percen = prow.Surcharge_Percen;
                            payment.Is_Uploaded = true;
                            payment.Is_Latest = true;
                            rcp.POS_Receipt_Payment.Add(payment);
                        }
                    }

                    if (row.Status == ReceiptStatus.Hold)
                    {
                        rcp.User_Profile = null;
                        rcp.Create_By = row.Create_By;
                        rcp.Create_On =  DateUtil.ToDate(row.Create_On);
                        _receipts.Add(rcp);
                    }
                    else
                    {
                        if (row.Receipt_ID.HasValue && row.Receipt_ID.Value > 0)
                        {
                            //update
                            if (rcp != null)
                            {
                                rcp.Receipt_ID = row.Receipt_ID.Value;
                                rcp.POS_Receipt_Payment = null;
                                rcp.POS_Products_Rcp = null;
                                _receipts.Add(rcp);
                            }
                        }
                        else
                        {
                            //insert
                            rcp.Create_By = row.Create_By;
                            rcp.Create_On = DateUtil.ToDate(row.Create_On);
                            _receipts.Add(rcp);
                        }
                    }

                }
            }

            var synService = new POSMasterSyncService();
            var sresult = synService.POSSyn(_terminals, _shifts, _receipts);
            if (sresult.Code == ERROR_CODE.SUCCESS)
            {
                return Json(new
                {
                    Valid = true,
                    Receipt = sresult.RcpResult,
                    Terminal = sresult.TerminalResult,
                    Shift = sresult.ShiftResult,
                }, JsonRequestBehavior.AllowGet);
            }


            return Json(new { Valid = false }, JsonRequestBehavior.AllowGet);
        }



        public JsonResult ClearTb(string pUserName, string pPassword, string pMacAddress)
        {
            User_Profile profile = null;
            if (!string.IsNullOrEmpty(pUserName) && !string.IsNullOrEmpty(pPassword))
            {
                var uService = new UserService();
                profile = uService.getUserProfile(pUserName, pPassword);
                if (profile == null)
                    return Json(new { Valid = false }, JsonRequestBehavior.AllowGet);
            }

            if (profile == null)
                return Json(new { Valid = false }, JsonRequestBehavior.AllowGet);

            var synService = new POSMasterSyncService();
            var result = synService.POSClear(pMacAddress);
            if (result.Code == ERROR_CODE.SUCCESS)
            {
                return Json(new { Valid = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Valid = false }, JsonRequestBehavior.AllowGet);
        }



    }
}