using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode2024
{
    class Day3
    {
        public static void main()
        {
            string content = File.ReadAllText(Program.INPUT_DIR + "input3.txt");

            MatchCollection mulMatches = Regex.Matches(content, "mul\\([0-9]{1,3},[0-9]{1,3}\\)"); // Only accept "mul(", a 1-3 digit number, a "," another 1-3 digit number, and a ")"
            int productSum = 0;
            foreach (Match match in mulMatches)
            {
                string mulExpr = match.Value;
                string[] mulFragments = mulExpr.Split(',');
                mulFragments[0] = mulFragments[0].Split('(')[1]; // Get everything past the "("
                mulFragments[1] = mulFragments[1].Split(')')[0]; // Get everything before the ")"

                productSum += int.Parse(mulFragments[0]) * int.Parse(mulFragments[1]);
            }

            Console.WriteLine(productSum);

            // PART TWO

            mulMatches = Regex.Matches(content, "mul\\([0-9]{1,3},[0-9]{1,3}\\)|do\\(\\)|don\\'t\\(\\)"); // Accept the mul(,), as well as do() and don't()
            bool mulEnabled = true;
            productSum = 0;
            foreach (Match match in mulMatches)
            {
                string expr = match.Value;
                if (expr == "do()")
                    mulEnabled = true;
                else if (expr == "don't()")
                    mulEnabled = false;
                else if (mulEnabled)
                {
                    // A mul expression must be what's remaining
                    string[] mulFragments = expr.Split(',');
                    mulFragments[0] = mulFragments[0].Split('(')[1]; // Get everything past the "("
                    mulFragments[1] = mulFragments[1].Split(')')[0]; // Get everything before the ")"

                    productSum += int.Parse(mulFragments[0]) * int.Parse(mulFragments[1]);
                }
            }

            Console.WriteLine(productSum);
        }
    }
}
