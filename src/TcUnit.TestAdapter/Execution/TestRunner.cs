using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using TcUnit.TestAdapter.Abstractions;
using TcUnit.TestAdapter.Common;
using TcUnit.TestAdapter.Extensions;
using TcUnit.TestAdapter.Models;
using TcUnit.TestAdapter.RunSettings;
using TwinCAT.Ads;

namespace TcUnit.TestAdapter.Execution
{
    public class TestRunner : ITestRunner
    {
        private readonly XUnitTestResultParser testResultParser = new XUnitTestResultParser();

        public IEnumerable<TestCase> DiscoverTests(TwinCATXAEProject project, IMessageLogger logger)
        {
            var tests = new List<TestCase>();

            foreach (var plcProject in project.PlcProjects)
            {
                try
                {
                    CheckPlcProjectSuitableForTestRun(plcProject);
                }
                catch (Exception e)
                {
                    logger.LogInformation($"Found non suitable PLC project for {plcProject.Name}: {e.Message}");
                    continue;
                }

                // assume single module class file
                var tmc = plcProject.ModuleClasses[0];

                // find datatypes of type FB_TestSuite
                var testSuiteDatatypes = tmc.DataTypes
                    .Where(dataType => dataType.ExtendsType == TestAdapter.TestSuiteBaseClass);

                // get a list of names
                var testSuiteDatatypeNames = testSuiteDatatypes.Select(dataType => dataType.Name).ToList();

                // find datatypes with subitems matching FB_TestSuite base type
                var testSuiteParentDatatypes = tmc.DataTypes
                    .Where(dataType => dataType.SubItems.Any(subItem => testSuiteDatatypeNames.Contains(subItem.Type)));

                // find PLC instances
                var modules = tmc.Modules
                    .Where(module => module.TcSmClass == TestAdapter.PlcObjClass);

                foreach (var module in modules)
                {
                    // each PLC may have multiple tasks
                    foreach (var dataArea in module.DataAreas)
                    {
                        // find symbols matching the datatypes found above
                        var symbols = dataArea.Symbols
                            .Where(symbol => testSuiteDatatypes.Any(testSuiteDatatype => testSuiteDatatype.Name == symbol.BaseType));

                        foreach (var symbol in symbols)
                        {
                            var symbolInstancePath = symbol.Name;

                            // try to match with testsuite POUs
                            var testSuiteFB = plcProject.FunctionBlocks
                                .Where(pou => pou.Name == symbol.BaseType)
                                .FirstOrDefault();

                            if (testSuiteFB == null)
                            {
                                logger.LogWarning( $"TestSuite {symbol.BaseType} not found in PLC project {plcProject.Name}");
                                continue;
                            }

                            var testSuite = TestSuite.ParseFromFunctionBlock(testSuiteFB);
                            foreach (var testMethod in testSuite.Tests)
                            {
                                var testName = symbolInstancePath + "." + testMethod.Name;

                                var test = new TestCase(testName, TestAdapter.ExecutorUri, project.FilePath);
                                test.LineNumber = 0;
                                test.CodeFilePath = testSuiteFB.FilePath;
                                test.DisplayName = testName;

                                tests.Add(test);
                            }
                        }

                        // find symbols matching the parent datatype
                        var parentSymbols = dataArea.Symbols
                            .Where(symbol => testSuiteParentDatatypes.Any(testSuiteParentDatatype => testSuiteParentDatatype.Name == symbol.BaseType));

                        foreach (var parentSymbol in parentSymbols)
                        {
                            var parentSymbolInstancePath = parentSymbol.Name;

                            // parent datatype
                            var parentDataType = testSuiteParentDatatypes
                                .Where(dataType => dataType.Name == parentSymbol.BaseType)
                                .FirstOrDefault();

                            // find all the subitems of the parent datatype where the sub item exists in the list of test suite datatypes
                            var subItems = parentDataType.SubItems
                                .Where(subItem => testSuiteDatatypeNames.Contains(subItem.Type));

                            // try to match subitem with testsuite POUs
                            foreach (var subItem in subItems)
                            {
                                var testSuiteFB = plcProject.FunctionBlocks
                                    .Where(pou => pou.Name == subItem.Type)
                                    .FirstOrDefault();

                                if (testSuiteFB == null)
                                {
                                    logger.LogWarning($"TestSuite {subItem.Type} not found in PLC project {plcProject.Name}");
                                    continue;
                                }

                                var testSuite = TestSuite.ParseFromFunctionBlock(testSuiteFB);
                                foreach (var testMethod in testSuite.Tests)
                                {
                                    var testName = parentSymbolInstancePath + "." + subItem.Name + "." + testMethod.Name;

                                    var test = new TestCase(testName, TestAdapter.ExecutorUri, project.FilePath);
                                    test.LineNumber = 0;
                                    test.CodeFilePath = testSuiteFB.FilePath;
                                    test.DisplayName = testName;

                                    tests.Add(test);
                                }
                            }
                        }
                    }
                }
            }

            return tests;
        }

