//using POS.Models;
//using POS.Common;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using SBSModel.Models;
//using SBSModel.Common;

//namespace POS.Controllers
//{
//    public class POSMobileController : ControllerBase
//    {

//        #region Common
//        [AllowAnonymous]
//        [HttpPost]
//        public JsonResult GetCurrentDate()
//        {
//            var currentdate = StoredProcedure.GetCurrentDate();

//            return Json(new
//            {
//                Valid = false,
//                Current_Date = DateUtil.ToDisplayDate(currentdate),
//                Current_Datetime = DateUtil.ToDisplayDateTime(currentdate),
//                Current_Time = DateUtil.ToDisplayTime(currentdate)
//            }, JsonRequestBehavior.AllowGet);
//        }

//        private string ConvertString(string str)
//        {
//            if (!string.IsNullOrEmpty(str))
//                return str;

//            return "";
//        }

//        private decimal ConvertDecimal(Nullable<decimal> dec)
//        {
//            if (dec.HasValue)
//                return dec.Value;

//            return 0;
//        }

//        private int ConvertInteger(Nullable<int> i)
//        {
//            if (i.HasValue)
//                return i.Value;

//            return 0;
//        }
//        #endregion

//        #region Branch
//        [AllowAnonymous]
//        [HttpPost]
//        public JsonResult LstBranch(int pCompanyID)
//        {
//            var bService = new BranchService();


//            var blst = bService.LstBranch(pCompanyID).Select(s => new
//            {
//                Branch_ID = s.Branch_ID,
//                Branch_Code = s.Branch_Code,
//                Branch_Name = s.Branch_Name,
//                Branch_Desc = s.Branch_Desc,

//            });

//            return Json(new
//            {
//                Branch = blst.ToArray()
//            }, JsonRequestBehavior.AllowGet);
//        }
//        #endregion

//        #region Category
//        //
//        // GET: /PosMobile/
//        //[HttpPost]
//        [AllowAnonymous]
//        [HttpPost]
//        public JsonResult LstCategory(int pCompanyID, string pCategoryName)
//        {
//            var iService = new InventoryService();

//            var cri = new CategoryCriteria()
//            {
//                Company_ID = pCompanyID,
//                Category_Name = pCategoryName
//            };

//            var clst = iService.LstCategory(cri).Select(s => new
//            {
//                Category_Name = s.Category_Name,
//                Product_Category_ID = s.Product_Category_ID
//            });


//            return Json(new
//            {
//                Category = clst.ToArray()
//            }, JsonRequestBehavior.AllowGet);
//        }
//        #endregion

//        #region Product
//        //
//        // GET: /PosMobile/
//        [HttpPost]
//        [AllowAnonymous]
//        public JsonResult LstProduct(int pCompanyID, string pCategoryID, string pProductName)
//        {
//            var iService = new InventoryService();

//            List<Product> plst = new List<Product>();


//            Nullable<int> catID = null;
//            if (!string.IsNullOrEmpty(pCategoryID))
//            {
//                catID = NumUtil.ParseInteger(pCategoryID);
//            }
//            var pcri = new ProductCriteria()
//            {
//                Company_ID = pCompanyID,
//                Category_ID = catID,
//                Product_Name = pProductName
//            };
//            var products = (List<Product>)iService.LstProduct(pcri).Object;
//            //foreach (var p in products)
//            //{
//            //    var Product_Profile_Photo = "";
//            //    if (p.Product_Profile_Photo != null)
//            //    {
//            //        //var mainPhoto = p.Product_Profile_Photo.Where(s => s.Photo_Index == null || s.Photo_Index == 0).FirstOrDefault();
//            //        //if (mainPhoto != null)
//            //        //{
//            //        //    var base64 = Convert.ToBase64String(mainPhoto.Photo);
//            //        //    Product_Profile_Photo = String.Format("data:image/gif;base64,{0}", base64);
//            //        //}

//            //    }
//            //    var c = new Product()
//            //    {
//            //        ID = 0,
//            //        Product_ID = p.Product_ID,
//            //        Product_Name = p.Product_Name,
//            //        Product_Category_ID = p.Product_Category_ID,
//            //        Product_Code = p.Product_Code,
//            //        Price = p.Selling_Price.HasValue ? p.Selling_Price.Value : 0,
//            //        Product_Profile_Photo = Product_Profile_Photo
//            //    };
//            //    plst.Add(c);
//            //}


//            return Json(new
//            {
//                Product = products.Select(p => new
//                {
//                    ID = p.Product_ID,
//                    Product_ID = p.Product_ID,
//                    Product_Name = p.Product_Name,
//                    Product_Category_ID = p.Product_Category_L1.HasValue ? p.Product_Category_L1.Value : 0,
//                    Product_Code = p.Product_Code,
//                    Price = p.Selling_Price.HasValue ? p.Selling_Price.Value : 0,
//                    Product_Profile_Photo = ""
//                }).ToArray()
//            }, JsonRequestBehavior.AllowGet);
//        }


//        //public class Product
//        //{
//        //    public int ID { get; set; }
//        //    public int Product_ID { get; set; }
//        //    public string Product_Name { get; set; }
//        //    public int Product_Category_ID { get; set; }
//        //    public string Product_Code { get; set; }
//        //    public decimal Price { get; set; }
//        //    public string Product_Profile_Photo { get; set; }
//        //    public int Product_Color_ID { get; set; }
//        //    public int Product_Size_ID { get; set; }
//        //}

