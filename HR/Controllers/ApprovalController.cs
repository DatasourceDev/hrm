using HR.Common;
using HR.Models;
using SBSModel.Common;
using SBSModel.Models;
using SBSWorkFlowAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SBSResourceAPI;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Web.Routing;
using SBSWorkFlowAPI.Constants;
using SBSWorkFlowAPI.ModelsAndService;

namespace HR.Controllers
{
   public class ApprovalController : ControllerBase
   {
      [HttpGet]
      [AllowAuthorized]
      public ActionResult Approval(ServiceResult result, string pAID, string pProfileID, string operation, string md, string atype)
      {
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var model = new ApprovalFlowViewModel();
         model.md = EncryptUtil.Decrypt(md);
         model.Approval_Type = EncryptUtil.Decrypt(atype);
         model.operation = EncryptUtil.Decrypt(operation);
         model.Approval_Flow_ID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pAID));
         model.Profile_ID = NumUtil.ParseInteger(EncryptUtil.Decrypt(pProfileID));

         if (string.IsNullOrEmpty(model.md)) model.md = ModuleCode.HR;
         //-------rights------------
         RightResult rightResult = base.validatePageRight(model.operation, "/" + model.Approval_Type + "/Configuration");
         if (rightResult.action != null)
            return rightResult.action;
         model.rights = rightResult.rights;
         model.result = result;

         var currentdate = StoredProcedure.GetCurrentDate();
         var cbService = new ComboService();
         var empService = new EmployeeService();
         var histService = new EmploymentHistoryService();
         var aService = new SBSWorkFlowAPI.Service();

