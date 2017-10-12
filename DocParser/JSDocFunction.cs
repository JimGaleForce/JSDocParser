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
using System.Diagnostics;
using System.Linq;

namespace DocParser
{
    public class JSDocFunction
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<JSDocParam> Parameters { get; set; } = new List<JSDocParam>();
        public JSDocParam Returns { get; set; }

        public static JSDocFunction Parse(string text, string previousLine, string nextLine)
        {
            var func = new JSDocFunction();

            var items = JSDocHelper.GetItems(text);
            foreach (var item in items)
            {
                switch(item.Key)
                {
                    case "param":
                    {
                        func.Parameters.Add(JSDocParam.Parse(item.Value));
                        break;
                    }
                    case "function":
                    case "func":
                    case "method":
                    case "name":
                    {
                        if (!string.IsNullOrWhiteSpace(item.Value))
                        {
                            func.Name = item.Value;
                        }

                        break;
                    }
                    case "returns":
                    case "return":
                    {
                        func.Returns = JSDocParam.Parse(item.Value);

                        break;
                    }
                    case "":
                    {
                        func.Description = item.Value;
                        break;
                    }
                    default:
                    {
                        Debug.WriteLine("unknown key: " + item.Key);
                        break;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(func.Name))
            {
                var fnLine = (previousLine != null && previousLine.Contains("function")) ? previousLine :
                    (nextLine != null && nextLine.Contains("function")) ? nextLine : string.Empty;

                if (!string.IsNullOrWhiteSpace(fnLine))
                {
                    var index = fnLine.IndexOf("function");
                    if (index > 0)
                    {
                        var before = fnLine.Substring(0, index).Trim();
                        if (!string.IsNullOrWhiteSpace(before))
                        {
                            var iEquals = before.IndexOf("=");
                            var beforeEquals = before.Substring(0, iEquals).Trim();
                            var lastWord = beforeEquals.Split(new[] { ' ' }).LastOrDefault();

                            func.Name = lastWord;
                        }
                    }
                    else
                    {
                        var words = fnLine.Substring(index + 8).Trim().Split(new[] { ' ', '(' });
                        if (words.Length > 0)
                        {
                            func.Name = words[0];
                        }
                    }

                    if (fnLine.Contains("("))
                    {
                        var parmset = JSDocHelper.Split(fnLine, "(", ")");
                        if (parmset.Length > 0)
                        {
                            var parms = parmset[0].Split(new[] { ',' });
                            foreach (var parm in parms)
                            {
                                var segments = parm.Split(new[] { '=', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                if (segments.Length > 1)
                                {
                                    var subparm = func.Parameters.FirstOrDefault(p => p.Name.Equals(segments[0]));
                                    if (subparm != null)
                                    {
                                        subparm.Default = segments[1].Trim();
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return func;
        }
    }
}