-- Crear la base de datos si no existe
IF DB_ID('aplAnalitycsApiControl') IS NULL
    CREATE DATABASE aplAnalitycsApiControl;
GO

-- Usar la base de datos
USE aplAnalitycsApiControl;
GO

-- Crear tabla Companies
CREATE TABLE Companies (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(100) NOT NULL UNIQUE,
    NIT INT NOT NULL UNIQUE
);

-- Crear tabla Roles
CREATE TABLE Roles (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Description TEXT,
    Status BIT NOT NULL DEFAULT 1,
    CompanyId INT NOT NULL,
    FOREIGN KEY (CompanyId) REFERENCES Companies(Id)
);

-- Crear tabla Permissions
CREATE TABLE Permissions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(100) NOT NULL UNIQUE,
    Description TEXT
);

-- Crear tabla RolePermissions
CREATE TABLE RolePermissions (
    RoleId INT NOT NULL,
    PermissionId INT NOT NULL,
    AssignmentDate DATETIME NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY (RoleId, PermissionId),
    FOREIGN KEY (RoleId) REFERENCES Roles(Id),
    FOREIGN KEY (PermissionId) REFERENCES Permissions(Id)
);

-- Crear tabla Users
CREATE TABLE Users (
    IdCCNit VARCHAR(50) NOT NULL PRIMARY KEY,  
    Id INT IDENTITY(1,1),  -- Auto-incrementing Id
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    CCIdentification INT NOT NULL,
    HashedPassword VARCHAR(255) NOT NULL,  -- Contraseña hasheada
    RoleId INT NOT NULL,  -- Clave foránea a Roles
    RegistrationDate DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

-- Crear tabla Tokens
CREATE TABLE Tokens (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    IdCCNit VARCHAR(50) NOT NULL,  -- Clave foránea a Users
    RecoveryToken VARCHAR(255) NULL,  -- Puede ser NULL
    Jti VARCHAR(255) NULL,
    DateCreated DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (IdCCNit) REFERENCES Users(IdCCNit)
);

CREATE TABLE LoginAudits (
    TokenId VARCHAR(50) PRIMARY KEY, -- JTI del token
    IdCCNit VARCHAR(50) NOT NULL,             -- ID del usuario
    IssuedAt DATETIME NOT NULL,       -- Fecha/hora de emisión
    ExpiresAt DATETIME NOT NULL,      -- Fecha/hora de expiración
    IPAddress VARCHAR(45),           -- Dirección IP del cliente
    DeviceInfo VARCHAR(255),         -- Información del dispositivo
    Status BIT DEFAULT 1 NOT NULL,             -- Estado del token (1 = válido, 0 = revocado)
    IsAccessToken BIT DEFAULT 1 NOT NULL,
    FOREIGN KEY (IdCCNit) REFERENCES Users(IdCCNit)
);


-- Insertar datos en la tabla Companies
INSERT INTO Companies (Name, NIT)
VALUES 
    ('Tech Solutions Inc.', 123456789),
    ('Global Enterprises', 987654321),
    ('Innovatech Ltd.', 567890123),
    ('prueba', 12345631);

-- Insertar datos en la tabla Roles
INSERT INTO Roles (Name, Description, Status, CompanyId)
VALUES
    ('Admin', 'Administrator Role', 1, 1),
    ('Manager', 'Manager Role', 1, 2),
    ('User', 'Regular User Role', 1, 3),
    ('prueba', 'prueba', 1, 4);

-- Insertar datos en la tabla Permissions
INSERT INTO Permissions (Name, Description)
VALUES
    ('Read', 'Permission to read data'),
    ('Write', 'Permission to write data'),
    ('Delete', 'Permission to delete data'),
    ('prueba', 'prueba');

-- Insertar datos en la tabla RolePermissions
INSERT INTO RolePermissions (RoleId, PermissionId, AssignmentDate)
VALUES
    (1, 1, '2024-10-01 09:00:00'),  -- Admin tiene permiso de lectura
    (1, 2, '2024-10-02 10:30:00'),  -- Admin tiene permiso de escritura
    (2, 1, '2024-10-03 14:45:00'),  -- Manager tiene permiso de lectura
    (3, 1, '2024-10-04 16:15:00'),  -- User tiene permiso de lectura
    (4, 1, '2024-10-04 16:15:00'),
    (4, 2, '2024-10-04 16:15:00'),
    (4, 3, '2024-10-04 16:15:00'),
    (4, 4, '2024-10-04 16:15:00');

-- Insertar datos en la tabla Users
INSERT INTO Users (IdCCNit, FirstName, LastName, Email, CCIdentification, HashedPassword, RoleId)
VALUES
    ('10101010', 'John', 'Doe', 'john.doe@example.com', 123456, 'hashedpassword1', 1),  -- Admin
    ('20202020', 'Jane', 'Smith', 'jane.smith@example.com', 654321, 'hashedpassword2', 2),  -- Manager
    ('30303030', 'Alice', 'Johnson', 'alice.johnson@example.com', 987654, 'hashedpassword3', 3),  -- User
    ('40404040', 'Bob', 'Williams', 'bob.williams@example.com', 111222, 'hashedpassword4', 3),  -- User sin permisos adicionales
    ('12345', 'prueba', 'prueba', 'prueba@example.com', 123231, '$2a$11$VTDt63kAgy//Q2LankkKeerI3LDUsRjQDpZoAFAdvh4AtCOoOQWDi', 3),  -- Usuario de prueba
    ('12323145-12345631', 'AdminUser', 'AdminUser', 'admin@admin.com', 12323145, '$2a$11$sFM19dUTdJ2zea2HMgiAuO2UbyCej73dEPVvsRKareVyC/ZD30uXS', 4);  -- Usuario de prueba

-- Insertar datos en la tabla Tokens
INSERT INTO Tokens (IdCCNit, RecoveryToken, Jti)
VALUES
    ('10101010', 'recovery-token-abc', 'jti-token-123'),  -- Token para John Doe (Admin)
    ('20202020', NULL, 'jti-token-456'),  -- Token para Jane Smith (Manager)
    ('30303030', 'recovery-token-xyz', 'jti-token-789'),  -- Token para Alice Johnson (User)
    ('40404040', NULL, NULL);  -- Bob Williams sin tokens


CREATE LOGIN [IIS APPPOOL\.NET Core 8.0] FROM WINDOWS;
USE aplAnalitycsApiControl;
CREATE USER [IIS APPPOOL\.NET Core 8.0] FOR LOGIN [IIS APPPOOL\.NET Core 8.0];
ALTER ROLE db_owner ADD MEMBER [IIS APPPOOL\.NET Core 8.0];
