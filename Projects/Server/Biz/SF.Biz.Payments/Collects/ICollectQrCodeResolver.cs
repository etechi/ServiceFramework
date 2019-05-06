namespace SF.Biz.Payments
{
    public interface ICollectQrCodeResolver
	{
		string GetQrCode(string ExtraData);
	}

}
