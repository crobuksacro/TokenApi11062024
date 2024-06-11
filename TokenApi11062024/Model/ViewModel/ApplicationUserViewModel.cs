using System.ComponentModel.DataAnnotations;

namespace TokenApi11062024.Model.ViewModel
{
    public class ApplicationUserViewModel
    {
        public string Id { get; set; }
        [Display(Name = "Ime")]
        public string FirstName { get; set; }
        [Display(Name = "Prezime")]
        public string LastName { get; set; }
        public DateTime? RegistrationDate { get; set; }
    }
}
