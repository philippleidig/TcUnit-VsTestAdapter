using System.IO;
using System.Linq;
using TcUnit.TestAdapter.Common;

namespace TcUnit.TestAdapter.Models
{
    public class TwinCATBootProject
    {
        public string CurrentConfigPath { get; private set; } = string.Empty;
        public string TargetPlattform { get; private set; } = string.Empty;
        public RTPlatform RTPlatform { get; private set; }
        public string ProjectName { get; private set; } = string.Empty;
        public string PlcProjectPath { get; private set; } = string.Empty;
        public string ProjectPath { get; private set; } = string.Empty;
        public bool IsPlcProjectIncluded { get; private set; }

        private TwinCATBootProject()
        {

        }

        public static TwinCATBootProject Load (string folderPath)
        {
            if(!Directory.Exists(folderPath))
            {
                throw new DirectoryNotFoundException(folderPath);
            }

            var currentConfig = Path.Combine(folderPath, "CurrentConfig.xml");

            if (!File.Exists(currentConfig))
            {
                throw new FileNotFoundException("CurrentConfig.xml");
            }

            var bootProject = new TwinCATBootProject();
            var targetPlattform = Path.GetFileName(folderPath);

            bootProject.PlcProjectPath = Path.Combine(folderPath, "Plc");
            bootProject.IsPlcProjectIncluded = Directory.Exists(bootProject.PlcProjectPath);
            bootProject.CurrentConfigPath = currentConfig;
            bootProject.TargetPlattform = targetPlattform;
            bootProject.RTPlatform = RTOperatingSystem.GetRTPlatformFromBuildConfiguration(targetPlattform);

            return bootProject;
        }
    }
}
