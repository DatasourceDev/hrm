using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using POS.Models;
using POS.Common;
using System.Net.NetworkInformation;
using SBSModel.Common;
using SBSModel.Models;
using SBSModel.Offline;

namespace POS.Controllers
{
    public class POSConfigController : ControllerBase
    {
        public static int DEFAULT_RECEIPT_LENGHT = 8;

        [HttpGet]
        [AllowAuthorized]
        public ActionResult Configuration()
        {

            var userlogin = UserSession.getUser(HttpContext);
            var model = new ConfigurationViewModel();

            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            //-------cashier rights------------
            RightResult rightResult = base.validatePageRight(UserSession.RIGHT_A);
            model.rights = rightResult.rights;

            //-------supervisor rights------------
            RightResult supRightResult = base.validatePageRight(UserSession.RIGHT_A, "/POSConfig/ConfigurationAdmin");
            model.rightsSup = supRightResult.rights;

            var cService = new POSConfigService();
            var pService = new POSService();
            var bService = new BranchService();
            var taxService = new TaxServie();

            if (rightResult.action != null && supRightResult.action != null)
            {
                // terminal and  config supervisor access denine
                return rightResult.action;
            }

            POS_Terminal terminal;
            if (AppSetting.POS_OFFLINE_CLIENT)
            {
                model.Mac_Address = cService.GetMacAddress();
                //if(string.IsNullOrEmpty(  model.Mac_Address))
                //    return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, "Mac Address");

                terminal = cService.GetTerminalByMacAddress(model.Mac_Address);
            }
            else
            {
                //if (Request.Url.Host == "localhost")
                //{
                //    model.Mac_Address = UserSession.GetMAC();
                //}
                //else
                //{
                //    var hoststr = Request.ServerVariables["REMOTE_HOST"];
                //    var host = System.Net.Dns.GetHostByAddress(hoststr);
                //    if (host != null)
                //        model.Mac_Address = host.HostName;
                //}
                terminal = cService.GetTerminalByCashierID(userlogin.Profile_ID);
            }


            if (rightResult.action == null)
            {
                // terminal 
                model.isCashier = true;
                model.branchlist = bService.LstBranch(userlogin.Company_ID);
               
                if (terminal == null)
                {
                    // new terminal
                    model.operation = UserSession.RIGHT_C;
                    model.IP_Address = Request.UserHostAddress;
                    model.Cashier_ID = userlogin.Profile_ID;
                    model.Cashier_Name = UserSession.getUserName(userlogin);
                }
                else
                {
                    // edit terminal
                    model.Terminal_ID = terminal.Terminal_ID;
                    model.Terminal_Name = terminal.Terminal_Name;
                    model.operation = UserSession.RIGHT_U;
                    model.Branch_ID = terminal.Branch_ID;
                    model.Printer_IP_Address = terminal.Printer_IP_Address;
                    model.IP_Address = terminal.IP_Address;
                    model.Cashier_ID = userlogin.Profile_ID;
                    model.Cashier_Name = UserSession.getUserName(userlogin);
                    model.Is_Uploaded = terminal.Is_Uploaded;
                    model.Is_Latest = terminal.Is_Latest;
                    if (terminal.Is_WebPRNT.HasValue)
                        model.Is_WebPRNT = terminal.Is_WebPRNT.Value;
                    else
                        model.Is_WebPRNT = false;

                }
            }

            if (supRightResult.action == null)
            {
                model.isSupervisor = true;
                // config supervisor 
                var currentdate = StoredProcedure.GetCurrentDate();
                var cbService = new ComboService();
                model.dateformatlist = cbService.LstDateFormat();
                model.paperSizelist = cbService.LstPaperSize();

                model.Sample = "RP" + currentdate.Year + currentdate.Month.ToString("00") + currentdate.Day.ToString("00") + "AMK";


                var rpService = new ReceiptConfigService();
                var rpConf = rpService.GetReceiptConfigByCompany(userlogin.Company_ID.Value);
                if (rpConf != null)
                {
                    model.Date_Format = rpConf.Date_Format;
                    model.Is_By_Branch = rpConf.Is_By_Branch.Value;
                    model.Num_Lenght = rpConf.Num_Lenght.Value;
                    model.Paper_Size = rpConf.Paper_Size;
                    model.Prefix = rpConf.Prefix;
                    model.Receipt_Conf_ID = rpConf.Receipt_Conf_ID;
                    model.Receipt_Footer = rpConf.Receipt_Footer;
                    model.Receipt_Header = rpConf.Receipt_Header;
                    model.Suffix = rpConf.Suffix;
                    model.Ref_Count = rpConf.Ref_Count;
                }
                if (model.Num_Lenght == 0) model.Num_Lenght = DEFAULT_RECEIPT_LENGHT;


                var iService = new InventoryService();
                var tax = taxService.GetTax(userlogin.Company_ID);
                if (tax != null)
                {
                    model.Tax_ID = tax.Tax_ID;
                    model.Surcharge_Include = tax.Include_Surcharge.HasValue ? tax.Include_Surcharge.Value : false;
                    model.Surcharge_Percen = tax.Surcharge_Percen;

                    model.Service_Charge_Include = tax.Include_Service_Charge.HasValue ? tax.Include_Service_Charge.Value : false;
                    model.Service_Charge_Percen = tax.Service_Charge_Percen;


                }

            }




            return View(model);
        }

