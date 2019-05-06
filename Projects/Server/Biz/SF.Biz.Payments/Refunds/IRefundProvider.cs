using System;
using System.Threading.Tasks;

namespace SF.Biz.Payments
{
    
    public class RefundResponse
    {
        public long Ident { get; set; }
        public string ExtIdent { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public string Error { get; set; }
        public decimal RefundAmount { get; set; }
        public PaymentRefundState State { get; set; }
    }

    public interface IRefundProvider
    {
        Task<RefundResponse> TryRefund(
            RefundStartArgument StartArgument
            );
    }


}
