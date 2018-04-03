using UnityEngine;

public interface IPoolableObject {

	void OnObjectSpawn();
	bool GetAlive();

}
