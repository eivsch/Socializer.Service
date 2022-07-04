CREATE view [dbo].[v_user_feed] as
	select 
		ur.UserId_Fk as FeedUserId,
		p.PostId,
		p.PostUserId_Fk,
		p.PostCreated,
		usr.Username,
		JSON_VALUE(p.PostDataJson, '$.Text') AS PostHeader,
	    JSON_VALUE(p.PostDataJson, '$.PictureId') AS PictureId,
	    JSON_VALUE(p.PostDataJson, '$.PictureUri') AS PictureUri
	from UserRelationship ur
	join Post p on p.PostUserId_Fk = ur.FollowUserId_Fk
	join SocializerUser usr on usr.UserId = p.PostUserId_Fk