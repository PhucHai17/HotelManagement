DROP TABLE IF EXISTS ChiTietSuDungDichVu;
DROP TABLE IF EXISTS DichVuSuDung;
DROP TABLE IF EXISTS HoaDon;
DROP TABLE IF EXISTS ChiTietCheckIn;
DROP TABLE IF EXISTS DatPhong;
DROP TABLE IF EXISTS Phong;
DROP TABLE IF EXISTS LoaiPhong;
DROP TABLE IF EXISTS DichVu;
DROP TABLE IF EXISTS NhanVien;
DROP TABLE IF EXISTS KhachHang;
DROP TABLE IF EXISTS TaiKhoan;

CREATE TABLE TaiKhoan
(
    MaTaiKhoan  INTEGER PRIMARY KEY AUTOINCREMENT,
    TenDangNhap TEXT NOT NULL UNIQUE,
    MatKhau     TEXT NOT NULL,
    VaiTro      TEXT NOT NULL CHECK (VaiTro IN ('KhachHang', 'LeTan')),
    NgayTao     TEXT NOT NULL DEFAULT (datetime('now', 'localtime'))
);

CREATE TABLE NhanVien
(
    MaNV        INTEGER PRIMARY KEY AUTOINCREMENT,
    HoTen       TEXT    NOT NULL,
    SoDienThoai TEXT,
    Email       TEXT,
    ChucVu      TEXT,
    MaTaiKhoan  INTEGER NOT NULL UNIQUE,
    FOREIGN KEY (MaTaiKhoan) REFERENCES TaiKhoan (MaTaiKhoan)
);

CREATE TABLE KhachHang
(
    MaKH         INTEGER PRIMARY KEY AUTOINCREMENT,
    HoTen        TEXT    NOT NULL,
    SoDienThoai  TEXT,
    Email        TEXT,
    CCCD_HoChieu TEXT UNIQUE,
    GhiChu       TEXT,
    SoLanO       INTEGER NOT NULL DEFAULT 0,
    MaTaiKhoan   INTEGER UNIQUE,
    FOREIGN KEY (MaTaiKhoan) REFERENCES TaiKhoan (MaTaiKhoan)
);

CREATE TABLE LoaiPhong
(
    MaLoaiPhong  INTEGER PRIMARY KEY AUTOINCREMENT,
    TenLoaiPhong TEXT    NOT NULL UNIQUE,
    GiaTheoDem   REAL    NOT NULL CHECK (GiaTheoDem >= 0),
    SoKhachToiDa INTEGER NOT NULL DEFAULT 2,
    TienNghi     TEXT,
    HinhAnh      TEXT
);

CREATE TABLE Phong
(
    MaPhong     INTEGER PRIMARY KEY AUTOINCREMENT,
    SoPhong     TEXT    NOT NULL UNIQUE,
    Tang        INTEGER NOT NULL,
    TrangThai   TEXT    NOT NULL DEFAULT 'Trong'
        CHECK (TrangThai IN ('Trong', 'CoKhach', 'DaDat', 'DangDon')),
    MaLoaiPhong INTEGER NOT NULL,
    FOREIGN KEY (MaLoaiPhong) REFERENCES LoaiPhong (MaLoaiPhong)
);

CREATE TABLE DichVu
(
    MaDichVu  INTEGER PRIMARY KEY AUTOINCREMENT,
    TenDichVu TEXT NOT NULL UNIQUE,
    DonGia    REAL NOT NULL CHECK (DonGia >= 0),
    DonViTinh TEXT NOT NULL,
    MoTa      TEXT
);

CREATE TABLE DatPhong
(
    MaDatPhong    INTEGER PRIMARY KEY AUTOINCREMENT,
    MaKH          INTEGER NOT NULL,
    MaPhong       INTEGER NOT NULL,
    NgayNhanPhong TEXT    NOT NULL,
    NgayTraPhong  TEXT    NOT NULL,
    SoKhach       INTEGER NOT NULL DEFAULT 1,
    TrangThai     TEXT    NOT NULL DEFAULT 'ChoDuyet'
        CHECK (TrangThai IN ('ChoDuyet', 'DaXacNhan', 'DaNhanPhong', 'DaHoanTat', 'DaHuy')),
    NgayTao       TEXT    NOT NULL DEFAULT (datetime('now', 'localtime')),
    CHECK (NgayTraPhong > NgayNhanPhong),
    FOREIGN KEY (MaKH) REFERENCES KhachHang (MaKH),
    FOREIGN KEY (MaPhong) REFERENCES Phong (MaPhong)
);

