-- Thêm dữ liệu mẫu vào bảng Customer
INSERT INTO Customer (Name, Email, Password, Phone, Address, RegistrationDate, Status, DeleteStatus)
VALUES
('Customer 1', 'customer1@example.com', 'password1', '0912345671', 'Address 1', GETDATE(), 'Active', 0),
('Customer 2', 'customer2@example.com', 'password2', '0912345672', 'Address 2', GETDATE(), 'Active', 0),
('Customer 3', 'customer3@example.com', 'password3', '0912345673', 'Address 3', GETDATE(), 'Inactive', 0),
('Customer 4', 'customer4@example.com', 'password4', '0912345674', 'Address 4', GETDATE(), 'Active', 0),
('Customer 5', 'customer5@example.com', 'password5', '0912345675', 'Address 5', GETDATE(), 'Active', 0),
('Customer 6', 'customer6@example.com', 'password6', '0912345676', 'Address 6', GETDATE(), 'Inactive', 0),
('Customer 7', 'customer7@example.com', 'password7', '0912345677', 'Address 7', GETDATE(), 'Active', 0);

-- Thêm dữ liệu mẫu vào bảng Staffs
INSERT INTO Staffs (StaffName, Email, Password, Role, Phone, Status, DeleteStatus)
VALUES
('Staff 1', 'staff1@example.com', 'password1', 'Manager', '0912345671', 'Active', 0),
('Staff 2', 'staff2@example.com', 'password2', 'Sale Staff', '0912345672', 'Active', 0),
('Staff 3', 'staff3@example.com', 'password3', 'Delivering Staff', '0912345673', 'Active', 0),
('Staff 4', 'staff4@example.com', 'password4', 'Manager', '0912345674', 'Active', 0),
('Staff 5', 'staff5@example.com', 'password5', 'Delivering Staff', '0912345675', 'Active', 0),
('Staff 6', 'staff6@example.com', 'password6', 'Sale Staff', '0912345676', 'Inactive', 0),
('Staff 7', 'staff7@example.com', 'password7', 'Manager', '0912345677', 'Active', 0);

-- Thêm dữ liệu mẫu vào bảng Order
SET IDENTITY_INSERT [Order] ON;
INSERT INTO [Order] (OrderID, StartLocation, Destination, TransportMethod, DepartureDate, ArrivalDate, Status, DeleteStatus)
VALUES
(0, 'HCM', 'Hanoi', 'air', GETDATE(), NULL, 'Ready', 1),
(1, 'Hanoi', 'HCM', 'road', GETDATE(), NULL, 'Delivering', 0),
(2, 'HCM', 'Hue', 'air', GETDATE(), NULL, 'Finish', 0),
(3, 'Hue', 'Hanoi', 'road', GETDATE(), NULL, 'Ready', 0),
(4, 'HCM', 'Hanoi', 'air', GETDATE(), NULL, 'Finish', 0),
(5, 'Hanoi', 'Hue', 'road', GETDATE(), NULL, 'Delivering', 0),
(6, 'HCM', 'Hanoi', 'air', GETDATE(), NULL, 'Finish', 0);
SET IDENTITY_INSERT [Order] OFF;

-- Thêm dữ liệu mẫu vào bảng Service
INSERT INTO Service (TransportMethod, WeightRange, FastDelivery, EconomyDelivery, ExpressDelivery, DeleteStatus)
VALUES
('air', '0-5', 100000, 80000, 120000, 0),
('road', '5-10', 120000, 90000, 150000, 0),
('air', '10-20', 150000, 120000, 180000, 0),
('road', '0-5', 90000, 70000, 110000, 0),
('air', '5-10', 130000, 100000, 160000, 0),
('road', '10-20', 140000, 110000, 170000, 0),
('air', '20-30', 180000, 140000, 200000, 0);

-- Thêm dữ liệu mẫu vào bảng Advanced_Service
INSERT INTO Advanced_Service (AServiceName, Price, DeleteStatus)
VALUES
('Insurance', 50000, 0),
('Packing', 30000, 0),
('Express Handling', 80000, 0);

-- Thêm dữ liệu mẫu vào bảng Order_Detail
INSERT INTO Order_Detail (OrderID, CustomerID, ServiceID, ServiceName, StartLocation, Destination, Weight, Quantity, Price, KoiStatus, AttachedItem, Status, DeleteStatus, ReceiverName, ReceiverPhone, Rating, Feedback, CreatedDate)
VALUES
(0, 1, 1, 'FastDelivery', 'HCM', 'Hanoi', 10.5, 2, 150000, 'Healthy', NULL, 'Pending', 0, 'Receiver 1', '0912345678', 5, 'Great service', GETDATE()),
(1, 2, 2, 'EconomyDelivery', 'Hanoi', 'HCM', 8.0, 1, 120000, 'Healthy', NULL, 'Waiting', 0, 'Receiver 2', '0912345679', 4, 'Satisfied', GETDATE()),
(2, 3, 3, 'ExpressDelivery', 'HCM', 'Hue', 15.0, 3, 180000, 'Injured', 'Box', 'Delivering', 0, 'Receiver 3', '0912345680', 3, 'Slow delivery', GETDATE()),
(3, 4, 4, 'FastDelivery', 'Hue', 'Hanoi', 12.0, 1, 160000, 'Healthy', 'Plastic Bag', 'Pending', 0, 'Receiver 4', '0912345681', 4, 'Good but expensive', GETDATE()),
(4, 5, 5, 'EconomyDelivery', 'Hanoi', 'HCM', 7.5, 2, 140000, 'Injured', NULL, 'Canceled', 0, 'Receiver 5', '0912345682', NULL, NULL, GETDATE()),
(5, 6, 6, 'ExpressDelivery', 'HCM', 'Hanoi', 18.0, 3, 200000, 'Healthy', NULL, 'Finish', 0, 'Receiver 6', '0912345683', 5, 'Excellent', GETDATE()),
(6, 7, 7, 'FastDelivery', 'Hue', 'Hanoi', 20.0, 1, 180000, 'Injured', NULL, 'Delivering', 0, 'Receiver 7', '0912345684', 2, 'Poor packaging', GETDATE());

-- Thêm dữ liệu mẫu vào bảng AService_OrderD
INSERT INTO AService_OrderD (OrderDetailID, AdvancedServiceID)
VALUES
(1, 1),
(2, 2),
(3, 3),
(4, 1),
(5, 2),
(6, 3),
(7, 1);

-- Thêm dữ liệu mẫu vào bảng Order_Staffs
INSERT INTO Order_Staffs (OrderID, StaffID)
VALUES 
(1, 2),
(1, 3),
(2, 4),
(3, 1),
(4, 5),
(5, 6),
(6, 7);

-- Thêm dữ liệu mẫu vào bảng Tracking
INSERT INTO Tracking (TrackingName)
VALUES
('Pending'),
('Waiting'),
('Delivering'),
('Finish'),
('Canceled');

-- Thêm dữ liệu mẫu vào bảng Tracking_OrderD
INSERT INTO Tracking_OrderD (OrderDetailID, TrackingID, Date)
VALUES
(1, 1, GETDATE()),
(2, 2, GETDATE()),
(3, 3, GETDATE()),
(4, 4, GETDATE()),
(5, 5, GETDATE()),
(6, 3, GETDATE()),
(7, 1, GETDATE());
