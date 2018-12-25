IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id('[dbo].[usp_Select_User]') and type = 'p')
DROP PROC [dbo].[usp_Select_User]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id('[dbo].[usp_SelectAll_Users]') and type = 'p')
DROP PROC [dbo].[usp_SelectAll_Users]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id('[dbo].[usp_Select_User_IdNamePairs]') and type = 'p')
DROP PROC [dbo].[usp_Select_User_IdNamePairs]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id('[dbo].[usp_Insert_User]') and type = 'p')
DROP PROC [dbo].[usp_Insert_User]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id('[dbo].[usp_Update_User]') and type = 'p')
DROP PROC [dbo].[usp_Update_User]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id('[dbo].[usp_Delete_User]') and type = 'p')
DROP PROC [dbo].[usp_Delete_User]
GO

IF EXISTS (SELECT * FROM systypes WHERE name = 'tt_User')
DROP TYPE [dbo].[tt_User]
GO

CREATE TYPE [dbo].[tt_User] AS TABLE(
	[id] nvarchar(50) NOT NULL,
	[first_name] nvarchar(50) NOT NULL,
	[middle_name] nvarchar(50) NULL,
	[last_name] nvarchar(50) NOT NULL,
	[mobile] char(10) NOT NULL,
	[email] nvarchar(50) NOT NULL,
	[account_balance] money NOT NULL,
	[pending_request_charges] money NOT NULL,
	[created_date] datetime NULL,
	[modified_date] datetime NULL
)
GO

CREATE PROC [dbo].[usp_Select_User]
@id nvarchar(50)
AS
BEGIN

	SELECT 
		[id],
		[first_name],
		[middle_name],
		[last_name],
		[mobile],
		[email],
		[account_balance],
		[pending_request_charges],
		[created_date],
		[modified_date]
	FROM [dbo].[User]
	WHERE id = @id

END
GO

CREATE PROC [dbo].[usp_SelectAll_Users]
AS
BEGIN

	SELECT 
		[id],
		[first_name],
		[middle_name],
		[last_name],
		[mobile],
		[email],
		[account_balance],
		[pending_request_charges],
		[created_date],
		[modified_date]
	FROM [dbo].[User]

END
GO

CREATE PROC [dbo].[usp_Select_User_IdNamePairs]
AS
BEGIN

	SELECT id, ([first_name] + ' ' + [middle_name] + ' ' + [last_name]) as name
	FROM [dbo].[User]

END
GO

CREATE PROC [dbo].[usp_Insert_User]
@User tt_User READONLY
AS
BEGIN
    
	DECLARE @today datetime = GETUTCDATE()
	
	INSERT INTO [dbo].[User](
		[id],
		[first_name],
		[middle_name],
		[last_name],
		[mobile],
		[email],
		[account_balance],
		[pending_request_charges],
		[created_date],
		[modified_date])
	SELECT 
		[id],
		[first_name],
		[middle_name],
		[last_name],
		[mobile],
		[email],
		[account_balance],
		[pending_request_charges],
		@today,
		@today
	FROM @User

END        
GO

CREATE PROC [dbo].[usp_Update_User]
@User tt_User READONLY
AS
BEGIN

	UPDATE [dbo].[User]
	SET	
		[first_name] = s.[first_name],
		[middle_name] = s.[middle_name],
		[last_name] = s.[last_name],
		[mobile] = s.[mobile],
		[email] = s.[email],
		[account_balance] = s.[account_balance],
		[pending_request_charges] = s.[pending_request_charges],
		[modified_date] = GETUTCDATE()
	FROM [dbo].[User] AS t
	INNER JOIN @User AS s
	ON t.id = s.id

END        
GO

CREATE PROC [dbo].[usp_Delete_User]
@id nvarchar(50)
AS
BEGIN

	DELETE FROM [dbo].[FavoriteMessageTemplate] WHERE [user_id] = @id

	DELETE FROM [dbo].[FavoriteScreen] WHERE [user_id] = @id

	DELETE FROM [dbo].[Login] WHERE [user_id] = @id

	DELETE FROM [dbo].[Request] WHERE [user_id] = @id

	DELETE FROM [dbo].[Screen] WHERE [owner_id] = @id

	DELETE FROM [dbo].[ScreenRating] WHERE [user_id] = @id

	DELETE FROM [dbo].[User] WHERE id = @id

END        
GO

