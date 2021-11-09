using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Trivial.Text;

namespace Trivial.Chemistry
{
    /// <summary>
    /// The periodic table of chemical element.
    /// </summary>
    public partial class ChemicalElement
    {
        /// <summary>
        /// Gets Hydrogen.
        /// </summary>
        public static ChemicalElement H => Get(1);

        /// <summary>
        /// Gets Helium.
        /// </summary>
        public static ChemicalElement He => Get(2);

        /// <summary>
        /// Gets Lithium.
        /// </summary>
        public static ChemicalElement Li => Get(3);

        /// <summary>
        /// Gets Beryllium.
        /// </summary>
        public static ChemicalElement Be => Get(4);

        /// <summary>
        /// Gets Boron.
        /// </summary>
        public static ChemicalElement B => Get(5);

        /// <summary>
        /// Gets Carbon.
        /// </summary>
        public static ChemicalElement C => Get(6);

        /// <summary>
        /// Gets Nitrogen.
        /// </summary>
        public static ChemicalElement N => Get(7);

        /// <summary>
        /// Gets Oxygen.
        /// </summary>
        public static ChemicalElement O => Get(8);

        /// <summary>
        /// Gets Fluorine.
        /// </summary>
        public static ChemicalElement F => Get(9);

        /// <summary>
        /// Gets Neon.
        /// </summary>
        public static ChemicalElement Ne => Get(10);

        /// <summary>
        /// Gets Sodium.
        /// </summary>
        public static ChemicalElement Na => Get(11);

        /// <summary>
        /// Gets Magnesium.
        /// </summary>
        public static ChemicalElement Mg => Get(12);

        /// <summary>
        /// Gets Aluminium.
        /// </summary>
        public static ChemicalElement Al => Get(13);

        /// <summary>
        /// Gets Silicon.
        /// </summary>
        public static ChemicalElement Si => Get(14);

        /// <summary>
        /// Gets Phosphorus.
        /// </summary>
        public static ChemicalElement P => Get(15);

        /// <summary>
        /// Gets Sulfur.
        /// </summary>
        public static ChemicalElement S => Get(16);

        /// <summary>
        /// Gets Chlorine.
        /// </summary>
        public static ChemicalElement Cl => Get(17);

        /// <summary>
        /// Gets Argon.
        /// </summary>
        public static ChemicalElement Ar => Get(18);

        /// <summary>
        /// Gets Potassium.
        /// </summary>
        public static ChemicalElement K => Get(19);

        /// <summary>
        /// Gets Calcium.
        /// </summary>
        public static ChemicalElement Ca => Get(20);

        /// <summary>
        /// Gets Scandium.
        /// </summary>
        public static ChemicalElement Sc => Get(21);

        /// <summary>
        /// Gets Titanium.
        /// </summary>
        public static ChemicalElement Ti => Get(22);

        /// <summary>
        /// Gets Vanadium.
        /// </summary>
        public static ChemicalElement V => Get(23);

        /// <summary>
        /// Gets Chromium.
        /// </summary>
        public static ChemicalElement Cr => Get(24);

        /// <summary>
        /// Gets Manganese.
        /// </summary>
        public static ChemicalElement Mn => Get(25);

        /// <summary>
        /// Gets Iron.
        /// </summary>
        public static ChemicalElement Fe => Get(26);

        /// <summary>
        /// Gets Cobalt.
        /// </summary>
        public static ChemicalElement Co => Get(27);

        /// <summary>
        /// Gets Nickel.
        /// </summary>
        public static ChemicalElement Ni => Get(28);

        /// <summary>
        /// Gets Copper.
        /// </summary>
        public static ChemicalElement Cu => Get(29);

        /// <summary>
        /// Gets Zinc.
        /// </summary>
        public static ChemicalElement Zn => Get(30);

        /// <summary>
        /// Gets Gallium.
        /// </summary>
        public static ChemicalElement Ga => Get(31);

        /// <summary>
        /// Gets Germanium.
        /// </summary>
        public static ChemicalElement Ge => Get(32);

        /// <summary>
        /// Gets Arsenic.
        /// </summary>
        public static ChemicalElement As => Get(33);

        /// <summary>
        /// Gets Selenium.
        /// </summary>
        public static ChemicalElement Se => Get(34);

        /// <summary>
        /// Gets Bromine.
        /// </summary>
        public static ChemicalElement Br => Get(35);

        /// <summary>
        /// Gets Krypton.
        /// </summary>
        public static ChemicalElement Kr => Get(36);

        /// <summary>
        /// Gets Rubidium.
        /// </summary>
        public static ChemicalElement Rb => Get(37);

        /// <summary>
        /// Gets Strontium.
        /// </summary>
        public static ChemicalElement Sr => Get(38);

        /// <summary>
        /// Gets Yttrium.
        /// </summary>
        public static ChemicalElement Y => Get(39);

