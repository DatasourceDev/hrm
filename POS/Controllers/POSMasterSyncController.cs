//using POS.Models;
//using POS.Common;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using SBSModel.Models;
//using SBSModel.Common;

//namespace POS.Controllers {
//    public class POSMasterSyncController : ControllerBase {

//        #region GET FUNCTIONS
//        [AllowAnonymous]
//        [HttpGet]
//        public JsonResult LstUsers(int pCompanyID, bool pActiveOnly = true) {
//            var pService = new UserService();
//            List<User_Profile> plst = new List<User_Profile>();

//            foreach (var p in pService.getUsers(pCompanyID)) {

//                var uProfile = new User_Profile() {
//                    Profile_ID = p.Profile_ID,
//                    Company_ID = p.Company_ID,
//                    User_Authentication_ID = p.User_Authentication_ID,
//                    Name = p.Name,
//                    Registration_Date = p.Registration_Date,
//                    User_Status = p.User_Status,
//                    Latest_Connection = p.Latest_Connection,
//                    User_Profile_Photo_ID = p.User_Profile_Photo_ID
//                };

//                if (p.User_Authentication != null) {

//                    var uAuth = new User_Authentication() {
//                        User_Authentication_ID = p.User_Authentication.User_Authentication_ID,
//                        Email_Address = p.User_Authentication.Email_Address,
//                        PWD = p.User_Authentication.PWD,
//                        Login_Attempt = p.User_Authentication.Login_Attempt,
//                        Activated = p.User_Authentication.Activated,
//                        ApplicationUser_Id = p.User_Authentication.ApplicationUser_Id
//                    };

//                    uProfile.User_Authentication = uAuth;
//                }

//                if (p.User_Profile_Photo != null) {

//                    var uPhoto = new User_Profile_Photo() {
//                        User_Profile_Photo_ID = p.User_Profile_Photo.User_Profile_Photo_ID,
//                        Photo = p.User_Profile_Photo.Photo
//                    };

//                    uProfile.User_Profile_Photo = uPhoto;
//                }

//                plst.Add(uProfile);
//            }

//            return Json(new {
//                User_Profile = plst.ToArray()
//            }, JsonRequestBehavior.AllowGet);
//        }

//        [AllowAnonymous]
//        [HttpGet]
//        public JsonResult LstProductCategories(int pCompanyID, bool pActiveOnly = true) {
//            var pService = new POSService();
//            List<Product_Category> plst = new List<Product_Category>();

//            foreach (var p in pService.LstCategory(pCompanyID, string.Empty, pActiveOnly)) {

//                var c = new Product_Category() {
//                    Product_Category_ID = p.Product_Category_ID,
//                    Category_Name = p.Category_Name,
//                    Record_Status = p.Record_Status,
//                    Company_ID = p.Company_ID
//                };

//                plst.Add(c);
//            }

//            return Json(new {
//                Product_Category = plst.ToArray()
//            }, JsonRequestBehavior.AllowGet);
//        }

//        [AllowAnonymous]
//        [HttpGet]
//        public JsonResult LstProducts(int pCompanyID, bool pActiveOnly = true) {
//            var pService = new POSService();
//            List<Product> plst = new List<Product>();

//            foreach (var p in pService.LstProduct(pCompanyID, string.Empty, string.Empty, pActiveOnly, false)) {

//                var c = new Product() {
//                    Product_ID = p.Product_ID,
//                    Company_ID = p.Company_ID,
//                    Product_Category_ID = p.Product_Category_ID,
//                    Product_Code = p.Product_Code,
//                    Product_Name = p.Product_Name,
//                    Selling_Price = p.Selling_Price,
//                    Discount_Price = p.Discount_Price,
//                    Description = p.Description,
//                    Product_Service = p.Product_Service,
//                    Type = p.Type,
//                    Record_Status = p.Record_Status,
//                    Product_Profile_Photo_ID = p.Product_Profile_Photo_ID,
//                    Assembly_Type = p.Assembly_Type,
//                    Is_Other_Product_Title = p.Is_Other_Product_Title,
//                    Branch_ID = p.Branch_ID
//                };

//                if (p.Product_Profile_Photo != null) {

