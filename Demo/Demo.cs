using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWWilliams
{
    class Demo
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Decimal prompt with defaults");
            decimal angle = ConsoleEx.PromptDecimal("Angle?");
            Console.WriteLine(angle);

            Console.WriteLine("***********");

            Console.WriteLine("Decimal prompt with verbosity turned off");
            decimal length = ConsoleEx.PromptDecimal("Length?", verbose: false);
            Console.WriteLine(length);

            Console.WriteLine("***********");

            Console.WriteLine("Integer prompt with min value set");
            int height = ConsoleEx.PromptInteger("Height?", minValue: 1);
            Console.WriteLine(height);
        }
    }
}
