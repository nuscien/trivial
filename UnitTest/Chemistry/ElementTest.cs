using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trivial.Text;

namespace Trivial.Chemistry
{
    /// <summary>
    /// Chemical element unit tests.
    /// </summary>
    [TestClass]
    public class ElementTest
    {
        /// <summary>
        /// Tests the chemical element, isotope and periodic table.
        /// </summary>
        [TestMethod]
        public void TestElement()
        {
            Assert.IsNull(ChemicalElement.Get(0));
            Assert.IsNull(ChemicalElement.Get(-1));
            Assert.IsNull(ChemicalElement.Get("Nothing"));
            Assert.IsFalse(new ChemicalElement(-1, null, "Nothing", true).IsValid());
            Assert.IsFalse(ChemicalElement.IsValid("Nothing"));

            var h = ChemicalElement.Get(1);
            Assert.IsTrue(h.IsValid());
            Assert.AreEqual(1, h.AtomicNumber);
            Assert.AreEqual("H", h.Symbol);
            Assert.AreEqual("Hydrogen", h.EnglishName);
            Assert.AreEqual(1, h.Period);
            Assert.AreEqual(1, h.Group);
            Assert.AreEqual(0, h.IndexInPeriod);
            Assert.AreEqual("s", h.Block);
            Assert.AreEqual(h, ChemicalElement.Get("H"));
            Assert.AreEqual(h, ChemicalElement.H);
            Assert.AreEqual(h, new ChemicalElement(1, "H", "No.1", true));
            Assert.IsTrue(h.HasAtomicWeight);
            Assert.IsTrue(h.AtomicWeight >= 1);
            Assert.IsTrue(h.ToString().Contains(h.Symbol));
            Assert.AreEqual("H", ((JsonObject)h).TryGetStringValue("symbol"));
            var d = h.Isotope(2);
            Assert.AreEqual("D", d.ToString());
            Assert.AreEqual(h, d.Element);
            Assert.AreEqual(1, d.AtomicNumber);
            Assert.AreEqual(2, d.AtomicMassNumber);
            Assert.AreEqual(1, d.Neutrons);
            Assert.IsTrue(d.HasAtomicWeight);
            Assert.AreEqual(d, new Isotope(h, 2));
            Assert.IsFalse(h.Isotope(10).HasAtomicWeight);

            var c = ChemicalElement.Get(6);
            Assert.IsTrue(c.IsValid());
            Assert.IsFalse(c == h);
            Assert.IsTrue(c != h);
            Assert.IsTrue(c > h);
            Assert.IsTrue(c >= h);
            Assert.IsFalse(c < h);
            Assert.IsFalse(c <= h);
            Assert.AreNotEqual(h, c);
            Assert.AreEqual(6, c.AtomicNumber);
            Assert.AreEqual("C", c.Symbol);
            Assert.AreEqual("Carbon", c.EnglishName);
            Assert.AreEqual(2, c.Period);
            Assert.AreEqual(14, c.Group);
            Assert.AreEqual(3, c.IndexInPeriod);
            Assert.AreEqual("p", c.Block);
            Assert.AreEqual(c, ChemicalElement.Get("C"));
            Assert.AreEqual(c, ChemicalElement.C);
            Assert.AreEqual(c, new ChemicalElement(6, "C", "No.6", true));
            Assert.IsTrue(c.HasAtomicWeight);
            Assert.IsTrue(c.AtomicWeight >= 12);
            Assert.IsTrue(c.ToString().Contains(c.Symbol));
            Assert.AreEqual(6, ((JsonObject)c).TryGetInt32Value("number"));
            var c12 = c.Isotope(12);
            Assert.AreNotEqual(h, c12);
            Assert.AreEqual(3, c12.ToString().Length);
            Assert.IsTrue(c12.ToString().EndsWith("C"));
            Assert.AreEqual(c, c12.Element);
            Assert.AreEqual(6, c12.AtomicNumber);
            Assert.AreEqual(12, c12.AtomicMassNumber);
            Assert.AreEqual(6, c12.Neutrons);
            Assert.IsTrue(c12.HasAtomicWeight);
            Assert.AreEqual(12, c12.AtomicWeight);
            Assert.AreEqual(c12, new Isotope(6, 12));

            var count = 0;
            foreach (var prop in typeof(ChemicalElement).GetProperties(BindingFlags.Static | BindingFlags.Public))
            {
                if (!prop.CanRead || prop.PropertyType != typeof(ChemicalElement)) continue;
                var ele = (ChemicalElement)prop.GetValue(null);
                Assert.IsNotNull(ele);
                Assert.AreEqual(prop.Name, ele.Symbol);
                Assert.AreEqual(ele, ChemicalElement.Get(ele.AtomicNumber));
                Assert.AreEqual(ele, ChemicalElement.Get(ele.Symbol));
                Assert.IsFalse(string.IsNullOrWhiteSpace(ele.Name));
                count++;
            }

            Assert.IsTrue(count >= 118);
            var usn = ChemicalElement.Get(170);
            Assert.AreNotEqual(h, usn);
            Assert.AreNotEqual(c, usn);
            Assert.AreEqual("Usn", usn.Symbol);
            Assert.AreEqual("Unseptnilium", usn.EnglishName);
            Assert.AreEqual(9, usn.Period);
            Assert.AreEqual(2, usn.Group);
            Assert.AreEqual(1, usn.IndexInPeriod);
            Assert.AreEqual("s", usn.Block);
            Assert.AreEqual(usn, ChemicalElement.Get("Usn"));
            Assert.IsFalse(usn.HasAtomicWeight);
            Assert.IsTrue(double.IsNaN(usn.AtomicWeight));
            Assert.IsTrue(usn.ToString().Contains(usn.Symbol));

            var bbb = ChemicalElement.Get(222);
            Assert.AreEqual("Bbb", bbb.Symbol);
            Assert.AreEqual("i", bbb.Block);

            var one = new ChemicalElement(17, "Seventeen", "A new sample element", true);
            Assert.IsTrue(one.IsValid());
            Assert.AreNotEqual(ChemicalElement.Get(17), one);
        }

