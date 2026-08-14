using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movie_Management_System.Models
{
    public class Booking
    {
        public int Booking_id { get; set; }
        public int User_id { get; set; }
        public int Cat_id { get; set; }
        public int Movie_id { get; set; }
        public int No_of_Tickets { get; set; }
        public int amount { get; set; }

    }
}