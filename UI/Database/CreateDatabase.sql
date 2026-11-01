CREATE TABLE LoaiPhong
(
    MaLoaiPhong INTEGER PRIMARY KEY AUTOINCREMENT,
    TenLoai     TEXT NOT NULL,
    GiaMotDem   REAL NOT NULL,
    DienTich    REAL,
    LoaiGiuong  TEXT,
    TienNghi    TEXT
);

CREATE TABLE Phong
(
    MaPhong     INTEGER PRIMARY KEY AUTOINCREMENT,
    SoPhong     TEXT    NOT NULL UNIQUE,
    Tang        INTEGER NOT NULL,
    MaLoaiPhong INTEGER NOT NULL,
    TrangThai   TEXT    NOT NULL DEFAULT 'Trong',
    FOREIGN KEY (MaLoaiPhong) REFERENCES LoaiPhong (MaLoaiPhong)
);

CREATE TABLE KhachHang
(
    MaKH   INTEGER PRIMARY KEY AUTOINCREMENT,
    HoTen  TEXT NOT NULL,
    SDT    TEXT,
    Email  TEXT,
    DiaChi TEXT,
    CCCD   TEXT,
    GhiChu TEXT
);

CREATE TABLE TaiKhoan
(
    MaTK        INTEGER PRIMARY KEY AUTOINCREMENT,
    TenDangNhap TEXT NOT NULL UNIQUE,
    MatKhau     TEXT NOT NULL,
    VaiTro      TEXT NOT NULL,
    MaKH        INTEGER,
    FOREIGN KEY (MaKH) REFERENCES KhachHang (MaKH)
);

CREATE TABLE DatPhong
(
    MaDatPhong    INTEGER PRIMARY KEY AUTOINCREMENT,
    MaKH          INTEGER NOT NULL,
    MaPhong       INTEGER NOT NULL,
    NgayNhan      TEXT    NOT NULL,
    NgayTra       TEXT    NOT NULL,
    TrangThai     TEXT    NOT NULL DEFAULT 'ChoDuyet',
    SoCCCD        TEXT,
    SoKhachDiCung INTEGER          DEFAULT 0,
    NgayCheckIn   TEXT,
    NgayCheckOut  TEXT,
    FOREIGN KEY (MaKH) REFERENCES KhachHang (MaKH),
    FOREIGN KEY (MaPhong) REFERENCES Phong (MaPhong)
);

CREATE TABLE DichVu
(
    MaDichVu  INTEGER PRIMARY KEY AUTOINCREMENT,
    TenDichVu TEXT NOT NULL,
    DonGia    REAL NOT NULL,
    DonVi     TEXT NOT NULL
);

CREATE TABLE DichVuSuDung
(
    MaSuDung   INTEGER PRIMARY KEY AUTOINCREMENT,
    MaDatPhong INTEGER NOT NULL,
    MaDichVu   INTEGER NOT NULL,
    SoLuong    INTEGER NOT NULL DEFAULT 1,
    FOREIGN KEY (MaDatPhong) REFERENCES DatPhong (MaDatPhong),
    FOREIGN KEY (MaDichVu) REFERENCES DichVu (MaDichVu)
);

CREATE TABLE HoaDon
(
    MaHoaDon          INTEGER PRIMARY KEY AUTOINCREMENT,
    MaDatPhong        INTEGER NOT NULL,
    TienPhong         REAL    NOT NULL DEFAULT 0,
    TienDichVu        REAL    NOT NULL DEFAULT 0,
    PhuThu            REAL    NOT NULL DEFAULT 0,
    VAT               REAL    NOT NULL DEFAULT 0,
    TongTien          REAL    NOT NULL DEFAULT 0,
    HinhThucThanhToan TEXT,
    DaThanhToan       INTEGER NOT NULL DEFAULT 0,
    FOREIGN KEY (MaDatPhong) REFERENCES DatPhong (MaDatPhong)
);

INSERT INTO LoaiPhong (TenLoai, GiaMotDem, DienTich, LoaiGiuong, TienNghi)
VALUES ('Standard', 500000, 22, 'Giường đôi', 'Wi-Fi, Điều hòa, TV'),
       ('Deluxe', 800000, 30, 'Giường king', 'Wi-Fi, Điều hòa, TV, Ban công, Minibar, Bồn tắm'),
       ('Suite', 1500000, 45, 'Giường king', 'Wi-Fi, Điều hòa, TV, Ban công, Minibar, Bồn tắm');

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

INSERT INTO DichVu (TenDichVu, DonGia, DonVi)
VALUES ('Ăn sáng', 100000, 'suất'),
       ('Giặt ủi', 50000, 'kg'),
       ('Minibar', 30000, 'món'),
       ('Đưa đón sân bay', 250000, 'lượt'),
       ('Spa', 400000, 'lượt');

