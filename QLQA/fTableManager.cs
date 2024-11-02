using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLQA
{
    public partial class fTableManager : Form
    {
        private bool Account_Type; // Biến thành viên để lưu loại tài khoản

        public fTableManager(bool isManager) // Thay đổi tham số để nhận kiểu bool
        {
            InitializeComponent();
            Account_Type = isManager; // Gán giá trị
            RestrictAccessBasedOnAccountType();
        }

        private Form currentFormChild;

        private void OpenChildForm(Form childForm)
        {
            if (currentFormChild != null)
            {
                currentFormChild.Close();
            }
            currentFormChild = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            bodypanel.Controls.Add(childForm);
            bodypanel.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }

        private void RestrictAccessBasedOnAccountType()
        {
            if (!Account_Type) // Nếu không phải là quản lý
            {
                // Vô hiệu hóa các nút thêm, sửa, xóa nhân viên
                btn_sanpham.Enabled = true; // Giữ nguyên nếu nhân viên vẫn có thể truy cập sản phẩm
                btn_taikhoan.Enabled = true; // Giữ nguyên nếu nhân viên vẫn có thể truy cập tài khoản
            }
            else // Nếu là quản lý
            {
                // Quản lý có thể thực hiện tất cả các chức năng
            }
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void fsanpham_Click(object sender, EventArgs e)
        {
            OpenChildForm(new fSanpham());
            lbl_home.Text = btn_sanpham.Text;
        }

        private void fhoadon_Click(object sender, EventArgs e)
        {
            OpenChildForm(new fHoadon() );
            lbl_home.Text = fhoadon.Text;
        }

        private void fnhanvien_Click(object sender, EventArgs e)
        {
            OpenChildForm(new fNhanVien(Account_Type)); // Truyền thông tin loại tài khoản
            lbl_home.Text = fnhanvien.Text;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenChildForm(new fTaikhoan());
            lbl_home.Text = btn_taikhoan.Text;
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (currentFormChild != null)
            {
                currentFormChild.Close();
            }
            lbl_home.Text = "Home";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
