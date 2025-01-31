using EventPlannerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EventPlannerApi.Data;

public class EventContext : DbContext
{
    public EventContext(DbContextOptions<EventContext> options) : base(options)
    {
    }

    public DbSet<Event> Events { get; set; }
    public DbSet<EventRegistration> EventRegistrations { get; set; }
}
