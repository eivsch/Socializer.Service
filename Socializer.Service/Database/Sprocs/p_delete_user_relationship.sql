create procedure [dbo].[p_delete_user_relationship]
 @current_user_name nvarchar(255), @user_to_unfollow_name nvarchar(255)
as
begin
	-- Get the IDs of the useres in question
	declare @current_user_id NVARCHAR(255), @user_to_unfollow_id NVARCHAR(255)
	
	select top 1 @current_user_id = UserId
	from SocializerUser
	where Username = @current_user_name

	select top 1 @user_to_unfollow_id = UserId
	from SocializerUser
	where Username = @user_to_unfollow_name

	-- Delete
	delete UserRelationship where UserId_Fk = @current_user_id and FollowUserId_Fk = @user_to_unfollow_id
end