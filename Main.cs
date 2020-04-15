using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using System.Xml;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CompanionOverhaul
{
    public class Main : MBSubModuleBase
    {
        private bool isLoaded;
        private UpdateBehaviour updateStatus;
        public static XmlDocument bodyPropertiesFile;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            bodyPropertiesFile = new XmlDocument();
            bodyPropertiesFile.Load(bodyPropertiesPath);

            var harmony = new Harmony("accrcd.companionoverhaul.patch");
            harmony.PatchAll();
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

            if (!isLoaded)
            {
                InformationManager.DisplayMessage(new InformationMessage("Loaded Companion Overhaul", Color.FromUint(4281584691)));
                isLoaded = true;
            }
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);

            if (Campaign.Current == null)
                return;

            CampaignGameStarter gameInitializer = (CampaignGameStarter)gameStarterObject;
            updateStatus = new UpdateBehaviour();
            gameInitializer.AddBehavior(updateStatus);
        }

        public static void GenerateStaticBodyProperties(string cultureCode, out StaticBodyProperties sbpMin, out StaticBodyProperties sbpMax)
        {
            ParseKeys(cultureCode, out string min, out string max);

            if (min.Length != keyLength || max.Length != keyLength)
                throw new FormatException("Keys must be of length " + keyLength);

            List<ulong> minKeyParts = new List<ulong>(keyPartCount);
            List<ulong> maxKeyParts = new List<ulong>(keyPartCount);

            for (int i = 0; i < keyLength; i += keyPartLength)
            {
                minKeyParts.Add(Convert.ToUInt64(min.Substring(i, keyPartLength), keyPartLength));
                maxKeyParts.Add(Convert.ToUInt64(max.Substring(i, keyPartLength), keyPartLength));
            }

            sbpMin = new StaticBodyProperties(minKeyParts[0], minKeyParts[1], minKeyParts[2], minKeyParts[3],
                                              minKeyParts[4], minKeyParts[5], minKeyParts[6], minKeyParts[7]);
            sbpMax = new StaticBodyProperties(maxKeyParts[0], maxKeyParts[1], maxKeyParts[2], maxKeyParts[3],
                                              maxKeyParts[4], maxKeyParts[5], maxKeyParts[6], maxKeyParts[7]);
        }

        private static void ParseKeys(string cultureCode, out string min, out string max)
        {
            if (cultureCode == null)
                throw new ArgumentNullException("CultureCode is null");

            min = max = "";

            foreach (XmlNode node in Main.bodyPropertiesFile.DocumentElement.ChildNodes)
                if (node.Attributes["id"]?.InnerText == cultureCode)
                {
                    min = node.FirstChild.Attributes["value"]?.InnerText;
                    max = node.LastChild.Attributes["value"]?.InnerText;
                    break;
                }

            if (min == null || max == null)
                throw new ArgumentNullException("Returned key is null");
        }

        private const int keyLength = 128;
        private const int keyPartCount = 8;
        private const int keyPartLength = keyLength / keyPartCount;

        private static readonly string bodyPropertiesPath = @"..\..\Modules\CompanionOverhaul\ModuleData\BodyProperties.xml";

        public static readonly Color GREEN = Color.FromUint(4281584691);
        public static readonly Color RED = Color.FromUint(4294901760);
    }
}