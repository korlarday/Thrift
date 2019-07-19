using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Thrift.Models.Data;

namespace Thrift.Models.ViewModel.Account
{
    public class CustomerMessagesVM
    {
        public CustomerMessagesVM()
        {

        }
        public CustomerMessagesVM(CustomerMessagesDTO row)
        {
            Id = row.Id;
            Title = row.Title;
            Body = row.Body;
            RequestReply = row.RequestReply;
            Status = row.Status;
            TimeSent = row.TimeSent;
            Approve = row.Approve;
            CustomerId = row.CustomerId;
            Slug = row.Slug;
        }

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