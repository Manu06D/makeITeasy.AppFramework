CREATE TABLE [dbo].[Country]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CountryCode] NCHAR(2) NOT NULL, 
    [Name] VARCHAR(250) NOT NULL,
    [CreationDate] DATETIME2 NULL, 
    [LastModificationDate] DATETIME2 NULL, 
)
