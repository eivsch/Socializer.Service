CREATE FUNCTION [dbo].[f_tbl_get_user_relation]
(	
	@current_user_name nvarchar(255),
	@rel_username nvarchar(255)
)
RETURNS TABLE 
AS
RETURN 
(
	WITH base AS (
		SELECT 
			u_current.Username AS CurrentUserName, 
			u_current.UserId AS CurrentUserId, 
			u_rel.Username AS RelatedUserName,
			u_rel.UserId AS RelatedUserId
		FROM SocializerUser u_current
		JOIN SocializerUser u_rel ON u_rel.Username = @rel_username
		WHERE u_current.Username = @current_user_name
	)
	SELECT 
		base.*, 
		IIF(rel.UserId_Fk IS NOT NULL, 1, 0) AS CurrentUserFollowsRelatedUser,
		IIF(rel2.UserId_Fk IS NOT NULL, 1, 0) AS RelatedUserFollowsCurrentUser
	FROM base
	LEFT JOIN UserRelationship rel ON rel.UserId_Fk = base.CurrentUserId AND rel.FollowUserId_Fk = base.RelatedUserId 
	LEFT JOIN UserRelationship rel2 ON rel2.UserId_Fk = base.RelatedUserId AND rel2.FollowUserId_Fk = base.CurrentUserId
)