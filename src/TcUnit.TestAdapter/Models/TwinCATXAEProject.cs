using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace TcUnit.TestAdapter.Models
{
    public class TwinCATXAEProject
    {
        private List<TwinCATBootProject> _bootProjects;
        private List<PlcProject> _plcProjects;

        public IEnumerable<TwinCATBootProject> BootProjects => _bootProjects;
        public IEnumerable<PlcProject> PlcProjects => _plcProjects;

        public Tuple<int, int> RealtimeSettings { get; private set; }

        public bool IsProjectPreBuild => BootProjects.Count() > 0;
        public bool IsPlcProjectIncluded => PlcProjects.Count() > 0;
        public string ProjectFolder { get; private set; }
        public string FilePath { get; private set; }

        private TwinCATXAEProject(string filepath)
        {
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
            var projectFile = XDocument.Load(FilePath);
            var realtimeSettings = projectFile.Element("TcSmProject")
                                                .Element("Project")
                                                .Element("System")
                                                .Element("Settings");

            if (realtimeSettings != null)
            {
                int.TryParse(realtimeSettings.Attribute("MaxCpus")?.Value, out int maxCpuCount);
                int.TryParse(realtimeSettings.Attribute("NonWinCpus")?.Value, out int isolatedCpuCount);
                var sharedCpuCount = maxCpuCount - isolatedCpuCount;

                RealtimeSettings = new Tuple<int,int>(maxCpuCount, isolatedCpuCount);
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

            var plcProjectFiles = Directory.GetFiles(ProjectFolder, "*.plcproj", SearchOption.AllDirectories);

            foreach (var projectFile in plcProjectFiles)
            {
                var plcProject = PlcProject.ParseFromProjectFile(projectFile);
                _plcProjects.Add(plcProject);
            }
        }

        public static TwinCATXAEProject Load(string filepath)
        {
            if (Path.GetExtension(filepath) != TestAdapter.TsProjFileExtension)
            {
                throw new ArgumentOutOfRangeException(nameof(filepath));
            }

            return new TwinCATXAEProject(filepath);
        }

    }
}