//                    //var px = new Product_Profile_Photo()
//                    //{
//                    //    Photo = p.Product_Profile_Photo.Photo,
//                    //    Product_Profile_Photo_ID = p.Product_Profile_Photo.Product_Profile_Photo_ID
//                    //};

//                    //c.Product_Profile_Photo = px;
//                }

//                plst.Add(c);
//            }

//            return Json(new {
//                Product = plst.ToArray()
//            }, JsonRequestBehavior.AllowGet);
//        }

//        [AllowAnonymous]
//        [HttpGet]
//        public JsonResult LstProductColors(int pProductID, bool pActiveOnly = true) {
//            var pService = new POSService();


//            List<Product_Color> plst = new List<Product_Color>();
//            Product_Color color;

//            foreach (var prodColor in pService.LstProductColor(pProductID)) {

//                color = new Product_Color() {
//                    Product_Color_ID = prodColor.Product_Color_ID,
//                    Product_ID = prodColor.Product_ID,
//                    Color_Code = prodColor.Color_Code,
//                    Color = prodColor.Color
//                };

//                plst.Add(color);

//            }

//            return Json(new {
//                Product_Color = plst.ToArray()
//            }, JsonRequestBehavior.AllowGet);
//        }

//        [AllowAnonymous]
//        [HttpGet]
//        public JsonResult GetProductTable(int pCompanyID) {
//            var pService = new POSService();
//            List<Product_Table> pList = new List<Product_Table>();
//            Product_Table productTable;

//            var table = pService.GetProductTable(pCompanyID);

//            if (table != null) {
//                productTable = new Product_Table() {
//                    Product_Table_ID = table.Product_Table_ID,
//                    Company_ID = table.Company_ID,
//                    Prefix = table.Prefix,
//                    No_Of_Table = table.No_Of_Table
//                };

//                pList.Add(productTable);
//            }

//            return Json(new {
//                Product_Table = pList.ToArray()
//            }, JsonRequestBehavior.AllowGet);
//        }

//        [AllowAnonymous]
//        [HttpGet]
//        public JsonResult LstBranch(int pCompanyID) {
//            var pService = new POSService();
//            List<Branch> pList = new List<Branch>();

//            foreach (var b in pService.LstBranch((int?)pCompanyID)) {

//                var c = new Branch() {
//                    Company_ID = b.Company_ID,
//                    Branch_ID = b.Branch_ID,
//                    Branch_Code = b.Branch_Code,
//                    Branch_Desc = b.Branch_Desc,
//                    Branch_Name = b.Branch_Name,
//                    CL_Sun = b.CL_Sun,
//                    CL_Mon = b.CL_Mon,
//                    CL_Tue = b.CL_Tue,
//                    CL_Wed = b.CL_Wed,
//                    CL_Thu = b.CL_Thu,
//                    CL_Fri = b.CL_Fri,
//                    ST_Sun_Time = b.ST_Sun_Time,
//                    ST_Mon_Time = b.ST_Mon_Time,
//                    ST_Tue_Time = b.ST_Tue_Time,
//                    ST_Wed_Time = b.ST_Wed_Time,
//                    ST_Thu_Time = b.ST_Thu_Time,
//                    ST_Fri_Time = b.ST_Fri_Time,
//                    ST_Sat_Time = b.ST_Sat_Time,
//                    ET_Sun_Time = b.ET_Sun_Time,
//                    ET_Mon_Time = b.ET_Mon_Time,
//                    ET_Tue_Time = b.ET_Tue_Time,
//                    ET_Wed_Time = b.ET_Wed_Time,
//                    ET_Thu_Time = b.ET_Thu_Time,
//                    ET_Fri_Time = b.ET_Fri_Time,
//                    ET_Sat_Time = b.ET_Sat_Time
//                };

//                pList.Add(c);
//            }

//            return Json(new {
//                Branch = pList.ToArray()
//            }, JsonRequestBehavior.AllowGet);
//        }

//        [AllowAnonymous]
//        [HttpGet]
//        public JsonResult LstPOSTerminal(int pCompanyID) {
//            var pService = new POSService();
//            List<POS_Terminal> pList = new List<POS_Terminal>();

