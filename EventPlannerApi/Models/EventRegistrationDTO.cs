namespace EventPlannerApi.Models
{
    public class EventRegistrationDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid EventId { get; set; }
        public DateTime RegisteredAt { get; set; }
    }
}
