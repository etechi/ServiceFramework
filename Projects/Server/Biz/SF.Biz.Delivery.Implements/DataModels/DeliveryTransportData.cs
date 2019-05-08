
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceProtocol.ObjectManager;
namespace ServiceProtocol.Biz.Delivery.Entity.Models
{
	public static class DeliveryTransportData
	{
		public static DeliveryTransport[] Items { get; } = new[]
		{
			#region items
			new DeliveryTransport{Order=1,Ident="ems",Name="EMS快递"},
			new DeliveryTransport{Order=2,Ident="shentong",Name="申通快递"},
			new DeliveryTransport{Order=3,Ident="shunfeng",Name="顺丰快递"},
			new DeliveryTransport{Order=4,Ident="yuantong",Name="圆通快递"},
			new DeliveryTransport{Order=5,Ident="yunda",Name="韵达快递"},
			new DeliveryTransport{Order=6,Ident="huitong",Name="百世汇通快递"},
			new DeliveryTransport{Order=7,Ident="tiantian",Name="天天快递"},
			new DeliveryTransport{Order=8,Ident="zhongtong",Name="中通快递"},
			new DeliveryTransport{Order=9,Ident="zhaijisong",Name="宅急送快递"},
			new DeliveryTransport{Order=10,Ident="pingyou",Name="中国邮政"},
			#endregion

		};

	}
}
