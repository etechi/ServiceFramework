using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Internal;
using System;
using SF.Core.NetworkService;

namespace SF.AspNetCore.NetworkServices
{
	public class ServiceBasedControllerActivator : DefaultControllerActivator
	{
		public ServiceBasedControllerActivator(ITypeActivatorCache TypeActivatorCache):base(TypeActivatorCache)
		{
		}
		static object IsCreatedFromDefaultActivator { get; } = new object();
		/// <inheritdoc />
		public override object Create(ControllerContext actionContext)
		{
			if (actionContext == null)
			{
				throw new ArgumentNullException("actionContext");
			}
			Type serviceType = actionContext.ActionDescriptor.ControllerTypeInfo.AsType();
			var services = actionContext.HttpContext.RequestServices;
			var s = services.GetService(serviceType);
			if (s == null)
			{
				s = base.Create(actionContext);
				if (s != null)
					actionContext.HttpContext.Items[IsCreatedFromDefaultActivator] = IsCreatedFromDefaultActivator;
			}
			return s;
		}

		/// <inheritdoc />
		public override void Release(ControllerContext context, object controller)
		{
			object v;
			if (context.HttpContext.Items.TryGetValue(IsCreatedFromDefaultActivator, out v) && v == IsCreatedFromDefaultActivator)
				base.Release(context, controller);
		}
	}
}