        /// <summary>
        /// Tests molecular formula.
        /// </summary>
        [TestMethod]
        public void TestMolecularFormula()
        {
            var s = "NonExist";
            Assert.IsNull(MolecularFormula.TryParse(s));

            s = "H2CO3";
            var m = MolecularFormula.Parse(s);
            Assert.AreEqual(s.Length, m.ToString().Length);
            s = m.ToString();
            Assert.IsTrue(s.Contains("CO"));
            Assert.AreEqual(6, m.Count);
            Assert.AreEqual(0, m.IndexOf("H"));
            Assert.AreEqual(-1, m.IndexOf("H", 2));
            Assert.AreEqual(1, m.IndexOf(ChemicalElement.C));
            Assert.AreEqual(1, m.IndexOf(ChemicalElement.C, 1));
            Assert.AreEqual(1, m.GetCount("C"));
            Assert.AreEqual(3, m.GetCount(ChemicalElement.O));
            Assert.IsFalse(m.IsIon);
            m = MolecularFormula.Parse(s);
            Assert.AreEqual(s, m.ToString());
            Assert.AreEqual(6, m.Count);
            m = new MolecularFormula(m.Items);
            Assert.AreEqual(s, m.ToString());

            s = "SO₄²ˉ";
            m = MolecularFormula.Parse(s);
            Assert.AreEqual(s, m.ToString());
            Assert.IsTrue(m.IsIon);
            Assert.AreEqual(-2, m.ChargeNumber);

            m = MolecularFormula.Parse("R-COOH");
            Assert.AreEqual(2, m.GetCount(ChemicalElement.C));
            Assert.AreEqual(2, m.GetCount(ChemicalElement.O));
            Assert.AreEqual(4, m.GetCount(ChemicalElement.H));

            m = ChemicalElement.Fe * 2 + 3 * ChemicalElement.O;
            Assert.AreEqual(2, m.GetCount("Fe"));
            Assert.AreEqual(3, m.GetCount(ChemicalElement.O));
            m = ChemicalElement.C + ChemicalElement.O;
            Assert.AreEqual(1, m.GetCount("C"));
            Assert.AreEqual(1, m.GetCount(ChemicalElement.O));
            m = m + ChemicalElement.O;
            Assert.AreEqual(1, m.GetCount("C"));
            Assert.AreEqual(2, m.GetCount(ChemicalElement.O));
            m = ChemicalElement.H + m;
            Assert.AreEqual(1, m.GetCount("H"));
            Assert.AreEqual(4, m.Count);

            m = ChemicalElement.H * 2 + ChemicalElement.O;
            var col = new List<MolecularFormula>
            {
                { m, 2, (MolecularFormula)ChemicalElement.Na }
            };
            Assert.AreEqual(3, col.Count);
            Console.WriteLine(MolecularFormula.ToString(col));
            Assert.IsTrue(MolecularFormula.ToString(col).Length > 6);
            col.RemoveAt(2);
            Assert.IsTrue(MolecularFormula.ConservationOfMass(
                new List<MolecularFormula>
                {
                    4 * ChemicalElement.H,
                    ChemicalElement.O * 2
                },
                col));
            Assert.IsFalse(MolecularFormula.ConservationOfMass(
                new List<MolecularFormula>
                {
                    ChemicalElement.C + ChemicalElement.O * 2
                },
                new List<MolecularFormula>
                {
                    (MolecularFormula)ChemicalElement.Na,
                    (MolecularFormula)ChemicalElement.Cl
                }));
            Assert.IsFalse(MolecularFormula.ConservationOfMass(
                new List<MolecularFormula>
                {
                    new MolecularFormula(new List<MolecularFormula.Item>
                    {
                        new MolecularFormula.Item(ChemicalElement.Cl)
                    }, -1)
                },
                new List<MolecularFormula>
                {
                    new MolecularFormula(ChemicalElement.Cl)
                }));
        }

