using Datatable.DBModel;
using Datatable.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;

namespace Datatable.Controllers
{
    public class AccountController : Controller
    {
        private UserDBEntities _dbContext = new UserDBEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Register()
        {
            UserModel userModel = new UserModel();
            return View(userModel);
        }

        [HttpPost]
        public ActionResult Register(UserModel userModel)
        {
            if (ModelState.IsValid)
            {
                var message = "";
                var Emails = (from x in _dbContext.Users select x.Email).ToList();

                if (Emails.Contains(userModel.Email))
                {
                    message = "Email already exists";
                    ViewBag.Message = message;
                    return View("Register");
                }
                else
                {
                    User user = new User();
                    user.IsAdmin = false;
                    user.FirstName = userModel.FirstName;
                    user.LastName = userModel.LastName;
                    user.Email = userModel.Email;
                    user.Mobile = userModel.Mobile;
                    user.Password = userModel.Password;
                    _dbContext.Users.Add(user);
                    _dbContext.SaveChanges();
                    message = userModel.FirstName;
                    ViewBag.Message = message;
                    return View("Success");
                }
            }
            return View("Login");
        }

        public ActionResult Login()
        {
            LoginModel LM = new LoginModel();
            return View(LM);
        }

        [HttpPost]
        public ActionResult Login(LoginModel LM)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                var condition = _dbContext.Users.Where(m => m.Email == LM.Email).FirstOrDefault();
                if (_dbContext.Users.Where(m => m.Email == LM.Email && m.Password == LM.Password).FirstOrDefault() ==
                    null)
                {
                    message = "Invalid Email/Password";
                    ViewBag.Message = message;
                    return View("Login");
                }
                else
                {
                    if (condition.IsAdmin)
                    {
                        Session["Email"] = LM.Email;
                        return RedirectToAction("Admin");
                    }
                    else
                    {
                        Session["Email"] = LM.Email;
                        return View("Normal", LM);
                    }
                }
            }
            return View();
        }

        public ActionResult Admin()
        {
            var list = _dbContext.Employees.Select(m => m).ToList();
            return View(list);
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Login", "Account");
        }

        [NonAction]
        public void SendVerificationLinkEmail(string Email, string resetCode)
        {
            var link = "<a href='" + Url.Action("ResetPassword", "Account", new { email = Email, id = resetCode }, "http") + "'>Reset Password</a>";

            var fromEmail = new MailAddress("mayankdudwewala@gmail.com", "Mayank Dudwewala");
            var toEmail = new MailAddress(Email);
            var fromEmailPassword = "*********";

            string subject = "Reset Password";
            string body = "Hi,<br/><br/>Please click on the link to reset your password" +
                          "<br/><br/><a href=" + link + "Reset Password link</a>";

            var smtp = new System.Net.Mail.SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string Email)
        {
            string message = "";
            var account = _dbContext.Users.Where(m => m.Email == Email).FirstOrDefault();
            if (account != null)
            {
                string resetCode = new Guid().ToString();
                SendVerificationLinkEmail(Email, resetCode);
                account.ResetPasswordCode = resetCode;
                _dbContext.Configuration.ValidateOnSaveEnabled = false;
                _dbContext.SaveChanges();
                message = "Reset Password Link has Been Sent On The Email";
            }
            else
            {
                message = "Account Not Found";
            }

            ViewBag.Message = message;
            return View();
        }

        public ActionResult ResetPassword(string email, string id)
        {
            var user = _dbContext.Users.Where(m => m.ResetPasswordCode == id).FirstOrDefault();
            if (user != null)
            {
                ResetPasswordModel resetPasswordModel = new ResetPasswordModel();
                resetPasswordModel.resetCode = id;
                return View(resetPasswordModel);
            }
            else
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            var message = "";
            var users = _dbContext.Users.Where(m => m.ResetPasswordCode == resetPasswordModel.resetCode)
                .FirstOrDefault();
            if (users != null)
            {
                users.Password = resetPasswordModel.NewPassword;
                users.ResetPasswordCode = "";
                _dbContext.Configuration.ValidateOnSaveEnabled = false;
                _dbContext.SaveChanges();
                message = "Password Changed Successfully";
            }
            else
            {
                message = "Something Went Wrong";
            }

            ViewBag.Message = message;
            return View();
        }

        public ActionResult AddEmployee()
        {
            EmployeeModel emp = new EmployeeModel();
            return View(emp);
        }

        [HttpPost]
        public ActionResult AddEmployee(EmployeeModel emp)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                Employee newEmp = new Employee();
                newEmp.Name = emp.Name;
                newEmp.Position = emp.Position;
                newEmp.Age = emp.Age;
                newEmp.Salary = emp.Salary;
                _dbContext.Employees.Add(newEmp);
                _dbContext.SaveChanges();
                return RedirectToAction("Admin");
            }
            else
            {
                message = "invalid entry";
                ViewBag.Message = message;
                return View("Admin");
            }
        }

        public ActionResult DeleteEmployee(int id)
        {
            Employee emp = _dbContext.Employees.Find(id);
            if (emp != null)
            {
                _dbContext.Employees.Remove(emp);
                _dbContext.SaveChanges();
                return RedirectToAction("Admin");
            }
            return View("Admin");
        }

        public ActionResult EditEmployee(int id)
        {
            Employee emp = _dbContext.Employees.Find(id);
            if (emp == null)
            {
                return View("Login");
            }

            return View(emp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditEmployee(Employee emp)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Entry(emp).State = EntityState.Modified;
                _dbContext.SaveChanges();
                return RedirectToAction("Admin");
            }
            return HttpNotFound();
        }

        public ActionResult searchFilter(string search)
        {
            var record = _dbContext.Employees.Where(x => x.Name.Contains(search)).ToList();
            return View("Admin", record);
        }
    }
}