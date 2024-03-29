/*
This script was created by Visual Studio on 06/04/2023 at 11:45.
Run this script on (localdb)\MSSQLLocalDB.ContosoUniversity2 (optimus\manu) to make it the same as (localdb)\MSSQLLocalDB.ContosoUniversity (optimus\manu).
This script performs its actions in the following order:
1. Disable foreign-key constraints.
2. Perform DELETE commands. 
3. Perform UPDATE commands.
4. Perform INSERT commands.
5. Re-enable foreign-key constraints.
Please back up your target database before running this script.
*/
SET NUMERIC_ROUNDABORT OFF
GO
SET XACT_ABORT, ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT, QUOTED_IDENTIFIER, ANSI_NULLS ON
GO
/*Pointer used for text / image updates. This might not be needed, but is declared here just in case*/
DECLARE @pv binary(16)
BEGIN TRANSACTION
ALTER TABLE [dbo].[OfficeAssignment] DROP CONSTRAINT [FK_OfficeAssignment_Instructor_InstructorID]
ALTER TABLE [dbo].[CourseAssignment] DROP CONSTRAINT [FK_CourseAssignment_Course_CourseID]
ALTER TABLE [dbo].[CourseAssignment] DROP CONSTRAINT [FK_CourseAssignment_Instructor_InstructorID]
ALTER TABLE [dbo].[Department] DROP CONSTRAINT [FK_Department_Instructor_InstructorID]
ALTER TABLE [dbo].[Enrollment] DROP CONSTRAINT [FK_Enrollment_Course_CourseID]
ALTER TABLE [dbo].[Enrollment] DROP CONSTRAINT [FK_Enrollment_Student_StudentID]
ALTER TABLE [dbo].[Course] DROP CONSTRAINT [FK_Course_Department_DepartmentID]
INSERT INTO [dbo].[Course] ([CourseID], [Title], [Credits], [DepartmentID]) VALUES (1045, N'Calculus', 4, 4)
INSERT INTO [dbo].[Course] ([CourseID], [Title], [Credits], [DepartmentID]) VALUES (1050, N'Chemistry', 3, 3)
INSERT INTO [dbo].[Course] ([CourseID], [Title], [Credits], [DepartmentID]) VALUES (2021, N'Composition', 3, 5)
INSERT INTO [dbo].[Course] ([CourseID], [Title], [Credits], [DepartmentID]) VALUES (2042, N'Literature', 4, 5)
INSERT INTO [dbo].[Course] ([CourseID], [Title], [Credits], [DepartmentID]) VALUES (3141, N'Trigonometry', 4, 4)
INSERT INTO [dbo].[Course] ([CourseID], [Title], [Credits], [DepartmentID]) VALUES (4022, N'Microeconomics', 3, 2)
INSERT INTO [dbo].[Course] ([CourseID], [Title], [Credits], [DepartmentID]) VALUES (4041, N'Macroeconomics', 3, 2)
SET IDENTITY_INSERT [dbo].[Enrollment] ON
INSERT INTO [dbo].[Enrollment] ([EnrollmentID], [CourseID], [StudentID], [Grade]) VALUES (1, 4022, 3, 1)
INSERT INTO [dbo].[Enrollment] ([EnrollmentID], [CourseID], [StudentID], [Grade]) VALUES (2, 1050, 3, NULL)
INSERT INTO [dbo].[Enrollment] ([EnrollmentID], [CourseID], [StudentID], [Grade]) VALUES (3, 2021, 2, 1)
INSERT INTO [dbo].[Enrollment] ([EnrollmentID], [CourseID], [StudentID], [Grade]) VALUES (4, 3141, 2, 1)
INSERT INTO [dbo].[Enrollment] ([EnrollmentID], [CourseID], [StudentID], [Grade]) VALUES (5, 1045, 2, 1)
INSERT INTO [dbo].[Enrollment] ([EnrollmentID], [CourseID], [StudentID], [Grade]) VALUES (6, 4041, 1, 1)
INSERT INTO [dbo].[Enrollment] ([EnrollmentID], [CourseID], [StudentID], [Grade]) VALUES (7, 4022, 1, 2)
INSERT INTO [dbo].[Enrollment] ([EnrollmentID], [CourseID], [StudentID], [Grade]) VALUES (8, 1050, 1, 0)
INSERT INTO [dbo].[Enrollment] ([EnrollmentID], [CourseID], [StudentID], [Grade]) VALUES (9, 1050, 4, 1)
INSERT INTO [dbo].[Enrollment] ([EnrollmentID], [CourseID], [StudentID], [Grade]) VALUES (10, 2021, 5, 1)
INSERT INTO [dbo].[Enrollment] ([EnrollmentID], [CourseID], [StudentID], [Grade]) VALUES (11, 2042, 6, 1)
SET IDENTITY_INSERT [dbo].[Enrollment] OFF
SET IDENTITY_INSERT [dbo].[Department] ON
INSERT INTO [dbo].[Department] ([DepartmentID], [Name], [Budget], [StartDate], [InstructorID]) VALUES (2, N'Economics', 100000.0000, '20070901 00:00:00.0000000', 2)
INSERT INTO [dbo].[Department] ([DepartmentID], [Name], [Budget], [StartDate], [InstructorID]) VALUES (3, N'Engineering', 350000.0000, '20070901 00:00:00.0000000', 3)
INSERT INTO [dbo].[Department] ([DepartmentID], [Name], [Budget], [StartDate], [InstructorID]) VALUES (4, N'Mathematics', 100000.0000, '20070901 00:00:00.0000000', 4)
INSERT INTO [dbo].[Department] ([DepartmentID], [Name], [Budget], [StartDate], [InstructorID]) VALUES (5, N'English', 350000.0000, '20070901 00:00:00.0000000', 5)
SET IDENTITY_INSERT [dbo].[Department] OFF
SET IDENTITY_INSERT [dbo].[Student] ON
INSERT INTO [dbo].[Student] ([ID], [LastName], [FirstName], [EnrollmentDate]) VALUES (1, N'Alexander', N'Carson', '20160901 00:00:00.0000000')
INSERT INTO [dbo].[Student] ([ID], [LastName], [FirstName], [EnrollmentDate]) VALUES (2, N'Alonso', N'Meredith', '20180905 00:00:00.0000000')
INSERT INTO [dbo].[Student] ([ID], [LastName], [FirstName], [EnrollmentDate]) VALUES (3, N'Anand', N'Arturo', '20190901 00:00:00.0000000')
INSERT INTO [dbo].[Student] ([ID], [LastName], [FirstName], [EnrollmentDate]) VALUES (4, N'Barzdukas', N'Gytis', '20180901 00:00:00.0000000')
INSERT INTO [dbo].[Student] ([ID], [LastName], [FirstName], [EnrollmentDate]) VALUES (5, N'Li', N'Yan', '20180901 00:00:00.0000000')
INSERT INTO [dbo].[Student] ([ID], [LastName], [FirstName], [EnrollmentDate]) VALUES (6, N'Justice', N'Peggy', '20170901 00:00:00.0000000')
INSERT INTO [dbo].[Student] ([ID], [LastName], [FirstName], [EnrollmentDate]) VALUES (7, N'Norman', N'Laura', '20190901 00:00:00.0000000')
INSERT INTO [dbo].[Student] ([ID], [LastName], [FirstName], [EnrollmentDate]) VALUES (8, N'Olivetto', N'Nino', '20110901 00:00:00.0000000')
SET IDENTITY_INSERT [dbo].[Student] OFF
SET IDENTITY_INSERT [dbo].[Instructor] ON
INSERT INTO [dbo].[Instructor] ([ID], [LastName], [FirstName], [HireDate]) VALUES (1, N'Zheng', N'Roger', '20040212 00:00:00.0000000')
INSERT INTO [dbo].[Instructor] ([ID], [LastName], [FirstName], [HireDate]) VALUES (2, N'Kapoor', N'Candace', '20010115 00:00:00.0000000')
INSERT INTO [dbo].[Instructor] ([ID], [LastName], [FirstName], [HireDate]) VALUES (3, N'Harui', N'Roger', '19980701 00:00:00.0000000')
INSERT INTO [dbo].[Instructor] ([ID], [LastName], [FirstName], [HireDate]) VALUES (4, N'Fakhouri', N'Fadi', '20020706 00:00:00.0000000')
INSERT INTO [dbo].[Instructor] ([ID], [LastName], [FirstName], [HireDate]) VALUES (5, N'Abercrombie', N'Kim', '19950311 00:00:00.0000000')
SET IDENTITY_INSERT [dbo].[Instructor] OFF
INSERT INTO [dbo].[CourseAssignment] ([CourseID], [InstructorID]) VALUES (1045, 4)
INSERT INTO [dbo].[CourseAssignment] ([CourseID], [InstructorID]) VALUES (1050, 2)
INSERT INTO [dbo].[CourseAssignment] ([CourseID], [InstructorID]) VALUES (1050, 3)
INSERT INTO [dbo].[CourseAssignment] ([CourseID], [InstructorID]) VALUES (2021, 5)
INSERT INTO [dbo].[CourseAssignment] ([CourseID], [InstructorID]) VALUES (2042, 5)
INSERT INTO [dbo].[CourseAssignment] ([CourseID], [InstructorID]) VALUES (3141, 3)
INSERT INTO [dbo].[CourseAssignment] ([CourseID], [InstructorID]) VALUES (4022, 1)
INSERT INTO [dbo].[CourseAssignment] ([CourseID], [InstructorID]) VALUES (4041, 1)
INSERT INTO [dbo].[OfficeAssignment] ([InstructorID], [Location]) VALUES (2, N'Thompson 304')
INSERT INTO [dbo].[OfficeAssignment] ([InstructorID], [Location]) VALUES (3, N'Gowan 27')
INSERT INTO [dbo].[OfficeAssignment] ([InstructorID], [Location]) VALUES (4, N'Smith 17')
ALTER TABLE [dbo].[OfficeAssignment]
    ADD CONSTRAINT [FK_OfficeAssignment_Instructor_InstructorID] FOREIGN KEY ([InstructorID]) REFERENCES [dbo].[Instructor] ([ID]) ON DELETE CASCADE
