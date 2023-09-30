using Microsoft.AspNetCore.Mvc.Rendering;
#nullable disable
namespace PurrfectPartners.Views.Appointments
{
    public class ManageAppointmentNav
    {

        public static string Index => "Index";


        public static string Animals => "Animals";


        public static string Feedback => "Feedback";


        public static string Contact => "Contact";


        public static string Privacy => "Privacy";


        public static string Services => "Services";

        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);


        public static string AnimalsNavCass(ViewContext viewContext) => PageNavClass(viewContext, Animals);


        public static string FeedbackNavClass(ViewContext viewContext) => PageNavClass(viewContext, Feedback);


        public static string ContactNavClass(ViewContext viewContext) => PageNavClass(viewContext, Contact);


        public static string PrivactNavClass(ViewContext viewContext) => PageNavClass(viewContext, Privacy);


        public static string ServicesNavClass(ViewContext viewContext) => PageNavClass(viewContext, Services);

        public static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                ?? Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}
