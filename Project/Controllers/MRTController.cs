using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Project.AssignValues;
using Project.MailSettings;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using Project.Models;
using Microsoft.AspNetCore.Identity;
using Project.Areas.Identity.Data;


namespace Project.Controllers
{
    public class MRTController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public MRTController (IConfiguration config, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            this.configuration = config;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        IList<MRTTicket> GetDbList()
        {

                IList<MRTTicket> dbList = new List<MRTTicket>();
                SqlConnection conn = new SqlConnection(configuration.
                    GetConnectionString("MRTConnStr"));

                string sql = @"SELECT * FROM MRTTicket";
                SqlCommand cmd = new SqlCommand(sql, conn);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        dbList.Add(new MRTTicket()
                        {
                            ViewId = reader.GetString(0),
                            ViewDateTime = reader.GetDateTime(1),
                            Name = reader.GetString(2),
                            Phone = reader.GetString(3),
                            IC = reader.GetString(4),
                            Email = reader.GetString(5),
                            IndexStatus = reader.GetInt32(6),
                            IndexFStation = reader.GetInt32(7),
                            IndexTStation = reader.GetInt32(8),
                            IndexPackage = reader.GetInt32(9),
                            Subtotal = reader.GetDouble(10),
                            Amount = reader.GetDouble(11),
                            Discount = reader.GetString(12),
                        });
                    }
                }

                catch
            {
                RedirectToAction("Error");
            }

            finally
            {
                conn.Close();
            }

