using SF.Sys.Threading;

namespace SF.Biz.Trades.Managements
{
    public interface ITradeSyncQueue: SF.Sys.Threading.ISyncQueue<long>
    {
    }
    public class TradeSyncQueue : 
	    ObjectSyncQueue<long>,
        ITradeSyncQueue
    {
    }
}
