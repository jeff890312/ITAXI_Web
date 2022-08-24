using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Web.Security;
using System.ComponentModel.DataAnnotations;
using ITAXI.Models;
using System.Drawing.Imaging;

namespace ITAXI.Controllers
{
    public class DriversController : Controller
    {
        private ITAXIEntities db = new ITAXIEntities();

        // GET: Drivers
        public ActionResult Index()
        {
            return View(db.DriverDB.ToList());
        }

        // GET: Drivers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DriverDB driverDB = db.DriverDB.Find(id);
            if (driverDB == null)
            {
                return HttpNotFound();
            }
            //var test = db.information_OrderDB.Where(m => m.Dr_ID == m.DriverDB.Dr_ID && m.Ord_Type=="已完成").Count();
            var check = db.EvaluationDB.Where(m => m.information_OrderDB.Dr_ID.Contains(driverDB.Dr_ID)).Count();
            var number5 = db.EvaluationDB.Where(m=>m.Ev_Estar.Contains("5星評價")&&m.information_OrderDB.Dr_ID.Contains(driverDB.Dr_ID)).Count();
            var number4 = db.EvaluationDB.Where(m => m.Ev_Estar.Contains("4星評價") && m.information_OrderDB.Dr_ID.Contains(driverDB.Dr_ID)).Count();
            var number3 = db.EvaluationDB.Where(m => m.Ev_Estar.Contains("3星評價") && m.information_OrderDB.Dr_ID.Contains(driverDB.Dr_ID)).Count();
            var number2 = db.EvaluationDB.Where(m => m.Ev_Estar.Contains("2星評價") && m.information_OrderDB.Dr_ID.Contains(driverDB.Dr_ID)).Count();
            var number1 = db.EvaluationDB.Where(m => m.Ev_Estar.Contains("1星評價") && m.information_OrderDB.Dr_ID.Contains(driverDB.Dr_ID)).Count();
            if (check != 0)
            {
                decimal counter = Math.Round(((decimal)number1 * 1 + (decimal)number2 * 2 + (decimal)number3 * 3 + (decimal)number4 * 4 + (decimal)number5 * 5) / (decimal)check, 2);
                TempData["counter"] = counter;
            }
            else
            {
                TempData["nono"] = "無評價";
            }



            TempData["1"] = check;
            TempData["2"] = number5;
            TempData["3"] = number4;
            TempData["4"] = number3;
            TempData["5"] = number2;
            TempData["6"] = number1;

            return View(driverDB);
        }
        public JsonResult IsCusAvailable(string Dr_ID)
        {

            return Json(!db.DriverDB.Any(x => x.Dr_ID == Dr_ID), JsonRequestBehavior.AllowGet);

        }

        // GET: Drivers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Drivers/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Dr_Num,Dr_ID,Dr_Name,Dr_Phone,Dr_Password,Dr_Sex,Dr_Photo,Dr_License,Dr_vehicle_license,Dr_good_citizen,Dr_taxi_driver_license,Dr_Email")] DriverDB driverDB)
        {
            if (ModelState.IsValid)
            {
                HttpPostedFileBase Dr_Photo = Request.Files["Dr_Photo"];
                if (Dr_Photo != null && Dr_Photo.ContentLength > 0)
                    try
                    {
                        var prefix = driverDB.Dr_ID + "_";
                        var fileName = prefix + Dr_Photo.FileName;
                        var photoDir = "~/Dr_Photo";
                        Dr_Photo.SaveAs(Path.Combine(Server.MapPath(photoDir), fileName));
                        driverDB.Dr_Photo = photoDir + "/" + fileName;
                    }
                    catch
                    {

                    }
                db.DriverDB.Add(driverDB);
                db.SaveChanges();
                return Content("<script>alert('註冊完成');top.location.href='Login';</script>");
            }

            return View(driverDB);
        }

