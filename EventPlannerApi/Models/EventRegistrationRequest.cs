namespace EventPlannerApi.Models
{
    public class EventRegistrationRequest
    {
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
    }
}
