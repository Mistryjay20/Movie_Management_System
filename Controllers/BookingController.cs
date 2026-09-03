using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Movie_Management_System.Models;

namespace Movie_Management_System.Controllers
{
    public class BookingController : Controller
    {
        public List<SelectListItem> Bind_Movie(int cat_id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["dbconnection"].ToString();
            List<SelectListItem> list = new List<SelectListItem>();
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("Bind_Movie", connection);
            cmd.Parameters.AddWithValue("@Cat_id", cat_id);
            cmd.CommandType = CommandType.StoredProcedure;
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new SelectListItem { Value = reader["Movie_id"].ToString(), Text = reader["Movie_name"].ToString() + "\t\t|\t\tPrice : " + reader["Rate"].ToString() });
            }
            ViewBag.MovieList = list;
            return list;
        }
        public int Calculate_Price(int movie_id, int no_of_tickets)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["dbconnection"].ToString();
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("Get_Rate", connection);
            cmd.Parameters.AddWithValue("@Movie_id", movie_id);
            cmd.CommandType = CommandType.StoredProcedure;
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            int price = 0;
            if (reader.Read())
            {
                price = Convert.ToInt32(reader["Rate"]);
            }
            int total_price = price * no_of_tickets;
            ViewBag.TotalPrice = total_price;
            return total_price;
        }
        public int Get_User_id()
        {
            int id = 0;
            if (Session["Email"] != null)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["dbconnection"].ToString();
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand("Get_User", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                connection.Open();
                cmd.Parameters.AddWithValue("@Email_id", Session["Email"]);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    id = Convert.ToInt32(reader["User_id"]);
                }

            }
            return id;
        }
            // GET: Booking
            public ActionResult Index()
        {
            return View();
        }

        // GET: Booking/Details/5
        public ActionResult Details(Booking booking)
        {
           
            string ConnectionString = ConfigurationManager.ConnectionStrings["dbconnection"].ToString();
            List<Booking> bookings = new List<Booking>();
            SqlConnection connection = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand("Get_booking", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@User_id", Get_User_id());
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                booking = new Booking();

                booking.Booking_id = Convert.ToInt32(reader["Booking_id"]);
                booking.User_id = Convert.ToInt32(reader["User_id"]);
                booking.Cat_type = Convert.ToString(reader["Cat_type"]);
                booking.Movie_name = Convert.ToString(reader["Movie_name"]);
                booking.No_of_Tickets = Convert.ToInt32(reader["No_of_Tickets"]);
                booking.amount = Convert.ToInt32(reader["amount"]);

                bookings.Add(booking);
            }
            reader.Close();
            connection.Close();

            return View(bookings);
        }

            // GET: Booking/Create
            public ActionResult Create(int? cat_id, int? movie_id, int? no_of_ticket)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["dbconnection"].ToString();
                List<SelectListItem> list = new List<SelectListItem>();
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand("Bind_Category", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new SelectListItem { Value = reader["Cat_id"].ToString(), Text = reader["Cat_Type"].ToString() });
                }
                ViewBag.CategoryList = list;
                ViewBag.MovieList = new List<SelectListItem>();
                // Bind Movie according to selected Category
                if (cat_id != null)
                {
                    ViewBag.MovieList = Bind_Movie(cat_id.Value);
                }
                else
                {
                    ViewBag.MovieList = new List<SelectListItem>();
                }

                Booking book = new Booking();

                if (cat_id != null)
                {
                    book.Cat_id = cat_id.Value;
                }

                if (movie_id != null)
                {
                    book.Movie_id = movie_id.Value;
                }

                if (no_of_ticket != null)
                {
                    book.No_of_Tickets = no_of_ticket.Value;
                }

                if (movie_id != null && no_of_ticket != null)
                {
                    book.amount = Calculate_Price(movie_id.Value, no_of_ticket.Value);
                    ViewBag.TotalPrice = book.amount;
                }
                return View(book);
            }

            // POST: Booking/Create
            [HttpPost]
        public ActionResult Create(Booking book)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string connectionString = ConfigurationManager.ConnectionStrings["dbconnection"].ToString();
                    SqlConnection connection = new SqlConnection(connectionString);
                    SqlCommand cmd = new SqlCommand("Insert_Booking", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    cmd.Parameters.AddWithValue("@User_id", Get_User_id());
                    cmd.Parameters.AddWithValue("@Cat_id", book.Cat_id);
                    cmd.Parameters.AddWithValue("@Movie_id", book.Movie_id);
                    cmd.Parameters.AddWithValue("@no_of_Tickets", book.No_of_Tickets);
                    cmd.Parameters.AddWithValue("@amount", book.amount);
                    int i = cmd.ExecuteNonQuery();
                    connection.Close();
                    if (i > 0)
                    {
                        ViewBag.Message = "Booking Insert Successfully";
                        return View(book);
                    }
                }

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex + " Insertion Failed";
                return View(book);
            }
        }

        // GET: Booking/Edit/5
        public ActionResult Edit(int id, int? cat_id, int? movie_id, int? no_of_ticket)
        {
            Booking book = new Booking();
            string connectionString = ConfigurationManager.ConnectionStrings["dbconnection"].ToString();

            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("Get_Booking_ById", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Booking_id", id);

            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                book.Booking_id = Convert.ToInt32(reader["Booking_id"]);
                book.User_id = Convert.ToInt32(reader["User_id"]);
                book.Cat_id = Convert.ToInt32(reader["Cat_id"]);
                book.Movie_id = Convert.ToInt32(reader["Movie_id"]);
                book.No_of_Tickets = Convert.ToInt32(reader["No_of_Tickets"]);
                book.amount = Convert.ToInt32(reader["Amount"]);
            }

            reader.Close();
            connection.Close();

            List<SelectListItem> list = new List<SelectListItem>();

            connection = new SqlConnection(connectionString);
            cmd = new SqlCommand("Bind_Category", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            connection.Open();
            reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new SelectListItem
                {
                    Value = reader["Cat_id"].ToString(),
                    Text = reader["Cat_Type"].ToString()
                });
            }

            reader.Close();
            connection.Close();

            ViewBag.CategoryList = list;

            if (cat_id != null)
            {
                book.Cat_id = cat_id.Value;
            }

            if (movie_id != null)
            {
                book.Movie_id = movie_id.Value;
            }

            if (no_of_ticket != null)
            {
                book.No_of_Tickets = no_of_ticket.Value;
            }

            ViewBag.MovieList = new List<SelectListItem>();

            if (book.Cat_id != 0)
            {
                ViewBag.MovieList = Bind_Movie(book.Cat_id);
            }

            if (movie_id != null && no_of_ticket != null)
            {
                book.amount = Calculate_Price(movie_id.Value, no_of_ticket.Value);
                ViewBag.TotalPrice = book.amount;
            }
            else
            {
                ViewBag.TotalPrice = book.amount;
            }

            return View(book);
        }

        // POST: Booking/Edit/5
        [HttpPost]
        public ActionResult Edit(Booking book)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    string connectionString =
                        ConfigurationManager.ConnectionStrings["dbconnection"].ToString();

                    // Calculate amount again using the selected movie
                    // and the new number of tickets.
                    book.amount = Calculate_Price(
                        book.Movie_id,
                        book.No_of_Tickets
                    );

                    using (SqlConnection connection =
                           new SqlConnection(connectionString))
                    {
                        SqlCommand cmd =
                            new SqlCommand("Update_Booking", connection);

                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue(
                            "@Booking_id",
                            book.Booking_id);

                        cmd.Parameters.AddWithValue(
                            "@User_id",
                            Get_User_id());

                        cmd.Parameters.AddWithValue(
                            "@Cat_id",
                            book.Cat_id);

                        cmd.Parameters.AddWithValue(
                            "@Movie_id",
                            book.Movie_id);

                        cmd.Parameters.AddWithValue(
                            "@no_of_Tickets",
                            book.No_of_Tickets);

                        cmd.Parameters.AddWithValue(
                            "@amount",
                            book.amount);

                        connection.Open();

                        int i = cmd.ExecuteNonQuery();

                        connection.Close();

                        if (i > 0)
                        {
                            ViewBag.Message =
                                "Booking Update Successfully";
                        }
                    }

                    ViewBag.TotalPrice = book.amount;

                    return View(book);
                }

                return View(book);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message + " Updation Failed";
                return View(book);
            }
        }

        // GET: Booking/Delete/5
        public ActionResult Delete(int id)
        {
            Booking book = new Booking();
            string connectionString = ConfigurationManager.ConnectionStrings["dbconnection"].ToString();
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("Get_Booking_ById", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Booking_id", id);

            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                book.Booking_id = Convert.ToInt32(reader["Booking_id"]);
                book.User_id = Convert.ToInt32(reader["User_id"]);
                book.Cat_type = (reader["Cat_type"]).ToString();
                book.Movie_name = (reader["Movie_name"]).ToString();
                book.No_of_Tickets = Convert.ToInt32(reader["No_of_Tickets"]);
                book.amount = Convert.ToInt32(reader["Amount"]);
            }

            reader.Close();
            connection.Close();

            return View(book);
        }

        // POST: Booking/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, Booking book)
        {
            try
            {
                // TODO: Add delete logic here
                string connectionString = ConfigurationManager.ConnectionStrings["dbconnection"].ToString();
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand("Delete_Booking", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                connection.Open();
                cmd.Parameters.AddWithValue("@Booking_id", id);
                int i = cmd.ExecuteNonQuery();
                connection.Close();
                if (i > 0)
                {
                    ViewBag.Message = "Delete Succssfully";
                }
                return View(book);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex + " Delete Failed";
                return View(book);
            }
        }
    }
}
