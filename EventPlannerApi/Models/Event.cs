namespace EventPlannerApi.Models
{
    public class Event
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public Guid EventHostUserId { get; set; }
        public string EventHostUserName { get; set; }
    }
}
