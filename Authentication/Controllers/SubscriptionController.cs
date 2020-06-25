using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Authentication.Models;
using SBSModel.Models;
using SBSModel.Common;
using SBSResourceAPI;
using Authentication.Common;
using System.Web.Routing;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.IO;
using System.Text;
using Ionic.Zip;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using Renci.SshNet;
using System.IO.Compression;


namespace Authentication.Controllers
{
   public class SubscriptionController : ControllerBase
   {
      //
      // GET: /Subscription/
      [HttpGet]
      public ActionResult CompanySignUp(Nullable<int> pBID, SignUpViewModel model)
      {
         var userService = new UserService();
         var comService = new CompanyService();
         var cbService = new ComboService();

         model.countryList = cbService.LstCountry(true);
         model.stateList = new List<ComboViewModel>();

         return View(model);
      }

      //[HttpPost]
      //public ActionResult CompanySignUp(SignUpViewModel model)
      //{
      //   var userService = new UserService();
      //   if (!string.IsNullOrEmpty(model.Email))
      //   {
      //      var dup = userService.getUserByEmail(model.Email);
      //      if (dup != null)
      //      {
      //         ModelState.AddModelError("Email", Resource.The + " " + Resource.Email + " " + Resource.Field + " " + Resource.Is_Duplicated_Lower);
      //      }
      //   }

      //   if (ModelState.IsValid)
      //   {
      //      //save company
      //      return RedirectToAction("ModuleSignUp", model);
      //   }

      //   var comService = new CompanyService();
      //   var cbService = new ComboService();

      //   model.countryList = cbService.LstCountry(true);
      //   model.stateList = new List<ComboViewModel>();
      //   if (model.Country_ID.HasValue)
      //   {
      //      model.stateList = cbService.LstState(model.Country_ID.Value.ToString(), true);
      //   }
      //   else if (model.countryList.Count() > 0)
      //   {
      //      model.stateList = cbService.LstState(model.countryList[0].Value, true);
      //   }

      //   return View(model);
      //}

      [HttpPost]
      public ActionResult CompanySignUp(SignUpViewModel model)
      {
          var userService = new UserService();
          if (!string.IsNullOrEmpty(model.Email))
          {
              var dup = userService.getUserByEmail(model.Email);
              if (dup != null)
              {
                  ModelState.AddModelError("Email", Resource.The + " " + Resource.Email + " " + Resource.Field + " " + Resource.Is_Duplicated_Lower);
              }
          }

          if (ModelState.IsValid)
          {
            var result = RegisterCompany(model);
                if (result.Code == ERROR_CODE.SUCCESS )
                {
                    return RedirectToAction("RegistrationComplete", new MessagePageViewModel { });                       
                }                           
          }
          var comService = new CompanyService();
          var cbService = new ComboService();

          model.countryList = cbService.LstCountry(true);
          model.stateList = new List<ComboViewModel>();
          if (model.Country_ID.HasValue)
          {
              model.stateList = cbService.LstState(model.Country_ID.Value.ToString(), true);
          }
          else if (model.countryList.Count() > 0)
          {
              model.stateList = cbService.LstState(model.countryList[0].Value, true);
          }

          return View(model);        
      }

      private ServiceResult RegisterCompany(SignUpViewModel model)
      {
          var comService = new CompanyService();
          var userlogin = UserSession.getUser(HttpContext);
          var currentdate = StoredProcedure.GetCurrentDate();
          var com = new Company();
          var detail = new Company_Details();

          detail.Company_Level = Companylevel.EndUser;
          detail.No_Of_Employees = 1;
          detail.Name = model.Company_Name;
          detail.Effective_Date = currentdate;
          detail.Address = model.Address;
          detail.Country_ID = model.Country_ID;
          detail.State_ID = model.State_ID;
          detail.Zip_Code = model.Zip_Code;
          detail.Billing_Address = model.Address;
          detail.Billing_Country_ID = model.Country_ID;
          detail.Billing_State_ID = model.State_ID;
          detail.Billing_Zip_Code = model.Zip_Code;
          detail.Phone = model.Phone;
          detail.Email = model.Email;
          detail.Belong_To_ID = 0;
          detail.Company_Status = RecordStatus.Active;
          detail.Create_On = currentdate;
          if(userlogin != null)
          {
              detail.Create_By = userlogin.User_Authentication.Email_Address;
              detail.Update_By = userlogin.User_Authentication.Email_Address;
          }
          else
          {
              detail.Create_By = model.Email;
              detail.Update_By = model.Email;
          }

          detail.Update_On = currentdate;

          var cbService = new ComboService();
          var currencyInfo = cbService.GetCurrencyCodeByCountry(model.Country_ID);
          detail.Currency_ID = currencyInfo.Currency_ID;
          detail.Registration_Date = currentdate;
          //detail.Fax = model.Fax;

          var user = new User_Profile();
          user.Email = model.Email;
          user.First_Name = model.First_Name;
          user.Last_Name = model.Last_Name;
          user.Middle_Name = model.Middle_Name;
          user.Phone = model.Phone;
          user.Registration_Date = currentdate;
          user.User_Status = RecordStatus.Active;
          user.Create_On = currentdate;
          user.Create_By = userlogin.User_Authentication.Email_Address;
          user.Update_On = currentdate;
          user.Update_By = userlogin.User_Authentication.Email_Address;

          com.User_Profile.Add(user);

          var sService = new SubscriptionService();
          var mmap = sService.GetModuleMappingList(1);
          Subscription[] lstSub = new Subscription[mmap.Count()];
          if (mmap != null)
          {
              var i = 0;
              foreach (var m in mmap)
              {
                  var sModel = new Subscription();
                  sModel.Module_Detail_ID = m.Module_Detail_ID;
                  sModel.No_Of_Users = 1;
                  sModel.Period_Month = 1;
                  sModel.Company_ID = model.Subscription[0].Company_ID;
                  lstSub[i] = sModel;
                  i++;
              }
          }
          model.Subscription = lstSub;

          if (model.Subscription != null)
          {
              foreach (var row in model.Subscription)
              {
                  if (model.Select_Module != null && model.Select_Module.Contains(row.Module_Detail_ID.Value))
                  {
                      row.Start_Date = currentdate;
                      com.Subscriptions.Add(row);
                  }
              }
          }
          com.Company_Details.Add(detail);
          //Company
          com.Create_On = currentdate;
          com.Create_By = userlogin.User_Authentication.Email_Address;
          com.Update_On = currentdate;
          com.Update_By = userlogin.User_Authentication.Email_Address;

          model.result = comService.InsertCompany(com);
          if (model.result.Code == ERROR_CODE.SUCCESS)
          {
              //Added by Moet on 2/Sept
              var pService = new PayrollService();
              var prg = new PRG();
              prg.Company_ID = model.Company_ID.Value;
              prg.Name = "Default Group";
              prg.Create_On = currentdate;
              prg.Create_By = userlogin.User_Authentication.Email_Address;
              model.result = pService.InsertPRG(prg);
                            
              if (model.result.Code == ERROR_CODE.SUCCESS)
              {
                  try
                  {
                      //SEND EMAIL
                      //5.2	System	Send email to that email to the registered email for instruction of activation
                      var moduleName = string.Empty;
                      if (AppSetting.IsLive == "true")
                          moduleName = ModuleDomain.Authentication;

                      if (AppSetting.IsStaging == "true")
                          moduleName = "AuthenSBS2-staging";

                      var domain = UrlUtil.GetDomain(Request.Url, moduleName);


                      //Edit by sun 10-02-2016
                      var comCurr = comService.GetCompany(userlogin.Company_ID);
                      if (comCurr == null)
                      {
                          model.result.Code = ERROR_CODE.ERROR_401_UNAUTHORIZED;
                      }
                      else
                      {
                          EmailTemplete.sendUserActivateEmail(model.Email, model.result.Object.ToString(), UserSession.GetUserName(user), detail.Name, comCurr.Phone, comCurr.Email, domain);
                      }

                      //EmailTemplete.sendUserActivateEmail(model.Email, model.result.Object.ToString(),UserSession.GetUserName(user), detail.Name, detail.Phone, detail.Email, domain);
                      //EmailTemplete.sendCustomerAdminActivateEmail(model.Email, model.result.Object.ToString(), UserSession.GetUserName(user), domain);
                  }
                  catch
                  {
                      model.result.Code = ERROR_CODE.ERROR_501_CANT_SEND_EMAIL;
                  }
              }
              
              return model.result;
          }
          return model.result;
      }
      [HttpGet]
      public ActionResult ModuleSignUp(ServiceResult result, SignUpViewModel model, string pComID)
      {          
         var userService = new UserService();
         var comService = new CompanyService();
         var cbService = new ComboService();

         model.result = result;
         model.countryList = cbService.LstCountry();
         if (!string.IsNullOrEmpty(pComID))
         {
            var comID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pComID));
            var com = comService.GetCompany(comID);
            if (com != null)
            {
               model.First_Name = "test";
               model.Last_Name = "test";
               model.Company_ID = com.Company_ID;
               model.Company_Name = com.Name;
               model.Address = com.Address;
               model.Country_ID = com.Country_ID;
               model.State_ID = com.State_ID;
               model.Zip_Code = com.Zip_Code;
               model.Email = com.Email;
               model.A7_Group_ID = com.A7_Group_ID;
               if (model.Country_ID.HasValue)
               {
                  model.stateList = cbService.LstState(model.Country_ID.Value.ToString(), true);
               }
               else if (model.countryList.Count() > 0)
               {
                  model.stateList = cbService.LstState(model.countryList[0].Value, true);
               }

               model.SubscriptionList = comService.LstSubscription(model.Company_ID).ToList();
            }
         }
         model.ModuleList = comService.LstModule();


