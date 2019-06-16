using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DatTiec.Models;
using System.IO;

namespace DatTiec.Controllers
{
    public class AdminController : Controller
    {
        dbDatTiecDataContext data = new dbDatTiecDataContext();
        // GET: Admin
        public ActionResult Index()
        {
            if (Session["UserAdmin"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Admin objUser)
        {
            if (ModelState.IsValid)
            {
                using (dbDatTiecDataContext data = new dbDatTiecDataContext())
                {

                    var obj = data.Admins.Where(a => a.UserAdmin.Equals(objUser.UserAdmin) && a.PassAdmin.Equals(objUser.PassAdmin)).FirstOrDefault();
                    if (obj != null)
                    {
                        Session["UserAdmin"] = obj;
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

        public ActionResult Loai()
        {
            var loai = from l in data.LoaiThucDons select l;
            return PartialView(loai);
        }

        public ActionResult Sanh()
        {
            if (Session["UserAdmin"] == null || Session["UserAdmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            var sanh = from s in data.Sanhs select s;
            return View(sanh);
        }

        public ActionResult ThucDon()
        {
            if (Session["UserAdmin"] == null || Session["UserAdmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            var thucdon = from td in data.ThucDons select td;
            return View(data.ThucDons.OrderBy(n => n.MaLoai));
        }

        public ActionResult Details(int id)
        {
            if (Session["UserAdmin"] == null || Session["UserAdmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            var thucdon = from td in data.ThucDons
                          where td.MaMonAn == id
                          select td;
            return View(thucdon.Single());
        }

        public ActionResult Details1(int id)
        {
            if (Session["UserAdmin"] == null || Session["UserAdmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            var sanh = from s in data.Sanhs
                       where s.MaSanh == id
                       select s;
            return View(sanh.Single());
        }

        public ActionResult MonAn(int id)
        {
            if (Session["UserAdmin"] == null || Session["UserAdmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            var thucdon = from td in data.ThucDons where td.MaLoai == id select td;
            return View(thucdon);
        }

        [HttpGet]
        public ActionResult ThemSanh()
        {
            if (Session["UserAdmin"] == null || Session["UserAdmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemSanh(Sanh s, HttpPostedFileBase fileUpload)
        {
            if (fileUpload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var filename = Path.GetFileName(fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Sanh"), filename);
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        fileUpload.SaveAs(path);
                    }
                    s.Hinh = filename;
                    data.Sanhs.InsertOnSubmit(s);
                    data.SubmitChanges();
                }
                return RedirectToAction("Sanh");
            }
        }

        [HttpGet]
        public ActionResult ThemThucDon()
        {
            if (Session["UserAdmin"] == null || Session["UserAdmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            ViewBag.MaLoai = new SelectList(data.LoaiThucDons.ToList().OrderBy(n => n.MaLoai), "MaLoai", "TenLoai");
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemThucDon(ThucDon td, HttpPostedFileBase fileUpload)
        {
            ViewBag.MaLoai = new SelectList(data.LoaiThucDons.ToList().OrderBy(n => n.MaLoai), "MaLoai", "TenLoai");
            if (fileUpload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var filename = Path.GetFileName(fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/ThucDon"), filename);
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        fileUpload.SaveAs(path);
                    }
                    td.Hinh = filename;
                    data.ThucDons.InsertOnSubmit(td);
                    data.SubmitChanges();
                }
                return RedirectToAction("ThucDon");
            }
        }

        [HttpGet]
        public ActionResult XoaSanh(int id)
        {
            Sanh s = data.Sanhs.SingleOrDefault(n => n.MaSanh == id);
            ViewBag.MaSP = s.MaSanh;
            if (s == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(s);
        }

        [HttpPost, ActionName("XoaSanh")]
        public ActionResult XacNhanXoaSanh(int id)
        {
            Sanh s = data.Sanhs.SingleOrDefault(n => n.MaSanh == id);
            ViewBag.MaSP = s.MaSanh;
            if (s == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.Sanhs.DeleteOnSubmit(s);
            data.SubmitChanges();
            return RedirectToAction("Sanh");
        }

        [HttpGet]
        public ActionResult XoaThucDon(int id)
        {
            ThucDon td = data.ThucDons.SingleOrDefault(n => n.MaMonAn == id);
            ViewBag.MaSP = td.MaMonAn;
            if (td == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(td);
        }

        [HttpPost, ActionName("XoaThucDon")]
        public ActionResult XacNhanXoaThucDon(int id)
        {
            ThucDon td = data.ThucDons.SingleOrDefault(n => n.MaMonAn == id);
            ViewBag.MaSP = td.MaMonAn;
            if (td == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.ThucDons.DeleteOnSubmit(td);
            data.SubmitChanges();
            return RedirectToAction("ThucDon");
        }

        [HttpGet]
        public ActionResult SuaSanh(int id)
        {
            Sanh s = data.Sanhs.SingleOrDefault(n => n.MaSanh == id);
            if (s == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(s);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SuaSanh(int id, Sanh s, HttpPostedFileBase fileUpload)
        {
            s = data.Sanhs.SingleOrDefault(n => n.MaSanh == id);
            if (fileUpload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var filename = Path.GetFileName(fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Sanh"), filename);
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        fileUpload.SaveAs(path);
                    }
                    s.Hinh = filename;
                    //Luu vao CSDL
                    UpdateModel(s);
                    data.SubmitChanges();
                }
                return RedirectToAction("Sanh");
            }
        }

        [HttpGet]
        public ActionResult SuaThucDon(int id)
        {
            ThucDon td = data.ThucDons.SingleOrDefault(n => n.MaMonAn == id);
            if (td == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.MaLoai = new SelectList(data.LoaiThucDons.ToList().OrderBy(n => n.MaLoai), "MaLoai", "TenLoai", td.MaLoai);
            return View(td);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SuaThucDon(int id, ThucDon td, HttpPostedFileBase fileUpload)
        {
            td = data.ThucDons.SingleOrDefault(n => n.MaMonAn == id);
            ViewBag.MaLoai = new SelectList(data.LoaiThucDons.ToList().OrderBy(n => n.MaLoai), "MaLoai", "TenLoai");
            if (fileUpload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var filename = Path.GetFileName(fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/ThucDon"), filename);
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        fileUpload.SaveAs(path);
                    }
                    td.Hinh = filename;
                    //Luu vao CSDL
                    UpdateModel(td);
                    data.SubmitChanges();
                }
                return RedirectToAction("ThucDon");
            }
        }

        public ActionResult TaiKhoan()
        {
            if (Session["UserAdmin"] == null || Session["UserAdmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            return View(data.NhanViens.ToList().OrderBy(n => n.MaNV));
        }

        [HttpGet]
        public ActionResult XoaTK(int id)
        {
            NhanVien nv = data.NhanViens.SingleOrDefault(n => n.MaNV == id);
            ViewBag.MaKH = nv.MaNV;
            if (nv == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(nv);
        }

        [HttpPost, ActionName("XoaTK")]
        public ActionResult XacNhanXoaTK(int id)
        {
            NhanVien nv = data.NhanViens.SingleOrDefault(n => n.MaNV == id);
            ViewBag.MaKH = nv.MaNV; ;
            if (nv == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.NhanViens.DeleteOnSubmit(nv);
            data.SubmitChanges();
            return RedirectToAction("TaiKhoan");
        }

        [HttpGet]
        public ActionResult SuaTK(int id)
        {
            NhanVien nv = data.NhanViens.SingleOrDefault(n => n.MaNV == id);
            if (nv == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(nv);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SuaTK(int id, NhanVien nv)
        {
            nv = data.NhanViens.SingleOrDefault(n => n.MaNV == id);
            UpdateModel(nv);
            data.SubmitChanges();
            return RedirectToAction("TaiKhoan");
        }

        [HttpGet]
        public ActionResult ThemTK()
        {
            if (Session["UserAdmin"] == null || Session["UserAdmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            return View();
        }

        [HttpPost]
        public ActionResult ThemTK(FormCollection collection, NhanVien nv)
        {
            var hoten = collection["HoTen"];
            var gioitinh = collection["GioiTinh"];
            var ngaysinh = collection["NgaySinh"];
            var email = collection["Email"];
            var dt = collection["SoDienThoai"];
            var taikhoan = collection["TaiKhoan"];
            var matkhau = collection["MatKhau"];
            var matkhaunhaplai = collection["MatKhauNhapLai"];
            if (String.IsNullOrEmpty(hoten))
            {
                ViewBag.ThongBao1 = "Họ tên khách hàng không được để trống";
            }
            else if (String.IsNullOrEmpty(gioitinh))
            {
                ViewBag.ThongBao2 = "Phải nhập giới tính";
            }
            else if (String.IsNullOrEmpty(ngaysinh))
            {
                ViewBag.ThongBao3 = "Phải nhập ngày sinh";
            }
            else if (String.IsNullOrEmpty(email))
            {
                ViewBag.ThongBao4 = "Phải nhập email";
            }
            else if (String.IsNullOrEmpty(dt))
            {
                ViewBag.ThongBao5 = "Phải nhập số điện thoại";
            }
            else if (String.IsNullOrEmpty(taikhoan))
            {
                ViewBag.ThongBao6 = "Phải nhập tài khoản";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewBag.ThongBao7 = "Phải nhập mật khẩu";
            }
            else if (String.IsNullOrEmpty(matkhaunhaplai))
            {
                ViewBag.ThongBao8 = "Phải nhập lại mật khẩu";
            }
            else
            {
                nv.HoTen = hoten;
                nv.GioiTinh = gioitinh;
                nv.NgaySinh = Convert.ToDateTime(ngaysinh);
                nv.Email = email;
                nv.SDT = Convert.ToInt32(dt);
                nv.TaiKhoan = taikhoan;
                nv.MatKhau = matkhau;
                data.NhanViens.InsertOnSubmit(nv);
                data.SubmitChanges();
                return RedirectToAction("TaiKhoan");
            }
            return this.ThemTK();
        }

        public ActionResult HoaDon()
        {
            if (Session["UserAdmin"] == null || Session["UserAdmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            return View(data.DonDatTiecs.ToList().OrderBy(n => n.MaDD));
        }

        public ActionResult DanhGia()
        {
            if (Session["UserAdmin"] == null || Session["UserAdmin"].ToString() == "")
            {
                return RedirectToAction("Login", "Admin");
            }
            return View(data.DanhGias.ToList().OrderBy(n => n.MaDanhGia));
        }

        public ActionResult XoaDanhGia(int id)
        {
            DanhGia dg = data.DanhGias.SingleOrDefault(n => n.MaDanhGia == id);
            ViewBag.MaDanhGia = dg.MaDanhGia;
            if (dg == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(dg);
        }

        [HttpPost, ActionName("XoaDanhGia")]
        public ActionResult XacNhanXoaDanhGia(int id)
        {
            DanhGia dg = data.DanhGias.SingleOrDefault(n => n.MaDanhGia == id);
            ViewBag.MaDanhGia = dg.MaDanhGia;
            if (dg == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.DanhGias.DeleteOnSubmit(dg);
            data.SubmitChanges();
            return RedirectToAction("DanhGia");
        }

        public ActionResult ChiTietDG(int id)
        {
            DanhGia dg = data.DanhGias.SingleOrDefault(n => n.MaDanhGia == id);
            ViewBag.MaDanhGia = dg.MaDanhGia;
            if (dg == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(dg);
        }
    }
}