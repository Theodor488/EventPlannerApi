using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace EventPlannerApi.Models
{
    public class User : IdentityUser
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserType { get; set; }
    }
}
