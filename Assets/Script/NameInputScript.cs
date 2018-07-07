using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NameInputScript : MonoBehaviour {


	public void NameSubmitButton(){
		InputField nameImputField = GameObject.Find ("NameInput").GetComponent<InputField> ();
		PlayerPrefs.SetString ("name", nameImputField.text);
		FadeManager.Instance.LoadScene ("StartScene", 2.0f);
	}
		
}
