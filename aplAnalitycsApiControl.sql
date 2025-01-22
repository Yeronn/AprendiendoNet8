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
    Nit INT NOT NULL UNIQUE
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
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    CCNumber INT NOT NULL,
    CompanyId INT NOT NULL,
    HashedPassword VARCHAR(255) NOT NULL,  -- Contraseña hasheada
    RoleId INT NOT NULL,  -- Clave foránea a Roles
    RegistrationDate DATETIME NOT NULL DEFAULT GETDATE(),
    UNIQUE (CCNumber, CompanyId),
    FOREIGN KEY (RoleId) REFERENCES Roles(Id),
    FOREIGN KEY (CompanyId) REFERENCES Companies(Id)
);

-- Crear la nueva tabla para los tipos de token
CREATE TABLE TokenTypes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(100) NOT NULL
);

-- Crear tabla TokenStatuses para los diferentes estados de los tokens
CREATE TABLE TokenStatuses (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(50) NOT NULL UNIQUE -- Nombre del estado del token (ejemplo: "Válido", "Revocado")
);

-- Crear la tabla LoginAudits con el campo CompanyId
CREATE TABLE LoginAudits (
    TokenId VARCHAR(50) PRIMARY KEY, -- JTI del token
    UserId INT NOT NULL,             -- ID del usuario
    IssuedAt DATETIME NOT NULL,      -- Fecha/hora de emisión
    ExpiresAt DATETIME NOT NULL,     -- Fecha/hora de expiración
    IPAddress VARCHAR(45),          -- Dirección IP del cliente
    DeviceInfo VARCHAR(255),        -- Información del dispositivo
    TokenStatusId INT NOT NULL,     -- ID del estado del token
    TokenTypeId INT NOT NULL,       -- ID del tipo de token (conectado con la tabla TokenTypes)
    CompanyId INT NOT NULL,         -- ID de la compañía
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (TokenStatusId) REFERENCES TokenStatuses(Id),
    FOREIGN KEY (TokenTypeId) REFERENCES TokenTypes(Id),
    FOREIGN KEY (CompanyId) REFERENCES Companies(Id)
);

--Vaciar las tablas
-- DELETE FROM RolePermissions;
-- DELETE FROM LoginAudits;
-- DELETE FROM Users;
-- DELETE FROM Roles;
-- DELETE FROM Companies;
-- DELETE FROM Permissions;
-- DELETE FROM TokenTypes;
-- DELETE FROM TokenStatuses;

-- -- Reiniciar los IDs de las tablas
-- DBCC CHECKIDENT ('Users', RESEED, 0);
-- DBCC CHECKIDENT ('Roles', RESEED, 0);
-- DBCC CHECKIDENT ('Companies', RESEED, 0);
-- DBCC CHECKIDENT ('Permissions', RESEED, 0);
-- DBCC CHECKIDENT ('TokenTypes', RESEED, 0);
-- DBCC CHECKIDENT ('TokenStatuses', RESEED, 0);

-- Insertar los tipos de token
INSERT INTO TokenTypes (Name)
VALUES
    ('Access'),
    ('Refresh');

-- Insertar los estados básicos en la tabla TokenStatuses
INSERT INTO TokenStatuses (Name)
VALUES 
    ('Valid'), 
    ('Revoked');

-- Insertar datos en la tabla Companies
INSERT INTO Companies (Name, Nit)
VALUES 
    ('Tech Solutions Inc.', 123456789),
    ('Global Enterprises', 987654321);

-- Insertar datos en la tabla Roles
INSERT INTO Roles (Name, Description, Status, CompanyId)
VALUES
    ('Admin', 'Administrator Role', 1, 1),
    ('Reader', 'Read-only Role', 1, 1),
    ('Writer', 'Write-only Role', 1, 1),
    ('Deleter', 'Delete-only Role', 1, 1),
    ('Manager', 'Manager Role', 1, 2),    -- Nuevo rol para la segunda empresa
    ('Auditor', 'Auditor Role', 1, 2);   -- Otro nuevo rol para la segunda empresa

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
    (4, 3, GETDATE()),  -- Deleter tiene permiso de eliminación
    (5, 1, GETDATE()),  -- Manager tiene permiso de lectura
    (5, 2, GETDATE()),  -- Manager tiene permiso de escritura
    (6, 1, GETDATE());  -- Auditor tiene permiso de lectura

