using System;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Configuration;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;

using SBSWorkFlowAPI.Models;
using SBSWorkFlowAPI.Constants;
using SBSWorkFlowAPI.ModelsAndService;

namespace SBSWorkFlowAPI
{
   public class Service
   {

      #region Workflow Functions

      public ReturnValue CreateWorkFlow(Approval_Flow approvalFlow)
      {
         ReturnValue rValue = new ReturnValue();

         try
         {

            approvalFlow.Record_Status = WfRecordStatus.Active;

            if (createWorkflow(approvalFlow))
            {
               rValue.IsSuccess = true;
               rValue.Message = "Workflow Created Successfully";
            }

         }
         catch (Exception ex)
         {
            rValue.IsSuccess = false;
            rValue.Message = ex.Message;
            rValue.Exception = ex;
         }

         return rValue;
      }

      public ReturnValue UpdateWorkFlow(Approval_Flow approvalFlow)
      {
         ReturnValue rValue = new ReturnValue();

         try
         {

            if (updateWorkflow(approvalFlow))
            {
               rValue.IsSuccess = true;
               rValue.Message = "Workflow Updated Successfully";
            }

         }
         catch (Exception ex)
         {
            rValue.IsSuccess = false;
            rValue.Message = ex.Message;
            rValue.Exception = ex;
         }

         return rValue;
      }

      public ReturnValue DeleteWorkFlow(Nullable<int> pWorkflowID, Nullable<int> pUserProfileID)
      {
         ReturnValue rValue = new ReturnValue();

         try
         {

            if (deleteWorkflow(pWorkflowID, pUserProfileID))
            {
               rValue.IsSuccess = true;
               rValue.Message = "Workflow Deleted Successfully";
            }

         }
         catch (Exception ex)
         {
            rValue.IsSuccess = false;
            rValue.Message = ex.Message;
            rValue.Exception = ex;
         }

         return rValue;
      }

      //Added by sun 19-10-2015
      //UpdateDeleteStatus
      public ReturnValue UpdateDeleteWorkFlowStatus(Nullable<int> pWorkflowID, Nullable<int> pUserProfileID, string pStatus)
      {
         ReturnValue rValue = new ReturnValue();
         try
         {
            if (UpdatedeleteWorkflow(pWorkflowID, pStatus, pUserProfileID))
            {
               rValue.IsSuccess = true;
               rValue.Message = "Workflow Deleted Successfully";
            }
         }
         catch (Exception ex)
         {
            rValue.IsSuccess = false;
            rValue.Message = ex.Message;
            rValue.Exception = ex;
         }

         return rValue;
      }

      private bool UpdatedeleteWorkflow(Nullable<int> pWorkflowID, string pStatus, Nullable<int> pUserProfileID)
      {
         var wflService = new WorkflowService();
         try
         {
            return wflService.UpdatedeleteDelete_WorkFlow(Convert.ToInt32(pWorkflowID), Convert.ToInt32(pUserProfileID), pStatus);

         }
         catch (Exception ex)
         {
            throw ex;
         }
      }

      public Tuple<List<Approval_Flow>, ReturnValue> GetWorkflowByDepartment(Nullable<int> pCompany_ID, Nullable<int> pDepartment_ID, string pModule, string pApproval_Type)
      {
         List<Approval_Flow> workflows = null;
         ReturnValue rValue = new ReturnValue();

         try
         {
            workflows = getWorkFlows(pCompany_ID, pDepartment_ID, null, pModule, pApproval_Type);
            rValue.IsSuccess = true;
            rValue.Message = "GetWorkflowByDepartment Successful";

         }
         catch (Exception ex)
         {
            rValue.IsSuccess = false;
            rValue.Message = ex.Message;
            rValue.Exception = ex;
         }

         return new Tuple<List<Approval_Flow>, ReturnValue>(workflows, rValue);
      }

      public Tuple<List<Approval_Flow>, ReturnValue> GetWorkflowByEmployee(Nullable<int> pCompany_ID, Nullable<int> pProfile_ID, string pModule, string pApproval_Type, Nullable<int> pDepartmentID)
      {
         List<Approval_Flow> workflows = null;
         ReturnValue rValue = new ReturnValue();

         try
         {
            workflows = getWorkFlows(pCompany_ID, pDepartmentID, pProfile_ID, pModule, pApproval_Type);
            rValue.IsSuccess = true;
            rValue.Message = "GetWorkflowByDepartment Successful";

         }
         catch (Exception ex)
         {
            rValue.IsSuccess = false;
            rValue.Message = ex.Message;
            rValue.Exception = ex;
         }

         return new Tuple<List<Approval_Flow>, ReturnValue>(workflows, rValue);
      }

