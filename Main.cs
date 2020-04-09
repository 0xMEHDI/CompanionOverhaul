using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using System.Windows.Forms;

namespace CompanionOverhaul
{
    public class Main : MBSubModuleBase 
    {
        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

            InformationManager.DisplayMessage(new InformationMessage("Character Overhaul: Loaded Module", Color.FromUint(4282569842U)));
        }

        public override void OnNewGameCreated(Game game, object initializerObject)
        {
            base.OnNewGameCreated(game, initializerObject);

            List<CharacterObject> characterObjects =
                new List<CharacterObject>(from o in CharacterObject.Templates 
                                          where o.IsFemale && o.Culture != null 
                                          select o) ;

            foreach (CharacterObject o in characterObjects)
            {
                try
                {
                    switch (o.Culture.GetCultureCode())
                    {
                        case CultureCode.Empire:
                            SetBodyProperties(o,
                            GenerateStaticBodyProperties("000B0C04400000014884400F00808484F010440F08440008444870007007000001377613030000000000540000804F8000000000000000000000000004740003"),
                            GenerateStaticBodyProperties("001CFC0CC0003010988BD00F80E08988F077B84F588888094E88944778FF071401A776130A1544440007AE0001804F800000000000000000000000007B444003"));
                            break;

                        case CultureCode.Sturgia:
                            SetBodyProperties(o,
                            GenerateStaticBodyProperties("00000C00000000018884401F11908484F000440F38541004448780107007000001777613071100300000A40000804F8000000000000000000000000004FC0003"),
                            GenerateStaticBodyProperties("000B8002C0003010888BB01F10C0B888F080BB4F88B9F0088987B55977FF071301C776130C5445C00007DF0000844E80000000000000000000000000798C4003"));
                            break;

                        case CultureCode.Aserai:
                            SetBodyProperties(o,
                            GenerateStaticBodyProperties("00167C0A000000014884400F00808484F000440E08884004488880007007000001377613030000100000400000804F8000000000000000000000000006740003"),
                            GenerateStaticBodyProperties("002DF40FC0003010988CC00F08938F87F178BB8F88CCE4084EF8888777FE0E1501A776130A4544900007AB0000804F800000000000000000000000007D4C4003"));
                            break;

                        case CultureCode.Vlandia:
                            SetBodyProperties(o,
                            GenerateStaticBodyProperties("00050C01400000018884401F11908484F000440F38540004447780007007000001577613050000000000540000804F8000000000000000000000000004BC0003"),
                            GenerateStaticBodyProperties("000B940580003010888BB01F10C0B888F080BB4F88B9F0088988B55977FF071301977613095445C000077B0000844E800000000000000000000000007F3C4003"));
                            break;

                        case CultureCode.Khuzait:
                            SetBodyProperties(o,
                            GenerateStaticBodyProperties("00057C0CC00000018884401E00807484E034470F78FF8104844881107117001101577613050000100000700011804E8000000000000000000000000000BC0003"),
                            GenerateStaticBodyProperties("001CFC0E40003010C889800E80F07488F0FBBB8FE8FFF00488E884177EFE0F0701B776130B4480F10007CB0011704E800000000000000000000000007F844003"));
                            break;

                        case CultureCode.Battania:
                            SetBodyProperties(o,
                            GenerateStaticBodyProperties("00050C02C00000018884401F11908484F000440F38540004448780107007000001877613080000100000740000804F8000000000000000000000000004FC0003"),
                            GenerateStaticBodyProperties("000BC00700003010888BB01F10C0B888F080BB4F88B9F0088988B55977FF071301C776130C5445C00007AB0000844E800000000000000000000000007F8C4003"));
                            break;

                        default:
                            break;
                    }
                }

                catch (Exception e)
                {
                    Exception exception = e.InnerException;
                    MessageBox.Show("[Companion Overhaul] ERROR Updating Companions: " + e.Message 
                        + "\n\n" + (exception?.Message));
                }
            }

            InformationManager.DisplayMessage(new InformationMessage("Character Overhaul: Companions Updated", Color.FromUint(4282569842U)));
        }

        private void SetBodyProperties(CharacterObject characterObject, StaticBodyProperties bodyPropertiesMin, StaticBodyProperties bodyPropertiesMax)
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