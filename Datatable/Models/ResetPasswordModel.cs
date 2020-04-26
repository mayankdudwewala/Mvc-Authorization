using System.ComponentModel.DataAnnotations;

namespace Datatable.Models
{
    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "you must provide a new password", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Password do not match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required]
        public string resetCode { get; set; }
    }
}