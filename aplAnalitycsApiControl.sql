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
    NIT VARCHAR(50) NOT NULL UNIQUE
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
    PRIMARY KEY (RoleId, PermissionId),
    FOREIGN KEY (RoleId) REFERENCES Roles(Id),
    FOREIGN KEY (PermissionId) REFERENCES Permissions(Id)
);

-- Create Users table with updated primary key (IdCardNit) and new fields
CREATE TABLE Users (
    IdCardNit int NOT NULL PRIMARY KEY,  -- Auto-incrementing Id
    Id INT IDENTITY(1,1),  -- Auto-incrementing Id
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    Identification VARCHAR(50),
    Password VARCHAR(255) NOT NULL,  -- Assuming hashed password
    PasswordSalt VARCHAR(255) NOT NULL,  -- Assuming salt for password hashing
    RoleId INT NOT NULL,  -- Foreign key to Roles
    RegistrationDate DATETIME NOT NULL DEFAULT GETDATE(),  -- Date of registration
    FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

-- Create Tokens table with nullable token fields
CREATE TABLE Tokens (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Token VARCHAR(255) NULL,
    RecoveryToken VARCHAR(255) NULL,  -- Can be NULL if not provided
    UserId INT NOT NULL,  -- Foreign key to Users
    FOREIGN KEY (UserId) REFERENCES Users(IdCardNit)
);

-- Example: Inserting data into the Companies table
INSERT INTO Companies (Name, NIT)
VALUES 
    ('Company A', '123456789'),
    ('Company B', '987654321');

-- Example: Inserting data into the Roles table
INSERT INTO Roles (Name, Description, Status, CompanyId)
VALUES
    ('Admin', 'Administrator Role', 1, 1),
    ('User', 'Regular User Role', 1, 2);

-- Example: Inserting data into the Permissions table
INSERT INTO Permissions (Name, Description)
VALUES
    ('Read', 'Read Permission'),
    ('Write', 'Write Permission'),
    ('Delete', 'Delete Permission');

-- Example: Inserting data into the RolePermissions table (many-to-many relation)
INSERT INTO RolePermissions (RoleId, PermissionId)
VALUES
    (1, 1),  -- Admin gets Read permission
    (1, 2),  -- Admin gets Write permission
    (2, 1);  -- User gets Read permission

-- Example: Inserting data into the Users table
INSERT INTO Users (IdCardNit, FirstName, LastName, Email, Identification, Password, PasswordSalt, RoleId)
VALUES
    (123456789, 'John', 'Doe', 'john.doe@example.com', 'ID12345', 'hashedpassword1', 'salt1', 1),  -- Admin user
    (987654321, 'Jane', 'Smith', 'jane.smith@example.com', 'ID54321', 'hashedpassword2', 'salt2', 2);  -- Regular user

-- Example: Inserting data into the Tokens table
INSERT INTO Tokens (Token, RecoveryToken, UserId)
VALUES
    ('token123', 'recoverytoken123', 123456789),
    ('token456', NULL, 987654321);  -- User without recovery token
