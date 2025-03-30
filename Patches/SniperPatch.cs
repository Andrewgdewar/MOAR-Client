using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EFT.Game.Spawning;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;

namespace MOAR.Patches
{
    public class SniperPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod() =>
            AccessTools.Method(typeof(SpawnPointManagerClass), "smethod_1");

        private static BotZone GetNearestZone(List<BotZone> zones, string fallbackName)
        {
            return zones.FirstOrDefault(z => z.NameZone == fallbackName)
                ?? zones[UnityEngine.Random.Range(0, zones.Count)];
        }

        private static bool IsSnipeZoneName(string name)
        {
            return !string.IsNullOrEmpty(name) && name.ToLowerInvariant().Contains("custom_snipe");
        }

        private static string GetBotZoneNameById(SpawnPointParams[] spawnPoints, string id)
        {
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                if (spawnPoints[i].Id == id)
                    return spawnPoints[i].BotZoneName;
            }
            return string.Empty;
        }

        private static void SetBotZoneName(SpawnPointParams[] spawnPoints, string id, string newName)
        {
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                if (spawnPoints[i].Id == id)
                {
                    spawnPoints[i].BotZoneName = newName;
                    return;
                }
            }
        }

        [PatchPostfix]
        private static void Postfix(ref SpawnPointMarker[] __result, SpawnPointParams[] parameters)
        {
            if (__result == null || parameters == null || __result.Length == 0 || parameters.Length == 0)
            {
                Plugin.LogSource.LogInfo("SniperPatch: No spawn points to process.");
                return;
            }

            var snipeZones = new List<BotZone>();
            var regularZones = new List<BotZone>();

            foreach (var marker in __result)
            {
                if (marker?.BotZone != null && !marker.BotZone.IsNullOrDestroyed())
                {
                    if (marker.BotZone.SnipeZone)
                        snipeZones.Add(marker.BotZone);
                    else
                        regularZones.Add(marker.BotZone);
                }
            }

            if (snipeZones.Count == 0 || regularZones.Count == 0)
            {
                Plugin.LogSource.LogInfo("SniperPatch: No valid snipe or regular zones found.");
                return;
            }

            foreach (var marker in __result)
            {
                if (marker == null || marker.SpawnPoint.Categories == ESpawnCategoryMask.None || marker.SpawnPoint.Categories.ContainPlayerCategory())
                    continue;

                if (marker.BotZone != null && !marker.BotZone.IsNullOrDestroyed())
                    continue;

                string zoneName = GetBotZoneNameById(parameters, marker.Id);

                if (!snipeZones.Any(z => z.NameZone == zoneName) && !IsSnipeZoneName(zoneName))
                {
                    var fallbackZone = GetNearestZone(regularZones, zoneName);
                    AccessTools.Field(typeof(BotZone), "_maxPersons").SetValue(fallbackZone, -1);
                    marker.BotZone = fallbackZone;
                }
                else
                {
                    if (IsSnipeZoneName(zoneName))
                        SetBotZoneName(parameters, marker.Id, "");

                    var snipeZone = GetNearestZone(snipeZones, zoneName);
                    int newMax = snipeZone.MaxPersons > 0 ? snipeZone.MaxPersons + 1 : 5;
                    AccessTools.Field(typeof(BotZone), "_maxPersons").SetValue(snipeZone, newMax);
                    marker.BotZone = snipeZone;
                }
            }

            Plugin.LogSource.LogInfo("SniperPatch: BotZone reassignment complete.");
        }
    }
}
