using System.ComponentModel.DataAnnotations;

namespace GymBooking.Models
{
    public class GymClass
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required")]
        [StringLength(40)]
        public string Name { get; set; }

        [Display(Name = "Start Time")]
        [Required(ErrorMessage = "Start Time is required")]
        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; }

        [Display(Name = "Duration")]
        [Required(ErrorMessage = "Duration is required")]
        [DataType(DataType.Duration)]
        public TimeSpan Duration { get; set; }

        [Display(Name = "End Time")]
        [DataType(DataType.DateTime)]
        public DateTime EndTime { get { return StartTime + Duration; } }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is required")]
        [StringLength(150)]
        public string Description { get; set; }

        public ICollection<ApplicationUserGymClass> AttendingMembers { get; set; } = new List<ApplicationUserGymClass>();
    }
}
