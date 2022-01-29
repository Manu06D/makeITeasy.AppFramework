CREATE TABLE [dbo].[Course] (
    [CourseID]     INT           NOT NULL,
    [Title]        NVARCHAR (50) NULL,
    [Credits]      INT           NOT NULL,
    [DepartmentID] INT           NOT NULL,
    CONSTRAINT [PK_Course] PRIMARY KEY CLUSTERED ([CourseID] ASC),
    CONSTRAINT [FK_Course_Department_DepartmentID] FOREIGN KEY ([DepartmentID]) REFERENCES [dbo].[Department] ([DepartmentID]) ON DELETE CASCADE
);