        [HttpPost]
        [AllowAuthorized]
        public ActionResult Configuration(ConfigurationViewModel model)
        {
            var userlogin = UserSession.getUser(HttpContext);
            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            var currentdate = StoredProcedure.GetCurrentDate();
            var cService = new POSConfigService();
            var pService = new POSService();
            var bService = new BranchService();
            if(AppSetting.POS_OFFLINE_CLIENT)
            {
                model.Mac_Address = cService.GetMacAddress();
                if(string.IsNullOrEmpty(  model.Mac_Address))
                    return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, "Mac Address");
            }

            // validate
            if (model.isSupervisor)
            {
                if (string.IsNullOrEmpty(model.Prefix))
                    ModelState.AddModelError("Prefix", "The " + Resources.ResourcePOS.Prefix + " field is required.");

                if (!model.Num_Lenght.HasValue)
                    ModelState.AddModelError("Num_Lenght", "The " + Resources.ResourcePOS.NumLenght + " field is required.");

                if (model.Surcharge_Include)
                {
                    if (!model.Surcharge_Percen.HasValue || model.Surcharge_Percen.Value == 0)
                    {
                        ModelState.AddModelError("Surcharge_Percen", "The " + Resources.ResourcePOS.SurchargePercen + " field cannot be less than zero.");
                    }
                }
                if (model.Service_Charge_Include)
                {
                    if (!model.Service_Charge_Percen.HasValue || model.Service_Charge_Percen.Value == 0)
                    {
                        ModelState.AddModelError("Service_Charge_Percen", "The " + Resources.ResourcePOS.ServiceCharge + " field cannot be less than zero.");
                    }
                }


            }
            if (model.isCashier)
            {
                if (!model.Branch_ID.HasValue)
                    ModelState.AddModelError("Branch_ID", "The " + Resources.ResourcePOS.Branch + " field is required.");

                if (string.IsNullOrEmpty(model.Terminal_Name))
                    ModelState.AddModelError("Terminal_Name", "The " + Resources.ResourcePOS.TerminalName + " field is required.");

                if (model.Surcharge_Include)
                {
                    if (!model.Surcharge_Percen.HasValue || model.Surcharge_Percen.Value <= 0)
                    {
                        ModelState.AddModelError("Surcharge_Percen", "The " + Resources.ResourcePOS.Surcharge + " field is required.");
                    }
                }
            }


