CREATE TABLE [dbo].[CourseAssignment] (
    [InstructorID] INT NOT NULL,
    [CourseID]     INT NOT NULL,
    CONSTRAINT [PK_CourseAssignment] PRIMARY KEY CLUSTERED ([CourseID] ASC, [InstructorID] ASC),
    CONSTRAINT [FK_CourseAssignment_Course_CourseID] FOREIGN KEY ([CourseID]) REFERENCES [dbo].[Course] ([CourseID]) ON DELETE CASCADE,
    CONSTRAINT [FK_CourseAssignment_Instructor_InstructorID] FOREIGN KEY ([InstructorID]) REFERENCES [dbo].[Instructor] ([ID]) ON DELETE CASCADE
);

