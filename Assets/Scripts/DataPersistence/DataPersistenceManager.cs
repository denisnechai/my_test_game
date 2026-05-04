using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]

    [SerializeField] private string fileName;

    private FileDataHandler datahandler;
    public static DataPersistenceManager Instance { get; private set; }

    private GameData gameData;

    private List<IDataPersistence> dataPersistenceObjects;
    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("Found more than one DataPersistenceManager in the scene");
            Destroy(gameObject);
        }
        else if(Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
    }
    public void NewGame()
    {
        this.gameData = new GameData();
    }
    public void SaveGame()
    {
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }
        datahandler.Save(gameData);
    }
    public void LoadGame()
    {
        this.gameData = datahandler.Load();
        if (this.gameData == null)
        {
            Debug.Log("No data was found. Initializing data to defaults.");
            NewGame();
        }
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }
    private void Start()
    {
        this.datahandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }
    private void OnApplicationQuit()
    {
        SaveGame();
    }
    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