        /// <summary>
        /// Tests molecular formula.
        /// </summary>
        [TestMethod]
        public void TestSerialization()
        {
            var s = "{ \"e\": 6 }";
            var m = JsonSerializer.Deserialize<Model>(s).Element;
            Assert.AreEqual(ChemicalElement.C, m);

            s = "{ \"e\": \"C\" }";
            m = JsonSerializer.Deserialize<Model>(s).Element;
            Assert.AreEqual(ChemicalElement.C, m);

            s = "{ \"e\": { \"symbol\": \"C\" } }";
            m = JsonSerializer.Deserialize<Model>(s).Element;
            Assert.AreEqual(ChemicalElement.C, m);

            s = "{ \"e\": { \"number\": 6 } }";
            m = JsonSerializer.Deserialize<Model>(s).Element;
            Assert.AreEqual(ChemicalElement.C, m);

            s = "{ \"e\": { \"symbol\": \"C\", \"number\": 6 } }";
            m = JsonSerializer.Deserialize<Model>(s).Element;
            Assert.AreEqual(ChemicalElement.C, m);

            s = "{ \"e\": { \"symbol\": \"H\", \"number\": 11 } }";
            m = JsonSerializer.Deserialize<Model>(s).Element;
            Assert.AreNotEqual(ChemicalElement.H, m);
            Assert.AreNotEqual(ChemicalElement.Na, m);
            Assert.AreEqual("H", m.Symbol);
            Assert.AreEqual(11, m.AtomicNumber);
        }
    }

    class Model
    {
        [JsonPropertyName("e")]
        [JsonConverter(typeof(ChemicalElementJsonConverter))]
        public ChemicalElement Element { get; set; }
    }
}
