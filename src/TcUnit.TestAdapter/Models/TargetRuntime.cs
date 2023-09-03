using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TcUnit.TestAdapter.Common;
using TwinCAT.Ads;

namespace TcUnit.TestAdapter.Models
{
    public class TargetRuntime
    {
        private readonly SystemService systemService;
        public TargetInfo Info { get; set; }

        public AmsNetId AmsNetId { get; set; }


        public TargetRuntime(string target)
        {
            AmsNetId = new AmsNetId(target);
            systemService = new SystemService(target);
            Info = systemService.GetDeviceInfo();
        }

        public bool IsReachable => systemService.IsReachable();

        public void SwitchToConfigMode()
        {
            systemService.SwitchRuntimeState(AdsState.Reconfig);
        }

        public void SwitchToRunMode()
        {
            systemService.SwitchRuntimeState(AdsState.Reset);
        }


        public void DownloadProject(TwinCATXAEProject xaeProject, bool cleanBeforeDownload = true)
        {
            if (!xaeProject.IsProjectPreBuild)
            {
                throw new ArgumentNullException("Project is not prebuild");
            }

            var bootProjects = xaeProject.BootProjects.Where(p => p.RTPlatform.Equals(Info.RTPlatform));

            if (!bootProjects.Any())
            {
                throw new ArgumentOutOfRangeException("Could not find the corresponding TwinCAT target plattform in the prebuild boot projects: " + Info.RTPlatform.ToString());
            }
            var bootProject = bootProjects.FirstOrDefault();

            if (!File.Exists(bootProject.CurrentConfigPath))
            {
                throw new FileNotFoundException("CurrentConfig.xml does not exist in the prebuild project folder");
            }

            if (Path.GetFileName(bootProject.CurrentConfigPath) != "CurrentConfig.xml")
            {
                throw new ArgumentOutOfRangeException("Wrong name of CurrentConfig.xml");
            }
         
            if (cleanBeforeDownload)
            {
                systemService.CleanUpBootDirectory(Info.ImageOsName);
            }

            DownloadBootProject(bootProject);
        }

        private void DownloadBootProject(TwinCATBootProject bootProject)
        {
            DownloadCurrentConfig(bootProject.CurrentConfigPath);

            // Download PLC project
            if (bootProject.IsPlcProjectIncluded)
            {
                DownloadPlcProject(bootProject.PlcProjectPath);
            }

        }

        public void DownloadCurrentConfig(string path)
        {
            var fileName = Path.GetFileName(path);

            try
            {
                // Download current config
                if (!systemService.DownloadFileToBootFolder(path, fileName))
                {
                    throw new Exception("Download CurrentConfig.xml to target failed");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Download CurrentConfig.xml to target failed with reason: " + ex.Message);
            }
        }

        public void DownloadPlcProject(string path)
        {
            // Ensure target boot folder structure
            try
            {
                if (!systemService.DirectoryExistsInBootFolder("Plc"))
                {
                    if (!systemService.CreateDirectoryInBootFolder("Plc"))
                    {
                        throw new Exception("Could not create /Boot/Plc folder on target.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not create /Boot/Plc folder on target with reason: " + ex.Message);
            }


            var plcAppFiles = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly);

            if (plcAppFiles.Count() == 0)
            {
                throw new ArgumentOutOfRangeException("Could not find PLC project files in the specified folder.");
            }

            foreach (var file in plcAppFiles)
            {
                var fileName = Path.GetFileName(file);

                if (!File.Exists(file))
                {
                    throw new FileNotFoundException("File does not exist " + file);
                }

                var filePath = "Plc" + RTOperatingSystem.GetSeperatorByOsName(Info.ImageOsName) + fileName;

                try
                {
                    if (!systemService.DownloadFileToBootFolder(file, filePath))
                    {
                        throw new Exception("Download file (" + file + ") to target failed.");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Download file (" + file + ") to target failed with reason: " + ex.Message);
                }

            }
        }

        public void CleanUpBootFolder()
        {
            systemService.CleanUpBootDirectory(Info.ImageOsName);
        }

        public void UploadTestRunResults(string filePath)
        {
            systemService.UploadFileFromBootFolder(filePath, TestAdapter.TestResultPath);
        }

        public bool IsTestRunFinished()
        {
            return systemService.FileExistsInBootFolder(TestAdapter.TestResultPath);
        }
    }
}
