using System.Data.SqlClient;
using Dapper;
using DomainModel.FeedEvents;
using DomainModel.FeedEvents.Interfaces;

namespace Infrastructure.Repositories
{
    public class FeedEventRepository : IFeedEventRepository
    {
        public async Task AddEventToQueue(FeedEvent newEvent)
        {
            string sql = @"INSERT INTO FeedEventQueue(
                            EventCreated, 
	                        EventType, 
	                        EventDataJson)
                        VALUES(
                            @EventCreated,
                            @EventType,
                            @EventDataJson)";

            using (var connection = new SqlConnection("Server=.;Database=WebGallery;Trusted_Connection=True;"))
            {
                var affectedRows = await connection.ExecuteAsync(sql,
                    new
                    {
                        EventCreated = newEvent.EventCreated,
                        EventType = newEvent.EventType,
                        EventDataJson = newEvent.EventDataJson,
                    }
                );
            }
        }
    }
}