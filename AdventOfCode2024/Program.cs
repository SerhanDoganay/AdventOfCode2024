using System;

namespace AdventOfCode2024
{
    class Program
    {
        public static string INPUT_DIR;

        static void Main(string[] args)
        {
            INPUT_DIR = args[0]; // The directory holding the input files (include '/')
            Day16.main(); // Change the day number to work through other days
        }
    }
}
