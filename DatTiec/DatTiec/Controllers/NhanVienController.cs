using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DatTiec.Models;

namespace DatTiec.Controllers
{
    public class NhanVienController : Controller
    {
        dbDatTiecDataContext data = new dbDatTiecDataContext();
        // GET: NhanVien
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

        public ActionResult MonAn(int id)
        {
            var thucdon = from td in data.ThucDons where td.MaLoai == id select td;
            return View(thucdon);
        }

        public ActionResult DonNhan()
        {
            return View(data.DonDatTiecNhaps.ToList());
        }

        [HttpGet]
        public ActionResult DatTiec()
        {
            ViewBag.HinhThucList = new SelectList(data.HinhThucs.ToList(), "MaHinhThuc", "HinhThucToChuc");
            ViewBag.SanhList = new SelectList(data.Sanhs.ToList(), "MaSanh", "TenSanh");
            ViewBag.BuoiList = new SelectList(data.Buois.ToList(), "MaBuoi", "BuoiToChuc");         
            return View();
        }

        public ActionResult DatTiec(FormCollection collection)
        {
            DonDatTiec dt = new DonDatTiec();
            NhanVien nv = (NhanVien)Session["TaiKhoan"];
            var ht = collection["hoten"];
            dt.HoTenKH = ht;
            var dc = collection["diachi"];
            dt.DiaChi = dc;
            var sdt = collection["sdt"];
            dt.SDT = int.Parse(sdt);
            var sl = collection["soluong"];
            dt.SLKhach = int.Parse(sl);
            dt.MaNV = nv.MaNV;
            var NgayLap = DateTime.Now;
            var NgayToChuc = String.Format("{0:dd/MM/yyyy}", collection["NgayToChuc"]);
            dt.NgayToChuc = DateTime.Parse(NgayToChuc);

            var hinhthuc = collection["HinhThucList"];
            ViewBag.HinhThucList = new SelectList(data.HinhThucs.ToList(), "MaHinhThuc", "HinhThucToChuc");
            dt.MaHinhThuc = int.Parse(hinhthuc);

            var sanh = collection["SanhList"];
            ViewBag.SanhList = new SelectList(data.Sanhs.ToList(), "MaSanh", "TenSanh");
            dt.MaSanh= int.Parse(sanh);

            var buoi = collection["BuoiList"];
            ViewBag.BuoiList = new SelectList(data.Buois.ToList(), "MaBuoi", "BuoiToChuc");
            dt.MaBuoi = int.Parse(buoi);

            data.DonDatTiecs.InsertOnSubmit(dt);
            data.SubmitChanges();

            return RedirectToAction("Index", "NhanVien");
        }
    }
}