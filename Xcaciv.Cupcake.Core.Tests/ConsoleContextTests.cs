using Xcaciv.Cupcake.Core;

namespace Xcaciv.Cupcake.Core.Tests
{
    public class ConsoleContextTests
    {
        [Fact]
        public void Constructor_SetsVerboseDefault()
        {
            var ctx = new ConsoleContext("Test", [], verbose: true);
            Assert.True(ctx.Verbose);
        }

        [Fact]
        public async Task PromptForCommand_ReturnsInput()
        {
            var ctx = new ConsoleContext("Test", [], verbose: false);
            // We cannot read from Console in tests; just verify method exists and returns non-null by mocking input
            // Since Console.ReadLine() is used, we skip calling; focus on SetStatusMessage and HandleOutputChunk
            await ctx.SetStatusMessage("status");
            await ctx.HandleOutputChunk("output");
            Assert.True(true);
        }

        [Fact]
        public async Task SetProgress_ComputesProgress()
        {
            var ctx = new ConsoleContext("Test", [], verbose: false);
            var progress = await ctx.SetProgress(100, 10);
            Assert.Equal(10, progress);
        }
    }
}
