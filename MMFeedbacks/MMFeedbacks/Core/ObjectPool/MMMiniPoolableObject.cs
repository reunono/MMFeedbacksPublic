using UnityEngine;
using System.Collections;
using MoreMountains.Feedbacks;
using System;

namespace MoreMountains.Feedbacks
{	
	/// <summary>
	/// Add this class to an object that you expect to pool from an objectPooler. 
	/// Note that these objects can't be destroyed by calling Destroy(), they'll just be set inactive (that's the whole point).
	/// </summary>
	public class MMMiniPoolableObject : MonoBehaviour 
	{
		public delegate void Events();
		public event Events OnSpawnComplete;

		/// The life time, in seconds, of the object. If set to 0 it'll live forever, if set to any positive value it'll be set inactive after that time.
		public float LifeTime = 0f;

		/// <summary>
		/// Turns the instance inactive, in order to eventually reuse it.
		/// </summary>
		public virtual void Destroy()
		{
			gameObject.SetActive(false);
		}

		/// <summary>
		/// When the objects get enabled (usually after having been pooled from an ObjectPooler, we initiate its death countdown.
		/// </summary>
	    protected virtual void OnEnable()
		{
			if (LifeTime>0)
			{
				Invoke("Destroy", LifeTime);	
			}
		}

		/// <summary>
		/// When the object gets disabled (maybe it got out of bounds), we cancel its programmed death
		/// </summary>
	    protected virtual void OnDisable()
		{
			CancelInvoke();
		}

		/// <summary>
		/// Triggers the on spawn complete event
		/// </summary>
		public void TriggerOnSpawnComplete()
		{
			if(OnSpawnComplete != null)
			{
				OnSpawnComplete();
			}
		}
	}
}