//        //
//        // GET: /PosMobile/
//        [HttpPost]
//        [AllowAnonymous]
//        public JsonResult LstProductProperty(int pProductID)
//        {
//            var iService = new InventoryService();
//            var colors = iService.LstProductColor(pProductID).Select(c => new Product_Color { Product_ID = c.Product_ID, Product_Color_ID = c.Product_Color_ID, Color = c.Color, Color_Code = c.Color_Code });
//            var sizes = iService.LstProductSize(pProductID).Select(s => new Product_Size { Product_ID = s.Product_ID, Product_Size_ID = s.Product_Size_ID, Size = s.Size });

//            return Json(new
//            {
//                Color = colors.ToArray(),
//                Size = sizes.ToArray()
//            }, JsonRequestBehavior.AllowGet);
//        }


//        //
//        // GET: /PosMobile/
//        //[HttpPost]
//        [AllowAnonymous]
//        [HttpPost]
//        public JsonResult LstCreditCardType(int pCompanyID)
//        {
//            var pService = new POSService();
//            var cService = new ComboService();

//            List<ComboViewModel> clst = cService.LstLookup(ComboType.Credit_Card_Type, pCompanyID, false);


//            return Json(new
//            {
//                CreditCardType = clst.ToArray()
//            }, JsonRequestBehavior.AllowGet);
//        }

//        //
//        // GET: /PosMobile/
//        //[HttpPost]
//        [AllowAnonymous]
//        [HttpPost]
//        public JsonResult LstBankname(int pCompanyID)
//        {
//            var pService = new POSService();
//            var cService = new ComboService();

//            List<ComboViewModel> clst = cService.LstLookup(ComboType.POS_Bank_Name, pCompanyID, false);

//            return Json(new
//            {
//                BankName = clst.ToArray()
//            }, JsonRequestBehavior.AllowGet);
//        }
//        #endregion

//        #region Terminal

//        //
//        // GET: /PosMobile/
//        //[HttpPost]
//        [AllowAnonymous]
//        [HttpPost]
//        public JsonResult GetTerminal(int pCashierID)
//        {
//            var tService = new POSConfigService();
//            var t = tService.GetTerminalByCashierID(pCashierID);

//            if (t != null)
//            {
//                return Json(new
//                {
//                    Terminal_ID = t.Terminal_ID,
//                    Terminal_Name = t.Terminal_Name,
//                    Branch_ID = t.Branch_ID,
//                }, JsonRequestBehavior.AllowGet);
//            }
//            return Json(new { Valid = false }, JsonRequestBehavior.AllowGet);
//        }

//        [AllowAnonymous]
//        [HttpPost]
//        public JsonResult SaveTerminal(TerminalViewModel model)
//        {
//            var currentdate = StoredProcedure.GetCurrentDate();
//            var t = new POS_Terminal();
//            t.Company_ID = model.Company_ID;
//            t.Cashier_ID = model.Cashier_ID;

//            t.Branch_ID = model.Branch_ID;
//            t.Host_Name = model.Host_Name;
//            t.Mac_Address = model.Mac_Address;
//            t.Terminal_Name = model.Terminal_Name;

//            var tService = new POSConfigService();

//            if (model.Terminal_ID.HasValue && model.Terminal_ID.Value > 0)
//            {
//                //update
//                t.Create_By = model.Create_By;
//                t.Create_On = currentdate;
//                t.Update_By = model.Update_By;
//                t.Update_On = currentdate;
//                t.Terminal_ID = model.Terminal_ID.Value;
//                var result = tService.UpdateTerminal(t);
//                return Json(new
//                {
//                    result = result.Code,
//                    Terminal_ID = t.Terminal_ID,
//                    Msg = result.Msg,
//                    Field = result.Field,
//                    Valid = true,
//                }, JsonRequestBehavior.AllowGet);
//            }
//            else
//            {
//                //insert
//                t.Create_By = model.Create_By;
//                t.Create_On = currentdate;
//                var result = tService.InsertTerminal(t);
//                return Json(new
//                {
//                    result = result.Code,
//                    Terminal_ID = t.Terminal_ID,
//                    Msg = result.Msg,
//                    Field = result.Field,
//                    Valid = true,
//                }, JsonRequestBehavior.AllowGet);
//            }
//        }

//        #endregion

//        #region Receipt

//        [AllowAnonymous]
//        [HttpPost]
//        public JsonResult GetReceipt(int pReceiptID)
//        {
//            var pService = new POSService();

//            var r = pService.GetPOSReceipt(pReceiptID);
//            if (r != null)
//            {
//                var p = r.POS_Products_Rcp.Select(s => new
//                {
//                    ID = s.ID,
//                    Receipt_ID = ConvertInteger(s.Receipt_ID),
//                    Price = ConvertDecimal(s.Price),
//                    Qty = ConvertInteger(s.Qty),
//                    Product_Color_ID = ConvertInteger(s.Product_Color_ID),
//                    Product_Size_ID = ConvertInteger(s.Product_Size_ID),
//                    Product_Color = s.Product_Color != null ? s.Product_Color.Color : "",
//                    Product_Size = s.Product_Size != null ? s.Product_Size.Size : "",
//                    Product_ID = s.Product_ID.HasValue ? s.Product_ID.Value : 0,
//                    Product_Name = s.Product_Color != null && s.Product_Size != null ? (s.Product.Product_Name + "\ncolour: " + s.Product_Color.Color + " size: " + s.Product_Size.Size) :
//                    s.Product_Color != null ? (s.Product.Product_Name + "\ncolour: " + s.Product_Color.Color) :
//                    s.Product_Size != null ? (s.Product.Product_Name + "\nsize: " + s.Product_Size.Size) : s.Product.Product_Name,
//                    Product_Code = s.Product.Product_Code,
//                    Discount = ConvertDecimal(s.Discount),
//                    Discount_Type = ConvertString(s.Discount_Type),
//                });