        public TestRun RunTests(TwinCATXAEProject project, IEnumerable<TestCase> tests, TestSettings runSettings, IMessageLogger logger)
        {

            if (!project.IsProjectPreBuild)
            {
                throw new Exception("TwinCAT XAE project is not pre build.");
            }

            if (!project.IsPlcProjectIncluded)
            {
                throw new Exception("TwinCAT XAE project does not contain at least one PLC project.");
            }

            var target = AmsNetId.Parse(runSettings.Target);
            var cleanUpAfterTestRun = runSettings.CleanUpAfterTestRun;
            var cleanUpBeforeTestRun = true;

            if(AmsNetId.LocalHost != target && AmsNetId.Local != target)
            {
                var adsRemoteRoutes = TwinCATEnvironment.GetRemoteRoutes();
                var targetRoute = adsRemoteRoutes.Where(r => r.NetId.Equals(target));

                if (!targetRoute.Any())
                    throw new Exception("No ADS route to target found.");
            }

            var targetRuntime = new TargetRuntime(target);

            if (!targetRuntime.IsReachable)
                throw new Exception("Target runtime is not reachable.");

            if (!project.IsSuitableForTarget(targetRuntime))
                throw new Exception("XAE project is not suitable for target runtime (e.g. different RT settings).");

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            IEnumerable<TestCaseResult> testResults = null;

            try
            {

                PrepareTargetForTestRun(targetRuntime, project, cleanUpBeforeTestRun);

                PerformTestRunOnTarget(targetRuntime);

                testResults = CollectTestRunResultsFromTarget(targetRuntime);
            }
            finally 
            {
                if (cleanUpAfterTestRun)
                {
                    CleanUpTargetAfterTestRun(targetRuntime);
                }

                targetRuntime.Disconnect();
            }

            stopWatch.Stop();

            var testRunDuration = stopWatch.Elapsed;

            var testRunResults = PopulateTestRunResults(tests, testResults);


            var testRun = new TestRun
            {
                Results = testRunResults,
                Context = new TestRunContext
                {
                    TwinCATVersion = targetRuntime.Info.TwinCATVersion.ToString(),
                    Target = target,
                    OperatingSystem = targetRuntime.Info.ImageOsName,
                    Duration = testRunDuration,
                    BuildConfiguration = RTOperatingSystem.GetBuildConfigurationFromRTPlatform(targetRuntime.Info.RTPlatform)
                }
            };

            return testRun;
        }

        private void CheckPlcProjectSuitableForTestRun(PlcProject project)
        {
            var tcUnitLibraryReference = project.References.Where(r => r.Name == "TcUnit");

            if (!tcUnitLibraryReference.Any())
            {
                throw NonSuitablePLCProjectException("TcUnit library not referenced");
            }

            var library = tcUnitLibraryReference.FirstOrDefault();

            var xUnitEnablePublish = "";

            if (!library.TryGetParameterValue("XUNITENABLEPUBLISH", out xUnitEnablePublish))
            {
                throw NonSuitablePLCProjectException("XUNITENABLEPUBLISH parameter not found");
            }

            if (string.IsNullOrEmpty(xUnitEnablePublish) || !xUnitEnablePublish.Equals("TRUE"))
            {
                throw NonSuitablePLCProjectException("XUNITENABLEPUBLISH parameter not set to TRUE");
            }

            var xUnitFilePath = "";

            if (!library.TryGetParameterValue("XUNITFILEPATH", out xUnitFilePath))
            {
                throw NonSuitablePLCProjectException("XUNITFILEPATH parameter not found");
            }

            if (string.IsNullOrEmpty(xUnitFilePath))
            {
                throw NonSuitablePLCProjectException("XUNITFILEPATH parameter not set");
            }
        }

        private void PrepareTargetForTestRun(TargetRuntime target, TwinCATXAEProject project, bool cleanUpBeforeTestRun)
        {
            target.SwitchToConfigMode(TimeSpan.FromSeconds(10));
            target.DownloadProject(project, cleanUpBeforeTestRun);
        }

        private IEnumerable<TestResult> PopulateTestRunResults(IEnumerable<TestCase> testCases, IEnumerable<TestCaseResult> testResults)
        {
            var testRunResults = new List<TestResult>();

            foreach (var test in testCases)
            {
                var testResult = new TestResult(test);

                try {
                    var result = testResults.Where(r => r.FullyQualifiedName == test.FullyQualifiedName).First();

                    testResult.Outcome = result.Outcome;
                    testResult.Duration = result.Duration;
                    testResult.ErrorMessage = result.ErrorMessage;
                }
                catch
                {
                    testResult.Outcome = TestOutcome.Skipped;
                }

                testRunResults.Add(testResult);
            }

            return testRunResults;
        }
        private void CleanUpTargetAfterTestRun(TargetRuntime target)
        {
            target.SwitchToConfigMode(TimeSpan.FromSeconds(10));
            target.CleanUpBootFolder();
        }

        private void PerformTestRunOnTarget(TargetRuntime target)
        {
            target.SwitchToRunMode(TimeSpan.FromSeconds(10));

            var isSuccess = RetryUntilSuccessOrTimeout(() => target.IsTestRunFinished(), TimeSpan.FromSeconds(10));

            if (!isSuccess)
            {
                throw new Exception();
            }
        }

        public bool RetryUntilSuccessOrTimeout(Func<bool> task, TimeSpan timeSpan)
        {
            bool success = false;
            int elapsed = 0;

            while ((!success) && (elapsed < timeSpan.TotalMilliseconds))
            {
                Thread.Sleep(500);
                elapsed += 500;
                success = task();
            }

            return success;
        }

        private IEnumerable<TestCaseResult> CollectTestRunResultsFromTarget(TargetRuntime target)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                target.UploadTestRunResults(ms);
                return testResultParser.Parse(ms);
            }
        }

        public Exception NonSuitablePLCProjectException(string message)
        {
            throw new Exception(message);
        }
    }
}
