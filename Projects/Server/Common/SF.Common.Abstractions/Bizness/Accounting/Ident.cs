using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProtocol.Biz.Accounting
{
    public static class Ident
    {
		public static string CreateDepositIdent(DateTime Time,string DstId,int Id)
		{
			return "DP" + Id.ToString().PadLeft(8, '0') + Time.ToString("yyyyMMdd") + DstId.PadLeft(6, '0')+ ExtSystemTag.Value;
		}
		public static int ParseDepositIdent(string Ident)
		{
			if (string.IsNullOrEmpty(Ident)) return 0;
			if (Ident.Length < 2 + 8 + 8 + 6) return 0;
			if (!Ident.StartsWith("DP")) return 0;
			int re;
			if (!int.TryParse(Ident.Substring(2, 8), out re)) return 0;
			return re;
		}

        public static string CreateRefundIdent(DateTime Time, string DstId, int Id)
        {
            return "RF" + Id.ToString().PadLeft(8, '0') + Time.ToString("yyyyMMdd") + DstId.PadLeft(6, '0')+ ExtSystemTag.Value;
        }
        public static int ParseRefundIdent(string Ident)
        {
            if (string.IsNullOrEmpty(Ident)) return 0;
            if (Ident.Length < 2 + 8 + 8 + 6) return 0;
            if (!Ident.StartsWith("RF")) return 0;
            int re;
            if (!int.TryParse(Ident.Substring(2, 8), out re)) return 0;
            return re;
        }
    }
}
