using System;
using System.IO;
using System.Linq;
using System.Xml;
using TcUnit.TestAdapter.Common;

namespace TcUnit.TestAdapter.Models
{
    public class TwinCATBootProject
    {
        public TwinCATBootProject()
        {

        }

        public string CurrentConfigPath { get; private set; } = string.Empty;
        public string TargetPlattform { get; private set; } = string.Empty;
        public RTPlatform RTPlatform { get; private set; }
        public string ProjectName { get; private set; } = string.Empty;
        public string PlcProjectPath { get; private set; } = string.Empty;
        public string ProjectPath { get; private set; } = string.Empty;
        public bool IsPlcProjectIncluded { get; private set; }

        public static TwinCATBootProject ParseFromLocalProjectBuildFolder(string folderPath)
        {
            var bootProject = new TwinCATBootProject();

            var targetPlattform = Path.GetFileName(folderPath);

            if (!File.Exists(Path.Combine(folderPath, "CurrentConfig.xml")))
            {
                throw new FileNotFoundException();
            }

            var currentConfig = Directory.GetFiles(folderPath, "CurrentConfig.xml", SearchOption.AllDirectories).FirstOrDefault();

            if (currentConfig == null || currentConfig == string.Empty)
            {
                throw new FileNotFoundException();
            }

            bootProject.PlcProjectPath = Path.Combine(folderPath, "Plc");
            bootProject.IsPlcProjectIncluded = Directory.Exists(bootProject.PlcProjectPath);

            bootProject.CurrentConfigPath = currentConfig;
            bootProject.TargetPlattform = targetPlattform;
            bootProject.RTPlatform = Common.RTOperatingSystem.GetRTPlatformFromBuildConfiguration(targetPlattform);

            return bootProject;
        }
    }
}
