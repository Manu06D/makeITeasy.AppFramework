CREATE TABLE [dbo].[OfficeAssignment] (
    [InstructorID] INT           NOT NULL,
    [Location]     NVARCHAR (50) NULL,
    CONSTRAINT [PK_OfficeAssignment] PRIMARY KEY CLUSTERED ([InstructorID] ASC),
    CONSTRAINT [FK_OfficeAssignment_Instructor_InstructorID] FOREIGN KEY ([InstructorID]) REFERENCES [dbo].[Instructor] ([ID]) ON DELETE CASCADE
);

