using SF.Core.DI;
using System.Linq;
using SF.Metadata;
using System;
using System.Reflection;
using SF.Core.Serialization;
using System.Collections.Generic;
using SF.Metadata.Models;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace SF.Core.Hosting
{
	public class ConsoleDefaultFilePathStructure : IDefaultFilePathStructure
	{
		public string BinaryPath { get; }
		public string RootPath { get; }

		public FilePathDefination FilePathDefination => new FilePathDefination();

		public ConsoleDefaultFilePathStructure()
		{
			BinaryPath = System.AppDomain.CurrentDomain.BaseDirectory;
			RootPath = Path.Combine(BinaryPath, "data");
			if (!Directory.Exists(RootPath))
				Directory.CreateDirectory(RootPath);
		}
	}
}