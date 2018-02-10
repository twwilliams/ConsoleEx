using System;
using System.IO;
using Xunit;

namespace TWWilliams
{
    [Collection("Console Wrappers")]
    public class PromptIntegerTests
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
                    ConsoleEx.PromptInteger(prompt);
                    Assert.Equal(prompt + " ", sw.ToString());
                }
            }

            TestFx.ResetConsole(stdOut, stdIn);
        }

        [Theory]
        [InlineData("Give me a number", "10", 10)]
        [InlineData("", "-15", -15)]
        [InlineData("", "0", 0)]
        public void ReturnsCorrectInteger(string prompt, string response, int expected)
        {
            var (stdOut, stdIn) = TestFx.ConsoleDefaults();

            int result = TestFx.HandleValidIntegerResponse(prompt, response);
            Assert.Equal(expected, result);

            TestFx.ResetConsole(stdOut, stdIn);
        }

        [Theory]
        [InlineData("6", 1, 10, 6)]
        [InlineData("-23", -100, 0, -23)]
        public void AcceptsIntegersInValidRange(string response, int min, int max, int expected)
        {
            var (stdOut, stdIn) = TestFx.ConsoleDefaults();

            int result = TestFx.HandleValidIntegerResponse("", response, min, max);
            Assert.Equal(expected, result);

            TestFx.ResetConsole(stdOut, stdIn);
        }

        [Theory]
        [InlineData("aoeunth")]
        [InlineData("0.005")]
        [InlineData("-530.0")]
        [InlineData("-2147483649")] // One below int.MinValue
        public void RepromptsWhenResponseIsInvalid(string response)
        {
            var (stdOut, stdIn) = TestFx.ConsoleDefaults();

            (int result, string output) = TestFx.RepromptWithInvalidFirstIntegerResponse("", response,
                "15", int.MinValue);
            Assert.Equal(15, result);

            TestFx.ResetConsole(stdOut, stdIn);
        }

        [Theory]
        [InlineData("0", 1)]
        [InlineData("-10", -5)]
        [InlineData("1000", 500000)]
        public void RepromptsWhenResponseBelowMinValue(string response, int minValue)
        {
            var (stdOut, stdIn) = TestFx.ConsoleDefaults();

            (int result, string output) = TestFx.RepromptWithInvalidFirstIntegerResponse("", response,
                $"{minValue + 1}", minValue);
            Assert.Equal(minValue + 1, result);

            TestFx.ResetConsole(stdOut, stdIn);
        }

        [Theory]
        [InlineData("50", 10)]
        [InlineData("0", -1)]
        [InlineData("1", 0)]
        [InlineData("-59", -100)]
        public void RepromptsWhenResponseAboveMaxValue(string response, int maxValue)
        {
            var (stdOut, stdIn) = TestFx.ConsoleDefaults();

            (int result, string output) = TestFx.RepromptWithInvalidFirstIntegerResponse("", response,
                $"{maxValue - 1}", null, maxValue);
            Assert.Equal(maxValue - 1, result);

            TestFx.ResetConsole(stdOut, stdIn);
        }

        [Theory]
        [InlineData("aoeunth")]
        [InlineData("0.005")]
        [InlineData("-530.0")]
        [InlineData("-2147483649")] // One below int.MinValue
        public void OutputsHelpMessageWithInvalidResponse(string response)
        {
            var (stdOut, stdIn) = TestFx.ConsoleDefaults();

            (int result, string output) = TestFx.RepromptWithInvalidFirstIntegerResponse("", response, "15");

            string expected =
                $"Please supply a whole number between {int.MinValue} and {int.MaxValue}.";
            Assert.Contains(expected, output);
            TestFx.ResetConsole(stdOut, stdIn);
        }

        [Theory]
        [InlineData("aoeunth")]
        [InlineData("0.005")]
        [InlineData("-530.0")]
        [InlineData("-2147483649")] // One below int.MinValue
        public void DoesNotOutputHelpMessageWithVerboseOff(string response)
        {
            var (stdOut, stdIn) = TestFx.ConsoleDefaults();

            (int result, string output) =
                TestFx.RepromptWithInvalidFirstIntegerResponse("", response, "15", verbose: false);

            Assert.DoesNotContain("whole number", output);
            TestFx.ResetConsole(stdOut, stdIn);
        }
    }
}
