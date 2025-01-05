using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode2024
{
    class Day11
    {
        public static Dictionary<KeyValuePair<ulong, int>, ulong> numNewStoneRecords = new Dictionary<KeyValuePair<ulong, int>, ulong>();
        public static Dictionary<ulong, List<Dictionary<ulong, int>>> stoneBlinkIterations = new Dictionary<ulong, List<Dictionary<ulong, int>>>(); // stoneRepetitions for each blink for each stone

        public static void main()
        {
            string[] stoneStrings = File.ReadAllText(Program.INPUT_DIR + "input11.txt").Split(' ');
            List<ulong> topLayerStones = stoneStrings.Select(x => ulong.Parse(x)).ToList();
            ulong numStones = (ulong)topLayerStones.Count;

            int numBlinks = 75;

            foreach (ulong stone in topLayerStones)
                numStones += GetNumNewStones(stone, numBlinks);

            Console.WriteLine(numStones);
        }

        public static ulong GetNumNewStones(ulong stone, int blink)
        {
            if (blink <= 0)
                return 0; // This stone won't split into any more stones

            KeyValuePair<ulong, int> stoneRecord = new KeyValuePair<ulong, int>(stone, blink);
            if (numNewStoneRecords.ContainsKey(stoneRecord))
                return numNewStoneRecords[stoneRecord]; // We've done this calculation before!

            ulong ret = 0;
            if (stoneBlinkIterations.ContainsKey(stone))
            {
                // At least we've seen this stone in a different blink before
                int numBlinksPassed = stoneBlinkIterations[stone].Count;
                numBlinksPassed = numBlinksPassed > blink ? blink : numBlinksPassed;

                Dictionary<ulong, int> stoneRepetitions = stoneBlinkIterations[stone][numBlinksPassed - 1];
                bool first = true;
                foreach (ulong stoneEntry in stoneRepetitions.Keys)
                {
                    if (first)
                    {
                        ret += (ulong)stoneRepetitions[stoneEntry] - 1;
                        first = false;
                    }
                    else
                        ret += (ulong)stoneRepetitions[stoneEntry];
                }

                blink -= numBlinksPassed;

                foreach (ulong currStone in stoneRepetitions.Keys)
                {
                    int numRepetitions = stoneRepetitions[currStone];
                    ret += GetNumNewStones(currStone, blink) * (ulong)numRepetitions;
                }
            }
            else
            {
                // We're seeing this stone for the very first time!
                Dictionary<ulong, int> newStoneRepetitions = new Dictionary<ulong, int>();
                int numDigits = stone.ToString().Length;

                if (stone == 0)
                    newStoneRepetitions.Add(1, 1);
                else if (numDigits % 2 == 0)
                {
                    ulong leftStone = ulong.Parse(stone.ToString().Substring(0, numDigits / 2));
                    ulong rightStone = ulong.Parse(stone.ToString().Substring(numDigits / 2));

                    newStoneRepetitions.Add(leftStone, 1);

                    if (newStoneRepetitions.ContainsKey(rightStone))
                        newStoneRepetitions[rightStone]++;
                    else
                        newStoneRepetitions.Add(rightStone, 1);

                    ret++; // We added a new stone
                }
                else
                    newStoneRepetitions.Add(stone * 2024, 1);

                stoneBlinkIterations.Add(stone, new List<Dictionary<ulong, int>>() { newStoneRepetitions });
                blink--;

                foreach (ulong currStone in newStoneRepetitions.Keys)
                {
                    int numRepetitions = newStoneRepetitions[currStone];
                    ret += GetNumNewStones(currStone, blink) * (ulong)numRepetitions;
                }
            }

            numNewStoneRecords.Add(stoneRecord, ret);
            return ret;
        }
    }
}
