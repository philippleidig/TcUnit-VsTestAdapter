using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcUnit.Core;

namespace TcUnit.TestAdapter
{
    public static class TestCaseExtensions
    {
       // public static TestCase ToVsTestCase(this Models.TestCase test)
       // {
       //     var normalizedPath = test.InputTestFile.ToLowerInvariant();
       //     var testCase = new TestCase(BuildFullyQualifiedName(test), AdapterConstants.ExecutorUri, normalizedPath)
       //     {
       //         CodeFilePath = normalizedPath,
       //         DisplayName = GetTestDisplayText(test),
       //         LineNumber = test.Line,
       //     };
       //
       //
       //     testCase.Traits.Add("Module", test.ModuleName);
       //     return testCase;
       // }
    }
}
