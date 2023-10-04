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

        public string Email { get; set; } = null!;

        public string? Phone { get; set; }

        public string Message { get; set; } = null!;

        public DateTime Date { get; set; }

    }
}