-- Insertar datos en la tabla Users
INSERT INTO Users (FirstName, LastName, Email, CCNumber, CompanyId, HashedPassword, RoleId)
VALUES
    ('AdminUser', 'Admin', 'admin@techsolutions.com', 12345678, 1, '$2a$11$42SEpdmo4cqPU12WDidWCuD./tZ9FTKYqk.6yJR6rBXssNkNAjYum', 1),  -- Admin con todos los permisos
    ('ReaderUser', 'ReadOnly', 'reader@techsolutions.com', 12345679, 1, '$2a$11$hBpNanyk4DjzEeN.UzUlN.EHPZXWwGly7D31FvyMfzGajzXVCoOhO', 2),  -- Usuario con permiso de lectura
    ('WriterUser', 'WriteOnly', 'writer@techsolutions.com', 12345680, 1, '$2a$11$Cbym63.EY0oXvUB.lmaKd.AYj0/IaA1vRPFjiCiSFeg79/3C/PbYG', 3),  -- Usuario con permiso de escritura
    ('DeleterUser', 'DeleteOnly', 'deleter@techsolutions.com', 12345681, 1, '$2a$11$38do9f0M6WLc6nw6NwbIeOrfx3GHraN4rmpTuK4fs3NsmudrRTIeK', 4),  -- Usuario con permiso de eliminación
    ('ManagerUser', 'Manager', 'manager@globalenterprises.com', 12345682, 2, '$2a$11$5Fg6CdPZ7UtpJTOFf./LReYB8ikIK9d/7G7qKQDbj3z9BoPg9lXje', 5),  -- Manager de la segunda empresa
    ('AuditorUser', 'Auditor', 'auditor@globalenterprises.com', 12345683, 2, '$2a$11$8Rt4LO7AeIW4Xm7p2cdVeu38U5WiJ6UPxFZVz3jHsew9C1kOIA4Q6', 6);  -- Auditor de la segunda empresa

CREATE LOGIN [IIS APPPOOL\.NET Core 8.0] FROM WINDOWS;
USE aplAnalitycsApiControl;
CREATE USER [IIS APPPOOL\.NET Core 8.0] FOR LOGIN [IIS APPPOOL\.NET Core 8.0];
ALTER ROLE db_owner ADD MEMBER [IIS APPPOOL\.NET Core 8.0];

-- [
--   {
--     "firstName": "AdminUser",
--     "lastName": "Admin",
--     "email": "admin@techsolutions.com",
--     "ccNumber": 12345678,
--     "companyId": 1,
--     "password": "admin123",
--     "roleId": 1
--   },
--   {
--     "firstName": "ReaderUser",
--     "lastName": "ReadOnly",
--     "email": "reader@techsolutions.com",
--     "ccNumber": 12345679,
--     "companyId": 1,
--     "password": "reader123",
--     "roleId": 2
--   },
--   {
--     "firstName": "WriterUser",
--     "lastName": "WriteOnly",
--     "email": "writer@techsolutions.com",
--     "ccNumber": 12345680,
--     "companyId": 1,
--     "password": "writer123",
--     "roleId": 3
--   },
--   {
--     "firstName": "DeleterUser",
--     "lastName": "DeleteOnly",
--     "email": "deleter@techsolutions.com",
--     "ccNumber": 12345681,
--     "companyId": 1,
--     "password": "deleter123",
--     "roleId": 4
--   },
--   {
--     "firstName": "ManagerUser",
--     "lastName": "Manager",
--     "email": "manager@globalenterprises.com",
--     "ccNumber": 12345682,
--     "companyId": 2,
--     "password": "manager123",
--     "roleId": 5
--   },
--   {
--     "firstName": "AuditorUser",
--     "lastName": "Auditor",
--     "email": "auditor@globalenterprises.com",
--     "ccNumber": 12345683,
--     "companyId": 2,
--     "password": "auditor123",
--     "roleId": 6
--   }
-- ]