//            foreach (var b in pService.LstTerminal(pCompanyID)) {

//                var c = new POS_Terminal() {
//                    Terminal_ID = b.Terminal_ID,
//                    Company_ID = b.Company_ID,
//                    Cashier_ID = b.Cashier_ID,
//                    Branch_ID = b.Branch_ID,
//                    Terminal_Name = b.Terminal_Name,
//                    Host_Name = b.Host_Name,
//                    Create_By = b.Create_By,
//                    Create_On = b.Create_On,
//                    Update_By = b.Update_By,
//                    Update_On = b.Update_On,
//                    Mac_Address = b.Mac_Address,
//                    Printer_IP_Address = b.Printer_IP_Address
//                };

//                pList.Add(c);
//            }

//            return Json(new {
//                POS_Terminal = pList.ToArray()
//            }, JsonRequestBehavior.AllowGet);
//        }

//        [AllowAnonymous]
//        [HttpGet]
//        public JsonResult LstCardTypes(int pCompanyID) {
//            var pService = new POSMasterSyncService();
//            List<Global_Lookup_Data> pCardList = new List<Global_Lookup_Data>();

//            foreach (var b in pService.LstLookUpData(pCompanyID, "Credit_Card_Type")) {

//                var c = new Global_Lookup_Data() {
//                    Lookup_Data_ID = b.Lookup_Data_ID,
//                    Def_ID = b.Def_ID,
//                    Name = b.Name,
//                    Description = b.Description,
//                    Create_By = b.Create_By,
//                    Create_On = b.Create_On,
//                    Update_By = b.Update_By,
//                    Update_On = b.Update_On,
//                    Colour_Config = b.Colour_Config,
//                    Company_ID = pCompanyID
//                };

//                pCardList.Add(c);
//            }

//            return Json(new {
//                Card_Type = pCardList.ToArray()
//            }, JsonRequestBehavior.AllowGet);
//        }

//        #endregion

//        #region POST FUNCTIONS
//        [AllowAnonymous]
//        [HttpPost]
//        public JsonResult UploadPOS_Shifts([System.Web.Http.FromBody]IEnumerable<POS_Shift> pShifts) {
//            var pService = new POSMasterSyncService();
//            List<Sync_ID_Reference> pList = new List<Sync_ID_Reference>();

//            foreach (var posShift in pShifts) {

//                Models.POS_Shift shift = new Models.POS_Shift() {
//                    Company_ID = posShift.Company_ID,
//                    Branch_ID = posShift.Branch_ID,
//                    Open_Time = posShift.Open_Time,
//                    Close_Time = posShift.Close_Time,
//                    Effective_Date = posShift.Effective_Date,
//                    Total_Amount = posShift.Total_Amount,
//                    Status = posShift.Status,
//                    Create_By = posShift.Create_By,
//                    Create_On = posShift.Create_On,
//                    Update_By = posShift.Update_By,
//                    Update_On = posShift.Update_On
//                };

//                if (posShift.Is_New.Value) { //insert
//                    if (pService.InsertPOS_Shift(shift)) {

//                        pList.Add(new Sync_ID_Reference() {
//                            Table_Name = "POS_Shift",
//                            Cloud_ID = shift.Shift_ID,
//                            Local_ID = posShift.Shift_ID,
//                            Company_ID = (int)shift.Company_ID
//                        });

//                    }

//                } else { // update
//                    shift.Shift_ID = posShift.Shift_ID;
//                    pService.UpdatePOS_Shift(shift);
//                }
//            }

//            return Json(new {
//                Sync_ID_Reference = pList.ToArray()
//            }, JsonRequestBehavior.AllowGet);
//        }

//        [AllowAnonymous]
//        [HttpPost]
//        public JsonResult UploadPOS_Terminals([System.Web.Http.FromBody]IEnumerable<POS_Terminal> pTerminals) {
//            var pService = new POSMasterSyncService();
//            List<Sync_ID_Reference> pList = new List<Sync_ID_Reference>();

//            foreach (var posTerminal in pTerminals) {

