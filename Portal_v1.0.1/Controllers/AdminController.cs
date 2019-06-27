using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Portal_v1._0._1.Identity;
using Portal_v1._0._1.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Portal_v1._0._1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private UserManager<PortalUser> userManager;
        private IdentityDataContext db = new IdentityDataContext();
        MailController mc = new MailController();
        ArrayList myList = new ArrayList();
        public Helper.IzinHakkiHesapla myClass = new Helper.IzinHakkiHesapla();
        public AdminController()
        {
            var userStore = new UserStore<PortalUser>(new IdentityDataContext());
            userManager = new UserManager<PortalUser>(userStore);

    }
        // GET: Admin
        public ActionResult Index()
        {
            var users = userManager.Users.Where(i => i.CiktiMi == false).ToList();
            myList.Clear();
            for (int x = 0; x < users.Count; x++)
            {
                var kalan = myClass.toplamIzinHakki(users[x].Id.ToString());
                myList.Add(kalan);
            }
            ViewBag.kalan = myList;

            return View(userManager.Users.Where(i => i.CiktiMi == false));
        }
        public ActionResult KullaniciIzinEkle(IzinModel model)
        {
            ViewBag.izinListesi = IzinTurRepository.IzinTurListesi();
            var users = userManager.Users.Where(i => i.CiktiMi == false)
            .Select(s => new
            {
                Text = s.Name + " " + s.LastName,
                Value = s.Id
            }).ToList();

            ViewBag.UserList3 = new SelectList(users, "Value", "Text");

            if (ModelState.IsValid)
            {
                var yeni = new IzinModel();
                yeni.PortalUserId = model.PortalUserId;
                yeni.IzinTuru = model.IzinTuru;
                yeni.IzinAciklama = model.IzinAciklama;
                yeni.BaslangicTarihi = model.BaslangicTarihi;
                yeni.BitisTarihi = model.BitisTarihi;
                yeni.Onaylandi = true;

                double gunSayi = 0;
                string gun = string.Empty;

                DateTime geciciTarih = model.BaslangicTarihi;
                while (geciciTarih <= model.BitisTarihi) //haftasonlarını geçiyor
                {
                    gun = geciciTarih.ToString("dddd");
                    if (gun != "Cumartesi" && gun != "Pazar")
                    {
                        gunSayi += 1;
                    }
                    geciciTarih = geciciTarih.AddDays(1);
                }
                yeni.IzinGun = gunSayi.ToString();

                var result = db.Izinler.Add(yeni);
                db.SaveChanges();
                ViewData["Success"] = "İzin ekleme işlemi başarılı";
            }
            else
            {
                return View(model);
            }
            return View();
        }

        public ActionResult InactiveUsers()
        {
            return View(userManager.Users.Where(i => i.CiktiMi == true));
        }

        public ActionResult ShowUser(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                TempData["Hata"] = "Gösterilecek Kullanıcı Bulunamadı !";
                return Redirect("/hata/404");
            }

            var user = userManager.Users.FirstOrDefault(i => i.Id == id);
            return View(user);
        }

        public ActionResult UserUpdate(string id)
        {

            if (String.IsNullOrEmpty(id))
            {
                TempData["Hata"] = "Gösterilecek Kullanıcı Bulunamadı !";
                return Redirect("/hata/404");
            }

            var user = userManager.Users.FirstOrDefault(i => i.Id == id);

            return View(user);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PasswordReset(PasswordResetModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = db.Users.FirstOrDefault(i => i.UserName == model.Username);

            userManager.RemovePassword(user.Id);

            var result = userManager.AddPassword(user.Id, model.Password);

            if (result.Succeeded)
            {
                TempData["Success"] = "Şifre Resetlendi";
                return View();
            }

            TempData["Message"] = "Bir hata oluştu !!!";
            return View(model);
        }

        [HttpGet]
        public ActionResult PasswordReset()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserUpdate(PortalUser gelen, HttpPostedFileBase file)
        {

            var user = db.Users.FirstOrDefault(i => i.Id == gelen.Id);
            if (ModelState.IsValid)
            {
                var extension = ".jpg";
                var randomfilename = "default_avatar_male";
                var folder = Server.MapPath("~/uploads/UserPicture");
                var filename = Path.ChangeExtension(randomfilename, extension);

                if (file != null)
                {
                    extension = Path.GetExtension(file.FileName);
                    randomfilename = Path.GetFileNameWithoutExtension(file.FileName) + Path.GetRandomFileName();
                    filename = Path.ChangeExtension(randomfilename, extension);
                    var path = Path.Combine(folder, filename);
                    file.SaveAs(path);
                }
                user.UserName = gelen.UserName;
                user.Name = gelen.Name;
                user.LastName = gelen.LastName;
                user.Title = gelen.Title;
                user.Email = gelen.Email;
                user.PhoneNumber = gelen.PhoneNumber;
                user.Address = gelen.Address;
                user.CiktiMi = gelen.CiktiMi;
                user.IsCikis = gelen.IsCikis ?? "Çıkış yapmadı";
                user.Resim = filename;
                db.SaveChanges();


                TempData["Success"] = "Kullanıcı güncellendi !";
                return RedirectToAction("Index");
            }
            else
            {
                return View(gelen);
            }
        }

        public ActionResult Izinler()
        {

            var izinler = db.Izinler.Select(i => new IzinGelen()
            {
                Id = i.Id,
                Name = i.User.Name,
                BaslangicTarihi = i.BaslangicTarihi,
                BitisTarihi = i.BitisTarihi,
                LastName = i.User.LastName,
                IzinAciklama = i.IzinAciklama,
                IzinGun = i.IzinGun,
                IzinTuru = i.IzinTuru,
                Onaylandi = i.Onaylandi,
            }).OrderBy(i => i.Id);

            return View(izinler);
        }

        public ActionResult IzinDetay(int? id)
        {
            if (String.IsNullOrEmpty(id.ToString()))
            {
                TempData["Hata"] = "Gösterilecek Kullanıcı Bulunamadı !";
                return Redirect("/hata/404");
            }
            var izin = db.Izinler.Where(i => i.Id == id).Select(i => new IzinGelen()
            {
                Id = i.Id,
                Name = i.User.Name,
                BaslangicTarihi = i.BaslangicTarihi,
                BitisTarihi = i.BitisTarihi,
                LastName = i.User.LastName,
                IzinAciklama = i.IzinAciklama,
                IzinGun = i.IzinGun,
                IzinTuru = i.IzinTuru,
                Onaylandi = i.Onaylandi
            }).FirstOrDefault();
            return View(izin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IzinOnay(int? id)
        {
            if (id == null)
            {
                TempData["Hata"] = "Gösterilecek Kullanıcı Bulunamadı !";
                return Redirect("/hata/404");
            }
            var izin = db.Izinler.FirstOrDefault(i => i.Id == id);
            var user = userManager.Users.FirstOrDefault(i => i.Id == izin.PortalUserId);

            var mesaj = String.Format("{0} ile {1} tarihleri arasındaki izin talebiniz onaylanmıştır", izin.BaslangicTarihi, izin.BitisTarihi);
            mc.MailGonderAsync(mesaj, user.Email);

            izin.Onaylandi = true;
            db.SaveChanges();

            TempData["Success"] = "İzin Onaylandı !";

            return RedirectToAction("Izinler");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IzinSil(int? id)
        {
            if (id == null)
            {
                TempData["Hata"] = "Gösterilecek Kullanıcı Bulunamadı !";
                return Redirect("/hata/404");
            }
            var izin = db.Izinler.FirstOrDefault(i => i.Id == id);
            if (izin != null)
            {
                db.Izinler.Remove(izin);
            }
            db.SaveChanges();
            TempData["Success"] = "İzin Silindi !";
            return RedirectToAction("Izinler");
        }
        [HttpGet]
        public ActionResult Raporlar()
        {
            var raporlar = db.Raporlar.Select(i => new RaporGelen()
            {
                Id = i.Id,
                Name = i.User.Name,
                LastName = i.User.LastName,
                BaslangicTarihi = i.BaslangicTarihi,
                BitisTarihi = i.BitisTarihi,
                RaporAciklama = i.RaporAciklama,
                RaporGun = i.RaporGun,
                Resim = i.Resim,
                Onaylandi = i.Onaylandi
            });

            return View(raporlar);
        }

        public ActionResult Bilgilendirmeler()
        {
            var bilgilendirmeler = db.Bilgilendirme.Select(i => new BilgilendirmeGelen()
            {
                Id = i.Id,
                Name = i.User.Name,
                LastName = i.User.LastName,
                Tarih = i.Date,
                BilgilendirmeAciklama = i.Description,
            });

            return View(bilgilendirmeler);
        }

        public ActionResult GenelBilgilendirmeler()
        {
            var bilgilendirmeler = db.BilgilendirmeGenel.Select(i => new BilgilendirmeGelen()
            {
                Id = i.Id,
                Name = i.User.Name,
                LastName = i.User.LastName,
                Tarih = i.Date,
                BilgilendirmeAciklama = i.Description,
            });

            return View(bilgilendirmeler);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BilgilendirmeSil(int? id)
        {
            if (id == null)
            {
                TempData["Hata"] = "Gösterilecek Bilgilendirme Bulunamadı !";
                return Redirect("/hata/404");
            }

            var silinecek = db.BilgilendirmeGenel.FirstOrDefault(i => i.Id == id);
            db.BilgilendirmeGenel.Remove(silinecek);
            db.SaveChanges();

            TempData["Success"] = "Bilgilendirme silindi !";
            return RedirectToAction("GenelBilgilendirmeler");

        }

        public ActionResult RaporDetay(int? id)
        {
            if (String.IsNullOrEmpty(id.ToString()))
            {
                TempData["Hata"] = "Gösterilecek Kullanıcı Bulunamadı !";
                return Redirect("/hata/404");
            }
            var rapor = db.Raporlar.Where(i => i.Id == id).Select(a => new RaporGelen
            {
                Id = a.Id,
                BaslangicTarihi = a.BaslangicTarihi,
                BitisTarihi = a.BitisTarihi,
                LastName = a.User.LastName,
                Name = a.User.Name,
                Onaylandi = a.Onaylandi,
                Resim = a.Resim,
                RaporAciklama = a.RaporAciklama,
                RaporGun = a.RaporGun


            }).FirstOrDefault();

            return View(rapor);
        }

        public ActionResult RaporOnay(int? id)
        {
            if (String.IsNullOrEmpty(id.ToString()))
            {
                TempData["Hata"] = "Gösterilecek Kullanıcı Bulunamadı !";
                return Redirect("/hata/404");
            }
            var rapor = db.Raporlar.FirstOrDefault(i => i.Id == id);
            var user = userManager.Users.FirstOrDefault(i => i.Id == rapor.PortalUserId);

            var mesaj = String.Format("{0} ile {1} tarihleri arasındaki rapor talebiniz onaylanmıştır", rapor.BaslangicTarihi, rapor.BitisTarihi);
            mc.MailGonderAsync(mesaj, user.Email);

            rapor.Onaylandi = true;
            db.SaveChanges();
            TempData["Success"] = "Rapor Onaylandı !";
            return RedirectToAction("Raporlar");
        }

        public ActionResult RaporSil(int? id)
        {
            if (String.IsNullOrEmpty(id.ToString()))
            {
                TempData["Hata"] = "Gösterilecek Kullanıcı Bulunamadı !";
                return Redirect("/hata/404");
            }
            var rapor = db.Raporlar.FirstOrDefault(i => i.Id == id);
            db.Raporlar.Remove(rapor);
            db.SaveChanges();
            TempData["Success"] = "Rapor Silindi !";
            return RedirectToAction("Raporlar");

        }
        public ActionResult Masraflar()
        {
            var sepet = db.MasrafSepetler.Where(y => y.Odendi == false).Select(i => new SepetGelen()
            {
                ToplamTutar = i.ToplamTutar,
                Onaylandi = i.Onaylandi,
                Tedarikci = db.MasrafUrunler.FirstOrDefault(x => x.MasrafSepetId == i.Id).Tedarikci,
                Name = i.User.Name,
                LastName = i.User.LastName,
                Odendi = i.Odendi,
                Aciklama = db.MasrafUrunler.FirstOrDefault(x => x.MasrafSepetId == i.Id).Aciklama,
                SepetId = i.Id
            });
            return View(sepet.ToList());
        }
        public ActionResult MasrafGoruntule(int? id)
        {
            if (String.IsNullOrEmpty(id.ToString()))
            {
                TempData["Hata"] = "Gösterilecek Kullanıcı Bulunamadı !";
                return Redirect("/hata/404");
            }
            var masraflar = db.MasrafUrunler.Where(i => i.MasrafSepetId == id);
            var _sepet = db.MasrafSepetler.FirstOrDefault(i => i.Id == id);
            ViewBag.user = userManager.Users.FirstOrDefault(i => i.Id == _sepet.PortalUserId);
            ViewBag.sepet = _sepet;
            return View(masraflar);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MasrafOnayla(int? id)
        {
            if (id == null)
            {
                TempData["Hata"] = "Gösterilecek Kullanıcı Bulunamadı !";
                return Redirect("/hata/404");
            }
            var sepet = db.MasrafSepetler.FirstOrDefault(i => i.Id == id);
            var user = userManager.Users.FirstOrDefault(i => i.Id == sepet.PortalUserId);

            var mesaj = String.Format("{0} ₺ toplam masrafınız onaylanmıştır ", sepet.ToplamTutar);
            mc.MailGonderAsync(mesaj, user.Email);

            sepet.Onaylandi = true;
            db.SaveChanges();
            TempData["Success"] = "Masraf Onaylandı !";
            return RedirectToAction("Masraflar");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MasrafOde(int? id)
        {
            if (id == null)
            {
                TempData["Hata"] = "Gösterilecek Kullanıcı Bulunamadı !";
                return Redirect("/hata/404");
            }
            var sepet = db.MasrafSepetler.FirstOrDefault(i => i.Id == id);
            var user = userManager.Users.FirstOrDefault(i => i.Id == sepet.PortalUserId);
            if (sepet.Onaylandi == false)
            {
                TempData["Error"] = "Ödemeden önce onaylamanız gerekir !";
            }
            else
            {
                var mesaj = String.Format("{0} ₺ toplam masrafınız ödenmiştir ", sepet.ToplamTutar);
                mc.MailGonderAsync(mesaj, user.Email);

                sepet.Odendi = true;
                db.SaveChanges();
                TempData["Success"] = "Masraf Ödendi !";
            }

            return RedirectToAction("Masraflar");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MasrafSil(int? id)
        {
            if (id == null)
            {
                TempData["Hata"] = "Gösterilecek Kullanıcı Bulunamadı !";
                return Redirect("/hata/404");
            }
            var sepet = db.MasrafSepetler.FirstOrDefault(i => i.Id == id);
            db.MasrafSepetler.Remove(sepet);
            db.SaveChanges();
            TempData["Success"] = "Masraf Silindi !";
            return RedirectToAction("Masraflar");
        }

        public ActionResult MasrafOdenenler()
        {
            var sepet = db.MasrafSepetler.Where(i => i.Odendi == true).Select(i => new SepetGelen()
            {
                ToplamTutar = i.ToplamTutar,
                Onaylandi = i.Onaylandi,
                Tedarikci = db.MasrafUrunler.FirstOrDefault(x => x.MasrafSepetId == i.Id).Tedarikci,
                Name = i.User.Name,
                LastName = i.User.LastName,
                Odendi = i.Odendi,
                Aciklama = db.MasrafUrunler.FirstOrDefault(x => x.MasrafSepetId == i.Id).Aciklama,
                SepetId = i.Id
            });
            return View(sepet);
        }

        public ActionResult CVGoruntule()
        {
            var cvler = db.CVler.Select(i => new CvGelen()
            {
                Id = i.Id,
                FileName = i.FileName,
                FilePath = i.FilePath,
                Name = i.User.Name,
                LastName = i.User.LastName

            }).ToList();
            return View(cvler);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCV(int? id)
        {
            if (id == null)
            {
                TempData["Hata"] = "Gösterilecek Kullanıcı Bulunamadı !";
                return Redirect("/hata/404");
            }

            var silinecek = db.CVler.FirstOrDefault(i => i.Id == id);
            string fullPath = Request.MapPath("~/uploads/UploadCv/" + silinecek.FileName);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }

            db.CVler.Remove(silinecek);
            db.SaveChanges();

            TempData["Success"] = "CV silindi !";
            return RedirectToAction("CVEkle");
        }


    }
}