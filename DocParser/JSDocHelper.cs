//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace DocParser
{
    public static class JSDocHelper
    {
        public static JSDocItems GetItems(JSDocSplitSet set)
        {
            var results = new JSDocItems();

            var sb = new StringBuilder();
            var key = string.Empty;

            var lines = GetLines(set.Text);
            foreach (var line in lines)
            {
                if (line.StartsWith("@"))
                {
                    if (!string.IsNullOrWhiteSpace(key) || !string.IsNullOrWhiteSpace(sb.ToString()))
                    {
                        results.List.Add(new KeyValuePair<string, string>(key, sb.ToString().Trim()));
                    }

                    sb.Clear();

                    var iAt = line.IndexOf(' ');
                    if (iAt > -1)
                    {
                        key = line.Substring(1, iAt - 1);
                        sb.Append(line.Substring(iAt + 1) + ' ');
                    }
                    else
                    {
                        // issue: if an @indicator is alone on a line, followed by the actual text, this will miss it.
                        key = line.Substring(1).Trim();
                        results.List.Add(new KeyValuePair<string, string>(key, sb.ToString().Trim()));
                        key = string.Empty;
                    }
                }
                else
                {
                    sb.Append(line + ' ');
                }
            }

            if (!string.IsNullOrWhiteSpace(key) || sb.Length > 0)
            {
                results.List.Add(new KeyValuePair<string, string>(key, sb.ToString().Trim()));
            }

            results.PreviousLine = set.PreviousLine;
            results.NextLine = set.NextLine;

            return results;
        }

        // Assumes text contains /** ... */ commenting. 'text' to contain a single comment block.
        // Returns lines without comments
        public static List<string> GetLines(string text)
        {
            var results = new List<string>();

            var lines = text.Split(new[] { '\n' });
            foreach (var fullline in lines)
            {
                var line = fullline.Trim();
                var iAt = line.IndexOf('*');
                if (iAt > -1)
                {
                    line = line.Substring(iAt + 1).Trim();
                }

                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                results.Add(line);
            }

            return results;
        }

        public static string[] SplitFrom(string text, string from, string between)
        {
            var iAt = text.IndexOf(from);
            if (iAt>-1)
            {
                text = text.Substring(iAt + from.Length);
            }

            return text.Split(new string[] { between }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] Split(string text, string left, string right)
        {
            var list = new List<string>();
            var iAt = -1;
            iAt = text.IndexOf(left, iAt+1);
            while (iAt>-1)
            {
                var iAt2 = text.IndexOf(right, iAt+left.Length);
                if (iAt2>-1)
                {
                    list.Add(text.Substring(iAt+left.Length, iAt2 - iAt - left.Length));
                    iAt = iAt2;
                }

                iAt = text.IndexOf(left, iAt + 1);
            }

            return list.ToArray();
        }

        public static JSDocSplitSet[] SpecialSplit(string text, string left, string right)
        {
            var list = new List<JSDocSplitSet>();
            var iAt = -1;
            iAt = text.IndexOf(left, iAt + 1);
            while (iAt > -1)
            {
                var iAt2 = text.IndexOf(right, iAt + left.Length);
                if (iAt2 > -1)
                {
                    var result = new JSDocSplitSet();

                    if (iAt > 0)
                    {
                        var before = JSDocHelper.PreviousLine(text, iAt);
                        if (before.ToLowerInvariant().Contains("function"))
                        {
                            result.PreviousLine = before;
                        }
                    }

                    if (iAt2 > -1) { 
                        var after = JSDocHelper.NextLine(text, iAt2);
                        if (after.ToLowerInvariant().Contains("function"))
                        {
                            result.NextLine = after;
                        }
                    }

                    result.Text = text.Substring(iAt + left.Length, iAt2 - iAt - left.Length);

                    list.Add(result);
                    iAt = iAt2;
                }

                iAt = text.IndexOf(left, iAt + 1);
            }

            return list.ToArray();
        }

        public static string PreviousLine(string text, int index)
        {
            var iAt = text.LastIndexOf("\n", index);
            if (iAt == -1)
            {
                return string.Empty;
            }

            var line = string.Empty;
            var valid = false;
            while (!valid)
            {
                var iAt2 = text.LastIndexOf("\n", iAt - 1);
                while (iAt2 > -1 && iAt - iAt2 == 1)
                {
                    iAt = iAt2;
                    iAt2 = text.LastIndexOf("\n", iAt - 1);
                }

                if (iAt2 > -1)
                {
                    line = text.Substring(iAt2 + 1, iAt - iAt2).Trim();
                    valid = !string.IsNullOrWhiteSpace(line);
                }
                else
                {
                    break;
                }

                iAt = iAt2;
                iAt2 = text.LastIndexOf("\n", iAt - 1);
            }

            return line;
        }

        public static string NextLine(string text, int index)
        {
            var iAt = text.IndexOf("\n", index);
            if (iAt == -1)
            {
                return text;
            }

            var line = string.Empty;
            var valid = false;
            while (!valid)
            {
                var iAt2 = text.IndexOf("\n", iAt + 1);
                while (iAt2 - iAt == 1)
                {
                    iAt = iAt2;
                    iAt2 = text.IndexOf("\n", iAt + 1);
                }

                if (iAt2 > -1)
                {
                    line = text.Substring(iAt + 1, iAt2 - iAt).Trim();
                    valid = !string.IsNullOrWhiteSpace(line);
                }
                else
                {
                    line = text.Substring(iAt + 1).Trim();
                    valid = !string.IsNullOrWhiteSpace(line);
                    break;
                }

                iAt = iAt2;
                iAt2 = text.IndexOf("\n", iAt + 1);
            }

            return line;
        }

        public static string After(string text, string after, bool wholeIfMissing = true)
        {
            var iAt = text.IndexOf(after);
            return iAt == -1 ? ((wholeIfMissing) ? text : string.Empty) : text.Substring(iAt + after.Length);
        }

        public static string Before(string text, string before, bool wholeIfMissing = true)
        {
            var iAt = text.IndexOf(before);
            return iAt == -1 ? ((wholeIfMissing) ? text : string.Empty) : text.Substring(0, iAt);
        }
    }
}