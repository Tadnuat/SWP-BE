USE KoiShipping;
GO

-- Sample data for Customer table
INSERT INTO Customer (Name, Email, Password, Phone, Address, RegistrationDate, Status, DeleteStatus)
VALUES 
('Nguyen Van A', 'nva@gmail.com', 'password123', '0901234567', '123 Le Loi, HCM', GETDATE(), 'Active', 0),
('Tran Thi B', 'ttb@gmail.com', 'password456', '0912345678', '456 Tran Hung Dao, HCM', GETDATE(), 'Inactive', 0),
('Le Van C', 'lvc@gmail.com', 'password789', '0933456789', '789 Nguyen Hue, HCM', GETDATE(), 'Active', 0);

-- Sample data for Staffs table
INSERT INTO Staffs (StaffName, Email, Password, Role, Phone, Status, DeleteStatus)
VALUES 
('Admin 1', 'a@gmail.com', 'admin123', 'Manager', '0909876543', 'Active', 0),
('Staff 1', 'b@gmail.com', 'staff456', 'Staff', '0918765432', 'Active', 0),
('Staff 2', 'c@gmail.com', 'staff789', 'Staff', '0927654321', 'Inactive', 0);

-- Sample data for Order table
INSERT INTO [Order] (StartLocation, Destination, TransportMethod, DepartureDate, ArrivalDate, Status, TotalWeight, TotalKoiFish, DeleteStatus)
VALUES 
('HCM', 'Hanoi', 'Road', '2024-10-01 10:00', '2024-10-02 18:00', 'In Transit', 20.50, 50, 0),
('HCM', 'Hue', 'Air', '2024-10-05 08:00', '2024-10-05 12:00', 'Delivered', 15.75, 30, 0),
('Hanoi', 'Hue', 'Road', '2024-09-29 07:00', '2024-09-30 20:00', 'Pending', 10.00, 20, 0);

-- Sample data for Service table
INSERT INTO Service (TransportMethod, WeightRange, FastDelivery, EconomyDelivery, ExpressDelivery, DeleteStatus)
VALUES 
('Road', '0-5 kg', 50000, 30000, 80000, 0),
('Air', '0-5 kg', 70000, 50000, 100000, 0),
('Road', '5-10 kg', 100000, 70000, 120000, 0),
('Air', '5-10 kg', 130000, 100000, 150000, 0);

-- Sample data for Advanced_Service table
INSERT INTO Advanced_Service (AServiceName, Price, DeleteStatus)
VALUES 
('Transport Insurance', 50000, 0),
('Special Packaging', 30000, 0),
('GPS Tracking', 70000, 0);

-- Sample data for Order_Detail table
INSERT INTO Order_Detail (OrderID, CustomerID, ServiceID, ServiceName, Weight, Quantity, Price, KoiStatus, AttachedItem, Status, DeleteStatus, ReceiverName, ReceiverPhone, Rating, Feedback, CreatedDate)
VALUES 
(1, 1, 1, 'Road (0-5 kg)', 5.00, 10, 500000, 'Healthy', 'Plastic Box', 'In Transit', 0, 'Nguyen Van D', '0909988776', 5, 'Good service', GETDATE()),
(2, 2, 2, 'Air (0-5 kg)', 3.50, 5, 350000, 'Healthy', 'Foam Box', 'Delivered', 0, 'Tran Thi E', '0912345566', 4, 'Quick delivery', GETDATE()),
(3, 3, 3, 'Road (5-10 kg)', 7.00, 20, 700000, 'Sick', 'Nylon Bag', 'Pending', 0, 'Le Van F', '0923456677', NULL, NULL, GETDATE());

-- Sample data for AService_OrderD table
INSERT INTO AService_OrderD (OrderDetailID, AdvancedServiceID)
VALUES 
(1, 1), -- Transport Insurance for order 1
(2, 2), -- Special Packaging for order 2
(3, 3); -- GPS Tracking for order 3

-- Sample data for Order_Staffs table
INSERT INTO Order_Staffs (OrderID, StaffID)
VALUES 
(1, 1), -- Admin 1 handles order 1
(2, 2), -- Staff 1 handles order 2
(3, 3); -- Staff 2 handles order 3
