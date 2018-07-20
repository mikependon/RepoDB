DELETE FROM [dbo].[Customer];
DELETE FROM [dbo].[Order];
DELETE FROM [dbo].[OrderDetail];
GO

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