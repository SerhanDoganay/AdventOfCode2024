using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode2024
{
    class Day12
    {
        public static void main()
        {
            string[] gardenLines = File.ReadAllLines(Program.INPUT_DIR + "input12.txt");

            List<List<KeyValuePair<int, int>>> plantData = new List<List<KeyValuePair<int, int>>>(); // Each plant (or group of plants) will have a list of coordinates which determine the area they encompass
            char[,] gardenGrid = new char[gardenLines[0].Length, gardenLines.Length];
            int[,] plantIDGrid = new int[gardenLines[0].Length, gardenLines.Length]; // Assigns each patch of plants a unique ID

            int currPlantID = 1; // if plantIDGrid[..,..] = 0, then that coordinate doesn't have an assigned plant patch

            for (int y = 0; y < gardenLines.Length; y++)
            {
                string gardenLine = gardenLines[y];
                for (int x = 0; x < gardenLine.Length; x++)
                {
                    char currChar = gardenLine[x];
                    gardenGrid[x, y] = currChar;

                    // Is this the start of a new patch of plants?
                    int[] surroundingIDs = new int[2]; // Because we're scanning in reading order, we only really know the cell to the left and the cell above
                    char[] surroundingChars = new char[2];

                    if (y > 0)
                    {
                        // We can search above
                        surroundingIDs[0] = plantIDGrid[x, y - 1];
                        surroundingChars[0] = gardenGrid[x, y - 1];
                    }
                    if (x > 0)
                    {
                        // We can search left
                        surroundingIDs[1] = plantIDGrid[x - 1, y];
                        surroundingChars[1] = gardenGrid[x - 1, y];
                    }

                    // Scenario 1: This is the top-left corner and no plant data is defined. Establish this as the start of the first patch
                    if (surroundingIDs[0] == 0 && surroundingIDs[1] == 0)
                    {
                        plantIDGrid[x, y] = currPlantID++;
                        plantData.Add(new List<KeyValuePair<int, int>>() { new KeyValuePair<int, int>(x, y) });
                    }
                    // Scenario 2: We have data above and to the left of us
                    else if (surroundingIDs[0] != 0 && surroundingIDs[1] != 0)
                    {
                        // Scenario 2.1: This is part of the same patch from above us
                        if (surroundingChars[0] == currChar)
                        {
                            plantIDGrid[x, y] = surroundingIDs[0];
                            plantData[surroundingIDs[0] - 1].Add(new KeyValuePair<int, int>(x, y));

                            // Scenario 2.1.1: This is part of the same patch to the left of us
                            if (surroundingChars[1] == currChar && surroundingIDs[1] != surroundingIDs[0])
                            {
                                // We thought the stuff to the left of us was from a different patch. They actually belong in THIS patch.
                                List<KeyValuePair<int, int>> oldPatchData = plantData[surroundingIDs[1] - 1];
                                foreach (KeyValuePair<int, int> oldPatchCoord in oldPatchData)
                                    plantIDGrid[oldPatchCoord.Key, oldPatchCoord.Value] = surroundingIDs[0];
                                plantData[surroundingIDs[0] - 1].AddRange(oldPatchData);
                                //plantData.RemoveAt(surroundingIDs[1] - 1);
                                plantData[surroundingIDs[1] - 1] = new List<KeyValuePair<int, int>>();
                            }
                            // Scenario 2.1.2: Otherwise, the patch to the left of us was indeed different so we need not worry.
                        }
                        // Scenario 2.2: This is part of the same patch to the left of us
                        else if (surroundingChars[1] == currChar)
                        {
                            plantIDGrid[x, y] = surroundingIDs[1];
                            plantData[surroundingIDs[1] - 1].Add(new KeyValuePair<int, int>(x, y));
                        }
                        // Scenario 2.3: This is entirely new as of right now
                        else
                        {
                            plantIDGrid[x, y] = currPlantID++;
                            plantData.Add(new List<KeyValuePair<int, int>>() { new KeyValuePair<int, int>(x, y) });
                        }
                    }
                    // Scenario 3: We only have data to the left of us
                    else if (surroundingIDs[1] != 0)
                    {
                        // Scenario 3.1: This continues a patch
                        if (surroundingChars[1] == currChar)
                        {
                            plantIDGrid[x, y] = surroundingIDs[1];
                            plantData[surroundingIDs[1] - 1].Add(new KeyValuePair<int, int>(x, y));
                        }
                        // Scenario 3.2: This looks new
                        else
                        {
                            plantIDGrid[x, y] = currPlantID++;
                            plantData.Add(new List<KeyValuePair<int, int>>() { new KeyValuePair<int, int>(x, y) });
                        }
                    }
                    // Scenario 4: We only have data above us
                    else
                    {
                        // Scenario 4.1: This continues a patch
                        if (surroundingChars[0] == currChar)
                        {
                            plantIDGrid[x, y] = surroundingIDs[0];
                            plantData[surroundingIDs[0] - 1].Add(new KeyValuePair<int, int>(x, y));
                        }
                        // Scenario 4.2: This looks new
                        else
                        {
                            plantIDGrid[x, y] = currPlantID++;
                            plantData.Add(new List<KeyValuePair<int, int>>() { new KeyValuePair<int, int>(x, y) });
                        }
                    }
                }
            }

            // Remove all the empty entries in plantData
            plantData = plantData.Where(x => x.Count > 0).ToList();

            // The area of each patch is just given by the number of coordinates it encompasses
            int priceSum = 0;
            foreach (List<KeyValuePair<int, int>> patchData in plantData)
            {
                Dictionary<KeyValuePair<int, int>, int> perimeterData = new Dictionary<KeyValuePair<int, int>, int>(); // key is edge position, value is whether this is a top edge, right edge, ...

                /*
                Imagine scaling up the grid so that we can make a box around each letter
                AA
                AA
                
                -----
                |A|A|
                -----
                |A|A|
                -----
                
                The grid was originally 2x2, and now we have 5x5 (x*2+1, y*2+1)
                The A at (0,0) is now at (1,1)
                */

                foreach (KeyValuePair<int, int> plant in patchData)
                {
                    int newX = plant.Key * 2 + 1;
                    int newY = plant.Value * 2 + 1;

                    KeyValuePair<int, int> topEdge = new KeyValuePair<int, int>(newX, newY - 1);
                    KeyValuePair<int, int> rightEdge = new KeyValuePair<int, int>(newX + 1, newY);
                    KeyValuePair<int, int> bottomEdge = new KeyValuePair<int, int>(newX, newY + 1);
                    KeyValuePair<int, int> leftEdge = new KeyValuePair<int, int>(newX - 1, newY);

                    KeyValuePair<int, int>[] edges = new KeyValuePair<int, int>[] { topEdge, rightEdge, bottomEdge, leftEdge };
                    //foreach (KeyValuePair<int, int> edge in edges)
                    for (int i = 0; i < edges.Length; i++)
                    {
                        KeyValuePair<int, int> edge = edges[i];

                        if (perimeterData.ContainsKey(edge))
                            perimeterData.Remove(edge); // Two plants share this edge! Remove it from consideration
                        else
                            perimeterData.Add(edge, i);
                    }
                }

                // Calculate the price (PART 1)
                // priceSum += patchData.Count * perimeterData.Count;

                // PART 2:
                // Try walking around clockwise? ...But that only considers the outermost edges
                int numSides = 0;
                while (perimeterData.Count > 0)
                {
                    KeyValuePair<int, int> startingEdge = perimeterData.Keys.First();
                    int currDirection = perimeterData[startingEdge];
                    KeyValuePair<int, int> currEdge = startingEdge; // Starting case
                    List<KeyValuePair<int, int>> traveledEdges = new List<KeyValuePair<int, int>>();
                    bool starting = true;
                    while ((startingEdge.Key != currEdge.Key || startingEdge.Value != currEdge.Value) || starting)
                    {
                        starting = false;

                        traveledEdges.Add(currEdge);

                        int x = currEdge.Key;
                        int y = currEdge.Value;

                        KeyValuePair<int, int> nextEdgeAlongPath = new KeyValuePair<int, int>(-1, -1);
                        KeyValuePair<KeyValuePair<int, int>, int>[] nextEdgeNextPathOptions = new KeyValuePair<KeyValuePair<int, int>, int>[2];

                        switch (currDirection)
                        {
                            case 0: // TOP EDGE
                                nextEdgeAlongPath = new KeyValuePair<int, int>(x + 2, y);
                                nextEdgeNextPathOptions[0] = new KeyValuePair<KeyValuePair<int, int>, int>(new KeyValuePair<int, int>(x + 1, y + 1), 1); // turns into a right edge?
                                nextEdgeNextPathOptions[1] = new KeyValuePair<KeyValuePair<int, int>, int>(new KeyValuePair<int, int>(x + 1, y - 1), 3); // turns into a left edge?
                                break;
                            case 1: // RIGHT EDGE
                                nextEdgeAlongPath = new KeyValuePair<int, int>(x, y + 2);
                                //nextEdgeNextPathOptions[0] = new KeyValuePair<int, int>(x - 1, y + 1); // turns into a bottom edge?
                                //nextEdgeNextPathOptions[1] = new KeyValuePair<int, int>(x + 1, y + 1); // turns into a top edge?
                                nextEdgeNextPathOptions[0] = new KeyValuePair<KeyValuePair<int, int>, int>(new KeyValuePair<int, int>(x - 1, y + 1), 2); // turns into a bottom edge?
                                nextEdgeNextPathOptions[1] = new KeyValuePair<KeyValuePair<int, int>, int>(new KeyValuePair<int, int>(x + 1, y + 1), 0); // turns into a top edge?
                                break;
                            case 2: // BOTTOM EDGE
                                nextEdgeAlongPath = new KeyValuePair<int, int>(x - 2, y);
                                //nextEdgeNextPathOptions[0] = new KeyValuePair<int, int>(x - 1, y - 1); // turns into a left edge?
                                //nextEdgeNextPathOptions[1] = new KeyValuePair<int, int>(x - 1, y + 1); // turns into a right edge?
                                nextEdgeNextPathOptions[0] = new KeyValuePair<KeyValuePair<int, int>, int>(new KeyValuePair<int, int>(x - 1, y - 1), 3); // turns into a left edge?
                                nextEdgeNextPathOptions[1] = new KeyValuePair<KeyValuePair<int, int>, int>(new KeyValuePair<int, int>(x - 1, y + 1), 1); // turns into a right edge?
                                break;
                            case 3: // LEFT EDGE
                                nextEdgeAlongPath = new KeyValuePair<int, int>(x, y - 2);
                                //nextEdgeNextPathOptions[0] = new KeyValuePair<int, int>(x + 1, y - 1); // turns into a top edge?
                                //nextEdgeNextPathOptions[1] = new KeyValuePair<int, int>(x - 1, y - 1); // turns into a bottom edge?
                                nextEdgeNextPathOptions[0] = new KeyValuePair<KeyValuePair<int, int>, int>(new KeyValuePair<int, int>(x + 1, y - 1), 0); // turns into a top edge?
                                nextEdgeNextPathOptions[1] = new KeyValuePair<KeyValuePair<int, int>, int>(new KeyValuePair<int, int>(x - 1, y - 1), 2); // turns into a bottom edge?
                                break;
                        }

                        // Can we continue along our current direction?
                        if (perimeterData.ContainsKey(nextEdgeAlongPath) && perimeterData[nextEdgeAlongPath] == currDirection)
                        {
                            // We're indeed continuing this path!
                            currEdge = nextEdgeAlongPath;
                        }
                        else
                        {
                            // We're changing direction!
                            KeyValuePair<int, int> nextEdgeNextPath = (perimeterData.ContainsKey(nextEdgeNextPathOptions[0].Key) && perimeterData[nextEdgeNextPathOptions[0].Key] == nextEdgeNextPathOptions[0].Value) ? nextEdgeNextPathOptions[0].Key : nextEdgeNextPathOptions[1].Key;

                            currDirection = perimeterData[nextEdgeNextPath];
                            currEdge = nextEdgeNextPath;
                            numSides++;
                        }
                    }

                    foreach (KeyValuePair<int, int> traveledEdge in traveledEdges)
                        perimeterData.Remove(traveledEdge);
                }

                priceSum += patchData.Count * numSides;
            }

            Console.WriteLine(priceSum);
        }
    }
}