        /// <summary>
        /// Gets Zirconium.
        /// </summary>
        public static ChemicalElement Zr => Get(40);

        /// <summary>
        /// Gets Niobium.
        /// </summary>
        public static ChemicalElement Nb => Get(41);

        /// <summary>
        /// Gets Molybdenum.
        /// </summary>
        public static ChemicalElement Mo => Get(42);

        /// <summary>
        /// Gets Technetium.
        /// </summary>
        public static ChemicalElement Tc => Get(43);

        /// <summary>
        /// Gets Ruthenium.
        /// </summary>
        public static ChemicalElement Ru => Get(44);

        /// <summary>
        /// Gets Rhodium.
        /// </summary>
        public static ChemicalElement Rh => Get(45);

        /// <summary>
        /// Gets Palladium.
        /// </summary>
        public static ChemicalElement Pd => Get(46);

        /// <summary>
        /// Gets Silver.
        /// </summary>
        public static ChemicalElement Ag => Get(47);

        /// <summary>
        /// Gets Cadmium.
        /// </summary>
        public static ChemicalElement Cd => Get(48);

        /// <summary>
        /// Gets Indium.
        /// </summary>
        public static ChemicalElement In => Get(49);

        /// <summary>
        /// Gets Tin.
        /// </summary>
        public static ChemicalElement Sn => Get(50);

        /// <summary>
        /// Gets Antimony.
        /// </summary>
        public static ChemicalElement Sb => Get(51);

        /// <summary>
        /// Gets Tellurium.
        /// </summary>
        public static ChemicalElement Te => Get(52);

        /// <summary>
        /// Gets Iodine.
        /// </summary>
        public static ChemicalElement I => Get(53);

        /// <summary>
        /// Gets Xenon.
        /// </summary>
        public static ChemicalElement Xe => Get(54);

        /// <summary>
        /// Gets Caesium.
        /// </summary>
        public static ChemicalElement Cs => Get(55);

        /// <summary>
        /// Gets Barium.
        /// </summary>
        public static ChemicalElement Ba => Get(56);

        /// <summary>
        /// Gets Lanthanum.
        /// </summary>
        public static ChemicalElement La => Get(57);

        /// <summary>
        /// Gets Cerium.
        /// </summary>
        public static ChemicalElement Ce => Get(58);

        /// <summary>
        /// Gets Praseodymium.
        /// </summary>
        public static ChemicalElement Pr => Get(59);

        /// <summary>
        /// Gets Neodymium.
        /// </summary>
        public static ChemicalElement Nd => Get(60);

        /// <summary>
        /// Gets Promethium.
        /// </summary>
        public static ChemicalElement Pm => Get(61);

        /// <summary>
        /// Gets Samarium.
        /// </summary>
        public static ChemicalElement Sm => Get(62);

        /// <summary>
        /// Gets Europium.
        /// </summary>
        public static ChemicalElement Eu => Get(63);

        /// <summary>
        /// Gets Gadolinium.
        /// </summary>
        public static ChemicalElement Gd => Get(64);

        /// <summary>
        /// Gets Terbium.
        /// </summary>
        public static ChemicalElement Tb => Get(65);

        /// <summary>
        /// Gets Dysprosium.
        /// </summary>
        public static ChemicalElement Dy => Get(66);

        /// <summary>
        /// Gets Holmium.
        /// </summary>
        public static ChemicalElement Ho => Get(67);

        /// <summary>
        /// Gets Erbium.
        /// </summary>
        public static ChemicalElement Er => Get(68);

        /// <summary>
        /// Gets Thulium.
        /// </summary>
        public static ChemicalElement Tm => Get(69);

        /// <summary>
        /// Gets Ytterbium.
        /// </summary>
        public static ChemicalElement Yb => Get(70);

        /// <summary>
        /// Gets Lutetium.
        /// </summary>
        public static ChemicalElement Lu => Get(71);

        /// <summary>
        /// Gets Hafnium.
        /// </summary>
        public static ChemicalElement Hf => Get(72);

        /// <summary>
        /// Gets Tantalum.
        /// </summary>
        public static ChemicalElement Ta => Get(73);

        /// <summary>
        /// Gets Tungsten.
        /// </summary>
        public static ChemicalElement W => Get(74);

        /// <summary>
        /// Gets Rhenium.
        /// </summary>
        public static ChemicalElement Re => Get(75);

        /// <summary>
        /// Gets Osmium.
        /// </summary>
        public static ChemicalElement Os => Get(76);

        /// <summary>
        /// Gets Iridium.
        /// </summary>
        public static ChemicalElement Ir => Get(77);

        /// <summary>
        /// Gets Platinum.
        /// </summary>
        public static ChemicalElement Pt => Get(78);

