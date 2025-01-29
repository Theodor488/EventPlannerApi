using Xunit;
using Moq;
using FluentAssertions;
using EventPlannerApi.Controllers;
using EventPlannerApi.Models;
using System.Threading.Tasks;

namespace EventPlannerApi.Tests
{
    public class EventServiceTests
    {
        private readonly Mock<IEventRepository> _mockRepo;
        private readonly EventService _eventService;

        public EventServiceTests()
        {
            _mockRepo = new Mock<IEventRepository>();
            _eventService = new EventService(_mockRepo.Object);
        }

        [Fact]
        public async Task CreateEvent_ShouldAddEventToDatabase()
        {
            // Arrange
            var newEvent = new Event { Id = 1, Name = "Tech Meetup", Location = "Seattle" };

            // Act
            await _eventService.CreateEventAsync(newEvent);

            // Assert
            _mockRepo.Verify(r => r.AddAsync(It.Is<Event>(e => e.Name == "Tech Meetup")), Times.Once);
        }
    }
}
