using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using POS.Models;
using POS.Common;
using System.IO;
using System.Web.Configuration;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.html;
using System.Text;
using System.Management;
using System.Management.Instrumentation;
using SBSModel.Common;
using SBSModel.Models;
using SBSModel.Offline;

namespace POS.Controllers {

    [Authorize]
    public class POSController : ControllerBase {
        // Page Action
        // action 1 = New Sale
        // action 2 = Report
        // action 3 = Hold Bill
        // action 4 = Back order

        // Action
        // action 1 : new sale page
        // action 2 : process payment page
        // action 3 : print receipt and back to new sale
        // action 4 : print receipt
        // action 5 : hold bill         
        // action 6 : hold bill and print receipt
        //
        // GET: /POS/



        [HttpGet]
        [AllowAuthorized]
        public ActionResult POS(POSAction pAction = POSAction.NewSale, string operation = "C", int pReceiptID = 0, int pPageAction = 1, int searchAction = 0) {
            var model = new POSViewModel();
            var userlogin = UserSession.getUser(HttpContext);
            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);


            var iService = new InventoryService();
            var rcpService = new ReceiptConfigService();
            var taxService = new TaxServie();
            var pService = new POSService();
            var cService = new CompanyService();
            var cbService = new ComboService();
            var currentdate = StoredProcedure.GetCurrentDate();
            var confService = new POSConfigService();

            POS_Terminal terminal;
            if (AppSetting.POS_OFFLINE_CLIENT)
                terminal = confService.GetTerminalByMacAddress(confService.GetMacAddress());
            else
                terminal = confService.GetTerminalByCashierID(userlogin.Profile_ID);

            if (terminal == null)
                return RedirectToAction("Configuration", "POSConfig");

            var rcpConfig = rcpService.GetReceiptConfigByCompany(userlogin.Company_ID.Value);
            if (rcpConfig == null) {
                //-------supervisor rights------------
                RightResult supRightResult = base.validatePageRight(UserSession.RIGHT_A, "/POSConfig/ConfigurationAdmin");
                if (supRightResult.action != null)
                    return errorPage(ERROR_CODE.ERROR_20_NO_RECEIPT_CONFIG);

                return RedirectToAction("Configuration", "POSConfig");

            }

            var shift = pService.GetCurrentOpenShift(userlogin.Company_ID, terminal.Terminal_ID);
            if (shift == null)
                return RedirectToAction("Shift", "POSConfig");

            //Validate Page Right
            RightResult rightResult = base.validatePageRight(operation);
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;

            var comp = new CompanyService().GetCompany(userlogin.Company_ID);
            if (comp != null) {
                model.Business_Type = comp.Business_Type;
                if (comp.Business_Type == BusinessType.FoodAndBeverage) {
                    model.productTable = pService.GetProductTable(userlogin.Company_ID);
                }

                if (comp.Currency != null)
                    model.Currency_Code = comp.Currency.Currency_Code;
            }


            model.Receipt_ID = pReceiptID;
            model.operation = operation;
            model.Page_Action = pPageAction;
            model.Company_ID = userlogin.Company_ID;



            //Added by sun 13-11-2015
            model.CategoryLVList = cbService.LstCategoryLV(true);
            //Added by gali 14-JAN-2016
            model.CategoryList = cbService.LstCategory(userlogin.Company_ID, false);

            var cardtypelist = cbService.LstLookup(ComboType.Credit_Card_Type, userlogin.Company_ID.Value);
            model.Master = false;
            model.Visa = false;
            model.AMEX = false;
            model.DinersClub = false;
            model.JCB = false;

            //Added By Jane 07-12-2015
            var cardMaster = 0;
            var cardVisa = 0;
            var cardAMEX = 0;
            var cardDiner = 0;
            var cardJCB = 0;
            foreach (var row in cardtypelist) {
                if (row.Text == CreditCardType.MasterCard) {
                    model.Master = true;
                    cardMaster = NumUtil.ParseInteger(row.Value);
                }
                if (row.Text == CreditCardType.Visa) {
                    model.Visa = true;
                    cardVisa = NumUtil.ParseInteger(row.Value);
                }
                if (row.Text == CreditCardType.AMEX) {
                    model.AMEX = true;
                    cardAMEX = NumUtil.ParseInteger(row.Value);
                }
                if (row.Text == CreditCardType.DinersClub) {
                    model.DinersClub = true;
                    cardDiner = NumUtil.ParseInteger(row.Value);
                }
                if (row.Text == CreditCardType.JCB) {
                    model.JCB = true;
                    cardJCB = NumUtil.ParseInteger(row.Value);
                }
            }

            var tax = taxService.GetTax(userlogin.Company_ID);
            if (tax != null) {
                model.Surcharge_Include = tax.Include_Surcharge.HasValue ? tax.Include_Surcharge.Value : false;

                if (model.Surcharge_Include) {
                    model.Surcharge_Percen = tax.Surcharge_Percen;
                    model.Surcharge_Master = tax.Surcharge_Percen;
                    model.Surcharge_Visa = tax.Surcharge_Percen;
                    model.Surcharge_AMEX = tax.Surcharge_Percen;
                    model.Surcharge_Diner = tax.Surcharge_Percen;
                    model.Surcharge_JCB = tax.Surcharge_Percen;
                    foreach (var row in tax.Tax_Surcharge) {
                        if (row.Tax_Title == cardMaster) {
                            model.Surcharge_Master = row.Tax;
                        } else if (row.Tax_Title == cardVisa) {
                            model.Surcharge_Visa = row.Tax;
                        } else if (row.Tax_Title == cardAMEX) {
                            model.Surcharge_AMEX = row.Tax;
                        } else if (row.Tax_Title == cardDiner) {
                            model.Surcharge_Diner = row.Tax;
                        } else if (row.Tax_Title == cardJCB) {
                            model.Surcharge_JCB = row.Tax;
                        }
                    }
                }

                model.Service_Charge_Include = tax.Include_Service_Charge.HasValue ? tax.Include_Service_Charge.Value : false;

                if (model.Service_Charge_Include)
                    model.Service_Charge_Rate = tax.Service_Charge_Percen;

                model.useGST = tax.Include_GST.HasValue ? tax.Include_GST.Value : false;
                var gst = tax.Tax_GST.Where(w => w.Tax_Type == TaxType.Exclusive && w.Record_Status == RecordStatus.Active && w.Is_Default == true).FirstOrDefault();
                if (gst != null) {
                    model.valGST = gst.Tax;
                }
            }

            model.Action = POSAction.NewSale;
            model.Discount_Type = DiscountType.Amount;

            var branchID = terminal.Branch_ID;
            if (model.operation == UserSession.RIGHT_C) {
                var pcri = new ProductCriteria() {
                    Company_ID = userlogin.Company_ID,
                    Branch_ID = branchID,
                    User_Authentication_ID = userlogin.User_Authentication_ID,
                    Record_Status = RecordStatus.Active,
                    hasBlank = true
                };
                model.products = (List<Product>)iService.LstProduct(pcri).Object;

                //Modified by Nay on 14-Oct-2015 
                //Do not allow to show the Categoreis which are don't have procuts
                var ccri = new CategoryCriteria() {
                    Company_ID = userlogin.Company_ID,
                    Branch_ID = branchID,
                    hasBlank = true,
                    Record_Status = RecordStatus.Active
                };
                model.productCategory = iService.LstCategory(ccri);

                model.Total_Discount = 0.00M;
                model.Changes = 0.00M;
            } else if (model.operation == UserSession.RIGHT_U) {
                model.Action = POSAction.ProcessPayment;
                if (model.Receipt_ID.HasValue) {
                    POS_Receipt rcp = pService.GetPOSReceipt(model.Receipt_ID);

                    if (rcp == null) {
                        return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);
                    }

                    if (rcp.Company_ID != userlogin.Company_ID.Value) {
                        return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);
                    }
                    if (pAction == POSAction.NewSale && rcp.Status == ReceiptStatus.Hold) {
                        //back from hold bill

                        var posTerminal = pService.GetTerminal(userlogin.Profile_ID);
                        model.Action = POSAction.NewSale;
                        //Added By sun

                        //Modified by Nay on 14-Oct-2015 
                        //Do not allow to show the Categoreis which are don't have procuts
                        var ccri = new CategoryCriteria() {
                            Company_ID = userlogin.Company_ID,
                            Branch_ID = posTerminal.Branch_ID,
                            hasBlank = true,
                            Record_Status = RecordStatus.Active
                        };
                        model.productCategory = iService.LstCategory(ccri);

                        //Modified by Nay on 04-Sept-2015
                        //To show correct product list if user picked from Category!                        
                        if (Session["pCategoryID"] != null) {

                            model.Product_Category_ID = Convert.ToInt16(Session["pCategoryID"]);

                            var pcri = new ProductCriteria() {
                                Company_ID = userlogin.Company_ID,
                                Branch_ID = posTerminal.Branch_ID,
                                User_Authentication_ID = userlogin.User_Authentication_ID,
                                Category_ID = model.Product_Category_ID,
                                Record_Status = RecordStatus.Active
                            };
                            model.products = (List<Product>)iService.LstProduct(pcri).Object;
                        } else {
                            var pcri = new ProductCriteria() {
                                Company_ID = userlogin.Company_ID,
                                Branch_ID = posTerminal.Branch_ID,
                                User_Authentication_ID = userlogin.User_Authentication_ID,
                                Record_Status = RecordStatus.Active
                            };
                            model.products = (List<Product>)iService.LstProduct(pcri).Object;
                        }
                    } else {
                        model.banks = cbService.LstLookup(ComboType.Bank_Name, userlogin.Company_ID.Value, false);
                        model.cardTypes = cardtypelist;
                    }

                    model.Receipt_ID = rcp.Receipt_ID;
                    model.Receipt_No = rcp.Receipt_No;

                    model.Changes = rcp.Changes.HasValue ? rcp.Changes.Value : 0;
                    model.Discount = rcp.Discount.HasValue ? rcp.Discount.Value : 0;
                    model.Net_Amount = rcp.Net_Amount.HasValue ? rcp.Net_Amount.Value : 0;
                    model.Receipt_Date = DateUtil.ToDisplayDate(currentdate);
                    model.Total_Amount = rcp.Total_Amount.HasValue ? rcp.Total_Amount.Value : 0;
                    model.Total_Discount = rcp.Total_Discount.HasValue ? rcp.Total_Discount.Value : 0;
                    model.Total_Qty = rcp.Total_Qty.HasValue ? rcp.Total_Qty.Value : 0;

                    model.Discount_Type = rcp.Discount_Type;
                    model.Payment_Type = rcp.Payment_Type;
                    model.Table_No = rcp.Table_No;
                    model.Select_Member_ID = rcp.Member_ID;
                    model.Select_Member_Discount = rcp.Member_Discount;
                    model.Select_Member_Discount_Type = rcp.Member_Discount_Type;
                    model.Select_Birthday = rcp.Is_Birthday_Discount.HasValue ? rcp.Is_Birthday_Discount.Value : false;

                    //Added by Nay on 06-Aug-2015
                    if (rcp.Total_GST_Amount.HasValue) {
                        model.Total_GST_Amount = rcp.Total_GST_Amount.Value;
                    }

                    model.Status = rcp.Status;
                    if (rcp.Status == ReceiptStatus.BackOrder) {
                        model.Back_Order = true;
                    }

                    model.Remark = rcp.Remark;
                    model.Customer_Name = rcp.Customer_Name;
                    model.Contact_No = rcp.Contact_No;
                    model.Customer_Email = rcp.Customer_Email;

                    if (rcp.Payment_Type == PaymentType.Credit_Card) {
                        model.Pos_Select_Mode = PosPaymentMode.Credit;
                    } else if (rcp.Payment_Type == PaymentType.Cash) {
                        model.Pos_Select_Mode = PosPaymentMode.Cash;
                    } else if (rcp.Payment_Type == PaymentType.Nets) {
                        model.Pos_Select_Mode = PosPaymentMode.Nets;
                    } else if (rcp.Payment_Type == PaymentType.Redeem) {
                        model.Pos_Select_Mode = PosPaymentMode.Member;
                    }

                    var mService = new MemberService();
                    var mcri = new MemberCriteria() {
                        Company_ID = userlogin.Company_ID,
                        Member_ID = rcp.Member_ID,
                    };
                    model.MemberList = mService.LstMember(mcri);

                    List<POSProductViewModel> posProducts = new List<POSProductViewModel>();

                    if (pAction == POSAction.NewSale && rcp.Status == ReceiptStatus.Hold) {
                        if (Session["pProductRow"] != null) {
                            var productRows = Session["pProductRow"] as List<POSProductViewModel>;
                            foreach (var row in productRows) {
                                var p = iService.GetProduct(row.Product_ID);
                                if (p != null) {
                                    POSProductViewModel product = new POSProductViewModel();
                                    product.ID = row.ID;
                                    if (row.Qty != 0)
                                        product.Qty = row.Qty;
                                    else
                                        product.Qty = 1;

                                    product.Price = row.Price;
                                    product.Product_ID = row.Product_ID.HasValue ? row.Product_ID.Value : -1;
                                    product.Code = row.Code;
                                    product.Name = row.Name;
                                    product.GST = row.GST;
                                    product.Discount = row.Discount;
                                    product.Discount_Type = row.Discount_Type;
                                    product.Receipt_Product_Local_ID = row.Receipt_Product_Local_ID;
                                    product.Receipt_Local_ID = row.Receipt_Local_ID;
                                    posProducts.Add(product);
                                }
                            }
                        }
                        //Modified by Nay on 04-Sept-2015
                        //row type of products                      
                    } else {
                        if (rcp.POS_Products_Rcp != null && rcp.POS_Products_Rcp.Count() > 0) {
                            foreach (POS_Products_Rcp row in rcp.POS_Products_Rcp.ToList()) {
                                var p = iService.GetProduct(row.Product_ID);
                                if (p != null) {
                                    POSProductViewModel product = new POSProductViewModel();
                                    product.ID = row.ID;
                                    product.Qty = row.Qty.HasValue ? row.Qty.Value : 1;
                                    product.Price = row.Price.Value;
                                    product.Product_ID = row.Product_ID.HasValue ? row.Product_ID.Value : -1;
                                    product.Code = row.Product != null ? row.Product.Product_Code : "";
                                    product.Name = row.Product_Name;
                                    product.Discount = row.Discount;
                                    product.Discount_Type = row.Discount_Type;
                                    product.Receipt_Product_Local_ID = row.Receipt_Product_Local_ID;
                                    product.Receipt_Local_ID = row.Receipt_Local_ID;
                                    if (row.GST.HasValue)
                                        product.GST = row.GST.Value;

                                    posProducts.Add(product);
                                }
                            }
                        }
                    }

