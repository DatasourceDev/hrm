using SBSModel.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web;
using System.Configuration;
using SBSModel.Common;
using SBSResourceAPI;
using System.Diagnostics;
using System.Globalization;
using SBSWorkFlowAPI.Constants;
using SBSWorkFlowAPI.Models;
namespace SBSModel.Common
{
   public class EmailItem
   {
      public int? Doc_ID { get; set; }
      public string Send_To_Email { get; set; }
      public string Send_To_Name { get; set; }
      public string Link { get; set; }
      public string Link2 { get; set; }
      public string LogoLink { get; set; }
      public string Module { get; set; }
      public string Approval_Type { get; set; }
      public string Status { get; set; }
      public Company_Details Company { get; set; }
      public string Received_From_Email { get; set; }
      public string Received_From_Name { get; set; }
      public string Received_From_Department { get; set; }
      public List<SBSWorkFlowAPI.Models.Reviewer> Reviewer { get; set; }
      public Leave_Application_Document Leave { get; set; }
      public Nullable<decimal> Leave_Left { get; set; }
      public Nullable<decimal> Weeks_Left { get; set; }
      public Expenses_Application Expenses { get; set; }
      public Time_Sheet Timesheet { get; set; }
      public FileAttach Attachment { get; set; }
      public Screen_Capture_Log Report_Log { get; set; }
      public string ECode { get; set; }
      public string Url { get; set; }
      public FileAttach Attachment_SmartDev { get; set; }
      public string Approver_Name { get; set; }
   }
   public class FileAttach
   {
      public string File_Name { get; set; }
      public MemoryStream File { get; set; }
   }
   public class EmailTemplete
   {
      private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      #region Authen
      public static void sendResetPasswordEmail(String email, String code, String Name, string domain)
      {
         String ResetPassword_link = domain + "Account/ResetPassword?code=" + code;

         String message = "<p>Dear " + Name + ", " +
             "<br/> <br />" +
             "Reset Password Link: <a href='" + ResetPassword_link + "'>" + ResetPassword_link + "</a>" +
             "<br/> <br />" +
             "If you have any enquiries, please contact us at 6696 5457 or email us at enquiry@bluecube.com.sg" +
             "<br/> <br />" +
             "Regards," +
             "<br/> " +
             "SBS Team";

         sendNotificationEmail(email, "SBS Reset Password of Account", message, null, null, null);

      }
      public static bool sendUserActivateEmail(String email, String activateCode, String Name, String CompanyName, String CompanyPhone, String CompanyEmail, string domain)
      {
         String activate_link = domain + "Account/Activate?code=" + activateCode;

         String message = "<p>Dear " + Name + ", " +
             "<br/> <br />" +
             "You have registered under " + CompanyName + " with an account." +
             "<br/> " +
             "In order for you to process, please activate your account using the link below or copy and paste the link in any browser:" +
             "<br/> <br />" +
             "Activation Link: <a href='" + activate_link + "'>" + activate_link + "</a>" +
             "<br/> <br />" +
            //"If you have any enquiries, please contact us at " + CompanyPhone + " or email us at " + CompanyEmail +
             "If you have any enquiries, please contact us at +65 66965457 or email us at support@sbsolutions.com.sg" +
             "<br/> <br />" +
             "Regards," +
             "<br/>" +
             "Support";
         //CompanyName;

         return sendNotificationEmail(email, "SBS Activation of Account[" + email + "]", message, null, null, null);

      }
      #endregion

      #region Payroll
      public static bool sendPayslipEmail(String email, String employeeName, string module, string domain, MemoryStream mm)
      {
         String message = "Dear " + employeeName + "," +
         "<br/> <br />" +
         "If you have encounter any technical issues, please contact your system administrator for help." +
         "<br/> <br />" +
         "Regards," +
         "<br/> <br />" +
         module + " System";

         var mems = new List<FileAttach>();
         mems.Add(new FileAttach { File_Name = "Payslip.pdf", File = mm });
         return sendNotificationEmail(email, module + " Payslip", message, null, mems, domain);

      }
      #endregion

      #region Leave expenses Quotation

