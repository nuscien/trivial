using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Trivial.Text;

namespace Trivial.Chemistry;

/// <summary>
/// The periodic table of chemical element.
/// </summary>
public partial class ChemicalElement
{
    /// <summary>
    /// Gets chemical element Hydrogen (Hydrogenium).
    /// A colourless, odourless, tasteless, flammable gaseous substance that is the simplest member of the family of chemical elements.
    /// </summary>
    public static ChemicalElement H => Get(1);

    /// <summary>
    /// Gets chemical element Helium.
    /// The second lightest element, helium is a colourless, odourless, and tasteless gas that becomes liquid at −268.9 °C (−452 °F).
    /// </summary>
    public static ChemicalElement He => Get(2);

    /// <summary>
    /// Gets chemical element Lithium.
    /// The lightest of the solid elements. The metal itself—which is soft, white, and lustrous—and several of its alloys and compounds are produced on an industrial scale.
    /// </summary>
    public static ChemicalElement Li => Get(3);

    /// <summary>
    /// Gets chemical element Beryllium, formerly (until 1957AD) glucinium.
    /// </summary>
    public static ChemicalElement Be => Get(4);

    /// <summary>
    /// Gets chemical element Boron.
    /// </summary>
    public static ChemicalElement B => Get(5);

    /// <summary>
    /// Gets chemical element Carbon (Carboneum).
    /// </summary>
    public static ChemicalElement C => Get(6);

    /// <summary>
    /// Gets chemical element Nitrogen (Nitrogenium).
    /// </summary>
    public static ChemicalElement N => Get(7);

    /// <summary>
    /// Gets chemical element Oxygen (Oxygenium).
    /// A colourless, odourless, tasteless gas essential to living organisms, being taken up by animals, which convert it to carbon dioxide.
    /// </summary>
    public static ChemicalElement O => Get(8);

    /// <summary>
    /// Gets chemical element Fluorine (Fluorum).
    /// </summary>
    public static ChemicalElement F => Get(9);

    /// <summary>
    /// Gets chemical element Neon.
    /// </summary>
    public static ChemicalElement Ne => Get(10);

    /// <summary>
    /// Gets chemical element Sodium (Natrium).
    /// A very soft silvery-white metal.
    /// </summary>
    public static ChemicalElement Na => Get(11);

    /// <summary>
    /// Gets chemical element Magnesium.
    /// </summary>
    public static ChemicalElement Mg => Get(12);

    /// <summary>
    /// Gets chemical element Aluminium.
    /// Also spelled aluminium, a lightweight silvery white metal.
    /// </summary>
    public static ChemicalElement Al => Get(13);

    /// <summary>
    /// Gets chemical element Silicon (Silicium).
    /// </summary>
    public static ChemicalElement Si => Get(14);

    /// <summary>
    /// Gets chemical element Phosphorus.
    /// </summary>
    public static ChemicalElement P => Get(15);

    /// <summary>
    /// Gets chemical element Sulfur.
    /// </summary>
    public static ChemicalElement S => Get(16);

    /// <summary>
    /// Gets chemical element Chlorine (Chlorum).
    /// </summary>
    public static ChemicalElement Cl => Get(17);

    /// <summary>
    /// Gets chemical element Argon.
    /// </summary>
    public static ChemicalElement Ar => Get(18);

    /// <summary>
    /// Gets chemical element Potassium (Kalium).
    /// </summary>
    public static ChemicalElement K => Get(19);

    /// <summary>
    /// Gets chemical element Calcium.
    /// </summary>
    public static ChemicalElement Ca => Get(20);

    /// <summary>
    /// Gets chemical element Scandium.
    /// </summary>
    public static ChemicalElement Sc => Get(21);

    /// <summary>
    /// Gets chemical element Titanium.
    /// </summary>
    public static ChemicalElement Ti => Get(22);

    /// <summary>
    /// Gets chemical element Vanadium.
    /// </summary>
    public static ChemicalElement V => Get(23);

    /// <summary>
    /// Gets chemical element Chromium.
    /// </summary>
    public static ChemicalElement Cr => Get(24);

    /// <summary>
    /// Gets chemical element Manganese (Manganum).
    /// </summary>
    public static ChemicalElement Mn => Get(25);

