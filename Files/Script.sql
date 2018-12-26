IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id('[dbo].[usp_Select_Country]') and type = 'p')
DROP PROC [dbo].[usp_Select_Country]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id('[dbo].[usp_SelectAll_Countries]') and type = 'p')
DROP PROC [dbo].[usp_SelectAll_Countries]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id('[dbo].[usp_Select_Country_IdNamePairs]') and type = 'p')
DROP PROC [dbo].[usp_Select_Country_IdNamePairs]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id('[dbo].[usp_Insert_Country]') and type = 'p')
DROP PROC [dbo].[usp_Insert_Country]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id('[dbo].[usp_Update_Country]') and type = 'p')
DROP PROC [dbo].[usp_Update_Country]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id('[dbo].[usp_Delete_Country]') and type = 'p')
DROP PROC [dbo].[usp_Delete_Country]
GO

IF EXISTS (SELECT * FROM systypes WHERE name = 'tt_Country')
DROP TYPE [dbo].[tt_Country]
GO

CREATE TYPE [dbo].[tt_Country] AS TABLE(
	[id] int NOT NULL,
	[name] nvarchar(100) NOT NULL
)
GO

CREATE PROC [dbo].[usp_Select_Country]
@id int
AS
BEGIN

	SELECT 
		[id],
		[name]
	FROM [dbo].[Country]
	WHERE id = @id

END
GO

CREATE PROC [dbo].[usp_SelectAll_Countries]
AS
BEGIN

	SELECT 
		[id],
		[name]
	FROM [dbo].[Country]

END
GO

CREATE PROC [dbo].[usp_Select_Country_IdNamePairs]
AS
BEGIN

	SELECT [id], ([name]) as [name]
	FROM [dbo].[Country]

END
GO

CREATE PROC [dbo].[usp_Insert_Country]
@Country tt_Country READONLY
AS
BEGIN


	INSERT INTO [dbo].[Country](
		[name])
	SELECT 
		[name]
	FROM @Country

	SELECT SCOPE_IDENTITY()

END        
GO

CREATE PROC [dbo].[usp_Update_Country]
@Country tt_Country READONLY
AS
BEGIN

	UPDATE [dbo].[Country]
	SET	
		[name] = s.[name]
	FROM [dbo].[Country] AS t
	INNER JOIN @Country AS s
	ON t.id = s.id

END        
GO

CREATE PROC [dbo].[usp_Delete_Country]
@id int
AS
BEGIN

	DELETE [dbo].[Country]
	FROM [dbo].[Country]
	INNER JOIN [dbo].[State] ON [dbo].[Country].[id] = [dbo].[State].[country_id]
	INNER JOIN [dbo].[City] ON [dbo].[State].[id] = [dbo].[City].[state_id]
	INNER JOIN [dbo].[Area] ON [dbo].[City].[id] = [dbo].[Area].[city_id]
	INNER JOIN [dbo].[Screen] ON [dbo].[Area].[id] = [dbo].[Screen].[area_id]
	INNER JOIN [dbo].[FavoriteScreen] ON [dbo].[Screen].[id] = [dbo].[FavoriteScreen].[screen_id]
	INNER JOIN [dbo].[Request] ON [dbo].[Screen].[id] = [dbo].[Request].[screen_id]
	INNER JOIN [dbo].[ScreenCharge] ON [dbo].[Screen].[id] = [dbo].[ScreenCharge].[screen_id]
	INNER JOIN [dbo].[ScreenOwnerMessage] ON [dbo].[Screen].[id] = [dbo].[ScreenOwnerMessage].[screen_id]
	INNER JOIN [dbo].[ScreenRating] ON [dbo].[Screen].[id] = [dbo].[ScreenRating].[screen_id]
	INNER JOIN [dbo].[ScreenSlotBooked] ON [dbo].[Request].[id] = [dbo].[ScreenSlotBooked].[request_id]
	WHERE [dbo].[Country].[id] = @id

END        
GO

