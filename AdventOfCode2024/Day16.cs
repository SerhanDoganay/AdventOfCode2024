using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode2024
{
    class Day16
    {
        private static char[,] ORIGINAL_GRID;
        private static bool[,] compositeGrid = null;

        private static int numColumns;
        private static int numRows;

        private static KeyValuePair<int, int> startTile;
        private static KeyValuePair<int, int> endTile;

        private static List<KeyValuePair<int, int>> nodes = null;
        private static List<int> degrees = null; // node[index] has a degree of degrees[index]
        private static List<KeyValuePair<KeyValuePair<int, int>, int>> paths = null; // First key represents the indices of the endpoints in the nodes list, and the value is the length

        private static List<int> pathsBlockedAllTime = new List<int>();

        private static int bestScore = -1;

        private static void MakeInitialGrid()
        {
            string[] lines = File.ReadAllLines(Program.INPUT_DIR + "input16.txt");
            numColumns = lines[0].Length; // How many characters wide?
            numRows = lines.Length; // How many characters tall?

            ORIGINAL_GRID = new char[numColumns, numRows];

            startTile = new KeyValuePair<int, int>();
            endTile = new KeyValuePair<int, int>();

            // Populate the grid
            for (int y = 0; y < numRows; y++)
            {
                string line = lines[y];
                for (int x = 0; x < numColumns; x++)
                {
                    ORIGINAL_GRID[x, y] = line[x];

                    if (line[x] == 'S')
                    {
                        startTile = new KeyValuePair<int, int>(x, y);
                        ORIGINAL_GRID[x, y] = '.';
                    }
                    else if (line[x] == 'E')
                    {
                        endTile = new KeyValuePair<int, int>(x, y);
                        ORIGINAL_GRID[x, y] = '.';
                    }

                }
            }
        }

        private static int GetScore(char[,] baseGrid, List<int> pathIDsToIgnore)
        {
            Console.WriteLine(string.Join(' ', pathIDsToIgnore));

            char[,] charGrid = new char[numColumns, numRows];
            Array.Copy(baseGrid, charGrid, numColumns * numRows);

            if (nodes == null && degrees == null && paths == null)
            {
                nodes = new List<KeyValuePair<int, int>>() { startTile };
                degrees = new List<int>() { -1 };
                paths = new List<KeyValuePair<KeyValuePair<int, int>, int>>(); // First key represents the indices of the endpoints in the nodes list, and the value is the length

                // BFS to find node and path data
                Queue<KeyValuePair<int, int>> visitedNodes = new Queue<KeyValuePair<int, int>>();
                visitedNodes.Enqueue(startTile);

                while (visitedNodes.Count > 0)
                {
                    KeyValuePair<int, int> currNode = visitedNodes.Dequeue();
                    int x = currNode.Key;
                    int y = currNode.Value;

                    int degree = 0;
                    int possibleDirections = 0; // 1 - up, 2 - right, 4 - down, 8 - left
                    if (charGrid[x, y - 1] == '.')
                    {
                        possibleDirections |= 1;
                        degree++;
                    }
                    if (charGrid[x + 1, y] == '.')
                    {
                        possibleDirections |= 2;
                        degree++;
                    }
                    if (charGrid[x, y + 1] == '.')
                    {
                        possibleDirections |= 4;
                        degree++;
                    }
                    if (charGrid[x - 1, y] == '.')
                    {
                        possibleDirections |= 8;
                        degree++;
                    }

                    degrees[nodes.IndexOf(currNode)] = degree;

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
                                degrees.Add(-1);
                                visitedNodes.Enqueue(newNode);
                            }

                            int aID = nodes.IndexOf(currNode);
                            int bID = nodes.IndexOf(newNode);
                            if (!paths.Any(x => x.Key.Key == bID && x.Key.Value == aID))
                                paths.Add(new KeyValuePair<KeyValuePair<int, int>, int>(new KeyValuePair<int, int>(aID, bID), pathLength));
                        }
                    }
                }

                // Prints grid with node letters
                /*
                for (int i = 0; i < nodes.Count; i++)
                {
                    KeyValuePair<int, int> node = nodes[i];
                    charGrid[node.Key, node.Value] = (char)(i > 26 ? (i - 27 + 'a') : (i - 1 + 'A'));
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
            }

            // Dijkstra (modified to prioritize number of turns over path length)
            KeyValuePair<KeyValuePair<int, int>, List<int>>[] shortestPaths = new KeyValuePair<KeyValuePair<int, int>, List<int>>[nodes.Count]; // Key: (Key=distance, Value=numturns); Value: path from start node to end node. The indicies of this array represent the end node indices
            for (int i = 0; i < shortestPaths.Length; i++)
                shortestPaths[i] = new KeyValuePair<KeyValuePair<int, int>, List<int>>(new KeyValuePair<int, int>(int.MaxValue, int.MaxValue), new List<int>());

            Queue<int> nodesToVisit = new Queue<int>();
            nodesToVisit.Enqueue(0); // Add the starting node first
            while (nodesToVisit.Count > 0)
            {
                int i = nodesToVisit.Dequeue(); // Current node

                // Get current distance traveled
                if (i == 0)
                    shortestPaths[i] = new KeyValuePair<KeyValuePair<int, int>, List<int>>(new KeyValuePair<int, int>(0, 0), new List<int>() { 0 }); // If this is the starting node, the distance to get here is 0

                int distanceTraveled = shortestPaths[i].Key.Key;
                int numTurns = shortestPaths[i].Key.Value;

                // Which nodes are adjacent to this one?
                List<KeyValuePair<KeyValuePair<int, int>, int>> incidentPaths = paths.Where(path => path.Key.Key == i || path.Key.Value == i).ToList();
                for (int pathID = 0; pathID < incidentPaths.Count; pathID++)
                {
                    KeyValuePair<KeyValuePair<int, int>, int> path = incidentPaths[pathID];
                    int neighbor = path.Key.Key == i ? path.Key.Value : path.Key.Key;

                    // Is this going to cost us another turn?
                    // First of all, what direction were we traveling up to now?
                    int newTurns = 0;

                    if (pathIDsToIgnore.Contains(paths.IndexOf(path)))
                    {
                        newTurns = 999;
                    }
                    else
                    {
                        int currDX = nodes[neighbor].Key - nodes[i].Key;
                        int currDY = nodes[neighbor].Value - nodes[i].Value;

                        if (currDX != 0) currDX /= Math.Abs(currDX);
                        if (currDY != 0) currDY /= Math.Abs(currDY);

                        if (shortestPaths[i].Value.Count > 1)
                        {
                            KeyValuePair<int, int> prevNode = nodes[shortestPaths[i].Value[shortestPaths[i].Value.Count - 2]];
                            int oldDX = nodes[i].Key - prevNode.Key;
                            int oldDY = nodes[i].Value - prevNode.Value;

                            if (oldDX != 0) oldDX /= Math.Abs(oldDX);
                            if (oldDY != 0) oldDY /= Math.Abs(oldDY);

                            if (currDX != oldDX || currDY != oldDY)
                                newTurns = 1;
                        }
                        else
                        {
                            // This is the second node, in which there is no history to check.
                            // Are we heading east?
                            if (currDX <= 0)
                                newTurns = 1; // We're not; we're going to need to turn
                        }
                    }

                    int distanceToNeighbor = distanceTraveled + path.Value;
                    int newTotalTurns = numTurns + newTurns;

                    if (newTotalTurns <= shortestPaths[neighbor].Key.Value)
                    {
                        if (newTotalTurns == shortestPaths[neighbor].Key.Value)
                        {
                            if (distanceToNeighbor < shortestPaths[neighbor].Key.Key)
                            {
                                // We found a shorter path to this node!
                                shortestPaths[neighbor] = new KeyValuePair<KeyValuePair<int, int>, List<int>>(new KeyValuePair<int, int>(distanceToNeighbor, newTotalTurns), shortestPaths[i].Value.Append(neighbor).ToList());
                                nodesToVisit.Enqueue(neighbor);
                            }
                        }
                        else
                        {
                            // We found a path with less turns to this node!
                            shortestPaths[neighbor] = new KeyValuePair<KeyValuePair<int, int>, List<int>>(new KeyValuePair<int, int>(distanceToNeighbor, newTotalTurns), shortestPaths[i].Value.Append(neighbor).ToList());
                            nodesToVisit.Enqueue(neighbor);
                        }
                    }

                }
            }

            // Calculate score
            KeyValuePair<KeyValuePair<int, int>, List<int>> finalPath = new KeyValuePair<KeyValuePair<int, int>, List<int>>();
            int score = -1;

            try
            {
                finalPath = shortestPaths[nodes.IndexOf(endTile)];
                score = finalPath.Key.Value * 1000 + finalPath.Key.Key;
            }
            catch (Exception)
            {
                // We ran into a dead end. No worries.
            }

            if (bestScore == -1)
                bestScore = score;

            if (score != -1 && bestScore == score)
            {
                if (compositeGrid == null)
                    compositeGrid = new bool[numColumns, numRows];

                List<int> pathIDsTaken = new List<int>();

                for (int fNodeID = 0; fNodeID < finalPath.Value.Count - 1; fNodeID++)
                {
                    int cNodeID = finalPath.Value[fNodeID];
                    int nNodeID = finalPath.Value[fNodeID + 1];

                    KeyValuePair<int, int> cNode = nodes[cNodeID];
                    KeyValuePair<int, int> nNode = nodes[nNodeID];

                    int dx = nNode.Key - cNode.Key;
                    int dy = nNode.Value - cNode.Value;
                    if (dx != 0) dx /= Math.Abs(dx);
                    if (dy != 0) dy /= Math.Abs(dy);

                    int x = cNode.Key;
                    int y = cNode.Value;

                    while (x != nNode.Key || y != nNode.Value)
                    {
                        compositeGrid[x, y] = true;

                        x += dx;
                        y += dy;
                    }

                    compositeGrid[x, y] = true;

                    /*
                    if (fNodeID > 0)
                        GetScore(charGrid, cNode);*/
                    // Find the path that connects these two nodes
                    int pathID = paths.IndexOf(paths.First(path => (path.Key.Key == cNodeID && path.Key.Value == nNodeID) || (path.Key.Key == nNodeID && path.Key.Value == cNodeID)));
                    //pathIDsTaken.Add(pathID);
                    pathsBlockedAllTime.Add(pathID);
                    //if (pathIDsToIgnore.Count == 0 || pathID > pathIDsToIgnore.Max())
                    if (degrees[cNodeID] > 2 && pathsBlockedAllTime.Count(x => x == pathID) < degrees[cNodeID])
                        GetScore(charGrid, pathIDsToIgnore.Append(pathID).ToList());
                }
            }

            // Console.WriteLine(score);
            return score;
        }

        public static void main()
        {
            MakeInitialGrid();
            GetScore(ORIGINAL_GRID, new List<int>());

            int numBestPathTiles = 0;
            for (int y = 0; y < numRows; y++)
            {
                string line = "";
                for (int x = 0; x < numColumns; x++)
                {
                    line += compositeGrid[x, y] ? 'O' : ' ';
                    if (compositeGrid[x, y])
                        numBestPathTiles++;
                }
                Console.WriteLine(line);
            }

            Console.WriteLine(numBestPathTiles);
        }
    }
}
