using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Text
{
    /// <summary>
    /// The Esperanto alphabet.
    /// </summary>
    public static class EsperantoAlphabet
    {
        /// <summary>
        /// The name of greek alphabet in Greek.
        /// </summary>
        public const string Name = "Esperanto";

        /// <summary>
        /// The language code.
        /// </summary>
        public const string LanguageCode = "epo";

        /// <summary>
        /// The capital letter ami (/a/).
        /// </summary>
        public const string CapitalAmi = "A";

        /// <summary>
        /// The small letter ami (/a/).
        /// </summary>
        public const string SmallAmi = "a";

        /// <summary>
        /// The capital letter bela (/b/).
        /// </summary>
        public const string CapitalBela = "B";

        /// <summary>
        /// The small letter bela (/b/).
        /// </summary>
        public const string SmallBela = "b";

        /// <summary>
        /// The capital letter celo (/ts/).
        /// </summary>
        public const string CapitalCelo = "C";

        /// <summary>
        /// The small letter celo (/ts/).
        /// </summary>
        public const string SmallCelo = "c";

        /// <summary>
        /// The capital letter ĉokolado (/tʃ/).
        /// </summary>
        public const string CapitalĈokolado = "Ĉ";

        /// <summary>
        /// The small letter ĉokolado (/tʃ/).
        /// </summary>
        public const string SmallĈokolado = "ĉ";

        /// <summary>
        /// The capital letter doni (/d/).
        /// </summary>
        public const string CapitalDoni = "D";

        /// <summary>
        /// The small letter doni (/d/).
        /// </summary>
        public const string SmallDoni = "d";

        /// <summary>
        /// The capital letter egala (/e/).
        /// </summary>
        public const string CapitalEgala = "E";

        /// <summary>
        /// The small letter egala (/e/).
        /// </summary>
        public const string SmallEgala = "e";

        /// <summary>
        /// The capital letter facila (/f/).
        /// </summary>
        public const string CapitalFacila = "F";

        /// <summary>
        /// The small letter facila (/f/).
        /// </summary>
        public const string SmallFacila = "f";

        /// <summary>
        /// The capital letter granda (/g/).
        /// </summary>
        public const string CapitalGranda = "G";

        /// <summary>
        /// The small letter granda (/g/).
        /// </summary>
        public const string SmallGranda = "g";

        /// <summary>
        /// The capital letter ĝui (/dʒ/).
        /// </summary>
        public const string CapitalĜui = "Ĝ";

        /// <summary>
        /// The small letter ĝui (/dʒ/).
        /// </summary>
        public const string SmallĜui = "ĝ";

        /// <summary>
        /// The capital letter horo (/h/).
        /// </summary>
        public const string CapitalHoro = "H";

        /// <summary>
        /// The small letter horo (/h/).
        /// </summary>
        public const string SmallHoro = "h";

        /// <summary>
        /// The capital letter ĥoro (/x/).
        /// </summary>
        public const string CapitalĤoro = "Ĥ";

        /// <summary>
        /// The small letter ĥoro (/x/).
        /// </summary>
        public const string SmallĤoro = "ĥ";

        /// <summary>
        /// The capital letter infano (/i/).
        /// </summary>
        public const string CapitalInfano = "I";

        /// <summary>
        /// The small letter infano (/i/).
        /// </summary>
        public const string SmallInfano = "i";

        /// <summary>
        /// The capital letter juna (/j/).
        /// </summary>
        public const string CapitalJuna = "J";

        /// <summary>
        /// The small letter juna (/j/).
        /// </summary>
        public const string SmallJuna = "j";

        /// <summary>
        /// The capital letter ĵurnalo (/ʒ/).
        /// </summary>
        public const string CapitalĴurnalo = "Ĵ";

        /// <summary>
        /// The small letter ĵurnalo (/ʒ/).
        /// </summary>
        public const string SmallĴurnalo = "ĵ";

        /// <summary>
        /// The capital letter kafo (/k/).
        /// </summary>
        public const string CapitalKafo = "K";

        /// <summary>
        /// The small letter kafo (/k/).
        /// </summary>
        public const string SmallKafo = "k";

        /// <summary>
        /// The capital letter lando (/l/).
        /// </summary>
        public const string CapitalLando = "L";

        /// <summary>
        /// The small letter lando (/l/).
        /// </summary>
        public const string SmallLando = "l";

        /// <summary>
        /// The capital letter maro (/m/).
        /// </summary>
        public const string CapitalMaro = "M";

        /// <summary>
        /// The small letter maro (/m/).
        /// </summary>
        public const string SmallMaro = "m";

        /// <summary>
        /// The capital letter nokto (/n/).
        /// </summary>
        public const string CapitalNokto = "N";

        /// <summary>
        /// The small letter nokto (/n/).
        /// </summary>
        public const string SmallNokto = "n";

        /// <summary>
        /// The capital letter oro (/o/).
        /// </summary>
        public const string CapitalOro = "O";

        /// <summary>
        /// The small letter oro (/o/).
        /// </summary>
        public const string SmallOro = "o";

        /// <summary>
        /// The capital letter paco (/p/).
        /// </summary>
        public const string CapitalPaco = "P";

        /// <summary>
        /// The small letter paco (/p/).
        /// </summary>
        public const string SmallPaco = "p";

        /// <summary>
        /// The capital letter rapida (/r/).
        /// </summary>
        public const string CapitalRapida = "R";

        /// <summary>
        /// The small letter rapida (/r/).
        /// </summary>
        public const string SmallRapida = "r";

        /// <summary>
        /// The capital letter salti (/s/).
        /// </summary>
        public const string CapitalSalti = "S";

        /// <summary>
        /// The small letter salti (/s/).
        /// </summary>
        public const string SmallSalti = "s";

        /// <summary>
        /// The capital letter ŝipo (/ʃ/).
        /// </summary>
        public const string CapitalŜipo = "Ŝ";

        /// <summary>
        /// The small letter ŝipo (/ʃ/).
        /// </summary>
        public const string SmallŜipo = "ŝ";

        /// <summary>
        /// The capital letter tago (/t/).
        /// </summary>
        public const string CapitalTago = "T";

        /// <summary>
        /// The small letter tago (/t/).
        /// </summary>
        public const string SmallTago = "t";

        /// <summary>
        /// The capital letter urbo (/u/).
        /// </summary>
        public const string CapitalUrbo = "U";

        /// <summary>
        /// The small letter urbo (/u/).
        /// </summary>
        public const string SmallUrbo = "u";

        /// <summary>
        /// The capital letter aŭto (/w/).
        /// </summary>
        public const string CapitalAŭto = "Ŭ";

        /// <summary>
        /// The small letter aŭto (/w/).
        /// </summary>
        public const string SmallAŭto = "ŭ";

        /// <summary>
        /// The capital letter vivo (/v/).
        /// </summary>
        public const string CapitalVivo = "V";

        /// <summary>
        /// The small letter vivo (/v/).
        /// </summary>
        public const string SmallVivo = "v";

        /// <summary>
        /// The capital letter zebro (/z/).
        /// </summary>
        public const string CapitalZebro = "Z";

        /// <summary>
        /// The small letter zebro (/z/).
        /// </summary>
        public const string SmallZebro = "z";
    }
}