      /* System send email to approval for request some action (approve or reject)*/
      public static bool sendRequestEmail(EmailItem eitem)
      {
         var message = new StringBuilder();
         var files = new List<FileAttach>();
         var subject = "";
         if (!string.IsNullOrEmpty(eitem.ECode))
            subject += eitem.ECode + " ";

         subject += eitem.Approval_Type + " Pending Approval";
         if (eitem.Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Canceling)
            subject += " Cancellation";

         message.Append("Dear <span >" + eitem.Send_To_Name + "</span>,");
         message.Append("<br/> <br />");

         if (eitem.Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Canceling)
            message.Append("You have received a " + eitem.Approval_Type + " " + eitem.Status + " request from <span style='font-weight:700;' >" + eitem.Received_From_Name + "</span> - " + eitem.Received_From_Department + " with the following details:");
         else if (eitem.Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Cancelled)
            message.Append(eitem.Received_From_Name + " has been<span style='color:#0E9D41;font-weight:700;font-size:18px;' >" + " Cancelled  " + "</span> the " + eitem.Approval_Type + "</span> application with the following details:");
         else
            message.Append("You have received a <span style='font-weight:700;' >" + eitem.Approval_Type + " application</span> from <span style='font-weight:700;' >" + eitem.Received_From_Name + "</span> - " + eitem.Received_From_Department + " with the following details:");

         message.Append("<br/> <br />");
         if (eitem.Leave != null && eitem.Leave.Leave_Config != null)
         {
            #region Leave
            var sdate = DateUtil.ToDisplayFullDate(eitem.Leave.Start_Date);
            if (!string.IsNullOrEmpty(eitem.Leave.Start_Date_Period))
               sdate += " " + eitem.Leave.Start_Date_Period;
            else
               sdate += " " + Period.AM;

            var edate = "";
            if (!eitem.Leave.End_Date.HasValue)
            {
               edate = DateUtil.ToDisplayFullDate(eitem.Leave.Start_Date);
               if (!string.IsNullOrEmpty(eitem.Leave.Start_Date_Period))
                  edate += " " + eitem.Leave.Start_Date_Period;
               else
                  edate += " " + Period.PM;
            }

            else
            {
               edate = DateUtil.ToDisplayFullDate(eitem.Leave.End_Date);
               if (!string.IsNullOrEmpty(eitem.Leave.End_Date_Period))
                  edate += " " + eitem.Leave.End_Date_Period;
               else
                  edate += " " + Period.AM;
            }

            message.Append("<table style='border-collapse: collapse; line-height: 30px;width:100%' cellpadding='6'> ");
            message.Append("<tr style='border-bottom: 1px solid #ccc'>");
            message.Append("<td style='width:120px;'></td>");
            message.Append("<td> <span style='font-weight:700;' ></span></td>");
            message.Append("</tr>");
            message.Append("<tr style='border-bottom: 1px solid #ccc'>");
            message.Append("<td style='width:120px;'>" + Resource.Type + ": </td>");
            message.Append("<td> <span style='font-weight:700;' >" + eitem.Leave.Leave_Config.Leave_Name + "</span></td>");
            message.Append("</tr>");
            message.Append("<tr style='border-bottom: 1px solid #ccc'>");
            message.Append("<td style='width:120px;'>" + Resource.Start_Date + ": </td>");
            message.Append("<td> <span style='font-weight:700;' >" + sdate + "</span></td>");
            message.Append("</tr>");
            message.Append("<tr style='border-bottom: 1px solid #ccc'>");
            message.Append("<td style='width:120px;'>" + Resource.End_Date + ": </td>");
            message.Append("<td> <span style='font-weight:700;' >" + edate + "</span></td>");
            message.Append("</tr>");
            message.Append("<tr style='border-bottom: 1px solid #ccc'>");
            message.Append("<td style='width:120px;'>" + Resource.Total_Days + ": </td>");
            message.Append("<td> <span style='font-weight:700;' >" + eitem.Leave.Days_Taken + " " + Resource.Days.ToLower() + "</span></td>");
            message.Append("</tr>");

            if (eitem.Leave.Leave_Config != null)
            {
               message.Append("<tr style='border-bottom: 1px solid #ccc'>");
               message.Append("<td style='width:120px;'>" + Resource.Description + ": </td>");
               message.Append("<td> <span style='font-weight:700;' >" + eitem.Leave.Leave_Config.Leave_Description + "</span></td>");
               message.Append("</tr>");
            }

            message.Append("</table>");
            message.Append("<br/> <br />");

            if (!string.IsNullOrEmpty(eitem.Link) && !string.IsNullOrEmpty(eitem.Link2))
            {
               message.Append("For your action: <br/><br/>");

               string[] actionList = new string[] { "Approve", "Reject" };
               string[] urls = new string[] { eitem.Link, eitem.Link2 };
               string[] colors = new string[] { "#89e0a5", "#f53649" };

               message.Append(getEmailActionButtons(actionList, urls, colors));
            }


            if (eitem.Leave.Upload_Document != null && eitem.Leave.Upload_Document.Count > 0)
            {
               foreach (var img in eitem.Leave.Upload_Document)
               {
                  MemoryStream ms = new MemoryStream(img.Document);
                  files.Add(new FileAttach { File = ms, File_Name = img.File_Name });
               }
            }
            #endregion
         }
         else if (eitem.Expenses != null)
         {
            #region Expenses
            var totalClaimAmount = 0M;
            foreach (var row in eitem.Expenses.Expenses_Application_Document)
               totalClaimAmount += row.Amount_Claiming.Value;

            message.Append("<table style='border-collapse: collapse; line-height: 30px;width:100%' cellpadding='6'> ");
            message.Append("<tr style='border-bottom: 1px solid #ccc'>");
            message.Append("<td style='width:120px;'></td>");
            message.Append("<td> <span style='font-weight:700;' ></span></td>");
            message.Append("</tr>");
            message.Append("<tr style='border-bottom: 1px solid #ccc'>");
            message.Append("<td style='width:120px;'>" + Resource.Expenses_Title + ": </td>");
            message.Append("<td> <span style='font-weight:700;' >" + eitem.Expenses.Expenses_Title + "</span></td>");
            message.Append("</tr>");
            message.Append("<tr style='border-bottom: 1px solid #ccc'>");
            message.Append("<td style='width:120px;'>" + Resource.Date_Applied + ": </td>");
            message.Append("<td> <span style='font-weight:700;' >" + DateUtil.ToDisplayFullDate(eitem.Expenses.Date_Applied) + "</span></td>");
            message.Append("</tr>");
            message.Append("<tr style='border-bottom: 1px solid #ccc'>");
            message.Append("<td style='width:120px;'>" + Resource.Total_Amount + " </td>");
            message.Append("<td> <span style='font-weight:700;' >" + totalClaimAmount.ToString("n2") + " " + (eitem.Company != null ? eitem.Company.Currency.Currency_Code : "") + "</span></td>");
            message.Append("</tr>");
            message.Append("</table>");
            message.Append("<br/> <br />");


            message.Append("<table style='border-collapse: collapse; line-height: 30px;width:100%' cellpadding='6'>");
            message.Append("<tr style='background-color: #E2ECFB'>");
            message.Append("<td><strong>" + Resource.Expenses_Type + "</strong></td>");
            message.Append("<td><strong>" + Resource.Expenses_Date + "</strong></td>");
            message.Append("<td><strong>" + Resource.Total_Amount + "</strong></td>");
            message.Append("<td><strong>" + Resource.Amount_Claiming + "</strong></td>");
            message.Append("</tr>");

            var even = false;
            foreach (var row in eitem.Expenses.Expenses_Application_Document)
            {
               if (even)
                  message.Append("<tr style='background-color: #F7F7F7'>");
               else
                  message.Append("<tr>");

               message.Append("<td>" + row.Expenses_Config.Expenses_Name + "</td>");
               message.Append("<td>" + DateUtil.ToDisplayFullDate(row.Expenses_Date) + "</td>");
               message.Append("<td>" + row.Total_Amount.Value.ToString("n2") + " " + (eitem.Company != null ? eitem.Company.Currency.Currency_Code : "") + "</td>");
               message.Append("<td>" + row.Amount_Claiming.Value.ToString("n2") + " " + (eitem.Company != null ? eitem.Company.Currency.Currency_Code : "") + "</td>");
               message.Append("</tr>");
               even = !even;
            }

            message.Append("<tr style='background-color: #E2ECFB'>");
            message.Append("<td><strong>" + Resource.Total_Amount + "</strong></td>");
            message.Append("<td/><td/>");
            message.Append("<td><strong>" + totalClaimAmount.ToString("n2") + " " + (eitem.Company != null ? eitem.Company.Currency.Currency_Code : "") + "</strong></td>");
            message.Append("</tr>");

            message.Append("</table>");
            message.Append("<br/> <br />");

            //Edit by sun 24-02-2016
            if (!string.IsNullOrEmpty(eitem.Link) && !string.IsNullOrEmpty(eitem.Link2))
            {
               message.Append("For your action: <br/><br/>");
               string[] actionList = new string[] { "Approve", "Reject" };
               string[] urls = new string[] { eitem.Link, eitem.Link2 };
               string[] colors = new string[] { "#89e0a5", "#f53649" };

               message.Append(getEmailActionButtons(actionList, urls, colors));
            }

            //Added by sun 09-12-2015
            foreach (var frow in eitem.Expenses.Expenses_Application_Document)
            {
               if (frow.Upload_Receipt != null && frow.Upload_Receipt.Count > 0)
               {
                  foreach (var img in frow.Upload_Receipt)
                  {
                     MemoryStream ms = new MemoryStream(img.Receipt);
                     files.Add(new FileAttach { File = ms, File_Name = img.File_Name });
                  }
               }
            }
            //******** Start Smart Dev  ********//
            if (eitem.Attachment_SmartDev != null)
            {
               files.Add(eitem.Attachment_SmartDev);
            }
            //******** End Smart Dev  ********//
            #endregion
         }

         return NewEmailNotification(eitem.Send_To_Email, subject, message.ToString(), eitem.Reviewer, files, eitem.Url, eitem.Approval_Type, eitem.Doc_ID);
         //return sendNotificationEmail(eitem.Send_To_Email, subject, message.ToString(), eitem.Reviewer, files, eitem.Url);
      }