                    model.Product_Rows = posProducts.ToArray<POSProductViewModel>();

                    if (rcp.POS_Receipt_Payment != null) {
                        foreach (var p in rcp.POS_Receipt_Payment) {
                            if (p.Payment_Type == PaymentType.Cash) {
                                model.Cash_Payment = p.Payment_Amount.HasValue ? p.Payment_Amount.Value : 0;
                            } else if (p.Payment_Type == PaymentType.Nets) {
                                model.Nets_Payment = p.Payment_Amount.HasValue ? p.Payment_Amount.Value : 0;
                            } else if (p.Payment_Type == PaymentType.Credit_Card) {
                                //Added By Jane 07-12-2015
                                foreach (var row in cardtypelist) {
                                    if (NumUtil.ParseInteger(row.Value) == p.Card_Type) {
                                        if (row.Text == CreditCardType.MasterCard) {
                                            model.Master_Card_Payment = p.Payment_Amount.HasValue ? p.Payment_Amount.Value : 0;
                                        } else if (row.Text == CreditCardType.Visa) {
                                            model.Visa_Payment = p.Payment_Amount.HasValue ? p.Payment_Amount.Value : 0;
                                        } else if (row.Text == CreditCardType.AMEX) {
                                            model.AMEX_Payment = p.Payment_Amount.HasValue ? p.Payment_Amount.Value : 0;
                                        } else if (row.Text == CreditCardType.DinersClub) {
                                            model.Diners_Club_Payment = p.Payment_Amount.HasValue ? p.Payment_Amount.Value : 0;
                                        } else if (row.Text == CreditCardType.JCB) {
                                            model.JCB_Payment = p.Payment_Amount.HasValue ? p.Payment_Amount.Value : 0;
                                        }
                                        break;
                                    }

                                }
                            } else if (p.Payment_Type == PaymentType.Redeem) {
                                model.Redeem_Credits = p.Payment_Amount.HasValue ? p.Payment_Amount.Value : 0;
                            }
                        }

                        if (rcp.POS_Receipt_Payment.Count > 1) {
                            model.Combine_Payment = true;
                        }
                    }
                } else {
                    return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);
                }

            }
            return View(model);
        }

        [HttpPost]
        [AllowAuthorized]
        public ActionResult POS(POSViewModel model, int searchAction = 0) {

            User_Profile userlogin = UserSession.getUser(HttpContext);
            if (userlogin == null) {
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);
            }

            //Validate Page Right
            RightResult rightResult = base.validatePageRight(model.operation);
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;

            var cService = new POSConfigService();
            var iService = new InventoryService();
            var pService = new POSService();
            var rcpService = new ReceiptConfigService();
            var taxService = new TaxServie();
            var cbService = new ComboService();
            var currentdate = StoredProcedure.GetCurrentDate();

            POS_Terminal posTerminal;
            if (AppSetting.POS_OFFLINE_CLIENT)
                posTerminal = cService.GetTerminalByMacAddress(cService.GetMacAddress());
            else
                posTerminal = cService.GetTerminalByCashierID(userlogin.Profile_ID);

            var tax = taxService.GetTax(userlogin.Company_ID);

            //Added by sun 13-11-2015
            model.CategoryLVList = cbService.LstCategoryLV(true);

            var receipt = "";
            var ipAddress = "";

            if (model.Action == POSAction.NewSale) {
                //Added by sun 13-11-2015  
                //reload Dlg if Product_Category_ID == All
                if (model.Product_Category_ID.HasValue && model.Product_Category_ID.Value == 0) {
                    model.TextSearch = null;
                    model.Category_LV = 0;
                }

                //search product or back click
                if (model.Page_Action == 1) {
                    //Added new parameter in function to search as "global search" by Nay
                    //Purpose : even filtered by category, fill in search box and click on button to search prodcuts which are match with current search results. 
                    if (searchAction == 1) {
                        var pcri = new ProductCriteria() {
                            Company_ID = userlogin.Company_ID,
                            Branch_ID = posTerminal.Branch_ID,
                            User_Authentication_ID = userlogin.User_Authentication_ID,
                            Text_Search = model.searchProduct,
                            Record_Status = RecordStatus.Active
                        };
                        model.products = (List<Product>)iService.LstProduct(pcri).Object;

                    } else {
                        if (model.Product_Category_ID.HasValue && model.Product_Category_ID.Value == 0) model.Product_Category_ID = null;
                        var pcri = new ProductCriteria() {
                            Company_ID = userlogin.Company_ID,
                            Branch_ID = posTerminal.Branch_ID,
                            User_Authentication_ID = userlogin.User_Authentication_ID,
                            Category_ID = model.Product_Category_ID,
                            Record_Status = RecordStatus.Active

                        };
                        model.products = (List<Product>)iService.LstProduct(pcri).Object;

                        //Added by sun 13-11-2015
                        model.Ch_Category_Di = false;
                    }


                    //Modified by Nay on 14-Oct-2015 
                    //Do not allow to show the Categoreis which are don't have procuts
                    var ccri = new CategoryCriteria() {
                        Company_ID = userlogin.Company_ID,
                        Branch_ID = posTerminal.Branch_ID,
                        hasBlank = true,
                        Record_Status = RecordStatus.Active,

                        //Added by sun 13-11-2015
                        Category_Name = model.TextSearch,
                        Category_LV = model.Category_LV
                    };

                    model.productCategory = iService.LstCategory(ccri);

                    if (model.Business_Type == BusinessType.FoodAndBeverage) {
                        model.productTable = pService.GetProductTable(userlogin.Company_ID);
                    }

                    //Modified by Nay on 15-Jun-2015 
                    //Purpose : do not re-create new object if Receipt_ID is coming with zero value.
                    //if (!string.IsNullOrEmpty(model.Receipt_No) | model.Receipt_ID.HasValue)
                    if (!string.IsNullOrEmpty(model.Receipt_No) | (model.Receipt_ID.HasValue && model.Receipt_ID > 0)) {
                        // Back when save receipt complete
                        model.Product_Rows = new List<POSProductViewModel>().ToArray();
                        model.Product_Category_ID = null;
                        model.operation = UserSession.RIGHT_C;
                        model.Table_No = "";
                    }
                    model.Pos_Select_Mode = PosPaymentMode.Cash;
                    model.Payment_Type = PaymentType.Cash;
                    model.Discount = 0;
                    model.Discount_Type = DiscountType.Amount;
                    model.Card_Branch = null;
                    model.Card_Type = null;
                    model.Receipt_No = null;
                    model.Receipt_ID = null;
                    model.Receipt_Date = null;
                    model.Cash_Payment = 0;
                    model.Credit_Card_Payment = 0;
                    model.Nets_Payment = 0;
                    model.Net_Amount = 0;
                    model.Changes = 0;
                    model.Status = null;
                    model.Combine_Payment = false;
                    ModelState.Clear();

                } else if (model.Page_Action == 2) {
                    return RedirectToAction("Report", "POS");
                } else if (model.Page_Action == 3) {
                    if (model.Status == ReceiptStatus.Hold) {
                        //Modified by Nay on 04-Sept-2015
                        //Added session keys to keep user selected CategoryID, product rows & types to pass as parameter.
                        Session["pCategoryID"] = model.Product_Category_ID;
                        Session["pProductRow"] = model.Product_Rows.ToList();

                        return RedirectToAction("POS", "POS", new { operation = UserSession.RIGHT_U, pReceiptID = model.Receipt_ID, pPageAction = 3, pAction = 1 });
                    } else {
                        return RedirectToAction("ViewHoldBill", "POS");
                    }

                }
            } else if (model.Action == POSAction.ProcessPayment) {
                // goto process receipt
                if (model.Is_Text_Search_Member) {
                    // clear selected member
                    model.Select_Member_ID = null;
                    model.Select_Birthday = false;
                    model.Redeem_Credits = null;
                    model.Select_Member_Discount = null;
                    model.Select_Member_Discount_Type = null;
                }
                var mService = new MemberService();
                var mcri = new MemberCriteria() {
                    Company_ID = userlogin.Company_ID,
                    Member_ID = model.Select_Member_ID,
                    Text_Search = model.Text_Search_Member
                };
                model.MemberList = mService.LstMember(mcri);
                var conf = mService.GetMemberConfig(userlogin.Company_ID);
                if (conf != null) {
                    model.Member_Discount = conf.Member_Discount;
                    model.Member_Discount_Type = conf.Member_Discount_Type;
                    model.Birthday_Discount = conf.Birthday_Discount;
                    model.Birthday_Discount_Type = conf.Birthday_Discount_Type;
                }

                model.banks = cbService.LstLookup(ComboType.Bank_Name, userlogin.Company_ID.Value, false);
                model.cardTypes = cbService.LstLookup(ComboType.Credit_Card_Type, userlogin.Company_ID.Value, false);

                if (string.IsNullOrEmpty(model.Pos_Select_Mode))
                    model.Pos_Select_Mode = PosPaymentMode.Cash;

                ModelState.Clear();
            } else if (model.Action == POSAction.SaveAndBacktoNewSale | model.Action == POSAction.SaveAndPrintReceipt | model.Action == POSAction.HoldBill | model.Action == POSAction.HoldBillAndPrintReceipt) {

                var mService = new MemberService();
                var mcri = new MemberCriteria() {
                    Company_ID = userlogin.Company_ID,
                    Member_ID = model.Select_Member_ID,
                };
                model.MemberList = mService.LstMember(mcri);


                POS_Receipt rcp = new POS_Receipt();
                if (model.operation == UserSession.RIGHT_C | model.operation == UserSession.RIGHT_U) {
                    if (model.operation == UserSession.RIGHT_U) {
                        if (model.Receipt_ID.HasValue && model.Receipt_ID.Value > 0) {
                            rcp = pService.GetPOSReceipt(model.Receipt_ID.Value);
                            rcp.User_Profile = null;
                        }

                    }
                    var posPayments = new List<POS_Receipt_Payment>();
                    rcp.Company_ID = userlogin.Company_ID;
                    rcp.Cashier = userlogin.Profile_ID;

                    //Edit by Jane 07-12-2015
                    if (model.Combine_Payment) {
                        if (model.Cash_Payment > 0)
                            posPayments.Add(CreateCashPayment(model.Cash_Payment));
                        if (model.Nets_Payment > 0)
                            posPayments.Add(CreateNetsPayment(model.Nets_Payment));
                        if (model.Redeem_Credits > 0)
                            posPayments.Add(CreateRedeemPayment(model.Redeem_Credits));
                        if (model.Visa_Payment > 0)
                            posPayments.Add(CreateCreditCardPayment(model.Visa_Payment, userlogin.Company_ID, model.Card_Type_Name));
                        if (model.Master_Card_Payment > 0)
                            posPayments.Add(CreateCreditCardPayment(model.Master_Card_Payment, userlogin.Company_ID, model.Card_Type_Name));
                        if (model.AMEX_Payment > 0)
                            posPayments.Add(CreateCreditCardPayment(model.AMEX_Payment, userlogin.Company_ID, model.Card_Type_Name));
                        if (model.Diners_Club_Payment > 0)
                            posPayments.Add(CreateCreditCardPayment(model.Diners_Club_Payment, userlogin.Company_ID, model.Card_Type_Name));
                        if (model.JCB_Payment > 0)
                            posPayments.Add(CreateCreditCardPayment(model.JCB_Payment, userlogin.Company_ID, model.Card_Type_Name));
                    } else {
                        if (model.Payment_Type == PaymentType.Cash) {
                            posPayments.Add(CreateCashPayment(model.Cash_Payment));
                        } else if (model.Payment_Type == PaymentType.Nets) {
                            posPayments.Add(CreateNetsPayment(model.Nets_Payment));
                        } else if (model.Payment_Type == PaymentType.Redeem) {
                            posPayments.Add(CreateRedeemPayment(model.Redeem_Credits));
                        } else if (model.Payment_Type == PaymentType.Credit_Card) {
                            if (model.Visa_Payment > 0)
                                posPayments.Add(CreateCreditCardPayment(model.Visa_Payment, userlogin.Company_ID, model.Card_Type_Name));
                            else if (model.Master_Card_Payment > 0)
                                posPayments.Add(CreateCreditCardPayment(model.Master_Card_Payment, userlogin.Company_ID, model.Card_Type_Name));
                            else if (model.AMEX_Payment > 0)
                                posPayments.Add(CreateCreditCardPayment(model.AMEX_Payment, userlogin.Company_ID, model.Card_Type_Name));
                            else if (model.Diners_Club_Payment > 0)
                                posPayments.Add(CreateCreditCardPayment(model.Diners_Club_Payment, userlogin.Company_ID, model.Card_Type_Name));
                            else if (model.JCB_Payment > 0)
                                posPayments.Add(CreateCreditCardPayment(model.JCB_Payment, userlogin.Company_ID, model.Card_Type_Name));
                        }
                    }

                    rcp.POS_Receipt_Payment = posPayments;

                    rcp.Cash_Payment = model.Cash_Payment;
                    rcp.Changes = model.Changes;
                    rcp.Discount = model.Discount;
                    rcp.Discount_Type = model.Discount_Type;
                    rcp.Member_ID = model.Select_Member_ID;
                    rcp.Member_Discount = model.Select_Member_Discount;
                    rcp.Member_Discount_Type = model.Select_Member_Discount_Type;
                    rcp.Is_Birthday_Discount = model.Select_Birthday;

                    if (model.Discount_Type == DiscountType.Percen) {
                        rcp.Total_Discount = model.Total_Amount * (model.Discount / 100);
                    } else {
                        rcp.Total_Discount = model.Discount;
                    }
                    rcp.Net_Amount = model.Net_Amount;
                    rcp.Receipt_Date = currentdate;
                    rcp.Total_Amount = model.Total_Amount;
                    rcp.Total_Qty = model.Total_Qty;
                    rcp.Payment_Type = model.Payment_Type;
                    rcp.Service_Charge_Rate = model.Service_Charge_Rate;
                    rcp.Service_Charge = model.Service_Charge_Amount;
                    //Added by NayThway on 30-May-2015
                    rcp.Total_GST_Amount = Math.Round(model.Total_GST_Amount, 2);
                    rcp.Table_No = model.Table_No;
                    rcp.Is_Uploaded = !AppSetting.POS_OFFLINE_CLIENT;
                    rcp.Is_Latest = !AppSetting.POS_OFFLINE_CLIENT;

                    decimal currGST = 0;
                    if (model.useGST) currGST = model.valGST.HasValue ? model.valGST.Value : 0;

                    if (model.operation == UserSession.RIGHT_C) {
                        rcp.Create_By = userlogin.User_Authentication.Email_Address;
                        rcp.Create_On = currentdate;
                        rcp.Update_By = userlogin.User_Authentication.Email_Address;
                        rcp.Update_On = currentdate;

                        foreach (var row in rcp.POS_Receipt_Payment) {
                            row.Create_By = userlogin.User_Authentication.Email_Address;
                            row.Create_On = currentdate;
                            row.Update_By = userlogin.User_Authentication.Email_Address;
                            row.Update_On = currentdate;
                        }
                        for (var i = 0; i < model.Product_Rows.Length; i++) {
                            var row = model.Product_Rows[i];
                            if (row.Row_Type == RowType.ADD) {
                                var rp = new POS_Products_Rcp() {
                                    Price = row.Price,
                                    Qty = row.Qty,
                                    Product_ID = row.Product_ID,
                                    Product_Name = row.Name,
                                    GST = (row.Price * row.Qty) * (currGST / (100 + currGST)),
                                    Discount = row.Discount,
                                    Discount_Type = row.Discount_Type,
                                    Is_Uploaded = !AppSetting.POS_OFFLINE_CLIENT,
                                    Is_Latest = !AppSetting.POS_OFFLINE_CLIENT,
                                    Create_By = userlogin.User_Authentication.Email_Address,
                                    Create_On = currentdate,
                                    Update_By = userlogin.User_Authentication.Email_Address,
                                    Update_On = currentdate
                                };

                                if (row.Product_ID < 0)
                                    rp.Product_ID = null;
                                rcp.POS_Products_Rcp.Add(rp);
                            }

                        }
                        rcp.Status = ReceiptStatus.Paid;
                        if (model.Action == POSAction.HoldBill | model.Action == POSAction.HoldBill) {
                            rcp.Status = ReceiptStatus.Hold;
                            rcp.Do_Not_Upload = true;
                            model.result = pService.InsertPOSHoldBillReceipt(rcp);
                        } else {
                            if (model.Back_Order) {
                                rcp.Status = ReceiptStatus.BackOrder;
                            }

                            rcp.Remark = model.Remark;
                            rcp.Customer_Name = model.Customer_Name;
                            rcp.Contact_No = model.Contact_No;
                            rcp.Customer_Email = model.Customer_Email;
                            rcp.Do_Not_Upload = false;
                            model.result = pService.InsertPOSReceipt(rcp);

                        }

                        if (model.result.Code == ERROR_CODE.SUCCESS) {
                            if (AppSetting.POS_OFFLINE_CLIENT) {
                                var mgOffline = new ManageOffline(userlogin.Company_ID);
                                mgOffline.SendDataToServer();

                                rcp = pService.GetPOSReceipt(null, rcp.Receipt_Local_ID);
                            }

                            if (model.Action == POSAction.SaveAndBacktoNewSale | model.Action == POSAction.SaveAndPrintReceipt | model.Action == POSAction.HoldBillAndPrintReceipt) {
                                var rcp_config = new ReceiptConfigService().GetReceiptConfigByCompany(rcp.Company_ID);
                                if (rcp_config != null) {
                                    if (!rcp_config.Ignore_Print.HasValue || !rcp_config.Ignore_Print.Value) {
                                        var company = new CompanyService().GetCompany(rcp.Company_ID);

                                        POS_Terminal terminal;
                                        if (AppSetting.POS_OFFLINE_CLIENT)
                                            terminal = cService.GetTerminalByMacAddress(cService.GetMacAddress());
                                        else
                                            terminal = cService.GetTerminalByCashierID(userlogin.Profile_ID);

                                        receipt = "";
                                        if (rcp.Payment_Type == PaymentType.Credit_Card) {
                                            if (terminal.Is_WebPRNT.HasValue && !terminal.Is_WebPRNT.Value) {
                                                receipt = ReportUtil.receiptData(rcp, company, rcp_config, terminal, terminal.User_Profile, tax);
                                            } else {
                                                receipt = ReportUtil.webPrntReceiptData(rcp, company, rcp_config, terminal, terminal.User_Profile, tax);
                                                ipAddress = terminal.Printer_IP_Address;
                                            }
                                        } else {

                                            if (model.Action == POSAction.HoldBillAndPrintReceipt) {
                                                rcp.Status = ReceiptStatus.Hold;
                                            }

                                            if (terminal.Is_WebPRNT.HasValue && !terminal.Is_WebPRNT.Value) {
                                                receipt = ReportUtil.receiptData(rcp, company, rcp_config, terminal, terminal.User_Profile, tax);
                                            } else {
                                                receipt = ReportUtil.webPrntReceiptData(rcp, company, rcp_config, terminal, terminal.User_Profile, tax);
                                                ipAddress = terminal.Printer_IP_Address;
                                            }
                                        }

                                        if (terminal.Is_WebPRNT.HasValue && !terminal.Is_WebPRNT.Value) {
                                            var msg = ReportUtil.printToPrinter(receipt, terminal.Printer_IP_Address);

                                            if (rcp.Status != ReceiptStatus.Hold) {
                                                var copy = ReportUtil.printToPrinter(receipt, terminal.Printer_IP_Address);
                                            }
                                        }
                                    }
                                }
                            }


                            if (model.Action == POSAction.SaveAndBacktoNewSale | model.Action == POSAction.HoldBill | model.Action == POSAction.HoldBillAndPrintReceipt) {
                                // goto new sale
                                model.Action = POSAction.NewSale;

                                var pcri = new ProductCriteria() {
                                    Company_ID = userlogin.Company_ID,
                                    Category_ID = model.Product_Category_ID,
                                    User_Authentication_ID = userlogin.User_Authentication_ID,
                                    Record_Status = RecordStatus.Active
                                };
                                model.products = (List<Product>)iService.LstProduct(pcri).Object;

                                //Modified by Nay on 14-Oct-2015 
                                //Do not allow to show the Categoreis which are don't have procuts

                                var ccri = new CategoryCriteria() {
                                    Company_ID = userlogin.Company_ID,
                                    Branch_ID = posTerminal.Branch_ID,
                                    hasBlank = true,
                                    Record_Status = RecordStatus.Active
                                };

                                model.productCategory = iService.LstCategory(ccri);

                                if (model.Business_Type == BusinessType.FoodAndBeverage) {
                                    model.productTable = pService.GetProductTable(userlogin.Company_ID);
                                }

                                model.Product_Rows = new List<POSProductViewModel>().ToArray();
                                model.Product_Category_ID = null;
                                model.Table_No = "";
                                model.operation = UserSession.RIGHT_C;
                                model.Pos_Select_Mode = PosPaymentMode.Cash;
                                model.Payment_Type = PaymentType.Cash;
                                model.Discount = 0;
                                model.Discount_Type = DiscountType.Amount;
                                model.Card_Branch = null;
                                model.Card_Type = null;
                                model.Receipt_No = null;
                                model.Receipt_ID = null;
                                model.Receipt_Date = null;
                                model.Cash_Payment = 0;
                                model.Credit_Card_Payment = 0;
                                model.Nets_Payment = 0;
                                model.Net_Amount = 0;
                                model.Changes = 0;
                                model.Status = null;
                                model.Combine_Payment = false;
                                ModelState.Clear();

                            } else if (model.Action == POSAction.SaveAndPrintReceipt) {
                                model.Receipt_ID = rcp.Receipt_ID;
                                model.Receipt_Date = DateUtil.ToDisplayDate2(rcp.Receipt_Date);
                                model.Receipt_No = rcp.Receipt_No;
                                model.Action = POSAction.ProcessPayment;
                                model.banks = cbService.LstLookup(ComboType.Bank_Name, userlogin.Company_ID.Value, false);
                                model.cardTypes = cbService.LstLookup(ComboType.Credit_Card_Type, userlogin.Company_ID.Value, false);
                                model.Status = rcp.Status;
                                model.operation = UserSession.RIGHT_U;

                                ModelState.Clear();
                            }
                        } else {
                            model.Action = POSAction.ProcessPayment;
                            model.banks = cbService.LstLookup(ComboType.Bank_Name, userlogin.Company_ID.Value, false);
                            model.cardTypes = cbService.LstLookup(ComboType.Credit_Card_Type, userlogin.Company_ID.Value, false);

                        }
                    } else if (model.operation == UserSession.RIGHT_U) {
                        //update
                        if (rcp != null) {
                            rcp.Update_By = userlogin.User_Authentication.Email_Address;
                            rcp.Update_On = currentdate;

                            //Added by Nay on 02-Sept-2015
                            //for unlimited "Hold Bill"
                            if (model.Action == POSAction.HoldBill | model.Action == POSAction.HoldBillAndPrintReceipt) {
                                rcp.Status = ReceiptStatus.Hold;
                                rcp.Receipt_Date = currentdate;
                                rcp.POS_Products_Rcp.Clear();
                                rcp.Do_Not_Upload = true; // no need to upload holdbill
                                foreach (var row in model.Product_Rows) {
                                    if (row.Row_Type != RowType.DELETE) {
                                        rcp.POS_Products_Rcp.Add(new POS_Products_Rcp() {
                                            Discount = row.Discount,
                                            Discount_Type = row.Discount_Type,
                                            GST = row.GST,
                                            ID = row.ID.HasValue ? row.ID.Value : 0,
                                            Price = row.Price,
                                            Product_ID = row.Product_ID,
                                            Product_Name = row.Name,
                                            Qty = row.Qty,
                                            Receipt_ID = row.Receipt_ID,
                                            Total_GST_Amount = row.GST,
                                            Receipt_Local_ID = row.Receipt_Local_ID,
                                            Receipt_Product_Local_ID = row.Receipt_Product_Local_ID,
                                            Do_Not_Upload = true // no need to upload holdbill
                                        });
                                    }
                                }

                                model.result = pService.UpdatePOSReceipt(rcp);
                                if (model.result.Code == ERROR_CODE.SUCCESS) {
                                    if (AppSetting.POS_OFFLINE_CLIENT) {
                                        var mgOffline = new ManageOffline(userlogin.Company_ID);
                                        mgOffline.SendDataToServer();

                                    }
                                    model.Receipt_ID = rcp.Receipt_ID;
                                    model.Receipt_Date = DateUtil.ToDisplayDate2(rcp.Receipt_Date);
                                    model.Receipt_No = rcp.Receipt_No;
                                    model.Action = POSAction.ProcessPayment;
                                    model.banks = cbService.LstLookup(ComboType.Bank_Name, userlogin.Company_ID.Value, false);
                                    model.cardTypes = cbService.LstLookup(ComboType.Credit_Card_Type, userlogin.Company_ID.Value, false);
                                    model.Status = rcp.Status;
                                    ModelState.Clear();
                                }
                            } else {
                                rcp.Receipt_Date = currentdate;
                                rcp.Status = ReceiptStatus.Paid;
                                rcp.Receipt_ID = model.Receipt_ID.Value;
                                rcp.Do_Not_Upload = false;
                                rcp.POS_Products_Rcp.Clear();
                                foreach (var row in model.Product_Rows) {
                                    if (row.Row_Type != RowType.DELETE) {
                                        rcp.POS_Products_Rcp.Add(new POS_Products_Rcp() {
                                            Discount = row.Discount,
                                            Discount_Type = row.Discount_Type,
                                            GST = row.GST,
                                            ID = row.ID.HasValue ? row.ID.Value : 0,
                                            Price = row.Price,
                                            Product_ID = row.Product_ID,
                                            Product_Name = row.Name,
                                            Qty = row.Qty,
                                            Receipt_ID = row.Receipt_ID,
                                            Total_GST_Amount = row.GST,
                                            Receipt_Local_ID = row.Receipt_Local_ID,
                                            Receipt_Product_Local_ID = row.Receipt_Product_Local_ID,
                                            Do_Not_Upload = false
                                        });
                                    }
                                }

                                model.result = pService.UpdatePOSReceipt(rcp);
                                if (model.result.Code == ERROR_CODE.SUCCESS) {
                                    if (AppSetting.POS_OFFLINE_CLIENT) {
                                        var mgOffline = new ManageOffline(userlogin.Company_ID);
                                        mgOffline.SendDataToServer();
                                        rcp = pService.GetPOSReceipt(null, rcp.Receipt_Local_ID);
                                    }


                                    if (model.Action == POSAction.SaveAndBacktoNewSale | model.Action == POSAction.SaveAndPrintReceipt) {
                                        var rcp_config = new ReceiptConfigService().GetReceiptConfigByCompany(rcp.Company_ID);
                                        if (rcp_config != null) {
                                            if (!rcp_config.Ignore_Print.HasValue || !rcp_config.Ignore_Print.Value) {
                                                var company = new CompanyService().GetCompany(rcp.Company_ID);

                                                POS_Terminal terminal;
                                                if (AppSetting.POS_OFFLINE_CLIENT)
                                                    terminal = cService.GetTerminalByMacAddress(cService.GetMacAddress());
                                                else
                                                    terminal = cService.GetTerminalByCashierID(userlogin.Profile_ID);

                                                receipt = "";
                                                if (rcp.Payment_Type == PaymentType.Credit_Card) {
                                                    if (terminal.Is_WebPRNT.HasValue && !terminal.Is_WebPRNT.Value) {
                                                        receipt = ReportUtil.receiptData(rcp, company, rcp_config, terminal, terminal.User_Profile, tax);
                                                    } else {
                                                        receipt = ReportUtil.webPrntReceiptData(rcp, company, rcp_config, terminal, terminal.User_Profile, tax);
                                                        ipAddress = terminal.Printer_IP_Address;
                                                    }
                                                } else {
                                                    if (terminal.Is_WebPRNT.HasValue && !terminal.Is_WebPRNT.Value) {
                                                        receipt = ReportUtil.receiptData(rcp, company, rcp_config, terminal, terminal.User_Profile, tax);
                                                    } else {
                                                        receipt = ReportUtil.webPrntReceiptData(rcp, company, rcp_config, terminal, terminal.User_Profile, tax);
                                                        ipAddress = terminal.Printer_IP_Address;
                                                    }
                                                }
                                                if (terminal.Is_WebPRNT.HasValue && !terminal.Is_WebPRNT.Value) {
                                                    var msg = ReportUtil.printToPrinter(receipt, terminal.Printer_IP_Address);
                                                    if (rcp.Status != ReceiptStatus.Hold) {
                                                        var copy = ReportUtil.printToPrinter(receipt, terminal.Printer_IP_Address);
                                                    }
                                                }
                                            }
                                        }

                                    }

                                    if (model.Action == POSAction.SaveAndBacktoNewSale) {
                                        if (model.Page_Action == 2) {
                                            return RedirectToAction("Report", "POS");
                                        } else if (model.Page_Action == 3) {
                                            return RedirectToAction("ViewHoldBill", "POS");
                                        }
                                    } else {
                                        model.Receipt_ID = rcp.Receipt_ID;
                                        model.Receipt_Date = DateUtil.ToDisplayDate2(rcp.Receipt_Date);
                                        model.Receipt_No = rcp.Receipt_No;
                                        model.Action = POSAction.ProcessPayment;
                                        model.banks = cbService.LstLookup(ComboType.Bank_Name, userlogin.Company_ID.Value, false);
                                        model.cardTypes = cbService.LstLookup(ComboType.Credit_Card_Type, userlogin.Company_ID.Value, false);
                                        model.Status = rcp.Status;
                                        ModelState.Clear();
                                    }
                                }
                            }
                        }
                    }
                }

                if (posTerminal.Is_WebPRNT.HasValue && posTerminal.Is_WebPRNT.Value) {
                    model.ReceiptData = receipt;
                    model.Terminal_IP_Address = ipAddress;
                }
            }


            return View(model);
        }

        private POS_Receipt_Payment CreateCashPayment(decimal pAmount) {
            return new POS_Receipt_Payment() {
                Payment_Amount = pAmount,
                Payment_Type = PaymentType.Cash,
                Is_Uploaded = !AppSetting.POS_OFFLINE_CLIENT,
                Is_Latest = !AppSetting.POS_OFFLINE_CLIENT
            };
        }
        private POS_Receipt_Payment CreateNetsPayment(decimal pAmount) {
            return new POS_Receipt_Payment() {
                Payment_Amount = pAmount,
                Payment_Type = PaymentType.Nets,
                Is_Uploaded = !AppSetting.POS_OFFLINE_CLIENT,
                Is_Latest = !AppSetting.POS_OFFLINE_CLIENT
            };
        }
        private POS_Receipt_Payment CreateRedeemPayment(Nullable<decimal> pAmount) {
            return new POS_Receipt_Payment() {
                Payment_Amount = pAmount,
                Payment_Type = PaymentType.Redeem,
                Is_Uploaded = !AppSetting.POS_OFFLINE_CLIENT,
                Is_Latest = !AppSetting.POS_OFFLINE_CLIENT
            };
        }

        private POS_Receipt_Payment CreateCreditCardPayment(Nullable<decimal> pAmount, Nullable<int> pCompayID, string pCardName) {
            var cbService = new ComboService();
            Global_Lookup_Data Card_Type = null;
            Nullable<int> cardID = null;
            if (!string.IsNullOrEmpty(pCardName)) {
                Card_Type = cbService.GetLookup(pCardName, pCompayID);
                if (Card_Type != null)
                    cardID = Card_Type.Lookup_Data_ID;
            }


            return new POS_Receipt_Payment() {
                Payment_Amount = pAmount,
                Payment_Type = PaymentType.Credit_Card,
                Is_Uploaded = !AppSetting.POS_OFFLINE_CLIENT,
                Is_Latest = !AppSetting.POS_OFFLINE_CLIENT,
                Card_Type = cardID
            };
        }

        [HttpGet]
        [AllowAuthorized]
        public ActionResult Member(POSMemberViewModel model) {
            User_Profile userlogin = UserSession.getUser(HttpContext);
            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            //var model = new POSMemberViewModel();

            RightResult rightResult = base.validatePageRight(UserSession.RIGHT_A, "/POS/POS");
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;


            var mService = new MemberService();
            var cri = new MemberCriteria() {
                Company_ID = userlogin.Company_ID,
                Text_Search = model.Search_Free_Text,
                Member_Name = model.Search_Name,
                Member_Card_No = model.Search_Card_No,
                Member_Email = model.Search_Email,
                Member_NRIC = model.Search_NRIC
            };

            model.memberList = mService.LstMember(cri);
            model.Company_ID = userlogin.Company_ID;

            var conf = mService.GetMemberConfig(userlogin.Company_ID);
            if (conf != null) {
                model.Member_Discount = conf.Member_Discount;
                model.Member_Discount_Type = conf.Member_Discount_Type;
                model.Birthday_Discount = conf.Birthday_Discount;
                model.Birthday_Discount_Type = conf.Birthday_Discount_Type;
            }

            var comp = new CompanyService().GetCompany(userlogin.Company_ID);
            if (comp != null) {
                if (comp.Currency != null)
                    model.Currency_Code = comp.Currency.Currency_Code;
            }


            return View(model);
        }

        [HttpGet]
        [AllowAuthorized]
        public ActionResult MemberBirthdays(string pBirthdate) {
            User_Profile userlogin = UserSession.getUser(HttpContext);
            POSMemberBirthdayViewModel model;


            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            model = new POSMemberBirthdayViewModel();

            RightResult rightResult = base.validatePageRight(UserSession.RIGHT_A, "/POS/POS");
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;

            var mService = new MemberService();

            var cri = new MemberCriteria() {
                Company_ID = userlogin.Company_ID,
                Text_Search = model.Text_Search,
                Birthdate = (string.IsNullOrEmpty(pBirthdate) ? DateTime.Today : DateUtil.ToDate(pBirthdate))
            };

            model.Members = mService.LstMemberBirthdays(cri);
            model.Search_Birthday_Month = DateUtil.ToDisplayDate(cri.Birthdate);

            return View(model);
        }

        public ActionResult ExportBirthdays(POSMemberBirthdayViewModel model) {
            User_Profile userlogin = UserSession.getUser(HttpContext);
            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            RightResult rightResult = base.validatePageRight(UserSession.RIGHT_A, "/POS/POS");
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;

            var mService = new MemberService();

            var cri = new MemberCriteria() {
                Company_ID = userlogin.Company_ID,
                Text_Search = model.Text_Search,
                Birthdate = DateUtil.ToDate(model.Search_Birthday_Month)
            };

            model.Members = mService.LstMemberBirthdays(cri);

            var csv = new StringBuilder();
            //Init Table
            csv.Append("<table border=1>");
            //Init Header
            csv.Append("<tr>");
            csv.Append("<td style='background-color:#ff0; width:50%;'><b>" + Resources.ResourcePOS.MemberName + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.MemberCardNo + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.NRICNo + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.PhoneNo + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.Email + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.DOB + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.Credits + "</b></td>");
            csv.Append("</tr>");

            foreach (var row in model.Members.OrderBy(m => m.Member_Name)) {
                if (!string.IsNullOrEmpty(row.Member_Name)) {
                    csv.Append("<tr>");
                    csv.Append("<td>" + row.Member_Name + "</td>");
                    csv.Append("<td>" + row.Member_Card_No + "</td>");
                    csv.Append("<td>" + row.NRIC_No + "</td>");
                    csv.Append("<td>" + row.Phone_No + "</td>");
                    csv.Append("<td>" + row.Email + "</td>");
                    csv.Append("<td>" + (row.DOB.HasValue ? row.DOB.Value.ToString("dd/MMM/yyyy") : "") + "</td>");
                    csv.Append("<td>" + (row.Credit.HasValue ? row.Credit.Value.ToString("n2") : "0") + "</td>");
                    csv.Append("</tr>");
                }

            }

            csv.Append("</table>");

            System.Web.UI.WebControls.GridView gv = new System.Web.UI.WebControls.GridView();
            //gv.DataSource = data;
            gv.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Birthday List.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            sw.Write(csv.ToString());
            System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View(model);
        }

        public JsonResult CurrentMonthBirthdays() {
            User_Profile userlogin = UserSession.getUser(HttpContext);
            POSMemberBirthdayViewModel model = new POSMemberBirthdayViewModel();
            int birthdayCount = 0;

            var cri = new MemberCriteria() {
                Company_ID = userlogin.Company_ID,
                Birthdate = DateTime.Today
            };

            var mService = new MemberService();

            model.Members = mService.LstMemberBirthdays(cri);

            if (model.Members != null) {
                birthdayCount = model.Members.Count;
            } else {
                birthdayCount = 0;
            }

            return Json(new { Count = birthdayCount }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomerPurchases(string pCategoryId) {
            User_Profile userlogin = UserSession.getUser(HttpContext);
            var customerCount = 0;

            var customerList = getCustomerPurchases(userlogin.Company_ID.Value, Convert.ToInt32(pCategoryId), string.Empty);

            if (customerList != null) {
                customerCount = customerList.Count;
            }

            return Json(new { Count = customerCount }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAuthorized]
        public ActionResult CustomerPurchases(string pCategoryId) {
            User_Profile userlogin = UserSession.getUser(HttpContext);
            POSCustomerPurchasesViewModel model;

            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            model = new POSCustomerPurchasesViewModel();

            RightResult rightResult = base.validatePageRight(UserSession.RIGHT_A, "/POS/POS");
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;

            var pService = new POSService();
            var iService = new InventoryService();

            if (!string.IsNullOrEmpty(pCategoryId)) {
                model.Category_Id = Convert.ToInt32(pCategoryId);
            }

            model.Category_List = iService.LstCategory(userlogin.Company_ID.Value, true);
            model.Customers = getCustomerPurchases(userlogin.Company_ID.Value, Convert.ToInt32(pCategoryId), model.Text_Search);

            return View(model);
        }

        [HttpGet]
        [AllowAuthorized]
        public ActionResult GetMember(Nullable<int> pMemberID) {
            var mService = new MemberService();

            var mem = mService.GetMember(pMemberID);
            if (mem != null) {
                return Json(new {
                    Member_ID = mem.Member_ID,
                    Credit = (mem.Credit.HasValue ? mem.Credit.Value : 0),
                    DOB = DateUtil.ToDisplayDate(mem.DOB),
                    Email = mem.Email,
                    Member_Card_No = mem.Member_Card_No,
                    Member_Discount = mem.Member_Discount,
                    Member_Discount_Type = mem.Member_Discount_Type,
                    Member_Name = mem.Member_Name,
                    Gender = mem.Gender,
                    Address = mem.Address,
                    Member_Status = mem.Member_Status,
                    NRIC_No = mem.NRIC_No,
                    Phone_No = mem.Phone_No,
                }, JsonRequestBehavior.AllowGet);

            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportMember(POSMemberViewModel model) {
            User_Profile userlogin = UserSession.getUser(HttpContext);
            model.operation = UserSession.RIGHT_A;
            //Validate Page Right
            RightResult rightResult = base.validatePageRight(UserSession.RIGHT_A, "/POS/POS");
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;

            var mService = new MemberService();
            var cri = new MemberCriteria() {
                Company_ID = userlogin.Company_ID,
                Text_Search = model.Search_Free_Text,
                Member_Name = model.Search_Name,
                Member_Card_No = model.Search_Card_No,
                Member_Email = model.Search_Email,
                Member_NRIC = model.Search_NRIC
            };

            model.memberList = mService.LstMember(cri);

            var conf = mService.GetMemberConfig(userlogin.Company_ID);
            if (conf != null) {
                model.Member_Discount = conf.Member_Discount;
                model.Member_Discount_Type = conf.Member_Discount_Type;
                model.Birthday_Discount = conf.Birthday_Discount;
                model.Birthday_Discount_Type = conf.Birthday_Discount_Type;
            }

            var comp = new CompanyService().GetCompany(userlogin.Company_ID);
            if (comp != null) {
                if (comp.Currency != null)
                    model.Currency_Code = comp.Currency.Currency_Code;
            }


            var csv = new StringBuilder();
            //Init Table
            csv.Append("<table border=1>");
            //Init Header
            csv.Append("<tr>");
            csv.Append("<td style='background-color:#ff0; width:50%;'><b>" + Resources.ResourcePOS.MemberName + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.MemberCardNo + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.NRICNo + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.PhoneNo + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.Email + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.DOB + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.Credits + "</b></td>");
            csv.Append("</tr>");

            foreach (var row in model.memberList.OrderBy(m => m.Member_Name)) {
                if (!string.IsNullOrEmpty(row.Member_Name)) {
                    csv.Append("<tr>");
                    csv.Append("<td>" + row.Member_Name + "</td>");
                    csv.Append("<td>" + row.Member_Card_No + "</td>");
                    csv.Append("<td>" + row.NRIC_No + "</td>");
                    csv.Append("<td>" + row.Phone_No + "</td>");
                    csv.Append("<td>" + row.Email + "</td>");
                    csv.Append("<td>" + (row.DOB.HasValue ? row.DOB.Value.ToString("dd/MMM/yyyy") : "") + "</td>");
                    csv.Append("<td>" + (row.Credit.HasValue ? row.Credit.Value.ToString("n2") : "0") + "</td>");
                    csv.Append("</tr>");
                }

            }

            csv.Append("</table>");

            System.Web.UI.WebControls.GridView gv = new System.Web.UI.WebControls.GridView();
            //gv.DataSource = data;
            gv.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Member List.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            sw.Write(csv.ToString());
            System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View(model);
        }

        [AllowAuthorized]
        public ActionResult SaveMember(POSMemberViewModel model) {
            var userlogin = UserSession.getUser(HttpContext);
            var currentdate = StoredProcedure.GetCurrentDate();
            var mService = new MemberService();

            if (!model.Member_ID.HasValue || model.Member_ID.Value == 0) {
                var mem = new Member();
                mem.Company_ID = model.Company_ID;
                mem.Credit = model.Credit;
                mem.DOB = DateUtil.ToDate(model.DOB);
                mem.Email = model.Email;
                mem.Gender = model.Gender;
                mem.Address = model.Address;
                mem.Member_Card_No = model.Member_Card_No;
                mem.Member_Discount = model.Member_Discount;
                mem.Member_Discount_Type = model.Member_Discount_Type; ;
                mem.Member_ID = model.Member_ID.HasValue ? model.Member_ID.Value : 0;
                mem.Member_Name = model.Member_Name;
                mem.NRIC_No = model.NRIC_No;
                mem.Phone_No = model.Phone_No;

                mem.Update_By = userlogin.User_Authentication.Email_Address;
                mem.Update_On = currentdate;
                mem.Create_By = userlogin.User_Authentication.Email_Address;
                mem.Create_On = currentdate;
                mem.Member_Status = "New";
                var result = mService.InsertMember(mem);
                return Json(result, JsonRequestBehavior.AllowGet);
            } else if (model.Member_ID.HasValue && model.Member_ID.Value > 0) {
                var mem = mService.GetMember(model.Member_ID);
                mem.DOB = DateUtil.ToDate(model.DOB);
                mem.Email = model.Email;
                mem.Member_Name = model.Member_Name;
                mem.NRIC_No = model.NRIC_No;
                mem.Phone_No = model.Phone_No;
                mem.Gender = model.Gender;
                mem.Address = model.Address;

                mem.Update_By = userlogin.User_Authentication.Email_Address;
                mem.Update_On = currentdate;
                mem.Member_Status = "Registered";
                var result = mService.UpdateMember(mem);
                return Json(result, JsonRequestBehavior.AllowGet);
            }


            return Json(new ServiceResult() { Code = ERROR_CODE.ERROR_514_INVALID_INPUT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_514_INVALID_INPUT_ERROR), Field = "Member" }, JsonRequestBehavior.AllowGet);
        }

        [AllowAuthorized]
        public ActionResult MemberPurchases(POSMemberViewModel model) {
            DateTime? searchDateFrom = null;
            DateTime? searchDateTo = null;

            if (!string.IsNullOrEmpty(model.Search_Date_From)) {
                searchDateFrom = DateUtil.ToDate(model.Search_Date_From, "/");
            }

            if (!string.IsNullOrEmpty(model.Search_Date_From)) {
                searchDateTo = DateUtil.ToDate(model.Search_Date_To, "/");
            }

            model = this.getMember(model.Member_ID.Value, searchDateFrom, searchDateTo, model.Search_Status, model.Search_Terminal, model.Search_Receipt_No);

            if (model != null) {
                return View(model);
            }
            return View();
        }

        public ActionResult ExportMemberPurchases(POSMemberViewModel model) {
            User_Profile userlogin = UserSession.getUser(HttpContext);
            model.operation = UserSession.RIGHT_A;
            //Validate Page Right
            RightResult rightResult = base.validatePageRight(UserSession.RIGHT_A, "/POS/Report");
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;

            bool orderByAsc = true;

            DateTime? searchDateFrom = null;
            DateTime? searchDateTo = null;

            if (!string.IsNullOrEmpty(model.Search_Date_From)) {
                searchDateFrom = DateUtil.ToDate(model.Search_Date_From, "/");
            }

            if (!string.IsNullOrEmpty(model.Search_Date_From)) {
                searchDateTo = DateUtil.ToDate(model.Search_Date_To, "/");
            }

            model = this.getMember(model.Member_ID.Value, searchDateFrom, searchDateTo, model.Search_Status, model.Search_Terminal, model.Search_Receipt_No);


            //Added by Nay on 16-Jun-2015
            var cbService = new ComboService();
            //Purpose : To take GST percentage for each record of respective company.

            //Edit By Jane
            decimal gstPercentage = 0;
            bool useGST = false;

            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            var rcpConfig = new ReceiptConfigService().GetReceiptConfigByCompany(userlogin.Company_ID.Value);
            if (rcpConfig == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            var tax = new TaxServie().GetTax(userlogin.Company_ID);
            var surchargePercen = 0M;
            var surchargeInclude = false;
            var serviceChargePercen = 0M;
            var serviceChargeInclude = false;
            if (tax != null) {
                surchargeInclude = tax.Include_Surcharge.HasValue ? tax.Include_Surcharge.Value : false;
                if (surchargeInclude)
                    surchargePercen = tax.Surcharge_Percen.HasValue ? tax.Surcharge_Percen.Value : 0;

                serviceChargeInclude = tax.Include_Service_Charge.HasValue ? tax.Include_Service_Charge.Value : false;
                if (serviceChargeInclude)
                    serviceChargePercen = tax.Service_Charge_Percen.HasValue ? tax.Service_Charge_Percen.Value : 0;

                useGST = tax.Include_GST.HasValue ? tax.Include_GST.Value : false;
                var gst = tax.Tax_GST.Where(w => w.Tax_Type == TaxType.Exclusive && w.Record_Status == RecordStatus.Active && w.Is_Default == true).FirstOrDefault();
                if (gst != null)
                    gstPercentage = gst.Tax.HasValue ? gst.Tax.Value : 0;

            }
            var csv = new StringBuilder();

            csv.Append("<table border=1>");
            csv.Append("<tr>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.Branch + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.Date + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.ReceiptNo + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.CategoryName + "</b></td>");
            //TEST
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.Product + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.Qty + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.UnitPrice + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.Amount + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.Discount + "(%)</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.Surcharge + "(%)</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.ReceivedAmount + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.Status + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.PaidBy + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.Terminal + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.Cashier + "</b></td>");

            csv.Append("</tr>");

            var currReceiptdate = "";
            var subTotal = 0M;
            var grandTotal = 0M;

            //Added by Nay on 17-Jun-2015 
            var gstTotal = 0M;

            var netTotal = 0M;
            var grandNetTotal = 0M;
            foreach (var row in model.purchaseList) {
                if (currReceiptdate != DateUtil.ToDisplayDate(row.Receipt_Date)) {
                    if (!string.IsNullOrEmpty(currReceiptdate)) {
                        csv.Append("<tr >");
                        csv.Append("<td style='background-color:#ff0'><b>Sub Total</b></td>");
                        csv.Append("<td style='background-color:#ff0'><b></b></td>");
                        csv.Append("<td style='background-color:#ff0'><b></b></td>");
                        //TEST
                        csv.Append("<td style='background-color:#ff0'><b></b></td>");
                        csv.Append("<td style='background-color:#ff0'><b></b></td>");
                        csv.Append("<td style='background-color:#ff0'><b></b></td>");
                        csv.Append("<td style='background-color:#ff0'><b>" + subTotal.ToString("n2") + "</b></td>");
                        csv.Append("<td style='background-color:#ff0'><b></b></td>");
                        csv.Append("<td style='background-color:#ff0'><b></b></td>");
                        csv.Append("<td style='background-color:#ff0'><b>" + netTotal.ToString("n2") + "</b></td>");
                        csv.Append("<td style='background-color:#ff0'><b></b></td>");
                        csv.Append("<td style='background-color:#ff0'><b></b></td>");
                        csv.Append("<td style='background-color:#ff0'><b></b></td>");
                        csv.Append("<td style='background-color:#ff0'><b></b></td>");
                        csv.Append("</tr>");
                    }
                    subTotal = 0;
                    netTotal = 0;

                    currReceiptdate = DateUtil.ToDisplayDate(row.Receipt_Date);
                }

                Nullable<decimal> discountPercen = 0;
                if (row.Discount_Type == DiscountType.Amount) {
                    if (row.Total_Amount > 0) {
                        discountPercen = ((100 * row.Discount) / row.Total_Amount);
                    } else {
                        discountPercen = 0;
                    }

                } else {
                    discountPercen = row.Discount;
                }

                var paymentType = "";
                //Added by Nay on 02-Sept-2015
                //To show sepprately "Normal" & "Combine" payments
                if (row.POS_Receipt_Payment.Count > 1) {
                    var combinePaid = new List<string>();
                    foreach (var paidBy in row.POS_Receipt_Payment) {
                        if (paidBy.Payment_Type == PaymentType.Cash)
                            combinePaid.Add("Cash");
                        else if (paidBy.Payment_Type == PaymentType.Nets)
                            combinePaid.Add("Nets");
                        else if (paidBy.Payment_Type == PaymentType.Credit_Card)
                            combinePaid.Add("Credit/Debit");
                    }
                    paymentType = combinePaid[0] + ", " + combinePaid[1];
                } else {
                    if (row.Payment_Type == PaymentType.Cash) {
                        paymentType = "Cash";
                    } else if (row.Payment_Type == PaymentType.Credit_Card) {
                        paymentType = "Credit/Debit";
                    }
                        //Added by Nay on 16-Jun-2015 
                        //Purpose : To show as if transaction was paid by Nets!
                      else if (row.Payment_Type == PaymentType.Nets) {
                        paymentType = "Nets";
                    }
                }


                var receiptProducts = orderByAsc ? row.POS_Products_Rcp.OrderBy(o => (o.Product != null && o.Product.Product_Category != null) ? o.Product.Product_Category.Category_Name : o.Product_Name).ThenBy(o => o.Product_Name) :
                    row.POS_Products_Rcp.OrderByDescending(o => (o.Product != null && o.Product.Product_Category != null) ? o.Product.Product_Category.Category_Name : o.Product_Name).ThenByDescending(o => o.Product_Name);

                foreach (var prow in receiptProducts) {

                    var amount = (prow.Qty.HasValue ? prow.Qty.Value : 0) * (prow.Price.HasValue ? prow.Price.Value : 0);
                    var receiveAmount = amount;
                    if (discountPercen > 0)
                        receiveAmount = amount - (amount * ((discountPercen.HasValue ? discountPercen.Value : 0) / 100));

                    //var cat = prow.Product.Product_Category;
                    var catname = "";
                    if (prow.Product != null && prow.Product.Product_Category != null) {
                        catname = prow.Product.Product_Category.Category_Name;
                    }
                    if (row.Payment_Type == PaymentType.Credit_Card) {
                        if (surchargeInclude) {
                            if (surchargePercen > 0) {
                                receiveAmount += (receiveAmount * (surchargePercen / 100));
                            }

                        }

                    }

                    csv.Append("<tr>");
                    csv.Append("<td>" + row.POS_Shift.POS_Terminal.Branch.Branch_Name + "</td>");
                    csv.Append("<td>" + DateUtil.ToDisplayDateTime(row.Receipt_Date) + "</td>");
                    csv.Append("<td>" + row.Receipt_No + "</td>");
                    csv.Append("<td>" + catname + "</td>");
                    //TEST
                    csv.Append("<td>" + prow.Product_Name + "</td>");
                    csv.Append("<td>" + (prow.Qty.HasValue ? prow.Qty.Value : 0).ToString("n0") + "</td>");
                    csv.Append("<td>" + (prow.Price.HasValue ? prow.Price.Value : 0).ToString("n2") + "</td>");
                    csv.Append("<td>" + amount.ToString("n2") + "</td>");
                    csv.Append("<td>" + (discountPercen.HasValue ? discountPercen.Value : 0).ToString("n2") + "</td>");
                    csv.Append("<td>" + surchargePercen.ToString("n2") + "</td>");
                    csv.Append("<td>" + receiveAmount.ToString("n2") + "</td>");
                    csv.Append("<td>" + row.Status + "</td>");
                    csv.Append("<td>" + paymentType + "</td>");
                    csv.Append("<td>" + (row.POS_Shift.POS_Terminal != null ? row.POS_Shift.POS_Terminal.Terminal_Name : "") + "</td>");
                    csv.Append("<td>" + (row.User_Profile != null ? row.User_Profile.Name : "") + "</td>");
                    csv.Append("</tr>");
                }

                subTotal += (row.Total_Amount.HasValue ? row.Total_Amount.Value : 0);
                grandTotal += (row.Total_Amount.HasValue ? row.Total_Amount.Value : 0);

                //Added by Nay on 17-Jun-2015 
                gstTotal += (decimal)(row.Total_GST_Amount.HasValue ? row.Total_GST_Amount : 0);

                netTotal += (row.Net_Amount.HasValue ? row.Net_Amount.Value : 0);
                grandNetTotal += (row.Net_Amount.HasValue ? row.Net_Amount.Value : 0);


                csv.Append("<tr >");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                //TEST
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b>Total Amount</b></td>");
                csv.Append("<td ><b>" + ((row.Total_Amount.HasValue ? row.Total_Amount.Value : 0).ToString("n2")) + "</b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("</tr>");

                //Added by Nay on 16-Jun-2015
                //Purpose : To show total amount of GST for current record as new line in excel!
                if (useGST) {
                    csv.Append("<tr >");
                    csv.Append("<td ><b></b></td>");
                    csv.Append("<td ><b></b></td>");
                    csv.Append("<td ><b></b></td>");
                    //TEST
                    csv.Append("<td ><b></b></td>");
                    csv.Append("<td ><b></b></td>");
                    csv.Append("<td ><b>GST (" + gstPercentage + "% Inclusive)</b></td>");
                    csv.Append("<td ><b>" + ((row.Total_GST_Amount.HasValue ? row.Total_GST_Amount.Value : 0).ToString("n2")) + "</b></td>");
                    csv.Append("<td ><b></b></td>");
                    csv.Append("<td ><b></b></td>");
                    csv.Append("<td ><b></b></td>");
                    csv.Append("<td ><b></b></td>");
                    csv.Append("<td ><b></b></td>");
                    csv.Append("<td ><b></b></td>");
                    csv.Append("<td ><b></b></td>");
                    csv.Append("</tr>");
                }


                decimal? totalDiscountPercen = null;

                if (row.Total_Amount > 0) {
                    totalDiscountPercen = ((100 * row.Total_Discount) / row.Total_Amount);
                } else {
                    totalDiscountPercen = 0;
                }

                csv.Append("<tr >");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                //TEST
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b>Item Discount (%)</b></td>");
                csv.Append("<td ><b>" + ((totalDiscountPercen.HasValue ? totalDiscountPercen.Value : 0).ToString("n2")) + "</b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("</tr>");

                decimal? memberDiscount = null;
                decimal? memberDiscountPercentage = null;

                if (row.Total_Amount > 0) {
                    if (row.Member_Discount_Type == "%") {
                        memberDiscount = ((row.Member_Discount.HasValue ? row.Member_Discount.Value : 0) / 100) * row.Total_Amount;
                    } else {
                        memberDiscount = row.Member_Discount;
                    }
                    memberDiscountPercentage = ((100 * memberDiscount) / row.Total_Amount);
                } else {
                    memberDiscountPercentage = 0;
                }

                csv.Append("<tr >");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                //TEST
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b>Member Discount (" + ((memberDiscountPercentage.HasValue ? memberDiscountPercentage.Value : 0).ToString("G0")) + " %)</b></td>");
                csv.Append("<td ><b>" + ((memberDiscount.HasValue ? memberDiscount.Value : 0).ToString("n2")) + "</b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("</tr>");

                if (row.POS_Receipt_Payment != null) {
                    foreach (var prow in row.POS_Receipt_Payment) {
                        if (prow.Payment_Type == PaymentType.Cash) {
                            var cashReceived = (prow.Payment_Amount.HasValue ? prow.Payment_Amount.Value : 0) - (row.Changes.HasValue ? row.Changes.Value : 0);
                            csv.Append("<tr >");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b>Cash</b></td>");
                            csv.Append("<td ><b>" + cashReceived.ToString("n2") + "</b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("</tr>");

                        } else if (prow.Payment_Type == PaymentType.Nets) {
                            csv.Append("<tr >");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            //TEST
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            //Commented by Nay on 12-Aug-2015
                            //Wrong tab in exported excel file for Net amount
                            //csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b>Nets</b></td>");
                            csv.Append("<td ><b>" + ((prow.Payment_Amount.HasValue ? prow.Payment_Amount.Value : 0).ToString("n2")) + "</b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("</tr>");
                        } else if (prow.Payment_Type == PaymentType.Credit_Card) {
                            csv.Append("<tr >");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            //TEST
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            //Modified by Nay on 03-Sept-2015
                            //To show using card name
                            //csv.Append("<td ><b>Card</b></td>");
                            //csv.Append("<td ><b>" + prow.Global_Lookup_Data1.Name + "</b></td>");
                            if (prow.Card_Type.HasValue) {
                                var cardType = cbService.GetLookup(prow.Card_Type);
                                if (cardType != null)
                                    csv.Append("<td ><b>" + cardType.Name + "</b></td>");
                            } else {
                                csv.Append("<td ><b>Credit/Debit</b></td>");
                            }

                            csv.Append("<td ><b>" + ((prow.Payment_Amount.HasValue ? prow.Payment_Amount.Value : 0).ToString("n2")) + "</b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("</tr>");

                            if (surchargeInclude) {
                                csv.Append("<tr >");
                                csv.Append("<td ><b></b></td>");
                                csv.Append("<td ><b></b></td>");
                                csv.Append("<td ><b></b></td>");
                                //TEST
                                csv.Append("<td ><b></b></td>");
                                csv.Append("<td ><b></b></td>");
                                csv.Append("<td ><b>Surcharge (%)</b></td>");
                                csv.Append("<td ><b>" + ((surchargePercen).ToString("n2")) + "</b></td>");
                                csv.Append("<td ><b></b></td>");
                                csv.Append("<td ><b></b></td>");
                                csv.Append("<td ><b></b></td>");
                                csv.Append("<td ><b></b></td>");
                                csv.Append("<td ><b></b></td>");
                                csv.Append("<td ><b></b></td>");
                                csv.Append("<td ><b></b></td>");
                                csv.Append("</tr>");
                            }
                        }
                    }
                }

                csv.Append("<tr >");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                //TEST
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b>Received Amount</b></td>");
                csv.Append("<td ><b>" + ((row.Net_Amount.HasValue ? row.Net_Amount.Value : 0).ToString("n2")) + "</b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("</tr>");
            }

            csv.Append("<tr >");
            csv.Append("<td style='background-color:#ff0'><b>Sub Total</b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            //TEST
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + subTotal.ToString("n2") + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + netTotal.ToString("n2") + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("</tr>");

            csv.Append("<tr >");
            csv.Append("<td style='background-color:#ff0'><b>Grand Total</b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            //TEST
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + grandTotal.ToString("n2") + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + grandNetTotal.ToString("n2") + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("</tr>");

            //Added by Nay on 17-Jun-2015 
            //To show grand total of GST also show.
            if (useGST) {
                csv.Append("<tr >");
                csv.Append("<td style='background-color:#ff0'><b>Total GST</b></td>");
                csv.Append("<td style='background-color:#ff0'><b></b></td>");
                csv.Append("<td style='background-color:#ff0'><b></b></td>");
                //TEST
                csv.Append("<td style='background-color:#ff0'><b></b></td>");
                csv.Append("<td style='background-color:#ff0'><b></b></td>");
                csv.Append("<td style='background-color:#ff0'><b></b></td>");
                csv.Append("<td style='background-color:#ff0'><b>" + gstTotal.ToString("n2") + "</b></td>");
                csv.Append("<td style='background-color:#ff0'><b></b></td>");
                csv.Append("<td style='background-color:#ff0'><b></b></td>");
                csv.Append("<td style='background-color:#ff0'><b></b></td>");
                csv.Append("<td style='background-color:#ff0'><b></b></td>");
                csv.Append("<td style='background-color:#ff0'><b></b></td>");
                csv.Append("<td style='background-color:#ff0'><b></b></td>");
                csv.Append("<td style='background-color:#ff0'><b></b></td>");
                csv.Append("</tr>");
            }


            csv.Append("</table>");



            System.Web.UI.WebControls.GridView gv = new System.Web.UI.WebControls.GridView();
            //gv.DataSource = data;
            gv.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Member Transactions (" + model.Member_Name + ").xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            sw.Write(csv.ToString());
            System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View(model);
        }

        [HttpGet]
        [AllowAuthorized]
        public ActionResult MemberPurchases(POSMemberViewModel model, int? pMemberID) {
            DateTime? searchDateFrom = null;
            DateTime? searchDateTo = null;

            if (pMemberID.HasValue) {
                model = this.getMember(pMemberID.Value);
            } else {
                if (!string.IsNullOrEmpty(model.Search_Date_From)) {
                    searchDateFrom = DateUtil.ToDate(model.Search_Date_From, "/");
                }

                if (!string.IsNullOrEmpty(model.Search_Date_From)) {
                    searchDateTo = DateUtil.ToDate(model.Search_Date_To, "/");
                }

                model = this.getMember(model.Member_ID.Value, searchDateFrom, searchDateTo, model.Search_Status, model.Search_Terminal, model.Search_Receipt_No);
            }


            if (model != null) {
                return View(model);
            }
            return View();
        }

        [AllowAuthorized]
        public ActionResult Topup(Nullable<int> Member_ID, Nullable<decimal> Credits) {
            var userlogin = UserSession.getUser(HttpContext);
            var currentdate = StoredProcedure.GetCurrentDate();
            var mService = new MemberService();


            var mem = mService.GetMember(Member_ID);
            if (mem != null) {
                mem.Credit = Credits;
                var result = mService.UpdateMember(mem);
                return Json(result, JsonRequestBehavior.AllowGet);
            }


            return Json(new ServiceResult() { Code = ERROR_CODE.ERROR_514_INVALID_INPUT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_514_INVALID_INPUT_ERROR), Field = "Topup" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddProduct(int pIndex, int pProductID, Nullable<bool> pOpenBill, Nullable<decimal> pOpenBillAmount = 0M) {
            var userlogin = UserSession.getUser(HttpContext);
            var pService = new POSService();
            var iService = new InventoryService();
            var model = new POSProductViewModel() { Index = pIndex };

            var product = iService.GetProductAndAttribute(pProductID);

            if (product != null) {
                model.Code = product.Product_Code;
                model.Name = product.Product_Name;
                model.Product_ID = product.Product_ID;
                model.Price = product.Selling_Price.HasValue ? product.Selling_Price.Value : 0;
                model.Qty = 1;
                model.Product_Attribute = product.Product_Attribute;
                model.Image = product.Image != null ? Convert.ToBase64String(product.Image) : "";
                model.Product_Name = product.Product_Name;
            }

            if (pOpenBill.HasValue && pOpenBill.Value) {
                model.Name = model.Name + " (*)";
                model.Price = pOpenBillAmount.HasValue ? pOpenBillAmount.Value : 0;
            }
            return PartialView("POSSelectProductRow", model);
        }

        public ActionResult AddOtherProduct(int pIndex, int pProductID, string pProductName, int pQty, decimal pPrice, string pProductCode, int pProductCategoryId) {
            var userlogin = UserSession.getUser(HttpContext);

            var model = new POSProductViewModel() { Index = pIndex };

            model.Code = pProductCode;
            model.Name = pProductName;
            model.Product_ID = pProductID;
            model.Price = pPrice;
            model.Qty = pQty;

            if (pProductCategoryId > 0) {
                var currentdate = StoredProcedure.GetCurrentDate();

                model.Quick_Add_Product = true;

                var product = new Product() {
                    Product_Code = pProductCode,
                    Product_Name = pProductName,
                    Product_Category_L1 = pProductCategoryId,
                    Selling_Price = pPrice,
                    Type = "Stock",
                    Sellable = true,
                    Record_Status = RecordStatus.Active,
                    Company_ID = userlogin.Company_ID,
                    Create_By = userlogin.User_Authentication.Email_Address,
                    Create_On = currentdate,
                    Update_By = userlogin.User_Authentication.Email_Address,
                    Update_On = currentdate
                };

                var iService = new InventoryService();
                iService.InsertProduct(product, null, null, null, null, null, new Dictionary<string, string[]>(), null, null, null, null);

                model.Product_ID = product.Product_ID;
                model.Code = product.Product_Code;

            } else {
                model.Quick_Add_Product = false;
            }

            return PartialView("POSSelectProductRow", model);
        }

        public ActionResult GetRelatedProducts(int pProductId) {
            var model = new POSViewModel();
            var iService = new InventoryService();

            var relatedProducts = iService.LstRelatedProduct(pProductId);

            if (relatedProducts != null) {
                model.RelatedProducts = new List<Product>();

                foreach (var rProd in relatedProducts) {

                    Product product = null;

                    if (pProductId == rProd.Related_Product_ID) {
                        product = iService.GetProduct(rProd.Based_Product_ID);
                    } else {
                        product = iService.GetProduct(rProd.Related_Product_ID);
                    }

                    if (product != null) {
                        model.RelatedProducts.Add(product);
                    }
                }
            }

            return PartialView("POSRelatedProducts", model);
        }

        //GET: /POS/Report
        [HttpGet]
        public ActionResult Report(POSReportViewModel model) {

            User_Profile userlogin = UserSession.getUser(HttpContext);
            model.operation = UserSession.RIGHT_A;
            //Validate Page Right
            RightResult rightResult = base.validatePageRight(model.operation);
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;

            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            var currentdate = StoredProcedure.GetCurrentDate();
            if (string.IsNullOrEmpty(model.Start_Date))
                model.Start_Date = DateUtil.ToDisplayDate(currentdate);

            if (string.IsNullOrEmpty(model.End_Date))
                model.End_Date = DateUtil.ToDisplayDate(currentdate);

            var pService = new POSService();
            var cService = new POSConfigService();

            POS_Terminal posTerminal;
            if (AppSetting.POS_OFFLINE_CLIENT)
                posTerminal = cService.GetTerminalByMacAddress(cService.GetMacAddress());
            else
                posTerminal = cService.GetTerminalByCashierID(userlogin.Profile_ID);

            var cri = new POSReciptCriteria();
            cri.Company_ID = userlogin.Company_ID;
            cri.Cashier_ID = userlogin.Profile_ID;
            cri.Start_Date = DateUtil.ToDate(model.Start_Date);
            cri.End_Date = DateUtil.ToDate(model.End_Date);
            cri.Text_Search = model.TextSearch;
            cri.Branch_ID = posTerminal.Branch_ID;
            cri.User_Authentication_ID = userlogin.User_Authentication_ID;

            model.receipts = pService.LstPOSReceipt(cri);


            var company = new CompanyService().GetCompany(userlogin.Company_ID.Value);

            ViewBag.BusinessType = company.Business_Type;

            return View(model);
        }

        public ActionResult ExportExcel(POSReportViewModel model) {
            User_Profile userlogin = UserSession.getUser(HttpContext);
            model.operation = UserSession.RIGHT_A;
            //Validate Page Right
            RightResult rightResult = base.validatePageRight(UserSession.RIGHT_A, "/POS/Report");
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;

            bool orderByAsc = true;

            //Added by Nay on 16-Jun-2015
            var cbService = new ComboService();
            //Purpose : To take GST percentage for each record of respective company.
            decimal gstPercentage = 0;
            bool useGST = false;

            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            var rcpConfig = new ReceiptConfigService().GetReceiptConfigByCompany(userlogin.Company_ID.Value);
            if (rcpConfig == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            var currentdate = StoredProcedure.GetCurrentDate();
            if (string.IsNullOrEmpty(model.Start_Date))
                model.Start_Date = DateUtil.ToDisplayDate(currentdate);

            if (string.IsNullOrEmpty(model.End_Date))
                model.End_Date = DateUtil.ToDisplayDate(currentdate);

            var pService = new POSService();

            var posTerminal = new POSService().GetTerminal(userlogin.Profile_ID);

            var cri = new POSReciptCriteria();
            cri.Company_ID = userlogin.Company_ID;
            cri.Cashier_ID = userlogin.Profile_ID;
            cri.Start_Date = DateUtil.ToDate(model.Start_Date);
            cri.End_Date = DateUtil.ToDate(model.End_Date);
            cri.Status = ReceiptStatus.Paid;
            cri.Text_Search = model.TextSearch;
            cri.Branch_ID = posTerminal.Branch_ID;
            cri.User_Authentication_ID = userlogin.User_Authentication_ID;
            cri.orderByAsc = orderByAsc;

            model.receipts = pService.LstPOSReceipt(cri);


            var tax = new TaxServie().GetTax(userlogin.Company_ID);
            var surchargePercen = 0M;
            var surchargeInclude = false;
            var serviceChargePercen = 0M;
            var serviceChargeInclude = false;
            if (tax != null) {
                surchargeInclude = tax.Include_Surcharge.HasValue ? tax.Include_Surcharge.Value : false;
                if (surchargeInclude)
                    surchargePercen = tax.Surcharge_Percen.HasValue ? tax.Surcharge_Percen.Value : 0;

                serviceChargeInclude = tax.Include_Service_Charge.HasValue ? tax.Include_Service_Charge.Value : false;
                if (serviceChargeInclude)
                    serviceChargePercen = tax.Service_Charge_Percen.HasValue ? tax.Service_Charge_Percen.Value : 0;

                useGST = tax.Include_GST.HasValue ? tax.Include_GST.Value : false;
                var gst = tax.Tax_GST.Where(w => w.Tax_Type == TaxType.Exclusive && w.Record_Status == RecordStatus.Active && w.Is_Default == true).FirstOrDefault();
                if (gst != null) {
                    gstPercentage = gst.Tax.HasValue ? gst.Tax.Value : 0;
                }
            }

            var csv = new StringBuilder();

            csv.Append("<table border=1>");
            csv.Append("<tr>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.Branch + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.Date + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.ReceiptNo + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.MemberName + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.CategoryName + "</b></td>");
            //TEST
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.Product + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.Qty + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.UnitPrice + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.Amount + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.Discount + "(%)</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.Surcharge + "(%)</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.ReceivedAmount + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.Status + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.PaidBy + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.Terminal + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.Cashier + "</b></td>");

            csv.Append("</tr>");

            var currReceiptdate = "";
            var subTotal = 0M;
            var grandTotal = 0M;

            //Added by Nay on 17-Jun-2015 
            var gstTotal = 0M;

            var netTotal = 0M;
            var grandNetTotal = 0M;
            foreach (var row in model.receipts) {
                if (currReceiptdate != DateUtil.ToDisplayDate(row.Receipt_Date)) {
                    if (!string.IsNullOrEmpty(currReceiptdate)) {
                        csv.Append("<tr >");
                        csv.Append("<td style='background-color:#ff0'><b>Sub Total</b></td>");
                        csv.Append("<td style='background-color:#ff0'><b></b></td>");
                        csv.Append("<td style='background-color:#ff0'><b></b></td>");
                        //TEST
                        csv.Append("<td style='background-color:#ff0'><b></b></td>");
                        csv.Append("<td style='background-color:#ff0'><b></b></td>");
                        csv.Append("<td style='background-color:#ff0'><b></b></td>");
                        csv.Append("<td style='background-color:#ff0'><b>" + subTotal.ToString("n2") + "</b></td>");
                        csv.Append("<td style='background-color:#ff0'><b></b></td>");
                        csv.Append("<td style='background-color:#ff0'><b></b></td>");
                        csv.Append("<td style='background-color:#ff0'><b>" + netTotal.ToString("n2") + "</b></td>");
                        csv.Append("<td style='background-color:#ff0'><b></b></td>");
                        csv.Append("<td style='background-color:#ff0'><b></b></td>");
                        csv.Append("<td style='background-color:#ff0'><b></b></td>");
                        csv.Append("<td style='background-color:#ff0'><b></b></td>");
                        csv.Append("</tr>");
                    }
                    subTotal = 0;
                    netTotal = 0;

                    currReceiptdate = DateUtil.ToDisplayDate(row.Receipt_Date);
                }

                Nullable<decimal> discountPercen = 0;
                if (row.Discount_Type == DiscountType.Amount) {
                    if (row.Total_Amount > 0) {
                        discountPercen = ((100 * row.Discount) / row.Total_Amount);
                    } else {
                        discountPercen = 0;
                    }

                } else {
                    discountPercen = row.Discount;
                }

                var paymentType = "";
                //Added by Nay on 02-Sept-2015
                //To show sepprately "Normal" & "Combine" payments
                if (row.POS_Receipt_Payment.Count > 1) {
                    var combinePaid = new List<string>();
                    foreach (var paidBy in row.POS_Receipt_Payment) {
                        if (paidBy.Payment_Type == PaymentType.Cash)
                            combinePaid.Add("Cash");
                        else if (paidBy.Payment_Type == PaymentType.Nets)
                            combinePaid.Add("Nets");
                        else if (paidBy.Payment_Type == PaymentType.Credit_Card)
                            combinePaid.Add("Credit/Debit");
                    }
                    paymentType = combinePaid[0] + ", " + combinePaid[1];
                } else {
                    if (row.Payment_Type == PaymentType.Cash) {
                        paymentType = "Cash";
                    } else if (row.Payment_Type == PaymentType.Credit_Card) {
                        paymentType = "Credit/Debit";
                    }
                        //Added by Nay on 16-Jun-2015 
                        //Purpose : To show as if transaction was paid by Nets!
                      else if (row.Payment_Type == PaymentType.Nets) {
                        paymentType = "Nets";
                    }
                }


                var receiptProducts = orderByAsc ? row.POS_Products_Rcp.OrderBy(o => (o.Product != null && o.Product.Product_Category != null) ? o.Product.Product_Category.Category_Name : o.Product_Name).ThenBy(o => o.Product_Name) :
                    row.POS_Products_Rcp.OrderByDescending(o => (o.Product != null && o.Product.Product_Category != null) ? o.Product.Product_Category.Category_Name : o.Product_Name).ThenByDescending(o => o.Product_Name);

                foreach (var prow in receiptProducts) {
                    var amount = (prow.Qty.HasValue ? prow.Qty.Value : 0) * (prow.Price.HasValue ? prow.Price.Value : 0);
                    var receiveAmount = amount;
                    if (discountPercen > 0)
                        receiveAmount = amount - (amount * ((discountPercen.HasValue ? discountPercen.Value : 0) / 100));

                    //var cat = prow.Product.Product_Category;
                    var catname = "";
                    if (prow.Product != null && prow.Product.Product_Category != null) {
                        catname = prow.Product.Product_Category.Category_Name;
                    }
                    if (row.Payment_Type == PaymentType.Credit_Card) {
                        if (surchargeInclude) {
                            if (surchargePercen > 0) {
                                receiveAmount += (receiveAmount * (surchargePercen / 100));
                            }

                        }

                    }

                    csv.Append("<tr>");
                    csv.Append("<td>" + row.POS_Shift.POS_Terminal.Branch.Branch_Name + "</td>");
                    csv.Append("<td>" + DateUtil.ToDisplayDateTime(row.Receipt_Date) + "</td>");
                    csv.Append("<td>" + row.Receipt_No + "</td>");
                    csv.Append("<td>" + (row.Member != null ? row.Member.Member_Name : "") + "</td>");
                    csv.Append("<td>" + catname + "</td>");
                    //TEST
                    csv.Append("<td>" + prow.Product_Name + "</td>");
                    csv.Append("<td>" + (prow.Qty.HasValue ? prow.Qty.Value : 0).ToString("n0") + "</td>");
                    csv.Append("<td>" + (prow.Price.HasValue ? prow.Price.Value : 0).ToString("n2") + "</td>");
                    csv.Append("<td>" + amount.ToString("n2") + "</td>");
                    csv.Append("<td>" + (discountPercen.HasValue ? discountPercen.Value : 0).ToString("n2") + "</td>");
                    csv.Append("<td>" + surchargePercen.ToString("n2") + "</td>");
                    csv.Append("<td>" + receiveAmount.ToString("n2") + "</td>");
                    csv.Append("<td>" + row.Status + "</td>");
                    csv.Append("<td>" + paymentType + "</td>");
                    csv.Append("<td>" + (row.POS_Shift.POS_Terminal != null ? row.POS_Shift.POS_Terminal.Terminal_Name : "") + "</td>");
                    csv.Append("<td>" + (row.User_Profile != null ? row.User_Profile.Name : "") + "</td>");
                    csv.Append("</tr>");
                }

                subTotal += (row.Total_Amount.HasValue ? row.Total_Amount.Value : 0);
                grandTotal += (row.Total_Amount.HasValue ? row.Total_Amount.Value : 0);

                //Added by Nay on 17-Jun-2015 
                gstTotal += (decimal)(row.Total_GST_Amount.HasValue ? row.Total_GST_Amount : 0);

                netTotal += (row.Net_Amount.HasValue ? row.Net_Amount.Value : 0);
                grandNetTotal += (row.Net_Amount.HasValue ? row.Net_Amount.Value : 0);


                csv.Append("<tr >");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                //TEST
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b>Total Amount</b></td>");
                csv.Append("<td ><b>" + ((row.Total_Amount.HasValue ? row.Total_Amount.Value : 0).ToString("n2")) + "</b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("</tr>");

                //Added by Nay on 16-Jun-2015
                //Purpose : To show total amount of GST for current record as new line in excel!
                if (useGST) {
                    csv.Append("<tr >");
                    csv.Append("<td ><b></b></td>");
                    csv.Append("<td ><b></b></td>");
                    csv.Append("<td ><b></b></td>");
                    //TEST
                    csv.Append("<td ><b></b></td>");
                    csv.Append("<td ><b></b></td>");
                    csv.Append("<td ><b>GST (" + gstPercentage + "% Inclusive)</b></td>");
                    csv.Append("<td ><b>" + ((row.Total_GST_Amount.HasValue ? row.Total_GST_Amount.Value : 0).ToString("n2")) + "</b></td>");
                    csv.Append("<td ><b></b></td>");
                    csv.Append("<td ><b></b></td>");
                    csv.Append("<td ><b></b></td>");
                    csv.Append("<td ><b></b></td>");
                    csv.Append("<td ><b></b></td>");
                    csv.Append("<td ><b></b></td>");
                    csv.Append("<td ><b></b></td>");
                    csv.Append("</tr>");
                }


                decimal? totalDiscountPercen = null;

                if (row.Total_Amount > 0) {
                    totalDiscountPercen = ((100 * row.Total_Discount) / row.Total_Amount);
                } else {
                    totalDiscountPercen = 0;
                }

                csv.Append("<tr >");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                //TEST
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b>Item Discount (%)</b></td>");
                csv.Append("<td ><b>" + ((totalDiscountPercen.HasValue ? totalDiscountPercen.Value : 0).ToString("n2")) + "</b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("</tr>");

                decimal? memberDiscount = null;
                decimal? memberDiscountPercentage = null;

                if (row.Total_Amount > 0) {
                    if (row.Member_Discount_Type == "%") {
                        memberDiscount = ((row.Member_Discount.HasValue ? row.Member_Discount.Value : 0) / 100) * row.Total_Amount;
                    } else {
                        memberDiscount = row.Member_Discount;
                    }
                    memberDiscountPercentage = ((100 * memberDiscount) / row.Total_Amount);
                } else {
                    memberDiscountPercentage = 0;
                }

                csv.Append("<tr >");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                //TEST
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b>Member Discount (" + ((memberDiscountPercentage.HasValue ? memberDiscountPercentage.Value : 0).ToString("G0")) + " %)</b></td>");
                csv.Append("<td ><b>" + ((memberDiscount.HasValue ? memberDiscount.Value : 0).ToString("n2")) + "</b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("</tr>");

                if (row.POS_Receipt_Payment != null) {
                    foreach (var prow in row.POS_Receipt_Payment) {
                        if (prow.Payment_Type == PaymentType.Cash) {
                            var cashReceived = (prow.Payment_Amount.HasValue ? prow.Payment_Amount.Value : 0) - (row.Changes.HasValue ? row.Changes.Value : 0);
                            csv.Append("<tr >");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            //TEST
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b>Cash</b></td>");
                            csv.Append("<td ><b>" + cashReceived.ToString("n2") + "</b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("</tr>");

                            //csv.Append("<tr >");
                            //csv.Append("<td ><b></b></td>");
                            //csv.Append("<td ><b></b></td>");
                            //csv.Append("<td ><b></b></td>");
                            //csv.Append("<td ><b></b></td>");
                            //csv.Append("<td ><b>Changes</b></td>");
                            //csv.Append("<td ><b>" + ((row.Changes.HasValue ? row.Changes.Value : 0).ToString("n2")) + "</b></td>");
                            //csv.Append("<td ><b></b></td>");
                            //csv.Append("<td ><b></b></td>");
                            //csv.Append("<td ><b></b></td>");
                            //csv.Append("<td ><b></b></td>");
                            //csv.Append("<td ><b></b></td>");
                            //csv.Append("<td ><b></b></td>");
                            //csv.Append("<td ><b></b></td>");
                            //csv.Append("</tr>");
                        } else if (prow.Payment_Type == PaymentType.Nets) {
                            csv.Append("<tr >");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            //TEST
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            //Commented by Nay on 12-Aug-2015
                            //Wrong tab in exported excel file for Net amount
                            //csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b>Nets</b></td>");
                            csv.Append("<td ><b>" + ((prow.Payment_Amount.HasValue ? prow.Payment_Amount.Value : 0).ToString("n2")) + "</b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("</tr>");
                        } else if (prow.Payment_Type == PaymentType.Credit_Card) {
                            csv.Append("<tr >");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            //TEST
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            //Modified by Nay on 03-Sept-2015
                            //To show using card name
                            //csv.Append("<td ><b>Card</b></td>");
                            //csv.Append("<td ><b>" + prow.Global_Lookup_Data1.Name + "</b></td>");
                            if (prow.Card_Type.HasValue) {
                                var cardType = cbService.GetLookup(prow.Card_Type);
                                if (cardType != null)
                                    csv.Append("<td ><b>" + cardType.Name + "</b></td>");
                            } else {
                                csv.Append("<td ><b>Credit/Debit</b></td>");
                            }

                            csv.Append("<td ><b>" + ((prow.Payment_Amount.HasValue ? prow.Payment_Amount.Value : 0).ToString("n2")) + "</b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("<td ><b></b></td>");
                            csv.Append("</tr>");

                            if (surchargeInclude) {
                                csv.Append("<tr >");
                                csv.Append("<td ><b></b></td>");
                                csv.Append("<td ><b></b></td>");
                                csv.Append("<td ><b></b></td>");
                                //TEST
                                csv.Append("<td ><b></b></td>");
                                csv.Append("<td ><b></b></td>");
                                csv.Append("<td ><b>Surcharge (%)</b></td>");
                                csv.Append("<td ><b>" + ((surchargePercen).ToString("n2")) + "</b></td>");
                                csv.Append("<td ><b></b></td>");
                                csv.Append("<td ><b></b></td>");
                                csv.Append("<td ><b></b></td>");
                                csv.Append("<td ><b></b></td>");
                                csv.Append("<td ><b></b></td>");
                                csv.Append("<td ><b></b></td>");
                                csv.Append("<td ><b></b></td>");
                                csv.Append("</tr>");
                            }
                        }
                    }
                }

                csv.Append("<tr >");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                //TEST
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b>Received Amount</b></td>");
                csv.Append("<td ><b>" + ((row.Net_Amount.HasValue ? row.Net_Amount.Value : 0).ToString("n2")) + "</b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("<td ><b></b></td>");
                csv.Append("</tr>");
            }

            csv.Append("<tr >");
            csv.Append("<td style='background-color:#ff0'><b>Sub Total</b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            //TEST
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + subTotal.ToString("n2") + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + netTotal.ToString("n2") + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("</tr>");

            csv.Append("<tr >");
            csv.Append("<td style='background-color:#ff0'><b>Grand Total</b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            //TEST
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + grandTotal.ToString("n2") + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + grandNetTotal.ToString("n2") + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("<td style='background-color:#ff0'><b></b></td>");
            csv.Append("</tr>");

            //Added by Nay on 17-Jun-2015 
            //To show grand total of GST also show.
            if (useGST) {
                csv.Append("<tr >");
                csv.Append("<td style='background-color:#ff0'><b>Total GST</b></td>");
                csv.Append("<td style='background-color:#ff0'><b></b></td>");
                csv.Append("<td style='background-color:#ff0'><b></b></td>");
                //TEST
                csv.Append("<td style='background-color:#ff0'><b></b></td>");
                csv.Append("<td style='background-color:#ff0'><b></b></td>");
                csv.Append("<td style='background-color:#ff0'><b></b></td>");
                csv.Append("<td style='background-color:#ff0'><b>" + gstTotal.ToString("n2") + "</b></td>");
                csv.Append("<td style='background-color:#ff0'><b></b></td>");
                csv.Append("<td style='background-color:#ff0'><b></b></td>");
                csv.Append("<td style='background-color:#ff0'><b></b></td>");
                csv.Append("<td style='background-color:#ff0'><b></b></td>");
                csv.Append("<td style='background-color:#ff0'><b></b></td>");
                csv.Append("<td style='background-color:#ff0'><b></b></td>");
                csv.Append("<td style='background-color:#ff0'><b></b></td>");
                csv.Append("</tr>");
            }


            csv.Append("</table>");



            System.Web.UI.WebControls.GridView gv = new System.Web.UI.WebControls.GridView();
            //gv.DataSource = data;
            gv.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=pos report.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            sw.Write(csv.ToString());
            System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View(model);
        }

        //GET: /POS/VoidAndRefund
        [HttpGet]
        public ActionResult VoidAndRefund(POSReportViewModel model) {

            User_Profile userlogin = UserSession.getUser(HttpContext);
            model.operation = UserSession.RIGHT_A;
            //Validate Page Right
            RightResult rightResult = base.validatePageRight(model.operation);
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;

            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            var currentdate = StoredProcedure.GetCurrentDate();
            if (string.IsNullOrEmpty(model.Start_Date))
                model.Start_Date = DateUtil.ToDisplayDate(currentdate);

            if (string.IsNullOrEmpty(model.End_Date))
                model.End_Date = DateUtil.ToDisplayDate(currentdate);

            var pService = new POSService();

            var posTerminal = pService.GetTerminal(userlogin.Profile_ID);

            var cri = new POSReciptCriteria();
            cri.Company_ID = userlogin.Company_ID;
            cri.Cashier_ID = userlogin.Profile_ID;
            cri.Start_Date = DateUtil.ToDate(model.Start_Date);
            cri.End_Date = DateUtil.ToDate(model.End_Date);
            cri.Status = ReceiptStatus.Void;
            cri.Text_Search = model.TextSearch;
            cri.Branch_ID = posTerminal.Branch_ID;
            cri.User_Authentication_ID = userlogin.User_Authentication_ID;
            model.receipts = pService.LstPOSReceipt(cri);

            var company = new CompanyService().GetCompany(userlogin.Company_ID.Value);

            ViewBag.BusinessType = company.Business_Type;

            return View(model);

        }

        //GET: /POS/Report
        [HttpGet]
        public ActionResult ViewHoldBill(POSReportViewModel model) {

            User_Profile userlogin = UserSession.getUser(HttpContext);
            model.operation = UserSession.RIGHT_A;
            //Validate Page Right
            RightResult rightResult = base.validatePageRight(model.operation, "/POS/POS");
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;

            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            var currentdate = StoredProcedure.GetCurrentDate();
            if (string.IsNullOrEmpty(model.Start_Date))
                model.Start_Date = DateUtil.ToDisplayDate(currentdate);

            if (string.IsNullOrEmpty(model.End_Date))
                model.End_Date = DateUtil.ToDisplayDate(currentdate);

            var pService = new POSService();

            var posTerminal = pService.GetTerminal(userlogin.Profile_ID);

            var cri = new POSReciptCriteria();
            cri.Company_ID = userlogin.Company_ID;
            cri.Cashier_ID = userlogin.Profile_ID;
            cri.Start_Date = DateUtil.ToDate(model.Start_Date);
            cri.End_Date = DateUtil.ToDate(model.End_Date);
            cri.Status = ReceiptStatus.Hold;
            cri.Text_Search = model.TextSearch;
            cri.Branch_ID = posTerminal.Branch_ID;
            cri.User_Authentication_ID = userlogin.User_Authentication_ID;
            model.receipts = pService.LstPOSReceipt(cri);



            var company = new CompanyService().GetCompany(userlogin.Company_ID.Value);

            ViewBag.BusinessType = company.Business_Type;

            return View(model);

        }

        [HttpGet]
        public ActionResult DeleteHoldBill(Nullable<int> pReceiptID, string pStartDate, string pEndDate) {
            var model = new POSReportViewModel();
            model.Start_Date = pStartDate;
            model.End_Date = pEndDate;

            User_Profile userlogin = UserSession.getUser(HttpContext);
            model.operation = UserSession.RIGHT_A;

            //Validate Page Right
            RightResult rightResult = base.validatePageRight(model.operation, "/POS/POS");
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;

            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);


            var pService = new POSService();
            model.result = pService.DeletePOSReceipt(pReceiptID);

            return RedirectToAction("ViewHoldBill", model);
        }


        public ActionResult PrintReport(Nullable<int> pReceiptID = 0, string pUrl = "", bool isvoid = false) {
            User_Profile userlogin = UserSession.getUser(HttpContext);

            var result = new ServiceResult();

            //Validate Page Right
            RightResult rightResult = base.validatePageRight(UserSession.RIGHT_A, pUrl);
            if (rightResult.action != null) {
                result.Code = ERROR_CODE.ERROR_401_UNAUTHORIZED;
                result.Msg = new Error().getError(ERROR_CODE.ERROR_401_UNAUTHORIZED);
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            var taxService = new TaxServie();
            var tax = taxService.GetTax(userlogin.Company_ID);

            var pService = new POSService();

            if (userlogin == null) {
                result.Code = ERROR_CODE.ERROR_401_UNAUTHORIZED;
                result.Msg = new Error().getError(ERROR_CODE.ERROR_401_UNAUTHORIZED);
                return Json(result, JsonRequestBehavior.AllowGet);
            }


            var company = new CompanyService().GetCompany(userlogin.Company_ID.Value);
            if (company == null) {
                result.Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND;
                result.Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);
                result.Field = Resources.ResourceCompany.Company;
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            var rcp_config = new ReceiptConfigService().GetReceiptConfigByCompany(userlogin.Company_ID.Value);
            if (rcp_config == null) {
                result.Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND;
                result.Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);
                result.Field = Resources.ResourcePOS.ReceiptConfig;
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            if (!pReceiptID.HasValue) {
                result.Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND;
                result.Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);
                result.Field = Resources.ResourcePOS.Receipt;
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            POS_Receipt rcp = pService.GetPOSReceipt(pReceiptID);
            if (rcp == null) {
                result.Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND;
                result.Msg = new Error().getError(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);
                result.Field = Resources.ResourcePOS.Receipt;
                return Json(result, JsonRequestBehavior.AllowGet);
            }


            if (rcp.Company_ID != userlogin.Company_ID.Value) {
                result.Code = ERROR_CODE.ERROR_401_UNAUTHORIZED;
                result.Msg = new Error().getError(ERROR_CODE.ERROR_401_UNAUTHORIZED);
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            if (isvoid) {
                rcp.Status = ReceiptStatus.Void;
                result = pService.VoidPOSReceipt(pReceiptID);
                if (result.Code != ERROR_CODE.SUCCESS) {
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

            }

            if (!rcp_config.Ignore_Print.HasValue || !rcp_config.Ignore_Print.Value) {

                var shift = pService.GetShift(rcp.Shift_ID);
                if (shift != null) {
                    var terminal = pService.GetTerminalByTerminalID(shift.Terminal_ID);

                    string ipAddress = terminal.Printer_IP_Address;
                    string receipt = "";

                    if (terminal.Is_WebPRNT.HasValue && !terminal.Is_WebPRNT.Value) {
                        receipt = ReportUtil.receiptData(rcp, company, rcp_config, terminal, terminal.User_Profile, tax);
                        var msg = ReportUtil.printToPrinter(receipt, ipAddress);

                        if (!string.IsNullOrEmpty(msg)) {
                            result.Msg = msg;
                            return Json(result, JsonRequestBehavior.AllowGet);
                        }
                    } else {
                        receipt = ReportUtil.webPrntReceiptData(rcp, company, rcp_config, terminal, terminal.User_Profile, tax);
                        result.Msg = receipt;

                        return Json(new { ipAddress = ipAddress, Message = receipt }, JsonRequestBehavior.AllowGet);
                    }
                }

            }


            result.Code = ERROR_CODE.SUCCESS;
            result.Msg = "Sale completed successfully.";

            if (isvoid) result.Msg = "The sale has been voided successfully.";
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult PrintVoidAndRefund(Nullable<int> pReceiptID = 0, string pUrl = "") {
            return PrintReport(pReceiptID, pUrl, true);
        }

        [HttpGet]
        public ActionResult QOHReport(POSQoHReportViewModel model) {
            User_Profile userlogin = UserSession.getUser(HttpContext);
            model.operation = UserSession.RIGHT_A;

            //Validate Page Right
            RightResult rightResult = base.validatePageRight(model.operation);
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;



            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            var pService = new POSService();
            var terminal = pService.GetTerminal(userlogin.Profile_ID);
            var iService = new InventoryService();

            //Modified by Nay on 14-Oct-2015 
            //Do not allow to show the Categoreis which are don't have procuts
            //model.CategoryList = pService.GetProductCategory(userlogin.Company_ID.Value, true);

            var ccri = new CategoryCriteria() {
                Company_ID = userlogin.Company_ID,
                Branch_ID = terminal.Branch_ID,
                hasBlank = true
            };

            model.CategoryList = iService.LstCategory(ccri);

            model.inventory_qoh = pService.GetProductQuantityOnHand(userlogin.Company_ID.Value, model.CategoryID, model.TextSearch, true);

            return View(model);
        }

        public ActionResult QOHExportExcel(POSQoHReportViewModel model) {
            User_Profile userlogin = UserSession.getUser(HttpContext);
            model.operation = UserSession.RIGHT_A;
            //Validate Page Right
            RightResult rightResult = base.validatePageRight(UserSession.RIGHT_A, "/POS/Report");
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;

            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            var rcpConfig = new ReceiptConfigService().GetReceiptConfigByCompany(userlogin.Company_ID.Value);
            if (rcpConfig == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            var pService = new POSService();
            var iService = new InventoryService();
            //Modified by Nay on 14-Oct-2015 
            //Do not allow to show the Categoreis which are don't have procuts
            var terminal = pService.GetTerminal(userlogin.Profile_ID);

            var ccri = new CategoryCriteria() {
                Company_ID = userlogin.Company_ID,
                Branch_ID = terminal.Branch_ID,
                hasBlank = true
            };

            model.CategoryList = iService.LstCategory(ccri);

            model.inventory_qoh = pService.GetProductQuantityOnHand(userlogin.Company_ID.Value, model.CategoryID, model.TextSearch, true);


            var csv = new StringBuilder(); ;

            csv.Append("<table border=1>");
            csv.Append("<tr>");
            csv.Append("<td style='background-color:#ff0'><b>SN.</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.Category + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.ProductCode + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.ProductName + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.TotalReceived + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.TotalSold + "</b></td>");
            csv.Append("<td style='background-color:#ff0'><b>" + Resources.ResourcePOS.QoH + "</b></td>");

            csv.Append("</tr>");

            var counter = 0;

            foreach (var row in model.inventory_qoh) {
                counter++;
                csv.Append("<tr>");
                csv.Append("<td align='center'>" + counter.ToString() + "</td>");
                csv.Append("<td>" + row.Category_Name + "</td>");
                csv.Append("<td>" + row.Product_Code + "</td>");
                csv.Append("<td>" + row.Product_Name + "</td>");
                csv.Append("<td align='center'>" + row.InvIN + "</td>");
                csv.Append("<td align='center'>" + row.POSOUT + "</td>");
                csv.Append("<td align='center'>" + row.QoH + "</td>");
                csv.Append("</tr>");
            }

            csv.Append("</table>");

            System.Web.UI.WebControls.GridView gv = new System.Web.UI.WebControls.GridView();
            gv.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=qoh pos report - " + DateTime.Today.ToString("yyyyMMdd") + ".xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            sw.Write(csv.ToString());
            System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View(model);
        }


        private POSMemberViewModel getMember(int pMemberID,
            Nullable<DateTime> searchDateFrom = null, Nullable<DateTime> searchDateTo = null, string searchStatus = null,
            string searchTerminal = null, string searchReceiptNo = null) {
            var mService = new MemberService();
            var mem = mService.GetMember(pMemberID);
            POSMemberViewModel model = null;

            if (mem != null) {
                model = new POSMemberViewModel() {
                    Member_ID = mem.Member_ID,
                    Credit = (mem.Credit.HasValue ? mem.Credit.Value : 0),
                    DOB = DateUtil.ToDisplayDate(mem.DOB),
                    Email = mem.Email,
                    Member_Card_No = mem.Member_Card_No,
                    Member_Discount = mem.Member_Discount,
                    Member_Discount_Type = mem.Member_Discount_Type,
                    Member_Name = mem.Member_Name,
                    Member_Status = mem.Member_Status,
                    NRIC_No = mem.NRIC_No,
                    Phone_No = mem.Phone_No
                };

                if (mem.POS_Receipt != null) {
                    model.purchaseList = new List<POS_Receipt>();

                    IEnumerable<POS_Receipt> posReceipts = mem.POS_Receipt;

                    if (searchDateFrom != null) {
                        posReceipts = posReceipts.Where(r => r.Receipt_Date >= searchDateFrom);
                    }

                    if (searchDateTo != null) {
                        posReceipts = posReceipts.Where(r => r.Receipt_Date >= searchDateFrom);
                    }

                    if (!string.IsNullOrEmpty(searchStatus)) {
                        posReceipts = posReceipts.Where(r => r.Status.ToLower().Equals(searchStatus.ToLower()));
                    }

                    if (!string.IsNullOrEmpty(searchTerminal)) {
                        posReceipts = posReceipts.Where(r => r.POS_Shift.POS_Terminal.Terminal_Name.ToLower().Contains(searchTerminal.ToLower()));
                    }

                    if (!string.IsNullOrEmpty(searchReceiptNo)) {
                        posReceipts = posReceipts.Where(r => r.Receipt_No.ToLower().Contains(searchReceiptNo.ToLower()));
                    }

                    foreach (var rcpt in posReceipts.OrderByDescending(p => p.Receipt_Date)) {
                        model.purchaseList.Add(rcpt);
                    }
                }
            }

            return model;
        }

        private List<CustomerPurchaseViewModel> getCustomerPurchases(int companyId, int categoryId, string textSearch) {
            var pService = new POSService();
            var iService = new InventoryService();
            var customers = new List<CustomerPurchaseViewModel>();

            var cri = new CustomerPurchaseCriteria() {
                Company_ID = companyId,
                Text_Search = textSearch,
            };

            if (categoryId > 0) {
                cri.Product_Category_ID = categoryId;
            }

            var receipts = pService.LstPOSReceipt(cri);

            if (receipts != null) {

                foreach (var rcp in receipts) {
                    if (!string.IsNullOrEmpty(rcp.Customer_Name)) {
                        var p = new CustomerPurchaseViewModel();

                        p.Member_Id = rcp.Member_ID;
                        p.Name = rcp.Customer_Name;
                        p.Contact_No = rcp.Contact_No;
                        p.Email = rcp.Customer_Email;
                        p.Purchase_Date = rcp.Receipt_Date;

                        customers.Add(p);
                    }
                }
            }

            return customers;
        }


    }

}