IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'REPODBTST')
BEGIN
	CREATE DATABASE REPODBTST;
END
GO

--------------------------------------------------------------------------------------------------------------------------------------

IF (EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'OrderDetail'))
BEGIN
	DROP TABLE [dbo].[OrderDetail];
END
GO

--------------------------------------------------------------------------------------------------------------------------------------

IF (EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'Order'))
BEGIN
	DROP TABLE [dbo].[Order];
END
GO

--------------------------------------------------------------------------------------------------------------------------------------

IF (EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'Customer'))
BEGIN
	DROP TABLE [dbo].[Customer];
END
GO

--------------------------------------------------------------------------------------------------------------------------------------

IF (EXISTS(SELECT 1 FROM [sys].[objects] WHERE type = 'U' AND name = 'TypeMap'))
BEGIN
	DROP TABLE [dbo].[TypeMap];
END
GO

--------------------------------------------------------------------------------------------------------------------------------------

CREATE TABLE [dbo].[Customer](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[GlobalId] [uniqueidentifier] NOT NULL,
	[FirstName] [nvarchar](128) NOT NULL,
	[LastName] [nvarchar](256) NOT NULL,
	[MiddleName] [nvarchar](256) NOT NULL,
	[Address] [nvarchar](1024) NOT NULL,
	[Email] [nvarchar](128) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[DateInsertedUtc] [datetime2](7) NOT NULL,
	[LastUpdatedUtc] [datetime2](7) NOT NULL,
	[LastUserId] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Customer] ADD  CONSTRAINT [DF_Customer_DateInsertedUtc]  DEFAULT (getutcdate()) FOR [DateInsertedUtc]
GO

ALTER TABLE [dbo].[Customer] ADD  CONSTRAINT [DF_Customer_LastUpdatedUtc]  DEFAULT (getutcdate()) FOR [LastUpdatedUtc]
GO

--------------------------------------------------------------------------------------------------------------------------------------

CREATE TABLE [dbo].[Order](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[GlobalId] [uniqueidentifier] NOT NULL,
	[OrderDateUtc] [datetime2](7) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[SubTotal] [decimal](18, 4) NOT NULL,
	[Freight] [decimal](18, 4) NOT NULL,
	[Tax] [decimal](18, 4) NOT NULL,
	[TotalDue] [decimal](18, 4) NOT NULL,
	[DateInsertedUtc] [datetime2](7) NOT NULL,
	[LastUpdatedUtc] [datetime2](7) NOT NULL,
	[LastUserId] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Order] ADD  CONSTRAINT [DF_Order_DateInsertedUtc]  DEFAULT (getutcdate()) FOR [DateInsertedUtc]
GO

ALTER TABLE [dbo].[Order] ADD  CONSTRAINT [DF_Order_LastUpdatedUtc]  DEFAULT (getutcdate()) FOR [LastUpdatedUtc]
GO

ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([Id])
GO

ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_Customer]
GO

--------------------------------------------------------------------------------------------------------------------------------------

