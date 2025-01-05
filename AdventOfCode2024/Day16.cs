using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode2024
{
    class Day16
    {
        public static void main()
        {
            string[] lines = File.ReadAllLines(Program.INPUT_DIR + "in16.txt");
            int numColumns = lines[0].Length; // How many characters wide?
            int numRows = lines.Length; // How many characters tall?

            char[,] charGrid = new char[numColumns, numRows];

            KeyValuePair<int, int> startTile = new KeyValuePair<int, int>();
            KeyValuePair<int, int> endTile = new KeyValuePair<int, int>();

            // Populate the grid
            for (int y = 0; y < numRows; y++)
            {
                string line = lines[y];
                for (int x = 0; x < numColumns; x++)
                {
                    charGrid[x, y] = line[x];

                    if (line[x] == 'S')
                    {
                        startTile = new KeyValuePair<int, int>(x, y);
                        charGrid[x, y] = '.';
                    }
                    else if (line[x] == 'E')
                    {
                        endTile = new KeyValuePair<int, int>(x, y);
                        charGrid[x, y] = '.';
                    }
                    
                }
            }

            List<KeyValuePair<int, int>> nodes = new List<KeyValuePair<int, int>>() { startTile };
            List<KeyValuePair<KeyValuePair<int, int>, int>> paths = new List<KeyValuePair<KeyValuePair<int, int>, int>>(); // First key represents the indices of the endpoints in the nodes list, and the value is the length

            // BFS to find node and path data
            Queue<KeyValuePair<int, int>> visitedNodes = new Queue<KeyValuePair<int, int>>();
            visitedNodes.Enqueue(startTile);

            while (visitedNodes.Count > 0)
            {
                KeyValuePair<int, int> currNode = visitedNodes.Dequeue();
                int x = currNode.Key;
                int y = currNode.Value;

                int possibleDirections = 0; // 1 - up, 2 - right, 4 - down, 8 - left
                if (charGrid[x, y - 1] == '.')
                    possibleDirections |= 1;
                if (charGrid[x + 1, y] == '.')
                    possibleDirections |= 2;
                if (charGrid[x, y + 1] == '.')
                    possibleDirections |= 4;
                if (charGrid[x - 1, y] == '.')
                    possibleDirections |= 8;

                for (int direction = 0; direction < 4; direction++)
                {
                    // Reset x and y to start at the node
                    x = currNode.Key;
                    y = currNode.Value;

                    if ((possibleDirections & (int)Math.Pow(2, direction)) != 0)
                    {
                        // We can travel in this direction from this node
                        int dx = 0, dy = 0;
                        switch (direction)
                        {
                            case 0:
                                // UP
                                dy = -1;
                                break;
                            case 1:
                                // RIGHT
                                dx = 1;
                                break;
                            case 2:
                                // DOWN
                                dy = 1;
                                break;
                            case 3:
                                // LEFT
                                dx = -1;
                                break;
                        }

                        // If a cell has only two possible directions and they are in opposite directions, then it's not a new path
                        x += dx;
                        y += dy;
                        int newDirections = 0;
                        if (charGrid[x, y - 1] == '.')
                            newDirections |= 1;
                        if (charGrid[x + 1, y] == '.')
                            newDirections |= 2;
                        if (charGrid[x, y + 1] == '.')
                            newDirections |= 4;
                        if (charGrid[x - 1, y] == '.')
                            newDirections |= 8;
                        int pathLength = 1;
                        while (newDirections == 5 || newDirections == 10)
                        {
                            x += dx;
                            y += dy;
                            newDirections = 0;
                            if (charGrid[x, y - 1] == '.')
                                newDirections |= 1;
                            if (charGrid[x + 1, y] == '.')
                                newDirections |= 2;
                            if (charGrid[x, y + 1] == '.')
                                newDirections |= 4;
                            if (charGrid[x - 1, y] == '.')
                                newDirections |= 8;
                            pathLength++;
                        }

                        KeyValuePair<int, int> newNode = new KeyValuePair<int, int>(x, y);
                        if (!nodes.Contains(newNode))
                        {
                            nodes.Add(newNode);
                            visitedNodes.Enqueue(newNode);
                        }

                        int aID = nodes.IndexOf(currNode);
                        int bID = nodes.IndexOf(newNode);
                        if (!paths.Any(x => x.Key.Key == bID && x.Key.Value == aID))
                            paths.Add(new KeyValuePair<KeyValuePair<int, int>, int>(new KeyValuePair<int, int>(aID, bID), pathLength));
                    }
                }
            }

            /*
            foreach (KeyValuePair<int, int> node in nodes)
            {
                charGrid[node.Key, node.Value] = (char)(nodes.IndexOf(node) + 'A');
            }
            for (int y = 0; y < numRows; y++)
            {
                string line = "";
                for (int x = 0; x < numColumns; x++)
                {
                    line += charGrid[x, y];
                }
                Console.WriteLine(line);
            }*/

            ;
        }
    }
}