      public Tuple<List<Approval_Flow>, ReturnValue> GetWorkflowByCompany(Nullable<int> pCompany_ID, string pModule, string pApproval_Type)
      {
         List<Approval_Flow> workflows = null;
         ReturnValue rValue = new ReturnValue();

         try
         {
            workflows = getWorkFlows(pCompany_ID, null, null, pModule, pApproval_Type);
            rValue.IsSuccess = true;
            rValue.Message = "GetWorkflowByCompany Successful";

         }
         catch (Exception ex)
         {
            rValue.IsSuccess = false;
            rValue.Message = ex.Message;
            rValue.Exception = ex;
         }

         return new Tuple<List<Approval_Flow>, ReturnValue>(workflows, rValue);
      }

      public Tuple<Approval_Flow, ReturnValue> GetWorkflow(Nullable<int> pApproval_ID)
      {
         Approval_Flow workflow = null;
         ReturnValue rValue = new ReturnValue();

         try
         {
            var wflService = new WorkflowService();
            workflow = wflService.GetApprovalFlowByApprovalID(pApproval_ID);
            rValue.IsSuccess = true;
            rValue.Message = "GetApprovalFlowByApprovalID Successful";

         }
         catch (Exception ex)
         {
            rValue.IsSuccess = false;
            rValue.Message = ex.Message;
            rValue.Exception = ex;
         }

         return new Tuple<Approval_Flow, ReturnValue>(workflow, rValue);
      }

      #endregion

      #region Request Functions

      //Added by sun 16-02-2016
      public ReturnValue SubmitRequestCanceling(RequestItem requestItem)
      {
         ReturnValue rValue = new ReturnValue();

         try
         {
            createNewRequestCanceling(ref requestItem);
            rValue.IsSuccess = true;
            rValue.Message = "Request Canceling Successfully";

         }
         catch (Exception ex)
         {
            rValue.IsSuccess = false;
            rValue.Message = ex.Message;
            rValue.Exception = ex;
         }

         return rValue;
      }

      public ReturnValue SubmitRequest(RequestItem requestItem)
      {
         ReturnValue rValue = new ReturnValue();

         try
         {
            createNewRequest(ref requestItem);
            rValue.IsSuccess = true;
            rValue.Message = "Request Submitted Successfully";

         }
         catch (Exception ex)
         {
            rValue.IsSuccess = false;
            rValue.Message = ex.Message;
            rValue.Exception = ex;
         }

         return rValue;
      }

      public ReturnValue SubmitRequest(ManualRequestItem requestItem)
      {
         ReturnValue rValue = new ReturnValue();

         try
         {
            createManualRequest(ref requestItem);
            rValue.IsSuccess = true;
            rValue.Message = "Request Submitted Successfully";
         }
         catch (Exception ex)
         {
            rValue.IsSuccess = false;
            rValue.Message = ex.Message;
            rValue.Exception = ex;
         }

         return rValue;
      }

      public Tuple<List<Request>, ReturnValue> GetMyRequests(Nullable<int> vCompany_ID, Nullable<int> vDepartment_ID, Nullable<int> vProfile_ID, string vModule, string vApproval_Type, Nullable<int> vDoc_ID = null)
      {
         List<Request> requests = null;
         ReturnValue rValue = new ReturnValue();

         try
         {
            requests = new List<Request>();
            requests = getRequests(vCompany_ID, vDepartment_ID, vProfile_ID, vModule, vApproval_Type, vDoc_ID);
            rValue.IsSuccess = true;
            rValue.Message = "GetMyRequests Successful";

         }
         catch (Exception ex)
         {
            rValue.IsSuccess = false;
            rValue.Message = ex.Message;
            rValue.Exception = ex;
         }

         return new Tuple<List<Request>, ReturnValue>(requests, rValue);
      }

      public Tuple<Request, ReturnValue> GetMyRequest(Nullable<int> vRequest_ID)
      {
         Request request = null;
         ReturnValue rValue = new ReturnValue();

         try
         {
            request = new Request();
            request = getRequest(vRequest_ID);
            rValue.IsSuccess = true;
            rValue.Message = "GetMyRequest Successful";

         }
         catch (Exception ex)
         {
            rValue.IsSuccess = false;
            rValue.Message = ex.Message;
            rValue.Exception = ex;
         }

         return new Tuple<Request, ReturnValue>(request, rValue);
      }


      public Tuple<List<Request>, ReturnValue> GetMyTasks(Nullable<int> vCompany_ID, Nullable<int> vDepartment_ID, Nullable<int> vProfile_ID, string vModule, string vApproval_Type, string vStatus)
      {
         List<Request> requests = null;
         ReturnValue rValue = new ReturnValue();

         try
         {
            requests = new List<Request>();
            requests = getTasks(vCompany_ID, vDepartment_ID, vProfile_ID, vModule, vApproval_Type, vStatus);
            rValue.IsSuccess = true;
            rValue.Message = "GetMyTasks Successful";

         }
         catch (Exception ex)
         {
            rValue.IsSuccess = false;
            rValue.Message = ex.Message;
            rValue.Exception = ex;
         }

         return new Tuple<List<Request>, ReturnValue>(requests, rValue);
      }

