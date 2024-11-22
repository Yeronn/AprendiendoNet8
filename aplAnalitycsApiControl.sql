-- Crear la base de datos si no existe
IF DB_ID('aplAnalitycsApiControl') IS NULL
    CREATE DATABASE aplAnalitycsApiControl;
GO

-- Usar la base de datos
USE aplAnalitycsApiControl;
GO

-- Create Empresas table
CREATE TABLE Companies (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(100) NOT NULL UNIQUE,
    NIT INT NOT NULL UNIQUE
);

-- Create Roles table (with new columns Status and CompanyId)
CREATE TABLE Roles (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(100) NOT NULL UNIQUE,
    Description TEXT,
    Status BIT NOT NULL DEFAULT 1,  -- Active by default
    CompanyId INT NOT NULL,
    FOREIGN KEY (CompanyId) REFERENCES Companies(Id)
);

-- Create Permissions table
CREATE TABLE Permissions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(100) NOT NULL UNIQUE,
    Description TEXT
);

-- Create RolePermission table (many-to-many relation between Roles and Permissions)
CREATE TABLE RolePermissions (
    RoleId INT NOT NULL,
    PermissionId INT NOT NULL,
    AssignmentDate DATE NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY (RoleId, PermissionId),
    FOREIGN KEY (RoleId) REFERENCES Roles(Id),
    FOREIGN KEY (PermissionId) REFERENCES Permissions(Id)
);

-- Create Users table with updated primary key (IdCardNit) and new fields
CREATE TABLE Users (
    IdCardNit INT NOT NULL PRIMARY KEY,  
    Id INT IDENTITY(1,1),  -- Auto-incrementing Id
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    Identification VARCHAR(50) NOT NULL,
    Password VARCHAR(255) NOT NULL,  -- Assuming hashed password
    PasswordSalt VARCHAR(255) NOT NULL,  -- Assuming salt for password hashing
    RoleId INT NOT NULL,  -- Foreign key to Roles
    RegistrationDate DATETIME NOT NULL DEFAULT GETDATE(),  -- Date of registration
    FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

-- Create Tokens table with nullable token fields
CREATE TABLE Tokens (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    LastJti VARCHAR(255) NULL,
    RecoveryToken VARCHAR(255) NULL,  -- Can be NULL if not provided
    UserId INT NOT NULL,  -- Foreign key to Users
    FOREIGN KEY (UserId) REFERENCES Users(IdCardNit)
);


-- Insertar datos en la tabla Companies
INSERT INTO Companies (Name, NIT)
VALUES 
    ('Tech Solutions Inc.', 123456789),
    ('Global Enterprises', 987654321),
    ('Innovatech Ltd.', 567890123);

-- Insertar datos en la tabla Roles
INSERT INTO Roles (Name, Description, Status, CompanyId)
VALUES
    ('Admin', 'Administrator Role', 1, 1),
    ('Manager', 'Manager Role', 1, 2),
    ('User', 'Regular User Role', 1, 3);

-- Insertar datos en la tabla Permissions
INSERT INTO Permissions (Name, Description)
VALUES
    ('Read', 'Permission to read data'),
    ('Write', 'Permission to write data'),
    ('Delete', 'Permission to delete data');

-- Insertar datos en la tabla RolePermissions (asignando permisos a roles)
INSERT INTO RolePermissions (RoleId, PermissionId, AssignmentDate)
VALUES
    (1, 1, '2024-10-01'),  -- Admin tiene permiso de lectura
    (1, 2, '2024-10-02'),  -- Admin tiene permiso de escritura
    (2, 1, '2024-10-03'),  -- Manager tiene permiso de lectura
    (3, 1, '2024-10-04');  -- User tiene permiso de lectura

-- Insertar datos en la tabla Users
INSERT INTO Users (IdCardNit, FirstName, LastName, Email, Identification, Password, PasswordSalt, RoleId)
VALUES
    (10101010, 'John', 'Doe', 'john.doe@example.com', 'JD123456', 'hashedpassword1', 'salt1', 1),  -- Admin
    (20202020, 'Jane', 'Smith', 'jane.smith@example.com', 'JS654321', 'hashedpassword2', 'salt2', 2),  -- Manager
    (30303030, 'Alice', 'Johnson', 'alice.johnson@example.com', 'AJ987654', 'hashedpassword3', 'salt3', 3),  -- User
    (40404040, 'Bob', 'Williams', 'bob.williams@example.com', 'BW111222', 'hashedpassword4', 'salt4', 3);  -- User sin permisos adicionales

-- Insertar datos en la tabla Tokens (vinculados a usuarios específicos)
INSERT INTO Tokens (LastJti, RecoveryToken, UserId)
VALUES
    ('jti-token-123', 'recovery-token-abc', 10101010),  -- Token para John Doe (Admin)
    ('jti-token-456', NULL, 20202020),  -- Token para Jane Smith (Manager)
    ('jti-token-789', 'recovery-token-xyz', 30303030),  -- Token para Alice Johnson (User)
    (NULL, NULL, 40404040);  -- Bob Williams sin tokens
