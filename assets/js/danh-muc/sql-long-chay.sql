/*CREATE PROCEDURE sp_long_DMVaiTroDuAn_GetList
    @Keyword NVARCHAR(200) = NULL,
    @CongTyID BIGINT = NULL
AS
BEGIN
    SELECT v.VaiTroDuAnID, v.CongTyID, ct.TenCongTy, v.MaVaiTro, v.TenVaiTro, v.ThuTu
    FROM DMVaiTroDuAn v
        INNER JOIN DMCongTy ct ON ct.CongTyID = v.CongTyID
    WHERE (@Keyword IS NULL OR v.MaVaiTro LIKE '%' + @Keyword + '%' OR v.TenVaiTro LIKE '%' + @Keyword + '%')
        AND (@CongTyID IS NULL OR v.CongTyID = @CongTyID)
    ORDER BY v.ThuTu ASC;
END
GO
CREATE PROCEDURE sp_long_DMVaiTroDuAn_Save
    @VaiTroDuAnID BIGINT,
    @CongTyID BIGINT,
    @MaVaiTro NVARCHAR(50),
    @TenVaiTro NVARCHAR(200),
    @ThuTu INT
AS
BEGIN
    IF @VaiTroDuAnID = 0
        INSERT INTO DMVaiTroDuAn
        (CongTyID, MaVaiTro, TenVaiTro, ThuTu)
    VALUES
        (@CongTyID, @MaVaiTro, @TenVaiTro, @ThuTu);
    ELSE
        UPDATE DMVaiTroDuAn
        SET CongTyID = @CongTyID, MaVaiTro = @MaVaiTro, TenVaiTro = @TenVaiTro, ThuTu = @ThuTu
        WHERE VaiTroDuAnID = @VaiTroDuAnID;
END
GO
CREATE PROCEDURE sp_long_DMVaiTroDuAn_Delete
    @VaiTroDuAnID BIGINT
AS
BEGIN
    DELETE FROM DMVaiTroDuAn WHERE VaiTroDuAnID = @VaiTroDuAnID;
END
GO
===Thêm để test==
INSERT INTO dbo.DMVaiTroDuAn (CongTyID, MaVaiTro, TenVaiTro, ThuTu)
VALUES (1, 'TL', N'Quản lý dự án', 1);
INSERT INTO dbo.DMVaiTroDuAn (CongTyID, MaVaiTro, TenVaiTro, ThuTu)
VALUES (1, 'TL1', N'Giám sát thi công', 2);
INSERT INTO dbo.DMVaiTroDuAn (CongTyID, MaVaiTro, TenVaiTro, ThuTu)
VALUES (1, 'Tl2', N'Kế toán dự án', 3);
INSERT INTO dbo.DMVaiTroDuAn (CongTyID, MaVaiTro, TenVaiTro, ThuTu)
VALUES (1, 'TL3', N'Pháp lý dự án', 4);
INSERT INTO dbo.DMVaiTroDuAn (CongTyID, MaVaiTro, TenVaiTro, ThuTu)
VALUES (1, 'TL4', N'Thiết kế thi công', 5);*/
/* thêm nhóm quyền
INSERT INTO dbo.DMNhomQuyen
    (CongTyID, MaNhomQuyen, TenNhomQuyen, ThuTu, TrangThai, NgayTao)
VALUES
    (1, 'SALE_MGR', N'Trưởng phòng kinh doanh', 1, 1, '2026-08-03 09:59:00');
INSERT INTO dbo.DMNhomQuyen
    (CongTyID, MaNhomQuyen, TenNhomQuyen, ThuTu, TrangThai, NgayTao)
VALUES
    (1, 'ACC_ACC', N'Trưởng phòng kế toán', 1, 1, '2026-08-03 10:00:00');
INSERT INTO dbo.DMNhomQuyen
    (CongTyID, MaNhomQuyen, TenNhomQuyen, ThuTu, TrangThai, NgayTao)
VALUES
    (1, 'BUILD_MGR', N'Trưởng phòng công trinh', 1, 1, '2026-08-03 10:05:00');

-- 1. Lấy danh sách
CREATE OR ALTER PROCEDURE dbo.sp_long_DMNhomQuyen_GetList
    @Keyword NVARCHAR(255) = NULL,
    @TrangThai TINYINT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        NhomQuyenID,
        MaNhomQuyen,
        TenNhomQuyen,
        MoTa,
        TrangThai,
        CONVERT(VARCHAR(10), NgayTao, 103) AS NgayTaoText
    FROM dbo.DMNhomQuyen WITH (NOLOCK)
    WHERE IsDeleted = 0
        AND (@Keyword IS NULL OR @Keyword = '' OR MaNhomQuyen LIKE '%' + @Keyword + '%' OR TenNhomQuyen LIKE '%' + @Keyword + '%')
        AND (@TrangThai IS NULL OR TrangThai = @TrangThai)
    ORDER BY NhomQuyenID DESC;
END;
GO

-- 2. Lấy chi tiết 1 record
CREATE OR ALTER PROCEDURE dbo.sp_long_DMNhomQuyen_GetById
    @NhomQuyenID BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        NhomQuyenID, MaNhomQuyen, TenNhomQuyen, MoTa, TrangThai
    FROM dbo.DMNhomQuyen WITH (NOLOCK)
    WHERE NhomQuyenID = @NhomQuyenID AND IsDeleted = 0;
END;
GO

-- 3. Lưu (Thêm mới / Cập nhật)
CREATE OR ALTER PROCEDURE dbo.sp_long_DMNhomQuyen_Save
    @NhomQuyenID BIGINT = 0,
    @MaNhomQuyen VARCHAR(50),
    @TenNhomQuyen NVARCHAR(255),
    @MoTa NVARCHAR(MAX) = NULL,
    @TrangThai TINYINT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1
    FROM dbo.DMNhomQuyen
    WHERE MaNhomQuyen = @MaNhomQuyen AND NhomQuyenID <> @NhomQuyenID AND IsDeleted = 0)
    BEGIN
        RAISERROR(N'Mã nhóm quyền đã tồn tại trong hệ thống!', 16, 1);
        RETURN;
    END

    IF @NhomQuyenID = 0
    BEGIN
        INSERT INTO dbo.DMNhomQuyen
            (MaNhomQuyen, TenNhomQuyen, MoTa, TrangThai, NgayTao, IsDeleted, Version)
        VALUES
            (@MaNhomQuyen, @TenNhomQuyen, @MoTa, @TrangThai, SYSUTCDATETIME(), 0, 1);
    END
    ELSE
    BEGIN
        UPDATE dbo.DMNhomQuyen
        SET MaNhomQuyen = @MaNhomQuyen,
            TenNhomQuyen = @TenNhomQuyen,
            MoTa = @MoTa,
            TrangThai = @TrangThai,
            Version = Version + 1
        WHERE NhomQuyenID = @NhomQuyenID AND IsDeleted = 0;
    END
END;
GO

-- 4. Xóa mềm
CREATE OR ALTER PROCEDURE dbo.sp_long_DMNhomQuyen_Delete
    @NhomQuyenID BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.DMNhomQuyen
    SET IsDeleted = 1
    WHERE NhomQuyenID = @NhomQuyenID;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_long_DMNhomQuyen_GetList
    @CongTyID BIGINT,
    @Keyword NVARCHAR(255) = NULL,
    @TrangThai TINYINT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        NhomQuyenID,
        CongTyID,
        MaNhomQuyen,
        TenNhomQuyen,
        ThuTu,
        TrangThai,
        CONVERT(VARCHAR(10), NgayTao, 103) AS NgayTaoText
    FROM dbo.DMNhomQuyen WITH (NOLOCK)
    WHERE CongTyID = @CongTyID
        AND (@Keyword IS NULL OR @Keyword = '' OR MaNhomQuyen LIKE '%' + @Keyword + '%' OR TenNhomQuyen LIKE '%' + @Keyword + '%')
        AND (@TrangThai IS NULL OR TrangThai = @TrangThai)
    ORDER BY ThuTu ASC, NhomQuyenID DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_long_DMNhomQuyen_GetById
    @NhomQuyenID BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT NhomQuyenID, CongTyID, MaNhomQuyen, TenNhomQuyen, ThuTu, TrangThai
    FROM dbo.DMNhomQuyen WITH (NOLOCK)
    WHERE NhomQuyenID = @NhomQuyenID;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_long_DMNhomQuyen_Save
    @NhomQuyenID BIGINT = 0,
    @CongTyID BIGINT,
    @MaNhomQuyen VARCHAR(50),
    @TenNhomQuyen NVARCHAR(255),
    @ThuTu INT = 0,
    @TrangThai TINYINT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1
    FROM dbo.DMNhomQuyen
    WHERE CongTyID = @CongTyID AND MaNhomQuyen = @MaNhomQuyen AND NhomQuyenID <> @NhomQuyenID)
    BEGIN
        RAISERROR(N'Mã nhóm quyền đã tồn tại trong công ty này!', 16, 1);
        RETURN;
    END

    IF @NhomQuyenID = 0
    BEGIN
        INSERT INTO dbo.DMNhomQuyen
            (CongTyID, MaNhomQuyen, TenNhomQuyen, ThuTu, TrangThai, NgayTao)
        VALUES
            (@CongTyID, @MaNhomQuyen, @TenNhomQuyen, @ThuTu, @TrangThai, SYSUTCDATETIME());
    END
    ELSE
    BEGIN
        UPDATE dbo.DMNhomQuyen
        SET MaNhomQuyen = @MaNhomQuyen,
            TenNhomQuyen = @TenNhomQuyen,
            ThuTu = @ThuTu,
            TrangThai = @TrangThai
        WHERE NhomQuyenID = @NhomQuyenID AND CongTyID = @CongTyID;
    END
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_long_DMNhomQuyen_Delete
    @NhomQuyenID BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    -- DMNhomQuyen không có IsDeleted -> xóa cứng.
    -- Chặn xóa nếu đang có Quyền (DMQuyen) tham chiếu tới Nhóm quyền này
    IF EXISTS (SELECT 1
    FROM dbo.DMQuyen
    WHERE NhomQuyenID = @NhomQuyenID)
    BEGIN
        RAISERROR(N'Không thể xóa vì Nhóm quyền này đang có Quyền trực thuộc!', 16, 1);
        RETURN;
    END

    DELETE FROM dbo.DMNhomQuyen WHERE NhomQuyenID = @NhomQuyenID;
END;
GO
*/

