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
            return View();
        }

        public ActionResult DonTiecNhap()
        {
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("Login", "NhanVien");
            }
            return View(data.DonDatTiecNhaps.ToList());
        }

        public ActionResult DonTiecLap()
        {
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("Login", "NhanVien");
            }
            return View(data.DonDatTiecs.ToList());
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
                    else
                        ViewBag.ThongBao = "Tài khoản không tồn tại";
                    return this.Login();
                }
            }
            return View(objUser);
        }

        public List<Menu> LayMenu()
        {
            List<Menu> lstMenu = Session["Menu"] as List<Menu>;
            if (lstMenu == null)
            {
                lstMenu = new List<Menu>();
                Session["Menu"] = lstMenu;
            }
            return lstMenu;
        }

        public ActionResult ThemMenu(int iMaMonAn, string strURL)
        {
            List<Menu> lstMenu = LayMenu();
            Menu sanpham = lstMenu.Find(n => n.iMaMonAn == iMaMonAn);
            if (sanpham == null)
            {
                sanpham = new Menu(iMaMonAn);
                lstMenu.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoluong++;
                return Redirect(strURL);
            }
        }

        private int TongSL()
        {
            int iTongSL = 0;
            List<Menu> lstMenu = Session["Menu"] as List<Menu>;
            if (lstMenu != null)
            {
                iTongSL = lstMenu.Sum(n => n.iSoluong);
            }
            return iTongSL;
        }

        private double TongTien()
        {
            double iTongTien = 0;
            List<Menu> lstMenu = Session["Menu"] as List<Menu>;
            if (lstMenu != null)
            {
                iTongTien = lstMenu.Sum(n => n.dThanhTien);
            }
            return iTongTien;
        }

        public ActionResult Menu()
        {
            List<Menu> lstMenu = LayMenu();
            if (lstMenu.Count == 0)
            {
                return RedirectToAction("ThucDon", "NhanVien");
            }
            ViewBag.Tongsoluong = TongSL();
            ViewBag.Tongtien = TongTien();
            return View(lstMenu);
        }

        public ActionResult XoaMenu(int iMaMonAn)
        {
            List<Menu> lstMenu = LayMenu();
            Menu sanpham = lstMenu.SingleOrDefault(n => n.iMaMonAn == iMaMonAn);
            if (sanpham != null)
            {
                lstMenu.RemoveAll(n => n.iMaMonAn == iMaMonAn);
                return RedirectToAction("Menu");
            }
            if (lstMenu.Count == 0)
            {
                return RedirectToAction("ThucDon", "NhanVien");
            }
            return RedirectToAction("Menu");
        }

        public ActionResult CapNhatMenu(int iMaMonAn, FormCollection f)
        {
            List<Menu> lstMenu = LayMenu();
            Menu sanpham = lstMenu.SingleOrDefault(n => n.iMaMonAn == iMaMonAn);
            if (sanpham != null)
            {
                sanpham.iSoluong = int.Parse(f["txtSoluong"].ToString());
            }
            return RedirectToAction("Menu");
        }

        public ActionResult XoaTatCaGMenu()
        {
            List<Menu> lstMenu = LayMenu();
            lstMenu.Clear();
            return RedirectToAction("Index", "NhanVien");
        }

        public ActionResult Dat()
        {
            List<Menu> lstMenu = LayMenu();
            ViewBag.Tongsoluong = TongSL();
            ViewBag.Tongtien = TongTien();
            return View(lstMenu);
        }

        [HttpGet]
        public ActionResult DatTiec()
        {
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("Login", "NhanVien");
            }
            List<Menu> lstMenu = LayMenu();
            ViewBag.HinhThucList = new SelectList(data.HinhThucs.ToList(), "MaHinhThuc", "HinhThucToChuc");
            ViewBag.SanhList = new SelectList(data.Sanhs.ToList(), "MaSanh", "TenSanh");
            ViewBag.BuoiList = new SelectList(data.Buois.ToList(), "MaBuoi", "BuoiToChuc");
            ViewBag.Tongsoluong = TongSL();
            ViewBag.Tongtien = TongTien();
            return View(lstMenu);
        }

        [HttpPost]
        public ActionResult DatTiec(FormCollection collection)
        {
            DonDatTiec dt = new DonDatTiec();
            List<Menu> mn = LayMenu();
            NhanVien nv = (NhanVien)Session["TaiKhoan"];
            dt.MaNV = nv.MaNV;
            var ht = collection["hoten"];

            var dc = collection["diachi"];

            var sdt = collection["sdt"];

            var sl = collection["soluong"];

            dt.MaNV = nv.MaNV;
            dt.NgayLap = DateTime.Now;
            var NgayToChuc = String.Format("{0:dd/MM/yyyy}", collection["NgayToChuc"]);


            var hinhthuc = collection["HinhThucList"];
            ViewBag.HinhThucList = new SelectList(data.HinhThucs.ToList(), "MaHinhThuc", "HinhThucToChuc");


            var sanh = collection["SanhList"];
            ViewBag.SanhList = new SelectList(data.Sanhs.ToList(), "MaSanh", "TenSanh");


            var buoi = collection["BuoiList"];
            ViewBag.BuoiList = new SelectList(data.Buois.ToList(), "MaBuoi", "BuoiToChuc");

            if (String.IsNullOrEmpty(ht))
            {
                ViewBag.ThongBao1 = "Họ tên khách hàng không được để trống";
            }
            else if (String.IsNullOrEmpty(dc))
            {
                ViewBag.ThongBao2 = "Phải nhập địa chỉ";
            }
            else if (String.IsNullOrEmpty(sdt))
            {
                ViewBag.ThongBao3 = "Phải nhập số điện thoại";
            }
            else if (String.IsNullOrEmpty(sl))
            {
                ViewBag.ThongBao4 = "Phải nhập số lượng khách";
            }
            else if (String.IsNullOrEmpty(NgayToChuc))
            {
                ViewBag.ThongBao5 = "Phải nhập ngày tổ chức";
            }
            else if (String.IsNullOrEmpty(hinhthuc))
            {
                ViewBag.ThongBao6 = "Phải nhập hình thức tổ chức";
            }
            else if (String.IsNullOrEmpty(sanh))
            {
                ViewBag.ThongBao7 = "Phải nhập sảnh";
            }
            else if (String.IsNullOrEmpty(buoi))
            {
                ViewBag.ThongBao8 = "Phải nhập buối tổ chức";
            }
            else
            {
                dt.HoTenKH = ht;
                dt.DiaChi = dc;
                dt.SDT = Convert.ToInt32(sdt);
                dt.SLKhach = int.Parse(sl);
                dt.MaNV = nv.MaNV;
                dt.NgayLap = Convert.ToDateTime(DateTime.Now);
                dt.NgayToChuc = Convert.ToDateTime(NgayToChuc);
                dt.MaHinhThuc = int.Parse(hinhthuc);
                dt.MaSanh = int.Parse(sanh);
                dt.MaBuoi = int.Parse(buoi);
                data.DonDatTiecs.InsertOnSubmit(dt);
                data.SubmitChanges();
                foreach (var item in mn)
                {
                    CTDonDat ctdd = new CTDonDat();
                    ctdd.MaDD = dt.MaDD;
                    ctdd.MaMonAn = item.iMaMonAn;
                    ctdd.SL = item.iSoluong;
                    ctdd.DonGia = (int)item.dDonGia;
                    data.CTDonDats.InsertOnSubmit(ctdd);
                }
                data.SubmitChanges();
                Session["Menu"] = null;
                return RedirectToAction("XacNhanDonHang", "NhanVien");
            }
            return this.DatTiec();
            
        }

        public ActionResult XacNhanDonHang()
        {
            return View();
        }

        public ActionResult ChiTietHD(int id)
        {
            var ctdd = from ct in data.CTDonDats where ct.MaDD == id select ct;
            return View(ctdd);
        }

        public ActionResult XoaDonDatNhap(int id)
        {
            DonDatTiecNhap dt = data.DonDatTiecNhaps.SingleOrDefault(n => n.MaNhap == id);
            ViewBag.MaNhap = dt.MaNhap;
            if (dt == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(dt);
        }

        [HttpPost, ActionName("XoaDonDatNhap")]
        public ActionResult XacNhanXoaDDN(int id)
        {
            DonDatTiecNhap dt = data.DonDatTiecNhaps.SingleOrDefault(n => n.MaNhap == id);
            ViewBag.MaNhap = dt.MaNhap; 
            if (dt == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.DonDatTiecNhaps.DeleteOnSubmit(dt);
            data.SubmitChanges();
            return RedirectToAction("DonTiecNhap");
        }

        [HttpGet]
        public ActionResult TraCuu()
        {
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("Login", "NhanVien");
            }
            ViewBag.SanhList = new SelectList(data.Sanhs.ToList(), "MaSanh", "TenSanh");
            ViewBag.BuoiList = new SelectList(data.Buois.ToList(), "MaBuoi", "BuoiToChuc");
            return View();
        }

        [HttpPost]
        public ActionResult TraCuu(FormCollection collection)
        {
            DonDatTiec dt = new DonDatTiec();
            var sanh = collection["SanhList"];
            ViewBag.SanhList = new SelectList(data.Sanhs.ToList(), "MaSanh", "TenSanh");
            dt.MaSanh = int.Parse(sanh);

            var buoi = collection["BuoiList"];
            ViewBag.BuoiList = new SelectList(data.Buois.ToList(), "MaBuoi", "BuoiToChuc");
            dt.MaBuoi = int.Parse(buoi);
            return View();
        }


        // GET: TimKiem
        [HttpPost]
        public ActionResult KetQuaTimKiem1(FormCollection collection)
        {
            String tukhoa1 = collection["sanh"].ToString();
            List<DonDatTiec> lstSanh = data.DonDatTiecs.Where(n => n.Sanh.TenSanh.Contains(tukhoa1)).ToList();
            if (lstSanh.Count == 0)
            {
                ViewBag.Thongbao = "Không tìm thấy tiệc ở sảnh này";
                return View(data.DonDatTiecs.OrderBy(n => n.MaDD));

            }
            return View(lstSanh.OrderBy(n => n.MaDD));
        }

        [HttpPost]
        public ActionResult KetQuaTimKiem2(FormCollection collection)
        {
            String tukhoa2 = collection["buoi"].ToString();
            List<DonDatTiec> lstBuoi = data.DonDatTiecs.Where(n => n.Buoi.BuoiToChuc.Contains(tukhoa2)).ToList();
            if (lstBuoi.Count == 0)
            {
                ViewBag.Thongbao = "Không tìm thấy tiệc tổ chức vào buổi này";
                return View(data.DonDatTiecs.OrderBy(n => n.MaDD));

            }
            return View(lstBuoi.OrderBy(n => n.MaDD));
        }

        [HttpPost]
        public ActionResult KetQuaTimKiem3(FormCollection collection)
        {
            String tukhoa3 = collection["ntc"].ToString();
            List<DonDatTiec> lstntc = data.DonDatTiecs.Where(n => Convert.ToString(n.NgayToChuc).Contains(tukhoa3)).ToList();
            if (lstntc.Count == 0)
            {
                ViewBag.Thongbao = "Không tìm thấy tiệc vào ngày này";
                return View(data.DonDatTiecs.OrderBy(n => n.MaDD));
            }
            return View(lstntc.OrderBy(n => n.MaDD));
        }
    }
}