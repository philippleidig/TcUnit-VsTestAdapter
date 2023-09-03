using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TcUnit.TestAdapter.Abstractions;
using TcUnit.TestAdapter.Common;
using TcUnit.TestAdapter.Models;
using TcUnit.TestAdapter.RunSettings;
using TwinCAT.Ads;

namespace TcUnit.TestAdapter.Execution
{
    public class TestRunner : ITestRunner
    {
        private readonly XUnitTestResultParser testResultParser = new XUnitTestResultParser();

        public IEnumerable<TestCase> DiscoverTests(string source)
        {
            return ListTestCasesInSource(source);
        }

        public TestRun RunTests(string source, IEnumerable<TestCase> tests, TestSettings runSettings)
        {
            var xaeProject = TwinCATXAEProject.Load(source);

            if(!xaeProject.IsProjectPreBuild)
            {
                throw new Exception("TwinCAT XAE project is not pre build.");
            }

            //if (!xaeProject.IsPlcProjectIncluded)
            //{
            //    throw new Exception("TwinCAT XAE project doas not contain at least one PLC projects.");
            //}

            var target = runSettings.Target;
            var cleanUpAfterTestRun = runSettings.CleanUpAfterTestRun;
            var cleanUpBeforeTestRun = true;
           
            var targetRuntime = new TargetRuntime(target);

            if(!targetRuntime.IsReachable)
            {
                throw new Exception("Target is not connected");
            }

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            targetRuntime.DownloadProject(xaeProject, cleanUpBeforeTestRun);

            targetRuntime.SwitchToRunMode();

            var isTestRunFinished = false;

            while(!isTestRunFinished)
            {
                isTestRunFinished = targetRuntime.IsTestRunFinished();
                Thread.Sleep(500);
            }

            Thread.Sleep(2000);

            targetRuntime.UploadTestRunResults(@"C:\Temp\tcunit_testresults.xml");
            var testResults = testResultParser.ParseFromFile(@"C:\Temp\testresults.xml");

            targetRuntime.SwitchToConfigMode();
            targetRuntime.CleanUpBootFolder();

            stopWatch.Stop();

            var testRunDuration = stopWatch.Elapsed;

            var testRunResults = new List<TestResult>();

            foreach (var test in tests)
            {
                var testResult = new TestResult(test);

                var result = testResults.Where(r => r.FullyQualifiedName == test.FullyQualifiedName).First();

                if (result != null)
                {
                    testResult.Outcome = result.Outcome;
                    testResult.Duration = result.Duration;
                    testResult.ErrorMessage = result.ErrorMessage;
                }
                else
                {
                    testResult.Outcome = TestOutcome.Skipped;
                }

                testRunResults.Add(testResult);
            }


            var testRun = new TestRun
            {
                Results = testRunResults,
                Conditions = new TestRunConditions
                {
                    TwinCATVersion = targetRuntime.Info.TwinCATVersion.ToString(),
                    Target = target,
                    OperatingSystem = targetRuntime.Info.ImageOsName,
                    Duration = testRunDuration,
                    BuildConfiguration = RTOperatingSystem.GetBuildConfigurationFromRTPlatform( targetRuntime.Info.RTPlatform )
                }
            };   

            return testRun;
        }
        private IEnumerable<TestCase> ListTestCasesInSource(string source)
        {
            var tests = new List<TestCase>();

            var projectFolder = Path.GetDirectoryName(source);
            var plcProjectFiles = Directory.GetFiles(projectFolder, "*.plcproj", SearchOption.AllDirectories);

            foreach(var projectFile in plcProjectFiles)
            {
                var plcProject = PlcProject.ParseFromProjectFile(projectFile);

                foreach (var pou in GetUnitTestsFromProjectFile(plcProject))
                {
                    var testSuite = TestSuite.ParseFromFunctionBlock(pou);

                    foreach (var testMethod in testSuite.Tests)
                    {
                        var testName = plcProject.Name + "." + testMethod.Name;

                        var test = new TestCase(testName, TestAdapter.ExecutorUri, source);
                        test.LineNumber = 0;
                        test.CodeFilePath = pou.FilePath;
                        test.DisplayName = testName;

                        tests.Add(test);
                    }
                }
            }

            return tests;
        }

        private IEnumerable<FunctionBlock_POU> GetUnitTestsFromProjectFile(PlcProject plcProject)
        {
            // TODO - get unit tests by base class?! or better use a attribute pragma?

            var unitTests = plcProject.FunctionBlocks; //.Where(pou => pou.Extends == "TcUnit.FB_TestSuite");

            foreach (var unitTest in unitTests)
            {
                if (!IsUnitTestExcluded(unitTest, ""))
                {
                    yield return unitTest;
                }
            }
        }

        private bool IsUnitTestExcluded(FunctionBlock_POU pou, string excludeFilter)
        {
            if (excludeFilter == null || excludeFilter == "")
                return false;

            if (Regex.IsMatch(pou.Name, excludeFilter))
            {
                return true;
            }
            // {attribute 'TcUnit.Exclude'}
            return false;
        }
    }
}
