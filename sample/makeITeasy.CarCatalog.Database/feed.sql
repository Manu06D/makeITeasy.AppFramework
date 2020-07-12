--connect to (LocalDb)\MSSQLLocalDB for local test

SET IDENTITY_INSERT [dbo].[Country] ON
INSERT INTO [dbo].[Country] ([ID], [CountryCode], [Name]) VALUES (1, N'FR','France')
INSERT INTO [dbo].[Country] ([ID], [CountryCode], [Name]) VALUES (2, N'IT', 'Italy')
INSERT INTO [dbo].[Country] ([ID], [CountryCode], [Name]) VALUES (3, N'US', 'USA')
SET IDENTITY_INSERT [dbo].[Country] OFF

SET IDENTITY_INSERT [dbo].[Brand] ON
INSERT INTO [dbo].[Brand] ([ID], [Name], CountryID) VALUES (1, N'Peugeot',1)
INSERT INTO [dbo].[Brand] ([ID], [Name], CountryID) VALUES (2, N'Renault',1)
INSERT INTO [dbo].[Brand] ([ID], [Name], CountryID) VALUES (3, N'Ferrari',2)
INSERT INTO [dbo].[Brand] ([ID], [Name], CountryID) VALUES (4, N'Fiat',2)
SET IDENTITY_INSERT [dbo].[Brand] OFF

SET IDENTITY_INSERT [dbo].[Car] ON
INSERT INTO [dbo].[Car] ([ID], [Name], [BrandID], [ReleaseYear]) VALUES (1,'205', 1, 1982)
INSERT INTO [dbo].[Car] ([ID], [Name], [BrandID], [ReleaseYear]) VALUES (2,'3008 II', 1, 2016)
INSERT INTO [dbo].[Car] ([ID], [Name], [BrandID], [ReleaseYear]) VALUES (3,'R5', 2, 1972)
INSERT INTO [dbo].[Car] ([ID], [Name], [BrandID], [ReleaseYear]) VALUES (4,'Talisman', 2, 2015)
INSERT INTO [dbo].[Car] ([ID], [Name], [BrandID], [ReleaseYear]) VALUES (5,'Testarossa', 3, 1984)
INSERT INTO [dbo].[Car] ([ID], [Name], [BrandID], [ReleaseYear]) VALUES (6,'500 (4eme génération)', 4, 2020)
INSERT INTO [dbo].[Car] ([ID], [Name], [BrandID], [ReleaseYear]) VALUES (7,'500 (3eme génération)', 4, 2007)
INSERT INTO [dbo].[Car] ([ID], [Name], [BrandID], [ReleaseYear]) VALUES (8,'500', 4, 1936)
SET IDENTITY_INSERT [dbo].[Car] OFF