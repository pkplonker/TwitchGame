using Unity.Netcode;
using UnityEngine;

namespace Multiplayer
{
	/// <summary>
	///GenericUnitySingleton - Persistent singleton class for Unity.
	/// </summary>
	public class NetworkSingleton<T> : NetworkBehaviour where T : Component
	{
		private static T instance = null;

		public static T Instance
		{
			get
			{
				if (instance != null) return instance;
				instance = GameObject.FindObjectOfType<T>();
				if (instance != null) return instance;
				var singletonObj = new GameObject();
				singletonObj.name = typeof(T).ToString();
				instance = singletonObj.AddComponent<T>();
				return instance;
			}
		}

		protected virtual void Awake()
		{
			if (instance != null)
			{
				Destroy(gameObject);
				Debug.LogWarning("Deleted duplicate instance " + nameof(T) + " on gameobject" + gameObject.name);
				return;
			}

			instance = GetComponent<T>();
			DontDestroyOnLoad(gameObject);
		}
	}
}