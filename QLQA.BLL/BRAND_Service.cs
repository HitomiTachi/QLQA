using QLQA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLQA.BLL
{
    internal class BRAND_Service
    {
        //Thêm
        public int Add_BRAND(BRAND brand)
        {
            if (brand == null)
                return 1;
            using (QuanLyQuanAoEntities db = new QuanLyQuanAoEntities())
            {
                if (db.BRAND.Any(n => n.TenBrand.Equals(brand.TenBrand, StringComparison.OrdinalIgnoreCase)))
                    return 2;
                db.BRAND.Add(brand);
                db.SaveChanges();
                return 0;
            }
        }
        //Cập nhật
        public int Update_BRAND(BRAND brand)
        {
            if (brand == null)
                return 1;
            using (QuanLyQuanAoEntities db = new QuanLyQuanAoEntities())
            {
                var brand_Update = db.BRAND.FirstOrDefault(n => n.TenBrand.Equals(brand.TenBrand, StringComparison.OrdinalIgnoreCase));

                if (brand_Update == null)
                {
                    db.BRAND.Add(brand_Update);
                    return 0;
                }
                else
                {
                    brand_Update.MaBrand = brand.MaBrand;

                    db.Entry(brand_Update).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return 0;
                }
            }
        }
        //TruyVấn_All
        public List<BRAND> Query_BRAND_All()
        {
            using (QuanLyQuanAoEntities db = new QuanLyQuanAoEntities())
            {
                return db.BRAND.ToList();
            }
        }
        //// Tìm kiếm và lọc sản phẩm theo thương hiệu
        //public List<PRODUCTS> GetProducts_Brand(string brandName)
        //{
        //    using (QuanLyQuanAoEntities db = new QuanLyQuanAoEntities())
        //    {
        //        // Tìm thương hiệu dựa trên tên thương hiệu
        //        if (String.IsNullOrEmpty(brandName))
        //            return null;
        //        using (QuanLyQuanAoEntities db = new QuanLyQuanAoEntities())
        //        {
        //            return db.BRAND.FirstOrDefault(n => n.TenBrand.Equals(brandName, StringComparison.OrdinalIgnoreCase));
        //        }


        //        // Lấy tất cả sản phẩm theo thương hiệu đó
        //        var productsByBrand = db.PRODUCTS.Where(p => p.MaBrand == brand.MaBrand).ToList();
        //        return productsByBrand;
        //    }
        //}
    }
}
