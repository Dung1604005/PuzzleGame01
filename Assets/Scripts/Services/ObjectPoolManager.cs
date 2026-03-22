using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
[System.Serializable]
public class PoolConfig
{
    public GameObject Prefab;
    public int DefaultCapacity = 10;
    public int MaxSize = 100;

    public bool AutoPrewarm = true;
}
public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }

    [Header("Pool Configurations (Tùy chọn)")]
    [Tooltip("Khai báo dung lượng riêng cho từng Prefab. Nếu không khai báo sẽ dùng mặc định.")]
    public List<PoolConfig> poolConfigs = new List<PoolConfig>();

    private Dictionary<int, IObjectPool<GameObject>> _pools = new Dictionary<int, IObjectPool<GameObject>>();
    private Dictionary<GameObject, IObjectPool<GameObject>> _spawnedObjects = new Dictionary<GameObject, IObjectPool<GameObject>>();

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

    private IObjectPool<GameObject> GetPool(GameObject prefab)
    {
        int prefabID = prefab.GetInstanceID();

        if (!_pools.TryGetValue(prefabID, out IObjectPool<GameObject> pool))
        {
            // 1. Khởi tạo thông số mặc định 
            int dCapacity = 10;
            int mSize = 50;

            // 2. Kiểm tra xem Prefab này có cấu hình riêng không?
            if (_configDict.TryGetValue(prefabID, out PoolConfig config))
            {
                dCapacity = config.DefaultCapacity;
                mSize = config.MaxSize;
            }

            // 3. Tạo Pool với thông số chuẩn xác
            pool = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(prefab, transform),
                actionOnGet: (obj) => obj.SetActive(true),
                actionOnRelease: (obj) => obj.SetActive(false),
                actionOnDestroy: (obj) => Destroy(obj),
                collectionCheck: true,
                defaultCapacity: dCapacity, // Lấy từ cấu hình
                maxSize: mSize              // Lấy từ cấu hình
            );

            _pools.Add(prefabID, pool);
        }
        return pool;
    }

    // --- CÁC HÀM API DÀNH CHO CÁC CLASS KHÁC GỌI ---


    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        IObjectPool<GameObject> pool = GetPool(prefab);
        GameObject obj = pool.Get(); // Mượn 1 object ra

        // Reset state from previous use before placing.
        obj.transform.SetParent(transform, false);
        obj.transform.localScale = Vector3.one;
        obj.transform.position = position;
        obj.transform.rotation = rotation;

        // Đánh dấu object này thuộc về pool nào để lát trả lại cho đúng
        _spawnedObjects[obj] = pool;

        return obj;
    }


    public void Despawn(GameObject obj)
    {
        if (_spawnedObjects.TryGetValue(obj, out IObjectPool<GameObject> pool))
        {
            obj.transform.SetParent(this.transform, false);
            obj.transform.localScale = Vector3.one;
            pool.Release(obj); // Trả về kho
            _spawnedObjects.Remove(obj); // Xóa khỏi danh sách đang mượn
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
        IObjectPool<GameObject> pool = GetPool(prefab);
        List<GameObject> tempObjects = new List<GameObject>();
        // 1. Mượn ra liên tục (Ép Pool phải gọi Instantiate n lần)
        for (int i = 0; i < count; i++)
        {
            tempObjects.Add(pool.Get());
        }
        // 2. Trả lại ngay lập tức (Ép Pool cất toàn bộ n cái vào kho)
        foreach (var obj in tempObjects)
        {
            pool.Release(obj);
        }
    }
}