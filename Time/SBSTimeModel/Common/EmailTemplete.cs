using SBSTimeModel.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web;
using System.Configuration;
using SBSModel.Models;
using SBSModel.Common;
using SBSResourceAPI;

namespace SBSTimeModel.Common
{
   public partial class EmailTimeItem : EmailItem
   {
      public Time_Sheet TimeSheet { get; set; }

   }
   public class EmailTimeTemplete : EmailTemplete
   {
      /* System send email to approval for request some action (approve or reject)*/
      public static bool sendRequestEmail(EmailTimeItem eitem)
      {
         var message = new StringBuilder();
         var files = new List<FileAttach>();
         var subject = "";
         //if (!string.IsNullOrEmpty(eitem.ECode))
         //   subject += eitem.ECode + " ";

         //subject += eitem.Approval_Type + " Pending Approval";
         //if (eitem.Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Canceling)
         //   subject += " Cancellation";

         //message.Append("Dear <span >" + eitem.Send_To_Name + "</span>,");
         //message.Append("<br/> <br />");

         //if (eitem.Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Canceling)
         //   message.Append("You have received a " + eitem.Approval_Type + " " + eitem.Status + " request from <span style='font-weight:700;' >" + eitem.Received_From_Name + "</span> - " + eitem.Received_From_Department + " with the following details:");
         //else if (eitem.Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Cancelled)
         //   message.Append(eitem.Received_From_Name + " has been<span style='color:#0E9D41;font-weight:700;font-size:18px;' >" + " Cancelled  " + "</span> the " + eitem.Approval_Type + "</span> application with the following details:");
         //else
         //   message.Append("You have received a <span style='font-weight:700;' >" + eitem.Approval_Type + " application</span> from <span style='font-weight:700;' >" + eitem.Received_From_Name + "</span> - " + eitem.Received_From_Department + " with the following details:");

         //message.Append("<br/> <br />");
         //if (eitem.TimeSheet != null)
         //{
         //   var clockIn = DateUtil.ToDisplayTime(eitem.TimeSheet.Clock_In);
         //   var clockOut = DateUtil.ToDisplayTime(eitem.TimeSheet.Clock_Out);
         //   var date = DateUtil.ToDisplayFullDate(eitem.TimeSheet.Date_Of_Date);

         //   message.Append("<table style='border-collapse: collapse; line-height: 30px;width:100%' cellpadding='6'> ");
         //   message.Append("<tr style='border-bottom: 1px solid #ccc'>");
         //   message.Append("<td style='width:120px;'></td>");
         //   message.Append("<td> <span style='font-weight:700;' ></span></td>");
         //   message.Append("</tr>");
         //   message.Append("<tr style='border-bottom: 1px solid #ccc'>");
         //   message.Append("<td style='width:120px;'>" + Resource.Indent_Name + ": </td>");
         //   message.Append("<td> <span style='font-weight:700;' >" + eitem.TimeSheet.Indent_Name + "</span></td>");
         //   message.Append("</tr>");
         //   message.Append("<tr style='border-bottom: 1px solid #ccc'>");
         //   message.Append("<td style='width:120px;'>" + Resource.Date + ": </td>");
         //   message.Append("<td> <span style='font-weight:700;' >" + date + "</span></td>");
         //   message.Append("</tr>");
         //   message.Append("</tr>");
         //   message.Append("<tr style='border-bottom: 1px solid #ccc'>");
         //   message.Append("<td style='width:120px;'>" + Resource.Clock_In + ": </td>");
         //   message.Append("<td> <span style='font-weight:700;' >" + clockIn + "</span></td>");
         //   message.Append("</tr>");
         //   message.Append("<tr style='border-bottom: 1px solid #ccc'>");
         //   message.Append("<td style='width:120px;'>" + Resource.Clock_Out + ": </td>");
         //   message.Append("<td> <span style='font-weight:700;' >" + clockOut + "</span></td>");
         //   message.Append("</tr>");
         //   message.Append("<tr style='border-bottom: 1px solid #ccc'>");
         //   message.Append("<td style='width:120px;'>" + Resource.Duration + ": </td>");
         //   message.Append("<td> <span style='font-weight:700;' >" + eitem.TimeSheet.Duration + "</span></td>");
         //   message.Append("</tr>");

         //   message.Append("<tr style='border-bottom: 1px solid #ccc'>");
         //   message.Append("<td style='width:120px;'>" + Resource.Total_Amount_SymbolDollar + ": </td>");
         //   message.Append("<td> <span style='font-weight:700;' >" + NumUtil.FormatCurrency(eitem.TimeSheet.Total_Amount,2) + "</span></td>");
         //   message.Append("</tr>");

         //   if (eitem.TimeSheet.Note != null)
         //   {
         //      message.Append("<tr style='border-bottom: 1px solid #ccc'>");
         //      message.Append("<td style='width:120px;'>" + Resource.Description + ": </td>");
         //      message.Append("<td> <span style='font-weight:700;' >" + eitem.TimeSheet.Note + "</span></td>");
         //      message.Append("</tr>");
         //   }

         //   message.Append("</table>");
         //   message.Append("<br/> <br />");

         //   if (!string.IsNullOrEmpty(eitem.Link) && !string.IsNullOrEmpty(eitem.Link2))
         //   {
         //      message.Append("For your action: <br/><br/>");

         //      string[] actionList = new string[] { "Approve", "Reject" };
         //      string[] urls = new string[] { eitem.Link, eitem.Link2 };
         //      string[] colors = new string[] { "#89e0a5", "#f53649" };

         //      message.Append(getEmailActionButtons(actionList, urls, colors));
         //   }
         //}

         return NewEmailNotification(eitem.Send_To_Email, subject, message.ToString(), eitem.Reviewer, null, eitem.Url, eitem.Approval_Type, eitem.Doc_ID);
         //return sendNotificationEmail(eitem.Send_To_Email, subject, message.ToString(), eitem.Reviewer, files, eitem.LogoLink);
      }

