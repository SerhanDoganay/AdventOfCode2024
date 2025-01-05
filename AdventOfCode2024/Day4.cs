using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode2024
{
    class Day4
    {
        public static void main()
        {
            string[] lines = File.ReadAllLines(Program.INPUT_DIR + "input4.txt");
            int numColumns = lines[0].Length; // How many characters wide?
            int numRows = lines.Count(x => x.Length > 0); // How many characters tall?

            char[,] charGrid = new char[numColumns, numRows];

            // Populate the grid
            for (int y = 0; y < numRows; y++)
            {
                string line = lines[y];
                for (int x = 0; x < numColumns; x++)
                    charGrid[x, y] = line[x];
            }

            // Conduct the word search
            List<KeyValuePair<int, int>> possibleDirections = new List<KeyValuePair<int, int>>()
            {
                new KeyValuePair<int, int>(-1, -1),
                new KeyValuePair<int, int>(-1, 0),
                new KeyValuePair<int, int>(-1, 1),
                new KeyValuePair<int, int>(0, -1),
                new KeyValuePair<int, int>(0, 1),
                new KeyValuePair<int, int>(1, -1),
                new KeyValuePair<int, int>(1, 0),
                new KeyValuePair<int, int>(1, 1)
            }; // The first int represents which direction the x should go, and the second int instructs the y.
            char[] keyLetters = { 'M', 'A', 'S' }; // Ignore 'X' because it's only after we locate X that we look in all directions for M, A, then S.
            int xmasCount = 0;
            for (int x = 0; x < numColumns; x++)
            {
                for (int y = 0; y < numRows; y++)
                {
                    if (charGrid[x, y] == 'X')
                    {
                        // Scan in all directions around this cell and hope to find the next letter
                        int letterIndex = 0;
                        while (letterIndex < keyLetters.Length && possibleDirections.Count > 0)
                        {
                            // We still have remaining letters, and we haven't run out of directions
                            List<int> directionsToRemove = new List<int>(); // Indices
                            for (int direction = 0; direction < possibleDirections.Count; direction++)
                            {
                                KeyValuePair<int, int> dCoord = possibleDirections[direction];
                                int dX = dCoord.Key * (letterIndex + 1); // e.g. if we're looking bottom-right for 'M', then search (x+1,y+1). When search for 'A', look at (x+2,y+2)...
                                int dY = dCoord.Value * (letterIndex + 1);

                                // First, are we going out of bounds if we try progressing in this direction?
                                if (x + dX >= numColumns || x + dX < 0 || y + dY >= numRows || y + dY < 0)
                                {
                                    // Stop looking in this direction.
                                    directionsToRemove.Add(direction);
                                }
                                // Second, does this direction have the next letter?
                                else if (charGrid[x + dX, y + dY] != keyLetters[letterIndex])
                                {
                                    // No it doesn't; stop looking in this direction.
                                    directionsToRemove.Add(direction);
                                }
                            }

                            // Eliminate the invalid directions
                            directionsToRemove.Reverse(); // Sorts the directions from largest index to smallest index, so that removing by index doesn't get messed up
                            foreach (int badDirection in directionsToRemove)
                                possibleDirections.RemoveAt(badDirection);

                            // After weeding out this round's invalid directions, look for the next letter
                            letterIndex++;
                        }

                        // Have we found XMAS?
                        if (letterIndex == 3)
                            xmasCount += possibleDirections.Count; // We found M, A and S at these directions

                        // Reset the directions list
                        possibleDirections = new List<KeyValuePair<int, int>>()
                        {
                            new KeyValuePair<int, int>(-1, -1),
                            new KeyValuePair<int, int>(-1, 0),
                            new KeyValuePair<int, int>(-1, 1),
                            new KeyValuePair<int, int>(0, -1),
                            new KeyValuePair<int, int>(0, 1),
                            new KeyValuePair<int, int>(1, -1),
                            new KeyValuePair<int, int>(1, 0),
                            new KeyValuePair<int, int>(1, 1)
                        };
                    }
                }
            }

            Console.WriteLine(xmasCount);

            // PART TWO
            // Start by searching for the center 'A'
            // We're skipping the outermost rows/columns because an X "MAS" can't be positioned there
            int crossMasCount = 0;
            for (int x = 1; x < (numColumns - 1); x++)
            {
                for (int y = 1; y < (numRows - 1); y++)
                {
                    if (charGrid[x, y] == 'A')
                    {
                        if (((charGrid[x - 1, y - 1] == 'M' && charGrid[x + 1, y + 1] == 'S') || (charGrid[x - 1, y - 1] == 'S' && charGrid[x + 1, y + 1] == 'M')) && ((charGrid[x + 1, y - 1] == 'M' && charGrid[x - 1, y + 1] == 'S') || (charGrid[x + 1, y - 1] == 'S' && charGrid[x - 1, y + 1] == 'M')))
                            crossMasCount++; // Both diagonals have "MAS"!
                    }
                }
            }

            Console.WriteLine(crossMasCount);
        }
    }
}