CREATE TABLE ChiTietCheckIn
(
    MaCheckIn          INTEGER PRIMARY KEY AUTOINCREMENT,
    MaDatPhong         INTEGER NOT NULL UNIQUE,
    MaNV               INTEGER NOT NULL,
    NgayCheckInThucTe  TEXT    NOT NULL DEFAULT (datetime('now', 'localtime')),
    NgayCheckOutThucTe TEXT,
    SoCCCD_KiemTra     TEXT    NOT NULL,
    SoKhachDiCung      INTEGER NOT NULL DEFAULT 0,
    FOREIGN KEY (MaDatPhong) REFERENCES DatPhong (MaDatPhong),
    FOREIGN KEY (MaNV) REFERENCES NhanVien (MaNV)
);

CREATE TABLE ChiTietSuDungDichVu
(
    MaChiTietDV INTEGER PRIMARY KEY AUTOINCREMENT,
    MaDatPhong  INTEGER NOT NULL,
    MaDichVu    INTEGER NOT NULL,
    SoLuong     INTEGER NOT NULL DEFAULT 1 CHECK (SoLuong > 0),
    DonGiaLuu   REAL    NOT NULL,
    ThanhTien   REAL    NOT NULL,
    NgaySuDung  TEXT    NOT NULL DEFAULT (datetime('now', 'localtime')),
    FOREIGN KEY (MaDatPhong) REFERENCES DatPhong (MaDatPhong),
    FOREIGN KEY (MaDichVu) REFERENCES DichVu (MaDichVu)
);

CREATE TABLE HoaDon
(
    MaHoaDon          INTEGER PRIMARY KEY AUTOINCREMENT,
    MaDatPhong        INTEGER NOT NULL UNIQUE,
    MaNV              INTEGER NOT NULL,
    NgayLap           TEXT    NOT NULL DEFAULT (datetime('now', 'localtime')),
    TienPhong         REAL    NOT NULL DEFAULT 0,
    TienDichVu        REAL    NOT NULL DEFAULT 0,
    PhuThu_GiamGia    REAL    NOT NULL DEFAULT 0,
    VAT               REAL    NOT NULL DEFAULT 0,
    TongTien          REAL    NOT NULL DEFAULT 0,
    HinhThucThanhToan TEXT CHECK (HinhThucThanhToan IN ('TienMat', 'The', 'ChuyenKhoan')),
    TrangThai         TEXT    NOT NULL DEFAULT 'ChuaThanhToan'
        CHECK (TrangThai IN ('ChuaThanhToan', 'DaThanhToan')),
    FOREIGN KEY (MaDatPhong) REFERENCES DatPhong (MaDatPhong),
    FOREIGN KEY (MaNV) REFERENCES NhanVien (MaNV)
);

INSERT INTO LoaiPhong (TenLoaiPhong, GiaTheoDem, SoKhachToiDa, TienNghi)
VALUES ('Standard', 500000, 2, 'Wi-Fi, Điều hòa, TV'),
       ('Deluxe', 800000, 2, 'Wi-Fi, Điều hòa, TV, Ban công, Minibar, Bồn tắm'),
       ('Suite', 1500000, 4, 'Wi-Fi, Điều hòa, TV, Ban công, Minibar, Bồn tắm');

INSERT INTO Phong (SoPhong, Tang, MaLoaiPhong, TrangThai)
VALUES ('101', 1, 1, 'Trong'),
       ('102', 1, 2, 'CoKhach'),
       ('103', 1, 1, 'DaDat'),
       ('104', 1, 1, 'DangDon'),
       ('105', 1, 2, 'Trong'),
       ('106', 1, 3, 'CoKhach'),
       ('201', 2, 1, 'CoKhach'),
       ('202', 2, 2, 'Trong'),
       ('203', 2, 1, 'DangDon'),
       ('204', 2, 2, 'DaDat'),
       ('205', 2, 1, 'Trong'),
       ('206', 2, 3, 'CoKhach');

INSERT INTO DichVu (TenDichVu, DonGia, DonViTinh, MoTa)
VALUES ('Ăn sáng', 100000, 'suất', 'Buffet sáng'),
       ('Giặt ủi', 50000, 'kg', 'Giặt ủi theo ký'),
       ('Minibar', 30000, 'món', 'Đồ uống trong phòng'),
       ('Đưa đón sân bay', 250000, 'lượt', 'Xe riêng'),
       ('Spa', 400000, 'lượt', 'Massage thư giãn');

INSERT INTO TaiKhoan (TenDangNhap, MatKhau, VaiTro)
VALUES ('letan01', '123456', 'LeTan');

INSERT INTO NhanVien (HoTen, SoDienThoai, ChucVu, MaTaiKhoan)
VALUES ('Nguyễn Văn An', '0901234567', 'Lễ tân', 1);