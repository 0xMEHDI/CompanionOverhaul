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

            List<CharacterObject> characterObjects =
                new List<CharacterObject>(from o in CharacterObject.Templates 
                                          where o.IsFemale && o.Culture != null 
                                          select o) ;

            try
            {
                foreach (CharacterObject o in characterObjects)
                {
                    ParseXML(@"..\..\Modules\CompanionOverhaul\BodyProperties.xml", o.Culture.GetCultureCode().ToString(), 
                        out string min, out string max);
                    SetBodyProperties(o, GenerateStaticBodyProperties(min), GenerateStaticBodyProperties(max));
                }

                InformationManager.DisplayMessage(new InformationMessage("Companions Updated", Color.FromUint(4281584691)));
            }

            catch (Exception e)
            {
                MessageBox.Show("Companion Overhaul // ERROR Updating Companions.\n\n" +
                    e.Message + "\n" + e.InnerException?.Message);

                InformationManager.DisplayMessage(new InformationMessage("Error Updating Companions\nNo changes have been applied",
                    Color.FromUint(4294901760)));
            }
        }

        private bool ParseXML(string filePath, string cultureCode, out string min, out string max)
        {
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

        private void SetBodyProperties(CharacterObject characterObject, 
            StaticBodyProperties bodyPropertiesMin, StaticBodyProperties bodyPropertiesMax)
        {
            characterObject.StaticBodyPropertiesMin = bodyPropertiesMin;
            characterObject.StaticBodyPropertiesMax = bodyPropertiesMax;
        }

        private StaticBodyProperties GenerateStaticBodyProperties(string key)
        {
            List<ulong> keyParts = new List<ulong>();

            for (int i = 0; i < key.Length; i += 16)
                keyParts.Add(Convert.ToUInt64(key.Substring(i, 16), 16));

            return new StaticBodyProperties(
                keyParts[0],
                keyParts[1],
                keyParts[2],
                keyParts[3],
                keyParts[4],
                keyParts[5],
                keyParts[6],
                keyParts[7]
                );
        }
    }
}