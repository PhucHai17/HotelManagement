PRAGMA foreign_keys = ON;

-- Bang vai tro
CREATE TABLE IF NOT EXISTS VaiTro
(
    MaVT  INTEGER PRIMARY KEY AUTOINCREMENT,
    TenVT TEXT NOT NULL UNIQUE
);

-- Bang quyen
CREATE TABLE IF NOT EXISTS Quyen
(
    MaQuyen  INTEGER PRIMARY KEY AUTOINCREMENT,
    TenQuyen TEXT NOT NULL UNIQUE
);

-- Lien ket vai tro va quyen
CREATE TABLE IF NOT EXISTS VaiTro_Quyen
(
    MaVT    INTEGER NOT NULL,
    MaQuyen INTEGER NOT NULL,
    PRIMARY KEY (MaVT, MaQuyen),
    FOREIGN KEY (MaVT) REFERENCES VaiTro (MaVT),
    FOREIGN KEY (MaQuyen) REFERENCES Quyen (MaQuyen)
);

-- Bang tai khoan
CREATE TABLE IF NOT EXISTS TaiKhoan
(
    MaTK        INTEGER PRIMARY KEY AUTOINCREMENT,
    MaVT        INTEGER NOT NULL,
    TenDangNhap TEXT    NOT NULL UNIQUE,
    MatKhauHash TEXT    NOT NULL,
    SoLanSaiMK  INTEGER NOT NULL DEFAULT 0,
    TrangThai   TEXT    NOT NULL DEFAULT 'HoatDong',
    FOREIGN KEY (MaVT) REFERENCES VaiTro (MaVT)
);

-- Bang nhan vien
CREATE TABLE IF NOT EXISTS NhanVien
(
    MaNV       INTEGER PRIMARY KEY AUTOINCREMENT,
    MaTK       INTEGER NOT NULL UNIQUE,
    MaQL       INTEGER,
    HoTen      TEXT    NOT NULL,
    SDT        TEXT,
    CCCD       TEXT,
    ChucVu     TEXT,
    NgayVaoLam TEXT,
    TrangThai  TEXT,
    FOREIGN KEY (MaTK) REFERENCES TaiKhoan (MaTK),
    FOREIGN KEY (MaQL) REFERENCES NhanVien (MaNV)
);

-- Bang lich phan ca
CREATE TABLE IF NOT EXISTS LichPhanCa
(
    MaLPC   INTEGER PRIMARY KEY AUTOINCREMENT,
    MaNV    INTEGER NOT NULL,
    MaQL    INTEGER NOT NULL,
    NgayLam TEXT    NOT NULL,
    LoaiCa  TEXT    NOT NULL,
    FOREIGN KEY (MaNV) REFERENCES NhanVien (MaNV),
    FOREIGN KEY (MaQL) REFERENCES NhanVien (MaNV)
);

-- Bang ca lam viec
CREATE TABLE IF NOT EXISTS CaLamViec
(
    MaCa          INTEGER PRIMARY KEY AUTOINCREMENT,
    MaNV          INTEGER NOT NULL,
    GioBatDau     TEXT    NOT NULL,
    GioKetThuc    TEXT,
    TienDauCa     REAL DEFAULT 0,
    TienCuoiCa    REAL,
    GhiChuBanGiao TEXT,
    FOREIGN KEY (MaNV) REFERENCES NhanVien (MaNV)
);

-- Bang hang thanh vien
CREATE TABLE IF NOT EXISTS HangThanhVien
(
    MaHang       INTEGER PRIMARY KEY AUTOINCREMENT,
    TenHang      TEXT NOT NULL,
    DiemToiThieu INTEGER DEFAULT 0,
    PhanTramGiam REAL    DEFAULT 0
);

-- Bang khach hang
CREATE TABLE IF NOT EXISTS KhachHang
(
    MaKH        INTEGER PRIMARY KEY AUTOINCREMENT,
    MaTK        INTEGER UNIQUE,
    MaHang      INTEGER NOT NULL,
    HoTen       TEXT    NOT NULL,
    SDT         TEXT,
    Email       TEXT,
    CCCD        TEXT,
    DiemTichLuy INTEGER DEFAULT 0,
    FOREIGN KEY (MaTK) REFERENCES TaiKhoan (MaTK),
    FOREIGN KEY (MaHang) REFERENCES HangThanhVien (MaHang)
);

