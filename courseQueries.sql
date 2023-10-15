USE [CourseManagement]
GO
/****** Object:  Table [dbo].[Course]    Script Date: 10/11/2023 11:41:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Course](
	[CourseId] [int] IDENTITY(1,1) NOT NULL,
	[AccId] [int] NOT NULL,
	[CourseName] [nvarchar](512) NOT NULL,
	[Image] [nvarchar](1024) NULL,
	[Fee] [float] NULL,
	[Status] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[EnrollmentCount] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[CourseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 10/11/2023 11:41:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[RoleId] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [varchar](100) NOT NULL,
	[Description] [varchar](1024) NULL,
PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 10/11/2023 11:41:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [varchar](100) NOT NULL,
	[Email] [varchar](256) NOT NULL,
	[Password] [varchar](256) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRole]    Script Date: 10/11/2023 11:41:01 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRole](
	[UserRoleId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[RoleId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[UserRoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Course] ON 
GO
INSERT [dbo].[Course] ([CourseId], [AccId], [CourseName], [Image], [Fee], [Status], [CreatedDate], [EnrollmentCount]) VALUES (1, 1, N'Course 1', NULL, 300000, 1, CAST(N'2023-10-11T00:00:00.000' AS DateTime), 3)
GO
SET IDENTITY_INSERT [dbo].[Course] OFF
GO
SET IDENTITY_INSERT [dbo].[Role] ON 
GO
INSERT [dbo].[Role] ([RoleId], [RoleName], [Description]) VALUES (1, N'Admin', NULL)
GO
INSERT [dbo].[Role] ([RoleId], [RoleName], [Description]) VALUES (2, N'Normal User', NULL)
GO
SET IDENTITY_INSERT [dbo].[Role] OFF
GO
SET IDENTITY_INSERT [dbo].[User] ON 
GO
INSERT [dbo].[User] ([UserId], [UserName], [Email], [Password]) VALUES (1, N'admin123', N'admin123@abc.com', N'123456')
GO
INSERT [dbo].[User] ([UserId], [UserName], [Email], [Password]) VALUES (2, N'admin13', N'user@example.com', N'string')
GO
INSERT [dbo].[User] ([UserId], [UserName], [Email], [Password]) VALUES (3, N'user133', N'use111@example.com', N'cnscs')
GO
SET IDENTITY_INSERT [dbo].[User] OFF
GO
SET IDENTITY_INSERT [dbo].[UserRole] ON 
GO
INSERT [dbo].[UserRole] ([UserRoleId], [UserId], [RoleId]) VALUES (1, 1, 1)
GO
INSERT [dbo].[UserRole] ([UserRoleId], [UserId], [RoleId]) VALUES (2, 1, 2)
GO
SET IDENTITY_INSERT [dbo].[UserRole] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Role__8A2B6160D538EBDF]    Script Date: 10/11/2023 11:41:01 AM ******/
ALTER TABLE [dbo].[Role] ADD UNIQUE NONCLUSTERED 
(
	[RoleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__User__A9D105342450E89A]    Script Date: 10/11/2023 11:41:01 AM ******/
ALTER TABLE [dbo].[User] ADD UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__User__C9F284560939D3D9]    Script Date: 10/11/2023 11:41:01 AM ******/
ALTER TABLE [dbo].[User] ADD UNIQUE NONCLUSTERED 
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Course] ADD  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[Course]  WITH CHECK ADD FOREIGN KEY([AccId])
REFERENCES [dbo].[User] ([UserId])
GO
ALTER TABLE [dbo].[UserRole]  WITH CHECK ADD FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([RoleId])
GO
ALTER TABLE [dbo].[UserRole]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([UserId])
GO
