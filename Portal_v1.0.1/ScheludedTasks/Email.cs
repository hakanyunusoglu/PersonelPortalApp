using Portal_v1._0._1.Identity;
using Quartz;
using System;
using System.Net;
using System.Net.Mail;
using System.Linq;
using System.Collections.Generic;
using Portal_v1._0._1.Models;
using OfficeOpenXml;
using System.IO;
using System.Configuration;

namespace ScheduledTasks.EmailTest
{
    public class EmailJob : IJob
    {
        IdentityDataContext db = new IdentityDataContext();
        public void Execute(IJobExecutionContext context)
        {
            MemoryStream ms = new MemoryStream();
            using (ExcelPackage package = new ExcelPackage())
            {
                var current_date = DateTime.Now;
                var start_date = current_date.AddDays(-7);
                var bilgiliste = (from f in db.Bilgilendirme.Where(i => i.Date >= start_date && i.Date <= current_date).AsEnumerable()
                select new BilgilendirmeModel()
                                  {
                                      Id = f.Id,
                                      PortalUserId = f.PortalUserId,
                                      Date = f.Date,
                                      Description = f.Description
                                  }).ToList();
                //create the WorkSheet
                ExcelWorksheet ws = package.Workbook.Worksheets.Add("Sheet 1");

                ws.Cells["A1"].Value = "Ad";
                ws.Cells["B1"].Value = "Soyad";
                ws.Cells["C1"].Value = "Tarih";
                ws.Cells["D1"].Value = "Açıklama";
                
                int rowStart = 2;
                foreach (var item in bilgiliste)
                {
                    rowStart += 1;
                    var user = db.Users.FirstOrDefault(i => i.Id == item.PortalUserId);
                    ws.Cells[string.Format("A{0}", rowStart)].Value = user.Name;
                    ws.Cells[string.Format("B{0}", rowStart)].Value = user.LastName;
                    ws.Cells[string.Format("C{0}", rowStart)].Value = string.Format("{0:dd MMMM yyyy}", item.Date);
                    ws.Cells[string.Format("D{0}", rowStart)].Value = item.Description;
                }

                //save the excel to the stream
                ms = new MemoryStream(package.GetAsByteArray());
            }
            using (var message = new MailMessage("kitap65@gmail.com", "kitap66@gmail.com"))
            {
                message.Subject = "Haftalık Bilgilendirme raporu";
                message.Attachments.Add(new Attachment(ms, string.Format("{0} Rapor.xlsx","asd"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));
                message.Body = "Rapor ektedir.";
                using (SmtpClient client = new SmtpClient
                {
                    EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["SmtpSsl"]),
                    Host = ConfigurationManager.AppSettings["SmtpHost"],
                    Port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]),
                    Credentials = new NetworkCredential(ConfigurationManager.AppSettings["GonderenMail"],ConfigurationManager.AppSettings["GonderenMailSifre"])
                })
                {
                    client.Send(message);
                }
            }
        }
    }
}