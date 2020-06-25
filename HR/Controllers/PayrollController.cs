using HR.Common;
using HR.Models;
using Ionic.Zip;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using Renci.SshNet;
using SBSModel.Common;
using SBSModel.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using SBSResourceAPI;
using System.Web.Routing;
using System.Text;
using Newtonsoft.Json.Linq;
using SBSWorkFlowAPI.Constants;


namespace HR.Controllers
{
    [Authorize]
    [AllowAuthorized]
    public class PayrollController : ControllerBase
    {
        #region Configuration
        [HttpGet]
        public ActionResult ConfigurationMaster(int[] donations, int[] cpf_formulas, ServiceResult result, PayrollConfigurationViewModel model, string tabAction, string apply)
        {
            var userlogin = UserSession.getUser(HttpContext);
            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            var rightResult = base.validatePageRight(UserSession.RIGHT_A);
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;
            model.result = result;
            model.tabAction = tabAction;

            var cSerevice = new ComboService();
            var pService = new PayrollService();

            if (tabAction == "donation")
            {
                if (apply == UserSession.RIGHT_D)
                {
                    if (donations != null)
                    {
                        var chkRefHas = false;
                        foreach (var danation in donations)
                        {
                            if (pService.ChkDonationFormulaUsed(danation))
                            {
                                chkRefHas = true;
                                break;
                            }
                        }
                        if (chkRefHas)
                            return RedirectToAction("ConfigurationMaster", new { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = model.result.Field, tabAction = "donation" });
                        else
                        {
                            apply = RecordStatus.Delete;
                            model.result = pService.UpdateMultipleDeleteDonationFormulaStatus(donations, apply, userlogin.User_Authentication.Email_Address);
                            if (model.result.Code == ERROR_CODE.SUCCESS)
                            {
                                return RedirectToAction("ConfigurationMaster", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "donation" });
                            }
                        }
                    }
                }
            }
            else if (tabAction == "cpf")
            {
                if (apply == UserSession.RIGHT_D)
                {
                    if (cpf_formulas != null)
                    {
                        var chkRefHas = false;
                        foreach (var cpf in cpf_formulas)
                        {
                            if (pService.ChkCPFFormulaUsed(cpf))
                            {
                                chkRefHas = true;
                                break;
                            }
                        }
                        if (chkRefHas)
                            return RedirectToAction("ConfigurationMaster", new { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = model.result.Field, tabAction = "cpf" });
                        else
                        {
                            apply = RecordStatus.Delete;
                            model.result = pService.UpdateMultipleDeleteCpfFormulaStatus(cpf_formulas, apply, userlogin.User_Authentication.Email_Address);
                            if (model.result.Code == ERROR_CODE.SUCCESS)
                            {
                                return RedirectToAction("ConfigurationMaster", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "cpf" });
                            }
                        }
                    }
                }
            }

            model.raceList = cSerevice.LstLookup(ComboType.Race, hasBlank: true);
            model.DonationFormulaList = pService.GetDonationFormulas(userlogin.Company_ID, model.search_Race);
            model.CpfFormulaList = pService.GetCPFFormulas(model.search_Cpf_Year);

