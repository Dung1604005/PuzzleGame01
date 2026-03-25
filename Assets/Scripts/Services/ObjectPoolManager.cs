using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
[System.Serializable]
public class PoolConfig
{
    public GameObject Prefab;
    public int DefaultCapacity = 10;
    public int MaxSize = 100;

    public Transform transformParent;

    public bool AutoPrewarm = true;
}
public class ObjectPoolManager : MonoBehaviour
{
    private const int DefaultPoolCapacity = 10;
    private const int DefaultPoolMaxSize = 50;

    private sealed class PoolRuntimeInfo
    {
        public Transform PoolParent;
        public IObjectPool<GameObject> Pool;
    }

    public static ObjectPoolManager Instance { get; private set; }

    [Header("Pool Configurations (Tùy chọn)")]
    [Tooltip("Khai báo dung lượng riêng cho từng Prefab. Nếu không khai báo sẽ dùng mặc định.")]
    public List<PoolConfig> poolConfigs = new List<PoolConfig>();

    private Dictionary<int, PoolRuntimeInfo> _poolInfos = new Dictionary<int, PoolRuntimeInfo>();
    private Dictionary<GameObject, PoolRuntimeInfo> _spawnedObjects = new Dictionary<GameObject, PoolRuntimeInfo>();

    // Từ điển tra cứu cấu hình nhanh
    private Dictionary<int, PoolConfig> _configDict = new Dictionary<int, PoolConfig>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Đổ List từ Inspector vào Dictionary
        foreach (var config in poolConfigs)
        {
            if (config.Prefab != null)
            {
                _configDict[config.Prefab.GetInstanceID()] = config;
            }
        }
    }

    private void Start()
    {
        // Duyệt qua tất cả các cấu hình bạn đã kéo thả trên Inspector
        foreach (var config in poolConfigs)
        {
            // Nếu bạn tick vào ô "AutoPrewarm"
            if (config.Prefab != null && config.AutoPrewarm)
            {
                // Tự động gọi Prewarm 
                Prewarm(config.Prefab, config.DefaultCapacity);
            }
        }
    }

    private PoolRuntimeInfo GetPoolInfo(GameObject prefab)
    {
        int prefabID = prefab.GetInstanceID();

        if (!_poolInfos.TryGetValue(prefabID, out PoolRuntimeInfo poolInfo))
        {
            int dCapacity = DefaultPoolCapacity;
            int mSize = DefaultPoolMaxSize;
            Transform parent = transform;

            if (_configDict.TryGetValue(prefabID, out PoolConfig config))
            {
                dCapacity = config.DefaultCapacity;
                mSize = config.MaxSize;
                if (config.transformParent != null)
                {
                    parent = config.transformParent;
                }
            }

            poolInfo = new PoolRuntimeInfo
            {
                PoolParent = parent
            };

            poolInfo.Pool = new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    GameObject obj = Instantiate(prefab, parent);
                    obj.SetActive(false);
                    return obj;
                },
                actionOnGet: (obj) => obj.SetActive(true),
                actionOnRelease: (obj) =>
                {
                    if (obj.transform.parent != parent)
                    {
                        obj.transform.SetParent(parent, false);
                    }
                    obj.SetActive(false);
                },
                actionOnDestroy: (obj) => Destroy(obj),
                collectionCheck: true,
                defaultCapacity: dCapacity,
                maxSize: mSize
            );

            _poolInfos.Add(prefabID, poolInfo);
        }
        return poolInfo;
    }

    // --- CÁC HÀM API DÀNH CHO CÁC CLASS KHÁC GỌI ---


    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parentOverride = null)
    {
        if (prefab == null)
        {
            Debug.LogError("Spawn thất bại: prefab null.");
            return null;
        }

        PoolRuntimeInfo poolInfo = GetPoolInfo(prefab);
        GameObject obj = poolInfo.Pool.Get();
        Transform targetParent = parentOverride != null ? parentOverride : poolInfo.PoolParent;
        Transform objTransform = obj.transform;

        if (objTransform.parent != targetParent)
        {
            objTransform.SetParent(targetParent, false);
        }

        objTransform.SetPositionAndRotation(position, rotation);
        objTransform.localScale = Vector3.one;

        _spawnedObjects[obj] = poolInfo;

        return obj;
    }


    public void Despawn(GameObject obj)
    {
        if (obj == null)
        {
            return;
        }

        if (_spawnedObjects.TryGetValue(obj, out PoolRuntimeInfo poolInfo))
        {
            obj.transform.localScale = Vector3.one;
            poolInfo.Pool.Release(obj);
            _spawnedObjects.Remove(obj);
        }
        else
        {
            Debug.LogWarning($"Object {obj.name} không thuộc hệ thống Pool. Tiến hành Destroy mặc định.");
            Destroy(obj);
        }
    }

    /// Ép hệ thống đẻ sẵn một số lượng object và cất vào kho ngay lập tức để tránh lag.

    public void Prewarm(GameObject prefab, int count)
    {
        if (prefab == null || count <= 0)
        {
            return;
        }

        PoolRuntimeInfo poolInfo = GetPoolInfo(prefab);
        List<GameObject> tempObjects = ListPool<GameObject>.Get();

        try
        {
            for (int i = 0; i < count; i++)
            {
                tempObjects.Add(poolInfo.Pool.Get());
            }

            foreach (GameObject obj in tempObjects)
            {
                poolInfo.Pool.Release(obj);
            }
        }
        finally
        {
            tempObjects.Clear();
            ListPool<GameObject>.Release(tempObjects);
        }
    }
}