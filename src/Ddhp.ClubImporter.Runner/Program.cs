namespace DdhpCore.ClubImporter.Runner
{
    using System;
    using DdhpCore.ClubImporter.Runner.Models;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using Newtonsoft.Json;
    public class Program
    {
        public static void Main(string[] args)
        {
            var thing = new ClubCreatedEvent("Cheats",
                "Simon Duff", "simon.j.duff@gmail.com");
            var storageModel = new Event(Guid.NewGuid(), 
                0, 
                "clubCreated",
                JsonConvert.SerializeObject(thing));
            Console.WriteLine(storageModel.PartitionKey);

            var storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("clubEvents");
            table.CreateIfNotExistsAsync().Wait();

            var operation = TableOperation.Insert(storageModel);
            table.ExecuteAsync(operation).Wait();
        }
    }
}
