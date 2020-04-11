using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace CompanionOverhaul
{
    public class UpdateStatus : CampaignBehaviorBase
    {
        public static UpdateStatusData StatusData = new UpdateStatusData();

        public override void RegisterEvents() { }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("_CompanionOverhaulUpdateStatusData", ref StatusData);
        }
    }
}
