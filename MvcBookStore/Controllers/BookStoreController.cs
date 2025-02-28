using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcBookStore.Models;
using MvcBookStore.Controllers;
//list
using PagedList;
using PagedList.Mvc;
namespace MvcBookStore.Controllers
{
    public class BookStoreController : Controller
    {
        //Tao 1 doi tuong chua tona bo CSDL tu dbQLBansach
        dbQLBansachDataContext data = new dbQLBansachDataContext();

        private List<SACH> Laysachmoi(int count)
        {
            //Sap xep giam dan theo Ngaycapnhat , lay gia tri count dong dau
            return data.SACHes.OrderByDescending(a => a.Ngaycapnhat).Take(count).ToList();
        }
        // GET: BookStore
        public ActionResult Index(int ? page)
        {
            //Tao bien quy dinh so san pham tren 1 trang
            int pageSize = 8;
            //Tao bien so trang
            int pageNum = (page ?? 1);
            //Lay nhung quyen sach moi nhat
            var sachmoi = Laysachmoi(16);
            return View(sachmoi.ToPagedList(pageNum,pageSize));
        }
        //Them chu de voi tung muc nav
        public ActionResult Loai()
        {
            var loai = from cd in data.Loais select cd;
            return PartialView(loai);
        }
        //Them nha xuat ban voi tung muc nav
        public ActionResult Nhaxuatban()
        {
            var nhaxuatban = from nxb in data.NHAXUATBANs select nxb;
            return PartialView(nhaxuatban);
        }
        //Chia san pham theo chu de
        public ActionResult SPTheoloai(int id,int ? page)
        {
            //Tao bien quy dinh so san pham tren 1 trang
            int pageSize = 8;
            //Tao bien so trang
            int pageNum = (page ?? 1);
            var sach = from s in data.SACHes where s.MaLoai == id select s;
            return View(sach.ToPagedList(pageNum, pageSize));
        }
        //Chia san pham theo Nha Xuat Ban
        public ActionResult SPTheoNXB(int id, int? page)
        {
            //Tao bien quy dinh so san pham tren 1 trang
            int pageSize = 8;
            //Tao bien so trang
            int pageNum = (page ?? 1);
            var sach = from s in data.SACHes where s.MaNXB == id select s;
            return View(sach.ToPagedList(pageNum, pageSize));
        }
        //Lay thong tin chi tiet sach
        public ActionResult Details(int id)
        {
            var sach = from s in data.SACHes
                       where s.Masach == id
                       select s;
            return View(sach.Single());
        }
        public ActionResult Thoat()
        {
            Session["Taikhoan"] = null;
            return View();
        }
    }
}