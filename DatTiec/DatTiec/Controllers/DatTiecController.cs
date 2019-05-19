using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DatTiec.Models;

namespace DatTiec.Controllers
{
    public class DatTiecController : Controller
    {
        dbDatTiecDataContext data = new dbDatTiecDataContext();
        // GET: DatTiec
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Loai()
        {
            var loai = from l in data.LoaiThucDons select l;
            return PartialView(loai);
        }

        public ActionResult Sanh()
        {
            var sanh = from s in data.Sanhs select s;
            return View(sanh);
        }

        public ActionResult ThucDon()
        {
            var thucdon = from td in data.ThucDons select td;
            return View(data.ThucDons.OrderBy(n => n.MaLoai));
        }

        public ActionResult MonAn(int id)
        {
            var thucdon = from td in data.ThucDons where td.MaLoai == id select td;
            return View(thucdon);
        }

        public ActionResult GioiThieu()
        {
            return View();
        }


        [HttpGet]
        public ActionResult LienHe()
        {
            return View();
        }

        public ActionResult LienHe(FormCollection collection)
        {
            DanhGia dg = new DanhGia();
            var ht = collection["hoten"];
            dg.HoTen = ht;
            var em = collection["email"];
            dg.Email = em;
            var sdt = collection["sdt"];
            dg.SDT = int.Parse(sdt);
            var nd = collection["noidung"];
            dg.NoiDung = nd;
            data.DanhGias.InsertOnSubmit(dg);
            data.SubmitChanges();

            return RedirectToAction("LienHe", "DatTiec");
        }


        [HttpGet]
        public ActionResult DatTiec()
        {
            ViewBag.BuoiList = new SelectList(data.Buois.ToList(), "MaBuoi", "BuoiToChuc");
            ViewBag.HinhThucList = new SelectList(data.HinhThucs.ToList(), "MaHinhThuc", "HinhThucToChuc");
            return View();
        }

        public ActionResult DatTiec(FormCollection collection)
        {
            DonDatTiecNhap dg = new DonDatTiecNhap();
            var ht = collection["hoten"];
            dg.HoTen = ht;
            var sdt = collection["sdt"];
            dg.SDT = int.Parse(sdt);
            var sl = collection["soluong"];
            dg.SLKhach = int.Parse(sl);
            ViewBag.BuoiList = new SelectList(data.Buois.ToList(), "MaBuoi", "BuoiToChuc");
            var buoi = collection["BuoiList"];
            dg.MaBuoi = int.Parse(buoi);
            dg.NgayToChuc = DateTime.Now;
            var hinhthuc = collection["HinhThucList"];
            ViewBag.HinhThucList = new SelectList(data.HinhThucs.ToList(), "MaHinhThuc", "HinhThucToChuc");
            dg.MaHinhThuc = int.Parse(hinhthuc);
            var dc = collection["diachi"];
            dg.DiaChi = dc;
            data.DonDatTiecNhaps.InsertOnSubmit(dg);
            data.SubmitChanges();

            return RedirectToAction("Index", "DatTiec");
        }

        public ActionResult Details(int id)
        {
            var thucdon = from td in data.ThucDons
                          where td.MaMonAn == id
                          select td;
            return View(thucdon.Single());
        }

        public ActionResult Details1(int id)
        {
            var sanh = from s in data.Sanhs
                          where s.MaSanh == id
                          select s;
            return View(sanh.Single());
        }

    }
}