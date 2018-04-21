using System;
using System.Collections.Generic;
using System.Text;

namespace RFIDApplication.DAL.Models
{
    public class RFIDPayloadModel
    {
        public string readerId { get; set; }
        public string tagCount { get; set; }
        public List<Tags> tags { get; set; }
    }   

    public class Tags
    {
        public string readId { get; set; }
        public string epc { get; set; }
        public string timestamp { get; set; }
        public int antenna { get; set; }

    }

    public class RFIDPayloadResponseModel
    {
        public string status { get; set; }
        public string message { get; set; }
        public int tagCount { get; set; }
        public List<TagsResponse> tags { get; set; }
    }

    public class TagsResponse
    {
        public string readId { get; set; }
        public string status { get; set; }
        public string message { get; set; }

    }
}