      /* System send email to employee after they create an application.*/
      public static bool sendProceedEmail(EmailItem eitem, string onBehalfEmpName = "")
      {
         var message = new StringBuilder();
         var files = new List<FileAttach>();
         var subject = "";
         if (!string.IsNullOrEmpty(eitem.ECode))
            subject += eitem.ECode + " ";

         subject += eitem.Approval_Type + " " + eitem.Status + " Application";

         if (onBehalfEmpName != "")
            message.Append("Dear <span >" + onBehalfEmpName + "</span>,");
         else
            message.Append("Dear <span >" + eitem.Send_To_Name + "</span>,");

         message.Append("<br/> <br />");

         if (eitem.Leave != null && eitem.Leave.Leave_Config != null)
         {
            #region Leave
            message.Append("Your <span style='font-weight:700;' >");
            message.Append(eitem.Approval_Type);

            //Edit by sun 24-02-2016 
            if (eitem.Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Canceling)
               message.Append(" " + eitem.Status + "</span> request has been ");
            else
            {
               if (onBehalfEmpName != "" && onBehalfEmpName != eitem.Send_To_Name)
               {
                  message.Append(" application on behalf of " + onBehalfEmpName + "</span> has been ");
               }
               else
                  message.Append(" application</span> has been ");
            }


            if (eitem.Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Rejected)
               message.Append("<span style='color:#CC0033;font-weight:700;font-size:18px;' >" + eitem.Status + "</span>");
            else if (eitem.Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Cancellation_Rejected)
               message.Append("<span style='color:#CC0033;font-weight:700;font-size:18px;' >" + "Rejected cancellation" + "</span>");
            else if (eitem.Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Canceling)
               message.Append("forwarded for the approval.");
            else
               message.Append("<span style='color:#0E9D41;font-weight:700;font-size:18px;' >" + eitem.Status + "</span>");

            message.Append("<br/> <br />");

            var sdate = DateUtil.ToDisplayFullDate(eitem.Leave.Start_Date);
            if (!string.IsNullOrEmpty(eitem.Leave.Start_Date_Period))
               sdate += " " + eitem.Leave.Start_Date_Period;
            else
               sdate += " " + Period.AM;

            var edate = "";
            if (!eitem.Leave.End_Date.HasValue)
            {
               edate = DateUtil.ToDisplayFullDate(eitem.Leave.Start_Date);
               if (!string.IsNullOrEmpty(eitem.Leave.Start_Date_Period))
                  edate += " " + eitem.Leave.Start_Date_Period;
               else
                  edate += " " + Period.PM;
            }
            else
            {
               edate = DateUtil.ToDisplayFullDate(eitem.Leave.End_Date);
               if (!string.IsNullOrEmpty(eitem.Leave.End_Date_Period))
                  edate += " " + eitem.Leave.End_Date_Period;
               else
                  edate += " " + Period.AM;
            }
            message.Append("<table style='border-collapse: collapse; line-height: 30px;width:100%' cellpadding='6'> ");
            message.Append("<tr style='border-bottom: 1px solid #ccc'>");
            message.Append("<td style='width:120px;'></td>");
            message.Append("<td> <span style='font-weight:700;' ></span></td>");
            message.Append("</tr>");
            message.Append("<tr style='border-bottom: 1px solid #ccc'>");
            message.Append("<td style='width:120px;'>" + Resource.Type + ": </td>");
            message.Append("<td> <span style='font-weight:700;' >" + eitem.Leave.Leave_Config.Leave_Name + "</span></td>");
            message.Append("</tr>");
            message.Append("<tr style='border-bottom: 1px solid #ccc'>");
            message.Append("<td style='width:120px;'>" + Resource.Start_Date + ": </td>");
            message.Append("<td> <span style='font-weight:700;' >" + sdate + "</span></td>");
            message.Append("</tr>");
            message.Append("<tr style='border-bottom: 1px solid #ccc'>");
            message.Append("<td style='width:120px;'>" + Resource.End_Date + ": </td>");
            message.Append("<td> <span style='font-weight:700;' >" + edate + "</span></td>");
            message.Append("</tr>");

            message.Append("<tr style='border-bottom: 1px solid #ccc'>");
            message.Append("<td style='width:120px;'>" + Resource.Total_Days + ": </td>");
            message.Append("<td> <span style='font-weight:700;' >" + eitem.Leave.Days_Taken + " " + Resource.Days.ToLower() + " </span></td>");
            message.Append("</tr>");

            message.Append("<tr style='border-bottom: 1px solid #ccc'>");
            message.Append("<td style='width:120px;'>" + Resource.Notes + ": </td>");
            message.Append("<td> <span style='font-weight:700;' >" + eitem.Leave.Reasons + " " + Resource.Days.ToLower() + " </span></td>");
            message.Append("</tr>");

            if (eitem.Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Cancelled)
            {

            }
            else
            {
               message.Append("<tr style='border-bottom: 1px solid #ccc'>");
               message.Append("<td style='width:120px;'>" + Resource.Approver + ": </td>");
               message.Append("<td> <span style='font-weight:700;' >" + eitem.Approver_Name + "</span></td>");
               message.Append("</tr>");
            }
            message.Append("</table>");

            message.Append("<br/> <br />");
            message.Append("<span  >Your Leave Balance:</span>");
            message.Append("<br/> <br />");
            message.Append("<table style=' line-height:40px;'>");
            message.Append("<tr>");
            message.Append("<td style='border:1px solid #ccc;'>");
            if (eitem.Leave.Relationship_ID.HasValue)
               message.Append("<span style='font-weight:700;font-size:18px;padding:30px;' >" + eitem.Weeks_Left + " " + Resource.Weeks.ToLower() + "</span>");
            else
               message.Append("<span style='font-weight:700;font-size:18px;padding:30px;' >" + eitem.Leave_Left + " " + Resource.Days.ToLower() + "</span>");
            message.Append("</td>");
            message.Append("</tr>");
            message.Append("</table>");

            //Added by sun 09-12-2015
            if (eitem.Leave.Upload_Document != null && eitem.Leave.Upload_Document.Count > 0)
            {
               foreach (var img in eitem.Leave.Upload_Document)
               {
                  MemoryStream ms = new MemoryStream(img.Document);
                  files.Add(new FileAttach { File = ms, File_Name = img.File_Name });
               }
            }
            #endregion
         }
         else if (eitem.Expenses != null)
         {
            #region Expenses
            var totalClaimAmount = 0M;
            message.Append("Your <span style='font-weight:700;' >");
            message.Append(eitem.Approval_Type);

            if (eitem.Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Canceling)
               message.Append(" " + eitem.Status + "</span> request has been ");
            else
               message.Append(" application</span> has been ");

            if (eitem.Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Rejected)
               message.Append("<span style='color:#CC0033;font-weight:700;font-size:18px;' >" + eitem.Status + "</span>");
            else if (eitem.Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Cancellation_Rejected)
               message.Append("<span style='color:#CC0033;font-weight:700;font-size:18px;' >" + "Rejected cancellation" + "</span>");
            else if (eitem.Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Canceling)
               message.Append("forwarded for the approval.");
            else
               message.Append("<span style='color:#0E9D41;font-weight:700;font-size:18px;' >" + eitem.Status + "</span>");

            message.Append("<br/> <br />");

            foreach (var row in eitem.Expenses.Expenses_Application_Document)
               totalClaimAmount += row.Amount_Claiming.Value;

            message.Append("<table style='border-collapse: collapse; line-height: 30px;width:100%' cellpadding='6'> ");
            message.Append("<tr style='border-bottom: 1px solid #ccc'>");
            message.Append("<td style='width:120px;'></td>");
            message.Append("<td> <span style='font-weight:700;' ></span></td>");
            message.Append("</tr>");
            message.Append("<tr style='border-bottom: 1px solid #ccc'>");
            message.Append("<td style='width:120px;'>" + Resource.Expenses_Title + ": </td>");
            message.Append("<td> <span style='font-weight:700;' >" + eitem.Expenses.Expenses_Title + "</span></td>");
            message.Append("</tr>");
            message.Append("<tr style='border-bottom: 1px solid #ccc'>");
            message.Append("<td style='width:120px;'>" + Resource.Date_Applied + ": </td>");
            message.Append("<td> <span style='font-weight:700;' >" + DateUtil.ToDisplayFullDate(eitem.Expenses.Date_Applied) + "</span></td>");
            message.Append("</tr>");
            message.Append("<tr style='border-bottom: 1px solid #ccc'>");
            message.Append("<td style='width:120px;'>" + Resource.Total_Amount + ": </td>");
            message.Append("<td> <span style='font-weight:700;' >" + totalClaimAmount.ToString("n2") + " " + (eitem.Company != null ? eitem.Company.Currency.Currency_Code : "") + "</span></td>");
            message.Append("</tr>");

            if (eitem.Status != SBSWorkFlowAPI.Constants.WorkflowStatus.Cancelled)
            {
               message.Append("<tr style='border-bottom: 1px solid #ccc'>");
               message.Append("<td style='width:120px;'>" + Resource.Approver + ": </td>");
               message.Append("<td> <span style='font-weight:700;' >" + eitem.Approver_Name + "</span></td>");
               message.Append("</tr>");
            }

            message.Append("</table>");
            message.Append("<br/> <br />");

            message.Append("<table style='border-collapse: collapse; line-height: 30px;width:100%' cellpadding='6'>");
            message.Append("<tr style='background-color: #E2ECFB'>");
            message.Append("<td><strong>" + Resource.Expenses_Type + "</strong></td>");
            message.Append("<td><strong>" + Resource.Expenses_Date + "</strong></td>");
            message.Append("<td><strong>" + Resource.Total_Amount + "</strong></td>");
            message.Append("<td><strong>" + Resource.Amount_Claiming + "</strong></td>");
            message.Append("</tr>");

            var even = false;
            foreach (var frow in eitem.Expenses.Expenses_Application_Document)
            {
               if (even)
                  message.Append("<tr style='background-color: #F7F7F7'>");
               else
                  message.Append("<tr>");

               message.Append("<td>" + frow.Expenses_Config.Expenses_Name + "</td>");
               message.Append("<td>" + DateUtil.ToDisplayFullDate(frow.Expenses_Date) + "</td>");
               message.Append("<td>" + frow.Total_Amount.Value.ToString("n2") + " " + (eitem.Company != null ? eitem.Company.Currency.Currency_Code : "") + "</td>");
               message.Append("<td>" + frow.Amount_Claiming.Value.ToString("n2") + " " + (eitem.Company != null ? eitem.Company.Currency.Currency_Code : "") + "</td>");
               message.Append("</tr>");
               even = !even;

               if (frow.Upload_Receipt != null && frow.Upload_Receipt.Count > 0)
               {
                  foreach (var img in frow.Upload_Receipt)
                  {
                     MemoryStream ms = new MemoryStream(img.Receipt);
                     files.Add(new FileAttach { File = ms, File_Name = img.File_Name });
                  }
               }
            }

            //******** Start Smart Dev  ********//
            if (eitem.Attachment_SmartDev != null)
            {
               files.Add(eitem.Attachment_SmartDev);
            }
            //******** End Smart Dev  ********//

            message.Append("<tr style='background-color: #E2ECFB'>");
            message.Append("<td><strong>" + Resource.Total_Amount + "</strong></td>");
            message.Append("<td/><td/>");
            message.Append("<td><strong>" + totalClaimAmount.ToString("n2") + " " + (eitem.Company != null ? eitem.Company.Currency.Currency_Code : "") + "</strong></td>");
            message.Append("</tr>");
            message.Append("</table>");
            message.Append("<br/> <br />");
            #endregion
         }
         //return NewEmailNotification("voranun@bluecube.com.sg", subject, message.ToString(), eitem.Reviewer, files, eitem.Url, eitem.Approval_Type);
         return NewEmailNotification(eitem.Send_To_Email, subject, message.ToString(), eitem.Reviewer, files, eitem.Url, eitem.Approval_Type, eitem.Doc_ID);

      }

