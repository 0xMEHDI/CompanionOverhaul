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

            int count = 0;

            try
            {
                foreach (CharacterObject o in companions)
                {
                    ParseXML(filePath, o.Culture.GetCultureCode().ToString(), out string min, out string max);
                    SetBodyProperties(o, GenerateStaticBodyProperties(min), GenerateStaticBodyProperties(max));

                    count++;
                }
            }

            catch (Exception e)
            {
                MessageBox.Show("Companion Overhaul | Error Updating Companions.\n\n" +
                    e.Message + "\n\n" + e.InnerException?.Message);
            }

            if (count == companions.Count)
                InformationManager.DisplayMessage(new InformationMessage(count + " Companions Updated", Color.FromUint(4281584691)));

            else if (count < companions.Count)
                InformationManager.DisplayMessage(new InformationMessage("Error Updating Companions\n" + 
                    count + " out of " + companions.Count + " Companions Updated", Color.FromUint(4294901760)));

            else
                InformationManager.DisplayMessage(new InformationMessage("If you see this, " +
                    "you've just witnessed some black fucking magic cause I have no idea how this happened", 
                    Color.FromUint(4294901760)));
        }

        /// <summary>
        /// Parses the provided XML file and return the appropriate <see cref="keyLength"/> chars long key
        /// required by <see cref="GenerateStaticBodyProperties(string)"/> to generate a <see cref="StaticBodyProperties"/>.
        /// </summary>
        /// <param name="filePath">Relative <see cref="filePath"/> of the external XML.</param>
        /// <param name="cultureCode">String required to compare against IDs parsed from the external XML.</param>
        /// <param name="min">Out parameter required to store the <see cref="CharacterObject.StaticBodyPropertiesMin"/> required by
        /// <see cref="SetBodyProperties(CharacterObject, StaticBodyProperties, StaticBodyProperties)"/>.</param>
        /// <param name="max">Out parameter required to store the <see cref="CharacterObject.StaticBodyPropertiesMax"/> required by
        /// <see cref="SetBodyProperties(CharacterObject, StaticBodyProperties, StaticBodyProperties)"/>.</param>
        /// <returns>Returns true when successful.</returns>
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

            return true;
        }

        /// <summary>
        /// Updates the given <see cref="CharacterObject"/>'s 
        /// <see cref="CharacterObject.StaticBodyPropertiesMin"/> and 
        /// <see cref="CharacterObject.StaticBodyPropertiesMax"/>.
        /// </summary>
        /// <param name="characterObject">The <see cref="CharacterObject"/> to update.</param>
        /// <param name="bodyPropertiesMin">The new <see cref="CharacterObject.StaticBodyPropertiesMin"/> to apply.</param>
        /// <param name="bodyPropertiesMax">The new <see cref="CharacterObject.StaticBodyPropertiesMax"/> to apply.</param>
        private void SetBodyProperties(CharacterObject characterObject, 
            StaticBodyProperties bodyPropertiesMin, StaticBodyProperties bodyPropertiesMax)
        {
            characterObject.StaticBodyPropertiesMin = bodyPropertiesMin;
            characterObject.StaticBodyPropertiesMax = bodyPropertiesMax;
        }

        /// <summary>
        /// Generates a <see cref="StaticBodyProperties"/> from a <see cref="keyLength"/> chars long key 
        /// by parsing and spliting it into <see cref="keyPartCount"/> constituant parts of length <see cref="keyPartLength"/>.
        /// </summary>
        /// <param name="key">Key required to generate the <see cref="StaticBodyProperties"/> object.</param>
        /// <returns>Returns the generated <see cref="StaticBodyProperties"/> object.</returns>
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