            if (!string.IsNullOrEmpty(model.Terminal_Name))
            {
                var dupt = cService.GetTerminalByTerminalName(userlogin.Company_ID, model.Terminal_Name);
                if (dupt != null && dupt.Terminal_ID != model.Terminal_ID)
                {
                    ModelState.AddModelError("Terminal_Name", "The " + Resources.ResourcePOS.TerminalName + " field is duplicated.");
                }
            }
            var haveError = false;
            if (ModelState.IsValid)
            {
                if (model.isSupervisor)
                {
                    var rpService = new ReceiptConfigService();

                    POS_Receipt_Configuration rpConf = new POS_Receipt_Configuration();
                    if (model.Receipt_Conf_ID.HasValue)
                    {
                        rpConf = rpService.GetReceiptConfig(model.Receipt_Conf_ID.Value);
                    }

                    rpConf.Company_ID = userlogin.Company_ID.Value;
                    rpConf.Date_Format = model.Date_Format;
                    rpConf.Is_By_Branch = model.Is_By_Branch;
                    rpConf.Num_Lenght = model.Num_Lenght;
                    rpConf.Paper_Size = model.Paper_Size;
                    rpConf.Prefix = model.Prefix;
                    rpConf.Receipt_Footer = model.Receipt_Footer;
                    rpConf.Receipt_Header = model.Receipt_Header;
                    rpConf.Suffix = model.Suffix;
                    rpConf.Update_By = userlogin.User_Authentication.Email_Address;
                    rpConf.Update_On = currentdate;

                    if (rpConf.Num_Lenght == 0) rpConf.Num_Lenght = DEFAULT_RECEIPT_LENGHT;

                    if (model.Receipt_Conf_ID.HasValue)
                    {
                        //update
                        model.result = rpService.UpdateReceiptConfig(rpConf);
                        if (model.result.Code != ERROR_CODE.SUCCESS)
                            haveError = true;
                    }
                    else
                    {
                        //Insert
                        rpConf.Create_By = userlogin.User_Authentication.Email_Address;
                        rpConf.Create_On = currentdate;
                        model.result = rpService.InsertReceiptConfig(rpConf);
                        if (model.result.Code != ERROR_CODE.SUCCESS)
                            haveError = true;
                    }


                    var taxService = new TaxServie();
                    var tax = new Tax();
                    if (model.Tax_ID.HasValue)
                    {
                        tax = taxService.GetTax(userlogin.Company_ID);
                    }

                    tax.Include_Service_Charge = model.Service_Charge_Include;
                    tax.Service_Charge_Percen = model.Service_Charge_Percen;

                    tax.Include_Surcharge = model.Surcharge_Include;
                    tax.Surcharge_Percen = model.Surcharge_Percen;
                    tax.Update_By = userlogin.User_Authentication.Email_Address;
                    tax.Update_On = currentdate;

                    if (model.Tax_ID.HasValue)
                    {
                        //update
                        model.result = taxService.UpdateTax(tax);
                        if (model.result.Code != ERROR_CODE.SUCCESS)
                            haveError = true;
                    }
                    else
                    {
                        //Insert
                        tax.Create_By = userlogin.User_Authentication.Email_Address;
                        tax.Create_On = currentdate;

                        model.result = taxService.InsertTax(tax);
                        if (model.result.Code != ERROR_CODE.SUCCESS)
                            haveError = true;
                    }

                }

                if (model.isCashier && !haveError)
                {
                    if (model.Terminal_ID.HasValue && model.Terminal_ID.Value > 0)
                    {
                        //update       
                        var t = cService.GetTerminal(model.Terminal_ID);
                        if (t != null)
                        {
                            t.Cashier_ID = model.Cashier_ID;

                            t.Branch_ID = model.Branch_ID;
                            t.Host_Name = model.Host_Name;
                            t.Mac_Address = model.Mac_Address;
                            t.Terminal_Name = model.Terminal_Name;
                            t.Printer_IP_Address = model.Printer_IP_Address;
                            t.Is_WebPRNT = model.Is_WebPRNT;
                            t.IP_Address = model.IP_Address;
                            t.Is_Latest = false;
                            if (!AppSetting.POS_OFFLINE_CLIENT)
                            {
                                // is not user online-offline mode
                                t.Is_Uploaded = true;
                                t.Is_Latest = true;
                            }
                            t.Update_By = userlogin.User_Authentication.Email_Address;
                            t.Update_On = currentdate;

                            model.result = cService.UpdateTerminal(t);
                            if (model.result.Code == ERROR_CODE.SUCCESS)
                            {
                                if (AppSetting.POS_OFFLINE_CLIENT)
                                {
                                    var mgOffline = new ManageOffline(userlogin.Company_ID);
                                    mgOffline.SendDataToServer();
                                }
                                return RedirectToAction("POS", "POS");
                            }
                        }



                    }
                    else
                    {
                        //insert       
                        var t = new POS_Terminal();
                        t.Company_ID = userlogin.Company_ID;
                        t.Cashier_ID = model.Cashier_ID;

                        t.Branch_ID = model.Branch_ID;
                        t.Host_Name = model.Host_Name;
                        t.Mac_Address = model.Mac_Address;
                        t.Terminal_Name = model.Terminal_Name;
                        t.Printer_IP_Address = model.Printer_IP_Address;
                        t.Is_WebPRNT = model.Is_WebPRNT;
                        t.IP_Address = model.IP_Address;

                        t.Is_Uploaded = model.Is_Uploaded;
                        t.Is_Latest = model.Is_Latest;

                        if (!AppSetting.POS_OFFLINE_CLIENT)
                        {
                            t.Is_Uploaded = true;
                            t.Is_Latest = true;
                        }

                        t.Create_By = userlogin.User_Authentication.Email_Address;
                        t.Create_On = currentdate;
                        t.Update_By = userlogin.User_Authentication.Email_Address;
                        t.Update_On = currentdate;

                        model.result = cService.InsertTerminal(t);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                        {
                            if (AppSetting.POS_OFFLINE_CLIENT)
                            {
                                var mgOffline = new ManageOffline(userlogin.Company_ID);
                                mgOffline.SendDataToServer();
                            }
                            return RedirectToAction("POS", "POS");
                        }
                    }
                }
            }


