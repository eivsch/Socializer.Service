namespace DomainModel.FeedEvents.Interfaces
{
    public interface IFeedEventRepository
    {
        Task AddEventToQueue(FeedEvent newEvent);
    }
}
