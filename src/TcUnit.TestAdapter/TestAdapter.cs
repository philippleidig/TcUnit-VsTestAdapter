using System;
using TcUnit.TestAdapter.RunSettings;

namespace TcUnit.TestAdapter
{
    public static class TestAdapter
    {
        public const string ExecutorUriString = "executor://TcUnitTestExecutor";
        public static readonly Uri ExecutorUri = new Uri(ExecutorUriString);

        public const string PlcProjFileExtension = ".plcproj";
        public const string TsProjFileExtension = ".tsproj";

        public const string TestResultPath = @"tcunit_testresults.xml"; 

        public const string RunSettingsName = "TcUnit";

        public const string TestSuiteBaseClass = "TcUnit.FB_TestSuite";

        public const string DefaultTargetRuntime = "127.0.0.1.1.1";
        public const bool DefaultCleanUpAfterTestRun = true;


    }
}
