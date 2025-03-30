using System.Collections.Generic;
using System.Linq;
using Comfort.Common;
using EFT;
using EFT.Game.Spawning;
using MOAR.Helpers;
using UnityEngine;

namespace MOAR.Components
{
    /// <summary>
    /// Draws labels above spawn points in real-time for development/debug purposes.
    /// </summary>
    public class BotZoneRenderer : MonoBehaviour
    {
        private static Player Player => Singleton<GameWorld>.Instance?.MainPlayer;

        private readonly List<BotZone> _botZones = new();
        private readonly List<SpawnPointInfo> _spawnPointInfos = new();
        private GUIStyle _guiStyle;
        private float _screenScale = 1f;

        private void Awake()
        {
            if (CameraClass.Instance?.SSAA?.isActiveAndEnabled == true)
            {
                float output = CameraClass.Instance.SSAA.GetOutputWidth();
                float input = CameraClass.Instance.SSAA.GetInputWidth();
                if (input > 0) _screenScale = output / input;
            }

            RefreshZones();
        }

        public void RefreshZones()
        {
            _botZones.Clear();
            _spawnPointInfos.Clear();

            _botZones.AddRange(LocationScene.GetAllObjectsAndWhenISayAllIActuallyMeanIt<BotZone>());

            foreach (var zone in _botZones)
                AddZoneSpawnPoints(zone);
        }

        private void AddZoneSpawnPoints(BotZone zone)
        {
            Color zoneColor = GenerateZoneColor();
            string zoneId = zone.Id.ToString();

            foreach (var point in zone.SpawnPoints)
            {
                string label = point.GetBotDebugName();
                _spawnPointInfos.Add(new SpawnPointInfo(point.Position, new GUIContent($"{zoneId} [{label}]"), zoneColor));
            }
        }

        private void OnGUI()
        {
            if (!Settings.enablePointOverlay.Value) return;
            if (Player == null || Camera.main == null) return;

            _guiStyle ??= CreateLabelStyle();

            foreach (var info in _spawnPointInfos)
            {
                if (string.IsNullOrEmpty(info.Content.text)) continue;

                float distance = Vector3.Distance(info.Position, Player.Transform.position);
                if (distance > 200f) continue;

                Vector3 screenPos = Camera.main.WorldToScreenPoint(info.Position + Vector3.up * 1.5f);
                if (screenPos.z <= 0f) continue;

                Vector2 size = _guiStyle.CalcSize(info.Content);
                Rect labelRect = new(
                    screenPos.x * _screenScale - size.x / 2f,
                    Screen.height - (screenPos.y * _screenScale + size.y),
                    size.x,
                    size.y);

                GUI.Box(labelRect, info.Content, _guiStyle);
            }
        }

        private void OnDestroy() => _spawnPointInfos.Clear();

        private GUIStyle CreateLabelStyle()
        {
            return new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14,
                normal = { textColor = Color.white },
                wordWrap = false
            };
        }

        private static Color GenerateZoneColor()
        {
            return new Color(Random.value, Random.value, Random.value, 0.6f);
        }

        /// <summary>
        /// Internal data container for spawn point label rendering.
        /// </summary>
        private class SpawnPointInfo
        {
            public Vector3 Position { get; }
            public GUIContent Content { get; }

            public SpawnPointInfo(Vector3 pos, GUIContent content, Color color)
            {
                Position = pos;
                Content = content;
            }
        }
    }

    public static class SpawnPointExtensions
    {
        public static string GetBotDebugName(this ISpawnPoint spawnPoint)
        {
            if (spawnPoint == null) return "NullPoint";

            if (spawnPoint.GetType().GetProperty("BotTemplateId")?.GetValue(spawnPoint) is string value)
                return value;

            return spawnPoint.ToString();
        }
    }
}
