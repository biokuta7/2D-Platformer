using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FireMode {
	AUTO,
	SEMIAUTO
}

public enum AmmoMode {

	UNLIMITED,
	LIMITED,
	RECHARGE

}

[CreateAssetMenu(fileName = "New Gun", menuName = "Items/Gun")]
public class Gun : ScriptableObject {

	[System.Serializable]
	public class GunLevel
	{
		public GameObject bullet;
		public int bulletCount;
		public float fireDelay = .25f;
		public float XPToNextLevel = 10.0f;
	}

	public new string name;
    public Sprite gunSpriteRight;
	public Sprite gunSpriteUp;
	public Vector3 handleOffset;
	public Vector3 muzzlePosition;
	public FireMode fireMode = FireMode.SEMIAUTO;
	public AmmoMode ammoMode = AmmoMode.UNLIMITED;

	private GunLevel currentGunLevel;

	public int maxAmmo;

	public List<GunLevel> gunLevels;

	public GunLevel GetCurrentGunLevel() {
		return currentGunLevel;
	}

	public virtual void Init() {

		for (int level = 1; level <= 3; level++) {

			currentGunLevel = gunLevels [level - 1];
			ObjectPooler.Pool pool = new ObjectPooler.Pool (GetGunTag (level), currentGunLevel.bullet, currentGunLevel.bulletCount);
			ObjectPooler.instance.AddToPool (pool);
		}

	}

	public string GetGunTag(int level) {
		return name + "_Ammo_" + level;
	}

	public float GetMaxXP() {
		float ret = 0;

		foreach (GunLevel gl in gunLevels) {
			ret += gl.XPToNextLevel;
		}

		return ret;

	}

	public int GetLevel(float XP) {
		float exp = XP;

		int ret = 0;

		while (exp >= 0 && ret <= 2) {
			
			ret++;
			GunLevel gl = gunLevels [ret - 1];
			exp -= gl.XPToNextLevel;
		}

		return Mathf.Clamp(ret, 1, 3);

	}

	public float GetXPPercent(int level, float XP) {

		if (level == 1) {
			return XP / gunLevels [0].XPToNextLevel;
		}

		float minXP = 0;

		for (int i = 0; i < (level-1); i++) {
			minXP += (gunLevels [i].XPToNextLevel);
		}

		float maxXP = minXP + gunLevels [level-1].XPToNextLevel;



		return Mathf.InverseLerp(minXP, maxXP, XP);

	}

	public virtual void Shoot(Vector3 pos, Vector3 direction, GameObject g) {

		Projectile p = g.GetComponent<Projectile> ();
		if (p != null) {
			p.Spawn (pos, direction);
		}
	}

}
