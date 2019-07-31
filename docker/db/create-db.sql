USE master;

CREATE LOGIN [icUser] WITH PASSWORD = "Abc$12345";
CREATE USER [icUser] FOR LOGIN [icUser];

CREATE DATABASE [ic-staging];
