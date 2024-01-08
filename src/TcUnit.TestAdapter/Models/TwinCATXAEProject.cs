using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using TcUnit.TestAdapter.Schemas;

namespace TcUnit.TestAdapter.Models
{
    public class TwinCATXAEProject
    {
		private TcSmProject _projectFile;
		private List<TwinCATBootProject> _bootProjects;
        private List<PlcProject> _plcProjects;

        public IEnumerable<TwinCATBootProject> BootProjects => _bootProjects;
        public IEnumerable<PlcProject> PlcProjects => _plcProjects;
        public Tuple<int, int> RealtimeSettings { get; private set; }
        public bool IsProjectPreBuild => BootProjects.Count() > 0;
        public bool IsPlcProjectIncluded => PlcProjects.Count() > 0;
        public string ProjectFolder { get; private set; }
        public string FilePath { get; private set; }

        private TwinCATXAEProject(string filepath, TcSmProject project )
		{
			_projectFile = project;

            _bootProjects = new List<TwinCATBootProject>();
            _plcProjects = new List<PlcProject>();

            FilePath = filepath;
            ProjectFolder = Path.GetDirectoryName(filepath);

            ParseRealtimeSettings();
            ParseBootProjects();
            ParsePlcProjects();
        }

        public bool IsSuitableForTarget (TargetRuntime targetRuntime)
        {
            var projectRTSettings = RealtimeSettings;
            var targetRTSettings = targetRuntime.RealtimeSettings;

            // max CPU cores
            if (projectRTSettings.Item1 > targetRTSettings.Item1)
                return false;

            // isolated CPU cores
            if (projectRTSettings.Item1 != targetRTSettings.Item1)
                return false;

            return true;
        }

        private void ParseRealtimeSettings()
        {
			var systemSettings = _projectFile.Project?.System?.Settings;

			if(systemSettings == null)
			{
				return;
			}

			if (systemSettings.MaxCpusSpecified) 
			{
				int maxCpuCount = systemSettings.MaxCpus;
				int isolatedCpuCount = systemSettings.NonWinCpus;
				int sharedCpuCount = maxCpuCount - isolatedCpuCount;

				RealtimeSettings = new Tuple<int, int>(sharedCpuCount, isolatedCpuCount);
			}
        }

        private void ParseBootProjects()
        {
            var bootProjectFolder = Path.Combine(ProjectFolder, "_Boot");

            if (!Directory.Exists(bootProjectFolder))
            {
                return;
            }

            var targetPlatforms = Directory.GetDirectories(bootProjectFolder);

            foreach (var platform in targetPlatforms)
            {
                var platformName = Path.GetFileName(platform);

                if (Common.RTOperatingSystem.AvailableRTPlattforms.Values.Contains(platformName))
                {
                    var bootProjectPath = Path.Combine(bootProjectFolder, platformName);
                    var bootProject = TwinCATBootProject.Load(bootProjectPath);
                    _bootProjects.Add(bootProject);
                }
            }
        }

        private void ParsePlcProjects()
        {
			_plcProjects.Clear();

			foreach (var plcProject in _projectFile.Project.Plc.Project)
			{
				var plcProjFilePath = "";

				if (!string.IsNullOrEmpty(plcProject.PrjFilePath) 
					&& string.IsNullOrEmpty(plcProject.File))
				{
					plcProjFilePath = plcProject.PrjFilePath; 
				}
				else if (string.IsNullOrEmpty(plcProject.PrjFilePath) 
					&& !string.IsNullOrEmpty(plcProject.File))
				{
					// independent project file - xti

					string xtiFilePath = new DirectoryInfo(FilePath).Parent.FullName.ToString() + "\\_Config\\PLC\\" + plcProject.File;

					StreamReader xtiReader = new StreamReader(xtiFilePath);
					XmlSerializer xtiSerializer = new XmlSerializer(typeof(TcSmItem));
					TcSmItem xti = (TcSmItem)xtiSerializer.Deserialize(xtiReader);
					xtiReader.Close();

					TcSmItemTypeProject project = (TcSmItemTypeProject)xti.Items[0];
					plcProjFilePath = !string.IsNullOrEmpty(project.PrjFilePath) ? project.PrjFilePath : "";
					plcProjFilePath = !string.IsNullOrEmpty(plcProjFilePath) ? plcProjFilePath.Replace("..\\", "") : "";				
				}

				plcProjFilePath = Path.Combine(ProjectFolder, plcProjFilePath);

				if (File.Exists(plcProjFilePath))
				{
					_plcProjects.Add(PlcProject.Parse(plcProjFilePath));
				}
			}
        }

        public static TwinCATXAEProject Load(string filepath)
        {
			if (Path.GetExtension(filepath) != TestAdapter.TsProjFileExtension)
			{
				throw new ArgumentOutOfRangeException(nameof(filepath));
			}

			if (!File.Exists(filepath))
			{
				throw new FileNotFoundException();
			}

			XmlSerializer serializer = new XmlSerializer(typeof(TcSmProject));
			StreamReader reader = new StreamReader(filepath);
			TcSmProject project = (TcSmProject)serializer.Deserialize(reader);
			reader.Close();

			return new TwinCATXAEProject(filepath, project);
        }

    }
}
