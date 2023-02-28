using System.ComponentModel.DataAnnotations;

namespace Formula1TeamApp.Models
{
    public class Team
    {
        public int TeamId { get; set; }

        [Required]
        [Display(Name = "Team-namn")]
        public string? Name { get; set; } = String.Empty;

        public List<Driver>? Drivers { get; set; }
    }
}
