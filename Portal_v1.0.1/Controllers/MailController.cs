using Portal_v1._0._1.Identity;
using Portal_v1._0._1.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace Portal_v1._0._1.Controllers
{
    public class MailController
    {
        public void MailGonderAsync(string mesaj, string key)
        {
           
            if (!String.IsNullOrEmpty(key))
            {
                
                var message = new MailMessage();
                if (key == "izin")
                {
                    message.To.Add(new MailAddress(ConfigurationManager.AppSettings["PortalIzinMailAdres"]));
                }
                else if (key == "masraf")
                {
                    message.To.Add(new MailAddress(ConfigurationManager.AppSettings["PortalMasrafMailAdres"]));
                }
                else
                {
                    message.To.Add(new MailAddress(key));
                }
                message.From = new MailAddress(ConfigurationManager.AppSettings["GonderenMail"]);  // replace with valid value
                message.Subject = "Portal Mesajı";
                SmtpClient smtpClient = new SmtpClient();
                message.Body = mesaj;
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = ConfigurationManager.AppSettings["GonderenMail"],  
                        Password = ConfigurationManager.AppSettings["GonderenMailSifre"]
                    };
                    smtp.Credentials = credential;
                    smtp.Host = ConfigurationManager.AppSettings["SmtpHost"];  
                    smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);
                    smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["SmtpSsl"]); 
                    smtp.Send(message);
                }
            }
        }
    }
}