//                var payments = r.POS_Receipt_Payment.Select(s => new
//                {
//                    Receipt_Payment_ID = s.Receipt_Payment_ID,
//                    Receipt_ID = s.Receipt_ID.HasValue ? s.Receipt_ID.Value : 0,
//                    Payment_Type = s.Payment_Type.HasValue ? s.Payment_Type.Value : 0,
//                    Payment_Amount = s.Payment_Amount.HasValue ? s.Payment_Amount.Value : 0,
//                    Approval_Code = s.Approval_Code != null ? s.Approval_Code : "",
//                    Card_Branch = r.Card_Branch.HasValue ? r.Card_Branch.Value : 0,
//                    Card_Type = r.Card_Type.HasValue ? r.Card_Type.Value : 0,
//                });

//                return Json(new
//                {
//                    Approval_Code = r.Approval_Code,
//                    Card_Branch = r.Card_Branch.HasValue ? r.Card_Branch.Value : 0,
//                    Card_Branch_Name = r.Global_Lookup_Data != null ? r.Global_Lookup_Data.Name : "",
//                    Card_Type = r.Card_Type.HasValue ? r.Card_Type.Value : 0,
//                    Card_Type_Name = r.Global_Lookup_Data1 != null ? r.Global_Lookup_Data1.Name : "",
//                    Cash_Payment = r.Cash_Payment.HasValue ? r.Cash_Payment.Value : 0,
//                    Cashier = r.Cashier.HasValue ? r.Cashier.Value : 0,
//                    Changes = r.Changes.HasValue ? r.Changes.Value : 0,
//                    Company_ID = r.Company_ID.HasValue ? r.Company_ID.Value : 0,
//                    Discount = r.Discount.HasValue ? r.Discount.Value : 0,
//                    Discount_Reason = r.Discount_Reason,
//                    Discount_Type = r.Discount_Type,
//                    Net_Amount = r.Net_Amount.HasValue ? r.Net_Amount.Value : 0,
//                    Payment_Type = r.Payment_Type.HasValue ? r.Payment_Type.Value : 0,
//                    Promotion_ID = r.Promotion_ID.HasValue ? r.Promotion_ID.Value : 0,
//                    Receipt_Date = DateUtil.ToDisplayDate2(r.Receipt_Date),
//                    Receipt_Time = DateUtil.ToDisplayTime(r.Receipt_Date),
//                    Receipt_ID = r.Receipt_ID,
//                    Receipt_No = r.Receipt_No,
//                    Terminal_ID = r.Terminal_ID.HasValue ? r.Terminal_ID.Value : 0,
//                    Total_Amount = r.Total_Amount.HasValue ? r.Total_Amount.Value : 0,
//                    Total_Discount = r.Total_Discount.HasValue ? r.Total_Discount.Value : 0,
//                    Total_Qty = r.Total_Qty.HasValue ? r.Total_Qty.Value : 0,
//                    Voucher_Amount = r.Voucher_Amount.HasValue ? r.Voucher_Amount.Value : 0,
//                    Terminal = r.POS_Terminal != null ? r.POS_Terminal.Terminal_Name : "",
//                    Branch = r.POS_Terminal != null ? r.POS_Terminal.Branch != null ? r.POS_Terminal.Branch.Branch_Name : "" : "",
//                    Status = r.Status != null ? r.Status : "",
//                    Remark = r.Remark != null ? r.Remark : "",
//                    Customer_Name = (r.Customer_Name != null ? r.Customer_Name : ""),
//                    Contact_No = (r.Contact_No != null ? r.Contact_No : ""),
//                    NRIC_No = (r.NRIC_No != null ? r.NRIC_No : ""),
//                    Member_ID = r.Member_ID.HasValue ? r.Member_ID.Value : 0,
//                    Product = p.ToArray(),
//                    Payments = payments.ToArray(),
//                    Valid = true,
//                }, JsonRequestBehavior.AllowGet);
//            }
//            return Json(new { Valid = false }, JsonRequestBehavior.AllowGet);
//        }


//        [AllowAnonymous]
//        [HttpPost]
//        public JsonResult LstReceipt(int pCompanyID, int pCashierID, string pStartDate, string pEndDate, string pStatus = "")
//        {
//            var pService = new POSService();
//            var currentdate = StoredProcedure.GetCurrentDate();
//            Nullable<DateTime> sDate = currentdate;
//            Nullable<DateTime> eDate = currentdate;
//            if (!string.IsNullOrEmpty(pStartDate))
//            {
//                sDate = DateUtil.ToDate(pStartDate, "-");
//            }

//            if (!string.IsNullOrEmpty(pEndDate))
//            {
//                eDate = DateUtil.ToDate(pEndDate, "-");
//            }

//            User_Profile userlogin = new UserService().getUser(pCashierID);

//            var posTerminal = pService.GetTerminal(pCashierID);

