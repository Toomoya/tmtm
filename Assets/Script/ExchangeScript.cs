using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExchangeScript : MonoBehaviour {
	public GameManagerScript GameManagerScript;

	public void Exchange () {
		//配列に「respawn」タグのついているオブジェクトを全て格納
		GameObject[] balls = GameObject.FindGameObjectsWithTag("Respawn");
		//全て取り出し、削除
		foreach (GameObject obs in balls) {
			Destroy(obs);
		}
		//ballScriptのDropBallメソッドを実行し、50のひよこを作成
		GameManagerScript.SendMessage("DropBall", 50);
	}

}
