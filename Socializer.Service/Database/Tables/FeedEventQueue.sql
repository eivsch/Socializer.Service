CREATE TABLE [dbo].[FeedEventQueue](
	[EventGuid] [uniqueidentifier] NOT NULL,
	[EventCreated] [datetime] NOT NULL,
	[EventType] [varchar](255) NOT NULL,
	[EventDataJson] [nvarchar](max) NULL,
 CONSTRAINT [PK_EventQueue] PRIMARY KEY NONCLUSTERED 
(
	[EventGuid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[FeedEventQueue] ADD  DEFAULT (newid()) FOR [EventGuid]