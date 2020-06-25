using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SBSWorkFlowAPI.Models;

namespace SBSWorkFlowAPI.ModelsAndService
{

   public class RequestItem
   {

      public RequestItem()
      {
         this.Reviewers = new List<Reviewer>();
         this.Assignees = new List<Task_Assignment>();
      }
      public string Module { get; set; }
      public string Approval_Type { get; set; }
      public int Company_ID { get; set; }
      public int Department_ID { get; set; }
      public int Requestor_Profile_ID { get; set; }
      public string Requestor_Name { get; set; }
      public string Requestor_Email { get; set; }

      public Nullable<int> Doc_ID { get; set; }

      public int Request_ID { get; set; }
      public string Status { get; set; }
      public Approver NextApprover { get; set; }
      public List<Reviewer> Reviewers { get; set; }
      public List<Task_Assignment> Assignees { get; set; }

      //Added by sun 21-04-2017
      public bool Is_Indent { get; set; }
      public List<IndentItem> IndentItems { get; set; }
      
   }

   public class ManualRequestItem
   {
      public ManualRequestItem()
      {
         this.Reviewers = new List<Reviewer>();
      }
      public string Module { get; set; }
      public string Approval_Type { get; set; }
      public int Company_ID { get; set; }
      public int Department_ID { get; set; }
      public int Requestor_Profile_ID { get; set; }
      public string Requestor_Name { get; set; }
      public string Requestor_Email { get; set; }
      public int Request_ID { get; set; }
      public string Status { get; set; }
      public List<Reviewer> Reviewers { get; set; }
      public Task_Assignment Assignee { get; set; }

   }

   public class ActionItem
   {
      public int Request_ID { get; set; }
      public int Request_Cancel_ID { get; set; }
      public int Actioner_Profile_ID { get; set; }
      public string Name { get; set; }
      public string Email { get; set; }
      public bool IsApprove { get; set; }
      public string Remarks { get; set; }
      public string Status { get; set; }
      public string Action { get; set; }
      public Approver NextApprover { get; set; }
      public ReturnIndentValue IndentValue { get; set; }
   }

   public class ReturnValue
   {
      public bool IsSuccess { get; set; }
      public string Message { get; set; }
      public Exception Exception { get; set; }
   }

   public class IndentItem
   {
      public int Requestor_Profile_ID { get; set; }
      public string Indent_No { get; set; }
      public string Requestor_Name { get; set; }
      public string Requestor_Email { get; set; }
      public bool IsSuccess { get; set; }
   }

   public class ReturnIndentValue
   {
      public bool IsIndent { get; set; }
      public bool SendRequest { get; set; }
   }
}