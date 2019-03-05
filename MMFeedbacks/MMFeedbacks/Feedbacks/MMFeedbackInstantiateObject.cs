using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// This feedback will instantiate the associated object (usually a VFX, but not necessarily), optionnally creating an object pool of them for performance
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackHelp("This feedback allows you to instantiate the object specified in its inspector, at the feedback's position (plus an optional offset). You can also optionally (and automatically) create an object pool at initialization to save on performance. In that case you'll need to specify a pool size (usually the maximum amount of these instantiated objects you plan on having in your scene at each given time).")]
    [FeedbackPath("GameObject/Instantiate Object")]
    public class MMFeedbackInstantiateObject : MMFeedback
    {
        [Header("Instantiate Object")]
        /// the vfx object to instantiate
        public GameObject VfxToInstantiate; 
        /// the position offset at which to instantiate the vfx object
        public Vector3 VfxPositionOffset;
        /// whether or not we should create automatically an object pool for this vfx
        public bool VfxCreateObjectPool;
        /// the initial and planned size of this object pool
        public int VfxObjectPoolSize = 5;

        protected MMMiniObjectPooler _objectPool; 
        protected GameObject _newGameObject;

        /// <summary>
        /// On init we create an object pool if needed
        /// </summary>
        /// <param name="owner"></param>
        protected override void CustomInitialization(GameObject owner)
        {
            base.CustomInitialization(owner);

            if (Active && VfxCreateObjectPool)
            {
                if (_objectPool != null)
                {
                    _objectPool.DestroyObjectPool();
                    Destroy(_objectPool.gameObject);
                }

                GameObject objectPoolGo = new GameObject();
                objectPoolGo.name = "FeedbackObjectPool";
                _objectPool = objectPoolGo.AddComponent<MMMiniObjectPooler>();
                _objectPool.GameObjectToPool = VfxToInstantiate;
                _objectPool.PoolSize = VfxObjectPoolSize;
                _objectPool.FillObjectPool();
            }
        }

        /// <summary>
        /// On Play we instantiate the specified object, either from the object pool or from scratch
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1.0f)
        {
            if (Active && (VfxToInstantiate != null))
            {
                if (_objectPool != null)
                {
                    _newGameObject = _objectPool.GetPooledGameObject();
                    if (_newGameObject != null)
                    {
                        _newGameObject.transform.position = position + VfxPositionOffset;
                        _newGameObject.SetActive(true);
                    }
                }
                else
                {
                    _newGameObject = GameObject.Instantiate(VfxToInstantiate) as GameObject;
                    _newGameObject.transform.position = position + VfxPositionOffset;
                }
            }
        }
    }
}
