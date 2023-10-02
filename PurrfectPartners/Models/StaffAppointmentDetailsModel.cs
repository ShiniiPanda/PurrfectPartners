using PurrfectPartners.Areas.Identity.Data;

namespace PurrfectPartners.Models
{
    public class StaffAppointmentDetailsModel
    {
        public StaffAppointmentDetailsModel(Appointment appointment)
        {
            Id = appointment.Id.ToString();
            ReservationDate = appointment.ReservationDate;
            BookingDate = appointment.BookingDate;
            Status = appointment.Status;
            Breed = appointment.Breed;
            AnimalImage = appointment.AnimalImage;
            UserId = appointment.UserId;
        }

        public string Id { get; set; } = null!;

        public DateTime ReservationDate { get; set; }

        public DateTime BookingDate { get; set; }

        public AppointmentStatus Status { get; set; }

        public string? Breed { get; set; }

        public string? AnimalName { get; set; }

        public string? AnimalImage { get; set; }

        public string? ServiceName { get; set; }

        public double ServicePrice { get; set; }

        public string UserId { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public string? UserEmail { get; set; } = null!;

        public string? UserPhone { get; set; } = null!;

        public string? UserAddress { get; set; } = null!;
    }
}
