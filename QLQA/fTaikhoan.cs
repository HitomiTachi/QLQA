using QLQA.DAL;
using System;
using System.Linq;
using System.Windows.Forms;

namespace QLQA
{
    public partial class fTaikhoan : Form
    {
        private QuanLyQuanAoEntities dbContext = new QuanLyQuanAoEntities();

        public fTaikhoan()
        {
            InitializeComponent();
            InitializeListView();
            LoadDataFromDatabase();
        }

        // Khởi tạo các cột cho ListView
        private void InitializeListView()
        {
            listViewTK.Columns.Add("Username", 100, HorizontalAlignment.Left);
            listViewTK.Columns.Add("Password", 100, HorizontalAlignment.Left);
            listViewTK.Columns.Add("Mã NV", 100, HorizontalAlignment.Left);
            listViewTK.Columns.Add("Tên hiển thị", 150, HorizontalAlignment.Left);
            listViewTK.View = View.Details;
            listViewTK.FullRowSelect = true;
        }

        // Tải dữ liệu từ cơ sở dữ liệu vào ListView
        private void LoadDataFromDatabase()
        {
            try
            {
                var accounts = dbContext.ACCOUNT.ToList();
                listViewTK.Items.Clear();

                foreach (var account in accounts)
                {
                    AddAccountToListView(account);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu tài khoản: " + ex.Message);
            }
        }

        // Thêm một tài khoản vào ListView
        private void AddAccountToListView(ACCOUNT account)
        {
            var employee = dbContext.EMPLOYERS.FirstOrDefault(e => e.MaNV == account.MaNV);
            string employeeName = employee != null ? employee.TenNV : "Chưa có tên";

            var item = new ListViewItem(account.Username)
            {
                SubItems = { account.Password, account.MaNV, employeeName }
            };
            listViewTK.Items.Add(item);
        }

        // Xử lý khi người dùng chọn một mục trong ListView
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewTK.SelectedItems.Count == 0) return;

            var selectedItem = listViewTK.SelectedItems[0];
            textBoxUsername.Text = selectedItem.Text;
            textBoxPassword.Text = selectedItem.SubItems[1].Text;
            textBoxMaNV.Text = selectedItem.SubItems[2].Text;
            textBoxTenNV.Text = selectedItem.SubItems[3].Text;
        }


        // Xóa tài khoản dựa trên mã nhân viên
        private void btn_Xoa_Click(object sender, EventArgs e)
        {
            try
            {
                var maNV = textBoxMaNV.Text;
                var account = dbContext.ACCOUNT.FirstOrDefault(acc => acc.MaNV == maNV);

                if (account != null)
                {
                    dbContext.ACCOUNT.Remove(account);
                    dbContext.SaveChanges();
                    LoadDataFromDatabase();
                    MessageBox.Show("Xóa tài khoản thành công!");
                }
                else
                {
                    MessageBox.Show("Không tìm thấy tài khoản để xóa.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa tài khoản: " + ex.Message);
            }
        }

        // Cập nhật tài khoản
        private void btn_Sua_Click(object sender, EventArgs e)
        {
            try
            {
                string username = textBoxUsername.Text;
                string password = textBoxPassword.Text;
                string maNV = textBoxMaNV.Text;
                string tenNV = textBoxTenNV.Text;

                // Find the account by MaNV or Username (you can use one depending on the flow)
                var account = dbContext.ACCOUNT.FirstOrDefault(acc => acc.MaNV == maNV || acc.Username == username);

                if (account != null)
                {
                    // Update account details
                    account.Username = username;
                    account.Password = password;
                    account.MaNV = maNV;

                    // Update employee details (if applicable)
                    var employee = dbContext.EMPLOYERS.FirstOrDefault(emt => emt.MaNV == maNV);
                    if (employee != null)
                    {
                        employee.TenNV = tenNV;
                    }

                    // Save changes to database
                    dbContext.SaveChanges();

                    // Update UI to reflect changes
                    if (listViewTK.SelectedItems.Count > 0)
                    {
                        ListViewItem selectedItem = listViewTK.SelectedItems[0];
                        selectedItem.Text = username;
                        selectedItem.SubItems[1].Text = password;
                        selectedItem.SubItems[2].Text = maNV;
                        selectedItem.SubItems[3].Text = tenNV;
                    }

                    MessageBox.Show("Tài khoản đã được cập nhật thành công.", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearInputFields();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy tài khoản với mã nhân viên hoặc tên đăng nhập đã nhập.", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật tài khoản: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Lưu thông tin tài khoản đã chỉnh sửa vào cơ sở dữ liệu
        private void btn_LuuSua_Click(object sender, EventArgs e)
        {
            if (listViewTK.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = listViewTK.SelectedItems[0];
                var account = dbContext.ACCOUNT.FirstOrDefault(a => a.Username == selectedItem.Text);

                if (account != null)
                {
                    account.Username = textBoxUsername.Text;
                    account.Password = textBoxPassword.Text;
                    account.MaNV = textBoxMaNV.Text;

                    // Cập nhật tên nhân viên nếu có
                    var employee = dbContext.EMPLOYERS.FirstOrDefault(emt => emt.MaNV == account.MaNV);
                    if (employee != null)
                    {
                        employee.TenNV = textBoxTenNV.Text;
                    }

                    dbContext.SaveChanges();

                    // Cập nhật ListView với thông tin mới
                    selectedItem.Text = textBoxUsername.Text;
                    selectedItem.SubItems[1].Text = textBoxPassword.Text;
                    selectedItem.SubItems[2].Text = textBoxMaNV.Text;
                    selectedItem.SubItems[3].Text = textBoxTenNV.Text;

                    MessageBox.Show("Tài khoản đã được chỉnh sửa thành công", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Xóa các trường nhập liệu sau khi lưu
                    ClearInputFields();
                }
            }
            else
            {
                MessageBox.Show("Chưa chọn tài khoản để chỉnh sửa", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Kiểm tra tính hợp lệ của dữ liệu nhập vào
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(textBoxMaNV.Text) || string.IsNullOrWhiteSpace(textBoxUsername.Text) || string.IsNullOrWhiteSpace(textBoxPassword.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.");
                return false;
            }
            return true;
        }

        // Xóa các trường nhập liệu
        private void ClearInputFields()
        {
            textBoxMaNV.Clear();
            textBoxUsername.Clear();
            textBoxPassword.Clear();
            textBoxTenNV.Clear();
        }
    }
}
