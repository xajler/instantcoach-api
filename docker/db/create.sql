USE [ic-staging];
GO

CREATE USER [icUser] FOR LOGIN [icUser];
ALTER ROLE [db_owner] ADD MEMBER [icUser];

GO

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

CREATE SEQUENCE [ic_hilo] START WITH 1 INCREMENT BY 10 NO MINVALUE NO MAXVALUE NO CYCLE;

GO

CREATE TABLE [InstantCoaches] (
    [Id] int NOT NULL,
    [Description] nvarchar(1000) NOT NULL,
    [Status] tinyint NOT NULL,
    [TicketId] nvarchar(64) NOT NULL,
    [Reference] nvarchar(16) NOT NULL,
    [EvaluatorId] int NOT NULL,
    [AgentId] int NOT NULL,
    [EvaluatorName] nvarchar(128) NOT NULL,
    [AgentName] nvarchar(128) NOT NULL,
    [CommentsCount] int NOT NULL,
    [Comments] NVARCHAR(MAX) NOT NULL,
    [BookmarkPins] NVARCHAR(MAX) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_InstantCoaches] PRIMARY KEY ([Id])
);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190708161941_Initial', N'2.2.6-servicing-10079');

GO

CREATE PROCEDURE [dbo].[InstantCoach_List]
	@Skip INT,
	@Take INT,
	@ShowCompleted BIT
AS
	WITH Result (RowNumber, Id, [Status], Reference, [Description], CreatedAt, UpdateAt, CommentsCount, EvaluatorName) AS
	(
		SELECT ROW_NUMBER() OVER (ORDER BY ic.CreatedAt DESC, ic.Status ASC) AS 'RowNumber',
		       ic.Id, ic.[Status], ic.Reference, ic.Description, ic.CreatedAt, ic.UpdatedAt, ic.CommentsCount, ic.EvaluatorName
		FROM InstantCoaches ic
        WHERE  (@ShowCompleted = 0 AND ic.[Status] < 5) OR (@ShowCompleted = 1 AND 1 = 1)
	)

	SELECT (SELECT CAST(MAX(RowNumber) AS INT) FROM Result) AS TotalCount, Id, [Status], Reference, [Description], CreatedAt, UpdateAt, CommentsCount, EvaluatorName
	FROM Result
	ORDER BY CreatedAt DESC, [Status] ASC
	OFFSET (@Skip * @Take) ROWS
	FETCH NEXT @Take ROWS ONLY


GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20190708162018_StoreProcedureInstantCoachList', N'2.2.6-servicing-10079');

GO
