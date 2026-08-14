using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Movie_Management_System.Models
{
    public class movie
    {
        public int Movie_ID { get; set; }
        public string Movie_name { get; set; }
        public string Release_date { get; set; }
        public int Cat_id { get; set; }
        public int Rate { get; set; }

    }
}