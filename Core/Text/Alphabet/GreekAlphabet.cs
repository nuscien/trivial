using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Text
{
    /// <summary>
    /// The Greek alphabet.
    /// </summary>
    public static class GreekAlphabet
    {
        /// <summary>
        /// The name of greek alphabet in Greek.
        /// </summary>
        public const string Name = "Ελληνικό Αλφάβητο";

        /// <summary>
        /// The name of greek alphabet in English.
        /// </summary>
        public const string EnglishName = "Greek Alphabet";

        /// <summary>
        /// The language code.
        /// </summary>
        public const string LanguageCode = "el";

        /// <summary>
        /// The capital letter alpha (/'ælfə/).
        ///</summary>
        public const string CapitalAlpha = "Α";

        /// <summary>
        /// The small letter alpha (/'ælfə/).
        ///</summary>
        public const string SmallAlpha = "α";

        /// <summary>
        /// The capital letter beta (/'bi:tə/ 或 /'beɪtə/).
        ///</summary>
        public const string CapitalBeta = "Β";

        /// <summary>
        /// The small letter beta (/'bi:tə/ 或 /'beɪtə/).
        ///</summary>
        public const string SmallBeta = "β";

        /// <summary>
        /// The capital letter gamma (/'gæmə/).
        ///</summary>
        public const string CapitalGamma = "Γ";

        /// <summary>
        /// The small letter gamma (/'gæmə/).
        ///</summary>
        public const string SmallGamma = "γ";

        /// <summary>
        /// The capital letter delta (/'deltə/).
        ///</summary>
        public const string CapitalDelta = "Δ";

        /// <summary>
        /// The small letter delta (/'deltə/).
        ///</summary>
        public const string SmallDelta = "δ";

        /// <summary>
        /// The capital letter epsilon (/'epsɪlɒn/).
        ///</summary>
        public const string CapitalEpsilon = "Ε";

        /// <summary>
        /// The small letter epsilon (/'epsɪlɒn/).
        ///</summary>
        public const string SmallEpsilon = "ε";

        /// <summary>
        /// The small letter epsilon (/'epsɪlɒn/).
        ///</summary>
        public const string SmallEpsilon2 = "ϵ";

        /// <summary>
        /// The capital letter dígamma (/'daɪˈɡæmə/).
        ///</summary>
        public const string CapitalDígamma = "Ϝ";

        /// <summary>
        /// The small letter dígamma (/'daɪˈɡæmə/).
        ///</summary>
        public const string SmallDígamma = "ϝ";

        /// <summary>
        /// The capital letter zeta (/'zi:tə/).
        ///</summary>
        public const string CapitalZeta = "Ζ";

        /// <summary>
        /// The small letter zeta (/'zi:tə/).
        ///</summary>
        public const string SmallZeta = "ζ";

        /// <summary>
        /// The capital letter eta (/'i:tə/).
        ///</summary>
        public const string CapitalEta = "Η";

        /// <summary>
        /// The small letter eta (/'i:tə/).
        ///</summary>
        public const string SmallEta = "η";

        /// <summary>
        /// The capital letter theta (/'θi:tə/).
        ///</summary>
        public const string CapitalTheta = "Θ";

        /// <summary>
        /// The small letter theta (/'θi:tə/).
        ///</summary>
        public const string SmallTheta = "θ";

        /// <summary>
        /// The capital letter iota (/aɪ'əʊtə/).
        ///</summary>
        public const string CapitalIota = "Ι";

        /// <summary>
        /// The small letter iota (/aɪ'əʊtə/).
        ///</summary>
        public const string SmallIota = "ι";

        /// <summary>
        /// The capital letter kappa (/'kæpə/).
        ///</summary>
        public const string CapitalKappa = "Κ";

        /// <summary>
        /// The small letter kappa (/'kæpə/).
        ///</summary>
        public const string SmallKappa = "κ";

        /// <summary>
        /// The capital letter lambda (/'læmdə/).
        ///</summary>
        public const string CapitalLambda = "Λ";

        /// <summary>
        /// The small letter lambda (/'læmdə/).
        ///</summary>
        public const string SmallLambda = "λ";

        /// <summary>
        /// The capital letter mu (/mju:/).
        ///</summary>
        public const string CapitalMu = "Μ";

        /// <summary>
        /// The small letter mu (/mju:/).
        ///</summary>
        public const string SmallMu = "μ";

        /// <summary>
        /// The capital letter nu (/nju:/).
        ///</summary>
        public const string CapitalNu = "Ν";

        /// <summary>
        /// The small letter nu (/nju:/).
        ///</summary>
        public const string SmallNu = "ν";

        /// <summary>
        /// The capital letter xi (/ksi/).
        ///</summary>
        public const string CapitalXi = "Ξ";

        /// <summary>
        /// The small letter xi (/ksi/).
        ///</summary>
        public const string SmallXi = "ξ";

        /// <summary>
        /// The capital letter omicron (/əuˈmaikrən/).
        ///</summary>
        public const string CapitalOmicron = "Ο";

        /// <summary>
        /// The small letter omicron (/əuˈmaikrən/).
        ///</summary>
        public const string SmallOmicron = "ο";

        /// <summary>
        /// The capital letter pi (/paɪ/).
        ///</summary>
        public const string CapitalPi = "∏";

        /// <summary>
        /// The small letter pi (/paɪ/).
        ///</summary>
        public const string SmallPi = "π";

        /// <summary>
        /// The capital letter rho (/rəʊ/).
        ///</summary>
        public const string CapitalRho = "Ρ";

        /// <summary>
        /// The small letter rho (/rəʊ/).
        ///</summary>
        public const string SmallRho = "ρ";

        /// <summary>
        /// The capital letter sigma (/'sɪɡmə/).
        ///</summary>
        public const string CapitalSigma = "∑";

        /// <summary>
        /// The small letter sigma (/'sɪɡmə/).
        ///</summary>
        public const string SmallSigma = "σ";

        /// <summary>
        /// The small letter sigma (/'sɪɡmə/).
        ///</summary>
        public const string SmallSigma2 = "ς";

        /// <summary>
        /// The capital letter tau (/tɔ:/ or /taʊ/).
        ///</summary>
        public const string CapitalTau = "Τ";

        /// <summary>
        /// The small letter tau (/tɔ:/ or /taʊ/).
        ///</summary>
        public const string SmallTau = "τ";

        /// <summary>
        /// The capital letter upsilon (/ˈipsɪlon/).
        ///</summary>
        public const string CapitalUpsilon = "Υ";

        /// <summary>
        /// The small letter upsilon (/ˈipsɪlon/).
        ///</summary>
        public const string SmallUpsilon = "υ";

        /// <summary>
        /// The capital letter phi (/faɪ/).
        ///</summary>
        public const string CapitalPhi = "Φ";

        /// <summary>
        /// The small letter phi (/faɪ/).
        ///</summary>
        public const string SmallPhi = "φ";

        /// <summary>
        /// The small letter phi (/faɪ/).
        ///</summary>
        public const string SmallPhi2 = "ϕ";

        /// <summary>
        /// The capital letter chi (/kaɪ/).
        ///</summary>
        public const string CapitalChi = "Χ";

        /// <summary>
        /// The small letter chi (/kaɪ/).
        ///</summary>
        public const string SmallChi = "χ";

        /// <summary>
        /// The capital letter psi (/psaɪ/).
        ///</summary>
        public const string CapitalPsi = "Ψ";

        /// <summary>
        /// The small letter psi (/psaɪ/).
        ///</summary>
        public const string SmallPsi = "ψ";

        /// <summary>
        /// The capital letter omega (/'əʊmɪɡə/).
        ///</summary>
        public const string CapitalOmega = "Ω";

        /// <summary>
        /// The small letter omega (/'əʊmɪɡə/).
        ///</summary>
        public const string SmallOmega = "ω";
    }
}
