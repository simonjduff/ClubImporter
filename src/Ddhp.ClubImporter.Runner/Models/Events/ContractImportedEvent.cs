using System;

namespace DdhpCore.ClubImporter.Runner.Models.Events
{
    public class ContractImportedEvent
    {
        public Guid PlayerId{get;set;}
        public int FromRound{get;set;}
        public int ToRound{get;set;}
        public int DraftPick{get;set;}
    }
}