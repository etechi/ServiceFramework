﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Entities;
using SF.Sys.Clients;
using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Services.Management.Models;

namespace SF.Biz.Payments
{

    public class RefundStartArgument
    {
        public DateTime SubmitTime { get; set; }
        public long CollectIdent { get; set; }
        public string CollectExtIdent { get; set; }
        public string Title { get; set; }
        public string Desc { get; set; }
        public decimal Amount { get; set; }

        public TrackIdent BizRoot { get; set; }
        public TrackIdent BizParent { get; set; }
        public ClientInfo ClientInfo { get; set; }
        public long PaymentPlatformId { get; set; }
        public RefundState CurState { get; set; }
    }
    public class RefundRequest : RefundStartArgument
    {
    }

    public class RefundRefreshResult
    {
        public long Id { get; set; }
        public DateTime UpdatedTime { set; get; }
        public string Error { get; set; }
        public RefundState State { get; set; }
        public string Desc { get; set; }
        public DateTime? Expires { get; set; }
    }


    public interface IRefundService
    {
        Task<RefundRefreshResult> Create(RefundRequest Argument);
        Task<RefundRefreshResult> RefreshRefundRecord(long Ident);
    }

}
