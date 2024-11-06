using QLQA.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace QLQA
{
    public partial class fTaikhoan : Form
    {
        private QuanLyQuanAoEntities dbContext = new QuanLyQuanAoEntities();
        public fTaikhoan()
        {
            InitializeComponent();

            // Thiết lập các cột cho ListView
            listViewTK.Columns.Add("Username", 100, HorizontalAlignment.Left);
            listViewTK.Columns.Add("Password", 100, HorizontalAlignment.Left);
            listViewTK.Columns.Add("Mã NV", 100, HorizontalAlignment.Left);

            // Thiết lập chế độ hiển thị chi tiết (dạng bảng)
            listViewTK.View = View.Details;
            listViewTK.FullRowSelect = true;
            LoadDataFromDatabase();
        }
        // Hàm để tải dữ liệu từ cơ sở dữ liệu
        private void LoadDataFromDatabase()
        {
            dbContext = new QuanLyQuanAoEntities();
            var accounts = dbContext.ACCOUNT.ToList();

            listViewTK.Items.Clear();
            foreach (var account in accounts)
            {
                AddAccountToListView(account);
            }
        }
        // Hàm để thêm một tài khoản vào ListView
        private void AddAccountToListView(ACCOUNT account)
        {
            ListViewItem item = new ListViewItem(account.Username);
            item.SubItems.Add(account.Password);
            item.SubItems.Add(account.MaNV);
            listViewTK.Items.Add(item);
        }

        private void BindListView(List<ACCOUNT> accounts)
        {
            listViewTK.Items.Clear();
            foreach (var account in accounts)
            {
                var item = new ListViewItem(account.Username);
                item.SubItems.Add(account.Password);
                item.SubItems.Add(account.MaNV);
                listViewTK.Items.Add(item);
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(textBoxMaNV.Text) || string.IsNullOrWhiteSpace(textBoxUsername.Text) || string.IsNullOrWhiteSpace(textBoxPassword.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.");
                return false;
            }
            return true;
        }

        private void ClearInputFields()
        {
            textBoxMaNV.Clear();
            textBoxUsername.Clear();
            textBoxPassword.Clear();
            textBoxTenNV.Clear();

        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewTK.SelectedItems.Count == 0) return;

            var selectedItem = listViewTK.SelectedItems[0];
            textBoxUsername.Text = selectedItem.Text;
            textBoxPassword.Text = selectedItem.SubItems[1].Text;
            textBoxMaNV.Text = selectedItem.SubItems[2].Text;
        }

        private void btn_Them_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem người dùng đã nhập đủ thông tin chưa
            if (string.IsNullOrEmpty(textBoxMaNV.Text) || string.IsNullOrEmpty(textBoxUsername.Text) || string.IsNullOrEmpty(textBoxPassword.Text))
            {
                MessageBox.Show("Bạn chưa nhập đầy đủ thông tin!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra xem Username có tồn tại trong bảng ACCOUNT không
            var existingAccount = dbContext.ACCOUNT.FirstOrDefault(a => a.Username == textBoxUsername.Text);
            if (existingAccount != null)
            {
                MessageBox.Show("Tài khoản này đã tồn tại!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Nếu MaNV hợp lệ và Username chưa tồn tại, tiếp tục thêm tài khoản vào bảng ACCOUNT
                ACCOUNT newAccount = new ACCOUNT
                {
                    Username = textBoxUsername.Text,
                    Password = textBoxPassword.Text, // Use hashing here for better security
                    MaNV = textBoxMaNV.Text
                };

                dbContext.ACCOUNT.Add(newAccount);
                dbContext.SaveChanges();

                // Tạo mới một ListViewItem
                ListViewItem lstvItem = new ListViewItem
                {
                    Text = textBoxUsername.Text
                };
                lstvItem.SubItems.Add(textBoxPassword.Text); // Consider not displaying the password
                lstvItem.SubItems.Add(textBoxMaNV.Text);
                listViewTK.Items.Add(lstvItem);

                // Hiển thị thông báo thêm thành công
                MessageBox.Show("Bạn đã thêm tài khoản thành công", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Xóa dữ liệu trong các ô nhập sau khi thêm xong
                textBoxMaNV.Clear();
                textBoxUsername.Clear();
                textBoxPassword.Clear();
            }
            catch (Exception ex)
            {
                // Log or show the exception message if there is an error
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btn_Xoa_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có mục nào được chọn trong ListView không
            if (listViewTK.SelectedItems.Count > 0)
            {
                // Lấy mục được chọn
                ListViewItem selectedItem = listViewTK.SelectedItems[0];

                // Tìm tài khoản trong cơ sở dữ liệu và xóa
                var accountToDelete = dbContext.ACCOUNT.FirstOrDefault(a => a.Username == selectedItem.Text);
                if (accountToDelete != null)
                {
                    dbContext.ACCOUNT.Remove(accountToDelete);
                    dbContext.SaveChanges();  // Lưu thay đổi vào cơ sở dữ liệu

                    // Xóa mục trong ListView
                    listViewTK.Items.Remove(selectedItem);

                    MessageBox.Show("Tài khoản đã được xóa thành công", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Không tìm thấy tài khoản", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn tài khoản cần xóa", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btn_Sua_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có mục nào được chọn trong ListView không
            if (listViewTK.SelectedItems.Count > 0)
            {
                // Lấy mục được chọn
                ListViewItem selectedItem = listViewTK.SelectedItems[0];

                // Cập nhật các ô nhập liệu với thông tin từ mục đã chọn
                textBoxUsername.Text = selectedItem.Text;
                textBoxPassword.Text = selectedItem.SubItems[1].Text;
                textBoxMaNV.Text = selectedItem.SubItems[2].Text;

                // Sau khi chỉnh sửa, người dùng bấm nút "Lưu" để cập nhật lại vào ListView
            }
            else
            {
                MessageBox.Show("Vui lòng chọn tài khoản cần chỉnh sửa", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btn_LuuSua_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có mục nào được chọn trong ListView không
            if (listViewTK.SelectedItems.Count > 0)
            {
                // Lấy mục được chọn
                ListViewItem selectedItem = listViewTK.SelectedItems[0];

                // Tìm tài khoản trong cơ sở dữ liệu và cập nhật thông tin
                var account = dbContext.ACCOUNT.FirstOrDefault(a => a.Username == selectedItem.Text);
                if (account != null)
                {
                    account.Username = textBoxUsername.Text;
                    account.Password = textBoxPassword.Text;
                    account.MaNV = textBoxMaNV.Text;

                    dbContext.SaveChanges();  // Lưu thay đổi vào cơ sở dữ liệu

                    // Cập nhật lại thông tin trong ListView
                    selectedItem.Text = textBoxUsername.Text;
                    selectedItem.SubItems[1].Text = textBoxPassword.Text;
                    selectedItem.SubItems[2].Text = textBoxMaNV.Text;

                    MessageBox.Show("Tài khoản đã được chỉnh sửa thành công", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Xóa dữ liệu trong các ô nhập sau khi cập nhật xong
                    textBoxMaNV.Clear();
                    textBoxUsername.Clear();
                    textBoxPassword.Clear();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy tài khoản", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Chưa chọn tài khoản để chỉnh sửa", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }



    }
}
