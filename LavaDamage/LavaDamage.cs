using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using JetBrains.Annotations;

namespace Stollie.LavaDamage
{
    [BepInPlugin(PluginId, "LavaDamage", "1.0.0")]
    public class LavaDamage : BaseUnityPlugin
    {
        public const string PluginId = "stollie.mods.lava_damage";
        private Harmony _harmony;
        private static LavaDamage _instance;
        private static ConfigEntry<bool> _loggingEnabled;
        private static ConfigEntry<float> _lavaDamage;

        [UsedImplicitly]
        private void Awake()
        {
            _instance = this;
            _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PluginId);
            _loggingEnabled = Config.Bind("Logging", "Logging Enabled", true, "Enable logging");
            _lavaDamage = Config.Bind<float>("Lava Damage", "Lava Damage Amount", 100.0f, "Amount of Lava Damage Sustained - Vanilla is 100.0f");
            Config.SettingChanged += Config_SettingChanged;
        }

        private void Config_SettingChanged(object sender, SettingChangedEventArgs e)
        {
            Log($"LavaDamage should now be {_lavaDamage.Value}");
        }

        [HarmonyPatch(typeof(Character), nameof(Character.UpdateHeatDamage))]
        public static class Character_UpdateHeatDamage_Patch
        {
            public static void Prefix(Character __instance)
            {
                __instance.m_lavaFullDamage = _lavaDamage.Value;
            }
        }

        #region Logging
        public static void Log(string message)
        {
            if (_loggingEnabled.Value)
                _instance.Logger.LogInfo($"LAVADAMAGE LOG: {message}");
        }

        public static void LogWarning(string message)
        {
            if (_loggingEnabled.Value)
                _instance.Logger.LogWarning($"LAVADAMAGE LOG: {message}");
        }

        public static void LogError(string message)
        {
            if (_loggingEnabled.Value)
                _instance.Logger.LogError($"LAVADAMAGE LOG: {message}");
        }
        #endregion

        [UsedImplicitly]
        private void OnDestroy()
        {
            _instance = null;
            _harmony?.UnpatchSelf();
        }
    }
}



