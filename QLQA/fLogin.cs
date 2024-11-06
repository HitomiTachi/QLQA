using QLQA.DAL;
using QLQA.BLL;
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
    public partial class fLogin : Form
    {
        public fLogin()
        {
            InitializeComponent();
            comboBoxRole.Items.Add("Quản lý");
            comboBoxRole.Items.Add("Nhân viên");
            comboBoxRole.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnlogin_Click(object sender, EventArgs e)
        {
            string username = textBoxUser.Text.Trim();
            string password = textBoxPass.Text.Trim();
            string selectedRole = comboBoxRole.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(selectedRole))
            {
                MessageBox.Show("Tài khoản, mật khẩu và loại tài khoản không được để trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ACCOUNT_Service accountService = new ACCOUNT_Service();
            var taikhoan = accountService.Query_Account(username);

            // Kiểm tra nếu tài khoản không tồn tại
            if (taikhoan == null)
            {
                MessageBox.Show("Tài khoản không tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.btnlogin.ForeColor = Color.IndianRed;
                return;
            }

            // Kiểm tra mật khẩu
            if (taikhoan.Password != password)
            {
                MessageBox.Show("Sai mật khẩu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.btnlogin.ForeColor = Color.IndianRed;
                return;
            }

            // Kiểm tra loại tài khoản dựa trên lựa chọn của người dùng
            bool isManager = selectedRole == "Quản lý";
            if (taikhoan.Account_Type != isManager)
            {
                MessageBox.Show($"Tài khoản không phải là {selectedRole}!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.btnlogin.ForeColor = Color.IndianRed;
                return;
            }

            // Đăng nhập thành công, chuyển đến form tiếp theo
            this.btnlogin.ForeColor = Color.Black; // Đặt lại màu mặc định nếu đăng nhập thành công
                                                   // Trong btnlogin_Click ở fLogin
            fTableManager f = new fTableManager(taikhoan.Account_Type);
            this.Hide();
            f.ShowDialog();
            this.Show();
        }


        private void fLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Bạn có thật sự muốn thoát ?", "Thông báo", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
            {
                e.Cancel = true;
            }
        }

        

        private void btnlogin_MouseMove(object sender, MouseEventArgs e)
        {
            this.btnlogin.ForeColor = Color.Black;
        }

        private void btnlogin_MouseLeave(object sender, EventArgs e)
        {
            this.btnlogin.ForeColor = Color.Black;
        }

        private void textBoxPass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnlogin.PerformClick();
            }
        }

        private void textBoxUser_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnlogin.PerformClick();
            }
        }
    }
}
