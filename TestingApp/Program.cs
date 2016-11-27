using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MiCHALosoft.Math;
using System.Numerics;

namespace TestingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = Number.Parse("0.055", CultureInfo.InvariantCulture);
            var b = Number.Parse("0.066", CultureInfo.InvariantCulture);
            Console.WriteLine($"a + b = {a + b}");
            Console.WriteLine($"a - b = {a - b}");
            var bn1 = Number.Parse("446744073709551614467440737095516144674407370955161446744073709551614467440737095516144674407370955161446744073709551614467440737095516144674407370955161446744073709551614467440737095516144674407370955161446744073709551614467440737095516144674407370955161446744073709551614467440737095516144674407370955161446744073709551614467440737095516144674407370955161446744073709551614467440737095516144674407370955161");
            var bn2 = Number.Parse("446744073709551614467440737095516144674407370955161446744073709551614467440737095516144674407370955161446744073709551614467440737095516144674407370955161446744073709551614467440737095516144674407370955161446744073709551614467440737095516144674407370955161446744073709551614467440737095516144674407370955161446744073709551614467440737095516144674407370955161446744073709551614467440737095516144674407370955161,446744073709551614467440737095516144674407370955161446744073709551614467440737095516144674407370955161446744073709551614467440737095516144674407370955161446744073709551614467440737095516144674407370955161446744073709551614467440737095516144674407370955161446744073709551614467440737095516144674407370955161446744073709551614467440737095516144674407370955161446744073709551614467440737095516144674407370955161");
            var bi1 = BigInteger.Parse("446744073709551614467440737095516144674407370955161446744073709551614467440737095516144674407370955161446744073709551614467440737095516144674407370955161446744073709551614467440737095516144674407370955161446744073709551614467440737095516144674407370955161446744073709551614467440737095516144674407370955161446744073709551614467440737095516144674407370955161446744073709551614467440737095516144674407370955161");

            var bn3 = Number.Parse("44674407370955161446744073709551614467440737.0955161446744073709551614467440737095516144674407370955161", CultureInfo.InvariantCulture);
            var bn4 = Number.Parse("44674407370955161446744073709551614467440737095.5161446744073709551614467440737095516144674407370955161", CultureInfo.InvariantCulture);

            var sn1 = new Number(22);
            var sn2 = new Number(7);
            //Console.WriteLine($"bn1 - bn1 = {bn1 - bn1}");
            //Console.WriteLine($"a - a = {a - a}");
            //Console.WriteLine($"b - a = {b - a}");
            //Console.WriteLine($"bn2 - bn1 = {bn2 - bn1}");
            //Console.WriteLine($"bn1 - bn2 = {bn1 - bn2}");
            //Console.WriteLine($"bn2 - bn2 = {bn2 - bn2}");
            //Console.WriteLine($"bn2 * 2 = {bn2 * 2}");
            //Console.WriteLine($"sn1 / sn2 = {sn1 / sn2}");
            //Console.WriteLine($"bn1^bn1 = {Number.Pow(bn1, 1000)}");
            //Console.WriteLine($"bn1^bn1 = {BigInteger.Pow(bi1, 1000)}");
            //Console.WriteLine($"bn3 / bn4 = {bn3 / bn4}");
            //Console.WriteLine($"10000! = {Number.Factorial(10000)}");

        }

        private static void WriteActionTime(Action action, string actionName = "", int stepCount = 10000)
        {
            var currentProcess = Process.GetCurrentProcess();
            currentProcess.Refresh();
            var startTime = currentProcess.TotalProcessorTime;

            try
            {
                for (var i = 0; i < stepCount; i++)
                {
                    action();
                }

                var endTime = (currentProcess.TotalProcessorTime - startTime).TotalMilliseconds;
                Console.WriteLine($"{actionName}: {endTime} ms");
            }
            catch (Exception e)
            {
                var old = Console.ForegroundColor;
                Console.Write($"{actionName}: ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error");
                Console.ForegroundColor = old;
            }
        }
        
    }
}
