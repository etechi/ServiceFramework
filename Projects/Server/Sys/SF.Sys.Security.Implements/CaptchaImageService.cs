#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using SF.Sys;
using SF.Sys.Drawing;
using SF.Sys.TimeServices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SF.Services.Security
{
	
	public  class CaptchaImageSetting
	{
		/// <summary>
		/// 过期时间(分钟)
		/// </summary>
		public int Expires { get; set; }

		/// <summary>
		/// 字符数
		/// </summary>
		public int CodeChars { get; set; }
	}
		
	public class CaptchaImageService : ICaptchaImageService
	{
		Lazy<IImageProvider> ImageProvider { get; }
		IDataProtector DataProtector { get; }
		ITimeService TimeService { get; }
		CaptchaImageSetting Setting { get; }

		public CaptchaImageService(Lazy<IImageProvider> ImageProvider, IDataProtector DataProtector, CaptchaImageSetting Setting)
		{
			this.ImageProvider = ImageProvider;
			this.DataProtector = DataProtector;
			this.Setting = Setting;
		}

		string CreateImageUrl(string code,Random r,int Width,int Height,string ForeColor,string BgColor)
		{
			var url = Transforms.Source.Draw(
				new Maths.Size(Width, Height),
				(ctx, img) =>
				{
					if (BgColor.HasContent())
						ctx.Clear(BgColor);
					var f = ctx.GetFont("Arial", Height / 2, FontStyle.Bold);
					var b = ctx.GetSolidBrush(Color.Parse(ForeColor ?? "#333"));
					int count = code.Length;
					int wi = Width / (count + 1);
					var rect = new Maths.RectD(0, 0, Width, Height);
					for (int i = 0; i < count; i++)
					{
						ctx.ResetTransform();
						ctx.TranslateTransform((i + 0.5f) * wi, Height / 6);
						ctx.RotateTransform(r.Next(60) - 30);
						ctx.DrawString(code.Substring(i, 1), rect, f, b);
					}
				})
				.ToImageUrl()
				.ApplyTo(ImageProvider.Value);
			return url;
		}
		
		public async Task<CaptchaImage> CreateImage(CaptchaImageCreateArgument Arg)
		{
			var r = RandomFactory.Create();
			var code = SF.Sys.Strings.NumberAndUpperChars.Random(Setting.CodeChars, r);
			var url = CreateImageUrl(code, r, Arg.Width, Arg.Height, Arg.ForeColor, Arg.BgColor);

			var prefix = await DataProtector.Encrypt(Arg.Target, "captcha".UTF8Bytes(), TimeService.Now.AddMinutes(Setting.Expires), code.UTF8Bytes());

			return new CaptchaImage
				{
					Image=url,
					CodePrefix= prefix.Base64()+":"
				};

		}
		public async Task<bool> Validate(string Target ,string Code)
		{
			var i = Code.LastIndexOf(':');
			if (i == -1)
				throw new ArgumentException();
			var data = Code.Substring(0, i);
			Code = Code.Substring(i + 1);

			var result = await DataProtector.Decrypt(
				Target,
				data.Base64(),
				 TimeService.Now,
				(buf, len) => Task.FromResult(Code.UTF8Bytes())
				);
			return result?.UTF8String() == "captcha";
		}

	}
}