/*bảng tác vụ AI
-- 1. Lấy danh sách Phòng ban (dropdown) theo Công ty hiện tại
CREATE OR ALTER PROCEDURE dbo.sp_long_AIAgent_GetPhongBanOptions
    @CongTyID BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT PhongBanID, TenPhongBan
    FROM dbo.DMPhongBan WITH (NOLOCK)
    WHERE CongTyID = @CongTyID AND IsDeleted = 0 AND TrangThai = 1
    ORDER BY TenPhongBan ASC;
END;
GO

-- 2. Lấy danh sách AIModel đang hoạt động (dropdown, dùng chung toàn hệ thống)
CREATE OR ALTER PROCEDURE dbo.sp_long_AIAgent_GetModelOptions
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ModelID, TenModel, Provider
    FROM dbo.AIModel WITH (NOLOCK)
    WHERE TrangThai = 1
    ORDER BY TenModel ASC;
END;
GO

-- 3. Lấy danh sách Tác nhân AI
CREATE OR ALTER PROCEDURE dbo.sp_long_AIAgent_GetList
    @CongTyID BIGINT,
    @Keyword NVARCHAR(255) = NULL,
    @TrangThai TINYINT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        ag.AIAgentID,
        ag.CongTyID,
        ag.PhongBanID,
        pb.TenPhongBan,
        ag.MaAgent,
        ag.TenAgent,
        ag.ModelMacDinhID,
        md.TenModel,
        ag.TrangThai
    FROM dbo.AIAgent ag
    LEFT JOIN dbo.DMPhongBan pb ON pb.PhongBanID = ag.PhongBanID AND pb.IsDeleted = 0
    LEFT JOIN dbo.AIModel md ON md.ModelID = ag.ModelMacDinhID
    WHERE ag.CongTyID = @CongTyID
      AND (@Keyword IS NULL OR @Keyword = '' OR ag.MaAgent LIKE '%' + @Keyword + '%' OR ag.TenAgent LIKE '%' + @Keyword + '%')
      AND (@TrangThai IS NULL OR ag.TrangThai = @TrangThai)
    ORDER BY ag.AIAgentID DESC;
END;
GO

-- 4. Lấy chi tiết 1 Tác nhân AI
CREATE OR ALTER PROCEDURE dbo.sp_long_AIAgent_GetById
    @AIAgentID BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT AIAgentID, CongTyID, PhongBanID, MaAgent, TenAgent, ModelMacDinhID, TrangThai
    FROM dbo.AIAgent WITH (NOLOCK)
    WHERE AIAgentID = @AIAgentID;
END;
GO

-- 5. Lưu (Thêm mới / Cập nhật)
CREATE OR ALTER PROCEDURE dbo.sp_long_AIAgent_Save
    @AIAgentID BIGINT = 0,
    @CongTyID BIGINT,
    @PhongBanID BIGINT,
    @MaAgent VARCHAR(50),
    @TenAgent NVARCHAR(255),
    @ModelMacDinhID BIGINT = NULL,
    @TrangThai TINYINT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.AIAgent WHERE CongTyID = @CongTyID AND MaAgent = @MaAgent AND AIAgentID <> @AIAgentID)
    BEGIN
        RAISERROR(N'Mã tác nhân AI đã tồn tại trong công ty này!', 16, 1);
        RETURN;
    END

    IF @AIAgentID = 0
    BEGIN
        INSERT INTO dbo.AIAgent (CongTyID, PhongBanID, MaAgent, TenAgent, ModelMacDinhID, TrangThai)
        VALUES (@CongTyID, @PhongBanID, @MaAgent, @TenAgent, @ModelMacDinhID, @TrangThai);
    END
    ELSE
    BEGIN
        UPDATE dbo.AIAgent
        SET PhongBanID = @PhongBanID,
            MaAgent = @MaAgent,
            TenAgent = @TenAgent,
            ModelMacDinhID = @ModelMacDinhID,
            TrangThai = @TrangThai
        WHERE AIAgentID = @AIAgentID AND CongTyID = @CongTyID;
    END
END;
GO

-- 6. Xóa (kiểm tra ràng buộc trước khi xóa cứng)
CREATE OR ALTER PROCEDURE dbo.sp_long_AIAgent_Delete
    @AIAgentID BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM dbo.AIPrompt WHERE AgentID = @AIAgentID)
       OR EXISTS (SELECT 1 FROM dbo.AIConversation WHERE AgentID = @AIAgentID)
       OR EXISTS (SELECT 1 FROM dbo.AITrainingHistory WHERE AgentID = @AIAgentID)
       OR EXISTS (SELECT 1 FROM dbo.AITokenUsage WHERE AgentID = @AIAgentID)
       OR EXISTS (SELECT 1 FROM dbo.AIConfiguration WHERE AgentID = @AIAgentID)
    BEGIN
        RAISERROR(N'Không thể xóa vì Tác nhân AI này đang được sử dụng trong hệ thống (Prompt/Hội thoại/Cấu hình...)!', 16, 1);
        RETURN;
    END

    DELETE FROM dbo.AIAgent WHERE AIAgentID = @AIAgentID;
END;
GO


INSERT INTO dbo.AIAgent (CongTyID, PhongBanID, MaAgent, TenAgent, ModelMacDinhID)
VALUES 
(1, 1, 'SALE_ASSIST', N'Trợ lý tư vấn bán hàng', 1),
(1, 1, 'HR_RECRUIT', N'Robot lọc hồ sơ tuyển dụng', NULL),
(1, 1, 'MARK_ANALYS', N'Agent phân tích thị trường', 2);


INSERT INTO dbo.AIModel (TenModel, Provider, ContextWindow)
VALUES 
('gpt-4o', 'OpenAI', 128000),
('gpt-4-turbo', 'OpenAI', 128000),
('claude-3-5-sonnet', 'Anthropic', 200000),
('gemini-1.5-pro', 'Google', 2000000),
('llama-3.1-70b', 'Meta', 128000);

*/