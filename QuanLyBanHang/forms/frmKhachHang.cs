using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
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

namespace QuanLyBanHang.forms
{
    public partial class frmKhachHang : Form
    {
        QLBHDbContext context = new QLBHDbContext();
        bool xulyThem = false;
        int id;
        public frmKhachHang()
        {
            InitializeComponent();
            HelpService.ApplyHelpToForm(this);
        }

        private void frmKhachHang_Load(object sender, EventArgs e)
        {
            BatTatChucNang(false);

            List<KhachHang> kh = context.KhachHang.ToList();

            BindingSource bindingSourse = new BindingSource();
            bindingSourse.DataSource = kh;

            txtHoVaTen.DataBindings.Clear();
            txtHoVaTen.DataBindings.Add("Text", bindingSourse, "HoVaTen", false, DataSourceUpdateMode.Never);

            txtDienThoai.DataBindings.Clear();
            txtDienThoai.DataBindings.Add("Text", bindingSourse, "DienThoai", false, DataSourceUpdateMode.Never);

            txtDiaChi.DataBindings.Clear();
            txtDiaChi.DataBindings.Add("Text", bindingSourse, "DiaChi", false, DataSourceUpdateMode.Never);

            dataGridView.DataSource = bindingSourse;
        }

        private void BatTatChucNang(bool giaTri)
        {
            btnLuu.Enabled = giaTri;
            btnHuy.Enabled = giaTri;
            txtHoVaTen.Enabled = giaTri;
            txtDienThoai.Enabled = giaTri;
            txtDiaChi.Enabled = giaTri;
            btnThem.Enabled = !giaTri;
            btnSua.Enabled = !giaTri;
            btnXoa.Enabled = !giaTri;
            btnTimKiem.Enabled = !giaTri;
            btnNhap.Enabled = !giaTri;
            btnXuat.Enabled = !giaTri;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            BatTatChucNang(true);
            xulyThem = true;

            txtHoVaTen.Clear(); txtDienThoai.Clear(); txtDiaChi.Clear();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            BatTatChucNang(true);
            xulyThem = false;

            id = Convert.ToInt32(dataGridView.CurrentRow.Cells["id"].Value.ToString());

        }

        private void btnLuu_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtHoVaTen.Text))
                MessageBox.Show("Vui lòng nhập họ và tên khách hàng?", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (xulyThem)
            {
                KhachHang kh = new KhachHang();
                kh.HoVaTen = txtHoVaTen.Text;
                kh.DienThoai = txtDienThoai.Text;
                kh.DiaChi = txtDiaChi.Text;

                context.KhachHang.Add(kh);
                context.SaveChanges();
            }
            else
            {
                KhachHang kh = context.KhachHang.Find(id);

                if (kh != null)
                {
                    kh.HoVaTen = txtHoVaTen.Text;
                    kh.DienThoai = txtDienThoai.Text;
                    kh.DiaChi = txtDiaChi.Text;

                    context.KhachHang.Update(kh);
                    context.SaveChanges();
                }
            }

            frmKhachHang_Load(sender, e);
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Xác nhận xóa khách hàng " + txtHoVaTen.Text + "?", "Xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                id = Convert.ToInt32(dataGridView.CurrentRow.Cells["ID"].Value.ToString());
                KhachHang kh = context.KhachHang.Find(id);

                if (kh != null)
                {
                    context.KhachHang.Remove(kh);
                }

                context.SaveChanges();
                frmKhachHang_Load(sender, e);
            }


        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            frmKhachHang_Load(sender, e);
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Thoát ứng dung?", "Thoát", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                this.Close();
        }

        private void btnNhap_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Xuất dữ liệu ra file Excel",
                Filter = "Tập tin Excel|*.xls;*.xlsx",
                Multiselect = false,
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
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
                            if (firstRow)
                            {
                                readRange = string.Format("{0}:{1}", 1, row.LastCellUsed().Address.ColumnNumber);

                                foreach (IXLCell cell in row.Cells(readRange))
                                {
                                    table.Columns.Add(cell.Value.ToString());
                                }

                                firstRow = false;

                            }
                            else
                            {
                                table.Rows.Add();
                                int cellIndex = 0;

                                foreach (IXLCell cell in row.Cells(readRange))
                                {
                                    table.Rows[table.Rows.Count - 1][cellIndex] = cell.Value.ToString();
                                    cellIndex++;
                                }


                            }
                        }

                        if (table.Rows.Count > 0)
                        {
                            foreach (DataRow row in table.Rows)
                            {
                                string hoTen = row["HoVaTen"].ToString();
                                string soDienThoai = row["DienThoai"].ToString();
                                MessageBox.Show(hoTen, soDienThoai);
                                var existingKhachHang = context.KhachHang.FirstOrDefault(r => r.HoVaTen == hoTen || r.DienThoai == soDienThoai);

                                if (existingKhachHang != null)
                                {
                                    existingKhachHang.HoVaTen = hoTen;
                                    existingKhachHang.DienThoai = soDienThoai;
                                    existingKhachHang.DiaChi = row["DiaChi"].ToString();
                                }
                                else
                                {
                                    KhachHang khachHang = new KhachHang();
                                    khachHang.HoVaTen = hoTen;
                                    khachHang.DienThoai = soDienThoai;
                                    khachHang.DiaChi = row["DiaChi"].ToString();
                                    context.KhachHang.Add(khachHang);
                                }

                                context.SaveChanges();
                            }

                            MessageBox.Show("Đã nhập thành công " + table.Rows.Count + " dòng.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            frmKhachHang_Load(sender, e);
                        }
                        if (firstRow)
                            MessageBox.Show("Tập tin Excel rỗng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void btnXuat_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "Xuất file Excel",
                Filter = "Tập tin Excel|*.xls;*.xlsx",
                FileName = "KhachHang_" + DateTime.Now.ToShortDateString().Replace("/", "_") + ".xlsx",
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    DataTable table = new DataTable();

                    table.Columns.AddRange(new DataColumn[4]
                    {
                        new DataColumn("ID", typeof(int)),
                        new DataColumn("HoVaTen", typeof(string)),
                        new DataColumn("DienThoai", typeof(string)),
                        new DataColumn("DiaChi", typeof(string))
                    });

                    var khachHangs = context.KhachHang.ToList();

                    foreach (var k in khachHangs) {
                        table.Rows.Add(k.ID, k.HoVaTen, k.DienThoai, k.DiaChi);
                    }

                    using (XLWorkbook wb = new XLWorkbook()) {
                        var sheet = wb.Worksheets.Add(table, "KhachHang");
                        sheet.Columns().AdjustToContents();
                        wb.SaveAs(saveFileDialog.FileName);

                        MessageBox.Show("Đã xuất dữ liệu ra tập tin Excel thành công.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }
    }
}
