PRAGMA foreign_keys = ON;

CREATE TABLE Drivers (
    DriverID INTEGER PRIMARY KEY AUTOINCREMENT,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    LicenseCategory VARCHAR(10) NOT NULL,
    HireDate DATE DEFAULT CURRENT_DATE
);

CREATE TABLE Vehicles (
    VehicleID INTEGER PRIMARY KEY AUTOINCREMENT,
    RegistrationNumber VARCHAR(15) NOT NULL UNIQUE,
    VehicleType VARCHAR(30) NOT NULL,
    Capacity INTEGER NOT NULL CHECK (Capacity > 0 AND Capacity <= 200),
    ManufactureYear INTEGER NOT NULL CHECK (ManufactureYear >= 1990)
);

CREATE TABLE Routes (
    RouteID INTEGER PRIMARY KEY AUTOINCREMENT,
    RouteNumber VARCHAR(10) NOT NULL UNIQUE,
    StartPoint VARCHAR(100) NOT NULL,
    EndPoint VARCHAR(100) NOT NULL
);

CREATE TABLE Stops (
    StopID INTEGER PRIMARY KEY AUTOINCREMENT,
    StopName VARCHAR(100) NOT NULL,
    IsTerminal INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE RouteStops (
    RouteID INTEGER,
    StopID INTEGER,
    StopOrder INTEGER NOT NULL CHECK (StopOrder > 0),
    PRIMARY KEY (RouteID, StopID),
    FOREIGN KEY (RouteID) REFERENCES Routes(RouteID),
    FOREIGN KEY (StopID) REFERENCES Stops(StopID)
);

CREATE TABLE Schedules (
    ScheduleID INTEGER PRIMARY KEY AUTOINCREMENT,
    RouteID INTEGER NOT NULL,
    VehicleID INTEGER NOT NULL,
    DriverID INTEGER NOT NULL,
    DepartureTime DATETIME NOT NULL,
    ArrivalTime DATETIME NOT NULL CHECK (DepartureTime < ArrivalTime),
    FOREIGN KEY (RouteID) REFERENCES Routes(RouteID),
    FOREIGN KEY (VehicleID) REFERENCES Vehicles(VehicleID),
    FOREIGN KEY (DriverID) REFERENCES Drivers(DriverID)
);

INSERT INTO Drivers (FirstName, LastName, LicenseCategory, HireDate)
VALUES
('Ivan', 'Dimitrov', 'D', '2015-04-12'),
('Georgi', 'Petrov', 'D', '2018-09-01'),
('Nikolay', 'Ivanov', 'D1', '2021-11-15');

INSERT INTO Vehicles (RegistrationNumber, VehicleType, Capacity, ManufactureYear)
VALUES
('A 1234 BB', 'Bus Solaris', 120, 2018),
('A 5678 BB', 'Articulated Bus', 180, 2019),
('A 9012 CX', 'Trolleybus Skoda', 100, 2020);

INSERT INTO Routes (RouteNumber, StartPoint, EndPoint)
VALUES
('B12', 'Meden Rudnik', 'Slaveykov'),
('211', 'Meden Rudnik', 'Izgrev'),
('T1', 'South Bus Station', 'Meden Rudnik');

INSERT INTO Stops (StopName, IsTerminal)
VALUES
('Terminal Meden Rudnik', 1),
('Sports Hall', 0),
('Hospital', 0),
('Terminal Izgrev', 1),
('South Bus Station', 1);

INSERT INTO RouteStops (RouteID, StopID, StopOrder)
VALUES
(1, 1, 1),
(1, 3, 2),
(1, 4, 3);

INSERT INTO Schedules (RouteID, VehicleID, DriverID, DepartureTime, ArrivalTime)
VALUES
(1, 1, 1, '2023-10-25 08:00:00', '2023-10-25 08:45:00'),
(2, 2, 2, '2023-10-25 08:15:00', '2023-10-25 09:10:00'),
(3, 3, 3, '2023-10-25 08:30:00', '2023-10-25 09:05:00');
