using System;
using System.Collections.Generic;
using System.Linq;
using DiffPlex;
using DiffPlex.Model;
using System.Text;

namespace CulturezVous.WP8.Services
{

    public static class TextDiff
    {
        /// <summary>
        /// Make a diff between two strings
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static DiffResult Diff(string a, string b)
        {
            var differ = new Differ();
            //var result = differ.CreateCharacterDiffs(a, b, false, true);

            var result = differ.CreateWordDiffs(a, b, true, true, new char[]{',',' ','\''});

            return result;
        }

        private const string InsertSymbolS = @"<Run Foreground=""{StaticResource PhoneAccentBrush}"" Text=""";
        private const string InsertSymbolE = @""" />";
        private const string DeleteSymbolS = @"<Run Foreground=""{StaticResource PhoneAccentBrush}"" FontWeight=""Bold"" Text=""";
        private const string DeleteSymbolE = @""" />";

        /// <summary>
        /// Format the result for a RichTextBox
        /// </summary>
        /// <param name="lineDiff"></param>
        /// <returns></returns>
        public static string Format(DiffResult lineDiff)
        {
            var uniLines = new StringBuilder();
            int bPos = 0;

            foreach (var diffBlock in lineDiff.DiffBlocks)
            {
                for (; bPos < diffBlock.InsertStartB; bPos++)
                    uniLines.Append(lineDiff.PiecesNew[bPos]);

                int i = 0;
                for (; i < Math.Min(diffBlock.DeleteCountA, diffBlock.InsertCountB); i++)
                {
                    uniLines.Append(DeleteSymbolS + lineDiff.PiecesOld[i + diffBlock.DeleteStartA] + DeleteSymbolE);
                    //uniLines.Append(InsertSymbolS + lineDiff.PiecesNew[i + diffBlock.InsertStartB] + InsertSymbolE);
                    bPos++;
                }

                if (diffBlock.DeleteCountA > diffBlock.InsertCountB)
                {
                    uniLines.Append(DeleteSymbolS);
                    for (; i < diffBlock.DeleteCountA; i++)
                        uniLines.Append(lineDiff.PiecesOld[i + diffBlock.DeleteStartA]);
                    uniLines.Append(DeleteSymbolE);
                }
                else
                {
                    if (i < diffBlock.InsertCountB)
                    {
                        //uniLines.Append(InsertSymbolS);
                        for (; i < diffBlock.InsertCountB; i++)
                        {
                            //uniLines.Append(lineDiff.PiecesNew[i + diffBlock.InsertStartB]);
                            bPos++;
                        }
                        //uniLines.Append(InsertSymbolE);
                    }
                }
            }
            for (; bPos < lineDiff.PiecesNew.Length; bPos++)
                uniLines.Append(lineDiff.PiecesNew[bPos]);

            return uniLines.ToString();
        }
    }


}
