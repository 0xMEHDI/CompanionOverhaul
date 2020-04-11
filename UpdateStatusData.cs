using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.SaveSystem;

namespace CompanionOverhaul
{
    public class UpdateStatusData
    {
        [SaveableField(0)] 
        public bool hasUpdated;
    }
}