            //-------cashier rights------------
            RightResult rightResult = base.validatePageRight(UserSession.RIGHT_A);
            model.rights = rightResult.rights;

            //-------supervisor rights------------
            RightResult supRightResult = base.validatePageRight(UserSession.RIGHT_A, "/POSConfig/ConfigurationAdmin");
            model.rightsSup = supRightResult.rights;


            if (rightResult.action != null && supRightResult.action != null)
            {
                // terminal and  config supervisor access denine
                return rightResult.action;
            }

            model.branchlist = bService.LstBranch(userlogin.Company_ID);

            var cbService = new ComboService();
            model.dateformatlist = cbService.LstDateFormat();
            model.paperSizelist = cbService.LstPaperSize();

            return View(model);
        }


        //-------------------------------sun-----------------------//

        [HttpGet]
        [AllowAuthorized]
        public ActionResult MemberConfiguration()
        {
            var userlogin = UserSession.getUser(HttpContext);
            var model = new MemberConfigurationViewModel();

            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            //-------Page rights------------
            RightResult rightResult = base.validatePageRight(UserSession.RIGHT_A);
            model.rights = rightResult.rights;

            var cService = new POSConfigService();
            var pService = new POSService();
            var coService = new ComboService();
            var meService = new MemberConfigService();

            model.DiscountTypeList = coService.LstDiscountType();

            var meConf = meService.GetMemberConfig(userlogin.Company_ID.Value, null);
            if (meConf != null)
            {
                model.operation = UserSession.RIGHT_U;
                model.Member_Configuration_ID = meConf.Member_Configuration_ID;
                model.Prefix = meConf.Prefix;
                model.Num_Lenght = meConf.Num_Lenght;
                model.Ref_Count = meConf.Ref_Count;
                model.Birthday_Discount = meConf.Birthday_Discount;
                model.Birthday_Discount_Type = meConf.Birthday_Discount_Type;
                model.Member_Discount = meConf.Member_Discount;
                model.Member_Discount_Type = meConf.Member_Discount_Type;
            }
            else
            {
                model.operation = UserSession.RIGHT_C;
            }

            return View(model);
        }


