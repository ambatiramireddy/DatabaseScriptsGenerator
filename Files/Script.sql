IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id('[dbo].[usp_Insert_SavedScreen]') and type = 'p')
DROP PROC [dbo].[usp_Insert_SavedScreen]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id('[dbo].[usp_Delete_SavedScreen]') and type = 'p')
DROP PROC [dbo].[usp_Delete_SavedScreen]
GO

IF EXISTS (SELECT * FROM systypes WHERE name = 'tt_SavedScreen')
DROP TYPE [dbo].[tt_SavedScreen]
GO

CREATE TYPE [dbo].[tt_SavedScreen] AS TABLE(
	[user_id] nvarchar(50) NOT NULL,
	[screen_id] uniqueidentifier NOT NULL
)
GO

CREATE PROC [dbo].[usp_Insert_SavedScreen]
@SavedScreen tt_SavedScreen READONLY
AS
BEGIN

	INSERT INTO [dbo].[SavedScreen](
		[user_id],
		[screen_id])
	SELECT 
		[user_id],
		[screen_id]
	FROM @SavedScreen


END        
GO

CREATE PROC [dbo].[usp_Delete_SavedScreen]
@user_id nvarchar(50),
@screen_id uniqueidentifier
AS
BEGIN

	DELETE [dbo].[SavedScreen]
	FROM [dbo].[SavedScreen]
	WHERE [dbo].[SavedScreen].[user_id] = @user_id AND [dbo].[SavedScreen].[screen_id] = @screen_id

END        
GO

