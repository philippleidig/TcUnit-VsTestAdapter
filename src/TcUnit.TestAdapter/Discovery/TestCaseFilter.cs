using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace TcUnit.TestAdapter.Discovery
{
    public class TestCaseFilter : ITestCaseFilter
    {
        const string DisplayNameString = "DisplayName";
        const string FullyQualifiedNameString = "FullyQualifiedName";

        readonly HashSet<string> knownTraits;
        List<string> supportedPropertyNames;
        readonly ITestCaseFilterExpression filterExpression;
        readonly bool successfullyGotFilter;
        readonly bool isDiscovery;

        readonly IMessageLogger logger;

        public TestCaseFilter(
            IRunContext runContext,
            IMessageLogger logger,
            string assemblyFileName,
            HashSet<string> knownTraits)
        {
            this.knownTraits = knownTraits;
            this.logger = logger;
            supportedPropertyNames = GetSupportedPropertyNames();
            successfullyGotFilter = GetTestCaseFilterExpression(runContext, assemblyFileName, out filterExpression);
        }

        public TestCaseFilter(IDiscoveryContext discoveryContext, IMessageLogger logger)
        {
            this.logger = logger;
            isDiscovery = true;
            // Traits are not known at discovery time because we load them from tests
            knownTraits = new HashSet<string>();
            supportedPropertyNames = GetSupportedPropertyNames();
            successfullyGotFilter = GetTestCaseFilterExpressionFromDiscoveryContext(discoveryContext, out filterExpression);
        }

        public bool MatchTestCase(TestCase testCase)
        {
            if (!successfullyGotFilter)
            {
                // Had an error while getting filter, match no testcase to ensure discovered test list is empty
                return false;
            }
            else if (filterExpression == null)
            {
                // No filter specified, keep every testcase
                return true;
            }

            return filterExpression.MatchTestCase(testCase, (p) => PropertyProvider(testCase, p));
        }

        public object PropertyProvider(
            TestCase testCase,
            string name)
        {
            // Special case for "FullyQualifiedName" and "DisplayName"
            if (string.Equals(name, FullyQualifiedNameString, StringComparison.OrdinalIgnoreCase))
                return testCase.FullyQualifiedName;
            if (string.Equals(name, DisplayNameString, StringComparison.OrdinalIgnoreCase))
                return testCase.DisplayName;

            // Traits filtering
            if (isDiscovery || knownTraits.Contains(name))
            {
                var result = new List<string>();

                foreach (var trait in GetTraits(testCase))
                    if (string.Equals(trait.Key, name, StringComparison.OrdinalIgnoreCase))
                        result.Add(trait.Value);

                if (result.Count > 0)
                    return result.ToArray();
            }

            return null;
        }

        bool GetTestCaseFilterExpression(
            IRunContext runContext,
            string assemblyFileName,
            out ITestCaseFilterExpression filter)
        {
            filter = null;

            try
            {
                filter = runContext.GetTestCaseFilter(supportedPropertyNames, s => null);
                return true;
            }
            catch (TestPlatformFormatException e)
            {
                logger.SendMessage(TestMessageLevel.Warning, $"{Path.GetFileNameWithoutExtension(assemblyFileName)}: Exception filtering tests: {e.Message}");
                return false;
            }
        }

        bool GetTestCaseFilterExpressionFromDiscoveryContext(
            IDiscoveryContext discoveryContext,
            out ITestCaseFilterExpression filter)
        {
            filter = null;

            if (discoveryContext is IRunContext runContext)
            {
                try
                {
                    filter = runContext.GetTestCaseFilter(supportedPropertyNames, s => null);
                    return true;
                }
                catch (TestPlatformException e)
                {
                    logger.SendMessage(TestMessageLevel.Warning, $"Exception filtering tests: {e.Message}");
                    return false;
                }
            }
            return false;
        }

        List<string> GetSupportedPropertyNames()
        {
            // Returns the set of well-known property names usually used with the Test Plugins (Used Test Traits + DisplayName + FullyQualifiedName)
            if (supportedPropertyNames == null)
            {
                supportedPropertyNames = knownTraits.ToList();
                supportedPropertyNames.Add(DisplayNameString);
                supportedPropertyNames.Add(FullyQualifiedNameString);
            }

            return supportedPropertyNames;
        }

        static IEnumerable<KeyValuePair<string, string>> GetTraits(TestCase testCase)
        {
            var traitProperty = TestProperty.Find("TestObject.Traits");
            if (traitProperty != null)
                return testCase.GetPropertyValue(traitProperty, Enumerable.Empty<KeyValuePair<string, string>>().ToArray());

            return Enumerable.Empty<KeyValuePair<string, string>>();
        }
    }
}