        [HttpPost]
        [AllowAuthorized]
        public ActionResult MemberConfiguration(MemberConfigurationViewModel model)
        {
            var userlogin = UserSession.getUser(HttpContext);
            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            //Validate Page Right
            RightResult rightResult = base.validatePageRight(model.operation);
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;

            var currentdate = StoredProcedure.GetCurrentDate();
            var meService = new MemberConfigService();
            var coService = new ComboService();

            if (string.IsNullOrEmpty(model.Prefix))
                ModelState.AddModelError("Prefix", "The " + Resources.ResourcePOS.Prefix + " field is required.");

            if (!model.Num_Lenght.HasValue)
                ModelState.AddModelError("Num_Lenght", "The " + Resources.ResourcePOS.NumLenght + " field is required.");

            if (!model.Ref_Count.HasValue)
                ModelState.AddModelError("Ref_Count", "The " + Resources.ResourcePOS.RefCount + " field is required.");

            if (model.Birthday_Discount_Type.ToLower() == "%")
            {
                if (model.Birthday_Discount.HasValue && model.Birthday_Discount.Value > 100)
                {
                    ModelState.AddModelError("Birthday_Discount", "The " + Resources.ResourcePOS.RefCount + " field is More 100%.");
                }
            }
            if (model.Member_Discount_Type.ToLower() == "%")
            {
                if (model.Member_Discount.HasValue && model.Member_Discount.Value > 100)
                {
                    ModelState.AddModelError("Member_Discount", "The " + Resources.ResourcePOS.RefCount + " field is More 100%.");
                }
            }

            if (ModelState.IsValid)
            {
                Member_Configuration meConf = new Member_Configuration();
                if (model.Member_Configuration_ID != 0)
                {
                    meConf = meService.GetMemberConfig(userlogin.Company_ID.Value, model.Member_Configuration_ID);
                }

                meConf.Company_ID = userlogin.Company_ID.Value;
                meConf.Prefix = model.Prefix;
                meConf.Num_Lenght = model.Num_Lenght;
                meConf.Ref_Count = model.Ref_Count;
                meConf.Birthday_Discount = model.Birthday_Discount;
                meConf.Birthday_Discount_Type = model.Birthday_Discount_Type;
                meConf.Member_Discount = model.Member_Discount;
                meConf.Member_Discount_Type = model.Member_Discount_Type;

                if (model.Member_Configuration_ID > 0 && model.operation == UserSession.RIGHT_U)
                {
                    //update
                    meConf.Update_By = userlogin.User_Authentication.Email_Address;
                    meConf.Update_On = currentdate;
                    model.result = meService.UpdatMemberConfig(meConf);
                }
                else if (model.operation == UserSession.RIGHT_C)
                {
                    //Insert
                    meConf.Create_By = userlogin.User_Authentication.Email_Address;
                    meConf.Create_On = currentdate;
                    model.result = meService.InsertMemberConfig(meConf);
                }

                if (model.result.Code == ERROR_CODE.SUCCESS)
                {
                    return RedirectToAction("POS", "POS");
                }
            }
            model.DiscountTypeList = coService.LstDiscountType();

            return View(model);

        }

        //-------------------------------sun-----------------------//

        [HttpGet]
        [AllowAuthorized]
        public ActionResult Shift()
        {
            var userlogin = UserSession.getUser(HttpContext);
            var model = new ShiftViewModel();

            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            //Validate Page Right
            RightResult rightResult = base.validatePageRight(UserSession.RIGHT_A);
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;

            var currentdate = StoredProcedure.GetCurrentDate();
            var pService = new POSService();
            var bService = new BranchService();
            var cService = new POSConfigService();

            POS_Terminal terminal;
            if (AppSetting.POS_OFFLINE_CLIENT)
                terminal = cService.GetTerminalByMacAddress(cService.GetMacAddress());
            else
                terminal = cService.GetTerminalByCashierID(userlogin.Profile_ID);
          
            if (terminal == null)
                return RedirectToAction("Configuration", "POSConfig");


            model.Branch_ID = terminal.Branch_ID;
            model.Terminal_ID = terminal.Terminal_ID;
            model.Terminal_Local_ID = terminal.Terminal_Local_ID;
            var currentShift = pService.GetCurrentOpenShift (userlogin.Company_ID, terminal.Terminal_ID);
            if (currentShift == null)
            {
                // open shift
                model.Effective_Date = DateUtil.ToDisplayDate(currentdate);
            }
            else
            {
                // edit shift
                model.Effective_Date = DateUtil.ToDisplayDate(currentShift.Effective_Date);
                model.Open_Time = DateUtil.ToDisplayTime(currentShift.Open_Time);
                model.Status = currentShift.Status;
                model.Shift_ID = currentShift.Shift_ID;
                model.Total_Amount = pService.GetPOSReceiptTotalAmount(currentShift.Shift_ID);
                model.Shift_Local_ID = currentShift.Shift_Local_ID;
                model.Is_Uploaded = currentShift.Is_Uploaded;
                model.Is_Latest = currentShift.Is_Latest;
            }
            model.branchlist = bService.LstBranch(userlogin.Company_ID);

            if (AppSetting.POS_OFFLINE_CLIENT)
                model.shiftlist = pService.LstPOSShift(userlogin.Company_ID, null, terminal.Terminal_ID);
            else
                model.shiftlist = pService.LstPOSShift(userlogin.Company_ID, terminal.Branch_ID, null);

            return View(model);
        }

