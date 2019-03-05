using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    public class MMMiniObjectPooler : MonoBehaviour
    {
        /// the game object we'll instantiate 
        public GameObject GameObjectToPool;
        /// the number of objects we'll add to the pool
        public int PoolSize = 20;
        /// if true, the pool will automatically add objects to the itself if needed
        public bool PoolCanExpand = true;

        /// if this is true, the pool will try not to create a new waiting pool if it finds one with the same name.
        public bool MutualizeWaitingPools = false;
        /// if this is true, all waiting and active objects will be regrouped under an empty game object. Otherwise they'll just be at top level in the hierarchy
        public bool NestWaitingPool = true;

        /// this object is just used to group the pooled objects
        protected GameObject _waitingPool = null;
        protected MMMiniObjectPool _objectPool;
        protected List<GameObject> _pooledGameObjects;

        /// <summary>
        /// On awake we fill our object pool
        /// </summary>
        protected virtual void Awake()
        {
            FillObjectPool();
        }

        /// <summary>
        /// Creates the waiting pool or tries to reuse one if there's already one available
        /// </summary>
        protected virtual void CreateWaitingPool()
        {
            if (!NestWaitingPool)
            {
                return;
            }

            if (!MutualizeWaitingPools)
            {
                // we create a container that will hold all the instances we create
                _waitingPool = new GameObject(DetermineObjectPoolName());
                _objectPool = _waitingPool.AddComponent<MMMiniObjectPool>();
                _objectPool.PooledGameObjects = new List<GameObject>();
                return;
            }
            else
            {
                GameObject waitingPool = GameObject.Find(DetermineObjectPoolName());
                if (waitingPool != null)
                {
                    _waitingPool = waitingPool;
                    _objectPool = _waitingPool.MMFGetComponentNoAlloc<MMMiniObjectPool>();
                }
                else
                {
                    _waitingPool = new GameObject(DetermineObjectPoolName());
                    _objectPool = _waitingPool.AddComponent<MMMiniObjectPool>();
                    _objectPool.PooledGameObjects = new List<GameObject>();
                }
            }
        }

        /// <summary>
        /// Determines the name of the object pool.
        /// </summary>
        /// <returns>The object pool name.</returns>
        protected virtual string DetermineObjectPoolName()
        {
            return ("[MiniObjectPool] " + this.name);
        }

        /// <summary>
        /// Implement this method to fill the pool with objects
        /// </summary>
        public virtual void FillObjectPool()
        {
            if (GameObjectToPool == null)
            {
                return;
            }

            CreateWaitingPool();

            // we initialize the list we'll use to 
            _pooledGameObjects = new List<GameObject>();

            int objectsToSpawn = PoolSize;

            if (_objectPool != null)
            {
                objectsToSpawn -= _objectPool.PooledGameObjects.Count;
                _pooledGameObjects = new List<GameObject>(_objectPool.PooledGameObjects);
            }

            // we add to the pool the specified number of objects
            for (int i = 0; i < objectsToSpawn; i++)
            {
                AddOneObjectToThePool();
            }
        }

        /// <summary>
        /// Implement this method to return a gameobject
        /// </summary>
        /// <returns>The pooled game object.</returns>
        public virtual GameObject GetPooledGameObject()
        {
            // we go through the pool looking for an inactive object
            for (int i = 0; i < _pooledGameObjects.Count; i++)
            {
                if (!_pooledGameObjects[i].gameObject.activeInHierarchy)
                {
                    // if we find one, we return it
                    return _pooledGameObjects[i];
                }
            }
            // if we haven't found an inactive object (the pool is empty), and if we can extend it, we add one new object to the pool, and return it		
            if (PoolCanExpand)
            {
                return AddOneObjectToThePool();
            }
            // if the pool is empty and can't grow, we return nothing.
            return null;
        }

        /// <summary>
		/// Adds one object of the specified type (in the inspector) to the pool.
		/// </summary>
		/// <returns>The one object to the pool.</returns>
		protected virtual GameObject AddOneObjectToThePool()
        {
            if (GameObjectToPool == null)
            {
                Debug.LogWarning("The " + gameObject.name + " ObjectPooler doesn't have any GameObjectToPool defined.", gameObject);
                return null;
            }
            GameObject newGameObject = (GameObject)Instantiate(GameObjectToPool);
            newGameObject.gameObject.SetActive(false);
            if (NestWaitingPool)
            {
                newGameObject.transform.SetParent(_waitingPool.transform);
            }
            newGameObject.name = GameObjectToPool.name + "-" + _pooledGameObjects.Count;

            _pooledGameObjects.Add(newGameObject);

            _objectPool.PooledGameObjects.Add(newGameObject);

            return newGameObject;
        }

        /// <summary>
        /// Destroys the object pool
        /// </summary>
        public virtual void DestroyObjectPool()
        {
            if (_waitingPool != null)
            {
                Destroy(_waitingPool.gameObject);
            }
        }
    }


    public class MMMiniObjectPool : MonoBehaviour
    {
        [MMFReadOnly]
        public List<GameObject> PooledGameObjects;
    }
}