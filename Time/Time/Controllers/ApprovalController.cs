using CivinTecAccessManager;
using SBSModel.Common;
using SBSModel.Models;
using SBSResourceAPI;
using SBSTimeModel.Common;
using SBSTimeModel.Models;
using SBSWorkFlowAPI.Constants;
using SBSWorkFlowAPI.Models;
using SBSWorkFlowAPI.ModelsAndService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Time.Models;


namespace Time.Controllers
{
   public class ApprovalController : ControllerBase
   {

      # region For Test  Approval Time Sheet
      public ActionResult ProcessWorkflow(string appID, string empID, string profileID, string reqID, string reqcancelID, string status, string code, string cancelStatus, string tsID)
      {

         var Time_Sheet_ID = NumUtil.ParseInteger(EncryptUtil.Decrypt(tsID));
         var Ac_Code = EncryptUtil.Decrypt(code);
         var Request_ID = NumUtil.ParseInteger(EncryptUtil.Decrypt(reqID));
         var Employee_Profile_ID = NumUtil.ParseInteger(EncryptUtil.Decrypt(empID));
         var Profile_ID = NumUtil.ParseInteger(EncryptUtil.Decrypt(profileID));
         var Overall_Status = EncryptUtil.Decrypt(status);
         var Cancel_Status = EncryptUtil.Decrypt(cancelStatus);
         var Approver_ID = NumUtil.ParseInteger(EncryptUtil.Decrypt(appID));
         var Request_Cancel_ID = NumUtil.ParseInteger(EncryptUtil.Decrypt(reqcancelID));

         var userlogin = UserUtil.getUser(HttpContext);
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

         if (Time_Sheet_ID > 0)
            return ProcessTimeSheet(Time_Sheet_ID, userlogin, Approver_ID, Employee_Profile_ID, Profile_ID, Request_ID, Overall_Status, Ac_Code, Request_Cancel_ID, Cancel_Status);

         return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);
      }
      #endregion


      # region Approval Time Sheet
      private ActionResult ProcessTimeSheet(int Time_Sheet_ID, User_Profile userlogin, int Approver_ID, int Employee_Profile_ID, int Profile_ID, int Request_ID, string Overall_Status, string Ac_Code, int Request_Cancel_ID, string Cancel_Status)
      {
         if (Time_Sheet_ID == 0)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Time_Sheet);

         var status = ""; //can be Approved, Rejected, Cancelled
         if (string.IsNullOrEmpty(Cancel_Status))
            status = Overall_Status;
         else
            status = Cancel_Status;

         var model = new TimeSheetViewModel();
         model.Time_Sheet_ID = Time_Sheet_ID;
         model.Request_ID = Request_ID;
         model.Remark_Rej = "Reject from email";
         model.Request_Cancel_ID = Request_Cancel_ID;
         model.Employee_Profile_ID = Employee_Profile_ID;
         model.Overall_Status = Overall_Status;
         model.Cancel_Status = Cancel_Status;

