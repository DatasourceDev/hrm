using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Authentication.Models;
using Authentication.Common;
using SBSModel.Models;
using SBSModel.Common;
using SBSResourceAPI;
using System.Web.Routing;

namespace Authentication.Controllers
{
   [Authorize]
   [AllowAuthorized]
   public class GlobalLookupController : ControllerBase
   {

      #region Company Global Lookup
      [HttpGet]
      public ActionResult CompanyGlobalLookup(int[] LookupIDs, ServiceResult result, GlobalLookupViewModel model, string pDefID, string operation, string apply)
      {
         var currentdate = StoredProcedure.GetCurrentDate();
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //Validate Page Right
         RightResult rightResult = base.validatePageRight(UserSession.RIGHT_A);
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;
         model.result = result;

         var comService = new CompanyService();
         var glService = new GlobalLookupService();
         var cbService = new ComboService();

         var DefID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pDefID));
         if (DefID > 0)
            model.Def_ID = DefID;

         model.operation = EncryptUtil.Decrypt(operation);

         //Filter
         model.GLookupDeflst = cbService.LstLookupType(true);

         if (!model.Def_ID.HasValue && model.GLookupDeflst != null && model.GLookupDeflst.Count() > 0)
            model.Def_ID = NumUtil.ParseInteger(model.GLookupDeflst[0].Value);

         var dup = new List<Global_Lookup_Data>();
         var criteria = new LookUpCriteria()
         {
            Company_ID = userlogin.Company_ID,
            Def_ID = model.Def_ID
         };
         var pResult = glService.LstLookUp(criteria);
         if (pResult.Object != null) model.GlobalLookupDataList = (List<Global_Lookup_Data>)pResult.Object;

         var def = glService.GetLookupDef(model.Def_ID);
         if (def != null)
            model.Lookup_Def_Name = def.Description;

