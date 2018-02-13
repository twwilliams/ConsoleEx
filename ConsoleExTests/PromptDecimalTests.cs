using System;
using System.IO;
using Xunit;

namespace TWWilliams
{
    [Collection("Console Wrappers")]
    public class PromptDecimalTests
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

        [Theory]
        [InlineData("aoeunth")]
        [InlineData("0.005f")]
        [InlineData("-79228162514264337593543950336")] // One below decimal.MinValue
        public void RepromptsWhenResponseIsInvalid(string response)
        {
            var (stdOut, stdIn) = TestFx.ConsoleDefaults();

            (decimal result, string output) =
                TestFx.RepromptWithInvalidFirstDecimalResponse("", response, "15", int.MinValue);
            Assert.Equal(15, result);

            TestFx.ResetConsole(stdOut, stdIn);
        }

        [Theory]
        [InlineData("0", 0.0001)]
        [InlineData("-10.005", -5)]
        [InlineData("1000", 500000)]
        public void RepromptsWhenResponseBelowMinValue(string response, decimal minValue)
        {
            var (stdOut, stdIn) = TestFx.ConsoleDefaults();

            (decimal result, string output) =
                TestFx.RepromptWithInvalidFirstDecimalResponse("", response, $"{minValue + 1}", minValue);
            Assert.Equal(minValue + 1, result);

            TestFx.ResetConsole(stdOut, stdIn);
        }

        [Theory]
        [InlineData("50", 10.5320)]
        [InlineData("0", -1)]
        [InlineData("0.00001", 0)]
        [InlineData("-59", -100)]
        public void RepromptsWhenResponseAboveMaxValue(string response, decimal maxValue)
        {
            var (stdOut, stdIn) = TestFx.ConsoleDefaults();

            (decimal result, string output) = TestFx.RepromptWithInvalidFirstDecimalResponse("", response,
                $"{maxValue - 1}", null, maxValue);
            Assert.Equal(maxValue - 1, result);

            TestFx.ResetConsole(stdOut, stdIn);
        }

        [Theory]
        [InlineData("aoeunth")]
        [InlineData("0.005f")]
        [InlineData("-79228162514264337593543950336")] // One below decimal.MinValue
        public void OutputsHelpMessageWithInvalidResponse(string response)
        {
            var (stdOut, stdIn) = TestFx.ConsoleDefaults();

            (decimal result, string output) = TestFx.RepromptWithInvalidFirstDecimalResponse("", response, "15");

            string expected =
                $"Please supply a decimal value between {decimal.MinValue} and {decimal.MaxValue}.";
            Assert.Contains(expected, output);
            TestFx.ResetConsole(stdOut, stdIn);
        }

        [Theory]
        [InlineData("aoeunth")]
        [InlineData("0.005f")]
        [InlineData("-79228162514264337593543950336")] // One below decimal.MinValue
        public void DoesNotOutputHelpMessageWithVerboseOff(string response)
        {
            var (stdOut, stdIn) = TestFx.ConsoleDefaults();

            (decimal result, string output) =
                TestFx.RepromptWithInvalidFirstDecimalResponse("", response, "15", verbose: false);

            Assert.DoesNotContain("decimal value", output);
            TestFx.ResetConsole(stdOut, stdIn);
        }

    }

}
