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
            var s = "Wo3men2 zuo2ri4 doU1zai去 yīqǐ kan4 wang1Waŋ1ㄨㄤ¯ ne, ni上ne轻?  Ẑesh4 miao1miAo¯Miao1r ma?";
            s = PinyinMarks.Format(s);
            Assert.AreEqual("Wǒmén zuórì dōuzài yīqǐ kàn wāngwāngwāng ne, nǐne? Zheshì miāomiāomiāor ma?", s);
        }
    }
}
