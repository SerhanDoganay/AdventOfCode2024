using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode2024
{
    class Day9
    {
        public static void main()
        {
            string filesystem = File.ReadAllText(Program.INPUT_DIR + "input9.txt");

            /*
            // Do an initial loop just to get a file length
            int totalFileLength = 0;
            for (int i = 0; i < (filesystem.Length / 2) + 1; i++)
            {
                totalFileLength += int.Parse(filesystem[i * 2].ToString());
            }

            List<int> test = new List<int>();
            long checksum = 0;
            int filesystemPosition = 0;
            int filesMoved = 0;
            int blocksMovedInFile = 0;
            int lastFileIndex = 9999;
            for (int i = 0; i < totalFileLength; ) // i is incremented in the loop
            {
                int fileSize = int.Parse(filesystem[filesystemPosition].ToString());
                int emptySize = int.Parse(filesystem[filesystemPosition + 1].ToString());

                int fileID = filesystemPosition / 2;

                // Have we reached a point where we're at a file that we've already been working with?
                if (lastFileIndex <= filesystemPosition && blocksMovedInFile > 0)
                {
                    emptySize = fileSize - blocksMovedInFile;
                }
                else
                {
                    filesystemPosition += 2; // Prepare to read the next pair of file size / empty size

                    // Calculate the checksum contribution for this file
                    while (fileSize > 0)
                    {
                        test.Add(fileID);

                        checksum += fileID * i;
                        i++; // Advance to the next position
                        fileSize--;
                    }
                }

                // See what we can do with the last file we haven't finished yet and with the empty space we have
                lastFileIndex = filesystem.Length - (filesMoved * 2) - 1;
                while (emptySize > 0 && lastFileIndex >= filesystemPosition)
                {
                    int lastFileID = lastFileIndex / 2;
                    int lastFileSize = int.Parse(filesystem[lastFileIndex].ToString());

                    // What if we already partially took care of this file?
                    lastFileSize -= blocksMovedInFile;

                    int emptySizeLeft = emptySize - lastFileSize;
                    if (emptySizeLeft >= 0)
                    {
                        // We can fit the file here!
                        while (lastFileSize > 0)
                        {
                            test.Add(lastFileID);

                            // Update the checksum
                            checksum += lastFileID * i;
                            i++;
                            lastFileSize--;
                        }

                        // Update how much empty space we have remaining
                        emptySize = emptySizeLeft;
                        blocksMovedInFile = 0;
                        filesMoved++;
                        lastFileIndex = filesystem.Length - (filesMoved * 2) - 1;
                    }
                    else
                    {
                        // We can't fit the file here. Put as much of the file as we can here.
                        while (emptySize > 0)
                        {
                            test.Add(lastFileID);

                            // Update the checksum
                            checksum += lastFileID * i;
                            i++;
                            emptySize--;
                            blocksMovedInFile++;
                        }
                    }
                }
            }

            // Console.WriteLine(new string(test.Select(x => (char)(x + '0')).ToArray()));
            Console.WriteLine(checksum);*/

            // DAY 2
            List<int> fsList = new List<int>();
            List<KeyValuePair<int, int>> emptyData = new List<KeyValuePair<int, int>>(); // The first value is the position in fsList, and the second value is the length
            List<KeyValuePair<int, int>> fileData = new List<KeyValuePair<int, int>>(); // (fileID is built into the fileData index
            for (int i = 0; i < filesystem.Length; i++)
            {
                int size = int.Parse(filesystem[i].ToString());
                int id = -1;
                if (i % 2 == 0)
                {
                    // This is a file
                    id = i / 2;
                    fileData.Add(new KeyValuePair<int, int>(fsList.Count, size));
                }
                else
                {
                    // This is empty data
                    emptyData.Add(new KeyValuePair<int, int>(fsList.Count, size));
                }

                while (size > 0)
                {
                    fsList.Add(id);
                    size--;
                }
            }
            
            for (int i = 0; i < fileData.Count; i++)
            {
                int currFileID = fileData.Count - i - 1; // Go from highest to lowest
                KeyValuePair<int, int> currInfo = fileData[currFileID];

                int pos = currInfo.Key;
                int size = currInfo.Value;

                int emptyID = 0;
                KeyValuePair<int, int> emptyInfo = emptyData[emptyID];
                while (emptyInfo.Key < pos)
                {
                    // There's empty space to the left of this file's data. Is the space big enough for this file?
                    if (emptyInfo.Value >= size)
                    {
                        // YES! Move this file over (changing fileData shouldn't be necessary)
                        int idx = emptyInfo.Key;

                        // Update emptyData
                        emptyData[emptyID] = new KeyValuePair<int, int>(idx + size, emptyInfo.Value - size);

                        // Rewrite fsList
                        while (size > 0)
                        {
                            fsList[idx] = currFileID;
                            size--;
                            idx++;
                        }

                        // Remove the old position in fsList
                        idx = 0;
                        while (idx < currInfo.Value && fsList[pos + idx] == currFileID)
                        {
                            fsList[pos + idx] = -1;
                            idx++;
                        }

                        break;
                    }
                    else
                    {
                        // Not quite. Maybe we'll have better luck with the next empty space?
                        emptyID++;
                        emptyInfo = emptyData[emptyID];
                    }
                }
            }

            long checksum = 0;
            for (int i = 0; i < fsList.Count; i++)
            {
                if (fsList[i] != -1)
                {
                    checksum += fsList[i] * i;
                }
            }
            Console.WriteLine(checksum);
        }
    }
}
