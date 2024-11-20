-- Crear la base de datos si no existe
IF DB_ID('aplAnalitycsApiControl') IS NULL
    CREATE DATABASE aplAnalitycsApiControl;
GO

-- Usar la base de datos
USE aplAnalitycsApiControl;
GO

CREATE TABLE Role (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) UNIQUE NOT NULL,
    Description NVARCHAR(255)
);
GO

-- Crear tabla User
CREATE TABLE [User] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) UNIQUE NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    Fullname NVARCHAR(100),
    LastJti NVARCHAR(255),
    RoleId INT NULL,
    CONSTRAINT FK_User_Role FOREIGN KEY (RoleId) REFERENCES Role(Id)
);
GO

-- Crear tabla Role

-- Crear tabla Permission
CREATE TABLE Permission (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) UNIQUE NOT NULL,
    Description NVARCHAR(255)
);
GO

-- Crear tabla intermedia RolePermission (relaci�n muchos a muchos entre Role y Permission)
CREATE TABLE RolePermission (
    RoleId INT NOT NULL,
    PermissionId INT NOT NULL,
    PRIMARY KEY (RoleId, PermissionId),
    FOREIGN KEY (RoleId) REFERENCES Role(Id),
    FOREIGN KEY (PermissionId) REFERENCES Permission(Id)
);
GO

-- Insertar datos en la tabla Role
INSERT INTO Role (Name, Description)
VALUES
    ('Admin', 'Administrator role with full permissions'),
    ('Editor', 'Editor role with edit permissions'),
    ('Viewer', 'Viewer role with read-only permissions'),
    ('Moderator', 'Moderator role with limited permissions'),
    ('Guest', 'Guest role with very limited access'),
    ('SuperAdmin', 'Super Administrator with all permissions');
GO
-- Insertar datos en la tabla User
INSERT INTO [User] (Username, Password, Fullname, LastJti, RoleId)
VALUES
    ('john_doe', '$2a$10$Dow5c', 'John Doe', NULL, 1),
    ('alice_smith', '$2a$10$Dow5c', 'Alice Smith', NULL, 2),
    ('bruce_wayne', '$2a$10$Dow5c', 'Bruce Wayne', NULL, 3),
    ('charlie', '$2a$10$Dow5c', 'Charlie Brown', NULL, 4),
    ('lucy', '$2a$10$Dow5c', 'Lucy van Pelt', NULL, 5),
    ('snoopy', '$2a$10$Dow5c', 'Snoopy', NULL, 6),
    ('linus', '$2a$10$Dow5c', 'Linus Van Pelt', NULL, 5),
    ('peppermint', '$2a$10$Dow5c', 'Peppermint Patty', NULL, 3);
GO


-- Insertar datos en la tabla Permission
INSERT INTO Permission (Name, Description)
VALUES
    ('Read', 'Permission to read data'),
    ('Write', 'Permission to write data'),
    ('Edit', 'Permission to edit data'),
    ('Delete', 'Permission to delete data'),
    ('AdminAccess', 'Full admin access to the system');
GO

-- Asignar Permisos a Roles en la tabla RolePermission
INSERT INTO RolePermission (RoleId, PermissionId)
VALUES
    (1, 1), -- Admin tiene permiso de lectura
    (1, 2), -- Admin tiene permiso de escritura
    (1, 3), -- Admin tiene permiso de edici�n
    (1, 4), -- Admin tiene permiso de eliminaci�n
    (1, 5), -- Admin tiene permiso de acceso completo
    (2, 1), -- Editor tiene permiso de lectura
    (2, 3), -- Editor tiene permiso de edici�n
    (3, 1), -- Viewer tiene permiso de lectura
    (4, 1), -- Moderator tiene permiso de lectura
    (4, 3), -- Moderator tiene permiso de edici�n
    (5, 1), -- Guest tiene permiso de lectura
    (6, 5); -- SuperAdmin tiene acceso completo
GO
