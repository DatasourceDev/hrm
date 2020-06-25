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



namespace Authentication.Controllers
{
    //Added By sun
    [Authorize]
    [AllowAuthorized]
    public class ConfigurationController : ControllerBase
    {
        [HttpGet]
        public ActionResult SystemConfiguration(ServiceResult result, ConfigurationViewModel model, string tabAction = "")
        {
            User_Profile userlogin = UserSession.getUser(HttpContext);
            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            //Validate Page Right
            RightResult rightResult = base.validatePageRight(UserSession.RIGHT_A);
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;
            model.result = result;

            model.tabAction = tabAction;

            //-------data------------   
            var userService = new UserService();
            var comService = new CompanyService();
            var cbService = new ComboService();

            model.Company_ID = userlogin.Company_ID;
            model.UserRoleList = userService.LstUserRoleAll();
            model.ModuleDetailLst = userService.LstModuleDetail();

            //Added by sun 10-02-2016
            model.PageLst = userService.LstPageAll();

            return View(model);
        }

        [HttpGet]
        public ActionResult UserRoleInfo(ServiceResult result, string pRolID, string operation)
        {

            User_Profile userlogin = UserSession.getUser(HttpContext);
            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            //-------data------------
            var model = new UserRoleInfoViewModel();
            var userService = new UserService();
            model.operation = EncryptUtil.Decrypt(operation);
            model.User_Role_ID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pRolID));

            //Validate Page Right
            RightResult rightResult = base.validatePageRight(model.operation, "/Configuration/SystemConfiguration");
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;
            model.result = result;


