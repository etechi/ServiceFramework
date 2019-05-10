
namespace SF.Biz.Delivery.Data
{
    public static class DeliveryTransportData
	{
		public static DataModels.DataDeliveryTransport[] Items { get; } = new[]
		{
			#region items
			new DataModels.DataDeliveryTransport{Order=1,Code="ems",Name="EMS快递"},
			new DataModels.DataDeliveryTransport{Order=2,Code="shentong",Name="申通快递"},
			new DataModels.DataDeliveryTransport{Order=3,Code="shunfeng",Name="顺丰快递"},
			new DataModels.DataDeliveryTransport{Order=4,Code="yuantong",Name="圆通快递"},
			new DataModels.DataDeliveryTransport{Order=5,Code="yunda",Name="韵达快递"},
			new DataModels.DataDeliveryTransport{Order=6,Code="huitong",Name="百世汇通快递"},
			new DataModels.DataDeliveryTransport{Order=7,Code="tiantian",Name="天天快递"},
			new DataModels.DataDeliveryTransport{Order=8,Code="zhongtong",Name="中通快递"},
			new DataModels.DataDeliveryTransport{Order=9,Code="zhaijisong",Name="宅急送快递"},
			new DataModels.DataDeliveryTransport{Order=10,Code="pingyou",Name="中国邮政"},
			#endregion

		};

	}
}
