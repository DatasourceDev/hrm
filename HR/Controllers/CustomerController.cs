using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HR.Models;
using HR.Common;
using SBSModel.Models;
using SBSModel.Common;
using SBSResourceAPI;
using SBSWorkFlowAPI.Constants;
using System.Web.Routing;

namespace HR.Controllers
{
   [Authorize]
   public class CustomerController : ControllerBase
   {

      [HttpGet]
      [AllowAuthorized]
      public ActionResult Customer(ServiceResult result, CustomerViewModels model, string apply, int[] Customer_IDs = null)
      {

         var currentdate = StoredProcedure.GetCurrentDate();
         User_Profile userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         ////Validate Page Right
         RightResult rightResult = base.validatePageRight(UserSession.RIGHT_A);
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;
         model.result = result;

         var custService = new Customer2Service();
         var cbService = new ComboService();
         var jobcostService = new JobCostService();

         //Filter 
         model.Countrylst = cbService.LstCountry(true);
         model.Statelst = new List<ComboViewModel>();
         if (model.Billing_Country_ID.HasValue)
            model.Statelst = cbService.LstState(model.Billing_Country_ID.Value.ToString(), true);
         else
            model.Statelst.Add(new ComboViewModel{ Value = null, Text = "-", Desc = "-" });

         var criteria = new CustomerCriteria()
         {
            Company_ID = userlogin.Company_ID,
            Customer_ID = model.Customer_ID,
            Billing_Country_ID = model.Billing_Country_ID,
            Billing_State_ID = model.Billing_State_ID
         };
         model.CustomerList = custService.LstCustomer(criteria);

         //model.Customerlst = cbService.LstCustomer(userlogin.Company_ID);
         //model.Statuslst = cbService.LstRecordStatus(true);

         if (Customer_IDs != null)
         {
            //Check use in Job_Cost
            var chkRefHas = false;
            if (apply == UserSession.RIGHT_D)
            {
               foreach (var Customer_ID in Customer_IDs)
               {
                  var criteria2 = new JobCostCriteria()
                  {
                     Company_ID = userlogin.Company_ID,
                     Customer_ID = Customer_ID
                  };
                  var ckUseData = jobcostService.LstJobCost(criteria2);
                  if (ckUseData.Count > 0)
                  {
                     chkRefHas = true;
                     break;
                  }
               }
            }
            if (chkRefHas)
            {
               if (apply == UserSession.RIGHT_D)
                  return RedirectToAction("Customer", new { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Customer });
            }
            else
            {
               foreach (var Customer_ID in Customer_IDs)
               {
                  var cust = custService.GetCustomer(Customer_ID);
                  if (cust != null)
                  {
                     if (apply == RecordStatus.Active | apply == RecordStatus.Inactive)
                        cust.Record_Status = apply;
                     else if (apply == UserSession.RIGHT_D)
                        cust.Record_Status = RecordStatus.Delete;

                     cust.Update_By = userlogin.User_Authentication.Email_Address;
                     cust.Update_On = currentdate;
                     model.result = custService.UpdateCustomer(cust);
                  }
               }
               if (model.result.Code == ERROR_CODE.SUCCESS)
               {
                  if (apply == RecordStatus.Active | apply == RecordStatus.Inactive)
                     return RedirectToAction("Customer", new RouteValueDictionary(model.result));
                  else if (apply == UserSession.RIGHT_D)
                     return RedirectToAction("Customer", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Customer });
               }
            }
         }

         return View(model);
      }

