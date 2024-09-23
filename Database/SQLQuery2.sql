-- Chèn dữ liệu vào bảng Customer
INSERT INTO Customer (CustomerID, Name, Email, Password, Phone, Address, RegistrationDate, Status, DeleteStatus)
VALUES
(1, 'Nguyen Van A', 'nguyenvana@example.com', 'password123', '0901234567', '123 Nguyen Trai, HCM', GETDATE(), 'Active', 'Not Deleted'),
(2, 'Tran Thi B', 'tranthib@example.com', 'password456', '0907654321', '456 Le Lai, Hanoi', GETDATE(), 'Active', 'Not Deleted');

-- Chèn dữ liệu vào bảng Staffs
INSERT INTO Staffs (StaffID, StaffName, Email, Password, Role, Phone, Status, DeleteStatus)
VALUES
(1, 'Le Van C', 'levanc@example.com', 'staffpass123', 'Manager', '0901112222', 'Active', 'Not Deleted'),
(2, 'Pham Thi D', 'phamthid@example.com', 'staffpass456', 'Driver', '0903334444', 'Active', 'Not Deleted');

-- Chèn dữ liệu vào bảng Order
INSERT INTO [Order] (OrderID, StartLocation, Destination, TransportMethod, DepartureDate, ArrivalDate, Status, TotalWeight, TotalKoiFish, StaffID, DeleteStatus)
VALUES
(1, 'Depot HCM', 'Depot Hanoi', 'Air', GETDATE(), DATEADD(DAY, 2, GETDATE()), 'Pending', 100.50, 20, 1, 'Not Deleted'),
(2, 'Depot Hue', 'Depot HCM', 'Land', GETDATE(), DATEADD(DAY, 1, GETDATE()), 'Completed', 200.00, 30, 2, 'Not Deleted');

-- Chèn dữ liệu vào bảng Service
INSERT INTO Service (ServiceID, TransportMethod, WeightRange, FastDelivery, EconomyDelivery, ExpressDelivery, DeleteStatus)
VALUES
(1, 'Air', '0-5 kg', 150000.00, 100000.00, 200000.00, 'Not Deleted'),
(2, 'Land', '5-10 kg', 100000.00, 70000.00, 150000.00, 'Not Deleted');

-- Chèn dữ liệu vào bảng Advanced_Service
INSERT INTO Advanced_Service (AdvancedServiceID, ServiceName, Price, DeleteStatus)
VALUES
(1, 'Insurance', 50000.00, 'Not Deleted'),
(2, 'Temperature Control', 30000.00, 'Not Deleted');

-- Chèn dữ liệu vào bảng Order_Detail
INSERT INTO Order_Detail (OrderDetailID, OrderID, CustomerID, ServiceID, Weight, Quantity, Price, KoiStatus, AttachedItem, Status, DeleteStatus)
VALUES
(1, 1, 1, 1, 5.00, 10, 150000.00, 'Healthy', 'None', 'Pending', 'Not Deleted'),
(2, 2, 2, 2, 10.00, 20, 100000.00, 'Healthy', 'None', 'Completed', 'Not Deleted');

-- Chèn dữ liệu vào bảng AService_OrderD
INSERT INTO AService_OrderD (AServiceOrderID, OrderDetailID, AdvancedServiceID)
VALUES
(1, 1, 1),
(2, 2, 2);

-- Chèn dữ liệu vào bảng Order_Staffs
INSERT INTO Order_Staffs (OrderStaffsID, OrderID, StaffID)
VALUES
(1, 1, 1),
(2, 2, 2);
