using System.ComponentModel.DataAnnotations;

namespace PurrfectPartners.Models
{
    public class SingleServiceModel
    {

        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(512, ErrorMessage = "Must be between 10-512 characters long!", MinimumLength = 10)]
        public string Description { get; set; } = null!;

        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue)]
        public double DefaultPrice { get; set; } = 0;

        public List<string> AnimalIds { get; set; } = new();

    }
}