      #endregion

      #region Report
      public static bool sendReportProblem(EmailItem eitem)
      {
         var message = new StringBuilder();
         message.Append("Dear <span >" + eitem.Send_To_Name + "</span>,");
         message.Append("<br/> <br />");
         message.Append("You have received a <span style='font-weight:700;' > report a problem</span> from <span style='font-weight:700;' >" + eitem.Received_From_Name + "</span> - with the following details:");
         message.Append("<br/> <br />");
         message.Append("<br/> <br />");

         var mems = new List<FileAttach>();
         if (eitem.Report_Log != null)
         {
            message.Append(eitem.Report_Log.Description);
            if (eitem.Report_Log.Screen_Capture_Image != null)
            {
               foreach (var img in eitem.Report_Log.Screen_Capture_Image)
               {
                  MemoryStream ms = new MemoryStream(img.Image);
                  mems.Add(new FileAttach { File = ms, File_Name = img.File_Name });

               }
            }
         }

         return sendNotificationEmail(eitem.Send_To_Email, "Report a Problem", message.ToString(), null, mems, eitem.Url);
      }
      #endregion

      #region
      public static bool sendNotificationEmail(string to, string header, string message, List<SBSWorkFlowAPI.Models.Reviewer> cc = null, List<FileAttach> files = null, string url = "")
      {
         log4net.Config.XmlConfigurator.Configure();
         log = log4net.LogManager.GetLogger(typeof(UrlUtil));
         try
         {
            if (!string.IsNullOrEmpty(url))
            {
               if (!Directory.Exists(@"c:\sbs\sbs_email_send_out"))
                  Directory.CreateDirectory(@"c:\sbs\sbs_email_send_out");

               var path = @"c:\sbs\sbs_email_send_out\" + header.Replace(":", "_") + ".htm";
               using (FileStream fs = new FileStream(path, FileMode.Create))
               {
                  using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8))
                  {
                     w.WriteLine(message);
                  }
               }
            }


