using CCL_GameScripts.CabControls;
using DV.Signs;
using DVCustomCarLoader;
using DVCustomCarLoader.LocoComponents;
using HarmonyLib;
using System.Linq;
using TMPro;
using UnityEngine;

namespace DvMod.Mph
{
    public static class Constants
    {
        public const float KmPerMile = 1.609344f;
    }

    public static class MphSigns
    {
        [HarmonyPatch(typeof(TextMeshPro), nameof(TextMeshPro.Awake))]
        public static class TMPAwakePatch
        {
            public static void Postfix(TextMeshPro __instance)
            {
                var tmp = __instance;
                if (tmp.transform.GetComponentInParent<SignDebug>() == null)
                    return;

                var text = tmp.text;
                if (text.Length > 0 && text.All(char.IsDigit))
                {
                    if (int.TryParse(text, out var kmh))
                    {
                        var mph = Mathf.RoundToInt(kmh * 10f / Constants.KmPerMile / 5) * 5;
                        tmp.text = mph.ToString();
                    }
                }
            }
        }
    }

    public static class Speedometers
    {
        [HarmonyPatch(typeof(IndicatorsDiesel), nameof(IndicatorsDiesel.Start))]
        public static class DieselSpeedometerRangePatch
        {
            public static void Postfix(IndicatorsDiesel __instance)
            {
                __instance.speed.maxValue *= Constants.KmPerMile;
            }
        }

        [HarmonyPatch(typeof(IndicatorsShunter), nameof(IndicatorsShunter.Start))]
        public static class ShunterSpeedometerRangePatch
        {
            public static void Postfix(IndicatorsShunter __instance)
            {
                __instance.speed.maxValue *= Constants.KmPerMile;
            }
        }

        [HarmonyPatch(typeof(IndicatorsSteam), nameof(IndicatorsSteam.Start))]
        public static class SteamSpeedometerRangePatch
        {
            public static void Postfix(IndicatorsSteam __instance)
            {
                __instance.speed.maxValue *= Constants.KmPerMile;
            }
        }

        public static void ModifyCustomCarPrefabs()
        {
            foreach (var car in CustomCarManager.CustomCarTypes.Where(customCar => customCar.InteriorPrefab != null))
            {
                foreach (var relay in car.InteriorPrefab.GetComponentsInChildren<IndicatorRelay>())
                {
                    if (relay.EventBinding == SimEventType.Speed)
                    {
                        Main.DebugLog($"Adjusting speedometer for {car.identifier}");
                        relay.Indicator.maxValue *= Constants.KmPerMile;
                    }
                }
            }
        }
    }
}
