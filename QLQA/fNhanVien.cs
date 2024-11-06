using QLQA.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace QLQA
{
    public partial class fNhanVien : Form
    {
        private bool Account_Type; // Biến thành viên để lưu loại tài khoản
        private List<EMPLOYERS> listNhanVien;


        public fNhanVien(bool isManager)  // Thay đổi tham số để nhận kiểu bool
        {
            InitializeComponent();
            Account_Type = isManager; // Gán giá trị
            RestrictAccessBasedOnAccountType();
            listNhanVien = new List<EMPLOYERS>();
            LoadEmployeeDataFromDatabase();
        }

        private void RestrictAccessBasedOnAccountType()
        {
            if (!Account_Type) // Nếu không phải là quản lý
            {
                // Vô hiệu hóa các nút thêm, sửa, xóa nhân viên
                btn_Them.Enabled = false;
                btn_Sua.Enabled = false;
                btn_Xoa.Enabled = false;
                btn_Anh.Enabled = false;
            }
        }

        private void LoadEmployeeDataFromDatabase()
        {
            try
            {
                using (var db = new QuanLyQuanAoEntities())
                {
                    listNhanVien = db.EMPLOYERS.ToList();
                }
                BindGrid(listNhanVien);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu nhân viên: " + ex.Message);
            }
        }

        private void BindGrid(List<EMPLOYERS> employees)
        {
            dataGridViewNV.Rows.Clear();
            using (var db = new QuanLyQuanAoEntities())
            {
                foreach (var item in employees)
                {
                    var account = db.ACCOUNT.FirstOrDefault(acc => acc.MaNV == item.MaNV);
                    string accountType = account?.Account_Type == true ? "Quản lý" : "Nhân viên";

                    int index = dataGridViewNV.Rows.Add();
                    dataGridViewNV.Rows[index].Cells[0].Value = item.MaNV;
                    dataGridViewNV.Rows[index].Cells[1].Value = item.TenNV;
                    dataGridViewNV.Rows[index].Cells[2].Value = item.GioiTinh ? "Nam" : "Nữ";
                    dataGridViewNV.Rows[index].Cells[3].Value = item.NgaySinh.ToString("dd/MM/yyyy");
                    dataGridViewNV.Rows[index].Cells[4].Value = item.SoDienThoai;
                    dataGridViewNV.Rows[index].Cells[5].Value = accountType;
                    dataGridViewNV.Rows[index].Cells[6].Value = item.Anh; // Hiển thị đường dẫn ảnh



                }
            }
        }




        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(textBox_MaNV.Text) ||
                string.IsNullOrWhiteSpace(textBox_TenNV.Text) ||
                string.IsNullOrWhiteSpace(maskedTextBox_SDT.Text))
            {
                MessageBox.Show("Cần nhập đầy đủ thông tin");
                return false;
            }

            if (!radioButtonNam.Checked && !radioButtonNu.Checked)
            {
                MessageBox.Show("Vui lòng chọn giới tính: Nam hoặc Nữ.");
                return false;
            }

            if (dtpNgaysinh.Value > DateTime.Now)
            {
                MessageBox.Show("Ngày sinh không thể là ngày trong tương lai.");
                return false;
            }

            if (textBox_TenNV.Text.Length > 50)
            {
                MessageBox.Show("Tên nhân viên không được vượt quá 50 ký tự.");
                return false;
            }

            if (maskedTextBox_SDT.Text.Length > 15)
            {
                MessageBox.Show("Số điện thoại không được vượt quá 15 ký tự.");
                return false;
            }
            if (!radioButtonQL.Checked && !radioButtonNV.Checked)
            {
                MessageBox.Show("Vui lòng chọn kiểu tài khoản: Quản lý hoặc Nhân viên.");
                return false;
            }
            return true;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void maskedTextBox2_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private EMPLOYERS CreateEmployeeFromInput()
        {
            return new EMPLOYERS
            {
                MaNV = textBox_MaNV.Text,
                TenNV = textBox_TenNV.Text,
                GioiTinh = radioButtonNam.Checked,
                NgaySinh = dtpNgaysinh.Value,
                SoDienThoai = maskedTextBox_SDT.Text,
                Anh = currentEmployer.Anh // Lưu thuộc tính Anh vào model EMPLOYERS
            };
        }

        private void btn_Them_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                EMPLOYERS newEmployee = CreateEmployeeFromInput();

                using (var db = new QuanLyQuanAoEntities())
                {
                    // Kiểm tra trùng mã nhân viên
                    if (db.EMPLOYERS.Any(emp => emp.MaNV == newEmployee.MaNV))
                    {
                        MessageBox.Show("Đã tồn tại. Vui lòng nhập mã khác.");
                        return;
                    }

                    db.EMPLOYERS.Add(newEmployee);

                    try
                    {
                        db.SaveChanges();
                        MessageBox.Show("Thêm nhân viên thành công!");
                        LoadEmployeeDataFromDatabase();
                        ResetInputFields();
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                    {
                        DisplayValidationErrors(ex);
                    }
                }
            }
        }




        private void btn_Sua_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                string maNV = textBox_MaNV.Text;
                using (var db = new QuanLyQuanAoEntities())
                {
                    EMPLOYERS employeeToUpdate = db.EMPLOYERS.FirstOrDefault(emp => emp.MaNV == maNV);

                    if (employeeToUpdate != null)
                    {
                        employeeToUpdate.TenNV = textBox_TenNV.Text;
                        employeeToUpdate.GioiTinh = radioButtonNam.Checked;
                        employeeToUpdate.NgaySinh = dtpNgaysinh.Value;
                        employeeToUpdate.SoDienThoai = maskedTextBox_SDT.Text;
                        employeeToUpdate.Anh = pictureBoxAnh.Text; // Cập nhật đường dẫn ảnh
                        // Cập nhật kiểu tài khoản
                        ACCOUNT accountToUpdate = db.ACCOUNT.FirstOrDefault(acc => acc.MaNV == employeeToUpdate.MaNV);
                        if (accountToUpdate != null)
                        {
                            accountToUpdate.Account_Type = radioButtonQL.Checked; // Cập nhật kiểu tài khoản
                        }
                        // Cập nhật đường dẫn ảnh nếu đã chọn ảnh mới
                        if (!string.IsNullOrEmpty(currentEmployer.Anh))
                        {
                            employeeToUpdate.Anh = currentEmployer.Anh;
                        }

                        try
                        {
                            db.SaveChanges();
                            MessageBox.Show("Cập nhật nhân viên thành công!");
                            LoadEmployeeDataFromDatabase();
                            ResetInputFields();
                        }
                        catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                        {
                            DisplayValidationErrors(ex);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy nhân viên để cập nhật.");
                    }

                }
            }
        }


        private void btn_Xoa_Click(object sender, EventArgs e)
        {
            if (dataGridViewNV.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một nhân viên để xóa.");
                return;
            }

            var confirmResult = MessageBox.Show("Bạn có chắc chắn muốn xóa nhân viên đã chọn?", "Xác nhận xóa", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                using (var db = new QuanLyQuanAoEntities())
                {
                    foreach (DataGridViewRow row in dataGridViewNV.SelectedRows)
                    {
                        string maNV = row.Cells["MaNV"].Value?.ToString();
                        if (!string.IsNullOrEmpty(maNV))
                        {
                            var employeeToDelete = db.EMPLOYERS.FirstOrDefault(emp => emp.MaNV == maNV);
                            if (employeeToDelete != null)
                            {
                                // Xóa tài khoản liên quan
                                var accountToDelete = db.ACCOUNT.FirstOrDefault(acc => acc.MaNV == maNV);
                                if (accountToDelete != null)
                                {
                                    db.ACCOUNT.Remove(accountToDelete);
                                }

                                db.EMPLOYERS.Remove(employeeToDelete);
                            }
                        }
                    }

                    try
                    {
                        db.SaveChanges();
                        MessageBox.Show("Xóa nhân viên và tài khoản thành công!");
                        LoadEmployeeDataFromDatabase();
                        ResetInputFields();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Có lỗi xảy ra khi xóa nhân viên: " + ex.Message);
                    }
                }
            }
        }




        private void dataGridViewNV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewNV.Rows[e.RowIndex];

                // Điền thông tin vào các TextBox
                textBox_MaNV.Text = row.Cells["MaNV"].Value?.ToString() ?? "";
                textBox_TenNV.Text = row.Cells["TenNV"].Value?.ToString() ?? "";

                // Điền giới tính
                string gioiTinh = row.Cells["Sex"].Value?.ToString();
                radioButtonNam.Checked = gioiTinh == "Nam";
                radioButtonNu.Checked = gioiTinh == "Nữ";

                // Điền ngày sinh, kiểm tra dữ liệu null hoặc không hợp lệ
                if (row.Cells["DateBirth"].Value != null && DateTime.TryParse(row.Cells["DateBirth"].Value.ToString(), out DateTime ngaySinh))
                {
                    dtpNgaysinh.Value = ngaySinh;
                }
                else
                {
                    dtpNgaysinh.Value = DateTime.Now; // Gán giá trị mặc định nếu không có ngày sinh
                }

                // Điền số điện thoại
                maskedTextBox_SDT.Text = row.Cells["Number"].Value?.ToString() ?? "";

                // Lấy Account_Type từ tài khoản liên quan
                using (var db = new QuanLyQuanAoEntities())
                {
                    var maNV = textBox_MaNV.Text;
                    var account = db.ACCOUNT.FirstOrDefault(acc => acc.MaNV == maNV);
                    if (account != null)
                    {
                        radioButtonQL.Checked = account.Account_Type;
                        radioButtonNV.Checked = !account.Account_Type;
                    }
                    else
                    {
                        radioButtonQL.Checked = false;
                        radioButtonNV.Checked = false;
                    }
                }

                // Hiển thị ảnh nếu có
                string imagePath = row.Cells["Pic"].Value?.ToString(); // Lấy đường dẫn ảnh từ cột "Anh"
                if (!string.IsNullOrEmpty(imagePath))
                {
                    try
                    {
                        pictureBoxAnh.Image = Image.FromFile(imagePath); // Hiển thị ảnh trong PictureBox
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Không thể tải ảnh: {ex.Message}");
                        pictureBoxAnh.Image = null; // Đặt ảnh mặc định nếu không thể tải ảnh
                    }
                }
                else
                {
                    pictureBoxAnh.Image = null; // Nếu không có ảnh, để PictureBox trống hoặc hiển thị ảnh mặc định
                }
            }
        }



        private void btn_TimKiem_Click(object sender, EventArgs e)
        {
            string maNVKeyword = textBox_MaNV.Text.Trim();
            string tenNVKeyword = textBox_TenNV.Text.Trim();
            string sdtKeyword = maskedTextBox_SDT.Text.Trim();

            // Vô hiệu hóa các trường nhập liệu
            radioButtonNam.Enabled = false;
            radioButtonNu.Enabled = false;
            dtpNgaysinh.Enabled = false;
            radioButtonQL.Enabled = false;
            radioButtonNV.Enabled = false;
            btn_Them.Enabled = false;
            btn_Sua.Enabled = false;
            btn_Xoa.Enabled = false;

            // Kiểm tra nếu tất cả các trường đều trống
            if (string.IsNullOrWhiteSpace(maNVKeyword) && string.IsNullOrWhiteSpace(tenNVKeyword) && string.IsNullOrWhiteSpace(sdtKeyword))
            {
                MessageBox.Show("Vui lòng nhập mã nhân viên, tên nhân viên hoặc số điện thoại để tìm kiếm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Thực hiện tìm kiếm
            SearchEmployee(maNVKeyword, tenNVKeyword, sdtKeyword);
        }

        private void SearchEmployee(string maNVKeyword, string tenNVKeyword, string sdtKeyword)
        {
            using (var db = new QuanLyQuanAoEntities())
            {
                // Tìm kiếm nhân viên theo mã nhân viên, tên nhân viên và số điện thoại
                var filteredEmployees = db.EMPLOYERS.AsQueryable();

                if (!string.IsNullOrWhiteSpace(maNVKeyword))
                {
                    filteredEmployees = filteredEmployees.Where(emp => emp.MaNV.Contains(maNVKeyword));
                }

                if (!string.IsNullOrWhiteSpace(tenNVKeyword))
                {
                    filteredEmployees = filteredEmployees.Where(emp => emp.TenNV.Contains(tenNVKeyword));
                }

                if (!string.IsNullOrWhiteSpace(sdtKeyword))
                {
                    filteredEmployees = filteredEmployees.Where(emp => emp.SoDienThoai.Contains(sdtKeyword));
                }

                var results = filteredEmployees.ToList();

                if (results.Count > 0)
                {
                    // Hiển thị thông tin của nhân viên đầu tiên tìm thấy
                    ShowEmployeeInfo(results.First());
                }
                else
                {
                    MessageBox.Show("Không tìm thấy nhân viên nào phù hợp.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }


        private void ShowEmployeeInfo(EMPLOYERS employee)
        {
            if (employee == null) return;

            StringBuilder info = new StringBuilder();
            info.AppendLine($"Mã NV: {employee.MaNV}");
            info.AppendLine($"Tên NV: {employee.TenNV}");
            info.AppendLine($"Giới Tính: {(employee.GioiTinh ? "Nam" : "Nữ")}");
            info.AppendLine($"Ngày Sinh: {employee.NgaySinh.ToString("dd/MM/yyyy")}");
            info.AppendLine($"Số Điện Thoại: {employee.SoDienThoai}");

            // Truy cập tài khoản từ ACCOUNT dựa vào MaNV
            using (var db = new QuanLyQuanAoEntities())
            {
                var account = db.ACCOUNT.FirstOrDefault(acc => acc.MaNV == employee.MaNV);
                string accountType = account?.Account_Type == true ? "Quản lý" : "Nhân viên"; // Kiểm tra kiểu tài khoản
                info.AppendLine($"Kiểu Tài Khoản: {accountType}");
            }

            // Kiểm tra nếu đường dẫn ảnh không rỗng
            if (!string.IsNullOrEmpty(employee.Anh)) // Giả sử employee.Anh chứa đường dẫn đến ảnh
            {
                try
                {
                    // Tải ảnh từ đường dẫn và hiển thị trên PictureBox
                    pictureBoxAnh.Image = Image.FromFile(employee.Anh);
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi nếu không thể tải ảnh từ đường dẫn
                    MessageBox.Show($"Không thể tải ảnh: {ex.Message}");
                }
            }
            else
            {
                // Nếu không có ảnh, có thể để trống PictureBox hoặc hiển thị ảnh mặc định
                pictureBoxAnh.Image = null; // Hoặc có thể gán ảnh mặc định như hình ảnh placeholder
            }


            MessageBox.Show(info.ToString(), "Thông Tin Nhân Viên", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SearchEmployee(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                LoadEmployeeDataFromDatabase(); // Nếu không có từ khóa, tải lại dữ liệu
                return;
            }

            using (var db = new QuanLyQuanAoEntities())
            {
                // Tìm kiếm nhân viên theo mã nhân viên hoặc tên nhân viên
                var filteredEmployees = db.EMPLOYERS
                    .Where(emp => emp.MaNV.Contains(keyword) || emp.TenNV.Contains(keyword))
                    .ToList();

                if (filteredEmployees.Count > 0)
                {
                    // Hiển thị thông tin của nhân viên đầu tiên tìm thấy
                    ShowEmployeeInfo(filteredEmployees.First());
                }
                else
                {
                    MessageBox.Show("Không tìm thấy nhân viên nào phù hợp.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btn_Lammoi_Click(object sender, EventArgs e)
        {
            ResetInputFields();          // Xóa dữ liệu trên các trường nhập liệu
            LoadEmployeeDataFromDatabase(); // Tải lại danh sách nhân viên từ cơ sở dữ liệu

            if (!radioButtonNam.Enabled)
                radioButtonNam.Enabled = true;

            if (!radioButtonNu.Enabled)
                radioButtonNu.Enabled = true;

            if (!dtpNgaysinh.Enabled)
                dtpNgaysinh.Enabled = true;

            if (!radioButtonQL.Enabled)
                radioButtonQL.Enabled = true;

            if (!radioButtonNV.Enabled)
                radioButtonNV.Enabled = true;

            if (!btn_Them.Enabled)
                btn_Them.Enabled = true;

            if (!btn_Sua.Enabled)
                btn_Sua.Enabled = true;

            if (!btn_Xoa.Enabled)
                btn_Xoa.Enabled = true;

            // Kích hoạt lại các trường nhập liệu
            textBox_MaNV.Enabled = true;
            textBox_TenNV.Enabled = true;
            maskedTextBox_SDT.Enabled = true;
        }
        // Đối tượng EMPLOYERS hiện tại để lưu trữ thông tin nhân viên
        private EMPLOYERS currentEmployer = new EMPLOYERS();


        private void DisplayValidationErrors(System.Data.Entity.Validation.DbEntityValidationException ex)
        {
            foreach (var validationError in ex.EntityValidationErrors)
            {
                foreach (var error in validationError.ValidationErrors)
                {
                    MessageBox.Show($"Property: {error.PropertyName} Error: {error.ErrorMessage}");
                }
            }
        }

        private void ResetInputFields()
        {
            textBox_MaNV.Clear();
            textBox_TenNV.Clear();
            maskedTextBox_SDT.Clear();
            pictureBoxAnh.Image = null;
            radioButtonNam.Checked = false;
            radioButtonNu.Checked = false;
            dtpNgaysinh.Value = DateTime.Now;
            radioButtonQL.Checked = false;
            radioButtonNV.Checked = false;

        }
        private void btn_Anh_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                openFileDialog.Title = "Chọn ảnh nhân viên";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Lưu đường dẫn ảnh vào thuộc tính Anh của currentEmployer
                    currentEmployer.Anh = openFileDialog.FileName;

                    // Hiển thị ảnh trong PictureBox (nếu có)
                    try
                    {
                        pictureBoxAnh.Image = Image.FromFile(currentEmployer.Anh);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Không thể tải ảnh: {ex.Message}");
                    }
                }
            }
        }



    }
}