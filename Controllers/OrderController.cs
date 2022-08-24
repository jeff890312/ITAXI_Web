using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Timers;
using System.Web;
using System.Web.Mvc;
using ITAXI.Models;

namespace ITAXI.Controllers
{
    public class OrderController : Controller
    {
        private ITAXIEntities db = new ITAXIEntities();

        // GET: Order
        public ActionResult Index()
        {
            var information_OrderDB = db.information_OrderDB.Include(i => i.CustomerDB).Include(i => i.DriverDB);
            information_OrderDB = information_OrderDB.OrderByDescending(s => s.Ord_date);
            return View(information_OrderDB.ToList());
        }

        // GET: Order/Details/5
        public ActionResult Details(int? id)
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
            if (information_OrderDB.Boarding_Time != null && information_OrderDB.Drop_off_Time != null)
            {
                TimeSpan BT = information_OrderDB.Boarding_Time;
                TimeSpan DOT = (TimeSpan)information_OrderDB.Drop_off_Time;
                TimeSpan JT = new TimeSpan(DOT.Ticks - BT.Ticks);
                TempData["JT"] = Convert.ToString(JT.TotalMinutes + "分鐘");

            }
            else
            {

            }

            return View(information_OrderDB);
        }

        // GET: Order/Create
        public ActionResult Create()
        {
            ViewBag.Cus_ID = new SelectList(db.CustomerDB, "Cus_ID", "Cus_Name");
            ViewBag.Dr_ID = new SelectList(db.DriverDB, "Dr_ID", "Dr_Name");
            return View();
        }
        // POST: Order/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Order_Num,Dr_ID,Cus_ID,Ord_Status,Ord_Type,Ord_date,Ord_Money,Boarding_Time,Boarding_Location,Drop_off_Time,Drop_off_location,Journey_Time,Ord_Numpeople")] information_OrderDB information_OrderDB)
        {
            if (ModelState.IsValid)
            {
                db.information_OrderDB.Add(information_OrderDB);
                db.SaveChanges();

                return RedirectToAction("beforehitchhiking", "Customers");
            }

            ViewBag.Cus_ID = new SelectList(db.CustomerDB, "Cus_ID", "Cus_Name", information_OrderDB.Cus_ID);
            ViewBag.Dr_ID = new SelectList(db.DriverDB, "Dr_ID", "Dr_Name", information_OrderDB.Dr_ID);
            return View(information_OrderDB);

        }


