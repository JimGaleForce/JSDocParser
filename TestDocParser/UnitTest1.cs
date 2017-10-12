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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using DocParser;

namespace TestDocParser
{

    [TestClass]
    public class UnitTest1
    {
        JSDocParser js;

        [TestInitialize]
        public void Init()
        {
            js = new JSDocParser();
        }

        [TestMethod]
        public void AllUpTest1()
        {
            var data = js.Parse("test1.js");

            Assert.AreEqual(data.Functions.Count, 2);
            Assert.AreEqual(data.Functions[0].Parameters.Count, 2);
            Assert.AreEqual(data.Functions[1].Parameters.Count, 5);
            Assert.AreEqual(data.Functions[0].Name, "createTile");
            Assert.AreEqual(data.Functions[1].Name, "createSecondaryTile");
            Assert.AreEqual(data.Functions[0].Parameters[0].Type, "string");
        }
    }
}