//                Models.POS_Terminal terminal = new Models.POS_Terminal() {
//                    Company_ID = posTerminal.Company_ID,
//                    Cashier_ID = posTerminal.Cashier_ID,
//                    Branch_ID = posTerminal.Branch_ID,
//                    Terminal_Name = posTerminal.Terminal_Name,
//                    Host_Name = posTerminal.Host_Name,
//                    Create_By = posTerminal.Create_By,
//                    Create_On = posTerminal.Create_On,
//                    Update_By = posTerminal.Update_By,
//                    Update_On = posTerminal.Update_On,
//                    Mac_Address = posTerminal.Mac_Address,
//                    Printer_IP_Address = posTerminal.Printer_IP_Address
//                };

//                if (posTerminal.Is_New.Value) { //insert
//                    if (pService.InsertPOS_Terminal(terminal)) {

//                        pList.Add(new Sync_ID_Reference() {
//                            Table_Name = "POS_Terminal",
//                            Cloud_ID = terminal.Terminal_ID,
//                            Local_ID = posTerminal.Terminal_ID,
//                            Company_ID = (int)terminal.Company_ID
//                        });

//                    }

//                } else { // update
//                    terminal.Terminal_ID = posTerminal.Terminal_ID;
//                    pService.UpdatePOS_Terminal(terminal);
//                }
//            }

//            return Json(new {
//                Sync_ID_Reference = pList.ToArray()
//            }, JsonRequestBehavior.AllowGet);
//        }

//        [AllowAnonymous]
//        [HttpPost]
//        public JsonResult UploadPOSReceiptConfig([System.Web.Http.FromBody]IEnumerable<Receipt_Configuration> pReceiptConfigs) {
//            var pService = new POSMasterSyncService();
//            List<Sync_ID_Reference> pList = new List<Sync_ID_Reference>();

//            foreach (var rcptConfig in pReceiptConfigs) {

//                POS_Receipt_Configuration config = new POS_Receipt_Configuration() {
//                    Company_ID = rcptConfig.Company_ID,
//                    Prefix = rcptConfig.Prefix,
//                    Date_Format = rcptConfig.Date_Format,
//                    Suffix = rcptConfig.Suffix,
//                    Num_Lenght = rcptConfig.Num_Lenght,
//                    Receipt_Header = rcptConfig.Receipt_Header,
//                    Receipt_Footer = rcptConfig.Receipt_Footer,
//                    Paper_Size = rcptConfig.Paper_Size,
//                    Is_By_Branch = rcptConfig.Is_By_Branch,
//                    Surcharge_Include = rcptConfig.Surcharge_Include,
//                    Surcharge_Percen = rcptConfig.Surcharge_Percen,
//                    Printer_IP_Address = rcptConfig.Printer_IP_Address,
//                    Ref_Count = rcptConfig.Ref_Count,
//                    Ignore_Print = rcptConfig.Ignore_Print
//                };

//                if (rcptConfig.Is_New.Value) { //insert
//                    if (pService.InsertPOS_ReceiptConfig(config)) {

//                        pList.Add(new Sync_ID_Reference() {
//                            Table_Name = "POS_Receipt_Configuration",
//                            Cloud_ID = config.Receipt_Conf_ID,
//                            Local_ID = rcptConfig.Receipt_Conf_ID,
//                            Company_ID = (int)config.Company_ID
//                        });
//                    }

//                } else { // update
//                    config.Receipt_Conf_ID = rcptConfig.Receipt_Conf_ID;
//                    pService.UpdatePOS_ReceiptConfig(config);
//                }
//            }

//            return Json(new {
//                Sync_ID_Reference = pList.ToArray()
//            }, JsonRequestBehavior.AllowGet);
//        }

//        [AllowAnonymous]
//        [HttpPost]
//        public JsonResult UploadPOS_Receipts([System.Web.Http.FromBody]IEnumerable<POS_Receipt> pReceipts) {
//            var pService = new POSMasterSyncService();
//            List<Sync_ID_Reference> pList = new List<Sync_ID_Reference>();

//            foreach (var rcpt in pReceipts) {

