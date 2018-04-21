using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RFIDApplication.DAL.Models
{
    public class ReadersModel
    {
        public int id {get; set;}
        public string readerId { get; set; }
        public string location { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}", ApplyFormatInEditMode = true, NullDisplayText = "Never", HtmlEncode = true)]
        public DateTime lastSeen { get; set; }
    }

    public class ReadersEditModel
    {
        public int id { get; set; }
        public string readerId { get; set; }
        public string location { get; set; }        
    }

    public class ReadersReturnModel
    {
        public int id { get; set; }
        public string readerId { get; set; }
        public string location { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mm tt}", ApplyFormatInEditMode = true, NullDisplayText = "Never", HtmlEncode = true)]
        public string lastSeen { get; set; }
    }
}
