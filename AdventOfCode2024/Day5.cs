using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode2024
{
    class Day5
    {
        public static void main()
        {
            string[] lines = File.ReadAllLines(Program.INPUT_DIR + "input5.txt");

            Dictionary<int, List<int>> pageOrdering = new Dictionary<int, List<int>>(); // The first entry represents a page number, and the second entry is all the pages that must come after it
            bool orderingMode = true;
            int middleSum1 = 0;
            int middleSum2 = 0;
            foreach (string line in lines)
            {
                if (line.Length == 0 && orderingMode)
                    orderingMode = false; // Now expect update data
                else if (orderingMode && line.Length > 0)
                {
                    int[] pageNumbers = line.Split('|').Select(x => int.Parse(x)).ToArray(); // Extract the page numbers

                    // Add the new rule to the dictionary
                    if (pageOrdering.ContainsKey(pageNumbers[0]))
                        pageOrdering[pageNumbers[0]].Add(pageNumbers[1]);
                    else
                        pageOrdering.Add(pageNumbers[0], new List<int>() { pageNumbers[1] });
                }
                else if (line.Length > 0)
                {
                    int[] pageNumbers = line.Split(',').Select(x => int.Parse(x)).ToArray(); // Extract the page numbers

                    // Iterate through each element. If pageOrdering[currElement].contains(earlier element), then we've violated a rule!
                    bool isInOrder = true;
                    for (int i = 1; i < pageNumbers.Length && isInOrder; i++)
                    {
                        int currPage = pageNumbers[i];
                        for (int j = 0; j < i && isInOrder && pageOrdering.ContainsKey(currPage); j++)
                        {
                            if (pageOrdering[currPage].Contains(pageNumbers[j]))
                                isInOrder = false;
                        }
                    }

                    if (isInOrder)
                    {
                        middleSum1 += pageNumbers[pageNumbers.Length / 2];
                    }
                    else
                    {
                        // !! PART 2 !!
                        // If an error is detected, just swap the pages
                        bool stillIncorrect = true;
                        while (stillIncorrect)
                        {
                            stillIncorrect = false; // Assume there's no error
                            for (int i = 1; i < pageNumbers.Length && !stillIncorrect; i++)
                            {
                                int currPage = pageNumbers[i];
                                for (int j = 0; j < i && !stillIncorrect && pageOrdering.ContainsKey(currPage); j++)
                                {
                                    if (pageOrdering[currPage].Contains(pageNumbers[j]))
                                    {
                                        stillIncorrect = true; // Error detected
                                        int tempPage = pageNumbers[j];
                                        pageNumbers[j] = currPage;
                                        pageNumbers[i] = tempPage;
                                    }
                                }
                            }
                        }

                        middleSum2 += pageNumbers[pageNumbers.Length / 2];
                    }
                }
            }

            Console.WriteLine(middleSum1);
            Console.WriteLine(middleSum2);
        }
    }
}
