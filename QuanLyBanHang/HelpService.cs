using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyBanHang
{
    public static class HelpService
    {
        private static HelpProvider _helpProvider;
        public static HelpProvider HelpProvider
        {
            get
            {
                if (_helpProvider == null)
                {
                    _helpProvider = new HelpProvider();
                }
                return _helpProvider;
            }
        }

        // Master dictionary for all help text
        private static Dictionary<string, string> AllHelpText = new Dictionary<string, string>
        {
            // Common controls
            { "btnThoat", "Thoát khỏi form hiện tại." },
            { "btnTimKiem", "Tìm kiếm dữ liệu." },
            { "btnXuat", "Xuất dữ liệu ra file Excel." },
            { "btnSua", "Sửa dữ liệu được chọn." },
            { "btnXoa", "Xóa dữ liệu được chọn." },
            { "btnLuu", "Lưu thông tin đã nhập." },
            { "btnThem", "Thêm dữ liệu mới." },
            
            // Login form
            { "txtTenDangNhap", "Nhập tên đăng nhập của bạn." },
            { "txtMatKhau", "Nhập mật khẩu của bạn." },
            { "btnDangNhap", "Đăng nhập vào hệ thống." },
            { "btnHuy", "Hủy thao tác đăng nhập." },
            
            // Loại sản phẩm form
            { "txtTenLoaiSanPham", "Nhập tên loại sản phẩm." },
            { "txtMaLoaiSanPham", "Mã loại sản phẩm." },
            { "txtMoTaLoaiSanPham", "Mô tả chi tiết về loại sản phẩm." },
            
            // Hãng sản xuất form
            { "txtTenHangSanXuat", "Nhập tên hãng sản xuất." },
            { "txtMaHangSanXuat", "Mã hãng sản xuất." },
            { "txtQuocGia", "Quốc gia của hãng sản xuất." },
            { "txtWebsite", "Website chính thức của hãng sản xuất." },
            
            // Sản phẩm form
            { "txtTenSanPham", "Nhập tên sản phẩm." },
            { "txtMaSanPham", "Mã sản phẩm." },
            { "cboLoaiSanPham", "Chọn loại sản phẩm." },
            { "cboHangSanXuat", "Chọn hãng sản xuất." },
            { "numSoLuong", "Nhập số lượng sản phẩm trong kho." },
            { "numDonGia", "Nhập giá bán sản phẩm." },
            { "txtMoTaSanPham", "Mô tả chi tiết về sản phẩm." },
            { "dtpNgayNhap", "Ngày nhập sản phẩm vào kho." },
            { "chkConHang", "Đánh dấu nếu sản phẩm còn hàng." },
            
            // Nhân viên form
            { "txtHoVaTen", "Nhập họ tên đầy đủ của nhân viên." },
            { "txtMaNhanVien", "Mã nhân viên." },
            { "txtDienThoai", "Nhập số điện thoại liên hệ." },
            { "txtDiaChi", "Nhập địa chỉ nhân viên." },
            { "txtEmail", "Nhập địa chỉ email nhân viên." },
            { "dtpNgaySinh", "Ngày sinh của nhân viên." },
            { "cboQuyenHan", "Chọn quyền hạn của nhân viên trong hệ thống." },
            { "rdoNam", "Chọn nếu nhân viên là nam." },
            { "rdoNu", "Chọn nếu nhân viên là nữ." },
            
            // Khách hàng form
            { "txtTenKhachHang", "Nhập tên khách hàng." },
            { "txtMaKhachHang", "Mã khách hàng." },
            { "txtDienThoaiKH", "Số điện thoại liên hệ của khách hàng." },
            { "txtDiaChiKH", "Địa chỉ của khách hàng." },
            { "txtEmailKH", "Email của khách hàng." },
            { "dtpNgaySinhKH", "Ngày sinh của khách hàng." },
            { "numDiemTichLuy", "Điểm tích lũy của khách hàng." },
            
            // Hóa đơn form
            { "txtMaHoaDon", "Mã hóa đơn." },
            { "dtpNgayLap", "Ngày lập hóa đơn." },
            { "cboNhanVien", "Chọn nhân viên thanh toán." },
            { "cboKhachHang", "Chọn khách hàng mua hàng." },
            { "txtGhiChuHoaDon", "Ghi chú cho hóa đơn." },
            { "lblTongTien", "Tổng tiền hóa đơn." },
            { "btnLapHoaDon", "Tạo hóa đơn mới." },
            { "btnInHoaDon", "In hóa đơn đã tạo." },
            { "btnThanhToan", "Xác nhận thanh toán hóa đơn." },
            
            // Chi tiết hóa đơn form
            { "cboSanPham", "Chọn sản phẩm cần thêm vào hóa đơn." },
            { "numDonGiaBan", "Giá bán của sản phẩm." },
            { "numSoLuongBan", "Số lượng sản phẩm bán." },
            { "btnThemSanPham", "Thêm sản phẩm vào hóa đơn." },
            { "btnXoaSanPham", "Xóa sản phẩm khỏi hóa đơn." },
            { "btnSuaSanPham", "Sửa thông tin sản phẩm trong hóa đơn." },
            
            // Báo cáo thống kê form
            { "dtpTuNgay", "Ngày bắt đầu thống kê." },
            { "dtpDenNgay", "Ngày kết thúc thống kê." },
            { "btnXemBaoCao", "Xem báo cáo theo điều kiện đã chọn." },
            { "btnInBaoCao", "In báo cáo hiện tại." },
            { "cboLoaiBaoCao", "Chọn loại báo cáo cần xem." }
        };

        // Form-specific help dictionaries
        private static Dictionary<string, Dictionary<string, string>> FormHelpText = new Dictionary<string, Dictionary<string, string>>
        {
            // Login Form
            { "frmDangNhap", new Dictionary<string, string>
                {
                    { "txtTenDangNhap", "Nhập tên đăng nhập của bạn." },
                    { "txtMatKhau", "Nhập mật khẩu của bạn." },
                    { "btnDangNhap", "Đăng nhập vào hệ thống." },
                    { "btnHuy", "Hủy thao tác đăng nhập." },
                    { "chkNhoMatKhau", "Đánh dấu để lưu thông tin đăng nhập cho lần sau." }
                }
            },
            
            // Loại sản phẩm Form
            { "frmLoaiSanPham", new Dictionary<string, string>
                {
                    { "txtTenLoaiSanPham", "Nhập tên loại sản phẩm." },
                    { "txtMaLoaiSanPham", "Mã loại sản phẩm." },
                    { "txtMoTaLoaiSanPham", "Mô tả chi tiết về loại sản phẩm." },
                    { "btnThem", "Thêm loại sản phẩm mới." },
                    { "btnSua", "Sửa thông tin loại sản phẩm." },
                    { "btnXoa", "Xóa loại sản phẩm." },
                    { "btnLuu", "Lưu thông tin loại sản phẩm." },
                    { "btnThoat", "Đóng form loại sản phẩm." },
                    { "dgvLoaiSanPham", "Danh sách các loại sản phẩm." }
                }
            },
            
            // Sản phẩm Form
            { "frmSanPham", new Dictionary<string, string>
                {
                    { "txtTenSanPham", "Nhập tên sản phẩm." },
                    { "txtMaSanPham", "Mã sản phẩm." },
                    { "cboLoaiSanPham", "Chọn loại sản phẩm." },
                    { "cboHangSanXuat", "Chọn hãng sản xuất." },
                    { "numSoLuong", "Nhập số lượng sản phẩm trong kho." },
                    { "numDonGia", "Nhập giá bán sản phẩm." },
                    { "txtMoTaSanPham", "Mô tả chi tiết về sản phẩm." },
                    { "dtpNgayNhap", "Ngày nhập sản phẩm vào kho." },
                    { "chkConHang", "Đánh dấu nếu sản phẩm còn hàng." },
                    { "btnThem", "Thêm sản phẩm mới." },
                    { "btnSua", "Sửa thông tin sản phẩm." },
                    { "btnXoa", "Xóa sản phẩm." },
                    { "btnLuu", "Lưu thông tin sản phẩm." },
                    { "btnTimKiem", "Tìm kiếm sản phẩm." },
                    { "btnThoat", "Đóng form sản phẩm." },
                    { "dgvSanPham", "Danh sách các sản phẩm." }
                }
            },
            
            // Nhân viên Form
            { "frmNhanVien", new Dictionary<string, string>
                {
                    { "txtHoVaTen", "Nhập họ tên đầy đủ của nhân viên." },
                    { "txtMaNhanVien", "Mã nhân viên." },
                    { "txtDienThoai", "Nhập số điện thoại liên hệ." },
                    { "txtDiaChi", "Nhập địa chỉ nhân viên." },
                    { "txtEmail", "Nhập địa chỉ email nhân viên." },
                    { "dtpNgaySinh", "Ngày sinh của nhân viên." },
                    { "txtTenDangNhap", "Tên đăng nhập vào hệ thống." },
                    { "txtMatKhau", "Mật khẩu đăng nhập." },
                    { "cboQuyenHan", "Chọn quyền hạn của nhân viên trong hệ thống." },
                    { "btnThem", "Thêm nhân viên mới." },
                    { "btnSua", "Sửa thông tin nhân viên." },
                    { "btnXoa", "Xóa nhân viên." },
                    { "btnLuu", "Lưu thông tin nhân viên." },
                    { "btnThoat", "Đóng form nhân viên." },
                    { "dgvNhanVien", "Danh sách các nhân viên." }
                }
            },
            
            // Hóa đơn Form
            { "frmHoaDon", new Dictionary<string, string>
                {
                    { "txtMaHoaDon", "Mã hóa đơn." },
                    { "dtpNgayLap", "Ngày lập hóa đơn." },
                    { "cboNhanVien", "Chọn nhân viên thanh toán." },
                    { "cboKhachHang", "Chọn khách hàng mua hàng." },
                    { "txtGhiChuHoaDon", "Ghi chú cho hóa đơn." },
                    { "lblTongTien", "Tổng tiền hóa đơn." },
                    { "btnLapHoaDon", "Tạo hóa đơn mới." },
                    { "btnInHoaDon", "In hóa đơn đã tạo." },
                    { "btnXoa", "Xóa hóa đơn đã chọn." },
                    { "btnSua", "Sửa thông tin hóa đơn." },
                    { "btnThanhToan", "Xác nhận thanh toán hóa đơn." },
                    { "btnTimKiem", "Tìm kiếm hóa đơn." },
                    { "btnThoat", "Đóng form hóa đơn." },
                    { "dgvHoaDon", "Danh sách các hóa đơn." },
                    { "dgvChiTietHoaDon", "Chi tiết sản phẩm trong hóa đơn." },
                    { "btnXuat", "Xuất hóa đơn ra file Excel." }
                }
            },
            
            // Chi tiết hóa đơn Form
            { "frmChiTietHoaDon", new Dictionary<string, string>
                {
                    { "cboSanPham", "Chọn sản phẩm cần thêm vào hóa đơn." },
                    { "numDonGiaBan", "Giá bán của sản phẩm." },
                    { "numSoLuongBan", "Số lượng sản phẩm bán." },
                    { "btnThemSanPham", "Thêm sản phẩm vào hóa đơn." },
                    { "btnXoaSanPham", "Xóa sản phẩm khỏi hóa đơn." },
                    { "btnSuaSanPham", "Sửa thông tin sản phẩm trong hóa đơn." },
                    { "btnLuu", "Lưu thay đổi vào hóa đơn." },
                    { "btnThoat", "Đóng form chi tiết hóa đơn." },
                    { "dgvChiTietHoaDon", "Danh sách sản phẩm trong hóa đơn." }
                }
            }
        };

        // Method to apply help based on form name
        public static void ApplyHelpToForm(Form form)
        {
            // Detect form name and apply specific help
            string formName = form.Name;

            // Check if we have specific help text for this form
            if (FormHelpText.ContainsKey(formName))
            {
                ApplySpecificHelpToControls(form.Controls, FormHelpText[formName]);
            }
            else
            {
                // Fallback to generic help
                ApplyGenericHelpToControls(form.Controls);
            }
        }

        // Apply specific help text to controls
        private static void ApplySpecificHelpToControls(Control.ControlCollection controls, Dictionary<string, string> helpTexts)
        {
            foreach (Control control in controls)
            {
                if (helpTexts.ContainsKey(control.Name))
                {
                    HelpProvider.SetShowHelp(control, true);
                    HelpProvider.SetHelpString(control, helpTexts[control.Name]);
                }

                if (control.HasChildren)
                {
                    ApplySpecificHelpToControls(control.Controls, helpTexts);
                }
            }
        }

        // Apply generic help text to controls
        private static void ApplyGenericHelpToControls(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                if (AllHelpText.ContainsKey(control.Name))
                {
                    HelpProvider.SetShowHelp(control, true);
                    HelpProvider.SetHelpString(control, AllHelpText[control.Name]);
                }

                if (control.HasChildren)
                {
                    ApplyGenericHelpToControls(control.Controls);
                }
            }
        }
    }
}
