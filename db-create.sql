------------
-- 1st part
------------

USE master;

-- SELECT * FROM sys.database_principals;

-- In master db
CREATE LOGIN [DbUsr] WITH PASSWORD = '6%Uxwp7Mcxo7Khy'
CREATE USER [DbUsr] FOR LOGIN [DbUsr]

CREATE DATABASE test;

------------
-- 2nd part
------------

USE test;

-- -- In user db
CREATE USER [DbUsr] FOR LOGIN [DbUsr]
ALTER ROLE [db_owner] ADD MEMBER [DbUsr]

-- Check for ownership on user db
-- select m.name as Member, r.name as Role
-- from sys.database_role_members
-- inner join sys.database_principals m on sys.database_role_members.member_principal_id = m.principal_id
-- inner join sys.database_principals r on sys.database_role_members.role_principal_id = r.principal_id