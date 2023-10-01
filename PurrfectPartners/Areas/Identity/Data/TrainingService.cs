using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurrfectPartners.Areas.Identity.Data
{
    [Table("Services")]
    public class TrainingService
    {
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public double StartingPrice { get; set; }

        public List<Animal> Animals { get; set; } = new List<Animal>();

        public ICollection<AnimalTrainingServices> JoinedAnimals { get; set; } = new List<AnimalTrainingServices>();

    }
}
