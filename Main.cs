using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using System.Windows.Forms;
using System.Xml;

namespace CompanionOverhaul
{
    public class Main : MBSubModuleBase 
    {
        private static readonly int keyLength = 128;
        private static readonly int keyPartCount = 8;
        private static readonly int keyPartLength = keyLength / keyPartCount;
        private static readonly string filePath = @"..\..\Modules\CompanionOverhaul\BodyProperties.xml";

        private bool isLoaded = false;

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

            if (!isLoaded)
            {
                InformationManager.DisplayMessage(new InformationMessage("Loaded Companion Overhaul", Color.FromUint(4281584691)));
                isLoaded = true;
            }
        }

        public override void OnNewGameCreated(Game game, object initializerObject)
        {
            base.OnNewGameCreated(game, initializerObject);

            List<CharacterObject> companions =
                new List<CharacterObject>(from o in CharacterObject.Templates 
                                          where o.IsFemale && o.Occupation.ToString() == "Wanderer"
                                          select o) ;

            int editedCount = 0;

            try
            {
                foreach (CharacterObject o in companions)
                {
                    ParseXML(filePath, o.Culture.GetCultureCode().ToString(), out string min, out string max);

                    o.StaticBodyPropertiesMin = GenerateStaticBodyProperties(min);
                    o.StaticBodyPropertiesMax = GenerateStaticBodyProperties(max);

                    editedCount++;
                }

                if (editedCount == companions.Count)
                    InformationManager.DisplayMessage(new InformationMessage($"All {editedCount} Companions Successfully Updated", Color.FromUint(4281584691)));

                else if (editedCount < companions.Count)
                    throw new Exception(companions.Count - editedCount + " Companions Failed to Update");

                else
                    throw new Exception("If you see this, you've just witness some black fucking magic cause I have no idea how this happened");
            }

            catch (Exception e)
            {
                MessageBox.Show("Companion Overhaul - Error Updating Companions.\n\n" +
                    e.Message + "\n\n" + e.InnerException?.Message);

                InformationManager.DisplayMessage(new InformationMessage("Error Updating Companions\n" +
                    (companions.Count - editedCount) + " Companions Failed to Update", Color.FromUint(4294901760)));
            }
        }

        private bool ParseXML(string filePath, string cultureCode, out string min, out string max)
        {
            if (cultureCode == null)
                throw new ArgumentNullException("CharacterObject has no CultureCode");

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(filePath);

            min = max = "";

            foreach (XmlNode node in xmlDocument.DocumentElement.ChildNodes)
            {
                if (node.Attributes["id"]?.InnerText == cultureCode)
                {
                    min = node.FirstChild.Attributes["value"]?.InnerText;
                    max = node.LastChild.Attributes["value"]?.InnerText;

                    break;
                }
            }

            if (min == null || max == null)
                throw new ArgumentNullException("Min and/or Max keys are NULL");

            return true;
        }

        private StaticBodyProperties GenerateStaticBodyProperties(string key)
        {
            if (key.Length != keyLength)
                throw new FormatException("StaticBodyProperty key isn't of length " + keyLength);

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