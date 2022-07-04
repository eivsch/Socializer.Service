CREATE TABLE [dbo].[SocializerUser](
	[UserId] [uniqueidentifier] NOT NULL,
	[Username] [nvarchar](255) NOT NULL,
	[UserCreated] [datetime] NOT NULL,
	[UserDataJson] [nvarchar](max) NULL,
 CONSTRAINT [PK_SocializerUser] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[SocializerUser] ADD  DEFAULT (newid()) FOR [UserId]