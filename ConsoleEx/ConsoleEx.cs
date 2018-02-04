using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWWilliams
{
    public static class ConsoleEx
    {
        /// <summary>
        /// Prompts for the user to enter an integer on the command line. Will continue prompting until an integer that
        /// meets the requirements is supplied.
        /// 
        /// To have no range constraints on the integer, do not specify either minValue or maxValue.
        /// </summary>
        /// <param name="prompt">The question to ask the user</param>
        /// <param name="minValue">Optional. The supplied integer must be greater than or equal to this value.</param>
        /// <param name="maxValue">Optional. The supplied integer must be less than or equal to this value.</param>
        /// <returns>The supplied integer</returns>
        public static int PromptInteger(string prompt,
            int minValue = int.MinValue, int maxValue = int.MaxValue,
            bool verbose = true)
        {
            int result;
            bool isValid = false;
            bool showMessage = false;

            do
            {
                string response = "";

                response = GetResponse(prompt, "integer", minValue, maxValue, showMessage);

                if (!int.TryParse(response, out result))
                {
                    showMessage = verbose;
                    continue;
                }

                if (result >= minValue && result <= maxValue)
                {
                    isValid = true;
                }
                showMessage = verbose;

            } while (!(isValid));

            return result;
        }
        public static decimal PromptDecimal(string prompt,
            decimal minValue = decimal.MinValue, decimal maxValue = decimal.MaxValue,
            bool verbose = true)
        {
            decimal result;
            bool isValid = false;
            bool showMessage = false;

            do
            {
                string response = "";

                response = GetResponse(prompt, "decimal", minValue, maxValue, showMessage);

                if (!decimal.TryParse(response, out result))
                {
                    showMessage = verbose;
                    continue;
                }

                if (result >= minValue && result <= maxValue)
                {
                    isValid = true;
                }
                showMessage = verbose;

            } while (!(isValid));

            return result;
        }


        private static string GetResponse(string prompt, string type,
            decimal minValue, decimal maxValue, bool showMessage)
        {
            if (showMessage)
            {
                Console.WriteLine(
                    $"Please supply a {type} value between {minValue} and {maxValue}.");
            }

            Console.Write($"{prompt} ");
            return Console.ReadLine();
        }

        /// <summary>
        /// Validates that a number is within the specified range
        /// </summary>
        /// <param name="number">The number to check</param>
        /// <param name="paramName">The name of the parameter being checked. Used in the exception message.</param>
        /// <param name="min">The minimum value. Defaults to 1.</param>
        /// <param name="max">The maximum value. Defaults to int.MaxValue.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if number is outside supplied range.</exception>
        public static void ValidateIntRange(int number, string paramName, int min = 1, int max = int.MaxValue)
        {
            if (number < min)
            {
                throw new ArgumentOutOfRangeException(paramName, $"The {paramName} parameter must be greater than or equal to {min}.");
            }

            if (number > max)
            {
                throw new ArgumentOutOfRangeException(paramName, $"The {paramName} parameter must be less than or equal to {max}.");
            }
        }

    }
}
