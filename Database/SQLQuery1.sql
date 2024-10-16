-- Tạo database KoiShipping
CREATE DATABASE KoiShipping;
GO

USE KoiShipping;
GO

-- Tạo bảng Customer
CREATE TABLE Customer (
    CustomerID INT PRIMARY KEY IDENTITY(1,1), -- ID tự động tăng
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Password NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(15),
    Address NVARCHAR(255),
    RegistrationDate DATETIME NOT NULL,
    Status NVARCHAR(100) NOT NULL,
    DeleteStatus BIT NOT NULL, -- Chỉ có giá trị 0 hoặc 1
	Otp NVARCHAR(6)
);

-- Tạo bảng Staffs
CREATE TABLE Staffs (
    StaffID INT PRIMARY KEY IDENTITY(1,1), -- ID tự động tăng
    StaffName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Password NVARCHAR(100) NOT NULL,
    Role NVARCHAR(100),
    Phone NVARCHAR(15),
    Status NVARCHAR(100) NOT NULL,
    DeleteStatus BIT NOT NULL -- Chỉ có giá trị 0 hoặc 1
);

-- Tạo bảng Order
CREATE TABLE [Order] (
    OrderID INT PRIMARY KEY IDENTITY(1,1), -- ID tự động tăng
    StartLocation NVARCHAR(255) NOT NULL,
    Destination NVARCHAR(255) NOT NULL,
    TransportMethod NVARCHAR(50),
    DepartureDate DATETIME,
    ArrivalDate DATETIME,
    Status NVARCHAR(50) NOT NULL,
    DeleteStatus BIT NOT NULL -- Chỉ có giá trị 0 hoặc 1
);

-- Tạo bảng Service
CREATE TABLE Service (
    ServiceID INT PRIMARY KEY IDENTITY(1,1), -- ID tự động tăng
    TransportMethod NVARCHAR(50) NOT NULL, -- Phương thức vận chuyển (Đường Bộ, Đường Hàng Không)
    WeightRange NVARCHAR(50) NOT NULL, -- Trọng Lượng (0-5 kg, 5-10 kg, ...)
    FastDelivery DECIMAL(10, 2) NOT NULL, -- Giao Nhanh (VNĐ)
    EconomyDelivery DECIMAL(10, 2) NOT NULL, -- Giao Tiết Kiệm (VNĐ)
    ExpressDelivery DECIMAL(10, 2) NOT NULL, -- Hỏa Tốc (VNĐ)
    DeleteStatus BIT NOT NULL -- Chỉ có giá trị 0 hoặc 1
);

-- Tạo bảng Advanced_Service
CREATE TABLE Advanced_Service (
    AdvancedServiceID INT PRIMARY KEY IDENTITY(1,1), -- ID tự động tăng
    AServiceName NVARCHAR(100) NOT NULL,
    Price DECIMAL(10, 2) NOT NULL,
    Description NVARCHAR(255), -- Thêm cột mô tả dịch vụ
    DeleteStatus BIT NOT NULL -- Chỉ có giá trị 0 hoặc 1
);

-- Tạo bảng Order_Detail
CREATE TABLE Order_Detail (
    OrderDetailID INT PRIMARY KEY IDENTITY(1,1), -- ID tự động tăng
    OrderID INT NOT NULL, -- Khóa ngoại tới bảng Order
    CustomerID INT NOT NULL, -- Khóa ngoại tới bảng Customer
    ServiceID INT NOT NULL, -- Khóa ngoại tới bảng Service
    ServiceName NVARCHAR(100), -- Thêm trường ServiceName
    StartLocation NVARCHAR(255) NOT NULL, -- Điểm xuất phát
    Destination NVARCHAR(255) NOT NULL, -- Điểm đến
    Weight DECIMAL(10, 2) NOT NULL,
    Quantity INT NOT NULL,
    Price DECIMAL(10, 2) NOT NULL,
    KoiStatus NVARCHAR(50) NOT NULL,
    AttachedItem NVARCHAR(255),
    Status NVARCHAR(50) NOT NULL,
    DeleteStatus BIT NOT NULL, -- Chỉ có giá trị 0 hoặc 1
    ReceiverName NVARCHAR(255) NOT NULL, -- Tên người nhận
    ReceiverPhone NVARCHAR(20) NOT NULL, -- SĐT người nhận
    Rating INT, -- Đánh giá từ 1 đến 5 sao
    Feedback NVARCHAR(500), -- Phản hồi từ khách hàng
    CreatedDate DATETIME NOT NULL, -- Ngày tạo
    CONSTRAINT FK_OrderDetail_Order FOREIGN KEY (OrderID) REFERENCES [Order](OrderID),
    CONSTRAINT FK_OrderDetail_Customer FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID),
    CONSTRAINT FK_OrderDetail_Service FOREIGN KEY (ServiceID) REFERENCES Service(ServiceID)
);

-- Tạo bảng AService_OrderD (Liên kết giữa Advanced_Service và Order_Detail)
CREATE TABLE AService_OrderD (
    AServiceOrderID INT PRIMARY KEY IDENTITY(1,1), -- ID tự động tăng
    OrderDetailID INT NOT NULL, -- Khóa ngoại tới bảng Order_Detail
    AdvancedServiceID INT NOT NULL, -- Khóa ngoại tới bảng Advanced_Service
    CONSTRAINT FK_AServiceOrderD_OrderDetail FOREIGN KEY (OrderDetailID) REFERENCES Order_Detail(OrderDetailID),
    CONSTRAINT FK_AServiceOrderD_AdvancedService FOREIGN KEY (AdvancedServiceID) REFERENCES Advanced_Service(AdvancedServiceID)
);

-- Tạo bảng Order_Staffs (Liên kết giữa Order và Staffs)
CREATE TABLE Order_Staffs (
    OrderStaffsID INT PRIMARY KEY IDENTITY(1,1), -- ID tự động tăng
    OrderID INT NOT NULL, -- Khóa ngoại tới bảng Order
    StaffID INT NOT NULL, -- Khóa ngoại tới bảng Staffs
    CONSTRAINT FK_OrderStaffs_Order FOREIGN KEY (OrderID) REFERENCES [Order](OrderID),
    CONSTRAINT FK_OrderStaffs_Staff FOREIGN KEY (StaffID) REFERENCES Staffs(StaffID)
);

-- Tạo bảng Tracking
CREATE TABLE Tracking (
    TrackingID INT PRIMARY KEY IDENTITY(1,1), -- ID tự động tăng
    TrackingName NVARCHAR(100) NOT NULL
);

-- Tạo bảng Tracking_OrderD (Liên kết nhiều-nhiều giữa Tracking và Order_Detail)
CREATE TABLE Tracking_OrderD (
    TrackingOrderDID INT PRIMARY KEY IDENTITY(1,1), -- ID tự động tăng
    OrderDetailID INT NOT NULL, -- Khóa ngoại tới bảng Order_Detail
    TrackingID INT NOT NULL, -- Khóa ngoại tới bảng Tracking
    Date DATETIME NOT NULL, -- Ngày gắn nhãn tracking
    CONSTRAINT FK_TrackingOrderD_OrderDetail FOREIGN KEY (OrderDetailID) REFERENCES Order_Detail(OrderDetailID),
    CONSTRAINT FK_TrackingOrderD_Tracking FOREIGN KEY (TrackingID) REFERENCES Tracking(TrackingID)
);
