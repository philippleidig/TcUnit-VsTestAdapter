using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TcUnit.TestAdapter.Common;
using static System.Net.Mime.MediaTypeNames;

namespace TcUnit.TestAdapter.Models
{
    public class TwinCATXAEProject
    {
        private List<TwinCATBootProject> _bootProjects;
        private List<PlcProject> _plcProjects;
        public IEnumerable<TwinCATBootProject> BootProjects => _bootProjects;
        public IEnumerable<PlcProject> PlcProjects => _plcProjects;

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

            ParseBootProjects();
            ParsePlcProjects();
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

                if (RTOperatingSystem.AvailableRTPlattforms.Values.Contains(platformName))
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
