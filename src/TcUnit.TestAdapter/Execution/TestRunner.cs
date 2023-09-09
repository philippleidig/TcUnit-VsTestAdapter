using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
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
using TcUnit.TestAdapter.Discovery;
using TcUnit.TestAdapter.Models;
using TcUnit.TestAdapter.RunSettings;
using TwinCAT.Ads;
using static System.Net.Mime.MediaTypeNames;

namespace TcUnit.TestAdapter.Execution
{
    public class TestRunner : ITestRunner
    {
        private readonly XUnitTestResultParser testResultParser = new XUnitTestResultParser();

        public IEnumerable<TestCase> DiscoverTests(string source, ITestCaseFilter testCaseFilter, IMessageLogger logger)
        {
            return ListTestCasesInSource(source)
                .Where(t => testCaseFilter.MatchTestCase(t));
        }

        public TestRun RunTests(string source, IEnumerable<TestCase> tests, TestSettings runSettings, IMessageLogger logger)
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

            PrepareTargetForTestRun(targetRuntime, xaeProject, cleanUpBeforeTestRun);

            PerformTestRunOnTarget(targetRuntime);

            var testResults = CollectTestRunResultsFromTarget(targetRuntime);

            if (cleanUpAfterTestRun)
            {
                CleanUpTargetAfterTestRun(targetRuntime);
            }
            
            stopWatch.Stop();

            var testRunDuration = stopWatch.Elapsed;

            var testRunResults = PopulateTestRunResults(tests, testResults);


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

        private void PrepareTargetForTestRun (TargetRuntime target, TwinCATXAEProject project, bool cleanUpBeforeTestRun)
        {
            target.SwitchToConfigMode();
            target.DownloadProject(project, cleanUpBeforeTestRun);
        }


        private IEnumerable<TestResult> PopulateTestRunResults (IEnumerable<TestCase> testCases, IEnumerable<TestCaseResult> testResults)
        {
            var testRunResults = new List<TestResult>();

            foreach (var test in testCases)
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

            return testRunResults;
        }
        private void CleanUpTargetAfterTestRun (TargetRuntime target)
        {
            target.SwitchToConfigMode();
            target.CleanUpBootFolder();
        }

        private void PerformTestRunOnTarget(TargetRuntime target)
        {
            var testRunTimeout = new Stopwatch();
            testRunTimeout.Start();

            target.SwitchToRunMode();

            var isTestRunFinished = false;

            while (!isTestRunFinished || ( testRunTimeout.ElapsedMilliseconds < 30000 ))
            {
                isTestRunFinished = target.IsTestRunFinished();
                Thread.Sleep(500);
            }

            Thread.Sleep(1000);
        }

        private IEnumerable<TestCaseResult> CollectTestRunResultsFromTarget (TargetRuntime target)
        {
            target.UploadTestRunResults(@"C:\Temp\tcunit_testresults.xml");
            return testResultParser.ParseFromFile(@"C:\Temp\testresults.xml");
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
            var unitTests = plcProject.FunctionBlocks
                .Where(pou => pou.Extends == TestAdapter.TestSuiteBaseClass);

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
