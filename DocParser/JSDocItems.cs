using System.Collections.Generic;

namespace DocParser
{
    public class JSDocItems
    {
        public List<KeyValuePair<string,string>> List { get; set; }
        public string PreviousLine { get; set; }
        public string NextLine { get; set; }
    }
}