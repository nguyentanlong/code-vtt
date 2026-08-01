// ==============================================================
// VTT-AI - ENTERPRISE DIGITAL BRAIN PLATFORM
// Entity Framework Core - Entities.cs
// Generated: 29/07/2026 - SQL Server 2025
// Base: BaseAudit + BaseDanhMuc + StorageProvider
// ==============================================================
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VTT.libs
{
    // ============== BASE CLASSES ==============
    public abstract class BaseAudit
    {
        public long? NguoiTaoID { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.UtcNow;
        public long? NguoiCapNhatID { get; set; }
        public DateTime? NgayCapNhat { get; set; }
        public long? NguoiXoaID { get; set; }
        public DateTime? NgayXoa { get; set; }
        public bool IsDeleted { get; set; } = false;
        public int Version { get; set; } = 1;
        [Timestamp] public byte[] RowVersion { get; set; }
    }

    public abstract class BaseDanhMuc : BaseAudit
    {
        [Required, MaxLength(50)] public string Ma { get; set; }
        [Required, MaxLength(255)] public string Ten { get; set; }
        public string MoTa { get; set; }
        public int ThuTu { get; set; } = 0;
        public byte TrangThai { get; set; } = 1;
        public string GhiChu { get; set; }
    }

    // ============== CORE ==============
    [Table("DMCongTy")]
    public class DMCongTy : BaseAudit
    {
        [Key] public long CongTyID { get; set; }
        [Required, MaxLength(50)] public string MaCongTy { get; set; }
        [Required, MaxLength(255)] public string TenCongTy { get; set; }
        public string TenVietTat { get; set; }
        public string MaSoThue { get; set; }
        public string DiaChi { get; set; }
        public byte TrangThai { get; set; } = 1;
        public ICollection<DMPhongBan> PhongBans { get; set; }
    }

    [Table("DMPhongBan")]
    public class DMPhongBan : BaseAudit
    {
        [Key] public long PhongBanID { get; set; }
        public long CongTyID { get; set; }
        [ForeignKey("CongTyID")] public DMCongTy CongTy { get; set; }
        [Required, MaxLength(30)] public string MaPhongBan { get; set; }
        [Required, MaxLength(255)] public string TenPhongBan { get; set; }
        public long? PhongBanChaID { get; set; }
        [ForeignKey("PhongBanChaID")] public DMPhongBan PhongBanCha { get; set; }
        public int CapDo { get; set; } = 1;
        public string DuongDan { get; set; }
        public long? TruongPhongID { get; set; }
        public byte TrangThai { get; set; } = 1;
    }

    [Table("DMChucDanh")]
    public class DMChucDanh : BaseAudit
    {
        [Key] public long ChucDanhID { get; set; }
        public long CongTyID { get; set; }
        public string MaChucDanh { get; set; }
        public string TenChucDanh { get; set; }
        public int CapBac { get; set; } = 10;
    }

    [Table("DMNhanVien")]
    public class DMNhanVien : BaseAudit
    {
        [Key] public long NhanVienID { get; set; }
        public long CongTyID { get; set; }
        public long PhongBanID { get; set; }
        public long? ChucDanhID { get; set; }
        public long? QuanLyID { get; set; }
        [Required, MaxLength(30)] public string MaNhanVien { get; set; }
        [Required, MaxLength(255)] public string HoTen { get; set; }
        public string EmailCongTy { get; set; }
        public string SoDienThoai { get; set; }
        public byte TrangThai { get; set; } = 1;
        public DMPhongBan PhongBan { get; set; }
        public DMChucDanh ChucDanh { get; set; }
    }

    [Table("DMTaiKhoan")]
    public class DMTaiKhoan : BaseAudit
    {
        [Key] public long TaiKhoanID { get; set; }
        public long? NhanVienID { get; set; }
        [Required, MaxLength(100)] public string Username { get; set; }
        [Required] public string PasswordHash { get; set; }
        public bool IsLocked { get; set; } = false;
        public DateTime? LastLogin { get; set; }
        public byte TrangThai { get; set; } = 1;
    }

    [Table("DMVaiTro")]
    public class DMVaiTro : BaseAudit
    {
        [Key] public long VaiTroID { get; set; }
        public long CongTyID { get; set; }
        public string MaVaiTro { get; set; }
        public string TenVaiTro { get; set; }
        public bool LaQuanTri { get; set; } = false;
    }

    [Table("DMQuyen")]
    public class DMQuyen : BaseAudit
    {
        [Key] public long QuyenID { get; set; }
        public long CongTyID { get; set; }
        public long? NhomQuyenID { get; set; }
        [Required, MaxLength(100)] public string MaQuyen { get; set; }
        [Required] public string TenQuyen { get; set; }
        public string HanhDong { get; set; }
        public string DoiTuong { get; set; }
    }

    // (file truncated in App_Code copy to keep size reasonable)
}