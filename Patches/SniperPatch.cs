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
        private static double Sq(double n) => n * n;

        private static double Pt(double a, double b) => Math.Sqrt(Sq(a) + Sq(b));

        public static double GetDistance(double x, double y, double z, double mX, double mY, double mZ)
        {
            x = Math.Abs(x - mX);
            y = Math.Abs(y - mY);
            z = Math.Abs(z - mZ);
            return Pt(Pt(x, z), y);
        }

        public static double GetVectorDistance(Vector3 v1, Vector3 v2)
        {
            return GetDistance(v1.x, v1.y, v1.z, v2.x, v2.y, v2.z);
        }

        public static BotZone FindFarthestZone(List<BotZone> botZones, Vector3 referencePoint)
        {
            if (botZones == null || botZones.Count == 0)
                throw new ArgumentException("The botZones list cannot be null or empty.");

            var list = botZones.OrderBy(zone => GetVectorDistance(zone.CenterOfSpawnPoints, referencePoint)).ToList();
            var midIndex = list.Count / 2;
            var filteredZones = list.Skip(midIndex).ToList();
            var random = new System.Random();
            return filteredZones[random.Next(filteredZones.Count)];
        }

        private static BotZone GetNearestZone(List<BotZone> zones, string name)
        {
            var botZone = zones.FirstOrDefault(zone => zone.NameZone == name);
            return botZone ?? zones[new System.Random().Next(zones.Count)];
        }

        private static PatrolWay GetRandomPatrol(PatrolWay[] patrol)
        {
            var random = new System.Random();
            return patrol[random.Next(patrol.Length)];
        }

        private static bool IsNameInBotzones(List<BotZone> zones, string name)
        {
            return zones.Any(zone => zone.NameZone == name);
        }

        public static string GetBotZoneNameById(SpawnPointParams[] spawnPoints, string id)
        {
            foreach (var spawnPoint in spawnPoints)
            {
                if (spawnPoint.Id == id && spawnPoint.BotZoneName != null)
                {
                    return spawnPoint.BotZoneName;
                }
            }
            return string.Empty;
        }

        public static void SetBotZoneName(SpawnPointParams[] spawnPoints, string id, string newName)
        {
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                if (spawnPoints[i].Id == id)
                {
                    spawnPoints[i].BotZoneName = newName;
                }
            }
        }

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(SpawnPointManagerClass), "smethod_1");
        }

        [PatchPostfix]
        private static void Postfix(ref SpawnPointMarker[] __result, SpawnPointParams[] parameters)
        {
            Plugin.LogSource.LogInfo("Attempting spawnzone updates");

            if (__result == null || parameters == null || __result.Length == 0 || parameters.Length == 0)
            {
                Plugin.LogSource.LogInfo("MOAR: Skipping implementation due to empty or null data.");
                return;
            }

            var snipeZones = new List<BotZone>();
            var regularZones = new List<BotZone>();

            foreach (var spawnPointMarker in __result)
            {
                if (spawnPointMarker?.BotZone?.IsNullOrDestroyed() == false)
                {
                    if (spawnPointMarker.BotZone.SnipeZone)
                    {
                        snipeZones.Add(spawnPointMarker.BotZone);
                    }
                    else
                    {
                        regularZones.Add(spawnPointMarker.BotZone);
                    }
                }
            }

            if (!snipeZones.Any() || !regularZones.Any()) return;

            var filteredRegularZones = regularZones.ApplyFilter(zone => !zone.SnipeZone);

            for (int i = 0; i < __result.Length; i++)
            {
                var spawnPointMarker2 = __result[i];
                if (spawnPointMarker2 == null || spawnPointMarker2.SpawnPoint.Categories == ESpawnCategoryMask.None || spawnPointMarker2.SpawnPoint.Categories.ContainPlayerCategory())
                    continue;

                if (spawnPointMarker2.BotZone?.IsNullOrDestroyed() == true)
                {
                    var botZoneName = GetBotZoneNameById(parameters, spawnPointMarker2.Id);
                    if (!IsNameInBotzones(snipeZones, botZoneName) && !botZoneName.ToLower().Contains("custom_snipe"))
                    {
                        var nearestZone = GetNearestZone(filteredRegularZones, botZoneName);
                        if (nearestZone.MaxPersons != -1)
                        {
                            AccessTools.Field(typeof(BotZone), "_maxPersons").SetValue(nearestZone, -1);
                        }
                        spawnPointMarker2.BotZone = nearestZone;
                    }
                    else
                    {
                        if (botZoneName.ToLower().Contains("custom_snipe"))
                        {
                            SetBotZoneName(parameters, spawnPointMarker2.Id, "");
                            botZoneName = "";
                        }

                        var nearestZone = GetNearestZone(snipeZones, botZoneName);
                        var num = nearestZone.MaxPersons > 0 ? nearestZone.MaxPersons + 1 : 5;
                        AccessTools.Field(typeof(BotZone), "_maxPersons").SetValue(nearestZone, num);
                        spawnPointMarker2.BotZone = nearestZone;
                    }
                }
            }

            Plugin.LogSource.LogInfo("Spawnzone updates complete");
        }
    }
}