         if (LookupIDs != null)
         {
            //Check use 
            foreach (var LookupID in LookupIDs)
            {
               var lookup = glService.GetLookUp(LookupID);
               if (lookup != null)
               {
                  if (apply == RecordStatus.Active | apply == RecordStatus.Inactive)
                     lookup.Record_Status = apply;

                  else if (apply == UserSession.RIGHT_D)
                     lookup.Record_Status = RecordStatus.Delete;

                  lookup.Update_By = userlogin.User_Authentication.Email_Address;
                  lookup.Update_On = currentdate;
                  model.result = glService.UpdateLookUp(lookup);
               }
            }
            if (model.result.Code == ERROR_CODE.SUCCESS)
            {
               if (apply == RecordStatus.Active | apply == RecordStatus.Inactive)
                  return RedirectToAction("CompanyGlobalLookup", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, pDefID = EncryptUtil.Encrypt(model.Def_ID) });
               else if (apply == UserSession.RIGHT_D)
                  return RedirectToAction("CompanyGlobalLookup", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Lookup_Data, pDefID = EncryptUtil.Encrypt(model.Def_ID) });
            }
         }
         return View(model);
      }

      [HttpGet]
      public ActionResult CompanyGlobalLookupInfo(ServiceResult result, string pDefID, string pLookUpID, string operation)
      {

         var currentdate = StoredProcedure.GetCurrentDate();
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var model = new GlobalLookupViewModel();
         var gService = new GlobalLookupService();
         var cbService = new ComboService();
         model.operation = EncryptUtil.Decrypt(operation);

         //Validate Page Right
         RightResult rightResult = base.validatePageRight(model.operation, "/GlobalLookup/CompanyGlobalLookup");
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;
         model.result = result;


         model.Def_ID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pDefID));

         var def = gService.GetLookupDef(model.Def_ID);
         if (def == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

         model.Lookup_Data_ID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pLookUpID));

         model.Statuslst = cbService.LstRecordStatus();
         model.Lookup_Def_Name = def.Description;
         if (model.operation == UserSession.RIGHT_C)
         {

         }
         else if (model.operation == UserSession.RIGHT_U)
         {
            var data = gService.GetLookUp(model.Lookup_Data_ID);
            if (data == null)
               return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);
            model.Lookup_Data_ID = data.Lookup_Data_ID;
            model.Def_ID = data.Def_ID;
            model.Name = data.Name;
            model.Description = data.Description;
            model.Record_Status = data.Record_Status;

         }
         else if (model.operation == UserSession.RIGHT_D)
         {
            var data = gService.GetLookUp(model.Lookup_Data_ID);
            if (data == null)
               return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

            data.Record_Status = RecordStatus.Delete;
            data.Update_By = userlogin.User_Authentication.Email_Address;
            data.Update_On = currentdate;

            model.result = gService.UpdateLookUp(data);
            if (model.result.Code == ERROR_CODE.SUCCESS)
               return RedirectToAction("CompanyGlobalLookup", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Lookup_Data, pDefID = EncryptUtil.Encrypt(model.Def_ID) });

         }
         return View(model);
      }

      [HttpPost]
      [AllowAuthorized]
      public ActionResult CompanyGlobalLookupInfo(GlobalLookupViewModel model)
      {

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //Validate Page Right
         RightResult rightResult = base.validatePageRight(model.operation, "/GlobalLookup/CompanyGlobalLookup");
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;

         var currentdate = StoredProcedure.GetCurrentDate();
         var gService = new GlobalLookupService();
         var cbService = new ComboService();

         var def = gService.GetLookupDef(model.Def_ID);
         if (def == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

         var dup = new List<Global_Lookup_Data>();
         var criteria = new LookUpCriteria()
         {
            Company_ID = userlogin.Company_ID,
            Lookup_Name = model.Name,
            Def_ID = model.Def_ID
         };
         var pResult = gService.LstLookUp(criteria);
         if (pResult.Object != null) dup = (List<Global_Lookup_Data>)pResult.Object;

         var dupLstLookUp = dup.FirstOrDefault();
         if (dupLstLookUp != null)
         {
            if (model.operation == UserSession.RIGHT_C)
               ModelState.AddModelError("Name", Resource.Message_Is_Duplicated);
            else if (model.operation == UserSession.RIGHT_U)
            {
               if (dupLstLookUp.Lookup_Data_ID != model.Lookup_Data_ID)
                  ModelState.AddModelError("Name", Resource.Message_Is_Duplicated);
            }
         }
         if (ModelState.IsValid)
         {
            var lookup = new Global_Lookup_Data();
            if (model.operation == UserSession.RIGHT_U)
            {
               lookup = gService.GetLookUp(model.Lookup_Data_ID);
               if (lookup == null)
                  return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);
            }
            lookup.Def_ID = model.Def_ID.Value;
            lookup.Name = model.Name;
            lookup.Description = model.Description;
            lookup.Record_Status = model.Record_Status;
            lookup.Update_By = userlogin.User_Authentication.Email_Address;
            lookup.Update_On = currentdate;
          
            if (model.operation == UserSession.RIGHT_C)
            {
               lookup.Company_ID = userlogin.Company_ID;
               lookup.Create_By = userlogin.User_Authentication.Email_Address;
               lookup.Create_On = currentdate;
               model.result = gService.InsertLookUp(lookup);
            }
            else if (model.operation == UserSession.RIGHT_U)
            {
               model.result = gService.UpdateLookUp(lookup);
            }
            if (model.result.Code == ERROR_CODE.SUCCESS)
               return RedirectToAction("CompanyGlobalLookup", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, pDefID = EncryptUtil.Encrypt(model.Def_ID) });
         }

         model.Statuslst = cbService.LstRecordStatus();

         return View(model);
      }
      #endregion

      #region Mater Global Lookup
      [HttpGet]
      public ActionResult MasterGlobalLookup(int[] LookupIDs, ServiceResult result, GlobalLookupViewModel model, string pDefID, string operation, string apply)
      {
         var currentdate = StoredProcedure.GetCurrentDate();
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //Validate Page Right
         RightResult rightResult = base.validatePageRight(UserSession.RIGHT_A);
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;
         model.result = result;

         var comService = new CompanyService();
         var glService = new GlobalLookupService();
         var cbService = new ComboService();

         var DefID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pDefID));
         if (DefID > 0)
            model.Def_ID = DefID;

         model.operation = EncryptUtil.Decrypt(operation);

         //Filter
         model.GLookupDeflst = cbService.LstLookupType(true);

         if (!model.Def_ID.HasValue && model.GLookupDeflst != null && model.GLookupDeflst.Count() > 0)
            model.Def_ID = NumUtil.ParseInteger(model.GLookupDeflst[0].Value);

         var dup = new List<Global_Lookup_Data>();
         var criteria = new LookUpCriteria()
         {
            Def_ID = model.Def_ID,
            Mater = true
         };
         var pResult = glService.LstLookUp(criteria);
         if (pResult.Object != null) model.GlobalLookupDataList = (List<Global_Lookup_Data>)pResult.Object;

         var def = glService.GetLookupDef(model.Def_ID);
         if (def != null)
            model.Lookup_Def_Name = def.Description;

         if (LookupIDs != null)
         {
            //Check use 
            foreach (var LookupID in LookupIDs)
            {
               var lookup = glService.GetLookUp(LookupID);
               if (lookup != null)
               {
                  if (apply == RecordStatus.Active | apply == RecordStatus.Inactive)
                     lookup.Record_Status = apply;

                  else if (apply == UserSession.RIGHT_D)
                     lookup.Record_Status = RecordStatus.Delete;

                  lookup.Update_By = userlogin.User_Authentication.Email_Address;
                  lookup.Update_On = currentdate;
                  model.result = glService.UpdateLookUp(lookup);
               }
            }
            if (model.result.Code == ERROR_CODE.SUCCESS)
            {
               if (apply == RecordStatus.Active | apply == RecordStatus.Inactive)
                  return RedirectToAction("MasterGlobalLookup", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, pDefID = EncryptUtil.Encrypt(model.Def_ID) });
               else if (apply == UserSession.RIGHT_D)
                  return RedirectToAction("MasterGlobalLookup", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Lookup_Data, pDefID = EncryptUtil.Encrypt(model.Def_ID) });
            }
         }
         return View(model);
      }

      [HttpGet]
      public ActionResult MasterGlobalLookupInfo(ServiceResult result, string pDefID, string pLookUpID, string operation)
      {

         var currentdate = StoredProcedure.GetCurrentDate();
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var model = new GlobalLookupViewModel();
         var gService = new GlobalLookupService();
         var cbService = new ComboService();
         model.operation = EncryptUtil.Decrypt(operation);

         //Validate Page Right
         RightResult rightResult = base.validatePageRight(model.operation, "/GlobalLookup/MasterGlobalLookup");
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;
         model.result = result;


         model.Def_ID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pDefID));

         var def = gService.GetLookupDef(model.Def_ID);
         if (def == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

         model.Lookup_Data_ID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pLookUpID));

         model.Statuslst = cbService.LstRecordStatus();
         model.Lookup_Def_Name = def.Description;
         if (model.operation == UserSession.RIGHT_C)
         {

         }
         else if (model.operation == UserSession.RIGHT_U)
         {
            var data = gService.GetLookUp(model.Lookup_Data_ID);
            if (data == null)
               return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);
            model.Lookup_Data_ID = data.Lookup_Data_ID;
            model.Def_ID = data.Def_ID;
            model.Name = data.Name;
            model.Description = data.Description;
            model.Record_Status = data.Record_Status;

         }
         else if (model.operation == UserSession.RIGHT_D)
         {
            var data = gService.GetLookUp(model.Lookup_Data_ID);
            if (data == null)
               return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

            data.Record_Status = RecordStatus.Delete;
            data.Update_By = userlogin.User_Authentication.Email_Address;
            data.Update_On = currentdate;

            model.result = gService.UpdateLookUp(data);
            if (model.result.Code == ERROR_CODE.SUCCESS)
               return RedirectToAction("MasterGlobalLookup", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Lookup_Data, pDefID = EncryptUtil.Encrypt(model.Def_ID) });

         }
         return View(model);
      }

      [HttpPost]
      [AllowAuthorized]
      public ActionResult MasterGlobalLookupInfo(GlobalLookupViewModel model)
      {

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         //Validate Page Right
         RightResult rightResult = base.validatePageRight(model.operation, "/GlobalLookup/MasterGlobalLookup");
         if (rightResult.action != null) return rightResult.action;
         model.rights = rightResult.rights;

         var currentdate = StoredProcedure.GetCurrentDate();
         var gService = new GlobalLookupService();
         var cbService = new ComboService();

         var def = gService.GetLookupDef(model.Def_ID);
         if (def == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

         var dup = new List<Global_Lookup_Data>();
         var criteria = new LookUpCriteria()
         {
            Lookup_Name = model.Name,
            Def_ID = model.Def_ID,
            Mater = true
         };
         var pResult = gService.LstLookUp(criteria);
         if (pResult.Object != null) dup = (List<Global_Lookup_Data>)pResult.Object;

         var dupLstLookUp = dup.FirstOrDefault();
         if (dupLstLookUp != null)
         {
            if (model.operation == UserSession.RIGHT_C)
               ModelState.AddModelError("Name", Resource.Message_Is_Duplicated);
            else if (model.operation == UserSession.RIGHT_U)
            {
               if (dupLstLookUp.Lookup_Data_ID != model.Lookup_Data_ID)
                  ModelState.AddModelError("Name", Resource.Message_Is_Duplicated);
            }
         }
         if (ModelState.IsValid)
         {
            var lookup = new Global_Lookup_Data();
            if (model.operation == UserSession.RIGHT_U)
            {
               lookup = gService.GetLookUp(model.Lookup_Data_ID);
               if (lookup == null)
                  return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

            }
            lookup.Def_ID = model.Def_ID.Value;
            lookup.Name = model.Name;
            lookup.Description = model.Description;
            lookup.Record_Status = model.Record_Status;
            lookup.Update_By = userlogin.User_Authentication.Email_Address;
            lookup.Update_On = currentdate;
            lookup.Company_ID = null;

            if (model.operation == UserSession.RIGHT_C)
            {
               lookup.Create_By = userlogin.User_Authentication.Email_Address;
               lookup.Create_On = currentdate;
               model.result = gService.InsertLookUp(lookup);
            }
            else if (model.operation == UserSession.RIGHT_U)
            {
               model.result = gService.UpdateLookUp(lookup);
            }
            if (model.result.Code == ERROR_CODE.SUCCESS)
               return RedirectToAction("MasterGlobalLookup", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, pDefID = EncryptUtil.Encrypt(model.Def_ID) });
         }

         model.Statuslst = cbService.LstRecordStatus();

         return View(model);
      }
      #endregion

   }

}