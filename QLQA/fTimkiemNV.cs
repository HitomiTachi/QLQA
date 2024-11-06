using QLQA.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QLQA
{
    public partial class fTimkiemNV : Form
    {
        public fTimkiemNV()
        {
            InitializeComponent();
            InitializeDataGridView();
        }

        private void InitializeDataGridView()
        {
            // Khởi tạo cột cho DataGridView nhưng không thêm dữ liệu sẵn có
            dataGridViewNV.Columns.Add("MaNV", "Mã NV");
            dataGridViewNV.Columns.Add("TenNV", "Tên NV");
            dataGridViewNV.Columns.Add("GioiTinh", "Giới Tính");
            dataGridViewNV.Columns.Add("NgaySinh", "Ngày Sinh");
            dataGridViewNV.Columns.Add("SoDienThoai", "Số Điện Thoại");
            dataGridViewNV.Columns.Add("AccountType", "Kiểu Tài Khoản");

            // Thêm sự kiện để theo dõi khi có thay đổi giá trị trong DataGridView
            dataGridViewNV.CellValueChanged += dataGridViewNV_CellValueChanged;
            dataGridViewNV.RowPrePaint += dgv_RowPrePaint; // Thêm sự kiện để đánh số thứ tự
        }

        private void dgv_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            dataGridViewNV.Rows[e.RowIndex].Cells["STT"].Value = e.RowIndex + 1; // Đánh số thứ tự
        }

        private void SearchEmployee(string maNVKeyword)
        {
            using (var db = new QuanLyQuanAoEntities())
            {
                // Tìm kiếm nhân viên dựa trên MaNV
                var filteredEmployees = db.EMPLOYERS.AsQueryable();

                if (!string.IsNullOrWhiteSpace(maNVKeyword))
                {
                    filteredEmployees = filteredEmployees.Where(emp => emp.MaNV.Contains(maNVKeyword));
                }

                var results = filteredEmployees.ToList();

                if (results.Any())
                {
                    // Hiển thị kết quả trong DataGridView
                    BindGrid(results);
                }
                else
                {
                    MessageBox.Show("Không tìm thấy nhân viên nào phù hợp.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BindGrid(List<EMPLOYERS> employees)
        {
            dataGridViewNV.Rows.Clear(); // Xóa các dòng cũ

            using (var db = new QuanLyQuanAoEntities())
            {
                foreach (var item in employees)
                {
                    var account = db.ACCOUNT.FirstOrDefault(acc => acc.MaNV == item.MaNV);
                    string accountType = account?.Account_Type == true ? "Quản lý" : "Nhân viên";

                    int index = dataGridViewNV.Rows.Add();
                    dataGridViewNV.Rows[index].Cells["MaNV"].Value = item.MaNV;
                    dataGridViewNV.Rows[index].Cells["TenNV"].Value = item.TenNV;
                    dataGridViewNV.Rows[index].Cells["GioiTinh"].Value = item.GioiTinh ? "Nam" : "Nữ";
                    dataGridViewNV.Rows[index].Cells["NgaySinh"].Value = item.NgaySinh.ToString("dd/MM/yyyy");
                    dataGridViewNV.Rows[index].Cells["SoDienThoai"].Value = item.SoDienThoai;
                    dataGridViewNV.Rows[index].Cells["AccountType"].Value = accountType;
                }
            }
        }
        private void dataGridViewNV_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra nếu có thay đổi trong dòng dữ liệu
            if (e.RowIndex >= 0)
            {
                var row = dataGridViewNV.Rows[e.RowIndex];
                string maNV = row.Cells["MaNV"].Value.ToString();
                string tenNV = row.Cells["TenNV"].Value.ToString();
                bool gioiTinh = row.Cells["GioiTinh"].Value.ToString() == "Nam";
                DateTime ngaySinh;
                bool isDateValid = DateTime.TryParse(row.Cells["NgaySinh"].Value.ToString(), out ngaySinh);
                string soDienThoai = row.Cells["SoDienThoai"].Value.ToString();

                if (!isDateValid)
                {
                    MessageBox.Show("Ngày sinh không hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Tạo đối tượng nhân viên từ thông tin trong dòng dữ liệu
                var updatedEmployee = new EMPLOYERS
                {
                    MaNV = maNV,
                    TenNV = tenNV,
                    GioiTinh = gioiTinh,
                    NgaySinh = ngaySinh,
                    SoDienThoai = soDienThoai
                };

                // Cập nhật thông tin vào cơ sở dữ liệu
                UpdateEmployee(updatedEmployee);
            }
        }

        private void dataGridViewNV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }
        private void UpdateEmployee(EMPLOYERS updatedEmployee)
        {
            using (var db = new QuanLyQuanAoEntities())
            {
                // Tìm nhân viên cần cập nhật trong cơ sở dữ liệu
                var existingEmployee = db.EMPLOYERS.FirstOrDefault(emp => emp.MaNV == updatedEmployee.MaNV);

                if (existingEmployee != null)
                {
                    // Cập nhật thông tin nhân viên
                    existingEmployee.TenNV = updatedEmployee.TenNV;
                    existingEmployee.GioiTinh = updatedEmployee.GioiTinh;
                    existingEmployee.NgaySinh = updatedEmployee.NgaySinh;
                    existingEmployee.SoDienThoai = updatedEmployee.SoDienThoai;

                    // Lưu thay đổi vào cơ sở dữ liệu
                    db.SaveChanges();
                    MessageBox.Show("Cập nhật thông tin nhân viên thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Không tìm thấy nhân viên để cập nhật.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string maNVKeyword = textBox_NV.Text.Trim();

            if (string.IsNullOrWhiteSpace(maNVKeyword))
            {
                MessageBox.Show("Vui lòng nhập mã nhân viên để tìm kiếm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Thực hiện tìm kiếm
            SearchEmployee(maNVKeyword);
        }
    }
}
