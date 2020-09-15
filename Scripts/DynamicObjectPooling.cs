using System.Collections.Generic;
using UnityEngine;

public class DynamicObjectPooling : MonoBehaviour
{
    //***********************//
    // This component creates a pool for GameObjects and manages their reusing.
    // Get() method used for getting the reference for the first GameObject in queue.
    // ReturnToPool() method returns the GameObject back to queue and deactivates it.
    //***********************//

    [SerializeField] private GameObject objectToPool;
    [SerializeField] private int count = 10;
    private Queue<GameObject> objectPool = new Queue<GameObject>();
    public static DynamicObjectPooling Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        CreatePool(count);
    }

    /// <summary>
    /// Creates GameObject pool.
    /// </summary>
    /// <param name="count">Size of pool to create.</param>
    private void CreatePool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject newObject = Instantiate(objectToPool, transform);
            newObject.gameObject.SetActive(false);
            objectPool.Enqueue(newObject);
        }
    }

    /// <summary>
    /// Return the GameObject to queue and deactivates it.
    /// </summary>
    /// <param name="instance">GameObject to return.</param>
    public void ReturnToPool(GameObject instance)
    {
        instance.gameObject.SetActive(false);
        objectPool.Enqueue(instance);
    }

    /// <summary>
    /// Returns first GameObject in queue and dequeues it. If the pool is empty, creates additional GameObject.
    /// </summary>
    /// <returns></returns>
    public GameObject Get()
    {
        if (objectPool.Count == 0)
        {
            CreatePool(1);
        }

        return objectPool.Dequeue();
    }
}
