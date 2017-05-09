using System.Collections.Generic;
using System.Reflection;
using DdhpCore.ClubImporter.Runner.Extensions;
using Microsoft.WindowsAzure.Storage;

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

        [ColumnName("eventType")]
        public string EventType{get;set;}
        [ColumnName("payload")] 
        public string Payload{get;set;}
        public T GetPayload<T>()
        {
            return (T)JsonConvert.DeserializeObject<T>(Payload);
        }

        public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            base.ReadEntity(properties, operationContext);

            foreach (var property in ColumnNameProperties)
            {
                var value = properties[property.Item2.ColumnName].StringValue;
                property.Item1.SetValue(this, value);
            }
        }

        public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            var dictionary = base.WriteEntity(operationContext);

            foreach (var propertyInfo in ColumnNameProperties)
            {
                dictionary.Add(propertyInfo.Item2.ColumnName, new EntityProperty((string)propertyInfo.Item1.GetValue(this)));
                dictionary.Remove(propertyInfo.Item1.Name);
            }

            return dictionary;
        }

        private IEnumerable<Tuple<PropertyInfo, ColumnNameAttribute>> ColumnNameProperties
        {
            get
            {
                foreach (var propertyInfo in GetType().GetProperties())
                {
                    var columnNameAttribute = propertyInfo.GetCustomAttribute<ColumnNameAttribute>();
                    if (columnNameAttribute != null)
                    {
                        yield return new Tuple<PropertyInfo, ColumnNameAttribute>(propertyInfo, columnNameAttribute);
                    }
                }
            }
        }
    }
}