//            var rlst = pService.LstPOSReceipt(pCompanyID, pCashierID, sDate, eDate, pStatus, pBranchID: posTerminal.Branch_ID, pUserAuthenticationID: userlogin.User_Authentication_ID).Select(r => new
//            {
//                Approval_Code = r.Approval_Code,
//                Card_Branch = r.Card_Branch.HasValue ? r.Card_Branch.Value : 0,
//                Card_Branch_Name = r.Global_Lookup_Data != null ? r.Global_Lookup_Data.Name : "",
//                Card_Type = r.Card_Type.HasValue ? r.Card_Type.Value : 0,
//                Card_Type_Name = r.Global_Lookup_Data1 != null ? r.Global_Lookup_Data1.Name : "",
//                Cash_Payment = r.Cash_Payment.HasValue ? r.Cash_Payment.Value : 0,
//                Cashier = r.Cashier.HasValue ? r.Cashier.Value : 0,
//                Changes = r.Changes.HasValue ? r.Changes.Value : 0,
//                Company_ID = r.Company_ID.HasValue ? r.Company_ID.Value : 0,
//                Discount = r.Discount.HasValue ? r.Discount.Value : 0,
//                Discount_Reason = r.Discount_Reason,
//                Discount_Type = r.Discount_Type,
//                Net_Amount = r.Net_Amount.HasValue ? r.Net_Amount.Value : 0,
//                Payment_Type = r.Payment_Type.HasValue ? r.Payment_Type.Value : 0,
//                Promotion_ID = r.Promotion_ID.HasValue ? r.Promotion_ID.Value : 0,
//                Receipt_Date = DateUtil.ToDisplayDate2(r.Receipt_Date),
//                Receipt_Time = DateUtil.ToDisplayTime(r.Receipt_Date),
//                Receipt_ID = r.Receipt_ID,
//                Receipt_No = r.Receipt_No != null ? r.Receipt_No : "",
//                Terminal_ID = r.Terminal_ID.HasValue ? r.Terminal_ID.Value : 0,
//                Total_Amount = r.Total_Amount.HasValue ? r.Total_Amount.Value : 0,
//                Total_Discount = r.Total_Discount.HasValue ? r.Total_Discount.Value : 0,
//                Total_Qty = r.Total_Qty.HasValue ? r.Total_Qty.Value : 0,
//                Voucher_Amount = r.Voucher_Amount.HasValue ? r.Voucher_Amount.Value : 0,
//                Terminal = r.POS_Terminal != null ? r.POS_Terminal.Terminal_Name : "",
//                Branch = r.POS_Terminal != null ? r.POS_Terminal.Branch != null ? r.POS_Terminal.Branch.Branch_Name : "" : "",
//                Status = r.Status != null ? r.Status : "",
//                Remark = r.Remark != null ? r.Remark : "",
//                Create_By = (r.Create_By != null ? r.Create_By : ""),
//                Create_On = DateUtil.ToDisplayDate(r.Create_On),
//                Update_By = (r.Update_By != null ? r.Update_By : ""),
//                Update_On = DateUtil.ToDisplayDate(r.Update_On),
//                Customer_Name = (r.Customer_Name != null ? r.Customer_Name : ""),
//                Contact_No = (r.Contact_No != null ? r.Contact_No : ""),
//                NRIC_No = (r.NRIC_No != null ? r.NRIC_No : ""),
//                Member_ID = r.Member_ID.HasValue ? r.Member_ID.Value : 0,
//            });

//            return Json(new
//            {
//                Receipt = rlst.ToArray(),
//                Valid = true,
//            }, JsonRequestBehavior.AllowGet);
//        }



//        [HttpPost]
//        [AllowAnonymous]
//        public JsonResult SaveReceipt(POSMobileViewModel model)
//        {
//            var currentdate = StoredProcedure.GetCurrentDate();
//            var pService = new POSService();

//            POS_Receipt rcp = new POS_Receipt();

//            if (model.Receipt_ID.HasValue && model.Receipt_ID.Value > 0)
//                rcp = pService.GetPOSReceipt(model.Receipt_ID.Value);

//            rcp.Company_ID = model.Company_ID;
//            rcp.Cashier = model.Profile_ID;
//            rcp.Cash_Payment = model.Cash_Payment;
//            rcp.Changes = model.Changes;

//            rcp.Discount = model.Discount;
//            rcp.Discount_Type = model.Discount_Type;
//            rcp.Discount_Reason = model.Discount_Reason;
//            rcp.Status = model.Status;
//            rcp.Terminal_ID = model.Terminal_ID;

//            rcp.Net_Amount = model.Net_Amount;
//            rcp.Receipt_Date = currentdate;
//            rcp.Total_Amount = model.Total_Amount;
//            rcp.Total_Discount = model.Total_Discount;
//            rcp.Total_Qty = model.Total_Qty;
//            rcp.Payment_Type = model.Payment_Type;
//            rcp.Remark = "";
//            rcp.Customer_Name = model.Customer_Name;
//            rcp.Contact_No = model.Contact_No;
//            rcp.NRIC_No = model.NRIC_No;
//            rcp.Member_ID = model.Member_ID;
//            if (model.Member_ID.HasValue && model.Member_ID.Value == 0) rcp.Member_ID = null;

//            if (model.Status == ReceiptStatus.BackOrder)
//            {
//                rcp.Remark = model.Remark;
//                rcp.Customer_Name = model.Customer_Name;
//                rcp.Contact_No = model.Contact_No;
//                rcp.NRIC_No = model.NRIC_No;
//            }



//            if (model.Status == ReceiptStatus.Hold)
//            {
//                rcp.POS_Products_Rcp = model.Products;
//                rcp.POS_Receipt_Payment = model.Payments;
//                rcp.User_Profile = null;

