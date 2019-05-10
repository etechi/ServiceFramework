using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Clients;
using SF.Sys.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SF.Biz.Accounting
{
    public class DrawbackArgument
    {
        public long DepositRecordId { get; set; }
        public long OperatorId { get; set; }
        public decimal Amount { get; set; }
        public string AccountTitle { get; set; }
        public string Description { get; set; }
        public string TrackEntityIdent { get; set; }
        public string CallbackName { get; set; }
        public string CallbackContext { get; set; }
        public string OpAddress { get; set; }
        public ClientDeviceType OpDevice { get; set; }
        public string Reason { get; set; }
    }
 
    public interface IDrawbackService
    {
        Task<long> Create(DrawbackArgument Arg);
        Task<DrawbackState> Refresh(long Id, long DstId);


    }
}
