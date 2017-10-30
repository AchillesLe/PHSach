using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PHSach.Models;

namespace PHSach.Controllers
{
    public class Inventory_AgencyController : Controller
    {
        private PhatHanhSachEntities db = new PhatHanhSachEntities();

        // GET: Inventory_Agency
        public ActionResult Index()
        {
            var inventory_Agency = db.Inventory_Agency.Include(i => i.Agency).Include(i => i.Book);
            return View(inventory_Agency.ToList());
        }

        // GET: Inventory_Agency/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventory_Agency inventory_Agency = db.Inventory_Agency.Find(id);
            if (inventory_Agency == null)
            {
                return HttpNotFound();
            }
            return View(inventory_Agency);
        }

        // GET: Inventory_Agency/Create
        public ActionResult Create()
        {
            ViewBag.Agency_id = new SelectList(db.Agencies, "Agency_id", "Agency_name");
            ViewBag.Book_id = new SelectList(db.Books, "Book_id", "Book_name");
            return View();
        }

        // POST: Inventory_Agency/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,Agency_id,Book_id,deliver_quantity,repay_quantity")] Inventory_Agency inventory_Agency)
        {
            if (ModelState.IsValid)
            {
                db.Inventory_Agency.Add(inventory_Agency);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Agency_id = new SelectList(db.Agencies, "Agency_id", "Agency_name", inventory_Agency.Agency_id);
            ViewBag.Book_id = new SelectList(db.Books, "Book_id", "Book_name", inventory_Agency.Book_id);
            return View(inventory_Agency);
        }

        // GET: Inventory_Agency/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventory_Agency inventory_Agency = db.Inventory_Agency.Find(id);
            if (inventory_Agency == null)
            {
                return HttpNotFound();
            }
            ViewBag.Agency_id = new SelectList(db.Agencies, "Agency_id", "Agency_name", inventory_Agency.Agency_id);
            ViewBag.Book_id = new SelectList(db.Books, "Book_id", "Book_name", inventory_Agency.Book_id);
            return View(inventory_Agency);
        }

        // POST: Inventory_Agency/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,Agency_id,Book_id,deliver_quantity,repay_quantity")] Inventory_Agency inventory_Agency)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inventory_Agency).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Agency_id = new SelectList(db.Agencies, "Agency_id", "Agency_name", inventory_Agency.Agency_id);
            ViewBag.Book_id = new SelectList(db.Books, "Book_id", "Book_name", inventory_Agency.Book_id);
            return View(inventory_Agency);
        }

        // GET: Inventory_Agency/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventory_Agency inventory_Agency = db.Inventory_Agency.Find(id);
            if (inventory_Agency == null)
            {
                return HttpNotFound();
            }
            return View(inventory_Agency);
        }

        // POST: Inventory_Agency/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Inventory_Agency inventory_Agency = db.Inventory_Agency.Find(id);
            db.Inventory_Agency.Remove(inventory_Agency);
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
    }
}
