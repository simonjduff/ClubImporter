namespace DdhpCore.ClubImporter.Runner.Models.Events
{
    public class ClubCreatedEvent
    {
        public ClubCreatedEvent(string clubName,
            string coachName,
            string email)
        {
            ClubName = clubName;
            CoachName = coachName;
            Email = email;
        }
        public ClubCreatedEvent()
        {
            
        }

        public string ClubName{get;set;}
        public string CoachName{get;set;}
        public string Email{get;set;}
    }
}