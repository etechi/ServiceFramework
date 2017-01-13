﻿namespace SF
{
	public enum ClientDeviceType
	{
		Unknown,
		PC,
		Mobile,
		APP,
		Wechat
	}
	public class ClientAccessInfo
	{
		public string Address { get; set; }
		public string Agent { get; set; }
		public ClientDeviceType DeviceType { get; set; }
	}
}
