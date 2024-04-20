using System.Collections;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private GameEventType currentState;
    public GameEventType CurrentState => currentState;

    private LevelManager levelManager;

    private void Awake()
    {
        SetFrameRate();
        Init();

        currentState = GameEventType.Ready;
    }

    public override void Init()
    {
        GameEventSystem.Instance.Init();
        PoolManager.Instance.Init();
        ParticleManager.Instance.Init();
        CameraManager.Instance.Init();
        SoundManager.Instance.Init();
        UIManager.Instance.Init();
        AdManager.Instance.Init();
        IAPManager.Instance.Init();
        BattleSystem.Instance.Init();

        levelManager = FindObjectOfType<LevelManager>();
    }

    private void Start()
    {
        ChangeGameState(GameEventType.Menu);
    }

    public void AttackTower(int damage)
	{
        levelManager.Attack(damage);
    }

	#region Game State

	public void ChangeGameState(GameEventType nextState)
	{
        if (currentState == nextState) return;
        currentState = nextState;
        GameEventSystem.Instance.PublishEvent(this, nextState);
	}

	#endregion

	#region Game Functions

	// 프레임 조정
	private void SetFrameRate()
    {
#if UNITY_ANDROID
        Application.targetFrameRate = 60;
#endif
    }

    // 게임 나가기
    public void FinishGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); 
#endif
    }

	private void OnApplicationQuit()
	{
        SaveLoadManager.Instance.SaveData();
	}

    // 이탈하였을 때는 false 반환,
    // 다시 돌아왔을 때는 true 반환
    private void OnApplicationFocus(bool focus)
    {
        isFocus = focus;
    }

#if UNITY_ANDROID

    // 이탈 중인지
    private bool isFocus = false;

    public void ShareInAnroid()
    {
        StartCoroutine(ShareTextInAnroid());
    }

    private IEnumerator ShareTextInAnroid()
    {
        string shareSubject = "대포를 발사하여 성을 무너뜨리세요!"; //Subject text
        string shareMessage = "https://play.google.com/store/apps/details?id=com.Team1243.HumanCannon"; //Your link

        if (!Application.isEditor)
        {
            //Create intent for action send
            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
            //put text and subject extra
            intentObject.Call<AndroidJavaObject>("setType", "text/plain");

            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), shareSubject);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), shareMessage);

            //call createChooser method of activity class
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "Share your high score");
            currentActivity.Call("startActivity", chooser);
        }

        // 디바이스에 공유하기 팝업을 띄우면 게임을 이탈한 걸로 판정 나기 때문에
        // 다시 돌아왔을 때 true가 되는 isFocus를 기다려준다.
        yield return new WaitUntil(() => isFocus); 
    }

#endif

    #endregion
}
