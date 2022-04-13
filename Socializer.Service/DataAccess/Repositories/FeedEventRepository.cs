using Dapper;
using DomainModel.FeedEvents;
using DomainModel.FeedEvents.Interfaces;

namespace Infrastructure.Repositories
{
    public class FeedEventRepository : IFeedEventRepository
    {
        private IDatabaseConnection _db;

        public FeedEventRepository(IDatabaseConnection socializerDbConnection)
        {
            _db = socializerDbConnection;
        }

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

            using (var connection = _db.GetConnection())
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