        [HttpPost]
        [AllowAuthorized]
        public ActionResult Shift(ShiftViewModel model, int shift_ID = 0)
        {
            var userlogin = UserSession.getUser(HttpContext);
            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            var currentdate = StoredProcedure.GetCurrentDate();
            var cService = new POSConfigService();
            var pService = new POSService();
            var bService = new BranchService();
            string salesReport = "";


            if (model.Action == 1 || model.Action == 3)
            {
                if (ModelState.IsValid)
                {
                    POS_Terminal terminal;
                    if (AppSetting.POS_OFFLINE_CLIENT)
                        terminal = cService.GetTerminalByMacAddress(cService.GetMacAddress());
                    else
                        terminal = cService.GetTerminalByCashierID(userlogin.Profile_ID);

                    if (model.Shift_ID.HasValue && model.Shift_ID.Value > 0)
                    {
                        //update
                        var s = pService.GetShift(model.Shift_ID);
                        if (s != null)
                        {
                            s.Close_Time = currentdate;
                            s.Total_Amount = model.Total_Amount;
                            s.Status = ShiftStatus.Close;

                            s.Update_By = userlogin.User_Authentication.Email_Address;
                            s.Update_On = currentdate;
                            s.Is_Latest = false;
                            if (!AppSetting.POS_OFFLINE_CLIENT)
                            {
                                s.Is_Uploaded = true;
                                s.Is_Latest = true;
                            }


                            // print sales report before closing shift
                            var posTerminal = new POS_Terminal();
                            printSalesReceipt(ref salesReport, ref posTerminal);
                            model.Terminal_IP_Address = posTerminal.Printer_IP_Address;
                            model.Sales_Report_Data = salesReport;

                            model.result = cService.UpdateShift(s);

                            if (model.result.Code == ERROR_CODE.SUCCESS)
                            {
                                if (AppSetting.POS_OFFLINE_CLIENT)
                                {
                                    var mgOffline = new ManageOffline(userlogin.Company_ID);
                                    mgOffline.SendDataToServer();
                                }

                                model.Status = s.Status;
                                model.Close_Time = DateUtil.ToDisplayTime(s.Close_Time);

                                ModelState.Clear();
                            }
                        }

                    }
                    else
                    {
                        //insert 
                        var s = new POS_Shift()
                        {
                            Company_ID = userlogin.Company_ID,
                            Branch_ID = model.Branch_ID,
                            Open_Time = currentdate,
                            Effective_Date = currentdate,
                            Total_Amount = model.Total_Amount,
                            Status = ShiftStatus.Open,
                            Create_By = userlogin.User_Authentication.Email_Address,
                            Create_On = currentdate,
                            Update_By = userlogin.User_Authentication.Email_Address,
                            Update_On = currentdate
                        };

                        if (terminal != null)
                        {
                            s.Terminal_ID = terminal.Terminal_ID;
                            s.Terminal_Local_ID = terminal.Terminal_Local_ID;
                        }


                        if (!AppSetting.POS_OFFLINE_CLIENT)
                        {
                            s.Is_Uploaded = true;
                            s.Is_Latest = true;
                        }


                        model.result = cService.InsertShift(s);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                        {
                            if (AppSetting.POS_OFFLINE_CLIENT)
                            {
                                var mgOffline = new ManageOffline(userlogin.Company_ID);
                                mgOffline.SendDataToServer();
                            }
                            return RedirectToAction("POS", "POS");
                        }
                    }
                }
            }
            else if (model.Action == 2)
            {
                var posTerminal = new POS_Terminal();
                printSalesReceipt(ref salesReport, ref posTerminal);

                if (posTerminal.Is_WebPRNT.HasValue && posTerminal.Is_WebPRNT.Value)
                {
                    model.Terminal_IP_Address = posTerminal.Printer_IP_Address;
                    model.Sales_Report_Data = salesReport;
                }
            }

            //Validate Page Right
            RightResult rightResult = base.validatePageRight(UserSession.RIGHT_A);
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;

            model.branchlist = bService.LstBranch(userlogin.Company_ID);
            if (AppSetting.POS_OFFLINE_CLIENT)
                model.shiftlist = pService.LstPOSShift(userlogin.Company_ID, null, model.Terminal_ID);
            else
                model.shiftlist = pService.LstPOSShift(userlogin.Company_ID, model.Branch_ID, null);
            return View(model);
        }

