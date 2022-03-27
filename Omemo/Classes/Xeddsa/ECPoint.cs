using Org.BouncyCastle.Math;

namespace Omemo.Classes.Xeddsa
{
    public class ECPoint
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public BigInteger y;
        public bool s;

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public ECPoint(byte[] point)
        {
            // Decoding described in: https://datatracker.ietf.org/doc/html/rfc8032#section-5.1.3
            BigInteger tmp = new BigInteger(point);
            s = ((point[31] & 0x80) >> 7) == 1;
            y = tmp.ClearBit(255);
        }

        public ECPoint(BigInteger y, bool s)
        {
            this.y = y;
            this.s = s;
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        public byte[] Encode()
        {
            // Encoding described in: https://datatracker.ietf.org/doc/html/rfc8032#section-5.1.2
            BigInteger point = new BigInteger(y.ToByteArray());
            if (s)
            {
                point.SetBit(255);
            }
            else
            {
                point.ClearBit(255);
            }
            return point.ToByteArray();
        }

        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
