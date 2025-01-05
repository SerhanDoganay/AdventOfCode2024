using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode2024
{
    class Day8
    {
        public static void main()
        {
            string[] lines = File.ReadAllLines(Program.INPUT_DIR + "input8.txt");
            int numColumns = lines[0].Length; // How many characters wide?
            int numRows = lines.Count(x => x.Length > 0); // How many characters tall?

            char[,] charGrid = new char[numColumns, numRows];
            Dictionary<char, List<KeyValuePair<int, int>>> antennaData = new Dictionary<char, List<KeyValuePair<int, int>>>();

            // Populate the grid
            for (int y = 0; y < numRows; y++)
            {
                string line = lines[y];
                for (int x = 0; x < numColumns; x++)
                {
                    char c = line[x];
                    charGrid[x, y] = c;

                    // Is this an antenna?
                    if (c != '.')
                    {
                        if (antennaData.ContainsKey(c))
                        {
                            // We've seen this antenna type before! Add a new record for this position
                            antennaData[c].Add(new KeyValuePair<int, int>(x, y));
                        }
                        else
                        {
                            // This is a new type! Make a new entry in the dictionary
                            antennaData.Add(c, new List<KeyValuePair<int, int>>() { new KeyValuePair<int, int>(x, y) });
                        }
                    }
                }
            }

            // Find antinode data
            List<KeyValuePair<int, int>> antinodes = new List<KeyValuePair<int, int>>();
            foreach (char antennaType in antennaData.Keys)
            {
                // Get every pair of antennae of this type
                for (int i = 1; i < antennaData[antennaType].Count; i++)
                {
                    for (int j = 0; j < i; j++)
                    {
                        KeyValuePair<int, int> pos_i = antennaData[antennaType][i];
                        KeyValuePair<int, int> pos_j = antennaData[antennaType][j];

                        // Get the x and y distances between this pair of antennae
                        int dx = pos_i.Key - pos_j.Key;
                        int dy = pos_i.Value - pos_j.Value;

                        // Find the coordinates of the antinodes
                        /*
                        // DAY 1
                        KeyValuePair<int, int> pos_new_1 = new KeyValuePair<int, int>(pos_i.Key + dx, pos_i.Value + dy);
                        KeyValuePair<int, int> pos_new_2 = new KeyValuePair<int, int>(pos_j.Key - dx, pos_j.Value - dy);

                        // Are the antinodes in bounds? (And is there not already an antinode here?)
                        if (!antinodes.Contains(pos_new_1) && pos_new_1.Key >= 0 && pos_new_1.Key < numColumns && pos_new_1.Value >= 0 && pos_new_1.Value < numRows)
                            antinodes.Add(pos_new_1);
                        if (!antinodes.Contains(pos_new_2) && pos_new_2.Key >= 0 && pos_new_2.Key < numColumns && pos_new_2.Value >= 0 && pos_new_2.Value < numRows)
                            antinodes.Add(pos_new_2);
                        */

                        // DAY 2
                        for (int n = -numColumns; n < numColumns; n++)
                        {
                            KeyValuePair<int, int> pos_new = new KeyValuePair<int, int>(pos_i.Key + n * dx, pos_i.Value + n * dy);

                            if (pos_new.Key >= 0 && pos_new.Key < numColumns && pos_new.Value >= 0 && pos_new.Value < numRows && !antinodes.Contains(pos_new))
                                antinodes.Add(pos_new);
                        }
                    }
                }
            }

            Console.WriteLine(antinodes.Count);
        }
    }
}