      /* System send email to employee after they create an application.*/
      public static bool sendProceedEmail(EmailTimeItem eitem)
      {
         var message = new StringBuilder();

         var subject = "";
         if (!string.IsNullOrEmpty(eitem.ECode))
            subject += eitem.ECode + " ";

         //subject += eitem.Approval_Type + " " + eitem.Status + " Application";

         //message.Append("Dear <span >" + eitem.Send_To_Name + "</span>,");
         //message.Append("<br/> <br />");

         //if (eitem.TimeSheet != null)
         //{
         //   message.Append("Your <span style='font-weight:700;' >");
         //   message.Append(eitem.Approval_Type);

         //   if (eitem.Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Canceling)
         //      message.Append(" " + eitem.Status + "</span> request has been ");
         //   else
         //      message.Append(" application</span> has been ");

         //   if (eitem.Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Rejected)
         //      message.Append("<span style='color:#CC0033;font-weight:700;font-size:18px;' >" + eitem.Status + "</span>");
         //   else if (eitem.Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Cancellation_Rejected)
         //      message.Append("<span style='color:#CC0033;font-weight:700;font-size:18px;' >" + "Rejected cancellation" + "</span>");
         //   else if (eitem.Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Canceling)
         //      message.Append("forwarded for the approval.");
         //   else
         //      message.Append("<span style='color:#0E9D41;font-weight:700;font-size:18px;' >" + eitem.Status + "</span>");

         //   message.Append("<br/> <br />");

         //   var clockIn = DateUtil.ToDisplayTime(eitem.TimeSheet.Clock_In);
         //   var clockOut = DateUtil.ToDisplayTime(eitem.TimeSheet.Clock_Out);
         //   var date = DateUtil.ToDisplayFullDate(eitem.TimeSheet.Date_Of_Date);

         //   message.Append("<table style='border-collapse: collapse; line-height: 30px;width:100%' cellpadding='6'> ");
         //   message.Append("<tr style='border-bottom: 1px solid #ccc'>");
         //   message.Append("<td style='width:120px;'></td>");
         //   message.Append("<td> <span style='font-weight:700;' ></span></td>");
         //   message.Append("</tr>");
         //   message.Append("<tr style='border-bottom: 1px solid #ccc'>");
         //   message.Append("<td style='width:120px;'>" + Resource.Indent_Name + ": </td>");
         //   message.Append("<td> <span style='font-weight:700;' >" + eitem.TimeSheet.Indent_Name + "</span></td>");
         //   message.Append("</tr>");
         //   message.Append("<tr style='border-bottom: 1px solid #ccc'>");
         //   message.Append("<td style='width:120px;'>" + Resource.Date + ": </td>");
         //   message.Append("<td> <span style='font-weight:700;' >" + date + "</span></td>");
         //   message.Append("</tr>");
         //   message.Append("</tr>");
         //   message.Append("<tr style='border-bottom: 1px solid #ccc'>");
         //   message.Append("<td style='width:120px;'>" + Resource.Clock_In + ": </td>");
         //   message.Append("<td> <span style='font-weight:700;' >" + clockIn + "</span></td>");
         //   message.Append("</tr>");
         //   message.Append("<tr style='border-bottom: 1px solid #ccc'>");
         //   message.Append("<td style='width:120px;'>" + Resource.Clock_Out + ": </td>");
         //   message.Append("<td> <span style='font-weight:700;' >" + clockOut + "</span></td>");
         //   message.Append("</tr>");
         //   message.Append("<tr style='border-bottom: 1px solid #ccc'>");
         //   message.Append("<td style='width:120px;'>" + Resource.Duration + ": </td>");
         //   message.Append("<td> <span style='font-weight:700;' >" + eitem.TimeSheet.Duration + "</span></td>");
         //   message.Append("</tr>");

         //   message.Append("<tr style='border-bottom: 1px solid #ccc'>");
         //   message.Append("<td style='width:120px;'>" + Resource.Total_Amount_SymbolDollar + ": </td>");
         //   message.Append("<td> <span style='font-weight:700;' >" + NumUtil.FormatCurrency(eitem.TimeSheet.Total_Amount,2) + "</span></td>");
         //   message.Append("</tr>");

         //   if (eitem.Status == SBSWorkFlowAPI.Constants.WorkflowStatus.Cancelled)
         //   {
         //   }
         //   else
         //   {
         //      message.Append("<tr style='border-bottom: 1px solid #ccc'>");
         //      message.Append("<td style='width:120px;'>" + Resource.Approver + ": </td>");
         //      message.Append("<td> <span style='font-weight:700;' >" + eitem.Approver_Name + "</span></td>");
         //      message.Append("</tr>");
         //   }
         //   message.Append("</table>");
         //}

         return NewEmailNotification(eitem.Send_To_Email, subject, message.ToString(), eitem.Reviewer, null, eitem.Url, eitem.Approval_Type, eitem.Doc_ID);
         //return sendNotificationEmail(eitem.Send_To_Email, subject, message.ToString(), eitem.Reviewer, null, eitem.LogoLink);

      }

