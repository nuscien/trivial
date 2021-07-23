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
        B = 0,

        /// <summary>
        /// Initial P (ㄆ p').
        /// </summary>
        P = 1,

        /// <summary>
        /// Initial M (ㄇ m).
        /// </summary>
        M = 2,

        /// <summary>
        /// Initial F (ㄈ f).
        /// </summary>
        F = 3,

        /// <summary>
        /// Initial D (ㄉ t).
        /// </summary>
        D = 4,

        /// <summary>
        /// Initial T (ㄊ t').
        /// </summary>
        T = 5,

        /// <summary>
        /// Initial N (ㄋ n).
        /// </summary>
        N = 6,

        /// <summary>
        /// Initial L (ㄌ l).
        /// </summary>
        L = 7,

        /// <summary>
        /// Initial G (ㄍ k).
        /// </summary>
        G = 8,

        /// <summary>
        /// Initial K (ㄎ k').
        /// </summary>
        K = 9,

        /// <summary>
        /// Initial H (ㄏ h).
        /// </summary>
        H = 10,

        /// <summary>
        /// Initial J (ㄐ ch).
        /// </summary>
        J = 11,

        /// <summary>
        /// Initial Q (ㄑ ch').
        /// </summary>
        Q = 12,

        /// <summary>
        /// Initial X (ㄒ hs).
        /// </summary>
        X = 13,

        /// <summary>
        /// Initial Zh (ㄓ chih).
        /// </summary>
        Zh = 14,

        /// <summary>
        /// Initial Ch (ㄔ ch'ih).
        /// </summary>
        Ch = 15,

        /// <summary>
        /// Initial Sh (ㄕ shih).
        /// </summary>
        Sh = 16,

        /// <summary>
        /// Initial R (ㄖ jih).
        /// </summary>
        R = 17,

        /// <summary>
        /// Initial Z (ㄗ tz).
        /// </summary>
        Z = 18,

        /// <summary>
        /// Initial C (ㄘ ts).
        /// </summary>
        C = 19,

        /// <summary>
        /// Initial S (ㄙ s).
        /// </summary>
        S = 20,

        /// <summary>
        /// Initial Y (丨 y). This is not a standard initial.
        /// </summary>
        Y = 21,

        /// <summary>
        /// Initial W (ㄨ w). This is not a standard initial.
        /// </summary>
        W = 22
    }
}
