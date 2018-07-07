using NCMB;
using System.Collections.Generic;
using UnityEngine;

//ハイスコアを管理するクラス(モデル)
[System.Serializable]
public class HighScore
{
	//プロパティを宣言
	public int score;
	public string name;

	// コンストラクタ -----------------------------------
	//『コンストラクタ』とはインスタンス化された瞬間に呼ばれるメソッド！(イニシャライザみたいなもんかな？)
	public HighScore(int _score, string _name)
	{
		score = _score;//プロパティscoreに引数_scoreの値を格納
		name  = _name;//プロパティnameに引数_nameの値を格納
	}
}
