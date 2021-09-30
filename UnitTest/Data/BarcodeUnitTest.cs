using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trivial.Collection;
using Trivial.CommandLine;

namespace Trivial.Data
{
    /// <summary>
    /// The unit test for barcode.
    /// </summary>
    [TestClass]
    public class BarcodeUnitTest
    {
        /// <summary>
        /// Tests International Article Number parser.
        /// </summary>
        [TestMethod]
        public void TestEan()
        {
            Assert.IsFalse(InternationalArticleNumber.Validate("abcdefg"));
            Assert.IsFalse(InternationalArticleNumber.Validate("4003994155480"));

            // EAN-13
            var ean = InternationalArticleNumber.Create("400399415548");
            var bin = ean.ToBarcodeString();
            ean = InternationalArticleNumber.Create(bin);
            var s = "4003994155486";
            Assert.AreEqual(s, ean.ToString());
            Assert.AreEqual(13, ean.ToList().Count);
            Assert.IsTrue(InternationalArticleNumber.Validate(s));

            // EAN-8
            ean = InternationalArticleNumber.Create("7351353");
            bin = ean.ToBarcodeString();
            ean = InternationalArticleNumber.Create(bin);
            s = "73513537";
            Assert.AreEqual(s, ean.ToString());
            Assert.AreEqual(8, ean.ToList().Count);
            Assert.IsTrue(InternationalArticleNumber.Validate(s));

            // EAN-5
            s = "52495";
            ean = InternationalArticleNumber.Create(s);
            bin = ean.ToBarcodeString();
            ean = InternationalArticleNumber.Create(bin);
            Assert.AreEqual(s, ean.ToString());
            Assert.AreEqual(5, ean.ToList().Count);
            Assert.IsTrue(InternationalArticleNumber.Validate(s));

            // EAN-2
            s = "53";
            ean = InternationalArticleNumber.Create(s);
            bin = ean.ToBarcodeString();
            ean = InternationalArticleNumber.Create(bin);
            Assert.AreEqual(s, ean.ToString());
            Assert.AreEqual(2, ean.ToList().Count);
            Assert.IsTrue(InternationalArticleNumber.Validate(s));

            // Code 128
            var code128 = Code128.CreateB(new byte[] { 43, 73, 78, 71, 67, 69, 65, 78 });
            Assert.AreEqual("Kingcean", code128.ToString());
            code128 = Code128.CreateA("Kingcean");
            Assert.AreEqual("Kingcean", code128.ToString());

            // Code 128 high character
            code128 = Code128.CreateB(new byte[] { 52, 100, 52, 52, 100, 100, 52, 52, 100, 52, 52, 100, 100, 52, 100, 100, 101, 52 });
            Assert.AreEqual("TÔTÔÔTÔTÔ", code128.ToString());

            // GS1-128
            code128 = Code128.CreateC(new byte[] { 102, 42, 18, 40, 20, 50, 101, 16 });
            Assert.AreEqual("[FNC1]42184020500", code128.ToString());
            var ai = code128.GetAiData();
            Assert.AreEqual("42184020500", ai.First());
            code128 = Code128.CreateGs1(421, "84020500");
            Assert.AreEqual("[Start C] [FNC1] 42 18 40 20 50 [Code A] 16 [Check symbol 92] [Stop]", code128.ToString(Code128.Formats.Values));
            Assert.AreEqual((byte)92, code128.Skip(code128.Count - 2).ToList()[0]);
            ai = code128.GetAiData();
            Assert.AreEqual("42184020500", ai.First());

            // Code 128 combination
            code128 += Code128.Gs1Generator.BankAccount("100016") + Code128.CreateA(" Something... ") + Code128.Join(new List<Code128>
            {
                Code128.CreateA("And "),
                Code128.CreateC(123),
                Code128.Gs1Generator.Sn("01234567890123456789")
            });
            Assert.AreEqual("[FNC1]42184020500[FNC1]8007100016 Something... And 123[FNC1]2101234567890123456789", code128.ToString());
        }
    }

    class BarcodeVerb : BaseCommandVerb
    {
        public static string Description => "International Article Number";

        /// <inheritdoc />
        protected override async Task OnProcessAsync(CancellationToken cancellationToken = default)
        {
            await RunAsync(null, cancellationToken);
            var s = Arguments.Verb.TryGet(0);
            if (string.IsNullOrWhiteSpace(s))
            {
                DefaultConsole.Write("Please type an EAN-13 code: ");
                s = DefaultConsole.ReadLine();
                if (string.IsNullOrWhiteSpace(s)) return;
            }
            //else if (s == "128A" || s == "128a" || s == "128B" || s == "128b" || s == "128C" || s == "128c")
            //{
            //    var c = Enum.Parse<Code128.Subtypes>(s.ToUpperInvariant()[3].ToString());
            //    s = Arguments.Verb.TryGet(1);
            //    if (string.IsNullOrWhiteSpace(s))
            //    {
            //        DefaultConsole.Write("Please type an Code 128: ");
            //        s = DefaultConsole.ReadLine();
            //        if (string.IsNullOrWhiteSpace(s)) return;
            //    }

            //    var code128 = Code128.Create(c, s);
            //}

            try
            {
                var ean = InternationalArticleNumber.Create(s);
                ean.ToBarcode(StyleConsole.Default);
            }
            catch (InvalidOperationException ex)
            {
                DefaultConsole.WriteLine(ex);
            }
            catch (ArgumentException ex)
            {
                DefaultConsole.WriteLine(ex);
            }
        }
    }
}