            if (model.operation == UserSession.RIGHT_C)
            {

            }
            else if (model.operation == UserSession.RIGHT_U && model.User_Role_ID > 0)
            {
                User_Role role = userService.getUserRole(model.User_Role_ID);
                if (role != null)
                {
                    model.User_Role_ID = role.User_Role_ID;
                    model.Role_Name = role.Role_Name;
                    model.Role_Description = role.Role_Description;
                }
            }
            else if (model.operation == UserSession.RIGHT_D && model.User_Role_ID > 5)
            {
                model.result = userService.deleteUserRole(model.User_Role_ID);
                if (model.result.Code == ERROR_CODE.SUCCESS)
                {
                    return RedirectToAction("SystemConfiguration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "userrole" });
                }
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult UserRoleInfo(UserRoleInfoViewModel model, string pageAction = "")
        {

            User_Profile userlogin = UserSession.getUser(HttpContext);
            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            ////Validate Page Right
            RightResult rightResult = base.validatePageRight(model.operation, "Configuration/SystemConfiguration");
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;

            //-------data------------
            var empService = new EmployeeService();
            var setUserRole = new User_Role();
            var userService = new UserService();
            var comService = new CompanyService();
            var emp = new Employee_Profile();
            var cbService = new ComboService();
            var currentdate = StoredProcedure.GetCurrentDate();

            model.pageAction = pageAction;

            if (ModelState.IsValid)
            {
                if (pageAction == "saveAdd")
                {
                    model.pageAction = "main";
                    setUserRole.Role_Name = model.Role_Name;
                    setUserRole.Role_Description = model.Role_Description;
                    setUserRole.Create_By = userlogin.User_Authentication.Email_Address;
                    setUserRole.Create_On = currentdate;
                    setUserRole.Update_By = userlogin.User_Authentication.Email_Address;
                    setUserRole.Update_On = currentdate;

                    model.result = userService.InsertUserRole(setUserRole);

                    if (model.result.Code == ERROR_CODE.SUCCESS)
                    {
                        return RedirectToAction("SystemConfiguration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "userrole" });
                    }

                }
                else if (pageAction == "saveEdit")
                {
                    model.pageAction = "main";

                    var gRole = userService.getUserRole(model.User_Role_ID);

                    if (gRole == null)
                    {
                        return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);
                    }
                    setUserRole.User_Role_ID = model.User_Role_ID;
                    setUserRole.Role_Name = model.Role_Name;
                    setUserRole.Role_Description = model.Role_Description;
                    setUserRole.Update_By = userlogin.User_Authentication.Email_Address;
                    setUserRole.Update_On = currentdate;
                    model.result = userService.UpdateUserRole(setUserRole);

                    if (model.result.Code == ERROR_CODE.SUCCESS)
                    {
                        return RedirectToAction("SystemConfiguration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "userrole" });
                    }
                }
            }

            model.pageAction = "main";

            return View(model);
        }

        [HttpGet]
        public ActionResult PageRoleInfo(ServiceResult result, string pModDeltID, string operation)
        {
            User_Profile userlogin = UserSession.getUser(HttpContext);
            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            //-------data------------
            var model = new PageRoleViewModel();
            var userService = new UserService();
            var cbService = new ComboService();

            var ModDeltID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pModDeltID));
            //if (ModDeltID == 0)
            //return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

            model.operation = EncryptUtil.Decrypt(operation);
            model.Company_ID = userlogin.Company_ID;
            model.Module_Detail_ID = ModDeltID;

            ////Validate Page Right
            RightResult rightResult = base.validatePageRight(model.operation, "Configuration/SystemConfiguration");
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;
            model.result = result;

            if (model.operation == UserSession.RIGHT_C)
            {

            }
            else if (model.operation == UserSession.RIGHT_U)
            {
                model.PageUrlLst = cbService.LstPage(ModDeltID);
                model.AccessRightLst = cbService.LstAccessRights();
                model.UserRoleLst = cbService.LstUserRole();

                model.ModuleDetailLst = cbService.LstModuleDetail();

                List<Page_Role> proles = userService.getUserRolesPageRole(ModDeltID);
                if (proles != null)
                {
                    var pageRoles = new List<PageRoleDetailViewModel>();
                    foreach (var row in proles)
                    {
                        var prole = new PageRoleDetailViewModel()
                        {
                            Page_Role_ID = row.Page_Role_ID,
                            User_Role_ID = row.User_Role_ID,
                            Page_ID = row.Page_ID,
                            Row_Type = RowType.EDIT,
                            Access_Page_Rows = row.Access_Page.Select(s => s.Access_ID).ToArray()
                        };
                        pageRoles.Add(prole);
                    }
                    model.Page_Role_Rows = pageRoles.ToArray();
                }
            }
            return View(model);

        }

        [HttpPost]
        public ActionResult PageRoleInfo(PageRoleViewModel model, string pageAction = "")
        {

            User_Profile userlogin = UserSession.getUser(HttpContext);
            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            ////Validate Page Right
            RightResult rightResult = base.validatePageRight(model.operation, "Configuration/SystemConfiguration");
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;

            //-------data------------
            var userService = new UserService();
            var cbService = new ComboService();
            var currentdate = StoredProcedure.GetCurrentDate();
            List<Page_Role> pRolesAdd = new List<Page_Role>();
            List<Page_Role> pRolesEdit = new List<Page_Role>();

            if (model.Module_Detail_ID != null)
            {
                model.PageUrlLst = cbService.LstPage(model.Module_Detail_ID);
            }

            if (!ModelState.IsValid & model.Page_Role_Rows != null)
            {
                var i = 0;
                foreach (var row in model.Page_Role_Rows)
                {
                    if (row.Row_Type == RowType.DELETE)
                    {
                        DeleteModelStateError("Page_Role_Rows[" + i + "]");
                    }

                    i++;
                }
            }

            if (model.Page_Role_Rows != null)
            {
                foreach (var row in model.Page_Role_Rows)
                {
                    if (row.Row_Type == RowType.ADD | row.Row_Type == RowType.EDIT)
                    {
                        if (row.User_Role_ID == null)
                        {

                            ModelState.AddModelError("Page_Role_Rows", Resource.The + " " + Resource.User_Role + " " + Resource.Field + " " + Resource.Is_Rrequired_Lower);
                        }
                        else if (row.Page_ID == null)
                        {
                            ModelState.AddModelError("Page_Role_Rows", Resource.The + " " + Resource.Page_URL + " " + Resource.Field + " " + Resource.Is_Rrequired_Lower);
                        }
                    }
                }
            }

            if (ModelState.IsValid)
            {
                if (model.Page_Role_Rows != null)
                {
                    foreach (var row in model.Page_Role_Rows)
                    {
                        if (row.Row_Type == RowType.ADD | row.Row_Type == RowType.EDIT)
                        {
                            if (row.Page_Role_ID != null)
                            {
                                var pRoleEdit = new Page_Role()
                                {
                                    Page_Role_ID = (row.Page_Role_ID.HasValue ? row.Page_Role_ID.Value : 0),
                                    User_Role_ID = row.User_Role_ID.Value,
                                    Page_ID = row.Page_ID.Value,
                                    Update_By = userlogin.User_Authentication.Email_Address,
                                    Update_On = currentdate,
                                };

                                if (row.Access_Page_Rows != null)
                                {
                                    foreach (var prow in row.Access_Page_Rows)
                                    {
                                        var pAccessEdit = new Access_Page()
                                        {
                                            Page_Role_ID = (row.Page_Role_ID.HasValue ? row.Page_Role_ID.Value : 0),
                                            Access_ID = prow,
                                            Update_By = userlogin.User_Authentication.Email_Address,
                                            Update_On = currentdate,
                                        };
                                        pRoleEdit.Access_Page.Add(pAccessEdit);
                                    }
                                }
                                pRolesEdit.Add(pRoleEdit);

                            }
                            else
                            {
                                var pRoleAdd = new Page_Role()
                                {
                                    User_Role_ID = row.User_Role_ID.Value,
                                    Page_ID = row.Page_ID.Value,
                                    Create_By = userlogin.User_Authentication.Email_Address,
                                    Create_On = currentdate,
                                    Update_By = userlogin.User_Authentication.Email_Address,
                                    Update_On = currentdate,

                                };

                                if (row.Access_Page_Rows != null)
                                {
                                    foreach (var prow in row.Access_Page_Rows)
                                    {
                                        var pAccessAdd = new Access_Page()
                                        {
                                            Access_ID = prow,
                                            Create_By = userlogin.User_Authentication.Email_Address,
                                            Create_On = currentdate,
                                            Update_By = userlogin.User_Authentication.Email_Address,
                                            Update_On = currentdate,
                                        };
                                        pRoleAdd.Access_Page.Add(pAccessAdd);
                                    }
                                }
                                pRolesAdd.Add(pRoleAdd);
                            }
                        }
                    }
                    model.result = userService.InsertAndUpdatePageRole(pRolesEdit, pRolesAdd, model.Module_Detail_ID);
                    if (model.result.Code == ERROR_CODE.SUCCESS)
                    {
                        return RedirectToAction("SystemConfiguration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "pagerole" });
                    }
                }
            }

            model.ModuleDetailLst = cbService.LstModuleDetail();
            model.PageUrlLst = cbService.LstPage(model.Module_Detail_ID);
            model.UserRoleLst = cbService.LstUserRole();
            model.AccessRightLst = cbService.LstAccessRights();

            if (model.operation == UserSession.RIGHT_U)
            {
                var pageRoles = new List<PageRoleDetailViewModel>();
                if (model.Page_Role_Rows != null)
                {
                    foreach (var row in model.Page_Role_Rows)
                    {
                        var prole = new PageRoleDetailViewModel()
                        {
                            Page_Role_ID = row.Page_Role_ID,
                            User_Role_ID = row.User_Role_ID,
                            Page_ID = row.Page_ID,
                            Row_Type = row.Row_Type,
                            Access_Page_Rows = row.Access_Page_Rows
                        };
                        pageRoles.Add(prole);
                    }
                }
                model.Page_Role_Rows = pageRoles.ToArray();
            }

            return View(model);

        }

        public ActionResult AddNewRowInfo(int pIndex, int pPage_ID)
        {

            //-------data------------
            var cbService = new ComboService();
            var model = new PageRoleDetailViewModel() { Index = pIndex, Page_ID = pPage_ID };

            model.PageUrlLst = cbService.LstPage(model.Page_ID);
            model.UserRoleLst = cbService.LstUserRole();
            model.AccessRightLst = cbService.LstAccessRights();

            return PartialView("RoleInfoRow", model);
        }

    }
}