using QLQA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLQA.BLL
{
    internal class EMPLOYERS_Service
    {
        //Thêm
        public int Add_Employer(EMPLOYERS Employer)
        {
            if (Employer == null)
                return 1;
            using (QuanLyQuanAoEntities db = new QuanLyQuanAoEntities())
            {
                if (db.EMPLOYERS.Any(n => n.TenNV.Equals(Employer.TenNV, StringComparison.OrdinalIgnoreCase)))
                    return 2;
                db.EMPLOYERS.Add(Employer);
                db.SaveChanges();
                return 0;
            }
        }
        //Cập nhật
        public int Update_Employer(EMPLOYERS Employer)
        {
            if (Employer == null)
                return 1;
            using (QuanLyQuanAoEntities db = new QuanLyQuanAoEntities())
            {
                var Employer_Update = db.EMPLOYERS.FirstOrDefault(n => n.TenNV.Equals(Employer.TenNV, StringComparison.OrdinalIgnoreCase));

                if (Employer_Update == null)
                {
                    db.EMPLOYERS.Add(Employer_Update);
                    return 0;
                }
                else
                {
                    Employer_Update.SoDienThoai = Employer.SoDienThoai;
                    Employer_Update.Anh = Employer.Anh;
                    Employer_Update.MaNV = Employer.MaNV;
                    Employer_Update.GioiTinh = Employer.GioiTinh;
                    Employer_Update.NgaySinh = Employer.NgaySinh;

                    db.Entry(Employer_Update).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return 0;
                }
            }
        }
        //Xóa
        public int Delete_Employer(String labor)
        {
            if (String.IsNullOrEmpty(labor))
                return 1;
            using (QuanLyQuanAoEntities db = new QuanLyQuanAoEntities())
            {
                var Employer_Delete = db.EMPLOYERS.FirstOrDefault(n => n.TenNV.Equals(labor));
                if (Employer_Delete != null)
                {
                    db.EMPLOYERS.Remove(Employer_Delete);
                    db.SaveChanges();
                }
                return 0;
            }
        }
        //TruyVấn
        public List<EMPLOYERS> Query_Account_All()
        {
            using (QuanLyQuanAoEntities db = new QuanLyQuanAoEntities())
            {
                return db.EMPLOYERS.ToList();
            }
        }
    }
}
