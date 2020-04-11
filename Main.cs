using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using System.Windows.Forms;
using System.Xml;
using HarmonyLib;

namespace CompanionOverhaul
{
    public class Main : MBSubModuleBase
    {
        private bool isLoaded;
        private static XmlDocument bodyPropertiesFile;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            bodyPropertiesFile = new XmlDocument();
            bodyPropertiesFile.Load(bodyPropertiesPath);

            var harmony = new Harmony("mehdi.companionoverhaul.patch");
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

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);

            try 
            {
                List<CharacterObject> templates = new List<CharacterObject>(from o in CharacterObject.Templates
                                                                            where o.IsFemale && o.Occupation == Occupation.Wanderer
                                                                            select o);
                if (Campaign.Current != null)
                {
                    if (Campaign.Current.CampaignGameLoadingType == Campaign.GameLoadingType.NewCampaign)
                    {
                        UpdateTemplates(templates);
                        UpdateStatus.StatusData.hasUpdated = true;
                    }
                        
                    else if (Campaign.Current.CampaignGameLoadingType == Campaign.GameLoadingType.SavedCampaign)
                    {
                        List<Hero> companions = new List<Hero>(from o in Hero.All
                                                               where o.IsFemale && o.IsWanderer
                                                               select o);

                        if (!UpdateStatus.StatusData.hasUpdated)
                        {
                            UpdateCompanions(companions);
                            UpdateStatus.StatusData.hasUpdated = true;
                        }
                        
                        /*
                        InformationManager.ShowInquiry(new InquiryData("Companion Overhaul", "Regenerate companions?", true, true, "Yes", "No",
                        delegate(){ UpdateCompanions(companions); InformationManager.DisplayMessage(new InformationMessage("Companions faces updated", GREEN)); },
                        delegate(){ InformationManager.DisplayMessage(new InformationMessage("Companions faces unchanged", RED)); }), false);
                        */
                    }
                }
            }

            catch (Exception e)
            {
                MessageBox.Show("Companion Overhaul: Error during companion update\n\n" + e.Message + "\n\n" + e.InnerException?.Message);
                InformationManager.DisplayMessage(new InformationMessage("Error updating companions", RED));
            }
        }

        private void UpdateTemplates(List<CharacterObject> templates)
        {
            if (templates.Count == 0)
                throw new Exception("Template list is empty");

            foreach (CharacterObject o in templates)
            {
                GenerateStaticBodyProperties(o.Culture.GetCultureCode().ToString(), out StaticBodyProperties sbpMin, out StaticBodyProperties sbpMax);

                o.StaticBodyPropertiesMin = sbpMin;
                o.StaticBodyPropertiesMax = sbpMax;
            }

            InformationManager.DisplayMessage(new InformationMessage("Templates successfully updated", GREEN));
        }

        private void UpdateCompanions(List<Hero> companions)
        {
            if (companions.Count == 0)
                throw new Exception("Companion list is empty");

            Random rand = new Random();
            foreach (Hero h in companions)
            {   
                GenerateStaticBodyProperties(h.Culture.GetCultureCode().ToString(), out StaticBodyProperties sbpMin, out StaticBodyProperties sbpMax);

                h.CharacterObject.UpdatePlayerCharacterBodyProperties(
                        BodyProperties.GetRandomBodyProperties(h.IsFemale,
                            new BodyProperties(h.DynamicBodyProperties, sbpMin),
                            new BodyProperties(h.DynamicBodyProperties, sbpMax),
                            (int)h.CharacterObject.Equipment.HairCoverType, rand.Next(),
                            h.CharacterObject.HairTags,
                            h.CharacterObject.BeardTags,
                            h.CharacterObject.TattooTags),
                        h.IsFemale);
            }

            InformationManager.DisplayMessage(new InformationMessage("Companions successfully updated", GREEN));
        }

        private void GenerateStaticBodyProperties(string cultureCode, out StaticBodyProperties sbpMin, out StaticBodyProperties sbpMax)
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

        private void ParseKeys(string cultureCode, out string min, out string max)
        {
            if (cultureCode == null)
                throw new ArgumentNullException("CultureCode is null");

            min = max = "";

            foreach (XmlNode node in bodyPropertiesFile.DocumentElement.ChildNodes)
                if (node.Attributes["id"]?.InnerText == cultureCode)
                {
                    min = node.FirstChild.Attributes["value"]?.InnerText;
                    max = node.LastChild.Attributes["value"]?.InnerText;
                    break;
                }

            if (min == null || max == null)
                throw new ArgumentNullException("Returned key is null");
        }

        private static readonly int keyLength = 128;
        private static readonly int keyPartCount = 8;
        private readonly int keyPartLength = keyLength / keyPartCount;
        private readonly string bodyPropertiesPath = @"..\..\Modules\CompanionOverhaul\ModuleData\BodyProperties.xml";

        private static readonly Color GREEN = Color.FromUint(4281584691);
        private static readonly Color RED = Color.FromUint(4294901760);
    }
}