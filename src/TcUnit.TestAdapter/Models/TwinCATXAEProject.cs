using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TcUnit.TestAdapter.Common;

namespace TcUnit.TestAdapter.Models
{
    public class TwinCATXAEProject
    {
        private List<TwinCATBootProject> _bootProjects;
        public IEnumerable<TwinCATBootProject> BootProjects => _bootProjects;

        public bool IsProjectPreBuild => BootProjects.Count() > 0;

        private TwinCATXAEProject(string filepath)
        {
            _bootProjects = new List<TwinCATBootProject>();

            if (Path.GetExtension(filepath) != ".tsproj")
            {
                throw new ArgumentOutOfRangeException();
            }

            var projectFolder = Path.GetDirectoryName(filepath);
            var bootProjectFolder = Path.Combine(projectFolder, "_Boot");

            if (!Directory.Exists(bootProjectFolder))
            {
                return;
            }

            var targetPlattforms = Directory.GetDirectories(bootProjectFolder);

            foreach (var plattform in targetPlattforms)
            {
                var plattformName = Path.GetFileName(plattform);

                if (Common.RTOperatingSystem.AvailableRTPlattforms.Values.Contains(plattformName))
                {
                    var bootProjectPath = Path.Combine(bootProjectFolder, plattform);
                    var bootProject = TwinCATBootProject.ParseFromLocalProjectBuildFolder(bootProjectPath);
                    _bootProjects.Add(bootProject);
                }
            }
        }

        public static TwinCATXAEProject Load(string filepath)
        {
            return new TwinCATXAEProject(filepath);
        }

    }
}
