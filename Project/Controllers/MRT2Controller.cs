using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project.MailSettings;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using Project.Models;
using Microsoft.AspNetCore.Identity;
using Project.Areas.Identity.Data;

namespace Project.Controllers
{
    public class MRT2Controller : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IConfiguration configuration;
        public MRT2Controller(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, IConfiguration config)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = config;
        }

        // GET: MRT2Controller
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

        public IActionResult Index()
        {
            if (signInManager.IsSignedIn(User))
            {

                IList<MRTTicket> dbList = GetDbList();
                //ViewBag.Email = dbList.Where(x => x.Email == userManager.GetUserName(User));

                var result = dbList.Select
                (x => new MRTTicket()
                {
                    ViewId = x.ViewId,
                    ViewDateTime = x.ViewDateTime,
                    Name = x.Name,
                    Email = x.Email,
                    Phone = x.Phone,
                    IndexStatus = x.IndexStatus,
                    IndexFStation = x.IndexFStation,
                    IndexTStation = x.IndexTStation,
                }).Where(x => x.Email == userManager.GetUserName(User));

                return View(result);

            }
            else
            {
                IList<MRTTicket> dbList = GetDbList();
                return View(dbList);
            }
        }

        // GET: MRT2Controller/Details/5
        public ActionResult Details(string id)
        {
            IList<MRTTicket> dbList = GetDbList();
            var result = dbList.First(x => x.ViewId == id);
            return View(result);
        }

        // GET: MRT2Controller/Create
        public ActionResult Create()
        {
            MRTTicket ticket = new MRTTicket();
            ticket.IndexFStation = ticket.IndexTStation =
                ticket.IndexStatus = ticket.IndexPackage = -1;
            return View(ticket);
        }

        // POST: MRT2Controller/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MRTTicket ticket)
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
                return View("CreateInvoice", ticket);
            }
            else
                return View(ticket);
        }

        // GET: MRT2Controller/Edit/5
        public ActionResult Edit(string id)
        {
            IList<MRTTicket> dbList = GetDbList();
            var result = dbList.First(x => x.ViewId == id);
            return View(result);
        }

        // POST: MRT2Controller/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, MRTTicket ticket)
        {
            SqlConnection conn = new SqlConnection(configuration.
            GetConnectionString("MRTConnStr"));
            SqlCommand cmd = new SqlCommand("spUpdateTicket", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            
            cmd.Parameters.AddWithValue("@ticketid", id);
            cmd.Parameters.AddWithValue("@name", ticket.Name);
            cmd.Parameters.AddWithValue("@ic", ticket.IC);
            cmd.Parameters.AddWithValue("@phone", ticket.Phone);
            
            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch
            {
                RedirectToAction("Error");
            }
            finally
            {
                conn.Close();
            }
            return RedirectToAction("Index");
        }

        // GET: MRT2Controller/Delete/5
        public ActionResult Delete(string id)
        {
            IList<MRTTicket> dbList = GetDbList();
            var result = dbList.First(x => x.ViewId == id);
            return View(result);
        }

        // POST: MRT2Controller/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult ConfirmDelete(string id)
        {
            SqlConnection conn = new SqlConnection(configuration.
            GetConnectionString("MRTConnStr"));
            SqlCommand cmd = new SqlCommand("spDeleteTicket", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ticketid", id);
            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
            finally
            {
                conn.Close();
            }

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
    }
}
