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

----Vaciar las tablas
--DELETE FROM RolePermissions;
--DELETE FROM Tokens;
--DELETE FROM LoginAudits;
--DELETE FROM Users;
--DELETE FROM Roles;
--DELETE FROM Companies;
--DELETE FROM Permissions;

---- Reiniciar los IDs de las tablas
--DBCC CHECKIDENT ('Companies', RESEED, 0);
--DBCC CHECKIDENT ('Roles', RESEED, 0);
--DBCC CHECKIDENT ('Permissions', RESEED, 0);
--DBCC CHECKIDENT ('Users', RESEED, 0);
--DBCC CHECKIDENT ('Tokens', RESEED, 0);


-- Insertar datos en la tabla Companies
INSERT INTO Companies (Name, NIT)
VALUES 
    ('Tech Solutions Inc.', 123456789),
    ('Global Enterprises', 987654321);

-- Insertar datos en la tabla Roles
INSERT INTO Roles (Name, Description, Status, CompanyId)
VALUES
    ('Admin', 'Administrator Role', 1, 1),
    ('Reader', 'Read-only Role', 1, 1),
    ('Writer', 'Write-only Role', 1, 1),
    ('Deleter', 'Delete-only Role', 1, 1);

-- Insertar datos en la tabla Permissions
INSERT INTO Permissions (Name, Description)
VALUES
    ('Read', 'Permission to read data'),
    ('Write', 'Permission to write data'),
    ('Delete', 'Permission to delete data');

-- Insertar datos en la tabla RolePermissions
INSERT INTO RolePermissions (RoleId, PermissionId, AssignmentDate)
VALUES
    (1, 1, GETDATE()),  -- Admin tiene permiso de lectura
    (1, 2, GETDATE()),  -- Admin tiene permiso de escritura
    (1, 3, GETDATE()),  -- Admin tiene permiso de eliminación
    (2, 1, GETDATE()),  -- Reader tiene permiso de lectura
    (3, 2, GETDATE()),  -- Writer tiene permiso de escritura
    (4, 3, GETDATE());  -- Deleter tiene permiso de eliminación

-- Insertar datos en la tabla Users
INSERT INTO Users (IdCCNit, FirstName, LastName, Email, CCIdentification, HashedPassword, RoleId)
VALUES
    ('12345678-123456789', 'AdminUser', 'Admin', 'admin@techsolutions.com', 12345678, '$2a$11$42SEpdmo4cqPU12WDidWCuD./tZ9FTKYqk.6yJR6rBXssNkNAjYum', 1),  -- Admin con todos los permisos
    ('12345679-123456789', 'ReaderUser', 'ReadOnly', 'reader@techsolutions.com', 12345679, '$2a$11$hBpNanyk4DjzEeN.UzUlN.EHPZXWwGly7D31FvyMfzGajzXVCoOhO', 2),  -- Usuario con permiso de lectura
    ('12345680-123456789', 'WriterUser', 'WriteOnly', 'writer@techsolutions.com', 12345680, '$2a$11$Cbym63.EY0oXvUB.lmaKd.AYj0/IaA1vRPFjiCiSFeg79/3C/PbYG', 3),  -- Usuario con permiso de escritura
    ('12345681-123456789', 'DeleterUser', 'DeleteOnly', 'deleter@techsolutions.com', 12345681, '$2a$11$38do9f0M6WLc6nw6NwbIeOrfx3GHraN4rmpTuK4fs3NsmudrRTIeK', 4);  -- Usuario con permiso de eliminación

-- Insertar datos en la tabla Tokens
INSERT INTO Tokens (IdCCNit, RecoveryToken, Jti)
VALUES
    ('12345678-123456789', 'recovery-token-admin', 'jti-admin'),
    ('12345679-123456789', 'recovery-token-reader', 'jti-reader'),
    ('12345680-123456789', 'recovery-token-writer', 'jti-writer'),
    ('12345681-123456789', 'recovery-token-deleter', 'jti-deleter');



CREATE LOGIN [IIS APPPOOL\.NET Core 8.0] FROM WINDOWS;
USE aplAnalitycsApiControl;
CREATE USER [IIS APPPOOL\.NET Core 8.0] FOR LOGIN [IIS APPPOOL\.NET Core 8.0];
ALTER ROLE db_owner ADD MEMBER [IIS APPPOOL\.NET Core 8.0];


 