CREATE TABLE [dbo].[UserRelationship](
	[UserId_Fk] [uniqueidentifier] NOT NULL,
	[FollowUserId_Fk] [uniqueidentifier] NOT NULL,
	[Created] [datetime] NOT NULL,
 CONSTRAINT [PK_UserRelationship] PRIMARY KEY CLUSTERED 
(
	[UserId_Fk] ASC,
	[FollowUserId_Fk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UserRelationship]  WITH CHECK ADD  CONSTRAINT [FK_UserRelationship_FollowUserId] FOREIGN KEY([FollowUserId_Fk])
REFERENCES [dbo].[SocializerUser] ([UserId])
GO

ALTER TABLE [dbo].[UserRelationship] CHECK CONSTRAINT [FK_UserRelationship_FollowUserId]
GO

ALTER TABLE [dbo].[UserRelationship]  WITH CHECK ADD  CONSTRAINT [FK_UserRelationship_UserId] FOREIGN KEY([UserId_Fk])
REFERENCES [dbo].[SocializerUser] ([UserId])
GO

ALTER TABLE [dbo].[UserRelationship] CHECK CONSTRAINT [FK_UserRelationship_UserId]