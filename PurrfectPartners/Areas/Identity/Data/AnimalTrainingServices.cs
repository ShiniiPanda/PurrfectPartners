using System.ComponentModel.DataAnnotations.Schema;

namespace PurrfectPartners.Areas.Identity.Data
{
    [Table("AnimalServices")]
    public class AnimalTrainingServices
    {
        public int AnimalId { get; set; }

        public int TrainingServiceId { get; set; }

        public Animal Animal { get; set; } = null!;

        public TrainingService TrainingService { get; set; } = null!;
    }
}
