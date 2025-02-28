using MvcBookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using PagedList;

namespace MvcBookStore.Controllers
{
    public class NguoidungController : Controller
    {
        // GET: Nguoidung
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Dangky()
        {
            return View();
        }

        //POST: Ham Dangky(post) nhan du lieu tu trang Dangky và thuc hien viec thuc hien tao moi du lieu
        [HttpPost]
        public ActionResult Dangky(FormCollection collection,KHACHHANG kh)
        {
            //Gan cac gia tri nguoi dung nhap nhap du lieu cho cac bien
            var hoten = collection["HotenKH"];
            var tendn = collection["TenDN"];
            var matkhau = collection["Matkhau"];
            var matkhaunhaplai = collection["Matkhaunhaplai"];
            var email = collection["Email"];
            var diachi = collection["Diachi"];
            var dienthoai = collection["Dienthoai"];
            var ngaysinh = String.Format("{0:MM/dd/yyyy}", collection["Ngaysinh"]);
            if (String.IsNullOrEmpty(hoten))
            {
                ViewData["Loi1"] = "Họ tên khách hàng không được để trống!";
            }
            else if(String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi2"] = "Tên đăng nhập không thể để trống!";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi3"] = "Mật khẩu không thể để trống!";
            }
            else if (String.IsNullOrEmpty(matkhaunhaplai))
            {
                ViewData["Loi4"] = "Mật khẩu nhập lại không thể để trống!";
            }
            else if (matkhaunhaplai != matkhau)
            {
                ViewData["Loi4"] = "Mật khẩu nhập lại khác mật khẩu";
            }
            else if (String.IsNullOrEmpty(email))
            {
                ViewData["Loi5"] = "Email không thể để trống!";
            }
            else if (String.IsNullOrEmpty(diachi))
            {
                ViewData["Loi6"] = "Địa chỉ không thể để trống!";
            }
            else if (String.IsNullOrEmpty(dienthoai))
            {
                ViewData["Loi7"] = "Số điện thoại không thể để trống!";
            }
            else
            {
                dbQLBansachDataContext db = new dbQLBansachDataContext();
                //Gan gia tri cho doi tuong duoc tao moi (kh)
                kh.HoTen = hoten;
                kh.Taikhoan = tendn;
                kh.Matkhau = matkhau;
                kh.Email = email;
                kh.DiachiKH = diachi;
                kh.DienthoaiKH = dienthoai;
                kh.Ngaysinh = DateTime.Parse(ngaysinh);
                db.KHACHHANGs.InsertOnSubmit(kh);
                db.SubmitChanges();
                return RedirectToAction("Dangnhap");
            }
            return this.Dangky();
        }
        [HttpGet]
        public ActionResult Dangnhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Dangnhap(FormCollection collection)
        {
            //Gan cac gia tri nguoi dung nhap vao cac bien
            var tendn = collection["TenDN"];
            var matkhau = collection["Matkhau"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Chưa nhập tên đăng nhập!";
            }   
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Chưa nhập mật khẩu!";
            }
            else
            {
                dbQLBansachDataContext db = new dbQLBansachDataContext();
                //Gan cac doi tuong duoc tao moi(kh)
                KHACHHANG kh = db.KHACHHANGs.SingleOrDefault(n => n.Taikhoan == tendn && n.Matkhau == matkhau);
                if (kh != null)
                {
                    //ViewBag.Thongbao = "Chúc mừng đăng nhập thành công!";
                    Session["Taikhoan"] = kh;
                    Session["TDN"] = kh.HoTen;
                    return RedirectToAction("DatHang", "Giohang");
                }
                else
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng!";
            }
            return View();
        }
        dbQLBansachDataContext db = new dbQLBansachDataContext();
        public ActionResult DonHang(int? page)
        {
            if (Session["TDN"] == null)
            {
                return RedirectToAction("Dangnhap", "Nguoidung");
            }
            int pageNumber = (page ?? 1);
            int pageSize = 10;
            //return View(db.SACHes.ToList());
            return View(db.DONDATHANGs.ToList().OrderBy(n => n.MaDonHang).ToPagedList(pageNumber, pageSize));
        }
    }
}