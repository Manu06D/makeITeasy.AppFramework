CREATE TABLE [dbo].[CompositeKeySubTable]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ParentKey1] INT NOT NULL, 
    [ParentKey2] INT NOT NULL, 
    [Value] NVARCHAR(50) NOT NULL, 
    CONSTRAINT [FK_CompositeKeySubTable_ToCompositeKeyTable] FOREIGN KEY (ParentKey1, ParentKey2) REFERENCES [CompositeKeyTable](Id1, Id2)
)
