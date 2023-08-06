using HarmonyLib;
using KitchenData;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenRiggedUpgrades.Patches
{
    [HarmonyPatch]
    internal static class GameDataConstructor_Patch
    {
        private const int MAX_ID_CONFLICT_REATTEMPTS = 1000;

        static List<(GameDataObject gdo, bool isNonPersistent)> GDOsToRegister = new List<(GameDataObject, bool)>();

        [HarmonyPatch(typeof(GameDataConstructor), "BuildGameData")]
        [HarmonyPrefix]
        static void BuildGameData_Prefix(List<GameDataObject> ___GameDataObjects, Dictionary<int, GameDataObject> ___All)
        {
            Main.LogWarning("BuildGameData");
            Main.LogInfo(GDOsToRegister.Count);
            foreach (bool isNonPersistent in new bool[] { false, true })
            {
                foreach (var item in GDOsToRegister)
                {
                    Main.LogInfo($"{item}");
                    Main.LogInfo(item.isNonPersistent);
                    if (item.isNonPersistent != isNonPersistent)
                        continue;
                    if (item.gdo.ID == 0)
                    {
                        Main.LogError($"Failed to register {item.gdo.GetType()}! ID cannot be 0.");
                        continue;
                    }

                    bool shouldRegister = true;
                    if (___All.ContainsKey(item.gdo.ID))
                    {
                        if (item.isNonPersistent)
                        {
                            item.gdo.ID = Random.Range(int.MinValue, int.MaxValue);
                            for (int i = 0; i < MAX_ID_CONFLICT_REATTEMPTS; i++)
                            {
                                item.gdo.ID++;
                                if (!___All.ContainsKey(item.gdo.ID))
                                    break;
                                if (i == MAX_ID_CONFLICT_REATTEMPTS - 1)
                                {
                                    shouldRegister = false;
                                    Main.LogError($"Failed to register {item.gdo.GetType()}! ID {item.gdo.ID - MAX_ID_CONFLICT_REATTEMPTS} to {item.gdo.ID} already in use.");
                                }
                            }
                        }
                        else
                        {
                            shouldRegister = false;
                            Main.LogError($"Failed to register {item.gdo.GetType()}! ID {item.gdo.ID} already in use.");
                        }
                    }

                    if (shouldRegister)
                    {
                        Main.LogWarning($"Added GDO {item.gdo.ID}");
                        ___GameDataObjects.Add(item.gdo);
                        ___All.Add(item.gdo.ID, item.gdo);
                    }
                }
            }
        }

        public static void AddGameDataObject(GameDataObject gameDataObject)
        {
            Main.LogInfo("AddGameDataObject");
            GDOsToRegister.Add((gameDataObject, false));
        }

        public static T Add<T>() where T : GameDataObject, new()
        {
            Main.LogInfo($"AddGameDataObject<{typeof(T).Name}>");
            T gdo = ScriptableObject.CreateInstance<T>();
            GDOsToRegister.Add((gdo, false));
            return gdo;
        }
    }
}
