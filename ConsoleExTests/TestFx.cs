using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWWilliams
{
    internal static class TestFx
    {
        /// <summary>
        /// Gets the default values for Console.Write* methods and Console.Read* methods so that
        /// they can be restored at the end of the test.
        /// </summary>
        /// <returns>Tuple of TextWriter and TextReader representing Console default IO.</returns>
        internal static (TextWriter, TextReader) ConsoleDefaults()
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
        internal static void ResetConsole(TextWriter stdOut, TextReader stdIn)
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
        internal static int HandleValidIntegerResponse(string prompt, string response, int? minValue = null,
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
        /// initial valid response.
        /// </summary>
        /// <param name="prompt">The prompt for the function to display</param>
        /// <param name="response">The response to supply to the function</param>
        /// <param name="minValue">The minimum acceptable value for the response</param>
        /// <param name="maxValue">The maximum acceptable value for the response</param>
        /// <returns></returns>
        internal static decimal HandleValidDecimalResponse(string prompt, string response, decimal? minValue = null,
            decimal? maxValue = null)
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
                            ? ConsoleEx.PromptDecimal(prompt, (decimal)minValue, (decimal)maxValue)
                            : ConsoleEx.PromptDecimal(prompt, (decimal)minValue);
                    }
                    else
                    {
                        return maxValue.HasValue
                            ? ConsoleEx.PromptDecimal(prompt, decimal.MinValue, (decimal)maxValue)
                            : ConsoleEx.PromptDecimal(prompt);
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
        internal static (int, string) RepromptWithInvalidFirstIntegerResponse(
            string prompt, string invalidResponse, string validResponse, int? minValue = null,
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

        internal static (decimal, string) RepromptWithInvalidFirstDecimalResponse(
            string prompt, string invalidResponse, string validResponse, decimal? minValue = null,
            decimal? maxValue = null, bool verbose = true)
        {
            decimal result_decimal;
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
                        result_decimal = maxValue.HasValue
                            ? ConsoleEx.PromptDecimal(prompt, (decimal)minValue, (decimal)maxValue,
                                                      verbose: verbose)
                            : ConsoleEx.PromptDecimal(prompt, (decimal)minValue, verbose: verbose);
                    }
                    else
                    {
                        result_decimal = maxValue.HasValue
                            ? ConsoleEx.PromptDecimal(prompt, decimal.MinValue, (decimal)maxValue,
                                                      verbose: verbose)
                            : ConsoleEx.PromptDecimal(prompt, verbose: verbose);
                    }
                }

                result_string = sw.ToString();
            }
            return (result_decimal, result_string);
        }

    }
}
