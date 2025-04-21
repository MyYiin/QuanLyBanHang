using QuanLyBanHang.data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;
using QuanLyBanHang.Reports;
namespace QuanLyBanHang.forms
{


    public partial class frmHoaDon : Form
    {
        QLBHDbContext context = new QLBHDbContext();
        int id;
        public frmHoaDon()
        {
            InitializeComponent();
            HelpService.ApplyHelpToForm(this);
        }

        private void frmHoaDon_Load(object sender, EventArgs e)
        {
            dataGridView.AutoGenerateColumns = false;
            if (dataGridView.Columns.Count == 0)
            {
                dataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "ID", DataPropertyName = "ID", HeaderText = "ID" });
                dataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "HoVaTenNhanVien", DataPropertyName = "HoVaTenNhanVien", HeaderText = "Nhân viên" });
                dataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "HoVaTenKhachHang", DataPropertyName = "HoVaTenKhachHang", HeaderText = "Khách hàng" });
                dataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "NgayLap", DataPropertyName = "NgayLap", HeaderText = "Ngày lập" });
                dataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "TongTienHoaDon", DataPropertyName = "TongTienHoaDon", HeaderText = "Tổng tiền" });
                dataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "XemChiTiet", DataPropertyName = "XemChiTiet", HeaderText = "Chi tiết" });
            }

            List<DanhSachHoaDon> hd = new List<DanhSachHoaDon>();

            hd = context.HoaDon.Select(r => new DanhSachHoaDon
            {
                ID = r.ID,
                NhanVienID = r.NhanVienID,
                HoVaTenNhanVien = r.NhanVien.HoVaTen,
                KhachHangID = r.KhachHangID,
                HoVaTenKhachHang = r.KhachHang.HoVaTen,
                NgayLap = r.NgayLap,
                GhiChuHoaDon = r.GhiChuHoaDon,
                TongTienHoaDon = r.HoaDon_ChiTiet.Sum(r => r.DonGiaBan * r.SoLuongBan),
                XemChiTiet = "Xem chi tiết"
            }).ToList();

            dataGridView.DataSource = hd;
        }

        private void btnLapHoaDon_Click(object sender, EventArgs e)
        {
            using (frmHoaDon_ChiTiet chiTiet = new frmHoaDon_ChiTiet())
            {
                chiTiet.ShowDialog();
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            id = Convert.ToInt32(dataGridView.CurrentRow.Cells["ID"].Value.ToString());
            using (frmHoaDon_ChiTiet chiTiet = new frmHoaDon_ChiTiet(id))
            {
                chiTiet.ShowDialog();
            }
        }

        private void btnXuat_Click(object sender, EventArgs e)
        {
            using (
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Title = "Xuất File Exel",
                    Filter = "File Excel|*xls;*.xlsx",
                    FileName = "HoaDon_" + DateTime.Now.ToShortDateString().Replace("/", "_") + ".xlsx"
                })
            {
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (XLWorkbook wb = new XLWorkbook())
                        {
                            DataTable tableHoaDon = new DataTable();
                            tableHoaDon.Columns.Add("ID");
                            tableHoaDon.Columns.Add("NgayLap");
                            tableHoaDon.Columns.Add("NhanVien");
                            tableHoaDon.Columns.Add("KhachHang");
                            tableHoaDon.Columns.Add("GhiChu");
                            tableHoaDon.Columns.Add("TongTien");

                            var hoaDons = context.HoaDon.Select(hd => new
                            {
                                hd.ID,
                                hd.NgayLap,
                                NhanVien = hd.NhanVien.HoVaTen,
                                KhachHang = hd.KhachHang.HoVaTen,
                                hd.GhiChuHoaDon,
                                TongTien = hd.HoaDon_ChiTiet.Sum(ct => ct.DonGiaBan * ct.SoLuongBan)
                            }).ToList();

                            foreach (var hd in hoaDons)
                            {
                                tableHoaDon.Rows.Add(hd.ID, hd.NgayLap, hd.NhanVien, hd.KhachHang, hd.GhiChuHoaDon, hd.TongTien);
                            }

                            var sheet = wb.Worksheets.Add(tableHoaDon, "HoaDon");
                            sheet.Columns().AdjustToContents();

                            // Chi tiết Hoá Đơn
                            DataTable tableChiTiet = new DataTable();
                            tableChiTiet.Columns.Add("HoaDonID");
                            tableChiTiet.Columns.Add("SanPham");
                            tableChiTiet.Columns.Add("SoLuong");
                            tableChiTiet.Columns.Add("DonGia");
                            tableChiTiet.Columns.Add("ThanhTien");

                            var chiTietHds = context.HoaDon_ChiTiet.Select(ct => new
                            {
                                HoaDonID = Convert.ToInt32(ct.HoaDonID),  // Chuyển đổi kiểu dữ liệu
                                SanPham = ct.SanPham.TenSanPham,
                                SoLuongBan = Convert.ToInt32(ct.SoLuongBan), // Ép kiểu nếu cần
                                DonGiaBan = Convert.ToInt32(ct.DonGiaBan), // Đảm bảo kiểu số thực
                                ThanhTien = Convert.ToInt32(ct.SoLuongBan) * Convert.ToInt32(ct.DonGiaBan)
                            }).ToList();


                            foreach (var ct in chiTietHds)
                            {
                                tableChiTiet.Rows.Add(ct.HoaDonID, ct.SanPham, ct.SoLuongBan, ct.DonGiaBan, ct.ThanhTien);
                            }

                            var wsChiTiet = wb.Worksheets.Add(tableChiTiet, "HoaDon_ChiTiet");
                            wsChiTiet.Columns().AdjustToContents();

                            wb.SaveAs(saveFileDialog.FileName);
                            MessageBox.Show("Xuất file Excel thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xuất file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
            }
        }

        private void btnInHoaDon_Click(object sender, EventArgs e)
        {
            id = Convert.ToInt32(dataGridView.CurrentRow.Cells["ID"].Value.ToString());
            using (frmInHoaDon inHoaDon = new frmInHoaDon(id))
            {
                inHoaDon.ShowDialog();
            }
        }
    }
}
