using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using XMPP_API.Classes.Crypto;
using XMPP_API.Classes.Network.XML.Messages.XEP_0392;

namespace UWPX_UI.Extensions
{
    public sealed class TextBlockOmemoFingerprintFormatExtension
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly DependencyProperty FingerprintProperty = DependencyProperty.Register("Fingerprint", typeof(byte[]), typeof(TextBlockChatMessageFormatExtension), new PropertyMetadata(new byte[] { }, OnFingerprintChanged));

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public static string GetFingerprint(DependencyObject obj)
        {
            return (string)obj.GetValue(FingerprintProperty);
        }

        public static void SetFingerprint(DependencyObject obj, string value)
        {
            obj.SetValue(FingerprintProperty, value);
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--


        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static void OnFingerprintChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (d is TextBlock textBlock)
            {
                if (args.NewValue is Byte[] fingerprint && fingerprint.Length > 0)
                {
                    int partCount = 4;
                    // Based on: https://stackoverflow.com/questions/11207526/best-way-to-split-an-array
                    Byte[][] parts = fingerprint
                    .Select((s, i) => new { Value = s, Index = i })
                    .GroupBy(x => x.Index / partCount)
                    .Select(grp => grp.Select(x => x.Value).ToArray())
                    .ToArray();

                    for (int e = 0; e < parts.Length; e++)
                    {
                        textBlock.Inlines.Add(new Run
                        {
                            Text = CryptoUtils.generateOmemoFingerprint(parts[e]),
                            Foreground = GenBrush(parts[e])
                        });

                        if (e == parts.Length / 2)
                        {
                            textBlock.Inlines.Add(new LineBreak());
                        }
                    }
                }
                else
                {
                    textBlock.Inlines.Add(new Run
                    {
                        Text = "Failed to load fingerprint!"
                    });
                }
            }
        }

        private static Brush GenBrush(byte[] part)
        {
            return new SolidColorBrush(ConsistentColorGenerator.GenColor(part));
        }

        #endregion
    }
}
