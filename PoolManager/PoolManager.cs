using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; } = null;

    /// <summary>
    /// Then set to true new objects will be added to the pool if trying to Get() from empty pool.
    /// </summary>
    [Tooltip("Then set to true new objects will be added to the pool if trying to Get() from empty pool.")]
    [SerializeField] private bool dynamicExtend = true;

    [Tooltip("Size of autoextended. Recommended to keep at 1.")]
    [SerializeField][Range(1, 1000)] private int extendAmount = 1;

    /// <summary>
    /// Then set to true will create empty game objects and parent pools to them.
    /// </summary>
    [Tooltip("Then set to true will create empty game objects and parent pools to them.")]
    [SerializeField] private bool poolParenting = true;

    /// <summary>
    /// Automatically adds PoolObject components on all CreatePool requests. Not recommended.
    /// </summary>
    [Tooltip("Automatically adds PoolObject components on all CreatePool requests. Not recommended.")]
    [SerializeField] private bool autoAddComponent = false;

    /// <summary>
    /// List of prefabs you want pooled at runtime.
    /// </summary>
    [Tooltip("List of prefabs to be pooled on Awake().")]
    [SerializeField] private List<GameObject> prefabsToPool = null;

    /// <summary>
    /// Size for default pools. Can't be less than 1.
    /// </summary>
    [Tooltip("Default pool size.")]
    [SerializeField][Range(1, 1000)] private int defaultSize = 10;

    /// <summary>
    /// Dictionary of all created pools, using prefab id as key.
    /// </summary>
    private Dictionary<int, Queue<GameObject>> pools = new Dictionary<int, Queue<GameObject>>();

    public void CreatePool(GameObject prefab, int size)
    {
        if (size <= 0)
        {
            print("Pool size too small.");
            return;
        }
        if (prefab.GetComponent<PoolObject>() == null)
        {
            if (!autoAddComponent)
            {
                print("Pooled objects need to have PoolObject component attached.");
                return;
            }
            else
            {
                prefab.AddComponent<PoolObject>();
            }
        }

        int key = prefab.GetInstanceID();

        if (!pools.ContainsKey(key))
        {
            GameObject parentObject = Instantiate(new GameObject());
            parentObject.name = $"_{prefab.name} pool";
            parentObject.transform.parent = gameObject.transform;

            pools.Add(key, new Queue<GameObject>());

            for (int i = 0; i < size; i++)
            {
                GameObject newObject = Instantiate(prefab);
                newObject.transform.parent = parentObject.transform;
                newObject.GetComponent<PoolObject>().PoolKey = key;
                newObject.SetActive(false);
                pools[key].Enqueue(newObject);
            }
        }
    }

    #region GET OBJECT FROM POOL

    /// <summary>
    /// Instantiate game object from pool.
    /// </summary>
    /// <param name="prefab">A prefab from which pool was created.</param>
    /// <returns>Object from pool.</returns>
    public GameObject Get(GameObject prefab)
    {
        int key = prefab.GetInstanceID();
        if (!pools.ContainsKey(key))
        {
            print("No pool with " + prefab.name + "'s instance ID has been found.");
            return null;
        }
        if (pools[key].Count == 0)
        {
            if (dynamicExtend)
            {
                CreatePool(prefab, extendAmount);
            }
            else
            {
                print("Trying to access empty pool with dynamicExtend set to false.");
                return null;
            }
        }
        GameObject obj = pools[key].Dequeue();
        obj.SetActive(true);
        return obj;
    }

    /// <summary>
    /// Instantiate game object from pool.
    /// </summary>
    /// <param name="prefab">A prefab from which pool was created.</param>
    /// <param name="position">Sets position of returned object to Vector3.</param>
    /// <returns>Object from pool.</returns>
    public GameObject Get(GameObject prefab, Vector3 position)
    {
        int key = prefab.GetInstanceID();
        if (!pools.ContainsKey(key))
        {
            print("No pool with " + prefab.name + "'s instance ID has been found.");
            return null;
        }
        if (pools[key].Count == 0)
        {
            if (dynamicExtend)
            {
                CreatePool(prefab, extendAmount);
            }
            else
            {
                print("Trying to access empty pool with dynamicExtend set to false.");
                return null;
            }
        }

        GameObject obj = pools[key].Dequeue();
        obj.transform.position = position;
        obj.SetActive(true);
        return obj;
    }

    /// <summary>
    /// Instantiate game object from pool.
    /// </summary>
    /// <param name="prefab">A prefab from which pool was created.</param>
    /// <param name="position">Sets position of returned object to Vector3.</param>
    /// <param name="rotation">Sets rotation of returned object to Quaternion.</param>
    /// <returns>Object from pool.</returns>
    public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int key = prefab.GetInstanceID();
        if (!pools.ContainsKey(key))
        {
            print("No pool with " + prefab.name + "'s instance ID has been found.");
            return null;
        }
        if (pools[key].Count == 0)
        {
            if (dynamicExtend)
            {
                CreatePool(prefab, extendAmount);
            }
            else
            {
                print("Trying to access empty pool with dynamicExtend set to false.");
                return null;
            }
        }

        GameObject obj = pools[key].Dequeue();
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);
        return obj;
    }

    #endregion GET OBJECT FROM POOL

    #region RETURN OBJECT TO POOL

    /// <summary>
    /// Returns THIS game object to pool.
    /// </summary>
    public void Return()
    {
        TryGetComponent<PoolObject>(out PoolObject poolObject);
        if (poolObject == null || !pools.ContainsKey(poolObject.PoolKey))
        {
            print(name + "isn't part of any existing pool");
            return;
        }

        gameObject.SetActive(false);
        pools[poolObject.PoolKey].Enqueue(gameObject);
    }

    /// <summary>
    /// Returns game object to pool.
    /// </summary>
    /// <param name="objectToReturn"></param>
    public void Return(GameObject objectToReturn)
    {
        objectToReturn.TryGetComponent<PoolObject>(out PoolObject poolObject);
        if (poolObject == null || !pools.ContainsKey(poolObject.PoolKey))
        {
            print(objectToReturn.name + "isn't part of any existing pool");
            return;
        }

        objectToReturn.SetActive(false);
        pools[poolObject.PoolKey].Enqueue(objectToReturn);
    }

    /// <summary>
    /// Returns game object to pool.
    /// </summary>
    /// <param name="poolObject"></param>
    public void Return(PoolObject poolObject)
    {
        if (!pools.ContainsKey(poolObject.PoolKey))
        {
            print(poolObject.gameObject.name + "isn't part of any existing pool");
            return;
        }
        poolObject.gameObject.SetActive(false);
        pools[poolObject.PoolKey].Enqueue(poolObject.gameObject);
    }

    #endregion RETURN OBJECT TO POOL

    #region SETTING UP SINGLETON

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
        foreach (GameObject prefab in prefabsToPool)
        {
            CreatePool(prefab, defaultSize);
        }
    }

    #endregion SETTING UP SINGLETON
}