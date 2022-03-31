using System.Diagnostics;
using NSec.Cryptography;
using Omemo.Classes.Keys;
using Org.BouncyCastle.Math;

namespace Omemo.Classes.Xeddsa
{
    /// <summary>
    /// Notes:
    /// Curve25519 -> Montgomery
    /// edwards25519 (Ed25519) -> Twisted Edwards
    /// Ed25519 is a public-key signature system with several attractive features (https://ed25519.cr.yp.to/)
    /// Ed25519 as defined in:
    /// https://tools.ietf.org/html/rfc7748
    /// https://tools.ietf.org/html/rfc8032
    /// </summary>
    public class Xeddsa25519
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private static readonly BigInteger p = BigInteger.Two.Pow(255).Subtract(BigInteger.ValueOf(19));

        private static readonly byte[] MINUS_ONE = new byte[] {0xec, 0xd3, 0xf5, 0x5c, 0x1a, 0x63, 0x12, 0x58,
                                    0xd6, 0x9c, 0xf7, 0xa2, 0xde, 0xf9, 0xde, 0x14,
                                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10};

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Signes the given <paramref name="msg"/> and returns the result.
        /// </summary>
        /// <param name="privKey">The Curve25519 (Montgomery) private key used for signing.</param>
        /// <param name="msg">The message that should be signed.</param>
        /// <param name="random">64 bytes secure random data used for signing.</param>
        /// <returns>The generated signature for the given <paramref name="msg"/>.</returns>
        public static byte[] Sign(ECPrivKeyModel privKey, byte[] msg, byte[] random)
        {
            Debug.Assert(random.Length == 64);
            Debug.Assert(msg.Length > 0);

            ECKeyPairModel edKeyPair = ToTwistedEdwardsKeyPair(privKey);

            Key key = Key.Import(SignatureAlgorithm.Ed25519, edKeyPair.privKey.key, KeyBlobFormat.RawPrivateKey);
            return SignatureAlgorithm.Ed25519.Sign(key, msg);
        }

        #endregion

        #region --Misc Methods (Private)--
        public static ECKeyPairModel ToTwistedEdwardsKeyPair(ECPrivKeyModel privKey)
        {
            byte[] A = new byte[Org.BouncyCastle.Math.EC.Rfc7748.X25519.PointSize];
            byte[] A2 = new byte[Org.BouncyCastle.Math.EC.Rfc7748.X25519.PointSize];
            Org.BouncyCastle.Math.EC.Rfc7748.X25519.ScalarMultBase(privKey.key, 0, A2, 0);
            byte[] u = Shared.Classes.SharedUtils.HexStringToByteArray("5866666666666666666666666666666666666666666666666666666666666666");
            Org.BouncyCastle.Math.EC.Rfc7748.X25519.ScalarMult(privKey.key, 0, u, 0, A, 0);

            byte s = (byte)((A[31] & 0x80) >> 7);
            byte[] a = new byte[Org.BouncyCastle.Math.EC.Rfc7748.X25519.PointSize];
            privKey.key.CopyTo(a, 0);
            byte[] aNeg = new byte[Org.BouncyCastle.Math.EC.Rfc7748.X25519.PointSize];

            Org.BouncyCastle.Math.EC.Rfc7748.X25519.ScalarMult(MINUS_ONE, 0, a, 0, aNeg, 0);
            sc_cmov(a, aNeg, s);
            A[31] &= 0x7F;

            return new ECKeyPairModel(new ECPrivKeyModel(a), new ECPubKeyModel(A));
        }

        private static void sc_cmov(byte[] f, byte[] g, byte b)
        {
            byte[] x = new byte[32];
            for (int count = 0; count < 32; count++)
            {
                x[count] = (byte)(f[count] ^ g[count]);
            }
            b = (byte)-b;
            for (int count = 0; count < 32; count++)
            {
                x[count] &= b;
            }

            for (int count = 0; count < 32; count++)
            {
                f[count] = (byte)(f[count] ^ x[count]);
            }
        }


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
