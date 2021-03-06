﻿using System;

namespace TWWilliams
{
    public static class ConsoleEx
    {
        /// <summary>
        /// Prompts for the user to enter an integer on the command line. Will continue
        /// prompting until an integer that meets the requirements is supplied.
        /// 
        /// To have no range constraints on the integer, do not specify either minValue
        /// or maxValue.
        /// </summary>
        /// <param name="prompt">The question to ask the user</param>
        /// <param name="minValue">Optional. The supplied integer must be greater than
        /// or equal to this value. Defaults to Int32.MinValue.</param>
        /// <param name="maxValue">Optional. The supplied integer must be less than or
        /// equal to this value. Defaults to Int32.MaxValue.</param>
        /// <param name="verbose">Optional. If true, show a message with valid range
        /// when invalid response given. Defaults to true.</param>
        /// <returns>The supplied integer</returns>
        public static int PromptInteger(string prompt,
            int minValue = int.MinValue, int maxValue = int.MaxValue,
            bool verbose = true)
        {
            return (int)PromptDecimalOrInteger(prompt, NumberType.Integer, minValue, maxValue,
                verbose);
        }

        /// <summary>
        /// Prompts for the user to enter a decimal on the command line. Will continue
        /// prompting until a decimal that meets the requirements is supplied.
        /// 
        /// To have no range constraints on the decimal, do not specify either minValue
        /// or maxValue.
        /// </summary>
        /// <param name="prompt">The question to ask the user</param>
        /// <param name="minValue">Optional. The supplied decimal must be greater than
        /// or equal to this value. Defaults to Decimal.MinValue.</param>
        /// <param name="maxValue">Optional. The supplied decimal must be less than or
        /// equal to this value. Defaults to Decimal.MaxValue.</param>
        /// <param name="verbose">Optional. If true, show a message with valid range
        /// when invalid response given. Defaults to true.</param>
        /// <returns>The supplied decimal</returns>
        public static decimal PromptDecimal(string prompt,
            decimal minValue = decimal.MinValue, decimal maxValue = decimal.MaxValue,
            bool verbose = true)
        {
            return PromptDecimalOrInteger(prompt, NumberType.Decimal, minValue, maxValue,
                verbose);
        }

        private static decimal PromptDecimalOrInteger(string prompt, NumberType type,
            decimal minValue, decimal maxValue, bool verbose)
        {
            decimal result;
            bool showMessage = false;

            do
            {
                string response = "";

                response = GetResponse(prompt, type, minValue, maxValue, showMessage);

                try
                {
                    result = (type == NumberType.Integer)
                        ? Convert.ToInt32(response)
                        : Convert.ToDecimal(response);

                    if (IsInRange(result, minValue, maxValue))
                    {
                        return result;
                    }
                    showMessage = verbose;
                }
                catch (Exception e)
                when (e is OverflowException ||
                      e is FormatException ||
                      e is ArgumentNullException)
                {
                    showMessage = verbose;
                    continue;
                }
            } while (true);
        }

        private static bool IsInRange(decimal value, decimal minValue, decimal maxValue)
        {
            return (value >= minValue && value <= maxValue);
        }

        // This is suppressed because we expect code calling this library to pass in the prompt
        // string rather than being a resource controlled by this library
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization",
            "CA1303:Do not pass literals as localized parameters",
            MessageId = "System.Console.Write(System.String)")]
        private static string GetResponse(string prompt, NumberType type,
            decimal minValue, decimal maxValue, bool showMessage)
        {
            if (showMessage)
            {
                Console.WriteLine(HelpMessage(type, minValue, maxValue));
            }

            Console.Write($"{prompt} ");
            return Console.ReadLine();
        }

        private static string HelpMessage(NumberType type, decimal minValue, decimal maxValue)
        {
            string typeString = (type == NumberType.Integer) ? "whole number" : "decimal value";

            return $"Please supply a {typeString} between {minValue} and {maxValue}.";
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

    public enum NumberType
    {
        Integer,
        Decimal
    }
}
