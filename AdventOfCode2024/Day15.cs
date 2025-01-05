using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AdventOfCode2024
{
    class Day15
    {
        public static void main()
        {
            string[] lines = File.ReadAllLines(Program.INPUT_DIR + "input15.txt");
            // PART 1
            /*
            int gridWidth = lines[0].Length;
            int gridHeight = Array.IndexOf(lines, "");
            char[,] gridData = new char[gridWidth, gridHeight];
            int robotX = -1, robotY = -1;
            string movements = "";

            bool movementMode = false; // Are we reading the map input or the directions input?
            int rowCounter = 0;
            foreach (string line in lines)
            {
                if (line == "")
                    movementMode = true; // An empty line signifies the start of the directions input

                if (!movementMode)
                {
                    // We are reading grid data
                    int columnCounter = 0;
                    foreach (char gridEntry in line)
                    {
                        gridData[columnCounter, rowCounter] = gridEntry;
                        if (gridEntry == '@')
                        {
                            // Keep track of the robot's position
                            robotX = columnCounter;
                            robotY = rowCounter;
                        }
                        columnCounter++;
                    }
                    rowCounter++;
                }
                else
                {
                    movements += line;
                }
            }

            // Parse the movements
            foreach (char movement in movements)
            {
                int dx = 0;
                int dy = 0;

                switch (movement)
                {
                    case '^':
                        dy = -1; // move up
                        break;
                    case '>':
                        dx = 1; // move right
                        break;
                    case 'v':
                        dy = 1; // move down
                        break;
                    case '<':
                        dx = -1; // move left
                        break;
                }

                int considerX = robotX + dx;
                int considerY = robotY + dy;

                // What's in our hypothetical next spot?
                bool movedBoxes = false;
                while (gridData[considerX, considerY] == 'O') // while we're looking at a box
                {
                    movedBoxes = true;
                    considerX += dx;
                    considerY += dy;
                }

                // Is there an empty space?
                if (gridData[considerX, considerY] == '.')
                {
                    if (movedBoxes)
                        gridData[considerX, considerY] = 'O'; // Push a box into this new spot

                    gridData[robotX, robotY] = '.'; // Clear the robot's old spot
                    gridData[robotX + dx, robotY + dy] = '@'; // Set the robot's new position

                    robotX += dx;
                    robotY += dy;
                }
            }

            // Locate the boxes
            int coordSum = 0;
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    if (gridData[x, y] == 'O')
                        coordSum += x + 100 * y;
                }
            }

            Console.WriteLine(coordSum);*/

            int gridWidth = lines[0].Length * 2;
            int gridHeight = Array.IndexOf(lines, "");
            char[,] gridData = new char[gridWidth, gridHeight];
            int robotX = -1, robotY = -1;
            string movements = "";

            bool movementMode = false; // Are we reading the map input or the directions input?
            int rowCounter = 0;
            foreach (string line in lines)
            {
                if (line == "")
                    movementMode = true; // An empty line signifies the start of the directions input

                if (!movementMode)
                {
                    // We are reading grid data
                    int columnCounter = 0;
                    foreach (char gridEntry in line)
                    {
                        switch (gridEntry)
                        {
                            case '#':
                            case '.':
                                gridData[columnCounter, rowCounter] = gridEntry;
                                gridData[columnCounter + 1, rowCounter] = gridEntry;
                                break;
                            case 'O':
                                gridData[columnCounter, rowCounter] = '[';
                                gridData[columnCounter + 1, rowCounter] = ']';
                                break;
                            case '@':
                                robotX = columnCounter;
                                robotY = rowCounter;
                                gridData[columnCounter, rowCounter] = '@';
                                gridData[columnCounter + 1, rowCounter] = '.';
                                break;
                        }
                        columnCounter += 2;
                    }
                    rowCounter++;
                }
                else
                {
                    movements += line;
                }
            }

            PrintGrid(gridData, gridWidth, gridHeight, (char)0);

            // Parse the movements
            foreach (char movement in movements)
            //while (true)
            {
                //char movement = Console.ReadKey(false).KeyChar;

                int dx = 0;
                int dy = 0;

                switch (movement)
                {
                    case '^':
                    case 'w':
                        dy = -1; // move up
                        break;
                    case '>':
                    case 'd':
                        dx = 1; // move right
                        break;
                    case 'v':
                    case 's':
                        dy = 1; // move down
                        break;
                    case '<':
                    case 'a':
                        dx = -1; // move left
                        break;
                }

                int considerX = robotX + dx;
                int considerY = robotY + dy;
                List<KeyValuePair<int, int>> listOfConsiderations = new List<KeyValuePair<int, int>>() { new KeyValuePair<int, int>(considerX, considerY) };
                List<KeyValuePair<int, int>> boxCoords = new List<KeyValuePair<int, int>>();
                bool foundBox = false;
                bool validMove = true;

                //foreach (KeyValuePair<int, int> consideration in listOfConsiderations)
                for (int i = 0; i < listOfConsiderations.Count; i++)
                {
                    KeyValuePair<int, int> consideration = listOfConsiderations[i];
                    considerX = consideration.Key;
                    considerY = consideration.Value;

                    // If there's a box, then check the next space if there's not a wall
                    bool redge = false;
                    while (gridData[considerX, considerY] == '[' || (gridData[considerX, considerY] == ']' && (dx == 0 || !foundBox)))
                    {
                        // Only consider the left half of the box
                        if (gridData[considerX, considerY] == ']')
                        {
                            redge = true;
                            considerX--;
                        }
                        foundBox = true;

                        boxCoords.Add(new KeyValuePair<int, int>(considerX, considerY));

                        foundBox = true;

                        // Are we moving up/down?
                        if (dy != 0)
                        {
                            // This box can potentially move two boxes...
                            listOfConsiderations.Add(new KeyValuePair<int, int>(considerX + 1, considerY + dy)); // We're gonna check the left half by default, so we need to ALSO worry about the right side (considerX + 1)
                            // KeyValuePair<int, int> rightStart = new KeyValuePair<int, int>(considerX + 1, considerY);
                            // ;
                            // if (!listOfConsiderations.Contains(rightStart))
                            //    listOfConsiderations[i] = rightStart;
                        }

                        int multiplier = dx == 0 ? 1 : 2; // Because the boxes are two spaces wide, if the robot is moving left or right, we need to check the spot two spaces over.
                        considerX += multiplier * dx;
                        considerY += dy;
                    }

                    if (redge)
                        considerX -= dx; // we overshot

                    validMove &= gridData[considerX, considerY] == '.';
                }

                if (validMove)
                {

                    boxCoords.Sort((a, b) => dx != 0 ? (/*we're moving horizontally*/ dx > 0 ? (/*we're moving right*/ -a.Key.CompareTo(b.Key)) : (/*left*/ a.Key.CompareTo(b.Key))) : (/*vertically*/ dy > 0 ? /*down*/ -a.Value.CompareTo(b.Value) : /*up*/ a.Value.CompareTo(b.Value)));
                    foreach (KeyValuePair<int, int> box in boxCoords)
                    {
                        gridData[box.Key, box.Value] = '.';
                        gridData[box.Key + 1, box.Value] = '.';
                        gridData[box.Key + dx, box.Value + dy] = '[';
                        gridData[box.Key + dx + 1, box.Value + dy] = ']';
                    }
                    /*
                    for (int i = 0; i < endPoints.Count; i++)
                    {
                        KeyValuePair<int, int> finalPoint = endPoints[i];
                        considerX = finalPoint.Key;
                        considerY = finalPoint.Value;

                        KeyValuePair<int, int> initialPoint = listOfConsiderations[i];
                        int initialX = initialPoint.Key - dx;
                        int initialY = initialPoint.Value - dy;

                        while (considerX != initialX || considerY != initialY)
                        {
                            gridData[considerX, considerY] = gridData[considerX - dx, considerY - dy];
                            considerX -= dx;
                            considerY -= dy;
                        }

                        gridData[robotX, robotY] = '.'; // Erase the old position
                    }*/

                    gridData[robotX, robotY] = '.'; // Erase the old position
                    robotX += dx;
                    robotY += dy;
                    gridData[robotX, robotY] = '@';
                }

                PrintGrid(gridData, gridWidth, gridHeight, movement);
            }

            // Locate the boxes
            int coordSum = 0;
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    if (gridData[x, y] == '[')
                        coordSum += x + 100 * y;
                }
            }

            Console.WriteLine(coordSum);
        }

        public static void PrintGrid(char[,] gridData, int width, int height, char movement)
        {
            return;
            /*
            if (movement == (char)0)
                Console.WriteLine("Initial state:");
            else
                Console.WriteLine("Move " + movement + ":");

            for (int y = 0; y < height; y++)
            {
                string line = "";
                for (int x = 0; x < width; x++)
                {
                    line += gridData[x, y];
                }
                Console.WriteLine(line);
            }

            Console.WriteLine();*/
        }
    }
}
