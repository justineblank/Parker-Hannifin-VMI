USE [SynogentRFID]
GO

/****** Object:  StoredProcedure [dbo].[Proc_payload_Add]    Script Date: 03/04/2018 7:06:25 PM ******/
DROP PROCEDURE [dbo].[Proc_payload_Add]
GO

/****** Object:  StoredProcedure [dbo].[Proc_payload_Add]    Script Date: 03/04/2018 7:06:25 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Proc_payload_Add]
(
@readerId		VARCHAR(256),
@location		VARCHAR(256),
@lastSeen		DATETIME,
@antenna		INT,
@epc			VARCHAR(45),
@timestamp		DATETIME,
@syncStatus		VARCHAR(45),
@message		VARCHAR(45) OUTPUT
)
AS	
BEGIN
	declare @t_readerID int = 0
	select @t_readerID = ID from [Readers] with(nolock) where  readerId = @readerId
	set @message = ''
	--	
	
	IF @t_readerID = 0
	BEGIN
		INSERT INTO [Readers]
		(readerId,location,lastSeen)
		VALUES
		(@readerId,@location,@lastSeen)

		SET @t_readerID = @@IDENTITY
	END
	--
	IF (
		SELECT COUNT(*) 
		FROM [Scans] WITH(NOLOCK) 
		WHERE [readerId] = @t_readerID 
		AND [epc] = @epc 
		AND [timestamp] > dateadd(HOUR, -36, @timestamp)
		) > 0
	BEGIN
		SET @message =  'epc already present'
	END
	ELSE
	BEGIN
	--
	INSERT INTO [Scans]
	(
		[readerId]		,
		[antenna]		,
		[epc]			,
		[timestamp]		,
		[syncStatus]	,
		[Message]
	)
	VALUES
	(
		@t_readerID,
		@antenna,
		@epc,
		@timestamp,
		@syncStatus,
		@message
	)
	END
	--
	IF @@ERROR <> 0
	BEGIN
    	select 99 as 'ResultValue';
	END
	ELSE
	BEGIN
	  	select 0 as 'ResultValue',@@IDENTITY as [id];	  
	END	
END
GO


