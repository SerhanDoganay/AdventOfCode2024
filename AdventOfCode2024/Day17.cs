using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AdventOfCode2024
{
    class Day17
    {
        private static uint A;
        private static uint B;
        private static uint C;

        private static MemoryStream program;
        private static BinaryReader br;

        public static void main()
        {
            string[] lines = File.ReadAllLines(Program.INPUT_DIR + "input17.txt");

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
                        string[] text = tokens[1].Split(',');
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
            br.BaseStream.Position = 0;

            bool firstOutput = true;

            // Run the program
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                // We still have instructions to execute
                uint opcode = br.ReadByte();
                switch (opcode)
                {
                    case 0:
                        // adv
                        A /= (uint)Math.Pow(2, GetComboOperand());
                        break;
                    case 1:
                        // bxl
                        B ^= br.ReadByte();
                        break;
                    case 2:
                        // bst
                        B = GetComboOperand() % 8;
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
                        if (firstOutput)
                            firstOutput = false;
                        else
                            Console.Write(',');
                        Console.Write((GetComboOperand() % 8));
                        break;
                    case 6:
                        // bdv
                        B = A / (uint)Math.Pow(2, GetComboOperand());
                        break;
                    case 7:
                        // cdv
                        C = A / (uint)Math.Pow(2, GetComboOperand());
                        break;
                }
            }
        }

        private static uint GetComboOperand()
        {
            uint combo = br.ReadByte();
            uint[] ret = new uint[] { 0, 1, 2, 3, A, B, C };
            return ret[combo];
        }
    }
}
