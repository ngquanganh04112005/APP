USE master;
GO
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'QLDoAn_CNTT')
BEGIN
    ALTER DATABASE QLDoAn_CNTT SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QLDoAn_CNTT;
END
GO
CREATE DATABASE QLDoAn_CNTT;
GO
USE QLDoAn_CNTT;
GO


CREATE TABLE chuc_vu (
    ma_chuc_vu NVARCHAR(10) PRIMARY KEY,
    ten_chuc_vu NVARCHAR(50) NOT NULL
);


CREATE TABLE loai_do_an (
    ma_loai NVARCHAR(10) PRIMARY KEY,
    ten_loai NVARCHAR(50) NOT NULL
);


CREATE TABLE giang_vien (
    ma_giang_vien NVARCHAR(10) PRIMARY KEY,
    ten_giang_vien NVARCHAR(100) NOT NULL,
    ma_chuc_vu NVARCHAR(10),
    so_dien_thoai NVARCHAR(15),
    email NVARCHAR(100),
    mat_khau NVARCHAR(100) DEFAULT '123456',
    FOREIGN KEY (ma_chuc_vu) REFERENCES chuc_vu(ma_chuc_vu) ON DELETE SET NULL
);


CREATE TABLE sinh_vien (
    ma_sinh_vien NVARCHAR(10) PRIMARY KEY,
    ten_sinh_vien NVARCHAR(100) NOT NULL,
    ten_lop NVARCHAR(50) NOT NULL,      
    ngay_sinh DATE,
    gioi_tinh NVARCHAR(5),
    so_dien_thoai NVARCHAR(15),
    email NVARCHAR(100)
);


CREATE TABLE do_an (
    ma_do_an NVARCHAR(10) PRIMARY KEY,
    ten_do_an NVARCHAR(200) NOT NULL,
    ma_sinh_vien NVARCHAR(10) NOT NULL,
    ma_giang_vien_huong_dan NVARCHAR(10),
    ma_loai NVARCHAR(10),
    ngay_nop DATE,
    trang_thai NVARCHAR(50) DEFAULT N'Đang thực hiện',
    file_dinh_kem NVARCHAR(255),
    FOREIGN KEY (ma_sinh_vien) REFERENCES sinh_vien(ma_sinh_vien) ON DELETE CASCADE,
    FOREIGN KEY (ma_giang_vien_huong_dan) REFERENCES giang_vien(ma_giang_vien) ON DELETE SET NULL,
    FOREIGN KEY (ma_loai) REFERENCES loai_do_an(ma_loai) ON DELETE SET NULL
);


CREATE TABLE thong_bao (
    ma_thong_bao INT IDENTITY(1,1) PRIMARY KEY,
    tieu_de NVARCHAR(200) NOT NULL,
    noi_dung NVARCHAR(MAX),
    ngay_dang DATETIME DEFAULT GETDATE(),
    nguoi_dang NVARCHAR(100),
    doi_tuong NVARCHAR(50) 
);


CREATE TABLE lop (
    malop NVARCHAR(10) PRIMARY KEY,
    tenlop NVARCHAR(50) NOT NULL
)

INSERT INTO lop(malop, tenlop)
VALUES
(N'CNTT-K12', N'CNTT-K12'),
(N'CNTT-K13', N'CNTT-K13'),
(N'CNTT-K14', N'CNTT-K14');

INSERT INTO chuc_vu (ma_chuc_vu, ten_chuc_vu) VALUES
(N'CV01', N'Trưởng bộ môn'),
(N'CV02', N'Phó bộ môn'),
(N'CV03', N'Giảng viên');


INSERT INTO loai_do_an (ma_loai, ten_loai) VALUES
(N'LDA01', N'Đồ án'),
(N'LDA02', N'Khóa luận'),
(N'LDA03', N'Tiểu luận');


INSERT INTO giang_vien (ma_giang_vien, ten_giang_vien, ma_chuc_vu, mat_khau, so_dien_thoai, email) VALUES
(N'2300189', N'Doãn Chí Bình', N'CV01', N'4112005', N'0123456789', N'ngquanganh411205@gmail.com'),
(N'GV002', N'Lý Mạc Sầu', N'CV02', N'123456', N'0123456789', N'eraser411205@gmail.com'),
(N'GV003', N'Từ Phượng Niên', N'CV03', N'123456', N'0123456789', N'tpn@gmail.com');


INSERT INTO sinh_vien (ma_sinh_vien, ten_sinh_vien, ten_lop, ngay_sinh, gioi_tinh, so_dien_thoai, email) VALUES
(N'SV001', N'Nguyễn Quang Anh', N'CNTT-K12', '2005-11-04', N'Nam', '0123456789', 'dcb@gmail.com'),
(N'SV002', N'Trần Văn Bình', N'CNTT-K13', '2005-02-20', N'Nam', '0123456789', 'tvb@gmail.com'),
(N'SV003', N'Lê Thị Mai', N'CNTT-K11', '2005-08-15', N'Nữ', '0123456789', 'ltm@gmail.com');


INSERT INTO do_an (ma_do_an, ten_do_an, ma_sinh_vien, ma_giang_vien_huong_dan, ma_loai, ngay_nop) VALUES
(N'DT001', N'Hệ thống quản lý điểm sinh viên trường Đại học Thái Bình', N'SV001', N'2300189', N'LDA01', '2026-05-19'),
(N'DT002', N'Xây dựng ứng dụng di động Flutter cho quản lý sự kiện', N'SV002', N'2300189', N'LDA02', '2026-05-20'),
(N'DT003', N'Phân tích bảo mật mạng không dây trong môi trường giáo dục', N'SV003', N'2300189', N'LDA03', '2026-05-21');


INSERT INTO thong_bao (tieu_de, noi_dung, nguoi_dang, doi_tuong) VALUES
(N'Thời gian nộp đồ án HK1 2025-2026', N'Các sinh viên CNTT-K12 vui lòng nộp đồ án trước ngày 10/06/2026.', N'TS. Doãn Chí Bình', N'sinh_vien'),
(N'Thời gian nộp đồ án HK1 2025-2026', N'Các sinh viên CNTT-K13 vui lòng nộp đồ án trước ngày 20/06/2026.', N'TS. Doãn Chí Bình', N'sinh_vien'),
(N'Thời gian nộp đồ án HK1 2025-2026', N'Các sinh viên CNTT-K14 vui lòng nộp đồ án trước ngày 30/06/2026.', N'TS. Doãn Chí Bình', N'sinh_vien');
GO