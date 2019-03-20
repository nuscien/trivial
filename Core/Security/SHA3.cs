using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Trivial.Security
{
    /// <summary>
    /// SHA-3 family hash algorithm.
    /// </summary>
    /// <remarks>Will replace by the .Net implementation when available.</remarks>
    internal class SHA3ManagedImpl : HashAlgorithm
    {
        public static new SHA3ManagedImpl Create()
        {
            return new SHA3ManagedImpl(512);
        }

        public static SHA3ManagedImpl Create224()
        {
            return new SHA3ManagedImpl(224);
        }

        public static SHA3ManagedImpl Create256()
        {
            return new SHA3ManagedImpl(256);
        }

        public static SHA3ManagedImpl Create384()
        {
            return new SHA3ManagedImpl(384);
        }

        public static SHA3ManagedImpl Create512()
        {
            return new SHA3ManagedImpl(512);
        }

        public const int KeccakB = 1600;
        public const int KeccakNumberOfRounds = 24;
        public const int KeccakLaneSizeInBits = 8 * 8;

        public readonly ulong[] RoundConstants;

        protected ulong[] state;
        protected byte[] buffer;
        protected int buffLength;
        protected int keccakR;

        public int KeccakR
        {
            get
            {
                return keccakR;
            }
            protected set
            {
                keccakR = value;
            }
        }

        public int SizeInBytes => KeccakR / 8;

        public int HashByteLength => HashSizeValue / 8;

        public override bool CanReuseTransform => true;

        private SHA3ManagedImpl(int hashBitLength)
        {
            if (hashBitLength != 224 && hashBitLength != 256 && hashBitLength != 384 && hashBitLength != 512)
                throw new ArgumentException("hashBitLength must be 224, 256, 384, or 512", "hashBitLength");
            Initialize();
            HashSizeValue = hashBitLength;
            switch (hashBitLength)
            {
                case 224:
                    KeccakR = 1152;
                    break;
                case 256:
                    KeccakR = 1088;
                    break;
                case 384:
                    KeccakR = 832;
                    break;
                case 512:
                    KeccakR = 576;
                    break;
            }
            RoundConstants = new ulong[]
            {
                0x0000000000000001UL,
                0x0000000000008082UL,
                0x800000000000808aUL,
                0x8000000080008000UL,
                0x000000000000808bUL,
                0x0000000080000001UL,
                0x8000000080008081UL,
                0x8000000000008009UL,
                0x000000000000008aUL,
                0x0000000000000088UL,
                0x0000000080008009UL,
                0x000000008000000aUL,
                0x000000008000808bUL,
                0x800000000000008bUL,
                0x8000000000008089UL,
                0x8000000000008003UL,
                0x8000000000008002UL,
                0x8000000000000080UL,
                0x000000000000800aUL,
                0x800000008000000aUL,
                0x8000000080008081UL,
                0x8000000000008080UL,
                0x0000000080000001UL,
                0x8000000080008008UL
            };
        }

        protected ulong ROL(ulong a, int offset)
        {
            return ((a) << (offset % KeccakLaneSizeInBits)) ^ ((a) >> (KeccakLaneSizeInBits - (offset % KeccakLaneSizeInBits)));
        }

        protected void AddToBuffer(byte[] array, ref int offset, ref int count)
        {
            var amount = Math.Min(count, buffer.Length - buffLength);
            Buffer.BlockCopy(array, offset, buffer, buffLength, amount);
            offset += amount;
            buffLength += amount;
            count -= amount;
        }

        public override byte[] Hash => HashValue;

        public override int HashSize => HashSizeValue;

        public override void Initialize()
        {
            buffLength = 0;
            state = new ulong[5 * 5]; // 1600 bits.
            HashValue = null;
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (ibStart < 0) throw new ArgumentOutOfRangeException(nameof(ibStart));
            if (cbSize > array.Length) throw new ArgumentOutOfRangeException(nameof(cbSize));
            if (ibStart + cbSize > array.Length) throw new ArgumentOutOfRangeException("ibStart or cbSize");
            if (cbSize == 0) return;

            var sizeInBytes = SizeInBytes;
            if (buffer == null) buffer = new byte[sizeInBytes];
            var stride = sizeInBytes >> 3;
            var utemps = new ulong[stride];
            if (buffLength == sizeInBytes) throw new Exception("Unexpected error, the internal buffer is full");
            AddToBuffer(array, ref ibStart, ref cbSize);

            if (buffLength == sizeInBytes)
            {
                Buffer.BlockCopy(buffer, 0, utemps, 0, sizeInBytes);
                KeccakF(utemps, stride);
                buffLength = 0;
            }

            for (; cbSize >= sizeInBytes; cbSize -= sizeInBytes, ibStart += sizeInBytes)
            {
                Buffer.BlockCopy(array, ibStart, utemps, 0, sizeInBytes);
                KeccakF(utemps, stride);
            }

            if (cbSize > 0)
            {
                Buffer.BlockCopy(array, ibStart, buffer, buffLength, cbSize);
                buffLength += cbSize;
            }
        }

        protected override byte[] HashFinal()
        {
            var sizeInBytes = SizeInBytes;
            var outb = new byte[HashByteLength];
            if (buffer == null) buffer = new byte[sizeInBytes];
            else Array.Clear(buffer, buffLength, sizeInBytes - buffLength);
            buffer[buffLength++] = 1;
            buffer[sizeInBytes - 1] |= 0x80;
            int stride = sizeInBytes >> 3;
            ulong[] utemps = new ulong[stride];
            Buffer.BlockCopy(buffer, 0, utemps, 0, sizeInBytes);
            KeccakF(utemps, stride);
            Buffer.BlockCopy(state, 0, outb, 0, HashByteLength);
            return outb;
        }

        private void KeccakF(ulong[] inb, int laneCount)
        {
            while (--laneCount >= 0) state[laneCount] ^= inb[laneCount];
            var round = laneCount;

            var Aba = state[0];
            var Abe = state[1];
            var Abi = state[2];
            var Abo = state[3];
            var Abu = state[4];
            var Aga = state[5];
            var Age = state[6];
            var Agi = state[7];
            var Ago = state[8];
            var Agu = state[9];
            var Aka = state[10];
            var Ake = state[11];
            var Aki = state[12];
            var Ako = state[13];
            var Aku = state[14];
            var Ama = state[15];
            var Ame = state[16];
            var Ami = state[17];
            var Amo = state[18];
            var Amu = state[19];
            var Asa = state[20];
            var Ase = state[21];
            var Asi = state[22];
            var Aso = state[23];
            var Asu = state[24];

            for (round = 0; round < KeccakNumberOfRounds; round += 2)
            {
                var BCa = Aba ^ Aga ^ Aka ^ Ama ^ Asa;
                var BCe = Abe ^ Age ^ Ake ^ Ame ^ Ase;
                var BCi = Abi ^ Agi ^ Aki ^ Ami ^ Asi;
                var BCo = Abo ^ Ago ^ Ako ^ Amo ^ Aso;
                var BCu = Abu ^ Agu ^ Aku ^ Amu ^ Asu;

                var Da = BCu ^ ROL(BCe, 1);
                var De = BCa ^ ROL(BCi, 1);
                var Di = BCe ^ ROL(BCo, 1);
                var Do = BCi ^ ROL(BCu, 1);
                var Du = BCo ^ ROL(BCa, 1);

                Aba ^= Da;
                BCa = Aba;
                Age ^= De;
                BCe = ROL(Age, 44);
                Aki ^= Di;
                BCi = ROL(Aki, 43);
                Amo ^= Do;
                BCo = ROL(Amo, 21);
                Asu ^= Du;
                BCu = ROL(Asu, 14);

                var Eba = BCa ^ ((~BCe) & BCi);
                Eba ^= RoundConstants[round];
                var Ebe = BCe ^ ((~BCi) & BCo);
                var Ebi = BCi ^ ((~BCo) & BCu);
                var Ebo = BCo ^ ((~BCu) & BCa);
                var Ebu = BCu ^ ((~BCa) & BCe);

                Abo ^= Do;
                BCa = ROL(Abo, 28);
                Agu ^= Du;
                BCe = ROL(Agu, 20);
                Aka ^= Da;
                BCi = ROL(Aka, 3);
                Ame ^= De;
                BCo = ROL(Ame, 45);
                Asi ^= Di;
                BCu = ROL(Asi, 61);

                var Ega = BCa ^ ((~BCe) & BCi);
                var Ege = BCe ^ ((~BCi) & BCo);
                var Egi = BCi ^ ((~BCo) & BCu);
                var Ego = BCo ^ ((~BCu) & BCa);
                var Egu = BCu ^ ((~BCa) & BCe);

                Abe ^= De;
                BCa = ROL(Abe, 1);
                Agi ^= Di;
                BCe = ROL(Agi, 6);
                Ako ^= Do;
                BCi = ROL(Ako, 25);
                Amu ^= Du;
                BCo = ROL(Amu, 8);
                Asa ^= Da;
                BCu = ROL(Asa, 18);

                var Eka = BCa ^ ((~BCe) & BCi);
                var Eke = BCe ^ ((~BCi) & BCo);
                var Eki = BCi ^ ((~BCo) & BCu);
                var Eko = BCo ^ ((~BCu) & BCa);
                var Eku = BCu ^ ((~BCa) & BCe);

                Abu ^= Du;
                BCa = ROL(Abu, 27);
                Aga ^= Da;
                BCe = ROL(Aga, 36);
                Ake ^= De;
                BCi = ROL(Ake, 10);
                Ami ^= Di;
                BCo = ROL(Ami, 15);
                Aso ^= Do;
                BCu = ROL(Aso, 56);

                var Ema = BCa ^ ((~BCe) & BCi);
                var Eme = BCe ^ ((~BCi) & BCo);
                var Emi = BCi ^ ((~BCo) & BCu);
                var Emo = BCo ^ ((~BCu) & BCa);
                var Emu = BCu ^ ((~BCa) & BCe);

                Abi ^= Di;
                BCa = ROL(Abi, 62);
                Ago ^= Do;
                BCe = ROL(Ago, 55);
                Aku ^= Du;
                BCi = ROL(Aku, 39);
                Ama ^= Da;
                BCo = ROL(Ama, 41);
                Ase ^= De;
                BCu = ROL(Ase, 2);

                var Esa = BCa ^ ((~BCe) & BCi);
                var Ese = BCe ^ ((~BCi) & BCo);
                var Esi = BCi ^ ((~BCo) & BCu);
                var Eso = BCo ^ ((~BCu) & BCa);
                var Esu = BCu ^ ((~BCa) & BCe);

                BCa = Eba ^ Ega ^ Eka ^ Ema ^ Esa;
                BCe = Ebe ^ Ege ^ Eke ^ Eme ^ Ese;
                BCi = Ebi ^ Egi ^ Eki ^ Emi ^ Esi;
                BCo = Ebo ^ Ego ^ Eko ^ Emo ^ Eso;
                BCu = Ebu ^ Egu ^ Eku ^ Emu ^ Esu;

                Da = BCu ^ ROL(BCe, 1);
                De = BCa ^ ROL(BCi, 1);
                Di = BCe ^ ROL(BCo, 1);
                Do = BCi ^ ROL(BCu, 1);
                Du = BCo ^ ROL(BCa, 1);

                Eba ^= Da;
                BCa = Eba;
                Ege ^= De;
                BCe = ROL(Ege, 44);
                Eki ^= Di;
                BCi = ROL(Eki, 43);
                Emo ^= Do;
                BCo = ROL(Emo, 21);
                Esu ^= Du;
                BCu = ROL(Esu, 14);
                Aba = BCa ^ ((~BCe) & BCi);
                Aba ^= RoundConstants[round + 1];
                Abe = BCe ^ ((~BCi) & BCo);
                Abi = BCi ^ ((~BCo) & BCu);
                Abo = BCo ^ ((~BCu) & BCa);
                Abu = BCu ^ ((~BCa) & BCe);

                Ebo ^= Do;
                BCa = ROL(Ebo, 28);
                Egu ^= Du;
                BCe = ROL(Egu, 20);
                Eka ^= Da;
                BCi = ROL(Eka, 3);
                Eme ^= De;
                BCo = ROL(Eme, 45);
                Esi ^= Di;
                BCu = ROL(Esi, 61);
                Aga = BCa ^ ((~BCe) & BCi);
                Age = BCe ^ ((~BCi) & BCo);
                Agi = BCi ^ ((~BCo) & BCu);
                Ago = BCo ^ ((~BCu) & BCa);
                Agu = BCu ^ ((~BCa) & BCe);

                Ebe ^= De;
                BCa = ROL(Ebe, 1);
                Egi ^= Di;
                BCe = ROL(Egi, 6);
                Eko ^= Do;
                BCi = ROL(Eko, 25);
                Emu ^= Du;
                BCo = ROL(Emu, 8);
                Esa ^= Da;
                BCu = ROL(Esa, 18);
                Aka = BCa ^ ((~BCe) & BCi);
                Ake = BCe ^ ((~BCi) & BCo);
                Aki = BCi ^ ((~BCo) & BCu);
                Ako = BCo ^ ((~BCu) & BCa);
                Aku = BCu ^ ((~BCa) & BCe);

                Ebu ^= Du;
                BCa = ROL(Ebu, 27);
                Ega ^= Da;
                BCe = ROL(Ega, 36);
                Eke ^= De;
                BCi = ROL(Eke, 10);
                Emi ^= Di;
                BCo = ROL(Emi, 15);
                Eso ^= Do;
                BCu = ROL(Eso, 56);
                Ama = BCa ^ ((~BCe) & BCi);
                Ame = BCe ^ ((~BCi) & BCo);
                Ami = BCi ^ ((~BCo) & BCu);
                Amo = BCo ^ ((~BCu) & BCa);
                Amu = BCu ^ ((~BCa) & BCe);

                Ebi ^= Di;
                BCa = ROL(Ebi, 62);
                Ego ^= Do;
                BCe = ROL(Ego, 55);
                Eku ^= Du;
                BCi = ROL(Eku, 39);
                Ema ^= Da;
                BCo = ROL(Ema, 41);
                Ese ^= De;
                BCu = ROL(Ese, 2);
                Asa = BCa ^ ((~BCe) & BCi);
                Ase = BCe ^ ((~BCi) & BCo);
                Asi = BCi ^ ((~BCo) & BCu);
                Aso = BCo ^ ((~BCu) & BCa);
                Asu = BCu ^ ((~BCa) & BCe);
            }

            state[0] = Aba;
            state[1] = Abe;
            state[2] = Abi;
            state[3] = Abo;
            state[4] = Abu;
            state[5] = Aga;
            state[6] = Age;
            state[7] = Agi;
            state[8] = Ago;
            state[9] = Agu;
            state[10] = Aka;
            state[11] = Ake;
            state[12] = Aki;
            state[13] = Ako;
            state[14] = Aku;
            state[15] = Ama;
            state[16] = Ame;
            state[17] = Ami;
            state[18] = Amo;
            state[19] = Amu;
            state[20] = Asa;
            state[21] = Ase;
            state[22] = Asi;
            state[23] = Aso;
            state[24] = Asu;
        }
    }
}
