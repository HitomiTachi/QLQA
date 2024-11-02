using QLQA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLQA.BLL
{
    internal class CATEGORIES_Service
    {
        //Thêm
        public int Add_Cate(CATEGORIES cate)
        {
            if (cate == null)
                return 1;
            using (QuanLyQuanAoEntities db = new QuanLyQuanAoEntities())
            {
                if (db.CATEGORIES.Any(n => n.TenLoaiHang.Equals(cate.TenLoaiHang, StringComparison.OrdinalIgnoreCase)))
                    return 2;
                db.CATEGORIES.Add(cate);
                db.SaveChanges();
                return 0;
            }
        }
        //Cập nhật
        public int Update_Cate(CATEGORIES cate)
        {
            if (cate == null)
                return 1;
            using (QuanLyQuanAoEntities db = new QuanLyQuanAoEntities())
            {
                var cate_Update = db.CATEGORIES.FirstOrDefault(n => n.TenLoaiHang.Equals(cate.TenLoaiHang, StringComparison.OrdinalIgnoreCase));

                if (cate_Update == null)
                {
                    db.CATEGORIES.Add(cate_Update);
                    return 0;
                }
                else
                {
                    cate_Update.MaLoaiHang = cate.MaLoaiHang;

                    db.Entry(cate_Update).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return 0;
                }
            }
        }
        //Xóa
        public int Delete_Cate(String group)
        {
            if (String.IsNullOrEmpty(group))
                return 1;
            using (QuanLyQuanAoEntities db = new QuanLyQuanAoEntities())
            {
                var cate_Delete = db.CATEGORIES.FirstOrDefault(n => n.TenLoaiHang.Equals(group));
                if (cate_Delete != null)
                {
                    db.CATEGORIES.Remove(cate_Delete);
                    db.SaveChanges();
                }
                return 0;
            }
        }
        //TruyVấn
        public List<CATEGORIES> Query_Account_All()
        {
            using (QuanLyQuanAoEntities db = new QuanLyQuanAoEntities())
            {
                return db.CATEGORIES.ToList();
            }
        }
    }
}