//                var result = pService.InsertPOSHoldBillReceipt(rcp);
//                return Json(new
//                {
//                    result = result.Code,
//                    Receipt_No = rcp.Receipt_No != null ? rcp.Receipt_No : "",
//                    Receipt_Date = DateUtil.ToDisplayDate2(rcp.Receipt_Date),
//                    Receipt_Time = DateUtil.ToDisplayTime(rcp.Receipt_Date),
//                    Receipt_ID = rcp.Receipt_ID,
//                    Status = rcp.Status,
//                    Msg = result.Msg,
//                    Field = result.Field,
//                    Valid = true,
//                }, JsonRequestBehavior.AllowGet);
//            }
//            else
//            {
//                if (model.Receipt_ID.HasValue && model.Receipt_ID.Value > 0)
//                {
//                    //update
//                    if (rcp != null)
//                    {
//                        rcp.Receipt_ID = model.Receipt_ID.Value;
//                        rcp.POS_Receipt_Payment = model.Payments;
//                        rcp.POS_Products_Rcp = model.Products;

//                        var result = pService.UpdatePOSReceipt(rcp);
//                        return Json(new
//                        {
//                            result = result.Code,
//                            Receipt_No = rcp.Receipt_No != null ? rcp.Receipt_No : "",
//                            Receipt_Date = DateUtil.ToDisplayDate2(rcp.Receipt_Date),
//                            Receipt_Time = DateUtil.ToDisplayTime(rcp.Receipt_Date),
//                            Receipt_ID = rcp.Receipt_ID,
//                            Status = rcp.Status,
//                            Msg = result.Msg,
//                            Field = result.Field,
//                            Valid = true,
//                        }, JsonRequestBehavior.AllowGet);
//                    }
//                }
//                else
//                {
//                    //insert
//                    rcp.POS_Products_Rcp = model.Products;
//                    rcp.POS_Receipt_Payment = model.Payments;

//                    var result = pService.InsertPOSReceipt(rcp);
//                    return Json(new
//                    {
//                        result = result.Code,
//                        Receipt_No = rcp.Receipt_No != null ? rcp.Receipt_No : "",
//                        Receipt_Date = DateUtil.ToDisplayDate2(rcp.Receipt_Date),
//                        Receipt_Time = DateUtil.ToDisplayTime(rcp.Receipt_Date),
//                        Receipt_ID = rcp.Receipt_ID,
//                        Member_ID = rcp.Member_ID.HasValue ? rcp.Member_ID.Value : 0,
//                        Status = rcp.Status,
//                        Msg = result.Msg,
//                        Field = result.Field,
//                        Valid = true,
//                    }, JsonRequestBehavior.AllowGet);

//                }
//            }
//            return Json(new { Valid = false }, JsonRequestBehavior.AllowGet);
//        }

//        [HttpPost]
//        [AllowAnonymous]
//        public JsonResult RemoveHoldBill(Nullable<int> pReceiptID)
//        {
//            var pService = new POSService();
//            var result = pService.DeletePOSReceipt(pReceiptID);
//            if (result.Code == ERROR_CODE.SUCCESS)
//            {
//                return Json(new
//                {
//                    result = result.Code,
//                    Msg = result.Msg,
//                    Field = result.Field,
//                    Valid = true,
//                }, JsonRequestBehavior.AllowGet);
//            }
//            return Json(new { Valid = false }, JsonRequestBehavior.AllowGet);
//        }

//        [HttpPost]
//        [AllowAnonymous]
//        public JsonResult ChangeReceiptStatus(int pReceiptID, string pStatus)
//        {

//            var pService = new POSService();

//            if (pReceiptID > 0)
//            {
//                POS_Receipt r = pService.GetPOSReceipt(pReceiptID);
//                if (r == null)
//                {
//                    return Json(new
//                    {
//                        result = ERROR_CODE.ERROR_511_DATA_NOT_FOUND,
//                        Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND)
//                    }, JsonRequestBehavior.AllowGet);
//                }

//                var rcpProduct = r.POS_Products_Rcp;
//                r.Status = pStatus;
//                var result = pService.UpdatePOSReceipt(r);
//                r.POS_Products_Rcp = rcpProduct;

//                if (result.Code == ERROR_CODE.SUCCESS)
//                {
//                    var p = r.POS_Products_Rcp.Select(s => new
//                    {
//                        ID = s.ID,
//                        Receipt_ID = s.Receipt_ID.HasValue ? s.Receipt_ID.Value : 0,
//                        Price = s.Price.HasValue ? s.Price.Value : 0,
//                        Qty = s.Qty.HasValue ? s.Qty.Value : 0,
//                        Product_Color_ID = s.Product_Color_ID.HasValue ? s.Product_Color_ID.Value : 0,
//                        Product_Size_ID = s.Product_Size_ID.HasValue ? s.Product_Size_ID.Value : 0,
//                        Product_Color = s.Product_Color != null ? s.Product_Color.Color : "",
//                        Product_Size = s.Product_Size != null ? s.Product_Size.Size : "",
//                        Product_ID = s.Product_ID.HasValue ? s.Product_ID.Value : 0,
//                        Product_Name = s.Product_Color != null && s.Product_Size != null ? (s.Product.Product_Name + "\ncolour: " + s.Product_Color.Color + " size: " + s.Product_Size.Size) :
//                        s.Product_Color != null ? (s.Product.Product_Name + "\ncolour: " + s.Product_Color.Color) :
//                        s.Product_Size != null ? (s.Product.Product_Name + "\nsize: " + s.Product_Size.Size) : s.Product.Product_Name,
//                        Product_Code = s.Product.Product_Code,
//                        Discount = ConvertDecimal(s.Discount),
//                        Discount_Type = ConvertString(s.Discount_Type),
//                    });
//                    var payments = r.POS_Receipt_Payment.Select(s => new
//                    {
//                        Receipt_Payment_ID = s.Receipt_Payment_ID,
//                        Receipt_ID = s.Receipt_ID.HasValue ? s.Receipt_ID.Value : 0,
//                        Payment_Type = s.Payment_Type.HasValue ? s.Payment_Type.Value : 0,
//                        Payment_Amount = s.Payment_Amount.HasValue ? s.Payment_Amount.Value : 0,
//                        Approval_Code = s.Approval_Code != null ? s.Approval_Code : "",
//                        Card_Branch = r.Card_Branch.HasValue ? r.Card_Branch.Value : 0,
//                        Card_Type = r.Card_Type.HasValue ? r.Card_Type.Value : 0
//                    });

