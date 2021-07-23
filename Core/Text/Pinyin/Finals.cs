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
    public enum PinyinFinals
    {
        /// <summary>
        /// Final A (ㄚ).
        /// </summary>
        A = 0,

        /// <summary>
        /// Final O (ㄛ).
        /// </summary>
        O = 1,

        /// <summary>
        /// Final E (ㄜ).
        /// </summary>
        E = 2,

        /// <summary>
        /// Final I (丨).
        /// </summary>
        I = 3,

        /// <summary>
        /// Final U (ㄨ).
        /// </summary>
        U = 4,

        /// <summary>
        /// Final V (Yu) (ㄩ).
        /// </summary>
        V = 5,

        // Eh = 6, // Reserved (ㄝ).

        /// <summary>
        /// Final Er (ㄦ).
        /// </summary>
        Er = 7,

        /// <summary>
        /// Final Ai (ㄞ).
        /// </summary>
        Ai = 8,

        /// <summary>
        /// Final Ei (ㄟ).
        /// </summary>
        Ei = 9,

        /// <summary>
        /// Final Ao (ㄠ).
        /// </summary>
        Ao = 10,

        /// <summary>
        /// Final Ou (ㄡ).
        /// </summary>
        Ou = 11,

        /// <summary>
        /// Final Ia (丨ㄚ).
        /// </summary>
        Ia = 12,

        /// <summary>
        /// Final Ie (丨ㄝ).
        /// </summary>
        Ie = 13,

        /// <summary>
        /// Final Ua (ㄨㄚ).
        /// </summary>
        Ua = 14,

        /// <summary>
        /// Final Uo (ㄨㄛ).
        /// </summary>
        Uo = 15,

        /// <summary>
        /// Final Ve (Yue) (ㄩㄝ).
        /// </summary>
        Ve = 16,

        /// <summary>
        /// Final Iao (丨ㄠ).
        /// </summary>
        Iao = 17,

        /// <summary>
        /// Final Iou (丨ㄡ).
        /// </summary>
        Iou = 18,

        /// <summary>
        /// Final Uai (ㄨㄞ).
        /// </summary>
        Uai = 19,

        /// <summary>
        /// Final Uei (ㄨㄟ).
        /// </summary>
        Uei = 20,

        /// <summary>
        /// Final An (ㄢ).
        /// </summary>
        An = 21,

        /// <summary>
        /// Final Ian (丨ㄢ).
        /// </summary>
        Ian = 22,

        /// <summary>
        /// Final Uan (ㄨㄢ).
        /// </summary>
        Uan = 23,

        /// <summary>
        /// Final Van (Yuan) (ㄩㄢ).
        /// </summary>
        Van = 24,

        /// <summary>
        /// Final En (ㄣ).
        /// </summary>
        En = 25,

        /// <summary>
        /// Final In (丨ㄣ).
        /// </summary>
        In = 26,

        /// <summary>
        /// Final Uen (ㄨㄣ).
        /// </summary>
        Uen = 27,

        /// <summary>
        /// Final Vn (Yun) (ㄩㄣ).
        /// </summary>
        Vn = 28,

        /// <summary>
        /// Final Ang (ㄤ).
        /// </summary>
        Ang = 29,

        /// <summary>
        /// Final Iang (丨ㄤ).
        /// </summary>
        Iang = 30,

        /// <summary>
        /// Final Uang (ㄨㄤ).
        /// </summary>
        Uang = 31,

        /// <summary>
        /// Final Eng (ㄥ).
        /// </summary>
        Eng = 32,

        /// <summary>
        /// Final Ing (丨ㄥ).
        /// </summary>
        Ing = 33,

        /// <summary>
        /// Final Ueng (ㄨㄥ).
        /// </summary>
        Ueng = 34,

        /// <summary>
        /// Final Ong (ㄨㄥ).
        /// </summary>
        Ong = 35,

        /// <summary>
        /// Final Iong (ㄩㄥ).
        /// </summary>
        Iong = 36
    }
}
