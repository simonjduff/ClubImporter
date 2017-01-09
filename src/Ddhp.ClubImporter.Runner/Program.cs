namespace DdhpCore.ClubImporter.Runner
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DdhpCore.ClubImporter.Runner.Extensions;
    using DdhpCore.ClubImporter.Runner.Models.Events;
    using DdhpCore.ClubImporter.Runner.Models.Storage;
    using Microsoft.Extensions.Configuration;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using Newtonsoft.Json;
    public class Program
    {
        public static string StorageConnectionString
        {
            get
            {
                const string envVar = "StorageConnectionString";
                return Configuration[envVar];
            }
        }

        public static bool IsDevelopment
        {
            get
            {
                bool isDevelopment;
                bool.TryParse(Environment.GetEnvironmentVariable("IsDevelopment"), out isDevelopment);

                return isDevelopment;
            }
        }

        public static IConfigurationRoot Configuration { get; set; }
        
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder();

            if (IsDevelopment)
            {
                Console.WriteLine("DEVELOPMENT");
                configuration.AddUserSecrets();
            }
            else
            {
                Console.WriteLine("PRODUCTION");
                configuration.AddEnvironmentVariables();
            }

            Configuration = configuration.Build();

            var program = new Program();
            program.Run();
        }

        private const int maximumBatchSize = 100;

        private static CloudTableClient _tableClient = null;
        private static CloudTableClient TableClient
        {
            get
            {
                if (_tableClient == null) 
                {
                    var storageAccount = CloudStorageAccount.Parse(StorageConnectionString);
                    _tableClient = storageAccount.CreateCloudTableClient();
                }

                return _tableClient;
            }
        }

        public void Run()
        {
            var clubsTable = TableClient.GetTableReference("clubs");
            var eventsTable = TableClient.GetTableReference("clubEvents");
            clubsTable.CreateIfNotExistsAsync().Wait();
            eventsTable.CreateIfNotExistsAsync().Wait();

            var query = new TableQuery<Club>();

            var clubs = query.BatchQuery(clubsTable).Result;

            ImportClubs(clubs, eventsTable);

            ImportContracts(clubs, eventsTable);
        }

        private void ImportContracts(IEnumerable<Club> clubs, CloudTable eventsTable)
        {
            var contractsTable = TableClient.GetTableReference("contracts");

            var tasks = new List<Task>();
            foreach (var club in clubs)
            {
                tasks.Add(Task.Run(() => {
                    var query = new TableQuery<Contract>()
                        .Where(TableQuery.GenerateFilterCondition("PartitionKey",
                            QueryComparisons.Equal, club.Id.ToString()));

                    var contracts = query.BatchQuery(contractsTable).Result;
                    int version = 1;

                    foreach (var contractPartition in contracts.Partition(maximumBatchSize))
                    {
                        var insert = new TableBatchOperation();

                        foreach (var contract in contractPartition)
                        {
                            var contractEvent = new ContractImportedEvent
                            {
                                PlayerId = contract.PlayerId,
                                FromRound = contract.FromRound,
                                ToRound = contract.ToRound,
                                DraftPick = contract.DraftPick
                            };

                            var insertEvent = new Event(club.Id, version++, "ContractImported", JsonConvert.SerializeObject(contractEvent));
                            insert.Add(TableOperation.Insert(insertEvent));
                        }

                        try
                        {
                            eventsTable.ExecuteBatchAsync(insert).Wait();
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                            throw;
                        }
                    }                    
                }));
            }

            Task.WaitAll(tasks.ToArray());
        }

        private void ImportClubs(IEnumerable<Club> clubs, CloudTable eventsTable)
        {
            var tasks = new List<Task>();
            foreach (var club in clubs)
            {
                var storage = new ClubCreatedEvent(club.ClubName, club.CoachName, club.Email);
                var createdEvent = new Event(club.Id, 0, "clubCreated", JsonConvert.SerializeObject(storage));
                var operation = TableOperation.Insert(createdEvent);
                tasks.Add(eventsTable.ExecuteAsync(operation));
            }

            Task.WaitAll(tasks.ToArray());
        }
    }
}
