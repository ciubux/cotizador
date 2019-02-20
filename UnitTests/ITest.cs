using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    interface ITest
    {
        [TestMethod]
        void TestCreate();
        [TestMethod]
        void TestUpdate();
        [TestMethod]
        void TestList();
        [TestMethod]
        void TestShow();
    }
}
