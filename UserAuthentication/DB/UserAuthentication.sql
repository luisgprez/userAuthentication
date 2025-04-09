--Se crea Base de datos
CREATE DATABASE UserAuthentication
GO
USE UserAuthentication --Se accese a base de datos
GO

----Tablas
--Estatus
CREATE TABLE Cat_Estatus(
EstatusId INT IDENTITY PRIMARY KEY,
DescriptionEstatus NVARCHAR(500) NOT NULL,
Active BIT default 1
);

--Usuarios
CREATE TABLE Users (
    UserId INT IDENTITY PRIMARY KEY,
    UserName NVARCHAR(100) UNIQUE NOT NULL,
    PasswordHash VARBINARY(MAX) NOT NULL,
	PasswordSalt VARBINARY(128) NOT NULL,
    FailedLoginCount  INT DEFAULT 0,
    Locked BIT DEFAULT 1,
    DateLockedEnd DATETIME NULL,
	Created DATETIME NOT NULL,
	EstatusId int FOREIGN KEY REFERENCES Cat_Estatus(EstatusId) NOT NULL,
);

--TokenBlackList
CREATE TABLE TokenBlacklist (
    TokenBlackListId INT IDENTITY PRIMARY KEY,
    Token NVARCHAR(MAX) NOT NULL,
    Expiration DATETIME NOT NULL
);

--LoginHistory
CREATE TABLE LoginHistory (
    HistoryId INT IDENTITY PRIMARY KEY NOT NULL,
    UserId INT FOREIGN KEY REFERENCES Users(UserId) NOT NULL,
    DateHistory DATETIME NOT NULL,
    Success BIT NOT NULL,
	MessageLogin NVARCHAR(MAX) NOT NULL
);
---Valores
insert into Cat_Estatus values ('Activo',1),('Desactivado',1);