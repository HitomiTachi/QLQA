using QLQA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLQA.BLL
{
    public class ACCOUNT_Service
    {
        //Thêm
        public int Add_Account(ACCOUNT account)
        {
            if (account == null)
                return 1;
            using (QuanLyQuanAoEntities db = new QuanLyQuanAoEntities())
            {
                if (db.ACCOUNT.Any(n => n.Username.Equals(account.Username, StringComparison.OrdinalIgnoreCase)))
                    return 2;
                db.ACCOUNT.Add(account);
                db.SaveChanges();
                return 0;
            }
        }
        //Cập nhật
        public int Update_Account(ACCOUNT account)
        {
            if (account == null)
                return 1;
            using (QuanLyQuanAoEntities db = new QuanLyQuanAoEntities())
            {
                var account_Update = db.ACCOUNT.FirstOrDefault(n => n.Username.Equals(account.Username, StringComparison.OrdinalIgnoreCase));

                if (account_Update == null)
                {
                    db.ACCOUNT.Add(account_Update);
                    return 0;
                }
                else
                {
                    account_Update.Password = account.Password;
                    account_Update.Account_Type = account.Account_Type;
                    account_Update.MaNV = account.MaNV;

                    db.Entry(account_Update).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return 0;
                }
            }
        }
        //Xóa
        public int Delete_Account(String user)
        {
            if (String.IsNullOrEmpty(user))
                return 1;
            using (QuanLyQuanAoEntities db = new QuanLyQuanAoEntities())
            {
                var account_Delete = db.ACCOUNT.FirstOrDefault(n => n.Username.Equals(user));
                if (account_Delete != null)
                {
                    db.ACCOUNT.Remove(account_Delete);
                    db.SaveChanges();
                }
                return 0;
            }
        }
        //TruyVấn
        public List<ACCOUNT> Query_Account_All()
        {
            using (QuanLyQuanAoEntities db = new QuanLyQuanAoEntities())
            {
                return db.ACCOUNT.ToList();
            }
        }

        public ACCOUNT Query_Account(string username)
        {
            using (QuanLyQuanAoEntities db = new QuanLyQuanAoEntities())
            {
                // Tìm tài khoản theo username
                return db.ACCOUNT.FirstOrDefault(a => a.Username == username);
            }
        }


        public EMPLOYERS GetEmployeeByUsername(string username)
        {
            // Logic để truy vấn tài khoản từ cơ sở dữ liệu dựa trên tên đăng nhập
            using (QuanLyQuanAoEntities db = new QuanLyQuanAoEntities())
            {
                return db.EMPLOYERS.FirstOrDefault(e => e.MaNV == username);
            }
        }

    }
}
