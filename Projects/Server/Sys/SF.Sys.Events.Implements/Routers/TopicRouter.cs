using SF.Sys.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SF.Sys.Events
{
	public class TopicRouter
	{
		EventManager EventManager { get; }
		TopicDirectory Root { get; } 
		public TopicRouter(EventManager ventManager)
		{
			this.EventManager = EventManager;
			this.Root = new TopicDirectory(EventManager, string.Empty, string.Empty);
		}

		static bool SegFilterMatch(IReadOnlyList<string> filters,string[] Segments,int Offset)
		{
			var segc = Segments.Length;
			
			for(var i = 0; i < filters.Count; i++)
			{
				var f = filters[i];
				if (f.Length == 1)
				{
					if (f[0] == '+')
						continue;
					else if (f[0] == '#')
						return true;
				}

				var segi = Offset + i;
				if (segi >= segc)
					return false;
				if (!f.Equals(Segments[segi]))
					return false;
			}
			return true;
		}
		
		public IEnumerable<IEventObserver<TPayload>> GetObservers<TPayload>(string Topic,bool Sync)
		{
			var dir = Root;
			var segs = Topic.Split('/');
			var segc = segs.Length;
			for(var i=0;i<segc;i++)
			{
				foreach (var v in dir.GetObservers<TPayload>(Sync))
				{
					if(v.SegFilters!=null && SegFilterMatch(v.SegFilters,segs,i))
						yield return v.Observer;
				}
				var seg = segs[i];
				if (!dir.TryGetValue(seg, out var cdir))
					yield break;
				dir = cdir;
			}
			foreach (var v in dir.GetObservers<TPayload>(Sync))
			{
				if (v.SegFilters == null || SegFilterMatch(v.SegFilters, segs, segc))
					yield return v.Observer;
			}
		}
		public IDisposable Subscribe<TPayload>(
			string Topic,
			string SubscriberIdent,
			EventDeliveryPolicy Policy,
			IEventObserver<TPayload> Observer
			)
		{
			var dir = Root;
			var segs = Topic.Split('/').ToArray();
			var i = 0;

			for(;i<segs.Length;i++)
			{
				var seg = segs[i];

				if (seg.Length == 1 && (seg[0] == '#' || seg[0] == '+'))
					break;

				TopicDirectory newDir = null;
				for (; ; )
				{
					if (dir.TryGetValue(seg, out var cdir))
					{
						dir = cdir;
						break;
					}
					else if (dir.TryAdd(
						seg, 
						cdir = newDir = newDir ?? new TopicDirectory(EventManager,seg,segs.Take(i+1).Join("/"))
						))
					{
						dir = cdir;
						break;
					}
				}
			}

			return dir.Subscribe(
				i == segs.Length ? null : segs.Skip(i).ToArray(),
				SubscriberIdent,
				Policy,
				Observer
				);
		}
    }
}
 