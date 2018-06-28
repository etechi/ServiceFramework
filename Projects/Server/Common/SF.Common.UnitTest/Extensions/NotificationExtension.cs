using Microsoft.VisualStudio.TestTools.UnitTesting;
using SF.Common.Notifications;
using SF.Common.Notifications.Management;
using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Sys.Services;
using SF.Sys;
using SF.Common.Notifications.Models;
using SF.Common.Notifications.Front;
using SF.Sys.Entities;
using SF.Sys.Clients;
using SF.Sys.Auth;

namespace SF.Common.UnitTest
{
	public static class NotificationExtension 
    {
		static string NewRandString(int Len)
			=> Strings.NumberAndLowerUpperChars.Random(Len);

        public static async Task<NotificationEditable> CreateBroadcastNotification(
			this IServiceProvider sp,
            DateTime? time=null,
            DateTime expires=default, 
            string title=null,
            string link=null,
            string image=null,
            string body=null,
            int? senderid=null,
            string trackid=null
            )
        {
            var nm = sp.Resolve<INotificationManager>();

            if (time == null)
                time = DateTime.Now.FloorToSecond();
            if (title == null)
                title = "title-"+ Strings.NumberAndLowerUpperChars.Random(10);
            if (link == null)
                link = "link-" + NewRandString(10);
            if(image==null)
                image= "image-" + NewRandString(10);
            if (body == null)
                body = "body-" + NewRandString(10);
			if (expires == default)
				expires = time.Value.AddDays(10);
            var id=await nm.CreateBroadcastNotification(
				"测试",
				new System.Collections.Generic.Dictionary<string, object>(),
                time.Value,
				expires,
				trackid,
				image,
				link,
				title,
                body
            );

            var re=await nm.LoadForEdit(ObjectKey.From(id));

            Assert.AreEqual(time, re.Time);
            Assert.AreEqual(expires, re.Expires);
            Assert.AreEqual(title, re.Name);
            Assert.AreEqual(link, re.Link);
            Assert.AreEqual(body, re.Content);
            Assert.AreEqual(image, re.Image);
            Assert.AreEqual(senderid, re.SenderId);
            Assert.AreEqual(trackid, re.BizIdent);
            
            return re;
        }
        public static async Task<NotificationEditable> CreateNormalNotification(
			this IServiceProvider sp,
            DateTime? time = null,
            string title = null,
            string link = null,
            string image = null,
            string body = null,
            int? senderid = null,
            string trackid = null,
            long[] targets = null
            )
        {
			var nm = sp.Resolve<INotificationManager>();

			if (time == null)
                time = DateTime.Now.FloorToSecond();
            if (title == null)
                title = "title-" + NewRandString(10);
            if (link == null)
                link = "link-" + NewRandString(10);
            if (image == null)
                image = "image-" + NewRandString(10);
            if (body == null)
                body = "body-" + NewRandString(10);
            if (trackid == null)
                trackid = "track-" + NewRandString(10);

            var id = await nm.CreateNormalNotification(
				targets==null?sp.Resolve<IAccessToken>().User.GetUserIdent():targets.Single(),
				"测试",
				new System.Collections.Generic.Dictionary<string, object>(),
				time.Value,
				default,
				trackid,
				image,
				link,
				title,
				body
            );

            var re = await nm.LoadForEdit(ObjectKey.From(id));

            Assert.AreEqual(time, re.Time);
            Assert.AreEqual(title, re.Name);
            Assert.AreEqual(link, re.Link);
            Assert.AreEqual(body, re.Content);
            Assert.AreEqual(image, re.Image);
            Assert.AreEqual(senderid, re.SenderId);
            Assert.AreEqual(trackid, re.BizIdent);

            return re;
        }

        public static async Task<UserNotification[]> ListNotification(
			this IServiceProvider sp,
			NotificationMode? mode
			)
        {
            var nm = sp.Resolve<INotificationService>();
			var re = await nm.Query(new SF.Common.Notifications.Front.NotificationQueryArgument
			{
				Mode = mode,
			});
            return re.Items.ToArray();
        }
        public static async Task<UserNotification> GetNotification(this IServiceProvider sp, long Id)
        {
			var nm = sp.Resolve<INotificationService>();
			return await nm.Get(Id);
        }
        public static async Task ReadNotification(this IServiceProvider sp, long[] Ids)
        {
			var nm = sp.Resolve<INotificationService>();
			await nm.SetReaded(new SetReadedArgument
			{
				NotificationIds = Ids
			});
            
            foreach (var id in Ids)
            {
                var nn = await GetNotification(sp,  id);
                Assert.IsNotNull(nn.ReadTime);
            }
        }
        public static async Task DeleteNotification(this IServiceProvider sp,  long NotificationId)
        {
			var nm = sp.Resolve<INotificationService>();
            await nm.Delete( NotificationId);
            Assert.IsNull(await GetNotification(sp,NotificationId));
        }
        public static async Task EnsureStatusChanged(
			this IServiceProvider sp,
            int TotalChanged, 
            int UnreadedChanged, 
            Func<Task> Action
            )
        {
			var nm = sp.Resolve<INotificationService>();
			var status = await nm.GetStatus();
            await Action();
            var newStatus = await nm.GetStatus();
            Assert.AreEqual(status.Received + TotalChanged, newStatus.Received);
            Assert.AreEqual(status.ReceivedUnreaded + UnreadedChanged, newStatus.ReceivedUnreaded);
        }

        public static async Task EnsureNotification(this IServiceProvider sp, Notification n)
        {
			var nm = sp.Resolve<INotificationService>();
			var un = await GetNotification(sp, n.Id);
            Assert.IsNotNull(un);
            Assert.AreEqual( true, un.HasBody);
            Assert.AreEqual(n.Image, un.Image);
            Assert.AreEqual(n.Link, un.Link);
            Assert.AreEqual(n.SenderId, un.SenderId);
            Assert.AreEqual(n.Time, un.Time);
            Assert.AreEqual(n.Name, un.Name);
            Assert.AreEqual(n.BizIdent, un.BizIdent);
            Assert.AreEqual(n.Content, un.Content);
        }


    }
}