-- Bang loai phong
CREATE TABLE IF NOT EXISTS LoaiPhong
(
    MaLoai   INTEGER PRIMARY KEY AUTOINCREMENT,
    TenLoai  TEXT    NOT NULL,
    GiaCoBan REAL    NOT NULL,
    SucChua  INTEGER NOT NULL,
    MoTa     TEXT
);

-- Bang tien nghi
CREATE TABLE IF NOT EXISTS TienNghi
(
    MaTN  INTEGER PRIMARY KEY AUTOINCREMENT,
    TenTN TEXT NOT NULL
);

-- Lien ket loai phong va tien nghi
CREATE TABLE IF NOT EXISTS LoaiPhong_TienNghi
(
    MaLoai INTEGER NOT NULL,
    MaTN   INTEGER NOT NULL,
    PRIMARY KEY (MaLoai, MaTN),
    FOREIGN KEY (MaLoai) REFERENCES LoaiPhong (MaLoai),
    FOREIGN KEY (MaTN) REFERENCES TienNghi (MaTN)
);

-- Bang hinh anh phong
CREATE TABLE IF NOT EXISTS HinhAnhPhong
(
    MaHA     INTEGER PRIMARY KEY AUTOINCREMENT,
    MaLoai   INTEGER NOT NULL,
    DuongDan TEXT    NOT NULL,
    FOREIGN KEY (MaLoai) REFERENCES LoaiPhong (MaLoai)
);

-- Bang phong
CREATE TABLE IF NOT EXISTS Phong
(
    MaPhong   INTEGER PRIMARY KEY AUTOINCREMENT,
    MaLoai    INTEGER NOT NULL,
    SoPhong   TEXT    NOT NULL UNIQUE,
    Tang      INTEGER NOT NULL,
    TrangThai TEXT    NOT NULL,
    FOREIGN KEY (MaLoai) REFERENCES LoaiPhong (MaLoai)
);

-- Bang gia phong
CREATE TABLE IF NOT EXISTS BangGia
(
    MaBG    INTEGER PRIMARY KEY AUTOINCREMENT,
    MaLoai  INTEGER NOT NULL,
    TuNgay  TEXT    NOT NULL,
    DenNgay TEXT    NOT NULL,
    Gia     REAL    NOT NULL,
    LoaiGia TEXT,
    FOREIGN KEY (MaLoai) REFERENCES LoaiPhong (MaLoai)
);

-- Bang khuyen mai
CREATE TABLE IF NOT EXISTS KhuyenMai
(
    MaKM         INTEGER PRIMARY KEY AUTOINCREMENT,
    MaCode       TEXT NOT NULL UNIQUE,
    PhanTramGiam REAL NOT NULL,
    TuNgay       TEXT NOT NULL,
    DenNgay      TEXT NOT NULL
);

-- Bang dat phong
CREATE TABLE IF NOT EXISTS DatPhong
(
    MaDP      INTEGER PRIMARY KEY AUTOINCREMENT,
    MaKH      INTEGER NOT NULL,
    MaNV      INTEGER,
    MaKM      INTEGER,
    NgayNhan  TEXT    NOT NULL,
    NgayTra   TEXT    NOT NULL,
    SoKhach   INTEGER NOT NULL,
    TienCoc   REAL DEFAULT 0,
    KenhDat   TEXT,
    TrangThai TEXT    NOT NULL,
    FOREIGN KEY (MaKH) REFERENCES KhachHang (MaKH),
    FOREIGN KEY (MaNV) REFERENCES NhanVien (MaNV),
    FOREIGN KEY (MaKM) REFERENCES KhuyenMai (MaKM)
);

-- Bang chi tiet dat phong
CREATE TABLE IF NOT EXISTS ChiTietDatPhong
(
    MaCTDP         INTEGER PRIMARY KEY AUTOINCREMENT,
    MaDP           INTEGER NOT NULL,
    MaPhong        INTEGER NOT NULL,
    GiaTaiThoiDiem REAL    NOT NULL,
    NgayNhanTT     TEXT    NOT NULL,
    NgayTraTT      TEXT    NOT NULL,
    FOREIGN KEY (MaDP) REFERENCES DatPhong (MaDP),
    FOREIGN KEY (MaPhong) REFERENCES Phong (MaPhong)
);

-- Bang khach di cung
CREATE TABLE IF NOT EXISTS KhachDiCung
(
    MaKDC  INTEGER PRIMARY KEY AUTOINCREMENT,
    MaCTDP INTEGER NOT NULL,
    HoTen  TEXT    NOT NULL,
    CCCD   TEXT,
    FOREIGN KEY (MaCTDP) REFERENCES ChiTietDatPhong (MaCTDP)
);

