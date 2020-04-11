using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace CompanionOverhaul
{
    public class UpdateBehaviour : CampaignBehaviorBase
    {
        public static UpdateStatus StatusData = new UpdateStatus();

        public override void RegisterEvents() 
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(
                this, new Action<CampaignGameStarter>(OnGameFinishedLoading));
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("_CompanionOverhaulUpdateStatusData", ref StatusData);
        }

        private void OnGameFinishedLoading(CampaignGameStarter obj)
        {
            try
            {
                if (Campaign.Current != null)
                    UpdateCompanions();
            }

            catch (Exception e)
            {
                MessageBox.Show("Companion Overhaul: Error during companion update\n\n" + e.Message + "\n\n" + e.InnerException?.Message);
                InformationManager.DisplayMessage(new InformationMessage("Error updating companions", Main.RED));
            }
        }

        private void UpdateCompanions()
        {
            if (StatusData.hasUpdated)
                return;

            List<Hero> companions = new List<Hero>(from o in Hero.All
                                                   where o.IsFemale && o.IsWanderer
                                                   select o);
            if (companions.Count == 0)
                throw new Exception("Companion list is empty");

            Random rand = new Random();
            foreach (Hero h in companions)
            {
                Main.GenerateStaticBodyProperties(h.Culture.GetCultureCode().ToString(), out StaticBodyProperties sbpMin, out StaticBodyProperties sbpMax);

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

            StatusData.hasUpdated = true;

            InformationManager.DisplayMessage(new InformationMessage("Companions successfully updated", Main.GREEN));
        }
    }
}
