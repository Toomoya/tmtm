using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using NCMB;

public class ScoreSceneScript : MonoBehaviour {

	public GameObject scoreText;
	public GameObject rankText;
	public GameObject happyEffect;
	int score;
	int myRank;
	bool isRankPanel=false;
	public List<HighScore> highScoreList;
	public List<HighScore> myHighScoreList;

	public GameObject rankPanel;

	private int _num;
	public int Num {
		set {
			_num = value;
			scoreText.GetComponentInChildren<Text> ().text = _num.ToString ();
		}
		get {
			return _num;
		}
	}
	AudioSource audioSource;
	public AudioClip voice;

	// Use this for initialization
	void Start () {
		rankPanel.transform.localScale=new Vector3(0,0,0);
		score = PlayerPrefs.GetInt ("score");
		audioSource = GetComponent<AudioSource> ();
		StartCoroutine (OpenScore ());
		FetchMyRank ();
		FetchTopRankingData ();


	}

	IEnumerator OpenScore(){
		yield return new WaitForSeconds (0.7f);
		DOTween.To (() => Num, (n) => Num = n, score, 2.5f).SetEase (Ease.Linear);
		yield return new WaitForSeconds (2.6f);
		Instantiate (happyEffect, new Vector3 (0, 0, 0), transform.rotation);
		audioSource.clip = voice;
		audioSource.Play ();
	}
	

	public void ToStartScene(){
		FadeManager.Instance.LoadScene ("StartScene", 2.0f);
	}

	//	自分の順位
	public void FetchMyRank ()
	{
		// データスコアの「HighScore」から検索
		NCMBQuery<NCMBObject> rankQuery = new NCMBQuery<NCMBObject> ("OnlineRanking");
		rankQuery.WhereGreaterThan("HighScore", PlayerPrefs.GetInt("score"));
		rankQuery.CountAsync((int count , NCMBException e )=>{
			if(e != null){
				//件数取得失敗s
			}else{
				//件数取得成功
				myRank = count+1; // 自分よりスコアが上の人がn人いたら自分はn+1位
				rankText.GetComponentInChildren<Text>().text=myRank.ToString();
				FetchMyRankingData ();
			}
		});
	}

	void FetchMyRankingData ()
	{
		Debug.Log (myRank);
		// スキップする数を決める（ただし自分が1位か2位のときは調整する）
		int numSkip = myRank - 3;
		if(numSkip < 0) 
			numSkip = 0;

		// データストアの「HighScore」クラスから検索
		NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject> ("OnlineRanking");
		query.OrderByDescending ("HighScore");
		query.Skip  = numSkip;
		query.Limit = 5;
		query.FindAsync ((List<NCMBObject> objList ,NCMBException e) => {
			if (e != null) {
				//検索失敗時の処理
			} else {
				//検索成功時の処理
				List<HighScore> list = new List<HighScore>();
				// 取得したレコードをHighScoreクラスとして保存

				foreach (NCMBObject obj in objList) {
					int    s = System.Convert.ToInt32(obj["HighScore"]);
					string n = System.Convert.ToString(obj["UserName"]);
					list.Add( new HighScore( s, n ) );
				}
				myHighScoreList = list;
				for(int i=0;i<myHighScoreList.Count; i++){
					Debug.Log(myHighScoreList[i].name);
					Debug.Log(myHighScoreList[i].score.ToString());
				}
			}
		});
	}
	public void FetchTopRankingData ()
	{
		// データストアの「HighScore」クラスから検索
		NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject> ("OnlineRanking");
		query.OrderByDescending ("HighScore");
		query.Limit = 5;
		query.FindAsync ((List<NCMBObject> objList ,NCMBException e) => {
			if (e != null) {
				//検索失敗時の処理
			} else {
				//検索成功時の処理
				List<HighScore> list = new List<HighScore>();
				// 取得したレコードをHighScoreクラスとして保存
				foreach (NCMBObject obj in objList) {
					int    s = System.Convert.ToInt32(obj["HighScore"]);
					string n = System.Convert.ToString(obj["UserName"]);
					list.Add( new HighScore( s, n ) );
				}
				highScoreList = list;
				for(int i=0;i<highScoreList.Count; i++){
					GameObject.Find("TopRank" + (i+1)).GetComponent<Text>().text =highScoreList[i].name;
					GameObject.Find("TopRankScore" + (i+1)).GetComponent<Text>().text =highScoreList[i].score.ToString();
					Debug.Log(highScoreList[i].name);
					Debug.Log(highScoreList[i].score.ToString());
				}
			}
		});
	}

	public void OpenRank(){
		
		if (!isRankPanel) {
			rankPanel.transform.localScale=new Vector3(1,1,1);
			isRankPanel = true;
		} else {
			rankPanel.transform.localScale=new Vector3(0,0,0);
			isRankPanel = false;
		}

	}

}