-- Bang dich vu
CREATE TABLE IF NOT EXISTS DichVu
(
    MaDV   INTEGER PRIMARY KEY AUTOINCREMENT,
    TenDV  TEXT NOT NULL,
    DonGia REAL NOT NULL,
    DonVi  TEXT
);

-- Bang su dung dich vu
CREATE TABLE IF NOT EXISTS SuDungDichVu
(
    MaSDDV  INTEGER PRIMARY KEY AUTOINCREMENT,
    MaCTDP  INTEGER NOT NULL,
    MaDV    INTEGER NOT NULL,
    SoLuong INTEGER NOT NULL,
    DonGia  REAL    NOT NULL,
    NgaySD  TEXT    NOT NULL,
    FOREIGN KEY (MaCTDP) REFERENCES ChiTietDatPhong (MaCTDP),
    FOREIGN KEY (MaDV) REFERENCES DichVu (MaDV)
);

-- Bang hoa don
CREATE TABLE IF NOT EXISTS HoaDon
(
    MaHD      INTEGER PRIMARY KEY AUTOINCREMENT,
    MaDP      INTEGER NOT NULL UNIQUE,
    MaNV      INTEGER NOT NULL,
    MaCa      INTEGER,
    TienPhong REAL DEFAULT 0,
    TienDV    REAL DEFAULT 0,
    GiamGia   REAL DEFAULT 0,
    VAT       REAL DEFAULT 0,
    TongTien  REAL DEFAULT 0,
    TrangThai TEXT    NOT NULL,
    FOREIGN KEY (MaDP) REFERENCES DatPhong (MaDP),
    FOREIGN KEY (MaNV) REFERENCES NhanVien (MaNV),
    FOREIGN KEY (MaCa) REFERENCES CaLamViec (MaCa)
);

-- Bang thanh toan
CREATE TABLE IF NOT EXISTS ThanhToan
(
    MaTT     INTEGER PRIMARY KEY AUTOINCREMENT,
    MaHD     INTEGER NOT NULL,
    SoTien   REAL    NOT NULL,
    HinhThuc TEXT    NOT NULL,
    LoaiGD   TEXT,
    NgayTT   TEXT    NOT NULL,
    FOREIGN KEY (MaHD) REFERENCES HoaDon (MaHD)
);

-- Bang don dep phong
CREATE TABLE IF NOT EXISTS DonDepPhong
(
    MaDD      INTEGER PRIMARY KEY AUTOINCREMENT,
    MaPhong   INTEGER NOT NULL,
    MaNV      INTEGER NOT NULL,
    TrangThai TEXT    NOT NULL,
    NgayXong  TEXT,
    FOREIGN KEY (MaPhong) REFERENCES Phong (MaPhong),
    FOREIGN KEY (MaNV) REFERENCES NhanVien (MaNV)
);

-- Bang bao tri phong
CREATE TABLE IF NOT EXISTS BaoTriPhong
(
    MaBT      INTEGER PRIMARY KEY AUTOINCREMENT,
    MaPhong   INTEGER NOT NULL,
    MoTa      TEXT    NOT NULL,
    TrangThai TEXT    NOT NULL,
    FOREIGN KEY (MaPhong) REFERENCES Phong (MaPhong)
);

-- Bang danh gia
CREATE TABLE IF NOT EXISTS DanhGia
(
    MaDG    INTEGER PRIMARY KEY AUTOINCREMENT,
    MaKH    INTEGER NOT NULL,
    MaDP    INTEGER NOT NULL UNIQUE,
    SoSao   INTEGER NOT NULL,
    NoiDung TEXT,
    FOREIGN KEY (MaKH) REFERENCES KhachHang (MaKH),
    FOREIGN KEY (MaDP) REFERENCES DatPhong (MaDP)
);

-- Bang nhat ky he thong
CREATE TABLE IF NOT EXISTS NhatKyHeThong
(
    MaNK        INTEGER PRIMARY KEY AUTOINCREMENT,
    MaTK        INTEGER NOT NULL,
    HanhDong    TEXT    NOT NULL,
    BangTacDong TEXT,
    ThoiGian    TEXT    NOT NULL,
    FOREIGN KEY (MaTK) REFERENCES TaiKhoan (MaTK)
);