            var SMTP_SERVER = ConfigurationManager.AppSettings["SMTP_SERVER"].ToString();
            var SMTP_PORT = Convert.ToInt32(ConfigurationManager.AppSettings["SMTP_PORT"].ToString());
            var SMTP_USERNAME = ConfigurationManager.AppSettings["SMTP_USERNAME"].ToString();
            var SMTP_PASSWORD = ConfigurationManager.AppSettings["SMTP_PASSWORD"].ToString();
            bool STMP_SSL = false;
            bool.TryParse(ConfigurationManager.AppSettings["SMTP_SSL"].ToString(), out STMP_SSL);

            SmtpClient smtpClient = new SmtpClient(SMTP_SERVER, SMTP_PORT);
            System.Net.NetworkCredential cred = new System.Net.NetworkCredential();
            cred.UserName = SMTP_USERNAME;
            cred.Password = SMTP_PASSWORD;

            smtpClient.UseDefaultCredentials = false;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = STMP_SSL;

            message += "<br/> <br />" +
                "<br/> <br />" +
                "This is an auto-generated email. Please do not reply.";


            MailMessage mail = new MailMessage(SMTP_USERNAME, to, header, message);

            if (cc != null && cc.Count > 0)
            {
               foreach (var row in cc)
               {
                  mail.CC.Add(new MailAddress(row.Email));
               }
            }

