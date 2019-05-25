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
            if (Session["MaNV"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult Loai()
        {
            var loai = from l in data.LoaiThucDons select l;
            return PartialView(loai);
        }

        public ActionResult Sanh()
        {
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("Login", "NhanVien");
            }
            var sanh = from s in data.Sanhs select s;
            return View(sanh);
        }

        public ActionResult ThucDon()
        {
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("Login", "NhanVien");
            }
            var thucdon = from td in data.ThucDons select td;
            return View(data.ThucDons.OrderBy(n => n.MaLoai));
        }

        public ActionResult Details(int id)
        {
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("Login", "NhanVien");
            }
            var thucdon = from td in data.ThucDons
                          where td.MaMonAn == id
                          select td;
            return View(thucdon.Single());
        }

        public ActionResult Details1(int id)
        {
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("Login", "NhanVien");
            }
            var sanh = from s in data.Sanhs
                       where s.MaSanh == id
                       select s;
            return View(sanh.Single());
        }

        public ActionResult MonAn(int id)
        {
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("Login", "NhanVien");
            }
            var thucdon = from td in data.ThucDons where td.MaLoai == id select td;
            return View(thucdon);
        }

        public ActionResult DonNhan()
        {
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("Login", "NhanVien");
            }
            return View(data.DonDatTiecNhaps.ToList());
        }

        [HttpGet]
        public ActionResult DatTiec()
        {
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("Login", "NhanVien");
            }
            ViewBag.HinhThucList = new SelectList(data.HinhThucs.ToList(), "MaHinhThuc", "HinhThucToChuc");
            ViewBag.SanhList = new SelectList(data.Sanhs.ToList(), "MaSanh", "TenSanh");
            ViewBag.BuoiList = new SelectList(data.Buois.ToList(), "MaBuoi", "BuoiToChuc");         
            return View();
        }

        public ActionResult DatTiec(FormCollection collection)
        {
            DonDatTiec dt = new DonDatTiec();
            NhanVien nv = (NhanVien)Session["TaiKhoan"];
            dt.MaNV = nv.MaNV;
            var ht = collection["hoten"];
            dt.HoTenKH = ht;
            var dc = collection["diachi"];
            dt.DiaChi = dc;
            var sdt = collection["sdt"];
            dt.SDT = int.Parse(sdt);
            var sl = collection["soluong"];
            dt.SLKhach = int.Parse(sl);
            dt.MaNV = nv.MaNV;
            dt.NgayLap = DateTime.Now;
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

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(NhanVien objUser)
        {
            if (ModelState.IsValid)
            {
                using (dbDatTiecDataContext data = new dbDatTiecDataContext())
                {
                   
                    var obj = data.NhanViens.Where(a => a.TaiKhoan.Equals(objUser.TaiKhoan) && a.MatKhau.Equals(objUser.MatKhau)).FirstOrDefault();
                    if (obj != null)
                    {
                        Session["MaNV"] = obj.MaNV.ToString();
                        Session["TaiKhoan"] = obj;
                        Session["HoTen"] = obj.HoTen.ToString();
                        return RedirectToAction("Index");
                    }
                }
            }
            return View(objUser);
        }

        // Lay gio hang
        public List<GioHang> Laygiohang()
        {
            List<GioHang> lstGiohang = Session["Giohang"] as List<GioHang>;
            if (lstGiohang == null)
            {
                //Neu gio hang chua ton tai thi khoi tao listGiohang
                lstGiohang = new List<GioHang>();
                Session["Giohang"] = lstGiohang;
            }
            return lstGiohang;
        }
        //Them gio hang
        public ActionResult ThemGioHang(int iMaSP, string strURL)
        {
            //Lay ra Session gio hang
            List<GioHang> lstGiohang = Laygiohang();
            //Kiem tra sản phẩm có tồn tại trong Session["Giohang"] chưa
            GioHang sanpham = lstGiohang.Find(n => n.iMaSP == iMaSP);
            if (sanpham == null)
            {
                sanpham = new GioHang(iMaSP);
                lstGiohang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoluong++;
                return Redirect(strURL);
            }
        }
        //Tổng SL
        private int TongSL()
        {
            int iTongSL = 0;
            List<GioHang> lstGiohang = Session["Giohang"] as List<GioHang>;
            if (lstGiohang != null)
            {
                iTongSL = lstGiohang.Sum(n => n.iSoluong);
            }
            return iTongSL;
        }
        //Tinh tong tien
        private double TongTien()
        {
            double iTongTien = 0;
            List<GioHang> lstGiohang = Session["Giohang"] as List<GioHang>;
            if (lstGiohang != null)
            {
                iTongTien = lstGiohang.Sum(n => n.dThanhTien);
            }
            return iTongTien;
        }
        //Xay dung trang gio hang
        public ActionResult GioHang()
        {
            List<GioHang> lstGioHang = Laygiohang();
            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "TiecCuoi");
            }
            ViewBag.Tongsoluong = TongSL();
            ViewBag.Tongtien = TongTien();
            return View(lstGioHang);
        }

        //Hien thi gio hang
        public ActionResult GioHangPartial()
        {
            ViewBag.Tongsoluong = TongSL();
            ViewBag.Tongtien = TongTien();
            return PartialView();
        }

        //Xoa gio hang
        public ActionResult XoaGioHang(int iMaSP)
        {
            //Lay gio hang tu session
            List<GioHang> lstGioHang = Laygiohang();
            //Kiem tra san pham da co trong Session["GioHang"]
            GioHang sanpham = lstGioHang.SingleOrDefault(n => n.iMaSP == iMaSP);
            //Neu ton tai thi cho sua soluong
            if (sanpham != null)
            {
                lstGioHang.RemoveAll(n => n.iMaSP == iMaSP);
                return RedirectToAction("GioHang");
            }
            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "TiecCuoi");
            }
            return RedirectToAction("GioHang");
        }

        //Cap nhat gio hang
        public ActionResult CapNhatGioHang(int iMaSP, FormCollection f)
        {
            //Lay gio hang tu session
            List<GioHang> lstGioHang = Laygiohang();
            //Kiem tra san pham da co trong Session["GioHang"]
            GioHang sanpham = lstGioHang.SingleOrDefault(n => n.iMaSP == iMaSP);
            //Neu ton tai thi cho sua soluong
            if (sanpham != null)
            {
                sanpham.iSoluong = int.Parse(f["txtSoluong"].ToString());
            }
            return RedirectToAction("GioHang");
        }

        //Xoa tat ca gio hang
        public ActionResult XoaTatCaGioHang()
        {
            //Lay gio hang tu Session
            List<GioHang> lstGioHang = Laygiohang();
            lstGioHang.Clear();
            return RedirectToAction("Index", "TiecCuoi");
        }

        public ActionResult Dat()
        {

            List<GioHang> lstGioHang = Laygiohang();
            ViewBag.Tongsoluong = TongSL();
            ViewBag.Tongtien = TongTien();
            return View(lstGioHang);
        }
    }
}