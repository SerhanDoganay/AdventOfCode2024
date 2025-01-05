using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AdventOfCode2024
{
    class Day13
    {
        public static void main()
        {
            string[] lines = File.ReadAllLines(Program.INPUT_DIR + "input13.txt");
            int lineCounter = 0;
            double tokenSum = 0;
            double[][] mtx = new double[][]
            {
                new double[] { -1, -1, -1 },
                new double[] { -1, -1, -1 }
            };
            for (int i = 0; i < lines.Length + 1; i++) // +1 so that the program can check the last claw machine
            {
                if (lineCounter == 3)
                {
                    // Finalize entry
                    mtx = matrixrref(mtx);

                    // Did we get whole-number answers?
                    double roundedAPresses = (double)Math.Floor(mtx[0][2]);
                    double roundedBPresses = (double)Math.Floor(mtx[1][2]);
                    double aFraction = mtx[0][2] - roundedAPresses;
                    double bFraction = mtx[1][2] - roundedBPresses;

                    if (aFraction == 0 && bFraction == 0)
                    {
                        // This claw machine has a valid solution!
                        tokenSum += roundedAPresses * 3 + roundedBPresses;
                    }

                    // Prepare for a new entry
                    mtx = new double[][]
                    {
                        new double[] { -1, -1, -1 },
                        new double[] { -1, -1, -1 }
                    };
                }
                else
                {
                    string[] halves = lines[i].Split(',');
                    double x = -1, y = -1;

                    if (lineCounter == 2)
                    {
                        // Prize
                        x = int.Parse(halves[0].Split('=')[1]);
                        y = int.Parse(halves[1].Split('=')[1]);

                        // PART 2
                        x += 10000000000000;
                        y += 10000000000000;
                    }
                    else
                    {
                        // Either button A or B
                        x = int.Parse(halves[0].Split('+')[1]);
                        y = int.Parse(halves[1].Split('+')[1]);
                    }

                    mtx[0][lineCounter] = x;
                    mtx[1][lineCounter] = y;
                }

                lineCounter++;
                lineCounter %= 4;
            }

            Console.WriteLine(tokenSum);
        }

        public static double[][] matrixrref(double[][] mtx)
        {
            // Assuming we're always working with a 2x3 matrix
            double factor = mtx[0][0] / mtx[1][0];

            mtx[1][0] *= -factor;
            mtx[1][1] *= -factor;
            mtx[1][2] *= -factor;

            mtx[1][0] += mtx[0][0];
            mtx[1][1] += mtx[0][1];
            mtx[1][2] += mtx[0][2];

            factor = mtx[0][1] / mtx[1][1];

            mtx[1][0] *= -factor;
            mtx[1][1] *= -factor;
            mtx[1][2] *= -factor;

            mtx[0][0] += mtx[1][0];
            mtx[0][1] += mtx[1][1];
            mtx[0][2] += mtx[1][2];

            factor = mtx[0][0];

            mtx[0][0] /= factor;
            mtx[0][1] /= factor;
            mtx[0][2] /= factor;

            factor = mtx[1][1];

            mtx[1][0] /= factor;
            mtx[1][1] /= factor;
            mtx[1][2] /= factor;

            mtx[0][2] = Math.Round(mtx[0][2], 3);
            mtx[1][2] = Math.Round(mtx[1][2], 3);
            return mtx;
        }
    }
}
