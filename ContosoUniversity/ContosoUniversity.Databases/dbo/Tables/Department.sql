CREATE TABLE [dbo].[Department] (
    [DepartmentID] INT           IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (50) NULL,
    [Budget]       MONEY         NOT NULL,
    [StartDate]    DATETIME2 (7) NOT NULL,
    [InstructorID] INT           NULL,
    [RowVersion]   ROWVERSION    NULL,
    CONSTRAINT [PK_Department] PRIMARY KEY CLUSTERED ([DepartmentID] ASC),
    CONSTRAINT [FK_Department_Instructor_InstructorID] FOREIGN KEY ([InstructorID]) REFERENCES [dbo].[Instructor] ([ID])
);

