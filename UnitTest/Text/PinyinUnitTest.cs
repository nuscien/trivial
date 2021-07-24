using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Trivial.Text
{
    /// <summary>
    /// Pinyin unit test.
    /// </summary>
    [TestClass]
    public class PinyinUnitTest
    {
        /// <summary>
        /// Tests Pinyin.
        /// </summary>
        [TestMethod]
        public void TestPinyin()
        {
            var s = "a'a'a~ Wo3men2 zuo2ri4 doU1zai去 yīQǐ kan4 Wang1Waŋ1ㄨㄤ¯ nE, ni上ne轻?  Ẑesh4 Miao1miAo¯Miao1r ma?";
            s = PinyinMarks.Format(s);
            Assert.AreEqual("A'a'a~ Wǒmén zuórì dōuzài yīqǐ kàn Wāngwāngwāng ne, nǐne? Zheshì Miāomiāomiāor ma?", s);
        }
    }
}
