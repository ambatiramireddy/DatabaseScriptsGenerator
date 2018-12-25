IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id('[dbo].[usp_Select_Request]') and type = 'p')
DROP PROC [dbo].[usp_Select_Request]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id('[dbo].[usp_SelectAll_Requests]') and type = 'p')
DROP PROC [dbo].[usp_SelectAll_Requests]
GO


IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id('[dbo].[usp_Insert_Request]') and type = 'p')
DROP PROC [dbo].[usp_Insert_Request]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id('[dbo].[usp_Update_Request]') and type = 'p')
DROP PROC [dbo].[usp_Update_Request]
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id('[dbo].[usp_Delete_Request]') and type = 'p')
DROP PROC [dbo].[usp_Delete_Request]
GO

IF EXISTS (SELECT * FROM systypes WHERE name = 'tt_Request')
DROP TYPE [dbo].[tt_Request]
GO

CREATE TYPE [dbo].[tt_Request] AS TABLE(
	[id] uniqueidentifier NOT NULL,
	[user_id] nvarchar(50) NOT NULL,
	[request_level] tinyint NOT NULL,
	[screen_id] int NOT NULL,
	[status_code] tinyint NOT NULL,
	[status_desc] nvarchar(500) NULL,
	[message_type] tinyint NOT NULL,
	[message] nvarchar(2000) NULL,
	[image] varbinary NULL,
	[date] date NOT NULL,
	[scheduled_start_time_minutes] smallint NOT NULL,
	[scheduled_duration_seconds] smallint NOT NULL,
	[actual_start_time_minutes] smallint NOT NULL,
	[actual_duration_seconds] smallint NOT NULL,
	[request_charges] money NOT NULL,
	[can_adjust_request_start_time] bit NOT NULL
)
GO

CREATE PROC [dbo].[usp_Select_Request]
@id uniqueidentifier
AS
BEGIN

	SELECT 
		[id],
		[user_id],
		[request_level],
		[screen_id],
		[status_code],
		[status_desc],
		[message_type],
		[message],
		[image],
		[date],
		[scheduled_start_time_minutes],
		[scheduled_duration_seconds],
		[actual_start_time_minutes],
		[actual_duration_seconds],
		[request_charges],
		[can_adjust_request_start_time]
	FROM [dbo].[Request]
	WHERE id = @id

END
GO

CREATE PROC [dbo].[usp_SelectAll_Requests]
AS
BEGIN

	SELECT 
		[id],
		[user_id],
		[request_level],
		[screen_id],
		[status_code],
		[status_desc],
		[message_type],
		[message],
		[image],
		[date],
		[scheduled_start_time_minutes],
		[scheduled_duration_seconds],
		[actual_start_time_minutes],
		[actual_duration_seconds],
		[request_charges],
		[can_adjust_request_start_time]
	FROM [dbo].[Request]

END
GO

CREATE PROC [dbo].[usp_Insert_Request]
@Request tt_Request READONLY
AS
BEGIN

	DECLARE @guid uniqueidentifier = NEWID()

	INSERT INTO [dbo].[Request](
		[id],
		[user_id],
		[request_level],
		[screen_id],
		[status_code],
		[status_desc],
		[message_type],
		[message],
		[image],
		[date],
		[scheduled_start_time_minutes],
		[scheduled_duration_seconds],
		[actual_start_time_minutes],
		[actual_duration_seconds],
		[request_charges],
		[can_adjust_request_start_time])
	SELECT 
		@guid,
		[user_id],
		[request_level],
		[screen_id],
		[status_code],
		[status_desc],
		[message_type],
		[message],
		[image],
		[date],
		[scheduled_start_time_minutes],
		[scheduled_duration_seconds],
		[actual_start_time_minutes],
		[actual_duration_seconds],
		[request_charges],
		[can_adjust_request_start_time]
	FROM @Request

	SELECT @guid

END        
GO

CREATE PROC [dbo].[usp_Update_Request]
@Request tt_Request READONLY
AS
BEGIN

	UPDATE [dbo].[Request]
	SET	
		[user_id] = s.[user_id],
		[request_level] = s.[request_level],
		[screen_id] = s.[screen_id],
		[status_code] = s.[status_code],
		[status_desc] = s.[status_desc],
		[message_type] = s.[message_type],
		[message] = s.[message],
		[image] = s.[image],
		[date] = s.[date],
		[scheduled_start_time_minutes] = s.[scheduled_start_time_minutes],
		[scheduled_duration_seconds] = s.[scheduled_duration_seconds],
		[actual_start_time_minutes] = s.[actual_start_time_minutes],
		[actual_duration_seconds] = s.[actual_duration_seconds],
		[request_charges] = s.[request_charges],
		[can_adjust_request_start_time] = s.[can_adjust_request_start_time]
	FROM [dbo].[Request] AS t
	INNER JOIN @Request AS s
	ON t.id = s.id

END        
GO

CREATE PROC [dbo].[usp_Delete_Request]
@id uniqueidentifier
AS
BEGIN

	DELETE FROM [dbo].[ScreenSlotBooked] WHERE [request_id] = @id

	DELETE FROM [dbo].[Request] WHERE id = @id

END        
GO

