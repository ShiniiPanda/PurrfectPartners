using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurrfectPartners.Areas.Identity.Data
{
    [Table("Appointments")]
    public class Appointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        // The data the appointment takes place
        [DataType(DataType.DateTime)]
        public DateTime ReservationDate { get; set; }

        // The date it was booked
        public DateTime BookingDate { get; set; }

        public string? Breed { get; set; }

        public string? AnimalImage { get; set; }

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

        public TrainingService? TrainingService { get; set; }

        public int? TrainingServiceId { get; set; }

        public User User { get; set; } = null!;

        public string UserId { get; set; } = null!;

        public Animal Animal { get; set; } = null!;

        public int AnimalId { get; set; }
    }
}
