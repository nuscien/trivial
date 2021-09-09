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
    /// The Pinyin initials.
    /// </summary>
    public enum PinyinInitials
    {
        /// <summary>
        /// Initial B (ㄅ p).
        /// </summary>
        B = 1,

        /// <summary>
        /// Initial P (ㄆ p').
        /// </summary>
        P = 2,

        /// <summary>
        /// Initial M (ㄇ m).
        /// </summary>
        M = 3,

        /// <summary>
        /// Initial F (ㄈ f).
        /// </summary>
        F = 4,

        /// <summary>
        /// Initial D (ㄉ t).
        /// </summary>
        D = 5,

        /// <summary>
        /// Initial T (ㄊ t').
        /// </summary>
        T = 6,

        /// <summary>
        /// Initial N (ㄋ n).
        /// </summary>
        N = 7,

        /// <summary>
        /// Initial L (ㄌ l).
        /// </summary>
        L = 8,

        /// <summary>
        /// Initial G (ㄍ k).
        /// </summary>
        G = 9,

        /// <summary>
        /// Initial K (ㄎ k').
        /// </summary>
        K = 10,

        /// <summary>
        /// Initial H (ㄏ h).
        /// </summary>
        H = 11,

        /// <summary>
        /// Initial J (ㄐ ch).
        /// </summary>
        J = 12,

        /// <summary>
        /// Initial Q (ㄑ ch').
        /// </summary>
        Q = 13,

        /// <summary>
        /// Initial X (ㄒ hs).
        /// </summary>
        X = 14,

        /// <summary>
        /// Initial Zh (ㄓ chih).
        /// </summary>
        Zh = 15,

        /// <summary>
        /// Initial Ch (ㄔ ch'ih).
        /// </summary>
        Ch = 16,

        /// <summary>
        /// Initial Sh (ㄕ shih).
        /// </summary>
        Sh = 17,

        /// <summary>
        /// Initial R (ㄖ jih).
        /// </summary>
        R = 18,

        /// <summary>
        /// Initial Z (ㄗ tz).
        /// </summary>
        Z = 19,

        /// <summary>
        /// Initial C (ㄘ ts).
        /// </summary>
        C = 20,

        /// <summary>
        /// Initial S (ㄙ s).
        /// </summary>
        S = 21,

        /// <summary>
        /// Initial Y (丨 y). This is not a standard initial.
        /// </summary>
        Y = 22,

        /// <summary>
        /// Initial W (ㄨ w). This is not a standard initial.
        /// </summary>
        W = 23
    }
}