    /// <summary>
    /// Gets chemical element Iron.
    /// </summary>
    public static ChemicalElement Fe => Get(26);

    /// <summary>
    /// Gets chemical element Cobalt (Cobaltum).
    /// </summary>
    public static ChemicalElement Co => Get(27);

    /// <summary>
    /// Gets chemical element Nickel.
    /// </summary>
    public static ChemicalElement Ni => Get(28);

    /// <summary>
    /// Gets chemical element Copper (Cuprum).
    /// </summary>
    public static ChemicalElement Cu => Get(29);

    /// <summary>
    /// Gets chemical element Zinc (Zincum).
    /// </summary>
    public static ChemicalElement Zn => Get(30);

    /// <summary>
    /// Gets chemical element Gallium.
    /// </summary>
    public static ChemicalElement Ga => Get(31);

    /// <summary>
    /// Gets chemical element Germanium.
    /// </summary>
    public static ChemicalElement Ge => Get(32);

    /// <summary>
    /// Gets chemical element Arsenic (Arsenicum).
    /// </summary>
    public static ChemicalElement As => Get(33);

    /// <summary>
    /// Gets chemical element Selenium.
    /// </summary>
    public static ChemicalElement Se => Get(34);

    /// <summary>
    /// Gets chemical element Bromine (Bromum).
    /// </summary>
    public static ChemicalElement Br => Get(35);

    /// <summary>
    /// Gets chemical element Krypton.
    /// </summary>
    public static ChemicalElement Kr => Get(36);

    /// <summary>
    /// Gets chemical element Rubidium.
    /// </summary>
    public static ChemicalElement Rb => Get(37);

    /// <summary>
    /// Gets chemical element Strontium.
    /// </summary>
    public static ChemicalElement Sr => Get(38);

    /// <summary>
    /// Gets chemical element Yttrium.
    /// </summary>
    public static ChemicalElement Y => Get(39);

    /// <summary>
    /// Gets chemical element Zirconium.
    /// </summary>
    public static ChemicalElement Zr => Get(40);

    /// <summary>
    /// Gets chemical element Niobium, formerly Columbium.
    /// </summary>
    public static ChemicalElement Nb => Get(41);

    /// <summary>
    /// Gets chemical element Molybdenum (Molybdaenum).
    /// </summary>
    public static ChemicalElement Mo => Get(42);

    /// <summary>
    /// Gets chemical element Technetium.
    /// </summary>
    public static ChemicalElement Tc => Get(43);

    /// <summary>
    /// Gets chemical element Ruthenium.
    /// </summary>
    public static ChemicalElement Ru => Get(44);

    /// <summary>
    /// Gets chemical element Rhodium.
    /// </summary>
    public static ChemicalElement Rh => Get(45);

    /// <summary>
    /// Gets chemical element Palladium.
    /// </summary>
    public static ChemicalElement Pd => Get(46);

    /// <summary>
    /// Gets chemical element Silver (Argentum).
    /// A white lustrous metal valued for its decorative beauty and electrical conductivity.
    /// </summary>
    public static ChemicalElement Ag => Get(47);

    /// <summary>
    /// Gets chemical element Cadmium.
    /// </summary>
    public static ChemicalElement Cd => Get(48);

    /// <summary>
    /// Gets chemical element Indium.
    /// </summary>
    public static ChemicalElement In => Get(49);

    /// <summary>
    /// Gets chemical element Tin.
    /// </summary>
    public static ChemicalElement Sn => Get(50);

    /// <summary>
    /// Gets chemical element Antimony.
    /// </summary>
    public static ChemicalElement Sb => Get(51);

    /// <summary>
    /// Gets chemical element Tellurium.
    /// </summary>
    public static ChemicalElement Te => Get(52);

    /// <summary>
    /// Gets chemical element Iodine (Iodum).
    /// </summary>
    public static ChemicalElement I => Get(53);

    /// <summary>
    /// Gets chemical element Xenon.
    /// </summary>
    public static ChemicalElement Xe => Get(54);

    /// <summary>
    /// Gets chemical element Caesium.
    /// </summary>
    public static ChemicalElement Cs => Get(55);

    /// <summary>
    /// Gets chemical element Barium.
    /// </summary>
    public static ChemicalElement Ba => Get(56);

