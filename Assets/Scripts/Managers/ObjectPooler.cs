using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static event Action OnSetupFinished;

    public static ObjectPooler Instance;
    public List<Pool> poolsList;
    public List<GetterPool> getterPoolsList;
    private Dictionary<PoolObjName, Queue<GameObject>> poolDictionary;
    private Dictionary<PoolObjName, Queue<GetterInformation>> getterPoolDictionary;

    /// <summary>
    /// Pools of objects that will be instantiated. Sets the key, prefabs and max size. Can be customized.
    /// </summary>
    [System.Serializable]
    public class Pool
    {
        public PoolObjName name;
        public GameObject prefab;
        public int size;
    }

    [System.Serializable]
    public class GetterPool
    {
        public PoolObjName name;
        public IPoolGetComponent componentGetter;
        public GameObject prefab;
        public int size;
    }

    /// <summary>
    /// In case of not wanting it to be a singleton and be a component, take Instance out.
    /// </summary>
    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
        }

        Instance = this;
        /// Creates the dictionary and populate it following pool class information.
        CreatePoolsDictionaries();
    }

    /// <summary>
    /// Creates the dictionary for pools available
    /// </summary>
    private void CreatePoolsDictionaries()
    {
        poolDictionary = new();
        getterPoolDictionary = new();
        IPoolGetComponent.OnDisabled += SetObjectToTopOfList;

        foreach (var pool in poolsList)
        {
            Queue<GameObject> objectPool = new();

            // Instantiate prefab, add it to the queue and give it a name for debug.
            for (int i = 0; i < pool.size; i++)
            {
                GameObject prefab = Instantiate(pool.prefab);
                prefab.SetActive(false);
                objectPool.Enqueue(prefab);
                prefab.name = pool.name.ToString() + $" #{i}";
            }

            // Populate the dictionary with the created queue.
            poolDictionary.Add(pool.name, objectPool);
        }

        foreach(var poolGetter in getterPoolsList)
        {
            if (poolDictionary.ContainsKey(poolGetter.name))
            {
                Debug.LogError("Objects can only be in one pool! Same object in getter pool and normal pool");
                continue;
            }

            Queue<GetterInformation> getterPool = new();

            for(int i = 0; i < poolGetter.size; i++)
            {
                GameObject prefab = Instantiate(poolGetter.prefab);
                prefab.SetActive(false);
                IPoolGetComponent getter = prefab.GetComponent<IPoolGetComponent>();

                if (getter == null)
                {
                    Debug.LogError($"Pool getter not found in gameobject in pool {poolGetter.name}");
                    break;
                }

                GetterInformation information = new(getter, prefab);
                getterPool.Enqueue(information);
                prefab.name = poolGetter.name.ToString() + $" #{i}";
            }

            getterPoolDictionary.Add(poolGetter.name, getterPool);
        }
    }

    /// <summary>
    /// Grab an existent pool object and spawn it at the given position.
    /// </summary>
    /// <param name="_name">Pool key, name of pool trying to be accessed</param>
    /// <param name="_pos">Position to be spawned at</param>
    /// <param name="_rotation">Rotation for the object when spawned</param>
    /// <returns>Object being spawned</returns>
    public GameObject SpawnFromPool(PoolObjName _name, Vector3 _pos, Quaternion _rotation)
    {
        if (!poolDictionary.ContainsKey(_name))
        {
            Debug.LogWarning($"Object requested not in selected pool (Gameobject Pool), obj name: {_name}");
            return null; 
        }

        GameObject obj = poolDictionary[_name].Dequeue();

        obj.SetActive(true);
        obj.transform.position = _pos;
        obj.transform.rotation = _rotation;

        // Add it back to the queue to be able to spawn it back again in the future.
        poolDictionary[_name].Enqueue(obj);
        return obj;
    }

    /// <summary>
    /// Grab an existent pool object and spawn it at the given position.
    /// </summary>
    /// <param name="_name">Pool key, name of pool trying to be accessed</param>
    /// <param name="_pos">Position to be spawned at</param>
    /// <param name="_rotation">Rotation for the object when spawned</param>
    /// <returns>Object being spawned</returns>
    public IPoolGetComponent SpawnFromGetterPool(PoolObjName _name, Vector3 _pos, Quaternion _rotation)
    {
        if (!getterPoolDictionary.ContainsKey(_name))
        {
            Debug.LogWarning($"Object requested not in selected pool (Getter Pool), obj name: {_name}");
            return null;
        }

        GetterInformation getterInformation = getterPoolDictionary[_name].Dequeue();
        GameObject gameObject = getterInformation.GameObject;

        gameObject.SetActive(true);
        gameObject.transform.position = _pos;
        gameObject.transform.rotation = _rotation;

        // Add it back to the queue to be able to spawn it back again in the future.
        getterPoolDictionary[_name].Enqueue(getterInformation);
        UpdateGetterPoolItemsIndexes(_name);

        return getterInformation.Getter;
    }

    /// <summary>
    /// Updates index values in queue items
    /// </summary>
    /// <param name="_name"></param>
    private void UpdateGetterPoolItemsIndexes(PoolObjName _name)
    {
        int index = 0;
        foreach (var getterInfo in getterPoolDictionary[_name])
        {
            getterInfo.Getter.Index = index;
            index++;
        }
    }

    /// <summary>
    /// Sets object to top of the queue
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_object"></param>
    private void SetObjectToTopOfList(PoolObjName _name, IPoolGetComponent _object)
    {
        if (getterPoolDictionary.ContainsKey(_name) && _object.Index > 0)
        {
            Queue<GetterInformation> queue = getterPoolDictionary[_name];
            GetterInformation currentInfo;

            for (int i = 0; i < _object.Index; i++)
            {
                currentInfo = queue.Dequeue();
                queue.Enqueue(currentInfo);
            }
            UpdateGetterPoolItemsIndexes(_name);
        }
    }

    /// <summary>
    /// Despawns all objects included in this pool dictionary. NOTE: They are still available for spawning again.
    /// </summary>
    public void DespawnAll()
    {
        foreach (var pool in poolDictionary)
        {
            for (int i = 0; i < pool.Value.Count; i++)
            {
                GameObject pieceToDespawn = pool.Value.Dequeue();
                pieceToDespawn.SetActive(false);
                pool.Value.Enqueue(pieceToDespawn);
            }
        }

        foreach (var getter in getterPoolDictionary)
        {
            for(int i = 0; i < getter.Value.Count; i++)
            {
                GetterInformation information = getter.Value.Dequeue();
                information.GameObject.SetActive(false);
                getter.Value.Enqueue(information);
            }
        }
    }

    /// <summary>
    /// Despawns all objects from a specific pool included in this pool dictionary. NOTE: They are still available for spawning again.
    /// </summary>
    /// <param name="_despawnPool">Pool key, pool to be despawned</param>
    public void DespawnAllInPool(PoolObjName _despawnPool)
    {
        if (poolDictionary.ContainsKey(_despawnPool))
        {
            foreach (var gameObject in poolDictionary[_despawnPool])
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning($"Pool requested is not part of pool dictionary, pool name: {_despawnPool}");
        }

    }

    /// <summary>
    /// Despawns all objects in given getter pool
    /// </summary>
    /// <param name="_despawnPool"></param>
    public void DespawnAllInGetterPool(PoolObjName _despawnPool)
    {
        foreach (var getter in getterPoolDictionary[_despawnPool])
        {
            getter.GameObject.SetActive(false);
        }
    }

    /// <summary>
    /// List of objects available to the object pool.
    /// </summary>
    [System.Serializable]
    public enum PoolObjName
    {
        TestCube,
        Windball,
        ShootingObject,
        POOL_TOTAL_SIZE,
    }

    public interface IPoolGetComponent
    {
        public static event Action<PoolObjName, IPoolGetComponent> OnDisabled;
        public int Index { get; set; }
        public virtual bool TryGetComponent<T>(out T _component)
        {
            if(this is T)
            {
                _component = (T)this;
                return true;
            }
            else
            {
                _component = default(T);
                return false;
            }
        }
        public static void SetToTopOfList(PoolObjName _name, IPoolGetComponent _getter)
        {
            OnDisabled?.Invoke(_name, _getter);
        }
    }

    private struct GetterInformation
    {
        public IPoolGetComponent Getter;
        public GameObject GameObject;

        public GetterInformation(IPoolGetComponent _getter, GameObject _gameObject)
        {
            Getter = _getter;
            GameObject = _gameObject;
        }
    }
}