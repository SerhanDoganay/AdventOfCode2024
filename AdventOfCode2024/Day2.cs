using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AdventOfCode2024
{
    class Day2
    {
        public enum ReportError
        {
            VALID_REPORT,
            WRONG_DIRECTION, // start ascending then descend or vice versa
            NO_STEP, // e.g. "40 40"
            LARGE_STEP // "e.g. "40 47"
        }

        public static void main()
        {
            string[] lines = File.ReadAllLines(Program.INPUT_DIR + "input2.txt");

            int numSafeReports = 0;
            foreach (string report in lines)
            {
                string[] levelStrings = report.Split(' ');
                if (levelStrings.Length > 1)
                {
                    // We have a valid line
                    List<int> levels = levelStrings.Select(x => int.Parse(x)).ToList(); // Extract the levels from the report
                    int problemIndex; // Contains the index of the level that caused the problem, if there is one

                    ReportError reportValidity = IsReportValid(levels, out problemIndex);
                    if (reportValidity == ReportError.VALID_REPORT)
                        numSafeReports++;
                    else
                    {
                        List<int> newLevels = new List<int>(levels);
                        int oldProblemIndex = problemIndex;

                        // Diagnose the error
                        if (reportValidity == ReportError.WRONG_DIRECTION)
                        {
                            // Remove the element at problemIndex. If problemIndex = 2, then perhaps our initial assumption of the direction was wrong
                            newLevels.RemoveAt(problemIndex);
                            if (IsReportValid(newLevels, out problemIndex) == ReportError.VALID_REPORT)
                                numSafeReports++;
                            else if (oldProblemIndex == 2)
                            {
                                // Perhaps try removing the first element
                                newLevels = new List<int>(levels);
                                newLevels.RemoveAt(0);
                                if (IsReportValid(newLevels, out problemIndex) == ReportError.VALID_REPORT)
                                    numSafeReports++;
                                else
                                {
                                    // Otherwise, try the element in between
                                    newLevels = new List<int>(levels);
                                    newLevels.RemoveAt(1);
                                    if (IsReportValid(newLevels, out problemIndex) == ReportError.VALID_REPORT)
                                        numSafeReports++;
                                }
                            }
                        }
                        else if (reportValidity == ReportError.NO_STEP)
                        {
                            // Remove the element at problemIndex.
                            newLevels.RemoveAt(problemIndex);
                            if (IsReportValid(newLevels, out problemIndex) == ReportError.VALID_REPORT)
                                numSafeReports++;
                        }
                        else
                        {
                            // We have a large step; either remove the element at problemIndex, or the one before it.
                            newLevels.RemoveAt(problemIndex - 1);
                            if (IsReportValid(newLevels, out problemIndex) == ReportError.VALID_REPORT)
                                numSafeReports++;
                            else
                            {
                                // Perhaps try removing the previous element instead
                                newLevels = new List<int>(levels);
                                newLevels.RemoveAt(oldProblemIndex);
                                if (IsReportValid(newLevels, out problemIndex) == ReportError.VALID_REPORT)
                                    numSafeReports++;
                            }
                        }
                    }
                }
            }

            Console.WriteLine(numSafeReports);
        }

        public static ReportError IsReportValid(List<int> levels, out int problemIndex)
        {
            problemIndex = -1; // Assume no problem was encountered
            int previousLevel = -1;
            bool? isAscending = null; // null = no pattern established
            bool validReport = true; // Assume the report is valid by default
            ReportError ret = ReportError.VALID_REPORT; // Assume the report is valid by default
            for (int i = 0; i < levels.Count && validReport; i++)
            {
                if (previousLevel != -1)
                {
                    // Compare with the previous level
                    int diff = levels[i] - previousLevel;

                    if (isAscending == null)
                        isAscending = diff > 0; // If the current level - previous level is positive, then we are ascending. Otherwise, we are descending

                    bool rightDirection = isAscending.Value == diff > 0; // Are we maintaining an ascending/descending pattern?
                    bool minStep = Math.Abs(diff) >= 1; // Do we not have a 0-difference?
                    bool maxStep = Math.Abs(diff) <= 3; // Do we not have too large of a difference?

                    validReport = rightDirection && minStep && maxStep;

                    if (!validReport)
                    {
                        problemIndex = i; // This element ruined the report
                        // Determine the cause of the error (wrong direction is higher priority than no/large step
                        if (!rightDirection)
                            ret = ReportError.WRONG_DIRECTION;
                        else if (!minStep)
                            ret = ReportError.NO_STEP;
                        else
                            ret = ReportError.LARGE_STEP;
                    }
                }
                previousLevel = levels[i];
            }

            return ret;
        }
    }
}
