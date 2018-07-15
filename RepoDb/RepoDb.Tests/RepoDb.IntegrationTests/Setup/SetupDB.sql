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