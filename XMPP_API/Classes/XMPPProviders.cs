using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace XMPP_API.Classes
{
    /// <summary>
    /// Represents on curated server provider.
    /// </summary>
    public struct Provider
    {
        public string jid;
        public bool company;
        public bool passwordReset;
        public int ratingComplianceTester;
        public string ratingImObservatoryCtS;
        public string ratingImObservatoryStS;
        public int maxFileUploadSize;
        public int fileUploadStorageTime;
        public int mamStorageTime;
        public bool professionalHosting;
        public bool free;
        public string legalNotice;
        public DateTime onlineSince;
    }

    /// <summary>
    /// A curated list of XMPP server providers based on:
    /// https://invent.kde.org/melvo/xmpp-providers/-/tree/master
    /// </summary>
    public class XMPPProviders
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        public static readonly XMPPProviders INSTANCE = new XMPPProviders();

        private static readonly string[] ADDITIONAL_PROVIDERS_SIMPLE = new string[] {
            "xmpp.uwpx.org",
            "dukgo.com",
            "mailbox.org",
            "kein.ninja"
        };

        public string[] providersASimple;
        public string[] providersBSimple;
        public string[] providersCSimple;

        public Provider[] providersA;
        public Provider[] providersB;
        public Provider[] providersC;

        /// <summary>
        /// Path to the simple list of A-level providers.
        /// </summary>
        private const string A_LEVEL_SIMPLE_PATH = @"Resources/Providers/providers-A-simple.json";
        /// <summary>
        /// Path to the list of A-level providers.
        /// </summary>
        private const string A_LEVEL_PATH = @"Resources/Providers/providers-A.json";
        /// <summary>
        /// Path to the simple list of B-level providers.
        /// </summary>
        private const string B_LEVEL_SIMPLE_PATH = @"Resources/Providers/providers-B-simple.json";
        /// <summary>
        /// Path to the list of B-level providers.
        /// </summary>
        private const string B_LEVEL_PATH = @"Resources/Providers/providers-B.json";
        /// <summary>
        /// Path to the simple list of C-level providers.
        /// </summary>
        private const string C_LEVEL_SIMPLE_PATH = @"Resources/Providers/providers-C-simple.json";
        /// <summary>
        /// Path to the list of C-level providers.
        /// </summary>
        private const string C_LEVEL_PATH = @"Resources/Providers/providers-C.json";

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        private XMPPProviders() { }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--


        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--
        /// <summary>
        /// Initialized the curated list of server providers and loads all them from the files.
        /// </summary>
        public async Task initAsync()
        {
            await loadProvidersSimpleAsync();
            await loadProvidersAsync();
        }

        #endregion

        #region --Misc Methods (Private)--

        private async Task loadProvidersSimpleAsync()
        {
            providersASimple = await loadProvidersSimpleFromFileAsync(A_LEVEL_SIMPLE_PATH);
            providersBSimple = await loadProvidersSimpleFromFileAsync(B_LEVEL_SIMPLE_PATH);
            providersCSimple = await loadProvidersSimpleFromFileAsync(C_LEVEL_SIMPLE_PATH);
        }

        private async Task loadProvidersAsync()
        {
            providersA = await loadProvidersFromFileAsync(A_LEVEL_PATH);
            providersB = await loadProvidersFromFileAsync(B_LEVEL_PATH);
            providersC = await loadProvidersFromFileAsync(C_LEVEL_PATH);
        }

        private async Task<string[]> loadProvidersSimpleFromFileAsync(string path)
        {
            Logger.Info($"Loading simple providers from: {path}");
            List<string> providers;
            try
            {
                using (StreamReader r = new StreamReader(path))
                {
                    string json = await r.ReadToEndAsync();
                    providers = JsonConvert.DeserializeObject<List<string>>(json);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Failed to load simple providers from file '{path}' with:", e);
                return ADDITIONAL_PROVIDERS_SIMPLE;
            }
            Logger.Info($"Loaded {providers.Count} simple providers from {path} successfully.");
            providers.AddRange(ADDITIONAL_PROVIDERS_SIMPLE);
            return providers.ToArray();
        }

        private List<Provider> parseProviders(IEnumerable<object> providers)
        {
            List<Provider> result = new List<Provider>();
            foreach (JObject p in providers)
            {
                try
                {
                    result.Add(new Provider
                    {
                        jid = p.Value<string>("jid"),
                        company = p.Value<bool>("company"),
                        fileUploadStorageTime = p.Value<int>("maximumHttpFileUploadStorageTime"),
                        free = p.Value<bool>("freeOfCharge"),
                        legalNotice = p.Value<JObject>("legalNotice")?.Value<string>("en"),
                        mamStorageTime = p.Value<int>("maximumMessageArchiveManagementStorageTime"),
                        maxFileUploadSize = p.Value<int>("maximumHttpFileUploadFileSize"),
                        onlineSince = DateTime.Parse(p.Value<string>("onlineSince")),
                        passwordReset = p.Values("passwordReset") is not null,
                        professionalHosting = p.Value<bool>("professionalHosting"),
                        ratingComplianceTester = p.Value<int>("ratingXmppComplianceTester"),
                        ratingImObservatoryCtS = p.Value<string>("ratingImObservatoryClientToServer"),
                        ratingImObservatoryStS = p.Value<string>("ratingImObservatoryServerToServer"),
                    });
                }
                catch (Exception e)
                {
                    Logger.Error("Failed to parse provider with: ", e);
                }
            }
            return result;
        }

        private async Task<Provider[]> loadProvidersFromFileAsync(string path)
        {
            Logger.Info($"Loading providers from: {path}");
            object providersObj;
            try
            {
                using (StreamReader r = new StreamReader(path))
                {
                    string json = await r.ReadToEndAsync();
                    providersObj = JsonConvert.DeserializeObject(json);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Failed to load providers from file '{path}' with:", e);
                return new Provider[] { };
            }

            List<Provider> providers = providersObj is IEnumerable<object> list ? parseProviders(list) : new List<Provider>();
            Logger.Info($"Loaded {providers.Count} providers from {path} successfully.");
            return providers.ToArray();
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