        [HttpPost]
        [AllowAuthorized]
        public ActionResult PrintSalesReport(ShiftViewModel model)
        {
            string salesReport = "";
            var posTerminal = new POS_Terminal();
            printSalesReceipt(ref salesReport, ref posTerminal);

            return RedirectToAction("Shift", new { srepdata = salesReport });
        }

        public ActionResult PrintSalesReport(Nullable<int> shiftID)
        {
            printSalesReceipt(shiftID.Value);
            return RedirectToAction("Shift");
        }

        private bool printSalesReceipt(ref string salesReport, ref POS_Terminal posTerminal, bool isClosing = false)
        {
            bool result = false;

            try
            {

                var userlogin = UserSession.getUser(HttpContext);
                var pService = new POSService();
                var bService = new BranchService();
                var rcpService = new ReceiptConfigService();
                var cService = new POSConfigService();
                var taxService = new TaxServie();
                var company = new CompanyService().GetCompany(userlogin.Company_ID);

                var tax = taxService.GetTax(userlogin.Company_ID);
                var receiptConfig = new ReceiptConfigService().GetReceiptConfigByCompany(userlogin.Company_ID);

                var macaddress = "C" + userlogin.Company_ID + System.Net.Dns.GetHostName();

                POS_Terminal terminal;
                if (AppSetting.POS_OFFLINE_CLIENT)
                    terminal = cService.GetTerminalByMacAddress(cService.GetMacAddress());
                else
                    terminal = cService.GetTerminalByCashierID(userlogin.Profile_ID);


                var shift = cService.GetOpenShift(terminal.Company_ID.Value, terminal.Branch_ID.Value, userlogin.User_Authentication.Email_Address);

                var cri = new POSReciptCriteria();
                cri.Company_ID = userlogin.Company_ID;
                cri.Cashier_ID = userlogin.Profile_ID;
                cri.Start_Date = shift.Open_Time;
                cri.Branch_ID = terminal.Branch_ID;
                cri.User_Authentication_ID = userlogin.User_Authentication_ID;
                cri.orderByAsc = false;
                cri.includeTime = true;
                cri.isByCashier = true;
                List<POS_Receipt> rcps = pService.LstPOSReceipt(cri);


                if (terminal.Is_WebPRNT.HasValue && !terminal.Is_WebPRNT.Value)
                {
                    salesReport = ReportUtil.dailySalesData(rcps, company, receiptConfig, terminal, tax, isClosing);
                    var msg = ReportUtil.printToPrinter(salesReport, terminal.Printer_IP_Address);
                }
                else
                {
                    salesReport = ReportUtil.webPrntSalesData(rcps, company, receiptConfig, terminal, tax, isClosing);
                }
                result = true;

            }
            catch
            {
                result = false;
            }

            return result;
        }

        private bool printSalesReceipt(int shiftID)
        {
            bool result = false;
            try
            {
                var userlogin = UserSession.getUser(HttpContext);
                var pService = new POSService();
                var rcpService = new ReceiptConfigService();
                var cService = new POSConfigService();
                var taxService = new TaxServie();

                var tax = taxService.GetTax(userlogin.Company_ID);
                var company = new CompanyService().GetCompany(userlogin.Company_ID);

                var rcp_config = new ReceiptConfigService().GetReceiptConfigByCompany(userlogin.Company_ID);

                var macaddress = "C" + userlogin.Company_ID + System.Net.Dns.GetHostName();


                var shift = cService.GetShift(shiftID);

                string salesReport = "";
                var cri = new POSReciptCriteria();
                cri.Company_ID = userlogin.Company_ID;
                cri.Cashier_ID = userlogin.Profile_ID;
                cri.Start_Date = shift.Open_Time;
                cri.Branch_ID = shift.POS_Terminal.Branch_ID;
                cri.User_Authentication_ID = userlogin.User_Authentication_ID;
                cri.includeTime = true;
                cri.isByCashier = true;
                List<POS_Receipt> rcps = pService.LstPOSReceipt(cri);


                salesReport = ReportUtil.dailySalesData(rcps, company, rcp_config, shift.POS_Terminal, tax, true, shift.Close_Time);

                var msg = ReportUtil.printToPrinter(salesReport, shift.POS_Terminal.Printer_IP_Address);

                result = true;

            }
            catch
            {
                result = false;
            }

            return result;
        }

    }
}