      public ReturnValue SubmitRequestAction(ActionItem actionItem)
      {
         ReturnValue rValue = new ReturnValue();

         try
         {
            if (approveOrRejectRequest(actionItem))
            {
               rValue.IsSuccess = true;
               rValue.Message = "Action Submitted Successfully";
            }

         }
         catch (Exception ex)
         {
            rValue.IsSuccess = false;
            rValue.Message = ex.Message;
            rValue.Exception = ex;
         }

         return rValue;
      }

      public ReturnValue CancelRequest(ActionItem actionItem)
      {
         ReturnValue rValue = new ReturnValue();

         try
         {

            actionItem.Action = WorkflowAction.Cancel;

            if (cancelorCloseRequest(actionItem))
            {
               rValue.IsSuccess = true;
               rValue.Message = "Request Cancelled Successfully";
            }

         }
         catch (Exception ex)
         {
            rValue.IsSuccess = false;
            rValue.Message = ex.Message;
            rValue.Exception = ex;
         }

         return rValue;
      }

      public ReturnValue CloseRequest(ActionItem actionItem)
      {
         ReturnValue rValue = new ReturnValue();

         try
         {

            actionItem.Action = WorkflowAction.Close;

            if (cancelorCloseRequest(actionItem))
            {
               rValue.IsSuccess = true;
               rValue.Message = "Request Closed Successfully";
            }

         }
         catch (Exception ex)
         {
            rValue.IsSuccess = false;
            rValue.Message = ex.Message;
            rValue.Exception = ex;
         }

         return rValue;
      }

      public ReturnValue IsMyTask(Nullable<int> vRequest_ID, Nullable<int> vProfile_ID)
      {

         ReturnValue rValue = new ReturnValue();

         try
         {

            if (isMyTask(vRequest_ID, vProfile_ID))
            {
               rValue.IsSuccess = true;
               rValue.Message = "Task Found.";
            }
            else
            {
               rValue.IsSuccess = false;
               rValue.Message = "Task Not Found.";
            }

         }
         catch (Exception ex)
         {
            rValue.IsSuccess = false;
            rValue.Message = ex.Message;
            rValue.Exception = ex;
         }

         return rValue;
      }

      #endregion

      #region Private Functions
      private bool createWorkflow(Approval_Flow pApprovalFlow)
      {
         var wflService = new WorkflowService();
         try
         {
            return wflService.Create_WorkFlow(pApprovalFlow);
         }
         catch (Exception ex)
         {
            throw ex;
         }
      }

      private bool updateWorkflow(Approval_Flow pApprovalFlow)
      {
         var wflService = new WorkflowService();
         try
         {
            return wflService.Update_WorkFlow(pApprovalFlow);
         }
         catch (Exception ex)
         {
            throw ex;
         }
      }

      private bool deleteWorkflow(Nullable<int> pWorkflowID, Nullable<int> pUserProfileID)
      {
         var wflService = new WorkflowService();
         try
         {
            return wflService.Delete_WorkFlow(Convert.ToInt32(pWorkflowID),
              Convert.ToInt32(pUserProfileID));

         }
         catch (Exception ex)
         {
            throw ex;
         }
      }

