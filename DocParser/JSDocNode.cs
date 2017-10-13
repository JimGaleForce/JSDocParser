using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocParser
{
    public class JSDocFile : JSDocNode
    {
    }

    public class JSDocNode
    {
        public string Name { get; set; }
        public string Description { get; set; }

        private static JSDocNode GetSpecificType(JSDocItems items)
        {
            foreach (var item in items.List)
            {
                switch (item.Key)
                {
                    case "function":
                    case "func":
                    case "method":
                    case "name":
                    case "param":
                    case "returns":
                    case "return":
                        return new JSDocFunction();
                    case "file":
                        return new JSDocFile();
                }
            }

            return new JSDocNode();
        }

        internal virtual void Parse(JSDocItems items)
        {   
        }

        public static JSDocNode Parse(JSDocSplitSet set)
        {
            var items = JSDocHelper.GetItems(set);

            var result = JSDocNode.GetSpecificType(items);

            foreach (var item in items.List)
            {
                switch (item.Key)
                {
                    case "name":
                        {
                            if (!string.IsNullOrWhiteSpace(item.Value))
                            {
                                result.Name = item.Value;
                            }

                            break;
                        }
                    case "":
                        {
                            result.Description = item.Value;
                            break;
                        }
                }
            }

            result.Parse(items);

            return result;
        }
    }
}
