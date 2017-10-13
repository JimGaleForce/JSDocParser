using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocParser
{
    public class JSDocNode
    {
        public string Name { get; set; }
        public string Description { get; set; }

        private JSDocNode GetSpecificType(JSDocItems items)
        {

        }

        public static JSDocNode Parse(JSDocSplitSet set)
        {
            var items = JSDocHelper.GetItems(set.Text);

            var result = JSDocNode.GetSpecificType(items);

            foreach (var item in items)
            {
                switch (item.Key)
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
