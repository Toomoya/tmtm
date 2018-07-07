using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour {

	public static DataManager instance;

	//シングルトン
	void Awake(){
		if(instance == null){
			instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
	}
}
