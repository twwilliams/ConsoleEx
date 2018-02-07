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
            var (stdOut, stdIn) = ConsoleDefaults();

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

            ResetConsole(stdOut, stdIn);
        }

        [Theory]
        [InlineData("Give me a number", "10", 10)]
        [InlineData("", "-15", -15)]
        [InlineData("", "0", 0)]
        public void ReturnsCorrectNumber(string prompt, string response, int expected)
        {
            var (stdOut, stdIn) = ConsoleDefaults();

            int result = HandleValidResponse(prompt, response);
            Assert.Equal(expected, result);

            ResetConsole(stdOut, stdIn);
        }

        [Theory]
        [InlineData("6", 1, 10, 6)]
        [InlineData("-23", -100, 0, -23)]
        public void AcceptsNumbersInValidRange(string response, int min, int max, int expected)
        {
            var (stdOut, stdIn) = ConsoleDefaults();

            int result = HandleValidResponse("", response, min, max);
            Assert.Equal(expected, result);

            ResetConsole(stdOut, stdIn);
        }

        [Theory]
        [InlineData("aoeunth")]
        [InlineData("0.005")]
        [InlineData("-530.0")]
        [InlineData("-2147483649")] // One below int.MinValue
        public void RepromptsWhenResponseIsInvalid(string response)
        {
            var (stdOut, stdIn) = ConsoleDefaults();

            int result = RepromptWithInvalidFirstResponse("", response, "15", int.MinValue);
            Assert.Equal(15, result);

            ResetConsole(stdOut, stdIn);
        }

        [Theory]
        [InlineData("0", 1)]
        [InlineData("-10", -5)]
        [InlineData("1000", 500000)]
        public void RepromptsWhenResponseBelowMinValue(string response, int minValue)
        {
            var (stdOut, stdIn) = ConsoleDefaults();

            int result = RepromptWithInvalidFirstResponse("", response, $"{minValue + 1}", minValue);
            Assert.Equal(minValue + 1, result);

            ResetConsole(stdOut, stdIn);
        }

        [Theory]
        [InlineData("50", 10)]
        [InlineData("0", -1)]
        [InlineData("1", 0)]
        [InlineData("-59", -100)]
        public void RepromptsWhenResponseAboveMaxValue(string response, int maxValue)
        {
            var (stdOut, stdIn) = ConsoleDefaults();

            int result = RepromptWithInvalidFirstResponse("", response, $"{maxValue - 1}", null,
                                                          maxValue);
            Assert.Equal(maxValue - 1, result);

            ResetConsole(stdOut, stdIn);
        }


        /// <summary>
        /// Gets the default values for Console.Write* methods and Console.Read* methods so that
        /// they can be restored at the end of the test.
        /// </summary>
        /// <returns>Tuple of TextWriter and TextReader representing Console default IO.</returns>
        private (TextWriter, TextReader) ConsoleDefaults()
        {
            TextWriter stdOut = Console.Out;
            TextReader stdIn = Console.In;

            return (stdOut, stdIn);
        }

        /// <summary>
        /// Restores Console IO settings to defaults
        /// </summary>
        /// <param name="stdOut">The stdOut value obtained from ConsoleDefaults()</param>
        /// <param name="stdIn">The stdin value obtained from ConsoleDefaults()</param>
        private void ResetConsole(TextWriter stdOut, TextReader stdIn)
        {
            Console.SetOut(stdOut);
            Console.SetIn(stdIn);
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
        private int RepromptWithInvalidFirstResponse(string prompt, string invalidResponse,
                                                     string validResponse, int? minValue = null,
                                                     int? maxValue = null, bool verbose = true)
        {
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
                        return maxValue.HasValue
                            ? ConsoleEx.PromptInteger(prompt, (int)minValue, (int)maxValue,
                                                      verbose: verbose)
                            : ConsoleEx.PromptInteger(prompt, (int)minValue, verbose: verbose);
                    }
                    else
                    {
                        return maxValue.HasValue
                            ? ConsoleEx.PromptInteger(prompt, int.MinValue, (int)maxValue,
                                                      verbose: verbose)
                            : ConsoleEx.PromptInteger(prompt, verbose: verbose);
                    }
                }
            }
        }
    }
}