      private void createNewRequest(ref RequestItem pRequestItem)
      {
         var wflService = new WorkflowService();
         try
         {
            List<Reviewer> lstReviewer = new List<Reviewer>();
            DateTime actionTime = Utils.GetDateTimeNow();

            var approvalFlow = wflService.GetApprovalFlow(
                pRequestItem.Module,
                pRequestItem.Approval_Type,
                pRequestItem.Company_ID,
                pRequestItem.Department_ID,
                pRequestItem.Requestor_Profile_ID);

            if (approvalFlow != null)
            {
               Request newRequest = new Models.Request()
               {
                  Doc_ID = pRequestItem.Doc_ID,
                  Approval_Flow_ID = approvalFlow.Approval_Flow_ID,
                  Company_ID = pRequestItem.Company_ID,
                  Requestor_Profile_ID = pRequestItem.Requestor_Profile_ID,
                  Requestor_Name = pRequestItem.Requestor_Name,
                  Requestor_Email = pRequestItem.Requestor_Email,
                  Approval_Level = 1,
                  Status = WorkflowStatus.Pending,
                  Request_Date = actionTime,
                  Last_Action_Date = actionTime,
                  Approval_Type = pRequestItem.Approval_Type,
                  Module = pRequestItem.Module
               };

               newRequest.Histories.Add(new History()
               {
                  Action = WorkflowAction.Submit,
                  Profile_ID = pRequestItem.Requestor_Profile_ID,
                  Action_By = pRequestItem.Requestor_Name,
                  Action_Email = pRequestItem.Requestor_Email,
                  Action_On = actionTime
               });

               var nextApprover = "";
               var diff = false;
               var appcnt = 0;
               var IsIndent = false;

               var haveIndent = false;
               if (approvalFlow.Approval_Type == ApprovalType.Expense || approvalFlow.Approval_Type == ApprovalType.TimeSheet)
                  haveIndent = true;

               foreach (var app in approvalFlow.Approvers.OrderBy(o => o.Approval_Level))
               {
                  if (app.Approval_Flow_Type == ApproverFlowType.Job_Cost && haveIndent)
                  {
                     #region Indent flow
                     IsIndent = false;
                     if (pRequestItem.Is_Indent)
                     {
                        foreach (var appIndent in pRequestItem.IndentItems)
                        {
                           if (appIndent.Requestor_Profile_ID == pRequestItem.Requestor_Profile_ID && !diff)
                           {
                              appcnt++;
                              appIndent.IsSuccess = true;
                              var task = new Task_Assignment()
                              {
                                 Profile_ID = appIndent.Requestor_Profile_ID,
                                 Name = appIndent.Requestor_Name,
                                 Email = appIndent.Requestor_Email,
                                 Approval_Level = app.Approval_Level,
                                 Record_Status = WfRecordStatus.InActive,
                                 Status = WorkflowStatus.Approved,
                                 Is_Indent = true,
                                 Indent_Closed = true
                              };

                              newRequest.Approval_Level = newRequest.Approval_Level + 1;
                              newRequest.Status = WorkflowStatus.Approved;
                              newRequest.Is_Indent = true;

                              if (appcnt == (pRequestItem.IndentItems.Count() + approvalFlow.Approvers.Count() - 1))
                              {
                                 task.Indent_Closed = true;
                                 task.Status = WorkflowStatus.Closed;
                                 newRequest.Status = WorkflowStatus.Closed;
                                 if (string.IsNullOrEmpty(nextApprover))
                                 {
                                    nextApprover = appIndent.Requestor_Email;
                                    pRequestItem.NextApprover = app;
                                    IsIndent = true;
                                 }
                              }
                              newRequest.Task_Assignment.Add(task);
                           }
                           else
                           {
                              //diff = true;
                              var task = new Task_Assignment()
                              {
                                 Profile_ID = appIndent.Requestor_Profile_ID,
                                 Name = appIndent.Requestor_Name,
                                 Email = appIndent.Requestor_Email,
                                 Approval_Level = app.Approval_Level,
                                 Record_Status = WfRecordStatus.Active,
                                 Is_Indent = true
                              };
                              newRequest.Is_Indent = true;
                              newRequest.Task_Assignment.Add(task);
                              if (string.IsNullOrEmpty(nextApprover))
                              {
                                 nextApprover = appIndent.Requestor_Email;
                                 pRequestItem.NextApprover = app;
                                 IsIndent = true;
                              }
                           }
                        }
                     }
                     else
                     {
                        if (appcnt == approvalFlow.Approvers.Where(w => w.Approval_Flow_Type != ApproverFlowType.Job_Cost).Count())
                           newRequest.Status = WorkflowStatus.Closed;
                     }
                     #endregion
                  }
                  else
                  {
                     #region Normal flow
                     if (app.Profile_ID == pRequestItem.Requestor_Profile_ID && !diff)
                     {
                        appcnt++;
                        var task = new Task_Assignment()
                        {
                           Profile_ID = app.Profile_ID,
                           Name = app.Name,
                           Email = app.Email,
                           Approval_Level = app.Approval_Level,
                           Record_Status = WfRecordStatus.InActive,
                           Status = WorkflowStatus.Approved
                        };
                        newRequest.Approval_Level = newRequest.Approval_Level + 1;
                        newRequest.Status = WorkflowStatus.Approved;
                        if (appcnt == approvalFlow.Approvers.Count())
                        {
                           task.Status = WorkflowStatus.Closed;
                           newRequest.Status = WorkflowStatus.Closed;
                           if (string.IsNullOrEmpty(nextApprover))
                           {
                              nextApprover = app.Email;
                              pRequestItem.NextApprover = app;
                              IsIndent = false;
                           }
                        }
                        newRequest.Task_Assignment.Add(task);
                     }
                     else
                     {
                        diff = true;
                        var task = new Task_Assignment()
                        {
                           Profile_ID = app.Profile_ID,
                           Name = app.Name,
                           Email = app.Email,
                           Approval_Level = app.Approval_Level,
                           Record_Status = WfRecordStatus.Active
                        };
                        newRequest.Task_Assignment.Add(task);
                        if (string.IsNullOrEmpty(nextApprover))
                        {
                           nextApprover = app.Email;
                           pRequestItem.NextApprover = app;
                           IsIndent = false;
                        }
                     }
                     #endregion
                  }
               }

               if (wflService.CreateRequest(newRequest))
               {
                  foreach (var rvw in approvalFlow.Reviewers)
                  {
                     lstReviewer.Add(new Reviewer()
                     {
                        Reviewer_ID = rvw.Reviewer_ID,
                        Approval_Flow_ID = rvw.Approval_Flow_ID,
                        Company_ID = rvw.Company_ID,
                        Profile_ID = rvw.Profile_ID,
                        Name = rvw.Name,
                        Email = rvw.Email
                     });
                  }

                  pRequestItem.Reviewers = lstReviewer;
                  pRequestItem.Request_ID = newRequest.Request_ID;
                  pRequestItem.Status = newRequest.Status;
                  pRequestItem.Is_Indent = IsIndent;
               }
            }
            else
            {
               throw new Exception("No Workflow found for Requestor : " + pRequestItem.Requestor_Name);
            }
         }
         catch (Exception ex)
         {
            throw ex;
         }

      }

