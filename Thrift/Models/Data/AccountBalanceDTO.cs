﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Thrift.Models.Data
{
    [Table("tblAccountBalance")]
    public class AccountBalanceDTO
    {
        [Key]
        public int Id { get; set; }
        public decimal AccountBalance { get; set; }
        public decimal WithdrawalBalance { get; set; }
    }
}