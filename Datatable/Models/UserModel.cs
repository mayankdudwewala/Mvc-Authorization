using Datatable.DBModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Datatable.Models
{
    public class UserModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }
        [Required(AllowEmptyStrings = false,ErrorMessage = "First Name is Required")]
        public string FirstName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "LastName is Required")]
        public string LastName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email Is Required")]
        public string Email { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password Is Required")]
        [DataType(DataType.Password)]

        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage = "Password And Confirm Password Should Be Same")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Confirm Password Is Required")]
        public string ConfirmPassword { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Mobile Is Required")]
        public string Mobile { get; set; }
        
        public bool IsAdmin { get; set; }

       
    }
}