    /// <summary>
    /// Gets chemical element Lanthanum.
    /// </summary>
    public static ChemicalElement La => Get(57);

    /// <summary>
    /// Gets chemical element Cerium.
    /// </summary>
    public static ChemicalElement Ce => Get(58);

    /// <summary>
    /// Gets chemical element Praseodymium.
    /// </summary>
    public static ChemicalElement Pr => Get(59);

    /// <summary>
    /// Gets chemical element Neodymium.
    /// </summary>
    public static ChemicalElement Nd => Get(60);

    /// <summary>
    /// Gets chemical element Promethium.
    /// </summary>
    public static ChemicalElement Pm => Get(61);

    /// <summary>
    /// Gets chemical element Samarium.
    /// </summary>
    public static ChemicalElement Sm => Get(62);

    /// <summary>
    /// Gets chemical element Europium.
    /// </summary>
    public static ChemicalElement Eu => Get(63);

    /// <summary>
    /// Gets chemical element Gadolinium.
    /// </summary>
    public static ChemicalElement Gd => Get(64);

    /// <summary>
    /// Gets chemical element Terbium.
    /// </summary>
    public static ChemicalElement Tb => Get(65);

    /// <summary>
    /// Gets chemical element Dysprosium.
    /// </summary>
    public static ChemicalElement Dy => Get(66);

    /// <summary>
    /// Gets chemical element Holmium.
    /// </summary>
    public static ChemicalElement Ho => Get(67);

    /// <summary>
    /// Gets chemical element Erbium.
    /// </summary>
    public static ChemicalElement Er => Get(68);

    /// <summary>
    /// Gets chemical element Thulium.
    /// </summary>
    public static ChemicalElement Tm => Get(69);

    /// <summary>
    /// Gets chemical element Ytterbium.
    /// </summary>
    public static ChemicalElement Yb => Get(70);

    /// <summary>
    /// Gets chemical element Lutetium.
    /// </summary>
    public static ChemicalElement Lu => Get(71);

    /// <summary>
    /// Gets chemical element Hafnium.
    /// </summary>
    public static ChemicalElement Hf => Get(72);

    /// <summary>
    /// Gets chemical element Tantalum.
    /// </summary>
    public static ChemicalElement Ta => Get(73);

    /// <summary>
    /// Gets chemical element Tungsten.
    /// </summary>
    public static ChemicalElement W => Get(74);

    /// <summary>
    /// Gets chemical element Rhenium.
    /// </summary>
    public static ChemicalElement Re => Get(75);

    /// <summary>
    /// Gets chemical element Osmium.
    /// </summary>
    public static ChemicalElement Os => Get(76);

    /// <summary>
    /// Gets chemical element Iridium.
    /// </summary>
    public static ChemicalElement Ir => Get(77);

    /// <summary>
    /// Gets chemical element Platinum.
    /// A very heavy, precious, silver-white metal, platinum is soft and ductile and has a high melting point and good resistance to corrosion and chemical attack.
    /// </summary>
    public static ChemicalElement Pt => Get(78);

    /// <summary>
    /// Gets chemical element Gold (Aurum).
    /// Gold has several qualities that have made it exceptionally valuable throughout history. It is attractive in colour and brightness, durable to the point of virtual indestructibility, highly malleable, and usually found in nature in a comparatively pure form.
    /// </summary>
    public static ChemicalElement Au => Get(79);

    /// <summary>
    /// Gets chemical element Mercury (Hydrargyrum).
    /// </summary>
    public static ChemicalElement Hg => Get(80);

    /// <summary>
    /// Gets chemical element Thallium.
    /// </summary>
    public static ChemicalElement Tl => Get(81);

    /// <summary>
    /// Gets chemical element Lead (Plumbum).
    /// </summary>
    public static ChemicalElement Pb => Get(82);

    /// <summary>
    /// Gets chemical element Bismuth (Bismuthum).
    /// </summary>
    public static ChemicalElement Bi => Get(83);

    /// <summary>
    /// Gets chemical element Polonium.
    /// </summary>
    public static ChemicalElement Po => Get(84);

    /// <summary>
    /// Gets chemical element Astatine (Astatium).
    /// </summary>
    public static ChemicalElement At => Get(85);

    /// <summary>
    /// Gets chemical element Radon.
    /// </summary>
    public static ChemicalElement Rn => Get(86);

