-- Chèn dữ liệu mẫu vào bảng Customer
INSERT INTO Customer (Name, Email, Password, Phone, Address, RegistrationDate, Status, DeleteStatus)
VALUES 
('Nguyen Van A', 'nguyenvana@example.com', 'password123', '0909123456', '123 Nguyen Trai, HCM', GETDATE(), 'Active', 0),
('Tran Thi B', 'tranthib@example.com', 'password456', '0912345678', '456 Le Loi, Hanoi', GETDATE(), 'Active', 0);

-- Chèn dữ liệu mẫu vào bảng Staffs
INSERT INTO Staffs (StaffName, Email, Password, Role, Phone, Status, DeleteStatus)
VALUES 
('Le Van C', 'levanc@example.com', 'password789', 'Manager', '0987654321', 'Working', 0),
('Hoang Thi D', 'hoangthid@example.com', 'password012', 'Driver', '0998765432', 'Working', 0);

-- Cho phép chèn ID tùy chỉnh cho bảng Order
SET IDENTITY_INSERT [Order] ON;

-- Chèn dữ liệu mẫu vào bảng Order (ID bắt đầu từ 0)
INSERT INTO [Order] (OrderID, StartLocation, Destination, TransportMethod, DepartureDate, ArrivalDate, Status, TotalWeight, TotalKoiFish, DeleteStatus)
VALUES 
(0, 'HCM', 'Hanoi', 'Air', '2024-09-01 10:00:00', '2024-09-01 12:00:00', 'Completed', 20.5, 50, 1),
(1, 'HCM', 'Hanoi', 'Air', '2024-09-01 10:00:00', '2024-09-01 12:00:00', 'Completed', 20.5, 50, 0),
(2, 'Hue', 'HCM', 'Road', '2024-09-05 08:00:00', '2024-09-05 18:00:00', 'In Transit', 10.2, 25, 0);

-- Tắt lại chế độ IDENTITY_INSERT cho bảng Order
SET IDENTITY_INSERT [Order] OFF;

-- Chèn dữ liệu mẫu vào bảng Service
INSERT INTO Service (TransportMethod, WeightRange, FastDelivery, EconomyDelivery, ExpressDelivery, DeleteStatus)
VALUES 
('Air', '0-5', 200000, 150000, 300000, 0),
('Road', '5-10', 100000, 75000, 150000, 0);

-- Chèn dữ liệu mẫu vào bảng Advanced_Service
INSERT INTO Advanced_Service (AServiceName, Price, DeleteStatus)
VALUES 
('Water Filtering System', 50000, 0),
('Air Pump', 30000, 0);

-- Chèn dữ liệu mẫu vào bảng Order_Detail
INSERT INTO Order_Detail (OrderID, CustomerID, ServiceID, ServiceName, StartLocation, Destination, Weight, Quantity, Price, KoiStatus, AttachedItem, Status, DeleteStatus, ReceiverName, ReceiverPhone, Rating, Feedback, CreatedDate)
VALUES 
(0, 1, 1, 'FastDelivery', 'HCM', 'Hanoi', 5.0, 10, 200000, 'Healthy', 'Oxygen Bag', 'Completed', 0, 'Tran Van E', '0923456789', 5, 'Good service', GETDATE()),
(1, 2, 2, 'EconomyDelivery', 'Hue', 'HCM', 7.5, 15, 150000, 'Healthy', 'Plastic Bag', 'In Transit', 0, 'Le Thi F', '0934567890', NULL, NULL, GETDATE());

-- Chèn dữ liệu mẫu vào bảng AService_OrderD
INSERT INTO AService_OrderD (OrderDetailID, AdvancedServiceID)
VALUES 
(1, 1), -- Dịch vụ bổ sung cho Order_Detail đầu tiên
(2, 2); -- Dịch vụ bổ sung cho Order_Detail thứ hai

-- Chèn dữ liệu mẫu vào bảng Order_Staffs
INSERT INTO Order_Staffs (OrderID, StaffID)
VALUES 
(0, 1), -- Nhân viên quản lý Order đầu tiên
(1, 2); -- Nhân viên quản lý Order thứ hai
