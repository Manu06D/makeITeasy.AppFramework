CREATE TABLE [dbo].[CompositeKeyTable]
(
	[Id1] INT NOT NULL, 
	[Id2] INT NOT NULL, 
    [Col1] INT NULL, 
    CONSTRAINT [PK_CompositeKeyTable] PRIMARY KEY CLUSTERED ([Id1], [Id2])
)
