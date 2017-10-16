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

namespace DocParser
{
    public class JSDocParam
    {
        public string Name { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Default { get; set; }
        public string Description { get; set; }

        public static JSDocParam Parse(string text)
        {
            var result = new JSDocParam();

            var types = JSDocHelper.Split(text, "{", "}");
            if (types.Length > 0)
            {
                result.Type = types[0];
            }

            var nameDescription = JSDocHelper.After(text, "}").Trim();
            if (nameDescription.Length > 0)
            {
                result.Name = JSDocHelper.Before(nameDescription, " ");
                result.Description = JSDocHelper.After(nameDescription, " ");
            }

            return result;
        }
    }
}