using Movie_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Movie_Management_System.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            if (Session["Email"] != null)
            {
                ViewBag.LoginSuccess = "Welcome " + Session["Email"].ToString();
            }
            else
            {
                ViewBag.LoginFailed = "Please Try Again Login Failed";
            }

            return View();
        }

        // GET: Login/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Login/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Login/Create
        [HttpPost]
        public ActionResult Create(login user)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["dbconnection"].ToString();
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand cmd = new SqlCommand("Get_User", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                connection.Open();
                cmd.Parameters.AddWithValue("@Email_id", user.Email_id);
                cmd.Parameters.AddWithValue("@User_password", user.User_password);
                int result = (int)cmd.ExecuteScalar();
                if (result > 0)
                {
                    Session["Email"] = user.Email_id;
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "Email or Password Invalid.";
                    return View(user);
                }
            }
            catch(Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(user);
            }
        }

        // GET: Login/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Login/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Login/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Login/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
