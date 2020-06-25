using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace POS.Common
{
    public class EmailTemplete
    {

        public static bool sendNotificationEmail(String to, String header, String message, String cc = "")
        {
            try
            {
                var SMTP_SERVER = WebConfigurationManager.AppSettings["SMTP_SERVER"].ToString();
                var SMTP_PORT = Convert.ToInt32(WebConfigurationManager.AppSettings["SMTP_PORT"].ToString());
                var SMTP_USERNAME = WebConfigurationManager.AppSettings["SMTP_USERNAME"].ToString();
                var SMTP_PASSWORD = WebConfigurationManager.AppSettings["SMTP_PASSWORD"].ToString();
                bool STMP_SSL = false;
                bool.TryParse(WebConfigurationManager.AppSettings["SMTP_SSL"].ToString(), out STMP_SSL);

                SmtpClient smtpClient = new SmtpClient(SMTP_SERVER, SMTP_PORT);

                System.Net.NetworkCredential cred = new System.Net.NetworkCredential();
                cred.UserName = SMTP_USERNAME;
                cred.Password = SMTP_PASSWORD;

                smtpClient.UseDefaultCredentials = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.EnableSsl = STMP_SSL;

                message += "<br/> <br />" +
                    "<br/> <br />" +
                    "This is an auto-generate email, please do not reply to this email.";

                MailMessage mail = new MailMessage(SMTP_USERNAME, to, header, message);
                if (cc != null && cc.Length > 0)
                {
                    mail.CC.Add(new MailAddress(cc));
                }

                mail.BodyEncoding = Encoding.UTF8;
                mail.IsBodyHtml = true;

                smtpClient.Credentials = cred;
                smtpClient.Send(mail);
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("EMAIL CANNOT BE SENT");
                return false;
            }
        }
    }
}