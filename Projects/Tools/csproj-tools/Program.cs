using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using CommandLine;
namespace csproj_tools
{

	class ProjectFile
	{
		public override string ToString()
		{
			return Name;
		}
		public string FilePath { get;  }
		public string Name { get;  }
		public ProjectFile[] References { get; set; }
		public List<ProjectFile> UserProjects { get; set; }
		public ProjectFile(string FilePath)
		{
			this.FilePath = FilePath;
			this.Name = Path.GetFileNameWithoutExtension(FilePath);
		}
		public void OpenProject(Func<XDocument,bool> callback)
		{
			var doc = XDocument.Load(FilePath);
			var re = callback(doc);
			if (re)
				doc.Save(FilePath);
		}
	}


	class SolutionOptions
	{
		[Option('s', HelpText = "默认为当前目录中的解决方案")]
		public string Solution { get; set; }

		static Regex regProject = new Regex("\"[^\"]+\"");

		static string PathNormalize(string Path)
		{
			var ps = Path.Split('\\').ToList();
			for (var i = 0; i < ps.Count; i++)
			{
				if (ps[i] == ".")
				{
					ps.RemoveAt(i);
					i--;
					continue;
				}
				if (ps[i] == "..")
				{
					ps.RemoveAt(i);
					ps.RemoveAt(i - 1);
					i -= 2;
					continue;
				}
			}
			return string.Join("\\", ps);
		}
		

		public IEnumerable<ProjectFile> ParseSolution()
		{
			if ((Solution?.Length ?? 0) == 0)
				Solution = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.sln").SingleOrDefault();
			if (Solution == null)
				throw new ArgumentException("找不到解决方案文件");
			if (!Solution.EndsWith(".sln"))
				Solution += ".sln";
			var fileInfo = new FileInfo(Solution);

			if(!fileInfo.Exists)
				throw new ArgumentException("找不到解决方案文件");
			var slnPath = fileInfo.Directory;

			var slnFileContent = File.ReadAllText(Solution);
			return regProject.Matches(slnFileContent).Select(m =>
			{
				if (!m.Value.EndsWith(".csproj\""))
					return null;
				var srcPath = PathNormalize(Path.Combine(fileInfo.DirectoryName, m.Value.Trim('"')));
				return srcPath;
			}).
			Where(p => p != null)
			.Select(p => new ProjectFile(p))
			.ToArray();
		}
		public void LoadReferences(IEnumerable<ProjectFile> Files)
		{
			var dict = Files.ToDictionary(f => f.FilePath);

			foreach(var f in Files)
			{
				var prj = XDocument.Load(f.FilePath);
				var basePath = Path.GetDirectoryName(f.FilePath);
				f.References =
					(from pr in prj.Root.DescendantsAndSelf("ProjectReference")
					 let include = pr.Attribute("Include").Value
					 where include.EndsWith(".csproj")
					 let dstPath = PathNormalize(Path.Combine(basePath, include))
					 let dstPrj = dict[dstPath]
					 select dstPrj
					).ToArray();
				foreach(var r in f.References)
				{
					if (r.UserProjects == null)
						r.UserProjects = new List<ProjectFile>();
					r.UserProjects.Add(f);
				}
			}

		}
	}

	[Verb("list", HelpText = "罗列项目")]
	class ListProjectsOptions : SolutionOptions
	{
		[Option('n', HelpText = "仅罗列名称")]
		public bool NameOnly{ get; set; }

		[Option('d', HelpText = "按依赖排序")]
		public bool Dependence { get; set; }

		public int Execute()
		{
			var files = ParseSolution();

			if (Dependence)
			{
				LoadReferences(files);
				var fs = new List<ProjectFile>();
				var exists = new HashSet<string>();
				for(; ; )
				{
					var empty = true;
					foreach(var f in files)
					{
						if (exists.Contains(f.FilePath))
							continue;

						if(f.References?.All(r=>exists.Contains(r.FilePath)) ?? true)
						{
							exists.Add(f.FilePath);
							fs.Add(f);
							empty = false;
						}
					}
					if (empty)
						break;
				}
				files = fs;
			}


			foreach (var p in files)
			{
				Console.WriteLine(NameOnly ? p.Name : p.FilePath);
			}
			return 0;
		}
	}

	[Verb("add-version", HelpText = "增加指定项目及依赖项目的版本号")]
	class AddVersionOptions : SolutionOptions
	{
		[Option('p',Required =true,HelpText ="目标项目名，不含扩展名")]
		public string Project{ get; set; }

		[Option('l',Default =3,HelpText ="增加版本号级别，1-主版本，2-子版本，3-补丁")]
		public int Level { get; set; }

		[Option('v', Default = 0, HelpText = "指定版本")]
		public int Version { get; set; }

		void AddVersion(ProjectFile f)
		{
			f.OpenProject(doc =>
			{
				var v = doc.Descendants("Version").FirstOrDefault();
				if (v == null)
				{
					var pg = doc.Descendants("PropertyGroup").First();
					pg.Add(v = new XElement("Version", "1.0.0"));
				}

				var vs = string.Join(
					".",
					v.Value.Split(".").Select((vi, i) =>  i == Level - 1 ?(Version==0?int.Parse(vi)+ 1:Version) : int.Parse(vi))
					);
				Console.WriteLine($"{f.Name} {v.Value}=>{vs}");
				v.Value = vs;
				return true;
			});
		}
		public int Execute()
		{
			var prjs = ParseSolution();
			LoadReferences(prjs);

			if(Project=="*")
			{
				foreach (var p in prjs)
					AddVersion(p);
				return 0;
			}

			var target = prjs.Single(p => p.Name == Project);

			
			var q = new Queue<ProjectFile>();
			q.Enqueue(target);
			var processed = new HashSet<string>();

			while (q.Count > 0)
			{
				var f = q.Dequeue();

				AddVersion(f);

				if (f.UserProjects!=null)
					foreach(var u in f.UserProjects.Where(p=>!processed.Contains(p.FilePath)))
					{
						processed.Add(u.FilePath);
						q.Enqueue(u);
					}

			}

			return 0;
		}
	}
	
	class Program
    {
		
		static void Main(string[] args)
        {
			Parser.Default.ParseArguments<
				AddVersionOptions, 
				ListProjectsOptions
				>(args)
			   .MapResult(
				 (AddVersionOptions opts) => opts.Execute(),
				 (ListProjectsOptions opts) => opts.Execute(),
				  errs =>
				  {
					  Console.WriteLine(string.Join("\n", errs.Select(e => e.ToString())));
					  return 0;
				  }
				 );

		}
	}
}