      [HttpGet]
      [AllowAuthorized]
      public ActionResult CustomerInfo(ServiceResult result, string pCustID, string operation)
      {
         var currentdate = StoredProcedure.GetCurrentDate();
         User_Profile userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var model = new CustomerViewModels();
         model.Customer_ID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pCustID));
         model.operation = EncryptUtil.Decrypt(operation);

         ////Validate Page Right
         RightResult rightResult = base.validatePageRight(model.operation, "/Customer/Customer");
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;
         model.result = result;

         var jobcostService = new JobCostService();
         var custService = new Customer2Service();
         var cbService = new ComboService();

         if (model.operation == UserSession.RIGHT_C)
         {
            //Thailand
            model.Billing_Country_ID = 36;
         }
         else if (model.operation == UserSession.RIGHT_U)
         {
            var cust = custService.GetCustomer(model.Customer_ID);
            if (cust == null)
               return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

            model.Customer_No = cust.Customer_No;
            model.Customer_Name = cust.Customer_Name;
            model.Person_In_Charge = cust.Person_In_Charge;
            model.Billing_Address = cust.Billing_Address;
            model.Billing_Postal_Code = cust.Billing_Postal_Code;
            model.Billing_Country_ID = cust.Billing_Country_ID;
            model.Billing_State_ID = cust.Billing_State_ID;
            model.Office_Phone = cust.Office_Phone;
            model.Mobile_Phone = cust.Mobile_Phone;
            model.Email = cust.Email;
            model.Record_Status = cust.Record_Status;

         }
         else if (model.operation == UserSession.RIGHT_D)
         {
            var cust = custService.GetCustomer(model.Customer_ID);
            if (cust == null)
               return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

            //Check use in Job_Cost
            var criteria2 = new JobCostCriteria()
            {
               Company_ID = userlogin.Company_ID,
               Customer_ID = cust.Customer_ID
            };
            var chkRefHas = false;
            var ckUseData = jobcostService.LstJobCost(criteria2);
            if (ckUseData.Count > 0)
            {
               chkRefHas = true;
            }

            if (chkRefHas)
               return RedirectToAction("Customer", new { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Customer });
            else
            {
               cust.Record_Status = RecordStatus.Delete;
               cust.Update_By = userlogin.User_Authentication.Email_Address;
               cust.Update_On = currentdate;
               model.result = custService.UpdateCustomer(cust);
               if (model.result.Code == ERROR_CODE.SUCCESS)
                  return RedirectToAction("Customer", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Customer });
            }
         }

         model.Countrylst = cbService.LstCountry(true);
         model.Statelst = new List<ComboViewModel>();

         if (model.Billing_Country_ID.HasValue)
            model.Statelst = cbService.LstState(model.Billing_Country_ID.Value.ToString(), true);
         else if (model.Countrylst.Count() > 0)
            model.Statelst = cbService.LstState(model.Countrylst[0].Value, true);

         model.Statuslst = cbService.LstRecordStatus();

         return View(model);

      }

      [HttpPost]
      [AllowAuthorized]
      public ActionResult CustomerInfo(CustomerViewModels model)
      {

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //Validate Page Right
         RightResult rightResult = base.validatePageRight(model.operation, "/Customer/Customer");
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;

         var currentdate = StoredProcedure.GetCurrentDate();
         var userService = new UserService();
         var custService = new Customer2Service();
         var cbService = new ComboService();

         var criteria = new CustomerCriteria() { Company_ID = userlogin.Company_ID, Customer_No_Dup = model.Customer_No, Record_Status = model.Record_Status };
         var dupCustomer = custService.LstCustomer(criteria).FirstOrDefault();
         if (dupCustomer != null)
         {
            if (model.operation == UserSession.RIGHT_C)
               ModelState.AddModelError("Customer_No", Resource.Message_Is_Duplicated);
            else if (model.operation == UserSession.RIGHT_U)
            {
               if (dupCustomer.Customer_ID != model.Customer_ID)
                  ModelState.AddModelError("Customer_No", Resource.Message_Is_Duplicated);
            }
         }

         if (ModelState.IsValid)
         {
            var cust = new Customer();
            if (model.operation == UserSession.RIGHT_U)
            {
               cust = custService.GetCustomer(model.Customer_ID);
               if (cust == null)
                  return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);
            }
            cust.Customer_No = model.Customer_No;
            cust.Customer_Name = model.Customer_Name;
            cust.Person_In_Charge = model.Person_In_Charge;
            cust.Billing_Address = model.Billing_Address;
            cust.Billing_Postal_Code = model.Billing_Postal_Code;
            cust.Billing_Country_ID = model.Billing_Country_ID;
            cust.Billing_State_ID = model.Billing_State_ID;
            cust.Office_Phone = model.Office_Phone;
            cust.Mobile_Phone = model.Mobile_Phone;
            cust.Email = model.Email;
            cust.Record_Status = model.Record_Status;
            cust.Update_By = userlogin.User_Authentication.Email_Address;
            cust.Update_On = currentdate;
            cust.Company_ID = userlogin.Company_ID;

            if (model.operation == UserSession.RIGHT_C)
            {
               cust.Create_By = userlogin.User_Authentication.Email_Address;
               cust.Create_On = currentdate;
               model.result = custService.InsertCustomer(cust);
            }
            else if (model.operation == UserSession.RIGHT_U)
            {
               model.result = custService.UpdateCustomer(cust);
            }
            if (model.result.Code == ERROR_CODE.SUCCESS)
               return RedirectToAction("Customer", new RouteValueDictionary(model.result));
         }

         model.Countrylst = cbService.LstCountry(true);

         model.Statelst = new List<ComboViewModel>();
         if (model.Billing_Country_ID.HasValue)
            model.Statelst = cbService.LstState(model.Billing_Country_ID.Value.ToString(), true);
         else if (model.Countrylst.Count() > 0)
            model.Statelst = cbService.LstState(model.Countrylst[0].Value, true);

         model.Statuslst = cbService.LstRecordStatus();

         return View(model);
      }
   }
}