using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TcUnit.TestAdapter.Common;
using TcUnit.TestAdapter.Services;
using TwinCAT;
using TwinCAT.Ads;

namespace TcUnit.TestAdapter.Models
{
    public class TargetRuntime
    {
        private readonly TwinCATSystemService systemService;
        private readonly TwinCATEventListener eventListener;

        public TargetInfo Info { get; set; }
        public AmsNetId AmsNetId { get; set; }
        public Tuple<int, int> RealtimeSettings { get; set; }

        public TargetRuntime(AmsNetId target)
        {
            AmsNetId = target;
            systemService = new TwinCATSystemService(target);
            eventListener = new TwinCATEventListener(target);

            Info = systemService.GetDeviceInfo();
            RealtimeSettings = systemService.GetRealtimeSettings();
        }

        public bool Disconnect()
        {
            return systemService.Disconnect();
        }

        public bool IsReachable => systemService.IsReachable();
        public bool IsInRunMode => systemService.IsInRunMode();
        public bool IsTestRunFinished => systemService.FileExistsInBootFolder(TestAdapter.TestResultPath);

        public void SwitchToConfigMode(TimeSpan timeout)
        {
            systemService.SwitchRuntimeState(AdsStateCommand.Reconfig, timeout);
        }

        public void SwitchToRunMode(TimeSpan timeout)
        {
            var errors = new List<AdsLogEntry>();

            eventListener.EventRaised += (s, e) =>
            {
                if(e.Event.LogLevel >= AdsLogLevel.Warning)
                {
                    errors.Add(e.Event);
                }
            };

            var succeeded = systemService.SwitchRuntimeState(AdsStateCommand.Reset, timeout);

            if (!succeeded)
            {
                var messages = string.Join("\n", errors);
                throw new AdsException("Failed to set TwinCAT runtime to requested state. \n" + messages);
            } 
        }

        public void DownloadProject(TwinCATXAEProject xaeProject, bool cleanBeforeDownload = true)
        {
            if (!xaeProject.IsProjectPreBuild)
            {
                throw new ArgumentNullException("Project is not prebuild");
            }

            var bootProjects = xaeProject.BootProjects.Where(p => Common.RTOperatingSystem.AvailableRTPlattforms[p.RTPlatform] == Common.RTOperatingSystem.AvailableRTPlattforms[Info.RTPlatform]);

            if (!bootProjects.Any())
            {
                var availableBootProjects = string.Join(", ", xaeProject.BootProjects.Select(p => p.RTPlatform.ToString()));
                throw new ArgumentOutOfRangeException($"Could not find the corresponding TwinCAT target platform in the prebuild boot projects. Platform name: {Info.RTPlatform}. Available boot projects: {availableBootProjects}");
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
                CleanUpBootFolder();
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
                        //throw new Exception("Could not create /Boot/Plc folder on target.");
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

                var filePath = "Plc" + Common.RTOperatingSystem.GetSeperatorByOsName(Info.ImageOsName) + fileName;

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

        public void UploadTestRunResults(Stream stream)
        {
            systemService.UploadFileFromBootFolder(stream, TestAdapter.TestResultPath);
        }
    }
}