      private void createNewRequestCanceling(ref RequestItem pRequestItem)
      {
         var wflService = new WorkflowService();
         try
         {
            List<Reviewer> lstReviewer = new List<Reviewer>();
            DateTime actionTime = Utils.GetDateTimeNow();

            var approvalFlow = wflService.GetApprovalFlow(
                pRequestItem.Module,
                pRequestItem.Approval_Type,
                pRequestItem.Company_ID,
                pRequestItem.Department_ID,
                pRequestItem.Requestor_Profile_ID);

            if (approvalFlow != null)
            {
               Request newRequest = new Models.Request()
               {
                  Doc_ID = pRequestItem.Doc_ID,
                  Approval_Flow_ID = approvalFlow.Approval_Flow_ID,
                  Company_ID = pRequestItem.Company_ID,
                  Requestor_Profile_ID = pRequestItem.Requestor_Profile_ID,
                  Requestor_Name = pRequestItem.Requestor_Name,
                  Requestor_Email = pRequestItem.Requestor_Email,
                  Approval_Level = 1,
                  Status = WorkflowStatus.Canceling,
                  Request_Date = actionTime,
                  Last_Action_Date = actionTime,
                  Approval_Type = pRequestItem.Approval_Type,
                  Module = pRequestItem.Module,
                  Request_Type = "Cancel"
               };

               newRequest.Histories.Add(new History()
               {
                  Action = WorkflowAction.Submit,
                  Profile_ID = pRequestItem.Requestor_Profile_ID,
                  Action_By = pRequestItem.Requestor_Name,
                  Action_Email = pRequestItem.Requestor_Email,
                  Action_On = actionTime
               });

               var nextApprover = "";
               var diff = false;
               var appcnt = 0;
               var IsIndent = false;

               var haveIndent = false;
               if (approvalFlow.Module == "HR" || approvalFlow.Module == "Time")
                  haveIndent = true;

               foreach (var app in approvalFlow.Approvers.OrderBy(o => o.Approval_Level))
               {
                  if (app.Approval_Flow_Type == ApproverFlowType.Job_Cost && haveIndent)
                  {
                     #region Indent flow
                     IsIndent = false;
                     if (pRequestItem.Is_Indent)
                     {
                        foreach (var appIndent in pRequestItem.IndentItems)
                        {
                           if (appIndent.Requestor_Profile_ID == pRequestItem.Requestor_Profile_ID && !diff)
                           {
                              appcnt++;
                              appIndent.IsSuccess = true;
                              var task = new Task_Assignment()
                              {
                                 Profile_ID = appIndent.Requestor_Profile_ID,
                                 Name = appIndent.Requestor_Name,
                                 Email = appIndent.Requestor_Email,
                                 Approval_Level = app.Approval_Level,
                                 Record_Status = WfRecordStatus.InActive,
                                 Status = WorkflowStatus.Approved,
                                 Is_Indent = true,
                                 Indent_Closed = true
                              };

                              newRequest.Approval_Level = newRequest.Approval_Level + 1;
                              newRequest.Status = WorkflowStatus.Approved;
                              newRequest.Is_Indent = true;

                              if (appcnt == (pRequestItem.IndentItems.Count() + approvalFlow.Approvers.Count() - 1))
                              {
                                 task.Indent_Closed = true;
                                 task.Status = WorkflowStatus.Closed;
                                 newRequest.Status = WorkflowStatus.Closed;
                                 if (string.IsNullOrEmpty(nextApprover))
                                 {
                                    nextApprover = appIndent.Requestor_Email;
                                    pRequestItem.NextApprover = app;
                                    IsIndent = true;
                                 }
                              }
                              newRequest.Task_Assignment.Add(task);
                           }
                           else
                           {
                              var task = new Task_Assignment()
                              {
                                 Profile_ID = appIndent.Requestor_Profile_ID,
                                 Name = appIndent.Requestor_Name,
                                 Email = appIndent.Requestor_Email,
                                 Approval_Level = app.Approval_Level,
                                 Record_Status = WfRecordStatus.Active,
                                 Is_Indent = true
                              };
                              newRequest.Is_Indent = true;
                              newRequest.Task_Assignment.Add(task);
                              if (string.IsNullOrEmpty(nextApprover))
                              {
                                 nextApprover = appIndent.Requestor_Email;
                                 pRequestItem.NextApprover = app;
                                 IsIndent = true;
                              }
                           }
                        }
                     }
                     else
                     {
                        if (appcnt == approvalFlow.Approvers.Where(w => w.Approval_Flow_Type != ApproverFlowType.Job_Cost).Count())
                           newRequest.Status = WorkflowStatus.Closed;
                     }
                     #endregion
                  }
                  else
                  {
                     #region Normal flow
                     if (app.Profile_ID == pRequestItem.Requestor_Profile_ID && !diff)
                     {
                        appcnt++;
                        var task = new Task_Assignment()
                        {
                           Profile_ID = app.Profile_ID,
                           Name = app.Name,
                           Email = app.Email,
                           Approval_Level = app.Approval_Level,
                           Record_Status = WfRecordStatus.InActive,
                           Status = WorkflowStatus.Approved
                        };
                        newRequest.Approval_Level = newRequest.Approval_Level + 1;
                        newRequest.Status = WorkflowStatus.Approved;
                        if (appcnt == approvalFlow.Approvers.Count())
                        {
                           task.Status = WorkflowStatus.Closed;
                           newRequest.Status = WorkflowStatus.Closed;
                           if (string.IsNullOrEmpty(nextApprover))
                           {
                              nextApprover = app.Email;
                              pRequestItem.NextApprover = app;
                              IsIndent = false;
                           }
                        }
                        newRequest.Task_Assignment.Add(task);
                     }
                     else
                     {
                        diff = true;
                        var task = new Task_Assignment()
                        {
                           Profile_ID = app.Profile_ID,
                           Name = app.Name,
                           Email = app.Email,
                           Approval_Level = app.Approval_Level,
                           Record_Status = WfRecordStatus.Active
                        };
                        newRequest.Task_Assignment.Add(task);
                        if (string.IsNullOrEmpty(nextApprover))
                        {
                           nextApprover = app.Email;
                           pRequestItem.NextApprover = app;
                           IsIndent = false;
                        }
                     }
                     #endregion
                  }
               }

               if (wflService.CreateRequest(newRequest))
               {
                  foreach (var rvw in approvalFlow.Reviewers)
                  {
                     lstReviewer.Add(new Reviewer()
                     {
                        Reviewer_ID = rvw.Reviewer_ID,
                        Approval_Flow_ID = rvw.Approval_Flow_ID,
                        Company_ID = rvw.Company_ID,
                        Profile_ID = rvw.Profile_ID,
                        Name = rvw.Name,
                        Email = rvw.Email
                     });
                  }
                  pRequestItem.Reviewers = lstReviewer;
                  pRequestItem.Request_ID = newRequest.Request_ID;
                  pRequestItem.Status = newRequest.Status;
                  pRequestItem.Is_Indent = IsIndent;
               }
            }
            else
            {
               throw new Exception("No Workflow found for Requestor : " + pRequestItem.Requestor_Name);
            }

         }
         catch (Exception ex)
         {
            throw ex;
         }

      }

