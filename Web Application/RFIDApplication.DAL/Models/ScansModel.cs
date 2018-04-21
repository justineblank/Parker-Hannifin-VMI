using System;
using System.Collections.Generic;
using System.Text;

namespace RFIDApplication.DAL.Models
{
    public class ScansModel
    {
        public int id { get; set; }
        public string readerId { get; set; }
        public int antenna { get; set; }
        public string epc { get; set; }
        public DateTime timestamp { get; set; }
        public string syncStatus { get; set; }
        public string Message { get; set; }
        public string location { get; set; }
        public DateTime lastSeen { get; set; }
        
    }
}