//                Models.POS_Receipt receipt = new Models.POS_Receipt() {
//                    Receipt_No = rcpt.Receipt_No,
//                    Receipt_Date = rcpt.Receipt_Date,
//                    Total_Qty = rcpt.Total_Qty,
//                    Total_Amount = rcpt.Total_Amount,
//                    Total_Discount = rcpt.Total_Discount,
//                    Net_Amount = rcpt.Net_Amount,
//                    Cash_Payment = rcpt.Cash_Payment,
//                    Approval_Code = rcpt.Approval_Code,
//                    Voucher_Amount = rcpt.Voucher_Amount,
//                    Cashier = rcpt.Cashier,
//                    Company_ID = rcpt.Company_ID,
//                    Terminal_ID = rcpt.Terminal_ID,
//                    Promotion_ID = rcpt.Promotion_ID,
//                    Discount = rcpt.Discount,
//                    Changes = rcpt.Changes,
//                    Discount_Reason = rcpt.Discount_Reason,
//                    Card_Type = rcpt.Card_Type,
//                    Card_Branch = rcpt.Card_Branch,
//                    Status = rcpt.Status,
//                    Discount_Type = rcpt.Discount_Type,
//                    Payment_Type = rcpt.Payment_Type,
//                    Table_No = rcpt.Table_No,
//                    Remark = rcpt.Remark,
//                    Shift_ID = rcpt.Shift_ID,
//                    Total_GST_Amount = rcpt.Total_GST_Amount
//                };

//                if (rcpt.Is_New.Value) { //insert
//                    if (pService.InsertPOS_Receipt(receipt)) {

//                        pList.Add(new Sync_ID_Reference() {
//                            Table_Name = "POS_Receipt",
//                            Cloud_ID = receipt.Receipt_ID,
//                            Local_ID = rcpt.Receipt_ID,
//                            Company_ID = (int)rcpt.Company_ID
//                        });

//                    }

//                } else { // update
//                    receipt.Receipt_ID = rcpt.Receipt_ID;
//                    pService.UpdatePOS_Receipt(receipt);
//                }
//            }

//            return Json(new {
//                Sync_ID_Reference = pList.ToArray()
//            }, JsonRequestBehavior.AllowGet);
//        }

//        [AllowAnonymous]
//        [HttpPost]
//        public JsonResult UploadPOS_Receipt_Products([System.Web.Http.FromBody]IEnumerable<POS_Products_Rcp> pReceiptProducts) {
//            var pService = new POSMasterSyncService();
//            List<Sync_ID_Reference> pList = new List<Sync_ID_Reference>();

//            foreach (var prd in pReceiptProducts) {

//                Models.POS_Products_Rcp product = new Models.POS_Products_Rcp() {
//                    Receipt_ID = prd.Receipt_ID,
//                    Product_ID = prd.Product_ID,
//                    Qty = prd.Qty,
//                    Price = prd.Price,
//                    Product_Color_ID = prd.Product_Color_ID,
//                    Product_Size_ID = prd.Product_Size_ID,
//                    Product_Name = prd.Product_Name,
//                    GST = prd.GST
//                };

//                if (prd.Is_New.Value) { //insert
//                    if (pService.InsertPOS_Receipt_Products(product)) {

//                        pList.Add(new Sync_ID_Reference() {
//                            Table_Name = "POS_Products_Rcp",
//                            Cloud_ID = product.ID,
//                            Local_ID = prd.ID,
//                            Company_ID = prd.Company_ID
//                        });

//                    }
//                } else { // update
//                    product.ID = prd.ID;
//                    pService.UpdatePOS_Receipt_Products(product);
//                }
//            }

//            return Json(new {
//                Sync_ID_Reference = pList.ToArray()
//            }, JsonRequestBehavior.AllowGet);
//        }

//        [AllowAnonymous]
//        [HttpPost]
//        public JsonResult UploadPOS_Payments([System.Web.Http.FromBody]IEnumerable<POS_Receipt_Payment> pPayments) {
//            var pService = new POSMasterSyncService();
//            List<Sync_ID_Reference> pList = new List<Sync_ID_Reference>();

//            foreach (var p in pPayments) {

//                Models.POS_Receipt_Payment payment = new Models.POS_Receipt_Payment() {
//                    Receipt_ID = p.Receipt_ID,
//                    Payment_Type = p.Payment_Type,
//                    Payment_Amount = p.Payment_Amount,
//                    Approval_Code = p.Approval_Code,
//                    Card_Type = p.Card_Type,
//                    Card_Branch = p.Card_Branch,
//                };

