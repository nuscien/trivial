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
    /// The unit test for International Article Number.
    /// </summary>
    [TestClass]
    public class EanUnitTest
    {
        /// <summary>
        /// Tests International Article Number parser.
        /// </summary>
        [TestMethod]
        public void TestEan()
        {
            Assert.IsFalse(InternationalArticleNumber.Validate("abcdefg"));
            Assert.IsFalse(InternationalArticleNumber.Validate("4003994155480"));

            var ean = InternationalArticleNumber.Create("400399415548");
            var bin = ean.ToBarcodeString();
            ean = InternationalArticleNumber.Create(bin);
            var s = "4003994155486";
            Assert.AreEqual(s, ean.ToString());
            Assert.AreEqual(13, ean.ToList().Count);
            Assert.IsTrue(InternationalArticleNumber.Validate(s));

            ean = InternationalArticleNumber.Create("7351353");
            bin = ean.ToBarcodeString();
            ean = InternationalArticleNumber.Create(bin);
            s = "73513537";
            Assert.AreEqual(s, ean.ToString());
            Assert.AreEqual(8, ean.ToList().Count);
            Assert.IsTrue(InternationalArticleNumber.Validate(s));

            s = "52495";
            ean = InternationalArticleNumber.Create(s);
            bin = ean.ToBarcodeString();
            ean = InternationalArticleNumber.Create(bin);
            Assert.AreEqual(s, ean.ToString());
            Assert.AreEqual(5, ean.ToList().Count);
            Assert.IsTrue(InternationalArticleNumber.Validate(s));

            s = "53";
            ean = InternationalArticleNumber.Create(s);
            bin = ean.ToBarcodeString();
            ean = InternationalArticleNumber.Create(bin);
            Assert.AreEqual(s, ean.ToString());
            Assert.AreEqual(2, ean.ToList().Count);
            Assert.IsTrue(InternationalArticleNumber.Validate(s));
        }
    }

    class EanVerb : BaseCommandVerb
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
