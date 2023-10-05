using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurrfectPartners.Areas.Identity.Data
{
    [Table("Feedback")]
    public class Feedback
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        public string? Phone { get; set; }

        [Required]
        [StringLength(1024, ErrorMessage = "Please ensure your message is under 1024 characters!", MinimumLength = 0)]
        public string Message { get; set; } = null!;

        public DateTime Date { get; set; } = DateTime.UtcNow.ToLocalTime();

    }
}
