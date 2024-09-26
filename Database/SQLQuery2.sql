USE KoiShipping;
GO

-- Chèn d? li?u m?u vào b?ng Customer
INSERT INTO Customer (CustomerID, Name, Email, Password, Phone, Address, RegistrationDate, Status, DeleteStatus)
VALUES 
(1, N'Nguyen Van A', 'a@gmail.com', 'password123', '0123456789', N'HCM City', GETDATE(), N'Active', 0),
(2, N'Tran Thi B', 'b@gmail.com', 'password456', '0987654321', N'Hanoi', GETDATE(), N'Active', 0);

-- Chèn d? li?u m?u vào b?ng Staffs
INSERT INTO Staffs (StaffID, StaffName, Email, Password, Role, Phone, Status, DeleteStatus)
VALUES
(1, N'Le Van C', 'c@gmail.com', 'password789', N'Manager', '0912345678', N'Active', 0),
(2, N'Pham Thi D', 'd@gmail.com', 'password987', N'Staff', '0908765432', N'Inactive', 0);

-- Chèn d? li?u m?u vào b?ng Order
INSERT INTO [Order] (OrderID, StartLocation, Destination, TransportMethod, DepartureDate, ArrivalDate, Status, TotalWeight, TotalKoiFish, DeleteStatus)
VALUES
(1, N'HCM City', N'Hanoi', N'Air', GETDATE(), DATEADD(DAY, 1, GETDATE()), N'Pending', 50.5, 100, 0),
(2, N'Hanoi', N'Hue', N'Road', GETDATE(), DATEADD(DAY, 2, GETDATE()), N'Delivered', 30.3, 60, 0);

-- Chèn d? li?u m?u vào b?ng Service
INSERT INTO Service (ServiceID, TransportMethod, WeightRange, FastDelivery, EconomyDelivery, ExpressDelivery, DeleteStatus)
VALUES
(1, N'Air', N'0-5 kg', 100000, 50000, 150000, 0),
(2, N'Road', N'5-10 kg', 80000, 40000, 120000, 0);

-- Chèn d? li?u m?u vào b?ng Advanced_Service
INSERT INTO Advanced_Service (AdvancedServiceID, ServiceName, Price, DeleteStatus)
VALUES
(1, N'Koi Care', 50000, 0),
(2, N'Oxygen Tank', 30000, 0);

-- Chèn d? li?u m?u vào b?ng Order_Detail
INSERT INTO Order_Detail (OrderDetailID, OrderID, CustomerID, ServiceID, Weight, Quantity, Price, KoiStatus, AttachedItem, Status, DeleteStatus, ReceiverName, ReceiverPhone, Rating, Feedback, CreatedDate)
VALUES
(1, 1, 1, 1, 10.5, 2, 200000, N'Healthy', N'Oxygen Tank', N'Pending', 0, N'Tran Van E', '0911122233', 5, N'Good service', GETDATE()),
(2, 2, 2, 2, 15.0, 3, 240000, N'Healthy', N'None', N'Delivered', 0, N'Le Thi F', '0922233444', 4, N'Satisfactory', GETDATE());

-- Chèn d? li?u m?u vào b?ng AService_OrderD
INSERT INTO AService_OrderD (OrderDetailID, AdvancedServiceID)
VALUES
(1, 1),
(2, 2);

-- Chèn d? li?u m?u vào b?ng Order_Staffs
INSERT INTO Order_Staffs (OrderID, StaffID)
VALUES
(1, 1),
(2, 2);
