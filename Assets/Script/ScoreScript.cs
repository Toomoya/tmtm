using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using NCMB;

public class ScoreScript : MonoBehaviour {

	int score = 0;
	int preScore = 0;
	private int _num;
	public Slider speSlider;
	float sliderValue;
	int highScore;
	public Button specialButton;

	public int Num {
		set {
			_num = value;
			GetComponent<Text> ().text = _num.ToString ();
		}
		get {
			return _num;
		}
	}


	// Use this for initialization
	void Start () {
		//初期スコア(0点)を表示
		GetComponent<Text>().text = score.ToString();
		speSlider = speSlider.GetComponent<Slider> ();

	}

	//ballScriptからSendMessageで呼ばれるスコア加算用メソッド
	public void AddPoint(int point){
		score += point;
		preScore += point;
		DOTween.To (() => Num, (n) => Num = n, score, 1.5f).SetEase (Ease.Linear);
		speSlider.value = (float)preScore / 2000;

		if (speSlider.value == 1) {
			specialButton.interactable = true;
		}
	
	}

	public void SliderReset(){
		specialButton.interactable = false;
		speSlider.value = 0f;
		preScore = 0;
	}

	public void ScoreSave(){
		PlayerPrefs.SetInt ("score", score);
		if (PlayerPrefs.HasKey ("highScore")) {
			highScore = PlayerPrefs.GetInt ("highScore");
			if (score > highScore) {
				PlayerPrefs.SetInt ("highScore", score);
				SendHighScore (score);
			} else {
				SendHighScore (highScore);
			}

		} else {
			PlayerPrefs.SetInt ("highScore", score);
			//GameObject.FindObjectOfType<HighScoreManager>().SendHighScore(int.Parse(scoreInputField.text));
			SendHighScore (score);
		}
	}

	public void SendHighScore (int sendScore)
	{
		Debug.Log ("hoge");
		//		nirty上の表の名前をインスタンス時のかっこの中に入れる
		NCMBObject obj = new NCMBObject ("OnlineRanking");

		//		もともとObjectIdを持っていたらObjectIdをセットする
		if (PlayerPrefs.HasKey ("ObjectId")) {
			obj.ObjectId = PlayerPrefs.GetString ("ObjectId");
		}
		//		データカラムを追加する
		obj.Add ("UserName", PlayerPrefs.GetString ("name"));
		obj.Add ("HighScore", sendScore);
		obj.SaveAsync ((NCMBException e) => {      
			if (e != null) {
				//エラー処理
				Debug.Log ("score data failed");
			} else {
				//成功時の処理
				Debug.Log ("score data sent successfully");
				PlayerPrefs.SetString("ObjectId",obj.ObjectId);
			}                   
		});
	}

}
