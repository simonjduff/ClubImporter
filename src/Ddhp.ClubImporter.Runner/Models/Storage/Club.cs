using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace DdhpCore.ClubImporter.Runner.Models.Storage
{
    public class Club : TableEntity
    {
        public Club()
        {
            PartitionKey = "ALL_CLUBS";
        }
        public Guid Id { get; set; }
        public int LegacyId { get; set; }
        public string CoachName { get; set; }

        private string _clubName;
        public string ClubName {
            get { return _clubName; }
            set
            {
                _clubName = value;
                RowKey = value;
            }
        }
        public string Email { get; set; }
    }
}