using EventPlannerApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EventPlannerApi.Data
{
    public class EventAuthContext : IdentityDbContext<User>
    {
        public EventAuthContext(DbContextOptions<EventAuthContext> options) : base(options)
        {
            // The base constructor handles initializing the DbContext with the provided options.
        }
    }
}
