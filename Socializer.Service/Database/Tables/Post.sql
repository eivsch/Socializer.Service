CREATE TABLE [dbo].[Post](
	[PostId] [uniqueidentifier] NOT NULL,
	[PostUserId_Fk] [uniqueidentifier] NOT NULL,
	[PostCreated] [datetime] NOT NULL,
	[PostDataJson] [nvarchar](max) NULL,
 CONSTRAINT [PK_Post] PRIMARY KEY CLUSTERED 
(
	[PostId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Post] ADD  DEFAULT (newid()) FOR [PostId]
GO

ALTER TABLE [dbo].[Post]  WITH CHECK ADD  CONSTRAINT [FK_Post_UserId] FOREIGN KEY([PostUserId_Fk])
REFERENCES [dbo].[SocializerUser] ([UserId])
GO

ALTER TABLE [dbo].[Post] CHECK CONSTRAINT [FK_Post_UserId]