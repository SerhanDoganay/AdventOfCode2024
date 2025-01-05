using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode2024
{
    class Day1
    {
        public static void main()
        {
            string[] lines = File.ReadAllLines(Program.INPUT_DIR + "input1.txt");
            List<int> leftList = new List<int>();
            List<int> rightList = new List<int>();

            // Populate leftList and rightList from the file
            foreach (string line in lines)
            {
                string[] tokens = line.Split(' ');
                tokens = tokens.Where(x => x != "").ToArray(); // Get rid of all the empty entries that arise from splitting by spaces
                if (tokens.Length == 2) // Just to skip the empty lines
                {
                    leftList.Add(int.Parse(tokens[0]));
                    rightList.Add(int.Parse(tokens[1]));
                }
            }

            // Sort the lists
            leftList.Sort();
            rightList.Sort();

            // Find the differences
            int diffSum = 0;
            for (int i = 0; i < leftList.Count; i++)
            {
                diffSum += Math.Abs(leftList[i] - rightList[i]); // Only return positive differences
            }

            // Print the sum of the differences
            Console.WriteLine(diffSum);

            // PART TWO:
            // Because the lists are already sorted, we can speed up the score calculations a bit.
            int currNum = -1; // This refers to the current value in the left list that we're calculating its score for
            int rightIndex = 0; // The current index to look in the right list
            int numRepetitions = 0; // The number of times the number has appeared on the left list
            int currScore = 0; // The score of the current number
            int scoreSum = 0;
            for (int i = 0; i < leftList.Count; i++)
            {
                if (currNum != leftList[i])
                {
                    // We've reached a new number! Calculate this score.
                    scoreSum += currScore * numRepetitions; // Add the previous number's score, times how many times that number appeared
                    // if (currScore > 0) Console.WriteLine(currNum + " appeared in rightList " + (currScore / currNum) + " times, and showed up in leftList " + numRepetitions + " times");
                    currScore = 0;
                    numRepetitions = 0;
                    currNum = leftList[i];

                    // To account for the possibility that rightList and leftList might not have their numbers aligned,
                    // Keep looking at the next index in the right list until we reach a number that's >= currNum
                    while (rightIndex < rightList.Count && currNum > rightList[rightIndex])
                        rightIndex++;

                    // Theoretically, currNum does indeed exist in the right list! Find how many times the right list has this number
                    while (rightIndex < rightList.Count && currNum == rightList[rightIndex])
                    {
                        rightIndex++;
                        currScore += currNum; // the score is currNum * how many times the right list has the number
                    }

                    // If currNum < rightList[rightIndex], then the number doesn't exist in the right list.                    
                }

                numRepetitions++;
            }
            scoreSum += currScore * numRepetitions; // Account for the score for the last number
            // if (currScore > 0) Console.WriteLine(currNum + " appeared in rightList " + (currScore / currNum) + " times, and showed up in leftList " + numRepetitions + " times");

            // Print the sum of the scores
            Console.WriteLine(scoreSum);
        }
    }
}
