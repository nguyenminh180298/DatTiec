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
            var em = collection["email"];
            var sdt = collection["sdt"];
            var nd = collection["noidung"];
            if (String.IsNullOrEmpty(ht))
            {
                ViewBag.ThongBao1 = "Họ tên không được để trống";
            }
            else if (String.IsNullOrEmpty(em))
            {
                ViewBag.ThongBao2 = "Phải nhập email";
            }
            else if (String.IsNullOrEmpty(sdt))
            {
                ViewBag.ThongBao3 = "Phải nhập số điện thoại";
            }
            else if (String.IsNullOrEmpty(nd))
            {
                ViewBag.ThongBao4 = "Phải nhập nội dung";
            }
            else
            {
                dg.HoTen = ht;
                dg.Email = em;
                dg.SDT = int.Parse(sdt);
                dg.NoiDung = nd;
                data.DanhGias.InsertOnSubmit(dg);
                data.SubmitChanges();
                return RedirectToAction("LienHe", "DatTiec");
            }
            return this.LienHe();
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
            var sdt = collection["sdt"];
            var sl = collection["soluong"];
            ViewBag.BuoiList = new SelectList(data.Buois.ToList(), "MaBuoi", "BuoiToChuc");
            var buoi = collection["BuoiList"];
            dg.NgayToChuc = DateTime.Now;
            var hinhthuc = collection["HinhThucList"];
            ViewBag.HinhThucList = new SelectList(data.HinhThucs.ToList(), "MaHinhThuc", "HinhThucToChuc");            
            var dc = collection["diachi"];
            if (String.IsNullOrEmpty(ht))
            {
                ViewBag.ThongBao1 = "Họ tên không được để trống";
            }
            else if (String.IsNullOrEmpty(sdt))
            {
                ViewBag.ThongBao2 = "Phải nhập số điện thoại";
            }
            else if (String.IsNullOrEmpty(sl))
            {
                ViewBag.ThongBao3 = "Phải nhập số lượng khách";
            }
            else if (String.IsNullOrEmpty(buoi))
            {
                ViewBag.ThongBao4 = "Phải nhập buối tổ chức";
            }
            else if (String.IsNullOrEmpty(hinhthuc))
            {
                ViewBag.ThongBao5 = "Phải nhập hình thức tổ chức";
            }
            else if (String.IsNullOrEmpty(dc))
            {
                ViewBag.ThongBao6 = "Phải nhập địa chỉ";
            }
            else
            {
                dg.HoTen = ht;
                dg.SDT = int.Parse(sdt);
                dg.SLKhach = int.Parse(sl);
                dg.MaBuoi = int.Parse(buoi);
                dg.NgayToChuc = DateTime.Now;
                dg.MaHinhThuc = int.Parse(hinhthuc);
                dg.DiaChi = dc;
                data.DonDatTiecNhaps.InsertOnSubmit(dg);
                data.SubmitChanges();
                return RedirectToAction("Index", "DatTiec");
            }
            return this.DatTiec();
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