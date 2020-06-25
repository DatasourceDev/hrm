using POSSyn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SBSModel.Models;
using SBSModel.Common;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace POSSyn.Controllers
{
    public class POSMobileController : ControllerBase
    {
        public POSMobileController()
            : this(new UserManager<SBSModel.Models.ApplicationUser>(new UserStore<SBSModel.Models.ApplicationUser>(new SBS2DBContext())))
        {
        }

        public POSMobileController(UserManager<SBSModel.Models.ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<SBSModel.Models.ApplicationUser> UserManager { get; private set; }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetDate()
        {
            var date = DateUtil.ToDisplayDate(StoredProcedure.GetCurrentDate());
            return Json(new { Date = date }, JsonRequestBehavior.AllowGet);
        }
        //[HttpPost]
        //[AllowAnonymous]
        //public JsonResult MobileLogin(string pUserName, string pPassword)
        //{

        //    if (string.IsNullOrEmpty(pUserName) || string.IsNullOrEmpty(pPassword))
        //        return Json(new { Valid = false }, JsonRequestBehavior.AllowGet);

        //    var uService = new UserService();
        //    var profile = uService.getUserProfile(pUserName, pPassword);
        //    if (profile != null)
        //    {
        //        //if (userService.getSubscription(profile.Company_ID.Value, Module.POS, "/POS/POS"))
        //        //{
        //        var Profile_Photo = "";
        //        if (profile.User_Profile_Photo != null && profile.User_Profile_Photo.FirstOrDefault() != null)
        //        {
        //            var base64 = Convert.ToBase64String(profile.User_Profile_Photo.FirstOrDefault().Photo);
        //            Profile_Photo = String.Format("data:image/gif;base64,{0}", base64);
        //        }
        //        var receiptConfig = new ReceiptConfigService().GetReceiptConfigByCompany(profile.Company_ID.Value);
        //        var terminal = new POSService().GetTerminal(profile.Profile_ID);
        //        var company = new CompanyService().GetCompany(profile.Company_ID);
        //        if (company != null)
        //        {
        //            var tbPrefix = "";
        //            var tbNo = 0;
        //            if (company.Business_Type == BusinessType.FoodAndBeverage)
        //            {
        //                var tableconf = new POSService().GetProductTable(company.Company_ID);
        //                if (tableconf != null)
        //                {
        //                    tbPrefix = tableconf.Prefix;
        //                    tbNo = tableconf.No_Of_Table.HasValue ? tableconf.No_Of_Table.Value : 0;
        //                }
        //            }

        //            var obj = new
        //            {
        //                Profile_ID = profile.Profile_ID,
        //                Email_Address = pUserName,
        //                Name = profile.Name, // Cashier Name
        //                Profile_Photo = Profile_Photo,
        //                Company_ID = profile.Company_ID,
        //                Company_Name = company.Name,
        //                Address = company.Address,
        //                Fax = company.Fax,
        //                Country = company.Country != null ? company.Country.Description : "",
        //                State = company.State != null ? company.State.Descrition : "",
        //                Zip_Code = company.Zip_Code,
        //                WebSite = company.Website,
        //                Phone = company.Phone,
        //                Registry = company.Registry,
        //                GST_Registration = company.GST_Registration,
        //                Terminal_ID = terminal != null ? terminal.Terminal_ID : 0,
        //                Terminal_Local_ID = terminal != null ? terminal.Terminal_Local_ID : "",
        //                Terminal = terminal != null ? terminal.Terminal_Name : "",
        //                Branch = terminal != null ? (terminal.Branch != null ? terminal.Branch.Branch_Name : "") : "",
        //                Branch_ID = terminal != null ? (terminal.Branch != null ? terminal.Branch.Branch_ID : 0) : 0,
        //                Receipt_Header = receiptConfig != null ? receiptConfig.Receipt_Header : "",
        //                Receipt_Footer = receiptConfig != null ? receiptConfig.Receipt_Footer : "",
        //                Surcharge_Include = receiptConfig != null ? receiptConfig.Surcharge_Include.HasValue ? receiptConfig.Surcharge_Include.Value : false : false,
        //                Surcharge_Percen = receiptConfig != null ? receiptConfig.Surcharge_Percen.HasValue ? receiptConfig.Surcharge_Percen.Value : 0 : 0,
        //                Currency_Code = company.Currency != null ? company.Currency.Currency_Code : "",
        //                Business_Type = company.Business_Type,
        //                Table_Frefix = tbPrefix,
        //                No_Of_Table = tbNo,
        //                Valid = true

        //            };
        //            //ObjectUtil.BindDefault(obj);

        //            return Json(obj, JsonRequestBehavior.AllowGet);
        //        }

        //        //}



        //    }
        //    return Json(new { Valid = false }, JsonRequestBehavior.AllowGet);
        //}

        #region Common
        [AllowAnonymous]
        [HttpPost]
        public JsonResult GetCurrentDate()
        {
            var currentdate = StoredProcedure.GetCurrentDate();

            return Json(new
            {
                Valid = false,
                Current_Date = DateUtil.ToDisplayDate(currentdate),
                Current_Datetime = DateUtil.ToDisplayDateTime(currentdate),
                Current_Time = DateUtil.ToDisplayTime(currentdate)
            }, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region Branch
        [AllowAnonymous]
        [HttpPost]
        public JsonResult LstBranch(int pCompanyID)
        {
            return Json(new
            {
                Branch = base.LstBranch(pCompanyID)
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Category
        //
        // GET: /PosMobile/
        //[HttpPost]
        [AllowAnonymous]
        [HttpPost]
        public JsonResult LstCategory(int pCompanyID, string pCategoryName)
        {
            var cri = new CategoryCriteria();
            cri.Company_ID = pCompanyID;
            cri.Category_Name = pCategoryName;


            return Json(new
            {
                Category = base.LstCatgory(cri)
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Product
        //
        // GET: /PosMobile/
        [HttpPost]
        [AllowAnonymous]
        public JsonResult LstProduct(int pCompanyID, string pCategoryID, string pProductName)
        {
            var iService = new InventoryService();

            List<Product> plst = new List<Product>();


            Nullable<int> catID = null;
            if (!string.IsNullOrEmpty(pCategoryID)) catID = NumUtil.ParseInteger(pCategoryID);

            var pcri = new ProductCriteria()
            {
                Company_ID = pCompanyID,
                Category_ID = catID,
                Product_Name = pProductName,
                QueryChild = false
            };

            var _products = base.LstProduct(pcri);

            return Json(new
            {
                Product = _products
            }, JsonRequestBehavior.AllowGet);
        }




        //
        // GET: /PosMobile/
        //[HttpPost]
        //[AllowAnonymous]
        //public JsonResult LstProductProperty(int pProductID)
        //{
        //    var iService = new InventoryService();
        //    var colors = iService.LstProductColor(pProductID).Select(c => new Product_Color { Product_ID = c.Product_ID, Product_Color_ID = c.Product_Color_ID, Color = c.Color, Color_Code = c.Color_Code });
        //    var sizes = iService.LstProductSize(pProductID).Select(s => new Product_Size { Product_ID = s.Product_ID, Product_Size_ID = s.Product_Size_ID, Size = s.Size });

        //    return Json(new
        //    {
        //        Color = colors.ToArray(),
        //        Size = sizes.ToArray()
        //    }, JsonRequestBehavior.AllowGet);
        //}



        //
        // GET: /PosMobile/
        //[HttpPost]
        [AllowAnonymous]
        [HttpPost]
        public JsonResult LstCreditCardType(int pCompanyID)
        {
            var pService = new POSService();
            var cbService = new ComboService();

            List<ComboViewModel> clst = cbService.LstLookup(ComboType.Credit_Card_Type, pCompanyID, false);


            return Json(new
            {
                CreditCardType = clst
            }, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /PosMobile/
        //[HttpPost]
        [AllowAnonymous]
        [HttpPost]
        public JsonResult LstBankname(int pCompanyID)
        {
            var pService = new POSService();
            var cbService = new ComboService();

            List<ComboViewModel> clst = cbService.LstLookup(ComboType.POS_Bank_Name, pCompanyID, false);

            return Json(new
            {
                BankName = clst
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Terminal

        //
        // GET: /PosMobile/
        //[HttpPost]
        [AllowAnonymous]
        [HttpPost]
        public JsonResult GetTerminal(int pCashierID)
        {
            var tService = new POSConfigService();
            var t = tService.GetTerminalByCashierID(pCashierID);

            if (t != null)
            {
                return Json(new
                {
                    Terminal_ID = t.Terminal_ID,
                    Terminal_Name = t.Terminal_Name,
                    Branch_ID = t.Branch_ID,
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Valid = false }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult SaveTerminal(_POS_Terminal model)
        {
            var currentdate = StoredProcedure.GetCurrentDate();
            var t = new POS_Terminal();
            t.Company_ID = model.Company_ID;
            t.Cashier_ID = model.Cashier_ID;

            t.Branch_ID = model.Branch_ID;
            t.Host_Name = model.Host_Name;
            t.Mac_Address = model.Mac_Address;
            t.Terminal_Name = model.Terminal_Name;

            var tService = new POSConfigService();

            if (model.Terminal_ID.HasValue && model.Terminal_ID.Value > 0)
            {
                //update
                t.Create_By = model.Create_By;
                t.Create_On = currentdate;
                t.Update_By = model.Update_By;
                t.Update_On = currentdate;
                t.Terminal_ID = model.Terminal_ID.Value;
                var result = tService.UpdateTerminal(t);
                return Json(new
                {
                    result = result.Code,
                    Terminal_ID = t.Terminal_ID,
                    Msg = result.Msg,
                    Field = result.Field,
                    Valid = true,
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //insert
                t.Create_By = model.Create_By;
                t.Create_On = currentdate;
                var result = tService.InsertTerminal(t);
                return Json(new
                {
                    result = result.Code,
                    Terminal_ID = t.Terminal_ID,
                    Msg = result.Msg,
                    Field = result.Field,
                    Valid = true,
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Receipt

        [AllowAnonymous]
        [HttpPost]
        public JsonResult GetReceipt(int pReceiptID)
        {
            var pService = new POSService();

            var row = pService.GetPOSReceipt(pReceiptID);
            if (row != null)
            {
                var _products = new List<_POS_Products_Rcp>();

                if (row.POS_Products_Rcp != null)
                {
                    foreach (var prow in row.POS_Products_Rcp)
                    {
                        var product = new _POS_Products_Rcp()
                        {
                            ID = prow.ID,
                            Receipt_ID = prow.Receipt_ID,
                            Price = prow.Price,
                            Qty = prow.Qty,
                            Product_Color_ID = prow.Product_Color_ID,
                            Product_Size_ID = prow.Product_Size_ID,
                            Product_Color = prow.Product_Color != null ? prow.Product_Color.Color : "",
                            Product_Size = prow.Product_Size != null ? prow.Product_Size.Size : "",
                            Product_ID = prow.Product_ID.HasValue ? prow.Product_ID.Value : 0,
                            Product_Name = prow.Product_Color != null && prow.Product_Size != null ? (prow.Product.Product_Name + "\ncolour: " + prow.Product_Color.Color + " size: " + prow.Product_Size.Size) :
                            prow.Product_Color != null ? (prow.Product.Product_Name + "\ncolour: " + prow.Product_Color.Color) :
                            prow.Product_Size != null ? (prow.Product.Product_Name + "\nsize: " + prow.Product_Size.Size) : prow.Product.Product_Name,
                            Product_Code = prow.Product.Product_Code,
                            Discount = prow.Discount,
                            Discount_Type = prow.Discount_Type,
                        };
                        ObjectUtil.BindDefault(product);
                        _products.Add(product);
                    }
                }

                var _payments = new List<_POS_Receipt_Payment>();
                if (row.POS_Receipt_Payment != null)
                {
                    foreach (var prow in row.POS_Receipt_Payment)
                    {
                        var pm = new _POS_Receipt_Payment()
                        {
                            Receipt_Payment_ID = prow.Receipt_Payment_ID,
                            Receipt_ID = prow.Receipt_ID,
                            Payment_Type = prow.Payment_Type,
                            Payment_Amount = prow.Payment_Amount,
                            Approval_Code = prow.Approval_Code,
                            Card_Branch = prow.Card_Branch,
                            Card_Type = prow.Card_Type,
                        };
                        ObjectUtil.BindDefault(pm);
                        _payments.Add(pm);
                    }
                }



                return Json(ObjectUtil.BindDefault(new
                {
                    Approval_Code = "",
                    Card_Branch = 0,
                    Card_Branch_Name = "",
                    Card_Type = 0,
                    Card_Type_Name = "",
                    Cash_Payment = row.Cash_Payment,
                    Cashier = row.Cashier,
                    Changes = row.Changes,
                    Company_ID = row.Company_ID,
                    Discount = row.Discount,
                    Discount_Reason = "",
                    Discount_Type = row.Discount_Type,
                    Net_Amount = row.Net_Amount,
                    Payment_Type = row.Payment_Type,
                    Promotion_ID = 0,
                    Receipt_Date = DateUtil.ToDisplayDate2(row.Receipt_Date),
                    Receipt_Time = DateUtil.ToDisplayTime(row.Receipt_Date),
                    Receipt_ID = row.Receipt_ID,
                    Receipt_No = row.Receipt_No,
                    Terminal_ID = row.POS_Shift.Terminal_ID,
                    Total_Amount = row.Total_Amount,
                    Total_Discount = row.Total_Discount,
                    Total_Qty = row.Total_Qty,
                    Voucher_Amount = 0,
                    Terminal = row.POS_Shift.POS_Terminal != null ? row.POS_Shift.POS_Terminal.Terminal_Name : "",
                    Branch = row.POS_Shift.POS_Terminal != null ? row.POS_Shift.POS_Terminal.Branch != null ? row.POS_Shift.POS_Terminal.Branch.Branch_Name : "" : "",
                    Status = row.Status,
                    Remark = row.Remark,
                    Create_By = row.Create_By,
                    Create_On = DateUtil.ToDisplayDate(row.Create_On),
                    Update_By = row.Update_By,
                    Update_On = DateUtil.ToDisplayDate(row.Update_On),
                    Customer_Name = row.Customer_Name,
                    Contact_No = row.Contact_No,
                    NRIC_No = row.NRIC_No,
                    Member_ID = row.Member_ID,
                    Product = _products,
                    Payments = _payments,
                    Valid = true,
                }), JsonRequestBehavior.AllowGet);
            }
            return Json(new { Valid = false }, JsonRequestBehavior.AllowGet);
        }


        [AllowAnonymous]
        [HttpPost]
        public JsonResult LstReceipt(int pCompanyID, int pCashierID, string pStartDate, string pEndDate, string pStatus = "")
        {
            var pService = new POSService();
            var currentdate = StoredProcedure.GetCurrentDate();
            Nullable<DateTime> sDate = currentdate;
            Nullable<DateTime> eDate = currentdate;
            if (!string.IsNullOrEmpty(pStartDate))
                sDate = DateUtil.ToDate(pStartDate, "-");

            if (!string.IsNullOrEmpty(pEndDate))
                eDate = DateUtil.ToDate(pEndDate, "-");

            User_Profile userlogin = new UserService().getUser(pCashierID);

            var posTerminal = pService.GetTerminal(pCashierID);

            var cri = new POSReciptCriteria();
            cri.Company_ID = pCompanyID;
            cri.Cashier_ID = pCashierID;
            cri.Start_Date = sDate;
            cri.Status = pStatus;
            cri.End_Date = eDate;
            cri.Branch_ID = posTerminal.Branch_ID;
            cri.User_Authentication_ID = userlogin.User_Authentication_ID;

            var _rcps = new List<_POS_Receipt>();
            var rcps = pService.LstPOSReceipt(cri);
            foreach (var row in rcps)
            {
                var rcp = new _POS_Receipt()
                {
                    Approval_Code = "",
                    Card_Branch = 0,
                    Card_Branch_Name = "",
                    Card_Type = 0,
                    Card_Type_Name = "",
                    Cash_Payment = row.Cash_Payment,
                    Cashier = row.Cashier,
                    Changes = row.Changes,
                    Company_ID = row.Company_ID,
                    Discount = row.Discount,
                    Discount_Reason = "",
                    Discount_Type = row.Discount_Type,
                    Net_Amount = row.Net_Amount,
                    Payment_Type = row.Payment_Type,
                    Promotion_ID = 0,
                    Receipt_Date = DateUtil.ToDisplayDate2(row.Receipt_Date),
                    Receipt_Time = DateUtil.ToDisplayTime(row.Receipt_Date),
                    Receipt_ID = row.Receipt_ID,
                    Receipt_No = row.Receipt_No,
                    Terminal_ID = row.POS_Shift.Terminal_ID,
                    Total_Amount = row.Total_Amount,
                    Total_Discount = row.Total_Discount,
                    Total_Qty = row.Total_Qty,
                    Voucher_Amount = 0,
                    Terminal = row.POS_Shift.POS_Terminal != null ? row.POS_Shift.POS_Terminal.Terminal_Name : "",
                    Branch = row.POS_Shift.POS_Terminal != null ? row.POS_Shift.POS_Terminal.Branch != null ? row.POS_Shift.POS_Terminal.Branch.Branch_Name : "" : "",
                    Status = row.Status,
                    Remark = row.Remark,
                    Create_By = row.Create_By,
                    Create_On = DateUtil.ToDisplayDate(row.Create_On),
                    Update_By = row.Update_By,
                    Update_On = DateUtil.ToDisplayDate(row.Update_On),
                    Customer_Name = row.Customer_Name,
                    Contact_No = row.Contact_No,
                    NRIC_No = row.NRIC_No,
                    Member_ID = row.Member_ID,
                    Service_Charge_Amount = row.Service_Charge,
                    Service_Charge_Percen = row.Service_Charge_Rate,
                    GST_Percen = row.GST_Percen,
                    Total_GST_Amount = row.Total_GST_Amount,
                    Surcharge_Amount = row.Surcharge_Amount,
                    Surcharge_Percen = row.Surcharge_Percen
                };

                ObjectUtil.BindDefault(rcp);
                _rcps.Add(rcp);
            }


            return Json(new
            {
                Receipt = _rcps,
                Valid = true,
            }, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        [AllowAnonymous]
        public JsonResult SaveReceipt(_POS_Receipt model)
        {
            var currentdate = StoredProcedure.GetCurrentDate();
            var pService = new POSService();

            POS_Receipt rcp = new POS_Receipt();
            rcp.POS_Products_Rcp = new List<POS_Products_Rcp>();
            rcp.POS_Receipt_Payment = new List<POS_Receipt_Payment>();

            if (model.Receipt_ID.HasValue && model.Receipt_ID.Value > 0)
                rcp = pService.GetPOSReceipt(model.Receipt_ID.Value);

            rcp.Company_ID = model.Company_ID;
            rcp.Cashier = model.Profile_ID;
            rcp.Cash_Payment = model.Cash_Payment;
            rcp.Changes = model.Changes;

            rcp.Discount = model.Discount;
            rcp.Discount_Type = model.Discount_Type;
            rcp.Status = model.Status;

            rcp.Net_Amount = model.Net_Amount;
            rcp.Receipt_Date = currentdate;
            rcp.Total_Amount = model.Total_Amount;
            rcp.Total_Discount = model.Total_Discount;
            rcp.Total_Qty = model.Total_Qty;
            rcp.Payment_Type = model.Payment_Type;
            rcp.Remark = "";
            rcp.Customer_Name = model.Customer_Name;
            rcp.Contact_No = model.Contact_No;
            rcp.NRIC_No = model.NRIC_No;
            rcp.Member_ID = model.Member_ID;
            rcp.Member_Discount = model.Member_Discount;
            rcp.Member_Discount_Type = model.Member_Discount_Type;
            rcp.Is_Birthday_Discount = model.Is_Birthday_Discount;
            rcp.Surcharge_Amount = model.Surcharge_Amount;
            rcp.Surcharge_Percen = model.Surcharge_Percen;
            rcp.Total_GST_Amount = model.Total_GST_Amount;
            rcp.Service_Charge = model.Service_Charge_Amount;
            rcp.Service_Charge_Rate = model.Service_Charge_Percen;
            rcp.GST_Percen = model.GST_Percen;


            if (model.Member_ID.HasValue && model.Member_ID.Value == 0) rcp.Member_ID = null;

            if (model.Status == ReceiptStatus.BackOrder)
            {
                rcp.Remark = model.Remark;
                rcp.Customer_Name = model.Customer_Name;
                rcp.Contact_No = model.Contact_No;
                rcp.NRIC_No = model.NRIC_No;
            }

            if (model.Products != null)
            {
                foreach (var prow in model.Products)
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
                    rcp.POS_Products_Rcp.Add(product);
                }

            }

            if (model.Payments != null)
            {
                foreach (var prow in model.Payments)
                {
                    ObjectUtil.BindDefault(prow, true);
                    var payment = new POS_Receipt_Payment();
                    payment.Approval_Code = prow.Approval_Code;
                    payment.Card_Branch = prow.Card_Branch;
                    payment.Card_Type = prow.Card_Type;
                    payment.Payment_Amount = prow.Payment_Amount;
                    payment.Payment_Type = prow.Payment_Type;
                    payment.Receipt_ID = prow.Receipt_ID;
                    payment.Receipt_Payment_ID = prow.Receipt_Payment_ID;
                    payment.Surcharge_Amount = prow.Surcharge_Amount;
                    payment.Surcharge_Percen = prow.Surcharge_Percen;
                    rcp.POS_Receipt_Payment.Add(payment);
                }
            }

            if (model.Status == ReceiptStatus.Hold)
            {
                rcp.User_Profile = null;

                var result = pService.InsertPOSHoldBillReceipt(rcp);
                return Json(new
                {
                    result = result.Code,
                    Receipt_No = rcp.Receipt_No != null ? rcp.Receipt_No : "",
                    Receipt_Date = DateUtil.ToDisplayDate2(rcp.Receipt_Date),
                    Receipt_Time = DateUtil.ToDisplayTime(rcp.Receipt_Date),
                    Receipt_ID = rcp.Receipt_ID,
                    Status = rcp.Status,
                    Msg = result.Msg,
                    Field = result.Field,
                    Valid = true,
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (model.Receipt_ID.HasValue && model.Receipt_ID.Value > 0)
                {
                    //update
                    if (rcp != null)
                    {
                        rcp.Receipt_ID = model.Receipt_ID.Value;
                        //rcp.POS_Receipt_Payment = null;
                        //rcp.POS_Products_Rcp = null;

                        var result = pService.UpdatePOSReceipt(rcp);
                        return Json(new
                        {
                            result = result.Code,
                            Receipt_No = rcp.Receipt_No != null ? rcp.Receipt_No : "",
                            Receipt_Date = DateUtil.ToDisplayDate2(rcp.Receipt_Date),
                            Receipt_Time = DateUtil.ToDisplayTime(rcp.Receipt_Date),
                            Receipt_ID = rcp.Receipt_ID,
                            Status = rcp.Status,
                            Msg = result.Msg,
                            Field = result.Field,
                            Valid = true,
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    //insert


                    var result = pService.InsertPOSReceipt(rcp);
                    return Json(new
                    {
                        result = result.Code,
                        Receipt_No = rcp.Receipt_No != null ? rcp.Receipt_No : "",
                        Receipt_Date = DateUtil.ToDisplayDate2(rcp.Receipt_Date),
                        Receipt_Time = DateUtil.ToDisplayTime(rcp.Receipt_Date),
                        Receipt_ID = rcp.Receipt_ID,
                        Member_ID = rcp.Member_ID.HasValue ? rcp.Member_ID.Value : 0,
                        Status = rcp.Status,
                        Msg = result.Msg,
                        Field = result.Field,
                        Valid = true,
                    }, JsonRequestBehavior.AllowGet);

                }
            }
            return Json(new { Valid = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult RemoveHoldBill(Nullable<int> pReceiptID)
        {
            var pService = new POSService();
            var result = pService.DeletePOSReceipt(pReceiptID);
            if (result.Code == ERROR_CODE.SUCCESS)
            {
                return Json(new
                {
                    result = result.Code,
                    Msg = result.Msg,
                    Field = result.Field,
                    Valid = true,
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Valid = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult ChangeReceiptStatus(int pReceiptID, string pStatus)
        {

            var pService = new POSService();

            if (pReceiptID > 0)
            {
                POS_Receipt row = pService.GetPOSReceipt(pReceiptID);
                if (row == null)
                {
                    return Json(new
                    {
                        result = ERROR_CODE.ERROR_511_DATA_NOT_FOUND,
                        Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND)
                    }, JsonRequestBehavior.AllowGet);
                }

                var rcpProduct = row.POS_Products_Rcp;
                row.Status = pStatus;
                var result = pService.UpdatePOSReceipt(row);
                row.POS_Products_Rcp = rcpProduct;

                if (result.Code == ERROR_CODE.SUCCESS)
                {
                    return GetReceipt(row.Receipt_ID);
                }


                return Json(new
                {
                    result = result.Code,
                    Receipt_No = row.Receipt_No,
                    Receipt_Date = DateUtil.ToDisplayDate(row.Receipt_Date),
                    Receipt_Time = DateUtil.ToDisplayTime(row.Receipt_Date),
                    Receipt_ID = row.Receipt_ID,
                    Msg = result.Msg,
                    Field = result.Field
                }, JsonRequestBehavior.AllowGet);


            }

            return Json(new { Valid = false }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Member

        // Added by Jane 07-08-2015
        [AllowAnonymous]
        [HttpPost]
        public JsonResult LstMember(int pCompanyID, string pMemberName)
        {
            var cri = new MemberCriteria();
            cri.Company_ID = pCompanyID;
            cri.Member_Name = pMemberName;

            return Json(new
            {
                Members = base.LstMember(cri),
                Valid = true
            }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult GetMember(int pMemberID)
        {
            var cri = new MemberCriteria();
            cri.Member_ID = pMemberID;

            var members = base.LstMember(cri);
            if (members.Count() > 0)
            {
                var m = members[0];
                ObjectUtil.BindDefault(m);
                return Json(new
                {
                    Member_ID = m.Member_ID,
                    Company_ID = m.Company_ID,
                    Member_Card_No = m.Member_Card_No,
                    Member_Name = m.Member_Name,
                    NRIC_No = m.NRIC_No,
                    Phone_No = m.Phone_No,
                    Email = m.Email,
                    DOB = m.DOB,
                    Member_Status = m.Member_Status,
                    Create_By = m.Create_By,
                    Create_On = m.Create_On,
                    Update_By = m.Update_By,
                    Update_On = m.Update_On,
                    Credit = m.Credit,
                    Valid = true
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Valid = false }, JsonRequestBehavior.AllowGet);
        }





        [HttpPost]
        [AllowAnonymous]
        public JsonResult InsertMember(_Member model)
        {
            var currentdate = StoredProcedure.GetCurrentDate();
            var mService = new MemberService();

            var m = new Member();

            m.Company_ID = model.Company_ID;
            m.Member_Card_No = model.Member_Card_No;
            m.Member_Name = model.Member_Name;
            m.NRIC_No = model.NRIC_No;
            m.Phone_No = model.Phone_No;
            m.Email = model.Email;
            m.DOB = DateUtil.ToDate(model.DOB);
            m.Member_Status = model.Member_Status;
            m.Create_By = model.Create_By;
            m.Create_On = currentdate;
            m.Update_By = model.Update_By;
            m.Update_On = currentdate;
            m.Credit = model.Credit;

            var result = mService.InsertMember(m);
            if (result.Code == ERROR_CODE.SUCCESS)
            {
                return Json(new
                {
                    result = result.Code,
                    Member_ID = m.Member_ID,
                    Member_Card_No = m.Member_Card_No,
                    Msg = result.Msg,
                    Field = result.Field,
                    Valid = true
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Valid = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult UpdateMember(_Member model)
        {
            var currentdate = StoredProcedure.GetCurrentDate();
            var mService = new MemberService();

            var m = mService.GetMember(model.Member_ID);
            if (m != null)
            {
                m.Company_ID = model.Company_ID;
                m.Member_Name = model.Member_Name;
                m.NRIC_No = model.NRIC_No;
                m.Phone_No = model.Phone_No;
                m.Email = model.Email;
                m.DOB = DateUtil.ToDate(model.DOB);
                m.Member_Status = model.Member_Status;
                m.Member_Discount = model.Member_Discount;
                m.Member_Discount_Type = model.Member_Discount_Type;
                m.Update_By = model.Update_By;
                m.Update_On = currentdate;
                m.Credit = model.Credit;

                var result = mService.UpdateMember(m);
                if (result.Code == ERROR_CODE.SUCCESS)
                {
                    return Json(new
                    {
                        result = result.Code,
                        Msg = result.Msg,
                        Field = result.Field,
                        Valid = true
                    }, JsonRequestBehavior.AllowGet);
                }
            }



            return Json(new { Valid = false }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult GetMemberConfiguration(int pCompanyID)
        {
            var mcService = new MemberConfigService();
            var memconf = mcService.GetMemberConfig(pCompanyID);
            if (memconf != null)
            {
                ObjectUtil.BindDefault(memconf);
                return Json(new
                {
                    Company_ID = memconf.Company_ID,
                    Member_Configuration_ID = memconf.Member_Configuration_ID,
                    Member_Discount = memconf.Member_Discount,
                    Member_Discount_Type = memconf.Member_Discount_Type,
                    Birthday_Discount = memconf.Birthday_Discount,
                    Birthday_Discount_Type = memconf.Birthday_Discount_Type,
                    Valid = true
                }, JsonRequestBehavior.AllowGet);

            }
            return Json(new { Valid = false }, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region Shift

        // Added by Jane 16-08-2015
        [AllowAnonymous]
        [HttpPost]
        public JsonResult LstShift(Nullable<int> pCompanyID, Nullable<int> pBranchID, Nullable<int> pTerminalID)
        {
            var pService = new POSService();
            var slst = new List<_POS_Shift>();

            foreach (var s in pService.LstPOSShift(pCompanyID, pBranchID, pTerminalID))
            {
                var row = new _POS_Shift()
                {
                    Shift_ID = s.Shift_ID,
                    Company_ID = s.Company_ID,
                    Branch_ID = s.Branch_ID,
                    Terminal_ID = s.Terminal_ID,
                    Branch_Name = (s.Branch != null ? s.Branch.Branch_Name : ""),
                    Open_Time = DateUtil.ToDisplayDateTime(s.Open_Time),
                    Close_Time = DateUtil.ToDisplayDateTime(s.Close_Time),
                    Effective_Date = DateUtil.ToDisplayDateTime(s.Effective_Date),
                    Total_Amount = s.Total_Amount,
                    Status = s.Status,
                    Email_Address = s.Create_By,
                    Create_By = s.Create_By,
                    Create_On = DateUtil.ToDisplayDate(s.Create_On),
                    Update_By = s.Update_By,
                    Update_On = DateUtil.ToDisplayDate(s.Update_On),
                };

                ObjectUtil.BindDefault(row);
                slst.Add(row);
            }
            return Json(new
            {
                Shifts = slst,
                Valid = true
            }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult GetCurrentOpenShift(Nullable<int> pCompanyID, Nullable<int> pTerminalID)
        {
            var pService = new POSService();

            var s = pService.GetCurrentOpenShift(pCompanyID, pTerminalID);

            if (s != null)
            {
                ObjectUtil.BindDefault(s);
                return Json(new
                {
                    Shift_ID = s.Shift_ID,
                    Company_ID = s.Company_ID,
                    Branch_ID = s.Branch_ID,
                    Branch_Name = (s.Branch != null ? s.Branch.Branch_Name : ""),
                    Open_Time = DateUtil.ToDisplayDateTime(s.Open_Time),
                    Close_Time = DateUtil.ToDisplayDateTime(s.Close_Time),
                    Effective_Date = DateUtil.ToDisplayDateTime(s.Effective_Date),
                    Total_Amount = s.Total_Amount,
                    Status = s.Status,
                    Email_Address = s.Create_By,
                    Create_By = s.Create_By,
                    Create_On = DateUtil.ToDisplayDate(s.Create_On),
                    Update_By = s.Update_By,
                    Update_On = DateUtil.ToDisplayDate(s.Update_On),
                    Valid = true
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Valid = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult OpenShift(_POS_Shift model)
        {
            var currentdate = StoredProcedure.GetCurrentDate();
            var pService = new POSService();
            var cService = new POSConfigService();
            var s = new POS_Shift();


            s.Company_ID = model.Company_ID;
            s.Branch_ID = model.Branch_ID;
            s.Open_Time = currentdate;
            s.Effective_Date = currentdate;
            s.Total_Amount = model.Total_Amount;
            s.Status = ShiftStatus.Open;
            s.Create_By = model.Create_By;
            s.Create_On = currentdate;
            s.Terminal_ID = model.Terminal_ID;

            var result = cService.InsertShift(s);
            if (result.Code == ERROR_CODE.SUCCESS)
            {
                return Json(new
                {
                    result = result.Code,
                    Member_ID = s.Shift_ID,
                    Msg = result.Msg,
                    Field = result.Field,
                    Valid = true
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Valid = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult CloseShift(_POS_Shift model)
        {
            var currentdate = StoredProcedure.GetCurrentDate();
            var pService = new POSService();
            var cService = new POSConfigService();

            var s = pService.GetShift(model.Shift_ID);
            if (s != null)
            {
                s.Close_Time = currentdate;
                s.Total_Amount = model.Total_Amount;
                s.Status = ShiftStatus.Close;

                s.Update_By = model.Update_By;
                s.Update_On = currentdate;

                var result = cService.UpdateShift(s);
                if (result.Code == ERROR_CODE.SUCCESS)
                {
                    model.Status = s.Status;
                    model.Close_Time = DateUtil.ToDisplayTime(s.Close_Time);

                    ModelState.Clear();
                    return Json(new
                    {
                        result = result.Code,
                        Msg = result.Msg,
                        Field = result.Field,
                        Valid = true,
                        Close_Time = DateUtil.ToDisplayTime(currentdate)
                    }, JsonRequestBehavior.AllowGet);
                }


            }



            return Json(new { Valid = false }, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}