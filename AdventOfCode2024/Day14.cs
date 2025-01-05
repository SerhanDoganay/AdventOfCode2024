using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode2024
{
    class Day14
    {
        public static int gridWidth = 101;
        public static int gridHeight = 103;

        public static void main()
        {
            string[] lines = File.ReadAllLines(Program.INPUT_DIR + "input14.txt");

            KeyValuePair<int, int>[] robotPositions = new KeyValuePair<int, int>[lines.Length];
            KeyValuePair<int, int>[] robotVelocities = new KeyValuePair<int, int>[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                int[][] halves = lines[i].Split(' ').Select(x => x.Split('=')[1]).Select(x => x.Split(',').Select(y => int.Parse(y)).ToArray()).ToArray();
                robotPositions[i] = new KeyValuePair<int, int>(halves[0][0], halves[0][1]);
                robotVelocities[i] = new KeyValuePair<int, int>(halves[1][0], halves[1][1]);
            }

            int[] robotsInQuadrants = new int[4] { 0, 0, 0, 0 };
            displayRobots(robotPositions, 0);

            /*
            for (int seconds = 1; seconds <= 1000; seconds++)
            {
                //System.Threading.Thread.Sleep(500);
                for (int i = 0; i < robotPositions.Length; i++)
                {
                    KeyValuePair<int, int> initialPosition = robotPositions[i];
                    KeyValuePair<int, int> velocity = robotVelocities[i];

                    int finalX = (initialPosition.Key + velocity.Key) % gridWidth;
                    int finalY = (initialPosition.Value + velocity.Value) % gridHeight;

                    // We want the final positions to be positive though
                    while (finalX < 0)
                        finalX += gridWidth;
                    while (finalY < 0)
                        finalY += gridHeight;

                    robotPositions[i] = new KeyValuePair<int, int>(finalX, finalY);

                    if (seconds == 100 && finalX != gridWidth / 2 && finalY != gridHeight / 2)
                    {
                        // This robot is not along the middle; we can figure out which quadrant this robot belongs in
                        bool isLeft = finalX < gridWidth / 2;
                        bool isTop = finalY < gridHeight / 2;

                        int quadrant = 0; // top left by default
                        if (!isLeft && isTop)
                            quadrant = 1; // top right
                        else if (isLeft && !isTop)
                            quadrant = 2; // bottom left
                        else if (!isLeft && !isTop)
                            quadrant = 3; // bottom right

                        robotsInQuadrants[quadrant]++;
                    }
                }
                displayRobots(robotPositions, seconds);
                //Console.WriteLine(seconds);
            }*/

            /*
            For part 2, I produced bitmaps for the first thousand seconds and noticed a few images with horizontal lines and a few images with vertical lines.
            The nth horizontal line was found in 27+103n seconds, and the nth vertical line was found in 52+101n seconds.
            Basically, find the first number that both of these sequences have in common.
            */
            int s = 6516;

            for (int i = 0; i < robotPositions.Length; i++)
            {
                KeyValuePair<int, int> initialPosition = robotPositions[i];
                KeyValuePair<int, int> velocity = robotVelocities[i];

                int finalX = (initialPosition.Key + velocity.Key * s) % gridWidth;
                int finalY = (initialPosition.Value + velocity.Value * s) % gridHeight;

                // We want the final positions to be positive though
                while (finalX < 0)
                    finalX += gridWidth;
                while (finalY < 0)
                    finalY += gridHeight;

                robotPositions[i] = new KeyValuePair<int, int>(finalX, finalY);
            }

            displayRobots(robotPositions, s);
            int safetyFactor = robotsInQuadrants[0] * robotsInQuadrants[1] * robotsInQuadrants[2] * robotsInQuadrants[3];
            // Console.WriteLine(safetyFactor);
        }

        public static void displayRobots(KeyValuePair<int, int>[] positions, int second)
        {
            Bitmap bmp = new Bitmap(gridWidth, gridHeight);

            //Console.Clear();
            //Console.WriteLine("\x1b[3J");

            for (int y = 0; y < gridHeight; y++)
            {
                //string currLine = "";
                for (int x = 0; x < gridWidth; x++)
                {
                    KeyValuePair<int, int> coord = new KeyValuePair<int, int>(x, y);
                    int numRobotsInCoord = positions.Count(x => x.Key == coord.Key && x.Value == coord.Value);

                    if (numRobotsInCoord == 0)
                        bmp.SetPixel(x, y, Color.Black); //currLine += ' ';
                    else
                        bmp.SetPixel(x, y, Color.White); //currLine += numRobotsInCoord;
                }
                //Console.WriteLine(currLine);
            }

            bmp.Save(Program.INPUT_DIR + "14/" + second + ".bmp");
            bmp.Dispose();
        }
    }
}
