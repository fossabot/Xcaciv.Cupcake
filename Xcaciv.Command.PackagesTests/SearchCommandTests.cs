using Xcaciv.Command;
using Xcaciv.Command.Interface;
using Xcaciv.Command.Packages;

namespace Xcaciv.Command.PackagesTests
{
    public class SearchCommandTests
    {
        [Fact]
        public void HandleExecution_ReturnsExpectedResult()
        {
            // Arrange
            var command = new SearchCommand();
            var parameters = new string[] { "XCBatch" };
            var env = new EnvironmentContext(); 

            // Act
            var result = command.HandleExecution(parameters, env);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("XCBatch.Core\nXCBatch.Interfaces", result); // Replace "ExpectedResult" with the expected result of the method
        }
    }
}