using System.ComponentModel.DataAnnotations;

namespace Common
{
    public class Enums
    {
        public enum StatusEnums
        {
            [Display(Name = "Active")]
            Active = 1,
            [Display(Name = "Deactive")]
            Deactive = 2,
            [Display(Name = "Pending")]
            Pending = 3,
            [Display(Name = "Completed")]
            Completed = 4,
            [Display(Name = "Removed")]
            Removed = 5,
            [Display(Name = "Passed")]
            Passed = 6,
            [Display(Name = "Failed")]
            Failed = 7,
            [Display(Name = "Deleted")]
            Deleted = 8,
        }
    }
}