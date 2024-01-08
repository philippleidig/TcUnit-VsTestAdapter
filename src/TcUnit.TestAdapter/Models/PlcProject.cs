using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using TcUnit.TestAdapter.Schemas;

namespace TcUnit.TestAdapter.Models
{
	public class PlcProject
    {
		private readonly Schemas.Project _projectFile;

		public string CompletePathInFileSystem { get; set; }
        public string FolderPathInFileSystem { get; set; }
        public string FileNameInFileSystem { get; set; }
        public string Name { get; set; }

        public List<FunctionBlock_POU> FunctionBlocks { get; set; } = new List<FunctionBlock_POU>();
        public List<PlcLibraryReference> References { get; set; } = new List<PlcLibraryReference> { };
        public List<TwinCATModuleClass> ModuleClasses { get; set; } = new List<TwinCATModuleClass> { };

        private PlcProject(string plcProjectFile, Project project)
        {
			_projectFile = project;

			CompletePathInFileSystem = plcProjectFile;
            FolderPathInFileSystem = Path.GetDirectoryName(plcProjectFile);
            FileNameInFileSystem = new DirectoryInfo(plcProjectFile).Name.ToString();
            Name = FileNameInFileSystem.Replace(".plcproj", "");

            ParsePOUs();
            ParseTMCs();
			ParseLibraries();
		}

        public static PlcProject Parse(string filepath)
        {
			if (!filepath.Contains(".plcproj"))
			{
				throw new ArgumentOutOfRangeException();
			}

			if (!File.Exists(filepath))
            {
                throw new FileNotFoundException();
            }

			XmlSerializer serializer = new XmlSerializer(typeof(Project));
			StreamReader reader = new StreamReader(filepath);
			Project project = (Project)serializer.Deserialize(reader);
			reader.Close();

			return new PlcProject(filepath, project);
        }

		private TwinCATModuleClass ParseTMC(string filepath)
		{
			if (!filepath.EndsWith(".tmc"))
			{
				return null;
			}

			var tmcFilePath = Path.Combine(FolderPathInFileSystem, filepath);

			if (!File.Exists(tmcFilePath))
			{
				return null;
			}

			return TwinCATModuleClass.ParseFromFilePath(tmcFilePath);
		}

        private void ParseTMCs()
        {
			ModuleClasses.Clear();

			foreach (var itemGroup in _projectFile.ItemGroup)
			{
				if (itemGroup.None != null)
				{
					var tmc = ParseTMC(itemGroup.None.Include);
					ModuleClasses.Add(tmc);
				}

				// manually added *.tmc files appear as content
				if (itemGroup.Content != null)
				{
					var tmc = ParseTMC(itemGroup.Content.Include);
					ModuleClasses.Add(tmc);
				}
			}
        }

        private void ParsePOUs()
        {
			FunctionBlocks.Clear();

			foreach (var itemGroup in _projectFile.ItemGroup)
			{
				if (itemGroup.Compile is null)
				{
					continue;
				}

				foreach (var pou in itemGroup.Compile)
				{
					if (!pou.Include.Contains(".TcPOU"))
					{
						continue;
					}

					var pouFilePath = Path.Combine(FolderPathInFileSystem, pou.Include);

					if (File.Exists(pouFilePath))
					{
						var functionBlock = FunctionBlock_POU.Load(pouFilePath);
						FunctionBlocks.Add(functionBlock);
					}
				}
			}
        }

		private PlcLibraryReference ParsePlcLibrary(ProjectItemGroupPlaceholderReference library)
		{
			var libraryName = library.Include;

			var reference = new PlcLibraryReference
			{
				Name = libraryName.Split(',')[0],
			};

			if (library.Parameters != null)
			{
				foreach (var parameter in library.Parameters)
				{
					var key = parameter.Key;
					var value = parameter.Value;

					reference.Parameters.Add(key, value);
				}
			}

			return reference;
		}


		private PlcLibraryReference ParsePlcLibrary(ProjectItemGroupLibraryReference library)
		{
			var libraryName = library.Include;

			var reference = new PlcLibraryReference
			{
				Name = libraryName.Split(',')[0],
			};

			if (library.Parameters != null)
			{
				foreach (var parameter in library.Parameters)
				{
					var key = parameter.Key;
					var value = parameter.Value;

					reference.Parameters.Add(key, value);
				}
			}

			return reference;
		}

		private void ParseLibraries ()
		{
			References.Clear();

			foreach (var itemGroup in _projectFile.ItemGroup)
			{
				if (itemGroup.PlaceholderReference != null)
				{
					foreach (var library in itemGroup.PlaceholderReference)
					{
						var reference = ParsePlcLibrary(library);
						References.Add(reference);
					}
				}

				if (itemGroup.LibraryReference != null)
				{
					foreach (var library in itemGroup.LibraryReference)
					{
						var reference = ParsePlcLibrary(library);
						References.Add(reference);
					}
				}
			}
		}
    }
}