            return View(model);
        }

        [HttpGet]
        public ActionResult Configuration(int[] allowances, int[] authorizations, ServiceResult result, PayrollConfigurationViewModel model, string tabAction, string apply)
        {
            var userlogin = UserSession.getUser(HttpContext);
            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            var rightResult = base.validatePageRight(UserSession.RIGHT_A);
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;
            model.result = result;

            model.tabAction = tabAction;
            var pService = new PayrollService();
            var cSerevice = new ComboService();


            if (tabAction == "allowance")
            {
                if (apply == UserSession.RIGHT_D)
                {
                    apply = RecordStatus.Delete;
                    if (model.Allowance_PRC_ID.HasValue)
                    {
                        model.result = pService.UpdateDeletePRCStatus(model.Allowance_PRC_ID, apply, userlogin.User_Authentication.Email_Address);
                    }
                    else if (allowances != null)
                    {
                        var chkRefHas = false;
                        foreach (var allowance in allowances)
                        {
                            if (pService.ChkEmpHistoryAllwUsed(allowance) || pService.ChkPRDsed(allowance))
                            {
                                chkRefHas = true;
                                break;
                            }
                        }
                        if (chkRefHas)
                            return RedirectToAction("Configuration", new { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = model.result.Field, tabAction = "allowance" });
                        else
                        {
                            //model.result = pService.MultipleDeletePRC(allowances);
                            model.result = pService.UpdateMultipleDeletePRCStatus(allowances, apply, userlogin.User_Authentication.Email_Address);
                        }
                    }
                    if (model.result.Code == ERROR_CODE.SUCCESS)
                    {
                        return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "allowance" });
                    }
                }
            }
            else if (tabAction == "authen")
            {
                if (apply == UserSession.RIGHT_D)
                {
                    apply = RecordStatus.Delete;
                    if (authorizations != null)
                    {
                        foreach (var prg_id in authorizations)
                        {
                            PRG PRG = pService.GetPRG(prg_id);
                            if (PRG == null)
                            {
                                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);
                            }
                            else if (PRG.Company_ID != userlogin.Company_ID.Value)
                            {
                                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);
                            }
                        }

                        model.result = pService.UpdateMultipleDeletePRGStatus(authorizations, apply, userlogin.User_Authentication.Email_Address);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                        {
                            return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "authen" });
                        }
                    }
                }
            }

            model.Donation_Selected_Formula_IDs = pService.GetCurrentSelectedDonationFormulas(userlogin.Company_ID);
            var cpfSelected = pService.GetCurrentSelectedCPFFormulas(userlogin.Company_ID);
            if (cpfSelected != null)
                model.CPF_Selected_Formula_ID = cpfSelected.CPF_Formula_ID;

            model.prtList = cSerevice.LstPRT(true);
            model.PRCList = pService.GetPRCs(userlogin.Company_ID.Value, model.search_Allowance_Type, model.search_Department);

            model.raceList = cSerevice.LstLookup(ComboType.Race, hasBlank: true);
            model.DonationFormulaList = pService.GetDonationFormulas(userlogin.Company_ID, model.search_Race);
            model.CpfFormulaList = pService.GetCPFFormulas(model.search_Cpf_Year);
            model.departmentList = cSerevice.LstDepartment(userlogin.Company_ID, true);
            model.PRGList = pService.GetPRGs(userlogin.Company_ID, model.search_Authen_Department);
            model.NotificationMonthList = cSerevice.LstMonth();

            var notification = pService.GetNotificationScheduler(userlogin.Company_ID);
            model.Company_ID = userlogin.Company_ID;
            if (notification == null)
            {
                model.operation = UserSession.RIGHT_C;
                model.Trigger_Set_Up = false;
                model.Trigger_Period = Term.Daily;
                model.Notice_Type = ModuleCode.Payroll;
                model.Recur_Every_Days = 1;
                model.Recur_Every_Weeks = 1;
            }
            else
            {
                model.operation = UserSession.RIGHT_U;
                model.Notification_Scheduler_ID = notification.Notification_Scheduler_ID;
                model.Notice_Type = notification.Notice_Type;
                model.Trigger_Set_Up = notification.Trigger_Set_Up.HasValue ? notification.Trigger_Set_Up.Value : false;
                model.Trigger_Period = notification.Trigger_Period;
                model.Start_Time = DateUtil.ToDisplayTime(notification.Start_Time);
                model.Start_Date = DateUtil.ToDisplayDate(notification.Start_Date);
                if (notification.Trigger_Period == Term.Daily)
                {
                    model.Recur_Every_Days = notification.Recur_Every_Days;
                    model.Recur_Every_Weeks = null;
                    model.Selected_Months = null;
                    //Week
                    model.Selected_Sunday = false;
                    model.Selected_Monday = false;
                    model.Selected_Tuesday = false;
                    model.Selected_Wednesday = false;
                    model.Selected_Thursday = false;
                    model.Selected_Friday = false;
                    model.Selected_Saturday = false;
                }
                else if (notification.Trigger_Period == Term.Weekly)
                {
                    model.Recur_Every_Weeks = notification.Recur_Every_Weeks;
                    model.Recur_Every_Days = null;
                    model.Selected_Months = null;
                    //Week
                    model.Selected_Sunday = notification.Selected_Sunday.HasValue ? notification.Selected_Sunday.Value : false;
                    model.Selected_Monday = notification.Selected_Monday.HasValue ? notification.Selected_Monday.Value : false;
                    model.Selected_Tuesday = notification.Selected_Tuesday.HasValue ? notification.Selected_Tuesday.Value : false;
                    model.Selected_Wednesday = notification.Selected_Wednesday.HasValue ? notification.Selected_Wednesday.Value : false;
                    model.Selected_Thursday = notification.Selected_Thursday.HasValue ? notification.Selected_Thursday.Value : false;
                    model.Selected_Friday = notification.Selected_Friday.HasValue ? notification.Selected_Friday.Value : false;
                    model.Selected_Saturday = notification.Selected_Saturday.HasValue ? notification.Selected_Saturday.Value : false;
                }
                else if (notification.Trigger_Period == Term.Monthly)
                {
                    model.Selected_Months = notification.Selected_Months;
                    model.Recur_Every_Weeks = null;
                    model.Recur_Every_Days = null;
                    //Week
                    model.Selected_Sunday = false;
                    model.Selected_Monday = false;
                    model.Selected_Tuesday = false;
                    model.Selected_Wednesday = false;
                    model.Selected_Thursday = false;
                    model.Selected_Friday = false;
                    model.Selected_Saturday = false;
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Configuration(PayrollConfigurationViewModel model, string tabAction = "")
        {

            var userlogin = UserSession.getUser(HttpContext);
            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            model.tabAction = tabAction;
            var currentdate = StoredProcedure.GetCurrentDate();
            var pService = new PayrollService();


            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.Allowance_Name))
                    ModelState.AddModelError("Allowance_Name", Resource.Message_Is_Required);
                if (!model.Allowance_PRT_ID.HasValue || model.Allowance_PRT_ID.Value == 0)
                    ModelState.AddModelError("Allowance_PRT_ID", Resource.Message_Is_Required);
            }

            if (ModelState.IsValid)
            {
                if (tabAction == "allowance")
                {
                    if (!model.Allowance_PRC_ID.HasValue || model.Allowance_PRC_ID.Value == 0)
                    {
                        var PRC = new PRC();
                        PRC.PRT_ID = model.Allowance_PRT_ID.HasValue ? model.Allowance_PRT_ID.Value : 0;
                        PRC.Record_Status = RecordStatus.Active;
                        PRC.Description = model.Allowance_Description;
                        PRC.Name = model.Allowance_Name;
                        PRC.CPF_Deductable = model.Allowance_CPF_Deductable;
                        PRC.Company_ID = userlogin.Company_ID.Value;
                        PRC.PRT = null;
                        PRC.Company = null;
                        PRC.OT_Multiplier = model.Allowance_OT_Multiplier;
                        PRC.PRC_Department = new List<PRC_Department>();
                        PRC.Create_By = userlogin.User_Authentication.Email_Address;
                        PRC.Create_On = currentdate;
                        PRC.Update_By = userlogin.User_Authentication.Email_Address;
                        PRC.Update_On = currentdate;

                        if (model.Allowance_Departments != null)
                        {
                            foreach (var row in model.Allowance_Departments)
                            {
                                PRC.PRC_Department.Add(new PRC_Department()
                                {
                                    Department_ID = row,
                                    Create_By = userlogin.User_Authentication.Email_Address,
                                    Create_On = currentdate,
                                    Update_By = userlogin.User_Authentication.Email_Address,
                                    Update_On = currentdate
                                });
                            }
                        }
                        model.result = pService.InsertPRC(PRC);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                            return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field });
                    }
                    else
                    {
                        var PRC = pService.GetPRC(model.Allowance_PRC_ID);
                        if (PRC == null)
                            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);
                        else if (PRC.Company_ID != userlogin.Company_ID.Value)
                            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

                        PRC.PRT_ID = model.Allowance_PRT_ID.HasValue ? model.Allowance_PRT_ID.Value : 0;
                        PRC.Record_Status = RecordStatus.Active;
                        PRC.Description = model.Allowance_Description;
                        PRC.Name = model.Allowance_Name;
                        PRC.CPF_Deductable = model.Allowance_CPF_Deductable;
                        PRC.Company_ID = userlogin.Company_ID.Value;
                        PRC.PRT = null;
                        PRC.Company = null;
                        PRC.OT_Multiplier = model.Allowance_OT_Multiplier;
                        PRC.PRC_Department = new List<PRC_Department>();
                        PRC.Update_By = userlogin.User_Authentication.Email_Address;
                        PRC.Update_On = currentdate;
                        if (model.Allowance_Departments != null)
                        {
                            foreach (var row in model.Allowance_Departments)
                            {
                                PRC.PRC_Department.Add(new PRC_Department()
                                {
                                    Department_ID = row,
                                    PRC_ID = PRC.PRC_ID,
                                    Create_By = userlogin.User_Authentication.Email_Address,
                                    Create_On = currentdate,
                                    Update_By = userlogin.User_Authentication.Email_Address,
                                    Update_On = currentdate

                                });
                            }
                        }

                        model.result = pService.UpdatePRC(PRC);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                            return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field });
                    }
                }
            }

            var rightResult = base.validatePageRight(model.operation);
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;

            var cSerevice = new ComboService();
            model.prtList = cSerevice.LstPRT(true);
            model.departmentList = cSerevice.LstDepartment(userlogin.Company_ID, true);
            model.raceList = cSerevice.LstLookup(ComboType.Race, hasBlank: true);
            model.PRCList = pService.GetPRCs(userlogin.Company_ID.Value, model.search_Allowance_Type, model.search_Department);
            model.DonationFormulaList = pService.GetDonationFormulas(userlogin.Company_ID, model.search_Race);
            model.CpfFormulaList = pService.GetCPFFormulas(model.search_Cpf_Year);
            model.PRGList = pService.GetPRGs(userlogin.Company_ID, model.search_Authen_Department);
            model.NotificationMonthList = cSerevice.LstMonth();

            var notification = pService.GetNotificationScheduler(userlogin.Company_ID);
            model.Company_ID = userlogin.Company_ID;
            if (notification == null)
            {
                model.operation = UserSession.RIGHT_C;
                model.Trigger_Set_Up = false;
                model.Trigger_Period = Term.Daily;
                model.Notice_Type = ModuleCode.Payroll;
                model.Recur_Every_Days = 1;
                model.Recur_Every_Weeks = 1;
            }
            else
            {
                model.operation = UserSession.RIGHT_U;
                model.Notification_Scheduler_ID = notification.Notification_Scheduler_ID;
                model.Notice_Type = notification.Notice_Type;
                model.Trigger_Set_Up = notification.Trigger_Set_Up.HasValue ? notification.Trigger_Set_Up.Value : false;
                model.Trigger_Period = notification.Trigger_Period;
                model.Start_Time = DateUtil.ToDisplayTime(notification.Start_Time);
                model.Start_Date = DateUtil.ToDisplayDate(notification.Start_Date);
                if (notification.Trigger_Period == Term.Daily)
                {
                    model.Recur_Every_Days = notification.Recur_Every_Days;
                    model.Recur_Every_Weeks = null;
                    model.Selected_Months = null;
                    //Week
                    model.Selected_Sunday = false;
                    model.Selected_Monday = false;
                    model.Selected_Tuesday = false;
                    model.Selected_Wednesday = false;
                    model.Selected_Thursday = false;
                    model.Selected_Friday = false;
                    model.Selected_Saturday = false;
                }
                else if (notification.Trigger_Period == Term.Weekly)
                {
                    model.Recur_Every_Weeks = notification.Recur_Every_Weeks;
                    model.Recur_Every_Days = null;
                    model.Selected_Months = null;
                    //Week
                    model.Selected_Sunday = notification.Selected_Sunday.HasValue ? notification.Selected_Sunday.Value : false;
                    model.Selected_Monday = notification.Selected_Monday.HasValue ? notification.Selected_Monday.Value : false;
                    model.Selected_Tuesday = notification.Selected_Tuesday.HasValue ? notification.Selected_Tuesday.Value : false;
                    model.Selected_Wednesday = notification.Selected_Wednesday.HasValue ? notification.Selected_Wednesday.Value : false;
                    model.Selected_Thursday = notification.Selected_Thursday.HasValue ? notification.Selected_Thursday.Value : false;
                    model.Selected_Friday = notification.Selected_Friday.HasValue ? notification.Selected_Friday.Value : false;
                    model.Selected_Saturday = notification.Selected_Saturday.HasValue ? notification.Selected_Saturday.Value : false;
                }
                else if (notification.Trigger_Period == Term.Monthly)
                {
                    model.Selected_Months = notification.Selected_Months;
                    model.Recur_Every_Weeks = null;
                    model.Recur_Every_Days = null;
                    //Week
                    model.Selected_Sunday = false;
                    model.Selected_Monday = false;
                    model.Selected_Tuesday = false;
                    model.Selected_Wednesday = false;
                    model.Selected_Thursday = false;
                    model.Selected_Friday = false;
                    model.Selected_Saturday = false;
                }
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult CPFFormula(string operation, string pFID)
        {
            var userlogin = UserSession.getUser(HttpContext);
            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            var model = new FormulaViewModel();
            model.operation = EncryptUtil.Decrypt(operation);
            var formulaID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pFID));

            var rightResult = base.validatePageRight(model.operation, "/Payroll/ConfigurationMaster");
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;

            var pService = new PayrollService();


            if (model.operation == UserSession.RIGHT_U)
            {
                var formula = pService.GetCPFFormula(formulaID);
                if (formula == null)
                    return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);
                model.Formula_ID = formula.CPF_Formula_ID;
                model.Formula = formula.Formula;
                model.Formula_Name = formula.Formula_Name;
                model.Formula_Desc = formula.Formula_Desc;
                model.Year = formula.Year;
            }
            else if (model.operation == UserSession.RIGHT_D)
            {
                var chkRefHas = false;
                if (pService.ChkCPFFormulaUsed(formulaID))
                    chkRefHas = true;

                if (chkRefHas)
                    return RedirectToAction("ConfigurationMaster", new { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), tabAction = "cpf" });
                else
                {
                    var apply = RecordStatus.Delete;
                    model.result = pService.UpdateDeleteCPFFormulaStatus(formulaID, apply, userlogin.User_Authentication.Email_Address);
                    if (model.result.Code == ERROR_CODE.SUCCESS)
                        return RedirectToAction("ConfigurationMaster", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "cpf" });
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CPFFormula(FormulaViewModel model)
        {
            var rightResult = base.validatePageRight(model.operation, "/Payroll/ConfigurationMaster");
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;

            var userlogin = UserSession.getUser(HttpContext);
            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            var pService = new PayrollService();


            if (model.Formula != null)
            {
                if (pService.ValidateFormula(model.Formula) >= 0)
                    ModelState.AddModelError("Formula", Resource.Error);
            }

            if (ModelState.IsValid)
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                if (model.operation == UserSession.RIGHT_C)
                {
                    var formula = new CPF_Formula();
                    formula.Formula = model.Formula;
                    formula.Formula_Name = model.Formula_Name;
                    formula.Formula_Desc = model.Formula_Desc;
                    formula.Year = model.Year;
                    formula.Create_By = userlogin.User_Authentication.Email_Address;
                    formula.Create_On = currentdate;
                    formula.Update_By = userlogin.User_Authentication.Email_Address;
                    formula.Update_On = currentdate;

                    model.result = pService.InsertCPFFormula(formula);
                    if (model.result.Code == ERROR_CODE.SUCCESS)
                        return RedirectToAction("ConfigurationMaster", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "cpf" });
                }
                else if (model.operation == UserSession.RIGHT_U)
                {
                    var formula = pService.GetCPFFormula(model.Formula_ID);
                    if (formula == null)
                        return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

                    formula.Formula = model.Formula;
                    formula.Formula_Name = model.Formula_Name;
                    formula.Formula_Desc = model.Formula_Desc;
                    formula.Year = model.Year;
                    formula.Update_By = userlogin.User_Authentication.Email_Address;
                    formula.Update_On = currentdate;

                    model.result = pService.UpdateCPFFormula(formula);
                    if (model.result.Code == ERROR_CODE.SUCCESS)
                        return RedirectToAction("ConfigurationMaster", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "cpf" });
                }
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult SelectedCPFFormula(string operation, string pFID)
        {
            var model = new SelectedCPFFormulaViewModel();
            model.operation = EncryptUtil.Decrypt(operation);

            var formulaID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pFID));
            var rightResult = base.validatePageRight(model.operation, "/Configuration/Configuration");
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;

            var userlogin = UserSession.getUser(HttpContext);
            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            var pService = new PayrollService();

            Selected_CPF_Formula selectformula = pService.GetSelectedCPFFormula(formulaID, userlogin.Company_ID);

            if (selectformula == null)
            {
                var formula = pService.GetCPFFormula(formulaID);
                if (formula == null)
                    return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

                model.CPF_Formula_ID = formula.CPF_Formula_ID;
                model.Formula = formula.Formula;
                model.Formula_Name = formula.Formula_Name;
                model.Formula_Desc = formula.Formula_Desc;
                model.Year = formula.Year;
            }
            else
            {
                if (selectformula.Company_ID != userlogin.Company_ID)
                    return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

                model.Selected_CPF_Formula_ID = selectformula.ID;
                model.CPF_Formula_ID = selectformula.CPF_Formula_ID;
                model.Formula = selectformula.CPF_Formula.Formula;
                model.Formula_Name = selectformula.CPF_Formula.Formula_Name;
                model.Formula_Desc = selectformula.CPF_Formula.Formula_Desc;
                model.Year = selectformula.CPF_Formula.Year;
                model.Effective_Date = DateUtil.ToDisplayDate(selectformula.Effective_Date);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SelectedCPFFormula(SelectedCPFFormulaViewModel model)
        {
            var userlogin = UserSession.getUser(HttpContext);
            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            if (ModelState.IsValid)
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                var pService = new PayrollService();

                if (model.Selected_CPF_Formula_ID.HasValue && model.Selected_CPF_Formula_ID.Value > 0)
                {
                    var formula = pService.GetSelectedCPFFormula(model.Selected_CPF_Formula_ID);
                    if (formula == null)
                        return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

                    if (formula.Company_ID != userlogin.Company_ID)
                        return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

                    formula.Effective_Date = DateUtil.ToDate(model.Effective_Date);
                    formula.Update_By = userlogin.User_Authentication.Email_Address;
                    formula.Update_On = currentdate;

                    model.result = pService.UpdateSelectedCPFFormula(formula);
                    if (model.result.Code == ERROR_CODE.SUCCESS)
                        return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "cpf" });
                }
                else if (model.operation == UserSession.RIGHT_U)
                {
                    Selected_CPF_Formula formula = new Selected_CPF_Formula();
                    formula.CPF_Formula_ID = model.CPF_Formula_ID.Value;
                    formula.Company_ID = userlogin.Company_ID.Value;
                    formula.Effective_Date = DateUtil.ToDate(model.Effective_Date);
                    formula.Create_By = userlogin.User_Authentication.Email_Address;
                    formula.Create_On = currentdate;
                    formula.Update_By = userlogin.User_Authentication.Email_Address;
                    formula.Update_On = currentdate;

                    model.result = pService.InsertSelectedCPFFormula(formula);
                    if (model.result.Code == ERROR_CODE.SUCCESS)
                        return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "cpf" });
                }
            }
            var rightResult = base.validatePageRight(model.operation, "/Configuration/Configuration");
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;
            return View(model);
        }

        [HttpGet]
        public ActionResult Authorization(string operation, string pPRGID)
        {
            PRGViewModel model = new PRGViewModel();
            model.operation = EncryptUtil.Decrypt(operation);
            model.PRG_ID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pPRGID));

            var rightResult = base.validatePageRight(model.operation, "/Configuration/Configuration");
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;

            var userlogin = UserSession.getUser(HttpContext);
            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);
            var pService = new PayrollService();
            var currentdate = StoredProcedure.GetCurrentDate();

            var cbService = new ComboService();
            var empService = new EmployeeService();

            model.departmentList = cbService.LstDepartment(userlogin.Company_ID);
            model.EmpList = new List<_Applicable_Employee>();
            //var emps = empService.LstEmployeeProfile(userlogin.Company_ID);
            var emps = empService.LstEmployeeProfileForPayrollAuth(userlogin.Company_ID);
            foreach (var emp in emps)
            {
                var hist = emp.Employment_History.Where(w => w.Effective_Date <= currentdate).OrderByDescending(o => o.Effective_Date).FirstOrDefault();
                if (hist != null)
                {
                    model.EmpList.Add(new _Applicable_Employee()
                    {
                        Department_ID = hist.Department_ID,
                        Email = emp.User_Profile.User_Authentication.Email_Address,
                        Name = UserSession.GetUserName(emp.User_Profile),
                        Profile_ID = emp.Profile_ID,
                        Employee_Profile_ID = emp.Employee_Profile_ID
                    });
                }
            }

            if (model.operation == UserSession.RIGHT_C)
            {
                var tmpDps = new List<ComboViewModel>();
                var usedDepartment = new List<int>();
                var dPREDL = pService.GetPRGs(userlogin.Company_ID);
                if (dPREDL != null)
                {
                    foreach (var d in dPREDL)
                    {
                        if (d.PREDLs != null)
                            usedDepartment.AddRange(d.PREDLs.Select(s => s.Department_ID));
                    }
                }
                foreach (var dp in model.departmentList)
                {
                    if (!usedDepartment.Contains(NumUtil.ParseInteger(dp.Value)))
                        tmpDps.Add(dp);
                }
                model.departmentList = tmpDps;

            }
            else if (model.operation == UserSession.RIGHT_U)
            {
                var PRG = pService.GetPRG(model.PRG_ID);
                if (PRG == null)
                    return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

                model.PRG_ID = PRG.PRG_ID;
                var prals = new List<PRALViewModel>();

                if (PRG.PRALs.Count > 0)
                {
                    foreach (var p in PRG.PRALs)
                    {
                        prals.Add(new PRALViewModel()
                        {
                            PRAL_ID = p.PRAL_ID,
                            Email = p.Employee_Profile.User_Profile.User_Authentication.Email_Address,
                            Employee_Profile_ID = p.Employee_Profile_ID,
                            Name = UserSession.GetUserName(p.Employee_Profile.User_Profile),
                            Profile_ID = p.Employee_Profile.Profile_ID,
                            Row_Type = RowType.EDIT
                        });
                    }
                }
                model.PRAL_Rows = prals.ToArray();

                if (PRG.PREDLs != null)
                    model.Departments = PRG.PREDLs.Select(s => s.Department_ID).ToArray();

                var tmpDps = new List<ComboViewModel>();
                var usedDepartment = new List<int>();
                var dPREDL = pService.GetPRGs(userlogin.Company_ID);
                if (dPREDL != null)
                {
                    foreach (var d in dPREDL)
                    {
                        if (d.PRG_ID != PRG.PRG_ID)
                            if (d.PREDLs != null)
                                usedDepartment.AddRange(d.PREDLs.Select(s => s.Department_ID));
                    }
                }
                foreach (var dp in model.departmentList)
                {
                    if (!usedDepartment.Contains(NumUtil.ParseInteger(dp.Value)))
                        tmpDps.Add(dp);
                }
                model.departmentList = tmpDps;

                var app = new List<int>();
                var notApp = new List<int>();

                if (PRG.PRELs != null)
                {
                    foreach (var emp in PRG.PRELs)
                    {
                        if (emp.Employee_Profile_ID != null)
                            app.Add(emp.Employee_Profile_ID);
                    }
                }

                if (model.EmpList != null)
                {
                    foreach (var emp in model.EmpList)
                    {
                        if (model.departmentList != null && PRG.PREDLs != null)
                        {
                            if (PRG.PREDLs.Select(s => s.Department_ID).Contains(emp.Department_ID.Value))
                            {
                                if (model.departmentList.Select(s => s.Value).Contains((emp.Department_ID.HasValue ? emp.Department_ID.Value : 0).ToString()))
                                {
                                    if (!app.Contains(emp.Employee_Profile_ID.Value))
                                        notApp.Add(emp.Employee_Profile_ID.Value);
                                }
                            }
                        }
                    }
                }

                model.Application_For = app.ToArray();
                model.Not_Application_For = notApp.ToArray();

                if (PRG.PREDLs.Count > 0)
                {
                    model.Departments = new int[PRG.PREDLs.Count];
                    int i = 0;
                    foreach (var p in PRG.PREDLs)
                    {
                        model.Departments[i++] = p.Department_ID;
                    }
                }
            }
            else if (model.operation == UserSession.RIGHT_D)
            {
                PRG PRG = pService.GetPRG(model.PRG_ID);
                if (PRG == null)
                    return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);
                else if (PRG.Company_ID != userlogin.Company_ID.Value)
                    return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);
                var apply = RecordStatus.Delete;
                model.result = pService.UpdateDeletePRGStatus(model.PRG_ID, apply, userlogin.User_Authentication.Email_Address);
                if (model.result.Code == ERROR_CODE.SUCCESS)
                    return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "authen" });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Authorization(PRGViewModel model)
        {

            var userlogin = UserSession.getUser(HttpContext);
            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            if (model.Departments == null || model.Departments.Count() == 0)
                ModelState.AddModelError("Departments", Resource.Message_Is_Required);

            if (model.PRAL_Rows == null || model.PRAL_Rows.Count() == 0 || model.PRAL_Rows.Where(w => w.Row_Type != RowType.DELETE).Count() == 0)
                ModelState.AddModelError("PRAL_Rows", Resource.Message_Is_Required);

            //------------------------PRAL Rows-------------------------//
            if (model.PRAL_Rows != null && model.PRAL_Rows.Count() != 0)
            {
                List<int> chk_Emp_ID = new List<int>();
                List<int> chk_Emp_ID_Er = new List<int>();

                var acnt = 0;
                foreach (var row in model.PRAL_Rows)
                {
                    if (row.Row_Type != RowType.DELETE)
                    {
                        if (row.Employee_Profile_ID.HasValue && !chk_Emp_ID.Contains(row.Employee_Profile_ID.Value))
                        {
                            chk_Emp_ID.Add(row.Employee_Profile_ID.Value);
                            continue;
                        }
                    }
                    else
                        DeleteModelStateError("PRAL_Rows[" + acnt + "]");

                    chk_Emp_ID_Er.Add(row.I);
                    acnt++;
                    continue;
                }

                if (chk_Emp_ID_Er.Count() != 0)
                {
                    foreach (var i in chk_Emp_ID_Er)
                    {
                        if (model.PRAL_Rows[i].Row_Type != RowType.DELETE)
                            ModelState.AddModelError("PRAL_Rows[" + i + "].Employee_Profile_ID", Resource.Message_Is_Duplicated);
                    }
                }
            }
            var pService = new PayrollService();
            var currentdate = StoredProcedure.GetCurrentDate();

            if (ModelState.IsValid)
            {
                var prg = new PRG();
                prg.Company_ID = userlogin.Company_ID.Value;

                if (model.Departments != null)
                {
                    foreach (var row in model.Departments)
                    {
                        var pD = new PREDL()
                        {
                            Department_ID = row,
                            Create_By = userlogin.User_Authentication.Email_Address,
                            Create_On = currentdate,
                            Update_By = userlogin.User_Authentication.Email_Address,
                            Update_On = currentdate,
                        };
                        prg.PREDLs.Add(pD);
                    }
                }
                if (model.Application_For != null)
                {
                    foreach (var row in model.Application_For)
                    {
                        var p = new PREL()
                        {
                            Employee_Profile_ID = row,
                            Create_By = userlogin.User_Authentication.Email_Address,
                            Create_On = currentdate,
                            Update_By = userlogin.User_Authentication.Email_Address,
                            Update_On = currentdate,
                        };
                        prg.PRELs.Add(p);
                    }
                }
                if (model.PRAL_Rows != null)
                {

                    foreach (var row in model.PRAL_Rows)
                    {
                        if (row.Row_Type == RowType.ADD || row.Row_Type == RowType.EDIT)
                        {
                            var pE = new PRAL()
                            {
                                Employee_Profile_ID = row.Employee_Profile_ID.Value,
                                Create_By = userlogin.User_Authentication.Email_Address,
                                Create_On = currentdate,
                                Update_By = userlogin.User_Authentication.Email_Address,
                                Update_On = currentdate,
                            };
                            prg.PRALs.Add(pE);
                        }
                    }
                }

                if (model.operation == UserSession.RIGHT_C)
                {
                    prg.Create_By = userlogin.User_Authentication.Email_Address;
                    prg.Create_On = currentdate;
                    prg.Update_By = userlogin.User_Authentication.Email_Address;
                    prg.Update_On = currentdate;

                    model.result = pService.InsertPRG(prg);
                    if (model.result.Code == ERROR_CODE.SUCCESS)
                        return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "authen" });
                }
                else if (model.operation == UserSession.RIGHT_U)
                {
                    var PRG = pService.GetPRG(model.PRG_ID);
                    if (PRG == null)
                        return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);
                    if (PRG.Company_ID != userlogin.Company_ID.Value)
                        return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

                    prg.Update_By = userlogin.User_Authentication.Email_Address;
                    prg.Update_On = currentdate;
                    prg.PRG_ID = model.PRG_ID.Value;

                    model.result = pService.UpdatePRG(prg);
                    if (model.result.Code == ERROR_CODE.SUCCESS)
                        return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "authen" });
                }
            }

            var rightResult = base.validatePageRight(model.operation, "/Configuration/Configuration");
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;

            var cbService = new ComboService();
            var empService = new EmployeeService();

            model.departmentList = cbService.LstDepartment(userlogin.Company_ID);
            model.EmpList = new List<_Applicable_Employee>();
            var emps = empService.LstEmployeeProfile(userlogin.Company_ID);
            foreach (var emp in emps)
            {
                var hist = emp.Employment_History.Where(w => w.Effective_Date <= currentdate).OrderByDescending(o => o.Effective_Date).FirstOrDefault();
                if (hist != null)
                {
                    model.EmpList.Add(new _Applicable_Employee()
                    {
                        Department_ID = hist.Department_ID,
                        Email = emp.User_Profile.User_Authentication.Email_Address,
                        Name = UserSession.GetUserName(emp.User_Profile),
                        Profile_ID = emp.Profile_ID,
                        Employee_Profile_ID = emp.Employee_Profile_ID
                    });
                }
            }

            var tmpDps = new List<ComboViewModel>();
            var usedDepartment = new List<int>();
            var dPREDL = pService.GetPRGs(userlogin.Company_ID);

            if (dPREDL != null)
            {
                if (model.PRG_ID != 0 && model.PRG_ID != null)
                {
                    PRG PRGRe = pService.GetPRG(model.PRG_ID);
                    if (PRGRe == null)
                        return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);
                    if (PRGRe.Company_ID != userlogin.Company_ID.Value)

                        return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);
                    foreach (var d in dPREDL)
                    {
                        if (d.PRG_ID != PRGRe.PRG_ID)
                        {
                            if (d.PREDLs != null)
                                usedDepartment.AddRange(d.PREDLs.Select(s => s.Department_ID));
                        }
                    }
                }
                else
                {
                    foreach (var d in dPREDL)
                    {
                        if (d.PREDLs != null)
                            usedDepartment.AddRange(d.PREDLs.Select(s => s.Department_ID));
                    }
                }
            }
            foreach (var dp in model.departmentList)
            {
                if (!usedDepartment.Contains(NumUtil.ParseInteger(dp.Value)))
                    tmpDps.Add(dp);
            }
            model.departmentList = tmpDps;

            return View(model);
        }

        public ActionResult AddNewPRAL(int pIndex)
        {
            var userlogin = UserSession.getUser(HttpContext);
            var model = new PRALViewModel() { Index = pIndex };

            var currentdate = StoredProcedure.GetCurrentDate();
            var empService = new EmployeeService();
            var emps = empService.LstEmployeeProfile(userlogin.Company_ID);
            model.EmpList = new List<_Applicable_Employee>();
            foreach (var emp in emps)
            {
                var hist = emp.Employment_History.Where(w => w.Effective_Date <= currentdate).OrderByDescending(o => o.Effective_Date).FirstOrDefault();
                if (hist != null)
                {
                    model.EmpList.Add(new _Applicable_Employee()
                    {
                        Department_ID = hist.Department_ID,
                        Email = emp.User_Profile.User_Authentication.Email_Address,
                        Name = UserSession.GetUserName(emp.User_Profile),
                        Profile_ID = emp.Profile_ID,
                        Employee_Profile_ID = emp.Employee_Profile_ID
                    });
                }
            }
            return PartialView("AuthorizationPRALRow", model);
        }

        [HttpGet]
        public ActionResult DonationFormula(string operation, string pFID)
        {
            var model = new DonationFormulaViewModel();
            model.operation = EncryptUtil.Decrypt(operation);


            var rightResult = base.validatePageRight(model.operation, "/Payroll/ConfigurationMaster");
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;

            var formulaID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pFID));

            var userlogin = UserSession.getUser(HttpContext);
            if (userlogin == null)
            {
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);
            }

            var cbService = new ComboService();
            model.donationTypelist = cbService.LstDonationType();
            model.racelist = cbService.LstLookup(ComboType.Race, userlogin.Company_ID);
            var pService = new PayrollService();

            if (model.operation == UserSession.RIGHT_U)
            {
                Donation_Formula formula = pService.GetDonationFormula(formulaID);
                if (formula == null)
                {
                    return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);
                }
                model.Formula_ID = formula.Donation_Formula_ID;
                model.Formula = formula.Formula;
                model.Formula_Name = formula.Formula_Name;
                model.Formula_Desc = formula.Formula_Desc;
                model.Year = formula.Year;
                model.Donation_Type_ID = formula.Donation_Type_ID;
                model.Race = formula.Race;
            }
            else if (model.operation == UserSession.RIGHT_D)
            {
                //Donation_Formula formula = pService.GetDonationFormula(formulaID);
                //if (formula == null)
                //{
                //    return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);
                //}
                ////TODO If it is not used by all, allow to delete
                //model.result = pService.DeleteDonationFormula(formula);
                var chkRefHas = false;
                if (pService.ChkDonationFormulaUsed(formulaID))
                {
                    chkRefHas = true;
                }

                if (chkRefHas)
                    return RedirectToAction("ConfigurationMaster", new { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), tabAction = "donation" });
                else
                {
                    var apply = RecordStatus.Delete;
                    model.result = pService.UpdateDeleteDonationFormulaStatus(formulaID, apply, userlogin.User_Authentication.Email_Address);
                    if (model.result.Code == ERROR_CODE.SUCCESS)
                    {
                        return RedirectToAction("ConfigurationMaster", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "donation" });
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DonationFormula(DonationFormulaViewModel model)
        {

            var rightResult = base.validatePageRight(model.operation, "/Payroll/ConfigurationMaster");
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;

            var userlogin = UserSession.getUser(HttpContext);
            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            var pService = new PayrollService();


            if (model.Formula != null)
            {
                //validate formula
                if (pService.ValidateFormula(model.Formula) >= 0)
                {
                    ModelState.AddModelError("Formula", Resource.Error);
                }
            }

            if (ModelState.IsValid)
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                if (model.operation == UserSession.RIGHT_C)
                {
                    Donation_Formula formula = new Donation_Formula();
                    formula.Formula = model.Formula;
                    formula.Formula_Name = model.Formula_Name;
                    formula.Create_By = userlogin.User_Authentication.Email_Address;
                    formula.Create_On = currentdate;
                    formula.Update_By = userlogin.User_Authentication.Email_Address;
                    formula.Update_On = currentdate;
                    formula.Donation_Type_ID = model.Donation_Type_ID;
                    formula.Race = model.Race;
                    formula.Formula_Desc = model.Formula_Desc;
                    formula.Year = model.Year;

                    model.result = pService.InsertDonationFormula(formula);
                    if (model.result.Code == ERROR_CODE.SUCCESS)
                    {
                        return RedirectToAction("ConfigurationMaster", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "donation" });
                    }
                }
                else if (model.operation == UserSession.RIGHT_U)
                {
                    Donation_Formula formula = pService.GetDonationFormula(model.Formula_ID);
                    if (formula == null)
                    {
                        return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);
                    }

                    formula.Formula = model.Formula;
                    formula.Formula_Name = model.Formula_Name;
                    formula.Update_By = userlogin.User_Authentication.Email_Address;
                    formula.Update_On = currentdate;
                    formula.Donation_Type_ID = model.Donation_Type_ID;
                    formula.Race = model.Race;
                    formula.Formula_Desc = model.Formula_Desc;
                    formula.Year = model.Year;

                    model.result = pService.UpdateDonationFormula(formula);
                    if (model.result.Code == ERROR_CODE.SUCCESS)
                    {
                        return RedirectToAction("ConfigurationMaster", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "donation" });
                    }
                }
            }

            var cbService = new ComboService();
            model.donationTypelist = cbService.LstDonationType();
            model.racelist = cbService.LstLookup(ComboType.Race, userlogin.Company_ID);
            return View(model);
        }

        [HttpGet]
        public ActionResult SelectedDonationFormula(string operation, string pFID)
        {
            var model = new SelectedDonationFormulaViewModel();
            model.operation = EncryptUtil.Decrypt(operation);

            var formulaID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pFID));

            var rightResult = base.validatePageRight(model.operation, "/Configuration/Configuration");
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;

            var userlogin = UserSession.getUser(HttpContext);
            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            var cbService = new ComboService();
            model.donationTypelist = cbService.LstDonationType();
            var pService = new PayrollService();

            var selectformula = pService.GetSelectedDonationFormula(formulaID, userlogin.Company_ID);

            if (selectformula == null)
            {
                var formula = pService.GetDonationFormula(formulaID);
                if (formula == null)
                    return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

                model.Donation_Formula_ID = formula.Donation_Formula_ID;
                model.Formula = formula.Formula;
                model.Formula_Name = formula.Formula_Name;
                model.Formula_Desc = formula.Formula_Desc;
                model.Year = formula.Year;
                model.Donation_Type_ID = formula.Donation_Type_ID;


                var race = cbService.GetLookup(formula.Race);
                if (race != null)
                    model.Race = race.Description;
            }
            else
            {
                if (selectformula.Company_ID != userlogin.Company_ID)
                    return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

                model.Selected_Donation_Formula_ID = selectformula.ID;
                model.Donation_Formula_ID = selectformula.Donation_Formula_ID;
                model.Formula = selectformula.Donation_Formula.Formula;
                model.Formula_Name = selectformula.Donation_Formula.Formula_Name;
                model.Formula_Desc = selectformula.Donation_Formula.Formula_Desc;
                model.Year = selectformula.Donation_Formula.Year;

                model.Donation_Type_ID = selectformula.Donation_Formula.Donation_Type_ID;
                model.Effective_Date = DateUtil.ToDisplayDate(selectformula.Effective_Date);

                var race = cbService.GetLookup(selectformula.Donation_Formula.Race);
                if (race != null)
                    model.Race = race.Description;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SelectedDonationFormula(SelectedDonationFormulaViewModel model)
        {

            var userlogin = UserSession.getUser(HttpContext);
            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            if (ModelState.IsValid)
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                var pService = new PayrollService();

                if (model.Selected_Donation_Formula_ID.HasValue && model.Selected_Donation_Formula_ID.Value > 0)
                {
                    var formula = pService.GetSelectedDonationFormula(model.Selected_Donation_Formula_ID);
                    if (formula == null)
                        return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

                    else if (formula.Company_ID != userlogin.Company_ID)
                        return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

                    formula.Effective_Date = DateUtil.ToDate(model.Effective_Date);
                    formula.Update_By = userlogin.User_Authentication.Email_Address;
                    formula.Update_On = currentdate;

                    model.result = pService.UpdateSelectedDonationFormula(formula);
                    if (model.result.Code == ERROR_CODE.SUCCESS)
                    {
                        return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "donation" });
                    }
                }
                else
                {
                    var formula = new Selected_Donation_Formula();
                    formula.Donation_Formula_ID = model.Donation_Formula_ID.Value;
                    formula.Company_ID = userlogin.Company_ID.Value;
                    formula.Effective_Date = DateUtil.ToDate(model.Effective_Date);
                    formula.Create_By = userlogin.User_Authentication.Email_Address;
                    formula.Create_On = currentdate;
                    formula.Update_By = userlogin.User_Authentication.Email_Address;
                    formula.Update_On = currentdate;

                    model.result = pService.InsertSelectedDonationFormula(formula);
                    if (model.result.Code == ERROR_CODE.SUCCESS)
                    {
                        return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "donation" });
                    }
                }

            }
            var rightResult = base.validatePageRight(model.operation, "/Configuration/Configuration");
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;

            var cbService = new ComboService();
            model.donationTypelist = cbService.LstDonationType();
            return View(model);
        }

        [HttpPost]
        public ActionResult NotificationScheduler(PayrollConfigurationViewModel model, string tabAction = "")
        {
            var userlogin = UserSession.getUser(HttpContext);
            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            var rightResult = base.validatePageRight(model.operation, "/Payroll/Configuration");
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;

            var comService = new CompanyService();
            var cbService = new ComboService();
            var payrollService = new PayrollService();
            var currentdate = StoredProcedure.GetCurrentDate();

            if (model.Trigger_Set_Up)
            {
                if (model.Trigger_Period == Term.Daily)
                {
                    if (!model.Recur_Every_Days.HasValue || model.Recur_Every_Days.Value <= 0)
                        ModelState.AddModelError("Recur_Every_Days", Resource.Message_Is_Required);

                }
                else if (model.Trigger_Period == Term.Weekly)
                {
                    if (!model.Recur_Every_Weeks.HasValue || model.Recur_Every_Weeks.Value <= 0)
                        ModelState.AddModelError("Recur_Every_Weeks", Resource.Message_Is_Required);
                }
                if (string.IsNullOrEmpty(model.Start_Date))
                    ModelState.AddModelError("Start_Date", Resource.Message_Is_Required);
            }

            if (ModelState.IsValid)
            {
                var com = comService.GetCompany(model.Company_ID);
                if (com == null)
                    return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

                Notification_Scheduler notification = new Notification_Scheduler();
                notification.Company_ID = model.Company_ID;
                notification.Notice_Type = ModuleCode.Payroll;
                notification.Trigger_Set_Up = model.Trigger_Set_Up;
                notification.Start_Date = DateUtil.ToDate(model.Start_Date);
                notification.Start_Time = DateUtil.ToTime(model.Start_Time);
                notification.Trigger_Period = model.Trigger_Period;
                notification.Update_On = currentdate;
                notification.Update_By = userlogin.User_Authentication.Email_Address;

                if (model.Trigger_Period == Term.Daily)
                {
                    notification.Recur_Every_Days = model.Recur_Every_Days;
                    notification.Recur_Every_Weeks = null;
                    notification.Selected_Months = null;
                    //Week
                    notification.Selected_Sunday = false;
                    notification.Selected_Monday = false;
                    notification.Selected_Tuesday = false;
                    notification.Selected_Wednesday = false;
                    notification.Selected_Thursday = false;
                    notification.Selected_Friday = false;
                    notification.Selected_Saturday = false;
                }
                else if (model.Trigger_Period == Term.Weekly)
                {
                    notification.Recur_Every_Weeks = model.Recur_Every_Weeks;
                    notification.Recur_Every_Days = null;
                    notification.Selected_Months = null;
                    //Week
                    notification.Selected_Sunday = model.Selected_Sunday;
                    notification.Selected_Monday = model.Selected_Monday;
                    notification.Selected_Tuesday = model.Selected_Tuesday;
                    notification.Selected_Wednesday = model.Selected_Wednesday;
                    notification.Selected_Thursday = model.Selected_Thursday;
                    notification.Selected_Friday = model.Selected_Friday;
                    notification.Selected_Saturday = model.Selected_Saturday;
                }
                else if (model.Trigger_Period == Term.Monthly)
                {
                    notification.Selected_Months = model.Selected_Months;
                    notification.Recur_Every_Days = null;
                    notification.Recur_Every_Weeks = null;
                    //Week
                    notification.Selected_Sunday = false;
                    notification.Selected_Monday = false;
                    notification.Selected_Tuesday = false;
                    notification.Selected_Wednesday = false;
                    notification.Selected_Thursday = false;
                    notification.Selected_Friday = false;
                    notification.Selected_Saturday = false;
                }

                if (model.operation == UserSession.RIGHT_C)
                {
                    notification.Notification_Scheduler_ID = model.Notification_Scheduler_ID;
                    notification.Create_By = userlogin.User_Authentication.Email_Address;
                    notification.Create_On = currentdate;

                    model.result = payrollService.InsertNotificationScheduler(notification);
                    if (model.result.Code == ERROR_CODE.SUCCESS)
                    {
                        return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "notification" });
                    }
                }
                else if (model.operation == UserSession.RIGHT_U)
                {
                    var currnotif = payrollService.GetNotificationScheduler(userlogin.Company_ID);
                    if (currnotif == null)
                        return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

                    notification.Notification_Scheduler_ID = currnotif.Notification_Scheduler_ID;
                    notification.Create_By = currnotif.Create_By;
                    notification.Create_On = currnotif.Create_On;

                    model.result = payrollService.UpdateNotificationScheduler(notification);
                    if (model.result.Code == ERROR_CODE.SUCCESS)
                        return RedirectToAction("Configuration", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, tabAction = "notification" });
                }
            }

            model.Donation_Selected_Formula_IDs = payrollService.GetCurrentSelectedDonationFormulas(userlogin.Company_ID);
            var cpfSelected = payrollService.GetCurrentSelectedCPFFormulas(userlogin.Company_ID);
            if (cpfSelected != null)
                model.CPF_Selected_Formula_ID = cpfSelected.CPF_Formula_ID;

            model.prtList = cbService.LstPRT(true);
            model.PRCList = payrollService.GetPRCs(userlogin.Company_ID.Value, model.search_Allowance_Type, model.search_Department);

            model.raceList = cbService.LstLookup(ComboType.Race, hasBlank: true);
            model.DonationFormulaList = payrollService.GetDonationFormulas(userlogin.Company_ID, model.search_Race);

            model.CpfFormulaList = payrollService.GetCPFFormulas(model.search_Cpf_Year);

            model.departmentList = cbService.LstDepartment(userlogin.Company_ID, true);
            model.PRGList = payrollService.GetPRGs(userlogin.Company_ID, model.search_Authen_Department);

            model.NotificationMonthList = cbService.LstMonth();
            model.tabAction = "notification";

            return View("Configuration", model);
        }
        #endregion

        #region Payroll
        [HttpGet]
        public ActionResult Payroll(ServiceResult result, PayrollViewModels model, string process = "")
        {
            var rightResult = base.validatePageRight(UserSession.RIGHT_A, "/Payroll/Payroll");
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;
            model.result = result;

            var userLanguages = Request.UserLanguages;

            var userlogin = UserSession.getUser(HttpContext);
            var currentdate = StoredProcedure.GetCurrentDate();
            var cSerevice = new ComboService();
            var payrollService = new PayrollService();
            var companyService = new CompanyService();

            var company = companyService.GetCompany(userlogin.Company_ID);

            if (model.Process_Year == 0)
                model.Process_Year = currentdate.Year;
            if (model.Process_Month == 0)
                model.Process_Month = currentdate.Month;

            model.departmentList = cSerevice.LstDepartment(userlogin.Company_ID, true);
            model.processDateList = cSerevice.LstMonth();
            model.processStatusList = cSerevice.LstStatus(true);
            model.payrollList = payrollService.ListPayroll(userlogin.Company_ID, model.sDepartment, model.Process_Month, model.Process_Year, model.sProcess);

            if (company != null && company.Currency != null)
            {
                model.Company_Currency_Code = company.Currency.Currency_Code;
                model.Company_Currency_ID = company.Currency_ID.Value;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Payroll(PayrollViewModels model, string process = "")
        {
            if (!string.IsNullOrEmpty(process))
            {
                if (process == PayrollStatus.Comfirm)
                    return PayrollAutoComfirm(model);
                else if (process == PayrollStatus.Process)
                    return PayrollAutolProcess(model);

            }

            var userlogin = UserSession.getUser(HttpContext);
            var currentdate = StoredProcedure.GetCurrentDate();
            var cSerevice = new ComboService();
            var payrollService = new PayrollService();
            var companyService = new CompanyService();

            model.departmentList = cSerevice.LstDepartment(userlogin.Company_ID, true);
            model.processDateList = cSerevice.LstMonth();
            model.processStatusList = cSerevice.LstStatus(true);
            model.payrollList = payrollService.ListPayroll(userlogin.Company_ID, model.sDepartment, model.Process_Month, model.Process_Year, model.sProcess);

            var rightResult = base.validatePageRight(UserSession.RIGHT_A, "/Payroll/Payroll");
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;
            return View(model);
        }

        [HttpGet]
        public ActionResult PayrollInfo(ServiceResult result, string pStatus, string pPrmID, string pEmpID, string pMonth, string pYear)
        {
            var userlogin = UserSession.getUser(HttpContext);
            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            var cSerevice = new ComboService();
            var payrollService = new PayrollService();

            var employeeService = new EmployeeService();
            var empHistService = new EmploymentHistoryService();
            var companyService = new CompanyService();
            var lService = new LeaveService();
            var bankinfoservice = new BankInfoService();

            var statue = EncryptUtil.Decrypt(pStatus);
            var empID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pEmpID));
            var month = NumUtil.ParseInteger(EncryptUtil.Decrypt(pMonth));
            var year = NumUtil.ParseInteger(EncryptUtil.Decrypt(pYear));
            var prmID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pPrmID));

            //empID = 1384;
            //month = 3;
            //year = 2018;

            var model = new PayrollViewModels();
            var rightResult = base.validatePageRight(UserSession.RIGHT_A, "/Payroll/Payroll");
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;
            model.result = result;
            var company = companyService.GetCompany(userlogin.Company_ID);
            if (company == null)
                return errorPage(new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Field = Resource.Company });

            var emp = employeeService.GetEmployeeProfile2(empID);
            if (emp == null)
                return errorPage(new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Field = Resource.Employee });

            if (userlogin.Company_ID != emp.User_Profile.Company_ID)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            model.Name = AppConst.GetUserName(emp.User_Profile);
            model.Employee_No = emp.Employee_No;
            model.Employee_Profile_ID = emp.Employee_Profile_ID;
            model.Opt_Out = emp.Opt_Out;

            var effectivedate = DateUtil.ToDate(1, month, year);
            var empHist = empHistService.GetPayrollEmploymentHistory(empID, effectivedate.Value);
            if (empHist == null)
                return errorPage(new ServiceResult() { Code = ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Field = Resource.Employment_History });

            model.History_ID = empHist.History_ID;
            model.Raw_Basic_Salary = NumUtil.ParseDecimal(EncryptUtil.Decrypt(empHist.Basic_Salary));
            if (model.Raw_Basic_Salary == 0)
                model.Raw_Basic_Salary = NumUtil.ParseDecimal(EncryptUtil.Decrypt(EncryptUtil.Decrypt(empHist.Basic_Salary)));

            model.Basic_Salary_Unit = empHist.Basic_Salary_Unit;
            if (model.Basic_Salary_Unit == Term.Hourly)
            {
                model.No_Of_Hours = 0;
                model.Hourly_Rate = model.Raw_Basic_Salary;
            }
            else
                model.Basic_Salary = model.Raw_Basic_Salary;

            model.Department_ID = empHist.Department_ID;
            model.Department = empHist.Department.Name;

            if (company.Currency != null)
            {
                model.Company_Currency_Code = company.Currency.Currency_Code;
                model.Company_Currency_ID = company.Currency_ID.Value;
                model.Company_ID = company.Company_ID;
            }
            model.Process_Date_From = DateUtil.ToDisplayDate(DateUtil.ToDate(1, month, year));
            model.Process_Date_To = DateUtil.ToDisplayDate(DateUtil.ToDate(DateTime.DaysInMonth(year, month), month, year));
            model.sLeave_Period_From = DateUtil.ToDisplayDate(DateUtil.ToDate(1, month, year));
            model.sLeave_Period_To = DateUtil.ToDisplayDate(DateUtil.ToDate(DateTime.DaysInMonth(year, month), month, year));
            model.sExpenses_Period_From = DateUtil.ToDisplayDate(DateUtil.ToDate(1, month, year));
            model.sExpenses_Period_To = DateUtil.ToDisplayDate(DateUtil.ToDate(DateTime.DaysInMonth(year, month), month, year));
            model.Process_Month = month;
            model.Process_Year = year;

            model.holidays = payrollService.GetHoliday(userlogin.Company_ID.Value, month, year);
            model.Working_Day = lService.GetWorkingDayOfWeek(userlogin.Company_ID, emp.Profile_ID);

            model.Total_Work_Days_All = (decimal)DateUtil.WorkDays(model.Process_Year, model.Process_Month, model.Working_Day, model.holidays);
            if (DateUtil.ToDate(model.Process_Date_From).HasValue && DateUtil.ToDate(model.Process_Date_To).HasValue)
                model.Total_Work_Days = (decimal)DateUtil.WorkDays(DateUtil.ToDate(model.Process_Date_From), DateUtil.ToDate(model.Process_Date_To), model.Working_Day, model.holidays);
            else
                model.Total_Work_Days = model.Total_Work_Days_All;

            model.Basic_Salary_Per_Day = model.Basic_Salary / model.Total_Work_Days_All;

            var bankinfo = bankinfoservice.GetCurrentBankInfo(empID);
            if (bankinfo != null)
                model.Payment_Type = bankinfo.Payment_Type;

            var prm = payrollService.GetPayroll(prmID);
            if (prm != null)
            {
                model.Run_Date = DateUtil.ToDisplayDate(prm.Run_date);
                model.Cheque_No = prm.Cheque_No;
                model.Total_Work_Days = prm.Total_Work_Days;
                model.No_Of_Hours = prm.No_Of_Hours.HasValue ? prm.No_Of_Hours.Value : 0;
                model.Hourly_Rate = prm.Hourly_Rate.HasValue ? prm.Hourly_Rate.Value : 0;
                model.PRM_ID = prm.PRM_ID;
                model.Leave_Period_From = DateUtil.ToDisplayDate(prm.Leave_Period_From);
                model.Leave_Period_To = DateUtil.ToDisplayDate(prm.Leave_Period_to);
                model.Expenses_Period_From = DateUtil.ToDisplayDate(prm.Expenses_Period_From);
                model.Expenses_Period_To = DateUtil.ToDisplayDate(prm.Expenses_Period_to);
                model.expensesList = payrollService.GetExpenseApplications(prm.PRM_ID);
                model.leaveList = payrollService.GetLeaveApplicationDocument(prm.PRM_ID);
                model.Allowance_Rows = PayrollConvertPRD(payrollService.LstPRD(prm.PRM_ID, PayrollAllowanceType.Allowance_Deduction));
                model.Overtime_Rows = PayrollConvertPRD(payrollService.LstPRD(prm.PRM_ID, PayrollAllowanceType.Overtime));
                model.Extra_Donation_Rows = PayrollConvertPRD(payrollService.LstPRD(prm.PRM_ID, PayrollAllowanceType.Donation));
                model.Process_Month = prm.Process_Month.Value;
                model.Process_Year = prm.Process_Year.Value;
                model.Selected_CPF_Formula_ID = prm.Selected_CPF_Formula_ID;
                model.Selected_OT_Formula_ID = prm.Selected_OT_Formula_ID;
                model.Gross_Salary = prm.Gross_Wages.Value;
                model.Bonus_Issue = prm.Bonus_Issue.HasValue ? prm.Bonus_Issue.Value : false;
                model.Total_Bonus = prm.Total_Bonus.HasValue ? prm.Total_Bonus.Value : model.Total_Bonus;
                model.Bonus_Amount = model.Total_Bonus;
                model.Director_Fee_Issue = prm.Director_Fee_Issue.HasValue ? prm.Director_Fee_Issue.Value : false;
                model.Total_Director_Fee = prm.Total_Director_Fee.HasValue ? prm.Total_Director_Fee.Value : model.Total_Director_Fee;
                model.Director_Fee_Amount = model.Total_Director_Fee;
                model.Selected_Donation_Formula_ID = prm.Selected_Donation_Formula_ID;
                model.Payment_Type = prm.Payment_Type;
                model.Revision_No = prm.Revision_No;
                if (prm.Process_Date_From.HasValue)
                    model.Process_Date_From = DateUtil.ToDisplayDate(prm.Process_Date_From);
                if (prm.Process_Date_To.HasValue)
                    model.Process_Date_To = DateUtil.ToDisplayDate(prm.Process_Date_To);

                if (prm.Basic_Salary.HasValue)
                    model.Basic_Salary = prm.Basic_Salary.Value;
            }
            else
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                model.Run_Date = DateUtil.ToDisplayDate(currentdate);

                if (model.History_ID.HasValue)
                {
                    var salraryAllowance = payrollService.LstHistoryAllowance(model.History_ID);
                    var allowances = new List<PRDViewModel>();
                    foreach (var sa in salraryAllowance)
                    {
                        allowances.Add(new PRDViewModel()
                        {
                            Amount = sa.Amount,
                            Currency_ID = company.Currency_ID,
                            PRC_ID = sa.PRC_ID,
                            PRT_ID = sa.PRT_ID,
                            History_Allowance_ID = sa.Employment_History_Allowance_ID,
                            Type = sa.PRT.Name,
                            Row_Type = RowType.ADD,
                            CPF_Deduction = (sa.PRC != null && sa.PRC.CPF_Deductable.HasValue ? sa.PRC.CPF_Deductable.Value : false),
                        });
                    }
                    model.Allowance_Rows = allowances.ToArray();
                }
            }


            model.prtallowanceList = cSerevice.LstPRT(PayrollAllowanceType.Allowance_Deduction);
            model.prcDonationList = cSerevice.LstPRC(userlogin.Company_ID, PayrollAllowanceType.Donation);
            model.prcOvertimeList = cSerevice.LstPRC(userlogin.Company_ID, PayrollAllowanceType.Overtime);
            model.paymentTypeList = cSerevice.LstLookup(ComboType.Payment_Type, userlogin.Company_ID);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PayrollInfo(PayrollViewModels model, string retrieve, string export = "")
        {
            var userlogin = UserSession.getUser(HttpContext);
            var currentdate = StoredProcedure.GetCurrentDate();
            var payrollService = new PayrollService();

            if (model.Overtime_Rows != null)
            {
                var i = 0;
                foreach (var row in model.Overtime_Rows)
                {
                    if (row.Row_Type == RowType.DELETE)
                        DeleteModelStateError("Overtime_Rows[" + i + "]");
                    i++;
                }
            }
            if (model.Allowance_Rows != null)
            {
                var i = 0;
                foreach (var row in model.Allowance_Rows)
                {
                    if (row.Row_Type == RowType.DELETE)
                        DeleteModelStateError("Allowance_Rows[" + i + "]");
                    i++;
                }
            }
            if (model.Extra_Donation_Rows != null)
            {
                var i = 0;
                foreach (var row in model.Extra_Donation_Rows)
                {
                    if (row.Row_Type == RowType.DELETE)
                        DeleteModelStateError("Extra_Donation_Rows[" + i + "]");
                    i++;
                }
            }

            if (string.IsNullOrEmpty(retrieve))
            {
                var pfdate = DateUtil.ToDate(model.Process_Date_From);
                var ptdate = DateUtil.ToDate(model.Process_Date_To);
                if (pfdate == null)
                    ModelState.AddModelError("Process_Date_From", Resource.Message_Is_Invalid);
                if (ptdate == null)
                    ModelState.AddModelError("Process_Date_To", Resource.Message_Is_Required);
                if (pfdate != null && ptdate != null)
                {
                    if (ptdate < pfdate)
                    {
                        ModelState.AddModelError("Process_Date_From", Resource.The + " " + Resource.Process_Date_From + " " + Resource.Field + " " + Resource.Cannot_Be_More_Than_Lower + " " + Resource.The + " " + Resource.Process_Date_To);
                        ModelState.AddModelError("Process_Date_To", Resource.The + " " + Resource.Process_Date_From + " " + Resource.Field + " " + Resource.Cannot_Be_More_Than_Lower + " " + Resource.The + " " + Resource.Process_Date_To);
                    }

                    if (pfdate.Value.Month != model.Process_Month)
                        ModelState.AddModelError("Process_Date_From", Resource.Message_Is_Invalid);
                    if (pfdate.Value.Year != model.Process_Year)
                        ModelState.AddModelError("Process_Date_From", Resource.Message_Is_Invalid);

                    if (ptdate.Value.Month != model.Process_Month)
                        ModelState.AddModelError("Process_Date_To", Resource.Message_Is_Invalid);
                    if (ptdate.Value.Year != model.Process_Year)
                        ModelState.AddModelError("Process_Date_To", Resource.Message_Is_Invalid);

                    var duplstprm = payrollService.LstPayrollByEmpID(model.Employee_Profile_ID, model.Process_Month, model.Process_Year);

                    var test = duplstprm.ToList();
                    if (!model.PRM_ID.HasValue)
                    {
                        var fdate = duplstprm.Where(w => (w.Process_Date_From <= pfdate & w.Process_Date_To >= pfdate) | w.Process_Date_From == pfdate | w.Process_Date_To == pfdate);
                        var from = fdate.ToList();
                        if (fdate.Count() > 0)
                        {
                            ModelState.AddModelError("Process_Date_From", Resource.Message_Is_Duplicated_PayPeriod);
                        }

                        var tdate = duplstprm.Where(w => (w.Process_Date_From <= ptdate & w.Process_Date_To >= ptdate) | w.Process_Date_From == ptdate | w.Process_Date_To == ptdate);
                        var to = fdate.ToList();
                        if (tdate.Count() > 0)
                        {
                            ModelState.AddModelError("Process_Date_To", Resource.Message_Is_Duplicated_PayPeriod);
                        }

                        var longdate = duplstprm.Where(w => ((w.Process_Date_From >= pfdate & w.Process_Date_To <= ptdate) | (w.Process_Date_To <= ptdate & w.Process_Date_From >= ptdate)));
                        if (longdate.Count() > 0)
                        {
                            ModelState.AddModelError("Process_Date_From", Resource.Message_The_Date_Range_Is_Duplicate);
                            ModelState.AddModelError("Process_Date_To", Resource.Message_The_Date_Range_Is_Duplicate);
                        }
                    }
                    else
                    {
                        duplstprm = duplstprm.Where(w => w.PRM_ID != model.PRM_ID.Value).ToList();
                        test = duplstprm.ToList();
                        var fdate = duplstprm.Where(w => (w.Process_Date_From <= pfdate & w.Process_Date_To >= pfdate) | w.Process_Date_From == pfdate | w.Process_Date_To == pfdate);
                        var from = fdate.ToList();
                        if (fdate.Count() > 0)
                        {
                            ModelState.AddModelError("Process_Date_From", Resource.Message_Is_Duplicated_PayPeriod);
                        }

                        var tdate = duplstprm.Where(w => (w.Process_Date_From <= ptdate & w.Process_Date_To >= ptdate) | w.Process_Date_From == ptdate | w.Process_Date_To == ptdate);
                        var to = tdate.ToList();
                        if (tdate.Count() > 0)
                        {
                            ModelState.AddModelError("Process_Date_To", Resource.Message_Is_Duplicated_PayPeriod);
                        }

                        var longdate = duplstprm.Where(w => ((w.Process_Date_From >= pfdate & w.Process_Date_To <= ptdate) | (w.Process_Date_To <= ptdate & w.Process_Date_From >= ptdate)));
                        if (longdate.Count() > 0)
                        {
                            ModelState.AddModelError("Process_Date_From", Resource.Message_The_Date_Range_Is_Duplicate);
                            ModelState.AddModelError("Process_Date_To", Resource.Message_The_Date_Range_Is_Duplicate);
                        }
                    }
                }
                var errors = GetErrorModelState();
                if (ModelState.IsValid)
                {
                    //double employeecon = 0;
                    //double employercon = 0;
                    //int cpf_Id = 0;

                    //double donation = 0;
                    //int donation_id = 0;
                    //string donation_type = "";
                    //PayrollProcessFormula2(model.Basic_Salary, model.Leave_Amount, model.Expenses_Amount, model.Allowance, model.Deductions, model.Deduction_Adhoc, model.Allowance_Adhoc, model.Adjustment_Addition, model.Adjustment_Deduction, model.Overtime, model.Commission, model.Total_Bonus, model.Employee_Profile_ID, ref employeecon, ref employercon, ref cpf_Id, model.Process_Month, model.Process_Year, ref donation, ref donation_id, ref donation_type);
                    //if (model.Opt_Out.HasValue && model.Opt_Out.Value)
                    //{
                    //    // clear donation CDAC EFC MBMF SINDA
                    //    model.Donation = 0M;
                    //    model.Selected_Donation_Formula_ID = null;
                    //    donation = 0;
                    //}

                    //var nett = (model.Basic_Salary + model.Commission + model.Overtime + model.Allowance + model.Adjustment_Addition + model.Total_Bonus + model.Expenses_Amount) - (model.Adjustment_Deduction + (model.Leave_Amount * -1) + (decimal)employeecon + model.Deductions + model.Deduction_Donation + (decimal)donation);
                    //var gross = (model.Basic_Salary + model.Commission + model.Overtime + model.Allowance + model.Adjustment_Addition + model.Total_Bonus) - (model.Adjustment_Deduction + (model.Leave_Amount * -1) + model.Deductions + model.Deduction_Donation + (decimal)donation);

                    var prm = new PRM();
                    if (model.PRM_ID.HasValue)
                        prm = payrollService.GetPayroll(model.PRM_ID);

                    prm.PRM_ID = (model.PRM_ID.HasValue ? model.PRM_ID.Value : 0);
                    prm.Payment_Type = model.Payment_Type;
                    prm.Cheque_No = model.Cheque_No;
                    prm.Run_date = currentdate;
                    prm.Employee_Profile_ID = model.Employee_Profile_ID;
                    prm.Total_Work_Days = model.Total_Work_Days;
                    prm.Total_Allowance = model.Total_Allowance;
                    prm.Total_Deduction = model.Total_Deduction;
                    prm.CPF_Employee = model.Employee_Contribution;
                    prm.CPF_Emplyer = model.Employer_Contribution;
                    prm.Selected_CPF_Formula_ID = model.Selected_CPF_Formula_ID;
                    prm.Selected_OT_Formula_ID = model.Selected_OT_Formula_ID;
                    prm.Nett_Wages = model.Net_Salary;
                    prm.Gross_Wages = model.Gross_Salary;
                    prm.Leave_Period_From = DateUtil.ToDate(model.Leave_Period_From);
                    prm.Leave_Period_to = DateUtil.ToDate(model.Leave_Period_To);
                    prm.Expenses_Period_From = DateUtil.ToDate(model.Expenses_Period_From);
                    prm.Expenses_Period_to = DateUtil.ToDate(model.Expenses_Period_To);
                    prm.Process_Month = NumUtil.ParseInteger(model.Process_Month);
                    prm.Process_Year = model.Process_Year;
                    prm.Process_Status = PayrollStatus.Process;
                    prm.Bonus_Issue = model.Bonus_Issue;
                    prm.Total_Bonus = model.Total_Bonus;
                    prm.Director_Fee_Issue = model.Director_Fee_Issue;
                    prm.Total_Director_Fee = model.Total_Director_Fee;
                    prm.Update_By = userlogin.User_Authentication.Email_Address;
                    prm.Update_On = currentdate;
                    prm.Basic_Salary = model.Basic_Salary;
                    prm.Total_Extra_Donation = model.Extra_Donation;
                    prm.Donation = model.Donation;
                    prm.Selected_Donation_Formula_ID = model.Selected_Donation_Formula_ID;
                    prm.No_Of_Hours = model.No_Of_Hours;
                    prm.Hourly_Rate = model.Hourly_Rate;
                    prm.Process_Date_From = DateUtil.ToDate(model.Process_Date_From);
                    prm.Process_Date_To = DateUtil.ToDate(model.Process_Date_To);
                    prm.Revision_No = model.Revision_No;

                    //Added by sun 09-02-2017
                    prm.Total_Allowance_Basic_Salary = model.Total_Allowance_Basic_Salary;

                    if (prm.Leave_Period_to == null)
                        prm.Leave_Period_to = prm.Leave_Period_From;

                    if (prm.Expenses_Period_to == null)
                        prm.Expenses_Period_to = prm.Expenses_Period_From;

                    if (model.Selected_CPF_Formula_ID == 0) prm.Selected_CPF_Formula_ID = null;
                    if (model.Selected_OT_Formula_ID == 0) prm.Selected_OT_Formula_ID = null;
                    if (model.Selected_Donation_Formula_ID == 0) prm.Selected_Donation_Formula_ID = null;

                    var _allowances = new List<_PRD>();
                    if (model.Allowance_Rows != null)
                    {
                        foreach (var row in model.Allowance_Rows)
                        {
                            var allw = new _PRD();
                            allw.Amount = row.Amount;
                            allw.Currency_ID = row.Currency_ID;
                            allw.Description = row.Description;
                            allw.History_Allowance_ID = row.History_Allowance_ID;
                            allw.Hours_Worked = row.Hours_Worked;
                            allw.Payroll_Detail_ID = row.Payroll_Detail_ID;
                            allw.PRC_ID = row.PRC_ID;
                            allw.PRM_ID = row.PRM_ID;
                            allw.PRT_ID = row.PRT_ID;
                            allw.Row_Type = row.Row_Type;
                            allw.Type = row.Type;
                            _allowances.Add(allw);
                        }
                    }
                    var _donations = new List<_PRD>();
                    if (model.Extra_Donation_Rows != null)
                    {
                        foreach (var row in model.Extra_Donation_Rows)
                        {
                            var donation = new _PRD();
                            donation.Amount = row.Amount;
                            donation.Currency_ID = row.Currency_ID;
                            donation.Description = row.Description;
                            donation.History_Allowance_ID = row.History_Allowance_ID;
                            donation.Hours_Worked = row.Hours_Worked;
                            donation.Payroll_Detail_ID = row.Payroll_Detail_ID;
                            donation.PRC_ID = row.PRC_ID;
                            donation.PRM_ID = row.PRM_ID;
                            donation.PRT_ID = row.PRT_ID;
                            donation.Row_Type = row.Row_Type;
                            donation.Type = row.Type;
                            _donations.Add(donation);
                        }
                    }
                    var _ots = new List<_PRD>();
                    if (model.Overtime_Rows != null)
                    {
                        foreach (var row in model.Overtime_Rows)
                        {
                            var ot = new _PRD();
                            ot.Amount = row.Amount;
                            ot.Currency_ID = row.Currency_ID;
                            ot.Description = row.Description;
                            ot.History_Allowance_ID = row.History_Allowance_ID;
                            ot.Hours_Worked = row.Hours_Worked;
                            ot.Payroll_Detail_ID = row.Payroll_Detail_ID;
                            ot.PRC_ID = row.PRC_ID;
                            ot.PRM_ID = row.PRM_ID;
                            ot.PRT_ID = row.PRT_ID;
                            ot.Row_Type = row.Row_Type;
                            ot.Type = row.Type;
                            _ots.Add(ot);
                        }
                    }
                    if (model.PRM_ID.HasValue)
                    {
                        model.result = payrollService.UpdatePayroll(prm, _allowances, _donations, _ots, model.Leave_Rows, model.Expenses_Rows, model.Basic_Salary);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                        {
                            if (string.IsNullOrEmpty(export))
                                return RedirectToAction("Payroll", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, Process_Month = model.Process_Month, Process_Year = model.Process_Year });
                            else
                            {
                                PayrollPrintSlip(EncryptUtil.Encrypt(model.PRM_ID));
                                ModelState.Clear();
                            }
                        }
                    }
                    else
                    {
                        prm.Create_By = userlogin.User_Authentication.Email_Address;
                        prm.Create_On = currentdate;
                        model.result = payrollService.InsertPayroll(prm, _allowances, _donations, _ots, model.Leave_Rows, model.Expenses_Rows, userlogin.Company_ID.Value, model.Basic_Salary);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                        {
                            if (string.IsNullOrEmpty(export))
                                return RedirectToAction("Payroll", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field, Process_Month = model.Process_Month, Process_Year = model.Process_Year });
                            else
                            {
                                PayrollPrintSlip(EncryptUtil.Encrypt(prm.PRM_ID));
                                ModelState.Clear();
                            }
                        }
                    }
                }
                else
                {
                    var err = GetErrorModelState();
                }
            }
            var lService = new LeaveService();
            var cService = new ComboService();
            if (retrieve == "leave")
            {
                if (DateUtil.ToDate(model.sLeave_Period_From) > DateUtil.ToDate(model.sLeave_Period_To))
                    ModelState.AddModelError("sLeave_Period_From", Resource.Message_Is_Invalid);
                else
                {
                    if (DateUtil.ToDate(model.sLeave_Period_From).HasValue)
                        model.leaveList = payrollService.GetLeaveApplicationDocument(model.Employee_Profile_ID, WorkflowStatus.Closed, DateUtil.ToDate(model.sLeave_Period_From), DateUtil.ToDate(model.sLeave_Period_To), model.PRM_ID);
                }

                if (model.Expenses_Rows != null)
                    model.expensesList = payrollService.GetExpenseApplications(model.Expenses_Rows);
                model.Leave_Period_From = model.sLeave_Period_From;
                model.Leave_Period_To = model.sLeave_Period_To;

                model.tabAction = "leave";
            }
            else if (retrieve == "expenses")
            {
                if (DateUtil.ToDate(model.sExpenses_Period_From) > DateUtil.ToDate(model.sExpenses_Period_To))
                    ModelState.AddModelError("Expenses_Period_From", Resource.Message_Is_Invalid);
                else
                {
                    if (DateUtil.ToDate(model.sExpenses_Period_From).HasValue)
                        model.expensesList = payrollService.GetPayrollExpense(model.Employee_Profile_ID, null, DateUtil.ToDate(model.sExpenses_Period_From), DateUtil.ToDate(model.sExpenses_Period_To), model.PRM_ID, true);
                }
                if (model.Leave_Rows != null)
                    model.leaveList = payrollService.GetLeaveApplicationDocument(model.Leave_Rows);
                model.Expenses_Period_From = model.sExpenses_Period_From;
                model.Expenses_Period_To = model.sExpenses_Period_To;

                model.tabAction = "expenses";
            }
            else
            {
                if (model.Expenses_Rows != null)
                    model.expensesList = payrollService.GetExpenseApplications(model.Expenses_Rows);
                if (model.Leave_Rows != null)
                    model.leaveList = payrollService.GetLeaveApplicationDocument(model.Leave_Rows);
            }
            model.prtallowanceList = cService.LstPRT(PayrollAllowanceType.Allowance_Deduction);
            model.prcOvertimeList = cService.LstPRC(userlogin.Company_ID, PayrollAllowanceType.Overtime);
            model.prcDonationList = cService.LstPRC(userlogin.Company_ID, PayrollAllowanceType.Donation);

            model.paymentTypeList = cService.LstLookup(ComboType.Payment_Type, userlogin.Company_ID);
            model.Run_Date = DateUtil.ToDisplayDate(currentdate);

            model.holidays = payrollService.GetHoliday(userlogin.Company_ID.Value, model.Process_Month, model.Process_Year);
            model.Working_Day = lService.GetWorkingDayOfWeek(userlogin.Company_ID, null, model.Employee_Profile_ID);
            var rightResult = base.validatePageRight(UserSession.RIGHT_A, "/Payroll/Payroll");
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;

            return View(model);
        }

        public ActionResult PayrollCalWorkHour(Nullable<int> pComID, Nullable<int> pEmpID, string pFromDate, string pToDate)
        {
            var hours = 0M;
            HttpWebResponse response = null;
            StreamReader readStream = null;
            var request = (HttpWebRequest)WebRequest.Create(string.Format(AppSetting.SERVER_NAME + ModuleDomain.Time + "/WServ/GetWorkingHours?pComID=" + pComID + "&pEmpID=" + pEmpID + "&pSDate=" + pFromDate.Replace("/", "-") + "&pEDate=" + pToDate.Replace("/", "-")));
            request.Method = "Get";
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                Stream receiveStream = response.GetResponseStream();
                readStream = new StreamReader(receiveStream, Encoding.UTF8);
                var rawJson = readStream.ReadToEnd();
                var json = JObject.Parse(rawJson);
                if (json.Count == 2)
                {
                    hours = NumUtil.ParseDecimal(json["workingHours"].ToObject<string>());
                    var mins = NumUtil.ParseDecimal(json["workingMins"].ToObject<string>());
                    hours += (mins / 60);
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (readStream != null)
                    readStream.Close();
                if (response != null)
                    response.Close();
            }
            return Json(new { No_Of_Hours = hours }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult PayrollNewPRD(int pIndex, string pAllowType)
        {
            var userlogin = UserSession.getUser(HttpContext);
            var cSerevice = new ComboService();
            var companyService = new CompanyService();
            var model = new PayrollDetailViewModel() { Index = pIndex, Row_Type = RowType.ADD, Amount = 0M, Hours_Worked = 0 };
            var company = companyService.GetCompany(userlogin.Company_ID);
            if (company != null && company.Currency != null)
            {
                model.Company_Currency_Code = company.Currency.Currency_Code;
                model.Company_Currency_ID = company.Currency_ID.Value;
            }

            model.prtList = cSerevice.LstPRT(pAllowType);
            model.CPF_Deduction = true;
            if (pAllowType == PayrollAllowanceType.Allowance_Deduction)
            {
                return PartialView("_PayrollAllowanceRow", model);
            }
            else if (pAllowType == PayrollAllowanceType.Overtime)
            {
                var ot = model.prtList.FirstOrDefault();
                if (ot != null)
                    model.PRT_ID = NumUtil.ParseInteger(ot.Value);

                model.prcOvertimeList = cSerevice.LstPRC(userlogin.Company_ID, pAllowType);
                return PartialView("_PayrollOvertimeRow", model);
            }

            else if (pAllowType == PayrollAllowanceType.Donation)
            {
                var donation = model.prtList.FirstOrDefault();
                if (donation != null)
                    model.PRT_ID = NumUtil.ParseInteger(donation.Value);

                model.prcDonationList = cSerevice.LstPRC(userlogin.Company_ID, pAllowType);
                return PartialView("_PayrollDonationRow", model);
            }
            return null;
        }

        public ActionResult PayrollProcessOTFormula(Nullable<int> select_ot_id, decimal basicsalary, decimal leaveamount, decimal expensesamount, decimal allowance, decimal deduction, decimal deductionadhoc, decimal allowanceadhoc, decimal adjustaddition, decimal adjustdeduction, decimal commission, decimal bonus, int empid, int prcid, int pmonth, int pyear)
        {
            var userlogin = UserSession.getUser(HttpContext);
            var pService = new PayrollService();

            var empService = new EmployeeService();

            var currentdate = StoredProcedure.GetCurrentDate();
            var emp = empService.GetEmployeeProfile2(empid);
            var prc = pService.GetPRC(prcid);

            Selected_OT_Formula otFormula = null;
            if (select_ot_id.HasValue)
                otFormula = pService.GetSelectedOTFormula(select_ot_id.Value);
            else
                otFormula = pService.GetCurrentSelectedOTFormulas(userlogin.Company_ID.Value);
            var empAge = 0;
            var pryear = 0;
            if (emp != null)
            {
                var processdate = DateUtil.ToDate(1, pmonth, pyear);
                if (emp.DOB.HasValue)
                {
                    if (processdate.Value.Date.Month > emp.DOB.Value.Month)
                        empAge = (processdate.Value.Date.Year - emp.DOB.Value.Year) + 1;
                    else
                        empAge = (processdate.Value.Date.Year - emp.DOB.Value.Year);
                }
                if (emp.NRIC_FIN_Issue_Date.HasValue)
                    pryear = (int)Math.Floor((processdate.Value.Date - emp.NRIC_FIN_Issue_Date.Value.Date).TotalDays / 365);
            }

            if (leaveamount < 0)
                leaveamount = leaveamount * -1;

            double ot = 0;
            if (otFormula != null && emp != null && prc != null)
            {
                var expr = "";
                select_ot_id = otFormula.ID;
                string formula = otFormula.OT_Formula.Formula;
                formula = formula.Replace("$", "");
                formula = formula.Replace(Environment.NewLine, "");
                formula = formula.Replace(FormulaVariable.Bonus, bonus.ToString());
                formula = formula.Replace(FormulaVariable.Deduction, deduction.ToString());
                formula = formula.Replace(FormulaVariable.Adjustment_Allowance, adjustaddition.ToString());
                formula = formula.Replace(FormulaVariable.Adjustment_Deductions, adjustdeduction.ToString());
                formula = formula.Replace(FormulaVariable.Deduction_Ad_Hoc, deductionadhoc.ToString());
                formula = formula.Replace(FormulaVariable.Allowance, allowance.ToString());
                formula = formula.Replace(FormulaVariable.Local, "L");
                formula = formula.Replace(FormulaVariable.PR, "P");
                formula = formula.Replace(FormulaVariable.Internship, "I");
                formula = formula.Replace(FormulaVariable.Employee_Status, emp.Emp_Status);
                formula = formula.Replace(FormulaVariable.PR_Years, pryear.ToString());
                formula = formula.Replace(FormulaVariable.Basic_Salary, basicsalary.ToString());
                formula = formula.Replace(FormulaVariable.Commission, commission.ToString());
                formula = formula.Replace(FormulaVariable.Current_Date, DateUtil.ToDisplayDate(currentdate));
                formula = formula.Replace(FormulaVariable.Employee_Age, empAge.ToString());
                formula = formula.Replace(FormulaVariable.Employee_Residential_Status, emp.Residential_Status);
                formula = formula.Replace(FormulaVariable.Leave_Amount, leaveamount.ToString());
                formula = formula.Trim();
                formula = formula.Replace(" ", "");

                expr = FormulaUtil.Condition(formula);

                var otVariable = FormulaVariable.Overtime.Replace(" ", "");
                string innerExp = "";
                for (int i = 0; i < expr.Length; i++)
                {
                    String s = expr.Substring(i, 1);
                    if (s.Equals("["))
                    {
                        if (!string.IsNullOrEmpty(innerExp))
                        {
                            if (innerExp.Contains(otVariable))
                            {
                                var otexpr = innerExp.Substring(innerExp.IndexOf("=") + 1, innerExp.Length - (innerExp.IndexOf("=") + 1));
                                ot = FormulaUtil.Evaluate(otexpr);
                                ot = (double)(prc.OT_Multiplier.HasValue ? prc.OT_Multiplier.Value : 1) * ot;
                                break;
                            }
                        }
                    }
                    innerExp += s;
                    if (i == expr.Length - 1)
                    {
                        if (innerExp.Contains(otVariable))
                        {
                            var otexpr = innerExp.Substring(innerExp.IndexOf("=") + 1, innerExp.Length - (innerExp.IndexOf("=") + 1));
                            ot = FormulaUtil.Evaluate(otexpr);
                            ot = (double)(prc.OT_Multiplier.HasValue ? prc.OT_Multiplier.Value : 1) * ot;
                            break;
                        }
                    }
                }
            }

            return Json(new { ot = ot, select_ot_id = select_ot_id }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PayrollProcessFormula(Nullable<int> select_cpf_id, decimal basicsalary, decimal leaveamount, decimal expensesamount, decimal allowance, decimal deduction, decimal deductionadhoc, decimal allowanceadhoc, decimal adjustaddition, decimal adjustdeduction, decimal overtime, decimal commission, decimal bonus, int empid, int pmonth, int pyear)
        {
            double employeecon = 0;
            double employercon = 0;
            int cpf_Id = 0;
            double donation = 0;
            int donation_id = 0;
            string donation_type = "";
            PayrollProcessFormula2(basicsalary, leaveamount, expensesamount, allowance, deduction, deductionadhoc, allowanceadhoc, adjustaddition, adjustdeduction, overtime, commission, bonus, empid, ref employeecon, ref employercon, ref cpf_Id, pmonth, pyear, ref donation, ref donation_id, ref donation_type);
            return Json(new { employeecon = employeecon, employercon = employercon, select_cpf_id = cpf_Id, donation = donation, donation_type = donation_type, select_donation_id = donation_id }, JsonRequestBehavior.AllowGet);
        }

        private void PayrollProcessFormula2(decimal basicsalary, decimal leaveamount, decimal expensesamount, decimal allowance, decimal deduction, decimal deductionadhoc, decimal allowanceadhoc, decimal adjustaddition, decimal adjustdeduction, decimal overtime, decimal commission, decimal bonus, int empid, ref double employeecon, ref double employercon, ref int select_cpf_id, int pmonth, int pyear, ref double donation, ref int select_donation_id, ref string donation_type)
        {
            var userlogin = UserSession.getUser(HttpContext);
            var pService = new PayrollService();

            var empService = new EmployeeService();
            var hService = new EmploymentHistoryService();

            var cpfldate = DateUtil.ToDate(DateTime.DaysInMonth(pyear, pmonth), pmonth, pyear);
            var processdate = DateUtil.ToDate(1, pmonth, pyear);
            var donationdate = DateUtil.ToDate(DateTime.DaysInMonth(pyear, pmonth), pmonth, pyear);
            var emp = empService.GetEmployeeProfile2(empid);
            var hist = hService.GetEmploymentHistory(empid, processdate);
            string empstatus = "";
            Selected_CPF_Formula cpfFormula = null;
            var empAge = 0;
            var pryear = 0;
            if (emp != null)
            {
                cpfFormula = pService.GetSelectedCPFFormulas(userlogin.Company_ID.Value, cpfldate);
                if (emp.DOB.HasValue)
                {
                    if (processdate.Value.Date.Month > emp.DOB.Value.Month)
                        empAge = (processdate.Value.Date.Year - emp.DOB.Value.Year) + 1;
                    else
                        empAge = (processdate.Value.Date.Year - emp.DOB.Value.Year);
                }
                if (emp.NRIC_FIN_Issue_Date.HasValue) //**********
                    pryear = (int)Math.Floor((processdate.Value.Date - emp.NRIC_FIN_Issue_Date.Value.Date).TotalDays / 365);

                if (hist != null && hist.Employee_Type.HasValue)
                {
                    var g = new ComboService().GetLookup(hist.Employee_Type);
                    if (g != null)
                    {
                        empstatus = g.Name.Substring(0, 1);
                    }
                }

            }

            if (leaveamount < 0)
                leaveamount = leaveamount * -1;

            if (emp != null)
            {
                if (cpfFormula != null)
                {
                    var expr = "";
                    select_cpf_id = cpfFormula.ID;
                    var formula = cpfFormula.CPF_Formula.Formula;

                    formula = formula.Replace("$", "");
                    formula = formula.Replace(Environment.NewLine, "");
                    formula = formula.Replace(FormulaVariable.Bonus, bonus.ToString());
                    formula = formula.Replace(FormulaVariable.Deduction, deduction.ToString());
                    formula = formula.Replace(FormulaVariable.Adjustment_Allowance, adjustaddition.ToString());
                    formula = formula.Replace(FormulaVariable.Adjustment_Deductions, adjustdeduction.ToString());
                    formula = formula.Replace(FormulaVariable.Deduction_Ad_Hoc, deductionadhoc.ToString());
                    formula = formula.Replace(FormulaVariable.Allowance, allowance.ToString());
                    formula = formula.Replace(FormulaVariable.Local, "L");
                    formula = formula.Replace(FormulaVariable.PR, "P");
                    formula = formula.Replace(FormulaVariable.Internship, "I");
                    formula = formula.Replace(FormulaVariable.Employee_Status, empstatus);
                    formula = formula.Replace(FormulaVariable.PR_Years, pryear.ToString());
                    formula = formula.Replace(FormulaVariable.Basic_Salary, basicsalary.ToString());
                    formula = formula.Replace(FormulaVariable.Commission, commission.ToString());
                    formula = formula.Replace(FormulaVariable.Current_Date, DateUtil.ToDisplayDate(processdate));
                    formula = formula.Replace(FormulaVariable.Employee_Age, empAge.ToString());
                    formula = formula.Replace(FormulaVariable.Employee_Residential_Status, emp.Residential_Status);
                    formula = formula.Replace(FormulaVariable.Leave_Amount, leaveamount.ToString());
                    formula = formula.Replace(FormulaVariable.Overtime, overtime.ToString());
                    formula = formula.Trim();
                    formula = formula.Replace(" ", "");
                    formula = formula.Replace("\t", "");

                    //var cnt = 0;
                    //foreach (var c in FormulaUtil.GetFormulaCase())
                    //{
                    //    var result = FormulaUtil.Condition(c);
                    //    if (result != "1")
                    //    {
                    //        var error = c;
                    //    }
                    //    cnt++;
                    //}

                    expr = FormulaUtil.Condition(formula);
                    var employeeVariable = FormulaVariable.Employee_Contribution.Replace(" ", "");
                    var totalVariable = FormulaVariable.Total_CPF_Contribution.Replace(" ", "");

                    double totalcon = 0;
                    string innerExp = "";
                    for (int i = 0; i < expr.Length; i++)
                    {
                        var s = expr.Substring(i, 1);
                        if (s.Equals("["))
                        {
                            if (!string.IsNullOrEmpty(innerExp))
                            {
                                if (innerExp.Contains(employeeVariable))
                                {
                                    var employeeexpr = innerExp.Substring(innerExp.IndexOf("=") + 1, innerExp.Length - (innerExp.IndexOf("=") + 1));
                                    employeecon = Math.Floor(FormulaUtil.Evaluate(employeeexpr));
                                    innerExp = "";
                                }
                                if (innerExp.Contains(totalVariable))
                                {
                                    var totalexpr = innerExp.Substring(innerExp.IndexOf("=") + 1, innerExp.Length - (innerExp.IndexOf("=") + 1));
                                    decimal total = NumUtil.ParseDecimal(FormulaUtil.Evaluate(totalexpr).ToString("n0"));
                                    totalcon = (double)total;
                                    innerExp = "";
                                }

                            }
                        }
                        innerExp += s;
                        if (i == expr.Length - 1)
                        {
                            if (innerExp.Contains(employeeVariable))
                            {
                                var employeeexpr = innerExp.Substring(innerExp.IndexOf("=") + 1, innerExp.Length - (innerExp.IndexOf("=") + 1));
                                employeecon = Math.Floor(FormulaUtil.Evaluate(employeeexpr));
                            }
                            if (innerExp.Contains(totalVariable))
                            {
                                var totalexpr = innerExp.Substring(innerExp.IndexOf("=") + 1, innerExp.Length - (innerExp.IndexOf("=") + 1));
                                decimal total = NumUtil.ParseDecimal(FormulaUtil.Evaluate(totalexpr).ToString("n0"));
                                totalcon = (double)total;
                            }
                        }
                    }
                    employercon = totalcon - employeecon;
                }

                if (emp.Residential_Status == "P" | emp.Residential_Status == "L")
                {
                    Selected_Donation_Formula donationFormula = pService.GetSelectedDonationFormulas(userlogin.Company_ID.Value, cpfldate, emp.Race);
                    if (donationFormula != null)
                    {
                        if (donationFormula.Donation_Formula != null)
                            donation_type = donationFormula.Donation_Formula.Formula_Name;

                        var expr = "";
                        select_donation_id = donationFormula.ID;
                        string formula = donationFormula.Donation_Formula.Formula;

                        formula = formula.Replace("$", "");
                        formula = formula.Replace(Environment.NewLine, "");
                        formula = formula.Replace(FormulaVariable.Bonus, bonus.ToString());
                        formula = formula.Replace(FormulaVariable.Deduction, deduction.ToString());
                        formula = formula.Replace(FormulaVariable.Adjustment_Allowance, adjustaddition.ToString());
                        formula = formula.Replace(FormulaVariable.Adjustment_Deductions, adjustdeduction.ToString());
                        formula = formula.Replace(FormulaVariable.Deduction_Ad_Hoc, deductionadhoc.ToString());
                        formula = formula.Replace(FormulaVariable.Allowance, allowance.ToString());
                        formula = formula.Replace(FormulaVariable.Local, "L");
                        formula = formula.Replace(FormulaVariable.PR, "P");
                        formula = formula.Replace(FormulaVariable.Internship, "I");
                        formula = formula.Replace(FormulaVariable.Employee_Status, emp.Emp_Status);
                        formula = formula.Replace(FormulaVariable.PR_Years, pryear.ToString());
                        formula = formula.Replace(FormulaVariable.Basic_Salary, basicsalary.ToString());
                        formula = formula.Replace(FormulaVariable.Commission, commission.ToString());
                        formula = formula.Replace(FormulaVariable.Current_Date, DateUtil.ToDisplayDate(processdate));
                        formula = formula.Replace(FormulaVariable.Employee_Age, empAge.ToString());
                        formula = formula.Replace(FormulaVariable.Employee_Residential_Status, emp.Residential_Status);
                        formula = formula.Replace(FormulaVariable.Leave_Amount, leaveamount.ToString());
                        formula = formula.Replace(FormulaVariable.Overtime, overtime.ToString());
                        formula = formula.Trim();
                        formula = formula.Replace(" ", "");
                        formula = formula.Replace("\t", "");

                        expr = FormulaUtil.Condition(formula);
                        var donationVariable = FormulaVariable.Donation.Replace(" ", "");

                        string innerExp = "";
                        for (int i = 0; i < expr.Length; i++)
                        {
                            String s = expr.Substring(i, 1);
                            if (s.Equals("["))
                            {
                                if (!string.IsNullOrEmpty(innerExp))
                                {
                                    if (innerExp.Contains(donationVariable))
                                    {
                                        var donationexpr = innerExp.Substring(innerExp.IndexOf("=") + 1, innerExp.Length - (innerExp.IndexOf("=") + 1));
                                        donation = FormulaUtil.Evaluate(donationexpr);
                                        innerExp = "";
                                    }

                                }
                            }
                            innerExp += s;
                            if (i == expr.Length - 1)
                            {
                                if (innerExp.Contains(donationVariable))
                                {
                                    var donationexpr = innerExp.Substring(innerExp.IndexOf("=") + 1, innerExp.Length - (innerExp.IndexOf("=") + 1));
                                    donation = FormulaUtil.Evaluate(donationexpr);
                                    innerExp = "";
                                }
                            }
                        }
                    }
                }
            }
        }

        public ActionResult PayrollLoadPRC(Nullable<int> pPrcID)
        {
            var pService = new PayrollService();
            var prc = pService.GetPRC(pPrcID);
            if (prc != null)
                return Json(new { prc_id = prc.PRC_ID, name = prc.Name, desc = prc.Description, deductable = prc.CPF_Deductable.HasValue ? prc.CPF_Deductable.Value : true }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { prc_id = pPrcID, name = "", desc = "", deductable = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PayrollAutoComfirm(PayrollViewModels model)
        {
            var payrollService = new PayrollService();
            var currentdate = StoredProcedure.GetCurrentDate();

            if (model.Process_Year == 0)
                model.Process_Year = currentdate.Year;
            if (model.Process_Month == 0)
                model.Process_Month = currentdate.Month;

            var result = payrollService.PayrollConfirm(model.empIds, model.Process_Month, model.Process_Year);
            if (result.Code == ERROR_CODE.SUCCESS)
            {
                /*send mail payslip*/
                if (result.Object != null)
                {
                    foreach (var prmID in result.Object as List<int>)
                    {
                        PayrollPrintSlip(prmID, sendmail: true);
                    }
                }
                return RedirectToAction("Payroll", new { Code = result.Code, Msg = result.Msg, Process_Month = model.Process_Month, Process_Year = model.Process_Year });
            }
            else
            {
                var userlogin = UserSession.getUser(HttpContext);
                var cSerevice = new ComboService();
                model.departmentList = cSerevice.LstDepartment(userlogin.Company_ID, true);
                model.processDateList = cSerevice.LstMonth();
                model.processStatusList = cSerevice.LstStatus(true);
                model.payrollList = payrollService.ListPayroll(userlogin.Company_ID, model.sDepartment, model.Process_Month, model.Process_Year, model.sProcess);

                var rightResult = base.validatePageRight(UserSession.RIGHT_A, "/Payroll/Payroll");
                if (rightResult.action != null) return rightResult.action;
                model.rights = rightResult.rights;
                model.result = result;
                return View("Payroll", model);
            }
        }

        private PRDViewModel[] PayrollConvertPRD(PRD[] prds)
        {
            var list = new List<PRDViewModel>();
            foreach (var row in prds)
            {
                list.Add(new PRDViewModel()
                {
                    Payroll_Detail_ID = row.Payroll_Detail_ID,
                    PRM_ID = row.PRM_ID,
                    PRT_ID = row.PRT_ID,
                    PRC_ID = row.PRC_ID,
                    Currency_ID = row.Currency_ID,
                    Amount = row.Amount,
                    Description = (row.PRC != null ? row.PRC.Name : row.Description),
                    Hours_Worked = row.Hours_Worked,
                    Type = row.PRT.Name,
                    Row_Type = RowType.EDIT,
                    History_Allowance_ID = row.Employment_History_Allowance_ID,
                    //CPF_Deduction = (row.PRC != null && row.PRC.CPF_Deductable.HasValue ? row.PRC.CPF_Deductable.Value : false),
                    CPF_Deduction = row.PRC != null ? (row.PRC.CPF_Deductable.HasValue ? row.PRC.CPF_Deductable.Value : false) : true
                });
            }

            return list.ToArray();
        }

        public ActionResult PayrollConfirm(PayrollViewModels model, int pPRMID)
        {
            var payrollService = new PayrollService();
            var userlogin = UserSession.getUser(HttpContext);
            var currentdate = StoredProcedure.GetCurrentDate();
            model.result = payrollService.PayrollConfirm(pPRMID);
            if (model.result.Code == ERROR_CODE.SUCCESS)
                PayrollPrintSlip(pPRMID, sendmail: true);/*do send mail*/


            var route = new RouteValueDictionary(model.result);
            route.Add("Process_Month", model.Process_Month);
            route.Add("Process_Year", model.Process_Year);
            return RedirectToAction("Payroll", route);
        }

        public ActionResult PayrollAutolProcess(PayrollViewModels model)
        {
            var userlogin = UserSession.getUser(HttpContext);
            var payrollService = new PayrollService();
            if (model.empIds != null && model.empIds.Length > 0)
            {
                var currentdate = StoredProcedure.GetCurrentDate();
                var employeeService = new EmployeeService();
                var empHistService = new EmploymentHistoryService();

                if (model.Process_Year == 0)
                    model.Process_Year = currentdate.Year;
                if (model.Process_Month == 0)
                    model.Process_Month = currentdate.Month;

                var prms = new List<PRM>();
                foreach (var empid in model.empIds)
                {
                    var emp = employeeService.GetEmployeeProfile2(empid);
                    if (emp != null)
                    {
                        var dupprm = payrollService.GetPRMEmployee(empid).Where(w => w.Process_Month == model.Process_Month && w.Process_Year == model.Process_Year).FirstOrDefault();
                        if (dupprm != null)
                            continue;

                        var bankinfoservice = new BankInfoService();
                        var bankinfo = bankinfoservice.GetCurrentBankInfo(emp.Employee_Profile_ID);

                        decimal basicsalary = 0;
                        decimal totalbonus = 0;

                        var effectivedate = DateUtil.ToDate(1, model.Process_Month, model.Process_Year);
                        var empHist = empHistService.GetPayrollEmploymentHistory(empid, effectivedate.Value);
                        if (empHist != null)
                        {

                            basicsalary = NumUtil.ParseDecimal(EncryptUtil.Decrypt(empHist.Basic_Salary));
                            if (basicsalary == 0)
                                basicsalary = NumUtil.ParseDecimal(EncryptUtil.Decrypt(EncryptUtil.Decrypt(empHist.Basic_Salary)));

                            if (empHist.Basic_Salary_Unit == Term.Hourly)
                            {

                            }
                        }

                        double employeecon = 0;
                        double employercon = 0;
                        int cpf_Id = 0;
                        double donation = 0;
                        int donation_id = 0;
                        string donation_type = "";

                        var nett = (basicsalary + totalbonus) - (decimal)employeecon;
                        var gross = (basicsalary + totalbonus);

                        PayrollProcessFormula2(basicsalary, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, totalbonus, empid, ref employeecon, ref employercon, ref cpf_Id, model.Process_Month, model.Process_Year, ref donation, ref donation_id, ref donation_type);

                        var prm = new PRM();
                        prm.Run_date = currentdate;
                        prm.Payment_Type = (bankinfo != null ? bankinfo.Payment_Type : null);
                        prm.Employee_Profile_ID = empid;
                        prm.Total_Allowance = 0;
                        prm.Total_Deduction = 0;
                        prm.Total_Bonus = totalbonus;
                        prm.CPF_Employee = (decimal)employeecon;
                        prm.CPF_Emplyer = (decimal)employercon;
                        prm.Selected_CPF_Formula_ID = cpf_Id;
                        prm.Nett_Wages = nett - (decimal)employeecon; // โค้ดที่เเก้ไข
                        prm.Gross_Wages = gross;
                        prm.Process_Month = model.Process_Month;
                        prm.Process_Year = model.Process_Year;
                        prm.Process_Status = PayrollStatus.Process;
                        prm.Create_By = userlogin.User_Authentication.Email_Address;
                        prm.Create_On = currentdate;
                        prm.Update_By = userlogin.User_Authentication.Email_Address;
                        prm.Update_On = currentdate;
                        prm.Process_Date_From = DateUtil.ToDate(1, model.Process_Month, model.Process_Year);
                        prm.Process_Date_To = DateUtil.ToDate(DateTime.DaysInMonth(model.Process_Year, model.Process_Month), model.Process_Month, model.Process_Year);
                        prm.Revision_No = 1;
                        prm.Hourly_Rate = model.Hourly_Rate;
                        if (prm.Selected_CPF_Formula_ID == 0)
                            prm.Selected_CPF_Formula_ID = null;

                        //Added by sun 09-02-2017
                        prm.Total_Allowance_Basic_Salary = 0;

                        prms.Add(prm);
                    }
                }

                var result = payrollService.InsertPayroll(prms);
                if (result.Code == ERROR_CODE.SUCCESS)
                    return RedirectToAction("Payroll", new { Code = result.Code, Msg = result.Msg, Field = result.Field, Process_Month = model.Process_Month, Process_Year = model.Process_Year });
                else
                    model.result = result;
            }

            var cSerevice = new ComboService();
            model.departmentList = cSerevice.LstDepartment(userlogin.Company_ID, true);
            model.processDateList = cSerevice.LstMonth();
            model.processStatusList = cSerevice.LstStatus(true);
            model.payrollList = payrollService.ListPayroll(userlogin.Company_ID, model.sDepartment, model.Process_Month, model.Process_Year, model.sProcess);

            var rightResult = base.validatePageRight(UserSession.RIGHT_A, "/Payroll/Payroll");
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;

            return View("Payroll", model);
        }

        [HttpGet]
        public ActionResult PayrollReport(ServiceResult result, PayrollReportViewModel model, string tabAction = "")
        {

            var userlogin = UserSession.getUser(HttpContext);
            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            //-------rights------------
            var rightResult = base.validatePageRight(UserSession.RIGHT_A);
            if (rightResult.action != null)
                return rightResult.action;
            model.rights = rightResult.rights;

            var currentdate = StoredProcedure.GetCurrentDate();
            var lService = new LeaveService();
            var cbService = new ComboService();
            var comService = new CompanyService();
            var deparService = new DepartmentService();
            var payrollService = new PayrollService();

            if (!model.sFromYear.HasValue && model.sFromYear == null)
                model.sFromYear = currentdate.Year;
            if (!model.sToYear.HasValue && model.sToYear == null)
                model.sToYear = currentdate.Year;
            //filter
            model.departmentList = cbService.LstDepartment(userlogin.Company_ID.Value, hasBlank: true);
            model.processDateList = cbService.LstMonth(hasBlank: true);
            model.prmList = payrollService.ListPayroll(userlogin.Company_ID.Value, model.Department, model.sFromMonth, model.sFromYear, model.sToMonth, model.sToYear);

            if (tabAction == "export")
            {
                string csv = "";
                string dep_name = "";
                string sFrom = "";
                string sTo = "";

                if (model.Department.HasValue)
                {
                    var department = deparService.GetDepartment(model.Department.Value);
                    if (department != null) dep_name = department.Name;
                }

                if (model.sFromMonth.HasValue)
                    sFrom = "/";
                if (model.sToMonth.HasValue)
                    sTo = "/";
                var fullName = UserSession.GetUserName(userlogin);

                //HEADER
                string compname = comService.GetCompany(userlogin.Company_ID).Name;
                csv += "<table><tr valign='top'><td valign='top'><b> " + compname + " </b></td><td>&nbsp;</td><td><b>" + Resource.Payroll_Report + "</b><br><b> " + Resource.From + " </b> " + model.sFromMonth + sFrom + model.sFromYear + " - " + model.sToMonth + sTo + model.sToYear + "</td></tr>";
                csv += "<tr><td>&nbsp;</td><td>&nbsp;</td><td><b>" + Resource.Department + "</b> " + dep_name + " </td></tr>";
                csv += "<tr><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td></tr></table>";

                csv += "<table border=1><tr><td><b>" + Resource.Employee_No_SymbolDot + "</b></td> ";
                csv += "<td><b>" + Resource.Employee_Name + "</b></td> ";
                csv += "<td><b>" + Resource.Designation + "</b></td> ";

                csv += "<td><b>" + Resource.Basic_Salary + "</b></td> ";
                csv += "<td><b>" + Resource.Expenses + "</b></td> ";
                csv += "<td><b>" + Resource.Allowance + "</b></td> ";
                csv += "<td><b>" + Resource.Adjustment_Addition + "</b></td> ";
                csv += "<td><b>" + Resource.Commission + "</b></td> ";
                csv += "<td><b>" + Resource.Overtime + "</b></td> ";
                csv += "<td><b>" + Resource.Donation + "</b></td> ";

                csv += "<td><b>" + Resource.Deductions + "</b></td> ";
                csv += "<td><b>" + Resource.Adjustment_Deduction + "</b></td> ";
                csv += "<td><b>" + Resource.Deduction_Donation + "</b></td> ";

                csv += "<td><b>" + Resource.Employer_CPF_Contribution + "</b></td> ";
                csv += "<td><b>" + Resource.Employee_CPF_Contribution + "</b></td> ";
                csv += "<td><b>" + Resource.Total + "</b></td></tr>";

                decimal a_BasicSalary = 0;
                decimal a_Expenses = 0;
                decimal a_Allowance = 0;
                decimal a_Adjustment_Addition = 0;
                decimal a_Commission = 0;
                decimal a_Overtime = 0;
                decimal a_Donation = 0;

                decimal a_Deduction = 0;
                decimal a_Adjustment_Deduction = 0;
                decimal a_Deduction_Donation = 0;
                decimal a_EmployeeCPF = 0;
                decimal a_EmployerCPF = 0;
                decimal a_total = 0;

                int emp_count = 0;
                var emps = model.prmList.Select(s => s.Employee_Profile).Distinct().ToList();
                if (emps != null && emps.Count > 0)
                {
                    emp_count = emps.Count;
                    foreach (var employee in emps)
                    {
                        string designation = "";

                        decimal BasicSalary = 0;
                        decimal Expenses = 0;
                        decimal Allowance = 0;
                        decimal Adjustment_Addition = 0;
                        decimal Commission = 0;
                        decimal Overtime = 0;
                        decimal Donation = 0;

                        decimal Deduction = 0;
                        decimal Adjustment_Deduction = 0;
                        decimal Deduction_Donation = 0;
                        decimal EmployeeCPF = 0;
                        decimal EmployerCPF = 0;
                        decimal total = 0;
                        string paymentterm = "";

                        if (employee != null && employee.Employment_History != null && employee.Employment_History.Count() > 0)
                        {
                            try
                            {
                                var emphist = (new EmploymentHistoryService()).GetCurrentEmploymentHistory(employee.Employee_Profile_ID);
                                if (emphist != null)
                                {
                                    designation = emphist.Designation.Name;
                                    paymentterm = emphist.Basic_Salary_Unit;
                                }
                            }
                            catch { }

                            List<PRM> prmList = model.prmList.Where(w => w.Employee_Profile_ID == employee.Employee_Profile_ID).ToList();
                            if (prmList != null && prmList.Count > 0)
                            {
                                foreach (PRM p in prmList)
                                {
                                    //Edit by sun 09-02-2016
                                    var total_salary = 0M;
                                    if (p.Basic_Salary.HasValue && p.Basic_Salary.Value > 0)
                                        total_salary = p.Basic_Salary.Value;

                                    if (p.Total_Allowance_Basic_Salary.HasValue && p.Total_Allowance_Basic_Salary.Value > 0)
                                        total_salary += p.Total_Allowance_Basic_Salary.Value;

                                    if (total_salary > 0)
                                        BasicSalary += total_salary;
                                    else
                                    {
                                        if (paymentterm == Term.Hourly)
                                        {
                                            BasicSalary += ((p.Hourly_Rate.HasValue ? p.Hourly_Rate.Value : 0) * (p.No_Of_Hours.HasValue ? p.No_Of_Hours.Value : 0));
                                        }
                                        else
                                        {
                                            var effectivedate = DateUtil.ToDate(1, p.Process_Month.Value, p.Process_Year.Value);
                                            var emphist2 = (new EmploymentHistoryService()).GetEmploymentHistory(employee.Employee_Profile_ID, effectivedate.Value);
                                            if (emphist2 != null)
                                            {
                                                var salary = NumUtil.ParseDecimal(EncryptUtil.Decrypt(emphist2.Basic_Salary));
                                                if (salary == 0)
                                                {
                                                    salary = NumUtil.ParseDecimal(EncryptUtil.Decrypt(EncryptUtil.Decrypt(emphist2.Basic_Salary)));
                                                }
                                                BasicSalary += salary;
                                            }

                                        }
                                    }


                                    foreach (PRD d in p.PRDs.ToList())
                                    {
                                        if (d.PRT != null && d.PRT.Name == PayrollAllowanceType.Allowance)
                                            Allowance += (d.Amount.HasValue ? d.Amount.Value : 0);
                                        else if (d.PRT != null && d.PRT.Name == PayrollAllowanceType.Adjustment_Addition)
                                            Adjustment_Addition += (d.Amount.HasValue ? d.Amount.Value : 0);
                                        else if (d.PRT != null && d.PRT.Name == PayrollAllowanceType.Commission)
                                            Commission += (d.Amount.HasValue ? d.Amount.Value : 0);
                                        else if (d.PRT != null && d.PRT.Name == PayrollAllowanceType.Overtime)
                                            Overtime += (d.Amount.HasValue ? d.Amount.Value : 0);
                                        else if (d.PRT != null && d.PRT.Name == PayrollAllowanceType.Donation)
                                            Donation += (d.Amount.HasValue ? d.Amount.Value : 0);

                                        //-------------------------------------------------------//
                                        else if (d.PRT != null && d.PRT.Name == PayrollAllowanceType.Deduction)
                                            Deduction += (d.Amount.HasValue ? d.Amount.Value : 0);
                                        else if (d.PRT != null && d.PRT.Name == PayrollAllowanceType.Adjustment_Deduction)
                                            Adjustment_Deduction += (d.Amount.HasValue ? d.Amount.Value : 0);
                                        else if (d.PRT != null && d.PRT.Name == PayrollAllowanceType.Deduction_Donation)
                                            Deduction_Donation += (d.Amount.HasValue ? d.Amount.Value : 0);
                                    }

                                    foreach (var prde in p.PRDEs.ToList())
                                    {
                                        Expenses = Expenses + (prde.Expenses_Application_Document.Amount_Claiming.HasValue ? prde.Expenses_Application_Document.Amount_Claiming.Value : 0);
                                    }

                                    EmployeeCPF += (p.CPF_Employee.HasValue ? p.CPF_Employee.Value : 0);
                                    EmployerCPF += (p.CPF_Emplyer.HasValue ? p.CPF_Emplyer.Value : 0);
                                }
                            }
                        }


                        total = (BasicSalary + Expenses + Allowance + Adjustment_Addition + Commission + Overtime + Donation) - (EmployeeCPF + Deduction + Adjustment_Deduction + Deduction_Donation);

                        var empFullName = "";
                        if (employee != null)
                            empFullName = UserSession.GetUserName(employee.User_Profile);

                        csv += "<tr><td>" + employee.Employee_No + "</td> ";
                        csv += "<td>" + empFullName + "</td> ";
                        csv += "<td>" + designation + "</td> ";
                        csv += "<td>" + BasicSalary.ToString("n2") + "</td> ";
                        csv += "<td>" + Expenses.ToString("n2") + "</td> ";
                        csv += "<td>" + Allowance.ToString("n2") + "</td> ";
                        csv += "<td>" + Adjustment_Addition.ToString("n2") + "</td> ";
                        csv += "<td>" + Commission.ToString("n2") + "</td> ";
                        csv += "<td>" + Overtime.ToString("n2") + "</td> ";
                        csv += "<td>" + Donation.ToString("n2") + "</td> ";

                        csv += "<td>" + Deduction.ToString("n2") + "</td> ";
                        csv += "<td>" + Adjustment_Deduction.ToString("n2") + "</td> ";
                        csv += "<td>" + Deduction_Donation.ToString("n2") + "</td> ";
                        csv += "<td>" + EmployerCPF.ToString("n2") + "</td> ";
                        csv += "<td>" + EmployeeCPF.ToString("n2") + "</td> ";
                        csv += "<td><b>" + total.ToString("n2") + "</b></td></tr>";

                        a_BasicSalary += BasicSalary;
                        a_Expenses += Expenses;
                        a_Allowance += Allowance;
                        a_Adjustment_Addition += Adjustment_Addition;
                        a_Commission += Commission;
                        a_Overtime += Overtime;
                        a_Donation += Donation;

                        a_Deduction += Deduction;
                        a_Adjustment_Deduction += Adjustment_Deduction;
                        a_Deduction_Donation += Deduction_Donation;
                        a_EmployeeCPF += EmployeeCPF;
                        a_EmployerCPF += EmployerCPF;
                        a_total += total;
                    }
                }

                csv += "<tr><td></td> ";
                csv += "<td></td> ";
                csv += "<td><b>" + Resource.Total + "</b></td> ";
                csv += "<td>" + a_BasicSalary.ToString("n2") + "</td> ";
                csv += "<td>" + a_Expenses.ToString("n2") + "</td> ";
                csv += "<td>" + a_Allowance.ToString("n2") + "</td> ";
                csv += "<td>" + a_Adjustment_Addition.ToString("n2") + "</td> ";
                csv += "<td>" + a_Commission.ToString("n2") + "</td> ";
                csv += "<td>" + a_Overtime.ToString("n2") + "</td> ";
                csv += "<td>" + a_Donation.ToString("n2") + "</td> ";

                csv += "<td>" + a_Deduction.ToString("n2") + "</td> ";
                csv += "<td>" + a_Adjustment_Deduction.ToString("n2") + "</td> ";
                csv += "<td>" + a_Deduction_Donation.ToString("n2") + "</td> ";
                csv += "<td><b>" + a_EmployerCPF.ToString("n2") + "</b></td> ";
                csv += "<td><b>" + a_EmployeeCPF.ToString("n2") + "</b></td> ";
                csv += "<td><b>" + a_total.ToString("n2") + "</b></td></tr>";

                csv += "<tr><td></td> ";
                csv += "<td></td> ";
                csv += "<td><b>" + Resource.Total_No_Of_Staff + "</b></td> ";
                csv += "<td><b>" + emp_count + "</b></td> ";
                csv += "<td></td> ";
                csv += "<td></td> ";
                csv += "<td></td> ";
                csv += "<td></td> ";
                csv += "<td></td> ";
                csv += "<td></td> ";
                csv += "<td></td> ";
                csv += "<td></td> ";
                csv += "<td></td> ";
                csv += "<td></td> ";
                csv += "<td></td> ";
                csv += "<td></tr>";

                csv += "</table>";
                csv += "<table><tr><td>&nbsp;</td></tr>";
                csv += "<tr><td><b> " + Resource.Printed_By + " </b> " + fullName + "</td></tr></table>";

                System.Web.UI.WebControls.GridView gv = new System.Web.UI.WebControls.GridView();
                //gv.DataSource = emp;
                gv.DataBind();
                Response.Clear();
                Response.ClearContent();
                Response.ClearHeaders();
                Response.Buffer = true;
                Response.BufferOutput = true;
                Response.AddHeader("content-disposition", "attachment; filename=PayrollReport.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                System.IO.StringWriter sw = new System.IO.StringWriter();
                sw.Write(csv);
                System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);
                gv.RenderControl(htw);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();

            }
            return View(model);
        }

        [HttpGet]
        public ActionResult PayrollImport()
        {
            var model = new ImportPayrollViewModels();

            var userlogin = UserSession.getUser(HttpContext);
            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);


            var rightResult = base.validatePageRight(UserSession.RIGHT_C);
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;

            model.prm_prd = new List<ImportPRM_PRD_>().ToArray();
            model.errMsg = new List<string>();
            model.validated_Main = true;

            return View(model);

        }

        [HttpPost]
        public ActionResult PayrollImport(ImportPayrollViewModels model, string pageAction)
        {
            var userlogin = UserSession.getUser(HttpContext);
            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);


            var rightResult = base.validatePageRight(UserSession.RIGHT_C);
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;

            //-------data------------
            DateTime currentdate = StoredProcedure.GetCurrentDate();
            var lService = new LeaveService();
            var cbService = new ComboService();
            var comService = new CompanyService();
            var empService = new EmployeeService();
            var payrollService = new PayrollService();
            var payrollService2 = new PayrollService();

            if (pageAction == "import")
            {
                if (model.prm_prd.Length > 0 && model.validated_Main)
                {
                    List<PRM> prms = new List<PRM>();
                    foreach (var row in model.prm_prd)
                    {
                        if (row.Validate && row.Employee_Profile_ID != null)
                        {
                            var prm = new PRM()
                            {
                                Employee_Profile_ID = row.Employee_Profile_ID.Value,
                                Process_Year = NumUtil.ParseInteger(row.Process_Year),
                                Process_Month = row.Process_Month,
                                Run_date = DateUtil.ToDate(row.Run_Date),
                                Total_Work_Days = row.Total_Work_Days,
                                Payment_Type = row.Payment_Type,
                                Process_Status = PayrollStatus.Process,
                                Create_By = userlogin.User_Authentication.Email_Address,
                                Create_On = currentdate,
                                Update_By = userlogin.User_Authentication.Email_Address,
                                Update_On = currentdate
                            };

                            if (row.PRT_ID_Overtime != null && row.PRT_ID_Overtime > 0)
                            {
                                var prd = new PRD()
                                {
                                    PRT_ID = row.PRT_ID_Overtime,
                                    PRC_ID = (row.PRC_ID_Overtime > 0 ? row.PRC_ID_Overtime : null),
                                    Amount = row.Overtime_Type_Amount,
                                    Hours_Worked = row.OT_Hours,
                                    Create_By = userlogin.User_Authentication.Email_Address,
                                    Create_On = currentdate,
                                    Update_By = userlogin.User_Authentication.Email_Address,
                                    Update_On = currentdate
                                };
                                prm.PRDs.Add(prd);
                            }

                            if (row.PRT_ID_Allowance_Type != null && row.PRT_ID_Allowance_Type > 0)
                            {
                                var prd = new PRD()
                                {
                                    PRT_ID = row.PRT_ID_Allowance_Type,
                                    PRC_ID = (row.PRC_ID_Description > 0 ? row.PRC_ID_Description : null),
                                    Amount = row.Description_Type_Amount,
                                    Create_By = userlogin.User_Authentication.Email_Address,
                                    Create_On = currentdate,
                                    Update_By = userlogin.User_Authentication.Email_Address,
                                    Update_On = currentdate
                                };
                                prm.PRDs.Add(prd);
                            }

                            if (row.PRT_ID_Donation != null && row.PRT_ID_Donation > 0)
                            {
                                var prd = new PRD()
                                {
                                    PRT_ID = row.PRT_ID_Donation,
                                    PRC_ID = (row.PRC_ID_Donation > 0 ? row.PRC_ID_Donation : null),
                                    Amount = row.Donation_Type_Amount,
                                    Create_By = userlogin.User_Authentication.Email_Address,
                                    Create_On = currentdate,
                                    Update_By = userlogin.User_Authentication.Email_Address,
                                    Update_On = currentdate
                                };
                                prm.PRDs.Add(prd);
                            }
                            prms.Add(prm);
                        }
                    }
                    if (prms != null)
                    {
                        model.result = payrollService2.InsertPayroll(prms.ToArray());
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                            return RedirectToAction("Payroll", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field });
                    }
                }
            }
            else
            {

                if (Request.Files.Count == 0)
                {
                    ModelState.AddModelError("Import_Payroll", Resource.Message_Cannot_Found_Excel_Sheet);
                    return View(model);
                }

                HttpPostedFileBase file = Request.Files[0];
                if (file != null)
                {
                    var com = comService.GetCompany(userlogin.Company_ID);
                    if (com == null)
                        return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

                    var employeeprofilelst = empService.LstEmployeeProfile(userlogin.Company_ID);
                    var monthlst = cbService.LstMonth();
                    var paymentTypelst = cbService.LstLookup(ComboType.Payment_Type, userlogin.Company_ID);

                    //Pass  OT
                    var prtOvertimeID = cbService.LstPRT(PayrollAllowanceType.Overtime).FirstOrDefault();
                    var prcOvertimelst = payrollService.GetPRCs(userlogin.Company_ID);

                    //Pass Allowance
                    var prtallowancelst = cbService.LstPRT(PayrollAllowanceType.Allowance_Deduction);
                    var prcalllst = cbService.LstPRC(userlogin.Company_ID, PayrollAllowanceType.Allowance_Deduction);

                    //Pass Donation
                    var prtDonationID = cbService.LstPRT(PayrollAllowanceType.Donation).FirstOrDefault();
                    var prcDonationlst = cbService.LstPRC(userlogin.Company_ID, NumUtil.ParseInteger(prtDonationID.Value));

                    try
                    {
                        using (var package = new ExcelPackage(file.InputStream))
                        {
                            List<string> chk_Emp_No = new List<string>();
                            model.validated_Main = true;

                            ExcelWorksheet worksheet_1 = package.Workbook.Worksheets[1];
                            if (worksheet_1.Dimension != null)
                            {
                                int totalRows_1 = worksheet_1.Dimension.End.Row;
                                int totalCols_1 = worksheet_1.Dimension.End.Column;

                                if (totalCols_1 != 14)
                                {
                                    ModelState.AddModelError("prm_prd", Resource.Message_Column_Count_Is_Invalid + " " + Resource.Message_Please_Edit_Reupload);
                                    model.validated_Main = false;
                                }
                                if (totalRows_1 <= 1)
                                {
                                    ModelState.AddModelError("prm_prd", Resource.Message_Row_Count_Is_Invalid + " " + Resource.Message_Please_Edit_Reupload);
                                    model.validated_Main = false;
                                }

                                if (ModelState.IsValid)
                                {
                                    if (totalRows_1 > 1)
                                    {
                                        var prm_prds = new List<ImportPRM_PRD_>();
                                        for (int i = 2; i <= totalRows_1; i++)
                                        {
                                            var prm_prd = new ImportPRM_PRD_();
                                            prm_prd.Company_ID = userlogin.Company_ID;
                                            prm_prd.Validate = true;
                                            var isempty = true;
                                            var err_ = new System.Text.StringBuilder();
                                            //Prt_ID from Prc_ID
                                            var prtIdTemp = 0;

                                            for (int j = 1; j <= totalCols_1; j++)
                                            {
                                                var columnName = worksheet_1.Cells[1, j].Value.ToString();
                                                isempty = false;
                                                if (worksheet_1.Cells[i, j].Value != null)
                                                {
                                                    if (j == PayrollImportColumn.Employee_No)
                                                    {
                                                        var emp_no = "";
                                                        emp_no = worksheet_1.Cells[i, j].Value.ToString();
                                                        prm_prd.Employee_No = emp_no;

                                                        var empprofil = employeeprofilelst.Where(w => w.Employee_No.ToString().Trim() == emp_no.ToString().Trim()).FirstOrDefault();
                                                        if (empprofil != null)
                                                        {
                                                            prm_prd.Employee_Profile_ID = empprofil.Employee_Profile_ID;
                                                        }
                                                        else
                                                        {
                                                            model.validated_Main = false;
                                                            prm_prd.Validate = false;
                                                            err_.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Not_Found_Lower);
                                                        }
                                                    }
                                                    else if (j == PayrollImportColumn.Process_Month)
                                                    {
                                                        var leaveType = monthlst.Where(w => w.Text.ToLower().Trim() == worksheet_1.Cells[i, j].Value.ToString().ToLower().Trim()).FirstOrDefault();
                                                        if (leaveType == null)
                                                        {
                                                            model.validated_Main = false;
                                                            prm_prd.Validate = false;
                                                            err_.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                                        }
                                                        else
                                                        {
                                                            prm_prd.Process_Month = NumUtil.ParseInteger(leaveType.Value);
                                                            prm_prd.Process_Month_ = leaveType.Text;
                                                        }
                                                    }
                                                    else if (j == PayrollImportColumn.Payment_Type)
                                                    {
                                                        var paymentType = paymentTypelst.Where(w => w.Text.ToLower().Trim() == worksheet_1.Cells[i, j].Value.ToString().ToLower().Trim()).FirstOrDefault();
                                                        if (paymentType == null)
                                                        {
                                                            model.validated_Main = false;
                                                            prm_prd.Validate = false;
                                                            err_.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                                        }
                                                        else
                                                        {
                                                            prm_prd.Payment_Type = NumUtil.ParseInteger(paymentType.Value);
                                                            prm_prd.Payment_Type_ = paymentType.Text;
                                                        }
                                                    }
                                                    else if (j == PayrollImportColumn.Run_Date)
                                                    {
                                                        var strdate = "";
                                                        try
                                                        {
                                                            var date = (DateTime)worksheet_1.Cells[i, j].Value;
                                                            strdate = DateUtil.ToDisplayDate(date);
                                                        }
                                                        catch
                                                        {
                                                            model.validated_Main = false;
                                                            prm_prd.Validate = false;
                                                            strdate = worksheet_1.Cells[i, j].Value.ToString();
                                                            err_.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                                        }

                                                        if (j == PayrollImportColumn.Run_Date)
                                                            prm_prd.Run_Date = strdate;

                                                    }
                                                    else if (j == PayrollImportColumn.Total_Work_Days)
                                                    {
                                                        decimal daystaken = 0;
                                                        try
                                                        {
                                                            daystaken = Convert.ToDecimal(worksheet_1.Cells[i, j].Value);
                                                        }
                                                        catch
                                                        {
                                                            model.validated_Main = false;
                                                            prm_prd.Validate = false;
                                                            daystaken = Convert.ToDecimal(worksheet_1.Cells[i, j].Value.ToString());
                                                            err_.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                                        }
                                                        if (j == PayrollImportColumn.Total_Work_Days)
                                                            prm_prd.Total_Work_Days = daystaken;
                                                    }
                                                    else if (j == PayrollImportColumn.Process_Year)
                                                    {
                                                        prm_prd.Process_Year = worksheet_1.Cells[i, j].Value.ToString();
                                                        if (prm_prd.Process_Year.Length != 4)
                                                        {
                                                            model.validated_Main = false;
                                                            prm_prd.Validate = false;
                                                            err_.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Message_Maximum_Length + " '4'.");
                                                        }
                                                    }
                                                    else if (j == PayrollImportColumn.OT_Hours
                                                        || j == PayrollImportColumn.Amount_Payable
                                                        || j == PayrollImportColumn.Description_Amount
                                                        || j == PayrollImportColumn.Donation_Amount)
                                                    {
                                                        decimal daystaken = 0;
                                                        try
                                                        {
                                                            daystaken = Convert.ToDecimal(worksheet_1.Cells[i, j].Value);
                                                        }
                                                        catch
                                                        {
                                                            model.validated_Main = false;
                                                            prm_prd.Validate = false;
                                                            daystaken = Convert.ToDecimal(worksheet_1.Cells[i, j].Value.ToString());
                                                            err_.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                                        }

                                                        if (j == PayrollImportColumn.OT_Hours)
                                                            prm_prd.OT_Hours = daystaken;
                                                        else if (j == PayrollImportColumn.Amount_Payable)
                                                            prm_prd.Overtime_Type_Amount = daystaken;
                                                        else if (j == PayrollImportColumn.Description_Amount)
                                                            prm_prd.Description_Type_Amount = daystaken;
                                                        else if (j == PayrollImportColumn.Donation_Amount)
                                                            prm_prd.Donation_Type_Amount = daystaken;
                                                    }
                                                    else if (j == PayrollImportColumn.OT_Rate)
                                                    {
                                                        decimal daystaken = 0;
                                                        try
                                                        {
                                                            daystaken = Convert.ToDecimal(worksheet_1.Cells[i, j].Value);
                                                        }
                                                        catch
                                                        {
                                                            model.validated_Main = false;
                                                            prm_prd.Validate = false;
                                                            daystaken = Convert.ToDecimal(worksheet_1.Cells[i, j].Value.ToString());
                                                            err_.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                                        }

                                                        var prcOvertime = prcOvertimelst.Where(w => w.OT_Multiplier == daystaken && w.PRT.Name == PayrollAllowanceType.Overtime).FirstOrDefault();
                                                        if (prcOvertime == null)
                                                        {
                                                            model.validated_Main = false;
                                                            prm_prd.Validate = false;
                                                            err_.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Not_Found_Lower + " " + Resource.Check_OT + ".");
                                                        }
                                                        else
                                                        {
                                                            if (prtOvertimeID != null)
                                                                prm_prd.PRT_ID_Overtime = NumUtil.ParseInteger(prtOvertimeID.Value);

                                                            prm_prd.PRC_ID_Overtime = NumUtil.ParseInteger(prcOvertime.PRC_ID);
                                                            prm_prd.PRC_ID_Overtime_ = prcOvertime.Name;
                                                            prm_prd.OT_Rate = daystaken;
                                                        }
                                                    }
                                                    else if (j == PayrollImportColumn.Allowance_Type)
                                                    {
                                                        var prtallowance = prtallowancelst.Where(w => w.Text.ToLower().Trim() == worksheet_1.Cells[i, j].Value.ToString().ToLower().Trim()).FirstOrDefault();
                                                        if (prtallowance == null)
                                                        {
                                                            model.validated_Main = false;
                                                            prm_prd.Validate = false;
                                                            err_.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                                        }
                                                        else
                                                        {
                                                            prm_prd.PRT_ID_Allowance_Type = NumUtil.ParseInteger(prtallowance.Value);
                                                            prm_prd.PRT_ID_Allowance_Type_ = prtallowance.Text;

                                                            //Prt_ID from Prc_ID
                                                            prtIdTemp = NumUtil.ParseInteger(prtallowance.Value);
                                                        }
                                                    }
                                                    else if (j == PayrollImportColumn.Description)
                                                    {
                                                        if (prtIdTemp != 0)
                                                        {
                                                            var prcfromprtId = cbService.LstPRC(userlogin.Company_ID, prtIdTemp);
                                                            if (prcfromprtId != null)
                                                            {
                                                                var prcDescription = prcfromprtId.Where(w => w.Text.ToLower().Trim() == worksheet_1.Cells[i, j].Value.ToString().ToLower().Trim() || w.Desc.ToLower().Trim() == worksheet_1.Cells[i, j].Value.ToString().ToLower().Trim()).FirstOrDefault();
                                                                if (prcDescription == null)
                                                                {
                                                                    model.validated_Main = false;
                                                                    prm_prd.Validate = false;
                                                                    err_.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower + " " + Resource.Please_Recheck_Lower + " " + Resource.Allowance_Type + " " + Resource.Again_Lower + ".");
                                                                }
                                                                else
                                                                {
                                                                    prm_prd.PRC_ID_Description = NumUtil.ParseInteger(prcDescription.Value);
                                                                    prm_prd.PRC_ID_Description_ = prcDescription.Text;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                model.validated_Main = false;
                                                                prm_prd.Validate = false;
                                                                err_.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            model.validated_Main = false;
                                                            prm_prd.Validate = false;
                                                            err_.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Not_Found_Lower + " " + Resource.Please_Recheck_Lower + " " + Resource.Allowance_Type + " " + Resource.Again_Lower + ".");
                                                        }
                                                    }
                                                    else if (j == PayrollImportColumn.Donation_Type)
                                                    {
                                                        var prcDonation = prcDonationlst.Where(w => w.Text.ToLower().Trim() == worksheet_1.Cells[i, j].Value.ToString().ToLower().Trim() || w.Desc.ToLower().Trim() == worksheet_1.Cells[i, j].Value.ToString().ToLower().Trim()).FirstOrDefault();
                                                        if (prcDonation == null)
                                                        {
                                                            model.validated_Main = false;
                                                            prm_prd.Validate = false;
                                                            err_.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Invalid_Lower);
                                                        }
                                                        else
                                                        {
                                                            if (prtDonationID != null)
                                                                prm_prd.PRT_ID_Donation = NumUtil.ParseInteger(prtDonationID.Value);

                                                            prm_prd.PRC_ID_Donation = NumUtil.ParseInteger(prcDonation.Value);
                                                            prm_prd.PRC_ID_Donation_ = prcDonation.Text;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    // Validate require
                                                    if (j == PayrollImportColumn.Employee_No
                                                        || j == PayrollImportColumn.Process_Year
                                                        || j == PayrollImportColumn.Process_Month
                                                        || j == PayrollImportColumn.Run_Date
                                                        || j == PayrollImportColumn.Total_Work_Days
                                                        || j == PayrollImportColumn.Payment_Type)
                                                    {
                                                        model.validated_Main = false;
                                                        prm_prd.Validate = false;
                                                        err_.AppendLine(Resource.The + " " + columnName + " " + Resource.Field + " " + Resource.Is_Rrequired_Lower);

                                                    }
                                                }
                                            }
                                            if (isempty)
                                            {
                                                model.validated_Main = false;
                                                prm_prd.Validate = false;
                                                err_.AppendLine(Resource.Message_Empty_Row);
                                            }
                                            prm_prd.ErrMsg = err_.ToString();
                                            prm_prd.Create_By = userlogin.User_Authentication.Email_Address;
                                            prm_prd.Create_On = currentdate;
                                            prm_prds.Add(prm_prd);
                                        }
                                        model.prm_prd = prm_prds.ToArray();
                                    }
                                }
                                else
                                    model.prm_prd = new List<ImportPRM_PRD_>().ToArray();
                            }
                        }
                    }
                    catch
                    {
                        ModelState.AddModelError("Import_Payroll", Resource.Message_Cannot_Found_Excel_Sheet + " " + Resource.Message_Please_Edit_Reupload);
                        model.prm_prd = new List<ImportPRM_PRD_>().ToArray();
                    }
                }
            }
            return View(model);
        }

        #endregion

        #region "Manual Payslip"
        [HttpPost]
        public ActionResult Manual_Payslip(PayrollViewModels model, string process = "")
        {
            var companyID = model.Company_ID; //Change company ID
            var prmID = model.PRM_ID; //Change prmID

            var userlogin = UserSession.getUser(HttpContext);
            var payrollService = new PayrollService();
            var empHistService = new EmploymentHistoryService();
            var empService = new EmployeeService();
            var companyService = new CompanyService();
            var bankinfoservice = new BankInfoService();
            var lService = new LeaveService();
            var currentdate = StoredProcedure.GetCurrentDate();

            Employee_Profile defaultemp = null;

            var htmlToConvert = new List<string>();
            var prm = payrollService.GetPayroll(prmID);
            var emp = prm.Employee_Profile;
            model = PayrollPrintSlip(model, prm, emp);

            var cService = new CompanyService();
            var cc = cService.GetCompany(model.Company_ID);
            model.Company_Currency_Code = cc.Currency.Currency_Code;
            model.Company_Currency_ID = cc.Currency.Currency_ID;
            model.Company_Currency_Name = cc.Currency.Currency_Name;
            model.Company_Name = cc.Name;
            defaultemp = emp;
            htmlToConvert.Add(RenderPartialViewAsString("PayrollSlip", model));

            if (defaultemp == null || htmlToConvert.Count() == 0)
            {
            }
            else
            {
                Response.ContentType = "application/pdf";
                var sr = new StringReader(htmlToConvert[0]);
                var pdfDoc = new Document(PageSize.A4);
                var memoryStream = new MemoryStream();
                var htmlparser = new HTMLWorker(pdfDoc);

                htmlparser.SetStyleSheet(GenerateStyleSheet("SG"));

                var writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                writer.SetEncryption(PdfWriter.STRENGTH128BITS, defaultemp.NRIC, defaultemp.NRIC, PdfWriter.AllowCopy | PdfWriter.ALLOW_PRINTING | PdfWriter.ALLOW_SCREENREADERS);

                var pageevent = new PDFPageEvent();
                pageevent.PrintTime = currentdate;
                pageevent.CountryName = "SG";

                var logo = companyService.GetLogo(companyID);
                if (logo != null && logo.Logo != null)
                    pageevent.Logoleft = logo.Logo;

                writer.PageEvent = pageevent;

                pdfDoc.Open();
                htmlparser.Parse(sr);
                writer.CloseStream = false;
                pdfDoc.Close();
                memoryStream.Position = 0;
                //var user = defaultemp.User_Profile;
                var uService = new UserService();
                var user = uService.getUserByEmployeeProfile(model.Employee_Profile_ID);
                user.Name = user.First_Name + " " + user.Middle_Name + " " + user.Last_Name;
                EmailTemplete.sendPayslipEmail("moet@bluecube.com.sg", user.Name, "Payroll", "", memoryStream);
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Manual_Payslip(ServiceResult result, string pStatus, string pPrmID, string pEmpID, string pMonth, string pYear)
        {
            var userlogin = UserSession.getUser(HttpContext);
            if (userlogin == null || userlogin.Company_ID != 1)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            //var cSerevice = new ComboService();
            //var payrollService = new PayrollService();

            //var employeeService = new EmployeeService();
            //var empHistService = new EmploymentHistoryService();
            //var companyService = new CompanyService();
            //var lService = new LeaveService();
            //var bankinfoservice = new BankInfoService();

            //var statue = EncryptUtil.Decrypt(pStatus);
            //var empID = 1736; //NumUtil.ParseInteger(EncryptUtil.Decrypt(pEmpID));
            //var month = 10; //NumUtil.ParseInteger(EncryptUtil.Decrypt(pMonth));
            //var year  = 2016; //NumUtil.ParseInteger(EncryptUtil.Decrypt(pYear));
            //var prmID = 2936; //NumUtil.ParseInteger(EncryptUtil.Decrypt(pPrmID));
            //var Company_ID = 58;

            var model = new PayrollViewModels();
            return View(model);
        }
        #endregion

        #region Payslip

        [HttpGet]
        public ActionResult Payslip(ServiceResult result, PayslipViewModels model, int Search_Month = 0, int Search_Year = 0, int[] PRM_Ids = null, string tabAction = "")
        {

            var userlogin = UserSession.getUser(HttpContext);
            var cSerevice = new ComboService();
            var employeeService = new EmployeeService();

            var EmpID = employeeService.GetEmployeeProfileByProfileID(userlogin.Profile_ID);
            var currentdate = StoredProcedure.GetCurrentDate();

            var rightResult = base.validatePageRight(UserSession.RIGHT_A, "/Payroll/Payslip");
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;
            model.result = result;

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
            model.prmlist = (new PayrollService()).LstPayrollByEmpID(EmpID.Employee_Profile_ID, model.Search_Month, model.Search_Year);

            return View(model);

        }

        public void PayrollPrintSlip(string PRM_ID = null, string EMP_ID = null, int[] PRM_Ids = null, string tabAction = "", bool sendmail = false)
        {
            var prmID = NumUtil.ParseInteger(EncryptUtil.Decrypt(PRM_ID));
            var empID = NumUtil.ParseInteger(EncryptUtil.Decrypt(EMP_ID));
            PayrollPrintSlip(prmID, empID, PRM_Ids, tabAction, sendmail);
        }

        private void PayrollPrintSlip(int? pPrmID = null, int? pEmpID = null, int[] PRM_Ids = null, string tabAction = "", bool sendmail = false)
        {
            var model = new PayrollViewModels();
            var userlogin = UserSession.getUser(HttpContext);
            var payrollService = new PayrollService();
            var empHistService = new EmploymentHistoryService();
            var empService = new EmployeeService();
            var companyService = new CompanyService();
            var bankinfoservice = new BankInfoService();
            var lService = new LeaveService();
            var currentdate = StoredProcedure.GetCurrentDate();

            model.leaveAmountList = new List<decimal>();
            var company = companyService.GetCompany(userlogin.Company_ID);
            if (company != null && company.Currency != null)
            {
                model.Company_Currency_Code = company.Currency.Currency_Code;
                model.Company_Currency_Name = company.Currency.Currency_Name;
                model.Company_Currency_ID = company.Currency_ID.Value;
                model.Company_Name = company.Name;
            }

            var prmIDs = new List<int>();
            if (PRM_Ids != null && PRM_Ids.Length > 0)
                prmIDs = PRM_Ids.ToList();
            else if (pPrmID.HasValue && pPrmID.Value > 0)
                prmIDs.Add(pPrmID.Value);
            else if (pEmpID.HasValue && pEmpID.Value > 0)
            {
                var emp = empService.GetEmployeeProfile2(pEmpID.Value);
                if (emp != null)
                {
                    var prm = payrollService.GetPayrollByEmployeeID(pEmpID, currentdate.Month, currentdate.Year);
                    if (prm != null)
                        prmIDs.Add(prm.PRM_ID);
                }
            }

            Employee_Profile defaultemp = null;
            var htmlToConvert = new List<string>();
            foreach (var id in prmIDs)
            {
                var prm = payrollService.GetPayroll(id);
                if (prm == null)
                    continue;

                var emp = prm.Employee_Profile;
                if (emp == null)
                    continue;

                model = PayrollPrintSlip(model, prm, emp);
                defaultemp = emp;
                htmlToConvert.Add(RenderPartialViewAsString("PayrollSlip", model));
            }

            if (sendmail)
            {
                if (defaultemp == null || htmlToConvert.Count() == 0)
                    return;

                Response.ContentType = "application/pdf";
                var sr = new StringReader(htmlToConvert[0]);
                var pdfDoc = new Document(PageSize.A4);
                var memoryStream = new MemoryStream();
                var htmlparser = new HTMLWorker(pdfDoc);


                var writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                writer.SetEncryption(PdfWriter.STRENGTH128BITS, defaultemp.NRIC, defaultemp.NRIC, PdfWriter.AllowCopy | PdfWriter.ALLOW_PRINTING | PdfWriter.ALLOW_SCREENREADERS);

                var pageevent = new PDFPageEvent();
                pageevent.PrintTime = currentdate;

                var logo = companyService.GetLogo(company.Company_ID);
                if (logo != null && logo.Logo != null)
                    pageevent.Logoleft = logo.Logo;

                if (company.Country_ID.HasValue)
                {
                    var country = companyService.GetCountry(company.Country_ID);
                    if (country != null)
                    {
                        if (country.Name != null)
                        {
                            htmlparser.SetStyleSheet(GenerateStyleSheet(country.Name));
                            pageevent.CountryName = country.Name;
                        }
                    }
                }

                writer.PageEvent = pageevent;

                pdfDoc.Open();
                htmlparser.Parse(sr);
                writer.CloseStream = false;
                pdfDoc.Close();
                memoryStream.Position = 0;
                var user = defaultemp.User_Profile;
                EmailTemplete.sendPayslipEmail(user.User_Authentication.Email_Address, UserSession.GetUserName(user), "Payroll", "", memoryStream);
            }
            else
            {
                var workStream = new MemoryStream();
                //StreamReader reader = new StreamReader(workStream, System.Text.Encoding.UTF8,true);
                using (var archive = new ZipArchive(workStream, ZipArchiveMode.Create, true))
                {
                    var file = archive.CreateEntry("PayrollSlip.pdf", System.IO.Compression.CompressionLevel.Optimal);
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

                        var pageevent = new PDFPageEvent();
                        pageevent.PrintTime = currentdate;
                        var logo = companyService.GetLogo(company.Company_ID);
                        if (logo != null && logo.Logo != null)
                            pageevent.Logoleft = logo.Logo;

                        if (company.Country_ID.HasValue)
                        {
                            var country = companyService.GetCountry(company.Country_ID);
                            if (country != null)
                            {
                                if (country.Name != null)
                                {
                                    htmlparser.SetStyleSheet(GenerateStyleSheet(country.Name));
                                    pageevent.CountryName = country.Name;
                                }
                            }
                        }
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
                        //Response.End();
                    }
                }

                if (tabAction == "zip")
                {
                    Response.Clear();
                    Response.ClearContent();
                    Response.ClearHeaders();
                    Response.ContentType = "application/zip";
                    Response.Charset = Encoding.UTF8.ToString();
                    Response.HeaderEncoding = Encoding.UTF8;
                    Response.ContentEncoding = Encoding.UTF8;
                    Response.Buffer = false;
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "PayrollSlip.zip"));
                    workStream.Seek(0, SeekOrigin.Begin);
                    workStream.CopyTo(Response.OutputStream);
                    Response.End();
                }
            }

        }


        private PayrollViewModels PayrollPrintSlip(PayrollViewModels model, PRM prm, Employee_Profile emp)
        {
            var userlogin = UserSession.getUser(HttpContext);
            var payrollService = new PayrollService();
            var empHistService = new EmploymentHistoryService();
            var effectivedate = DateUtil.ToDate(1, prm.Process_Month.Value, prm.Process_Year.Value);



            model.Name = UserSession.GetUserName(emp.User_Profile);
            model.Employee_No = emp.Employee_No;
            model.Employee_Profile_ID = emp.Employee_Profile_ID;

            model.Run_Date = DateUtil.ToDisplayDate(prm.Run_date);
            model.Payment_Type = prm.Payment_Type;
            if (prm.Global_Lookup_Data != null)
                model.Payment_Type_Name = prm.Global_Lookup_Data.Name;

            model.Cheque_No = prm.Cheque_No;
            model.Total_Work_Days = prm.Total_Work_Days;
            model.PRM_ID = prm.PRM_ID;

            model.Leave_Period_From = DateUtil.ToDisplayDate(prm.Leave_Period_From);
            model.Leave_Period_To = DateUtil.ToDisplayDate(prm.Leave_Period_to);
            model.Expenses_Period_From = DateUtil.ToDisplayDate(prm.Expenses_Period_From);
            model.Expenses_Period_To = DateUtil.ToDisplayDate(prm.Expenses_Period_to);

            model.expensesList = payrollService.GetExpenseApplications(prm.PRM_ID);
            model.leaveList = payrollService.GetLeaveApplicationDocument(prm.PRM_ID);

            model.Allowance_Rows = PayrollConvertPRD(payrollService.LstPRD(prm.PRM_ID, PayrollAllowanceType.Allowance_Deduction));
            model.Overtime_Rows = PayrollConvertPRD(payrollService.LstPRD(prm.PRM_ID, PayrollAllowanceType.Overtime));
            model.Extra_Donation_Rows = PayrollConvertPRD(payrollService.LstPRD(prm.PRM_ID, PayrollAllowanceType.Donation));

            model.Process_Month = prm.Process_Month.Value;
            model.Process_Year = prm.Process_Year.Value;
            model.Selected_CPF_Formula_ID = prm.Selected_CPF_Formula_ID;
            model.Selected_OT_Formula_ID = prm.Selected_OT_Formula_ID;
            model.Employee_Contribution = prm.CPF_Employee.HasValue ? prm.CPF_Employee.Value : 0;
            model.Employer_Contribution = prm.CPF_Emplyer.HasValue ? prm.CPF_Emplyer.Value : 0;
            model.Bonus_Issue = prm.Bonus_Issue.HasValue ? prm.Bonus_Issue.Value : false;
            model.Total_Bonus = prm.Total_Bonus.HasValue ? prm.Total_Bonus.Value : model.Total_Bonus;
            model.Bonus_Amount = model.Total_Bonus;
            model.Net_Salary = prm.Nett_Wages.HasValue ? prm.Nett_Wages.Value : 0;
            model.Gross_Salary = prm.Gross_Wages.HasValue ? prm.Gross_Wages.Value : 0;
            model.Total_Allowance = prm.Total_Allowance.HasValue ? prm.Total_Allowance.Value : 0;
            model.Total_Deduction = prm.Total_Deduction.HasValue ? prm.Total_Deduction.Value : 0;
            model.Donation = prm.Donation.HasValue ? prm.Donation.Value : 0;
            model.Extra_Donation = prm.Total_Extra_Donation.HasValue ? prm.Total_Extra_Donation.Value : 0;
            model.Payment_Type = prm.Payment_Type;
            model.No_Of_Hours = prm.No_Of_Hours.HasValue ? prm.No_Of_Hours.Value : 0;
            model.Hourly_Rate = prm.Hourly_Rate.HasValue ? prm.Hourly_Rate.Value : 0;
            //Added by sun 09-02-2016
            model.Total_Allowance_Basic_Salary = prm.Total_Allowance_Basic_Salary.HasValue ? prm.Total_Allowance_Basic_Salary.Value : 0;


            if (prm.Process_Date_From.HasValue)
                model.Process_Date_From = DateUtil.ToDisplayDate(prm.Process_Date_From);
            else
                model.Process_Date_From = DateUtil.ToDisplayDate(DateUtil.ToDate(1, model.Process_Month, model.Process_Year));

            if (prm.Process_Date_To.HasValue)
                model.Process_Date_To = DateUtil.ToDisplayDate(prm.Process_Date_To);
            else
                model.Process_Date_To = DateUtil.ToDisplayDate(DateUtil.ToDate(DateTime.DaysInMonth(model.Process_Year, model.Process_Month), model.Process_Month, model.Process_Year));

            if (prm.Selected_Donation_Formula != null && prm.Selected_Donation_Formula.Donation_Formula != null)
                model.Donation_Label = prm.Selected_Donation_Formula.Donation_Formula.Formula_Name;
            var cbService = new ComboService();
            model.ExpensesLst = cbService.LstExpensesType(userlogin.Company_ID.Value);
            model.LeaveLst = cbService.LstLeaveType(userlogin.Company_ID.Value);


            var empHist = empHistService.GetPayrollEmploymentHistory(emp.Employee_Profile_ID, effectivedate.Value);
            if (empHist != null)
            {
                //Edit by sun 09-02-2016
                var total_salary = 0M;
                if (prm.Basic_Salary.HasValue && prm.Basic_Salary.Value > 0)
                    total_salary = prm.Basic_Salary.Value;

                if (prm.Total_Allowance_Basic_Salary.HasValue && prm.Total_Allowance_Basic_Salary.Value > 0)
                    total_salary += prm.Total_Allowance_Basic_Salary.Value;

                if (total_salary > 0)
                    model.Basic_Salary = total_salary;
                else
                {
                    if (empHist.Basic_Salary_Unit == Term.Hourly)
                        model.Basic_Salary = model.Hourly_Rate * (prm.No_Of_Hours.HasValue ? prm.No_Of_Hours.Value : 0);
                    else
                    {
                        model.Basic_Salary = NumUtil.ParseDecimal(EncryptUtil.Decrypt(empHist.Basic_Salary));
                        if (model.Basic_Salary == 0)
                            model.Basic_Salary = NumUtil.ParseDecimal(EncryptUtil.Decrypt(EncryptUtil.Decrypt(empHist.Basic_Salary)));
                    }
                }
                model.Department = empHist.Department.Name;
            }
            return model;
        }
        #endregion

        #region IR8A
        [HttpGet]
        public ActionResult IR8A(string operation, string pEmpID, string pIRID, int sYear = 0, int sDepartment = 0)
        {
            IR8AViewModel model = new IR8AViewModel();
            if (string.IsNullOrEmpty(operation))
                operation = EncryptUtil.Encrypt(UserSession.RIGHT_A);

            model.operation = EncryptUtil.Decrypt(operation);

            var empID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pEmpID));
            model.Employee_Profile_ID = empID;
            model.iid = NumUtil.ParseInteger(EncryptUtil.Decrypt(pIRID));

            var rightResult = base.validatePageRight(model.operation);
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;

            var userlogin = UserSession.getUser(HttpContext);
            var currentdate = StoredProcedure.GetCurrentDate();
            var payrollService = new PayrollService();
            var histService = new EmploymentHistoryService();
            var empService = new EmployeeService();

            if (sYear == 0)
                model.sYear = currentdate.Year;
            else
                model.sYear = sYear;

            if (model.operation == UserSession.RIGHT_A)
            {
                model.employeeList = new LeaveService().getEmployeeListAll(userlogin.Company_ID);
                model.etira8 = payrollService.GetETIRA8s(userlogin.Company_ID);

                model.departmentList = new ComboService().LstDepartment(userlogin.Company_ID, true);
                model.sDepartment = sDepartment;

                model.sYearList = new List<int>();
                for (int i = 2014; i <= currentdate.Year + 1; i++)
                {
                    model.sYearList.Add(i);
                }
            }
            else
            {
                model.nationalityList = new ComboService().LstNationality(false);
                model.genderList = new ComboService().LstLookup(ComboType.Gender, userlogin.Company_ID, false);

                if (model.operation == UserSession.RIGHT_C)
                {
                    if (!model.Employee_Profile_ID.HasValue || model.Employee_Profile_ID.Value == 0)
                        return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

                    model.Employee_Profile = empService.GetEmployeeProfile(model.Employee_Profile_ID);
                    if (model.Employee_Profile == null)
                        return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

                    model.Run_Date = DateUtil.ToDisplayDate(currentdate);

                    if (model.Employee_Profile == null || model.Employee_Profile.User_Profile.Company_ID != userlogin.Company_ID)
                        return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

                    var bankinfo = model.Employee_Profile.Banking_Info.Where(w => w.Selected == true).FirstOrDefault();
                    if (bankinfo != null)
                        model.Bank_Name = bankinfo.Bank_Name;

                    model.company = new CompanyService().GetCompany(userlogin.Company_ID);

                    var emphist = histService.GetCurrentEmploymentHistory(model.Employee_Profile_ID);
                    if (emphist != null && emphist.Designation != null)
                        model.Designation_Name = emphist.Designation.Name;

                    model.employer = empService.GetEmployeeProfile(userlogin.Employee_Profile.FirstOrDefault().Employee_Profile_ID);
                    if (model.employer != null)
                    {
                        var employerhist = histService.GetCurrentEmploymentHistory(model.employer.Employee_Profile_ID);
                        if (employerhist != null && employerhist.Designation != null)
                            model.employer_Desination_Name = employerhist.Designation.Name;
                    }

                    var expensesTypeList = new ExpenseService().getExpenseTypes(userlogin.Company_ID);
                    int medtype = 0;
                    int entype = 0;
                    int trantype = 0;
                    if (expensesTypeList != null)
                    {
                        var mclaim = expensesTypeList.Where(w => w.Expenses_Name.Contains("Medical")).FirstOrDefault();
                        if (mclaim != null)
                            medtype = mclaim.Expenses_Config_ID;

                        var ent = expensesTypeList.Where(w => w.Expenses_Name.Contains("Entertainment")).FirstOrDefault();
                        if (ent != null)
                            entype = ent.Expenses_Config_ID;

                        var tran = expensesTypeList.Where(w => w.Expenses_Name.Contains("Transport")).FirstOrDefault();
                        if (tran != null)
                            trantype = tran.Expenses_Config_ID;
                    }


                    //Get Employee's PRM 
                    List<PRM> prms = payrollService.GetPRMEmployee(model.Employee_Profile_ID);
                    if (prms != null && prms.Count > 0)
                    {
                        prms = prms.Where(w => w.Process_Year == model.sYear - 1 && w.Process_Status == PayrollStatus.Comfirm).ToList();
                        foreach (PRM prm in prms)
                        {
                            var prmemphist = payrollService.GetPRMEmpHist(prm.Employee_Profile_ID, prm.Process_Month.Value, prm.Process_Year.Value);
                            if (prmemphist != null)
                            {
                                //Edit by sun 09-02-2016
                                var total_salary = 0M;
                                if (prm.Basic_Salary.HasValue && prm.Basic_Salary.Value > 0)
                                    total_salary = prm.Basic_Salary.Value;

                                if (prm.Total_Allowance_Basic_Salary.HasValue && prm.Total_Allowance_Basic_Salary.Value > 0)
                                    total_salary += prm.Total_Allowance_Basic_Salary.Value;

                                if (total_salary > 0)
                                    model.Gross_Salary += total_salary;
                                else
                                {
                                    if (prmemphist.Basic_Salary_Unit == Term.Hourly)
                                    {
                                        var hours = prm.No_Of_Hours.HasValue ? prm.No_Of_Hours.Value : 0;
                                        var rate = prm.Hourly_Rate.HasValue ? prm.Hourly_Rate.Value : 0;
                                        model.Gross_Salary += (rate * hours);
                                    }
                                    else
                                    {
                                        var salaryamount = NumUtil.ParseDecimal(EncryptUtil.Decrypt(emphist.Basic_Salary));
                                        if (salaryamount == 0)
                                            salaryamount = NumUtil.ParseDecimal(EncryptUtil.Decrypt(EncryptUtil.Decrypt((emphist.Basic_Salary))));
                                        model.Gross_Salary += salaryamount;
                                    }
                                }

                                //if (prmemphist.Basic_Salary_Unit == Term.Hourly)
                                //{
                                //   var hours = prm.No_Of_Hours.HasValue ? prm.No_Of_Hours.Value : 0;
                                //   var rate = prm.Hourly_Rate.HasValue ? prm.Hourly_Rate.Value : 0;
                                //   model.Gross_Salary += (rate * hours);
                                //}
                                //else
                                //{
                                //   var salaryamount = NumUtil.ParseDecimal(EncryptUtil.Decrypt(emphist.Basic_Salary));
                                //   if (salaryamount == 0)
                                //      salaryamount = NumUtil.ParseDecimal(EncryptUtil.Decrypt(EncryptUtil.Decrypt((emphist.Basic_Salary))));
                                //   model.Gross_Salary += salaryamount;
                                //}
                            }

                            if (prm.PRDs != null && prm.PRDs.Count > 0)
                            {
                                foreach (PRD d in prm.PRDs)
                                {

                                    if (d.PRC != null)
                                    {
                                        if (d.PRC.Description == "CDAC")
                                        {
                                            model.Donation += d.Amount.Value;
                                            model.CDAC = "on";
                                        }
                                        else if (d.PRC.Description == "Yayasan Mendaki Fund")
                                        {
                                            model.Donation += d.Amount.Value;
                                            model.Yayasan_Mendaki_Fund = "on";
                                        }
                                        else if (d.PRC.Description == "Community Chest of Singapore")
                                        {
                                            model.Donation += d.Amount.Value;
                                            model.Community_chest_of_Singapore = "on";
                                        }
                                        else if (d.PRC.Description == "SINDA")
                                        {
                                            model.Donation += d.Amount.Value;
                                            model.SINDA = "on";
                                        }
                                        else if (d.PRC.Description == "ECF")
                                        {
                                            model.Donation += d.Amount.Value;
                                            model.ECF = "on";
                                        }
                                        else if (d.PRC.Description == "Other")
                                        {
                                            model.Donation += d.Amount.Value;
                                            model.Other_tax_exempt_donations = "on";
                                        }
                                        else if (d.PRC.Description == "Bonus")
                                        {
                                            model.Bonus += d.Amount.Value;
                                        }
                                        else if (d.PRC.Description == "Commission")
                                        {
                                            model.Commission_Amount += d.Amount.Value;
                                        }
                                        else if (d.PRC.Description == "Director Fee")
                                        {
                                            model.Director_Fee += d.Amount.Value;
                                        }
                                    }
                                }
                            }
                            model.Employee_CPF += prm.CPF_Employee.HasValue ? prm.CPF_Employee.Value : 0;
                        }
                    }


                    var firstHist = histService.GetFirstEmploymentHistory(model.Employee_Profile_ID);
                    if (firstHist != null && firstHist.Effective_Date.HasValue && firstHist.Effective_Date.Value.Year == model.sYear - 1)
                        model.Commission_Start = DateUtil.ToDisplayDate(firstHist.Effective_Date);
                    else
                        model.Commission_Start = "1/1/" + (model.sYear - 1).ToString();

                    if (firstHist != null && firstHist.Effective_Date.HasValue)
                        model.Hired_Date = DateUtil.ToDisplayDate(firstHist.Effective_Date);

                    model.Commission_End = "31/12/" + (model.sYear - 1).ToString();

                    List<Expenses_Application_Document> expenses = new ExpenseService().getExpenseApplicationDocs(model.Employee_Profile.Profile_ID);
                    ///Filter Year

                    if (expenses != null && expenses.Count > 0)
                    {
                        expenses = expenses.Where(w => w.Date_Applied.Value.Year == model.sYear - 1 && w.Overall_Status == WorkflowStatus.Closed).ToList();

                        foreach (Expenses_Application_Document exp in expenses.Where(w => w.Expenses_Config_ID == entype).ToList())
                        {
                            model.Allowance_Entertain += exp.Amount_Claiming.Value;
                        }

                        foreach (Expenses_Application_Document exp in expenses.Where(w => w.Expenses_Config_ID == trantype).ToList())
                        {
                            model.Allowance_Transport += exp.Amount_Claiming.Value;
                        }

                        foreach (Expenses_Application_Document exp in expenses.Where(w => w.Expenses_Config_ID != trantype && w.Expenses_Config_ID != entype).ToList())
                        {
                            model.Allowance_Others += exp.Amount_Claiming.Value;
                        }
                    }

                }
                else if (model.operation == UserSession.RIGHT_U)
                {
                    if (model.iid == 0)
                        return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

                    var eir8a = payrollService.GetETIRA8(model.iid);
                    if (eir8a == null)
                        return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

                    var firstHist = histService.GetFirstEmploymentHistory(eir8a.Employee_Profile_ID);
                    if (firstHist != null && firstHist.Effective_Date.HasValue)
                        model.Hired_Date = DateUtil.ToDisplayDate(firstHist.Effective_Date);

                    model.Run_Date = DateUtil.ToDisplayDate(eir8a.Create_On);
                    model.P_YEAR = eir8a.P_YEAR;
                    model.Date_of_Cessation = DateUtil.ToDisplayDate(eir8a.Date_of_Cessation);
                    model.Gross_Salary = eir8a.Gross_Salary.HasValue ? eir8a.Gross_Salary.Value : 0;
                    model.Bonus = eir8a.Bonus.HasValue ? eir8a.Bonus.Value : 0;
                    model.Director_Fee = eir8a.Director_Fee.HasValue ? eir8a.Director_Fee.Value : 0;
                    model.Allowance_Transport = eir8a.Allowance_Transport.HasValue ? eir8a.Allowance_Transport.Value : 0;
                    model.Allowance_Entertain = eir8a.Allowance_Entertain.HasValue ? eir8a.Allowance_Entertain.Value : 0;
                    model.Allowance_Others = eir8a.Allowance_Others.HasValue ? eir8a.Allowance_Others.Value : 0;
                    model.Commission_Start = DateUtil.ToDisplayDate(eir8a.Commission_Start);
                    model.Commission_End = DateUtil.ToDisplayDate(eir8a.Commission_End);
                    model.Commission_Amount = eir8a.Commission_Amount.HasValue ? eir8a.Commission_Amount.Value : 0;
                    model.Commission_Type = eir8a.Commission_Type.HasValue ? eir8a.Commission_Type.Value : 0;
                    model.Pension = eir8a.Pension.HasValue ? eir8a.Pension.Value : 0;
                    model.Gratuity = eir8a.Gratuity.HasValue ? eir8a.Gratuity.Value : 0;
                    model.Notice_Pay = eir8a.Notice_Pay.HasValue ? eir8a.Notice_Pay.Value : 0;
                    model.Ex_Gratia = eir8a.Ex_Gratia.HasValue ? eir8a.Ex_Gratia.Value : 0;
                    model.Lump_Sum_Others = eir8a.Total_Lump_Sum.HasValue ? eir8a.Lump_Sum_Others.Value : 0;
                    model.Nature = eir8a.Nature;
                    model.Compensation_Loss = eir8a.Compensation_Loss.HasValue ? eir8a.Compensation_Loss.Value : 0;
                    model.Approval_IRAS = eir8a.Approval_IRAS.HasValue ? eir8a.Approval_IRAS.Value : false;
                    model.Date_Approval = DateUtil.ToDisplayDate(eir8a.Date_Approval);
                    model.Reason_Payment = eir8a.Reason_Payment;
                    model.Length_Service = eir8a.Length_Service.HasValue ? eir8a.Length_Service.Value : 0;
                    model.Basis_Payment = eir8a.Basis_Payment;
                    model.Total_Lump_Sum = eir8a.Total_Lump_Sum.HasValue ? eir8a.Total_Lump_Sum.Value : 0;
                    model.Retirement_Pension = eir8a.Retirement_Pension;
                    model.Amount_Accured_1992 = eir8a.Amount_Accured_1992.HasValue ? eir8a.Amount_Accured_1992.Value : 0;
                    model.Amount_Accured_1993 = eir8a.Amount_Accured_1993.HasValue ? eir8a.Amount_Accured_1993.Value : 0;
                    model.Contribution_Out_Singapore = eir8a.Contribution_Out_Singapore.HasValue ? eir8a.Contribution_Out_Singapore.Value : 0;
                    model.Excess_Employer = eir8a.Excess_Employer.HasValue ? eir8a.Excess_Employer.Value : 0;
                    model.Excess_Less = eir8a.Excess_Less.HasValue ? eir8a.Excess_Less.Value : 0;
                    model.Gain_Profit = eir8a.Gain_Profit.HasValue ? eir8a.Gain_Profit.Value : 0;
                    model.Value_Benefit = eir8a.Value_Benefit.HasValue ? eir8a.Value_Benefit.Value : 0;
                    model.Income_Tax_Employer = eir8a.Income_Tax_Employer.HasValue ? eir8a.Income_Tax_Employer.Value : false;
                    model.Tax_Partially = eir8a.Tax_Partially.HasValue ? eir8a.Tax_Partially.Value : 0;
                    model.Tax_Fixed = eir8a.Tax_Fixed.HasValue ? eir8a.Tax_Fixed.Value : 0;
                    model.Employee_CPF = eir8a.Employee_CPF.HasValue ? eir8a.Employee_CPF.Value : 0;
                    model.Name_CPF = eir8a.Name_CPF;
                    model.Donation = eir8a.Donation.HasValue ? eir8a.Donation.Value : 0;
                    model.Contribution_Mosque = eir8a.Contribution_Mosque.HasValue ? eir8a.Contribution_Mosque.Value : 0;
                    model.Life_Insurance = eir8a.Life_Insurance.HasValue ? eir8a.Life_Insurance.Value : 0;
                    model.Yayasan_Mendaki_Fund = eir8a.Yayasan_Mendaki_Fund.HasValue ? (eir8a.Yayasan_Mendaki_Fund.Value ? "on" : "off") : "off";
                    model.SINDA = eir8a.SINDA.HasValue ? (eir8a.SINDA.Value ? "on" : "off") : "off";
                    model.ECF = eir8a.ECF.HasValue ? (eir8a.ECF.Value ? "on" : "off") : "off";
                    model.Community_chest_of_Singapore = eir8a.Community_chest_of_Singapore.HasValue ? (eir8a.Community_chest_of_Singapore.Value ? "on" : "off") : "off";
                    model.CDAC = eir8a.CDAC.HasValue ? (eir8a.CDAC.Value ? "on" : "off") : "off";
                    model.Other_tax_exempt_donations = eir8a.Other_tax.HasValue ? (eir8a.Other_tax.Value ? "on" : "off") : "off";
                    model.Bank_Name = eir8a.Bank_Name;

                    model.Employee_Profile_ID = eir8a.Employee_Profile_ID;
                    model.iid = eir8a.ETIRA8_ID;
                    model.Contribution_With_Name = eir8a.Contribution_With_Name;
                    model.Contribution_With_Establishment = eir8a.Contribution_With_Establishment;
                    model.Contribution_With_Madatory = eir8a.Contribution_With_Madatory;
                    model.Contribution_With_Singapore = eir8a.Contribution_With_Singapore.HasValue ? eir8a.Contribution_With_Singapore.Value : 0;
                    model.Remission_Income = eir8a.Remission_Income.HasValue ? eir8a.Remission_Income.Value : 0;
                    model.Non_Tax_Income = eir8a.Non_Tax_Income.HasValue ? eir8a.Non_Tax_Income.Value : 0;

                    model.Employee_Profile = empService.GetEmployeeProfile(eir8a.Employee_Profile_ID);

                    if (model.Employee_Profile == null || model.Employee_Profile.User_Profile.Company_ID != userlogin.Company_ID)
                        return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

                    var emphist = histService.GetCurrentEmploymentHistory(model.Employee_Profile_ID);
                    if (emphist != null && emphist.Designation != null)
                        model.Designation_Name = emphist.Designation.Name;

                    model.company = new CompanyService().GetCompany(userlogin.Company_ID);

                    User_Profile u = new UserService().getUser(eir8a.Employer.HasValue ? eir8a.Employer.Value : 0);
                    if (u == null)
                        u = new UserService().getUserProfile(eir8a.Create_By);

                    if (u != null && u.Employee_Profile != null && u.Employee_Profile.Count > 0)
                        model.employer = empService.GetEmployeeProfile(u.Employee_Profile.FirstOrDefault().Employee_Profile_ID);
                    else
                        model.employer = empService.GetEmployeeProfile(userlogin.Employee_Profile.FirstOrDefault().Employee_Profile_ID);

                    if (model.employer == null)
                        return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

                    var employerhist = histService.GetCurrentEmploymentHistory(model.employer.Employee_Profile_ID);
                    if (employerhist != null && employerhist.Designation != null)
                        model.employer_Desination_Name = employerhist.Designation.Name;

                }
            }

            if (model.operation == UserSession.RIGHT_A)
                return View(model);
            else
            {
                if (model.sYear == 2015)
                    return View("IR8A2014", model);
                else if (model.sYear == 2016)
                    return View("IR8A2015", model);
                else if (model.sYear == 2017)
                    return View("IR8A2016", model);
                else if (model.sYear == 2018)
                    return View("IR8A2017", model);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult IR8A(IR8AViewModel model)
        {
            var rightResult = base.validatePageRight(model.operation);
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;

            var userlogin = UserSession.getUser(HttpContext);
            var currentdate = StoredProcedure.GetCurrentDate();
            var payrollService = new PayrollService();

            var empService = new EmployeeService();
            var cbService = new ComboService();
            model.employeeList = new LeaveService().getEmployeeList(userlogin.Company_ID.Value);
            model.nationalityList = cbService.LstNationality(false);
            model.Employee_Profile = empService.GetEmployeeProfile(model.Employee_Profile_ID);
            model.genderList = cbService.LstLookup(ComboType.Gender, userlogin.Company_ID, false);
            model.company = new CompanyService().GetCompany(userlogin.Company_ID);
            model.employer = empService.GetEmployeeProfile(userlogin.Employee_Profile.FirstOrDefault().Employee_Profile_ID);
            model.departmentList = cbService.LstDepartment(userlogin.Company_ID, true);

            if (model.operation == UserSession.RIGHT_C)
            {
                if (!model.Employee_Profile_ID.HasValue || model.Employee_Profile_ID.Value == 0)
                    return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();
                    if (errors != null && errors.Count() > 0)
                    {

                        if (model.sYear == 2015)
                            return View("IR8A2014", model);
                        else if (model.sYear == 2016)
                            return View("IR8A2015", model);
                        else if (model.sYear == 2017)
                            return View("IR8A2016", model);
                        else if (model.sYear == 2018)
                            return View("IR8A2017", model);
                        return View(model);
                    }
                }

                model.etira8 = payrollService.GetETIRA8s(userlogin.Company_ID.Value);
                if (model.etira8.Where(w => w.Employee_Profile_ID == model.Employee_Profile_ID && w.P_YEAR == model.sYear.ToString()).ToList().Count > 0)
                    return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

                model.Employee_Profile = empService.GetEmployeeProfile(model.Employee_Profile_ID);
                if (model.Employee_Profile == null || model.Employee_Profile.User_Profile.Company_ID != userlogin.Company_ID)
                    return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

                var eir8a = new ETIRA8();
                eir8a.P_YEAR = model.sYear.ToString();
                eir8a.Date_of_Cessation = DateUtil.ToDate(model.Date_of_Cessation);
                eir8a.Gross_Salary = model.Gross_Salary;
                eir8a.Bonus = model.Bonus;
                eir8a.Director_Fee = model.Director_Fee;
                eir8a.Allowance_Transport = model.Allowance_Transport;
                eir8a.Allowance_Entertain = model.Allowance_Entertain;
                eir8a.Allowance_Others = model.Allowance_Others;
                eir8a.Commission_Start = DateUtil.ToDate(model.Commission_Start);
                eir8a.Commission_End = DateUtil.ToDate(model.Commission_End);
                eir8a.Commission_Amount = model.Commission_Amount;
                eir8a.Commission_Type = model.Commission_Type;
                eir8a.Pension = model.Pension;
                eir8a.Gratuity = model.Gratuity;
                eir8a.Notice_Pay = model.Notice_Pay;
                eir8a.Ex_Gratia = model.Ex_Gratia;
                eir8a.Lump_Sum_Others = model.Lump_Sum_Others;
                eir8a.Nature = model.Nature;
                eir8a.Compensation_Loss = model.Compensation_Loss;
                eir8a.Approval_IRAS = model.Approval_IRAS;
                eir8a.Date_Approval = DateUtil.ToDate(model.Date_Approval);
                eir8a.Reason_Payment = model.Reason_Payment;
                eir8a.Length_Service = model.Length_Service;
                eir8a.Basis_Payment = model.Basis_Payment;
                eir8a.Total_Lump_Sum = model.Total_Lump_Sum;
                eir8a.Retirement_Pension = model.Retirement_Pension;
                eir8a.Amount_Accured_1992 = model.Amount_Accured_1992;
                eir8a.Amount_Accured_1993 = model.Amount_Accured_1993;
                eir8a.Contribution_Out_Singapore = model.Contribution_Out_Singapore;
                eir8a.Excess_Employer = model.Excess_Employer;
                eir8a.Excess_Less = model.Excess_Less;
                eir8a.Gain_Profit = model.Value_Benefit;
                eir8a.Value_Benefit = model.Value_Benefit;
                eir8a.Income_Tax_Employer = model.Income_Tax_Employer;
                eir8a.Tax_Partially = model.Tax_Partially;
                eir8a.Tax_Fixed = model.Tax_Fixed;
                eir8a.Employee_CPF = model.Employee_CPF;
                eir8a.Name_CPF = model.Name_CPF;
                eir8a.Donation = model.Donation;
                eir8a.Contribution_Mosque = model.Contribution_Mosque;
                eir8a.Life_Insurance = model.Life_Insurance;
                eir8a.Yayasan_Mendaki_Fund = model.Yayasan_Mendaki_Fund == "on";
                eir8a.SINDA = model.SINDA == "on";
                eir8a.ECF = model.ECF == "on";
                eir8a.Community_chest_of_Singapore = model.Community_chest_of_Singapore == "on";
                eir8a.CDAC = model.CDAC == "on";
                eir8a.Other_tax = model.Other_tax_exempt_donations == "on";
                eir8a.Employee_Profile_ID = model.Employee_Profile_ID;
                eir8a.Company_Running_ID = userlogin.Company_ID.Value;
                eir8a.Create_By = userlogin.User_Authentication.Email_Address;
                eir8a.Create_On = currentdate;
                eir8a.Update_By = userlogin.User_Authentication.Email_Address;
                eir8a.Update_On = currentdate;
                eir8a.Employer = userlogin.Profile_ID;
                eir8a.Bank_Name = model.Bank_Name;

                eir8a.Contribution_With_Name = model.Contribution_With_Name;
                eir8a.Contribution_With_Establishment = model.Contribution_With_Establishment;
                eir8a.Contribution_With_Madatory = model.Contribution_With_Madatory;
                eir8a.Contribution_With_Singapore = model.Contribution_With_Singapore;
                eir8a.Remission_Income = model.Remission_Income;
                eir8a.Non_Tax_Income = model.Non_Tax_Income;

                model.result = payrollService.insertETIRA8(eir8a);
                if (model.result.Code == ERROR_CODE.SUCCESS)
                    model.operation = UserSession.RIGHT_A;
            }

            model.sDepartment = 0;
            model.etira8 = payrollService.GetETIRA8s(userlogin.Company_ID.Value);
            model.sYearList = new List<int>();
            for (int i = 2014; i <= currentdate.Year + 1; i++)
            {
                model.sYearList.Add(i);
            }
            if (model.sYear == 0)
                model.sYear = currentdate.Year;

            if (model.operation == UserSession.RIGHT_A)
                return View(model);
            else
            {
                if (model.sYear == 2015)
                    return View("IR8A2014", model);
                else if (model.sYear == 2016)
                    return View("IR8A2015", model);
                else if (model.sYear == 2017)
                    return View("IR8A2016", model);
                else if (model.sYear == 2018)
                    return View("IR8A2017", model);
            }
            return View(model);
        }

        [HttpGet]
        public void IR8APDF(string operation, string iid)
        {
            IR8AViewModel model = new IR8AViewModel();
            model.operation = EncryptUtil.Decrypt(operation);
            model.iid = NumUtil.ParseInteger(EncryptUtil.Decrypt(iid));


            var rightResult = base.validatePageRight(model.operation, "/Payroll/IR8A");
            if (rightResult.action == null)
            {
                model.rights = rightResult.rights;

                var userlogin = UserSession.getUser(HttpContext);
                var payrollService = new PayrollService();
                var empService = new EmployeeService();
                var histService = new EmploymentHistoryService();
                var companyService = new CompanyService();
                var company = companyService.GetCompany(userlogin.Company_ID);
                if (model.iid != 0)
                {
                    ETIRA8 eir8a = payrollService.GetETIRA8(model.iid);
                    if (eir8a != null && eir8a.Company_Running_ID == userlogin.Company_ID.Value)
                    {
                        var firstHist = histService.GetFirstEmploymentHistory(eir8a.Employee_Profile_ID);
                        if (firstHist != null && firstHist.Effective_Date.HasValue)
                            model.Hired_Date = DateUtil.ToDisplayDate(firstHist.Effective_Date);

                        model.P_YEAR = eir8a.P_YEAR;
                        model.Date_of_Cessation = DateUtil.ToDisplayDate(eir8a.Date_of_Cessation);
                        model.Gross_Salary = eir8a.Gross_Salary.HasValue ? eir8a.Gross_Salary.Value : 0;
                        model.Bonus = eir8a.Bonus.HasValue ? eir8a.Bonus.Value : 0;
                        model.Director_Fee = eir8a.Director_Fee.HasValue ? eir8a.Director_Fee.Value : 0;
                        model.Allowance_Transport = eir8a.Allowance_Transport.HasValue ? eir8a.Allowance_Transport.Value : 0;
                        model.Allowance_Entertain = eir8a.Allowance_Entertain.HasValue ? eir8a.Allowance_Entertain.Value : 0;
                        model.Allowance_Others = eir8a.Allowance_Others.HasValue ? eir8a.Allowance_Others.Value : 0;
                        model.Commission_Start = DateUtil.ToDisplayDate(eir8a.Commission_Start);
                        model.Commission_End = DateUtil.ToDisplayDate(eir8a.Commission_End);
                        model.Commission_Amount = eir8a.Commission_Amount.HasValue ? eir8a.Commission_Amount.Value : 0;
                        model.Commission_Type = eir8a.Commission_Type.HasValue ? eir8a.Commission_Type.Value : 0;
                        model.Pension = eir8a.Pension.HasValue ? eir8a.Pension.Value : 0;
                        model.Gratuity = eir8a.Gratuity.HasValue ? eir8a.Gratuity.Value : 0;
                        model.Notice_Pay = eir8a.Notice_Pay.HasValue ? eir8a.Notice_Pay.Value : 0;
                        model.Ex_Gratia = eir8a.Ex_Gratia.HasValue ? eir8a.Ex_Gratia.Value : 0;
                        model.Lump_Sum_Others = eir8a.Lump_Sum_Others.HasValue ? eir8a.Lump_Sum_Others.Value : 0;
                        model.Nature = eir8a.Nature;
                        model.Compensation_Loss = eir8a.Compensation_Loss.HasValue ? eir8a.Compensation_Loss.Value : 0;
                        model.Approval_IRAS = eir8a.Approval_IRAS.HasValue ? eir8a.Approval_IRAS.Value : false;
                        model.Date_Approval = DateUtil.ToDisplayDate(eir8a.Date_Approval);
                        model.Reason_Payment = eir8a.Reason_Payment;
                        model.Length_Service = eir8a.Length_Service.HasValue ? eir8a.Length_Service.Value : 0;
                        model.Basis_Payment = eir8a.Basis_Payment;
                        model.Total_Lump_Sum = eir8a.Total_Lump_Sum.HasValue ? eir8a.Total_Lump_Sum.Value : 0;
                        model.Retirement_Pension = eir8a.Retirement_Pension;
                        model.Amount_Accured_1992 = eir8a.Amount_Accured_1992.HasValue ? eir8a.Amount_Accured_1992.Value : 0;
                        model.Amount_Accured_1993 = eir8a.Amount_Accured_1993.HasValue ? eir8a.Amount_Accured_1993.Value : 0;
                        model.Contribution_Out_Singapore = eir8a.Contribution_Out_Singapore.HasValue ? eir8a.Contribution_Out_Singapore.Value : 0;
                        model.Excess_Employer = eir8a.Excess_Employer.HasValue ? eir8a.Excess_Employer.Value : 0;
                        model.Excess_Less = eir8a.Excess_Less.HasValue ? eir8a.Excess_Less.Value : 0;
                        model.Gain_Profit = eir8a.Gain_Profit.HasValue ? eir8a.Gain_Profit.Value : 0;
                        model.Value_Benefit = eir8a.Value_Benefit.HasValue ? eir8a.Value_Benefit.Value : 0;
                        model.Income_Tax_Employer = eir8a.Income_Tax_Employer.HasValue ? eir8a.Income_Tax_Employer.Value : false;
                        model.Tax_Partially = eir8a.Tax_Partially.HasValue ? eir8a.Tax_Partially.Value : 0;
                        model.Tax_Fixed = eir8a.Tax_Fixed.HasValue ? eir8a.Tax_Fixed.Value : 0;
                        model.Employee_CPF = eir8a.Employee_CPF.HasValue ? eir8a.Employee_CPF.Value : 0;
                        model.Name_CPF = eir8a.Name_CPF;
                        model.Donation = eir8a.Donation.HasValue ? eir8a.Donation.Value : 0;
                        model.Contribution_Mosque = eir8a.Contribution_Mosque.HasValue ? eir8a.Contribution_Mosque.Value : 0;
                        model.Life_Insurance = eir8a.Life_Insurance.HasValue ? eir8a.Life_Insurance.Value : 0;
                        model.Yayasan_Mendaki_Fund = eir8a.Yayasan_Mendaki_Fund.HasValue ? (eir8a.Yayasan_Mendaki_Fund.Value ? "on" : "off") : "off";
                        model.SINDA = eir8a.SINDA.HasValue ? (eir8a.SINDA.Value ? "on" : "off") : "off";
                        model.ECF = eir8a.ECF.HasValue ? (eir8a.ECF.Value ? "on" : "off") : "off";
                        model.Community_chest_of_Singapore = eir8a.Community_chest_of_Singapore.HasValue ? (eir8a.Community_chest_of_Singapore.Value ? "on" : "off") : "off";
                        model.CDAC = eir8a.CDAC.HasValue ? (eir8a.CDAC.Value ? "on" : "off") : "off";
                        model.Other_tax_exempt_donations = eir8a.Other_tax.HasValue ? (eir8a.Other_tax.Value ? "on" : "off") : "off";
                        model.Bank_Name = eir8a.Bank_Name;
                        model.Employee_Profile_ID = eir8a.Employee_Profile_ID;
                        model.Employee_Profile = empService.GetEmployeeProfile(eir8a.Employee_Profile_ID);
                        model.Contribution_With_Name = eir8a.Contribution_With_Name;
                        model.Contribution_With_Establishment = eir8a.Contribution_With_Establishment;
                        model.Contribution_With_Madatory = eir8a.Contribution_With_Madatory;
                        model.Contribution_With_Singapore = eir8a.Contribution_With_Singapore.HasValue ? eir8a.Contribution_With_Singapore.Value : 0;
                        model.Remission_Income = eir8a.Remission_Income.HasValue ? eir8a.Remission_Income.Value : 0;
                        model.Non_Tax_Income = eir8a.Non_Tax_Income.HasValue ? eir8a.Non_Tax_Income.Value : 0;
                        if (model.Employee_Profile == null)
                            return;

                        model.company = new CompanyService().GetCompany(eir8a.Company_Running_ID);

                        model.Run_Date = DateUtil.ToDisplayDate(eir8a.Create_On);

                        User_Profile u = new UserService().getUser(eir8a.Employer.HasValue ? eir8a.Employer.Value : 0);
                        if (u == null)
                        {
                            u = new UserService().getUserProfile(eir8a.Create_By);
                        }

                        if (u != null && u.Employee_Profile != null && u.Employee_Profile.Count > 0)
                        {
                            model.employer = empService.GetEmployeeProfile(u.Employee_Profile.FirstOrDefault().Employee_Profile_ID);
                        }
                        else
                        {
                            model.employer = empService.GetEmployeeProfile(userlogin.Employee_Profile.FirstOrDefault().Employee_Profile_ID);
                        }

                        if (model.employer == null)
                        {
                            return;
                        }
                        model.nationalityList = new ComboService().LstNationality(false);
                        model.genderList = new ComboService().LstLookup(ComboType.Gender, userlogin.Company_ID, false);

                        var viewname = "IR8APDF2014";
                        if (model.P_YEAR == "2016")
                            viewname = "IR8APDF2015";
                        else if (model.P_YEAR == "2017")
                            viewname = "IR8APDF2016";
                        else if (model.P_YEAR == "2018")
                            viewname = "IR8APDF2017";

                        string htmlToConvert = RenderPartialViewAsString(viewname, model);
                        StyleSheet style = new StyleSheet();

                        Response.ContentType = "application/pdf";
                        StringReader sr = new StringReader(htmlToConvert);
                        Document pdfDoc = new Document(PageSize.A4, 40.2f, 21.6f, 18f, 13.7f);
                        HTMLWorker htmlparser = new HTMLWorker(pdfDoc);

                        //if (company != null)
                        //{
                        //   if (company.Country_ID.HasValue)
                        //   {
                        //      htmlparser.SetStyleSheet(GenerateStyleSheet(company.Country_ID));
                        //   }
                        //}

                        var writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                        pdfDoc.Open();
                        htmlparser.Parse(sr);
                        pdfDoc.Close();

                    }
                }
            }
        }
        #endregion

        #region PAT_CPF

        [HttpGet]
        public ActionResult CPFGenerateFile()
        {
            var model = new CPFGenerateViewModels();
            var userlogin = UserSession.getUser(HttpContext);
            var payrollService = new PayrollService();
            var cSerevice = new ComboService();

            model.processDateList = cSerevice.LstMonth();

            var paidYearList = payrollService.GetCPFYear(userlogin.Company_ID.Value);
            if (paidYearList != null)
            {
                model.CPF_YearList = paidYearList;
            }

            model.generatedCPF_File = payrollService.GetFileExport_HistoryList(userlogin.Company_ID.Value, "CPF");

            return View(model);
        }

        [HttpPost]
        public ActionResult CPFGenerateFile(CPFGenerateViewModels model)
        {
            var userlogin = UserSession.getUser(HttpContext);
            var payrollService = new PayrollService();
            var cSerevice = new ComboService();

            model.processDateList = cSerevice.LstMonth();

            var paidYearList = payrollService.GetCPFYear(userlogin.Company_ID.Value);
            if (paidYearList != null)
            {
                model.CPF_YearList = paidYearList;
            }

            //model.Payment_Type = 0;

            if (model.CPF_ProcessMonth == 0)
                ModelState.AddModelError("CPF_ProcessMonth", Resource.Message_Is_Required);

            if (model.CPF_ProcessYear == 0)
                ModelState.AddModelError("CPF_ProcessYear", Resource.Message_Is_Required);

            if (ModelState.IsValid)
            {
                string url = WebConfigurationManager.AppSettings["PATGenerateUrl"] + "FileExport/CPFExport?month=" + model.CPF_ProcessMonth + "&year=" + model.CPF_ProcessYear + "&compID=" + userlogin.Company_ID.Value;
                model.result = payrollService.chkCPFConfig(userlogin.Company_ID.Value);
                if (model.result.Code != ERROR_CODE.ERROR_22_NO_PAT_CONFIG)
                {
                    model.result = payrollService.chkCPFExists(userlogin.Company_ID.Value, model.CPF_ProcessMonth, model.CPF_ProcessYear);

                    if (model.result.Code == ERROR_CODE.SUCCESS_GENERATE)
                    {
                        //var response = await vClient.PostAsync<>("POSMasterSync/UploadPOS_Receipts", new {month = model.CPF_ProcessMonth, year = model.CPF_ProcessYear, compID = userlogin.Company_ID.Value});

                        //HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(string.Format(url));
                        //webReq.Method = "Get";
                        //HttpWebResponse webResponse = (HttpWebResponse)webReq.GetResponse();

                        HttpWebResponse response = null;
                        var request = (HttpWebRequest)WebRequest.Create(string.Format(url));
                        request.Method = "Get";

                        try
                        {
                            response = (HttpWebResponse)request.GetResponse();
                            //return RedirectToAction("Payroll", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field });
                        }
                        catch
                        {
                            /* A WebException will be thrown if the status of the response is not `200 OK` */
                            //If webService is down 
                            model.result.Code = ERROR_CODE.ERROR_18_NO_PAYROLL_RECORDS;
                            model.result.Msg = Resource.ERROR_There_Is_Some_Problems_Related_With_File_Generating_Service;
                            model.result.Field = Resource.CPF_File;
                            //return RedirectToAction("CPFGenerateFile", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field });
                        }
                        finally
                        {
                            // Don't forget to close your response.
                            if (response != null)
                            {
                                response.Close();
                            }
                        }

                        //
                        //Stream answer = webResponse.GetResponseStream();
                        //StreamReader _recivedAnswer = new StreamReader(answer);

                        //return RedirectToAction("CPFGenerateFile", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field });
                    }
                }
            }
            model.generatedCPF_File = payrollService.GetFileExport_HistoryList(userlogin.Company_ID.Value, "CPF");

            return View(model);
        }

        [HttpGet]
        public ActionResult CPFGenerateConfig()
        {
            var userlogin = UserSession.getUser(HttpContext);
            var model = new CompanyInfoViewModel();
            var comService = new CompanyService();

            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            var rightResult = base.validatePageRight(UserSession.RIGHT_A, "/Payroll/CPFGenerateConfig");
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;

            var com = comService.GetCompany(userlogin.Company_ID);
            if (com != null)
            {
                model.Company_ID = userlogin.Company_ID;
                model.Company_Name = com.Name;
                model.CPF_Submission_No = com.CPF_Submission_No;
                model.patUser_ID = com.patUser_ID;
                model.patPassword = com.patPassword;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult CPFGenerateConfig(CompanyInfoViewModel model)
        {
            var userlogin = UserSession.getUser(HttpContext);
            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            var comService = new CompanyService();

            var com = comService.GetCompany(model.Company_ID);
            if (com == null)
                return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

            if (model.CPF_Submission_No != null && model.patUser_ID != null && model.patPassword != null)
            {
                com.CPF_Submission_No = model.CPF_Submission_No;
                com.patUser_ID = model.patUser_ID;
                com.patPassword = model.patPassword;

                model.result = comService.UpdateCompany(com);
                model.result.Field = Resource.PAT_CPF_Configuration;
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult CPFGenerateFileDetail(string generatedID)
        {
            var genID = Convert.ToInt32(EncryptUtil.Decrypt(generatedID));
            var model = new CPFGenerateViewModels();
            var userlogin = UserSession.getUser(HttpContext);
            var payrollService = new PayrollService();
            var cbService = new ComboService();

            model.Generated_ID = genID;
            model.departments = payrollService.GetDepartment(userlogin.Company_ID);
            model.designations = payrollService.GetDesignation(userlogin.Company_ID);
            model.races = payrollService.GetRace();
            model.residentialStatusList = cbService.LstResidentialStatus();

            model.generatedCPF_FileDetail = payrollService.GetCPF_HistoryDetailList(model.Generated_ID);

            return View(model);
        }

        [HttpPost]
        public ActionResult CPFGenerateFileDetail(CPFGenerateViewModels model, string SubmitControl = "")
        {
            var userlogin = UserSession.getUser(HttpContext);
            var payrollService = new PayrollService();
            var cbService = new ComboService();

            model.departments = payrollService.GetDepartment(userlogin.Company_ID);
            model.designations = payrollService.GetDesignation(userlogin.Company_ID);
            model.races = payrollService.GetRace();
            model.residentialStatusList = cbService.LstResidentialStatus();

            if (SubmitControl == "SubCPF" && model.Generated_ID > 0)
            {
                var fileExport = payrollService.GetFileHistoryHeader(model.Generated_ID);
                string cpfFile = "";
                if (fileExport != null)
                {
                    cpfFile = fileExport.File_LocalPath + fileExport.File_Name;
                }

                SftpClient client = payrollService.checkConnection(WebConfigurationManager.AppSettings["sFTPDomainName"], WebConfigurationManager.AppSettings["sFTPPort"], WebConfigurationManager.AppSettings["sFTPUserName"]);
                if (client != null)
                {
                    payrollService.UploadFileToSFTPServer(cpfFile, client, WebConfigurationManager.AppSettings["sFTPUploadFilePath"]);
                    model.result = new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS), Field = Resource.CPF_File };
                }
                else
                {
                    model.result = new ServiceResult() { Code = ERROR_CODE.ERROR_18_NO_PAYROLL_RECORDS, Msg = Resource.ERROR_There_Is_Some_Problems_Related_With_File_Generating_Service, Field = Resource.CPF_File };
                }
            }

            model.generatedCPF_FileDetail = payrollService.GetCPF_HistoryDetailList(model.Generated_ID, model.CPF_Department, model.CPF_Designation, model.CPF_Race, model.CPF_Residential);

            return View(model);
        }

        [HttpPost]
        public void PrintCPFSubmission(PayrollViewModels model, string pFileType)
        {
            var currentdate = StoredProcedure.GetCurrentDate();
            var userlogin = UserSession.getUser(HttpContext);
            var payrollService = new PayrollService();
            //----- day - month -  year -----
            var curentDay = currentdate.Day.ToString("00");
            var currentMonth = currentdate.Month.ToString("00");
            var currentYear = currentdate.Year;

            var currentHour = currentdate.Hour.ToString("00");
            var currentMin = currentdate.Minute.ToString("00");
            var currentSec = currentdate.Second.ToString("00");

            string sFileName = System.IO.Path.GetRandomFileName();
            //Added By sun 20-08-2015
            model.File_Type = pFileType;
            string sGenName = "CPF_E_Submission_" + curentDay + currentMonth + currentYear + "." + model.File_Type;

            if (!System.IO.Directory.Exists(Server.MapPath("CPF_E_Submission/")))
                System.IO.Directory.CreateDirectory(Server.MapPath("CPF_E_Submission/"));

            var companyService = new CompanyService();
            var company = companyService.GetCompany(userlogin.Company_ID);

            //----- Company Regitry -----
            var companyReg = "";
            if (company.Registry != null) companyReg = company.Registry.PadLeft(10, '0');


            var paymenttype = "PTE";
            var sn = "01";
            var adviceCode = "01";

            //---------Header------------------
            var header =
                "F" +                                       // 1 - 1 Submission mode
                " " +                                       // 2 - 2 Space
                companyReg.PadLeft(10, '0') +               // 3 - 12 Company Register UEN/NRIC/FIN
                paymenttype +                               // 13 - 15 Payment Type eg. ‘PTE’, ‘AMS’
                sn +                                        // 16 - 17 Serial number
                " " +                                       // 18 - 18 Space
                adviceCode +                                // 19 - 20 Advice code
                currentYear + currentMonth + curentDay +    // 21 - 28 File Creation Date CCYYMMDD
                currentHour + currentMin + currentSec +     // 29 - 34 File Creation Time HHMMSS
                "FTP" + "." + model.File_Type;              // 35 - 47 For file transfer mode

            header = header.PadRight(150, ' ');
            //--------- end header -----------

            var paymentcode = "01";
            decimal sumcontributionAmount = 0; // Sum cpf amount

            var empHistService = new EmploymentHistoryService();
            var emps = payrollService.ListPayroll(userlogin.Company_ID, model.sDepartment, model.Process_Month, model.Process_Year, model.sProcess);

            var details = new List<string>();
            //-------- emp detail -------
            foreach (var emp in emps)
            {
                if (emp.Residential_Status == "P" || (emp.Nationality != null && emp.Nationality.Name == "SG"))
                {
                    var prm = emp.PRMs.Where(w => w.Process_Year == model.Process_Year & w.Process_Month == model.Process_Month).FirstOrDefault();
                    if (prm != null && prm.Process_Status == PayrollStatus.Comfirm)
                    {
                        var empAccountNo = "";
                        decimal contributionAmount = 0;
                        decimal allowance = 0;
                        decimal deduction = 0;
                        decimal salary = 0;
                        decimal commision = 0;
                        decimal bonus = 0;
                        decimal diretorfee = 0;
                        if (emp.Residential_Status == "L")
                        {
                            //Local
                            empAccountNo = emp.NRIC != null ? emp.NRIC : "";
                        }
                        else if (emp.Residential_Status == "P")
                        {
                            //Permanent Resident
                            empAccountNo = emp.PR_No != null ? emp.PR_No : "";
                        }

                        var payrolldate = DateUtil.ToDate(1, model.Process_Month, model.Process_Year);

                        //Edit by sun 09-02-2017
                        var total_salary = 0M;
                        if (prm.Basic_Salary.HasValue && prm.Basic_Salary.Value > 0)
                            total_salary = prm.Basic_Salary.Value;

                        if (prm.Total_Allowance_Basic_Salary.HasValue && prm.Total_Allowance_Basic_Salary.Value > 0)
                            total_salary += prm.Total_Allowance_Basic_Salary.Value;

                        if (total_salary > 0)
                            salary = total_salary;

                        //if (prm.Basic_Salary.HasValue)
                        //   salary = prm.Basic_Salary.Value;
                        else
                        {
                            var empHist = empHistService.GetPayrollEmploymentHistory(emp.Employee_Profile_ID, payrolldate.Value);
                            if (empHist != null)
                            {
                                if (empHist.Basic_Salary_Unit == Term.Hourly)
                                {
                                    salary = (prm.Hourly_Rate.HasValue ? prm.Hourly_Rate.Value : 0) * (prm.No_Of_Hours.HasValue ? prm.No_Of_Hours.Value : 0);
                                }
                                else
                                {
                                    salary = NumUtil.ParseDecimal(EncryptUtil.Decrypt(empHist.Basic_Salary));
                                    if (salary == 0)
                                        salary = NumUtil.ParseDecimal(EncryptUtil.Decrypt(EncryptUtil.Decrypt(empHist.Basic_Salary)));
                                }
                            }
                        }


                        contributionAmount = (prm.CPF_Emplyer.HasValue ? prm.CPF_Emplyer.Value : 0) + (prm.CPF_Employee.HasValue ? prm.CPF_Employee.Value : 0);
                        sumcontributionAmount = sumcontributionAmount + contributionAmount;
                        foreach (var prd in prm.PRDs)
                        {
                            //Added by sun 09-02-2017
                            if (prd.PRT.Name == PayrollAllowanceType.Allowance_Add_On_To_Basic_Salary)
                                continue;

                            if (prd.PRT.Type == PayrollAllowanceType.Allowance_Deduction)
                            {
                                if (prd.PRT.Name == "Allowance")
                                {
                                    allowance = allowance + (prd.Amount.HasValue ? prd.Amount.Value : 0);
                                }
                                else
                                {
                                    deduction = deduction + (prd.Amount.HasValue ? prd.Amount.Value : 0);
                                }
                            }
                            else if (prd.PRT.Type == PayrollAllowanceType.Commission)
                            {
                                commision = commision + (prd.Amount.HasValue ? prd.Amount.Value : 0);
                            }
                        }
                        bonus = prm.Total_Bonus.HasValue ? prm.Total_Bonus.Value : 0;
                        diretorfee = prm.Total_Director_Fee.HasValue ? prm.Total_Director_Fee.Value : 0;
                        //Edit by sun 09-02-2016
                        //salary = prm.Basic_Salary.HasValue ? prm.Basic_Salary.Value : salary;


                        var ordinaryWage = (salary + allowance) - deduction;
                        var additionwage = commision + bonus + diretorfee;
                        var empName = UserSession.GetUserName(emp.User_Profile);

                        var employmentStatus = " ";

                        //var actions = emp.Employee_Profile_Action.OrderBy(o => o.Effective_Date).ToList();
                        //if (actions.Count() > 0)
                        //{
                        //    var laststatus = actions[actions.Count() - 1];
                        //    if (laststatus.Action == "Terminate")
                        //    {
                        //        // leave
                        //        var terminatedate = laststatus.Effective_Date.Value;
                        //        if (terminatedate.Month == emp.Hired_Date.Value.Month && terminatedate.Year == emp.Hired_Date.Value.Year)
                        //        {
                        //            //'O' - employee who joins and leaves in the same month 
                        //            employmentStatus = "O";
                        //        }
                        //        else
                        //        {
                        //            //'L' - Leaver
                        //            employmentStatus = "L";
                        //        }
                        //    }
                        //    else
                        //    {
                        //        // Existing
                        //        if (model.Process_Month == emp.Hired_Date.Value.Month && model.Process_Year == emp.Hired_Date.Value.Year)
                        //        {
                        //            //'N' - New Joiner
                        //            employmentStatus = "N";
                        //        }
                        //        else
                        //        {
                        //            //'E' - Existing employee or employee who leaves and joins in the same month
                        //            employmentStatus = "E";
                        //        }
                        //    }
                        //}

                        var condetail =
                        "F" +                                         // 1 - 1 Submission mode
                        "1" +                                         // 2 - 2 Record type
                        companyReg.PadLeft(10, '0') +                 // 3 - 12 Company Register UEN/NRIC/FIN
                        paymenttype +                                 // 13 - 15 Payment Type eg. ‘PTE’, ‘AMS’
                        sn +                                          // 16 - 17 Serial number
                        " " +                                         // 18 - 18 Space
                        adviceCode +                                  // 19 - 20 Advice code
                        currentYear + currentMonth +                  // 21 - 26 File Creation Date CCYYMM
                        paymentcode +                                 // 27 - 28 Payment code '01'- CPF Contribution
                        empAccountNo.PadLeft(9, '0') +                // 29 - 37 Employee account no.
                        contributionAmount.ToString("n0").Replace(".", "").Replace(",", "").PadLeft(12, '0') +    // 38 - 49 contribution amount for CPF
                        ordinaryWage.ToString("n2").Replace(".", "").Replace(",", "").PadLeft(10, '0') +          // 50 - 59 Ordinary wages = (Salary + Allowance) – Deduction 
                        additionwage.ToString("n2").Replace(".", "").Replace(",", "").PadLeft(10, '0') +          // 60 - 69 Additional wages = Commission + Bonus + Director Fee
                        employmentStatus +                            // 70 - 70 Employment Status 'E' - Existing employee, 'L' - Leaver, 'N' - New Joiner ,'O' - (alpha letter 'O') 
                        empName                                       // 71 - 92 Employee name                                       
                        ;

                        condetail = condetail.PadRight(150, ' ');
                        details.Add(condetail);
                    }


                }

            }

            //-------- end emp detail -------


            //-------- Summary ------------
            var donorcount = "0".PadLeft(7, '0');

            var conSummary =
                "F" +                                       // 1 - 1 Submission mode
                "0" +                                       // 2 - 2 Record type
                companyReg.PadLeft(10, '0') +               // 3 - 12 Company Register UEN/NRIC/FIN
                paymenttype +                               // 13 - 15 Payment Type eg. ‘PTE’, ‘AMS’
                sn +                                        // 16 - 17 Serial number
                " " +                                       // 18 - 18 Space
                adviceCode +                                // 19 - 20 Advice code
                currentYear + currentMonth +                // 21 - 26 File Creation Date CCYYMM
                paymentcode +                               // 27 - 28 Payment code '01'- CPF Contribution
                sumcontributionAmount.ToString("n0").Replace(".", "").Replace(",", "").PadLeft(12, '0') + // 29 - 40 Contribution summary amount
                donorcount;                                 // 41 - 47 Donor count
            conSummary = conSummary.PadRight(150, ' ');
            //-------- end summary--------

            var empRecordCount = emps.Count;

            //-----trailer---------------
            var trailer =
                "F" +                                           // 1 - 1 Submission mode
                "9" +                                           // 2 - 2 Record type
                companyReg.PadLeft(10, '0') +                   // 3 - 12 Company Register UEN/NRIC/FIN
                paymenttype +                                   // 13 - 15 Payment Type eg. ‘PTE’, ‘AMS’
                sn +                                            // 16 - 17 Serial number
                " " +                                           // 18 - 18 Space
                adviceCode +                                    // 19 - 20 Advice code
                empRecordCount.ToString().PadLeft(7, '0') +     // 21 - 27 Employer record count
                sumcontributionAmount.ToString("n2").Replace(".", "").Replace(",", "").PadLeft(15, '0')  // 28 - 42 Contribution summary amount
                ;

            trailer = trailer.PadRight(150, ' ');

            //YOu could omit these lines here as you may
            //not want to save the textfile to the server
            //I have just left them here to demonstrate that you could create the text file 
            using (System.IO.StreamWriter SW = new System.IO.StreamWriter(Server.MapPath("CPF_E_Submission/" + sFileName + "." + model.File_Type)))
            {
                SW.WriteLine(header);
                SW.WriteLine(conSummary);
                foreach (var d in details)
                {
                    SW.WriteLine(d);
                }
                SW.WriteLine(trailer);
                SW.Close();
            }

            System.IO.FileStream fs = null;
            fs = System.IO.File.Open(Server.MapPath("CPF_E_Submission/" + sFileName + "." + model.File_Type), System.IO.FileMode.Open);
            byte[] btFile = new byte[fs.Length];
            fs.Read(btFile, 0, Convert.ToInt32(fs.Length));
            fs.Close();
            Response.AddHeader("Content-disposition", "attachment; filename=" + sGenName);
            Response.ContentType = "application/octet-stream";
            Response.BinaryWrite(btFile);
            Response.End();
        }

        #endregion

        #region PAT_IR8A
        [HttpGet]
        public ActionResult IR8AGenerateConfig()
        {
            var userlogin = UserSession.getUser(HttpContext);
            var model = new CompanyInfoViewModel();
            var comService = new CompanyService();
            var cbService = new ComboService();
            var branchService = new BranchService();

            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            var rightResult = base.validatePageRight(UserSession.RIGHT_A, "/Payroll/IR8AGenerateConfig");
            if (rightResult.action != null) return rightResult.action;
            model.rights = rightResult.rights;

            var com = comService.GetCompany(userlogin.Company_ID);
            if (com != null)
            {
                model.Company_ID = userlogin.Company_ID;
                model.Company_Name = com.Name;
                model.Company_Source = com.Company_Source;
                model.PayerID_Type = com.PayerID_Type;
                model.PayerID_No = com.PayerID_No;
                if (com.Branch_ID != null)
                    model.Branch_ID = com.Branch_ID.Value;
                else
                    model.Branch_ID = 0;
            }
            model.companySoures = cbService.LstCompanySource();
            model.payerIDTypes = cbService.LstPayerIDTypes();
            model.branches = branchService.LstBranch(userlogin.Company_ID);

            return View(model);
        }

        [HttpPost]
        public ActionResult IR8AGenerateConfig(CompanyInfoViewModel model)
        {

            var userlogin = UserSession.getUser(HttpContext);
            if (userlogin == null)
                return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

            var currentdate = StoredProcedure.GetCurrentDate();
            var cbService = new ComboService();
            var branchService = new BranchService();
            var comService = new CompanyService();

            var com = comService.GetCompany(model.Company_ID);
            if (com == null)
                return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);

            //com.Name = model.Name;
            if (model.Company_Source != null && model.PayerID_Type != null && model.PayerID_No != null && model.Branch_ID > 0)
            {
                com.Company_Source = model.Company_Source;
                com.PayerID_Type = model.PayerID_Type;
                com.PayerID_No = model.PayerID_No;
                com.Branch_ID = model.Branch_ID;
                com.Update_By = userlogin.User_Authentication.Email_Address;
                com.Update_On = currentdate;

                model.result = comService.UpdateCompany(com);
                model.result.Field = Resource.IR8A_File_Config;
            }

            model.companySoures = cbService.LstCompanySource();
            model.payerIDTypes = cbService.LstPayerIDTypes();
            model.branches = branchService.LstBranch(userlogin.Company_ID);

            return View(model);
        }

        [HttpGet]
        public ActionResult IR8AGenerateFile()
        {
            var model = new CPFGenerateViewModels();
            var userlogin = UserSession.getUser(HttpContext);
            var payrollService = new PayrollService();

            var yearList = payrollService.GetCPFYear(userlogin.Company_ID.Value);
            if (yearList != null)
                model.IR8A_YearList = yearList;

            model.generatedIR8A_File = payrollService.GetFileExport_HistoryList(userlogin.Company_ID.Value, "IR8A");

            return View(model);
        }

        [HttpPost]
        public ActionResult IR8AGenerateFile(CPFGenerateViewModels model)
        {
            var userlogin = UserSession.getUser(HttpContext);
            var payrollService = new PayrollService();
            var cSerevice = new ComboService();

            var yearList = payrollService.GetCPFYear(userlogin.Company_ID.Value);
            if (yearList != null)
                model.IR8A_YearList = yearList;

            if (model.IR8A_Year == 0)
                ModelState.AddModelError("IR8A_Year", Resource.Message_Is_Required);

            if (ModelState.IsValid)
            {
                string url = WebConfigurationManager.AppSettings["PATGenerateUrl"] + "FileExport/IR8AExport?year=" + model.IR8A_Year + "&compID=" + userlogin.Company_ID.Value;
                model.result = payrollService.chkIR8AConfig(userlogin.Company_ID.Value);
                if (model.result.Code != ERROR_CODE.ERROR_22_NO_PAT_CONFIG)
                {
                    model.result = payrollService.chkIR8AExists(userlogin.Company_ID.Value, model.IR8A_Year);

                    if (model.result.Code == ERROR_CODE.SUCCESS_GENERATE)
                    {

                        HttpWebResponse response = null;
                        var request = (HttpWebRequest)WebRequest.Create(string.Format(url));
                        request.Method = "Get";

                        try
                        {
                            response = (HttpWebResponse)request.GetResponse();
                            //return RedirectToAction("Payroll", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field });
                        }
                        catch
                        {
                            /* A WebException will be thrown if the status of the response is not `200 OK` */
                            //If webService is down 
                            model.result.Code = ERROR_CODE.ERROR_18_NO_PAYROLL_RECORDS;
                            model.result.Msg = Resource.ERROR_There_Is_Some_Problems_Related_With_File_Generating_Service;
                            model.result.Field = Resource.IR8A_File;
                            //return RedirectToAction("CPFGenerateFile", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field });
                        }
                        finally
                        {
                            // Don't forget to close your response.
                            if (response != null)
                            {
                                response.Close();
                            }
                        }
                    }
                }
            }
            model.generatedIR8A_File = payrollService.GetFileExport_HistoryList(userlogin.Company_ID.Value, "IR8A");

            return View(model);
        }

        [HttpGet]
        public ActionResult IR8AGenerateFileDetail(string generatedID)
        {
            var genID = Convert.ToInt32(EncryptUtil.Decrypt(generatedID));
            var model = new CPFGenerateViewModels();
            var userlogin = UserSession.getUser(HttpContext);
            var payrollService = new PayrollService();

            model.Generated_ID = genID;
            model.departments = payrollService.GetDepartment(userlogin.Company_ID);
            model.designations = payrollService.GetDesignation(userlogin.Company_ID);

            model.generatedIR8A_FileDetail = payrollService.GetIR8A_DetailList(model.Generated_ID);

            return View(model);
        }

        [HttpPost]
        public ActionResult IR8AGenerateFileDetail(CPFGenerateViewModels model, string SubmitControl = "")
        {
            var userlogin = UserSession.getUser(HttpContext);
            var payrollService = new PayrollService();
            var cbService = new ComboService();

            model.departments = payrollService.GetDepartment(userlogin.Company_ID);
            model.designations = payrollService.GetDesignation(userlogin.Company_ID);

            if (SubmitControl == "SubCPF" && model.Generated_ID != null)
            {
                var fileExport = payrollService.GetFileHistoryHeader(model.Generated_ID);
                string ir8aFile = "";
                if (fileExport != null)
                {
                    ir8aFile = fileExport.File_LocalPath + fileExport.File_Name;
                }

                SftpClient client = payrollService.checkConnection(WebConfigurationManager.AppSettings["sFTPDomainName"], WebConfigurationManager.AppSettings["sFTPPort"], WebConfigurationManager.AppSettings["sFTPUserName"]);
                if (client != null)
                {
                    payrollService.UploadFileToSFTPServer(ir8aFile, client, WebConfigurationManager.AppSettings["sFTPUploadFilePathIR8A"]);

                    ServiceResult sResult = new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS), Field = Resource.IR8A_File };
                    model.result = sResult;
                }
                else
                {
                    ServiceResult sResult = new ServiceResult() { Code = ERROR_CODE.ERROR_18_NO_PAYROLL_RECORDS, Msg = Resource.ERROR_There_Is_Some_Problems_Related_With_File_Generating_Service, Field = Resource.IR8A_File };
                    model.result = sResult;
                }
            }

            model.generatedIR8A_FileDetail = payrollService.GetIR8A_DetailList(model.Generated_ID, model.CPF_Department, model.CPF_Designation);

            return View(model);
        }

        [HttpGet]
        public ActionResult IR8ACommissionAmount(int eid, string sdate, string edate)
        {
            decimal amount = 0;
            PayrollService payrollService = new PayrollService();
            amount = payrollService.GetCommisionAmount(eid, DateUtil.ToDate(sdate), DateUtil.ToDate(edate));
            return Json(new { amount = amount }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region PAT_IR8S
        [HttpGet]
        public ActionResult IR8SGenerateFile()
        {
            var model = new CPFGenerateViewModels();
            var userlogin = UserSession.getUser(HttpContext);
            var payrollService = new PayrollService();

            var yearList = payrollService.GetCPFYear(userlogin.Company_ID.Value);
            if (yearList != null)
                model.IR8S_YearList = yearList;

            model.generatedIR8S_File = payrollService.GetFileExport_HistoryList(userlogin.Company_ID.Value, "IR8S");

            return View(model);
        }

        [HttpPost]
        public ActionResult IR8SGenerateFile(CPFGenerateViewModels model)
        {
            var userlogin = UserSession.getUser(HttpContext);
            var payrollService = new PayrollService();
            var cSerevice = new ComboService();

            var yearList = payrollService.GetCPFYear(userlogin.Company_ID.Value);
            if (yearList != null)
                model.IR8S_YearList = yearList;

            if (model.IR8S_Year == 0)
                ModelState.AddModelError("IR8S_Year", Resource.Message_Is_Required);

            if (ModelState.IsValid)
            {
                string url = WebConfigurationManager.AppSettings["PATGenerateUrl"] + "FileExport/IR8SExport?year=" + model.IR8A_Year + "&compID=" + userlogin.Company_ID.Value;
                model.result = payrollService.chkIR8AConfig(userlogin.Company_ID.Value);
                if (model.result.Code != ERROR_CODE.ERROR_22_NO_PAT_CONFIG)
                {
                    model.result = payrollService.chkIR8AExists(userlogin.Company_ID.Value, model.IR8S_Year);

                    if (model.result.Code == ERROR_CODE.SUCCESS_GENERATE)
                    {

                        HttpWebResponse response = null;
                        var request = (HttpWebRequest)WebRequest.Create(string.Format(url));
                        request.Method = "Get";

                        try
                        {
                            response = (HttpWebResponse)request.GetResponse();
                            //return RedirectToAction("Payroll", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field });
                        }
                        catch
                        {
                            /* A WebException will be thrown if the status of the response is not `200 OK` */
                            //If webService is down 
                            model.result.Code = ERROR_CODE.ERROR_18_NO_PAYROLL_RECORDS;
                            model.result.Msg = Resource.ERROR_There_Is_Some_Problems_Related_With_File_Generating_Service;
                            model.result.Field = Resource.IR8S_File;
                            //return RedirectToAction("CPFGenerateFile", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field });
                        }
                        finally
                        {
                            // Don't forget to close your response.
                            if (response != null)
                            {
                                response.Close();
                            }
                        }
                    }
                }
            }
            model.generatedIR8S_File = payrollService.GetFileExport_HistoryList(userlogin.Company_ID.Value, "IR8S");

            return View(model);
        }

        [HttpGet]
        public ActionResult IR8SGenerateFileDetail(string generatedID)
        {
            var genID = Convert.ToInt32(EncryptUtil.Decrypt(generatedID));
            var model = new CPFGenerateViewModels();
            var userlogin = UserSession.getUser(HttpContext);
            var payrollService = new PayrollService();

            model.Generated_ID = genID;
            model.departments = payrollService.GetDepartment(userlogin.Company_ID);
            model.designations = payrollService.GetDesignation(userlogin.Company_ID);

            model.generatedIR8S_FileDetail = payrollService.GetIR8S_DetailList(model.Generated_ID);

            return View(model);
        }

        [HttpPost]
        public ActionResult IR8SGenerateFileDetail(CPFGenerateViewModels model, string SubmitControl = "")
        {
            var userlogin = UserSession.getUser(HttpContext);
            var payrollService = new PayrollService();
            var cbService = new ComboService();

            model.departments = payrollService.GetDepartment(userlogin.Company_ID);
            model.designations = payrollService.GetDesignation(userlogin.Company_ID);

            if (SubmitControl == "SubIR8S" && model.Generated_ID != null)
            {
                var fileExport = payrollService.GetFileHistoryHeader(model.Generated_ID);
                string ir8sFile = "";
                if (fileExport != null)
                {
                    ir8sFile = fileExport.File_LocalPath + fileExport.File_Name;
                }

                SftpClient client = payrollService.checkConnection(WebConfigurationManager.AppSettings["sFTPDomainName"], WebConfigurationManager.AppSettings["sFTPPort"], WebConfigurationManager.AppSettings["sFTPUserName"]);
                if (client != null)
                {
                    payrollService.UploadFileToSFTPServer(ir8sFile, client, WebConfigurationManager.AppSettings["sFTPUploadFilePathIR8A"]);

                    ServiceResult sResult = new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS), Field = Resource.IR8S_File };
                    model.result = sResult;
                }
                else
                {
                    ServiceResult sResult = new ServiceResult() { Code = ERROR_CODE.ERROR_18_NO_PAYROLL_RECORDS, Msg = Resource.ERROR_There_Is_Some_Problems_Related_With_File_Generating_Service, Field = Resource.IR8S_File };
                    model.result = sResult;
                }
            }

            model.generatedIR8S_FileDetail = payrollService.GetIR8S_DetailList(model.Generated_ID, model.CPF_Department, model.CPF_Designation);

            return View(model);
        }
        #endregion

        #region PAT_IRA8B

        [HttpGet]
        public ActionResult IRA8BGenerateFile()
        {
            var model = new CPFGenerateViewModels();
            var userlogin = UserSession.getUser(HttpContext);
            var payrollService = new PayrollService();

            var yearList = payrollService.GetCPFYear(userlogin.Company_ID.Value);
            if (yearList != null)
                model.IR8B_YearList = yearList;

            model.generatedIR8B_File = payrollService.GetFileExport_HistoryList(userlogin.Company_ID.Value, "IRA8B");

            return View(model);
        }

        [HttpPost]
        public ActionResult IRA8BGenerateFile(CPFGenerateViewModels model)
        {
            var userlogin = UserSession.getUser(HttpContext);
            var payrollService = new PayrollService();
            var cSerevice = new ComboService();

            var yearList = payrollService.GetCPFYear(userlogin.Company_ID.Value);
            if (yearList != null)
                model.IR8A_YearList = yearList;

            if (model.IR8A_Year == 0)
                ModelState.AddModelError("IR8A_Year", Resource.Message_Is_Required);

            if (ModelState.IsValid)
            {
                string url = WebConfigurationManager.AppSettings["PATGenerateUrl"] + "FileExport/IRA8BExport?year=" + model.IR8B_Year + "&compID=" + userlogin.Company_ID.Value;
                model.result = payrollService.chkIR8AConfig(userlogin.Company_ID.Value);
                if (model.result.Code != ERROR_CODE.ERROR_22_NO_PAT_CONFIG)
                {
                    model.result = payrollService.chkIR8AExists(userlogin.Company_ID.Value, model.IR8A_Year);

                    if (model.result.Code == ERROR_CODE.SUCCESS_GENERATE)
                    {

                        HttpWebResponse response = null;
                        var request = (HttpWebRequest)WebRequest.Create(string.Format(url));
                        request.Method = "Get";

                        try
                        {
                            response = (HttpWebResponse)request.GetResponse();
                            //return RedirectToAction("Payroll", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field });
                        }
                        catch
                        {
                            /* A WebException will be thrown if the status of the response is not `200 OK` */
                            //If webService is down 
                            model.result.Code = ERROR_CODE.ERROR_18_NO_PAYROLL_RECORDS;
                            model.result.Msg = Resource.ERROR_There_Is_Some_Problems_Related_With_File_Generating_Service;
                            model.result.Field = Resource.IR8B_File;
                            //return RedirectToAction("CPFGenerateFile", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field });
                        }
                        finally
                        {
                            // Don't forget to close your response.
                            if (response != null)
                            {
                                response.Close();
                            }
                        }
                    }
                }
            }
            model.generatedIR8B_File = payrollService.GetFileExport_HistoryList(userlogin.Company_ID.Value, "IRA8B");

            return View(model);
        }

        [HttpGet]
        public ActionResult IRA8BGenerateFileDetail(string generatedID)
        {
            var genID = Convert.ToInt32(EncryptUtil.Decrypt(generatedID));
            var model = new CPFGenerateViewModels();
            var userlogin = UserSession.getUser(HttpContext);
            var payrollService = new PayrollService();

            model.Generated_ID = genID;
            model.departments = payrollService.GetDepartment(userlogin.Company_ID);
            model.designations = payrollService.GetDesignation(userlogin.Company_ID);

            model.generatedIR8B_FileDetail = payrollService.GetIR8B_DetailList(model.Generated_ID);

            return View(model);
        }

        [HttpPost]
        public ActionResult IRA8BGenerateFileDetail(CPFGenerateViewModels model, string SubmitControl = "")
        {
            var userlogin = UserSession.getUser(HttpContext);
            var payrollService = new PayrollService();
            var cbService = new ComboService();

            model.departments = payrollService.GetDepartment(userlogin.Company_ID);
            model.designations = payrollService.GetDesignation(userlogin.Company_ID);

            if (SubmitControl == "SubCPF" && model.Generated_ID != null)
            {
                var fileExport = payrollService.GetFileHistoryHeader(model.Generated_ID);
                string ir8bFile = "";
                if (fileExport != null)
                {
                    ir8bFile = fileExport.File_LocalPath + fileExport.File_Name;
                }

                SftpClient client = payrollService.checkConnection(WebConfigurationManager.AppSettings["sFTPDomainName"], WebConfigurationManager.AppSettings["sFTPPort"], WebConfigurationManager.AppSettings["sFTPUserName"]);
                if (client != null)
                {
                    payrollService.UploadFileToSFTPServer(ir8bFile, client, WebConfigurationManager.AppSettings["sFTPUploadFilePathIR8A"]);

                    ServiceResult sResult = new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS), Field = Resource.IR8B_File };
                    model.result = sResult;
                }
                else
                {
                    ServiceResult sResult = new ServiceResult() { Code = ERROR_CODE.ERROR_18_NO_PAYROLL_RECORDS, Msg = Resource.ERROR_There_Is_Some_Problems_Related_With_File_Generating_Service, Field = Resource.IR8B_File };
                    model.result = sResult;
                }
            }

            model.generatedIR8B_FileDetail = payrollService.GetIR8B_DetailList(model.Generated_ID, model.CPF_Department, model.CPF_Designation);

            return View(model);
        }
        #endregion

        #region PAT_IRA8A

        [HttpGet]
        public ActionResult IRA8AGenerateFile()
        {
            var model = new CPFGenerateViewModels();
            var userlogin = UserSession.getUser(HttpContext);
            var payrollService = new PayrollService();

            var yearList = payrollService.GetCPFYear(userlogin.Company_ID.Value);
            if (yearList != null)
                model.IRA8A_YearList = yearList;

            model.generatedIRA8A_File = payrollService.GetFileExport_HistoryList(userlogin.Company_ID.Value, "IRA8A");

            return View(model);
        }

        [HttpPost]
        public ActionResult IRA8AGenerateFile(CPFGenerateViewModels model)
        {
            var userlogin = UserSession.getUser(HttpContext);
            var payrollService = new PayrollService();
            var cSerevice = new ComboService();

            var yearList = payrollService.GetCPFYear(userlogin.Company_ID.Value);
            if (yearList != null)
                model.IRA8A_YearList = yearList;

            if (model.IRA8A_Year == 0)
                ModelState.AddModelError("IRA8A_Year", Resource.Message_Is_Required);

            if (ModelState.IsValid)
            {
                string url = WebConfigurationManager.AppSettings["PATGenerateUrl"] + "FileExport/IRA8AExport?year=" + model.IR8B_Year + "&compID=" + userlogin.Company_ID.Value;
                model.result = payrollService.chkIR8AConfig(userlogin.Company_ID.Value);
                if (model.result.Code != ERROR_CODE.ERROR_22_NO_PAT_CONFIG)
                {
                    model.result = payrollService.chkIR8AExists(userlogin.Company_ID.Value, model.IRA8A_Year);

                    if (model.result.Code == ERROR_CODE.SUCCESS_GENERATE)
                    {

                        HttpWebResponse response = null;
                        var request = (HttpWebRequest)WebRequest.Create(string.Format(url));
                        request.Method = "Get";

                        try
                        {
                            response = (HttpWebResponse)request.GetResponse();
                            //return RedirectToAction("Payroll", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field });
                        }
                        catch
                        {
                            /* A WebException will be thrown if the status of the response is not `200 OK` */
                            //If webService is down 
                            model.result.Code = ERROR_CODE.ERROR_18_NO_PAYROLL_RECORDS;
                            model.result.Msg = Resource.ERROR_There_Is_Some_Problems_Related_With_File_Generating_Service;
                            model.result.Field = Resource.IRA8A_File;
                            //return RedirectToAction("CPFGenerateFile", new { Code = model.result.Code, Msg = model.result.Msg, Field = model.result.Field });
                        }
                        finally
                        {
                            // Don't forget to close your response.
                            if (response != null)
                            {
                                response.Close();
                            }
                        }
                    }
                }
            }
            model.generatedIRA8A_File = payrollService.GetFileExport_HistoryList(userlogin.Company_ID.Value, "IRA8A");

            return View(model);
        }

        [HttpGet]
        public ActionResult IRA8AGenerateFileDetail(string generatedID)
        {
            var genID = Convert.ToInt32(EncryptUtil.Decrypt(generatedID));
            var model = new CPFGenerateViewModels();
            var userlogin = UserSession.getUser(HttpContext);
            var payrollService = new PayrollService();

            model.Generated_ID = genID;
            model.departments = payrollService.GetDepartment(userlogin.Company_ID);
            model.designations = payrollService.GetDesignation(userlogin.Company_ID);

            model.generatedIRA8A_FileDetail = payrollService.GetIRA8A_DetailList(model.Generated_ID);

            return View(model);
        }

        [HttpPost]
        public ActionResult IRA8AGenerateFileDetail(CPFGenerateViewModels model, string SubmitControl = "")
        {
            var userlogin = UserSession.getUser(HttpContext);
            var payrollService = new PayrollService();
            var cbService = new ComboService();

            model.departments = payrollService.GetDepartment(userlogin.Company_ID);
            model.designations = payrollService.GetDesignation(userlogin.Company_ID);

            if (SubmitControl == "SubCPF" && model.Generated_ID != null)
            {
                var fileExport = payrollService.GetFileHistoryHeader(model.Generated_ID);
                string ir8bFile = "";
                if (fileExport != null)
                {
                    ir8bFile = fileExport.File_LocalPath + fileExport.File_Name;
                }

                SftpClient client = payrollService.checkConnection(WebConfigurationManager.AppSettings["sFTPDomainName"], WebConfigurationManager.AppSettings["sFTPPort"], WebConfigurationManager.AppSettings["sFTPUserName"]);
                if (client != null)
                {
                    payrollService.UploadFileToSFTPServer(ir8bFile, client, WebConfigurationManager.AppSettings["sFTPUploadFilePathIR8A"]);

                    ServiceResult sResult = new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS), Field = Resource.IRA8A_File };
                    model.result = sResult;
                }
                else
                {
                    ServiceResult sResult = new ServiceResult() { Code = ERROR_CODE.ERROR_18_NO_PAYROLL_RECORDS, Msg = Resource.ERROR_There_Is_Some_Problems_Related_With_File_Generating_Service, Field = Resource.IRA8A_File };
                    model.result = sResult;
                }
            }

            model.generatedIRA8A_FileDetail = payrollService.GetIRA8A_DetailList(model.Generated_ID);

            return View(model);
        }
        #endregion
    }

}