//                if (p.Is_New.Value) { //insert
//                    if (pService.InsertPOS_Receipt_Payment(payment)) {

//                        pList.Add(new Sync_ID_Reference() {
//                            Table_Name = "POS_Receipt_Payment",
//                            Cloud_ID = payment.Receipt_Payment_ID,
//                            Local_ID = p.Receipt_Payment_ID,
//                            Company_ID = p.Company_ID
//                        });

//                    }

//                } else { // update
//                    payment.Receipt_Payment_ID = p.Receipt_Payment_ID;
//                    pService.UpdatePOS_Receipt_Payment(payment);
//                }
//            }

//            return Json(new {
//                Sync_ID_Reference = pList.ToArray()
//            }, JsonRequestBehavior.AllowGet);
//        }

//        [AllowAnonymous]
//        [HttpPost]
//        public JsonResult DeleteReceiptsFromLocal([System.Web.Http.FromBody]IEnumerable<Sync_ID_Reference> pReferences) {
//            var pService = new POSService();

//            foreach (var reference in pReferences) {
//                pService.DeletePOSReceipt(reference.Cloud_ID);
//            }

//            return Json(new {
//                Message = "Success"
//            }, JsonRequestBehavior.AllowGet);
//        }

//        //[AllowAnonymous]
//        //[HttpPost]
//        //public JsonResult UploadInvTransactions([System.Web.Http.FromBody]IEnumerable<Inventory_Transaction> pInvTransactions)
//        //{
//        //    var pService = new POSMasterSyncService();
//        //    List<Sync_ID_Reference> pList = new List<Sync_ID_Reference>();

//        //    foreach (var transaction in pInvTransactions)
//        //    {

//        //        Models.Inventory_Transaction invTrans = new Models.Inventory_Transaction()
//        //        {
//        //            Company_ID = transaction.Company_ID,
//        //            Product_ID = transaction.Product_ID,
//        //            Transaction_Type = transaction.Transaction_Type,
//        //            Qty = transaction.Qty,
//        //            Selling_Price = transaction.Selling_Price,
//        //            Receipt_ID = transaction.Receipt_ID
//        //        };

//        //        if (transaction.Is_New.Value)
//        //        { //insert
//        //            if (pService.InsertInventory_Transaction(invTrans))
//        //            {

//        //                pList.Add(new Sync_ID_Reference()
//        //                {
//        //                    Table_Name = "Inventory_Transaction",
//        //                    Cloud_ID = invTrans.Transaction_ID,
//        //                    Local_ID = transaction.Transaction_ID,
//        //                    Company_ID = transaction.Company_ID
//        //                });

//        //            }

//        //        }
//        //        else
//        //        { // update
//        //            invTrans.Transaction_ID= transaction.Transaction_ID;
//        //            pService.UpdateInventory_Transaction(invTrans);
//        //        }
//        //    }

//        //    return Json(new
//        //    {
//        //        Sync_ID_Reference = pList.ToArray()
//        //    }, JsonRequestBehavior.AllowGet);
//        //}
//        #endregion

//        #region Custom Models
//        private class User_Profile {

//            public int Profile_ID { get; set; }
//            public int? Company_ID { get; set; }
//            public int? User_Authentication_ID { get; set; }
//            public string Name { get; set; }
//            public DateTime? Registration_Date { get; set; }
//            public string User_Status { get; set; }
//            public DateTime? Latest_Connection { get; set; }
//            public Guid? User_Profile_Photo_ID { get; set; }
//            public User_Authentication User_Authentication { get; set; }
//            public User_Profile_Photo User_Profile_Photo { get; set; }
//        }

//        private class User_Authentication {
//            public int User_Authentication_ID { get; set; }
//            public string Email_Address { get; set; }
//            public string PWD { get; set; }
//            public int Login_Attempt { get; set; }
//            public bool Activated { get; set; }
//            public string ApplicationUser_Id { get; set; }
//        }

//        private class User_Profile_Photo {
//            public Guid User_Profile_Photo_ID { get; set; }
//            public byte[] Photo { get; set; }
//        }