      //# region For Test send email
      //public static bool sendNotificationEmailTest(string to, string header, string message, List<SBSWorkFlowAPI.Models.Reviewer> cc = null, List<FileAttach> files = null, string url = "")
      //{
      //   try
      //   {
      //      if (!string.IsNullOrEmpty(url) && url.Contains("localhost"))
      //      {
      //         if (!Directory.Exists(@"c:\sbs\sbs_email_send_out"))
      //            Directory.CreateDirectory(@"c:\sbs\sbs_email_send_out");

      //         var path = @"c:\sbs\sbs_email_send_out\" + header.Replace(":", "_") + ".htm";
      //         using (FileStream fs = new FileStream(path, FileMode.Create))
      //         {
      //            using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8))
      //            {
      //               w.WriteLine(message);
      //            }
      //         }
      //      }
      //      if (to.Contains("smartdevsolution") | to.Contains("kpngreen"))
      //      {
      //         return true;
      //      }

      //      var SMTP_SERVER = ConfigurationManager.AppSettings["SMTP_SERVER"].ToString();
      //      var SMTP_PORT = Convert.ToInt32(ConfigurationManager.AppSettings["SMTP_PORT"].ToString());
      //      var SMTP_USERNAME = ConfigurationManager.AppSettings["SMTP_USERNAME"].ToString();
      //      var SMTP_PASSWORD = ConfigurationManager.AppSettings["SMTP_PASSWORD"].ToString();
      //      bool STMP_SSL = false;
      //      bool.TryParse(ConfigurationManager.AppSettings["SMTP_SSL"].ToString(), out STMP_SSL);

      //      SmtpClient smtpClient = new SmtpClient(SMTP_SERVER, SMTP_PORT);
      //      System.Net.NetworkCredential cred = new System.Net.NetworkCredential();
      //      cred.UserName = SMTP_USERNAME;
      //      cred.Password = SMTP_PASSWORD;

      //      smtpClient.UseDefaultCredentials = true;
      //      smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
      //      smtpClient.EnableSsl = STMP_SSL;

      //      message += "<br/> <br />" +
      //          "<br/> <br />" +
      //          "This is an auto-generated email. Please do not reply.";


      //      MailMessage mail = new MailMessage(SMTP_USERNAME, to, header, message);

      //      if (cc != null && cc.Count > 0)
      //      {
      //         foreach (var row in cc)
      //         {
      //            mail.CC.Add(new MailAddress(row.Email));
      //         }
      //      }

      //      if (files != null && files.Count > 0)
      //      {
      //         foreach (var file in files)
      //         {
      //            Attachment data = new Attachment(file.File, file.File_Name);
      //            mail.Attachments.Add(data);
      //         }
      //      }

      //      mail.BodyEncoding = Encoding.UTF8;
      //      mail.IsBodyHtml = true;

      //      smtpClient.Credentials = cred;
      //      mail.To.Add("sbs.datasource@gmail.com");
      //      smtpClient.Send(mail);

      //      return true;
      //   }
      //   catch
      //   {
      //      System.Diagnostics.Debug.WriteLine("EMAIL CANNOT BE SENT");
      //      return false;
      //   }
      //}
      //#endregion
   }
}
