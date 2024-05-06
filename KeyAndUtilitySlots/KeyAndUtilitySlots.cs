using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using JetBrains.Annotations;
using AzuExtendedPlayerInventory;

namespace Stollie.KeyAndUtilitySlots
{
    [BepInPlugin(PluginId, "KeyAndUtilitySlots", "1.0.0")]
    [BepInDependency("Azumatt.AzuExtendedPlayerInventory", BepInDependency.DependencyFlags.HardDependency)]
    public class MyMod : BaseUnityPlugin
    {
        public const string PluginId = "stollie.mods.keyandutilityslots";
        private Harmony _harmony;
        private static MyMod _instance;
        private static ConfigEntry<bool> _loggingEnabled;
        
        private const string CRYPT_KEY_SLOT_NAME = "Crypt";
        private const string CRYPT_KEY_PREFAB_NAME = "CryptKey";
        
        private const string WISP_SLOT_NAME = "Wisp";
        private const string WISP_PREFAB_NAME = "Demister";

        private const string MEGINGJORD_SLOT_NAME = "Belt";
        private const string MEGINGJORD_PREFAB_NAME = "BeltStrength";

        [UsedImplicitly]
        private void Awake()
        {
            _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PluginId);
            _instance = this;

            _loggingEnabled = Config.Bind("Logging", "Logging Enabled", false, "Enable logging");

            API.OnHudAwake += (hud) => {  };
            API.SlotAdded += (slotName) => {  };

            bool isAPILoaded = API.IsLoaded();
            if (API.IsLoaded())
            {
                // Add a custom slot for the Crypt Key item
                int desiredCryptKeyPosition = SetSlotPositionWithGapAfterExistingSlots(0);
                bool cryptKeySlotAddedSuccess = API.AddSlot(CRYPT_KEY_SLOT_NAME, GetCryptKeyItem, CanPlaceCryptKeyItem, desiredCryptKeyPosition);
                if (!cryptKeySlotAddedSuccess)
                {
                    LogError("Failed to add custom slot for Crypt Key item.");
                }
                else
                    Log($"Successfully added Crypt Key slot to Slot# {desiredCryptKeyPosition}");

                // Add a custom slot for the Wisp item
                int desiredWispPosition = SetSlotPositionWithGapAfterExistingSlots(0);
                bool wispSlotAddedSuccess = API.AddSlot(WISP_SLOT_NAME, GetWispItem, CanPlaceWispItem, desiredWispPosition);
                if (!wispSlotAddedSuccess)
                {
                    LogError("Failed to add custom slot for Wisp item.");
                }
                else
                    Log($"Successfully added Wisp slot to Slot# {desiredWispPosition}");

                // Add a custom slot for the Megingjord item
                int desiredMegingjordPosition = SetSlotPositionWithGapAfterExistingSlots(0);
                bool megingjordSlotAddedSuccess = API.AddSlot(MEGINGJORD_SLOT_NAME, GetMegingjordItem, CanPlaceMegingjordItem, desiredMegingjordPosition);
                if (!megingjordSlotAddedSuccess)
                {
                    LogError("Failed to add custom slot for Megingjord item.");
                }
                else
                    Log($"Successfully added Megingjord slot to Slot# {desiredMegingjordPosition}");
            }
        }

        // Function to find the position for the new slot
        private int SetSlotPositionWithGapAfterExistingSlots(int slotGap)
        {
            // Get all current slots
            SlotInfo slots = API.GetSlots();
            Log($"Current Slot count is {slots.SlotNames.Length}");
            return slots.SlotNames.Length + slotGap;
        }

        // Function to get the Crypt Key item from the player's inventory
        private ItemDrop.ItemData GetCryptKeyItem(Player player)
        {
            // Check player's inventory for Crypt Key item
            foreach (ItemDrop.ItemData item in player.GetInventory().GetAllItems())
            {
                if (item.m_dropPrefab.name == CRYPT_KEY_PREFAB_NAME)
                {
                    return item;
                }
            }
            return null; // Crypt Key not found in inventory
        }

        // Function to check if the Crypt Key item can be placed in the custom slot
        private bool CanPlaceCryptKeyItem(ItemDrop.ItemData item)
        {
            if (item.m_dropPrefab.name != CRYPT_KEY_PREFAB_NAME)
                LogWarning("Tried to place non Crypt Key item in slot");
            else
                Log("Placed Crypt Key in slot");

            return item.m_dropPrefab.name == CRYPT_KEY_PREFAB_NAME; // Only allow Crypt Key item to be placed
        }

        // Function to get the Wisp item from the player's inventory
        private ItemDrop.ItemData GetWispItem(Player player)
        {
            // Check player's inventory for Wisp item
            foreach (ItemDrop.ItemData item in player.GetInventory().GetAllItems())
            {
                if (item.m_dropPrefab.name == WISP_PREFAB_NAME)
                {
                    return item;
                }
            }
            return null; // Wisp not found in inventory
        }

        // Function to check if the Wisp item can be placed in the custom slot
        private bool CanPlaceWispItem(ItemDrop.ItemData item)
        {
            if (item.m_dropPrefab.name != WISP_PREFAB_NAME)
                LogWarning("Tried to place non Wisp item in slot");
            else
                Log("Placed Wisp in slot");

            return item.m_dropPrefab.name == WISP_PREFAB_NAME; // Only allow Wisp item to be placed
        }

        // Function to get the Megingjord item from the player's inventory
        private ItemDrop.ItemData GetMegingjordItem(Player player)
        {
            // Check player's inventory for Megingjord item
            foreach (ItemDrop.ItemData item in player.GetInventory().GetAllItems())
            {
                if (item.m_dropPrefab.name == MEGINGJORD_PREFAB_NAME)
                {
                    return item;
                }
            }
            return null; // Megingjord not found in inventory
        }

        // Function to check if the Megingjord item can be placed in the custom slot
        private bool CanPlaceMegingjordItem(ItemDrop.ItemData item)
        {
            if (item.m_dropPrefab.name != MEGINGJORD_PREFAB_NAME)
                LogWarning("Tried to place non Megingjord item in slot");
            else
                Log("Placed Megingjord in slot");

            return item.m_dropPrefab.name == MEGINGJORD_PREFAB_NAME; // Only allow Megingjord item to be placed
        }

        #region Logging
        public static void Log(string message)
        {
            if (_loggingEnabled.Value)
                _instance.Logger.LogInfo($"KEYCHAIN LOG: {message}");
        }

        public static void LogWarning(string message)
        {
            if (_loggingEnabled.Value)
                _instance.Logger.LogWarning($"KEYCHAIN LOG: {message}");
        }

        public static void LogError(string message)
        {
            if (_loggingEnabled.Value)
                _instance.Logger.LogError($"KEYCHAIN LOG: {message}");
        }
        #endregion

        [UsedImplicitly]
        private void OnDestroy()
        {
            _harmony?.UnpatchSelf();
            _instance = null;
        }
    }
}



