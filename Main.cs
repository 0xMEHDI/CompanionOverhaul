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
using System.Reflection;

namespace CompanionOverhaul
{
    public class Main : MBSubModuleBase
    {
        private static readonly int keyLength = 128;
        private static readonly int keyPartCount = 8;
        private static readonly int keyPartLength = keyLength / keyPartCount;
        private readonly string bodyPropertiesPath = @"..\..\Modules\CompanionOverhaul\BodyProperties.xml";

        private bool isLoaded = false;

        private XmlDocument bodyPropertiesFile;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            bodyPropertiesFile = new XmlDocument();
            bodyPropertiesFile.Load(bodyPropertiesPath);

            var harmony = new Harmony("companionoverhaul.characterobject.patch");
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

        public override bool DoLoading(Game game)
        {
            base.DoLoading(game);

            try 
            {
                if (Campaign.Current.CampaignGameLoadingType == Campaign.GameLoadingType.NewCampaign)
                {
                    List<CharacterObject> templates =
                        new List<CharacterObject>(from o in CharacterObject.Templates
                                                  where o.IsFemale && o.Occupation == Occupation.Wanderer
                                                  select o);

                    if (templates.Count == 0)
                        throw new Exception("Template list is empty");

                    int editedTemplateCount = 0;

                    foreach (CharacterObject o in templates)
                    {
                        ParseXML(o.Culture.GetCultureCode().ToString(), out string min, out string max);

                        o.StaticBodyPropertiesMin = GenerateStaticBodyProperties(min);
                        o.StaticBodyPropertiesMax = GenerateStaticBodyProperties(max);

                        editedTemplateCount++;
                    }

                    if (editedTemplateCount == templates.Count)
                        InformationManager.DisplayMessage(new InformationMessage("All templates successfully updated", Color.FromUint(4281584691)));
                    else if (editedTemplateCount < templates.Count && editedTemplateCount > 0)
                        InformationManager.DisplayMessage(new InformationMessage(editedTemplateCount + " companions successfully updated", Color.FromUint(4281584691)));
                }

                if (Campaign.Current.CampaignGameLoadingType == Campaign.GameLoadingType.SavedCampaign)
                {
                    List<Hero> companions =
                          new List<Hero>(from o in Hero.All
                                         where o.IsFemale && o.IsWanderer
                                         select o);

                    if (companions.Count == 0)
                        throw new Exception("Companion list is empty");

                    int editedCompanionCount = 0;
                    Random rand = new Random();

                    foreach (Hero h in companions)
                    {
                        ParseXML(h.Culture.GetCultureCode().ToString(), out string min, out string max);

                        h.CharacterObject.UpdatePlayerCharacterBodyProperties(
                                BodyProperties.GetRandomBodyProperties(h.IsFemale,
                                    new BodyProperties(h.DynamicBodyProperties, GenerateStaticBodyProperties(min)),
                                    new BodyProperties(h.DynamicBodyProperties, GenerateStaticBodyProperties(max)),
                                    (int)h.CharacterObject.Equipment.HairCoverType, rand.Next(),
                                    h.CharacterObject.HairTags,
                                    h.CharacterObject.BeardTags,
                                    h.CharacterObject.TattooTags),
                                h.IsFemale);

                        editedCompanionCount++;
                    }

                    if (editedCompanionCount == companions.Count && companions.Count != 0)
                        InformationManager.DisplayMessage(new InformationMessage($"All {editedCompanionCount} companions successfully updated", Color.FromUint(4281584691)));
                    else if (editedCompanionCount < companions.Count && editedCompanionCount > 0)
                        InformationManager.DisplayMessage(new InformationMessage(editedCompanionCount + " companions successfully updated", Color.FromUint(4281584691)));
                } 
            }

            catch (Exception e)
            {
                MessageBox.Show("Companion Overhaul: Error during companion update\n\n" + e.Message + "\n\n" + e.InnerException?.Message);
                InformationManager.DisplayMessage(new InformationMessage("Error updating companions", Color.FromUint(4294901760)));
            }

            return true;
        }

        private bool ParseXML(string cultureCode, out string min, out string max)
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

            return true;
        }

        private StaticBodyProperties GenerateStaticBodyProperties(string key)
        {
            if (key.Length != keyLength)
                throw new FormatException("Key must be of length " + keyLength);

            List<ulong> keyParts = new List<ulong>(keyPartCount);

            for (int i = 0; i < keyLength; i += keyPartLength)
                keyParts.Add(Convert.ToUInt64(key.Substring(i, keyPartLength), keyPartLength));

            return new StaticBodyProperties(
                keyParts[0],
                keyParts[1],
                keyParts[2],
                keyParts[3],
                keyParts[4],
                keyParts[5],
                keyParts[6],
                keyParts[7]);
        }
    }
}