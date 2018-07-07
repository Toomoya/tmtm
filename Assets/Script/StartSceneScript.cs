using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartSceneScript : MonoBehaviour {

	public Text helloText;
	string playerName;

	void Start(){
		if (!PlayerPrefs.HasKey ("name")) {
			FadeManager.Instance.LoadScene ("NameInput", 2.0f);
		}else{
			playerName = PlayerPrefs.GetString ("name");
			helloText.text= ("HELLO " + playerName);
		}
	}
	public void ToRename(){
		FadeManager.Instance.LoadScene ("NameInput", 2.0f);
	}

	public void ToGameScene(){
		FadeManager.Instance.LoadScene ("Main", 2.0f);
	}
}
