using System;
using System.IO;
using System.Text;
using Xunit;

namespace TWWilliams
{
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
        public void ReturnsCorrectNumber(string prompt, string response, int expected)
        {
            var (stdOut, stdIn) = TestFx.ConsoleDefaults();

            int result = HandleValidResponse(prompt, response);
            Assert.Equal(expected, result);

            TestFx.ResetConsole(stdOut, stdIn);
        }

        [Theory]
        [InlineData("6", 1, 10, 6)]
        [InlineData("-23", -100, 0, -23)]
        public void AcceptsNumbersInValidRange(string response, int min, int max, int expected)
        {
            var (stdOut, stdIn) = TestFx.ConsoleDefaults();

            int result = HandleValidResponse("", response, min, max);
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

            (int result, string output) = RepromptWithInvalidFirstResponse("", response,
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

            (int result, string output) = RepromptWithInvalidFirstResponse("", response,
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

            (int result, string output) = RepromptWithInvalidFirstResponse("", response,
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

            (int result, string output) = RepromptWithInvalidFirstResponse("", response, "15");

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
                RepromptWithInvalidFirstResponse("", response, "15", verbose: false);

            Assert.DoesNotContain("whole number", output);
            TestFx.ResetConsole(stdOut, stdIn);
        }

        /// <summary>
        /// Redirect Console.Out and Console.In to drive ReadWrite.PromptForInteger() with an
        /// initial valid response.
        /// </summary>
        /// <param name="prompt">The prompt for the function to display</param>
        /// <param name="response">The response to supply to the function</param>
        /// <param name="minValue">The minimum acceptable value for the response</param>
        /// <param name="maxValue">The maximum acceptable value for the response</param>
        /// <returns></returns>
        private int HandleValidResponse(string prompt, string response, int? minValue = null,
            int? maxValue = null)
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                StringBuilder input = new StringBuilder();
                input.AppendLine(response);

                using (StringReader sr = new StringReader(input.ToString()))
                {
                    Console.SetIn(sr);
                    if (minValue.HasValue)
                    {
                        return maxValue.HasValue
                            ? ConsoleEx.PromptInteger(prompt, (int)minValue, (int)maxValue)
                            : ConsoleEx.PromptInteger(prompt, (int)minValue);
                    }
                    else
                    {
                        return maxValue.HasValue
                            ? ConsoleEx.PromptInteger(prompt, int.MinValue, (int)maxValue)
                            : ConsoleEx.PromptInteger(prompt);
                    }
                }
            }
        }

        /// <summary>
        /// Redirect Console.Out and Console.In to drive ReadWrite.PromptForInteger() with an
        /// initial invalid response, followed by a valid one in order to cause the function to
        /// reprompt.
        /// </summary>
        /// <param name="prompt">The prompt for the function to display</param>
        /// <param name="invalidResponse">The invalid response to supply to the function</param>
        /// <param name="validResponse">The valid response to supply to the function</param>
        /// <param name="minValue">The minimum acceptable value for the response</param>
        /// <param name="maxValue">The maximum acceptable value for the response</param>
        /// <returns></returns>
        private (int, string) RepromptWithInvalidFirstResponse(string prompt, string invalidResponse,
                                                     string validResponse, int? minValue = null,
                                                     int? maxValue = null, bool verbose = true)
        {
            int result_int;
            string result_string;
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);

                StringBuilder input = new StringBuilder();
                input.AppendLine(invalidResponse);
                input.AppendLine(validResponse);

                using (StringReader sr = new StringReader(input.ToString()))
                {
                    Console.SetIn(sr);
                    if (minValue.HasValue)
                    {
                        result_int = maxValue.HasValue
                            ? ConsoleEx.PromptInteger(prompt, (int)minValue, (int)maxValue,
                                                      verbose: verbose)
                            : ConsoleEx.PromptInteger(prompt, (int)minValue, verbose: verbose);
                    }
                    else
                    {
                        result_int = maxValue.HasValue
                            ? ConsoleEx.PromptInteger(prompt, int.MinValue, (int)maxValue,
                                                      verbose: verbose)
                            : ConsoleEx.PromptInteger(prompt, verbose: verbose);
                    }
                }

                result_string = sw.ToString();
            }
            return (result_int, result_string);
        }
    }
}
