using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode2024
{
    class Day7
    {
        public static void main()
        {
            string[] lines = File.ReadAllLines(Program.INPUT_DIR + "input7.txt");
            long answerSum = 0;
            foreach (string line in lines)
            {
                if (line == "")
                    continue;

                string[] sidesOfEquation = line.Split(':');
                long testVal = long.Parse(sidesOfEquation[0]);

                int[] terms = sidesOfEquation[1].Split(' ').Where(x => x != "").Select(x => int.Parse(x)).ToArray();

                // Our only operations are adding and subtracting. If we have two terms, then we have two options. If we have three terms, then we have 2*2 options. If we have four terms, we have 2*2*2 options.
                // For n terms, we have 2^(n-1) options.
                // Convert to binary. Whether the nth bit is 0 or 1 represents whether to add or multiply

                // For part 2, we have three operators, so basically replace 2 with 3
                int numOptions = (int)Math.Pow(3, terms.Length - 1);
                for (int option = 0; option < numOptions; option++)
                {
                    int tempOption = option;
                    int[] operationArray = new int[terms.Length - 1];
                    for (int i = 0; i < operationArray.Length; i++)
                    {
                        operationArray[i] = tempOption % 3;
                        tempOption /= 3;
                    }

                    long testOption = terms[0]; // The answer for our choice for operators
                    for (int i = 0; i < operationArray.Length; i++)
                    {
                        int operand = terms[i + 1];
                        if (operationArray[i] == 0)
                            testOption += operand; // ADD
                        else if (operationArray[i] == 1)
                            testOption *= operand; // MULTIPLY
                        else
                        {
                            // CONCATENATE
                            string curr = testOption.ToString();
                            curr += operand.ToString();
                            testOption = long.Parse(curr);
                        }
                    }

                    if (testOption == testVal)
                    {
                        answerSum += testVal;
                        break; // Don't check other combinations
                    }
                }
            }
            Console.WriteLine(answerSum);
        }
    }
}
