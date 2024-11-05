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
            dataGridViewNV.Columns.Add("MaNV", "Mã NV");
            dataGridViewNV.Columns.Add("TenNV", "Tên NV");
            dataGridViewNV.Columns.Add("GioiTinh", "Giới Tính");
            dataGridViewNV.Columns.Add("NgaySinh", "Ngày Sinh");
            dataGridViewNV.Columns.Add("SoDienThoai", "Số Điện Thoại");
            dataGridViewNV.Columns.Add("AccountType", "Kiểu Tài Khoản");
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
            dataGridViewNV.Rows.Clear();
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

        private void dataGridViewNV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewNV.Rows[e.RowIndex];

                StringBuilder info = new StringBuilder();
                info.AppendLine($"Mã NV: {row.Cells["MaNV"].Value}");
                info.AppendLine($"Tên NV: {row.Cells["TenNV"].Value}");
                info.AppendLine($"Giới Tính: {row.Cells["GioiTinh"].Value}");
                info.AppendLine($"Ngày Sinh: {row.Cells["NgaySinh"].Value}");
                info.AppendLine($"Số Điện Thoại: {row.Cells["SoDienThoai"].Value}");
                info.AppendLine($"Kiểu Tài Khoản: {row.Cells["AccountType"].Value}");

                MessageBox.Show(info.ToString(), "Thông Tin Chi Tiết Nhân Viên", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string maNVKeyword = textBox_NV.Text.Trim();

            // Kiểm tra nếu trường MaNV trống
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
