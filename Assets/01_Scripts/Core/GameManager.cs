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

	// ������ ����
	private void SetFrameRate()
    {
#if UNITY_ANDROID
        Application.targetFrameRate = 60;
#endif
    }

    // ���� ������
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

    // ��Ż�Ͽ��� ���� false ��ȯ,
    // �ٽ� ���ƿ��� ���� true ��ȯ
    private void OnApplicationFocus(bool focus)
    {
        isFocus = focus;
    }

#if UNITY_ANDROID

    // ��Ż ������
    private bool isFocus = false;

    public void ShareInAnroid()
    {
        StartCoroutine(ShareTextInAnroid());
    }

    private IEnumerator ShareTextInAnroid()
    {
        string shareSubject = "������ �߻��Ͽ� ���� ���ʶ߸�����!"; //Subject text
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

        // ����̽��� �����ϱ� �˾��� ���� ������ ��Ż�� �ɷ� ���� ���� ������
        // �ٽ� ���ƿ��� �� true�� �Ǵ� isFocus�� ��ٷ��ش�.
        yield return new WaitUntil(() => isFocus); 
    }

#endif

    #endregion
}
