using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BunusEffectScript : MonoBehaviour {

	public GameObject BonusParticle;

	// Use this for initialization
	void Start () {
		StartCoroutine (Particle ());
	}

	IEnumerator Particle(){
		for (int i = 0; i < 45; i++) {
			float xRand = Random.Range (-4f, 4f);
			float yRand = Random.Range (-8f, 7f);
			Instantiate (BonusParticle, transform.position + new Vector3 (xRand, yRand, 1), transform.rotation);
			yield return new WaitForSeconds (0.1f);
		}
	}
}
