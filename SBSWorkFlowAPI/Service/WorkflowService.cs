
using SBSWorkFlowAPI.Constants;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Data;
using System.Text;
using System.Data.Entity.Validation;
using SBSWorkFlowAPI.ModelsAndService;


namespace SBSWorkFlowAPI.Models
{
   internal class WorkflowService
   {

      //create workflow
      internal bool Create_WorkFlow(Approval_Flow vApprovalFlow)
      {
         bool result = false;

         try
         {
            using (var db = new WorkflowDBContext())
            {
               db.Entry(vApprovalFlow).State = EntityState.Added;
               db.SaveChanges();
               result = true;
            }
         }
         catch
         {
            result = false;
         }
         return result;
      }

      //update workflow
      internal bool Update_WorkFlow(Approval_Flow vApprovalFlow)
      {
         bool result = false;

         try
         {
            using (var db = new WorkflowDBContext())
            {

               var wf = db.Approval_Flow.Where(a => a.Approval_Flow_ID == vApprovalFlow.Approval_Flow_ID).SingleOrDefault();

               if (wf.Approvers != null && wf.Approvers.Count > 0)
               {
                  db.Approvers.RemoveRange(wf.Approvers);
               }

               foreach (var app in vApprovalFlow.Approvers)
               {
                  wf.Approvers.Add(new Approver()
                  {
                     Approval_Flow_ID = wf.Approval_Flow_ID,
                     Approval_Level = app.Approval_Level,
                     Company_ID = app.Company_ID,
                     Profile_ID = app.Profile_ID,
                     Name = app.Name,
                     Email = app.Email,
                     Approval_Flow_Type = app.Approval_Flow_Type,
                  });
               }

               if (wf.Conditions != null && wf.Conditions.Count > 0)
               {
                  db.Conditions.RemoveRange(wf.Conditions);
               }

               foreach (var cond in vApprovalFlow.Conditions)
               {
                  wf.Conditions.Add(new Condition()
                  {
                     Approval_Flow_ID = cond.Approval_Flow_ID,
                     LeftRange = cond.LeftRange,
                     RightRange = cond.RightRange,
                     StrCondition = cond.StrCondition
                  });
               }

               if (wf.Departments != null && wf.Departments.Count > 0)
               {
                  db.Departments.RemoveRange(wf.Departments);
               }

               foreach (var dept in vApprovalFlow.Departments)
               {
                  wf.Departments.Add(new Department()
                  {
                     Approval_Flow_ID = dept.Approval_Flow_ID,
                     User_Department_ID = dept.User_Department_ID,
                     Name = dept.Name
                  });
               }

               if (wf.Reviewers != null && wf.Reviewers.Count > 0)
               {
                  db.Reviewers.RemoveRange(wf.Reviewers);
               }

               foreach (var rvw in vApprovalFlow.Reviewers)
               {
                  wf.Reviewers.Add(new Reviewer()
                  {
                     Approval_Flow_ID = rvw.Approval_Flow_ID,
                     Company_ID = rvw.Company_ID,
                     Profile_ID = rvw.Profile_ID,
                     Name = rvw.Name,
                     Email = rvw.Email
                  });
               }

               if (wf.Applicable_Employee != null && wf.Applicable_Employee.Count > 0)
               {
                  db.Applicable_Employee.RemoveRange(wf.Applicable_Employee);
               }

               foreach (var appl in vApprovalFlow.Applicable_Employee)
               {
                  wf.Applicable_Employee.Add(new Applicable_Employee()
                  {
                     Approval_Flow_ID = appl.Approval_Flow_ID,
                     Company_ID = appl.Company_ID,
                     Profile_ID = appl.Profile_ID,
                     Name = appl.Name,
                     Email = appl.Email,
                     Is_Applicable = appl.Is_Applicable
                  });
               }

               wf.Approval_Type = vApprovalFlow.Approval_Type;
               wf.Branch_ID = vApprovalFlow.Branch_ID;
               wf.Branch_Name = vApprovalFlow.Branch_Name;
               wf.Company_ID = vApprovalFlow.Company_ID;
               wf.Updated_By = vApprovalFlow.Updated_By;
               wf.Updated_On = vApprovalFlow.Updated_On;
               wf.Module = vApprovalFlow.Module;
               wf.Record_Status = vApprovalFlow.Record_Status;

               db.Entry(wf).State = EntityState.Modified;
               db.SaveChanges();
               result = true;
            }
         }
         catch (DbEntityValidationException e)
         {
            StringBuilder sb = new StringBuilder();
            foreach (var eve in e.EntityValidationErrors)
            {
               sb.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                               eve.Entry.Entity.GetType().Name,
                                               eve.Entry.State));
               foreach (var ve in eve.ValidationErrors)
               {
                  sb.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                              ve.PropertyName,
                                              ve.ErrorMessage));
               }
            }
            result = false;
         }
         return result;
      }

      //delete workflow
      internal bool Delete_WorkFlow(Nullable<int> vApprovalFlowID, Nullable<int> vProfileID)
      {
         bool result = false;

         try
         {
            using (var db = new WorkflowDBContext())
            {
               var wf = db.Approval_Flow.Where(w => w.Approval_Flow_ID == vApprovalFlowID).FirstOrDefault();

               if (wf != null)
               {
                  DateTime actionOn = Utils.GetDateTimeNow();

                  wf.Record_Status = WfRecordStatus.InActive;
                  wf.Updated_On = actionOn;
                  wf.Updated_By = vProfileID;

                  db.Entry(wf).State = EntityState.Modified;
                  db.SaveChanges();
                  result = true;
               }

            }
         }
         catch
         {
            result = false;
         }
         return result;
      }

      //submit request
      internal bool CreateRequest(Request vRequest)
      {
         bool result = false;
         try
         {
            using (var db = new WorkflowDBContext())
            {
               db.Entry(vRequest).State = EntityState.Added;
               db.SaveChanges();
               db.Entry(vRequest).GetDatabaseValues();
               result = true;
            }
         }
         catch
         {
         }
         return result;
      }

      //get request
      internal Request GetRequestByRequestID(Nullable<int> vRequest_ID)
      {
         try
         {
            using (var db = new WorkflowDBContext())
            {
               var request = db.Requests
                   .Include(r => r.Approval_Flow)
                   .Include(r => r.Histories)
                   .Include(r => r.Task_Assignment)
                   .Include(r => r.Approval_Flow.Approvers)
                   .Where(w => w.Request_ID == vRequest_ID).FirstOrDefault();

               return request;
            }
         }
         catch (Exception ex)
         {
            throw ex;
         }
      }

      //list workflow
      internal List<Approval_Flow> GetApprovalFlows(Nullable<int> vCompany_ID, Nullable<int> vDepartment_ID, Nullable<int> vProfile_ID, string vModule, string vApproval_Type, bool vIncludeInactive = false)
      {

         try
         {
            using (var db = new WorkflowDBContext())
            {
               var flow = db.Approval_Flow
                   .Include(r => r.Approvers)
                   .Include(r => r.Reviewers)
                   .Include(r => r.Conditions)
                   .Include(r => r.Applicable_Employee)
                   .Include(r => r.Requests)
                   .Include(r => r.Departments)
                   .Where(w => w.Module == vModule &&
                       w.Approval_Type == vApproval_Type &&
                       w.Company_ID == vCompany_ID &&
                       w.Record_Status != WfRecordStatus.Delete);


               if (vDepartment_ID.HasValue)
               {
                  flow = flow.Where(w => w.Departments.Any(x => x.User_Department_ID == vDepartment_ID));
               }

               if (vProfile_ID.HasValue)
               {
                  flow = flow.Where(w => w.Applicable_Employee.Any(x => x.Profile_ID == vProfile_ID & x.Is_Applicable == true));
               }
               if (!vIncludeInactive)
               {
                  flow = flow.Where(w => w.Record_Status == WfRecordStatus.Active);
               }

               if (flow != null)
               {
                  return flow.ToList();
               }
               else
               {
                  return null;
               }
            }
         }
         catch
         {
            return null;
         }

      }

      internal Approval_Flow GetApprovalFlowByApprovalID(Nullable<int> vApproval_ID)
      {

         try
         {
            using (var db = new WorkflowDBContext())
            {
               var flow = db.Approval_Flow
                   .Include(r => r.Approvers)
                   .Include(r => r.Reviewers)
                   .Include(r => r.Conditions)
                   .Include(r => r.Applicable_Employee)
                   .Include(r => r.Requests)
                   .Include(r => r.Departments)
                   .Where(w => w.Approval_Flow_ID == vApproval_ID);


               if (flow != null)
               {
                  return flow.FirstOrDefault();
               }
               else
               {
                  return null;
               }
            }
         }
         catch
         {
            return null;
         }

      }

      internal Approval_Flow GetApprovalFlow(string vModule, string vApprovalType, Nullable<int> vCompany_ID, Nullable<int> vDepartment_ID, Nullable<int> vUser_Profile_ID)
      {

         try
         {
            using (var db = new WorkflowDBContext())
            {
               var flow = db.Approval_Flow
                   .Include(r => r.Approvers)
                   .Include(r => r.Reviewers)
                   .Include(r => r.Conditions)
                   .Include(r => r.Applicable_Employee)
                   .Include(r => r.Requests)
                   .Include(r => r.Departments)
                   .Where(w => w.Module == vModule &
                       w.Approval_Type == vApprovalType &
                       w.Record_Status == WfRecordStatus.Active &
                       w.Company_ID == vCompany_ID && w.Departments.Any(x => x.User_Department_ID == vDepartment_ID));


               if (flow != null)
               {
                  return flow.FirstOrDefault();
               }
               else
               {
                  return null;
               }
            }
         }
         catch
         {
            return null;
         }

      }

      internal Request GetRequest(Nullable<int> vRequest_ID)
      {
         Request requests = new Request();

         try
         {
            using (var db = new WorkflowDBContext())
            {
               var q = db.Requests
                   .Include(x => x.Approval_Flow)
                   .Include(x => x.Approval_Flow.Reviewers)
                   .Include(x => x.Approval_Flow.Departments)
                   .Include(x => x.Approval_Flow.Approvers)
                   .Include(x => x.Approval_Flow.Applicable_Employee)
                   .Include(x => x.Approval_Flow.Conditions)
                   .Include(x => x.Task_Assignment)
                   .Where(x => x.Request_ID == vRequest_ID);

               return q.FirstOrDefault();
            }
         }
         catch
         {
            return null;
         }
      }

      internal List<Request> GetRequests(Nullable<int> vCompany_ID, Nullable<int> vDepartment_ID, Nullable<int> vRequestor_ID = null, string vModule = "", string vApproval_Type = "", Nullable<int> vDoc_ID = null)
      {
         List<Request> requests = new List<Request>();

         try
         {
            using (var db = new WorkflowDBContext())
            {
               var q = db.Requests
                   .Include(x => x.Approval_Flow)
                    .Include(x => x.Approval_Flow.Departments)
                   .Include(x => x.Approval_Flow.Approvers)
                   .Include(x => x.Approval_Flow.Applicable_Employee)
                   .Include(x => x.Approval_Flow.Conditions)
                   .Include(x => x.Approval_Flow.Reviewers)
                   .Include(x => x.Task_Assignment)
                   .Where(x => x.Company_ID == vCompany_ID);

               if (!string.IsNullOrEmpty(vModule))
               {
                  q = q.Where(x => x.Approval_Flow.Module == vModule);
               }

               if (!string.IsNullOrEmpty(vApproval_Type))
               {
                  q = q.Where(x => x.Approval_Flow.Approval_Type == vApproval_Type);
               }

               if (vRequestor_ID != null)
               {
                  q = q.Where(x => x.Requestor_Profile_ID == vRequestor_ID.Value);
               }
               if (vDoc_ID.HasValue)
               {
                  q = q.Where(x => x.Doc_ID == vDoc_ID);
               }

               return q.ToList();
            }
         }
         catch
         {
            return null;
         }
      }

      internal bool ApproveOrRejectRequest(Nullable<int> vRequest_ID, Nullable<int> vAction_User_ID, string vUser_Name, string vEmail, bool vIsApprove, string vRemarks, ref string vStatus, string vAction, ref Approver vNextAppr, ref bool vIsIndent, ref bool vIsSendMail)
      {
         bool result = false;
         try
         {
            using (var db = new WorkflowDBContext())
            {
               DateTime actionOn = Utils.GetDateTimeNow();
               string action = string.Empty;
               var haveIndent = false;
               var recheck = false;

               Request req = GetRequestByRequestID(vRequest_ID);
               if (req != null)
               {
                  if (req.Is_Indent.HasValue && req.Is_Indent.Value)
                     haveIndent = true;

                  req.Last_Action_Date = actionOn;
                  if (req.Approval_Flow_ID != null)
                  {
                     if (vIsApprove)
                     {
                        List<Approver> nextApprovers = GetNextApprovers(req.Company_ID,
                            req.Approval_Flow.Approval_Type, req.Approval_Flow.Module, req.Requestor_Profile_ID, req.Approval_Level + 1);

                        if (nextApprovers.Count > 0)
                        {
                           req.Approval_Level += 1;
                           action = WorkflowAction.Approve;
                           req.Status = WorkflowStatus.Approved;
                           var nextApp = nextApprovers.FirstOrDefault();
                           if (nextApp.Approval_Flow_Type == ApproverFlowType.Job_Cost)
                           {
                              if (haveIndent)
                              {
                                 vIsIndent = true;
                                 vIsSendMail = true;
                                 recheck = true;
                              }
                              else
                              {
                                 req.Status = WorkflowStatus.Closed;
                              }
                           }
                           else
                              vNextAppr = nextApp;
                        }
                        else
                        {
                           if (haveIndent)
                           {
                              action = WorkflowAction.Approve;
                              req.Status = WorkflowStatus.Approved;

                              if (GetRequestAssigneesIndentClosed(vRequest_ID))
                                 req.Status = WorkflowStatus.Closed;
                              else
                                 recheck = true;

                              vIsIndent = true;
                              vIsSendMail = false;
                           }
                           else
                           {
                              action = WorkflowAction.Approve;
                              req.Status = WorkflowStatus.Closed;
                           }
                        }
                     }
                     else
                     {
                        action = WorkflowAction.Reject;
                        req.Status = WorkflowStatus.Rejected;
                        req.Approval_Level = 1;
                     }
                  }
                  else
                  {
                     if (vIsApprove)
                     {
                        action = WorkflowAction.Approve;
                        req.Status = WorkflowStatus.Closed;
                     }
                     else
                     {
                        action = WorkflowAction.Reject;
                        req.Status = WorkflowStatus.Rejected;
                     }
                  }

                  History hist = new History()
                  {
                     Action = action,
                     Remarks = vRemarks,
                     Profile_ID = (vAction_User_ID.HasValue ? vAction_User_ID.Value : 0),
                     Action_By = vUser_Name,
                     Action_Email = vEmail,
                     Action_On = actionOn,
                     Request_ID = req.Request_ID
                  };

                  db.Entry(hist).State = EntityState.Added;

                  var totalcount = req.Task_Assignment.Where(w => w.Is_Indent == true).Count();
                  foreach (var assignee in req.Task_Assignment)
                  {
                     if (assignee.Profile_ID == vAction_User_ID)
                     {
                        if (assignee.Is_Indent.HasValue && assignee.Is_Indent.Value)
                           assignee.Indent_Closed = true;

                        assignee.Record_Status = WfRecordStatus.InActive;
                        assignee.Status = req.Status;
                        db.Entry(assignee).State = EntityState.Modified;

                        if (!vIsIndent)
                           break;
                     }
                  }

                  if (haveIndent && recheck)
                  {
                     var indentcount = req.Task_Assignment.Where(w => w.Is_Indent == true && w.Indent_Closed == true).Count();
                     if (indentcount == totalcount)
                        req.Status = WorkflowStatus.Closed;
                  }

                  vStatus = req.Status;
                  db.Entry(req).State = EntityState.Modified;
                  db.SaveChanges();
                  result = true;
               }
            }
         }
         catch (Exception ex)
         {
            throw ex;
         }

         return result;
      }

      internal bool CancelorCloseRequest(Nullable<int> vRequest_ID, Nullable<int> vAction_User_ID, string vUser_Name, string vEmail, string vRemarks, ref string vStatus, string vAction)
      {
         bool result = false;
         try
         {
            using (var db = new WorkflowDBContext())
            {
               DateTime actionOn = Utils.GetDateTimeNow();
               Request req = GetRequestByRequestID(vRequest_ID);
               string action = "";

               if (req != null)
               {
                  req.Last_Action_Date = actionOn;

                  if (vAction.Equals(WorkflowAction.Cancel))
                  {

                     req.Status = WorkflowStatus.Cancelled;
                     action = WorkflowAction.Cancel;

                  }
                  else if (vAction.Equals(WorkflowAction.Close))
                  {

                     req.Status = WorkflowStatus.Closed;
                     action = WorkflowAction.Close;

                  }

                  History hist = new History()
                  {
                     Action = action,
                     Remarks = vRemarks,
                     Profile_ID = (vAction_User_ID.HasValue ? vAction_User_ID.Value : 0),
                     Action_By = vUser_Name,
                     Action_Email = vEmail,
                     Action_On = actionOn,
                     Request_ID = req.Request_ID
                  };

                  db.Entry(hist).State = EntityState.Added;

                  //foreach (var assignee in req.Task_Assignment) {
                  //    if (assignee.Profile_ID == vAction_User_ID) {
                  //        assignee.Record_Status = WfRecordStatus.InActive;
                  //        db.Entry(assignee).State = EntityState.Modified;
                  //    }
                  //}

                  vStatus = req.Status;
                  db.Entry(req).State = EntityState.Modified;
                  db.SaveChanges();
                  result = true;
               }
            }

         }
         catch (Exception ex)
         {
            throw ex;
         }

         return result;
      }

      internal List<Request> GetTasks(Nullable<int> vCompany_ID, Nullable<int> vDepartment_ID, Nullable<int> vAssignee_ID = null, string vModule = "", string vApproval_Type = "", string vStatus = "")
      {
         List<Request> requests = new List<Request>();

         try
         {
            using (var db = new WorkflowDBContext())
            {
               var q = db.Requests
                   .Include(x => x.Approval_Flow)
                   .Include(x => x.Approval_Flow.Departments)
                   .Include(x => x.Approval_Flow.Approvers)
                   .Include(x => x.Approval_Flow.Applicable_Employee)
                   .Include(x => x.Approval_Flow.Conditions)
                   .Include(x => x.Approval_Flow.Reviewers)
                   .Include(x => x.Task_Assignment)
                   .Where(x => x.Company_ID == vCompany_ID);

               if (!string.IsNullOrEmpty(vModule))
               {
                  q = q.Where(x => x.Approval_Flow.Module == vModule);
               }

               if (!string.IsNullOrEmpty(vApproval_Type))
               {
                  q = q.Where(x => x.Approval_Flow.Approval_Type == vApproval_Type);
               }

               if (vAssignee_ID != null)
               {
                  if (string.IsNullOrEmpty(vStatus))
                  {
                     q = q.Where(t => t.Task_Assignment.Where(x => x.Profile_ID == vAssignee_ID.Value).FirstOrDefault() != null);
                  }
                  else
                  {
                     q = q.Where(t => t.Task_Assignment.Where(x => x.Profile_ID == vAssignee_ID.Value && x.Record_Status == vStatus).FirstOrDefault() != null);
                  }

               }

               return q.ToList();
            }
         }
         catch
         {
            return null;
         }
      }

      internal List<Task_Assignment> GetRequestAssignees(Nullable<int> vRequest_ID)
      {
         List<Task_Assignment> requests = new List<Task_Assignment>();

         try
         {
            using (var db = new WorkflowDBContext())
            {
               var q = db.Task_Assignment
                   .Where(x => x.Request_ID == vRequest_ID && x.Record_Status == WfRecordStatus.Active);

               return q.ToList();
            }
         }
         catch
         {
            return null;
         }
      }

      internal void InactivateAssignee(Task_Assignment assignee)
      {

         try
         {
            assignee.Record_Status = WfRecordStatus.InActive;

            using (var db = new WorkflowDBContext())
            {
               db.Entry(assignee).State = EntityState.Modified;
               db.SaveChanges();
            }
         }
         catch
         {

         }
      }

      internal List<Approver> GetNextApprovers(Nullable<int> vCompany_ID, string vApproval_Type, string vModule, Nullable<int> vUser_Profile_ID, Nullable<int> vIndex)
      {
         List<Approver> appList = new List<Approver>();

         using (var db = new WorkflowDBContext())
         {
            var q = db.Approvers
                    .Include(x => x.Approval_Flow)
                    .Include(x => x.Approval_Flow.Applicable_Employee)
                    .Where(x => x.Company_ID == vCompany_ID &&
                        x.Approval_Flow.Record_Status == WfRecordStatus.Active &&
                        x.Approval_Level == vIndex &&
                        x.Approval_Flow.Approval_Type == vApproval_Type &&
                        x.Approval_Flow.Module == vModule &&
                        x.Approval_Flow.Applicable_Employee.Any(ap => ap.Profile_ID == vUser_Profile_ID));

            if (q != null)
            {

               foreach (var app in q)
               {
                  appList.Add(app);
               }

            }
         }



         return appList;
      }

      internal bool IsMyTask(Nullable<int> vRequest_ID, Nullable<int> vProfile_ID)
      {
         try
         {
            using (var db = new WorkflowDBContext())
            {
               return db.Task_Assignment.Any(x => x.Request_ID == vRequest_ID && x.Profile_ID == vProfile_ID);
            }
         }
         catch
         {
            return false;
         }
      }

      //Added by sun 19-10-2015
      internal bool UpdatedeleteDelete_WorkFlow(Nullable<int> vApprovalFlowID, Nullable<int> vProfileID, string pStatus)
      {
         bool result = false;

         try
         {
            using (var db = new WorkflowDBContext())
            {
               var wf = db.Approval_Flow.Where(w => w.Approval_Flow_ID == vApprovalFlowID).FirstOrDefault();

               if (wf != null)
               {
                  DateTime actionOn = Utils.GetDateTimeNow();

                  wf.Record_Status = pStatus;
                  wf.Updated_On = actionOn;
                  wf.Updated_By = vProfileID;

                  db.Entry(wf).State = EntityState.Modified;
                  db.SaveChanges();
                  result = true;
               }

            }
         }
         catch
         {
            result = false;
         }
         return result;
      }

      internal bool GetRequestAssigneesIndentClosed(Nullable<int> vRequest_ID)
      {
         bool result = false;

         try
         {
            using (var db = new WorkflowDBContext())
            {
               var q = db.Task_Assignment
                   .Where(x => x.Request_ID == vRequest_ID && x.Is_Indent == true && x.Indent_Closed == true).ToList();

               var q2 = db.Task_Assignment
                  .Where(x => x.Request_ID == vRequest_ID && x.Is_Indent == true).ToList();

               if (q2.Count() == q.Count() + 1)
                  result = true;

               return result;
            }
         }
         catch
         {
            return result;
         }
      }
   }
}