//                    return Json(new
//                    {
//                        Approval_Code = r.Approval_Code,
//                        Card_Branch = r.Card_Branch.HasValue ? r.Card_Branch.Value : 0,
//                        Card_Branch_Name = r.Global_Lookup_Data != null ? r.Global_Lookup_Data.Name : "",
//                        Card_Type = r.Card_Type.HasValue ? r.Card_Type.Value : 0,
//                        Card_Type_Name = r.Global_Lookup_Data1 != null ? r.Global_Lookup_Data1.Name : "",
//                        Cash_Payment = r.Cash_Payment.HasValue ? r.Cash_Payment.Value : 0,
//                        Cashier = r.Cashier.HasValue ? r.Cashier.Value : 0,
//                        Changes = r.Changes.HasValue ? r.Changes.Value : 0,
//                        Company_ID = r.Company_ID.HasValue ? r.Company_ID.Value : 0,
//                        Discount = r.Discount.HasValue ? r.Discount.Value : 0,
//                        Discount_Reason = r.Discount_Reason,
//                        Discount_Type = r.Discount_Type,
//                        Net_Amount = r.Net_Amount.HasValue ? r.Net_Amount.Value : 0,
//                        Payment_Type = r.Payment_Type.HasValue ? r.Payment_Type.Value : 0,
//                        Promotion_ID = r.Promotion_ID.HasValue ? r.Promotion_ID.Value : 0,
//                        Receipt_Date = DateUtil.ToDisplayDate2(r.Receipt_Date),
//                        Receipt_Time = DateUtil.ToDisplayTime(r.Receipt_Date),
//                        Receipt_ID = r.Receipt_ID,
//                        Receipt_No = r.Receipt_No != null ? r.Receipt_No : "",
//                        Terminal_ID = r.Terminal_ID.HasValue ? r.Terminal_ID.Value : 0,
//                        Total_Amount = r.Total_Amount.HasValue ? r.Total_Amount.Value : 0,
//                        Total_Discount = r.Total_Discount.HasValue ? r.Total_Discount.Value : 0,
//                        Total_Qty = r.Total_Qty.HasValue ? r.Total_Qty.Value : 0,
//                        Voucher_Amount = r.Voucher_Amount.HasValue ? r.Voucher_Amount.Value : 0,
//                        Terminal = r.POS_Terminal != null ? r.POS_Terminal.Terminal_Name : "",
//                        Branch = r.POS_Terminal != null ? r.POS_Terminal.Branch != null ? r.POS_Terminal.Branch.Branch_Name : "" : "",
//                        Product = p.ToArray(),
//                        Payments = payments.ToArray(),
//                        Status = r.Status != null ? r.Status : "",
//                        result = result.Code,
//                        Msg = result.Msg,
//                        Field = result.Field
//                    }, JsonRequestBehavior.AllowGet);
//                }


//                return Json(new
//                {
//                    result = result.Code,
//                    Receipt_No = r.Receipt_No,
//                    Receipt_Date = DateUtil.ToDisplayDate(r.Receipt_Date),
//                    Receipt_Time = DateUtil.ToDisplayTime(r.Receipt_Date),
//                    Receipt_ID = r.Receipt_ID,
//                    Msg = result.Msg,
//                    Field = result.Field
//                }, JsonRequestBehavior.AllowGet);


//            }

//            return Json(new { Valid = false }, JsonRequestBehavior.AllowGet);
//        }

//        #endregion

//        #region Member

//        // Added by Jane 07-08-2015
//        [AllowAnonymous]
//        [HttpPost]
//        public JsonResult LstMember(int pCompanyID, string pMemberName)
//        {
//            var mService = new MemberService();

//            var mlst = mService.LstMember(pCompanyID, pMemberName).Select(m => new POSMobileMemberViewModel
//            {
//                Member_ID = m.Member_ID,
//                Company_ID = m.Company_ID,
//                Member_Card_No = (m.Member_Card_No != null ? m.Member_Card_No : ""),
//                Member_Name = (m.Member_Name != null ? m.Member_Name : ""),
//                NRIC_No = (m.NRIC_No != null ? m.NRIC_No : ""),
//                Phone_No = (m.NRIC_No != null ? m.Phone_No : ""),
//                Email = (m.Email != null ? m.Email : ""),
//                DOB = DateUtil.ToDisplayDate(m.DOB),
//                Member_Status = (m.Member_Status != null ? m.Member_Status : ""),
//                Create_By = (m.Create_By != null ? m.Create_By : ""),
//                Create_On = DateUtil.ToDisplayDate(m.Create_On),
//                Update_By = (m.Update_By != null ? m.Update_By : ""),
//                Update_On = DateUtil.ToDisplayDate(m.Update_On),
//                Credit = (m.Credit.HasValue ? m.Credit.Value : 0),
//            });

