using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode2024
{
    class Day10
    {
        public static void main()
        {
            string[] lines = File.ReadAllLines(Program.INPUT_DIR + "input10.txt");
            int numColumns = lines[0].Length; // How many characters wide?
            int numRows = lines.Count(x => x.Length > 0); // How many characters tall?

            char[,] charGrid = new char[numColumns, numRows];

            List<List<KeyValuePair<int, int>>> trailData = new List<List<KeyValuePair<int, int>>>(); // Each entry represents a single starting point. Each subentry represents a unique path

            // Populate the grid
            for (int y = 0; y < numRows; y++)
            {
                string line = lines[y];
                for (int x = 0; x < numColumns; x++)
                {
                    charGrid[x, y] = line[x];
                    if (line[x] == '0')
                    {
                        // Keep track of where the potential trailheads are
                        trailData.Add(new List<KeyValuePair<int, int>>() { new KeyValuePair<int, int>(x, y) });
                    }
                }
            }

            for (int trailhead = 0; trailhead < trailData.Count; trailhead++)
            {
                int currHeight = 0;
                bool validTrailHead = true;
                while (currHeight < 9 && validTrailHead)
                {
                    bool validNewPath = false;
                    int numInitialPossiblePaths = trailData[trailhead].Count;
                    for (int possiblePath = 0; possiblePath < numInitialPossiblePaths; possiblePath++)
                    {
                        KeyValuePair<int, int> currPos = trailData[trailhead][possiblePath];
                        int x = currPos.Key;
                        int y = currPos.Value;

                        if (x == -1 && y == -1)
                            continue; // Ignore this; this is a dead end

                        // Search the surrounding directions for the next height
                        bool alreadyFoundPath = false;
                        if (x - 1 >= 0 && charGrid[x - 1, y] == (currHeight + '1')) // Check left
                        {
                            alreadyFoundPath = true;
                            trailData[trailhead][possiblePath] = new KeyValuePair<int, int>(x - 1, y);
                        }
                        if (x + 1 < numColumns && charGrid[x + 1, y] == (currHeight + '1')) // Check right
                        {
                            KeyValuePair<int, int> nextStep = new KeyValuePair<int, int>(x + 1, y);
                            if (alreadyFoundPath)
                            {
                                // ADD
                                trailData[trailhead].Add(nextStep);
                            }
                            else
                            {
                                // REPLACE
                                trailData[trailhead][possiblePath] = nextStep;
                            }
                            alreadyFoundPath = true;
                        }
                        if (y - 1 >= 0 && charGrid[x, y - 1] == (currHeight + '1')) // Check up
                        {
                            KeyValuePair<int, int> nextStep = new KeyValuePair<int, int>(x, y - 1);
                            if (alreadyFoundPath)
                            {
                                // ADD
                                trailData[trailhead].Add(nextStep);
                            }
                            else
                            {
                                // REPLACE
                                trailData[trailhead][possiblePath] = nextStep;
                            }
                            alreadyFoundPath = true;
                        }
                        if (y + 1 < numRows && charGrid[x, y + 1] == (currHeight + '1')) // Check down
                        {
                            KeyValuePair<int, int> nextStep = new KeyValuePair<int, int>(x, y + 1);
                            if (alreadyFoundPath)
                            {
                                // ADD
                                trailData[trailhead].Add(nextStep);
                            }
                            else
                            {
                                // REPLACE
                                trailData[trailhead][possiblePath] = nextStep;
                            }
                            alreadyFoundPath = true;
                        }

                        if (!alreadyFoundPath)
                            trailData[trailhead][possiblePath] = new KeyValuePair<int, int>(-1, -1); // Screw this; this path leads to a dead end
                        else
                            validNewPath = true;
                    }

                    // What if every path led to a dead end?
                    if (!validNewPath)
                        validTrailHead = false; // Screw this; end the loop for this trailhead

                    currHeight++;
                }
            }

            // Calculate scores
            int scoreSum = 0;
            int ratingSum = 0;
            for (int i = 0; i < trailData.Count; i++)
            {
                List<KeyValuePair<int, int>> uniqueEndPoints = new List<KeyValuePair<int, int>>();
                for (int j = 0; j < trailData[i].Count; j++)
                {
                    KeyValuePair<int, int> testEndPoint = trailData[i][j];
                    if (testEndPoint.Key != -1 && testEndPoint.Value != -1)
                    {
                        ratingSum++;
                        if (!uniqueEndPoints.Contains(testEndPoint))
                        {
                            uniqueEndPoints.Add(testEndPoint);
                        }
                    }
                }
                scoreSum += uniqueEndPoints.Count;
            }
            Console.WriteLine(scoreSum);
            Console.WriteLine(ratingSum);
        }
    }
}
