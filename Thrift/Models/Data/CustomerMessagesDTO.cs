using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Thrift.Models.Data
{
    [Table("tblCustomerMessages")]
    public class CustomerMessagesDTO
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public bool RequestReply { get; set; }
        public bool Status { get; set; }
        public DateTime TimeSent { get; set; }
        public bool Approve { get; set; }
        public int CustomerId { get; set; }
        public string Slug { get; set; }
    }
}