            return dbList;
        }

        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = userManager.Users;
            return View(users);
        }

        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id ={id} cannot be found";
                return View("Not Found");
            }
            else
            {
                var result = await userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View("ListUsers");
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if(user == null)
            {
                ViewBag.ErrorMessage = $"User with Id ={id} cannot be found";
                return View("Not Found");
            }

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Phone = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser (EditUserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id ={model.Id} cannot be found";
                return View("Not Found");
            }
            else
            {
                user.PhoneNumber = model.Phone;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                

                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }
                foreach( var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
        }

        public IActionResult Index()
        {
            IList<MRTTicket> dbList = GetDbList();
            return View(dbList);
        }
            
        public IActionResult Error()
            {
                return View();
            }
        

        public IActionResult MRTTable()
        {
            MRTTable mrttable = new MRTTable();
            ViewBag.FromStations = mrttable.FromStation;
            ViewBag.ToStations = mrttable.ToStation;
            ViewBag.Fares = mrttable.fares;

            return View();
        }
        [HttpGet]
        public IActionResult PurchaseTicket()
        {
            MRTTicket ticket = new MRTTicket();
            ticket.IndexFStation = ticket.IndexTStation =
                ticket.IndexStatus = ticket.IndexPackage = -1;
            return View(ticket);
        }

        [HttpPost]
        public IActionResult PurchaseTicket(MRTTicket ticket)
        {
            if (ModelState.IsValid)
            {
                SqlConnection conn = new SqlConnection(configuration.
                    GetConnectionString("MRTConnStr"));
                SqlCommand cmd = new SqlCommand("spPurchaseTicket", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ticketid", ticket.TicketId);
                cmd.Parameters.AddWithValue("@ticketdatetime", ticket.TicketDateTime);
                cmd.Parameters.AddWithValue("@name", ticket.Name);
                cmd.Parameters.AddWithValue("@phone", ticket.Phone);
                cmd.Parameters.AddWithValue("@ic", ticket.IC);
                if (ticket.Email != null)
                    cmd.Parameters.AddWithValue("@email", ticket.Email);
                else
                    cmd.Parameters.AddWithValue("@email", " ");
                cmd.Parameters.AddWithValue("@indexstatus", ticket.IndexStatus);
                cmd.Parameters.AddWithValue("@indexfstation", ticket.IndexFStation);
                cmd.Parameters.AddWithValue("@indextstation", ticket.IndexTStation);
                cmd.Parameters.AddWithValue("@indexpackage", ticket.IndexPackage);
                cmd.Parameters.AddWithValue("@subtotal", ticket.Subtotal);
                cmd.Parameters.AddWithValue("@amount", ticket.Amount);
                cmd.Parameters.AddWithValue("@discount", ticket.Discount);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch
                {
                    return View(ticket);
                }
                finally
                {
                    conn.Close();
                }
                return View("PurchaseTicketInvoice", ticket);
            }
            else
                return View(ticket);
        }

        public IActionResult SendMail(string id)
        {
            IList<MRTTicket> dbList = GetDbList();
            var result = dbList.First(x => x.ViewId == id);

            var subject = "Ticket Information " + result.ViewId;
            var body = "Ticket Id: " + result.ViewId + "<br>" +
                "Date and time: " + result.ViewDateTime + "<br>" +
                "Name: " + result.Name + "<br>" +
                "Phone" + result.Phone + "<br>" +
                "Status: " + result.DictStatus[result.IndexStatus] + "<br>" +
                "From Station: " + result.DictFStation[result.IndexFStation] + "<br>" +
                "To Station: " + result.DictTStation[result.IndexTStation] + "<br>" +
                "Paid Amount: " + result.Amount.ToString("c2");

            var mail = new Mail(configuration);

            if (mail.Send(configuration["Gmail:Username"], result.Email, subject, body))
            {

                ViewBag.Message = "Mail successfully sent to " + result.Email;
                ViewBag.Body = body;
            }
            else
            {

                ViewBag.Message = "Sent Mail Failed";
                ViewBag.Body = "";
            }
            return View(result);
        }

        [HttpGet]
        public IActionResult ReportTravel()
        {
            MRTTicket ticket = new MRTTicket();
            ticket.IndexFStation = ticket.IndexTStation =
                ticket.IndexStatus = ticket.IndexPackage = -1;
            return View(ticket);
        }
        [HttpPost]
        public IActionResult ReportTravel(MRTTicket ticket)
        {

            return View("ReportTravelInvoice", ticket);
        }

        [HttpGet]
        public IActionResult AdminLogin()
        {
            
            return View();
        }

        [HttpPost]
        public IActionResult AdminLogin(LoginAcc account)
        {
            string email = "admin@gmail.com";
            string password = "12345";
            string invalid = "Invalid email or password";


            if ((email == account.Email) && (password == account.Password))
            {
                return View("Home", account);
            }
            else
                ViewBag.Message = invalid;
            return View(account);
        }
        
        [HttpGet]
        public IActionResult Complaint()
        {

            Helpdesk complaint = new Helpdesk();
                complaint.IndexStatus = -1;
            return View(complaint);
        }
        
        [HttpPost]
        public IActionResult Complaint(Helpdesk complaint)
        {
            if (ModelState.IsValid)
            {
                SqlConnection conn = new SqlConnection(configuration.
                    GetConnectionString("MRTConnStr"));
                SqlCommand cmd = new SqlCommand("spCService", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@userid", complaint.userID);
                if (complaint.complain != null)
                    cmd.Parameters.AddWithValue("@complaint", complaint.complain);
                else
                    cmd.Parameters.AddWithValue("@complaint", " ");
                if (complaint.feedback != null)
                    cmd.Parameters.AddWithValue("@feedback", complaint.feedback);
                else
                    cmd.Parameters.AddWithValue("@feedback", " ");

                cmd.Parameters.AddWithValue("@status", complaint.IndexStatus);
              
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch
                {
                    return View(complaint);
                }
                finally
                {
                    conn.Close();
                }
                return View("ComplainSubmit", complaint);
            }
            else
                return View(complaint);
        }
        
        [HttpGet]
        public IActionResult Feedback()
        {

            Helpdesk feedbacks = new Helpdesk();
            feedbacks.IndexStatus = -1;
            return View(feedbacks);
        }
        
        [HttpPost]
        public IActionResult Feedback(Helpdesk feedbacks)
        {
            if (ModelState.IsValid)
            {
                SqlConnection conn = new SqlConnection(configuration.
                    GetConnectionString("MRTConnStr"));
                SqlCommand cmd = new SqlCommand("spCService", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@userid", feedbacks.userID);
                if (feedbacks.complain != null)
                    cmd.Parameters.AddWithValue("@complaint", feedbacks.complain);
                else
                    cmd.Parameters.AddWithValue("@complaint", " ");
                if (feedbacks.feedback != null)
                    cmd.Parameters.AddWithValue("@feedback", feedbacks.feedback);
                else
                    cmd.Parameters.AddWithValue("@feedback", " ");

                cmd.Parameters.AddWithValue("@status", feedbacks.IndexStatus);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch
                {
                    return View(feedbacks);
                }
                finally
                {
                    conn.Close();
                }
                return View("FeedbackSubmit", feedbacks);
            }
            else
                return View(feedbacks);
        }
    }
}
