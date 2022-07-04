CREATE procedure [dbo].[p_insert_user_relationship]
 @current_user_name nvarchar(255), @user_to_follow_name nvarchar(255)
as
begin
	-- Get the IDs of the useres in question
	declare @current_user_id NVARCHAR(255), @user_to_follow_id NVARCHAR(255)
	
	select top 1 @current_user_id = UserId
	from SocializerUser
	where Username = @current_user_name

	select top 1 @user_to_follow_id = UserId
	from SocializerUser
	where Username = @user_to_follow_name

	-- Insert
	insert into UserRelationship
	(
		UserId_Fk,
		FollowUserId_Fk,
		Created
	)
	values 
	(
		@current_user_id,
		@user_to_follow_id,
		GETUTCDATE()
	)

end