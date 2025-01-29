﻿using System.ComponentModel.DataAnnotations;

namespace EventPlannerApi.Models
{
    public class CreateEventDTO
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Location { get; set; }
    }
}
