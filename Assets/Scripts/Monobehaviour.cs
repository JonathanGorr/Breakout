using UnityEngine;

public abstract class MonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
	private static object _lock = new object();
	private static bool _isQuitting = false;

	private static T _instance;

	public static T Instance
	{
		get
		{
			lock (_lock)
			{
				if (_instance != null)
				{
					return _instance;
				}
				if (_isQuitting || !Application.isPlaying)
				{
					return null;
				}
				var inst = FindObjectOfType<T>();
				if (inst != null)
				{
					Instance = inst;
					return _instance;
				}

				Instance = new GameObject().AddComponent<T>();
				return _instance;
			}
		}

		private set
		{
			if (_instance != null || _isQuitting)
			{
				return;
			}
			_instance = value;
			_instance.transform.SetParent(null);
			_instance.transform.position = Vector3.zero;
			_instance.transform.rotation = Quaternion.identity;
			_instance.transform.localScale = Vector3.one;
			_instance.gameObject.name = typeof(T).Name;
			//DontDestroyOnLoad(_instance.gameObject);
		}
	}

	protected virtual void Awake()
	{
		_isQuitting = false;
		if (_instance == null)
		{
			Instance = this as T;
		}
		else if (_instance != this)
		{
			Destroy(gameObject);
		}
	}

	protected virtual void OnDestroy()
	{
		_isQuitting = true;
	}
}