         return ApplicationMngt(model, status);
      }

      private ActionResult ApplicationMngt(TimeSheetViewModel model, string pStatus)
      {

         var userlogin = UserUtil.getUser(HttpContext);
         if (userlogin == null)
            return errorPage(ERROR_CODE.ERROR_401_UNAUTHORIZED);

         var tsService = new TimeSheetService(userlogin);
         var cbService = new ComboService();
         var empService = new EmployeeService();
         var hService = new EmploymentHistoryService();
         var uService = new UserService();
         var currentdate = StoredProcedure.GetCurrentDate();
         var cpService = new CompanyService();
         var aService = new SBSWorkFlowAPI.Service();
         var jService = new JobCostService();

         var com = cpService.GetCompany(userlogin.Company_ID);
         if (com == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Company);

         var ts = tsService.GetTimeSheet(model.Time_Sheet_ID);
         if (ts == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Time_Sheet);

         if (ts.Cancel_Status == WorkflowStatus.Cancelled)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Time_Sheet);

         if (ts.Overall_Status == WorkflowStatus.Rejected)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Time_Sheet);

         var hist = hService.GetCurrentEmploymentHistory(ts.Employee_Profile_ID);
         if (hist == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.Employment_History);

         var user = uService.getUser(hist.Employee_Profile.Profile_ID, false);
         if (user == null)
            return errorPage(ERROR_CODE.ERROR_511_DATA_NOT_FOUND, Resource.User);

         ts.Update_By = userlogin.User_Authentication.Email_Address;
         ts.Update_On = currentdate;
         var status = pStatus;
         var Ac_Code = "T" + ts.Time_Sheet_ID + userlogin.Profile_ID + "_";
         if (string.IsNullOrEmpty(ts.Cancel_Status))
         {
            if (model.Request_ID.HasValue && model.Request_ID.Value > 0)
            {
               #region Workflow
               var action = new ActionItem();
               action.Actioner_Profile_ID = userlogin.Profile_ID;
               action.Email = userlogin.User_Authentication.Email_Address;
               action.Name = AppConst.GetUserName(userlogin);
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
                     if (action.Status == WorkflowStatus.Closed)
                     {
                        ts.Overall_Status = WorkflowStatus.Closed;
                        model.result = tsService.UpdateTimeSheet(ts);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                        {
                           jService.CalCosting(ts.Job_Cost_ID, ts.Total_Amount);
                           uService.ExpireActivationByPrefix(Ac_Code);
                           sendProceedEmail(ts, com, user, userlogin, hist, action.Status, null);
                           return RedirectToAction("MessagePage", "Account", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_APPROVE), Field = Resource.Time_Sheet });
                        }
                     }
                     else if (action.Status == WorkflowStatus.Rejected)
                     {
                        ts.Overall_Status = WorkflowStatus.Rejected;
                        model.result = tsService.UpdateTimeSheet(ts);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                        {
                           uService.ExpireActivationByPrefix(Ac_Code);
                           sendProceedEmail(ts, com, user, userlogin, hist, action.Status, null);
                           return RedirectToAction("MessagePage", "Account", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_REJECT), Field = Resource.Time_Sheet });
                        }
                     }
                     else
                     {
                        if (action.NextApprover == null)
                        {
                           #region Indent flow
                           var haveSendRequestEmail = false;
                           if (action.IndentValue != null && action.IndentValue.IsIndent && action.IndentValue.SendRequest)
                              haveSendRequestEmail = true;

                           if (haveSendRequestEmail)
                           {
                              List<IndentItem> IndentItems = getIndentSupervisor(ts.Job_Cost_ID.HasValue ? ts.Job_Cost_ID.Value : 0);
                              if (IndentItems != null && IndentItems.Count > 0)
                              {
                                 foreach (var row in IndentItems)
                                 {
                                    if (action.Actioner_Profile_ID == row.Requestor_Profile_ID)
                                       continue;

                                    var param = new Dictionary<string, object>();
                                    param.Add("tsID", ts.Time_Sheet_ID);
                                    param.Add("appID", row.Requestor_Profile_ID);
                                    param.Add("empID", ts.Employee_Profile_ID);
                                    param.Add("reqID", ts.Request_ID);
                                    param.Add("status", WorkflowStatus.Approved);
                                    param.Add("code", uService.GenActivateCode("T" + ts.Time_Sheet_ID + row.Requestor_Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));

                                    var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                                    param["status"] = WorkflowStatus.Rejected;
                                    var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                                    var appr = uService.getUser(row.Requestor_Profile_ID, false);
                                    if (appr != null)
                                       sendRequestEmail(ts, com, appr, user, hist, WorkflowStatus.Submitted, null, linkApp, linkRej);
                                 }
                              }
                           }
                           #endregion
                        }
                        else
                        {
                           #region Normal flow
                           var param = new Dictionary<string, object>();
                           param.Add("tsID", ts.Time_Sheet_ID);
                           param.Add("appID", action.NextApprover.Profile_ID);
                           param.Add("empID", ts.Employee_Profile_ID);
                           param.Add("reqID", ts.Request_ID);
                           param.Add("status", WorkflowStatus.Approved);
                           param.Add("code", uService.GenActivateCode("T" + ts.Time_Sheet_ID + action.NextApprover.Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));

                           var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                           param["status"] = WorkflowStatus.Rejected;
                           var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                           var appr = uService.getUser(action.NextApprover.Profile_ID, false);
                           if (appr != null)
                              sendRequestEmail(ts, com, appr, user, hist, WorkflowStatus.Submitted, null, linkApp, linkRej);
                           #endregion
                        }
                        uService.ExpireActivationByPrefix(Ac_Code);
                        return RedirectToAction("MessagePage", "Account", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_APPROVE), Field = Resource.Time_Sheet });
                     }
                  }
               }
               #endregion
            }
            else if (hist.Supervisor.HasValue && hist.Supervisor > 0)
            {
               /*approval by supervisor*/
               #region Supervisor
               if (pStatus == WorkflowStatus.Approved)
                  ts.Overall_Status = WorkflowStatus.Closed;
               else
                  ts.Overall_Status = WorkflowStatus.Rejected;

               model.result = tsService.UpdateTimeSheet(ts);
               if (model.result.Code == ERROR_CODE.SUCCESS)
               {
                  uService.ExpireActivationByPrefix(Ac_Code);
                  sendProceedEmail(ts, com, user, userlogin, hist, ts.Overall_Status, null);
                  if (pStatus == WorkflowStatus.Approved)
                  {
                     jService.CalCosting(ts.Job_Cost_ID, ts.Total_Amount);
                     return RedirectToAction("MessagePage", "Account", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_APPROVE), Field = Resource.Time_Sheet });
                  }
                  else
                     return RedirectToAction("MessagePage", "Account", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_REJECT), Field = Resource.Time_Sheet });
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
               action.Name = AppConst.GetUserName(userlogin);
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
                     if (action.Status == WorkflowStatus.Closed)
                     {
                        ts.Cancel_Status = WorkflowStatus.Cancelled;
                        model.result = tsService.UpdateTimeSheet(ts);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                        {
                           jService.CalCosting(ts.Job_Cost_ID, -1 * ts.Total_Amount);
                           uService.ExpireActivationByPrefix(Ac_Code);
                           sendProceedEmail(ts, com, user, userlogin, hist, WorkflowStatus.Cancelled, null);
                           return RedirectToAction("MessagePage", "Account", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_APPROVE), Field = Resource.Time_Sheet });
                        }
                     }
                     else if (action.Status == WorkflowStatus.Rejected)
                     {
                        ts.Cancel_Status = WorkflowStatus.Cancellation_Rejected;
                        model.result = tsService.UpdateTimeSheet(ts);
                        if (model.result.Code == ERROR_CODE.SUCCESS)
                        {
                           uService.ExpireActivationByPrefix(Ac_Code);
                           sendProceedEmail(ts, com, user, userlogin, hist, WorkflowStatus.Cancellation_Rejected, null);
                           return RedirectToAction("MessagePage", "Account", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL_REJECT), Field = Resource.Time_Sheet });
                        }
                     }
                     else
                     {
                        if (action.NextApprover == null)
                        {
                           #region Indent flow
                           var haveSendRequestEmail = false;
                           if (action.IndentValue != null && action.IndentValue.IsIndent && action.IndentValue.SendRequest)
                              haveSendRequestEmail = true;

                           if (haveSendRequestEmail)
                           {
                              List<IndentItem> IndentItems = getIndentSupervisor(ts.Job_Cost_ID.HasValue ? ts.Job_Cost_ID.Value : 0);
                              if (IndentItems != null && IndentItems.Count > 0)
                              {
                                 foreach (var row in IndentItems)
                                 {
                                    if (action.Actioner_Profile_ID == row.Requestor_Profile_ID)
                                       continue;

                                    var param = new Dictionary<string, object>();
                                    param.Add("tsID", ts.Time_Sheet_ID);
                                    param.Add("appID", row.Requestor_Profile_ID);
                                    param.Add("empID", ts.Employee_Profile_ID);
                                    param.Add("reqcancelID", action.Request_ID);
                                    param.Add("cancelStatus", WorkflowStatus.Cancelled);
                                    param.Add("code", uService.GenActivateCode("T" + ts.Time_Sheet_ID + row.Requestor_Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));

                                    var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                                    param["cancelStatus"] = WorkflowStatus.Rejected;
                                    var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                                    var appr = uService.getUser(row.Requestor_Profile_ID, false);
                                    if (appr != null)
                                       sendRequestEmail(ts, com, appr, user, hist, ts.Cancel_Status, null, linkApp, linkRej);
                                 }
                              }
                           }
                           #endregion
                        }
                        else
                        {
                           #region Normal flow
                           var param = new Dictionary<string, object>();
                           param.Add("tsID", ts.Time_Sheet_ID);
                           param.Add("appID", action.NextApprover.Profile_ID);
                           param.Add("empID", ts.Employee_Profile_ID);
                           param.Add("reqcancelID", action.Request_ID);
                           param.Add("cancelStatus", WorkflowStatus.Cancelled);
                           param.Add("code", uService.GenActivateCode("T" + ts.Time_Sheet_ID + action.NextApprover.Profile_ID + "_", userlogin.User_Authentication.User_Authentication_ID));
                           var linkApp = GenerateActionLink("Approval", "ProcessWorkflow", param);
                           param["cancelStatus"] = WorkflowStatus.Rejected;
                           var linkRej = GenerateActionLink("Approval", "ProcessWorkflow", param);

                           var appr = uService.getUser(action.NextApprover.Profile_ID, false);
                           if (appr != null)
                              sendRequestEmail(ts, com, appr, user, hist, ts.Cancel_Status, null, linkApp, linkRej);
                           #endregion
                        }

                        uService.ExpireActivationByPrefix(Ac_Code);
                        return RedirectToAction("MessagePage", "Account", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_APPROVE), Field = Resource.Time_Sheet });
                     }
                  }
               }
               #endregion
            }
            else if (hist.Supervisor.HasValue && hist.Supervisor > 0)
            {
               #region Supervisor
               /*cancel approval by supervisor*/
               if (pStatus == WorkflowStatus.Cancelled)/*DIF*/
                  ts.Cancel_Status = WorkflowStatus.Cancelled;
               else
                  ts.Cancel_Status = WorkflowStatus.Cancellation_Rejected;

               model.result = tsService.UpdateTimeSheet(ts);
               if (model.result.Code == ERROR_CODE.SUCCESS)
               {
                  uService.ExpireActivationByPrefix(Ac_Code);
                  sendProceedEmail(ts, com, user, userlogin, hist, ts.Cancel_Status, null);
                  if (pStatus == WorkflowStatus.Cancelled)/*DIF*/
                  {
                     jService.CalCosting(ts.Job_Cost_ID, -1 * ts.Total_Amount);
                     return RedirectToAction("MessagePage", "Account", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL), Field = Resource.Time_Sheet });
                  }
                  else
                     return RedirectToAction("MessagePage", "Account", new { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_CANCEL_REJECT), Field = Resource.Time_Sheet });
               }
               #endregion
            }
         }

         //-------request------------
         model.JobCostlst = cbService.LstJobCost(userlogin.Company_ID, false);
         if (model.Request_ID.HasValue)
         {
            var r = aService.GetMyRequests(userlogin.Company_ID, null, null, ModuleCode.Time, ApprovalType.TimeSheet, model.Time_Sheet_ID);
            if (r.Item2.IsSuccess && r.Item1 != null)
               model.Time_Sheet_Request = r.Item1;
         }

         //-------rights------------
         RightResult rightResult = base.validatePageRight(Operation.A);
         if (rightResult.action != null)
            return rightResult.action;
         model.rights = rightResult.rights;

         return View(model);
      }

      #endregion
   }
}