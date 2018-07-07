using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour {

	public GameObject ballPrefab; //Instantiateするball
	public Material[] ballMaterial; //ballの色を変える要素
	private GameObject firstBall; //最初にドラッグしたボール
	private GameObject lastBall; //最後にドラッグしたボール
	private string currentName; //名前判定用のstring変数
	//削除するボールのリスト
	List<GameObject> removableBallList = new List<GameObject>();
	float ballDeleteDistance = 1.6f; //ボールが消える最低の距離
	int lowestDestroyCount = 3; //ballを消す最低の個数

	float gameTimer = 60f; //ゲームの残り時間
	float maxGameTimer = 60f;//ゲームの制限時間

	bool isTimer = false; //ゲーム制限時間を管理

	public Text gameTimerText; //ゲームの制限時間を表示するためのテキスト
	public Text timeText; //スタート時のカウントダウンや制限時間終了後のタイムアップを表示するためのテキスト

	public GameObject scoreGUI; //スコアを表示
	int point = 100; //スコアのレート

	public GameObject exchangeButton;  //ball入れ替えボタン
	public GameObject roundButton; //タイマーのアナログ表示に関するもの①
	public Image imageRoundButton; //タイマーのアナログ表示に関するもの②(for GetComponent)
	float imageRoundTimer; //タイマーのアナログ表示に関するもの③
	public GameObject destroyEffect; //ボールが消える時にでるparticle
	public GameObject BonusEffect; //ボーナス時のparticle
	public GameObject panel; //一時停止時のパネル
	public GameObject homeButton; //ホームに戻るボタン
	public GameObject RetryButton; //ゲームをやり直すボタン
	public GameObject ReplayButton; //ゲームプレイに戻るボタン

	public GameObject bonusLight; //ボーナス時に光らせる

	bool isPlaying = false; //ボールに触ってプレイできるかどうかの判定
	bool isSpecialOk = true; //スペシャルポイントをとれるかどうかの判定
	bool isSceneChanegeOk = true; //シーン遷移のために一回だけ呼ばれるようにする判定基準
	bool isBonusOk = true; //ボーナスタイムに入るために一回だけ呼ばれるようにする判定

	AudioSource audioSource;
	public AudioClip  destroySound;

	void Start () 
	{
		audioSource = GetComponent<AudioSource> ();
//		ballの生成コルーチン
		StartCoroutine(DropBall(50));
//		最初のカウントのコルーチン
		StartCoroutine (StartTimer());
		imageRoundButton= roundButton.GetComponent<Image> ();

	}

	void Update () 
	{
		Controll ();
		Timer ();
	}

//	操作に関するメソッド
	void Controll(){
		if (isPlaying) {
			//画面をクリックし、firstBallがnullの時実行
			if (Input.GetMouseButtonDown (0) && firstBall == null) {
				OnDragStart ();
			} else if (Input.GetMouseButtonUp (0)) {
				//クリックを終えた時
				OnDragEnd ();
				//OnDragStartメソッド実行後
			} else if (firstBall != null) {
				OnDragging ();
			}
		}
	}
	//ボールの生成について
	IEnumerator DropBall(int count) 
	{	
//		引数で50が渡された場合
		if(count == 50) {
			StartCoroutine("RestrictPush");
		}
		for (int i = 0; i < count; i++) {
			//ballの生成
			Vector3 pos = new Vector3(Random.Range(-2.0f, 2.0f), 5f, 0f);
			GameObject ball = Instantiate(ballPrefab, pos, Quaternion.AngleAxis(Random.Range(-40, 40), Vector3.forward)) as GameObject;
			//ballの名前がBall1, Ball2,,,とランダムに変更していく
			int materialId = Random.Range(0, 5);
			ball.name = "Ball" + materialId;
			//ballの名前によって色を変更
			ball.GetComponent<Renderer>().material = ballMaterial[materialId]; 
			yield return new WaitForSeconds(0.05f);
		}
	}
	//ボタンの制限について
	IEnumerator RestrictPush () {
		exchangeButton.GetComponent<Button>().interactable = false;
		yield return new WaitForSeconds(5.0f);
		exchangeButton.GetComponent<Button>().interactable = true;
	}

	private void OnDragStart() 
	{
//		rayで取得してきたカメラでの位置情報をInspectorの位置情報に変換する
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit)){
			if (hit.collider != null) {
				GameObject hitObj = hit.collider.gameObject;
				//オブジェクトの名前を前方一致で判定
				string ballName = hitObj.name;
				if (ballName.StartsWith ("Ball")) {
					firstBall = hitObj;
					lastBall = hitObj;
					currentName = hitObj.name;
					//削除対象オブジェクトリストの初期化
					removableBallList = new List<GameObject> ();
					//削除対象のオブジェクトを格納
					PushToList (hitObj);
				}
			}
		}
	}

	private void OnDragging ()
	{
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit)) {
			if (hit.collider != null) {
				GameObject hitObj = hit.collider.gameObject;
				//同じ名前のブロックをクリック＆lastBallとは別オブジェクトである時
				if (hitObj.name == currentName && lastBall != hitObj) {
					//２つのオブジェクトの距離を取得
					float distance = Vector3.Distance (hitObj.transform.position, lastBall.transform.position);
//					ボール同士の距離がballDeleteDistanceより短かかったら
					if (distance < ballDeleteDistance) {
						//削除対象のオブジェクトを格納
						lastBall = hitObj;
						PushToList (hitObj);
					}
				}
			}
		}
	}

	private void OnDragEnd () 
	{
//		リストにある削除対象のオブジェクトの数をとってくる
		int remove_cnt = removableBallList.Count;
		if (remove_cnt >= lowestDestroyCount) {
			StartCoroutine (DestroyObject (remove_cnt));

			scoreGUI.SendMessage ("AddPoint", point * remove_cnt);

			//消えた分新たにボールを生成
			StartCoroutine (DropBall (remove_cnt));
		} else {
			//色の透明度を100%に戻す
			for (int i = 0; i < remove_cnt; i++) {
				ChangeColor (removableBallList [i], 1.0f);
			}
		}
			firstBall = null;
			lastBall = null;

	}

	IEnumerator DestroyObject (int count){
		for (int i = 0; i < count; i++) {
			Instantiate (destroyEffect, removableBallList [i].transform.position, removableBallList [i].transform.rotation);
			audioSource.clip = destroySound;
			audioSource.Play ();
			Destroy (removableBallList [i]);
			yield return new WaitForSeconds (0.1f);
		}
	}


