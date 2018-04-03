using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour {

	[System.Serializable]
	public class Pool
	{
		public string tag;
		public GameObject prefab;
		public int size;

		public Pool(string _tag, GameObject _prefab, int _size) {
			tag = _tag;
			prefab = _prefab;
			size = _size;
		}

	}

	#region Singleton

	public static ObjectPooler instance;

	private void Awake() {
		instance = this;
		poolDictionary = new Dictionary<string, Queue<GameObject>> ();

		foreach (Pool pool in pools) {

			Queue<GameObject> objectPool = GetQueueFromPool (pool);

			poolDictionary.Add (pool.tag, objectPool);

		}
	}

	#endregion

	public List<Pool> pools;
	public Dictionary<string, Queue<GameObject>> poolDictionary;

	public void AddToPool(Pool poolToAdd) {
		
		if (!poolDictionary.ContainsKey (poolToAdd.tag)) {
			pools.Add (poolToAdd);
			Queue<GameObject> objectPool = GetQueueFromPool (poolToAdd);
			poolDictionary.Add (poolToAdd.tag, objectPool);
		}
	}

	private Queue<GameObject> GetQueueFromPool(Pool p) {
		Queue<GameObject> objectPool = new Queue<GameObject> ();

		for (int i = 0; i < p.size; i++) 
		{
			GameObject obj = Instantiate (p.prefab);
			obj.SetActive (false);
			objectPool.Enqueue (obj);
		}

		return objectPool;
	}

	public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation) {

		if (!poolDictionary.ContainsKey (tag)) {

			Debug.LogWarning ("Pool with tag " + tag + " doesn't exist!");
			return null;
		}

		IPoolableObject poolableObject = poolDictionary [tag].Peek ().GetComponent<IPoolableObject>();

		if (poolableObject.GetAlive ()) {
			return null;
		}

		GameObject objectToSpawn = poolDictionary [tag].Dequeue ();

		objectToSpawn.SetActive (true);
		objectToSpawn.transform.position = position;
		objectToSpawn.transform.rotation = rotation;

		//IPoolableObject poolableObject = objectToSpawn.GetComponent<IPoolableObject> ();

		if (poolableObject != null) {
			poolableObject.OnObjectSpawn ();
		}

		poolDictionary [tag].Enqueue (objectToSpawn);

		return objectToSpawn;

	}




}