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
    /// Alphabet unit test.
    /// </summary>
    [TestClass]
    public class AlphabetUnitTest
    {
        /// <summary>
        /// Tests Latin.
        /// </summary>
        [TestMethod]
        public void TestLatin()
        {
            Assert.AreEqual(LatinAlphabet.English.Count, LatinAlphabet.English.CapitalLetters().Count());
            Assert.AreEqual(LatinAlphabet.English.Count, LatinAlphabet.English.SmallLetters().Count());
            Assert.AreEqual(LatinAlphabet.French.Count, LatinAlphabet.French.CapitalLetters().Count());
            Assert.AreEqual(LatinAlphabet.French.Count, LatinAlphabet.French.SmallLetters().Count());
            Assert.AreEqual(LatinAlphabet.German.Count, LatinAlphabet.German.CapitalLetters().Count());
            Assert.AreEqual(LatinAlphabet.German.Count, LatinAlphabet.German.SmallLetters().Count());
            Assert.AreEqual(LatinAlphabet.Spanish.Count, LatinAlphabet.Spanish.CapitalLetters().Count());
            Assert.AreEqual(LatinAlphabet.Spanish.Count, LatinAlphabet.Spanish.SmallLetters().Count());
            Assert.AreEqual(LatinAlphabet.Portuguese.Count, LatinAlphabet.Portuguese.CapitalLetters().Count());
            Assert.AreEqual(LatinAlphabet.Portuguese.Count, LatinAlphabet.Portuguese.SmallLetters().Count());
            Assert.AreEqual(LatinAlphabet.Italian.Count, LatinAlphabet.Italian.CapitalLetters().Count());
            Assert.AreEqual(LatinAlphabet.Italian.Count, LatinAlphabet.Italian.SmallLetters().Count());
            Assert.AreEqual(LatinAlphabet.Dutch.Count, LatinAlphabet.Dutch.CapitalLetters().Count());
            Assert.AreEqual(LatinAlphabet.Dutch.Count, LatinAlphabet.Dutch.SmallLetters().Count());
            Assert.AreEqual(LatinAlphabet.Esperanto.Count, LatinAlphabet.Esperanto.CapitalLetters().Count());
            Assert.AreEqual(LatinAlphabet.Esperanto.Count, LatinAlphabet.Esperanto.SmallLetters().Count());
        }

        /// <summary>
        /// Tests Pinyin.
        /// </summary>
        [TestMethod]
        public void TestPinyin()
        {
            var s = "a'a'a~ Wo3men2 zuo2ri4 doU1zai去 yīQǐ kan4 Wang1Waŋ1ㄨㄤ¯ nE, ni上ne轻?  Ẑesh4 Miao1miAo¯Miao1r ma? ňg hng2.";
            s = PinyinMarks.Format(s);
            Assert.AreEqual("A'a'a~ Wǒmén zuórì dōuzài yīqǐ kàn Wāngwāngwāng ne, nǐne? Zheshì Miāomiāomiāor ma? Ňg hńg.", s);
        }
    }
}
