using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PurrfectPartners.Areas.Identity.Data;

namespace PurrfectPartners.Data;

public class PurrfectPartnersContext : IdentityDbContext<User>
{
    public PurrfectPartnersContext(DbContextOptions<PurrfectPartnersContext> options)
        : base(options)
    {
    }

    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<TrainingService> TrainingServices { get; set; }

    public DbSet<Animal> Animals { get; set; }

    public DbSet<AnimalTrainingServices> AnimalServices { get; set; }

    public DbSet<Feedback> Feedback { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);

        builder.Entity<Appointment>()
            .Property(a => a.Status)
            .HasConversion<int>()
            .HasDefaultValue(AppointmentStatus.Pending);

        builder.Entity<Appointment>()
            .HasOne(a => a.Animal)
            .WithMany(a => a.Appointments)
            .IsRequired();

        builder.Entity<Animal>()
            .HasMany(a => a.TrainingServices)
            .WithMany(s => s.Animals)
            .UsingEntity<AnimalTrainingServices>
            (
                l => l.HasOne<TrainingService>(t => t.TrainingService).WithMany(t => t.JoinedAnimals),
                r => r.HasOne<Animal>(a => a.Animal).WithMany(a => a.JoinedServices)
            );
    }
}
