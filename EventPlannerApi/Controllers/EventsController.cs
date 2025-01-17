using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventPlannerApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace EventPlannerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly EventContext _context;

        public EventsController(EventContext context)
        {
            _context = context;
        }

        // GET: api/Events
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<EventDTO>>> GetEvents()
        {
            return await _context.Events
                .Select(x => ItemToDTO(x))
                .ToListAsync();
        }

        // GET: api/Events/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EventDTO>> GetEvent(Guid id)
        {
            var eventItem = await _context.Events.FindAsync(id);

            if (eventItem == null)
            {
                return NotFound();
            }

            return ItemToDTO(eventItem);
        }

        // PUT: api/Events/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutEvent(Guid id, EventDTO eventDTO)
        {
            if (id != eventDTO.Id)
            {
                return BadRequest();
            }

            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null)
            {
                return NotFound();
            }

            eventItem.Name = eventDTO.Name;
            eventItem.Location = eventDTO.Location;
            eventItem.Description = eventDTO.Description;
            eventItem.Date = eventDTO.Date;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!EventExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/Events
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<EventDTO>> PostEvent(EventDTO eventDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var eventItem = new Event
            {
                Name = eventDTO.Name,
                Location = eventDTO.Location,
                Description = eventDTO.Description,
                Date = eventDTO.Date
            };

            _context.Events.Add(eventItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEvent), // eventItem
                new { id = eventItem.Id },
                ItemToDTO(eventItem));
        }

        // DELETE: api/Events/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null)
            {
                return NotFound();
            }

            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EventExists(Guid id)
        {
            return _context.Events.Any(e => e.Id == id);
        }

        private static EventDTO ItemToDTO(Event eventItem) =>
       new EventDTO
       {
           Id = eventItem.Id,
           Name = eventItem.Name,
           Location = eventItem.Location,
           Description = eventItem.Description,
           Date = eventItem.Date
       };
    }
}
