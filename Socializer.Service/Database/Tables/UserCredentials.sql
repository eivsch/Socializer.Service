CREATE TABLE [dbo].[UserCredentials](
	[CredentialsId] [int] IDENTITY(1,1) NOT NULL,
	[UserId_Fk] [uniqueidentifier] NOT NULL,
	[Password] [nvarchar](255) NOT NULL,
	[Token] [nvarchar](255) NOT NULL,
	[Created] [datetime] NOT NULL,
 CONSTRAINT [PK_Account] PRIMARY KEY CLUSTERED 
(
	[CredentialsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UserCredentials]  WITH CHECK ADD  CONSTRAINT [FK_UserCredentials_UserId] FOREIGN KEY([UserId_Fk])
REFERENCES [dbo].[SocializerUser] ([UserId])
GO

ALTER TABLE [dbo].[UserCredentials] CHECK CONSTRAINT [FK_UserCredentials_UserId]