      private void createManualRequest(ref ManualRequestItem pRequestItem)
      {
         var wflService = new WorkflowService();
         try
         {
            List<Reviewer> lstReviewer = new List<Reviewer>();
            DateTime actionTime = Utils.GetDateTimeNow();

            Request newRequest = new Models.Request()
            {
               Company_ID = pRequestItem.Company_ID,
               Module = pRequestItem.Module,
               Approval_Type = pRequestItem.Approval_Type,
               Requestor_Profile_ID = pRequestItem.Requestor_Profile_ID,
               Requestor_Name = pRequestItem.Requestor_Name,
               Requestor_Email = pRequestItem.Requestor_Email,
               Approval_Level = 1,
               Status = WorkflowStatus.Pending,
               Request_Date = actionTime,
               Last_Action_Date = actionTime
            };

            newRequest.Histories.Add(new History()
            {
               Action = WorkflowAction.Submit,
               Profile_ID = pRequestItem.Requestor_Profile_ID,
               Action_By = pRequestItem.Requestor_Name,
               Action_Email = pRequestItem.Requestor_Email,
               Action_On = actionTime
            });

            if (pRequestItem.Assignee != null)
            {
               if (pRequestItem.Assignee.Profile_ID != pRequestItem.Requestor_Profile_ID)
               {
                  var task = new Task_Assignment()
                  {
                     Profile_ID = pRequestItem.Assignee.Profile_ID,
                     Name = pRequestItem.Assignee.Name,
                     Email = pRequestItem.Assignee.Email,
                     Approval_Level = pRequestItem.Assignee.Approval_Level,
                     Record_Status = WfRecordStatus.Active,
                     Status = WorkflowStatus.Pending
                  };
                  newRequest.Task_Assignment.Add(task);
               }
               else
               {
                  var task = new Task_Assignment()
                  {
                     Profile_ID = pRequestItem.Assignee.Profile_ID,
                     Name = pRequestItem.Assignee.Name,
                     Email = pRequestItem.Assignee.Email,
                     Approval_Level = pRequestItem.Assignee.Approval_Level,
                     Record_Status = WfRecordStatus.InActive,
                     Status = WorkflowStatus.Closed
                  };

                  newRequest.Status = WorkflowStatus.Closed;
                  newRequest.Task_Assignment.Add(task);
               }
            }
            else
            {
               var task = new Task_Assignment()
               {
                  Profile_ID = pRequestItem.Assignee.Profile_ID,
                  Name = pRequestItem.Assignee.Name,
                  Email = pRequestItem.Assignee.Email,
                  Approval_Level = pRequestItem.Assignee.Approval_Level,
                  Record_Status = WfRecordStatus.InActive,
                  Status = WorkflowStatus.Closed
               };

               newRequest.Status = WorkflowStatus.Closed;
               newRequest.Task_Assignment.Add(task);
            }

            if (wflService.CreateRequest(newRequest))
            {
               pRequestItem.Request_ID = newRequest.Request_ID;
               pRequestItem.Status = newRequest.Status;
            }
            else
            {
               throw new Exception("No Workflow found for Requestor : " + pRequestItem.Requestor_Name);
            }

         }
         catch (Exception ex)
         {
            throw ex;
         }

      }

