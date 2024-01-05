using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Moq;
using TcUnit.TestAdapter.Discovery;

namespace TcUnit.TestAdapter.Tests
{
    [TestClass]
    public class TestCaseFilterTests
    {
        private IEnumerable<TestCase> GetDummyTestCases()
        {
            var testCase1 = new TestCase("TestNamespace.TestClass.TestMethod1", new Uri("executor://dummy"), "TestSource");
            var testCase2 = new TestCase("TestNamespace.TestClass.TestMethod2", new Uri("executor://dummy"), "TestSource");
            var testCase3 = new TestCase("TestNamespace.TestClass.TestMethod3", new Uri("executor://dummy"), "TestSource");

            return new List<TestCase> { testCase1, testCase2, testCase3 };
        }

        [TestMethod]
        public void MatchTestCase_NoFilterExpression_ReturnsTrue()
        {
            // Arrange
            var dummyTestCase = new TestCase("DummyTestCase", new Uri("executor://dummy"), "TestSource");
            var loggerMock = new Mock<IMessageLogger>();

            var filter = new TestCaseFilter(null, loggerMock.Object);

            // Act
            var result = filter.MatchTestCase(dummyTestCase);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void MatchTestCase_FilterExpressionMatches_ReturnsTrue()
        {
            // Arrange
            var dummyTestCase = new TestCase("DummyTestCase", new Uri("executor://dummy"), "TestSource");
            var loggerMock = new Mock<IMessageLogger>();
            var runContextMock = new Mock<IRunContext>();

            var filterExpressionMock = new Mock<ITestCaseFilterExpression>();
            filterExpressionMock.Setup(x => x.MatchTestCase(It.IsAny<TestCase>(), It.IsAny<Func<string, object>>()))
                                .Returns(true);

            runContextMock.Setup(x => x.GetTestCaseFilter(It.IsAny<IEnumerable<string>>(), It.IsAny<Func<string, TestProperty>>()))
                          .Returns(filterExpressionMock.Object);

            var filter = new TestCaseFilter(runContextMock.Object, loggerMock.Object);

            // Act
            var result = filter.MatchTestCase(dummyTestCase);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void MatchTestCase_FilterExpressionDoesNotMatch_ReturnsFalse()
        {
            // Arrange
            var dummyTestCase = new TestCase("DummyTestCase", new Uri("executor://dummy"), "TestSource");
            var loggerMock = new Mock<IMessageLogger>();
            var runContextMock = new Mock<IRunContext>();

            var filterExpressionMock = new Mock<ITestCaseFilterExpression>();
            filterExpressionMock.Setup(x => x.MatchTestCase(It.IsAny<TestCase>(), It.IsAny<Func<string, object>>()))
                                .Returns(false);

            runContextMock.Setup(x => x.GetTestCaseFilter(It.IsAny<IEnumerable<string>>(), It.IsAny<Func<string, TestProperty>>()))
                          .Returns(filterExpressionMock.Object);

            var filter = new TestCaseFilter(runContextMock.Object, loggerMock.Object);

            // Act
            var result = filter.MatchTestCase(dummyTestCase);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MatchTestCase_WithFilterExpression_MatchesOnlyFilteredCases()
        {
            // Arrange
            var dummyTestCaseList = GetDummyTestCases();
            var loggerMock = new Mock<IMessageLogger>();

            var filterExpression = new Mock<ITestCaseFilterExpression>();
            filterExpression.Setup(x => x.MatchTestCase(It.IsAny<TestCase>(), It.IsAny<Func<string, object>>()))
                            .Returns<TestCase, Func<string, object>>((t, f) => t.DisplayName.Equals("TestNamespace.TestClass.TestMethod2"));
            
            var runContextMock = new Mock<IRunContext>();
            runContextMock.Setup(x => x.GetTestCaseFilter(It.IsAny<IEnumerable<string>>(), It.IsAny<Func<string, TestProperty>>()))
                          .Returns<IEnumerable<string>, Func<string, object>>((properties, propertyProvider) => filterExpression.Object);

            var filter = new TestCaseFilter(runContextMock.Object, loggerMock.Object);

            // Act
            var results = dummyTestCaseList.Where(filter.MatchTestCase);

            // Assert
            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("TestNamespace.TestClass.TestMethod2", results.First().DisplayName);
        }

        [TestMethod]
        public void MatchTestCase_WithoutFilterExpression_MatchesAllCases()
        {
            // Arrange
            var dummyTestCaseList = GetDummyTestCases();
            var loggerMock = new Mock<IMessageLogger>();

            var runContextMock = new Mock<IRunContext>();
            runContextMock.Setup(x => x.GetTestCaseFilter(It.IsAny<IEnumerable<string>>(), It.IsAny<Func<string, TestProperty>>()))
                          .Returns<ITestCaseFilterExpression>(null);

            var filter = new TestCaseFilter(runContextMock.Object, loggerMock.Object);

            // Act
            var results = dummyTestCaseList.Where(filter.MatchTestCase);

            // Assert
            Assert.AreEqual(dummyTestCaseList.Count(), results.Count());
        }
    }
}
