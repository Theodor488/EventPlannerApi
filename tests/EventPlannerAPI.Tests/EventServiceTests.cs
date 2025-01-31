using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using EventPlannerApi.Controllers;
using EventPlannerApi.Models;
using System.Threading.Tasks;

namespace EventPlannerApi.Tests
{
    public class EventControllerTests
    {
        private readonly EventsController _eventController;
        private readonly EventContext _dbContext;

        public EventControllerTests()
        {
            // Set up in-memory database
            var options = new DbContextOptionsBuilder<EventContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new EventContext(options);
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            _eventController = new EventsController(_dbContext, null);

            // Mock HttpContext and User Identity
            var fakeUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "TestUser"),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            }, "mock"));

            _eventController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = fakeUser }
            };
        }

        [Fact]
        public async Task CreateEvent_ShouldAddEventToDatabase()
        {
            var createdResult = await PostEventToInMemoryDB();

            // Ensure the event was saved to the database
            var eventInDb = await _dbContext.Events.FirstOrDefaultAsync(e => e.Name == "Test Event");
            eventInDb.Should().NotBeNull();
            eventInDb.EventHostUserName.Should().Be("TestUser");
        }

        [Fact]
        public async Task GetEventByID()
        {
            var createdResult = await PostEventToInMemoryDB();

            var eventId = (Guid)createdResult.RouteValues["eventId"];

            var result = await _eventController.GetEvent(eventId);

            Console.WriteLine($"Event Count: {_dbContext.Events.Count()}");
            Console.WriteLine($"Retrieved Event: {_dbContext.Events.FirstOrDefault()?.Name}");


            var retrievedEvent = result.Value as EventDTO;
            retrievedEvent.Should().NotBeNull();
            retrievedEvent.Name.Should().Be("Test Event");
        }

        private async Task<CreatedAtActionResult> PostEventToInMemoryDB()
        {
            var newEvent = new CreateEventDTO 
            { 
                Name = "Test Event", 
                Description = "Here is a description",
                Location = "Seattle" 
            };

            var result = await _eventController.PostEvent(newEvent);

            // Extract the actual `CreatedAtActionResult`
            var createdResult = result.Result as CreatedAtActionResult;

            await _dbContext.SaveChangesAsync();

            var allEvents = await _dbContext.Events.ToListAsync();
            Console.WriteLine($"Total Events in DB After Saving: {allEvents.Count}");
            foreach (var e in allEvents)
            {
                Console.WriteLine($"Event in DB: {e.Name}, ID: {e.Id}");
            }

            var eventInDb = await _dbContext.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Name == "Test Event");
            eventInDb.Should().NotBeNull();  // This confirms the event is actually there

            Console.WriteLine($"Event Count: {_dbContext.Events.Count()}");
            Console.WriteLine($"Retrieved Event: {_dbContext.Events.FirstOrDefault()?.Name}");

            // Assert
            createdResult.Should().NotBeNull();
            createdResult.Should().BeOfType<CreatedAtActionResult>();
            return createdResult;
        }
    }
}