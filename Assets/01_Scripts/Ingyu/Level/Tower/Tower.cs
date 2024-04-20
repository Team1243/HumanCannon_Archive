using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tower : LevelObject
{
	[Header("References")]
	[SerializeField] private TextMeshPro healthTxt;
	[SerializeField] private ParticleSystem explosionParticle;
	[SerializeField] private AudioClip clearAudioClip;

	private List<MeshRenderer> visuals = new();
	private MeshRenderer currentMesh;

	private MaterialPropertyBlock mpb;
	private readonly int fillHash = Shader.PropertyToID("_Fill_Height");
	private readonly int fillColorHash = Shader.PropertyToID("_Fill_Color");
	private readonly int defaultColorHash = Shader.PropertyToID("_Default_Color");

	private Animator animator;
	private Collider col;
	private readonly int damageHash = Animator.StringToHash("damage");
	private readonly int spawnHash = Animator.StringToHash("spawn");
	private readonly int deathHash = Animator.StringToHash("death");
	private readonly int coinHash = Animator.StringToHash("coin");

	[Header("Values")]
	private int maxHealth;
    private int health;
	private int coinGain;
	private int coinGainColorIndex = 0;
	private bool isDead = false;

	private Coroutine textAnimationCo = null;
	[field:SerializeField]
	public override bool AnimationEnd { get; set; }

	private void Awake()
	{
		animator = GetComponent<Animator>();
		col = GetComponent<Collider>();
		mpb = new MaterialPropertyBlock();

		MeshRenderer[] rends = transform.Find("Visual").GetComponentsInChildren<MeshRenderer>(true);
		foreach (MeshRenderer elem in rends)
		{
			visuals.Add(elem);
		}
		currentMesh = rends[0];
	}

	private void Start()
	{
		SetFillMaterialColor(manager.levelData.towerDefaultColor, manager.levelData.towerFillColors[0]);
	}

	#region Visual Settings

	public void SetVisual(string visual, bool random = false)
	{
		currentMesh.gameObject.SetActive(false);
		MeshRenderer newMesh = null;

		if (random)
		{
			if (visuals.Count == 1) { }
			else
			{
				do
				{
					int ranNum = Random.Range(0, visuals.Count);
					newMesh = visuals[ranNum];
				} while (newMesh == currentMesh);
			}
		}
		else
		{
			newMesh = visuals.Find(x => x.gameObject.name == visual);
		}
		if (newMesh != null)
			currentMesh = newMesh;
		currentMesh.gameObject.SetActive(true);
	}

	private void SetFillMaterialColor(Color defaultColor, Color fillColor)
	{
		mpb.SetColor(defaultColorHash, defaultColor);
		mpb.SetColor(fillColorHash, fillColor);
		currentMesh.SetPropertyBlock(mpb);
	}

	private void SetFillAmount(float amount)
	{
		mpb.SetFloat(fillHash, amount);
		currentMesh.SetPropertyBlock(mpb);
	}

	#endregion

	#region Running Methods

	/// <summary>
	/// Calculate health by stage number
	/// </summary>
	/// <param name="n">stage number</param>
	/// <returns>calculated health</returns>
	private int HealthCalculator(int n) => Mathf.Clamp((int)(n * 1.2f + 9), 10, 100);

	/// <summary>
	/// Setting Member values
	/// </summary>
	/// <param name="health">체력</param>
	public void SetHealth(int health)
	{
		maxHealth = health;
		this.health = health;
	}

	public override void CreateLevel(int difficult)
	{
		isDead = false;
		col.enabled = true;
		coinGainColorIndex = 0;

		SetHealth(HealthCalculator(difficult));
		healthTxt.text = health.ToString();
		SetFillMaterialColor(manager.levelData.towerDefaultColor, manager.levelData.towerFillColors[0]);
		SetFillAmount(0.0f);
		SetVisual("", true);

		animator.SetTrigger(spawnHash);
	}

	public void Damage(int damage)
	{
		if (!isDead)
		{
			health -= damage; // 체력 감소
			health = Mathf.Clamp(health, 0, health);
			animator.SetTrigger(damageHash); // 데미지 애니메이션 실행
			healthTxt.text = health.ToString(); // 체력 텍스트 변경
			SetFillAmount((maxHealth - health) / (float)maxHealth); // 메테리얼 설정

			if (health == 0)
			{
				GameManager.Instance.ChangeGameState(GameEventType.Clear); // 체력이 0이 되면 클리어 상태로 변경한다. 클리어에 관한 모션은 추후 추가될 예정
			}
		}
		else
		{
			// print($"Coin : {coinGain}");
			// print($"Damage : {damage}");
			if (coinGain / manager.levelData.coinFillColorMax < (coinGain + damage) / manager.levelData.coinFillColorMax)
			{
				Color[] colors = manager.levelData.towerFillColors;
				coinGainColorIndex = (coinGainColorIndex + 1) % colors.Length;
				SetFillMaterialColor(colors[coinGainColorIndex], colors[(coinGainColorIndex + 1) % colors.Length]);
				SoundManager.Instance.Play(clearAudioClip);
			}
			coinGain += damage;
			SetFillAmount((coinGain % manager.levelData.coinFillColorMax) / (float)manager.levelData.coinFillColorMax);
			animator.SetTrigger(coinHash); // 코인 획득 애니메이션 실행
		}
	}

	#endregion

	#region Animations

	private IEnumerator HealthTextAnimationCo()
	{
		float time = 0;
		while (time <= manager.levelData.appearAnimTime)
		{
			healthTxt.text = ((int)Mathf.Lerp(0, health, time / manager.levelData.appearAnimTime)).ToString();
			time += Time.deltaTime;
			yield return null;
		}
		healthTxt.text = health.ToString();
		AnimationEnd = true;
	}

	public override void Appear()
	{
		healthTxt.gameObject.SetActive(true);
		AnimationEnd = false;
		if (textAnimationCo != null)
			StopCoroutine(textAnimationCo);
		textAnimationCo = StartCoroutine(HealthTextAnimationCo());
	}

	public override void Disappear()
	{
		col.enabled = false;
		AnimationEnd = false;
		animator.ResetTrigger(damageHash);
		animator.SetTrigger(deathHash);
		print("coin get = " + coinGain);
		UserDataManager.Instance.AddCoin(coinGain);
	}

	public void AnimationEndTrigger()
	{
		explosionParticle.Play();
		StopAllCoroutines();
		StartCoroutine(EffectDelayCo());
	}

	private IEnumerator EffectDelayCo()
	{
		yield return new WaitForSeconds(manager.levelData.disappearAnimTime);

		AnimationEnd = true;
	}

	public override void Clear()
	{
		coinGain = 0;
		isDead = true;
		animator.ResetTrigger(damageHash);
		animator.SetTrigger(coinHash);
		healthTxt.text = "0";
		healthTxt.gameObject.SetActive(false);
		Color[] colors = manager.levelData.towerFillColors;
		SetFillMaterialColor(colors[coinGainColorIndex], colors[(coinGainColorIndex + 1) % colors.Length]);
		SetFillAmount(0f);
	}

	public override void Over()
	{
		//SetFillMaterialColor(manager.levelData.towerDefaultColor, manager.levelData.towerFillColors[0]);

		isDead = false;
		coinGainColorIndex = 0;
		animator.ResetTrigger(damageHash);

		health = maxHealth;
		healthTxt.text = "0";
		healthTxt.gameObject.SetActive(false);
		SetFillAmount(0f);
	}

	#endregion
}
