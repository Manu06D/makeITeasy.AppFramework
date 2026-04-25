CREATE TABLE [dbo].[Enrollment] (
    [EnrollmentID] INT IDENTITY (1, 1) NOT NULL,
    [CourseID]     INT NOT NULL,
    [StudentID]    INT NOT NULL,
    [Grade]        INT NULL,
    CONSTRAINT [PK_Enrollment] PRIMARY KEY CLUSTERED ([EnrollmentID] ASC),
    CONSTRAINT [FK_Enrollment_Course_CourseID] FOREIGN KEY ([CourseID]) REFERENCES [dbo].[Course] ([CourseID]) ON DELETE CASCADE,
    CONSTRAINT [FK_Enrollment_Student_StudentID] FOREIGN KEY ([StudentID]) REFERENCES [dbo].[Student] ([ID]) ON DELETE CASCADE
);

