using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TWWilliams
{
    [Collection("Console Wrappers")]
    public class PromptDigitalTests
    {
        [Theory]
        [InlineData("Give me a number")]
        [InlineData("")]
        public void IssuesCorrectPrompt(string prompt)
        {
            var (stdOut, stdIn) = TestFx.ConsoleDefaults();

            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                using (StringReader sr = new StringReader("15" + Environment.NewLine))
                {
                    Console.SetIn(sr);
                    ConsoleEx.PromptDecimal(prompt);
                    Assert.Equal(prompt + " ", sw.ToString());
                }
            }

            TestFx.ResetConsole(stdOut, stdIn);
        }

        [Theory]
        [InlineData("Give me a number", "10", 10)]
        [InlineData("", "-15", -15)]
        [InlineData("", "0", 0)]
        [InlineData("", "1.5350", 1.535)]
        public void ReturnsCorrectNumber(string prompt, string response, decimal expected)
        {
            var (stdOut, stdIn) = TestFx.ConsoleDefaults();

            decimal result = TestFx.HandleValidDecimalResponse(prompt, response);
            Assert.Equal(expected, result);

            TestFx.ResetConsole(stdOut, stdIn);
        }

        [Theory]
        [InlineData("6", 1, 10, 6)]
        [InlineData("-23", -100, 0, -23)]
        public void AcceptsDecimalsInValidRange(string response, decimal min, decimal max, decimal expected)
        {
            var (stdOut, stdIn) = TestFx.ConsoleDefaults();

            decimal result = TestFx.HandleValidDecimalResponse("", response, min, max);
            Assert.Equal(expected, result);

            TestFx.ResetConsole(stdOut, stdIn);
        }

    }

}
