using PurrfectPartners.Areas.Identity.Data;

namespace PurrfectPartners.Models
{
    public class ManageServicesModel
    {

        public List<TrainingService> Services { get; set; } = new();

        public SingleServiceModel NewService { get; set; } = new();

    }
}
