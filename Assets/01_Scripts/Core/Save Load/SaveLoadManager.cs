using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int stage;
	public int coin;
	public int fireRateLevel;
	public int humanSpeedLevel;
	public bool isMuted;
}

public class SaveLoadManager : MonoSingleton<SaveLoadManager>
{
	private List<IDataObserver> observers;

	public SaveData data;
	private string path;
	private string fileName = "data.json";


	private void Awake()
	{
		path = Path.Combine(Application.persistentDataPath, "savefiles");
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}

		observers = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IDataObserver>().ToList();
		LoadData();
	}

	public void SaveData()
	{
		string filePath = Path.Combine(path, fileName);
		if (!File.Exists(filePath))
		{
			File.Create(filePath);
		}

		data = new SaveData();
		foreach (IDataObserver observer in observers)
		{
			observer.WriteData(ref data);
		}

		string jsonStr = JsonUtility.ToJson(data, true);
		byte[] jsonByte = System.Text.Encoding.UTF8.GetBytes(jsonStr);
		string jsonBase64 = System.Convert.ToBase64String(jsonByte);

		File.WriteAllText(filePath, jsonBase64);
		Debug.Log("Data Saved");
	}

	public void LoadData()
	{
		string filePath = Path.Combine(path, fileName);
		if (!File.Exists(filePath))
		{
			Debug.LogError(filePath + "Does not exists");
		}

		string jsonBase64 = File.ReadAllText(filePath);
		byte[] jsonByte = System.Convert.FromBase64String(jsonBase64);
		string jsonStr = System.Text.Encoding.UTF8.GetString(jsonByte);
		data = JsonUtility.FromJson<SaveData>(jsonStr);

		foreach (IDataObserver observer in observers)
		{
			observer.ReadData(data);
		}
		Debug.Log("Data Loaded");
	}
}
