using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using QuanLyBanHang.data;
using System.Text.RegularExpressions;
using System.Diagnostics;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;

namespace QuanLyBanHang.forms
{
    public partial class frmSanPham : Form
    {
        QLBHDbContext context = new QLBHDbContext(); // Khởi tạo DBContext
        bool xuLyThem = false; // Kiểm tra có nhấn Thêm hay không
        int id; // Mã sản phẩm (dùng cho Sửa, Xóa)
        string imagesFolder = Application.StartupPath.Replace("bin\\Debug\\net8.0-windows", "Images");

        public frmSanPham()
        {
            InitializeComponent();
            HelpService.ApplyHelpToForm(this);
        }

        private void frmSanPham_Load(object sender, EventArgs e)
        {
            BatTatChucNang(false);
            LayLoaiSanPhamVaoComboBox();
            LayHangSanXuatVaoComboBox();

            dataGridView.AutoGenerateColumns = false;

            // Thêm cột thủ công
            if (dataGridView.Columns.Count == 0)
            {
                dataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "ID", DataPropertyName = "ID", HeaderText = "ID" });
                dataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenLoai", DataPropertyName = "TenLoai", HeaderText = "Phân loại" });
                dataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenHangSanXuat", DataPropertyName = "TenHangSanXuat", HeaderText = "Hãng sản xuất" });
                dataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenSanPham", DataPropertyName = "TenSanPham", HeaderText = "Tên sản phẩm" });
                dataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "SoLuong", DataPropertyName = "SoLuong", HeaderText = "Số lượng" });
                dataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "DonGia", DataPropertyName = "DonGia", HeaderText = "Đơn giá" });

                DataGridViewImageColumn imageCol = new DataGridViewImageColumn();
                imageCol.Name = "HinhAnh";
                imageCol.DataPropertyName = "HinhAnh";
                dataGridView.Columns.Add(imageCol);
            }
            List<DanhSachSanPham> sp = new List<DanhSachSanPham>();
            sp = context.SanPham.Select(r => new DanhSachSanPham
            {
                ID = r.ID,
                LoaiSanPhamID = r.LoaiSanPhamID,
                TenLoai = r.LoaiSanPham.TenLoai,
                HangSanXuatID = r.HangSanXuatID,
                TenHangSanXuat = r.HangSanXuat.TenHangSanXuat,
                TenSanPham = r.TenSanPham,
                SoLuong = r.SoLuong,
                DonGia = r.DonGia,
                HinhAnh = r.HinhAnh
            }).ToList();

            BindingSource bindingSource = new BindingSource();
            bindingSource.DataSource = sp;

            cboLoaiSanPham.DataBindings.Clear();
            cboLoaiSanPham.DataBindings.Add("SelectedValue", bindingSource, "LoaiSanPhamID", false, DataSourceUpdateMode.Never);

            cboHangSanXuat.DataBindings.Clear();
            cboHangSanXuat.DataBindings.Add("SelectedValue", bindingSource, "HangSanXuatID", false, DataSourceUpdateMode.Never);

            txtTenSanPham.DataBindings.Clear();
            txtTenSanPham.DataBindings.Add("Text", bindingSource, "TenSanPham", false, DataSourceUpdateMode.Never);

            txtMoTa.DataBindings.Clear();
            txtMoTa.DataBindings.Add("Text", bindingSource, "Mota", false, DataSourceUpdateMode.Never);

            numSoLuong.DataBindings.Clear();
            numSoLuong.DataBindings.Add("Value", bindingSource, "SoLuong", false, DataSourceUpdateMode.Never);

            numDonGia.DataBindings.Clear();
            numDonGia.DataBindings.Add("Value", bindingSource, "DonGia", false, DataSourceUpdateMode.Never);

            picHinhAnh.DataBindings.Clear();
            Binding hinhAnh = new Binding("ImageLocation", bindingSource, "HinhAnh");
            hinhAnh.Format += (s, e) =>
            {
                e.Value = LayDuongDanAnh(e.Value?.ToString());
            };
            picHinhAnh.DataBindings.Add(hinhAnh);

            dataGridView.DataSource = bindingSource;
        }

        // Đường dẫn Image
        private string LayDuongDanAnh(string tenFile)
        {
            string filePath = Path.Combine(imagesFolder, tenFile ?? "no-image.png");
            return File.Exists(filePath) ? filePath : Path.Combine(imagesFolder, "no-image.png");
        }

        // Xoá ảnh
        private void XoaAnhCu(string oldFileName)
        {
            if (!string.IsNullOrEmpty(oldFileName) && oldFileName != "no-image.png")
            {
                string oldFilePath = Path.Combine(imagesFolder, oldFileName);
                //MessageBox.Show(oldFilePath);
                if (File.Exists(oldFilePath))
                {
                    try
                    {
                        picHinhAnh.Image?.Dispose();  // Giải phóng ảnh cũ
                        picHinhAnh.Image = null;

                        File.Delete(oldFilePath);
                    }
                    catch (IOException)
                    {
                        MessageBox.Show("Không thể xóa ảnh cũ, có thể nó đang được sử dụng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }


        // Validate input
        private bool KiemTraHopLe()
        {
            if (cboLoaiSanPham.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn loại sản phẩm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboLoaiSanPham.Focus();
                return false;
            }

            if (cboHangSanXuat.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn hãng sản xuất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboHangSanXuat.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtTenSanPham.Text))
            {
                MessageBox.Show("Tên sản phẩm không được để trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenSanPham.Focus();
                return false;
            }

            if (numSoLuong.Value <= 0)
            {
                MessageBox.Show("Số lượng phải lớn hơn 0!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                numSoLuong.Focus();
                return false;
            }

            if (numDonGia.Value <= 0)
            {
                MessageBox.Show("Đơn giá phải lớn hơn 0!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                numDonGia.Focus();
                return false;
            }

            if (picHinhAnh.Image == null)
            {
                MessageBox.Show("Vui lòng chọn hình ảnh sản phẩm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }


        // Ẩn/Hiện các nút điều khiển
        private void BatTatChucNang(bool giaTri)
        {
            btnLuu.Enabled = giaTri;
            btnHuy.Enabled = giaTri;
            cboHangSanXuat.Enabled = giaTri;
            cboLoaiSanPham.Enabled = giaTri;
            txtTenSanPham.Enabled = giaTri;
            numSoLuong.Enabled = giaTri;
            numDonGia.Enabled = giaTri;
            txtMoTa.Enabled = giaTri;
            picHinhAnh.Enabled = giaTri;
            btnDoiAnh.Enabled = giaTri;

            btnThem.Enabled = !giaTri;
            btnSua.Enabled = !giaTri;
            btnXoa.Enabled = !giaTri;
        }

        // Load dữ liệu vào ComboBox
        private void LayLoaiSanPhamVaoComboBox()
        {
            cboLoaiSanPham.DataSource = context.LoaiSanPham.ToList();
            cboLoaiSanPham.ValueMember = "ID";
            cboLoaiSanPham.DisplayMember = "TenLoai";
        }

        private void LayHangSanXuatVaoComboBox()
        {
            cboHangSanXuat.DataSource = context.HangSanXuat.ToList();
            cboHangSanXuat.ValueMember = "ID";
            cboHangSanXuat.DisplayMember = "TenHangSanXuat";
        }

        // Xử lý khi nhấn nút Thêm
        private void btnThem_Click(object sender, EventArgs e)
        {
            xuLyThem = true;
            BatTatChucNang(true);

            cboLoaiSanPham.SelectedIndex = -1;
            cboHangSanXuat.SelectedIndex = -1;
            txtTenSanPham.Clear();
            txtMoTa.Clear();
            numSoLuong.Value = 0;
            numDonGia.Value = 0;

            picHinhAnh.Image = Image.FromFile(Path.Combine(imagesFolder, "no-image.png"));
            picHinhAnh.Tag = "no-image.png";
        }

        // Xử lý khi nhấn nút Lưu
        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (!KiemTraHopLe()) return;

            if (xuLyThem)
            {
                var sp = new SanPham
                {
                    LoaiSanPhamID = (int)cboLoaiSanPham.SelectedValue,
                    HangSanXuatID = (int)cboHangSanXuat.SelectedValue,
                    TenSanPham = txtTenSanPham.Text,
                    SoLuong = (int)numSoLuong.Value,
                    DonGia = (int)numDonGia.Value,
                    MoTa = txtMoTa.Text,
                    HinhAnh = string.IsNullOrEmpty(picHinhAnh.Tag?.ToString()) ? "no-image.png" : picHinhAnh.Tag.ToString()
                };

                context.SanPham.Add(sp);
            }
            else
            {
                var sp = context.SanPham.Find(id);
                if (sp != null)
                {
                    sp.LoaiSanPhamID = (int)cboLoaiSanPham.SelectedValue;
                    sp.HangSanXuatID = (int)cboHangSanXuat.SelectedValue;
                    sp.TenSanPham = txtTenSanPham.Text;
                    sp.SoLuong = (int)numSoLuong.Value;
                    sp.DonGia = (int)numDonGia.Value;
                    sp.MoTa = txtMoTa.Text;

                    // Kiểm tra nếu không có ảnh mới, dùng ảnh cũ hoặc ảnh mặc định
                    sp.HinhAnh = string.IsNullOrEmpty(picHinhAnh.Tag?.ToString()) ? sp.HinhAnh ?? "no-image.png" : picHinhAnh.Tag.ToString();
                }
            }

            context.SaveChanges();
            frmSanPham_Load(sender, e);
        }

        // Xử lý khi nhấn nút Xóa
        private void btnXoa_Click(object sender, EventArgs e)
        {
            id = Convert.ToInt32(dataGridView.CurrentRow.Cells["ID"].Value.ToString());

            SanPham sp = context.SanPham.Find(id);

            if (MessageBox.Show($"Xác nhận xóa sản phẩm ${sp.TenSanPham}?", "Xóa", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (sp != null)
                {
                    XoaAnhCu(sp.HinhAnh);

                    context.SanPham.Remove(sp);
                    context.SaveChanges();
                }
                frmSanPham_Load(sender, e);
            }
        }

        // Xử lý khi nhấn nút Đổi ảnh
        private void btnDoiAnh_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Chọn hình ảnh sản phẩm",
                Filter = "Hình ảnh|*.jpg;*.jpeg;*.png"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                string extension = Path.GetExtension(openFileDialog.FileName);
                string oldFileName = picHinhAnh.Tag?.ToString();

                // Chuẩn hóa tên file: chuyển về chữ thường, thay khoảng trắng bằng dấu '-'
                string normalizedFileName = Regex.Replace(fileName.ToLower(), @"[^a-z0-9]+", "-") + extension;
                string destPath = Path.Combine(imagesFolder, normalizedFileName);

                // Kiểm tra trùng lặp và tạo tên mới nếu cần
                int counter = 1;
                while (File.Exists(destPath))
                {
                    normalizedFileName = Regex.Replace(fileName.ToLower(), @"[^a-z0-9]+", "-") + $"-{counter}" + extension;
                    destPath = Path.Combine(imagesFolder, normalizedFileName);
                    counter++;
                }

                // Xóa file ảnh cũ nếu không phải ảnh mặc định
                if (!string.IsNullOrEmpty(oldFileName) && oldFileName != "no-image.png")
                {
                    XoaAnhCu(oldFileName);
                }

                // Sao chép file mới vào thư mục
                File.Copy(openFileDialog.FileName, destPath, true);

                // Hiển thị ảnh mới
                picHinhAnh.Image = Image.FromFile(destPath);
                picHinhAnh.Tag = normalizedFileName;
            }
        }

        private void dataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView.Columns[e.ColumnIndex].DataPropertyName == "HinhAnh")
            {
                Image image = Image.FromFile(Path.Combine(imagesFolder, e.Value.ToString()));
                image = new Bitmap(image, 24, 24);
                e.Value = image;
            }

        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            xuLyThem = false;
            BatTatChucNang(true);
            id = Convert.ToInt32(dataGridView.CurrentRow.Cells["ID"].Value.ToString());
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            frmSanPham_Load(sender, e);

        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Thoát ứng dung?", "Thoát", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                this.Close();
        }

        private void btnNhap_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog =  new OpenFileDialog
            {
                Title = "Xuất dữ liệu ra file Excel",
                Filter = "Tập tin Excel|*.xls;*.xlsx",
                Multiselect = false,
            })
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK) {
                    try
                    {
                        DataTable table = new DataTable();

                        using (XLWorkbook wb = new XLWorkbook(openFileDialog.FileName))
                        {
                            var sheet = wb.Worksheet(1);
                            bool firstRow = true;
                            string readRange = "1:1";

                            foreach (IXLRow row in sheet.RowsUsed())
                            {
                                if (firstRow) // Đọc tiêu đề cột
                                {
                                    readRange = string.Format("{0}:{1}", 1, row.LastCellUsed().Address.ColumnNumber);
                                    foreach (IXLCell cell in row.Cells(readRange))
                                    {
                                        table.Columns.Add(cell.Value.ToString().Trim());
                                    }
                                    firstRow = false;
                                }
                                else // Đọc dữ liệu
                                {
                                    table.Rows.Add();
                                    int cellIndex = 0;
                                    foreach (IXLCell cell in row.Cells(readRange))
                                    {
                                        table.Rows[table.Rows.Count - 1][cellIndex] = cell.Value.ToString().Trim();
                                        cellIndex++;
                                    }
                                }

                            }

                            if (table.Rows.Count > 0)
                            {
                                int soLuongCapNhat = 0, soLuongThemMoi = 0;

                                foreach (DataRow row in table.Rows)
                                {
                                    string tenSanPham = row["TenSanPham"].ToString().Trim();
                                    string tenLoai = row["TenLoai"].ToString().Trim();
                                    string tenHang = row["TenHangSanXuat"].ToString().Trim();

                                    var loaiSP = context.LoaiSanPham.FirstOrDefault(l => l.TenLoai == tenLoai);
                                    var hangSX = context.HangSanXuat.FirstOrDefault(h => h.TenHangSanXuat == tenHang);

                                    var existingSanPham = context.SanPham.FirstOrDefault(sp => sp.TenSanPham == tenSanPham);

                                    if (existingSanPham != null)
                                    {
                                        // Cập nhật sản phẩm nếu đã tồn tại
                                        existingSanPham.LoaiSanPhamID = loaiSP?.ID ?? 0;
                                        existingSanPham.HangSanXuatID = hangSX?.ID ?? 0;
                                        existingSanPham.SoLuong = int.TryParse(row["SoLuong"].ToString(), out int soLuong) ? soLuong : existingSanPham.SoLuong;
                                        existingSanPham.DonGia = int.TryParse(row["DonGia"].ToString(), out int donGia) ? donGia : existingSanPham.DonGia;
                                        existingSanPham.MoTa = row["MoTa"].ToString();
                                        soLuongCapNhat++;
                                    }
                                    else
                                    {
                                        // Thêm mới sản phẩm nếu chưa tồn tại
                                        SanPham sp = new SanPham
                                        {
                                            LoaiSanPhamID = loaiSP?.ID ?? 0,
                                            HangSanXuatID = hangSX?.ID ?? 0,
                                            TenSanPham = tenSanPham,
                                            SoLuong = int.TryParse(row["SoLuong"].ToString(), out int soLuong) ? soLuong : 0,
                                            DonGia = int.TryParse(row["DonGia"].ToString(), out int donGia) ? donGia : 0,
                                            MoTa = row["MoTa"].ToString(),
                                            HinhAnh = "no-image.png"
                                        };
                                        context.SanPham.Add(sp);
                                        soLuongThemMoi++;
                                    }
                                }

                                context.SaveChanges();

                                MessageBox.Show($"Cập nhật: {soLuongCapNhat} sản phẩm\nThêm mới: {soLuongThemMoi} sản phẩm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                frmSanPham_Load(sender, e);
                            }
                            else
                            {
                                MessageBox.Show("Tập tin Excel không có dữ liệu.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                        }
                    }
                    catch (Exception ex) {
                        MessageBox.Show("Lỗi khi nhập Excel: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnXuat_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Xuất File Excel",
                Filter = "File Excel|*.xls;*.xlsx",
                FileName = "SanPham_" + DateTime.Now.ToShortDateString().Replace("/", "_") + ".xlsx",
            })
            {
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (XLWorkbook wb = new XLWorkbook())
                        {
                            DataTable dt = new DataTable();
                            dt.Columns.Add("ID");
                            dt.Columns.Add("TenLoai");
                            dt.Columns.Add("TenHangSanXuat");
                            dt.Columns.Add("TenSanPham");
                            dt.Columns.Add("SoLuong");
                            dt.Columns.Add("DonGia");
                            dt.Columns.Add("Mota");
                            dt.Columns.Add("HinhAnh");

                            var sanPhams = context.SanPham.Select(sp => new
                            {
                                sp.ID,
                                sp.LoaiSanPham.TenLoai,
                                sp.HangSanXuat.TenHangSanXuat,
                                sp.TenSanPham,
                                sp.SoLuong,
                                sp.DonGia,
                                sp.MoTa,
                                sp.HinhAnh
                            }).ToList();

                            foreach (var sp in sanPhams)
                            {
                                dt.Rows.Add(sp.ID, sp.TenLoai, sp.TenHangSanXuat, sp.TenSanPham, sp.SoLuong, sp.DonGia, sp.MoTa, sp.HinhAnh);
                            }

                            var sheet = wb.Worksheets.Add(dt, "SanPham");
                            sheet.Columns().AdjustToContents();
                            wb.SaveAs(saveFileDialog.FileName);
                        }

                        MessageBox.Show("Xuất Excel thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex) {
                        MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    }
                }
            }
        }
    }
}