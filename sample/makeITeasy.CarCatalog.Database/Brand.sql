﻿CREATE TABLE [dbo].[Brand]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(100) NOT NULL, 
    [Logo] NVARCHAR(250) NULL, 
    [CountryID] INT NOT NULL, 
        [CreationDate] DATETIME2 NULL, 
    [LastModificationDate] DATETIME2 NULL, 
    [DynamicBrandDetails] NVARCHAR(MAX) NULL, 
    CONSTRAINT [FK_Brand_ToCountry] FOREIGN KEY ([CountryID]) REFERENCES [Country]([ID])
)