//        private class Product_Category {
//            public int Product_Category_ID { get; set; }
//            public string Category_Name { get; set; }
//            public string Record_Status { get; set; }
//            public int? Company_ID { get; set; }

//        }

//        private class Product_Profile_Photo {
//            public Guid Product_Profile_Photo_ID { get; set; }
//            public byte[] Photo { get; set; }
//        }

//        private class Product {
//            public int Product_ID { get; set; }
//            public int? Company_ID { get; set; }
//            public int? Product_Category_ID { get; set; }
//            public string Product_Code { get; set; }
//            public string Product_Name { get; set; }
//            public decimal? Selling_Price { get; set; }
//            public decimal? Discount_Price { get; set; }
//            public string Description { get; set; }
//            public object Product_Service { get; set; }
//            public string Type { get; set; }
//            public string Record_Status { get; set; }
//            public Guid? Product_Profile_Photo_ID { get; set; }
//            public Product_Profile_Photo Product_Profile_Photo { get; set; }
//            public string Assembly_Type { get; set; }
//            public bool? Is_Other_Product_Title { get; set; }
//            public int? Branch_ID { get; set; }
//        }

//        private class Product_Color {
//            public int Product_Color_ID { get; set; }
//            public int? Product_ID { get; set; }
//            public string Color { get; set; }
//            public string Color_Code { get; set; }
//        }

//        public class POS_Shift {
//            public int Shift_ID { get; set; }
//            public int? Company_ID { get; set; }
//            public int? Branch_ID { get; set; }
//            public DateTime? Open_Time { get; set; }
//            public DateTime? Close_Time { get; set; }
//            public DateTime? Effective_Date { get; set; }
//            public decimal? Total_Amount { get; set; }
//            public string Status { get; set; }
//            public string Create_By { get; set; }
//            public DateTime? Create_On { get; set; }
//            public string Update_By { get; set; }
//            public DateTime? Update_On { get; set; }
//            public bool? Is_New { get; set; }
//        }

//        private class Product_Table {
//            public int Product_Table_ID { get; set; }
//            public int? Company_ID { get; set; }
//            public string Prefix { get; set; }
//            public int? No_Of_Table { get; set; }
//        }

//        private class Branch {
//            public int Branch_ID { get; set; }
//            public int Company_ID { get; set; }
//            public string Branch_Code { get; set; }
//            public string Branch_Name { get; set; }
//            public string Branch_Desc { get; set; }
//            public TimeSpan? ST_Sun_Time { get; set; }
//            public TimeSpan? ST_Mon_Time { get; set; }
//            public TimeSpan? ST_Tue_Time { get; set; }
//            public TimeSpan? ST_Wed_Time { get; set; }
//            public TimeSpan? ST_Thu_Time { get; set; }
//            public TimeSpan? ST_Fri_Time { get; set; }
//            public TimeSpan? ST_Sat_Time { get; set; }
//            public TimeSpan? ET_Sun_Time { get; set; }
//            public TimeSpan? ET_Mon_Time { get; set; }
//            public TimeSpan? ET_Tue_Time { get; set; }
//            public TimeSpan? ET_Wed_Time { get; set; }
//            public TimeSpan? ET_Thu_Time { get; set; }
//            public TimeSpan? ET_Fri_Time { get; set; }
//            public TimeSpan? ET_Sat_Time { get; set; }
//            public bool? CL_Sun { get; set; }
//            public bool? CL_Mon { get; set; }
//            public bool? CL_Tue { get; set; }
//            public bool? CL_Wed { get; set; }
//            public bool? CL_Thu { get; set; }
//            public bool? CL_Fri { get; set; }
//            //public bool? CL_Sat { get; set; }
//            //public bool? ST_Same { get; set; }
//            //public bool? ET_Same { get; set; }
//            //public TimeSpan? ST_Time { get; set; }
//            //public TimeSpan? ET_Time { get; set; }
//        }

