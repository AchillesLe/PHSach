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
    public class Bill_ExportController : Controller
    {
        private PhatHanhSachEntities db = new PhatHanhSachEntities();

        // GET: Bill_Export
        public ActionResult Index()
        {
            var bill_Export = db.Bill_Export.Include(b => b.Agency);
            return View(bill_Export.ToList());
        }

        public ActionResult Deletectpx(int bookid)
        {
            var list = (List<Detail_Bill_Export>)Session["ctphieuxuat"];
            list.RemoveAll(p => p.Book_id == bookid);
            return RedirectToAction("ThemChiTietPhieuXuat");
        }

        // GET: Bill_Export/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bill_Export bill_Export = db.Bill_Export.Find(id);
            var a = (from b in db.Detail_Bill_Export
                     where b.Bill_Export_id == id
                     select b).ToList();
            ViewBag.ctpx = a;
            if (bill_Export == null)
            {
                return HttpNotFound();
            }
            return View(bill_Export);
        }

        // GET: Bill_Export/Create
        public ActionResult Create()
        {
            ViewBag.Agency_id = new SelectList(db.Agencies, "Agency_id", "Agency_name");
            return View();
        }

        // POST: Bill_Export/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Bill_Export_id,Agency_id,Total,Recipients,CreatedDate")] Bill_Export bill_Export)
        {
            if (ModelState.IsValid)
            {
                bill_Export.CreatedDate = DateTime.Now;
                var tempbillexport = bill_Export;
                Session["ctphieuxuat"] = new List<Detail_Bill_Export>();
                Session["PhieuXuat"] = bill_Export;
                return RedirectToAction("ThemChiTietPhieuXuat");
            }

            ViewBag.Agency_id = new SelectList(db.Agencies, "Agency_id", "Agency_name", bill_Export.Agency_id);
            return View(bill_Export);
        }

        [HttpGet]
        public ActionResult ThemChiTietPhieuXuat()
        {
            ViewBag.sach = new SelectList(db.Books, "Book_id", "Book_name");
            //var phieuxuat = (Bill_Export)TempData["PhieuXuat"];
            return View();
        }

        [HttpPost]
        public ActionResult ThemChiTietPhieuXuat(FormCollection chitiet)
        {
            ViewBag.sach = new SelectList(db.Books, "Book_id", "Book_name");
            ViewBag.loi = null;
            if (Request.Form["add"] != null)
            {
                if (ModelState.IsValid)
                {
                    bool check = true;
                    foreach (var ctpx in (List<Detail_Bill_Export>)Session["ctphieuxuat"])
                    {
                        if (ctpx.Book_id == Int32.Parse(chitiet["sach"].ToString()))
                        {
                            check = false;
                            ViewBag.loi = "Sách đã được thêm vào phiếu trước đó";                           
                            break;
                        }
                    }
                    if (check)
                    {
                        var sach = db.Books.Find(Int32.Parse(chitiet["sach"].ToString()));
                        //kiểm tra danh sách chi tiết null
                        //if ((List<Detail_Bill_Export>)Session["ctphieuxuat"] == null)
                        //{
                        int soluong = Int32.Parse(chitiet["soluong"].ToString());
                        Inventory_Book tonkho1 = db.Inventory_Book.OrderByDescending(m => m.id).FirstOrDefault(m => m.Book_id == (int)sach.Book_id);

                        //kiểm tra mã sách
                        if (sach == null)
                        {
                            ViewBag.loi = "Mã sách không tồn tại";
                            goto baoloi;
                        }
                        else
                        {
                            //kiểm tra tồn kho
                            if (tonkho1 == null)
                            {
                                ViewBag.loi = "Không có tồn kho";
                                //ModelState.AddModelError("", "Không có tồn kho");
                                goto baoloi;
                            }
                            else
                            {
                                if (tonkho1.Quantity <= 0)
                                {
                                    ViewBag.loi = "Sách đã hết trong kho";
                                    goto baoloi;
                                    //ModelState.AddModelError("", "Sách đã hết hàng trong kho");
                                }
                                else
                                {
                                    //kiểm tra điều kiện vượt quá số lượng trong kho
                                    if (tonkho1.Quantity < soluong)
                                    {
                                        ViewBag.loi = "Số lượng sách không đủ để xuất";
                                        goto baoloi;
                                        //ModelState.AddModelError("", "Vượt quá số lượng cho phép xuất");
                                    }
                                    else
                                    {
                                        Detail_Bill_Export ctpx = new Detail_Bill_Export();
                                        ctpx.Bill_Export_id = (db.Bill_Export.Max(u => (int?)u.Bill_Export_id)!=null ? db.Bill_Export.Max(u => u.Bill_Export_id):0 ) + 1;// db.Bill_Export.Count() + 1;
                                        ctpx.Book_id = Int32.Parse(chitiet["sach"].ToString());
                                        ctpx.Quantity = Int32.Parse(chitiet["soluong"].ToString());
                                        ctpx.Cost = double.Parse(db.Books.Find(ctpx.Book_id).Cost_Export.ToString());
                                        ctpx.Total = ctpx.Quantity * ctpx.Cost;
                                        //((List<Int32>)Session["BookID"]).Add(ctpx.Book_id);
                                        ((List<Detail_Bill_Export>)Session["ctphieuxuat"]).Add(ctpx);
                                    }
                                }
                            }
                        }
                        //}
                        return RedirectToAction("ThemChiTietPhieuXuat");
                    }
                    else
                    {
                       
                    }
                }
            }
            else if (Request.Form["create"] != null)
            {
                if (ModelState.IsValid)
                {
                    double tongTien = 0;
                    double? temptongtien = 0;
                    Bill_Export test = new Bill_Export();
                    test = (Bill_Export)Session["PhieuXuat"];
                    var luu = db.Bill_Export.Add(test);
                    db.SaveChanges();
                    /*foreach (var ctpx in (List<Detail_Bill_Export>)Session["ctphieuxuat"])
                    {
                        temptongtien += (ctpx.Cost * ctpx.Quantity);
                        tongTien = double.Parse(temptongtien.ToString());
                        //tongSoLuong += ctpx.SoLuongXuat;
                    }
                    Debt_Agency debt1 = db.Debt_Agency.OrderByDescending(m => m.id).FirstOrDefault(m => m.Agency_id == (int)luu.Agency_id);
                    if (debt1 == null || (debt1 != null && debt1.debt >= 0 && debt1.debt >= tongTien))
                    {*/
                    foreach (var ctpx in (List<Detail_Bill_Export>)Session["ctphieuxuat"])
                    {
                        temptongtien += (ctpx.Cost * ctpx.Quantity);
                        tongTien = double.Parse(temptongtien.ToString());
                        //tongSoLuong += ctpx.SoLuongXuat;

                        //lưu chi tiết phiếu xuất
                        Detail_Bill_Export ctPhieuXuat = new Detail_Bill_Export();
                        ctPhieuXuat = ctpx;
                        db.Detail_Bill_Export.Add(ctPhieuXuat);
                        //db.SaveChanges();

                        //cập nhật tồn kho công ty
                        Inventory_Book tonkho = new Inventory_Book();
                        tonkho.UpdatedDate = DateTime.Now;
                        Inventory_Book tonkho1 = db.Inventory_Book.OrderByDescending(m => m.id).FirstOrDefault(m => m.Book_id == (int)ctpx.Book_id);
                        //if (tonkho1 != null)
                        //{
                        tonkho.Book_id = ctpx.Book_id;
                        tonkho.Quantity = tonkho1.Quantity - ctpx.Quantity;
                        //}
                        //Tạm thời để đó
                        /* else
                         {
                             tonkho.Book_id = ctpx.Book_id;
                             tonkho.Quantity = ctpx.Quantity;
                         }*/
                        db.Inventory_Book.Add(tonkho);


                        //cập nhật tồn kho đại lý
                        Inventory_Agency tonkhodaily = new Inventory_Agency();
                        Inventory_Agency tonkhodaily1 = db.Inventory_Agency.OrderByDescending(n => n.id).FirstOrDefault(n => (n.Agency_id == (int)luu.Agency_id && n.Book_id == (int)ctpx.Book_id));
                        //tonkhodaily.repay_quantity = tonkhodaily1.repay_quantity;
                        tonkhodaily.UpdatedDate = DateTime.Now;
                        if (tonkhodaily1 != null)
                        {                       
                            tonkhodaily.Book_id = ctpx.Book_id;
                            tonkhodaily.Agency_id = luu.Agency_id;
                            tonkhodaily.deliver_quantity = tonkhodaily1.deliver_quantity + ctpx.Quantity;
                            tonkhodaily.repay_quantity = tonkhodaily1.repay_quantity;
                        }
                        else
                        {
                            tonkhodaily.Book_id = ctpx.Book_id;
                            tonkhodaily.Agency_id = luu.Agency_id;
                            tonkhodaily.deliver_quantity = ctpx.Quantity;
                            tonkhodaily.repay_quantity = 0;
                        }
                        db.Inventory_Agency.Add(tonkhodaily);
                    }
                    //lưu phiếu xuất
                    //addedPhieuXuat.TongSoLuong = tongSoLuong;

                    Debt_Agency debt1 = db.Debt_Agency.OrderByDescending(m => m.id).FirstOrDefault(m => m.Agency_id == (int)luu.Agency_id);
                    if (debt1 == null || (debt1 != null && debt1.debt > 0 && debt1.debt >= tongTien) || debt1.debt == 0)
                    {
                        luu.Total = tongTien;
                        db.Bill_Export.Attach(luu);
                        db.Entry(luu).State = EntityState.Modified;
                        db.SaveChanges();
                        //cập nhật công nợ Đại Lý
                        Debt_Agency debt = new Debt_Agency();

                        //Tạm thời để đó
                        //Debt_Agency debt1 = db.Debt_Agency.OrderByDescending(m => m.id).FirstOrDefault(m => m.Agency_id == (int)luu.Agency_id);
                        debt.update_date = DateTime.Now;
                        if (debt1 != null)
                        {
                            debt.Agency_id = luu.Agency_id;
                            debt.debt = debt1.debt + luu.Total;
                        }
                        else
                        {
                            debt.Agency_id = luu.Agency_id;
                            debt.debt = luu.Total;
                        }
                        db.Debt_Agency.Add(debt);

                        db.SaveChanges();

                        Session["ctphieuxuat"] = null;
                        Session["PhieuXuat"] = null;
                        return RedirectToAction("Create");
                    }
                    else
                    {
                        ViewBag.loi = "Vượt quá số nợ cho phép, hãy bấm hủy phiếu để tạo lại!";
                        goto baoloi;
                        //return RedirectToAction("ThemChiTietPhieuXuat");
                    }
                }
            }         
            baoloi:
            return View();
        }


        // GET: Bill_Export/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bill_Export bill_Export = db.Bill_Export.Find(id);
            if (bill_Export == null)
            {
                return HttpNotFound();
            }
            ViewBag.Agency_id = new SelectList(db.Agencies, "Agency_id", "Agency_name", bill_Export.Agency_id);
            return View(bill_Export);
        }

        // POST: Bill_Export/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Bill_Export_id,Agency_id,Total,Recipients,CreatedDate")] Bill_Export bill_Export)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bill_Export).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Agency_id = new SelectList(db.Agencies, "Agency_id", "Agency_name", bill_Export.Agency_id);
            return View(bill_Export);
        }

        // GET: Bill_Export/Delete/5
        public ActionResult Delete(int? id)
        {
            Bill_Export bill = db.Bill_Export.Find(id);
            db.Bill_Export.Remove(bill);
            db.SaveChanges();
            return RedirectToAction("Index");
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //Bill_Export bill_Export = db.Bill_Export.Find(id);
            //if (bill_Export == null)
            //{
            //    return HttpNotFound();
            //}
            //return View(bill_Export);
        }

        //// POST: Bill_Export/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Bill_Export bill_Export = db.Bill_Export.Find(id);
        //    db.Bill_Export.Remove(bill_Export);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
