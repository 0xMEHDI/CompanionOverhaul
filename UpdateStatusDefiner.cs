using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.SaveSystem;
using TaleWorlds.SaveSystem.Definition;

namespace CompanionOverhaul
{
    class UpdateStatusDefiner : SaveableTypeDefiner
    {
        public UpdateStatusDefiner() : base(450100) { }

        protected override void DefineClassTypes()
        {
            AddClassDefinition(typeof(UpdateStatusData), 450101);
        }
    }
}