ALTER TABLE [dbo].[CourseAssignment]
    ADD CONSTRAINT [FK_CourseAssignment_Course_CourseID] FOREIGN KEY ([CourseID]) REFERENCES [dbo].[Course] ([CourseID]) ON DELETE CASCADE
ALTER TABLE [dbo].[CourseAssignment]
    ADD CONSTRAINT [FK_CourseAssignment_Instructor_InstructorID] FOREIGN KEY ([InstructorID]) REFERENCES [dbo].[Instructor] ([ID]) ON DELETE CASCADE
ALTER TABLE [dbo].[Department]
    ADD CONSTRAINT [FK_Department_Instructor_InstructorID] FOREIGN KEY ([InstructorID]) REFERENCES [dbo].[Instructor] ([ID])
ALTER TABLE [dbo].[Enrollment]
    ADD CONSTRAINT [FK_Enrollment_Course_CourseID] FOREIGN KEY ([CourseID]) REFERENCES [dbo].[Course] ([CourseID]) ON DELETE CASCADE
ALTER TABLE [dbo].[Enrollment]
    ADD CONSTRAINT [FK_Enrollment_Student_StudentID] FOREIGN KEY ([StudentID]) REFERENCES [dbo].[Student] ([ID]) ON DELETE CASCADE
ALTER TABLE [dbo].[Course]
    ADD CONSTRAINT [FK_Course_Department_DepartmentID] FOREIGN KEY ([DepartmentID]) REFERENCES [dbo].[Department] ([DepartmentID]) ON DELETE CASCADE
COMMIT TRANSACTION
