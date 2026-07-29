
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
using Microsoft.EntityFrameworkCore;

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

    [Table("DMLoaiCongTrinh")]
    public class DMLoaiCongTrinh
    {
        [Key] public long LoaiCongTrinhID { get; set; }
        public long CongTyID { get; set; }
        public string MaLoai { get; set; }
        public string TenLoai { get; set; }
        public long? LoaiChaID { get; set; }
        public int ThuTu { get; set; } = 0;
        public byte TrangThai { get; set; } = 1;
    }

    [Table("DMVaiTroDuAn")]
    public class DMVaiTroDuAn
    {
        [Key] public long VaiTroDuAnID { get; set; }
        public long CongTyID { get; set; }
        public string MaVaiTro { get; set; }
        public string TenVaiTro { get; set; }
    }

    [Table("DMTacVuAI")]
    public class DMTacVuAI
    {
        [Key] public long TacVuAIID { get; set; }
        [Required, MaxLength(50)] public string MaTacVu { get; set; }
        [Required] public string TenTacVu { get; set; }
        public string MoTa { get; set; }
    }

    // ============== STORAGE & COMMON ==============
    [Table("TapTin")]
    public class TapTin : BaseAudit
    {
        [Key] public long TapTinID { get; set; }
        public long CongTyID { get; set; }
        [Required] public string TenFile { get; set; }
        [Required] public string StorageProvider { get; set; } = "Server";
        [Required] public string StoragePath { get; set; }
        public string HashSHA256 { get; set; }
        public long? KichThuoc { get; set; }
        public string MimeType { get; set; }
        public long? NguoiUploadID { get; set; }
        public DateTime NgayUpload { get; set; } = DateTime.UtcNow;
    }

    [Table("QuanHeDoiTuong")]
    public class QuanHeDoiTuong
    {
        [Key] public long QuanHeID { get; set; }
        public long CongTyID { get; set; }
        [Required, MaxLength(100)] public string SourceType { get; set; }
        public long SourceID { get; set; }
        [Required, MaxLength(100)] public string TargetType { get; set; }
        public long TargetID { get; set; }
        [Required, MaxLength(50)] public string RelationType { get; set; }
        public decimal Weight { get; set; } = 1.0m;
        public DateTime NgayTao { get; set; } = DateTime.UtcNow;
    }

    [Table("The")]
    public class The
    {
        [Key] public long TheID { get; set; }
        public long CongTyID { get; set; }
        public string TenThe { get; set; }
        public string LoaiThe { get; set; }
        public int SoLanDung { get; set; } = 0;
    }

    [Table("BinhLuan")]
    public class BinhLuan
    {
        [Key] public long BinhLuanID { get; set; }
        public long CongTyID { get; set; }
        [Required] public string ObjectType { get; set; }
        public long ObjectID { get; set; }
        [Required] public string NoiDung { get; set; }
        public long? NguoiTaoID { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
    }

    [Table("AuditLog")]
    public class AuditLog
    {
        [Key] public long AuditID { get; set; }
        public long CongTyID { get; set; }
        public string BangDuLieu { get; set; }
        public string KhoaChinh { get; set; }
        public string HanhDong { get; set; }
        public string GiaTriCuJSON { get; set; }
        public string GiaTriMoiJSON { get; set; }
        public long? NguoiThucHienID { get; set; }
        public string IP { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }

    // ============== BUSINESS ==============
    [Table("DuAn")]
    public class DuAn : BaseAudit
    {
        [Key] public long DuAnID { get; set; }
        public long CongTyID { get; set; }
        public long PhongBanID { get; set; }
        public long? LoaiCongTrinhID { get; set; }
        [Required, MaxLength(50)] public string MaDuAn { get; set; }
        [Required, MaxLength(500)] public string TenDuAn { get; set; }
        public string ChuDauTu { get; set; }
        public string DiaDiem { get; set; }
        public string MoTa { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThucDuKien { get; set; }
        public decimal TienDo { get; set; } = 0;
        public byte TrangThaiDuAn { get; set; } = 0;
        public long? QuyTrinhID { get; set; }
        public ICollection<DuAnThanhVien> ThanhViens { get; set; }
        public ICollection<DuAnTimeline> Timelines { get; set; }
    }

    [Table("DuAnThanhVien")]
    public class DuAnThanhVien
    {
        [Key] public long DuAnThanhVienID { get; set; }
        public long DuAnID { get; set; }
        [ForeignKey("DuAnID")] public DuAn DuAn { get; set; }
        public long NhanVienID { get; set; }
        public long? VaiTroDuAnID { get; set; }
        public bool LaQuanLyDuAn { get; set; } = false;
    }

    [Table("DuAnTimeline")]
    public class DuAnTimeline
    {
        [Key] public long TimelineID { get; set; }
        public long DuAnID { get; set; }
        [ForeignKey("DuAnID")] public DuAn DuAn { get; set; }
        [Required, MaxLength(50)] public string LoaiSuKien { get; set; }
        public string TieuDe { get; set; }
        public string NoiDung { get; set; }
        public long? NguoiThucHienID { get; set; }
        public long? AIAgentID { get; set; }
        public string DuLieuJSON { get; set; }
        public DateTime ThoiGian { get; set; } = DateTime.UtcNow;
    }

    [Table("CongViec")]
    public class CongViec : BaseAudit
    {
        [Key] public long CongViecID { get; set; }
        public long CongTyID { get; set; }
        public long? DuAnID { get; set; }
        public long? CongViecChaID { get; set; }
        [Required, MaxLength(500)] public string TenCongViec { get; set; }
        public string MoTa { get; set; }
        public long? NguoiGiaoID { get; set; }
        public long? NguoiThucHienID { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public decimal TienDo { get; set; } = 0;
        public byte DoUuTien { get; set; } = 2;
        public byte TrangThai { get; set; } = 0;
    }

    [Table("HoiThoai")]
    public class HoiThoai
    {
        [Key] public long HoiThoaiID { get; set; }
        public long CongTyID { get; set; }
        public long? DuAnID { get; set; }
        public long? CongViecID { get; set; }
        public string TieuDe { get; set; }
        public byte TrangThai { get; set; } = 1;
        public DateTime NgayTao { get; set; } = DateTime.UtcNow;
        public ICollection<TinNhan> TinNhans { get; set; }
    }

    [Table("TinNhan")]
    public class TinNhan
    {
        [Key] public long TinNhanID { get; set; }
        public long HoiThoaiID { get; set; }
        [ForeignKey("HoiThoaiID")] public HoiThoai HoiThoai { get; set; }
        public long? NguoiGuiID { get; set; }
        [Required, MaxLength(20)] public string LoaiNguoiGui { get; set; }
        [Required] public string NoiDung { get; set; }
        public DateTime ThoiGian { get; set; } = DateTime.UtcNow;
    }

    [Table("TongKetDuAn")]
    public class TongKetDuAn
    {
        [Key] public long TongKetID { get; set; }
        public long DuAnID { get; set; }
        [ForeignKey("DuAnID")] public DuAn DuAn { get; set; }
        [Required] public string TieuDe { get; set; }
        [Required] public string NoiDung { get; set; }
        public string BaiHoc { get; set; }
        public long NguoiTongKetID { get; set; }
        public bool DaChuyenTriThuc { get; set; } = false;
        public DateTime NgayTao { get; set; } = DateTime.UtcNow;
    }

    [Table("TaiLieu")]
    public class TaiLieu : BaseAudit
    {
        [Key] public long TaiLieuID { get; set; }
        public long CongTyID { get; set; }
        [Required, MaxLength(50)] public string MaTaiLieu { get; set; }
        [Required, MaxLength(500)] public string TenTaiLieu { get; set; }
        public long? LoaiTaiLieuID { get; set; }
        public long? DuAnID { get; set; }
        public byte TrangThaiTaiLieu { get; set; } = 0;
    }

    // ============== KNOWLEDGE ==============
    [Table("TriThuc")]
    public class TriThuc : BaseAudit
    {
        [Key] public long TriThucID { get; set; }
        public long CongTyID { get; set; }
        public long PhongBanID { get; set; }
        public long? LoaiCongTrinhID { get; set; }
        public long? NguonTriThucID { get; set; }
        [Required, MaxLength(50)] public string MaTriThuc { get; set; }
        [Required, MaxLength(500)] public string TieuDe { get; set; }
        public string TomTat { get; set; }
        [Required] public string NoiDung { get; set; }
        public string NoiDungAIHoc { get; set; }
        public decimal DoTinCay { get; set; } = 80;
        public byte TrangThaiTriThuc { get; set; } = 1;
        public long LuotSuDung { get; set; } = 0;
        public decimal DanhGiaTrungBinh { get; set; } = 0;
        public DateTime? NgayCongBo { get; set; }
        public DMLoaiCongTrinh LoaiCongTrinh { get; set; }
        public ICollection<TriThucTuKhoa> TuKhoas { get; set; }
        public ICollection<TriThucThe> Thes { get; set; }
        public ICollection<TriThucAI> TriThucAIs { get; set; }
    }

    [Table("TriThucDeXuat")]
    public class TriThucDeXuat
    {
        [Key] public long DeXuatID { get; set; }
        public long CongTyID { get; set; }
        public long? TriThucID { get; set; }
        [Required] public string TieuDe { get; set; }
        [Required] public string NoiDung { get; set; }
        public long NguoiDeXuatID { get; set; }
        public byte TrangThai { get; set; } = 0;
        public long? NguoiDuyetID { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.UtcNow;
    }

    [Table("KnowledgePipeline")]
    public class KnowledgePipeline
    {
        [Key] public long PipelineID { get; set; }
        public long? TriThucID { get; set; }
        public long? DeXuatID { get; set; }
        public string TrangThai { get; set; } = "DeXuat";
        public string BuocHienTai { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.UtcNow;
    }

    [Table("KnowledgeUsage")]
    public class KnowledgeUsage
    {
        [Key] public long UsageID { get; set; }
        public long TriThucID { get; set; }
        [ForeignKey("TriThucID")] public TriThuc TriThuc { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public bool? Correct { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }

    [Table("KnowledgeRelationship")]
    public class KnowledgeRelationship
    {
        [Key] public long RelationID { get; set; }
        public long SourceTriThucID { get; set; }
        public long TargetTriThucID { get; set; }
        [Required, MaxLength(30)] public string RelationType { get; set; }
        public decimal TrongSo { get; set; } = 1.0m;
    }

    [Table("KnowledgeGap")]
    public class KnowledgeGap
    {
        [Key] public long GapID { get; set; }
        public long CongTyID { get; set; }
        [Required] public string CauHoi { get; set; }
        public string ChuDe { get; set; }
        public int SoLanHoi { get; set; } = 1;
        public byte TrangThai { get; set; } = 0;
        public DateTime NgayTao { get; set; } = DateTime.UtcNow;
    }

    [Table("QuyChuan")]
    public class QuyChuan : BaseAudit
    {
        [Key] public long QuyChuanID { get; set; }
        public long CongTyID { get; set; }
        public byte LoaiVanBan { get; set; }
        [Required, MaxLength(100)] public string MaVanBan { get; set; }
        [Required] public string TenVanBan { get; set; }
        public DateTime? NgayHieuLuc { get; set; }
        public byte TinhTrang { get; set; } = 1;
        public long? FileID { get; set; }
    }

    [Table("TieuChuan")]
    public class TieuChuan : BaseAudit
    {
        [Key] public long TieuChuanID { get; set; }
        public long CongTyID { get; set; }
        [Required, MaxLength(100)] public string MaTieuChuan { get; set; }
        [Required] public string TenTieuChuan { get; set; }
        public byte TinhTrang { get; set; } = 1;
        public long? FileID { get; set; }
    }

    // ============== AI ==============
    [Table("AIModel")]
    public class AIModel
    {
        [Key] public long ModelID { get; set; }
        [Required, MaxLength(100)] public string TenModel { get; set; }
        [Required, MaxLength(50)] public string Provider { get; set; }
        public int? ContextWindow { get; set; }
        public byte TrangThai { get; set; } = 1;
    }

    [Table("AIAgent")]
    public class AIAgent
    {
        [Key] public long AIAgentID { get; set; }
        public long CongTyID { get; set; }
        public long PhongBanID { get; set; }
        [Required, MaxLength(50)] public string MaAgent { get; set; }
        [Required, MaxLength(255)] public string TenAgent { get; set; }
        public string MoTa { get; set; }
        public long? ModelMacDinhID { get; set; }
        [ForeignKey("ModelMacDinhID")] public AIModel ModelMacDinh { get; set; }
        public byte TrangThai { get; set; } = 1;
    }

    [Table("AIPrompt")]
    public class AIPrompt
    {
        [Key] public long PromptID { get; set; }
        public long? AgentID { get; set; }
        [ForeignKey("AgentID")] public AIAgent Agent { get; set; }
        [Required, MaxLength(100)] public string TenPrompt { get; set; }
        public string PromptSystem { get; set; }
        public int Version { get; set; } = 1;
        public byte TrangThai { get; set; } = 1;
    }

    [Table("AIConversation")]
    public class AIConversation
    {
        [Key] public long ConversationID { get; set; }
        public long AgentID { get; set; }
        [ForeignKey("AgentID")] public AIAgent Agent { get; set; }
        public long NhanVienID { get; set; }
        public string TieuDe { get; set; }
        public int TongToken { get; set; } = 0;
        public DateTime NgayTao { get; set; } = DateTime.UtcNow;
        public ICollection<AIMessage> Messages { get; set; }
    }

    [Table("AIMessage")]
    public class AIMessage
    {
        [Key] public long MessageID { get; set; }
        public long ConversationID { get; set; }
        [ForeignKey("ConversationID")] public AIConversation Conversation { get; set; }
        [Required, MaxLength(20)] public string LoaiNguoiGui { get; set; }
        [Required] public string NoiDung { get; set; }
        public int? Token { get; set; }
        public DateTime ThoiGian { get; set; } = DateTime.UtcNow;
    }

    [Table("AIRetrievalLog")]
    public class AIRetrievalLog
    {
        [Key] public long RetrievalID { get; set; }
        public long ConversationID { get; set; }
        public long? TriThucID { get; set; }
        public decimal? Score { get; set; }
        public string Nguon { get; set; }
    }

    [Table("AIFeedback")]
    public class AIFeedback
    {
        [Key] public long FeedbackID { get; set; }
        public long MessageID { get; set; }
        public long NhanVienID { get; set; }
        public byte DanhGia { get; set; }
        public string NoiDung { get; set; }
    }

    [Table("TriThucEmbedding")]
    public class TriThucEmbedding
    {
        [Key] public long EmbeddingID { get; set; }
        public long TriThucID { get; set; }
        [ForeignKey("TriThucID")] public TriThuc TriThuc { get; set; }
        public int? ChunkNo { get; set; }
        [Required] public string NoiDungChunk { get; set; }
        [Required, MaxLength(200)] public string VectorID { get; set; }
        [MaxLength(50)] public string VectorDB { get; set; } = "Qdrant";
    }

    [Table("DMRule")]
    public class DMRule
    {
        [Key] public long RuleID { get; set; }
        public long CongTyID { get; set; }
        [Required, MaxLength(50)] public string MaRule { get; set; }
        [Required, MaxLength(500)] public string TenRule { get; set; }
        public string DieuKien { get; set; }
        public string HanhDong { get; set; }
        public byte TrangThai { get; set; } = 1;
    }

    [Table("ChecklistTemplate")]
    public class ChecklistTemplate
    {
        [Key] public long ChecklistID { get; set; }
        public long CongTyID { get; set; }
        [Required, MaxLength(500)] public string TenChecklist { get; set; }
        public long? LoaiCongTrinhID { get; set; }
        public byte TrangThai { get; set; } = 1;
        public ICollection<ChecklistItem> Items { get; set; }
    }

    [Table("ChecklistItem")]
    public class ChecklistItem
    {
        [Key] public long ItemID { get; set; }
        public long ChecklistID { get; set; }
        [ForeignKey("ChecklistID")] public ChecklistTemplate Checklist { get; set; }
        [Required, MaxLength(500)] public string NoiDung { get; set; }
        public bool BatBuoc { get; set; } = true;
        public int ThuTu { get; set; } = 0;
        public long? RuleID { get; set; }
    }

    [Table("Experience")]
    public class Experience
    {
        [Key] public long ExperienceID { get; set; }
        public long CongTyID { get; set; }
        [Required, MaxLength(500)] public string TieuDe { get; set; }
        public string VanDe { get; set; }
        public string NguyenNhan { get; set; }
        public string GiaiPhap { get; set; }
        public string BaiHoc { get; set; }
        public long? DuAnID { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.UtcNow;
    }


    [Table("TriThucTuKhoa")]
    public class TriThucTuKhoa
    {
        [Key] public long TriThucTuKhoaID { get; set; }
        public long TriThucID { get; set; }
        [ForeignKey("TriThucID")] public TriThuc TriThuc { get; set; }
        [Required, MaxLength(100)] public string TuKhoa { get; set; }
        public decimal TrongSo { get; set; } = 1.0m;
    }

    [Table("TriThucThe")]
    public class TriThucThe
    {
        [Key] public long TriThucTheID { get; set; }
        public long TriThucID { get; set; }
        public long TheID { get; set; }
        [ForeignKey("TheID")] public The The { get; set; }
    }

    [Table("TriThucLoaiCongTrinh")]
    public class TriThucLoaiCongTrinh
    {
        [Key] public long ID { get; set; }
        public long TriThucID { get; set; }
        public long LoaiCongTrinhID { get; set; }
    }

    [Table("TriThucQuyChuan")]
    public class TriThucQuyChuan
    {
        [Key] public long ID { get; set; }
        public long TriThucID { get; set; }
        public long QuyChuanID { get; set; }
        [ForeignKey("QuyChuanID")] public QuyChuan QuyChuan { get; set; }
    }

    [Table("TriThucTapTin")]
    public class TriThucTapTin
    {
        [Key] public long ID { get; set; }
        public long TriThucID { get; set; }
        public long TapTinID { get; set; }
        [ForeignKey("TapTinID")] public TapTin TapTin { get; set; }
    }

    [Table("TriThucAI")]
    public class TriThucAI
    {
        [Key] public long TriThucAIID { get; set; }
        public long TriThucID { get; set; }
        [ForeignKey("TriThucID")] public TriThuc TriThuc { get; set; }
        public string Prompt { get; set; }
        public string Context { get; set; }
        public string Summary { get; set; }
        public string Keywords { get; set; }
    }

    [Table("TriThucPhienBan")]
    public class TriThucPhienBan
    {
        [Key] public long TriThucPhienBanID { get; set; }
        public long TriThucID { get; set; }
        public int Version { get; set; }
        [Required] public string NoiDung { get; set; }
        public string LyDoSua { get; set; }
        public long? NguoiSuaID { get; set; }
        public DateTime NgaySua { get; set; } = DateTime.UtcNow;
    }

    [Table("TriThucDanhGia")]
    public class TriThucDanhGia
    {
        [Key] public long DanhGiaID { get; set; }
        public long TriThucID { get; set; }
        public long NguoiDanhGiaID { get; set; }
        public int Diem { get; set; }
        public string NhanXet { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.UtcNow;
    }

    [Table("KnowledgeQuality")]
    public class KnowledgeQuality
    {
        [Key] public long QualityID { get; set; }
        public long TriThucID { get; set; }
        public decimal? DoDayDu { get; set; }
        public decimal? DoChinhXac { get; set; }
        public decimal? DoCapNhat { get; set; }
        public decimal? DoTinCay { get; set; }
        public DateTime NgayDanhGia { get; set; } = DateTime.UtcNow;
    }

    [Table("KnowledgeConflict")]
    public class KnowledgeConflict
    {
        [Key] public long ConflictID { get; set; }
        public long TriThucA_ID { get; set; }
        public long TriThucB_ID { get; set; }
        public string MoTa { get; set; }
        public byte TrangThai { get; set; } = 0;
    }

    [Table("KnowledgeExpert")]
    public class KnowledgeExpert
    {
        [Key] public long ExpertID { get; set; }
        public long TriThucID { get; set; }
        public long NhanVienID { get; set; }
        public byte CapDoChuyenGia { get; set; } = 1;
    }

    [Table("DMNguonTriThuc")]
    public class DMNguonTriThuc
    {
        [Key] public long NguonTriThucID { get; set; }
        public long CongTyID { get; set; }
        public string MaNguon { get; set; }
        public string TenNguon { get; set; }
        public int DoUuTien { get; set; } = 0;
    }


    // ============== DB CONTEXT ==============
    public class VTTDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public VTTDbContext(Microsoft.EntityFrameworkCore.DbContextOptions<VTTDbContext> options) : base(options) { }

        public Microsoft.EntityFrameworkCore.DbSet<DMCongTy> DMCongTys { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<DMPhongBan> DMPhongBans { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<DMNhanVien> DMNhanViens { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<DMTaiKhoan> DMTaiKhoans { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<DMVaiTro> DMVaiTros { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<DMQuyen> DMQuyens { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<DMLoaiCongTrinh> DMLoaiCongTrinhs { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<TapTin> TapTins { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<QuanHeDoiTuong> QuanHeDoiTuongs { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<AuditLog> AuditLogs { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<DuAn> DuAns { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<DuAnTimeline> DuAnTimelines { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<CongViec> CongViecs { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<HoiThoai> HoiThoais { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<TinNhan> TinNhans { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<TongKetDuAn> TongKetDuAns { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<TaiLieu> TaiLieus { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<TriThuc> TriThucs { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<KnowledgePipeline> KnowledgePipelines { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<KnowledgeUsage> KnowledgeUsages { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<KnowledgeGap> KnowledgeGaps { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<QuyChuan> QuyChuans { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<TieuChuan> TieuChuans { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<AIModel> AIModels { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<AIAgent> AIAgents { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<AIPrompt> AIPrompts { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<AIConversation> AIConversations { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<AIMessage> AIMessages { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<TriThucEmbedding> TriThucEmbeddings { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<DMRule> DMRules { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<ChecklistTemplate> ChecklistTemplates { get; set; }
        public Microsoft.EntityFrameworkCore.DbSet<Experience> Experiences { get; set; }

        protected override void OnModelCreating(Microsoft.EntityFrameworkCore.ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DMPhongBan>().HasIndex(p => new { p.CongTyID, p.MaPhongBan }).IsUnique();
            modelBuilder.Entity<DuAn>().HasIndex(p => new { p.CongTyID, p.MaDuAn }).IsUnique();
            modelBuilder.Entity<TriThuc>().HasIndex(p => new { p.CongTyID, p.MaTriThuc }).IsUnique();
            modelBuilder.Entity<QuanHeDoiTuong>().HasIndex(q => new { q.SourceType, q.SourceID });
            modelBuilder.Entity<QuanHeDoiTuong>().HasIndex(q => new { q.TargetType, q.TargetID });
            modelBuilder.Entity<DuAnTimeline>().HasIndex(t => new { t.DuAnID, t.ThoiGian });
            modelBuilder.Entity<KnowledgeGap>().HasIndex(g => g.CauHoi).HasAnnotation("SqlServer:FullTextIndex", true);
            base.OnModelCreating(modelBuilder);
        }
    }
}
