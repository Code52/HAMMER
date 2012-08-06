using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HAMMER.Pants.Tests
{
    [TestClass]
    public class InputProcessorTests
    {
        [TestMethod]
        public void Process_WithInvalidArguments_ReturnsException()
        {
            var args = new[] {""};
            var subject = InputProcessor.GetArgs(args);
            Assert.IsNotNull(subject.Exception);
        }

        [TestMethod]
        public void Process_WithValidArguments_DoesNotSetException()
        {
            var args = new[] { "/colour:FFFFFF" };
            var subject = InputProcessor.GetArgs(args);
            Assert.IsNull(subject.Exception);
        }

        [TestMethod]
        public void Process_WithValidFile_DoesNotSetException()
        {
            var args = new[] { "/config:hammer.config" };
            var subject = InputProcessor.GetArgsFromFile(args);
            Assert.IsNull(subject.Exception);
        }

        [TestMethod]
        public void Process_WithNoParametersButFileFound_DoesNotSetException()
        {
            var args = new[] { "" };
            var subject = InputProcessor.GetArgsFromFile(args);
            Assert.IsNull(subject.Exception);
        }

        [TestMethod]
        public void Process_WithInvalidFile_SetsException()
        {
            var args = new[] { "/config:blah.config" };
            var subject = InputProcessor.GetArgsFromFile(args);
            Assert.IsNotNull(subject.Exception);
        }
    }
}