//        public class Receipt_Configuration {
//            public int Receipt_Conf_ID { get; set; }
//            public int? Company_ID { get; set; }
//            public string Prefix { get; set; }
//            public string Date_Format { get; set; }
//            public string Suffix { get; set; }
//            public int? Num_Lenght { get; set; }
//            public string Receipt_Header { get; set; }
//            public string Receipt_Footer { get; set; }
//            public string Paper_Size { get; set; }
//            public bool? Is_By_Branch { get; set; }
//            public bool? Surcharge_Include { get; set; }
//            public decimal? Surcharge_Percen { get; set; }
//            public string Printer_IP_Address { get; set; }
//            public int? Ref_Count { get; set; }
//            public bool? Ignore_Print { get; set; }
//            public bool? Is_New { get; set; }
//        }

//        public class POS_Terminal {
//            public int Terminal_ID { get; set; }
//            public int? Company_ID { get; set; }
//            public int? Cashier_ID { get; set; }
//            public int? Branch_ID { get; set; }
//            public string Terminal_Name { get; set; }
//            public string Host_Name { get; set; }
//            public string Create_By { get; set; }
//            public DateTime? Create_On { get; set; }
//            public string Update_By { get; set; }
//            public DateTime? Update_On { get; set; }
//            public string Mac_Address { get; set; }
//            public string Printer_IP_Address { get; set; }
//            public bool? Is_New { get; set; }
//        }

//        public class POS_Receipt {

//            public int Receipt_ID { get; set; }
//            public string Receipt_No { get; set; }
//            public DateTime? Receipt_Date { get; set; }
//            public int? Total_Qty { get; set; }
//            public decimal? Total_Amount { get; set; }
//            public decimal? Total_Discount { get; set; }
//            public decimal? Net_Amount { get; set; }
//            public decimal? Cash_Payment { get; set; }
//            public string Approval_Code { get; set; }
//            public decimal? Voucher_Amount { get; set; }
//            public int? Cashier { get; set; }
//            public int? Company_ID { get; set; }
//            public int? Terminal_ID { get; set; }
//            public int? Promotion_ID { get; set; }
//            public decimal? Discount { get; set; }
//            public decimal? Changes { get; set; }
//            public string Discount_Reason { get; set; }
//            public int? Card_Type { get; set; }
//            public int? Card_Branch { get; set; }
//            public string Status { get; set; }
//            public string Discount_Type { get; set; }
//            public int? Payment_Type { get; set; }
//            public string Table_No { get; set; }
//            public string Remark { get; set; }
//            public int? Shift_ID { get; set; }
//            public decimal? Total_GST_Amount { get; set; }
//            public bool? Is_New { get; set; }
//        }

//        public class POS_Products_Rcp {
//            public int ID { get; set; }
//            public int? Receipt_ID { get; set; }
//            public int? Product_ID { get; set; }
//            public int? Qty { get; set; }
//            public decimal? Price { get; set; }
//            public int? Product_Color_ID { get; set; }
//            public int? Product_Size_ID { get; set; }
//            public string Product_Name { get; set; }
//            public int Company_ID { get; set; }
//            public decimal? GST { get; set; }
//            public bool? Is_New { get; set; }
//        }

//        public class POS_Receipt_Payment {
//            public int Receipt_Payment_ID { get; set; }
//            public int? Receipt_ID { get; set; }
//            public int? Payment_Type { get; set; }
//            public decimal? Payment_Amount { get; set; }
//            public string Approval_Code { get; set; }
//            public int? Card_Type { get; set; }
//            public int? Card_Branch { get; set; }
//            public int Company_ID { get; set; }
//            public bool? Is_New { get; set; }
//        }

//        public class Inventory_Transaction
//        {
//            public int Transaction_ID { get; set; }
//            public int Company_ID { get; set; }
//            public int Product_ID { get; set; }
//            public string Transaction_Type { get; set; }
//            public decimal? Qty { get; set; }
//            public decimal? Selling_Price { get; set; }
//            public int? Receipt_ID { get; set; }
//            public bool? Is_New { get; set; }
//        }

//        public class Sync_ID_Reference {
//            public int ID { get; set; }
//            public int Company_ID { get; set; }
//            public int Local_ID { get; set; }
//            public int Cloud_ID { get; set; }
//            public string Table_Name { get; set; }
//        }

//        #endregion

//    }
//}