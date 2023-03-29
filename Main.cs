using System;
using HarmonyLib;
using UnityModManagerNet;

namespace DvMod.Mph
{
    [EnableReloading]
    public static class Main
    {
        public static UnityModManager.ModEntry? mod;

        static public bool Load(UnityModManager.ModEntry modEntry)
        {
            mod = modEntry;

            modEntry.OnToggle = OnToggle;

            return true;
        }

        static private bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            var harmony = new Harmony(modEntry.Info.Id);
            if (value)
            {
                harmony.PatchAll();
                UnityModManager.ModEntry ccl = UnityModManager.FindMod("DVCustomCarLoader");
                if ((ccl?.Loaded ?? false) && ccl.Version >= Version.Parse("1.7.2"))
                    Mph.Speedometers.ModifyCustomCarPrefabs();
            }
            else
            {
                harmony.UnpatchAll(modEntry.Info.Id);
            }
            return true;
        }

        static public void DebugLog(string s)
        {
            mod?.Logger.Log(s);
        }
    }
}
