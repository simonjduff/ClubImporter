using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace DdhpCore.ClubImporter.Runner.Models.Storage
{
    public class Contract : TableEntity
    {
        private Guid _clubId;
        private int _toRound;
        private Guid _playerId;
        public int FromRound { get; set; }

        public int ToRound
        {
            get { return _toRound; }
            set
            {
                _toRound = value;
                SetRowKey();
            }
        }

        public Guid PlayerId
        {
            get { return _playerId; }
            set
            {
                _playerId = value;
                SetRowKey();
            }
        }

        public int DraftPick { get; set; }

        private void SetRowKey()
        {
            RowKey = $"{FromRound}-{ToRound}-{PlayerId}";
        }

        public Guid ClubId
        {
            get { return _clubId; }
            set
            {
                PartitionKey = value.ToString();
                _clubId = value;
            }
        }
    }
}