         //-------data------------
         model.departmentList = cbService.LstDepartment(userlogin.Company_ID);
         model.EmpList = new List<_Applicable_Employee>();
         model.ApproverFlowTypeList = cbService.LstApproverFlowType();

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
                  Name = AppConst.GetUserName(emp.User_Profile),
                  Profile_ID = emp.Profile_ID,
                  Employee_Profile_ID = emp.Employee_Profile_ID
               });
            }
         }

         if (model.operation == UserSession.RIGHT_C)
         {
            var tmpDps = new List<ComboViewModel>();
            var usedDepartment = new List<int>();
            var allwf = aService.GetWorkflowByCompany(userlogin.Company_ID, model.md, model.Approval_Type);
            if (allwf.Item2.IsSuccess && allwf.Item1 != null)
            {
               foreach (var wf in allwf.Item1)
               {
                  if (wf.Departments != null)
                  {
                     usedDepartment.AddRange(wf.Departments.Select(s => s.User_Department_ID));
                  }
               }
            }
            foreach (var dp in model.departmentList)
            {
               if (!usedDepartment.Contains(NumUtil.ParseInteger(dp.Value)))
               {
                  tmpDps.Add(dp);
               }
            }
            model.departmentList = tmpDps;
         }
         else if (model.operation == UserSession.RIGHT_U)
         {
            if (model.Approval_Flow_ID.HasValue && model.Profile_ID.HasValue)
            {
               var r = aService.GetWorkflow(model.Approval_Flow_ID.Value);
               if (r.Item2.IsSuccess && r.Item1 != null)
               {
                  var approval = r.Item1;
                  model.Approval_Flow_ID = approval.Approval_Flow_ID;
                  model.Approval_Type = approval.Approval_Type;
                  model.Branch_ID = approval.Branch_ID;
                  model.Branch_Name = approval.Branch_Name;
                  model.Company_ID = approval.Company_ID;
                  model.Module = approval.Module;
                  model.Record_Status = approval.Record_Status;

                  if (approval.Departments != null)
                  {
                     model.Departments = approval.Departments.Select(s => s.User_Department_ID).ToArray();
                  }
                  var tmpDps = new List<ComboViewModel>();
                  var usedDepartment = new List<int>();
                  var allwf = aService.GetWorkflowByCompany(userlogin.Company_ID, model.md, model.Approval_Type);
                  if (allwf.Item2.IsSuccess && allwf.Item1 != null)
                  {
                     foreach (var wf in allwf.Item1)
                     {
                        if (wf.Approval_Flow_ID != approval.Approval_Flow_ID)
                        {
                           if (wf.Departments != null)
                           {
                              usedDepartment.AddRange(wf.Departments.Select(s => s.User_Department_ID));
                           }
                        }
                     }
                  }
                  foreach (var dp in model.departmentList)
                  {
                     if (!usedDepartment.Contains(NumUtil.ParseInteger(dp.Value)))
                     {
                        tmpDps.Add(dp);
                     }
                  }
                  model.departmentList = tmpDps;

                  var app = new List<int>();
                  var notApp = new List<int>();

                  if (approval.Applicable_Employee != null)
                  {
                     foreach (var emp in approval.Applicable_Employee)
                     {
                        if (emp.Is_Applicable.HasValue && emp.Is_Applicable.Value)
                        {
                           app.Add(emp.Profile_ID);
                        }
                     }
                  }

                  if (model.EmpList != null)
                  {
                     foreach (var emp in model.EmpList)
                     {
                        if (model.departmentList != null && approval.Departments != null)
                        {
                           if (approval.Departments.Select(s => s.User_Department_ID).Contains(emp.Department_ID.Value))
                           {
                              if (model.departmentList.Select(s => s.Value).Contains((emp.Department_ID.HasValue ? emp.Department_ID.Value : 0).ToString()))
                              {
                                 if (!app.Contains(emp.Profile_ID.Value))
                                 {
                                    notApp.Add(emp.Profile_ID.Value);
                                 }
                              }
                           }
                        }
                     }
                  }

                  model.Application_For = app.ToArray();
                  model.Not_Application_For = notApp.ToArray();

                  var approvals = new List<ApproverViewModel>();
                  if (approval.Approvers != null)
                  {
                     foreach (var a in approval.Approvers)
                     {
                        approvals.Add(new ApproverViewModel()
                        {
                           Approval_Flow_ID = a.Approval_Flow_ID,
                           Approver_ID = a.Approver_ID,
                           Approval_Level = a.Approval_Level,
                           Email = a.Email,
                           Name = a.Name,
                           Profile_ID = a.Profile_ID,
                           Row_Type = RowType.EDIT,
                           Approver_Flow_Type = a.Approval_Flow_Type
                        });
                     }

                     model.Approver_Rows = approvals.ToArray();
                  }
                  var reviewers = new List<ReviewerViewModel>();
                  if (approval.Reviewers != null)
                  {
                     foreach (var a in approval.Reviewers)
                     {
                        reviewers.Add(new ReviewerViewModel()
                        {
                           Approval_Flow_ID = a.Approval_Flow_ID,
                           Reviewer_ID = a.Reviewer_ID,
                           Email = a.Email,
                           Name = a.Name,
                           Profile_ID = a.Profile_ID,
                           Row_Type = RowType.EDIT
                        });
                     }
                     model.Reviewer_Rows = reviewers.ToArray();
                  }
               }
               else
               {
                  return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);
               }
            }
            else
            {
               return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);
            }
         }
         else if (model.operation == UserSession.RIGHT_D)
         {
            if (model.Approval_Flow_ID.HasValue && model.Profile_ID.HasValue)
            {
               var r = aService.UpdateDeleteWorkFlowStatus(model.Approval_Flow_ID.Value, userlogin.Profile_ID, WfRecordStatus.Delete);
               //if (model.Approval_Type == ApprovalType.Leave | model.Approval_Type == ApprovalType.Expense)
               //{
               if (r.IsSuccess)
                  return RedirectToAction("Configuration", model.Approval_Type, new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Approval, tabAction = "approval" });
               else
                  return RedirectToAction("Configuration", model.Approval_Type, new { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Approval, tabAction = "approval" });
               //}
               //else
               //{
               //   if (r.IsSuccess)
               //      return RedirectToAction(UrlUtil.Action(Url, "Configuration", model.Approval_Type, new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_DELETE), Field = Resource.Approval, tabAction = "approval" }));
               //   else
               //      return RedirectToAction(UrlUtil.Action(Url, "Configuration", model.Approval_Type, new { Code = ERROR_CODE.ERROR_505_DELETE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_505_DELETE_ERROR), Field = Resource.Approval, tabAction = "approval" }));
               //}
            }
         }
         return View(model);
      }

      [HttpPost]
      [AllowAuthorized]
      public ActionResult Approval(ApprovalFlowViewModel model)
      {
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);


         //-------rights------------
         RightResult rightResult = base.validatePageRight(model.operation, "/" + model.Approval_Type + "/Configuration");
         if (rightResult.action != null)
            return rightResult.action;
         model.rights = rightResult.rights;

         var currentdate = StoredProcedure.GetCurrentDate();
         if (model.Departments == null || model.Departments.Count() == 0)
            ModelState.AddModelError("Departments", Resource.Message_Is_Required);

         if (model.Approver_Rows == null || model.Approver_Rows.Count() == 0 || model.Approver_Rows.Where(w => w.Row_Type != RowType.DELETE).Count() == 0)
            ModelState.AddModelError("Approver_Rows", Resource.The + " " + Resource.Approver + " " + Resource.Is_Rrequired_Lower);

         //----------------Approver_Rows--------------------//
         if (model.Approver_Rows != null && model.Approver_Rows.Count() != 0)
         {
            List<int> chk = new List<int>();
            List<int> chk_Er = new List<int>();

            var acnt = 0;
            foreach (var row in model.Approver_Rows)
            {
               if (row.Row_Type != RowType.DELETE)
               {
                  if (row.Approver_Flow_Type == ApproverFlowType.Job_Cost)
                  {
                     DeleteModelStateError("Approver_Rows[" + acnt + "].Profile_ID");
                  }
                  else
                  {
                     if (row.Profile_ID.HasValue && !chk.Contains(row.Profile_ID.Value))
                     {
                        chk.Add(row.Profile_ID.Value);
                        continue;
                     }
                  }
               }
               else
               {
                  DeleteModelStateError("Approver_Rows[" + acnt + "]");
               }
               chk_Er.Add(row.I);
               acnt++;
               continue;
            }

            if (chk_Er.Count() != 0)
            {
               foreach (var i in chk_Er)
               {
                  if (model.Approver_Rows[i].Row_Type != RowType.DELETE)
                  {
                     if (model.Approver_Rows[i].Approver_Flow_Type != ApproverFlowType.Job_Cost)
                     {
                        ModelState.AddModelError("Approver_Rows[" + i + "].Profile_ID", Resource.Message_Is_Duplicated);
                     }
                  }
               }
            }
         }
         //------------------------Reviewer_Rows-------------------------//
         if (model.Reviewer_Rows != null && model.Reviewer_Rows.Count() != 0)
         {
            List<int> chk_Re = new List<int>();
            List<int> chk_Re_Er = new List<int>();

            var acnt = 0;
            foreach (var row in model.Reviewer_Rows)
            {
               if (row.Row_Type != RowType.DELETE)
               {
                  if (row.Profile_ID.HasValue && !chk_Re.Contains(row.Profile_ID.Value))
                  {
                     chk_Re.Add(row.Profile_ID.Value);
                     continue;
                  }
               }
               else
               {
                  DeleteModelStateError("Reviewer_Rows[" + acnt + "]");
               }
               //chk_Re_Er.Add(row.I);
               acnt++;
               continue;
            }

            if (chk_Re_Er.Count() != 0)
            {
               foreach (var i in chk_Re_Er)
               {
                  if (model.Reviewer_Rows[i].Row_Type != RowType.DELETE)
                  {
                     ModelState.AddModelError("Reviewer_Rows[" + i + "].Profile_ID", Resource.Message_Is_Duplicated);
                  }
               }
            }
         }
         //--------------------------------------------------------//

         model.EmpList = new List<_Applicable_Employee>();
         var empService = new EmployeeService();
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
                  Name = AppConst.GetUserName(emp.User_Profile),
                  Profile_ID = emp.Profile_ID,
                  Employee_Profile_ID = emp.Employee_Profile_ID
               });
            }
         }

         var err = GetErrorModelState();

         if (ModelState.IsValid)
         {
            var aService = new SBSWorkFlowAPI.Service();
            var aflow = new Approval_Flow();

            aflow.Approval_Type = model.Approval_Type;
            aflow.Module = model.md;
            aflow.Company_ID = userlogin.Company_ID.Value;
            aflow.Record_Status = WfRecordStatus.Active;

            aflow.Departments = new List<SBSWorkFlowAPI.Models.Department>();
            aflow.Approvers = new List<SBSWorkFlowAPI.Models.Approver>();
            aflow.Applicable_Employee = new List<SBSWorkFlowAPI.Models.Applicable_Employee>();
            aflow.Reviewers = new List<SBSWorkFlowAPI.Models.Reviewer>();

            foreach (var row in model.Departments)
            {
               var d = new DepartmentService().GetDepartment(row);
               if (d != null)
                  aflow.Departments.Add(new SBSWorkFlowAPI.Models.Department() { User_Department_ID = row, Name = d.Name });
            }

            if (model.Application_For != null)
            {
               foreach (var row in model.Application_For)
               {
                  if (model.EmpList != null)
                  {
                     var emp = model.EmpList.Where(w => w.Profile_ID == row).FirstOrDefault();
                     aflow.Applicable_Employee.Add(new SBSWorkFlowAPI.Models.Applicable_Employee()
                     {
                        Company_ID = userlogin.Company_ID.Value,
                        Profile_ID = row,
                        Name = emp.Name,
                        Email = emp.Email,
                        Is_Applicable = true
                     });
                  }
               }
            }
            if (model.Not_Application_For != null)
            {
               foreach (var row in model.Not_Application_For)
               {
                  if (model.EmpList != null)
                  {
                     var emp = model.EmpList.Where(w => w.Profile_ID == row).FirstOrDefault();
                     aflow.Applicable_Employee.Add(new SBSWorkFlowAPI.Models.Applicable_Employee()
                     {
                        Company_ID = userlogin.Company_ID.Value,
                        Profile_ID = row,
                        Name = emp.Name,
                        Email = emp.Email,
                        Is_Applicable = false
                     });
                  }
               }
            }

            foreach (var row in model.Approver_Rows)
            {
               if (row.Row_Type != RowType.DELETE)
               {
                  if (model.EmpList != null)
                  {
                     if (row.Approver_Flow_Type == ApproverFlowType.Job_Cost)
                     {
                        aflow.Approvers.Add(new SBSWorkFlowAPI.Models.Approver()
                        {
                           Approval_Level = row.Approval_Level.Value,
                           Company_ID = userlogin.Company_ID.Value,
                           Approval_Flow_Type = ApproverFlowType.Job_Cost
                        });
                     }
                     else
                     {
                        var emp = model.EmpList.Where(w => w.Profile_ID == row.Profile_ID).FirstOrDefault();
                        aflow.Approvers.Add(new SBSWorkFlowAPI.Models.Approver()
                        {
                           Approval_Level = row.Approval_Level.Value,
                           Company_ID = userlogin.Company_ID.Value,
                           Profile_ID = row.Profile_ID.Value,
                           Name = emp.Name,
                           Email = emp.Email,
                           Approval_Flow_Type = ApproverFlowType.Employee
                        });
                     }
                  }
               }
            }
            if (model.Reviewer_Rows != null)
            {
               foreach (var row in model.Reviewer_Rows)
               {
                  if (row.Row_Type != RowType.DELETE)
                  {
                     if (model.EmpList != null)
                     {
                        var emp = model.EmpList.Where(w => w.Profile_ID == row.Profile_ID).FirstOrDefault();
                        aflow.Reviewers.Add(new SBSWorkFlowAPI.Models.Reviewer()
                        {
                           Company_ID = userlogin.Company_ID.Value,
                           Profile_ID = row.Profile_ID.Value,
                           Name = emp.Name,
                           Email = emp.Email
                        });
                     }
                  }
               }
            }

            if (model.operation == UserSession.RIGHT_C)
            {
               aflow.Created_By = userlogin.Profile_ID;
               aflow.Created_On = currentdate;
               var result = aService.CreateWorkFlow(aflow);
               //if (model.Approval_Type == ApprovalType.Leave | model.Approval_Type == ApprovalType.Expense)
               //{
               if (result.IsSuccess)
                  return RedirectToAction("Configuration", model.Approval_Type, new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Approval, tabAction = "approval" });
               else
                  return RedirectToAction("Configuration", model.Approval_Type, new { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Approval, tabAction = "approval" });
               //}
               //else
               //{
               //   if (result.IsSuccess)
               //      return Redirect(UrlUtil.Action(Url, "Configuration", model.Approval_Type, new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CREATE), Field = Resource.Approval, tabAction = "approval" }));
               //   else
               //      return Redirect(UrlUtil.Action(Url, "Configuration", model.Approval_Type, new { Code = ERROR_CODE.ERROR_503_INSERT_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_503_INSERT_ERROR), Field = Resource.Approval, tabAction = "approval" }));
               //}
            }
            else
            {
               // update approval
               aflow.Approval_Flow_ID = model.Approval_Flow_ID.Value;
               aflow.Updated_By = userlogin.Profile_ID;
               aflow.Updated_On = currentdate;
               var result = aService.UpdateWorkFlow(aflow);
               //if (model.Approval_Type == ApprovalType.Leave | model.Approval_Type == ApprovalType.Expense)
               //{
               if (result.IsSuccess)
                  return RedirectToAction("Configuration", model.Approval_Type, new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Approval, tabAction = "approval" });
               else
                  return RedirectToAction("Configuration", model.Approval_Type, new { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Approval, tabAction = "approval" });
               //}
               //else
               //{
               //   if (result.IsSuccess)
               //      return Redirect(UrlUtil.Action(Url, "Configuration", model.Approval_Type, new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_EDIT), Field = Resource.Approval, tabAction = "approval" }));
               //   else
               //      return Redirect(UrlUtil.Action(Url, "Configuration", model.Approval_Type, new { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Error().getError(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Approval, tabAction = "approval" }));
               //}
            }
         }

         var cbService = new ComboService();
         var histService = new EmploymentHistoryService();
         model.departmentList = cbService.LstDepartment(userlogin.Company_ID);
         model.ApproverFlowTypeList = cbService.LstApproverFlowType();

         //Added by sun 30-09-2015
         //-----------------------------------------------//
         var aaService = new SBSWorkFlowAPI.Service();
         var tmpDps = new List<ComboViewModel>();
         var usedDepartment = new List<int>();
         var r = aaService.GetWorkflow(model.Approval_Flow_ID.Value);
         if (r != null & r.Item1 != null)
         {
            var approval = r.Item1;
            var allwf = aaService.GetWorkflowByCompany(userlogin.Company_ID, model.md, model.Approval_Type);
            if (allwf.Item2.IsSuccess && allwf.Item1 != null)
            {
               foreach (var wf in allwf.Item1)
               {
                  if (wf.Approval_Flow_ID != approval.Approval_Flow_ID)
                  {
                     if (wf.Departments != null)
                     {
                        usedDepartment.AddRange(wf.Departments.Select(s => s.User_Department_ID));
                     }
                  }
               }
            }
            foreach (var dp in model.departmentList)
            {
               if (!usedDepartment.Contains(NumUtil.ParseInteger(dp.Value)))
               {
                  tmpDps.Add(dp);
               }
            }
            model.departmentList = tmpDps;
         }
         else
         {
            //return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND);
         }
         //-------------------------------//

         return View(model);
      }

      [AllowAuthorized]
      public ActionResult AddNewApprover(int pIndex, string pApprovalType)
      {
         var userlogin = UserSession.getUser(HttpContext);
         var model = new ApproverViewModel() { Index = pIndex };
         var currentdate = StoredProcedure.GetCurrentDate();
         var empService = new EmployeeService();
         var cbService = new ComboService();

         var emps = empService.LstEmployeeProfile(userlogin.Company_ID);
         model.ApproverFlowTypeList = cbService.LstApproverFlowType();
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
                  Name = AppConst.GetUserName(emp.User_Profile),
                  Profile_ID = emp.Profile_ID,
                  Employee_Profile_ID = emp.Employee_Profile_ID
               });
            }
         }
         model.Approval_Type = pApprovalType;
         model.Approver_Flow_Type = ApproverFlowType.Employee;

         return PartialView("ApproverRow", model);
      }

      [AllowAuthorized]
      public ActionResult AddNewReviewer(int pIndex)
      {
         var userlogin = UserSession.getUser(HttpContext);
         var model = new ReviewerViewModel() { Index = pIndex };

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
                  Name = AppConst.GetUserName(emp.User_Profile),
                  Profile_ID = emp.Profile_ID,
                  Employee_Profile_ID = emp.Employee_Profile_ID
               });
            }
         }

         return PartialView("ReviewerRow", model);
      }

      [HttpGet]
      [AllowAuthorized]
      public ActionResult ProcessWorkflow(string lID, string expID, string pID, string appID, string empID, string profileID, string reqID, string reqcancelID, string status, string code, string cancelStatus, string tsID)
      {

         var Leave_Application_Document_ID = NumUtil.ParseInteger(EncryptUtil.Decrypt(lID));
         var Expenses_Application_ID = NumUtil.ParseInteger(EncryptUtil.Decrypt(expID));
         var TsExID = NumUtil.ParseInteger(EncryptUtil.Decrypt(tsID));
         var Ac_Code = EncryptUtil.Decrypt(code);
         var Request_ID = NumUtil.ParseInteger(EncryptUtil.Decrypt(reqID));
         var Employee_Profile_ID = NumUtil.ParseInteger(EncryptUtil.Decrypt(empID));
         var Profile_ID = NumUtil.ParseInteger(EncryptUtil.Decrypt(profileID));
         var Overall_Status = EncryptUtil.Decrypt(status);
         var Cancel_Status = EncryptUtil.Decrypt(cancelStatus);
         var Approver_ID = NumUtil.ParseInteger(EncryptUtil.Decrypt(appID));
         var Request_Cancel_ID = NumUtil.ParseInteger(EncryptUtil.Decrypt(reqcancelID));

         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var uService = new UserService();
         if (Approver_ID != userlogin.Profile_ID)
         {
            var currentdate = StoredProcedure.GetCurrentDate();
            var sdata = new User_Session_Data()
            {
               Actived = true,
               Session_ID = Session.SessionID,
               Profile_ID = Approver_ID,
               Session_Data = Request.Url.OriginalString,
               Update_On = currentdate,
               Create_On = currentdate,
            };
            uService.SaveSessionData(sdata);
            return RedirectToAction("LogOut", "Account");
         }

         var ac = uService.getActivationLink(Ac_Code);
         if (ac == null)
            return errorPage(ERROR_CODE.ERROR_3_ACTIVATE_CODE_NOT_FOUND);

         if (!ac.Active.HasValue || ac.Active.Value == false)
            return errorPage(ERROR_CODE.ERROR_2_ACTIVATE_CODE_EXPIRE);

         if (TsExID > 0)
            return TsExMngt(TsExID, Overall_Status, "Account", "MessagePage");
         else if (Leave_Application_Document_ID > 0)
            return ProcessLeave(Leave_Application_Document_ID, userlogin, Approver_ID, Employee_Profile_ID, Profile_ID, Request_ID, Overall_Status, Ac_Code, Request_Cancel_ID, Cancel_Status);
         else if (Expenses_Application_ID > 0)
            return ProcessExpenses(Expenses_Application_ID, userlogin, Approver_ID, Employee_Profile_ID, Profile_ID, Request_ID, Overall_Status, Ac_Code, Request_Cancel_ID, Cancel_Status);
         else if (TsExID > 0)
         {
            //return Redirect("http://localhost:50825/Approval/ProcessWorkflow?tsID=" + HttpUtility.UrlEncode(tsID) + "&appID=" + HttpUtility.UrlEncode(appID) + "&empID=" + HttpUtility.UrlEncode(empID) + "&profileID=" + HttpUtility.UrlEncode(profileID) + "&reqID=" + HttpUtility.UrlEncode(reqID) + "&status=" + HttpUtility.UrlEncode(status) + "&Remark=" + HttpUtility.UrlEncode(Remark) + "&code=" + HttpUtility.UrlEncode(code) + "&cancelStatus=" + HttpUtility.UrlEncode(cancelStatus));
            return Redirect(AppSetting.SERVER_NAME + ModuleDomain.Time + "/Approval/ProcessWorkflow?tsID=" + HttpUtility.UrlEncode(tsID) + "&appID=" + HttpUtility.UrlEncode(appID) + "&empID=" + HttpUtility.UrlEncode(empID) + "&profileID=" + HttpUtility.UrlEncode(profileID) + "&reqID=" + HttpUtility.UrlEncode(reqID) + "&status=" + HttpUtility.UrlEncode(status) + "&reqcancelID=" + HttpUtility.UrlEncode(reqcancelID) + "&code=" + HttpUtility.UrlEncode(code) + "&cancelStatus=" + HttpUtility.UrlEncode(cancelStatus));
         }


         return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);
      }

      #region Leave
      private ActionResult ProcessLeave(int Leave_Application_Document_ID, User_Profile userlogin, int Approver_ID, int Employee_Profile_ID, int Profile_ID, int Request_ID, string Overall_Status, string Ac_Code, int Request_Cancel_ID, string Cancel_Status)
      {
         var currentdate = StoredProcedure.GetCurrentDate();
         var leaveService = new LeaveService();

         if (Leave_Application_Document_ID == 0)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Leave);

         var status = ""; //can be Approved, Rejected, Cancelled
         if (string.IsNullOrEmpty(Cancel_Status))
            status = Overall_Status;
         else
            status = Cancel_Status;

         var model = new LeaveViewModel();
         model.Leave_Application_Document_ID = Leave_Application_Document_ID;
         model.Request_ID = Request_ID;
         model.Remark_Rej = "Reject from email";
         model.Request_Cancel_ID = Request_Cancel_ID;
         model.Employee_Profile_ID = Employee_Profile_ID;
         model.Overall_Status = Overall_Status;
         model.Cancel_Status = Cancel_Status;
         model.Profile_ID = Profile_ID;
         return ApplicationMngt(model, status);
      }

      private ActionResult ApplicationMngt(LeaveViewModel model, string pStatus)
      {
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var leaveService = new LeaveService();
         var cbService = new ComboService();
         var empService = new EmployeeService();
         var hService = new EmploymentHistoryService();
         var uService = new UserService();
         var currentdate = StoredProcedure.GetCurrentDate();

         var leave = leaveService.GetLeaveApplicationDocument(model.Leave_Application_Document_ID);
         if (leave == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Leave);

         if (leave.Cancel_Status == WorkflowStatus.Cancelled)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Leave);

         if (leave.Overall_Status == WorkflowStatus.Rejected)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Leave);

         var hist = hService.GetCurrentEmploymentHistory(leave.Employee_Profile_ID);
         if (hist == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Employment_History);

         var user = uService.getUser(model.Profile_ID, false);
         if (user == null)
            user = uService.getUser(hist.Employee_Profile.Profile_ID, false);
         if (user == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.User);

         var aService = new SBSWorkFlowAPI.Service();
         var status = pStatus;

         var Ac_Code = "L" + leave.Leave_Application_Document_ID + userlogin.Profile_ID + "_";
         if (string.IsNullOrEmpty(leave.Cancel_Status))
         {
            if (model.Request_ID.HasValue && model.Request_ID > 0)
            {
               var action = new ActionItem();
               action.Actioner_Profile_ID = userlogin.Profile_ID;
               action.Email = userlogin.User_Authentication.Email_Address;
               action.Name = UserSession.GetUserName(userlogin);
               action.Request_ID = model.Request_ID.Value;
               if (pStatus == WorkflowStatus.Approved)
               {
                  action.IsApprove = true;
                  action.Action = WorkflowAction.Approve;
               }
               else
               {
                  if (string.IsNullOrEmpty(model.Remark_Rej))
                  {
                     ModelState.AddModelError("Remark_Rej", Resource.Message_Is_Required);
                     model.isRejectPopUp = true;
                  }
                  else
                     model.isRejectPopUp = false;

                  action.IsApprove = false;
                  action.Remarks = model.Remark_Rej;
                  action.Action = WorkflowAction.Reject;
               }

               if (ModelState.IsValid)
               {
                  var r = aService.SubmitRequestAction(action);
                  if (r.IsSuccess)
                  {
                     if (action.Status == WorkflowStatus.Closed)
                     {
                        model.result = leaveService.UpdateLeaveUse(model.Leave_Application_Document_ID, model.Employee_Profile_ID, action.Status);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                        {
                           uService.ExpireActivationByPrefix(Ac_Code);
                           sendProceedEmail(leave, null, user, userlogin, hist, action.Status, null);
                           return RedirectToAction("MessagePage", "Account", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_APPROVE), Field = Resource.Leave });
                        }
                     }
                     else if (action.Status == WorkflowStatus.Rejected)
                     {
                        model.result = leaveService.UpdateLeaveStatus(model.Leave_Application_Document_ID, action.Status);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                        {
                           uService.ExpireActivationByPrefix(Ac_Code);
                           sendProceedEmail(leave, null, user, userlogin, hist, action.Status, null);
                           return RedirectToAction("MessagePage", "Account", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_REJECT), Field = Resource.Leave });
                        }
                     }
                     else
                     {
                        //sendProceedEmail(leave, null, user, userlogin, hist, action.Status, null);
                        var param = new Dictionary<string, object>();
                        param.Add("lID", leave.Leave_Application_Document_ID);
                        param.Add("appID", action.NextApprover.Profile_ID);
                        param.Add("empID", leave.Employee_Profile_ID);
                        param.Add("reqID", leave.Request_ID);
                        param.Add("status", WorkflowStatus.Approved);
                        param.Add("code", uService.GenActivateCode("L" + leave.Leave_Application_Document_ID + action.NextApprover.Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));

                        var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                        param["status"] = WorkflowStatus.Rejected;
                        var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                        var appr = uService.getUser(action.NextApprover.Profile_ID, false);
                        if (appr != null)
                           sendRequestEmail(leave, null, appr, user, hist, WorkflowStatus.Submitted, null, linkApp, linkRej);

                        uService.ExpireActivationByPrefix(Ac_Code);
                        return RedirectToAction("MessagePage", "Account", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_APPROVE), Field = Resource.Leave });
                     }
                  }
               }
            }
            else if (hist.Supervisor.HasValue && hist.Supervisor > 0)
            {
               /*approval by supervisor*/
               if (pStatus == WorkflowStatus.Approved)
                  leave.Overall_Status = WorkflowStatus.Closed;
               else
                  leave.Overall_Status = WorkflowStatus.Rejected;

               model.result = leaveService.UpdateLeaveUse(leave.Leave_Application_Document_ID, leave.Employee_Profile_ID, leave.Overall_Status);
               if (model.result.Code == ERROR_CODE.SUCCESS)
               {
                  uService.ExpireActivationByPrefix(Ac_Code);
                  sendProceedEmail(leave, null, user, userlogin, hist, leave.Overall_Status, null);
                  if (pStatus == WorkflowStatus.Approved)
                     return RedirectToAction("MessagePage", "Account", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_APPROVE), Field = Resource.Leave });
                  else
                     return RedirectToAction("MessagePage", "Account", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_REJECT), Field = Resource.Leave });
               }
            }
         }
         else
         {
            /* approve canncel workflow*/
            if (model.Request_Cancel_ID.HasValue && model.Request_Cancel_ID > 0)
            {
               var action = new ActionItem();
               action.Actioner_Profile_ID = userlogin.Profile_ID;
               action.Email = userlogin.User_Authentication.Email_Address;
               action.Name = UserSession.GetUserName(userlogin);
               action.Request_ID = model.Request_Cancel_ID.Value;
               if (pStatus == WorkflowStatus.Cancelled)/*DIF*/
               {
                  action.IsApprove = true;
                  action.Action = WorkflowAction.Approve;
               }
               else
               {
                  if (string.IsNullOrEmpty(model.Remark_Rej))
                  {
                     ModelState.AddModelError("Remark_Rej", Resource.Message_Is_Required);
                     model.isRejectPopUp = true;
                  }
                  else
                     model.isRejectPopUp = false;

                  action.IsApprove = false;
                  action.Remarks = model.Remark_Rej;
                  action.Action = WorkflowAction.Reject;
               }

               if (ModelState.IsValid)
               {
                  var r = aService.SubmitRequestAction(action);
                  if (r.IsSuccess)
                  {
                     if (action.Status == WorkflowStatus.Closed)
                     {
                        model.result = leaveService.UpdateLeaveUse(model.Leave_Application_Document_ID, model.Employee_Profile_ID, null, WorkflowStatus.Cancelled, false);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                        {
                           uService.ExpireActivationByPrefix(Ac_Code);
                           sendProceedEmail(leave, null, user, userlogin, hist, WorkflowStatus.Cancelled, null);
                           return RedirectToAction("MessagePage", "Account", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_APPROVE), Field = Resource.Leave });
                        }
                     }
                     else if (action.Status == WorkflowStatus.Rejected)
                     {
                        model.result = leaveService.UpdateLeaveStatus(model.Leave_Application_Document_ID, null, WorkflowStatus.Cancellation_Rejected);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                        {
                           uService.ExpireActivationByPrefix(Ac_Code);
                           sendProceedEmail(leave, null, user, userlogin, hist, WorkflowStatus.Cancellation_Rejected, null);
                           return RedirectToAction("MessagePage", "Account", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL_REJECT), Field = Resource.Leave });
                        }
                     }
                     else
                     {
                        var param = new Dictionary<string, object>();
                        param.Add("lID", leave.Leave_Application_Document_ID);
                        param.Add("appID", action.NextApprover.Profile_ID);
                        param.Add("empID", leave.Employee_Profile_ID);
                        param.Add("reqID", leave.Request_ID);
                        param.Add("cancelStatus", WorkflowStatus.Cancelled);
                        //param.Add("status", WorkflowStatus.Approved);
                        param.Add("code", uService.GenActivateCode("L" + leave.Leave_Application_Document_ID + action.NextApprover.Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));

                        var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                        param["status"] = WorkflowStatus.Rejected;
                        var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                        var appr = uService.getUser(action.NextApprover.Profile_ID, false);
                        if (appr != null)
                           sendRequestEmail(leave, null, appr, user, hist, WorkflowStatus.Submitted, null, linkApp, linkRej);

                        uService.ExpireActivationByPrefix(Ac_Code);
                        return RedirectToAction("MessagePage", "Account", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_APPROVE), Field = Resource.Leave });

                     }
                  }
               }
            }
            else if (hist.Supervisor.HasValue && hist.Supervisor > 0)
            {
               /*cancel approval by supervisor*/
               if (pStatus == WorkflowStatus.Cancelled)/*DIF*/
                  leave.Cancel_Status = WorkflowStatus.Cancelled;
               else
                  leave.Cancel_Status = WorkflowStatus.Cancellation_Rejected;

               model.result = leaveService.UpdateLeaveUse(leave.Leave_Application_Document_ID, leave.Employee_Profile_ID, null, leave.Cancel_Status, false);
               if (model.result.Code == ERROR_CODE.SUCCESS)
               {
                  uService.ExpireActivationByPrefix(Ac_Code);
                  sendProceedEmail(leave, null, user, userlogin, hist, leave.Cancel_Status, null);
                  if (pStatus == WorkflowStatus.Cancelled)/*DIF*/
                     return RedirectToAction("MessagePage", "Account", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL), Field = Resource.Leave });
                  else
                     return RedirectToAction("MessagePage", "Account", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL_REJECT), Field = Resource.Leave });
               }
            }
         }

         return RedirectToAction("MessagePage", "Account", new { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Success().getSuccess(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Leave }); /*DIF*/
      }
      #endregion

      #region le dup
      //private void sendRequestEmail(Leave_Application_Document leave, Company_Details com, User_Profile sentto, User_Profile receivedfrom, Employment_History receivedhist, string Overall_Status, List<Reviewer> Reviewers, string linkApp, string linkRej)
      //{
      //   var ecode = "[RL" + leave.Leave_Application_Document_ID + "_";
      //   if (leave.Request_Cancel_ID.HasValue)
      //      ecode += "RQC" + leave.Request_Cancel_ID;
      //   else if (leave.Request_ID.HasValue)
      //      ecode += "RQ" + leave.Request_ID;

      //   ecode += "S" + receivedfrom.User_Authentication.User_Authentication_ID + "R" + sentto.User_Authentication.User_Authentication_ID + "]";

      //   var leaveService = new LeaveService();
      //   var cri = new LeaveTypeCriteria()
      //   {
      //      Profile_ID = leave.Employee_Profile.Profile_ID,
      //      Leave_Config_ID = leave.Leave_Config_ID,
      //      Relationship_ID = leave.Relationship_ID
      //   };
      //   var leaveleft = leaveService.CalculateLeaveLeft(cri);
      //   var eitem = new EmailItem()
      //   {
      //      LogoLink = GenerateLogoLink(),
      //      Company = com,
      //      Send_To_Email = sentto.User_Authentication.Email_Address,
      //      Send_To_Name = AppConst.GetUserName(sentto),
      //      Received_From_Email = receivedfrom.User_Authentication.Email_Address,
      //      Received_From_Name = AppConst.GetUserName(receivedfrom),
      //      Received_From_Department = receivedhist.Department != null ? receivedhist.Department.Name : "",
      //      Module = ModuleCode.HR,
      //      Approval_Type = ApprovalType.Leave,
      //      Leave = leave,
      //      Leave_Left = leaveleft != null ? leaveleft.Left : 0,
      //      Weeks_Left = leaveleft != null ? leaveleft.Weeks_Left : 0,
      //      Status = Overall_Status,
      //      Reviewer = Reviewers,
      //      Link = linkApp,
      //      Link2 = linkRej,
      //      Url = Request.Url.AbsoluteUri,
      //      ECode = ecode,
      //   };
      //   EmailTemplete.sendRequestEmail(eitem);
      //}

      //private void sendProceedEmail(Leave_Application_Document leave, Company_Details com, User_Profile sentto, User_Profile receivedfrom, Employment_History receivedhist, string Overall_Status, List<Reviewer> Reviewers)
      //{
      //   var ecode = "[PL" + leave.Leave_Application_Document_ID + "_";
      //   if (leave.Request_Cancel_ID.HasValue)
      //      ecode += "RQC" + leave.Request_Cancel_ID;
      //   else if (leave.Request_ID.HasValue)
      //      ecode += "RQ" + leave.Request_ID;

      //   ecode += "S" + receivedfrom.User_Authentication.User_Authentication_ID + "R" + sentto.User_Authentication.User_Authentication_ID + "]";

      //   var leaveService = new LeaveService();
      //   var cri = new LeaveTypeCriteria()
      //   {
      //      Profile_ID = leave.Employee_Profile.Profile_ID,
      //      Leave_Config_ID = leave.Leave_Config_ID,
      //      Relationship_ID = leave.Relationship_ID
      //   };
      //   var leaveleft = leaveService.CalculateLeaveLeft(cri);
      //   var eitem = new EmailItem()
      //   {
      //      LogoLink = GenerateLogoLink(),
      //      Company = com,
      //      Send_To_Email = sentto.User_Authentication.Email_Address,
      //      Send_To_Name = AppConst.GetUserName(sentto),
      //      Received_From_Email = receivedfrom.User_Authentication.Email_Address,
      //      Received_From_Name = AppConst.GetUserName(receivedfrom),
      //      Received_From_Department = receivedhist.Department != null ? receivedhist.Department.Name : "",
      //      Module = ModuleCode.HR,
      //      Approval_Type = ApprovalType.Leave,
      //      Leave = leave,
      //      Leave_Left = leaveleft != null ? leaveleft.Left : 0,
      //      Weeks_Left = leaveleft != null ? leaveleft.Weeks_Left : 0,
      //      Status = Overall_Status,
      //      Reviewer = Reviewers,
      //      Url = Request.Url.AbsoluteUri,
      //      ECode = ecode,
      //   };
      //   EmailTemplete.sendProceedEmail(eitem);
      //}
      #endregion

      #region Ex Dup
      //private void sendRequestEmail(Expenses_Application ex, Company_Details com, User_Profile sentto, User_Profile receivedfrom, Employment_History receivedhist, string Overall_Status, List<Reviewer> Reviewers, string linkApp, string linkRej)
      //{
      //   var ecode = "[RE" + ex.Expenses_Application_ID + "_";
      //   if (ex.Request_Cancel_ID.HasValue)
      //      ecode += "RQC" + ex.Request_Cancel_ID;
      //   else if (ex.Request_ID.HasValue)
      //      ecode += "RQ" + ex.Request_ID;

      //   ecode += "S" + receivedfrom.User_Authentication.User_Authentication_ID + "R" + sentto.User_Authentication.User_Authentication_ID + "]";

      //   //Fixed by sun 04-01-2016
      //   var Ex_Employee_Profile_ID = ex.Employee_Profile_ID.HasValue ? ex.Employee_Profile_ID.Value : 0;
      //   var Ex_Profile_ID = 0;
      //   if (ex.Employee_Profile != null && ex.Employee_Profile.Profile_ID.HasValue)
      //      Ex_Profile_ID = ex.Employee_Profile.Profile_ID.Value;

      //   //******** Start Smart Dev  ********//
      //   FileAttach SmartDevPdfRequest = getFileExpenseSmartDevPdf(EncryptUtil.Encrypt(ex.Expenses_Application_ID), EncryptUtil.Encrypt(UserSession.RIGHT_U), Ex_Employee_Profile_ID, Ex_Profile_ID);
      //   //******** End Smart Dev  ********//

      //   var eitem = new EmailItem()
      //   {
      //      LogoLink = GenerateLogoLink(),
      //      Company = com,
      //      Send_To_Email = sentto.User_Authentication.Email_Address,
      //      Send_To_Name = AppConst.GetUserName(sentto),
      //      Received_From_Email = receivedfrom.User_Authentication.Email_Address,
      //      Received_From_Name = AppConst.GetUserName(receivedfrom),
      //      Received_From_Department = receivedhist.Department != null ? receivedhist.Department.Name : "",
      //      Module = ModuleCode.HR,
      //      Approval_Type = ApprovalType.Expense,
      //      Expenses = ex,
      //      Status = Overall_Status,
      //      Reviewer = Reviewers,
      //      Link = linkApp,
      //      Link2 = linkRej,
      //      Url = Request.Url.AbsoluteUri,
      //      ECode = ecode,
      //      Attachment_SmartDev = SmartDevPdfRequest
      //   };
      //   EmailTemplete.sendRequestEmail(eitem);
      //}

      //private void sendProceedEmail(Expenses_Application ex, Company_Details com, User_Profile sentto, User_Profile receivedfrom, Employment_History receivedhist, string Overall_Status, List<Reviewer> Reviewers)
      //{
      //   var ecode = "[RP" + ex.Expenses_Application_ID + "_";
      //   if (ex.Request_Cancel_ID.HasValue)
      //      ecode += "RQC" + ex.Request_Cancel_ID;
      //   else if (ex.Request_ID.HasValue)
      //      ecode += "RQ" + ex.Request_ID;

      //   ecode += "S" + receivedfrom.User_Authentication.User_Authentication_ID + "R" + sentto.User_Authentication.User_Authentication_ID + "]";

      //   //Fixed by sun 04-01-2016
      //   var Ex_Employee_Profile_ID = ex.Employee_Profile_ID.HasValue ? ex.Employee_Profile_ID.Value : 0;
      //   var Ex_Profile_ID = 0;
      //   if (ex.Employee_Profile != null && ex.Employee_Profile.Profile_ID.HasValue)
      //      Ex_Profile_ID = ex.Employee_Profile.Profile_ID.Value;

      //   //******** Start Smart Dev  ********//
      //   FileAttach SmartDevPdfProceed = getFileExpenseSmartDevPdf(EncryptUtil.Encrypt(ex.Expenses_Application_ID), EncryptUtil.Encrypt(UserSession.RIGHT_U), Ex_Employee_Profile_ID, Ex_Profile_ID);
      //   //******** End Smart Dev  ********/

      //   var eitem = new EmailItem()
      //   {
      //      LogoLink = GenerateLogoLink(),
      //      Company = com,
      //      Send_To_Email = sentto.User_Authentication.Email_Address,
      //      Send_To_Name = AppConst.GetUserName(sentto),
      //      Received_From_Email = receivedfrom.User_Authentication.Email_Address,
      //      Received_From_Name = AppConst.GetUserName(receivedfrom),
      //      Received_From_Department = receivedhist.Department != null ? receivedhist.Department.Name : "",
      //      Module = ModuleCode.HR,
      //      Approval_Type = ApprovalType.Expense,
      //      Expenses = ex,
      //      Status = Overall_Status,
      //      Reviewer = Reviewers,
      //      Url = Request.Url.AbsoluteUri,
      //      ECode = ecode,
      //      Attachment_SmartDev = SmartDevPdfProceed
      //   };
      //   EmailTemplete.sendProceedEmail(eitem);
      //}
      #endregion

      #region Expenses
      private ActionResult ProcessExpenses(int Expenses_Application_ID, User_Profile userlogin, int Approver_ID, int Employee_Profile_ID, int Profile_ID, int Request_ID, string Overall_Status, string Ac_Code, int Request_Cancel_ID, string Cancel_Status)
      {
         if (Expenses_Application_ID == 0)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Expense);

         var status = ""; //can be Approved, Rejected, Cancelled
         if (string.IsNullOrEmpty(Cancel_Status))
            status = Overall_Status;
         else
            status = Cancel_Status;

         var model = new ExpensesViewModel();
         model.Expenses_ID = Expenses_Application_ID;
         model.Request_ID = Request_ID;
         model.Remark_Rej = "Reject from email";
         model.Request_Cancel_ID = Request_Cancel_ID;
         model.Employee_Profile_ID = Employee_Profile_ID;
         model.Overall_Status = Overall_Status;
         model.Cancel_Status = Cancel_Status;

         return ApplicationMngt(model, status);
      }
      private ActionResult ApplicationMngt(ExpensesViewModel model, string pStatus)
      {
         var userlogin = UserSession.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var eService = new ExpenseService();
         var cbService = new ComboService();
         var empService = new EmployeeService();
         var hService = new EmploymentHistoryService();
         var uService = new UserService();
         var currentdate = StoredProcedure.GetCurrentDate();
         var cpService = new CompanyService();

         var com = cpService.GetCompany(userlogin.Company_ID);
         if (com == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Company);

         var ex = eService.getExpenseApplication(model.Expenses_ID);
         if (ex == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Expenses);

         if (ex.Cancel_Status == WorkflowStatus.Cancelled)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Expenses);

         if (ex.Overall_Status == WorkflowStatus.Rejected)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Expenses);

         var hist = hService.GetCurrentEmploymentHistory(ex.Employee_Profile_ID);
         if (hist == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Employment_History);

         var user = uService.getUser(hist.Employee_Profile.Profile_ID, false);
         if (user == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.User);

         var aService = new SBSWorkFlowAPI.Service();
         var status = pStatus;

         var Ac_Code = "E" + ex.Expenses_Application_ID + userlogin.Profile_ID + "_";
         ex.Update_By = userlogin.User_Authentication.Email_Address;
         ex.Update_On = currentdate;
         if (string.IsNullOrEmpty(ex.Cancel_Status))
         {
            if (model.Request_ID.HasValue && model.Request_ID.Value > 0)
            {
               #region Workflow
               var action = new ActionItem();
               action.Actioner_Profile_ID = userlogin.Profile_ID;
               action.Email = userlogin.User_Authentication.Email_Address;
               action.Name = UserSession.GetUserName(userlogin);
               action.Request_ID = model.Request_ID.Value;
               if (pStatus == WorkflowStatus.Approved)
               {
                  action.IsApprove = true;
                  action.Action = WorkflowAction.Approve;
               }
               else
               {
                  if (string.IsNullOrEmpty(model.Remark_Rej))
                  {
                     ModelState.AddModelError("Remark_Rej", Resource.The + " " + Resource.Remark + " " + Resource.Field + " " + Resource.Is_Rrequired_Lower);
                     model.isRejectPopUp = true;
                  }
                  else
                     model.isRejectPopUp = false;

                  action.IsApprove = false;
                  action.Remarks = model.Remark_Rej;
                  action.Action = WorkflowAction.Reject;
               }
               if (ModelState.IsValid)
               {
                  var r = aService.SubmitRequestAction(action);
                  if (r.IsSuccess)
                  {         
                     ex.Approver = getApprovalStrIDs(ex.Approver, userlogin.Profile_ID.ToString());
                     if (action.Status == WorkflowStatus.Closed)
                     {
                        ex.Next_Approver = null;
                        ex.Overall_Status = WorkflowStatus.Closed;
                        model.result = eService.updateExpenseApplication(ex);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                        {
                           uService.ExpireActivationByPrefix(Ac_Code);
                           sendProceedEmail(ex, com, user, userlogin, hist, action.Status, null);
                           return RedirectToAction("MessagePage", "Account", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_APPROVE), Field = Resource.Expense });
                        }
                     }
                     else if (action.Status == WorkflowStatus.Rejected)
                     {
                        ex.Next_Approver = null;
                        ex.Overall_Status = WorkflowStatus.Rejected;
                        model.result = eService.updateExpenseApplication(ex);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                        {
                           uService.ExpireActivationByPrefix(Ac_Code);
                           sendProceedEmail(ex, com, user, userlogin, hist, action.Status, null);
                           return RedirectToAction("MessagePage", "Account", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_REJECT), Field = Resource.Expense });/*DIF*/
                        }
                     }
                     else
                     {
                        var mstr = "";
                        if (action.NextApprover == null)
                        {
                           #region Indent flow
                           var haveSendRequestEmail = false;
                           if (action.IndentValue != null && action.IndentValue.IsIndent && action.IndentValue.SendRequest)
                              haveSendRequestEmail = true;

                           if (haveSendRequestEmail)
                           {
                              ex.Next_Approver = null;
                              List<IndentItem> IndentItems = getIndentSupervisor(ex.Expenses_Application_ID);
                              if (IndentItems != null && IndentItems.Count > 0)
                              {
                                 foreach (var row in IndentItems)
                                 {
                                    if (action.Actioner_Profile_ID == row.Requestor_Profile_ID)
                                       continue;

                                    var param = new Dictionary<string, object>();
                                    param.Add("expID", ex.Expenses_Application_ID);
                                    param.Add("appID", row.Requestor_Profile_ID);
                                    param.Add("empID", ex.Employee_Profile_ID);
                                    param.Add("reqID", ex.Request_ID);
                                    param.Add("status", WorkflowStatus.Approved);
                                    param.Add("code", uService.GenActivateCode("E" + ex.Expenses_Application_ID + row.Requestor_Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));

                                    var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                                    param["status"] = WorkflowStatus.Rejected;
                                    var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                                    var appr = uService.getUser(row.Requestor_Profile_ID, false);
                                    if (appr != null)
                                       sendRequestEmail(ex, com, appr, user, hist, WorkflowStatus.Submitted, null, linkApp, linkRej);

                                    mstr = getApprovalStrIDs(mstr, row.Requestor_Profile_ID.ToString());
                                 }
                              }
                           }
                           else
                           {
                              var str = getApprovalStrIDs(null, userlogin.Profile_ID.ToString());
                              if (!string.IsNullOrEmpty(str))
                                 mstr = ex.Next_Approver.Replace(str, "|");
                           }
                           #endregion
                        }
                        else
                        {
                           #region Normal flow
                           var param = new Dictionary<string, object>();
                           param.Add("expID", ex.Expenses_Application_ID);
                           param.Add("appID", action.NextApprover.Profile_ID);
                           param.Add("empID", ex.Employee_Profile_ID);
                           param.Add("reqID", ex.Request_ID);
                           param.Add("status", WorkflowStatus.Approved);
                           param.Add("code", uService.GenActivateCode("E" + ex.Expenses_Application_ID + action.NextApprover.Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));

                           var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                           param["status"] = WorkflowStatus.Rejected;
                           var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                           var appr = uService.getUser(action.NextApprover.Profile_ID, false);
                           if (appr != null)
                              sendRequestEmail(ex, com, appr, user, hist, WorkflowStatus.Submitted, null, linkApp, linkRej);

                           mstr = getApprovalStrIDs(null, action.NextApprover.Profile_ID.ToString());
                           #endregion
                        }

                        ex.Next_Approver = mstr;
                        model.result = eService.updateExpenseApplication(ex);

                        uService.ExpireActivationByPrefix(Ac_Code);
                        return RedirectToAction("MessagePage", "Account", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_APPROVE), Field = Resource.Expense });/*DIF*/
                     }

                  }
               }
               #endregion
            }
            else if (hist.Supervisor.HasValue && hist.Supervisor.Value > 0)
            {
               #region Supervisor
               /*approval by supervisor*/
               if (pStatus == WorkflowStatus.Approved)
                  ex.Overall_Status = WorkflowStatus.Closed;
               else
                  ex.Overall_Status = WorkflowStatus.Rejected;

               ex.Next_Approver = null;
               ex.Approver = getApprovalStrIDs(null, userlogin.Profile_ID.ToString());
               model.result = eService.updateExpenseApplication(ex);
               if (model.result.Code == ERROR_CODE.SUCCESS)
               {
                  uService.ExpireActivationByPrefix(Ac_Code);
                  sendProceedEmail(ex, com, user, userlogin, hist, ex.Overall_Status, null);
                  if (pStatus == WorkflowStatus.Approved)
                     return RedirectToAction("MessagePage", "Account", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_APPROVE), Field = Resource.Expense });/*DIF*/
                  else
                     return RedirectToAction("MessagePage", "Account", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_REJECT), Field = Resource.Expense });/*DIF*/
               }
               #endregion
            }
         }
         else
         {
            /* approve canncel workflow*/
            if (model.Request_Cancel_ID.HasValue && model.Request_Cancel_ID.Value > 0)
            {
               #region Workflow
               var action = new ActionItem();
               action.Actioner_Profile_ID = userlogin.Profile_ID;
               action.Email = userlogin.User_Authentication.Email_Address;
               action.Name = UserSession.GetUserName(userlogin);
               action.Request_ID = model.Request_Cancel_ID.Value;
               if (pStatus == WorkflowStatus.Cancelled)/*DIF*/
               {
                  action.IsApprove = true;
                  action.Action = WorkflowAction.Approve;
               }
               else
               {
                  if (string.IsNullOrEmpty(model.Remark_Rej))
                  {
                     ModelState.AddModelError("Remark_Rej", Resource.The + " " + Resource.Remark + " " + Resource.Field + " " + Resource.Is_Rrequired_Lower);
                     model.isRejectPopUp = true;
                  }
                  else
                     model.isRejectPopUp = false;

                  action.IsApprove = false;
                  action.Remarks = model.Remark_Rej;
                  action.Action = WorkflowAction.Reject;
               }

               if (ModelState.IsValid)
               {
                  var r = aService.SubmitRequestAction(action);
                  if (r.IsSuccess)
                  {
                    
                     ex.Approver = getApprovalStrIDs(ex.Approver, userlogin.Profile_ID.ToString());
                     if (action.Status == WorkflowStatus.Closed)
                     {
                        ex.Next_Approver = null;
                        ex.Cancel_Status = WorkflowStatus.Cancelled;
                        model.result = eService.updateExpenseApplication(ex);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                        {
                           uService.ExpireActivationByPrefix(Ac_Code);
                           sendProceedEmail(ex, com, user, userlogin, hist, WorkflowStatus.Cancelled, null);
                           return RedirectToAction("MessagePage", "Account", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_APPROVE), Field = Resource.Expense });/*DIF*/
                        }
                     }
                     else if (action.Status == WorkflowStatus.Rejected)
                     {
                        ex.Next_Approver = null;
                        ex.Cancel_Status = WorkflowStatus.Cancellation_Rejected;
                        model.result = eService.updateExpenseApplication(ex);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                        {
                           uService.ExpireActivationByPrefix(Ac_Code);
                           sendProceedEmail(ex, com, user, userlogin, hist, WorkflowStatus.Cancellation_Rejected, null);
                           return RedirectToAction("MessagePage", "Account", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL_REJECT), Field = Resource.Expense });/*DIF*/
                        }
                     }
                     else
                     {
                        var nextappstr = "";
                        if (action.NextApprover == null)
                        {
                           #region Indent flow
                           var haveSendRequestEmail = false;
                           if (action.IndentValue != null && action.IndentValue.IsIndent && action.IndentValue.SendRequest)
                              haveSendRequestEmail = true;

                           if (haveSendRequestEmail)
                           {
                              ex.Next_Approver = null;
                              List<IndentItem> IndentItems = getIndentSupervisor(ex.Expenses_Application_ID);
                              if (IndentItems != null && IndentItems.Count > 0)
                              {
                                 foreach (var row in IndentItems)
                                 {

                                    if (action.Actioner_Profile_ID == row.Requestor_Profile_ID)
                                       continue;

                                    var param = new Dictionary<string, object>();
                                    param.Add("expID", ex.Expenses_Application_ID);
                                    param.Add("appID", row.Requestor_Profile_ID);
                                    param.Add("empID", ex.Employee_Profile_ID);
                                    param.Add("reqcancelID", action.Request_ID);
                                    param.Add("cancelStatus", WorkflowStatus.Cancelled);
                                    param.Add("code", uService.GenActivateCode("E" + ex.Expenses_Application_ID + row.Requestor_Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));
                                    var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                                    param["cancelStatus"] = WorkflowStatus.Rejected;
                                    var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                                    var appr = uService.getUser(row.Requestor_Profile_ID, false);
                                    if (appr != null)
                                       sendRequestEmail(ex, com, appr, user, hist, ex.Cancel_Status, null, linkApp, linkRej);

                                    nextappstr = getApprovalStrIDs(nextappstr, row.Requestor_Profile_ID.ToString());
                                 }
                              }
                           }
                           else
                           {
                              var str = getApprovalStrIDs(null, userlogin.Profile_ID.ToString());
                              if (!string.IsNullOrEmpty(str))
                                 nextappstr = ex.Next_Approver.Replace(str, "|");
                           }
                           #endregion
                        }
                        else
                        {
                           #region Normal flow
                           var param = new Dictionary<string, object>();
                           param.Add("expID", ex.Expenses_Application_ID);
                           param.Add("appID", action.NextApprover.Profile_ID);
                           param.Add("empID", ex.Employee_Profile_ID);
                           param.Add("reqcancelID", action.Request_ID);
                           param.Add("cancelStatus", WorkflowStatus.Cancelled);
                           param.Add("code", uService.GenActivateCode("E" + ex.Expenses_Application_ID + action.NextApprover.Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));
                           var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                           param["cancelStatus"] = WorkflowStatus.Rejected;
                           var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                           var appr = uService.getUser(action.NextApprover.Profile_ID, false);
                           if (appr != null)
                              sendRequestEmail(ex, com, appr, user, hist, ex.Cancel_Status, null, linkApp, linkRej);

                           nextappstr = getApprovalStrIDs(null, action.NextApprover.Profile_ID.ToString());
                           #endregion
                        }

                        ex.Next_Approver = nextappstr;
                        model.result = eService.updateExpenseApplication(ex);

                        uService.ExpireActivationByPrefix(Ac_Code);
                        return RedirectToAction("MessagePage", "Account", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_APPROVE), Field = Resource.Expense });/*DIF*/
                     }
                  }
               }
               #endregion
            }
            else if (hist.Supervisor.HasValue && hist.Supervisor.Value > 0)
            {
               #region Supervisor
               /*cancel approval by supervisor*/
               if (pStatus == WorkflowStatus.Cancelled)/*DIF*/
                  ex.Cancel_Status = WorkflowStatus.Cancelled;
               else
                  ex.Cancel_Status = WorkflowStatus.Cancellation_Rejected;

               ex.Next_Approver = null;
               ex.Approver = getApprovalStrIDs(null, userlogin.Profile_ID.ToString());
               model.result = eService.updateExpenseApplication(ex);
               if (model.result.Code == ERROR_CODE.SUCCESS)
               {
                  uService.ExpireActivationByPrefix(Ac_Code);
                  sendProceedEmail(ex, com, user, userlogin, hist, ex.Cancel_Status, null);
                  if (pStatus == WorkflowStatus.Cancelled)/*DIF*/
                     return RedirectToAction("MessagePage", "Account", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL), Field = Resource.Expense });/*DIF*/
                  else
                     return RedirectToAction("MessagePage", "Account", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL_REJECT), Field = Resource.Expense });/*DIF*/
               }
               #endregion
            }
         }
         return RedirectToAction("MessagePage", "Account", new { Code = ERROR_CODE.ERROR_504_UPDATE_ERROR, Msg = new Success().getSuccess(ERROR_CODE.ERROR_504_UPDATE_ERROR), Field = Resource.Expense }); /*DIF*/
      }
      #endregion
   }
}