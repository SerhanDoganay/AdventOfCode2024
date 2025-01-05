using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode2024
{
    class Day6
    {
        public static void main()
        {
            // (Borrowed grid code from day 4)
            string[] lines = File.ReadAllLines(Program.INPUT_DIR + "input6.txt");
            int numColumns = lines[0].Length; // How many characters wide?
            int numRows = lines.Count(x => x.Length > 0); // How many characters tall?

            char[,] charGrid = new char[numColumns, numRows];

            int GuardX = -1;
            int GuardY = -1;

            // Populate the grid
            for (int y = 0; y < numRows; y++)
            {
                string line = lines[y];
                for (int x = 0; x < numColumns; x++)
                {
                    charGrid[x, y] = line[x];
                    // Have we not found the guard yet?
                    if (GuardX == -1 && line[x] == '^')
                    {
                        // Record the guard's position
                        GuardX = x;
                        GuardY = y;
                    }
                }
            }

            // Move the guard around while she's still in the grid
            int numPossiblePositions = 0; // For part 2
            for (int y = 0; y < numRows; y++)
            {
                for (int x = 0; x < numColumns; x++)
                {
                    if (charGrid[x, y] == '^' || charGrid[x, y] == '#')
                        continue;

                    char[,] testCharGrid = new char[numColumns, numRows];
                    Array.Copy(charGrid, testCharGrid, charGrid.Length);
                    testCharGrid[x, y] = '#';

                    // Guard code
                    int guardX = GuardX;
                    int guardY = GuardY;
                    int currDirection = 0; // 0-up,1-right,2-down,3-left
                    while (guardX >= 0 && guardX < numColumns && guardY >= 0 && guardY < numRows)
                    {
                        if ((testCharGrid[guardX, guardY] & 0b11110000) == 0 && (testCharGrid[guardX, guardY] & (char)Math.Pow(2, currDirection)) != 0)
                        {
                            numPossiblePositions++;
                            break;
                        }
                        if (testCharGrid[guardX, guardY] == '^' || testCharGrid[guardX, guardY] == '.')
                            testCharGrid[guardX, guardY] = (char)0;
                        testCharGrid[guardX, guardY] |= (char)Math.Pow(2, currDirection); // The guard passed by this space! Encode the direction(s) into the space

                        // Is this still the right direction?
                        bool invalidDirection = true;
                        int newX = 0, newY = 0;
                        while (invalidDirection)
                        {
                            newX = guardX;
                            newY = guardY;

                            switch (currDirection)
                            {
                                case 0: // UP
                                    newY--;
                                    break;
                                case 1: // RIGHT
                                    newX++;
                                    break;
                                case 2: // DOWN
                                    newY++;
                                    break;
                                case 3: // LEFT
                                    newX--;
                                    break;
                            }

                            try
                            {
                                if (testCharGrid[newX, newY] != '#')
                                    invalidDirection = false; // There's no obstacle ahead of us! We're safe!
                            }
                            catch (IndexOutOfRangeException)
                            {
                                // The guard's gonna move out of bounds
                                invalidDirection = false;
                            }

                            if (invalidDirection)
                                currDirection = (currDirection + 1) % 4; // Switch direction
                        }

                        // Update the guard's position
                        guardX = newX;
                        guardY = newY;
                    }
                }
            }

            Console.WriteLine(numPossiblePositions);
        }
    }
}