        // GET: Order/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.Cus_ID = new SelectList(db.CustomerDB, "Cus_ID", "Cus_Name", information_OrderDB.Cus_ID);
            ViewBag.Dr_ID = new SelectList(db.DriverDB, "Dr_ID", "Dr_Name", information_OrderDB.Dr_ID);
            return View(information_OrderDB);
        }

        // POST: Order/Edit/5
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Order_Num,Dr_ID,Cus_ID,Ord_Status,Ord_Type,Ord_date,Ord_Money,Boarding_Time,Boarding_Location,Drop_off_Time,Drop_off_location,Journey_Time,Ord_Numpeople")] information_OrderDB information_OrderDB)
        {
            if (ModelState.IsValid)
            {
                db.Entry(information_OrderDB).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Cus_ID = new SelectList(db.CustomerDB, "Cus_ID", "Cus_Name", information_OrderDB.Cus_ID);
            ViewBag.Dr_ID = new SelectList(db.DriverDB, "Dr_ID", "Dr_Name", information_OrderDB.Dr_ID);
            return View(information_OrderDB);
        }

        // GET: Order/Delete/5
        public ActionResult Delete(int? id)
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
            return View(information_OrderDB);
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            information_OrderDB information_OrderDB = db.information_OrderDB.Find(id);
            db.information_OrderDB.Remove(information_OrderDB);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult OrderBack(int id)
        {
            information_OrderDB information_OrderDB = db.information_OrderDB.Find(id);
            db.information_OrderDB.Remove(information_OrderDB);
            db.SaveChanges();
            return RedirectToAction("Logining", "Customers");
        }

        //預約訂單
        public ActionResult reservation(information_OrderDB dr_id)
        {
            var reservationOrder = from s in db.information_OrderDB
                                   select s;
            reservationOrder = reservationOrder.Where(s => s.Ord_Status.Contains("配對中") &&
                                s.Ord_Type.Contains("預約訂單") && s.Ord_Classification.Contains("汽車"));

            var Order = reservationOrder.Count();
            var pc1 = db.information_OrderDB.Where(m => m.Ord_Status.Contains("配對中") && m.Ord_Type.Contains("預約訂單") && m.Ord_Classification.Contains("汽車") && m.Ord_Numpeople.Contains("1人")).Count();
            var pc2 = db.information_OrderDB.Where(m => m.Ord_Status.Contains("配對中") && m.Ord_Type.Contains("預約訂單") && m.Ord_Classification.Contains("汽車") && m.Ord_Numpeople.Contains("2人")).Count();
            var pc3 = db.information_OrderDB.Where(m => m.Ord_Status.Contains("配對中") && m.Ord_Type.Contains("預約訂單") && m.Ord_Classification.Contains("汽車") && m.Ord_Numpeople.Contains("3人")).Count();
            var pc4 = db.information_OrderDB.Where(m => m.Ord_Status.Contains("配對中") && m.Ord_Type.Contains("預約訂單") && m.Ord_Classification.Contains("汽車") && m.Ord_Numpeople.Contains("4人")).Count();
            var pc5 = db.information_OrderDB.Where(m => m.Ord_Status.Contains("配對中") && m.Ord_Type.Contains("預約訂單") && m.Ord_Classification.Contains("汽車") && m.Ord_Numpeople.Contains("5人")).Count();
            if (Order != 0)
            {
                decimal Money1 = Math.Round(((decimal)pc1 * 70), 2);
                decimal Money2 = Math.Round(((decimal)pc2 * 140), 2);
                decimal Money3 = Math.Round(((decimal)pc3 * 210), 2);
                decimal Money4 = Math.Round(((decimal)pc4 * 280), 2);
                decimal Money5 = Math.Round(((decimal)pc5 * 350), 2);
                TempData["Money1"] = Money1;
                TempData["Money2"] = Money2;
                TempData["Money3"] = Money3;
                TempData["Money4"] = Money4;
                TempData["Money5"] = Money5;
            }
            else
            {
                TempData["nono"] = "無訂單";
            }
            reservationOrder = reservationOrder.OrderByDescending(s => s.Ord_date);
            return View(reservationOrder.ToList());
        }
        public ActionResult reservation_Moto(information_OrderDB dr_id)
        {
            var reservationOrder = from s in db.information_OrderDB
                                   select s;
            reservationOrder = reservationOrder.Where(s => s.Ord_Status.Contains("配對中") &&
                                s.Ord_Type.Contains("預約訂單") && s.Ord_Classification.Contains("摩托車"));
            reservationOrder = reservationOrder.OrderByDescending(s => s.Ord_date);
            return View(reservationOrder.ToList());
        }

        //立即訂單
        public ActionResult immediately()
        {
            var immediatelyOrder = from s in db.information_OrderDB
                                   select s;
            TimeSpan now = DateTime.Now.TimeOfDay;
            TimeSpan now30 = DateTime.Now.AddMinutes(-30).TimeOfDay;
            immediatelyOrder = immediatelyOrder.Where(s => s.Ord_Status.Contains("配對中") &&
                                s.Ord_Type.Contains("立即訂單") && s.Ord_Classification.Contains("汽車") && s.Boarding_Time <= now && s.Boarding_Time > now30);
            immediatelyOrder = immediatelyOrder.OrderByDescending(s => s.Ord_date);

            var Order = immediatelyOrder.Count();
            var pc1 = db.information_OrderDB.Where(m => m.Ord_Status.Contains("配對中") && m.Ord_Type.Contains("立即訂單") && m.Ord_Classification.Contains("汽車") && m.Ord_Numpeople.Contains("1人")).Count();
            var pc2 = db.information_OrderDB.Where(m => m.Ord_Status.Contains("配對中") && m.Ord_Type.Contains("立即訂單") && m.Ord_Classification.Contains("汽車") && m.Ord_Numpeople.Contains("2人")).Count();
            var pc3 = db.information_OrderDB.Where(m => m.Ord_Status.Contains("配對中") && m.Ord_Type.Contains("立即訂單") && m.Ord_Classification.Contains("汽車") && m.Ord_Numpeople.Contains("3人")).Count();
            var pc4 = db.information_OrderDB.Where(m => m.Ord_Status.Contains("配對中") && m.Ord_Type.Contains("立即訂單") && m.Ord_Classification.Contains("汽車") && m.Ord_Numpeople.Contains("4人")).Count();
            var pc5 = db.information_OrderDB.Where(m => m.Ord_Status.Contains("配對中") && m.Ord_Type.Contains("立即訂單") && m.Ord_Classification.Contains("汽車") && m.Ord_Numpeople.Contains("5人")).Count();
            if (Order != 0)
            {
                decimal Money1 = Math.Round(((decimal)pc1 * 70), 2);
                decimal Money2 = Math.Round(((decimal)pc2 * 140), 2);
                decimal Money3 = Math.Round(((decimal)pc3 * 210), 2);
                decimal Money4 = Math.Round(((decimal)pc4 * 280), 2);
                decimal Money5 = Math.Round(((decimal)pc5 * 350), 2);
                TempData["Money1"] = Money1;
                TempData["Money2"] = Money2;
                TempData["Money3"] = Money3;
                TempData["Money4"] = Money4;
                TempData["Money5"] = Money5;
            }
            else
            {
                TempData["nono"] = "無訂單";
            }

            string Dr_account = Session["Dr_account"].ToString();
            var count = db.information_OrderDB.Where(s => s.Dr_ID.Contains(Dr_account) && s.Ord_Status.Contains("載客中")).Count();
            if (count >= 5)
            {
                TempData["message"] = Session["Dr_account"] + "接單已達上限，5單為上限\n目前已接" + count + "單";
                return RedirectToAction("Logining", "Drivers");
            }
            return View(immediatelyOrder.ToList());
        }
        public ActionResult immediately_Moto()
        {
            var immediatelyOrder = from s in db.information_OrderDB
                                   select s;
            TimeSpan now = DateTime.Now.TimeOfDay;
            TimeSpan now30 = DateTime.Now.AddMinutes(-30).TimeOfDay;
            immediatelyOrder = immediatelyOrder.Where(s => s.Ord_Status.Contains("配對中") &&
                                s.Ord_Type.Contains("立即訂單") && s.Ord_Classification.Contains("摩托車") && s.Boarding_Time <= now && s.Boarding_Time > now30);
            immediatelyOrder = immediatelyOrder.OrderByDescending(s => s.Ord_date);
            string Dr_account = Session["Dr_account"].ToString();
            var count = db.information_OrderDB.Where(s => s.Dr_ID.Contains(Dr_account) && s.Ord_Status.Contains("載客中")).Count();
            if (count >= 5)
            {
                TempData["message"] = Session["Dr_account"] + "接單已達上限，5單為上限\n目前已接" + count + "單";
                return RedirectToAction("Logining", "Drivers");
            }
            return View(immediatelyOrder.ToList());
        }

        //接收預約訂單
        public ActionResult acceptreservation(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            information_OrderDB information_Order = db.information_OrderDB.Find(id);
            if (information_Order == null)
            {
                return HttpNotFound();
            }
            //ViewBag.Dr_ID = new SelectList(db.Driver, "Dr_ID","Dr_Num", information_Order.Dr_Num);
            ViewBag.Cus_ID = new SelectList(db.CustomerDB, "Cus_ID", "Cus_Name", information_Order.Cus_ID);
            return View(information_Order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult acceptreservation([Bind(Include = "Order_Num,Dr_ID,Cus_ID,Ord_Status,Ord_Type,Ord_date,Ord_Money,Boarding_Time,Boarding_Location,drop_off_Time,drop_off_location,Journey_Time,Ord_Numpeople,Ord_Classification")] information_OrderDB information_Order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(information_Order).State = EntityState.Modified;
                db.SaveChanges();

                if (information_Order.Dr_ID == "A123456789")
                {
                    string token = "s1KTJSJ5coT4WZTzpSjoB1v8j3M82QG5bwyD6fw0";
                    string message = "已成功接收訂單!!!\n" + information_Order.Ord_Type + "-" + information_Order.Ord_Classification
                        + "\n" + "訂單編號:" + information_Order.Order_Num
                        + "\n" + "訂單狀態:" + information_Order.Ord_Status
                        + "\n" + "訂單日期:" + information_Order.Ord_date
                        + "\n" + "乘客姓名:" + information_Order.Cus_ID
                        + "\n" + "訂單人數:" + information_Order.Ord_Numpeople
                        + "\n" + "訂單金額:(每人)" + information_Order.Ord_Money
                        + "\n" + "上下車地點:" + information_Order.Boarding_Location + "~" + information_Order.Drop_off_location;
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
                }
                else
                {

                }

                return RedirectToAction("Index");
            }
            //ViewBag.Dr_ID = new SelectList(db.Driver, "Dr_Num", "Dr_ID", information_Order.Dr_Num);
            ViewBag.Cus_ID = new SelectList(db.CustomerDB, "Cus_ID", "Cus_Name", information_Order.Cus_ID);
            return View(information_Order);
        }

        public ActionResult acceptimmediately(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            information_OrderDB information_Order = db.information_OrderDB.Find(id);
            if (information_Order == null)
            {
                return HttpNotFound();
            }
            //ViewBag.Dr_ID = new SelectList(db.Driver, "Dr_ID","Dr_Num", information_Order.Dr_Num);
            ViewBag.Cus_ID = new SelectList(db.CustomerDB, "Cus_ID", "Cus_Name", information_Order.Cus_ID);
            return View(information_Order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //接收立即訂單
        public ActionResult acceptimmediately([Bind(Include = "Order_Num,Dr_ID,Cus_ID,Ord_Status,Ord_Type,Ord_date,Ord_Money,Boarding_Time,Boarding_Location,drop_off_Time,drop_off_location,Journey_Time,Ord_Numpeople,Ord_Classification")] information_OrderDB information_Order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(information_Order).State = EntityState.Modified;
                db.SaveChanges();

                if (information_Order.Dr_ID == "A123456789")
                {
                    string token = "s1KTJSJ5coT4WZTzpSjoB1v8j3M82QG5bwyD6fw0";
                    string message = "已成功接收訂單!!!\n" + information_Order.Ord_Type + "-" + information_Order.Ord_Classification
                        + "\n" + "訂單編號:" + information_Order.Order_Num
                        + "\n" + "訂單狀態:" + information_Order.Ord_Status
                        + "\n" + "訂單日期:" + information_Order.Ord_date
                        + "\n" + "乘客姓名:" + information_Order.Cus_ID
                        + "\n" + "訂單人數:" + information_Order.Ord_Numpeople
                        + "\n" + "訂單金額:(每人)" + information_Order.Ord_Money
                        + "\n" + "上下車地點:" + information_Order.Boarding_Location + "~" + information_Order.Drop_off_location;
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
                }
                else { }
                return RedirectToAction("Logining", "Drivers");
            }
            //ViewBag.Dr_ID = new SelectList(db.Driver, "Dr_Num", "Dr_ID", information_Order.Dr_Num);
            ViewBag.Cus_ID = new SelectList(db.CustomerDB, "Cus_ID", "Cus_Name", information_Order.Cus_ID);
            return View(information_Order);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        //public ActionResult Create_immediately()
        //{
        //    TimeSpan now = DateTime.Now.TimeOfDay;
        //    TimeSpan now30 = DateTime.Now.AddMinutes(-30).TimeOfDay;
        //    ViewBag.Cus_ID = new SelectList(db.CustomerDB, "Cus_ID", "Cus_Name");
        //    ViewBag.Dr_ID = new SelectList(db.DriverDB, "Dr_ID", "Dr_Name");
        //    if (Session["account"] == null)
        //    {
        //        return RedirectToAction("Login", "Customers");
        //    }
        //    else
        //    {
        //        var immediatelyOrder = from s in db.information_OrderDB
        //                               select s;
        //        string account = Session["account"].ToString();
        //        var count = db.information_OrderDB.Where(s => s.Cus_ID.Contains(account) && (s.Ord_Status.Contains("配對中") && s.Boarding_Time >= now && s.Boarding_Time < now30 || s.Ord_Status.Contains("載客中"))).Count();
        //        if (count > 0)
        //        {
        //            return Content("<script>alert('您已建立立即訂單，請去搭車紀錄查詢!!!');top.location.href='HitchhikingRecord';</script>");
        //        }
        //        else
        //        {
        //            return View();
        //        }
        //    }
        //}

        //// POST: Order/Create
        //// 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        //// 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create_immediately([Bind(Include = "Order_Num,Dr_ID,Cus_ID,Ord_Status,Ord_Type,Ord_date,Ord_Money,Boarding_Time,Boarding_Location,Drop_off_Time,Drop_off_location,Journey_Time,Ord_Numpeople")] information_OrderDB information_OrderDB)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.information_OrderDB.Add(information_OrderDB);
        //        db.SaveChanges();
        //        return RedirectToAction("beforehitchhiking", "Customers", new { id = information_OrderDB.Order_Num });
        //    }

        //    ViewBag.Cus_ID = new SelectList(db.CustomerDB, "Cus_ID", "Cus_Name", information_OrderDB.Cus_ID);
        //    ViewBag.Dr_ID = new SelectList(db.DriverDB, "Dr_ID", "Dr_Name", information_OrderDB.Dr_ID);
        //    return View(information_OrderDB);
        //}
        //public ActionResult Create_reservation()
        //{
        //    ViewBag.Cus_ID = new SelectList(db.CustomerDB, "Cus_ID", "Cus_Name");
        //    ViewBag.Dr_ID = new SelectList(db.DriverDB, "Dr_ID", "Dr_Name");
        //    return View();
        //}

        //// POST: Order/Create
        //// 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        //// 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create_reservation([Bind(Include = "Order_Num,Dr_ID,Cus_ID,Ord_Status,Ord_Type,Ord_date,Ord_Money,Boarding_Time,Boarding_Location,Drop_off_Time,Drop_off_location,Journey_Time,Ord_Numpeople")] information_OrderDB information_OrderDB)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.information_OrderDB.Add(information_OrderDB);
        //        db.SaveChanges();
        //        return RedirectToAction("beforehitchhiking", "Customers", new { id = information_OrderDB.Order_Num });
        //    }

        //    ViewBag.Cus_ID = new SelectList(db.CustomerDB, "Cus_ID", "Cus_Name", information_OrderDB.Cus_ID);
        //    ViewBag.Dr_ID = new SelectList(db.DriverDB, "Dr_ID", "Dr_Name", information_OrderDB.Dr_ID);
        //    return View(information_OrderDB);
        //}
        //搭車紀錄
        public ActionResult HitchhikingRecord()
        {
            var HitchhikingRecordOrder = from s in db.information_OrderDB
                                         select s;
            string Cus_account = Session["account"].ToString();
            HitchhikingRecordOrder = HitchhikingRecordOrder.Where(s => s.Cus_ID.Contains(Cus_account));
            HitchhikingRecordOrder = HitchhikingRecordOrder.OrderByDescending(s => s.Ord_date).ThenByDescending(s => s.Ord_Status.Contains("載客中")).ThenByDescending(s => s.Ord_Status.Contains("配對中")).ThenByDescending(s => s.Ord_Status.Contains("已完成")).ThenByDescending(s => s.Ord_Status.Contains("已取消"));
            return View(HitchhikingRecordOrder.ToList());
        }
        //載客紀錄
        public ActionResult PassengerRecord()
        {
            var PassengerRecordOrder = from s in db.information_OrderDB
                                       select s;
            string Dr_account = Session["Dr_account"].ToString();
            PassengerRecordOrder = PassengerRecordOrder.Where(s => s.Dr_ID.Contains(Dr_account));
            PassengerRecordOrder = PassengerRecordOrder.OrderByDescending(s => s.Ord_date).ThenByDescending(s => s.Ord_Status.Contains("載客中")).ThenByDescending(s => s.Ord_Status.Contains("配對中")).ThenByDescending(s => s.Ord_Status.Contains("已完成")).ThenByDescending(s => s.Ord_Status.Contains("已取消"));
            return View(PassengerRecordOrder.ToList());
        }
        public ActionResult Earn_day()
        {
            return View();
        }
        public ActionResult Earnings_oneday(string FromDate, string ToDate)
        {
            var driverAccount = Session["Dr_account"].ToString();

            var result = from o in db.information_OrderDB
                         where o.Dr_ID.Contains(driverAccount) && String.Compare(o.Ord_date, FromDate) > -1 && String.Compare(o.Ord_date, ToDate) < 1
                         select o;

            var Order = result.Count();
            var pc1 = db.information_OrderDB.Where(m => m.Dr_ID.Contains(driverAccount) && String.Compare(m.Ord_date, FromDate) > -1 && String.Compare(m.Ord_date, ToDate) < 1 && m.Ord_Numpeople.Contains("1人") && m.Ord_Classification.Contains("汽車")).Count();
            var pc2 = db.information_OrderDB.Where(m => m.Dr_ID.Contains(driverAccount) && String.Compare(m.Ord_date, FromDate) > -1 && String.Compare(m.Ord_date, ToDate) < 1 && m.Ord_Numpeople.Contains("2人") && m.Ord_Classification.Contains("汽車")).Count();
            var pc3 = db.information_OrderDB.Where(m => m.Dr_ID.Contains(driverAccount) && String.Compare(m.Ord_date, FromDate) > -1 && String.Compare(m.Ord_date, ToDate) < 1 && m.Ord_Numpeople.Contains("3人") && m.Ord_Classification.Contains("汽車")).Count();
            var pc4 = db.information_OrderDB.Where(m => m.Dr_ID.Contains(driverAccount) && String.Compare(m.Ord_date, FromDate) > -1 && String.Compare(m.Ord_date, ToDate) < 1 && m.Ord_Numpeople.Contains("4人") && m.Ord_Classification.Contains("汽車")).Count();
            var pc5 = db.information_OrderDB.Where(m => m.Dr_ID.Contains(driverAccount) && String.Compare(m.Ord_date, FromDate) > -1 && String.Compare(m.Ord_date, ToDate) < 1 && m.Ord_Numpeople.Contains("5人") && m.Ord_Classification.Contains("汽車")).Count();
            var pm1 = db.information_OrderDB.Where(m => m.Dr_ID.Contains(driverAccount) && String.Compare(m.Ord_date, FromDate) > -1 && String.Compare(m.Ord_date, ToDate) < 1 && m.Ord_Numpeople.Contains("1人") && m.Ord_Classification.Contains("摩托車")).Count();

            if (Order != 0)
            {
                TempData["Orderall"] = Order;
                decimal Money = Math.Round(((decimal)pc1 * 70 + (decimal)pc2 * 140 + (decimal)pc3 * 210 + (decimal)pc4 * 280 + (decimal)pc5 * 350 + (decimal)pm1 * 30), 2);
                TempData["Moneyall"] = Money;
            }
            else
            {
                TempData["nono"] = "無接單";
            }

            TempData["Start"] = FromDate;
            TempData["End"] = ToDate;
            ViewBag.FomDate = FromDate;
            ViewBag.ToDate = ToDate;
            return View(result);
        }

        public ActionResult Cancelorder(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            information_OrderDB information_Order = db.information_OrderDB.Find(id);
            if (information_Order == null)
            {
                return HttpNotFound();
            }
            ViewBag.Cus_ID = new SelectList(db.CustomerDB, "Cus_ID", "Cus_Name", information_Order.Cus_ID);
            return View(information_Order);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cancelorder([Bind(Include = "Order_Num,Dr_Num,Cus_ID,Ord_Status,Ord_Type,Ord_date,Ord_Money,Boarding_Time,Boarding_Location,drop_off_Time,drop_off_location,Journey_Time,Ord_Numpeople")] information_OrderDB information_Order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(information_Order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Logining", "Customers");
            }
            ViewBag.Cus_ID = new SelectList(db.CustomerDB, "Cus_ID", "Cus_Name", information_Order.Cus_ID);
            return View(information_Order);
        }
        public ActionResult Cus_OrderDown(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            information_OrderDB information_Order = db.information_OrderDB.Find(id);
            if (information_Order == null)
            {
                return HttpNotFound();
            }
            ViewBag.Cus_ID = new SelectList(db.CustomerDB, "Cus_ID", "Cus_Name", information_Order.Cus_ID);
            return View(information_Order);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cus_OrderDown([Bind(Include = "Order_Num,Dr_ID,Cus_ID,Ord_Status,Ord_Type,Ord_date,Ord_Money,Boarding_Time,Boarding_Location,drop_off_Time,drop_off_location,Journey_Time,Ord_Numpeople,Ord_Classification")] information_OrderDB information_Order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(information_Order).State = EntityState.Modified;
                db.SaveChanges();

                if (information_Order.Dr_ID == "A123456789")
                {
                    string token = "s1KTJSJ5coT4WZTzpSjoB1v8j3M82QG5bwyD6fw0";
                    string message = "訂單已完成!!!\n" + information_Order.Ord_Type + "-" + information_Order.Ord_Classification
                        + "\n" + "訂單編號:" + information_Order.Order_Num
                        + "\n" + "訂單狀態:" + information_Order.Ord_Status
                        + "\n" + "訂單日期:" + information_Order.Ord_date
                        + "\n" + "乘客姓名:" + information_Order.Cus_ID
                        + "\n" + "訂單人數:" + information_Order.Ord_Numpeople
                        + "\n" + "訂單金額:(每人)" + information_Order.Ord_Money
                        + "\n" + "請記得提醒乘客的隨身物品及付款!!!辛苦了~";
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
                }
                else { }

                Session["ord"] = information_Order.Order_Num;
                return RedirectToAction("Create", "Evaluations");
            }
            ViewBag.Cus_ID = new SelectList(db.CustomerDB, "Cus_ID", "Cus_Name", information_Order.Cus_ID);
            return View(information_Order);
        }
        public ActionResult Search()
        {
            return View();
        }
        public ActionResult Search_Evaluations(string Evano)
        {
            var search_eva = from s in db.EvaluationDB
                             select s;
            var eva = from s in db.information_OrderDB
                      select s;
            DriverDB driverDB = db.DriverDB.Find(Evano);
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
            TempData["ID"] = driverDB.Dr_ID;
            TempData["Name"] = driverDB.Dr_Name;

            TempData["1"] = check;
            TempData["2"] = number5;
            TempData["3"] = number4;
            TempData["4"] = number3;
            TempData["5"] = number2;
            TempData["6"] = number1;
            //ViewBag.Evano = Evano;
            //search_eva = search_eva.Where(s => s.Ev_Num.ToString().Contains(Evano));
            ViewBag.Evano = Evano;
            search_eva = search_eva.Where(s => s.information_OrderDB.Dr_ID.ToString().Contains(Evano));
            search_eva = search_eva.OrderByDescending(s => s.information_OrderDB.Ord_date);
            return View(search_eva.ToList());
        }
        //======================================================================================================================
        public ActionResult Create_immediately_Car()
        {
            TimeSpan now = DateTime.Now.TimeOfDay;
            TimeSpan now30 = DateTime.Now.AddMinutes(-30).TimeOfDay;
            ViewBag.Cus_ID = new SelectList(db.CustomerDB, "Cus_ID", "Cus_Name");
            ViewBag.Dr_ID = new SelectList(db.DriverDB, "Dr_ID", "Dr_Name");
            var immediatelyOrder = from s in db.information_OrderDB
                                   select s;
            string account = Session["account"].ToString();
            var count = db.information_OrderDB.Where(s => s.Cus_ID.Contains(account) && (s.Ord_Status.Contains("配對中") && s.Boarding_Time <= now && s.Boarding_Time > now30 || s.Ord_Status.Contains("載客中"))).Count();
            if (count > 0)
            {
                return Content("<script>alert('您已建立立即訂單，請去搭車紀錄查詢!!!');top.location.href='HitchhikingRecord';</script>");
            }
            else
            {
                return View();
            }
        }

        // POST: Order/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create_immediately_Car([Bind(Include = "Order_Num,Dr_ID,Cus_ID,Ord_Status,Ord_Type,Ord_date,Ord_Money,Boarding_Time,Boarding_Location,Drop_off_Time,Drop_off_location,Journey_Time,Ord_Numpeople,Ord_Classification")] information_OrderDB information_OrderDB)
        {
            if (ModelState.IsValid)
            {
                db.information_OrderDB.Add(information_OrderDB);
                db.SaveChanges();

                if (information_OrderDB.Cus_ID == "A9999999")
                {
                    string token = "5Oyojoefz84UlyCNKta1aJed2yHUwaVDRU9yAzjdP7r";
                    string message = "訂單已成功建立!!!\n" + information_OrderDB.Ord_Type + "-" + information_OrderDB.Ord_Classification
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
                }
                else
                {

                }
                return RedirectToAction("beforehitchhiking", "Customers", new { id = information_OrderDB.Order_Num });
            }

            ViewBag.Cus_ID = new SelectList(db.CustomerDB, "Cus_ID", "Cus_Name", information_OrderDB.Cus_ID);
            ViewBag.Dr_ID = new SelectList(db.DriverDB, "Dr_ID", "Dr_Name", information_OrderDB.Dr_ID);
            return View(information_OrderDB);
        }
        //=====================================================================================================================================
        public ActionResult Create_immediately_Moto()
        {
            TimeSpan now = DateTime.Now.TimeOfDay;
            TimeSpan now30 = DateTime.Now.AddMinutes(-30).TimeOfDay;
            ViewBag.Cus_ID = new SelectList(db.CustomerDB, "Cus_ID", "Cus_Name");
            ViewBag.Dr_ID = new SelectList(db.DriverDB, "Dr_ID", "Dr_Name");
            var immediatelyOrder = from s in db.information_OrderDB
                                   select s;
            string account = Session["account"].ToString();
            var count = db.information_OrderDB.Where(s => s.Cus_ID.Contains(account) && (s.Ord_Status.Contains("配對中") && s.Boarding_Time <= now && s.Boarding_Time > now30 || s.Ord_Status.Contains("載客中"))).Count();
            if (count > 0)
            {
                return Content("<script>alert('您已建立立即訂單，請去搭車紀錄查詢!!!');top.location.href='HitchhikingRecord';</script>");
            }
            else
            {
                return View();
            }
        }

        // POST: Order/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create_immediately_Moto([Bind(Include = "Order_Num,Dr_ID,Cus_ID,Ord_Status,Ord_Type,Ord_date,Ord_Money,Boarding_Time,Boarding_Location,Drop_off_Time,Drop_off_location,Journey_Time,Ord_Numpeople,Ord_Classification")] information_OrderDB information_OrderDB)
        {
            if (ModelState.IsValid)
            {
                db.information_OrderDB.Add(information_OrderDB);
                db.SaveChanges();

                if (information_OrderDB.Cus_ID == "A9999999")
                {
                    string token = "5Oyojoefz84UlyCNKta1aJed2yHUwaVDRU9yAzjdP7r";
                    string message = "訂單已成功建立!!!\n" + information_OrderDB.Ord_Type + "-" + information_OrderDB.Ord_Classification
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
                }
                else
                {
                    
                }

                return RedirectToAction("beforehitchhiking", "Customers", new { id = information_OrderDB.Order_Num });
            }

            ViewBag.Cus_ID = new SelectList(db.CustomerDB, "Cus_ID", "Cus_Name", information_OrderDB.Cus_ID);
            ViewBag.Dr_ID = new SelectList(db.DriverDB, "Dr_ID", "Dr_Name", information_OrderDB.Dr_ID);
            return View(information_OrderDB);
        }
        //=========================================================================================================================================
        public ActionResult Create_reservation_Car()
        {
            ViewBag.Cus_ID = new SelectList(db.CustomerDB, "Cus_ID", "Cus_Name");
            ViewBag.Dr_ID = new SelectList(db.DriverDB, "Dr_ID", "Dr_Name");
            return View();
        }

        // POST: Order/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create_reservation_Car([Bind(Include = "Order_Num,Dr_ID,Cus_ID,Ord_Status,Ord_Type,Ord_date,Ord_Money,Boarding_Time,Boarding_Location,Drop_off_Time,Drop_off_location,Journey_Time,Ord_Numpeople,Ord_Classification")] information_OrderDB information_OrderDB)
        {
            if (ModelState.IsValid)
            {
                db.information_OrderDB.Add(information_OrderDB);
                db.SaveChanges();

                if (information_OrderDB.Cus_ID == "A9999999")
                {
                    string token = "5Oyojoefz84UlyCNKta1aJed2yHUwaVDRU9yAzjdP7r";
                    string message = "訂單已成功建立!!!\n" + information_OrderDB.Ord_Type + "-" + information_OrderDB.Ord_Classification
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
                }
                else
                {

                }

                return RedirectToAction("beforehitchhiking", "Customers", new { id = information_OrderDB.Order_Num });
            }

            ViewBag.Cus_ID = new SelectList(db.CustomerDB, "Cus_ID", "Cus_Name", information_OrderDB.Cus_ID);
            ViewBag.Dr_ID = new SelectList(db.DriverDB, "Dr_ID", "Dr_Name", information_OrderDB.Dr_ID);
            return View(information_OrderDB);
        }
        public ActionResult Create_reservation_Moto()
        {
            ViewBag.Cus_ID = new SelectList(db.CustomerDB, "Cus_ID", "Cus_Name");
            ViewBag.Dr_ID = new SelectList(db.DriverDB, "Dr_ID", "Dr_Name");
            return View();
        }

        // POST: Order/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create_reservation_Moto([Bind(Include = "Order_Num,Dr_ID,Cus_ID,Ord_Status,Ord_Type,Ord_date,Ord_Money,Boarding_Time,Boarding_Location,Drop_off_Time,Drop_off_location,Journey_Time,Ord_Numpeople,Ord_Classification")] information_OrderDB information_OrderDB)
        {
            if (ModelState.IsValid)
            {
                db.information_OrderDB.Add(information_OrderDB);
                db.SaveChanges();

                if (information_OrderDB.Cus_ID == "A9999999")
                {
                    string token = "5Oyojoefz84UlyCNKta1aJed2yHUwaVDRU9yAzjdP7r";
                    string message = "訂單已成功建立!!!\n" + information_OrderDB.Ord_Type + "-" + information_OrderDB.Ord_Classification
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
                }
                else
                {

                }

                return RedirectToAction("beforehitchhiking", "Customers", new { id = information_OrderDB.Order_Num });
            }

            ViewBag.Cus_ID = new SelectList(db.CustomerDB, "Cus_ID", "Cus_Name", information_OrderDB.Cus_ID);
            ViewBag.Dr_ID = new SelectList(db.DriverDB, "Dr_ID", "Dr_Name", information_OrderDB.Dr_ID);
            return View(information_OrderDB);
        }
    }
}
