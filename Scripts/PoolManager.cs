using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{

    // TODO: Parenting, TryGetComponent, Edge cases of no objects in queue, cleanup
    public static PoolManager Instance { get; private set; } = null;


    /// <summary>
    /// Dictionary of all created pools, using prefab id as key.
    /// </summary>
    Dictionary<int, Queue<GameObject>> pools = new Dictionary<int, Queue<GameObject>>();

    public void CreatePool(GameObject prefab, int size)
    {
        if (size <= 0)
        {
            print("Pool size too small.");
            return;
        }
        if (prefab.GetComponent<PoolObject>() == null)
        {
            print("Pooled objects need to have PoolObject component attached.");
            return;
        }
        int key = prefab.GetInstanceID();

        if (!pools.ContainsKey(key))
        {
            pools.Add(key, new Queue<GameObject>());

            for (int i = 0; i < size; i++)
            {
                GameObject newObject = Instantiate(prefab);
                newObject.GetComponent<PoolObject>().PoolKey = key;
                newObject.SetActive(false);
                pools[key].Enqueue(newObject);
            }
        }
    }

    #region GET OBJECT FROM POOL
    public GameObject Get(GameObject prefab)
    {
        int key = prefab.GetInstanceID();
        if (!pools.ContainsKey(key))
        {
            print("No pool with " + prefab.name + "'s instance ID has been found.");
            return null;
        }

        GameObject obj = pools[key].Dequeue();
        obj.GetComponent<PoolObject>().PoolKey = key;
        obj.SetActive(true);
        return obj;

    }

    public GameObject Get(GameObject prefab, Vector3 position)
    {
        int key = prefab.GetInstanceID();
        if (!pools.ContainsKey(key))
        {
            print("No pool with " + prefab.name + "'s instance ID has been found.");
            return null;
        }

        GameObject obj = pools[key].Dequeue();
        obj.GetComponent<PoolObject>().PoolKey = key;
        obj.transform.position = position;
        obj.SetActive(true);
        return obj;
    }

    public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int key = prefab.GetInstanceID();
        if (!pools.ContainsKey(key))
        {
            print("No pool with " + prefab.name + "'s instance ID has been found.");
            return null;
        }

        GameObject obj = pools[key].Dequeue();
        obj.GetComponent<PoolObject>().PoolKey = key;
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);
        return obj;
    }
    #endregion

    #region RETURN OBJECT TO POOL
    public void Return()
    {
        TryGetComponent<PoolObject>(out PoolObject poolObject);
        if (poolObject == null)
        {
            print(name + "isn't part of any existing pool");
            Destroy(gameObject);
            return;
        }

        gameObject.SetActive(false);
        pools[poolObject.PoolKey].Enqueue(gameObject);
    }

    public void Return(GameObject objectToReturn)
    {
        objectToReturn.TryGetComponent<PoolObject>(out PoolObject poolObject);
        if (poolObject == null)
        {
            print(objectToReturn.name + "isn't part of any existing pool");
            Destroy(gameObject);
            return;
        }

        objectToReturn.SetActive(false);
        pools[poolObject.PoolKey].Enqueue(objectToReturn);
    }

    public void Return(PoolObject poolObject)
    {
        poolObject.gameObject.SetActive(false);
        pools[poolObject.PoolKey].Enqueue(poolObject.gameObject);
    }
    #endregion

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this; 
        }
        else if (Instance == this)
        {
            Destroy(gameObject); 
        }
    }
}
