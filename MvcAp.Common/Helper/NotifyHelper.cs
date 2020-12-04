using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using NLog;
using MvcAp.Models.ViewModels;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using MvcAp.Models;
using Newtonsoft.Json;

namespace MvcAp.Common.Helper
{
    public static class NotifyHelper
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static bool IsValidEmail(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public static List<string> IsValidEmailList(string emailList)
        {
            List<string> result = new List<string>();
            try
            {
                List<string> elist = emailList.Split(',').ToList();
                foreach (var email in elist)
                {
                    try
                    {
                        MailAddress m = new MailAddress(email);
                        result.Add(email);
                    }
                    catch (FormatException)
                    {
                    }
                }
            }
            catch (Exception e)
            {

            }
            return result;
        }

        public static SystemSetting GetSystemSetting()
        {
            SystemSetting setting=new SystemSetting();
            using (var context = new Entities())
            {
                DbConfig data = context.DbConfig.FirstOrDefault(p => p.KeyName == "System" && p.Type == "Setting");
                if (data != null && !string.IsNullOrEmpty(data.Value))
                {
                    setting = JsonConvert.DeserializeObject<SystemSetting>(data.Value);
                    return setting;
                }
            }
            return null;
        }
        public static void SendMail(SystemSetting setting, string mail, string subject, string body, bool isHtml = false)
        {
            List<string> mailList = new List<string>();

            mailList.Add(mail);

            SendMail(setting, mailList, subject, body, isHtml);
        }

        public static void SendMail(SystemSetting setting, List<string> mailList, string subject, string body, bool isHtml = false, string qrCodePath= "")
        {
            MailMessage msg = new MailMessage();
            msg.To.Add(string.Join(",", mailList.ToArray()));
            string from = setting.SenderEmail;
            string smtpHost = setting.SMTPHost;
            string fromTitle = setting.SenderDisplayName;
            int port = setting.Port;
            LinkedResource emailImage;

            msg.From = new MailAddress(from, fromTitle, Encoding.UTF8);
            //郵件標題   
            msg.Subject = subject;
            //郵件標題編碼    
            msg.SubjectEncoding = Encoding.UTF8;
            //郵件內容              
            msg.Body = body;
            msg.IsBodyHtml = isHtml;
            msg.BodyEncoding = Encoding.UTF8;//郵件內容編碼   

            if (!string.IsNullOrWhiteSpace(qrCodePath))
            {
                Attachment attachment = new Attachment(qrCodePath);
                attachment.Name = System.IO.Path.GetFileName(qrCodePath);
                attachment.ContentDisposition.Inline = true;
                attachment.ContentDisposition.DispositionType = System.Net.Mime.DispositionTypeNames.Inline;
                msg.Attachments.Add(attachment);

                msg.Attachments[0].ContentId = qrCodePath + ".jpg";
                msg.Body += @"<img width=300 height=300  src='cid:" + qrCodePath + ".jpg'>";
            }

            msg.Priority = MailPriority.Normal;
            SmtpClient MySmtp = new SmtpClient(smtpHost, port);

            string ac = setting.SMTPUserName;
            string pw = setting.SMTPUserPassword;
            if (!string.IsNullOrEmpty(ac) && !string.IsNullOrEmpty(pw))
            {
                MySmtp.Credentials = new System.Net.NetworkCredential(ac, pw);
                MySmtp.EnableSsl = true;
            }
            try
            {
                MySmtp.Send(msg);
            }
            catch (Exception e)
            {
                logger.CusotmerLog(e, "UpdateEventSign");
            }                       
        }
        
    }
}
