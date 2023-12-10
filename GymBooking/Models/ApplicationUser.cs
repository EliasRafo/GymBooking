using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GymBooking.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name is required")]
        [StringLength(30)]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(40)]
        public string LastName { get; set; }

        [Display(Name = "Full Name")]
        public string FullName => FirstName + " " + LastName;

        [Display(Name = "Time of Registration")]
        [DataType(DataType.DateTime)]
        public DateTime TimeOfRegistration { get; set; }

        public ICollection<ApplicationUserGymClass> AttendedClasses { get; set; }
    }
}
