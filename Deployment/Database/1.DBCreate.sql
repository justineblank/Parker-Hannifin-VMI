USE [master]
GO
/****** Object:  Database [SynogentRFID]    Script Date: 15/03/2018 11:47:00 AM ******/
CREATE DATABASE [SynogentRFID]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'SynogentRFID', FILENAME = N'D:\SQL Data\SynogentRFID.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'SynogentRFID_log', FILENAME = N'D:\SQL Data\SynogentRFID_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [SynogentRFID] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [SynogentRFID].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [SynogentRFID] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [SynogentRFID] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [SynogentRFID] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [SynogentRFID] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [SynogentRFID] SET ARITHABORT OFF 
GO
ALTER DATABASE [SynogentRFID] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [SynogentRFID] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [SynogentRFID] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [SynogentRFID] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [SynogentRFID] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [SynogentRFID] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [SynogentRFID] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [SynogentRFID] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [SynogentRFID] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [SynogentRFID] SET  DISABLE_BROKER 
GO
ALTER DATABASE [SynogentRFID] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [SynogentRFID] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [SynogentRFID] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [SynogentRFID] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [SynogentRFID] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [SynogentRFID] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [SynogentRFID] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [SynogentRFID] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [SynogentRFID] SET  MULTI_USER 
GO
ALTER DATABASE [SynogentRFID] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [SynogentRFID] SET DB_CHAINING OFF 
GO
ALTER DATABASE [SynogentRFID] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [SynogentRFID] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [SynogentRFID] SET DELAYED_DURABILITY = DISABLED 
GO
USE [SynogentRFID]
GO
/****** Object:  Table [dbo].[Readers]    Script Date: 15/03/2018 11:47:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Readers](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[readerId] [varchar](256) NOT NULL,
	[location] [varchar](256) NOT NULL,
	[lastSeen] [datetime] NOT NULL,
 CONSTRAINT [PK_Readers] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Scans]    Script Date: 15/03/2018 11:47:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Scans](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[readerId] [int] NOT NULL,
	[antenna] [int] NULL,
	[epc] [varchar](45) NOT NULL,
	[timestamp] [datetime] NOT NULL,
	[syncStatus] [varchar](45) NULL,
	[Message] [varchar](45) NOT NULL,
 CONSTRAINT [PK_Scans] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  StoredProcedure [dbo].[Proc_payload_Add]    Script Date: 15/03/2018 11:47:00 AM ******/
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
@message		VARCHAR(45)
)
AS	
BEGIN
	declare @t_readerID int = 0
	select @t_readerID = ID from [Readers] where  readerId = @readerId
	if @t_readerID = 0
	begin
		insert into [Readers]
		(readerId,location,lastSeen)
		values
		(@readerId,@location,@lastSeen)

		set @t_readerID = @@IDENTITY
	end
	--
	insert into [Scans]
	(
		[readerId]		,
		[antenna]		,
		[epc]			,
		[timestamp]		,
		[syncStatus]	,
		[Message]
	)
	values
	(
		@t_readerID,
		@antenna,
		@epc,
		@timestamp,
		@syncStatus,
		@message
	)
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
/****** Object:  StoredProcedure [dbo].[Proc_Reader_Add]    Script Date: 15/03/2018 11:47:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Proc_Reader_Add]
(
@ID				INT OUTPUT,
@readerId		VARCHAR(256),
@location		VARCHAR(256),
@lastSeen		DATETIME
)
AS	
BEGIN
	declare @t_readerID int = 0
	select @t_readerID = ID from [Readers] where  readerId = @readerId
	if @t_readerID = 0
	begin
		insert into [Readers]
		(readerId,location,lastSeen)
		values
		(@readerId,@location,@lastSeen)

		set @t_readerID = @@IDENTITY
	end
	--
	SET @ID = @t_readerID
END
GO
/****** Object:  StoredProcedure [dbo].[Proc_Readers_Edit]    Script Date: 15/03/2018 11:47:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Proc_Readers_Edit]
(
@ID				INT OUTPUT,
@location		VARCHAR(256)
)
AS	
BEGIN

	UPDATE [Readers]
	SET 
		[location] = @location
	WHERE
		id	=	@ID

END
GO
/****** Object:  StoredProcedure [dbo].[Proc_Readers_Get]    Script Date: 15/03/2018 11:47:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Proc_Readers_Get]
@readerId varchar(256) = NULL
AS	
BEGIN
	SELECT 
		[id]		,
		[readerId]	,
		[location]	,
		[lastSeen]
	FROM 
	[Readers] WITH (NOLOCK)

	WHERE 
		([readerId] = IsNull(@readerId,[readerId])
		or
		 [location]= IsNull(@readerId,[location])
		)

	order by 
		[lastSeen] desc
END


GO
/****** Object:  StoredProcedure [dbo].[Proc_Scans_Get]    Script Date: 15/03/2018 11:47:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Proc_Scans_Get]
AS	
BEGIN
	SELECT 
		[Scans].[id]			,
		[Readers].[readerId]	,
		[Scans].[antenna]		,
		[Scans].[epc]			,
		[Scans].[timestamp]		,
		[Scans].[syncStatus]	,
		[Scans].[Message]		,
		[Readers].[location]	,
		[Readers].lastSeen
	FROM 
	[Scans] WITH (NOLOCK) 
	inner join
	[Readers] WITH (NOLOCK) 
	on [Readers].id = [Scans].readerId

	Order by 
	[Scans].[timestamp] desc
	
END

GO
USE [master]
GO
ALTER DATABASE [SynogentRFID] SET  READ_WRITE 
GO
