using HarmonyLib;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace CompanionOverhaul
{
    [HarmonyPatch(typeof(CharacterObject), "UpdatePlayerCharacterBodyProperties")]
    internal class CharacterObjectPatcher
    {
        static bool Prefix(CharacterObject __instance, BodyProperties properties, ref bool isFemale)
        {
            PropertyInfo staticBodyProperties = typeof(Hero).GetProperty("StaticBodyProperties",
                     BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            if (__instance.IsHero)
            {
                staticBodyProperties.SetValue(__instance.HeroObject, properties.StaticProperties);
                __instance.HeroObject.DynamicBodyProperties = properties.DynamicProperties;
                __instance.HeroObject.UpdatePlayerGender(isFemale);

                InformationManager.DisplayMessage(new InformationMessage("Updated " + __instance.Name));
            }

            return true;
        }
    }
}
