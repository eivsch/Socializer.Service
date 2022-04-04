using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainModel.FeedEvents.Interfaces;

namespace DomainModel.FeedEvents
{
    public class FeedEvent
    {
        public string Id { get; set; }
        public DateTime EventCreated { get; set; }
        public string EventType { get; set; }
        public string EventDataJson { get; set; }
    }
}
