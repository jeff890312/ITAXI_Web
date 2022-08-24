using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ITAXI.Models;

namespace ITAXI.Controllers
{
    public class EvaluationsController : Controller
    {
        private ITAXIEntities db = new ITAXIEntities();

        // GET: Evaluations
        public ActionResult Index()
        {
            var evaluationDB = db.EvaluationDB.Include(e => e.information_OrderDB);
            return View(evaluationDB.ToList());
        }

        // GET: Evaluations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvaluationDB evaluationDB = db.EvaluationDB.Find(id);
            if (evaluationDB == null)
            {
                return HttpNotFound();
            }
            return View(evaluationDB);
        }

        // GET: Evaluations/Create
        public ActionResult Create()
        {
            ViewBag.Order_Num = new SelectList(db.information_OrderDB, "Order_Num", "Dr_ID");
            return View();
        }

        // POST: Evaluations/Create
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Ev_Num,Order_Num,Ev_Estar,Ev_evaluation")] EvaluationDB evaluationDB)
        {
            if (ModelState.IsValid)
            {
                db.EvaluationDB.Add(evaluationDB);
                db.SaveChanges();

                string token = "s1KTJSJ5coT4WZTzpSjoB1v8j3M82QG5bwyD6fw0";
                string message = "剛剛的乘客為您評價了喔~"
                    + "\n" + "訂單編號:" + evaluationDB.Order_Num
                    + "\n" + "評價星星:" + evaluationDB.Ev_Estar
                    + "\n" + "評價文字:" + evaluationDB.Ev_evaluation;
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

                return RedirectToAction("Logining", "Customers");
            }

            ViewBag.Order_Num = new SelectList(db.information_OrderDB, "Order_Num", "Dr_ID", evaluationDB.Order_Num);
            return View(evaluationDB);
        }

        // GET: Evaluations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvaluationDB evaluationDB = db.EvaluationDB.Find(id);
            if (evaluationDB == null)
            {
                return HttpNotFound();
            }
            ViewBag.Order_Num = new SelectList(db.information_OrderDB, "Order_Num", "Dr_ID", evaluationDB.Order_Num);
            return View(evaluationDB);
        }

        // POST: Evaluations/Edit/5
        // 若要避免過量張貼攻擊，請啟用您要繫結的特定屬性。
        // 如需詳細資料，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Ev_Num,Order_Num,Ev_Estar,Ev_evaluation")] EvaluationDB evaluationDB)
        {
            if (ModelState.IsValid)
            {
                db.Entry(evaluationDB).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Order_Num = new SelectList(db.information_OrderDB, "Order_Num", "Dr_ID", evaluationDB.Order_Num);
            return View(evaluationDB);
        }

        // GET: Evaluations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EvaluationDB evaluationDB = db.EvaluationDB.Find(id);
            if (evaluationDB == null)
            {
                return HttpNotFound();
            }
            return View(evaluationDB);
        }

        // POST: Evaluations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EvaluationDB evaluationDB = db.EvaluationDB.Find(id);
            db.EvaluationDB.Remove(evaluationDB);
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
        //public ActionResult Search()
        //{
        //    return View();
        //}
        //public ActionResult Search_Evaluations(string Evano)
        //{
        //    var search_eva = from s in db.EvaluationDB
        //                               select s;
        //    //var q = db.information_OrderDB.Select(o => o.Ord_Money).Count();
        //    //string Dr_account = Session["Dr_account"].ToString();
        //    //var count = db.information_OrderDB.Where(o => o.Dr_ID.Contains(Dr_account) && o.Ord_date.Contains(theday)).Count();
        //    //int a = 70;
        //    //var total = a * count;
        //    //TempData["countall"] = "總共:" + total + "元";
        //    ViewBag.Evano = Evano;
        //    search_eva = search_eva.Where(s => s.Ev_Num.ToString().Contains(Evano));
        //    return View(search_eva.ToList());
        //}
    }
}