CREATE TABLE [dbo].[OrderDetail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[GlobalId] [uniqueidentifier] NOT NULL,
	[OrderId] [int] NOT NULL,
	[ProductCode] [nvarchar](32) NOT NULL,
	[ProductName] [nvarchar](128) NOT NULL,
	[UnitPrice] [decimal](18, 4) NOT NULL,
	[Quantity] [int] NOT NULL,
	[Discount] [decimal](18, 4) NOT NULL,
	[LineTotal] [decimal](18, 4) NOT NULL,
	[DateInsertedUtc] [datetime2](7) NOT NULL,
	[LastUpdatedUtc] [datetime2](7) NOT NULL,
	[LastUserId] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK_OrderDetail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[OrderDetail] ADD  CONSTRAINT [DF_OrderDetail_DateInsertedUtc]  DEFAULT (getutcdate()) FOR [DateInsertedUtc]
GO

ALTER TABLE [dbo].[OrderDetail] ADD  CONSTRAINT [DF_OrderDetail_LastUpdatedUtc]  DEFAULT (getutcdate()) FOR [LastUpdatedUtc]
GO

ALTER TABLE [dbo].[OrderDetail]  WITH CHECK ADD  CONSTRAINT [FK_OrderDetail_Order] FOREIGN KEY([OrderId])
REFERENCES [dbo].[Order] ([Id])
GO

ALTER TABLE [dbo].[OrderDetail] CHECK CONSTRAINT [FK_OrderDetail_Order]
GO

--------------------------------------------------------------------------------------------------------------------------------------

DBCC CHECKIDENT ([Customer], RESEED, 1);
DBCC CHECKIDENT ([Order], RESEED, 1);
DBCC CHECKIDENT ([OrderDetail], RESEED, 1);
GO

DECLARE @counter INT = (SELECT MAX(Id) FROM [dbo].[Customer]);
SET @counter = COALESCE(@counter, 1);
WHILE (@counter < 10)
BEGIN

	INSERT INTO [dbo].[Customer](
	 [GlobalId]
	,[FirstName]
	,[LastName]
	,[MiddleName]
	,[Address]
	,[Email]
	,[IsActive]
	,[DateInsertedUtc]
	,[LastUpdatedUtc]
	,[LastUserId]
	) VALUES (
	 NEWID()
	,'Juan'
	,'dela Cruz'
	,'Pinto'
	,'San Lorenzo, Makati, Philippines'
	,'juandelacruz@gmail.com'
	,1
	,GETUTCDATE()
	,GETUTCDATE()
	,SYSTEM_USER
	);
	SET @counter = @counter + 1;
END
GO

--------------------------------------------------------------------------------------------------------------------------------------

CREATE TABLE [dbo].[TypeMap](
	[SessionId] [uniqueidentifier] NOT NULL,
	[bigint_column] [bigint] NULL,
	[binary_column] [binary](255) NULL,
	[bit_column] [bit] NULL,
	[char_column] [char](32) NULL,
	[date_column] [date] NULL,
	[datetime_column] [datetime] NULL,
	[datetime2_column] [datetime2](7) NULL,
	[datetimeoffset_column] [datetimeoffset](7) NULL,
	[decimal_column] [decimal](18, 4) NULL,
	[float_column] [float] NULL,
	[geography_column] [geography] NULL,
	[geometry_column] [geometry] NULL,
	[hierarchyid_column] [hierarchyid] NULL,
	[image_column] [image] NULL,
	[int_column] [int] NULL,
	[money_column] [money] NULL,
	[nchar_column] [nchar](32) NULL,
	[ntext_column] [ntext] NULL,
	[numeric_column] [numeric](18, 4) NULL,
	[nvarchar_column] [nvarchar](50) NULL,
	[nvarcharmax_column] [nvarchar](max) NULL,
	[real_column] [real] NULL,
	[smalldatetime_column] [smalldatetime] NULL,
	[smallint_column] [smallint] NULL,
	[smallmoney_column] [smallmoney] NULL,
	[sql_variant_column] [sql_variant] NULL,
	[text_column] [text] NULL,
	[time_column] [time](7) NULL,
	[timestamp_column] [timestamp] NULL,
	[tinyint_column] [tinyint] NULL,
	[uniqueidentifier] [uniqueidentifier] NULL,
	[varbinary_column] [varbinary](50) NULL,
	[varbinarymax_column] [varbinary](max) NULL,
	[varchar_column] [varchar](255) NULL,
	[varcharmax_column] [varchar](max) NULL,
	[xml_column] [xml] NULL,
 CONSTRAINT [PK_TypeMap] PRIMARY KEY CLUSTERED 
(
	[SessionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[TypeMap] ADD  CONSTRAINT [DF_TypeMap_SessionId]  DEFAULT (newid()) FOR [SessionId]
GO