         if (model.Select_Module_Str != null)
         {
            var sel = new List<int>();
            var sp = model.Select_Module_Str.Split('|');
            foreach (var m in sp)
            {
               if (!string.IsNullOrEmpty(m))
               {
                  sel.Add(NumUtil.ParseInteger(m));
               }
            }
            model.Select_Module = sel.ToArray();
         }
         return View(model);
      }

      [HttpPost]
      public ActionResult ModuleSignUp(SignUpViewModel model)
      {

         if (model.Select_Module == null || model.Select_Module.Length == 0)
         {
            ModelState.AddModelError("Select_Module", Resource.Please_Select_Item);
         }

         if (ModelState.IsValid)
         {
            //save company
            var mstr = "";
            foreach (var m in model.Select_Module)
            {
               mstr = mstr + m + "|";
            }
            model.Select_Module_Str = mstr;
            return RedirectToAction("DetailSignUp", model);
         }

         var userService = new UserService();
         var comService = new CompanyService();
         var cbService = new ComboService();

         model.ModuleList = comService.LstModule();
         return View(model);
      }

      [HttpGet]
      public ActionResult DetailSignUp(ServiceResult result, SignUpViewModel model)
      {                  
         var userService = new UserService();
         var comService = new CompanyService();
         var cbService = new ComboService();

         var userlogin = UserSession.getUser(HttpContext);
         model.result = result;         
         model.Company_ID = userlogin.Company_ID;
         var c = comService.GetCompany(userlogin.Company_ID);
          if (c != null)
          {
              model.Company_Name = c.Name;
          }
         //var sel = new List<int>();
         //if (model.Select_Module_Str != null)
         //{
         //   var sp = model.Select_Module_Str.Split('|');
         //   foreach (var m in sp)
         //   {
         //      if (!string.IsNullOrEmpty(m))
         //      {
         //         sel.Add(NumUtil.ParseInteger(m));
         //      }
         //   }
         //}         
         //if (sel != null)
         //{
         //   foreach (var row in model.ModuleList)
         //   {
         //      if (sel.Contains(row.Module_Detail_ID))
         //      {
         //         var detail = new SignUpDetailViewModel();
         //         detail.Module_ID = row.Module_ID;
         //         detail.Module_Detail_ID = row.Module_Detail_ID;
         //         detail.Module_Detail_Name = row.Module_Detail_Name;
         //         detail.Module_Detail_Description = row.Module_Detail_Description;
         //         detail.Price = row.Price;
         //         detail.Price_Per_Person = row.Price_Per_Person;
         //         details.Add(detail);
         //      }
         //   }
         //}
         var details = new List<SignUpDetailViewModel>();
         var prices = new List<ModulePriceModel>();
         var detail = new SignUpDetailViewModel(); 
          //Added by Moet
          var PromotionID = 0;
          var PromotionName = "";
          var PromotionDesc = "";
          
          HttpWebResponse response = null;          
          StreamReader readStream = null;
          var request = (HttpWebRequest)WebRequest.Create(string.Format(AppSetting.WSVR_URL + "/GetPromotionByPromotionID?pID=1"));
          request.Method = "Get";
          try
          {
              response = (HttpWebResponse)request.GetResponse();
              Stream receiveStream = response.GetResponseStream();
              readStream = new StreamReader(receiveStream, Encoding.UTF8);
              var rawJson = readStream.ReadToEnd();
              var json = JObject.Parse(rawJson);
              PromotionID = json["Data"]["Promotion_ID"].ToObject<int>();
              PromotionName = json["Data"]["Promotion_Name"].ToString();
              PromotionDesc = json["Data"]["Promotion_Description"].ToString();
              if(json["Data"]["Prices"].Count() > 0)
              {
                  for (var i = 0; i < json["Data"]["Prices"].Count(); i++) {
                    var obj = json["Data"]["Prices"][i];
                    var p = new ModulePriceModel();                    
                    p.Qty_From = obj["Qty_From"].ToObject<int>();
                    p.Qty_To = obj["Qty_To"].ToObject<int>();
                    p.Price = obj["Price"].ToObject<decimal>();
                    prices.Add(p);          
                  }
                  detail.Prices = prices;
              }
          }
          catch (Exception ex)
          {                            
              model.result.Code = 180;
              model.result.Msg = ex.Message;
              model.result.Field = Resource.Subscription;
          }
          finally
          {
              // Don't forget to close your response.
              if (response != null)
              {
                  response.Close();
              }
          }
          detail.Module_ID = PromotionID;
          detail.Module_Detail_ID = PromotionID;
          detail.Module_Detail_Name = PromotionName;
          detail.Module_Detail_Description = PromotionDesc;
          detail.Price = 0; //Not necessary
          detail.Price_Per_Person = 0; //No necessary
         details.Add(detail);

         model.Details = details.ToArray();
         model.SubscriptionList = comService.LstSubscription(model.Company_ID).ToList();

         var mList = new List<SBS_Module_Detail>(); 
          var m = new SBS_Module_Detail(); 
         m.Module_ID = PromotionID;
          m.Module_Detail_ID = PromotionID;
          m.Module_Detail_Name = PromotionName;
          m.Module_Detail_Description = PromotionDesc;
          mList.Add(m);
          model.ModuleList = mList;
         return View(model);
      }

      [HttpPost]
      public ActionResult DetailSignUp(SignUpViewModel model)
      {
          var pageAction = "";
          var comService = new CompanyService();
          var userlogin = UserSession.getUser(HttpContext);
          var prices = new List<ModulePriceModel>();
          var detail = new SignUpDetailViewModel();            

          if (userlogin != null)
          {
              //get pricing from Billing
              HttpWebResponse response = null;
              StreamReader readStream = null;
              var sService = new SubscriptionService();
              var request = (HttpWebRequest)WebRequest.Create(string.Format(AppSetting.WSVR_URL + "/GetPromotionByPromotionID?pID=1"));
              request.Method = "Get";
              try
              {
                  response = (HttpWebResponse)request.GetResponse();
                  Stream receiveStream = response.GetResponseStream();
                  readStream = new StreamReader(receiveStream, Encoding.UTF8);
                  var rawJson = readStream.ReadToEnd();
                  var json = JObject.Parse(rawJson);
                  detail.Module_ID = json["Data"]["Promotion_ID"].ToObject<int>();
                  detail.Module_Detail_Name = json["Data"]["Promotion_Name"].ToString();
                  if (json["Data"]["Prices"].Count() > 0)
                  {
                      for (var i = 0; i < json["Data"]["Prices"].Count(); i++)
                      {
                          var obj = json["Data"]["Prices"][i];
                          var p = new ModulePriceModel();
                          p.Qty_From = obj["Qty_From"].ToObject<int>();
                          p.Qty_To = obj["Qty_To"].ToObject<int>();
                          p.Price = obj["Price"].ToObject<decimal>();
                          prices.Add(p);
                      }
                      detail.Prices = prices;
                  }
              }
              catch (Exception ex)
              {
                  model.result.Code = 180;
                  model.result.Msg = ex.Message;
                  model.result.Field = Resource.Subscription;
              }
              finally
              {
                  // Don't forget to close your response.
                  if (response != null)
                  {
                      response.Close();
                  }
              }

              var mmap = sService.GetModuleMappingList(model.Subscription[0].Module_Detail_ID.Value);
              Subscription[] lstSub = new Subscription[mmap.Count()]; 
              if (mmap != null)
              {
                  var i = 0;
                  foreach (var m in mmap)
                  {
                      var sModel = new Subscription();
                      sModel.Module_Detail_ID = m.Module_Detail_ID;
                      sModel.No_Of_Users = model.Subscription[0].No_Of_Users;
                      sModel.Period_Month = model.Subscription[0].Period_Month;
                      sModel.Company_ID = model.Subscription[0].Company_ID;
                      if (detail.Prices.Count > 0)
                      {
                          foreach (var p in detail.Prices)
                          {
                              if (sModel.No_Of_Users >= p.Qty_From && sModel.No_Of_Users <= p.Qty_To)
                              {
                                  sModel.Price = p.Price;
                              }
                          }
                      }
                      lstSub[i] = sModel;
                      i++;
                  }
              }
              model.Subscription = lstSub;
                            
              var uService = new UserService();
              var comlogin = comService.GetCompany(userlogin.Company_ID);
              if (comlogin != null && comlogin.Company_Level != Companylevel.EndUser)
              {
                  if (model.Subscription != null)
                  {
                      User_Profile comadmin = null;
                      //var sService = new SubscriptionService();
                      //var mmap = sService.GetModuleMappingList(model.Subscription[0].Module_Detail_ID.Value);
                      //Subscription[] lstSub = new Subscription[mmap.Count()];
                      
                      foreach (var sub in model.Subscription)
                      {
                          if (comadmin == null)
                              comadmin = uService.getCompanyAdminUser(sub.Company_ID);
                          if (comadmin != null)
                          {
                              var uam = new User_Assign_Module();
                              uam.Profile_ID = comadmin.Profile_ID;
                              sub.User_Assign_Module.Add(uam);
                          }
                          sub.Create_By = userlogin.Email;
                          sub.Create_On = StoredProcedure.GetCurrentDate();
                          sub.Start_Date = StoredProcedure.GetCurrentDate();
                      }
                      var result = comService.InsertSubscription(model.Subscription);
                      var bSuccess = SaveSubscriptionToBilling(model.Subscription, mmap);
                      if (result.Code == ERROR_CODE.SUCCESS && bSuccess == true)
                      {
                          var route = new RouteValueDictionary(result);
                          route.Add("pComID", EncryptUtil.Encrypt(comadmin.Company_ID));
                          return RedirectToAction("CompanyInfo", "Company", route);
                      }
                  }

                  return RedirectToAction("PaymentError", new { pageAction = "Company" });
              }
              else
              {
                  System.Web.HttpContext.Current.Session["Subscription_PageAction"] = "";
                  if (model.Company_ID.HasValue)
                  {
                      System.Web.HttpContext.Current.Session["Subscription_PageAction"] = "Company";
                  }
                  var bluecube = comService.GetCompany(1);
                  NVPAPICaller payPalCaller = new NVPAPICaller();
                  //Edit by sun 11-01-2015
                  //payPalCaller.SetCredentials(bluecube.APIUsername, bluecube.APIPassword, bluecube.APISignature,false);
                  payPalCaller.SetCredentials(bluecube.APIUsername, bluecube.APIPassword, bluecube.APISignature, bluecube.Is_Sandbox);
                  string retMsg = "";
                  string token = "";

                  string returnURL = UrlUtil.GetDomain(Request.Url, ModuleDomain.Authentication) + "Subscription/PaymentComplete";
                  string cancelURL = UrlUtil.GetDomain(Request.Url, ModuleDomain.Authentication) + "Home/Index";

                  var items = new List<Paypal>();
                  var total = 0M;

                  if (model.Company_ID.HasValue && model.Company_ID.Value > 0)
                  {
                      var SubscriptionList = comService.LstSubscription(model.Company_ID).ToList();
                  }
                  if (model.Subscription != null)
                  {
                      var sub = model.Subscription[0];
                      var sumamout = sub.Period_Month * sub.Price;
                      total += (sub.No_Of_Users.Value * sumamout.Value);
                      items.Add(new Paypal() { Item_Name = detail.Module_Detail_Name, Amount = sumamout, Qty = sub.No_Of_Users });
                  }
                  bool ret = payPalCaller.ShortcutExpressCheckout(total, ref token, ref retMsg, items, returnURL, cancelURL);
                  if (ret)
                  {
                      //Edit by sun 11-02-2016
                      System.Web.HttpContext.Current.Session["token"] = token;
                      System.Web.HttpContext.Current.Session["Paypal_Amount"] = total;
                      System.Web.HttpContext.Current.Session["Subscription_Register"] = model;
                      System.Web.HttpContext.Current.Response.Redirect(retMsg);
                      return null;
                      //Session["token"] = token;
                      //Session["Paypal_Amount"] = total;
                      //Session["Subscription_Register"] = model;
                      //Response.Redirect(retMsg);
                  }
                  else
                  {
                      // return error by paypal
                      return RedirectToAction("PaymentError", new { pageAction = pageAction });
                  }
              }
          }
         return RedirectToAction("PaymentError", new { pageAction = pageAction });
      }
      private bool SaveSubscriptionToBilling(Subscription[] lstSubscription, List<Module_Mapping> lstModuleMap)
      {
          HttpWebResponse response = null;
          StreamReader readStream = null;
          var s = lstSubscription[0];
          var m = lstModuleMap.Where(w => w.Module_Detail_ID == s.Module_Detail_ID).First();
          var request = (HttpWebRequest)WebRequest.Create(string.Format(AppSetting.WSVR_URL + "/SaveSubscriptionInfo?Product_ID="
          + m.Product_ID + "&Promotion_ID=" + m.Promotion_ID
          + "&Map_Subscription_ID=" + s.Subscription_ID + "&Map_Module_ID=" + m.Module_ID
          + "&Company_ID=" + s.Company_ID + "&Start_Date=" + s.Start_Date
          + "&No_Of_Users=" + s.No_Of_Users + "&Period_Month=" + s.Period_Month
          + "&Period_Date=" + s.Period_Day + "&Record_Status=" + s.Active
          + "&Price=" + s.Price + "&Create_By=" + s.Create_By
          + "&Create_On=" + s.Create_On));
          request.Method = "Get";
          try
          {
              response = (HttpWebResponse)request.GetResponse();
              Stream receiveStream = response.GetResponseStream();
              readStream = new StreamReader(receiveStream, Encoding.UTF8);
              var rawJson = readStream.ReadToEnd();
              var json = JObject.Parse(rawJson);
              //if (json != null && json.Count > 0)
              //    return json["result"].ToObject<bool>();
          }
          catch (Exception ex)
          {
              return false;
          }
          finally
          {
              if (readStream != null)
                  readStream.Close();
              if (response != null)
                  response.Close();
          }
          return true;
      }
      [HttpGet]
      public ActionResult PaymentComplete(string token, string PayerID)
      {

         var a = Session["Paypal_Amount"];
         var t = Session["token"];

         if (System.Web.HttpContext.Current.Session["token"] == null || System.Web.HttpContext.Current.Session["token"].ToString() != token | System.Web.HttpContext.Current.Session["Paypal_Amount"] == null)
            return RedirectToAction("PaymentError");

         var pageAction = Session["Subscription_PageAction"].ToString();

         string retMsg = "";
         NVPAPICaller payPalCaller = new NVPAPICaller();
         NVPCodec decoder = new NVPCodec();
         string payerId = "";
         bool ret = payPalCaller.GetCheckoutDetails(token, ref payerId, ref decoder, ref retMsg);
         if (ret)
         {
            var amount = NumUtil.ParseDecimal(Session["Paypal_Amount"].ToString());
            ret = payPalCaller.DoCheckoutPayment(amount.ToString(), token, PayerID, ref decoder, ref retMsg);
            if (ret)
            {
            }
            else
            {
               return RedirectToAction("PaymentError", new MessagePageViewModel { pageAction = pageAction });
            }
         }
         else
         {
            return RedirectToAction("PaymentError", new MessagePageViewModel { pageAction = pageAction });
         }
         //Moet added on 2/Sep
         var pModel = (InvoiceViewModels)Session["InvoiceViewModels"];
         var sService = new SubscriptionService();
         var userlogin = UserSession.getUser(HttpContext);
         if (pModel != null)
         {
             var InvList = new List<Invoice_Header>();
             // Current Month Invoice
             var Inv = new Invoice_Header();
             Inv.Invoice_ID = pModel.Invoice_ID;
             Inv.Company_ID = pModel.Company_ID;
             Inv.Invoice_No = pModel.Invoice_No;
             Inv.Due_Amount = pModel.Due_Amount;
             Inv.Invoice_Month = pModel.Invoice_Month;
             Inv.Invoice_Year = pModel.Invoice_Year;
             Inv.Invoice_To = pModel.Invoice_To;
             Inv.Generated_On = pModel.Generated_On;
             Inv.Invoice_Status = PaymentStatus.Paid;
             Inv.Paid_By = userlogin.Email;
             InvList.Add(Inv);

             // Outstanding Invoice
             if (pModel.Outstanding_Invoices != null)
             {
                 foreach(var o in pModel.Outstanding_Invoices)
                 {
                     o.Invoice_Status = PaymentStatus.Paid;
                     o.Paid_By = userlogin.Email;
                     InvList.Add(o);
                 }                                 
             }
             var bSuccess = sService.Update_InvoiceStatus(InvList);
             if (bSuccess == true)
             {
                 Session["InvoiceViewModels"] = null;
                 Session["Subscription_PageAction"] = null;
                 Session["Paypal_Amount"] = null;
                 return View("PaymentComplete", new MessagePageViewModel { pageAction = pageAction });
             }
         }
         //Moet comment out here on 2/Sept
         //var comService = new CompanyService();
         //var model = (SignUpViewModel)Session["Subscription_Register"];
         //if (pageAction == "Company")
         //{         
         //    var result = comService.InsertSubscription(model.Subscription);
         //    var sService = new SubscriptionService();
         //    var mmap = sService.GetModuleMappingList(model.Subscription[0].Module_Detail_ID.Value);
         //    var bSuccess = SaveSubscriptionToBilling(model.Subscription, mmap);
         //    if (result.Code == ERROR_CODE.SUCCESS)
         //    {
         //        Session["Subscription_Register"] = null;
         //        Session["Subscription_PageAction"] = null;
         //        Session["Paypal_Amount"] = null;
         //        return View("PaymentComplete", new MessagePageViewModel { pageAction = pageAction });
         //    }
         //}         
         //else
         //{
         //   var currentdate = StoredProcedure.GetCurrentDate();
         //   var com = new Company();
         //   var detail = new Company_Details();

         //   detail.Name = model.Company_Name;
         //   detail.Effective_Date = currentdate;
         //   detail.Address = model.Address;
         //   detail.Country_ID = model.Country_ID;
         //   detail.State_ID = model.State_ID;
         //   detail.Zip_Code = model.Zip_Code;
         //   detail.Billing_Address = model.Address;
         //   detail.Billing_Country_ID = model.Country_ID;
         //   detail.Billing_State_ID = model.State_ID;
         //   detail.Billing_Zip_Code = model.Zip_Code;
         //   detail.Phone = model.Phone;
         //   detail.Email = model.Email;
         //   detail.Company_Status = RecordStatus.Active;
         //   detail.Create_On = currentdate;
         //   detail.Create_By = "Front";
         //   detail.Update_On = currentdate;
         //   detail.Update_By = "Front";
         //   //Added by sun 12-02-2016
         //   detail.Company_Level = Companylevel.EndUser;
         //   detail.Belong_To = "1";
         //   detail.A7_Group_ID = model.A7_Group_ID;
         //   if (model.Country_ID.HasValue && model.Country_ID != null)
         //   {
         //      var curreny = comService.GetCurrency(model.Country_ID.Value);
         //      if (curreny != null)
         //         detail.Currency_ID = curreny.Currency_ID;
         //   }

         //   var user = new User_Profile();
         //   user.Email = model.Email;
         //   user.First_Name = model.First_Name;
         //   user.Last_Name = model.Last_Name;
         //   user.Middle_Name = model.Middle_Name;
         //   user.Phone = model.Phone;
         //   user.Registration_Date = currentdate;
         //   user.User_Status = RecordStatus.Active;
         //   user.Create_On = currentdate;
         //   user.Create_By = "Front";
         //   user.Update_On = currentdate;
         //   user.Update_By = "Front";
         //   user.A7_User_ID = model.A7_User_ID;
         //   com.User_Profile.Add(user);

         //   if (model.Subscription != null)
         //   {
         //      foreach (var row in model.Subscription)
         //      {
         //         row.Start_Date = currentdate;
         //         com.Subscriptions.Add(row);
         //      }
         //   }
         //   com.Company_Details.Add(detail);
         //   //Company
         //   com.Create_On = currentdate;
         //   com.Create_By = "Front";
         //   com.Update_On = currentdate;
         //   com.Update_By = "Front";

         //   model.result = comService.InsertCompany(com);
         //   if (model.result.Code == ERROR_CODE.SUCCESS)
         //   {
         //      try
         //      {
         //         //SEND EMAIL
         //         //5.2	System	Send email to that email to the registered email for instruction of activation
         //         var domain = UrlUtil.GetDomain(Request.Url, ModuleDomain.Authentication);
         //         //Added by sun 11-02-2016
         //         var comCurr = comService.GetCompany(1);
         //         EmailTemplete.sendUserActivateEmail(model.Email, model.result.Object.ToString(), UserSession.GetUserName(user), detail.Name, comCurr.Phone, comCurr.Email, domain);
         //      }
         //      catch
         //      {
         //         model.result.Code = ERROR_CODE.ERROR_501_CANT_SEND_EMAIL;
         //      }

         //      Session["Subscription_Register"] = null;
         //      Session["Subscription_PageAction"] = null;
         //      Session["Paypal_Amount"] = null;
         //      return View("PaymentComplete", new MessagePageViewModel { });
         //   }
         //}

         return RedirectToAction("PaymentError", new MessagePageViewModel { pageAction = pageAction });
      }

      [HttpGet]
      public ActionResult PaymentError(string pageAction)
      {
         var model = new MessagePageViewModel();
         model.pageAction = pageAction;
         return View(model);
      }

      // GET: /Subscription/
      [HttpGet]
      public ActionResult Subscription(SubscriptionViewModel model)
      {

         return View(model);
      }




      //[HttpGet]
      //public ActionResult SubscriptionReport(ServiceResult result,SubscriptionMainViewModel model)
      //{

      //    var userlogin = UserSession.getUser(HttpContext);
      //    if (userlogin == null)
      //        return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

      //    //Validate Page Right
      //    RightResult rightResult = base.validatePageRight(UserSession.RIGHT_A);
      //    if (rightResult.action != null) return rightResult.action;
      //    model.rights = rightResult.rights;
      //    model.result = result;

      //    var currentdate = StoredProcedure.GetCurrentDate();
      //    var comService = new CompanyService();
      //    var cbService = new ComboService();
      //    var uService = new UserService();

      //    model.Companylist = cbService.LstCompany(true);

      //    model.CompanyLst = comService.LstCompany(model.Company_ID);
      //    var SubscriptionReport = new List<SubscriptionReportViewModel>();
      //    if (model.CompanyLst != null)
      //    {
      //        foreach (var row in model.CompanyLst)
      //        {
      //            if (row.Company_ID != 0)
      //            {
      //                var address = "";
      //                var com = comService.GetCompany(NumUtil.ParseInteger(row.Company_ID));
      //                if (com == null)
      //                    return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);
      //                if (com.Address != null)
      //                    address = com.Address;

      //                if (com.Zip_Code != null)
      //                    address += " " + com.Zip_Code;

      //                var user = uService.getUsers(pCompanyID: com.Company_ID).Where(w => w.User_Status == RecordStatus.Active && w.User_Authentication.Activated == true);
      //                var SubscriptionList = comService.LstSubscription(com.Company_ID);

      //                var detail = new List<SubscriptionDetailViewModel>();
      //                if (SubscriptionList != null)
      //                {
      //                    foreach (var lrow in SubscriptionList)
      //                    {
      //                        var unassignuser = 0;
      //                        if (lrow.User_Assign_Module != null)
      //                        {
      //                            unassignuser = lrow.No_Of_Users.Value - lrow.User_Assign_Module.Count();
      //                        }

      //                        var pmonth = 1;
      //                        if (lrow.Period_Month.HasValue)
      //                        {
      //                            pmonth = lrow.Period_Month.Value;
      //                        }
      //                        var expiredate = lrow.Start_Date.Value.AddMonths(pmonth);

      //                        detail.Add(new SubscriptionDetailViewModel()
      //                        {
      //                            Module_Name = lrow.SBS_Module_Detail.Module_Detail_Name,
      //                            Start_Date = lrow.Start_Date.Value,
      //                            Expiration_Date = expiredate,
      //                            Total_Licenses = (lrow.No_Of_Users.HasValue ? lrow.No_Of_Users.Value : 0),
      //                            UnAssigned_Licenses = unassignuser,
      //                            Assigned_Licenses = (lrow.User_Assign_Module.Count() > 0 ? lrow.User_Assign_Module.Count() : 0),
      //                        });
      //                    }
      //                }
      //                SubscriptionReport.Add(new SubscriptionReportViewModel()
      //                {
      //                    Company_ID = NumUtil.ParseInteger(row.Company_ID),
      //                    Company_Name = row.Name,
      //                    No_Of_Users = (user.Count() > 0 ? user.Count() : 0),
      //                    Address = address,
      //                    SubscriptionDetail_Rows = detail.ToArray(),
      //                });
      //            }
      //        }
      //        model.LstSubscription = SubscriptionReport;
      //    }
      //    return View(model);
      //}

      [HttpGet]
      public ActionResult SubscriptionReport(ServiceResult result, SubscriptionMainViewModel model)
      {

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //Validate Page Right
         RightResult rightResult = base.validatePageRight(UserSession.RIGHT_A);
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;
         model.result = result;

         var currentdate = StoredProcedure.GetCurrentDate();
         var comService = new CompanyService();
         var cbService = new ComboService();
         var uService = new UserService();

         model.Companylist = cbService.LstCompany(true);

         var cri = new CompanyCriteria();
         cri.Company_ID = model.Company_ID;
         model.CompanyLst = comService.LstCompany(cri);
         var SubscriptionReport = new List<SubscriptionReportViewModel>();
         if (model.CompanyLst != null)
         {
            var detail = new List<SubscriptionDetailViewModel>();
            foreach (var row in model.CompanyLst)
            {
               if (row.Company_ID != 0)
               {
                  var address = "";
                  var com = comService.GetCompany(NumUtil.ParseInteger(row.Company_ID));
                  if (com == null)
                     return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);
                  if (com.Address != null)
                     address = com.Address;

                  if (com.Zip_Code != null)
                     address += " " + com.Zip_Code;

                  var user = uService.getUsers(pCompanyID: com.Company_ID).Where(w => w.User_Status == RecordStatus.Active && w.User_Authentication.Activated == true);
                  var SubscriptionList = comService.LstSubscription(com.Company_ID);



                  if (SubscriptionList != null)
                  {
                     foreach (var lrow in SubscriptionList)
                     {
                        var unassignuser = 0;
                        if (lrow.User_Assign_Module != null)
                        {
                           unassignuser = lrow.No_Of_Users.Value - lrow.User_Assign_Module.Count();
                        }

                        var pmonth = 1;
                        if (lrow.Period_Month.HasValue)
                        {
                           pmonth = lrow.Period_Month.Value;
                        }
                        var expiredate = lrow.Start_Date.Value.AddMonths(pmonth);

                        detail.Add(new SubscriptionDetailViewModel()
                        {
                           Company_ID = NumUtil.ParseInteger(row.Company_ID),
                           Company_Name = row.Name,
                           No_Of_Users = (user.Count() > 0 ? user.Count() : 0),
                           Address = address,
                           Module_Name = lrow.SBS_Module_Detail.Module_Detail_Name,
                           Start_Date = lrow.Start_Date.Value,
                           Expiration_Date = expiredate,
                           Total_Licenses = (lrow.No_Of_Users.HasValue ? lrow.No_Of_Users.Value : 0),
                           UnAssigned_Licenses = unassignuser,
                           Assigned_Licenses = (lrow.User_Assign_Module.Count() > 0 ? lrow.User_Assign_Module.Count() : 0),
                        });
                     }
                  }
                  //SubscriptionReport.Add(new SubscriptionReportViewModel()
                  //{   
                  //    SubscriptionDetail_Rows = detail.ToArray(),
                  //});

               }
            }
            model.LstSubscription = detail;

         }
         return View(model);
      }

       [HttpGet]
      public ActionResult BillingReport(string token, string PayerID,ServiceResult result, BillingViewModels model, int Search_Month = 0, int Search_Year = 0, int[] Invoice_Ids = null, string tabAction = "")
      {                                                 
           var userlogin = UserSession.getUser(HttpContext);
           if(userlogin == null)
           {
               Response.Redirect("~/Account/Login?returnURL=" + Request.Url.AbsolutePath);
               return View(model);
           }
               

          var cSerevice = new ComboService();
          //var employeeService = new EmployeeService();

          //var EmpID = employeeService.GetEmployeeProfileByProfileID(userlogin.Profile_ID);
          var currentdate = StoredProcedure.GetCurrentDate();

          var rightResult = base.validatePageRight(UserSession.RIGHT_A, "/Subscription/BillingReport");
          if (rightResult.action != null) return rightResult.action;
          model.rights = rightResult.rights;
          model.result = result;

          if (Request.QueryString["ret"] != null)
          {
              var strReturn = Request.QueryString["ret"];
              if (strReturn == "Paypal")
              {
                  var a = Session["Paypal_Amount"];
                  var t = Session["token"];

                  if (System.Web.HttpContext.Current.Session["token"] == null || System.Web.HttpContext.Current.Session["token"].ToString() != token | System.Web.HttpContext.Current.Session["Paypal_Amount"] == null)
                      return RedirectToAction("PaymentError");

                  var pageAction = Session["Subscription_PageAction"].ToString();

                  string retMsg = "";
                  NVPAPICaller payPalCaller = new NVPAPICaller();
                  var comService = new CompanyService();
                  var HostCompany = comService.GetCompany(1);
                  if (AppSetting.IsLive == "true")
                  {// live account takes from db
                      payPalCaller.SetCredentials(HostCompany.APIUsername, HostCompany.APIPassword, HostCompany.APISignature, HostCompany.Is_Sandbox);
                  }
                  else
                  {//test account will use the hardcode credential in PaypalUtil.cs
                      payPalCaller.SetCredentials("", "", "", true);
                  }
                  NVPCodec decoder = new NVPCodec();
                  string payerId = "";
                  bool ret = payPalCaller.GetCheckoutDetails(token, ref payerId, ref decoder, ref retMsg);
                  if (ret)
                  {
                      var amount = NumUtil.ParseDecimal(Session["Paypal_Amount"].ToString());
                      ret = payPalCaller.DoCheckoutPayment(amount.ToString(), token, PayerID, ref decoder, ref retMsg);
                      if (ret)
                      {
                          //Moet added on 2/Sep
                          var pModel = (InvoiceViewModels)Session["InvoiceViewModels"];
                          var sService = new SubscriptionService();
                          if (pModel != null)
                          {
                              var InvList = new List<Invoice_Header>();
                              // Current Month Invoice
                              var Inv = new Invoice_Header();
                              Inv.Invoice_ID = pModel.Invoice_ID;
                              Inv.Company_ID = pModel.Company_ID;
                              Inv.Invoice_No = pModel.Invoice_No;
                              Inv.Due_Amount = pModel.Due_Amount;
                              Inv.Invoice_Month = pModel.Invoice_Month;
                              Inv.Invoice_Year = pModel.Invoice_Year;
                              //Inv.Invoice_To = pModel.Invoice_To;
                              Inv.Generated_On = pModel.Generated_On;
                              Inv.Invoice_Status = PaymentStatus.Paid;
                              Inv.Paid_By = userlogin.Email;
                              InvList.Add(Inv);

                              // Outstanding Invoice
                              if (pModel.Outstanding_Invoices != null)
                              {
                                  foreach (var o in pModel.Outstanding_Invoices)
                                  {
                                      o.Invoice_Status = PaymentStatus.Paid;
                                      o.Paid_By = userlogin.Email;
                                      InvList.Add(o);
                                  }
                              }
                              var bSuccess = sService.Update_InvoiceStatus(InvList);
                              if (bSuccess == true)
                              {
                                  Session["InvoiceViewModels"] = null;
                                  Session["Subscription_PageAction"] = null;
                                  Session["Paypal_Amount"] = null;
                                  //return View("PaymentComplete", new MessagePageViewModel { pageAction = pageAction });
                                  model.result.Code = 1; //Success
                              }
                          }

                      }
                      else
                      {
                          //return RedirectToAction("PaymentError", new MessagePageViewModel { pageAction = pageAction });
                          model.result.Code = 3; // Error
                      }
                  }
                  else
                  {
                      //return RedirectToAction("PaymentError", new MessagePageViewModel { pageAction = pageAction });
                      model.result.Code = 3; // Error
                  }
              }
              if (strReturn == "PaypalR")
              {
                  model.result.Code = 2; // Cancel
              }
          }          

          model.YearList = new List<int>();
          for (int i = 2014; i <= currentdate.Year; i++)
          {
              model.YearList.Add(i);
          }

          if (Search_Year == 0)
              model.Search_Year = currentdate.Year;
          else
              model.Search_Year = Search_Year;

          model.processDateList = cSerevice.LstMonth(true);                    
          model.Invoicelist = (new SubscriptionService()).GetInvoiceList(userlogin.Company_ID.Value, model.Search_Year.Value, model.Search_Month);     
          return View(model);

      }
       public byte[] imageToByteArray(System.Drawing.Image imageIn)
       {
           using (var ms = new MemoryStream())
           {
               imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
               return ms.ToArray();
           }
       }
       public void PrintInvoice(string pCompanyID = null, string pInvoiceID = null)
       {
           var iCompanyID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pCompanyID));
           var iInvoiceID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pInvoiceID));           
           
           var userlogin = UserSession.getUser(HttpContext);
           var comp = UserSession.getCompany(HttpContext);
           var sService = new SubscriptionService();
           var cService = new CompanyService();
           var HostCompany = cService.GetCompany(1);
           
           var model = new InvoiceViewModels();
           model = GenerateInvoice(iCompanyID, iInvoiceID, HostCompany);

           //2.Render the page to generate Pdf
           var htmlToConvert = new List<string>();
           htmlToConvert.Add(RenderPartialViewAsString("DailyUsageReport", model));

           //3. Generate pdf
           var workStream = new MemoryStream();
           //StreamReader reader = new StreamReader(workStream, System.Text.Encoding.UTF8,true);
           using (var archive = new ZipArchive(workStream, ZipArchiveMode.Create, true))
           {
               var fileName = model.Invoice_No + ".pdf";
               var file = archive.CreateEntry(fileName, System.IO.Compression.CompressionLevel.Optimal);
               using (Stream stream = file.Open())
               {
                   //------------------PDF---------------------------//
                   var pdfDoc = new Document(PageSize.A4);
                   var htmlparser = new HTMLWorker(pdfDoc);

                   Response.Clear();
                   Response.ClearContent();
                   Response.ClearHeaders();
                   Response.ContentType = "application/pdf";
                   Response.Charset = Encoding.UTF8.ToString();
                   Response.HeaderEncoding = Encoding.UTF8;
                   Response.ContentEncoding = Encoding.UTF8;
                   Response.Buffer = true;

                   var writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);

                   //--------------ZIP------------------------------//
                   var writerZip = PdfWriter.GetInstance(pdfDoc, stream);

                   //var pageevent = new PDFPageEvent();
                   var pageevent = new ITextEvents();
                   //pageevent.PrintTime = StoredProcedure.GetCurrentDate();
                   //var logo = cService.GetLogo(1);
                   //if (logo != null && logo.Logo != null)
                   //    pageevent.Logoleft = logo.Logo;

                   //WebClient wc = new WebClient();
                   //byte[] bytes = wc.DownloadData(AppSetting.SERVER_NAME + "/" + AppSetting.SBSTmpAPI + "/Images/logo-sbsolution.png");
                   //MemoryStream ms = new MemoryStream(bytes);
                   //System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                   //pageevent.Logoleft = bytes;

                   //var headerRight = HostCompany.Name; //+ "<br />" + HostCompany.Billing_Address;
                   //pageevent.HeaderRight = headerRight;                   

                   writer.PageEvent = pageevent;
                   writerZip.PageEvent = pageevent;

                   pdfDoc.Open();

                   var action = new PdfAction(PdfAction.PRINTDIALOG);
                   writer.SetOpenAction(action);
                   writerZip.SetOpenAction(action);
                   for (int i = 0; i < htmlToConvert.Count; i++)
                   {
                       if (i != 0)
                           pdfDoc.Add(Chunk.NEXTPAGE);

                       var sr1 = new StringReader(htmlToConvert[i]);
                       htmlparser.Parse(sr1);
                   }
                   pdfDoc.Close();
               }
           }                                 
       }

       public ActionResult Payment(string pCompanyID = null, string pInvoiceID = null)
       {
           var pageAction = "";
           var userlogin = UserSession.getUser(HttpContext);
           if (userlogin == null)
           {               
               return RedirectToAction("Login", "Account", new { returnUrl = Request.Url.PathAndQuery });               
           }
               

           System.Web.HttpContext.Current.Session["Subscription_PageAction"] = "";
           var iCompanyID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pCompanyID));
           var iInvoiceID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pInvoiceID));

           var cService = new CompanyService();
           var HostCompany = cService.GetCompany(1);
           
           var model = new InvoiceViewModels();
           model = GenerateInvoice(iCompanyID, iInvoiceID, HostCompany);
           
           NVPAPICaller payPalCaller = new NVPAPICaller();
           //Edit by sun 11-01-2015
           //payPalCaller.SetCredentials(bluecube.APIUsername, bluecube.APIPassword, bluecube.APISignature,false);
           if (AppSetting.IsLive == "true")
           {// live account takes from db
               payPalCaller.SetCredentials(HostCompany.APIUsername, HostCompany.APIPassword, HostCompany.APISignature, HostCompany.Is_Sandbox);
           }
           else
           {//test account will use the hardcode credential in PaypalUtil.cs
               payPalCaller.SetCredentials("", "", "", true);
           }
           
           string retMsg = "";
           string token = "";

           var strModuleDomain = ModuleDomain.Authentication;
           if(AppSetting.IsStaging == "true")
           {
               strModuleDomain = ModuleDomain.Authentication + "-staging";
           }
           string returnURL = UrlUtil.GetDomain(Request.Url, strModuleDomain) + "Subscription/BillingReport?ret=Paypal";
           string cancelURL = UrlUtil.GetDomain(Request.Url, strModuleDomain) + "Subscription/BillingReport?ret=PaypalC";

           var items = new List<Paypal>();
           var total = 0M;
           var itemName = "";

           if (model != null)
           {
               
               total = Convert.ToDecimal(model.Licenses * 0.05);
               if(model.Outstanding_Invoices != null)
               {
                   foreach(var o in model.Outstanding_Invoices)
                   {
                       DateTime dt = new DateTime(model.Invoice_Year.Value, o.Invoice_Month.Value, 1);
                       total += o.Due_Amount.Value;
                       itemName = Resource.Outstanding_Balance + " (" + dt.ToString("MMM") + ")";
                       items.Add(new Paypal() { Item_Name = itemName , Amount = o.Due_Amount.Value, Qty = 1 }); 
                   }
               }
               if (model.Storage_Upgrade_List != null)
               {
                   foreach (var o in model.Storage_Upgrade_List)
                   {
                       //DateTime dt = new DateTime(model.Invoice_Year.Value, o.Invoice_Month.Value, 1);
                       total += o.Price;
                       itemName = Resource.Upgrade_Storage + " (" + DateUtil.ToDisplayDDMMMYYYY(o.Upgrade_On.Value) + ")";
                       items.Add(new Paypal() { Item_Name = itemName, Amount = o.Price, Qty = 1 });
                   }
               }                     
               items.Add(new Paypal() { Item_Name = model.Product_Name, Amount = Convert.ToDecimal(0.05), Qty = model.Licenses });
           }
           bool ret = payPalCaller.ShortcutExpressCheckout(total, ref token, ref retMsg, items, returnURL, cancelURL);
           if (ret)
           {
               //Edit by sun 11-02-2016
               System.Web.HttpContext.Current.Session["token"] = token;
               System.Web.HttpContext.Current.Session["Paypal_Amount"] = total;
               System.Web.HttpContext.Current.Session["InvoiceViewModels"] = model;
               System.Web.HttpContext.Current.Response.Redirect(retMsg);
               return null;
           }
           else
           {
               // return error by paypal
              return RedirectToAction("PaymentError", new { pageAction = pageAction });
               
           }
       }

       private InvoiceViewModels GenerateInvoice(int pCompanyID, int pInvoiceID, Company_Details HostCompany)
       {
           var userlogin = UserSession.getUser(HttpContext);
           var comp = UserSession.getCompany(HttpContext);
           var sService = new SubscriptionService();
                
           var model = new InvoiceViewModels();
           var inv = sService.GetInvoice(pInvoiceID);

           if (inv != null)
           {
               // To get previous month outstanding amount
               var pYear = inv.Invoice_Year;
               var pMonth = inv.Invoice_Month;
               model.Outstanding_Invoices = sService.Get_Outstanding_Invoice(pCompanyID, pYear.Value, pMonth.Value, PaymentStatus.Outstanding);
               List<Storage_Upgrade> LstStorage = new List<Storage_Upgrade>();
               LstStorage = sService.GetStorageUpgradeList(pCompanyID, pYear.Value, pMonth.Value);
               if(model.Outstanding_Invoices.Count > 0)
               {
                   foreach(var r in model.Outstanding_Invoices)
                   {
                       var l = sService.GetStorageUpgradeList(pCompanyID, r.Invoice_Year.Value, r.Invoice_Month.Value);
                       if(l.Count >0)
                       {
                           foreach(var s in l)
                                LstStorage.Add(s);
                       }
                   }
               }
               model.Storage_Upgrade_List  = LstStorage;
               //1. Generate the model to display in pdf
               model.Company_Name = HostCompany.Name;
               model.CurrencyCode = HostCompany.Currency.Currency_Code;

               model.Invoice_Month = inv.Invoice_Month;
               model.Invoice_Year = inv.Invoice_Year;
               model.Invoice_To = userlogin.First_Name + " " + userlogin.Last_Name;
               if (inv.Invoice_Details.Count > 0)
                   model.Invoice_Details = inv.Invoice_Details.ToList();
               model.Invoice_ID = inv.Invoice_ID;
               model.Invoice_No = inv.Invoice_No;
               model.Company_ID = inv.Company_ID;
               model.Generated_On = inv.Generated_On;
               model.Generated_On = inv.Generated_On;
               var uService = new UserService();
               model.transList = uService.getUserTransactions(userlogin.Company_ID.Value);
               //1.1 Populate the transactions
               //DateTime now = DateTime.Now;
               int mm = inv.Invoice_Month.GetValueOrDefault();
               int yy = inv.Invoice_Year.GetValueOrDefault();
               var startDate = new DateTime(yy, mm, 1);
               var endDate = startDate.AddMonths(1).AddDays(-1);
               var days = DateTime.DaysInMonth(inv.Invoice_Year.Value, inv.Invoice_Month.Value);
               var mList = new List<Invoice_Transactions>();
               var totalLicenses = 0;
               decimal totalAmount = 0;
               for (var i = 1; i <= days; i++)
               {
                   var m = new Invoice_Transactions();
                   m.TranDate = new DateTime(yy, mm, i);

                   foreach (var t in model.transList)
                   {
                       if (t.Activate_On <= m.TranDate)
                       {
                           m.NoOfUsers += 1;
                           if (t.Deactivate_On != null)
                           {
                               m.NoOfUsers -= 1;
                           }
                       }
                       m.Amount = Convert.ToDecimal(m.NoOfUsers * 0.05);
                   }
                   mList.Add(m);
                   totalAmount += m.Amount;
                   totalLicenses += m.NoOfUsers;
               }
               model.InvTrans = GenerateTransactionToDisplay(mList);
               model.Due_Amount = totalAmount;
               model.Licenses = totalLicenses;
               model.Due_Date = endDate.AddDays(7);
               if (userlogin.Email != null)
                    model.Invoice_To_Address = userlogin.Email;
               else
                   model.Invoice_To_Address = comp.Email;
               model.Product_Name = sService.getBillingPromotionName(1);               
           }
           return model;
       }
       private List<Display_Transactions> GenerateTransactionToDisplay(List<Invoice_Transactions> mList)
       {
           var dispInvoiceList = new List<Display_Transactions>();
           if (mList.Count > 0)
           {
               DateTime sDate = DateTime.MinValue, eDate = DateTime.MinValue;
               int sLicense = 0, nLicense = 0;
               int dd = 0;
               int cnt = 0;
               foreach (var t in mList)
               {                   
                   cnt++;
                   if(t.NoOfUsers > 0)
                   {
                       dd++;
                       if (sDate == DateTime.MinValue)
                       {
                           if (dd > 1)
                               sDate = t.TranDate.AddDays(-1);
                           else
                               sDate = t.TranDate;
                           eDate = t.TranDate;
                           sLicense = t.NoOfUsers; nLicense = t.NoOfUsers;
                       }
                       else
                       {
                           nLicense = t.NoOfUsers;
                           if (sLicense != nLicense || cnt == mList.Count)
                           {
                               var dis = new Display_Transactions();
                               dis.StartDate = sDate;
                               if (cnt == mList.Count)
                               {
                                   dis.EndDate = t.TranDate;
                                   dis.Amount = Convert.ToDecimal((t.NoOfUsers * 0.05) * dd);
                               }                                   
                               else
                               {
                                   dis.EndDate = eDate;
                                   dd = dd - 1;
                                   dis.Amount = Convert.ToDecimal((sLicense * 0.05) * dd);
                               }

                               dis.NoOfDays = dd;
                               dis.NoOfUsers = sLicense;
                               
                               dispInvoiceList.Add(dis);
                               //Intialize again
                               sDate = DateTime.MinValue;
                               dd = 1;
                               sLicense = t.NoOfUsers;
                           }
                           eDate = t.TranDate;
                       }
                   }                   
               }
           }
           return dispInvoiceList;
       }
   }
}
