using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWWilliams
{
    public static class TestFx
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


    }
}
