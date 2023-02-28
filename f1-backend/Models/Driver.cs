using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Formula1TeamApp.Models
{
    public class Driver
    {
        public int DriverId { get; set; }

        [Required]
        [Display(Name = "Förarnamn:")]
        public string Name { get; set; } = String.Empty;

        [Required]
        [Display(Name = "Lön (miljoner, i heltal):")]
        public int Salary { get; set; } = 0;

        [Display(Name = "Filnamn - bild")]
        public string? ImageName { get; set; }

        [NotMapped]
        public string ImageWebp { get; set; } = String.Empty;

        [NotMapped]
        [Display(Name = "Bild")]
        public IFormFile? ImageFile { get; set; }

        [NotMapped]
        public string? TeamName { get; internal set; }

        // Relations
        [Display(Name = "Team")]
        public int? TeamId { get; set; }
        public Team? Team { get; set; }
    }
}
