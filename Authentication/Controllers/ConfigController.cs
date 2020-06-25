using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Authentication.Models;
using Authentication.Common;
using SBSModel.Models;
using SBSModel.Common;

namespace Authentication.Controllers
{
    public class ConfigController : ControllerBase
    {

        [HttpGet]
        [AllowAuthorized]
        public ActionResult GlobalLookupList(ConfigViewModel model)
        {
            var userService = new UserService();
            if (model.IsCompanyConfig)
            {
                model.config_rights = userService.getPageRights(getUser().User_Authentication_ID.Value, "/Config/CompanyLookupList");
            }
            else
            {
                model.config_rights = userService.getPageRights(getUser().User_Authentication_ID.Value, "/Config/GlobalLookupList");
            }
            if (model.config_rights == null || !model.config_rights.Contains(UserSession.RIGHT_A))
            {
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);
            }

            var comboService = new ComboService();
            model.LookupTypeList = comboService.LstLookupType();
            if (!model.Def_ID.HasValue && model.LookupTypeList.Count > 0)
            {
                model.Def_ID = NumUtil.ParseInteger(model.LookupTypeList[0].Value);
            }

            if (model.Def_ID.HasValue)
            {
                ConfigService conService = new ConfigService();
                var globallookup = conService.GetGlobalLookup(model.Def_ID.Value);
                model.Def_ID = model.Def_ID.Value;
                model.Description = globallookup.Description;
                model.Name = globallookup.Name;
                model.IsCompanyConfig = model.IsCompanyConfig;
                if (model.IsCompanyConfig)
                {
                    User_Profile user = UserSession.getUser(HttpContext);
                    model.GlobalLookupDetailList = conService.LstGlobalLookupDetail(model.Def_ID.Value, model.IsCompanyConfig, user.Company_ID);
                    model.Company_ID = user.Company_ID;
                }
                else
                {
                    model.GlobalLookupDetailList = conService.LstGlobalLookupDetail(model.Def_ID.Value);
                }

            }
            return View(model);
        }
        [HttpGet]
        [AllowAuthorized]
        public ActionResult GlobalLookupDetail(Nullable<int> pLookupDataID, int pDefID, bool pIsCompanyConfig)
        {
            var model = new ConfigViewModel() { Lookup_Data_ID = pLookupDataID, Def_ID = pDefID, IsCompanyConfig = pIsCompanyConfig };
            var userService = new UserService();
            if (pIsCompanyConfig)
            {
                model.config_rights = userService.getPageRights(getUser().User_Authentication_ID.Value, "/Config/CompanyLookupList");
            }
            else
            {
                model.config_rights = userService.getPageRights(getUser().User_Authentication_ID.Value, "/Config/GlobalLookupList");
            }
            if (model.config_rights == null || !model.config_rights.Contains(UserSession.RIGHT_A))
            {
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);
            }

            if (pLookupDataID.HasValue)
            {
                ConfigService conService = new ConfigService();
                Global_Lookup_Data lookupdetail = conService.GetGlobalLookupDetail(pLookupDataID.Value);
                if (lookupdetail != null)
                {
                    model.Lookup_Data_ID = lookupdetail.Lookup_Data_ID;
                    model.Def_ID = lookupdetail.Def_ID;
                    model.Name = lookupdetail.Name;
                    model.Description = lookupdetail.Description;
                    model.Record_Status = lookupdetail.Record_Status;
                    model.Create_By = lookupdetail.Create_By;
                    if (lookupdetail.Create_On.HasValue)
                    {
                        model.Create_On = lookupdetail.Create_On.Value;
                    }
                    model.Update_By = lookupdetail.Update_By;
                    if (lookupdetail.Update_On.HasValue)
                    {
                        model.Update_On = lookupdetail.Update_On.Value;
                    }
                }
                if (pIsCompanyConfig)
                {
                    User_Profile user = UserSession.getUser(HttpContext);
                    model.Company_ID = user.Company_ID;
                }
            }
            return View(model);
        }

        [HttpPost]
        [AllowAuthorized]
        public ActionResult GlobalLookupDetail(ConfigViewModel model)
        {
            if (ModelState.IsValid)
            {
                User_Profile user = UserSession.getUser(HttpContext);
                ConfigService conService = new ConfigService();
                Global_Lookup_Data lookupdetail = new Global_Lookup_Data
                {
                    Name = model.Name,
                    Description = model.Description,
                    Record_Status = model.Record_Status,
                    Update_By = user.Name,
                    Def_ID = model.Def_ID.Value
                };
                if (model.IsCompanyConfig)
                {
                    lookupdetail.Company_ID = user.Company_ID;
                }
                if (model.Lookup_Data_ID.HasValue && model.Lookup_Data_ID > 0)
                {
                    if (conService.isDuplicatedName(model.Name, model.Def_ID.Value, model.Lookup_Data_ID.Value, pCompanyID: model.Company_ID))
                    {
                        ModelState.AddModelError("Name", "Duplication");
                    }
                    else
                    {
                        lookupdetail.Lookup_Data_ID = model.Lookup_Data_ID.Value;
                        conService.UpdateGlobalLookupDetail(lookupdetail);
                    }
                }
                else
                {
                    if (conService.isDuplicatedName(model.Name, model.Def_ID.Value, pCompanyID: lookupdetail.Company_ID))
                    {
                        ModelState.AddModelError("Name", "Duplication");
                    }
                    else
                    {
                        lookupdetail.Create_By = user.Name;
                        conService.InsertGlobalLookupDetail(lookupdetail);
                    }

                }
                return RedirectToAction("GlobalLookupList", new { Def_ID = model.Def_ID.Value, IsCompanyConfig = model.IsCompanyConfig });
            }
            return View("GlobalLookupDetail", model);
        }

        public ActionResult DeleteLookupDetail(int pLookupDataID, int pDefID, bool pIsCompanyConfig)
        {
            ConfigService conService = new ConfigService();
            conService.DeleteGlobalLookupDetail(pLookupDataID);

            return RedirectToAction("GlobalLookupList", new { Def_ID = pDefID, IsCompanyConfig = pIsCompanyConfig });
        }


    }
}