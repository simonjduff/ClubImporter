namespace DdhpCore.ClubImporter.Runner.Models.Events
{
    using System;
    using Microsoft.WindowsAzure.Storage.Table;
    using Newtonsoft.Json;

    public sealed class Event : TableEntity
    {
        public Event(Guid entityId,
         int entityVersion,
         string eventType,
         string payload)
        {
            RowKey = entityVersion.ToString("0000000000");
            PartitionKey = entityId.ToString();
            EventType = eventType;
            Payload = payload;
        }

        [Obsolete("This is for deserialization only, do not use")]
        public Event()
        {

        }

        public string EventType{get;set;}
        public string Payload{get;set;}
        public T GetPayload<T>()
        {
            return (T)JsonConvert.DeserializeObject<T>(Payload);
        }
    }
}