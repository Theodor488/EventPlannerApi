using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace EventPlannerApi.Models
{
    public class User : IdentityUser
    {
        [MaxLength(30)]
        public string? Name { get; set; }
    }
}