    /// <summary>
    /// Gets chemical element Francium.
    /// </summary>
    public static ChemicalElement Fr => Get(87);

    /// <summary>
    /// Gets chemical element Radium.
    /// </summary>
    public static ChemicalElement Ra => Get(88);

    /// <summary>
    /// Gets chemical element Actinium.
    /// </summary>
    public static ChemicalElement Ac => Get(89);

    /// <summary>
    /// Gets chemical element Thorium.
    /// </summary>
    public static ChemicalElement Th => Get(90);

    /// <summary>
    /// Gets chemical element Protactinium.
    /// </summary>
    public static ChemicalElement Pa => Get(91);

    /// <summary>
    /// Gets chemical element Uranium.
    /// </summary>
    public static ChemicalElement U => Get(92);

    /// <summary>
    /// Gets chemical element Neptunium.
    /// </summary>
    public static ChemicalElement Np => Get(93);

    /// <summary>
    /// Gets chemical element Plutonium.
    /// </summary>
    public static ChemicalElement Pu => Get(94);

    /// <summary>
    /// Gets chemical element Americium.
    /// </summary>
    public static ChemicalElement Am => Get(95);

    /// <summary>
    /// Gets chemical element Curium.
    /// </summary>
    public static ChemicalElement Cm => Get(96);

    /// <summary>
    /// Gets chemical element Berkelium.
    /// </summary>
    public static ChemicalElement Bk => Get(97);

    /// <summary>
    /// Gets chemical element Californium.
    /// </summary>
    public static ChemicalElement Cf => Get(98);

    /// <summary>
    /// Gets chemical element Einsteinium.
    /// </summary>
    public static ChemicalElement Es => Get(99);

    /// <summary>
    /// Gets chemical element Fermium.
    /// </summary>
    public static ChemicalElement Fm => Get(100);

    /// <summary>
    /// Gets chemical element Mendelevium.
    /// </summary>
    public static ChemicalElement Md => Get(101);

    /// <summary>
    /// Gets chemical element Nobelium.
    /// </summary>
    public static ChemicalElement No => Get(102);

    /// <summary>
    /// Gets chemical element Lawrencium.
    /// </summary>
    public static ChemicalElement Lr => Get(103);

    /// <summary>
    /// Gets chemical element Rutherfordium.
    /// </summary>
    public static ChemicalElement Rf => Get(104);

    /// <summary>
    /// Gets chemical element Dubnium.
    /// </summary>
    public static ChemicalElement Db => Get(105);

    /// <summary>
    /// Gets chemical element Seaborgium.
    /// </summary>
    public static ChemicalElement Sg => Get(106);

    /// <summary>
    /// Gets chemical element Bohrium.
    /// </summary>
    public static ChemicalElement Bh => Get(107);

    /// <summary>
    /// Gets chemical element Hassium.
    /// </summary>
    public static ChemicalElement Hs => Get(108);

    /// <summary>
    /// Gets chemical element Meitnerium.
    /// </summary>
    public static ChemicalElement Mt => Get(109);

    /// <summary>
    /// Gets chemical element Darmstadtium.
    /// </summary>
    public static ChemicalElement Ds => Get(110);

    /// <summary>
    /// Gets chemical element Roentgenium.
    /// </summary>
    public static ChemicalElement Rg => Get(111);

    /// <summary>
    /// Gets chemical element Copernicium.
    /// </summary>
    public static ChemicalElement Cn => Get(112);

    /// <summary>
    /// Gets chemical element Nihonium.
    /// </summary>
    public static ChemicalElement Nh => Get(113);

    /// <summary>
    /// Gets chemical element Flerovium.
    /// </summary>
    public static ChemicalElement Fl => Get(114);

    /// <summary>
    /// Gets chemical element Moscovium.
    /// </summary>
    public static ChemicalElement Mc => Get(115);

    /// <summary>
    /// Gets chemical element Livermorium.
    /// </summary>
    public static ChemicalElement Lv => Get(116);

    /// <summary>
    /// Gets chemical element Tennessine.
    /// </summary>
    public static ChemicalElement Ts => Get(117);

    /// <summary>
    /// Gets chemical element Oganesson.
    /// </summary>
    public static ChemicalElement Og => Get(118);
}
