-- Crear la base de datos si no existe
IF DB_ID('aplAnalitycsApiControl') IS NULL
    CREATE DATABASE aplAnalitycsApiControl;
GO

-- Usar la base de datos
USE aplAnalitycsApiControl;
GO

-- Crear tabla User
CREATE TABLE [User] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) UNIQUE NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    Fullname NVARCHAR(100),
    LastJti NVARCHAR(255)
);
GO

-- Crear tabla Role
CREATE TABLE Role (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) UNIQUE NOT NULL,
    Description NVARCHAR(255)
);
GO

-- Crear tabla Permission
CREATE TABLE Permission (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) UNIQUE NOT NULL,
    Description NVARCHAR(255)
);
GO

-- Crear tabla intermedia UserRole (relación muchos a muchos entre User y Role)
CREATE TABLE UserRole (
    UserId INT NOT NULL,
    RoleId INT NOT NULL,
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES [User](Id),
    FOREIGN KEY (RoleId) REFERENCES Role(Id)
);
GO

-- Crear tabla intermedia RolePermission (relación muchos a muchos entre Role y Permission)
CREATE TABLE RolePermission (
    RoleId INT NOT NULL,
    PermissionId INT NOT NULL,
    PRIMARY KEY (RoleId, PermissionId),
    FOREIGN KEY (RoleId) REFERENCES Role(Id),
    FOREIGN KEY (PermissionId) REFERENCES Permission(Id)
);
GO

-- Insertar datos en la tabla User
INSERT INTO [User] (Username, Password, Fullname, LastJti)
VALUES
    ('john_doe', '$2a$10$Dow5c', 'John Doe', NULL),
    ('alice_smith', '$2a$10$Dow5c', 'Alice Smith', NULL),
    ('bruce_wayne', '$2a$10$Dow5c', 'Bruce Wayne', NULL),
    ('charlie', '$2a$10$Dow5c', 'Charlie Brown', NULL),
    ('lucy', '$2a$10$Dow5c', 'Lucy van Pelt', NULL),
    ('snoopy', '$2a$10$Dow5c', 'Snoopy', NULL),
    ('linus', '$2a$10$Dow5c', 'Linus Van Pelt', NULL),
    ('peppermint', '$2a$10$Dow5c', 'Peppermint Patty', NULL);
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

-- Insertar datos en la tabla Permission
INSERT INTO Permission (Name, Description)
VALUES
    ('Read', 'Permission to read data'),
    ('Write', 'Permission to write data'),
    ('Edit', 'Permission to edit data'),
    ('Delete', 'Permission to delete data'),
    ('AdminAccess', 'Full admin access to the system');
GO

-- Asignar Roles a Usuarios en la tabla UserRole
INSERT INTO UserRole (UserId, RoleId)
VALUES
    (1, 1), -- John Doe es Admin
    (1, 4), -- John Doe es Moderator
    (2, 2), -- Alice Smith es Editor
    (2, 5), -- Alice Smith es Guest
    (3, 3), -- Bruce Wayne es Viewer
    (4, 1), -- Charlie Brown es Admin
    (4, 4), -- Charlie Brown es Moderator
    (5, 6), -- Lucy van Pelt es SuperAdmin
    (6, 5), -- Snoopy sin rol
    (7, 6), -- Linus Van Pelt sin rol
    (8, 3), -- Peppermint Patty es Viewer
    (8, 1); -- Peppermint Patty es Admin
GO

-- Asignar Permisos a Roles en la tabla RolePermission
INSERT INTO RolePermission (RoleId, PermissionId)
VALUES
    (1, 1), -- Admin tiene permiso de lectura
    (1, 2), -- Admin tiene permiso de escritura
    (1, 3), -- Admin tiene permiso de edición
    (1, 4), -- Admin tiene permiso de eliminación
    (1, 5), -- Admin tiene permiso de acceso completo
    (2, 1), -- Editor tiene permiso de lectura
    (2, 3), -- Editor tiene permiso de edición
    (3, 1), -- Viewer tiene permiso de lectura
    (4, 1), -- Moderator tiene permiso de lectura
    (4, 3), -- Moderator tiene permiso de edición
    (5, 1), -- Guest tiene permiso de lectura
    (6, 5); -- SuperAdmin tiene acceso completo
GO
