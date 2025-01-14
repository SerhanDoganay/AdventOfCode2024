using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode2024
{
    class Day17
    {
        private static ulong A;
        private static ulong B;
        private static ulong C;

        private static MemoryStream program;
        private static BinaryReader br;

        public static void main()
        {
            string[] lines = File.ReadAllLines(Program.INPUT_DIR + "input17.txt");
            string[] text = null;

            foreach (string line in lines)
            {
                string[] tokens = line.Split(' ');
                if (tokens.Length > 1)
                {
                    if (tokens[0] == "Register")
                    {
                        uint regValue = uint.Parse(tokens[2]);
                        if (tokens[1] == "A:")
                            A = regValue;
                        else if (tokens[1] == "B:")
                            B = regValue;
                        else if (tokens[1] == "C:")
                            C = regValue;
                    }
                    else if (tokens[0] == "Program:")
                    {
                        text = tokens[1].Split(',');
                        program = new MemoryStream(text.Length);

                        BinaryWriter bw = new BinaryWriter(program);
                        foreach (string inst in text)
                        {
                            bw.Write(byte.Parse(inst));
                        }
                    }
                }
            }

            br = new BinaryReader(program);

            /*
            For part 2, I recognized that the smallest initial value for register A that matched 1-9 values of the output seemed to contain part of a previous level's register A within it,
            So assuming that my final answer would end with "0b0001_0010_1110_1100_0100_1110_0101_0110" (which produced 9 values from the program), I received the value (minTest) to return
            11 values from the program, and so I used that to arrive at the final answer
            */

            //ulong minTest = (ulong)Math.Pow(2, text.Length * 3 - 3); // There is only one instruction that affects A, and if A is a 46-48 bit number, then the program will output the right number of values
            ulong minTest = 23026073833;
            ulong testA = minTest;
            //ulong testA = 0;
            int numOutputs = 0;
            int highScore = -1;
            ulong prefix = 1;
            while (numOutputs < text.Length)
            {
                testA = (prefix++ << 35) | minTest;

                numOutputs = 0;
                A = testA;
                B = 0;
                C = 0;

                br.BaseStream.Position = 0;

                bool quit = false;

                // Run the program
                while (br.BaseStream.Position < br.BaseStream.Length && !quit)
                {
                    // We still have instructions to execute
                    byte opcode = br.ReadByte();
                    switch (opcode)
                    {
                        case 0:
                            // adv
                            A >>= (int)GetComboOperand();
                            break;
                        case 1:
                            // bxl
                            B ^= br.ReadByte();
                            break;
                        case 2:
                            // bst
                            B = GetComboOperand() & 0b111;
                            break;
                        case 3:
                            // jnz
                            byte newOffset = br.ReadByte();
                            if (A != 0)
                                br.BaseStream.Position = newOffset;
                            break;
                        case 4:
                            // bxc
                            B ^= C;
                            br.ReadByte();
                            break;
                        case 5:
                            // out
                            ulong res = GetComboOperand() & 0b111;
                            if (text[numOutputs++] != res.ToString())
                                quit = true;
                            else if (numOutputs > highScore || numOutputs == 14)
                            {
                                highScore = numOutputs;
                                if (numOutputs != 14) prefix--;
                            }

                            break;
                        case 6:
                            // bdv
                            B = A >> (int)GetComboOperand();
                            break;
                        case 7:
                            // cdv
                            C = A >> (int)GetComboOperand();
                            break;
                    }
                }

                testA++;
            }

            Console.WriteLine(testA - 1);
        }

        private static ulong GetComboOperand()
        {
            byte combo = br.ReadByte();
            ulong[] ret = new ulong[] { 0, 1, 2, 3, A, B, C };
            return ret[combo];
        }
    }
}