      private void createOpenRequest(ref RequestItem pRequestItem)
      {
         var wflService = new WorkflowService();
         try
         {
            List<Reviewer> lstReviewer = new List<Reviewer>();
            DateTime actionTime = Utils.GetDateTimeNow();

            var approvalFlow = wflService.GetApprovalFlow(
                pRequestItem.Module,
                pRequestItem.Approval_Type,
                pRequestItem.Company_ID,
                pRequestItem.Department_ID,
                pRequestItem.Requestor_Profile_ID);

            if (approvalFlow != null)
            {

               Request newRequest = new Models.Request()
               {
                  Approval_Flow_ID = approvalFlow.Approval_Flow_ID,
                  Company_ID = pRequestItem.Company_ID,
                  Requestor_Profile_ID = pRequestItem.Requestor_Profile_ID,
                  Requestor_Name = pRequestItem.Requestor_Name,
                  Requestor_Email = pRequestItem.Requestor_Email,
                  Approval_Level = 1,
                  Status = WorkflowStatus.Pending,
                  Request_Date = actionTime,
                  Last_Action_Date = actionTime
               };

               newRequest.Histories.Add(new History()
               {
                  Action = WorkflowAction.Submit,
                  Profile_ID = pRequestItem.Requestor_Profile_ID,
                  Action_By = pRequestItem.Requestor_Name,
                  Action_Email = pRequestItem.Requestor_Email,
                  Action_On = actionTime
               });

               var nextApprover = "";
               var diff = false;
               var appcnt = 0;
               foreach (var app in approvalFlow.Approvers.OrderBy(o => o.Approval_Level))
               {
                  appcnt++;
                  if (app.Profile_ID == pRequestItem.Requestor_Profile_ID && !diff)
                  {
                     var task = new Task_Assignment()
                     {
                        Profile_ID = app.Profile_ID,
                        Name = app.Name,
                        Email = app.Email,
                        Approval_Level = app.Approval_Level,
                        Record_Status = WfRecordStatus.InActive,
                        Status = WorkflowStatus.Approved
                     };
                     newRequest.Approval_Level = newRequest.Approval_Level + 1;
                     newRequest.Status = WorkflowStatus.Approved;
                     if (appcnt == approvalFlow.Approvers.Count())
                     {
                        task.Status = WorkflowStatus.Closed;
                        newRequest.Status = WorkflowStatus.Closed;
                        if (string.IsNullOrEmpty(nextApprover))
                        {
                           nextApprover = app.Email;
                           pRequestItem.NextApprover = app;
                        }
                     }


                     newRequest.Task_Assignment.Add(task);
                  }
                  else
                  {
                     diff = true;
                     var task = new Task_Assignment()
                     {
                        Profile_ID = app.Profile_ID,
                        Name = app.Name,
                        Email = app.Email,
                        Approval_Level = app.Approval_Level,
                        Record_Status = WfRecordStatus.Active
                     };
                     newRequest.Task_Assignment.Add(task);
                     if (string.IsNullOrEmpty(nextApprover))
                     {
                        nextApprover = app.Email;
                        pRequestItem.NextApprover = app;
                     }
                  }




               }

               if (wflService.CreateRequest(newRequest))
               {
                  foreach (var rvw in approvalFlow.Reviewers)
                  {
                     lstReviewer.Add(new Reviewer()
                     {
                        Reviewer_ID = rvw.Reviewer_ID,
                        Approval_Flow_ID = rvw.Approval_Flow_ID,
                        Company_ID = rvw.Company_ID,
                        Profile_ID = rvw.Profile_ID,
                        Name = rvw.Name,
                        Email = rvw.Email
                     });
                  }

                  pRequestItem.Reviewers = lstReviewer;
                  pRequestItem.Request_ID = newRequest.Request_ID;
                  pRequestItem.Status = newRequest.Status;
               }

            }
            else
            {
               throw new Exception("No Workflow found for Requestor : " + pRequestItem.Requestor_Name);
            }

         }
         catch (Exception ex)
         {
            throw ex;
         }

      }

      private List<Approver> getNextApprover(ICollection<Approver> approverList, Nullable<int> index)
      {
         List<Approver> appList = new List<Approver>();
         foreach (var app in approverList.Where(z => z.Approval_Level == index))
         {
            appList.Add(app);
         }
         return appList;
      }