//            return Json(new
//            {
//                Members = mlst.ToArray(),
//                Valid = true
//            }, JsonRequestBehavior.AllowGet);
//        }

//        [AllowAnonymous]
//        [HttpPost]
//        public JsonResult GetMember(int pMemberID)
//        {
//            var mService = new MemberService();

//            var m = mService.GetMember(pMemberID);

//            if (m != null)
//            {
//                return Json(new
//                {
//                    Member_ID = m.Member_ID,
//                    Company_ID = m.Company_ID,
//                    Member_Card_No = (m.Member_Card_No != null ? m.Member_Card_No : ""),
//                    Member_Name = (m.Member_Name != null ? m.Member_Name : ""),
//                    NRIC_No = (m.NRIC_No != null ? m.NRIC_No : ""),
//                    Phone_No = (m.NRIC_No != null ? m.Phone_No : ""),
//                    Email = (m.Email != null ? m.Email : ""),
//                    DOB = DateUtil.ToDisplayDate(m.DOB),
//                    Member_Status = (m.Member_Status != null ? m.Member_Status : ""),
//                    Create_By = (m.Create_By != null ? m.Create_By : ""),
//                    Create_On = DateUtil.ToDisplayDate(m.Create_On),
//                    Update_By = (m.Update_By != null ? m.Update_By : ""),
//                    Update_On = DateUtil.ToDisplayDate(m.Update_On),
//                    Credit = (m.Credit.HasValue ? m.Credit.Value : 0),
//                    Valid = true
//                }, JsonRequestBehavior.AllowGet);
//            }
//            return Json(new { Valid = false }, JsonRequestBehavior.AllowGet);
//        }


//        [HttpPost]
//        [AllowAnonymous]
//        public JsonResult InsertMember(POSMobileMemberViewModel model)
//        {
//            var currentdate = StoredProcedure.GetCurrentDate();
//            var mService = new MemberService();

//            var m = new Member();

//            m.Company_ID = model.Company_ID;
//            m.Member_Card_No = model.Member_Card_No;
//            m.Member_Name = model.Member_Name;
//            m.NRIC_No = model.NRIC_No;
//            m.Phone_No = model.Phone_No;
//            m.Email = model.Email;
//            m.DOB = DateUtil.ToDate(model.DOB);
//            m.Member_Status = model.Member_Status;
//            m.Create_By = model.Create_By;
//            m.Create_On = currentdate;
//            m.Update_By = model.Update_By;
//            m.Update_On = currentdate;
//            m.Credit = model.Credit;

//            var result = mService.InsertMember(m);
//            if (result.Code == ERROR_CODE.SUCCESS)
//            {
//                return Json(new
//                {
//                    result = result.Code,
//                    Member_ID = m.Member_ID,
//                    Member_Card_No = m.Member_Card_No,
//                    Msg = result.Msg,
//                    Field = result.Field,
//                    Valid = true
//                }, JsonRequestBehavior.AllowGet);
//            }

//            return Json(new { Valid = false }, JsonRequestBehavior.AllowGet);
//        }

//        [HttpPost]
//        [AllowAnonymous]
//        public JsonResult UpdateMember(POSMobileMemberViewModel model)
//        {
//            var currentdate = StoredProcedure.GetCurrentDate();
//            var mService = new MemberService();

//            var m = mService.GetMember(model.Member_ID);
//            if (m != null)
//            {
//                m.Company_ID = model.Company_ID;
//                m.Member_Card_No = model.Member_Card_No;
//                m.Member_Name = model.Member_Name;
//                m.NRIC_No = model.NRIC_No;
//                m.Phone_No = model.Phone_No;
//                m.Email = model.Email;
//                m.DOB = DateUtil.ToDate(model.DOB);
//                m.Member_Status = model.Member_Status;
//                m.Member_Discount = model.Member_Discount;
//                m.Member_Discount_Type = model.Member_Discount_Type;
//                m.Update_By = model.Update_By;
//                m.Update_On = currentdate;
//                m.Credit = model.Credit;

//                var result = mService.UpdateMember(m);
//                if (result.Code == ERROR_CODE.SUCCESS)
//                {
//                    return Json(new
//                    {
//                        result = result.Code,
//                        Msg = result.Msg,
//                        Field = result.Field,
//                        Valid = true
//                    }, JsonRequestBehavior.AllowGet);
//                }
//            }



//            return Json(new { Valid = false }, JsonRequestBehavior.AllowGet);
//        }

//        [AllowAnonymous]
//        [HttpPost]
//        public JsonResult GetMemberConfiguration(int pCompanyID)
//        {
//            var mcService = new MemberConfigService();
//            var memconf = mcService.GetMemberConfig(pCompanyID);
//            if (memconf != null)
//            {
//                return Json(new
//                {
//                    Company_ID = memconf.Company_ID,
//                    Member_Configuration_ID = memconf.Member_Configuration_ID,
//                    Member_Discount = memconf.Member_Discount,
//                    Member_Discount_Type = memconf.Member_Discount_Type,
//                    Birthday_Discount = memconf.Birthday_Discount,
//                    Birthday_Discount_Type = memconf.Birthday_Discount_Type,
//                    Valid = true
//                }, JsonRequestBehavior.AllowGet);

//            }
//            return Json(new { Valid = false }, JsonRequestBehavior.AllowGet);

//        }
//        #endregion

//        #region Shift

