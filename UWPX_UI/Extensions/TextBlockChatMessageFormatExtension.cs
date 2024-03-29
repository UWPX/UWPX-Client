﻿using System;
using System.Linq;
using System.Text.RegularExpressions;
using NeoSmart.Unicode;
using Storage.Classes;
using UWPX_UI_Context.Classes;
using Windows.ApplicationModel.Contacts;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace UWPX_UI.Extensions
{
    /// <summary>
    /// Based on: https://stackoverflow.com/questions/38208741/auto-detect-url-phone-number-email-in-textblock
    /// </summary>
    public sealed class TextBlockChatMessageFormatExtension
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        /// <summary>
        /// The default prefix to use to convert a relative URI to an absolute URI
        /// The Windows RunTime is only working with absolute URI
        /// </summary>
        private const string RELATIVE_URI_DEFAULT_PREFIX = "https://";
        private const string URL_REGEX_PATTERN = @"(?i)\b((?:[a-z][\w-]+:(?:/{1,3}|[a-z0-9%])|www\d{0,3}[.]|[a-z0-9.\-]+[.][a-z]{2,4}/)(?:[^\s()<>]+|\(([^\s()<>]+|(\([^\s()<>]+\)))*\))+(?:\(([^\s()<>]+|(\([^\s()<>]+\)))*\)|[^\s`!()\[\]{};:'"".,<>?«»“”‘’]))";
        private const string EMAIL_REGEX_PATTERN = @"(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))";
        private const string PHONE_REGEX_PATTERN = @"\+?(\d{2,}[\-\(\)\. ]?){2,}\d\b";
        private const string NEW_LINE_REGEX_PATTERN = @"\r\n|\r|\n";
        private const string QUOTATION_REGEX_PATTERN = @"^\s*>\s*";

        private static readonly Regex URL_REGEX = new Regex(URL_REGEX_PATTERN, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(500));
        private static readonly Regex EMAIL_REGEX = new Regex(EMAIL_REGEX_PATTERN, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(500));
        private static readonly Regex PHONE_REGEX = new Regex(PHONE_REGEX_PATTERN, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        private static readonly Regex NEW_LINE_REGEX = new Regex(NEW_LINE_REGEX_PATTERN);
        private static readonly Regex QUOTATION_REGEX = new Regex(QUOTATION_REGEX_PATTERN);

        public static readonly DependencyProperty FormattedTextProperty = DependencyProperty.Register("FormattedText", typeof(string), typeof(TextBlockChatMessageFormatExtension), new PropertyMetadata("Loading...", OnFormattedTextChanged));

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--


        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public static string GetFormattedText(DependencyObject obj)
        {
            return (string)obj.GetValue(FormattedTextProperty);
        }

        public static void SetFormattedText(DependencyObject obj, string value)
        {
            obj.SetValue(FormattedTextProperty, value);
        }

        private static SolidColorBrush GetQuoteForeground()
        {
            return ThemeUtils.GetThemeResource<SolidColorBrush>("CaptionTextBrush");
        }

        private static SolidColorBrush GetHyperlinkLinkForeground(FrameworkElement element)
        {
            return ThemeUtils.GetThemeResource<SolidColorBrush>("LinkAccentColorBrush");
        }

        private static void ToQuoteRun(Run run)
        {
            run.Foreground = GetQuoteForeground();
            run.FontStyle = FontStyle.Italic;
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        /// <summary>
        /// This method will extract a fragment of the raw text string, create a Run element with the fragment and
        /// add it to the <see cref="TextBlock.Inlines"/> collection.
        /// </summary>
        /// <param name="textBlock">The <see cref="TextBlock"/> where to add the run element.</param>
        /// <param name="rawText">The raw text where the fragment will be extracted.</param>
        /// <param name="startPosition">The start position to extract the fragment.</param>
        /// <param name="endPosition">The end position to extract the fragment.</param>
        /// <param name="isQuote">True in case we are currently inside a quote.</param>
        private static void CreateRunElement(TextBlock textBlock, string rawText, int startPosition, int endPosition, bool isQuote)
        {
            string fragment = rawText.Substring(startPosition, endPosition - startPosition);
            Run run = new Run
            {
                Text = fragment,
                Foreground = GetHyperlinkLinkForeground(textBlock)
            };
            if (isQuote)
            {
                ToQuoteRun(run);
            }
            textBlock.Inlines.Add(run);
        }

        /// <summary>
        /// Create an URL element with the provided match result from the URL regex
        /// It will create the Hyperlink element that will contain the URL and add it to the provided textblock
        /// </summary>
        /// <param name="textBlock">the textblock where to add the hyperlink</param>
        /// <param name="urlMatch">the match for the URL to use to create the hyperlink element</param>
        /// <returns>the newest position on the source string for the parsing</returns>
        private static int CreateUrlElement(TextBlock textBlock, Match urlMatch, bool isQuote)
        {
            Run run = new Run
            {
                Text = urlMatch.Value,
                Foreground = GetHyperlinkLinkForeground(textBlock)
            };
            if (isQuote)
            {
                ToQuoteRun(run);
            }
            if (Uri.TryCreate(urlMatch.Value, UriKind.RelativeOrAbsolute, out Uri targetUri) && (targetUri.IsAbsoluteUri || Uri.TryCreate(RELATIVE_URI_DEFAULT_PREFIX + targetUri.OriginalString, UriKind.RelativeOrAbsolute, out targetUri)))
            {

                Hyperlink link = new Hyperlink
                {
                    Inlines = { run },
                    NavigateUri = targetUri
                };
                textBlock.Inlines.Add(link);
            }
            else
            {
                textBlock.Inlines.Add(new Run
                {
                    Text = urlMatch.Value
                });
            }

            return urlMatch.Index + urlMatch.Length;
        }

        /// <summary>
        /// Create a hyperlink element with the provided match result from the regex that will open the contact application
        /// with the provided contact information (it should be a phone number or an email address
        /// This is used only if the email address / phone number is not prefixed with the mailto: / tel: scheme
        /// It will create the Hyperlink element that will contain the email/phone number hyperlink and add it to the provided textblock.
        /// Clicking on the link will open the contact application
        /// </summary>
        /// <param name="textBlock">the textblock where to add the hyperlink</param>
        /// <param name="emailMatch">the match for the email to use to create the hyperlink element. Set to null if not available but at least one of emailMatch and phoneMatch must be not null.</param>
        /// <param name="phoneMatch">the match for the phone number to create the hyperlink element. Set to null if not available but at least one of emailMatch and phoneMatch must be not null.</param>
        /// <returns>the newest position on the source string for the parsing</returns>
        private static int CreateContactElement(TextBlock textBlock, Match emailMatch, Match phoneMatch, bool isQuote)
        {
            Match currentMatch = emailMatch ?? phoneMatch;

            Run run = new Run
            {
                Text = currentMatch.Value,
                Foreground = GetHyperlinkLinkForeground(textBlock)
            };
            if (isQuote)
            {
                ToQuoteRun(run);
            }
            Hyperlink link = new Hyperlink { Inlines = { run } };
            link.Click += (s, a) =>
            {
                Contact contact = new Contact();
                if (emailMatch != null)
                {
                    contact.Emails.Add(new ContactEmail { Address = emailMatch.Value });
                }
                if (phoneMatch != null)
                {
                    contact.Phones.Add(new ContactPhone { Number = new string(phoneMatch.Value.Where(c => char.IsDigit(c) || c == '+').ToArray()) });
                }

                ContactManager.ShowFullContactCard(contact, new FullContactCardOptions());
            };

            textBlock.Inlines.Add(link);
            return currentMatch.Index + currentMatch.Length;
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--
        private static void OnFormattedTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not TextBlock textBlock)
            {
                return;
            }

            // Clear all inlines:
            textBlock.Inlines.Clear();

            // Empty message:
            if (e.NewValue is not string text || string.IsNullOrWhiteSpace(text))
            {
                textBlock.Inlines.Add(new Run { Text = "" });
                return;
            }

            // Check if advanced chat message processing is disabled:
            if (Settings.GetSettingBoolean(SettingsConsts.DISABLE_ADVANCED_CHAT_MESSAGE_PROCESSING))
            {
                textBlock.Inlines.Add(new Run { Text = text });
                return;
            }

            bool isEmoji = Emoji.IsEmoji(text.TrimEnd(UiUtils.TRIM_CHARS).TrimStart(UiUtils.TRIM_CHARS));
            if (isEmoji)
            {
                textBlock.Inlines.Add(new Run
                {
                    Text = text,
                    FontSize = 50
                });
            }
            else
            {
                // Split into lines:
                string[] lines = NEW_LINE_REGEX.Split(text);
                Match quoteMatch;
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    quoteMatch = QUOTATION_REGEX.Match(line);
                    if (quoteMatch.Success)
                    {
                        line = line.Substring(quoteMatch.Length);

                    }

                    if (i < lines.Length - 1)
                    {
                        line += '\n';
                    }

                    int lastPosition = 0;
                    Match[] matches = new Match[3] { Match.Empty, Match.Empty, Match.Empty };

                    do
                    {
                        try
                        {
                            matches[0] = URL_REGEX.Match(line, lastPosition);
                            matches[1] = EMAIL_REGEX.Match(line, lastPosition);
                            matches[2] = PHONE_REGEX.Match(line, lastPosition);
                        }
                        catch (RegexMatchTimeoutException) { }

                        Match firstMatch = matches.Where(x => !(x is null) && x.Success).OrderBy(x => x.Index)?.FirstOrDefault();
                        if (firstMatch == matches[0])
                        {
                            // the first match is an URL:
                            CreateRunElement(textBlock, line, lastPosition, firstMatch.Index, quoteMatch.Success);
                            lastPosition = CreateUrlElement(textBlock, firstMatch, quoteMatch.Success);
                        }
                        else if (firstMatch == matches[1])
                        {
                            // the first match is an email:
                            CreateRunElement(textBlock, line, lastPosition, firstMatch.Index, quoteMatch.Success);
                            lastPosition = CreateContactElement(textBlock, firstMatch, null, quoteMatch.Success);
                        }
                        else if (firstMatch == matches[2])
                        {
                            // the first match is a phone number:
                            CreateRunElement(textBlock, line, lastPosition, firstMatch.Index, quoteMatch.Success);
                            lastPosition = CreateContactElement(textBlock, null, firstMatch, quoteMatch.Success);
                        }
                        else
                        {
                            // no match, we add the whole text:
                            CreateRunElement(textBlock, line, lastPosition, line.Length, quoteMatch.Success);
                            lastPosition = line.Length;
                        }

                    } while (lastPosition < line.Length);
                }
            }
        }

        #endregion
    }
}