        /// <summary>
        /// Gets Gold.
        /// </summary>
        public static ChemicalElement Au => Get(79);

        /// <summary>
        /// Gets Mercury.
        /// </summary>
        public static ChemicalElement Hg => Get(80);

        /// <summary>
        /// Gets Thallium.
        /// </summary>
        public static ChemicalElement Tl => Get(81);

        /// <summary>
        /// Gets Lead.
        /// </summary>
        public static ChemicalElement Pb => Get(82);

        /// <summary>
        /// Gets Bismuth.
        /// </summary>
        public static ChemicalElement Bi => Get(83);

        /// <summary>
        /// Gets Polonium.
        /// </summary>
        public static ChemicalElement Po => Get(84);

        /// <summary>
        /// Gets Astatine.
        /// </summary>
        public static ChemicalElement At => Get(85);

        /// <summary>
        /// Gets Radon.
        /// </summary>
        public static ChemicalElement Rn => Get(86);

        /// <summary>
        /// Gets Francium.
        /// </summary>
        public static ChemicalElement Fr => Get(87);

        /// <summary>
        /// Gets Radium.
        /// </summary>
        public static ChemicalElement Ra => Get(88);

        /// <summary>
        /// Gets Actinium.
        /// </summary>
        public static ChemicalElement Ac => Get(89);

        /// <summary>
        /// Gets Thorium.
        /// </summary>
        public static ChemicalElement Th => Get(90);

        /// <summary>
        /// Gets Protactinium.
        /// </summary>
        public static ChemicalElement Pa => Get(91);

        /// <summary>
        /// Gets Uranium.
        /// </summary>
        public static ChemicalElement U => Get(92);

        /// <summary>
        /// Gets Neptunium.
        /// </summary>
        public static ChemicalElement Np => Get(93);

        /// <summary>
        /// Gets Plutonium.
        /// </summary>
        public static ChemicalElement Pu => Get(94);

        /// <summary>
        /// Gets Americium.
        /// </summary>
        public static ChemicalElement Am => Get(95);

        /// <summary>
        /// Gets Curium.
        /// </summary>
        public static ChemicalElement Cm => Get(96);

        /// <summary>
        /// Gets Berkelium.
        /// </summary>
        public static ChemicalElement Bk => Get(97);

        /// <summary>
        /// Gets Californium.
        /// </summary>
        public static ChemicalElement Cf => Get(98);

        /// <summary>
        /// Gets Einsteinium.
        /// </summary>
        public static ChemicalElement Es => Get(99);

        /// <summary>
        /// Gets Fermium.
        /// </summary>
        public static ChemicalElement Fm => Get(100);

        /// <summary>
        /// Gets Mendelevium.
        /// </summary>
        public static ChemicalElement Md => Get(101);

        /// <summary>
        /// Gets Nobelium.
        /// </summary>
        public static ChemicalElement No => Get(102);

        /// <summary>
        /// Gets Lawrencium.
        /// </summary>
        public static ChemicalElement Lr => Get(103);

        /// <summary>
        /// Gets Rutherfordium.
        /// </summary>
        public static ChemicalElement Rf => Get(104);

        /// <summary>
        /// Gets Dubnium.
        /// </summary>
        public static ChemicalElement Db => Get(105);

        /// <summary>
        /// Gets Seaborgium.
        /// </summary>
        public static ChemicalElement Sg => Get(106);

        /// <summary>
        /// Gets Bohrium.
        /// </summary>
        public static ChemicalElement Bh => Get(107);

        /// <summary>
        /// Gets Hassium.
        /// </summary>
        public static ChemicalElement Hs => Get(108);

        /// <summary>
        /// Gets Meitnerium.
        /// </summary>
        public static ChemicalElement Mt => Get(109);

        /// <summary>
        /// Gets Darmstadtium.
        /// </summary>
        public static ChemicalElement Ds => Get(110);

        /// <summary>
        /// Gets Roentgenium.
        /// </summary>
        public static ChemicalElement Rg => Get(111);

        /// <summary>
        /// Gets Copernicium.
        /// </summary>
        public static ChemicalElement Cn => Get(112);

        /// <summary>
        /// Gets Nihonium.
        /// </summary>
        public static ChemicalElement Nh => Get(113);

        /// <summary>
        /// Gets Flerovium.
        /// </summary>
        public static ChemicalElement Fl => Get(114);

        /// <summary>
        /// Gets Moscovium.
        /// </summary>
        public static ChemicalElement Mc => Get(115);

        /// <summary>
        /// Gets Livermorium.
        /// </summary>
        public static ChemicalElement Lv => Get(116);

        /// <summary>
        /// Gets Tennessine.
        /// </summary>
        public static ChemicalElement Ts => Get(117);

        /// <summary>
        /// Gets Oganesson.
        /// </summary>
        public static ChemicalElement Og => Get(118);
    }
}
