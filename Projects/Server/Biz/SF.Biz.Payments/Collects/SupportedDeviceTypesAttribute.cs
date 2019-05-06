using SF.Sys.Clients;
using System;

namespace SF.Biz.Payments
{
    /// <summary>
    /// 声明支持的客户端类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SupportedDeviceTypesAttribute : Attribute
    {
        public ClientDeviceType[] DeviceTypes { get; }
        public SupportedDeviceTypesAttribute(ClientDeviceType[] DeviceTypes)
        {
            this.DeviceTypes = DeviceTypes;
        }
    }

}
