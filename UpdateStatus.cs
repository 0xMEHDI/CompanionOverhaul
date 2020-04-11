using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.SaveSystem;

namespace CompanionOverhaul
{
    public class UpdateStatus
    {
        [SaveableField(0)] 
        public bool hasUpdated;
    }

    public class UpdateStatusDefiner : SaveableTypeDefiner
    {
        public UpdateStatusDefiner() : base(450100) { }

        protected override void DefineClassTypes()
        {
            AddClassDefinition(typeof(UpdateStatus), 450101);
        }
    }
}
