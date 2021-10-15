using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Security;
using System.Text;
using System.Text.Json;

namespace Trivial.Text
{
    /// <summary>
    /// The Pinyin finals.
    /// </summary>
    public enum PinyinFinals : byte
    {
        /// <summary>
        /// Final A (ㄚ).
        /// </summary>
        A = 1,

        /// <summary>
        /// Final O (ㄛ).
        /// </summary>
        O = 2,

        /// <summary>
        /// Final E (ㄜ).
        /// </summary>
        E = 3,

        /// <summary>
        /// Final I (丨).
        /// </summary>
        I = 4,

        /// <summary>
        /// Final U (ㄨ).
        /// </summary>
        U = 5,

        /// <summary>
        /// Final V (Yu) (ㄩ).
        /// </summary>
        V = 6,

        // Eh = 7, // Reserved for final Ê (ㄝ).

        /// <summary>
        /// Final Er (ㄦ).
        /// </summary>
        Er = 8,

        /// <summary>
        /// Final Ai (ㄞ).
        /// </summary>
        Ai = 9,

        /// <summary>
        /// Final Ei (ㄟ).
        /// </summary>
        Ei = 10,

        /// <summary>
        /// Final Ao (ㄠ).
        /// </summary>
        Ao = 11,

        /// <summary>
        /// Final Ou (ㄡ).
        /// </summary>
        Ou = 12,

        /// <summary>
        /// Final Ia (丨ㄚ).
        /// </summary>
        Ia = 13,

        /// <summary>
        /// Final Ie (丨ㄝ).
        /// </summary>
        Ie = 14,

        /// <summary>
        /// Final Ua (ㄨㄚ).
        /// </summary>
        Ua = 15,

        /// <summary>
        /// Final Uo (ㄨㄛ).
        /// </summary>
        Uo = 16,

        /// <summary>
        /// Final Ve (Yue) (ㄩㄝ).
        /// </summary>
        Ve = 17,

        /// <summary>
        /// Final Iao (丨ㄠ).
        /// </summary>
        Iao = 18,

        /// <summary>
        /// Final Iou (丨ㄡ).
        /// </summary>
        Iou = 19,

        /// <summary>
        /// Final Uai (ㄨㄞ).
        /// </summary>
        Uai = 20,

        /// <summary>
        /// Final Uei (ㄨㄟ).
        /// </summary>
        Uei = 21,

        /// <summary>
        /// Final An (ㄢ).
        /// </summary>
        An = 22,

        /// <summary>
        /// Final Ian (丨ㄢ).
        /// </summary>
        Ian = 23,

        /// <summary>
        /// Final Uan (ㄨㄢ).
        /// </summary>
        Uan = 24,

        /// <summary>
        /// Final Van (Yuan) (ㄩㄢ).
        /// </summary>
        Van = 25,

        /// <summary>
        /// Final En (ㄣ).
        /// </summary>
        En = 26,

        /// <summary>
        /// Final In (丨ㄣ).
        /// </summary>
        In = 27,

        /// <summary>
        /// Final Uen (ㄨㄣ).
        /// </summary>
        Uen = 28,

        /// <summary>
        /// Final Vn (Yun) (ㄩㄣ).
        /// </summary>
        Vn = 29,

        /// <summary>
        /// Final Ang (ㄤ).
        /// </summary>
        Ang = 30,

        /// <summary>
        /// Final Iang (丨ㄤ).
        /// </summary>
        Iang = 31,

        /// <summary>
        /// Final Uang (ㄨㄤ).
        /// </summary>
        Uang = 32,

        /// <summary>
        /// Final Eng (ㄥ).
        /// </summary>
        Eng = 33,

        /// <summary>
        /// Final Ing (丨ㄥ).
        /// </summary>
        Ing = 34,

        /// <summary>
        /// Final Ueng (ㄨㄥ).
        /// </summary>
        Ueng = 35,

        /// <summary>
        /// Final Ong (ㄨㄥ).
        /// </summary>
        Ong = 36,

        /// <summary>
        /// Final Iong (ㄩㄥ).
        /// </summary>
        Iong = 37
    }
}
