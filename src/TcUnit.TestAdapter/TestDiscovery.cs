using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using TcUnit.TestAdapter.Models;

namespace TcUnit.TestAdapter
{
    [FileExtension(TestExecutor.FileExtension)]
    [DefaultExecutorUri(TestExecutor.ExecutorUriString)]
    public class TestDiscovery : ITestDiscoverer
    {
        // C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\Common7\IDE\Extensions\TestPlatform\Extensions
        // C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\Common7\IDE\Extensions\TestPlatform\Extensions
        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
        {
            try
            {
                var settingsProvider = discoveryContext.RunSettings.GetSettings("TcUnit") as SettingsProvider;
               // var settings = settingsProvider != null ? settingsProvider.Settings : new ChutzpahAdapterSettings();

                //var testOptions = new TestOptions
                //{
                //    MaxDegreeOfParallelism = settings.MaxDegreeOfParallelism,
                //    ChutzpahSettingsFileEnvironments = new ChutzpahSettingsFileEnvironments(settings.ChutzpahSettingsFileEnvironments)
                //};

                //var callback = new ParallelRunnerCallbackAdapter(new DiscoveryCallback(logger, discoverySink));
                //var testCases = testRunner.DiscoverTests(sources, testOptions, callback);

                var tests = GetTests(sources, discoverySink);
            }
            catch (Exception ex)
            {
                logger.SendMessage(TestMessageLevel.Error, ex.Message);
            }
        }

        internal static IEnumerable<TestCase> GetTests(IEnumerable<string> sources, ITestCaseDiscoverySink discoverySink)
        {
            var tests = new List<TestCase>();

            foreach (var source in sources)
            {

                var projectFolder = Path.GetDirectoryName(source);

                var plcProjectFiles = Directory.GetFiles(projectFolder, "*.plcproj", SearchOption.AllDirectories);

                foreach(var file in plcProjectFiles)
                {
                    var plcProject = PlcProject.ParseFromProjectFile(file);

                    foreach (var pou in plcProject.FunctionBlocks)
                    {
                        foreach(var method in pou.Methods)
                        {
                            var testName = plcProject.Name + "."+ pou.Name + "." +  method.Key;
          
                            var test = new TestCase(testName, TestExecutor.ExecutorUri, source);
                            test.LineNumber = 0;
                            test.CodeFilePath = pou.FilePath;
                            test.DisplayName = testName;

                            tests.Add(test);

                            if (discoverySink != null)
                            {
                                  discoverySink.SendTestCase(test);
                            }
                        }
                    }
                }
            }

            return tests;
        }
        private static IEnumerable<POU> GetUnitTestsFromProjectFile(PlcProject plcProject)
        {
            // TODO - get unit tests by base class?! or better use a attribute pragma?

            var unitTests = plcProject.FunctionBlocks; //.Where(pou => pou.Extends == "TcUnit.FB_TestSuite");

            foreach (var unitTest in unitTests) 
            {
                if(!IsUnitTestExcluded(unitTest, ""))
                {
                    yield return unitTest;
                }
            }
        }

        private static bool IsUnitTestExcluded(Models.POU pou, string excludeFilter)
        {
            if (excludeFilter == null || excludeFilter == "")
                return false;

            if (Regex.IsMatch(pou.Name, excludeFilter))
            {
                return true;
            }
            // {attribute 'Exclude'}
            return false;
        }
    }
}
