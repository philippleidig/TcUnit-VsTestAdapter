using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using static System.Net.WebRequestMethods;

namespace TcUnit.TestAdapter.Discovery
{
    public class TestCaseFilter 
    {
        private readonly Dictionary<string, TestProperty> _supportedProperties;

        private readonly ITestCaseFilterExpression _filterExpression;
        private readonly bool _filterHasError;

        public TestCaseFilter(IDiscoveryContext discoveryContext, IMessageLogger logger)
        {
            _supportedProperties = new Dictionary<string, TestProperty>(StringComparer.OrdinalIgnoreCase)
            {
                [TestCaseProperties.FullyQualifiedName.Label] = TestCaseProperties.FullyQualifiedName,
                [TestCaseProperties.DisplayName.Label] = TestCaseProperties.DisplayName
            };

            _filterExpression = GetFilterExpression(discoveryContext, logger, out _filterHasError);
        }

        public bool MatchTestCase(TestCase testCase)
        {
            if (_filterHasError)
            {
                return false;
            }
            else if (_filterExpression == null)
            {
                return true;
            }

            return _filterExpression.MatchTestCase(testCase, (p) => PropertyProvider(testCase, p));
        }

        internal object PropertyProvider(TestCase testCase, string name)
        {
           if (string.Equals(name, TestCaseProperties.FullyQualifiedName.Label, StringComparison.OrdinalIgnoreCase))
               return testCase.FullyQualifiedName;
           
           if (string.Equals(name, TestCaseProperties.DisplayName.Label, StringComparison.OrdinalIgnoreCase))
               return testCase.DisplayName;

           return null;
        }

        internal ITestCaseFilterExpression GetFilterExpression(IDiscoveryContext context, IMessageLogger logger, out bool filterHasError)
        {
            filterHasError = false;

            if (context == null)
            {
                return null;
            }

            ITestCaseFilterExpression filter = null;
            try
            {
                filter = context is IRunContext runContext
                    ? GetTestCaseFilterExpressionFromRunContext(runContext)
                    : GetTestCaseFilterFromDiscoveryContext(context, logger);
            }
            catch (TestPlatformFormatException ex)
            {
                filterHasError = true;
                logger.SendMessage(TestMessageLevel.Error, ex.Message);
            }

            return filter;
        }

        private ITestCaseFilterExpression GetTestCaseFilterExpressionFromRunContext(IRunContext runContext)
        {
            return runContext.GetTestCaseFilter(_supportedProperties.Keys, s => null);
        }

        private ITestCaseFilterExpression GetTestCaseFilterFromDiscoveryContext(IDiscoveryContext context, IMessageLogger logger)
        {
            try
            {
                var method = context.GetType().GetRuntimeMethod("GetTestCaseFilter", new[] { typeof(IEnumerable<string>), typeof(Func<string, TestProperty>) });
                return method?.Invoke(context, new object[] { _supportedProperties.Keys, (Func<TestCase, string, object>)PropertyProvider }) as ITestCaseFilterExpression;
            }
            catch (Exception ex)
            {
                if (ex is TargetInvocationException)
                {
                    throw ex.InnerException;
                }

                logger.SendMessage(TestMessageLevel.Warning, ex.Message);
            }

            return null;
        }
    }
}
