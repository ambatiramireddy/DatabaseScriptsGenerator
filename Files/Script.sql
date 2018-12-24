IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id('[dbo].[usp_Select_Screen]') and type = 'p')
DROP PROC [dbo].[usp_Select_Screen]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id('[dbo].[usp_SelectAll_Screens]') and type = 'p')
DROP PROC [dbo].[usp_SelectAll_Screens]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id('[dbo].[usp_Insert_Screen]') and type = 'p')
DROP PROC [dbo].[usp_Insert_Screen]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id('[dbo].[usp_Update_Screen]') and type = 'p')
DROP PROC [dbo].[usp_Update_Screen]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id('[dbo].[usp_Delete_Screen]') and type = 'p')
DROP PROC [dbo].[usp_Delete_Screen]
GO

IF EXISTS (SELECT * FROM systypes WHERE name = 'tt_Screen')
DROP TYPE [dbo].[tt_Screen]
GO

CREATE TYPE [dbo].[tt_Screen] AS TABLE(
	[id] int NOT NULL,
	[state_id] smallint NOT NULL,
	[city_id] smallint NOT NULL,
	[area_id] smallint NOT NULL,
	[owner_id] nvarchar(50) NOT NULL,
	[name] nchar(50) NOT NULL,
	[description] nvarchar(250) NULL,
	[type] nvarchar(50) NOT NULL,
	[size_inches] int NOT NULL,
	[resolution] nvarchar(20) NULL,
	[opening_time_minutes] smallint NOT NULL,
	[closing_time_minutes] smallint NOT NULL,
	[charges_per_second] money NOT NULL,
	[allowed_for_public_use] bit NOT NULL
)
GO

CREATE PROC [dbo].[usp_Select_Screen]
@id int
AS
BEGIN

	SELECT 
		[id],
		[state_id],
		[city_id],
		[area_id],
		[owner_id],
		[name],
		[description],
		[type],
		[size_inches],
		[resolution],
		[opening_time_minutes],
		[closing_time_minutes],
		[charges_per_second],
		[allowed_for_public_use]
	FROM [dbo].[Screen]
	WHERE id = @id

END
GO

CREATE PROC [dbo].[usp_SelectAll_Screens]
AS
BEGIN

	SELECT 
		[id],
		[state_id],
		[city_id],
		[area_id],
		[owner_id],
		[name],
		[description],
		[type],
		[size_inches],
		[resolution],
		[opening_time_minutes],
		[closing_time_minutes],
		[charges_per_second],
		[allowed_for_public_use]
	FROM [dbo].[Screen]

END
GO

CREATE PROC [dbo].[usp_Insert_Screen]
@Screen tt_Screen READONLY
AS
BEGIN
    
	INSERT INTO [dbo].[Screen](
		[id],
		[state_id],
		[city_id],
		[area_id],
		[owner_id],
		[name],
		[description],
		[type],
		[size_inches],
		[resolution],
		[opening_time_minutes],
		[closing_time_minutes],
		[charges_per_second],
		[allowed_for_public_use])
	SELECT 
		[id],
		[state_id],
		[city_id],
		[area_id],
		[owner_id],
		[name],
		[description],
		[type],
		[size_inches],
		[resolution],
		[opening_time_minutes],
		[closing_time_minutes],
		[charges_per_second],
		[allowed_for_public_use]
	FROM @Screen

END        
GO

CREATE PROC [dbo].[usp_Update_Screen]
@Screen tt_Screen READONLY
AS
BEGIN

	UPDATE [dbo].[Screen]
	SET	
		[state_id] = s.[state_id],
		[city_id] = s.[city_id],
		[area_id] = s.[area_id],
		[owner_id] = s.[owner_id],
		[name] = s.[name],
		[description] = s.[description],
		[type] = s.[type],
		[size_inches] = s.[size_inches],
		[resolution] = s.[resolution],
		[opening_time_minutes] = s.[opening_time_minutes],
		[closing_time_minutes] = s.[closing_time_minutes],
		[charges_per_second] = s.[charges_per_second],
		[allowed_for_public_use] = s.[allowed_for_public_use]
	FROM [dbo].[Screen] AS t
	INNER JOIN @Screen AS s
	ON t.id = s.id

END        
GO

CREATE PROC [dbo].[usp_Delete_Screen]
@id int
AS
BEGIN

	DELETE FROM [dbo].[FavoriteScreen] WHERE [screen_id] = @id

	DELETE FROM [dbo].[Request] WHERE [screen_id] = @id

	DELETE FROM [dbo].[ScreenCharge] WHERE [screen_id] = @id

	DELETE FROM [dbo].[ScreenOwnerMessage] WHERE [screen_id] = @id

	DELETE FROM [dbo].[ScreenRating] WHERE [screen_id] = @id

	DELETE FROM [dbo].[ScreenSlotBooked] WHERE [screen_id] = @id

	DELETE FROM [dbo].[Screen] WHERE id = @id

END        
GO

