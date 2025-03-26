-- Tạo database
CREATE DATABASE TrafficViolationDB;
GO

-- Sử dụng database
USE TrafficViolationDB;
GO

-- Tạo bảng Users
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    FullName VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    Password VARCHAR(255) NOT NULL,
    Role VARCHAR(20) NOT NULL CHECK (Role IN ('Citizen', 'TrafficPolice')),
    Phone VARCHAR(15) NOT NULL,
    Address TEXT
);

-- Tạo bảng Vehicles
CREATE TABLE Vehicles (
    VehicleID INT PRIMARY KEY IDENTITY(1,1),
    PlateNumber VARCHAR(15) NOT NULL UNIQUE,
    OwnerID INT NOT NULL,
    Brand VARCHAR(50),
    Model VARCHAR(50),
    ManufactureYear INT,
    FOREIGN KEY (OwnerID) REFERENCES Users(UserID)
);

-- Tạo bảng Reports
CREATE TABLE Reports (
    ReportID INT PRIMARY KEY IDENTITY(1,1),
    ReporterID INT NOT NULL,
    ViolationType VARCHAR(50) NOT NULL,
    Description TEXT NOT NULL,
    PlateNumber VARCHAR(15) NOT NULL,
    ImageURL TEXT,
    VideoURL TEXT,
    Location VARCHAR(255) NOT NULL,
    ReportDate DATETIME DEFAULT GETDATE(),
    Status VARCHAR(20) DEFAULT 'Pending' CHECK (Status IN ('Pending', 'Approved', 'Rejected')),
    ProcessedBy INT,
    FOREIGN KEY (ReporterID) REFERENCES Users(UserID),
    FOREIGN KEY (ProcessedBy) REFERENCES Users(UserID),
    FOREIGN KEY (PlateNumber) REFERENCES Vehicles(PlateNumber)
);

-- Tạo bảng Violations
CREATE TABLE Violations (
    ViolationID INT PRIMARY KEY IDENTITY(1,1),
    ReportID INT NOT NULL,
    PlateNumber VARCHAR(15) NOT NULL,
    ViolatorID INT,
    FineAmount DECIMAL(10,2) NOT NULL,
    FineDate DATETIME DEFAULT GETDATE(),
    PaidStatus BIT DEFAULT 0,
    FOREIGN KEY (ReportID) REFERENCES Reports(ReportID),
    FOREIGN KEY (PlateNumber) REFERENCES Vehicles(PlateNumber),
    FOREIGN KEY (ViolatorID) REFERENCES Users(UserID)
);

-- Tạo bảng Notifications
CREATE TABLE Notifications (
    NotificationID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    Message TEXT NOT NULL,
    PlateNumber VARCHAR(15),
    SentDate DATETIME DEFAULT GETDATE(),
    IsRead BIT DEFAULT 0,
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (PlateNumber) REFERENCES Vehicles(PlateNumber)
);

-- Chèn dữ liệu vào bảng Users
INSERT INTO Users (FullName, Email, Password, Role, Phone, Address)
VALUES 
    ('Nguyen Van A', 'citizen1@example.com', '123456', 'Citizen', '0901234567', 'Hanoi'),
    ('Tran Thi B', 'citizen2@example.com', '123456', 'Citizen', '0901234568', 'Ho Chi Minh'),
    ('Le Van C', 'citizen3@example.com', '123456', 'Citizen', '0901234569', 'Da Nang'),
    ('Pham Thi D', 'police1@example.com', '123456', 'TrafficPolice', '0901234570', 'Hanoi'),
    ('Hoang Van E', 'police2@example.com', '123456', 'TrafficPolice', '0901234571', 'Ho Chi Minh');

-- Chèn dữ liệu vào bảng Vehicles
INSERT INTO Vehicles (PlateNumber, OwnerID, Brand, Model, ManufactureYear)
VALUES 
    ('29A-12345', 1, 'Toyota', 'Camry', 2020),
    ('51B-67890', 2, 'Honda', 'Civic', 2019),
    ('43C-54321', 3, 'Mazda', 'CX-5', 2021),
    ('29A-98765', 1, 'Ford', 'Ranger', 2022),
    ('51B-11111', 2, 'Hyundai', 'Tucson', 2020);

-- Chèn dữ liệu vào bảng Reports
INSERT INTO Reports (ReporterID, ViolationType, Description, PlateNumber, Location, ReportDate, Status, ProcessedBy)
VALUES 
    (1, 'Vượt đèn đỏ', 'Xe vượt đèn đỏ tại ngã tư', '29A-12345', 'Hanoi', '2025-03-20 10:00:00', 'Approved', 4),
    (2, 'Đi ngược chiều', 'Xe đi ngược chiều trên đường 1 chiều', '51B-67890', 'Ho Chi Minh', '2025-03-21 14:30:00', 'Pending', NULL),
    (3, 'Đỗ xe sai quy định', 'Xe đỗ dưới lòng đường cấm đỗ', '43C-54321', 'Da Nang', '2025-03-22 09:15:00', 'Rejected', 5),
    (1, 'Chạy quá tốc độ', 'Xe chạy 80km/h trong khu 50km/h', '29A-98765', 'Hanoi', '2025-03-23 16:45:00', 'Approved', 4),
    (2, 'Không đội mũ bảo hiểm', 'Người lái xe máy không đội mũ', '51B-11111', 'Ho Chi Minh', '2025-03-24 08:30:00', 'Pending', NULL);

-- Chèn dữ liệu vào bảng Violations
INSERT INTO Violations (ReportID, PlateNumber, ViolatorID, FineAmount, FineDate, PaidStatus)
VALUES 
    (1, '29A-12345', 1, 500000, '2025-03-20 10:30:00', 0),
    (4, '29A-98765', 1, 700000, '2025-03-23 17:00:00', 0),
    (2, '51B-67890', 2, 300000, '2025-03-21 15:00:00', 1),
    (3, '43C-54321', 3, 200000, '2025-03-22 09:45:00', 0),
    (5, '51B-11111', 2, 150000, '2025-03-24 09:00:00', 0);

-- Chèn dữ liệu vào bảng Notifications
INSERT INTO Notifications (UserID, Message, PlateNumber, SentDate, IsRead)
VALUES 
    (1, 'Phản ánh về 29A-12345 đã được phê duyệt.', '29A-12345', '2025-03-20 10:35:00', 0),
    (2, 'Bạn bị phạt 300000 vì vi phạm đi ngược chiều.', '51B-67890', '2025-03-21 15:05:00', 1),
    (3, 'Phản ánh về 43C-54321 đã bị từ chối.', '43C-54321', '2025-03-22 09:50:00', 0),
    (1, 'Bạn bị phạt 700000 vì chạy quá tốc độ.', '29A-98765', '2025-03-23 17:05:00', 0),
    (2, 'Phản ánh về 51B-11111 đang chờ xử lý.', '51B-11111', '2025-03-24 09:05:00', 0);

-- Kiểm tra dữ liệu
SELECT * FROM Users;
SELECT * FROM Vehicles;
SELECT * FROM Reports;
SELECT * FROM Violations;
SELECT * FROM Notifications;