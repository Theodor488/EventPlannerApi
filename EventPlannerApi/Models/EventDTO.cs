using System.Text.Json.Serialization;

namespace EventPlannerApi.Models
{
    public class EventDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }

        [JsonIgnore]
        public Guid Id { get; set; } // Hidden from Swagger
        [JsonIgnore]
        public Guid EventHostUserId { get; set; } // Hidden from Swagger
        [JsonIgnore]
        public string EventHostUserName { get; set; } // Hidden from Swagger
    }
}