      private List<Approval_Flow> getWorkFlows(Nullable<int> pCompany_ID, Nullable<int> pDepartment_ID, Nullable<int> pProfile_ID, string pModule, string pApproval_Type)
      {
         List<Approval_Flow> workflows = null;
         var wflService = new WorkflowService();

         try
         {

            workflows = new List<Approval_Flow>();
            foreach (var workflow in wflService.GetApprovalFlows(pCompany_ID, pDepartment_ID, pProfile_ID, pModule, pApproval_Type))
            {
               workflows.Add(workflow);
            }

         }
         catch (Exception ex)
         {
            throw ex;
         }

         return workflows;
      }

      private Request getRequest(Nullable<int> pRequest_ID)
      {
         var wflService = new WorkflowService();
         try
         {
            return wflService.GetRequest(pRequest_ID);
         }
         catch
         {
            return null;
         }
      }



      private List<Request> getRequests(Nullable<int> pCompany_ID, Nullable<int> pDepartment_ID, Nullable<int> pProfile_ID, string pModule, string pApproval_Type, Nullable<int> pDocID = null)
      {
         List<Request> requests = new List<Request>();

         var wflService = new WorkflowService();

         try
         {

            foreach (var rq in wflService.GetRequests(pCompany_ID, pDepartment_ID, pProfile_ID, pModule, pApproval_Type, pDocID))
            {
               Request vRq = new Request()
               {
                  Request_ID = rq.Request_ID,
                  Approval_Flow_ID = rq.Approval_Flow_ID,
                  Approval_Level = rq.Approval_Level,
                  Company_ID = rq.Company_ID,
                  Requestor_Name = rq.Requestor_Name,
                  Requestor_Email = rq.Requestor_Email,
                  Requestor_Profile_ID = rq.Requestor_Profile_ID,
                  Status = rq.Status,
                  Request_Type = rq.Request_Type,
                  Task_Assignment = rq.Task_Assignment
               };
               requests.Add(vRq);
            }

         }
         catch (Exception ex)
         {
            throw ex;
         }

         return requests;
      }

      private List<Request> getTasks(Nullable<int> pCompany_ID, Nullable<int> pDepartment_ID, Nullable<int> pProfile_ID, string pModule, string pApproval_Type, string pStatus)
      {
         List<Request> requests = new List<Request>();
         var wflService = new WorkflowService();

         try
         {

            foreach (var rq in wflService.GetTasks(pCompany_ID, pDepartment_ID, pProfile_ID, pModule, pApproval_Type, pStatus))
            {
               Request vRq = new Request()
               {
                  Request_ID = rq.Request_ID,
                  Approval_Flow_ID = rq.Approval_Flow_ID,
                  Approval_Level = rq.Approval_Level,
                  Company_ID = rq.Company_ID,
                  Requestor_Name = rq.Requestor_Name,
                  Requestor_Email = rq.Requestor_Email,
                  Requestor_Profile_ID = rq.Requestor_Profile_ID,
                  Status = rq.Status,
                  Request_Type = rq.Request_Type,
                  Task_Assignment = rq.Task_Assignment
               };

               if (rq.Approval_Flow != null)
               {


               }

               requests.Add(vRq);
            }

         }
         catch
         {
            return null;
         }

         return requests;
      }

      private bool approveOrRejectRequest(ActionItem actionItem)
      {
         var wflService = new WorkflowService();

         try
         {
            Approver nextAppr = null;
            var status = "";
            var IsIndent = false;
            var IsSendMail = false;

            var result = wflService.ApproveOrRejectRequest(actionItem.Request_ID, actionItem.Actioner_Profile_ID,
                actionItem.Name, actionItem.Email, actionItem.IsApprove, actionItem.Remarks, ref status, actionItem.Action, ref nextAppr, ref IsIndent, ref IsSendMail);

            ReturnIndentValue rIndentValue = new ReturnIndentValue();
            rIndentValue.IsIndent = IsIndent;
            rIndentValue.SendRequest = IsSendMail;

            actionItem.Status = status;
            actionItem.NextApprover = nextAppr;
            actionItem.IndentValue = rIndentValue;
            return result;

         }
         catch (Exception ex)
         {
            throw ex;
         }
      }

      private bool cancelorCloseRequest(ActionItem actionItem)
      {
         var wflService = new WorkflowService();

         try
         {
            var status = "";
            var result = wflService.CancelorCloseRequest(actionItem.Request_ID, actionItem.Actioner_Profile_ID, actionItem.Name, actionItem.Email, actionItem.Remarks, ref status, actionItem.Action);

            actionItem.Status = status;

            return result;

         }
         catch (Exception ex)
         {
            throw ex;
         }
      }

      private bool isMyTask(Nullable<int> pRequest_ID, Nullable<int> pProfile_ID)
      {
         var wflService = new WorkflowService();

         try
         {
            return wflService.IsMyTask(pRequest_ID, pProfile_ID);

         }
         catch (Exception ex)
         {
            throw ex;
         }
      }

      #endregion


   }
}
