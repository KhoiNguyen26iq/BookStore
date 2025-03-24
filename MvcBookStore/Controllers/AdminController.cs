using MvcBookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.IO;

namespace MvcBookStore.Controllers
{
    public class AdminController : Controller
    {
        dbQLBansachDataContext db = new dbQLBansachDataContext();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Sach(int ?page)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            int pageNumber = (page ?? 1);
            int pageSize = 10;
            //return View(db.SACHes.ToList());
            return View(db.SACHes.ToList().OrderBy(n => n.Masach).ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            //Gan cac gia tri nguoi dung nhap lieu cac bien
            var tendn = collection["username"];
            var matkhau = collection["password"];
            if(String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập!";
            }
            else if(String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu!";
            }
            else
            {
                //Gan gia tri cho doi tuong duoc tao moi
                Admin ad = db.Admins.SingleOrDefault(n => n.UserAdmin == tendn && n.PassAdmin == matkhau);
                if (ad != null)
                {
                    Session["Ten"] = ad.Hoten;
                    Session["Taikhoanadmin"] = ad;
                    return RedirectToAction("Index", "Admin");
                }
                else
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng!";
            }
            return View();
        }
        [HttpGet]
        public ActionResult ThemmoiSach()
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            //Dua du lieu vao dropdownList
            //Lay ds tu table LOAI, sap xep theo ten loai, cho lay gia tri Ma Loai, hien thi Tenloai
            ViewBag.MaLoai = new SelectList(db.Loais.ToList().OrderBy(n => n.TenLoai), "MaLoai", "TenLoai");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemmoiSach(SACH sach, HttpPostedFileBase fileupload)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            //Dua du lieu vao dropdownload
            ViewBag.MaLoai = new SelectList(db.Loais.ToList().OrderBy(n => n.TenLoai), "MaLoai", "TenLoai");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            //Kiem tra duong dan file
            if(fileupload==null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh nền!";
                return View();
            }
            //Them vao CSDL
            else
            {
                if (ModelState.IsValid)
                {
                    //Luu ten file, luu y bo sung thu vien
                    var fileName = Path.GetFileName(fileupload.FileName);
                    //Luu duong dan cua file
                    var path = Path.Combine(Server.MapPath("~/images/anhsp/"), fileName);
                    //Kiem tra anh da ton tai hay chua
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại tên tương tự! Vui lòng đặt tên khác!";
                    }
                    else
                    {
                        //Luu hinh anh vao duong dan
                        fileupload.SaveAs(path);
                    }
                    sach.Anhbia = fileName;
                    //Luu vao CSDL
                    db.SACHes.InsertOnSubmit(sach);
                    db.SubmitChanges();
                }
            }
            return View();
        }
        public ActionResult Chitietsach(int id)
        {
            //Lay doi tuong sach theo ma
            SACH sach = db.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.Masach = sach.Masach;
            if(sach==null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sach);
        }
        public ActionResult Xoasach(int id)
        {
            //Lay ra doi tuong sach can xoa theo ma
            SACH sach = db.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.Masach = sach.Masach;
            if(sach==null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sach);
        }
        [HttpPost,ActionName("Xoasach")]
        public ActionResult Xacnhanxoa(int id)
        {
            //Lay ra doi tuong sach can xoa theo ma
            SACH sach = db.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.Masach = sach.Masach;
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.SACHes.DeleteOnSubmit(sach);
            db.SubmitChanges();
            return RedirectToAction("Sach");
        }
        //Sua san pham
        [HttpGet]
        public ActionResult Suasach(int id)
        {
            //Lay ra doi tuong sach can xoa theo ma
            SACH sach = db.SACHes.SingleOrDefault(n => n.Masach == id);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Lay ds tu table LOAI, sap xep theo ten loai, cho lay gia tri Ma Loai, hien thi Tenloai
            ViewBag.MaLoai = new SelectList(db.Loais.ToList().OrderBy(n => n.TenLoai), "MaLoai", "TenLoai");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            return View(sach);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Suasach(SACH sach, HttpPostedFileBase fileupload)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            //Dua du lieu vao dropdownload
            ViewBag.MaLoai = new SelectList(db.Loais.ToList().OrderBy(n => n.TenLoai), "MaLoai", "TenLoai");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            //Kiem tra duong dan file
            if (fileupload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh nền!";
                return View();
            }
            //Them vao CSDL
            else
            {
                if (ModelState.IsValid)
                {
                    //Luu ten file, luu y bo sung thu vien
                    var fileName = Path.GetFileName(fileupload.FileName);
                    //Luu duong dan cua file
                    var path = Path.Combine(Server.MapPath("~/images/anhsp/"), fileName);
                    //Kiem tra anh da ton tai hay chua
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại tên tương tự! Vui lòng đặt tên khác!";
                    }
                    else
                    {
                        //Luu hinh anh vao duong dan
                        fileupload.SaveAs(path);
                    }
                    sach.Anhbia = fileName;
                    //Luu vao CSDL
                    UpdateModel(sach);
                    db.SubmitChanges();
                }
            }
            return RedirectToAction("Sach");
        }

        public ActionResult DonHang(int? page)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            int pageNumber = (page ?? 1);
            int pageSize = 10;
            return View(db.DONDATHANGs.ToList().OrderBy(n => n.MaDonHang).ToPagedList(pageNumber, pageSize));
        }
        //Đơn hàng
        public ActionResult Chitietdonhang(int id)
        {
            //Lay doi tuong sach theo ma
            CHITIETDONTHANG dh = db.CHITIETDONTHANGs.FirstOrDefault(n => n.MaDonHang == id);
            ViewBag.MaDonHang = dh.MaDonHang;
            if (dh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(dh);
        }
        [HttpGet]
        public ActionResult Suadonhang(int id)
        {
            DONDATHANG dh = db.DONDATHANGs.SingleOrDefault(n => n.MaDonHang == id);
            if (dh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(dh);
        }

        [HttpPost]
        public ActionResult Suadonhang(DONDATHANG dh)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            if (ModelState.IsValid)
            {
                DONDATHANG orderUpdate = db.DONDATHANGs.SingleOrDefault(n => n.MaDonHang == dh.MaDonHang);
                if (orderUpdate != null)
                {
                    orderUpdate.Ngaydat = dh.Ngaydat;
                    orderUpdate.Ngaygiao = dh.Ngaygiao;
                    orderUpdate.Tinhtranggiaohang = dh.Tinhtranggiaohang;
                    orderUpdate.Dathanhtoan = dh.Dathanhtoan;
                    db.SubmitChanges();
                }
                return RedirectToAction("DonHang");
            }
            return View(dh);
        }
        //Người dùng
        public ActionResult Chitietnguoidung(int id)
        {
            //Lay doi tuong nguoi dung theo ma
            KHACHHANG nguoidung = db.KHACHHANGs.SingleOrDefault(n => n.MaKH == id);
            ViewBag.MaKH = nguoidung.MaKH;
            if (nguoidung == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(nguoidung);
        }

        //Phân loại sách
        public ActionResult Loai(int? page)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            int pageNumber = (page ?? 1);
            int pageSize = 10;
            //return View(db.SACHes.ToList());
            return View(db.Loais.ToList().OrderBy(n => n.MaLoai).ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult ThemmoiLoai()
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            return View();
        }

        [HttpPost]
        public ActionResult ThemmoiLoai(Loai loai)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            if (ModelState.IsValid)
            {
                db.Loais.InsertOnSubmit(loai);
                db.SubmitChanges();
                return RedirectToAction("Loai");
            }
            return View(loai);
        }

        [HttpGet]
        public ActionResult SuaLoai(int id)
        {
            Loai loai = db.Loais.SingleOrDefault(n => n.MaLoai == id);
            if (loai == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(loai);
        }

        [HttpPost]
        public ActionResult SuaLoai(Loai loai)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            if (ModelState.IsValid)
            {
                Loai loaiUpdate = db.Loais.SingleOrDefault(n => n.MaLoai == loai.MaLoai);
                if (loaiUpdate != null)
                {
                    loaiUpdate.TenLoai = loai.TenLoai;
                    db.SubmitChanges();
                }
                return RedirectToAction("Loai");
            }
            return View(loai);
        }

        [HttpGet]
        public ActionResult XoaLoai(int id)
        {
            Loai loai = db.Loais.SingleOrDefault(n => n.MaLoai == id);
            if (loai == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(loai);
        }

        [HttpPost, ActionName("XoaLoai")]
        public ActionResult XacNhanXoaLoai(int id)
        {
            Loai loai = db.Loais.SingleOrDefault(n => n.MaLoai == id);
            if (loai == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.Loais.DeleteOnSubmit(loai);
            db.SubmitChanges();
            return RedirectToAction("Loai");
        }


        //Phân loại sách
        public ActionResult NXB(int? page)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            int pageNumber = (page ?? 1);
            int pageSize = 10;
            //return View(db.SACHes.ToList());
            return View(db.NHAXUATBANs.ToList().OrderBy(n => n.MaNXB).ToPagedList(pageNumber, pageSize));
        }
        // Thêm mới nhà xuất bản
        [HttpGet]
        public ActionResult ThemmoiNXB()
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            return View();
        }

        [HttpPost]
        public ActionResult ThemmoiNXB(NHAXUATBAN nxb)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            if (ModelState.IsValid)
            {
                db.NHAXUATBANs.InsertOnSubmit(nxb);
                db.SubmitChanges();
                return RedirectToAction("NXB");
            }
            return View(nxb);
        }

        // Sửa nhà xuất bản
        [HttpGet]
        public ActionResult SuaNXB(int id)
        {
            NHAXUATBAN nxb = db.NHAXUATBANs.SingleOrDefault(n => n.MaNXB == id);
            if (nxb == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(nxb);
        }

        [HttpPost]
        public ActionResult SuaNXB(NHAXUATBAN nxb)
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("Login", "Admin");
            }
            if (ModelState.IsValid)
            {
                NHAXUATBAN nxbUpdate = db.NHAXUATBANs.SingleOrDefault(n => n.MaNXB == nxb.MaNXB);
                if (nxbUpdate != null)
                {
                    nxbUpdate.TenNXB = nxb.TenNXB;
                    nxbUpdate.Diachi = nxb.Diachi;
                    nxbUpdate.DienThoai = nxb.DienThoai;
                    db.SubmitChanges();
                }
                return RedirectToAction("NXB");
            }
            return View(nxb);
        }

        // Xóa nhà xuất bản
        [HttpGet]
        public ActionResult XoaNXB(int id)
        {
            NHAXUATBAN nxb = db.NHAXUATBANs.SingleOrDefault(n => n.MaNXB == id);
            if (nxb == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(nxb);
        }

        [HttpPost, ActionName("XoaNXB")]
        public ActionResult XacNhanXoaNXB(int id)
        {
            NHAXUATBAN nxb = db.NHAXUATBANs.SingleOrDefault(n => n.MaNXB == id);
            if (nxb == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.NHAXUATBANs.DeleteOnSubmit(nxb);
            db.SubmitChanges();
            return RedirectToAction("NXB");
        }

        // GET: Admin/Logout 
        public ActionResult Logout()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Logout")]
        public ActionResult LogoutConfirmed()
        {
            Session["Taikhoanadmin"] = null;
            Session["Ten"] = null;
            return RedirectToAction("Login", "Admin");
        }
    }
}