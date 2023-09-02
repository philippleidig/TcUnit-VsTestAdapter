using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TcUnit.TestAdapter.Models;
using TcUnit.TestAdapter.RunSettings;

namespace TcUnit.TestAdapter.Execution
{
    public class TestRunner
    {
        private readonly TestResultCollector resultCollecter = new TestResultCollector();



        public IEnumerable<TestCase> DiscoverTests(IEnumerable<string> sources, TestSettings settings)
        {
            return ListTestsInPlcProjects(sources);
        }

        public void RunTests(string target, bool cleanAfterRun = true)
        {

        }

        public void CollectTestResults()
        {

        }

        public bool IsTestRunFinished { get; }

        public void CleapUpAfterTestRun()
        {

        }

        internal static IEnumerable<TestCase> ListTestsInPlcProjects(IEnumerable<string> sources)
        {
            var tests = new List<TestCase>();

            foreach (var source in sources)
            {
                tests.AddRange(ListTestCasesInPlcProject(source));
            }

            return tests;
        }

        internal static IEnumerable<TestCase> ListTestCasesInPlcProject(string file)
        {
            var tests = new List<TestCase>();

            var plcProject = PlcProject.ParseFromProjectFile(file);

            foreach (var pou in plcProject.FunctionBlocks)
            {
                foreach (var method in pou.Methods)
                {
                    var testName = plcProject.Name + "." + pou.Name + "." + method.Key;

                    var test = new TestCase(testName, TestAdapter.ExecutorUri, file);
                    test.LineNumber = 0;
                    test.CodeFilePath = pou.FilePath;
                    test.DisplayName = testName;

                    tests.Add(test);
                }
            }

            return tests;
        }

        internal static bool IsTwinCATPlcProjectFile(string file)
        {
            return file.Contains(TestAdapter.PlcProjFileExtension);
        }

        private static IEnumerable<POU> GetUnitTestsFromProjectFile(PlcProject plcProject)
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

        private static bool IsUnitTestExcluded(POU pou, string excludeFilter)
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
