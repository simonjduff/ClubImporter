using System;

namespace DdhpCore.ClubImporter.Runner.Models.Events
{
    public class ClubPlayerDraftedEvent
    {
        public ClubPlayerDraftedEvent(Guid clubId, 
            int clubVersion)
        {
        }

        [Obsolete("This constructor is for serialization only, do not use")]
        public ClubPlayerDraftedEvent()
        {            
        }

        public Guid PlayerId{get;set;}
    }
}