//        // Added by Jane 16-08-2015
//        [AllowAnonymous]
//        [HttpPost]
//        public JsonResult LstShift(Nullable<int> pCompanyID, Nullable<int> pBranchID)
//        {
//            var pService = new POSService();

//            var slst = pService.LstPOSShift(pCompanyID, pBranchID).Select(s => new ShiftMobileViewModel
//            {
//                Shift_ID = s.Shift_ID,
//                Company_ID = s.Company_ID,
//                Branch_ID = s.Branch_ID,
//                Branch_Name = (s.Branch != null ? s.Branch.Branch_Name : ""),
//                Open_Time = DateUtil.ToDisplayDateTime(s.Open_Time),
//                Close_Time = DateUtil.ToDisplayDateTime(s.Close_Time),
//                Effective_Date = DateUtil.ToDisplayDateTime(s.Effective_Date),
//                Total_Amount = (s.Total_Amount.HasValue ? s.Total_Amount.Value : 0),
//                Status = s.Status,
//                Email_Address = (s.Create_By != null ? s.Create_By : ""),
//                Create_By = (s.Create_By != null ? s.Create_By : ""),
//                Create_On = DateUtil.ToDisplayDate(s.Create_On),
//                Update_By = (s.Update_By != null ? s.Update_By : ""),
//                Update_On = DateUtil.ToDisplayDate(s.Update_On),

//            });

//            return Json(new
//            {
//                Shifts = slst.ToArray(),
//                Valid = true
//            }, JsonRequestBehavior.AllowGet);
//        }

//        [AllowAnonymous]
//        [HttpPost]
//        public JsonResult GetCurrentOpenShift(Nullable<int> pCompanyID, Nullable<int> pBranchID, string pEmailAddress)
//        {
//            var pService = new POSService();

//            var s = pService.GetCurrentOpenShift(pCompanyID, pBranchID, pEmailAddress);

//            if (s != null)
//            {
//                return Json(new
//                {
//                    Shift_ID = s.Shift_ID,
//                    Company_ID = s.Company_ID,
//                    Branch_ID = s.Branch_ID,
//                    Branch_Name = (s.Branch != null ? s.Branch.Branch_Name : ""),
//                    Open_Time = DateUtil.ToDisplayDateTime(s.Open_Time),
//                    Close_Time = DateUtil.ToDisplayDateTime(s.Close_Time),
//                    Effective_Date = DateUtil.ToDisplayDateTime(s.Effective_Date),
//                    Total_Amount = (s.Total_Amount.HasValue ? s.Total_Amount.Value : 0),
//                    Status = s.Status,
//                    Email_Address = (s.Create_By != null ? s.Create_By : ""),
//                    Create_By = (s.Create_By != null ? s.Create_By : ""),
//                    Create_On = DateUtil.ToDisplayDate(s.Create_On),
//                    Update_By = (s.Update_By != null ? s.Update_By : ""),
//                    Update_On = DateUtil.ToDisplayDate(s.Update_On),
//                    Valid = true
//                }, JsonRequestBehavior.AllowGet);
//            }
//            return Json(new { Valid = false }, JsonRequestBehavior.AllowGet);
//        }

//        [HttpPost]
//        [AllowAnonymous]
//        public JsonResult OpenShift(ShiftMobileViewModel model)
//        {
//            var currentdate = StoredProcedure.GetCurrentDate();
//            var pService = new POSService();
//            var cService = new POSConfigService();
//            var s = new POS_Shift();


//            s.Company_ID = model.Company_ID;
//            s.Branch_ID = model.Branch_ID;
//            s.Open_Time = currentdate;
//            s.Effective_Date = currentdate;
//            s.Total_Amount = model.Total_Amount;
//            s.Status = ShiftStatus.Open;
//            s.Create_By = model.Create_By;
//            s.Create_On = currentdate;
            
//            var result = cService.InsertShift(s);
//            if (result.Code == ERROR_CODE.SUCCESS)
//            {
//                return Json(new
//                {
//                    result = result.Code,
//                    Member_ID = s.Shift_ID,
//                    Msg = result.Msg,
//                    Field = result.Field,
//                    Valid = true
//                }, JsonRequestBehavior.AllowGet);
//            }

//            return Json(new { Valid = false }, JsonRequestBehavior.AllowGet);
//        }

//        [HttpPost]
//        [AllowAnonymous]
//        public JsonResult CloseShift(ShiftMobileViewModel model)
//        {
//            var currentdate = StoredProcedure.GetCurrentDate();
//            var pService = new POSService();
//            var cService = new POSConfigService();

//            var s = pService.GetShift(model.Shift_ID);
//            if (s != null)
//            {
//                s.Close_Time = currentdate;
//                s.Total_Amount = model.Total_Amount;
//                s.Status = ShiftStatus.Close;

//                s.Update_By = model.Update_By;
//                s.Update_On = currentdate;

//                var result = cService.UpdateShift(s);
//                if (result.Code == ERROR_CODE.SUCCESS)
//                {
//                    model.Status = s.Status;
//                    model.Close_Time = DateUtil.ToDisplayTime(s.Close_Time);

//                    ModelState.Clear();
//                    return Json(new
//                    {
//                        result = result.Code,
//                        Msg = result.Msg,
//                        Field = result.Field,
//                        Valid = true,
//                        Close_Time = DateUtil.ToDisplayTime(currentdate)
//                    }, JsonRequestBehavior.AllowGet);
//                }


//            }



//            return Json(new { Valid = false }, JsonRequestBehavior.AllowGet);
//        }

//        #endregion

//    }
//}