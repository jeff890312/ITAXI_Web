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
using System.Net.Http;
using System.Text;
using System.Collections.Specialized;

namespace ITAXI.Controllers
{
    public class CustomersController : Controller
    {
        private ITAXIEntities db = new ITAXIEntities();

        // GET: Customers
        public ActionResult Index()
        {
            return View(db.CustomerDB.ToList());
        }

        // GET: Customers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerDB customerDB = db.CustomerDB.Find(id);
            if (customerDB == null)
            {
                return HttpNotFound();
            }
            return View(customerDB);
        }

        // GET: Customers/Create

        public JsonResult IsCusAvailable(string Cus_ID)
        {

            return Json(!db.CustomerDB.Any(x => x.Cus_ID == Cus_ID), JsonRequestBehavior.AllowGet);

        }
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Cus_ID,Cus_Name,Cus_Phone,Cus_Password,Cus_Sex,Cus_Photo,Cus_Email")] CustomerDB customerDB)
        {

            if (ModelState.IsValid)
            {
                HttpPostedFileBase Cus_Photo = Request.Files["Cus_Photo"];
                if (Cus_Photo != null && Cus_Photo.ContentLength > 0)
                    try
                    {
                        var prefix = customerDB.Cus_ID + "_";
                        var fileName = prefix + Cus_Photo.FileName;
                        var photoDir = "~/Photo";
                        Cus_Photo.SaveAs(Path.Combine(Server.MapPath(photoDir), fileName));
                        customerDB.Cus_Photo = photoDir + "/" + fileName;
                    }
                    catch
                    {

                    }
              
                db.CustomerDB.Add(customerDB);
                db.SaveChanges();

                return Content("<script>alert('註冊完成');top.location.href='Login';</script>");
            }

            return View(customerDB);
        }

        // GET: Customers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerDB customerDB = db.CustomerDB.Find(id);
            if (customerDB == null)
            {
                return HttpNotFound();
            }
            return View(customerDB);
        }

        // POST: Customers/Edit/5
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Cus_ID,Cus_Name,Cus_Phone,Cus_Password,Cus_Sex,Cus_Photo,Cus_Email")] CustomerDB customerDB)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customerDB).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customerDB);
        }

        // GET: Customers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerDB customerDB = db.CustomerDB.Find(id);
            if (customerDB == null)
            {
                return HttpNotFound();
            }
            return View(customerDB);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            CustomerDB customerDB = db.CustomerDB.Find(id);
            db.CustomerDB.Remove(customerDB);
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
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(CustomerDB _Cus)
        {
            var ListOne = from m in db.CustomerDB
                          where (m.Cus_ID == _Cus.Cus_ID && m.Cus_Password == _Cus.Cus_Password) || (m.Cus_ID == "A0000000" && _Cus.Cus_Password == "admin")
                          select m;
            CustomerDB _result = ListOne.FirstOrDefault();
            if (_result == null)
            {
                ViewData["ErrorMessage"] = "帳號或密碼錯誤";
                return View();
            }
            else
            {
                if (_Cus.Cus_ID == "A0000000" || _Cus.Cus_Password == "admin")
                {
                    FormsAuthentication.SetAuthCookie(_result.Cus_ID, false);
                    Session["admin"] = _Cus.Cus_ID;
                    return Content("<script>alert('管理者登入');top.location.href='AdminHome';</script>");
                }

                FormsAuthentication.SetAuthCookie(_result.Cus_ID, false);
                Session["account"] = _Cus.Cus_ID;
                TempData["message"] = Session["account"] + " 歡迎登入";
               
                return RedirectToAction("Logining", "Customers");
               
                //return Content("<script>alert('歡迎登入!');top.location.href='Logining';</script>");
            }
        }
        public ActionResult Logining()
        {
            

            return View();
        }

        //搭車前畫面
        public ActionResult beforehitchhiking(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            information_OrderDB information_OrderDB = db.information_OrderDB.Find(id);
            if (information_OrderDB == null)
            {
                return HttpNotFound();
            }
            if (information_OrderDB.Dr_ID != null)
            {
                string token = "5Oyojoefz84UlyCNKta1aJed2yHUwaVDRU9yAzjdP7r";
                string message = "訂單已成功配對!!!\n" + information_OrderDB.Ord_Type + "-" + information_OrderDB.Ord_Classification
                    + "\n" + "訂單編號:" + information_OrderDB.Order_Num
                    + "\n" + "訂單狀態:" + information_OrderDB.Ord_Status
                    + "\n" + "訂單日期:" + information_OrderDB.Ord_date
                    + "\n" + "司機帳號:" + information_OrderDB.Dr_ID
                    + "\n" + "訂單人數:" + information_OrderDB.Ord_Numpeople
                    + "\n" + "上下車地點:" + information_OrderDB.Boarding_Location + "~" + information_OrderDB.Drop_off_location;
                string url = "https://notify-api.line.me/api/notify";
                string postData = "message=" + WebUtility.HtmlEncode("\r\n" + message);
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                Uri target = new Uri(url);
                WebRequest request = WebRequest.Create(target);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;
                request.Headers.Add("Authorization", "Bearer " + token);

                using (var dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }

                return RedirectToAction("Details", "Order", new { id = information_OrderDB.Order_Num });
            }

            return View(information_OrderDB);

        }
        public ActionResult takeride()
        {
            return View();
        }
        public ActionResult Dr_look_Cus_Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerDB customerDB = db.CustomerDB.Find(id);
            if (customerDB == null)
            {
                return HttpNotFound();
            }
            return View(customerDB);
        }
        public ActionResult AdminHome()
        {
            return View();
        }
    }
}
