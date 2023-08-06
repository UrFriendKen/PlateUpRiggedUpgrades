using Kitchen.Layouts;
using Kitchen;
using KitchenData;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenRiggedUpgrades.Utils
{
    internal static class SnapshotUtils
    {
        private static Dictionary<int, Texture2D> Snapshots = new Dictionary<int, Texture2D>();

        private static float NightFade;

        private static readonly int Fade = Shader.PropertyToID("_NightFade");

        private static int CacheMaxSize = 20;

        private static Dictionary<int, Texture2D> _CachedImages = new Dictionary<int, Texture2D>();

        private static void CacheShaderValues()
        {
            NightFade = Shader.GetGlobalFloat(Fade);
            Shader.SetGlobalFloat(Fade, 0f);
        }

        private static void ResetShaderValues()
        {
            Shader.SetGlobalFloat(Fade, NightFade);
        }

        public static void ClearCache()
        {
            Snapshots.Clear();
        }

        public static bool HasSnapshot(int cache_id)
        {
            if (Snapshots == null)
            {
                Snapshots = new Dictionary<int, Texture2D>();
            }
            return Snapshots.ContainsKey(cache_id);
        }

        public static Texture2D GetSnapshot(GameObject prefab)
        {
            int instanceID = prefab.GetInstanceID();
            if (Snapshots == null)
            {
                Snapshots = new Dictionary<int, Texture2D>();
            }
            if (!Snapshots.ContainsKey(instanceID) || Snapshots[instanceID] == null)
            {
                CacheShaderValues();
                Quaternion rotation = Quaternion.LookRotation(new Vector3(0f, 0f, 1f), Vector3.up);
                SnapshotTexture snapshotTexture = Snapshot.RenderPrefabToTexture(128, 128, prefab, rotation, 0.5f, 0.5f);
                ResetShaderValues();
                Snapshots[instanceID] = snapshotTexture.Snapshot;
            }
            return Snapshots[instanceID];
        }

        public static Texture2D GetCosmeticSnapshot(PlayerCosmetic cosmetic, int width = 512, int height = 512)
        {
            int iD = cosmetic.ID;
            if (Snapshots == null)
            {
                Snapshots = new Dictionary<int, Texture2D>();
            }
            if (!Snapshots.ContainsKey(iD) || Snapshots[iD] == null)
            {
                CacheShaderValues();
                GameObject gameObject = Object.Instantiate((cosmetic.CosmeticType == CosmeticType.Hat) ? GameData.Main.ReferableObjects.CosmeticHatSnapshotPrefab : GameData.Main.ReferableObjects.CosmeticBodySnapshotPrefab);
                PlayerCosmeticSubview component = gameObject.GetComponent<PlayerCosmeticSubview>();
                component.SetCosmetic(cosmetic);
                SnapshotTexture snapshotTexture = Snapshot.RenderToTexture(width, height, component.gameObject, 1f, 1f, -10f, 10f, component.transform.localPosition);
                ResetShaderValues();
                gameObject.SetActive(value: false);
                Object.Destroy(gameObject);
                Snapshots[iD] = snapshotTexture.Snapshot;
            }
            return Snapshots[iD];
        }

        public static Texture2D GetCardSnapshot(UnlockCardElement element, ICard card, int width = 512, int height = 512)
        {
            int num = ((card is UnlockCard unlockCard) ? unlockCard.ID : ((card is Dish dish) ? dish.ID : 0));
            if (Snapshots == null)
            {
                Snapshots = new Dictionary<int, Texture2D>();
            }
            if (num != 0 || !Snapshots.ContainsKey(num) || Snapshots[num] == null)
            {
                CacheShaderValues();
                element.SetUnlock(card);
                SnapshotTexture snapshotTexture = Snapshot.RenderToTexture(width, height, element.gameObject, 1f, 1f, -10f, 10f, element.transform.localPosition);
                ResetShaderValues();
                Snapshots[num] = snapshotTexture.Snapshot;
            }
            return Snapshots[num];
        }

        public static Texture2D GetItemSnapshot(GameObject prefab)
        {
            int instanceID = prefab.GetInstanceID();
            if (Snapshots == null)
            {
                Snapshots = new Dictionary<int, Texture2D>();
            }
            if (!Snapshots.ContainsKey(instanceID) || Snapshots[instanceID] == null)
            {
                CacheShaderValues();
                Quaternion rotation = Quaternion.LookRotation(new Vector3(1f, -1f, 1f), new Vector3(0f, 1f, 1f));
                SnapshotTexture snapshotTexture = Snapshot.RenderPrefabToTexture(512, 512, prefab, rotation, 0.5f, 0.5f);
                ResetShaderValues();
                Snapshots[instanceID] = snapshotTexture.Snapshot;
            }
            return Snapshots[instanceID];
        }

        public static Texture2D GetApplianceSnapshot(GameObject prefab)
        {
            int instanceID = prefab.GetInstanceID();
            if (Snapshots == null)
            {
                Snapshots = new Dictionary<int, Texture2D>();
            }
            if (!Snapshots.ContainsKey(instanceID) || Snapshots[instanceID] == null)
            {
                CacheShaderValues();
                Quaternion rotation = Quaternion.LookRotation(new Vector3(1f, -1f, 1f), new Vector3(0f, 1f, 1f));
                SnapshotTexture snapshotTexture = Snapshot.RenderPrefabToTexture(512, 512, prefab, rotation, 0.5f, 0.5f, -10f, 10f, 0.5f, -0.25f * new Vector3(0f, 1f, 1f));
                ResetShaderValues();
                Snapshots[instanceID] = snapshotTexture.Snapshot;
            }
            return Snapshots[instanceID];
        }

        public static Texture2D GetFoodSnapshot(GameObject prefab, ItemView.ViewData data)
        {
            GameObject gameObject = Object.Instantiate(prefab);
            gameObject.GetComponent<ItemView>().UpdateData(data);
            CacheShaderValues();
            Quaternion rotation = Quaternion.LookRotation(new Vector3(0f, -0.5f, 0.5f), Vector3.up);
            SnapshotTexture snapshotTexture = Snapshot.RenderPrefabToTexture(128, 128, gameObject, rotation, 0.5f, 0.5f);
            ResetShaderValues();
            Object.Destroy(gameObject);
            return snapshotTexture.Snapshot;
        }

        public static Texture2D GetLayoutSnapshot(SiteView prefab, LayoutBlueprint blueprint)
        {
            int iD = blueprint.ID;
            if (_CachedImages.TryGetValue(iD, out var value) && value != null)
            {
                return value;
            }
            SiteView siteView = Object.Instantiate(prefab);
            siteView.UpdateData(new SiteView.ViewData
            {
                Floorplan = blueprint
            });
            CacheShaderValues();
            Quaternion rotation = Quaternion.LookRotation(new Vector3(1f, 0f, 0f), Vector3.up);
            SnapshotTexture snapshotTexture = Snapshot.RenderPrefabToTexture(128, 128, siteView.gameObject, rotation, 1.5f, 1.5f);
            ResetShaderValues();
            Object.Destroy(siteView.GameObject);
            if (_CachedImages.Count > CacheMaxSize)
            {
                _CachedImages.Clear();
            }
            _CachedImages[iD] = snapshotTexture.Snapshot;
            return snapshotTexture.Snapshot;
        }
    }
}
