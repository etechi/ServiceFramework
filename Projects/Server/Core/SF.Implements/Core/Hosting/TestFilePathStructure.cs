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
	public class TestFilePathStructure : IDefaultFilePathStructure
	{
		public string BinaryPath { get; set; }
		public string RootPath { get; set; }

		public FilePathDefination FilePathDefination => new FilePathDefination();

	}
}