            if (files != null && files.Count > 0)
            {
               foreach (var file in files)
               {
                  Attachment data = new Attachment(file.File, file.File_Name);
                  mail.Attachments.Add(data);
               }
            }

            mail.BodyEncoding = Encoding.UTF8;
            mail.IsBodyHtml = true;

            smtpClient.Credentials = cred;
            //mail.Bcc.Add("sbs.datasource@gmail.com");
            smtpClient.Send(mail);

            return true;
         }
         catch (Exception ex)
         {
            Debug.WriteLine(ex.Message);
            log.Error(DateTime.Now, ex);
            System.Diagnostics.Debug.WriteLine("EMAIL CANNOT BE SENT");
            return false;
         }
      }

      public static string getEmailActionButtons(string[] actionList, string[] url, string[] color)
      {
         var sb = new StringBuilder();
         var ctr = 0;
         sb.Append("<div> <!--[if mso]>");
         foreach (var action in actionList)
         {
            sb.Append("<v:roundrect xmlns:v='urn:schemas-microsoft-com:vml' xmlns:w='urn:schemas-microsoft-com:office:word' href='" + url[ctr] + "' style='height:60px;v-text-anchor:middle;width:150px;' arcsize='10%' stroke='f' fillcolor='" + color[ctr] + "'>");
            sb.Append("<w:anchorlock/>");
            sb.Append("<center style='color:#ffffff;font-family:sans-serif;font-size:16px;font-weight:bold;'>");
            sb.Append(actionList[ctr]);
            sb.Append("</center>");
            sb.Append("</v:roundrect>");

            ctr++;
         }
         sb.Append("<![endif]-->");
         sb.Append("<![if !mso]>");
         sb.Append("<table cellspacing='0' cellpadding='0'> <tr>");

         var ctr2 = 0;
         foreach (var action in actionList)
         {
            sb.Append("<td align='center' width='150' height='60' bgcolor='" + color[ctr2] + "' style='-webkit-border-radius: 5px; -moz-border-radius: 5px; border-radius: 5px; color: #ffffff;'>");
            sb.Append("<a href='" + url[ctr2] + "' style='font-size:16px; font-weight: bold; font-family:sans-serif; text-decoration: none; line-height:40px; width:100%; display:inline-block'>");
            sb.Append("<span style='color: #ffffff;'>");
            sb.Append(actionList[ctr2]);
            sb.Append("</span>");
            sb.Append("</a></td>");
            sb.Append("<td width='5' >&nbsp;</td>");

            ctr2++;
         }

         sb.Append("</tr></table><![endif]></div>");

         return sb.ToString();
      }

      // approver send approve email to employee (for final approve only)
      public static bool sendApproveEmail(String email, String employeeName, string module, string approverName, string domain, string cc = null)
      {

         String message = "Dear " + employeeName + "," +
         "<br/> <br />" +
         "Your " + module + " has been approved by " + approverName + "." +
         "<br/> <br />" +
         "Please login to " + domain + " to check the detail of the " + module + " application." +
         "<br/> <br />" +
         "If you have encounter any technical issues, please contact your system administrator for help." +
         "<br/> <br />" +
         "Regards," +
         "<br/> <br />" +
         module + " System";
         return sendNotificationEmail(email, module + " Application", message, null, null, domain);

      }

      public static bool sendApproveEmail(String email, String employeeName, string module, string approverName, int id, string domain)
      {
         string link = "";
         if (module == "Quotation")
         {
            link = domain + "Quotation/Quotation?pQuotationID=" + id + "&operation=U";
         }
         else if (module == "SaleOrder")
         {
            link = domain + "SaleOrder/SaleOrder?pSaleOrderID=" + id + "&operation=U";
         }
         else if (module == "PurchaseOrder")
         {
            link = domain + "Purchase/PurchaseOrder?pid=" + id + "&operation=U";
         }


         String message = "Dear " + employeeName + "," +
         "<br/> <br />" +
         "Your " + module + " has been approved by " + approverName + "." +
         "<br/> <br />" +
         "Please login to " + link + " to check the detail of the " + module + " application." +
         "<br/> <br />" +
         "If you have encounter any technical issues, please contact your system administrator for help." +
         "<br/> <br />" +
         "<br/> <br />" +
         "Regards," +
         "<br/> <br />" +
         module + " System" +
         "<br/> <br />" +
         "THIS IS AN AUTO-GENERATED EMAIL. PLEASE DO NOT REPLY TO THIS EMAIL.";
         return sendNotificationEmail(email, module + " Application", message, null, null, domain);

      }

      public static bool sendRejectEmail(String email, String employeeName, string module, string approverName, int id, string domain)
      {
         string link = "";
         if (module == "Quotation")
         {
            link = domain + "Quotation/Quotation?pQuotationID=" + id + "&operation=U";
         }
         else if (module == "SaleOrder")
         {
            link = domain + "SaleOrder/SaleOrder?pSaleOrderID=" + id + "&operation=U";
         }
         else if (module == "PurchaseOrder")
         {
            link = domain + "Purchase/PurchaseOrder?pid=" + id + "&operation=U";
         }


         String message = "Dear " + employeeName + "," +
         "<br/> <br />" +
         "Your " + module + " has been rejected by " + approverName + "." +
         "<br/> <br />" +
         "Please login to " + link + " to check the detail of the " + module + " application." +
         "<br/> <br />" +
         "If you have encounter any technical issues, please contact your system administrator for help." +
         "<br/> <br />" +
         "<br/> <br />" +
         "Regards," +
         "<br/> <br />" +
         module + " System" +
         "<br/> <br />" +
         "THIS IS AN AUTO-GENERATED EMAIL. PLEASE DO NOT REPLY TO THIS EMAIL.";
         return sendNotificationEmail(email, module + " Application", message, null, null, domain);

      }

      // Added by Moet on 12/Sep
      public static bool sendInvoiceEmail(EmailItem eitem, decimal dueAmt, decimal OutAmt, int OutMonth, List<Storage_Upgrade> lstUpg, MemoryStream mm, string domain, string cc = null)
      {
         //string projectName = "AuthenSBS2";
         //if (AppSetting.IsStaging == "true")
         //    projectName = "AuthenSBS2-Staging";
         //string link = domain + "/" + projectName + "/Subscription/BillingReport";
         var message = new StringBuilder();
         message.Append("Dear " + eitem.Send_To_Name + ",");

         message.Append("<br/> <br />");
         string strMonthName = DateTime.Now.ToString("MMM", CultureInfo.InvariantCulture);
         message.Append("The " + strMonthName + " Invoice is ready for your preview!");
         message.Append("<br/> <br />");
         message.Append("Please click the <strong>Payment</strong> button below to make your payment or <strong>Preview</strong> button to view the Invoice details.");
         message.Append("<br/> <br />");
         message.Append("Alternatively, please login to " + eitem.Link2 + " to make the payment.");
         message.Append("<br/> <br />");
         message.Append("<table width='50%'>");
         if (OutMonth > 0)
         {
            string strOutMonthName = new DateTime(2016, OutMonth, 1).ToString("MMM", CultureInfo.InvariantCulture);
            message.Append("<tr>");
            message.Append("<td>Outstanding Balance</td><td rowspan='2'>USD" + OutAmt + "</td>");
            message.Append("</tr>");
            message.Append("<tr>");
            message.Append("<td>(" + strOutMonthName + ")</td>");
            message.Append("</tr>");
         }
         message.Append("<tr>");
         message.Append("<td>SBSolution HRM </td><td rowspan='2'>USD" + dueAmt + "</td>");
         message.Append("</tr>");
         message.Append("<tr>");
         message.Append("<td>(" + strMonthName + ")</td>");
         message.Append("</tr>");
         decimal upgAmt = 0;
         foreach (var r in lstUpg)
         {
            var uStorage = r.Upgrade_Space;
            var uUnit = "MB";
            if (r.Upgrade_Space >= 1000)
            {
               uStorage = r.Upgrade_Space / 1000;
               uUnit = "GB";
            }
            message.Append("<tr>");
            message.Append("<td>Upgrade Storage - " + uStorage + uUnit + "(" + DateUtil.ToDisplayDDMMMYYYY(r.Upgrade_On.Value) + ")</td>");
            message.Append("<td>USD" + r.Price + "</td>");
            message.Append("</tr>");
            upgAmt += r.Price;
         }
         message.Append("<tr>");
         decimal allSumAmt = dueAmt + OutAmt + upgAmt;
         message.Append("<td>Total amount: </td><td>USD" + allSumAmt + "</td>");
         message.Append("</tr>");
         message.Append("</table>");

         message.Append("For your action: <br/><br/>");

         string[] actionList = new string[] { "Payment", "Preview" };
         string[] urls = new string[] { eitem.Link, eitem.Link2 };
         string[] colors = new string[] { "#89e0a5", "#f53649" };

         message.Append(getEmailActionButtons(actionList, urls, colors));
         //message.Append("<br/><br/>This is an auto-generated email. Please do not reply.");

         var mems = new List<FileAttach>();
         mems.Add(new FileAttach { File_Name = eitem.ECode + ".pdf", File = mm });
         return sendInvoiceNotificationEmail(eitem.Send_To_Email, cc, "Invoice #" + eitem.ECode, message.ToString(), mems);

      }

      // Added by Moet on 12/Sep
      public static bool sendInvoiceNotificationEmail(string to, string cc, string header, string message, List<FileAttach> files = null)
      {
         try
         {
            var SMTP_SERVER = ConfigurationManager.AppSettings["SMTP_SERVER"].ToString();
            var SMTP_PORT = Convert.ToInt32(ConfigurationManager.AppSettings["SMTP_PORT"].ToString());
            var SMTP_USERNAME = ConfigurationManager.AppSettings["SMTP_USERNAME"].ToString();
            var SMTP_PASSWORD = ConfigurationManager.AppSettings["SMTP_PASSWORD"].ToString();
            bool STMP_SSL = false;
            bool.TryParse(ConfigurationManager.AppSettings["SMTP_SSL"].ToString(), out STMP_SSL);

            SmtpClient smtpClient = new SmtpClient(SMTP_SERVER, SMTP_PORT);
            System.Net.NetworkCredential cred = new System.Net.NetworkCredential();
            cred.UserName = SMTP_USERNAME;
            cred.Password = SMTP_PASSWORD;

            smtpClient.UseDefaultCredentials = true;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = STMP_SSL;

            message += "<br/> <br />" +
                "<br/> <br />" +
                "This is an auto-generated email. Please do not reply.";


            MailMessage mail = new MailMessage(SMTP_USERNAME, to, header, message);

            //if (cc != null && cc.Count > 0)
            //{
            //    foreach (var row in cc)
            //    {
            //        mail.CC.Add(new MailAddress(row.Email));
            //    }
            //}
            mail.CC.Add(new MailAddress(cc));

            if (files != null && files.Count > 0)
            {
               foreach (var file in files)
               {
                  Attachment data = new Attachment(file.File, file.File_Name);
                  mail.Attachments.Add(data);
               }
            }

            mail.BodyEncoding = Encoding.UTF8;
            mail.IsBodyHtml = true;

            smtpClient.Credentials = cred;
            //mail.Bcc.Add("sbs.datasource@gmail.com");
            smtpClient.Send(mail);

            return true;
         }
         catch
         {
            System.Diagnostics.Debug.WriteLine("EMAIL CANNOT BE SENT");
            return false;
         }
      }
      #endregion

      #region Email Notification
      public static bool NewEmailNotification(string to, string header, string message, List<Reviewer> cc = null, List<FileAttach> files = null, string url = "", string approvalType = "", int? docID = null)
      {
         var currentdate = StoredProcedure.GetCurrentDate();
         var SchService = new ScheddulerService();
         var email = new Email_Notification();

         LogUtil.WriteLog("Module: " + approvalType + " Receiver Email: " + to, "Scheduled_Log");

         //email.Company_ID = 1;
         email.Sender = ConfigurationManager.AppSettings["SMTP_USERNAME"].ToString();
         email.Receiver = to;
         email.Subject = header;
         email.Message = message;
         email.Module = approvalType;
         email.Create_On = currentdate;
         email.Update_On = currentdate;
         email.Status = ScheduledAction.Pending;
         email.Doc_ID = docID;

         if (cc != null && cc.Count > 0)
            email.CC = (string.Join(",", cc.Select(x => x.Email.ToString()).ToArray()));

         if (files != null && files.Count > 0)
         {
            var attacs = new List<Email_Attachment>();
            foreach (var file in files)
            {
               var attac = new Email_Attachment();
               attac.Attachment_File_Name = file.File_Name;
               attac.Attachment_File = file.File.ToArray();
               attacs.Add(attac);
            }
            email.Email_Attachment = attacs;
         }
         return SchService.InsertEmailNotification(email);
      }

      public static ServiceResult sendEmailNotification(string to, string header, string message, string cc = "", List<Email_Attachment> files = null, string url = "")
      {
         try
         {
            if (!string.IsNullOrEmpty(url))
            {
               if (!Directory.Exists(@"c:\sbs\sbs_email_send_out"))
                  Directory.CreateDirectory(@"c:\sbs\sbs_email_send_out");

               var path = @"c:\sbs\sbs_email_send_out\" + header.Replace(":", "_") + ".htm";
               using (FileStream fs = new FileStream(path, FileMode.Create))
               {
                  using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8))
                  {
                     w.WriteLine(message);
                  }
               }
            }

            var SMTP_SERVER = ConfigurationManager.AppSettings["SMTP_SERVER"].ToString();
            var SMTP_PORT = Convert.ToInt32(ConfigurationManager.AppSettings["SMTP_PORT"].ToString());
            var SMTP_USERNAME = ConfigurationManager.AppSettings["SMTP_USERNAME"].ToString();
            var SMTP_PASSWORD = ConfigurationManager.AppSettings["SMTP_PASSWORD"].ToString();
            bool STMP_SSL = false;
            bool.TryParse(ConfigurationManager.AppSettings["SMTP_SSL"].ToString(), out STMP_SSL);

            SmtpClient smtpClient = new SmtpClient(SMTP_SERVER, SMTP_PORT);
            System.Net.NetworkCredential cred = new System.Net.NetworkCredential();
            cred.UserName = SMTP_USERNAME;
            cred.Password = SMTP_PASSWORD;

            smtpClient.UseDefaultCredentials = false;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = STMP_SSL;

            message += "<br/> <br />" +
                "<br/> <br />" +
                "This is an auto-generated email. Please do not reply.";

            MailMessage mail = new MailMessage(SMTP_USERNAME, to, header, message);
            if (!string.IsNullOrEmpty(cc))
            {
               string[] CCId = cc.Split(',');
               foreach (string CCEmail in CCId)
                  mail.CC.Add(new MailAddress(CCEmail)); //Adding Multiple CC email  
            }

            if (files != null && files.Count > 0)
            {
               foreach (var file in files)
               {
                  if (file.Attachment_File.Length > 0)
                     mail.Attachments.Add(new Attachment(new MemoryStream(file.Attachment_File), file.Attachment_File_Name));
               }
            }

            mail.BodyEncoding = Encoding.UTF8;
            mail.IsBodyHtml = true;
            smtpClient.Credentials = cred;
            smtpClient.Send(mail);
            return new ServiceResult() { Code = ERROR_CODE.SUCCESS, Msg = new Success().getSuccess(ERROR_CODE.SUCCESS_SEND_EMAIL), Field = "Send Email" };
         }
         catch (Exception ex)
         {
            Debug.WriteLine(ex.Message);
            return new ServiceResult() { Code = ERROR_CODE.ERROR_501_CANT_SEND_EMAIL, Msg = ex.Message, Field = "Send Email" };
         }
      }
      #endregion

   }
}