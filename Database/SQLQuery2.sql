-- Chèn dữ liệu mẫu vào bảng Customer
INSERT INTO Customer (CustomerID, Name, Email, Password, Phone, Address, RegistrationDate, Status, DeleteStatus)
VALUES
(1, 'Nguyen Van A', 'nguyenvana@example.com', 'password123', '0123456789', '123 Nguyen Trai, HCM', GETDATE(), 'Active', 0),
(2, 'Tran Thi B', 'tranthib@example.com', 'password456', '0987654321', '456 Le Loi, HN', GETDATE(), 'Active', 0),
(3, 'Le Van C', 'levanc@example.com', 'password789', '0112233445', '789 Tran Hung Dao, Hue', GETDATE(), 'Inactive', 1);

-- Chèn dữ liệu mẫu vào bảng Staffs
INSERT INTO Staffs (StaffID, StaffName, Email, Password, Role, Phone, Status, DeleteStatus)
VALUES
(1, 'Pham Van D', 'phamvand@example.com', 'password123', 'Manager', '0123456789', 'Active', 0),
(2, 'Ngo Thi E', 'ngothie@example.com', 'password456', 'Staff', '0987654321', 'Active', 0),
(3, 'Hoang Van F', 'hoangvanf@example.com', 'password789', 'Staff', '0112233445', 'Inactive', 1);

-- Chèn dữ liệu mẫu vào bảng Order
INSERT INTO [Order] (OrderID, StartLocation, Destination, TransportMethod, DepartureDate, ArrivalDate, Status, TotalWeight, TotalKoiFish, StaffID, DeleteStatus)
VALUES
(1, 'HCM', 'HN', 'Air', GETDATE(), DATEADD(DAY, 2, GETDATE()), 'In Transit', 100.5, 20, 1, 0),
(2, 'HN', 'Hue', 'Land', GETDATE(), DATEADD(DAY, 3, GETDATE()), 'Delivered', 50.0, 10, 2, 0),
(3, 'Hue', 'HCM', 'Air', GETDATE(), DATEADD(DAY, 1, GETDATE()), 'Pending', 75.0, 15, 1, 1);

-- Chèn dữ liệu mẫu vào bảng Service
INSERT INTO Service (ServiceID, TransportMethod, WeightRange, FastDelivery, EconomyDelivery, ExpressDelivery, DeleteStatus)
VALUES
(1, 'Air', '0-5 kg', 500000, 300000, 700000, 0),
(2, 'Land', '5-10 kg', 400000, 200000, 600000, 0),
(3, 'Air', '10-15 kg', 600000, 350000, 800000, 1);

-- Chèn dữ liệu mẫu vào bảng Advanced_Service
INSERT INTO Advanced_Service (AdvancedServiceID, ServiceName, Price, DeleteStatus)
VALUES
(1, 'Insured Transport', 150000, 0),
(2, 'Priority Handling', 200000, 0),
(3, 'Real-time Tracking', 100000, 1);

-- Chèn dữ liệu mẫu vào bảng Order_Detail
INSERT INTO Order_Detail (OrderDetailID, OrderID, CustomerID, ServiceID, Weight, Quantity, Price, KoiStatus, AttachedItem, Status, DeleteStatus, ReceiverName, ReceiverPhone, Rating, Feedback)
VALUES
(1, 1, 1, 1, 5.0, 2, 500000, 'Healthy', 'N/A', 'Pending', 0, 'Nguyen Van G', '0123456789', 5, 'Great service!'),
(2, 2, 2, 2, 7.5, 1, 400000, 'Healthy', 'N/A', 'Delivered', 0, 'Tran Thi H', '0987654321', 4, 'Satisfactory service.'),
(3, 3, 3, 3, 10.0, 3, 600000, 'Healthy', 'N/A', 'In Transit', 1, 'Le Van J', '0112233445', NULL, NULL);

-- Chèn dữ liệu mẫu vào bảng AService_OrderD
INSERT INTO AService_OrderD (AServiceOrderID, OrderDetailID, AdvancedServiceID)
VALUES
(1, 1, 1),
(2, 2, 2),
(3, 3, 1);

-- Chèn dữ liệu mẫu vào bảng Order_Staffs
INSERT INTO Order_Staffs (OrderStaffsID, OrderID, StaffID)
VALUES
(1, 1, 1),
(2, 2, 2),
(3, 3, 1);
