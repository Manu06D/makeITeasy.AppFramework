CREATE TABLE [dbo].[Instructor] (
    [ID]        INT           IDENTITY (1, 1) NOT NULL,
    [LastName]  NVARCHAR (50) NOT NULL,
    [FirstName] NVARCHAR (50) NOT NULL,
    [HireDate]  DATETIME2 (7) NOT NULL,
    CONSTRAINT [PK_Instructor] PRIMARY KEY CLUSTERED ([ID] ASC)
);