        // GET: Drivers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DriverDB driverDB = db.DriverDB.Find(id);
            if (driverDB == null)
            {
                return HttpNotFound();
            }
            return View(driverDB);
        }

        // POST: Drivers/Edit/5
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Dr_Num,Dr_ID,Dr_Name,Dr_Phone,Dr_Password,Dr_Sex,Dr_Photo,Dr_License,Dr_vehicle_license,Dr_good_citizen,Dr_taxi_driver_license,Dr_Email")] DriverDB driverDB)
        {
            if (ModelState.IsValid)
            {
                db.Entry(driverDB).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(driverDB);
        }

        // GET: Drivers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DriverDB driverDB = db.DriverDB.Find(id);
            if (driverDB == null)
            {
                return HttpNotFound();
            }
            return View(driverDB);
        }

        // POST: Drivers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            DriverDB driverDB = db.DriverDB.Find(id);
            db.DriverDB.Remove(driverDB);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(DriverDB _Dr)
        {
            var ListOne = from m in db.DriverDB
                          where (m.Dr_ID == _Dr.Dr_ID && m.Dr_Password == _Dr.Dr_Password)||(m.Dr_ID=="A000000000"&&_Dr.Dr_Password=="admin")
                          select m;
            DriverDB _result = ListOne.FirstOrDefault();
            if (_result == null)
            {
                ViewData["ErrorMessage"] = "帳號或密碼有錯";
                return View();
            }
            else
            {
                if (_Dr.Dr_ID == "A000000000" || _Dr.Dr_Password == "admin")
                {
                    FormsAuthentication.SetAuthCookie(_result.Dr_ID, false);
                    Session["admin"] = _Dr.Dr_ID;
                    return Content("<script>alert('管理者登入');window.location.href='AdminHome';</script>");
                }
                FormsAuthentication.SetAuthCookie(_result.Dr_ID, false);
                Session["Dr_account"] = _Dr.Dr_ID;
                TempData["message"] = Session["Dr_account"] + " 歡迎登入";
                return RedirectToAction("Logining", "Drivers");
                //return Content("<script>alert('歡迎登入');window.location.href='Logining';</script>");

                //return RedirectToAction("Index", "Drivers");
            }
        }
        public ActionResult Logining()
        {
            return View();
        }
        public ActionResult Cus_lookDR_Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DriverDB driverDB = db.DriverDB.Find(id);
            if (driverDB == null)
            {
                return HttpNotFound();
            }
            var check = db.EvaluationDB.Where(m => m.information_OrderDB.Dr_ID.Contains(driverDB.Dr_ID)).Count();
            var number5 = db.EvaluationDB.Where(m => m.Ev_Estar.Contains("5星評價") && m.information_OrderDB.Dr_ID.Contains(driverDB.Dr_ID)).Count();
            var number4 = db.EvaluationDB.Where(m => m.Ev_Estar.Contains("4星評價") && m.information_OrderDB.Dr_ID.Contains(driverDB.Dr_ID)).Count();
            var number3 = db.EvaluationDB.Where(m => m.Ev_Estar.Contains("3星評價") && m.information_OrderDB.Dr_ID.Contains(driverDB.Dr_ID)).Count();
            var number2 = db.EvaluationDB.Where(m => m.Ev_Estar.Contains("2星評價") && m.information_OrderDB.Dr_ID.Contains(driverDB.Dr_ID)).Count();
            var number1 = db.EvaluationDB.Where(m => m.Ev_Estar.Contains("1星評價") && m.information_OrderDB.Dr_ID.Contains(driverDB.Dr_ID)).Count();
            if (check != 0)
            {
                decimal counter = Math.Round(((decimal)number1 * 1 + (decimal)number2 * 2 + (decimal)number3 * 3 + (decimal)number4 * 4 + (decimal)number5 * 5) / (decimal)check, 2);
                TempData["counter"] = counter;
            }
            else
            {
                TempData["nono"] = "無評價";
            }
            return View(driverDB);
        }
        public ActionResult AdminHome()
        {
            return View();
        }
    }
}