//	選んでいるballをリストに追加する
	void PushToList (GameObject obj) 
	{
//		削除するボール用のリストにobj(OnClickなball)を追加する
		removableBallList.Add (obj);
		//色の透明度を50%に落とす
		ChangeColor(obj, 0.5f);
	}
	//materialの透明度を変更する
	void ChangeColor(GameObject obj, float transparency)
	{
//		Rendererを取得
		Renderer rend = obj.GetComponent<Renderer>();
//		透明度を変更
		rend.material.color = new Color (rend.material.color.r, rend.material.color.g, rend.material.color.b, transparency);
	}

//	最初のカウントダウンのタイマー
	IEnumerator StartTimer()
	{
		for(int i=5; i>0; i--)
		{
			timeText.text = i.ToString ();
			yield return new WaitForSeconds(1.0f);
		}
		timeText.text = "START!!";
		yield return new WaitForSeconds (0.7f);
		timeText.text = "";
		isTimer = true;
		isPlaying = true;

	}
		
//	制限時間のタイマー
	void Timer()
	{
		if (isTimer) 
		{
			if (gameTimer > 0) 
			{
				gameTimer -= Time.deltaTime;
				imageRoundTimer += Time.deltaTime;
				gameTimerText.text = gameTimer.ToString ("f0"); 
//				アナログのタイマーを変更する
				if (imageRoundTimer > 5f) {
					imageRoundButton.fillAmount = gameTimer / maxGameTimer;
					imageRoundTimer = 0;
				}

			} else { //タイムアップになったら
				gameTimerText.text = "0";
				timeText.GetComponent<Text> ().enabled = true;
				timeText.text = "Time Up!!";
				imageRoundButton.fillAmount = 0;
				isPlaying = false;
//				1回だけ呼ぶ
				if (isSceneChanegeOk) {
					scoreGUI.SendMessage ("ScoreSave");
					isSceneChanegeOk = false;
					StartCoroutine (ScoreFadeOut());
				}
			}
			//ボーナスタイム突入！！
			if (gameTimer < 20) {
				if (isBonusOk) {
					isBonusOk = false;
					point = 150;
					Instantiate (BonusEffect, new Vector3 (0, 0, 1), transform.rotation);
				}
			}
		}
	}

	IEnumerator ScoreFadeOut(){
		yield return new WaitForSeconds (1.5f);
		FadeManager.Instance.LoadScene ("ScoreScene", 2.0f);
	}

//	一時停止操作
	public void Stop(){
		panel.GetComponent<Image>().enabled = true;
		Time.timeScale = 0;
		isPlaying = false;
		homeButton.SetActive(true);
		RetryButton.SetActive(true);
		ReplayButton.SetActive(true);
	}
//	ホーム画面(StartScene)に戻る
	public void Home(){
		Time.timeScale = 1;
		FadeManager.Instance.LoadScene ("StartScene", 2.0f);
	}

//	もう一回ゲームをやり直す
	public void Retry(){
		Time.timeScale = 1;
		FadeManager.Instance.LoadScene ("Main", 2.0f);
	}

//	ゲームプレイ画面に戻る
	public void Replay(){
		panel.GetComponent<Image>().enabled = false;
		Time.timeScale = 1;
		isPlaying = true;
		homeButton.SetActive(false);
		RetryButton.SetActive(false);
		ReplayButton.SetActive(false);
	}

	public void Special(){
		if (isSpecialOk) {
			StartCoroutine (SpecialEffect ());
		}
	}

//	ボーナスボタンを押した時の操作
	IEnumerator SpecialEffect (){
		
		int ballRandomNumber = Random.Range (1, 6);
		bonusLight.SetActive (true);
		yield return new WaitForSeconds (0.4f);
		bonusLight.SetActive (false);

		for (int i = 1; i <= 5; i++) {
			GameObject g = GameObject.Find ("Ball"+ballRandomNumber);
			Instantiate (destroyEffect, g.transform.position, g.transform.rotation);
			audioSource.clip = destroySound;
			audioSource.Play ();
			Destroy (g);

			yield return new WaitForSeconds (0.1f);
		}

		isSpecialOk = false;
		//ポイントの追加
		scoreGUI.SendMessage ("AddPoint", point * 5);
		//ゲージのリセット
		scoreGUI.SendMessage ("SliderReset");
		//ボールの生成
		StartCoroutine(DropBall(5));
		isSpecialOk = true;
	}

}