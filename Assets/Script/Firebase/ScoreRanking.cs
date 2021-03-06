using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.UI;
public class ScoreRanking : MonoBehaviour {
	public DatabaseReference ScoreRankDB;
    private int mapNum = 0;
   	private GameObject rankList;
    public const int RankingCounts = 5;
   	private Score[] scoreList = new Score[RankingCounts];
    public Text firstPrizeUser, secondPrizeUser, thirdPrizeUser, forthPrizeUser, fifthPrizeUser;
    public Text firstPrizeScore, secondPrizeScore, thirdPrizeScore, forthPrizeScore, fifthPrizeScore;


  // Use this for initialization
    void Start () {
        // 参考 http://sleepnel.hatenablog.com/entry/2017/01/26/124500
        // https://firebase.google.com/docs/database/unity/start
        // 公式ドキュメントの方がわかりやすい
        // データベースURLを設定
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://geometria-161bd.firebaseio.com/");
        // DB参照をprivateフィールドにとっておく
        ScoreRankDB = FirebaseDatabase.DefaultInstance.GetReference("ScoreRanking");
        // for test
        // addNewUser("aaa","aaa","aaa");
        // insertScore(0, "aaa", "aaa", 5000);
        initialSet(ScoreRankDB, mapNum);
    }

    


    // 特定のマップのみ取得/更新
    private void initialSet(DatabaseReference DB, int mapNum){
        DB.Child(mapNum.ToString()).OrderByChild("point").LimitToLast(RankingCounts).ChildAdded += HandleChildAdded;
        DB.Child(mapNum.ToString()).OrderByChild("point").LimitToLast(RankingCounts).ChildChanged += HandleChildChanged;
        DB.Child(mapNum.ToString()).OrderByChild("point").LimitToLast(RankingCounts).ChildRemoved += HandleChildRemoved;
        DB.Child(mapNum.ToString()).OrderByChild("point").LimitToLast(RankingCounts).ChildMoved += HandleChildMoved;
    }

    void HandleChildAdded(object sender, ChildChangedEventArgs args) {
        if (args.DatabaseError != null) {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        Debug.Log("HandleChildAdded");
        getScoreRanking(args.Snapshot.Reference.Parent);
        // Do something with the data in args.Snapshot
    }

    void HandleChildChanged(object sender, ChildChangedEventArgs args) {
        if (args.DatabaseError != null) {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        Debug.Log("HandleChildChanged");
	    getScoreRanking(args.Snapshot.Reference.Parent);
        // Do something with the data in args.Snapshot
    }

    void HandleChildRemoved(object sender, ChildChangedEventArgs args) {
        if (args.DatabaseError != null) {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        Debug.Log("HandleChildRemoved");
        getScoreRanking(args.Snapshot.Reference.Parent);
         // Do something with the data in args.Snapshot
    }

    void HandleChildMoved(object sender, ChildChangedEventArgs args) {
        if (args.DatabaseError != null) {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        Debug.Log("HandleChildMoved");
        getScoreRanking(args.Snapshot.Reference.Parent);
        // Do something with the data in args.Snapshot
    }
    public void getScoreRanking(DatabaseReference DB){
        DB.OrderByChild("point").LimitToLast(RankingCounts).GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted) {
            // Handle the error...
            }
            else if (task.IsCompleted) {
                DataSnapshot snapshot = task.Result;
                // Do something with snapshot...
                IEnumerator<DataSnapshot> en = snapshot.Children.GetEnumerator();
                int count=1;
                while(en.MoveNext()){
                    DataSnapshot data = en.Current;
                    string userName = (string)data.Child("userName").GetValue(true);
                    string point = data.Child("point").GetValue(true).ToString();
                    Score score = new Score(userName, point);
                    scoreList[RankingCounts-count] = score;
                    count++;
                }
                this.firstPrizeUser.text = scoreList[0].userName;
                this.secondPrizeUser.text = scoreList[1].userName;
                this.thirdPrizeUser.text = scoreList[2].userName;
                this.forthPrizeUser.text = scoreList[3].userName;
                this.fifthPrizeUser.text = scoreList[4].userName;
                this.firstPrizeScore.text = scoreList[0].point;
                this.secondPrizeScore.text = scoreList[1].point;
                this.thirdPrizeScore.text = scoreList[2].point;
                this.forthPrizeScore.text = scoreList[3].point;
                this.fifthPrizeScore.text = scoreList[4].point;

            }
        });
    }
    private void insertScore(int mapNum, string userId, string userName, string point){
        Score score = new Score(userName, point);
        string json = JsonUtility.ToJson(score);

        ScoreRankDB.Child(mapNum.ToString()).Child(userId).SetRawJsonValueAsync(json);
    }
}
