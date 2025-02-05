﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventPlannerApi.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using EventPlannerApi.Data;

namespace EventPlannerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly EventContext _context;
        private readonly EventAuthContext _authContext;

        public EventsController(EventContext context, EventAuthContext authContext)
        {
            _context = context;
            _authContext = authContext;
        }

        // GET: api/Events
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<EventDTO>>> GetEvents()
        {
            return await _context.Events
                .Select(x => ItemToEventDTO(x))
                .ToListAsync();
        }

        // GET: api/Events/5
        [HttpGet("{eventId}")]
        public async Task<ActionResult<EventDTO>> GetEvent(Guid eventId)
        {
            var eventItem = await _context.Events.FindAsync(eventId);

            if (eventItem == null)
            {
                return NotFound();
            }

            return ItemToEventDTO(eventItem);
        }

        // PUT: api/Events/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{eventId}")]
        public async Task<IActionResult> PutEvent(Guid eventId, CreateEventDTO eventItem)
        {
            var foundEventItem = await _context.Events.FindAsync(eventId);
            if (foundEventItem == null)
            {
                return NotFound();
            }

            var user = HttpContext.User;
            Guid loggedIn_userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            Guid eventHostId = foundEventItem.EventHostUserId;

            // Deny access if logged in user is not event host or admin
            if (!user.IsInRole("Admin") && loggedIn_userId != eventHostId)
            {
                return Forbid("Access Denied. User is not an admin or the event host.");
            }

            if (eventId != foundEventItem.Id)
            {
                return BadRequest();
            }

            foundEventItem.Name = eventItem.Name;
            foundEventItem.Location = eventItem.Location;
            foundEventItem.Description = eventItem.Description;
            foundEventItem.Date = eventItem.Date;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!EventExists(eventId))
            {
                return NotFound();
            }

            var updatedEventDTO = ItemToEventDTO(foundEventItem);
            return Ok(updatedEventDTO);
        }

        // POST: api/Events
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<EventDTO>> PostEvent([FromBody] CreateEventDTO eventDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Guid newEventId = Guid.NewGuid();
            string userName = HttpContext.User.Identity.Name;
            Guid userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // Prevent Duplicate Event Names
            if (_context.Events.Any(e => e.Name == eventDTO.Name))
            {
                return BadRequest($"An event with name \"{eventDTO.Name}\" already exists!");
            }

            var eventItem = new Event
            {
                Id = newEventId,
                Name = eventDTO.Name,
                Description = eventDTO.Description,
                Date = eventDTO.Date,
                Location = eventDTO.Location,
                EventHostUserName = userName,
                EventHostUserId = userId
            };

            _context.Events.Add(eventItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEvent), // eventItem
                new { eventId = eventItem.Id },
                ItemToEventDTO(eventItem));
        }

        // DELETE: api/Events/5
        [HttpDelete("{eventId}")]
        public async Task<IActionResult> DeleteEvent(Guid eventId)
        {
            Event eventItem = await _context.Events.FindAsync(eventId);
            var user = HttpContext.User;
            Guid loggedIn_userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            Guid eventHostId = eventItem.EventHostUserId;

            // Deny access if logged in user is not event host or admin
            if (!user.IsInRole("Admin") && loggedIn_userId != eventHostId)
            {
                return Forbid("Access Denied. User is not an admin or the event host.");
            }
            
            if (eventItem == null)
            {
                return NotFound();
            }

            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Events/5/registerEvent
        [HttpPost("{eventId}/registerEvent")]
        public async Task<IActionResult> RegisterUser(Guid eventId, Guid userId)
        {
            bool eventExists = EventExists(eventId);
            if (!eventExists) return NotFound("Event not found");

            var registration = new EventRegistration { EventId = eventId, UserId = userId };
            _context.EventRegistrations.Add(registration);
            await _context.SaveChangesAsync();

            return Ok("User to event registered successfully");
        }

        // GET: api/Events/5/attendees
        [HttpGet("{eventId}/attendees")]
        public async Task<ActionResult<IEnumerable<EventRegistrationDTO>>> GetEventAttendees(Guid eventId)
        {
            // Return EventRegistrations
            var eventRegistrations = await _context.EventRegistrations
                .Where(e => e.EventId == eventId)
                .Select(e => ItemToEventRegistrationsDTO(e))
                .ToListAsync();

            // Return Event Info
            var events = await _context.Events
                .Where(e => e.Id == eventId)
                .ToListAsync();

            // Get list of distinct UserIDs
            List<string> userIDs = eventRegistrations.Select(e => e.UserId.ToString()).ToList();

            // Get User Info
            var users = await _authContext.Users
                .Where(user => userIDs.Contains(user.Id))
                .Select(user => new
                {
                    user.Name,
                    user.UserName,
                    user.Id
                }).ToListAsync();

            // Get attendees
            var attendees = eventRegistrations.Select(reg => new
            {
                reg.EventId,
                reg.UserId,
                UserName = users.FirstOrDefault(u => u.Id == reg.UserId.ToString())?.UserName ?? "Unknown",
                Name = users.FirstOrDefault(u => u.Id == reg.UserId.ToString())?.Name ?? "Unknown",
                EventName = events.FirstOrDefault(e => e.Id == reg.EventId)?.Name ?? "Unknown"
            }).ToList();

            return Ok(attendees);
        }

        private bool EventExists(Guid eventId)
        {
            return _context.Events.Any(e => e.Id == eventId);
        }

        private static EventDTO ItemToEventDTO(Event eventItem) => new EventDTO
        {
            Name = eventItem.Name,
            Description = eventItem.Description,
            Date = eventItem.Date,
            Location = eventItem.Location
        };

        private static EventRegistrationDTO ItemToEventRegistrationsDTO(EventRegistration eventRegistrationItem) => new EventRegistrationDTO
        {
            Id = eventRegistrationItem.Id,
            UserId = eventRegistrationItem.UserId,
            EventId = eventRegistrationItem.EventId,
            RegisteredAt = eventRegistrationItem.RegisteredAt
        };
    }
}