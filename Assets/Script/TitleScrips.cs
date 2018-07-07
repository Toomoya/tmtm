using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScrips : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ToMenu(){
		FadeManager.Instance.LoadScene ("StartScene", 2.0f);
	}
}
