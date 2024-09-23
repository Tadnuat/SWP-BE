Create database KoiShipping
-- Tạo bảng Customer
CREATE TABLE Customer (
    CustomerID INT PRIMARY KEY, -- Không có IDENTITY
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Password NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(15),
    Address NVARCHAR(255),
    RegistrationDate DATETIME NOT NULL,
    Status NVARCHAR(100) NOT NULL,
    DeleteStatus NVARCHAR(100) NOT NULL
);

-- Tạo bảng Staffs
CREATE TABLE Staffs (
    StaffID INT PRIMARY KEY, -- Không có IDENTITY
    StaffName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Password NVARCHAR(100) NOT NULL,
    Role NVARCHAR(50),
    Phone NVARCHAR(15),
    Status NVARCHAR(100) NOT NULL,
    DeleteStatus NVARCHAR(100) NOT NULL
);

-- Tạo bảng Order
CREATE TABLE [Order] (
    OrderID INT PRIMARY KEY, -- Không có IDENTITY
    StartLocation NVARCHAR(255) NOT NULL,
    Destination NVARCHAR(255) NOT NULL,
    TransportMethod NVARCHAR(50),
    DepartureDate DATETIME NOT NULL,
    ArrivalDate DATETIME,
    Status NVARCHAR(50) NOT NULL,
    TotalWeight DECIMAL(10, 2),
    TotalKoiFish INT,
    StaffID INT NOT NULL, -- Khóa ngoại tới bảng Staffs
    DeleteStatus NVARCHAR(100) NOT NULL,
    CONSTRAINT FK_Order_Staffs FOREIGN KEY (StaffID) REFERENCES Staffs(StaffID)
);

-- Tạo bảng Service
CREATE TABLE Service (
    ServiceID INT PRIMARY KEY, -- ID không tự tăng
    TransportMethod NVARCHAR(50) NOT NULL, -- Phương thức vận chuyển (Đường Bộ, Đường Hàng Không)
    WeightRange NVARCHAR(50) NOT NULL, -- Trọng Lượng (0-5 kg, 5-10 kg, ...)
    FastDelivery DECIMAL(10, 2) NOT NULL, -- Giao Nhanh (VNĐ)
    EconomyDelivery DECIMAL(10, 2) NOT NULL, -- Giao Tiết Kiệm (VNĐ)
    ExpressDelivery DECIMAL(10, 2) NOT NULL, -- Hỏa Tốc (VNĐ)
    DeleteStatus NVARCHAR(100) NOT NULL
);

-- Tạo bảng Advanced_Service
CREATE TABLE Advanced_Service (
    AdvancedServiceID INT PRIMARY KEY, -- Không có IDENTITY
    ServiceName NVARCHAR(100) NOT NULL,
    Price DECIMAL(10, 2) NOT NULL,
    DeleteStatus NVARCHAR(100) NOT NULL
);

-- Tạo bảng Order_Detail
CREATE TABLE Order_Detail (
    OrderDetailID INT PRIMARY KEY, -- Không có IDENTITY
    OrderID INT NOT NULL, -- Khóa ngoại tới bảng Order
    CustomerID INT NOT NULL, -- Khóa ngoại tới bảng Customer
    ServiceID INT NOT NULL, -- Khóa ngoại tới bảng Service
    Weight DECIMAL(10, 2) NOT NULL,
    Quantity INT NOT NULL,
    Price DECIMAL(10, 2) NOT NULL,
    KoiStatus NVARCHAR(50) NOT NULL,
    AttachedItem NVARCHAR(255),
    Status NVARCHAR(50) NOT NULL,
    DeleteStatus NVARCHAR(100) NOT NULL,
    CONSTRAINT FK_OrderDetail_Order FOREIGN KEY (OrderID) REFERENCES [Order](OrderID),
    CONSTRAINT FK_OrderDetail_Customer FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID),
    CONSTRAINT FK_OrderDetail_Service FOREIGN KEY (ServiceID) REFERENCES Service(ServiceID)
);

-- Tạo bảng AService_OrderD (Liên kết giữa Advanced_Service và Order_Detail)
CREATE TABLE AService_OrderD (
    AServiceOrderID INT PRIMARY KEY, -- Không có IDENTITY
    OrderDetailID INT NOT NULL, -- Khóa ngoại tới bảng Order_Detail
    AdvancedServiceID INT NOT NULL, -- Khóa ngoại tới bảng Advanced_Service
    CONSTRAINT FK_AServiceOrderD_OrderDetail FOREIGN KEY (OrderDetailID) REFERENCES Order_Detail(OrderDetailID),
    CONSTRAINT FK_AServiceOrderD_AdvancedService FOREIGN KEY (AdvancedServiceID) REFERENCES Advanced_Service(AdvancedServiceID)
);

-- Tạo bảng Order_Staffs (Liên kết giữa Order và Staffs)
CREATE TABLE Order_Staffs (
    OrderStaffsID INT PRIMARY KEY, -- Không có IDENTITY
    OrderID INT NOT NULL, -- Khóa ngoại tới bảng Order
    StaffID INT NOT NULL, -- Khóa ngoại tới bảng Staffs
    CONSTRAINT FK_OrderStaffs_Order FOREIGN KEY (OrderID) REFERENCES [Order](OrderID),
    CONSTRAINT FK_OrderStaffs_Staff FOREIGN KEY (StaffID) REFERENCES Staffs(StaffID)
);
