using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurrfectPartners.Areas.Identity.Data
{
    [Table("Animals")]
    public class Animal
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

        public List<TrainingService> TrainingServices { get; set; } = new List<TrainingService>();

        public List<AnimalTrainingServices> JoinedServices { get; set; } = new List<AnimalTrainingServices>();


    }
}
