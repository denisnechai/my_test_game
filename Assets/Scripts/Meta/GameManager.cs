using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

public class GameManager : MonoBehaviour, IDataPersistence
{
    [SerializeField]

    private static float GridChange = 1;
    private float timer;
    private float timerLimit = 2.0f;
    public static List<GameState> savedStates = new List<GameState>();
    public static bool isMoving;
    public static bool noInput;
    public static bool dontSave;
    private bool first = true;
    private bool exit = false;
    private static bool turnEnded = false;
    private static bool UndoAfter = false;
    private static GameObject[] players;
    private static GameObject[] boxes;
    private static GameObject[] rigidboxes;
    private static GameObject[] rigidplayers;
    private static List<GameObject> objects = new();
    private List<List<List<string>>> enterplayers = new();
    private List<List<List<string>>> exitplayers = new();
    private List<string> transitions = new();
    private static List<LevelScript> levels;
    private List<string> scenes = new();
    private static List<int> indexes = new();
    private List<string> exitscenes = new();
    private List<string> enterboxes = new();
    private List<string> exitboxes = new();
    public static GameManager instance;
    public LayerMask Goal_;
    public LayerMask Player_goal_;
    public static LayerMask Goal;
    public static LayerMask Player_goal;
    public LayerMask blockingLayer_;
    public static LayerMask blockingLayer;
    public static bool won = false;
    public static bool UndidWon = false;
    public static bool dontDestroyOnLoad = false;
    public static Dictionary<string, string> parentscenes = new();
    public static Dictionary<string, bool> LevelsWon;
    public SerializableDictionary<string, bool> LevelsCompleted;

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.Log("Found more than one GameManager in the scene");
            Destroy(gameObject);
        }
        else if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
    }
    void Start()
    {
        if (LevelsCompleted != null && LevelsCompleted.Count != 0)
        {
            LevelsWon = new SerializableDictionary<string, bool>();
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                {
                    if (LevelsCompleted[Path.GetFileNameWithoutExtension(scene.path)])
                    {
                        LevelsWon.Add(Path.GetFileNameWithoutExtension(scene.path), true);
                    }
                    else
                    {
                        LevelsWon.Add(Path.GetFileNameWithoutExtension(scene.path), false);
                    }
                }
                Debug.Log(Path.GetFileNameWithoutExtension(scene.path));
            }
            foreach (KeyValuePair<string, bool> level in LevelsCompleted)
            {
                Debug.Log(level.Key + " " + level.Value);
            }
            foreach (KeyValuePair<string, bool> level in LevelsWon)
            {
                Debug.Log(level.Key + " " + level.Value);
            }
        }
        else
        {
            LevelsWon = new SerializableDictionary<string, bool>();
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                {
                    LevelsWon.Add(Path.GetFileNameWithoutExtension(scene.path), false);
                }
                Debug.Log(Path.GetFileNameWithoutExtension(scene.path));
            }
            LevelsCompleted = (SerializableDictionary<string, bool>)LevelsWon;
            foreach (KeyValuePair<string, bool> level in LevelsCompleted)
            {
                Debug.Log(level.Key + " " + level.Value);
            }
        }
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                if(Path.GetFileNameWithoutExtension(scene.path).Contains("Level 1-"))
                {
                    parentscenes.Add(Path.GetFileNameWithoutExtension(scene.path), "World 1");
                }
                else if (Path.GetFileNameWithoutExtension(scene.path).Contains("Level 2-"))
                {
                    parentscenes.Add(Path.GetFileNameWithoutExtension(scene.path), "World 2");
                }
                else if (Path.GetFileNameWithoutExtension(scene.path).Contains("Level 3-"))
                {
                    parentscenes.Add(Path.GetFileNameWithoutExtension(scene.path), "World 3");
                }
                else if (Path.GetFileNameWithoutExtension(scene.path).Contains("World ") && Path.GetFileNameWithoutExtension(scene.path) != "World 0")
                {
                    parentscenes.Add(Path.GetFileNameWithoutExtension(scene.path), "World 0");
                }
                else if (Path.GetFileNameWithoutExtension(scene.path) == "World 0")
                {
                    parentscenes.Add(Path.GetFileNameWithoutExtension(scene.path), "Main hub");
                }
            }
        }
        Goal = Goal_;
        Player_goal = Player_goal_;
        blockingLayer = blockingLayer_;
    }

    

    void Update()
    {
        timer += Time.deltaTime;
        if (Input.GetKey(KeyCode.U) && timerLimit >= 0.1f)
        {
            timerLimit -= 0.00025f;
        }
        else if (!Input.GetKey(KeyCode.U))
        {
            timerLimit = 0.2f;
        }
        if (first)
        {
            Debug.Log("First frame");
            if (UndoAfter)
            {
                UndoAfter = false;
                UndoMove();
                ColorCheck();
            }
            else
            {
                SaveGameState();
            }
            ColorCheck();
            first = false;
        }
        if (exit)
        {
            isMoving = true;
            Debug.Log("Exiting");
            var offset = new Vector3();
            var offset1 = new Vector3();
            var offset2 = new Vector3();
            var levels = GameObject.FindGameObjectsWithTag("Box").ToList<GameObject>();
            foreach(var rigidbox in GameObject.FindGameObjectsWithTag("Rigidbox").ToList<GameObject>())
            {
                if (rigidbox.transform.parent != null)
                {
                    levels.Add(rigidbox);
                }
            }
            levels.AddRange(GameObject.FindGameObjectsWithTag("Wall").ToList<GameObject>());
            foreach (GameObject box in levels)
            {
                Debug.Log(box.name);
                if (enterboxes.Count != 0 && box.GetComponent<LevelScript>() != null && box.GetComponent<LevelScript>().SceneName == enterboxes[^1] && !box.GetComponent<LevelScript>().shortcut)
                {
                    if (box.transform.position.y < GridChange)
                    {
                        if (enterplayers[^1][^1].Count == 2)
                        {
                            if (enterplayers[^1][^1][^1] == "Up")
                            {
                                offset = Vector3.up;
                            }
                            else if (enterplayers[^1][^1][^1] == "Down")
                            {
                                offset = Vector3.down;
                            }
                            else if (enterplayers[^1][^1][^1] == "Left")
                            {
                                offset = Vector3.left;
                            }
                            else if (enterplayers[^1][^1][^1] == "Right")
                            {
                                offset = Vector3.right;
                            }
                            else if (enterplayers[^1][^1][^1] == "Up3D")
                            {
                                offset = Vector3.back;
                            }
                            else if (enterplayers[^1][^1][^1] == "Down3D")
                            {
                                offset = Vector3.forward;
                            }
                            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
                            {
                                if (player.name == enterplayers[^1][^1][0])
                                {
                                    dontSave = true;
                                    player.GetComponent<PlayerMovement>().camerapos = box.transform.position + offset;
                                    player.transform.localScale = Vector3.zero;
                                    player.transform.position = box.transform.position + offset;
                                    player.GetComponent<PlayerMovement>().SpriteUpdate();
                                    player.transform.localScale = Vector3.zero;
                                    player.transform.position = box.transform.position;
                                    player.GetComponent<PlayerMovement>().camerapos = player.transform.position + offset;
                                    player.GetComponent<PlayerMovement>().StartCoroutine(player.GetComponent<PlayerMovement>().MoveToPosition(box.transform.position + offset, 6 * Vector3.Distance(player.transform.position, box.transform.position + offset), offset, 6 * Vector3.Distance(player.transform.position, box.transform.position + offset), false, false, true));
                                    exitboxes.Add(enterboxes[^1]);
                                    enterboxes.RemoveAt(enterboxes.Count - 1);
                                }
                            }
                        }
                        else if (enterplayers[^1][^1].Count == 4)
                        {
                            if (enterplayers[^1][^1][1] == "Up")
                            {
                                offset1 = Vector3.up;
                            }
                            else if (enterplayers[^1][^1][1] == "Down")
                            {
                                offset1 = Vector3.down;
                            }
                            else if (enterplayers[^1][^1][1] == "Left")
                            {
                                offset1 = Vector3.left;
                            }
                            else if (enterplayers[^1][^1][1] == "Right")
                            {
                                offset1 = Vector3.right;
                            }
                            else if (enterplayers[^1][^1][1] == "Up3D")
                            {
                                offset1 = Vector3.back;
                            }
                            else if (enterplayers[^1][^1][1] == "Down3D")
                            {
                                offset1 = Vector3.forward;
                            }
                            if (enterplayers[^1][^1][3] == "Up")
                            {
                                offset2 = Vector3.up;
                            }
                            else if (enterplayers[^1][^1][3] == "Down")
                            {
                                offset2 = Vector3.down;
                            }
                            else if (enterplayers[^1][^1][3] == "Left")
                            {
                                offset2 = Vector3.left;
                            }
                            else if (enterplayers[^1][^1][3] == "Right")
                            {
                                offset2 = Vector3.right;
                            }
                            else if (enterplayers[^1][^1][3] == "Up3D")
                            {
                                offset2 = Vector3.back;
                            }
                            else if (enterplayers[^1][^1][3] == "Down3D")
                            {
                                offset2 = Vector3.forward;
                            }
                            foreach (GameObject rigidplayer in GameObject.FindGameObjectsWithTag("Rigidplayer"))
                            {
                                if (rigidplayer.GetComponent<RigidplayerMovement>() != null && rigidplayer.GetComponent<RigidplayerMovement>().Rigidplayer1.name == enterplayers[^1][^1][0] && rigidplayer.GetComponent<RigidplayerMovement>().Rigidplayer2.name == enterplayers[^1][^1][2])
                                {
                                    dontSave = true;
                                    rigidplayer.GetComponent<RigidplayerMovement>().camerapos1 = box.transform.position + offset1;
                                    rigidplayer.GetComponent<RigidplayerMovement>().camerapos2 = rigidplayer.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position + offset2;
                                    rigidplayer.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.localScale = Vector3.zero;
                                    rigidplayer.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.localScale = Vector3.zero;
                                    rigidplayer.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position = box.transform.position + offset1;
                                    rigidplayer.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position = rigidplayer.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position + offset2;
                                    rigidplayer.GetComponent<RigidplayerMovement>().SpriteUpdate();
                                    rigidplayer.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.localScale = Vector3.zero;
                                    rigidplayer.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.localScale = Vector3.zero;
                                    rigidplayer.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position = box.transform.position;
                                    rigidplayer.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position = box.transform.position;
                                    rigidplayer.GetComponent<RigidplayerMovement>().camerapos1 = rigidplayer.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position;
                                    rigidplayer.GetComponent<RigidplayerMovement>().camerapos2 = rigidplayer.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position;
                                    Debug.Log(rigidplayer.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position);
                                    Debug.Log(rigidplayer.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position);
                                    rigidplayer.GetComponent<RigidplayerMovement>().StartCoroutine(rigidplayer.GetComponent<RigidplayerMovement>().MoveToPosition(box.transform.position + offset1, 6 * Vector3.Distance(rigidplayer.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position, box.transform.position + offset1), box.transform.position + offset1 + offset2, 6 * Vector3.Distance(rigidplayer.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position, box.transform.position + offset1 + offset2), false, false, true));
                                    exitboxes.Add(enterboxes[^1]);
                                    enterboxes.RemoveAt(enterboxes.Count - 1);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (enterplayers[^1][^1].Count == 2)
                        {
                            if (enterplayers[^1][^1][^1] == "Up")
                            {
                                offset = Vector3.up;
                            }
                            else if (enterplayers[^1][^1][^1] == "Down")
                            {
                                offset = Vector3.down;
                            }
                            else if (enterplayers[^1][^1][^1] == "Left")
                            {
                                offset = Vector3.left / 2;
                            }
                            else if (enterplayers[^1][^1][^1] == "Right")
                            {
                                offset = Vector3.right / 2;
                            }
                            else if (enterplayers[^1][^1][^1] == "Up3D")
                            {
                                offset = Vector3.back;
                            }
                            else if (enterplayers[^1][^1][^1] == "Down3D")
                            {
                                offset = Vector3.forward;
                            }
                            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
                            {
                                if (player.name == enterplayers[^1][^1][0])
                                {
                                    dontSave = true;
                                    player.GetComponent<PlayerMovement>().camerapos = box.transform.position + offset;
                                    player.transform.localScale = Vector3.zero;
                                    player.transform.position = box.transform.position + offset;
                                    player.GetComponent<PlayerMovement>().SpriteUpdate();
                                    player.transform.localScale = Vector3.zero;
                                    player.transform.position = box.transform.position;
                                    player.GetComponent<PlayerMovement>().camerapos = player.transform.position + offset;
                                    player.GetComponent<PlayerMovement>().StartCoroutine(player.GetComponent<PlayerMovement>().MoveToPosition(box.transform.position + offset, 6 * Vector3.Distance(player.transform.position, box.transform.position + offset), offset, 6 * Vector3.Distance(player.transform.position, box.transform.position + offset), false, false, true));
                                    exitboxes.Add(enterboxes[^1]);
                                    enterboxes.RemoveAt(enterboxes.Count - 1);
                                }
                            }
                        }
                        else if (enterplayers[^1][^1].Count == 4)
                        {
                            if (enterplayers[^1][^1][1] == "Up")
                            {
                                offset1 = Vector3.up;
                            }
                            else if (enterplayers[^1][^1][1] == "Down")
                            {
                                offset1 = Vector3.down;
                            }
                            else if (enterplayers[^1][^1][1] == "Left")
                            {
                                offset1 = Vector3.left / 2;
                            }
                            else if (enterplayers[^1][^1][1] == "Right")
                            {
                                offset1 = Vector3.right / 2;
                            }
                            else if (enterplayers[^1][^1][1] == "Up3D")
                            {
                                offset1 = Vector3.back;
                            }
                            else if (enterplayers[^1][^1][1] == "Down3D")
                            {
                                offset1 = Vector3.forward;
                            }
                            if (enterplayers[^1][^1][3] == "Up")
                            {
                                offset2 = Vector3.up;
                            }
                            else if (enterplayers[^1][^1][3] == "Down")
                            {
                                offset2 = Vector3.down;
                            }
                            else if (enterplayers[^1][^1][3] == "Left")
                            {
                                offset2 = Vector3.left / 2;
                            }
                            else if (enterplayers[^1][^1][3] == "Right")
                            {
                                offset2 = Vector3.right / 2;
                            }
                            else if (enterplayers[^1][^1][3] == "Up3D")
                            {
                                offset2 = Vector3.back;
                            }
                            else if (enterplayers[^1][^1][3] == "Down3D")
                            {
                                offset2 = Vector3.forward;
                            }
                            foreach (GameObject rigidplayer in GameObject.FindGameObjectsWithTag("Rigidplayer"))
                            {
                                if (rigidplayer.GetComponent<RigidplayerMovement>() != null && rigidplayer.GetComponent<RigidplayerMovement>().Rigidplayer1.name == enterplayers[^1][^1][0] && rigidplayer.GetComponent<RigidplayerMovement>().Rigidplayer2.name == enterplayers[^1][^1][2])
                                {
                                    dontSave = true;
                                    rigidplayer.GetComponent<RigidplayerMovement>().camerapos1 = box.transform.position + offset1;
                                    rigidplayer.GetComponent<RigidplayerMovement>().camerapos2 = rigidplayer.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position + offset2;
                                    rigidplayer.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.localScale = Vector3.zero;
                                    rigidplayer.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.localScale = Vector3.zero;
                                    rigidplayer.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position = box.transform.position + offset1;
                                    rigidplayer.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position = rigidplayer.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position + offset2;
                                    rigidplayer.GetComponent<RigidplayerMovement>().SpriteUpdate();
                                    rigidplayer.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.localScale = Vector3.zero;
                                    rigidplayer.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.localScale = Vector3.zero;
                                    rigidplayer.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position = box.transform.position;
                                    rigidplayer.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position = box.transform.position;
                                    rigidplayer.GetComponent<RigidplayerMovement>().camerapos1 = rigidplayer.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position;
                                    rigidplayer.GetComponent<RigidplayerMovement>().camerapos2 = rigidplayer.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position;
                                    rigidplayer.GetComponent<RigidplayerMovement>().StartCoroutine(rigidplayer.GetComponent<RigidplayerMovement>().MoveToPosition(box.transform.position + offset1, 6 * Vector3.Distance(rigidplayer.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position, box.transform.position + offset1), box.transform.position + offset1 + offset2, 6 * Vector3.Distance(rigidplayer.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position, box.transform.position + offset1 + offset2), false, false, true));
                                    exitboxes.Add(enterboxes[^1]);
                                    enterboxes.RemoveAt(enterboxes.Count - 1);
                                }
                            }
                        }
                    }
                    break;
                }
            }
            
            exitplayers.Add(enterplayers[^1]);
            enterplayers.RemoveAt(enterplayers.Count - 1);
            ColorCheck();
            Debug.Log(enterplayers.Count);
            Debug.Log(enterboxes.Count);
            var str_ = string.Empty;
            foreach (var box in enterboxes)
            {
                str_ += box;
                str_ += ", ";
            }
            Debug.Log(str_);
            isMoving = false;
            exit = false;
        }
        players = GameObject.FindGameObjectsWithTag("Player");
        boxes = GameObject.FindGameObjectsWithTag("Box");
        rigidboxes = GameObject.FindGameObjectsWithTag("Rigidbox");
        rigidplayers = GameObject.FindGameObjectsWithTag("Rigidplayer");
        objects = new();
        foreach (GameObject player in players)
        {
            objects.Add(player);
        }
        foreach (GameObject box in boxes)
        {
            objects.Add(box);
        }
        foreach (GameObject rigidbox in rigidboxes)
        {
            if (rigidbox.transform.parent != null)
            {
                objects.Add(rigidbox);
            }
        }
        foreach (GameObject rigidplayer_ in rigidplayers)
        {
            if (rigidplayer_.transform.parent != null)
            {
                objects.Add(rigidplayer_);
            }
        }
        if (objects.Count > 0)
        {
            var moving = false;
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i] != null && ((objects[i].GetComponent<PlayerMovement>() != null && objects[i].GetComponent<PlayerMovement>().isMoving) || (objects[i].GetComponent<BoxMovement>() != null && objects[i].GetComponent<BoxMovement>().isMoving) || (objects[i].transform.parent != null && objects[i].transform.parent.GetComponent<RigidboxMovement>() != null && (objects[i].transform.parent.GetComponent<RigidboxMovement>().isMoving1 || objects[i].transform.parent.GetComponent<RigidboxMovement>().isMoving2)) || (objects[i].transform.parent != null && objects[i].transform.parent.GetComponent<RigidplayerMovement>() != null && (objects[i].transform.parent.GetComponent<RigidplayerMovement>().isMoving1 || objects[i].transform.parent.GetComponent<RigidplayerMovement>().isMoving2))))
                {
                    isMoving = true;
                    moving = true;
                    turnEnded = true;
                    break;
                }
                
            }
            if (!moving)
            {
                isMoving = false;
            }
            var noinputs = false;
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i] != null && (objects[i].GetComponent<PlayerMovement>() != null && objects[i].GetComponent<PlayerMovement>().noInput) || (objects[i].GetComponent<BoxMovement>() != null && objects[i].GetComponent<BoxMovement>().noInput) || (objects[i].transform.parent != null && objects[i].transform.parent.GetComponent<RigidboxMovement>() != null && objects[i].transform.parent.GetComponent<RigidboxMovement>().noInput) || (objects[i].transform.parent != null && objects[i].transform.parent.GetComponent<RigidplayerMovement>() != null && objects[i].transform.parent.GetComponent<RigidplayerMovement>().noInput))
                {
                    noinputs = true;
                    noInput = true;
                    break;
                }

            }
            if (!noinputs)
            {
                noInput = false;
            }
            if (UndidWon)
            {
                noInput = true;
            }
            if (!isMoving && !noInput && turnEnded)
            {
                ColorCheck();
                GameManager.CheckWin();
                Debug.Log("dontSave =" + dontSave);
                if (!dontSave && !noInput)
                {
                    SaveGameState();
                }
                turnEnded = false;
            }
        }

        if (((Input.GetKey(KeyCode.U) && !(savedStates.Count > 1 && savedStates[^2].SceneName != SceneManager.GetActiveScene().name)) || Input.GetKeyDown(KeyCode.U)) && !(Input.GetKey(KeyCode.W)) && !(Input.GetKey(KeyCode.E)) && !(Input.GetKey(KeyCode.A)) && !(Input.GetKey(KeyCode.S)) && !(Input.GetKey(KeyCode.D)) && !(Input.GetKey(KeyCode.Z)) && !(Input.GetKey(KeyCode.X)) && timer >= timerLimit)
        {
            if (savedStates.Count > 1 && savedStates[^2].SceneName != SceneManager.GetActiveScene().name)
            {
                foreach(string transition in transitions)
                {
                    Debug.Log(transition);
                }
                if (transitions.Count != 0 && transitions[^1] == "Enter")
                {
                    enterplayers.RemoveAt(enterplayers.Count - 1);
                    scenes.RemoveAt(scenes.Count - 1);
                    enterboxes.RemoveAt(enterboxes.Count - 1);
                    transitions.RemoveAt(transitions.Count -1);
                }
                else if(transitions.Count != 0 && transitions[^1] == "Exit")
                {
                    enterplayers.Add(exitplayers[^1]);
                    exitplayers.RemoveAt(exitplayers.Count - 1);
                    scenes.Add(exitscenes[^1]);
                    exitscenes.RemoveAt(exitscenes.Count - 1);
                    enterboxes.Add(exitboxes[^1]);
                    exitboxes.RemoveAt(exitboxes.Count - 1);
                    Debug.Log(enterplayers.Count);
                    Debug.Log(exitplayers.Count);
                    transitions.RemoveAt(transitions.Count - 1);
                }
                UndoAfter = true;
                SceneManager.LoadScene(savedStates[^2].SceneName);
                first = true;
            }
            else
            {
                Debug.Log("isMoving = " + isMoving);
                if (isMoving || noInput)
                {
                    SaveGameState();
                }
                foreach (var obj in objects)
                {
                    obj.SetActive(false);
                    if (obj.GetComponent<PlayerMovement>() != null) obj.GetComponent<PlayerMovement>().isMoving = false;
                    if (obj.GetComponent<PlayerMovement>() != null) obj.GetComponent<PlayerMovement>().readyToMove = false;
                    if (obj.GetComponent<BoxMovement>() != null) obj.GetComponent<BoxMovement>().isMoving = false;
                    if (obj.transform.parent != null && obj.transform.parent.GetComponent<RigidboxMovement>() != null) obj.transform.parent.GetComponent<RigidboxMovement>().isMoving1 = false;
                    if (obj.transform.parent != null && obj.transform.parent.GetComponent<RigidboxMovement>() != null) obj.transform.parent.GetComponent<RigidboxMovement>().isMoving2 = false;
                    if (obj.transform.parent != null && obj.transform.parent.GetComponent<RigidplayerMovement>() != null) obj.transform.parent.GetComponent<RigidplayerMovement>().isMoving1 = false;
                    if (obj.transform.parent != null && obj.transform.parent.GetComponent<RigidplayerMovement>() != null) obj.transform.parent.GetComponent<RigidplayerMovement>().isMoving2 = false;
                    if (obj.GetComponent<PlayerMovement>() != null) obj.GetComponent<PlayerMovement>().noInput = false;
                    if (obj.GetComponent<BoxMovement>() != null) obj.GetComponent<BoxMovement>().noInput = false;
                    if (obj.transform.parent != null && obj.transform.parent.GetComponent<RigidboxMovement>() != null) obj.transform.parent.GetComponent<RigidboxMovement>().noInput = false;
                    if (obj.transform.parent != null && obj.transform.parent.GetComponent<RigidplayerMovement>() != null) obj.transform.parent.GetComponent<RigidplayerMovement>().noInput = false;
                    obj.SetActive(true);
                }
                isMoving = false;
                noInput = false;
                Debug.Log(isMoving);
                Debug.Log(noInput);
                UndoMove();
                ColorCheck();
                timer = 0f;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Return) && !(Input.GetKey(KeyCode.W)) && !(Input.GetKey(KeyCode.E)) && !(Input.GetKey(KeyCode.A)) && !(Input.GetKey(KeyCode.S)) && !(Input.GetKey(KeyCode.D)) && !(Input.GetKey(KeyCode.Z)) && !(Input.GetKey(KeyCode.X)) && timer >= 0.2f && isMoving == false && noInput == false)
        {
            EnterLevel();
            timer = 0f;
        }
        if (Input.GetKeyDown(KeyCode.Backspace) && !(Input.GetKey(KeyCode.W)) && !(Input.GetKey(KeyCode.E)) && !(Input.GetKey(KeyCode.A)) && !(Input.GetKey(KeyCode.S)) && !(Input.GetKey(KeyCode.D)) && !(Input.GetKey(KeyCode.Z)) && !(Input.GetKey(KeyCode.X)) && timer >= 0.2f && isMoving == false && noInput == false || won)
        {
            if (won)
            {
                LevelsWon[SceneManager.GetActiveScene().name] = true;
                LevelsCompleted = (SerializableDictionary<string, bool>)LevelsWon;
                won = false;
            }
            ExitLevel();
            timer = 0f;
        }
        if (Input.GetKeyDown(KeyCode.R) && !(Input.GetKey(KeyCode.W)) && !(Input.GetKey(KeyCode.E)) && !(Input.GetKey(KeyCode.A)) && !(Input.GetKey(KeyCode.S)) && !(Input.GetKey(KeyCode.D)) && !(Input.GetKey(KeyCode.Z)) && !(Input.GetKey(KeyCode.X)))
        {
            if (savedStates.Count <= 1)
            {
                Debug.Log("No moves to undo");
            }
            else
            {
                foreach (var obj in objects)
                {
                    obj.SetActive(false);
                    if (obj.GetComponent<PlayerMovement>() != null) obj.GetComponent<PlayerMovement>().isMoving = false;
                    if (obj.GetComponent<PlayerMovement>() != null) obj.GetComponent<PlayerMovement>().readyToMove = false;
                    if (obj.GetComponent<BoxMovement>() != null) obj.GetComponent<BoxMovement>().isMoving = false;
                    if (obj.transform.parent != null && obj.transform.parent.GetComponent<RigidboxMovement>() != null) obj.transform.parent.GetComponent<RigidboxMovement>().isMoving1 = false;
                    if (obj.transform.parent != null && obj.transform.parent.GetComponent<RigidboxMovement>() != null) obj.transform.parent.GetComponent<RigidboxMovement>().isMoving2 = false;
                    if (obj.transform.parent != null && obj.transform.parent.GetComponent<RigidplayerMovement>() != null) obj.transform.parent.GetComponent<RigidplayerMovement>().isMoving1 = false;
                    if (obj.transform.parent != null && obj.transform.parent.GetComponent<RigidplayerMovement>() != null) obj.transform.parent.GetComponent<RigidplayerMovement>().isMoving2 = false;
                    if (obj.GetComponent<PlayerMovement>() != null) obj.GetComponent<PlayerMovement>().noInput = false;
                    if (obj.GetComponent<BoxMovement>() != null) obj.GetComponent<BoxMovement>().noInput = false;
                    if (obj.transform.parent != null && obj.transform.parent.GetComponent<RigidboxMovement>() != null) obj.transform.parent.GetComponent<RigidboxMovement>().noInput = false;
                    if (obj.transform.parent != null && obj.transform.parent.GetComponent<RigidplayerMovement>() != null) obj.transform.parent.GetComponent<RigidplayerMovement>().noInput = false;
                    obj.SetActive(true);
                }
                isMoving = false;
                noInput = false;
                for (int i = 1; i <= savedStates.Count; i++)
                {
                    if (savedStates[^i].SceneName != SceneManager.GetActiveScene().name)
                    {
                        savedStates[^(i-1)].LoadGameState();
                        break;
                    }
                    else if(i == savedStates.Count)
                    {
                        savedStates[^(i)].LoadGameState();
                        break;
                    }
                }
                SaveGameState();
                timer = 0f;
            }
        }
        
    }

    public static void SaveGameState()
    {
        if (savedStates.Count == 0 || GameState.GetCurrentState() != savedStates[^1])
        {
            savedStates.Add(GameState.GetCurrentState());
        }
    }
    public static void UndoMove()
    {
        Debug.Log(savedStates.Count);
        if (savedStates.Count <= 1)
        {
            Debug.Log("No moves to undo");
        }
        else
        {
            isMoving = true;
            savedStates[^2].LoadGameState();
            if (!UndoAfter)
            {
                savedStates.RemoveAt(savedStates.Count - 1);
            }
            Debug.Log("Undo " + isMoving);
            Physics2D.SyncTransforms();
            isMoving = false;
            Debug.Log(savedStates.Count);
            Debug.Log("Undo " + isMoving);
            Debug.Log("Undo " + noInput);
        }
    }

    public void EnterLevel()
    {
        Debug.Log("Entering");
        transitions.Add("Enter");
        var players_ = GameObject.FindGameObjectsWithTag("Player");
        var rigidplayers = GameObject.FindGameObjectsWithTag("Rigidplayer").ToList();
        var rigidplayers_ = new List<GameObject>();
        foreach (var rigidplayer in rigidplayers)
        {
            if (rigidplayer.transform.parent != null)
            {
                rigidplayers_.Add(rigidplayer);
            }
        }
        var levels = new List<LevelScript>();
        var indexes = new List<int>();
        var playersToLevels = new Dictionary<GameObject, Vector3>();
        foreach (GameObject player in players_)
        {
            Debug.Log(player.name);
            if (player.transform.position.y < GridChange)
            {
                RaycastHit2D up = Physics2D.Raycast(player.transform.position, Vector3.up, 1, blockingLayer, player.transform.position.z, player.transform.position.z);
                RaycastHit2D down = Physics2D.Raycast(player.transform.position, Vector3.down, 1, blockingLayer, player.transform.position.z, player.transform.position.z);
                RaycastHit2D right = Physics2D.Raycast(player.transform.position, Vector3.right, 1, blockingLayer, player.transform.position.z, player.transform.position.z);
                RaycastHit2D left = Physics2D.Raycast(player.transform.position, Vector3.left, 1, blockingLayer, player.transform.position.z, player.transform.position.z);
                if (up.collider != null && up.collider.gameObject.GetComponent<LevelScript>() != null && (!up.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[up.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(up.collider.gameObject.GetComponent<LevelScript>());
                }
                if (down.collider != null && down.collider.gameObject.GetComponent<LevelScript>() != null && (!down.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[down.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(down.collider.gameObject.GetComponent<LevelScript>());
                }
                if (right.collider != null && right.collider.gameObject.GetComponent<LevelScript>() != null && (!right.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[right.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(right.collider.gameObject.GetComponent<LevelScript>());
                }
                if (left.collider != null && left.collider.gameObject.GetComponent<LevelScript>() != null && (!left.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[left.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(left.collider.gameObject.GetComponent<LevelScript>());
                }
                if (Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z - 1, player.transform.position.z - 1) != null && Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z - 1, player.transform.position.z - 1).gameObject.GetComponent<LevelScript>() != null && (!Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z - 1, player.transform.position.z - 1).gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z - 1, player.transform.position.z - 1).gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z - 1, player.transform.position.z - 1).gameObject.GetComponent<LevelScript>());
                }
                if (Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z + 1, player.transform.position.z + 1) != null && Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z + 1, player.transform.position.z + 1).gameObject.GetComponent<LevelScript>() != null && (!Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z + 1, player.transform.position.z + 1).gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z + 1, player.transform.position.z + 1).gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z + 1, player.transform.position.z + 1).gameObject.GetComponent<LevelScript>());
                }

            }
            else if (player.transform.localScale == Vector3.one)
            {
                RaycastHit2D down = Physics2D.Raycast(player.transform.position, Vector3.down, 1, blockingLayer, player.transform.position.z, player.transform.position.z);
                RaycastHit2D right = Physics2D.Raycast(player.transform.position, Vector3.right, 0.5f, blockingLayer, player.transform.position.z, player.transform.position.z);
                RaycastHit2D left = Physics2D.Raycast(player.transform.position, Vector3.left, 0.5f, blockingLayer, player.transform.position.z, player.transform.position.z);
                if (down.collider != null && down.collider.gameObject.GetComponent<LevelScript>() != null && (!down.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[down.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(down.collider.gameObject.GetComponent<LevelScript>());
                }
                if (right.collider != null && right.collider.gameObject.GetComponent<LevelScript>() != null && (!right.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[right.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(right.collider.gameObject.GetComponent<LevelScript>());
                }
                if (left.collider != null && left.collider.gameObject.GetComponent<LevelScript>() != null && (!left.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[left.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(left.collider.gameObject.GetComponent<LevelScript>());
                }
                if (Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z - 1, player.transform.position.z - 1) != null && Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z - 1, player.transform.position.z - 1).gameObject.GetComponent<LevelScript>() != null && (!Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z - 1, player.transform.position.z - 1).gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z - 1, player.transform.position.z - 1).gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z - 1, player.transform.position.z - 1).gameObject.GetComponent<LevelScript>());
                }
                if (Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z + 1, player.transform.position.z + 1) != null && Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z + 1, player.transform.position.z + 1).gameObject.GetComponent<LevelScript>() != null && (!Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z + 1, player.transform.position.z + 1).gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z + 1, player.transform.position.z + 1).gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z + 1, player.transform.position.z + 1).gameObject.GetComponent<LevelScript>());
                }
            }
            else if (player.transform.localScale == new Vector3(1, -1, 1))
            {
                RaycastHit2D up = Physics2D.Raycast(player.transform.position, Vector3.up, 1, blockingLayer, player.transform.position.z, player.transform.position.z);
                RaycastHit2D right = Physics2D.Raycast(player.transform.position, Vector3.right, 0.5f, blockingLayer, player.transform.position.z, player.transform.position.z);
                RaycastHit2D left = Physics2D.Raycast(player.transform.position, Vector3.left, 0.5f, blockingLayer, player.transform.position.z, player.transform.position.z);
                if (up.collider != null && up.collider.gameObject.GetComponent<LevelScript>() != null && (!up.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[up.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(up.collider.gameObject.GetComponent<LevelScript>());
                }
                if (right.collider != null && right.collider.gameObject.GetComponent<LevelScript>() != null && (!right.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[right.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(right.collider.gameObject.GetComponent<LevelScript>());
                }
                if (left.collider != null && left.collider.gameObject.GetComponent<LevelScript>() != null && (!left.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[left.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(left.collider.gameObject.GetComponent<LevelScript>());
                }
                if (Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z - 1, player.transform.position.z - 1) != null && Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z - 1, player.transform.position.z - 1).gameObject.GetComponent<LevelScript>() != null && (!Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z - 1, player.transform.position.z - 1).gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z - 1, player.transform.position.z - 1).gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z - 1, player.transform.position.z - 1).gameObject.GetComponent<LevelScript>());
                }
                if (Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z + 1, player.transform.position.z + 1) != null && Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z + 1, player.transform.position.z + 1).gameObject.GetComponent<LevelScript>() != null && (!Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z + 1, player.transform.position.z + 1).gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z + 1, player.transform.position.z + 1).gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z + 1, player.transform.position.z + 1).gameObject.GetComponent<LevelScript>());
                }
            }
        }
        foreach (GameObject rigidplayer in rigidplayers_)
        {
            Debug.Log(rigidplayer.name);
            if (rigidplayer.transform.position.y < GridChange)
            {
                RaycastHit2D up = Physics2D.Raycast(rigidplayer.transform.position, Vector3.up, 1, blockingLayer, rigidplayer.transform.position.z, rigidplayer.transform.position.z);
                RaycastHit2D down = Physics2D.Raycast(rigidplayer.transform.position, Vector3.down, 1, blockingLayer, rigidplayer.transform.position.z, rigidplayer.transform.position.z);
                RaycastHit2D right = Physics2D.Raycast(rigidplayer.transform.position, Vector3.right, 1, blockingLayer, rigidplayer.transform.position.z, rigidplayer.transform.position.z);
                RaycastHit2D left = Physics2D.Raycast(rigidplayer.transform.position, Vector3.left, 1, blockingLayer, rigidplayer.transform.position.z, rigidplayer.transform.position.z);
                if (up.collider != null && up.collider.gameObject.GetComponent<LevelScript>() != null && (!up.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[up.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(up.collider.gameObject.GetComponent<LevelScript>());
                }
                if (down.collider != null && down.collider.gameObject.GetComponent<LevelScript>() != null && (!down.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[down.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(down.collider.gameObject.GetComponent<LevelScript>());
                }
                if (right.collider != null && right.collider.gameObject.GetComponent<LevelScript>() != null && (!right.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[right.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(right.collider.gameObject.GetComponent<LevelScript>());
                }
                if (left.collider != null && left.collider.gameObject.GetComponent<LevelScript>() != null && (!left.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[left.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(left.collider.gameObject.GetComponent<LevelScript>());
                }
                if (Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z - 1, rigidplayer.transform.position.z - 1) != null && Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z - 1, rigidplayer.transform.position.z - 1).gameObject.GetComponent<LevelScript>() != null && (!Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z - 1, rigidplayer.transform.position.z - 1).gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z - 1, rigidplayer.transform.position.z - 1).gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z - 1, rigidplayer.transform.position.z - 1).gameObject.GetComponent<LevelScript>());
                }
                if (Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z + 1, rigidplayer.transform.position.z + 1) != null && Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z + 1, rigidplayer.transform.position.z + 1).gameObject.GetComponent<LevelScript>() != null && (!Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z + 1, rigidplayer.transform.position.z + 1).gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z + 1, rigidplayer.transform.position.z + 1).gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z + 1, rigidplayer.transform.position.z + 1).gameObject.GetComponent<LevelScript>());
                }

            }
            else if (rigidplayer.transform.localScale == Vector3.one)
            {
                RaycastHit2D down = Physics2D.Raycast(rigidplayer.transform.position, Vector3.down, 1, blockingLayer, rigidplayer.transform.position.z, rigidplayer.transform.position.z);
                RaycastHit2D right = Physics2D.Raycast(rigidplayer.transform.position, Vector3.right, 0.5f, blockingLayer, rigidplayer.transform.position.z, rigidplayer.transform.position.z);
                RaycastHit2D left = Physics2D.Raycast(rigidplayer.transform.position, Vector3.left, 0.5f, blockingLayer, rigidplayer.transform.position.z, rigidplayer.transform.position.z);
                if (down.collider != null && down.collider.gameObject.GetComponent<LevelScript>() != null && (!down.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[down.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(down.collider.gameObject.GetComponent<LevelScript>());
                }
                if (right.collider != null && right.collider.gameObject.GetComponent<LevelScript>() != null && (!right.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[right.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(right.collider.gameObject.GetComponent<LevelScript>());
                }
                if (left.collider != null && left.collider.gameObject.GetComponent<LevelScript>() != null && (!left.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[left.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(left.collider.gameObject.GetComponent<LevelScript>());
                }
                if (Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z - 1, rigidplayer.transform.position.z - 1) != null && Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z - 1, rigidplayer.transform.position.z - 1).gameObject.GetComponent<LevelScript>() != null && (!Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z - 1, rigidplayer.transform.position.z - 1).gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z - 1, rigidplayer.transform.position.z - 1).gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z - 1, rigidplayer.transform.position.z - 1).gameObject.GetComponent<LevelScript>());
                }
                if (Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z + 1, rigidplayer.transform.position.z + 1) != null && Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z + 1, rigidplayer.transform.position.z + 1).gameObject.GetComponent<LevelScript>() != null && (!Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z + 1, rigidplayer.transform.position.z + 1).gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z + 1, rigidplayer.transform.position.z + 1).gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z + 1, rigidplayer.transform.position.z + 1).gameObject.GetComponent<LevelScript>());
                }
            }
            else if (rigidplayer.transform.localScale == new Vector3(1, -1, 1))
            {
                RaycastHit2D up = Physics2D.Raycast(rigidplayer.transform.position, Vector3.up, 1, blockingLayer, rigidplayer.transform.position.z, rigidplayer.transform.position.z);
                RaycastHit2D right = Physics2D.Raycast(rigidplayer.transform.position, Vector3.right, 0.5f, blockingLayer, rigidplayer.transform.position.z, rigidplayer.transform.position.z);
                RaycastHit2D left = Physics2D.Raycast(rigidplayer.transform.position, Vector3.left, 0.5f, blockingLayer, rigidplayer.transform.position.z, rigidplayer.transform.position.z);
                if (up.collider != null && up.collider.gameObject.GetComponent<LevelScript>() != null && (!up.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[up.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(up.collider.gameObject.GetComponent<LevelScript>());
                }
                if (right.collider != null && right.collider.gameObject.GetComponent<LevelScript>() != null && (!right.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[right.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(right.collider.gameObject.GetComponent<LevelScript>());
                }
                if (left.collider != null && left.collider.gameObject.GetComponent<LevelScript>() != null && (!left.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[left.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(left.collider.gameObject.GetComponent<LevelScript>());
                }
                if (Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z - 1, rigidplayer.transform.position.z - 1) != null && Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z - 1, rigidplayer.transform.position.z - 1).gameObject.GetComponent<LevelScript>() != null && (!Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z - 1, rigidplayer.transform.position.z - 1).gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z - 1, rigidplayer.transform.position.z - 1).gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z - 1, rigidplayer.transform.position.z - 1).gameObject.GetComponent<LevelScript>());
                }
                if (Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z + 1, rigidplayer.transform.position.z + 1) != null && Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z + 1, rigidplayer.transform.position.z + 1).gameObject.GetComponent<LevelScript>() != null && (!Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z + 1, rigidplayer.transform.position.z + 1).gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z + 1, rigidplayer.transform.position.z + 1).gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z + 1, rigidplayer.transform.position.z + 1).gameObject.GetComponent<LevelScript>());
                }
            }
        }
        foreach (var level in levels)
        {
            if (level.SceneBuildIndex(level.SceneName) != -1)
            {
                indexes.Add(level.SceneBuildIndex(level.SceneName));
            }
        }
        if (indexes.Count != 0)
        {
            enterplayers.Add(new List<List<string>>());
            Debug.Log("List Added");
        }
        foreach(var level in levels)
        {
            Debug.Log(level.SceneName);
            if (level.SceneBuildIndex(level.SceneName) == indexes.Min())
            {
                Debug.Log("Min");
                scenes.Add(parentscenes[level.SceneName]);
                enterboxes.Add(level.SceneName);
                if (level.transform.position.y < GridChange)
                {
                    RaycastHit2D up = Physics2D.Raycast(level.transform.position, Vector3.up, 1, blockingLayer, level.transform.position.z, level.transform.position.z);
                    RaycastHit2D down = Physics2D.Raycast(level.transform.position, Vector3.down, 1, blockingLayer, level.transform.position.z, level.transform.position.z);
                    RaycastHit2D right = Physics2D.Raycast(level.transform.position, Vector3.right, 1, blockingLayer, level.transform.position.z, level.transform.position.z);
                    RaycastHit2D left = Physics2D.Raycast(level.transform.position, Vector3.left, 1, blockingLayer, level.transform.position.z, level.transform.position.z);
                    if (up.collider != null)
                    {
                        Debug.Log(up.collider.gameObject.name);
                    }
                    if (down.collider != null)
                    {
                        Debug.Log(down.collider.gameObject.name);
                    }
                    if (right.collider != null)
                    {
                        Debug.Log(right.collider.gameObject.name);
                    }
                    if (left.collider != null)
                    {
                        Debug.Log(left.collider.gameObject.name);
                    }
                    if (up.collider != null && up.collider.gameObject.CompareTag("Player"))
                    {
                        enterplayers[^1].Add(new List<string>());
                        enterplayers[^1][^1].Add(up.collider.gameObject.name);
                        enterplayers[^1][^1].Add("Up");
                        playersToLevels.Add(up.collider.gameObject, level.transform.position);
                        Debug.Log("Up");
                    }
                    if (down.collider != null && down.collider.gameObject.CompareTag("Player"))
                    {
                        enterplayers[^1].Add(new List<string>());
                        enterplayers[^1][^1].Add(down.collider.gameObject.name);
                        enterplayers[^1][^1].Add("Down");
                        playersToLevels.Add(down.collider.gameObject, level.transform.position);
                        Debug.Log("Down");
                    }
                    if (right.collider != null && right.collider.gameObject.CompareTag("Player"))
                    {
                        enterplayers[^1].Add(new List<string>());
                        enterplayers[^1][^1].Add(right.collider.gameObject.name);
                        enterplayers[^1][^1].Add("Right");
                        playersToLevels.Add(right.collider.gameObject, level.transform.position);
                        Debug.Log("Right");
                    }
                    if (left.collider != null && left.collider.gameObject.CompareTag("Player"))
                    {
                        enterplayers[^1].Add(new List<string>());
                        enterplayers[^1][^1].Add(left.collider.gameObject.name);
                        enterplayers[^1][^1].Add("Left");
                        playersToLevels.Add(left.collider.gameObject, level.transform.position);
                        Debug.Log("Left");
                    }
                    if (Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z - 1, level.transform.position.z - 1) != null && Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z - 1, level.transform.position.z - 1).gameObject.CompareTag("Player"))
                    {
                        enterplayers[^1].Add(new List<string>());
                        enterplayers[^1][^1].Add(Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z - 1, level.transform.position.z - 1).gameObject.name);
                        enterplayers[^1][^1].Add("Up3D");
                        playersToLevels.Add(Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z - 1, level.transform.position.z - 1).gameObject, level.transform.position);
                        Debug.Log("Up3D");
                    }
                    if (Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z + 1, level.transform.position.z + 1) != null && Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z + 1, level.transform.position.z + 1).gameObject.CompareTag("Player"))
                    {
                        enterplayers[^1].Add(new List<string>());
                        enterplayers[^1][^1].Add(Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z + 1, level.transform.position.z + 1).gameObject.name);
                        enterplayers[^1][^1].Add("Down3D");
                        playersToLevels.Add(Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z + 1, level.transform.position.z + 1).gameObject, level.transform.position);
                        Debug.Log("Down3D");
                    }
                    if (up.collider != null && up.collider.gameObject.CompareTag("Rigidplayer"))
                    {
                        var player = up.collider.gameObject;
                        enterplayers[^1].Add(new List<string>());
                        enterplayers[^1][^1].Add(player.name);
                        enterplayers[^1][^1].Add("Up");
                        playersToLevels.Add(player.transform.parent.gameObject, level.transform.position);
                        if (player.transform.parent.GetComponent<RigidplayerMovement>() != null && player == player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1)
                        {
                            enterplayers[^1][^1].Add(player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.name);
                            if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.up)
                            {
                                enterplayers[^1][^1].Add("Up");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.down)
                            {
                                enterplayers[^1][^1].Add("Down");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.right || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.right / 2)
                            {
                                enterplayers[^1][^1].Add("Right");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.left || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.left / 2)
                            {
                                enterplayers[^1][^1].Add("Left");
                            }
                        }
                        else if (player.transform.parent.GetComponent<RigidplayerMovement>() != null && player == player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2)
                        {
                            enterplayers[^1][^1].Add(player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.name);
                            if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.up)
                            {
                                enterplayers[^1][^1].Add("Up");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.down)
                            {
                                enterplayers[^1][^1].Add("Down");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.right || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.right / 2)
                            {
                                enterplayers[^1][^1].Add("Right");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.left || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.left / 2)
                            {
                                enterplayers[^1][^1].Add("Left");
                            }
                            if (enterplayers[^1][^1].Count == 4)
                            {
                                var temp = enterplayers[^1][^1][0];
                                enterplayers[^1][^1] [0] = enterplayers[^1][^1][2];
                                enterplayers[^1][^1][2] = temp;
                            }
                        }
                        Debug.Log("Up");
                    }
                    if (down.collider != null && down.collider.gameObject.CompareTag("Rigidplayer"))
                    {
                        var player = down.collider.gameObject;
                        enterplayers[^1].Add(new List<string>());
                        enterplayers[^1][^1].Add(down.collider.gameObject.name);
                        enterplayers[^1][^1].Add("Down");
                        playersToLevels.Add(player.transform.parent.gameObject, level.transform.position);
                        if (player.transform.parent.GetComponent<RigidplayerMovement>() != null && player == player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1)
                        {
                            enterplayers[^1][^1].Add(player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.name);
                            if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.up)
                            {
                                enterplayers[^1][^1].Add("Up");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.down)
                            {
                                enterplayers[^1][^1].Add("Down");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.right || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.right / 2)
                            {
                                enterplayers[^1][^1].Add("Right");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.left || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.left / 2)
                            {
                                enterplayers[^1][^1].Add("Left");
                            }
                        }
                        else if (player.transform.parent.GetComponent<RigidplayerMovement>() != null && player == player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2)
                        {
                            enterplayers[^1][^1].Add(player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.name);
                            if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.up)
                            {
                                enterplayers[^1][^1].Add("Up");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.down)
                            {
                                enterplayers[^1][^1].Add("Down");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.right || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.right / 2)
                            {
                                enterplayers[^1][^1].Add("Right");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.left || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.left / 2)
                            {
                                enterplayers[^1][^1].Add("Left");
                            }
                            if (enterplayers[^1][^1].Count == 4)
                            {
                                var temp = enterplayers[^1][^1][0];
                                enterplayers[^1][^1][0] = enterplayers[^1][^1][2];
                                enterplayers[^1][^1][2] = temp;
                            }
                        }
                        Debug.Log("Down");
                    }
                    if (right.collider != null && right.collider.gameObject.CompareTag("Rigidplayer"))
                    {
                        var player = right.collider.gameObject;
                        enterplayers[^1].Add(new List<string>());
                        enterplayers[^1][^1].Add(right.collider.gameObject.name);
                        enterplayers[^1][^1].Add("Right");
                        playersToLevels.Add(player.transform.parent.gameObject, level.transform.position);
                        if (player.transform.parent.GetComponent<RigidplayerMovement>() != null && player == player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1)
                        {
                            enterplayers[^1][^1].Add(player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.name);
                            if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.up)
                            {
                                enterplayers[^1][^1].Add("Up");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.down)
                            {
                                enterplayers[^1][^1].Add("Down");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.right || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.right / 2)
                            {
                                enterplayers[^1][^1].Add("Right");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.left || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.left / 2)
                            {
                                enterplayers[^1][^1].Add("Left");
                            }
                        }
                        else if (player.transform.parent.GetComponent<RigidplayerMovement>() != null && player == player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2)
                        {
                            enterplayers[^1][^1].Add(player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.name);
                            if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.up)
                            {
                                enterplayers[^1][^1].Add("Up");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.down)
                            {
                                enterplayers[^1][^1].Add("Down");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.right || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.right / 2)
                            {
                                enterplayers[^1][^1].Add("Right");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.left || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.left / 2)
                            {
                                enterplayers[^1][^1].Add("Left");
                            }
                            if (enterplayers[^1][^1].Count == 4)
                            {
                                var temp = enterplayers[^1][^1][0];
                                enterplayers[^1][^1][0] = enterplayers[^1][^1][2];
                                enterplayers[^1][^1][2] = temp;
                            }
                        }
                        Debug.Log("Right");
                    }
                    if (left.collider != null && left.collider.gameObject.CompareTag("Rigidplayer"))
                    {
                        var player = left.collider.gameObject;
                        enterplayers[^1].Add(new List<string>());
                        enterplayers[^1][^1].Add(left.collider.gameObject.name);
                        enterplayers[^1][^1].Add("Left");
                        playersToLevels.Add(player.transform.parent.gameObject, level.transform.position);
                        if (player.transform.parent.GetComponent<RigidplayerMovement>() != null && player == player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1)
                        {
                            enterplayers[^1][^1].Add(player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.name);
                            if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.up)
                            {
                                enterplayers[^1][^1].Add("Up");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.down)
                            {
                                enterplayers[^1][^1].Add("Down");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.right || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.right / 2)
                            {
                                enterplayers[^1][^1].Add("Right");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.left || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.left / 2)
                            {
                                enterplayers[^1][^1].Add("Left");
                            }
                        }
                        else if (player.transform.parent.GetComponent<RigidplayerMovement>() != null && player == player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2)
                        {
                            enterplayers[^1][^1].Add(player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.name);
                            if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.up)
                            {
                                enterplayers[^1][^1].Add("Up");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.down)
                            {
                                enterplayers[^1][^1].Add("Down");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.right || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.right / 2)
                            {
                                enterplayers[^1][^1].Add("Right");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.left || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.left / 2)
                            {
                                enterplayers[^1][^1].Add("Left");
                            }
                            if (enterplayers[^1][^1].Count == 4)
                            {
                                var temp = enterplayers[^1][^1][0];
                                enterplayers[^1][^1][0] = enterplayers[^1][^1][2];
                                enterplayers[^1][^1][2] = temp;
                            }
                        }
                        Debug.Log("Left");
                    }
                    if (Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z - 1, level.transform.position.z - 1) != null && Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z - 1, level.transform.position.z - 1).gameObject.CompareTag("Rigidplayer"))
                    {
                        var player = Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z - 1, level.transform.position.z - 1).gameObject;
                        enterplayers[^1].Add(new List<string>());
                        enterplayers[^1][^1].Add(Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z - 1, level.transform.position.z - 1).gameObject.name);
                        enterplayers[^1][^1].Add("Up3D");
                        playersToLevels.Add(player.transform.parent.gameObject, level.transform.position);
                        if (player.transform.parent.GetComponent<RigidplayerMovement>() != null && player == player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1)
                        {
                            enterplayers[^1][^1].Add(player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.name);
                            if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.up)
                            {
                                enterplayers[^1][^1].Add("Up");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.down)
                            {
                                enterplayers[^1][^1].Add("Down");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.right || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.right / 2)
                            {
                                enterplayers[^1][^1].Add("Right");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.left || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.left / 2)
                            {
                                enterplayers[^1][^1].Add("Left");
                            }
                        }
                        else if (player.transform.parent.GetComponent<RigidplayerMovement>() != null && player == player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2)
                        {
                            enterplayers[^1][^1].Add(player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.name);
                            if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.up)
                            {
                                enterplayers[^1][^1].Add("Up");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.down)
                            {
                                enterplayers[^1][^1].Add("Down");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.right || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.right / 2)
                            {
                                enterplayers[^1][^1].Add("Right");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.left || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.left / 2)
                            {
                                enterplayers[^1][^1].Add("Left");
                            }
                            if (enterplayers[^1][^1].Count == 4)
                            {
                                var temp = enterplayers[^1][^1][0];
                                enterplayers[^1][^1][0] = enterplayers[^1][^1][2];
                                enterplayers[^1][^1][2] = temp;
                            }
                        }
                        Debug.Log("Up3D");
                    }
                    if (Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z + 1, level.transform.position.z + 1) != null && Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z + 1, level.transform.position.z + 1).gameObject.CompareTag("Rigidplayer"))
                    {
                        var player = Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z + 1, level.transform.position.z + 1).gameObject;
                        enterplayers[^1].Add(new List<string>());
                        enterplayers[^1][^1].Add(Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z + 1, level.transform.position.z + 1).gameObject.name);
                        enterplayers[^1][^1].Add("Down3D");
                        playersToLevels.Add(player.transform.parent.gameObject, level.transform.position);
                        if (player.transform.parent.GetComponent<RigidplayerMovement>() != null && player == player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1)
                        {
                            enterplayers[^1][^1].Add(player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.name);
                            if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.up)
                            {
                                enterplayers[^1][^1].Add("Up");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.down)
                            {
                                enterplayers[^1][^1].Add("Down");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.right || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.right / 2)
                            {
                                enterplayers[^1][^1].Add("Right");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.left || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.left / 2)
                            {
                                enterplayers[^1][^1].Add("Left");
                            }
                        }
                        else if (player.transform.parent.GetComponent<RigidplayerMovement>() != null && player == player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2)
                        {
                            enterplayers[^1][^1].Add(player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.name);
                            if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.up)
                            {
                                enterplayers[^1][^1].Add("Up");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.down)
                            {
                                enterplayers[^1][^1].Add("Down");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.right || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.right / 2)
                            {
                                enterplayers[^1][^1].Add("Right");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.left || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.left / 2)
                            {
                                enterplayers[^1][^1].Add("Left");
                            }
                            if (enterplayers[^1][^1].Count == 4)
                            {
                                var temp = enterplayers[^1][^1][0];
                                enterplayers[^1][^1][0] = enterplayers[^1][^1][2];
                                enterplayers[^1][^1][2] = temp;
                            }
                        }
                        Debug.Log("Down3D");
                    }
                }
                else if (level.transform.localScale == Vector3.one)
                {
                    RaycastHit2D down = Physics2D.Raycast(level.transform.position, Vector3.down, 1, blockingLayer, level.transform.position.z, level.transform.position.z);
                    RaycastHit2D right = Physics2D.Raycast(level.transform.position, Vector3.right, 0.5f, blockingLayer, level.transform.position.z, level.transform.position.z);
                    RaycastHit2D left = Physics2D.Raycast(level.transform.position, Vector3.left, 0.5f, blockingLayer, level.transform.position.z, level.transform.position.z);
                    if (down.collider != null && down.collider.gameObject.CompareTag("Player"))
                    {
                        enterplayers[^1].Add(new List<string>());
                        enterplayers[^1][^1].Add(down.collider.gameObject.name);
                        enterplayers[^1][^1].Add("Down");
                        playersToLevels.Add(down.collider.gameObject, level.transform.position);
                    }
                    if (right.collider != null && right.collider.gameObject.CompareTag("Player"))
                    {
                        enterplayers[^1].Add(new List<string>());
                        enterplayers[^1][^1].Add(right.collider.gameObject.name);
                        enterplayers[^1][^1].Add("Right");
                        playersToLevels.Add(right.collider.gameObject, level.transform.position);
                    }
                    if (left.collider != null && left.collider.gameObject.CompareTag("Player"))
                    {
                        enterplayers[^1].Add(new List<string>());
                        enterplayers[^1][^1].Add(left.collider.gameObject.name);
                        enterplayers[^1][^1].Add("Left");
                        playersToLevels.Add(left.collider.gameObject, level.transform.position);
                    }
                    if (Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z - 1, level.transform.position.z - 1) != null && Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z - 1, level.transform.position.z - 1).gameObject.CompareTag("Player"))
                    {
                        enterplayers[^1].Add(new List<string>());
                        enterplayers[^1][^1].Add(Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z - 1, level.transform.position.z - 1).gameObject.name);
                        enterplayers[^1][^1].Add("Up3D");
                        Debug.Log("Up3D");
                        playersToLevels.Add(Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z - 1, level.transform.position.z - 1).gameObject, level.transform.position);
                    }
                    if (Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z + 1, level.transform.position.z + 1) != null && Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z + 1, level.transform.position.z + 1).gameObject.CompareTag("Player"))
                    {
                        enterplayers[^1].Add(new List<string>());
                        enterplayers[^1][^1].Add(Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z + 1, level.transform.position.z + 1).gameObject.name);
                        enterplayers[^1][^1].Add("Down3D");
                        Debug.Log("Down3D");
                        playersToLevels.Add(Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z + 1, level.transform.position.z + 1).gameObject, level.transform.position);
                    }
                    if (down.collider != null && down.collider.gameObject.CompareTag("Rigidplayer"))
                    {
                        var player = down.collider.gameObject;
                        enterplayers[^1].Add(new List<string>());
                        enterplayers[^1][^1].Add(down.collider.gameObject.name);
                        enterplayers[^1][^1].Add("Down");
                        playersToLevels.Add(player.transform.parent.gameObject, level.transform.position);
                        if (player.transform.parent.GetComponent<RigidplayerMovement>() != null && player == player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1)
                        {
                            enterplayers[^1][^1].Add(player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.name);
                            if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.up)
                            {
                                enterplayers[^1][^1].Add("Up");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.down)
                            {
                                enterplayers[^1][^1].Add("Down");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.right || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.right / 2)
                            {
                                enterplayers[^1][^1].Add("Right");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.left || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.left / 2)
                            {
                                enterplayers[^1][^1].Add("Left");
                            }
                        }
                        else if (player.transform.parent.GetComponent<RigidplayerMovement>() != null && player == player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2)
                        {
                            enterplayers[^1][^1].Add(player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.name);
                            if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.up)
                            {
                                enterplayers[^1][^1].Add("Up");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.down)
                            {
                                enterplayers[^1][^1].Add("Down");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.right || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.right / 2)
                            {
                                enterplayers[^1][^1].Add("Right");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.left || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.left / 2)
                            {
                                enterplayers[^1][^1].Add("Left");
                            }
                            if (enterplayers[^1][^1].Count == 4)
                            {
                                var temp = enterplayers[^1][^1][0];
                                enterplayers[^1][^1][0] = enterplayers[^1][^1][2];
                                enterplayers[^1][^1][2] = temp;
                            }
                        }
                        Debug.Log("Down");
                    }
                    if (right.collider != null && right.collider.gameObject.CompareTag("Rigidplayer"))
                    {
                        var player = right.collider.gameObject;
                        enterplayers[^1].Add(new List<string>());
                        enterplayers[^1][^1].Add(right.collider.gameObject.name);
                        enterplayers[^1][^1].Add("Right");
                        playersToLevels.Add(player.transform.parent.gameObject, level.transform.position);
                        if (player.transform.parent.GetComponent<RigidplayerMovement>() != null && player == player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1)
                        {
                            enterplayers[^1][^1].Add(player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.name);
                            if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.up)
                            {
                                enterplayers[^1][^1].Add("Up");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.down)
                            {
                                enterplayers[^1][^1].Add("Down");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.right || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.right / 2)
                            {
                                enterplayers[^1][^1].Add("Right");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.left || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.left / 2)
                            {
                                enterplayers[^1][^1].Add("Left");
                            }
                        }
                        else if (player.transform.parent.GetComponent<RigidplayerMovement>() != null && player == player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2)
                        {
                            enterplayers[^1][^1].Add(player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.name);
                            if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.up)
                            {
                                enterplayers[^1][^1].Add("Up");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.down)
                            {
                                enterplayers[^1][^1].Add("Down");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.right || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.right / 2)
                            {
                                enterplayers[^1][^1].Add("Right");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.left || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.left / 2)
                            {
                                enterplayers[^1][^1].Add("Left");
                            }
                            if (enterplayers[^1][^1].Count == 4)
                            {
                                var temp = enterplayers[^1][^1][0];
                                enterplayers[^1][^1][0] = enterplayers[^1][^1][2];
                                enterplayers[^1][^1][2] = temp;
                            }
                        }
                        Debug.Log("Right");
                    }
                    if (left.collider != null && left.collider.gameObject.CompareTag("Rigidplayer"))
                    {
                        var player = left.collider.gameObject;
                        enterplayers[^1].Add(new List<string>());
                        enterplayers[^1][^1].Add(left.collider.gameObject.name);
                        enterplayers[^1][^1].Add("Left");
                        playersToLevels.Add(player.transform.parent.gameObject, level.transform.position);
                        if (player.transform.parent.GetComponent<RigidplayerMovement>() != null && player == player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1)
                        {
                            enterplayers[^1][^1].Add(player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.name);
                            if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.up)
                            {
                                enterplayers[^1][^1].Add("Up");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.down)
                            {
                                enterplayers[^1][^1].Add("Down");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.right || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.right / 2)
                            {
                                enterplayers[^1][^1].Add("Right");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.left || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.left / 2)
                            {
                                enterplayers[^1][^1].Add("Left");
                            }
                        }
                        else if (player.transform.parent.GetComponent<RigidplayerMovement>() != null && player == player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2)
                        {
                            enterplayers[^1][^1].Add(player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.name);
                            if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.up)
                            {
                                enterplayers[^1][^1].Add("Up");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.down)
                            {
                                enterplayers[^1][^1].Add("Down");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.right || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.right / 2)
                            {
                                enterplayers[^1][^1].Add("Right");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.left || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.left / 2)
                            {
                                enterplayers[^1][^1].Add("Left");
                            }
                            if (enterplayers[^1][^1].Count == 4)
                            {
                                var temp = enterplayers[^1][^1][0];
                                enterplayers[^1][^1][0] = enterplayers[^1][^1][2];
                                enterplayers[^1][^1][2] = temp;
                            }
                        }
                        Debug.Log("Left");
                    }
                    if (Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z - 1, level.transform.position.z - 1) != null && Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z - 1, level.transform.position.z - 1).gameObject.CompareTag("Rigidplayer"))
                    {
                        var player = Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z - 1, level.transform.position.z - 1).gameObject;
                        enterplayers[^1].Add(new List<string>());
                        enterplayers[^1][^1].Add(Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z - 1, level.transform.position.z - 1).gameObject.name);
                        enterplayers[^1][^1].Add("Up3D");
                        playersToLevels.Add(player.transform.parent.gameObject, level.transform.position);
                        if (player.transform.parent.GetComponent<RigidplayerMovement>() != null && player == player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1)
                        {
                            enterplayers[^1][^1].Add(player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.name);
                            if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.up)
                            {
                                enterplayers[^1][^1].Add("Up");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.down)
                            {
                                enterplayers[^1][^1].Add("Down");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.right || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.right / 2)
                            {
                                enterplayers[^1][^1].Add("Right");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.left || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.left / 2)
                            {
                                enterplayers[^1][^1].Add("Left");
                            }
                        }
                        else if (player.transform.parent.GetComponent<RigidplayerMovement>() != null && player == player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2)
                        {
                            enterplayers[^1][^1].Add(player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.name);
                            if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.up)
                            {
                                enterplayers[^1][^1].Add("Up");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.down)
                            {
                                enterplayers[^1][^1].Add("Down");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.right || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.right / 2)
                            {
                                enterplayers[^1][^1].Add("Right");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.left || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.left / 2)
                            {
                                enterplayers[^1][^1].Add("Left");
                            }
                            if (enterplayers[^1][^1].Count == 4)
                            {
                                var temp = enterplayers[^1][^1][0];
                                enterplayers[^1][^1][0] = enterplayers[^1][^1][2];
                                enterplayers[^1][^1][2] = temp;
                            }
                        }
                        Debug.Log("Up3D");
                    }
                    if (Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z + 1, level.transform.position.z + 1) != null && Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z + 1, level.transform.position.z + 1).gameObject.CompareTag("Rigidplayer"))
                    {
                        var player = Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z + 1, level.transform.position.z + 1).gameObject;
                        enterplayers[^1].Add(new List<string>());
                        enterplayers[^1][^1].Add(Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z + 1, level.transform.position.z + 1).gameObject.name);
                        enterplayers[^1][^1].Add("Down3D");
                        playersToLevels.Add(player.transform.parent.gameObject, level.transform.position);
                        if (player.transform.parent.GetComponent<RigidplayerMovement>() != null && player == player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1)
                        {
                            enterplayers[^1][^1].Add(player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.name);
                            if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.up)
                            {
                                enterplayers[^1][^1].Add("Up");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.down)
                            {
                                enterplayers[^1][^1].Add("Down");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.right || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.right / 2)
                            {
                                enterplayers[^1][^1].Add("Right");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.left || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.left / 2)
                            {
                                enterplayers[^1][^1].Add("Left");
                            }
                        }
                        else if (player.transform.parent.GetComponent<RigidplayerMovement>() != null && player == player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2)
                        {
                            enterplayers[^1][^1].Add(player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.name);
                            if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.up)
                            {
                                enterplayers[^1][^1].Add("Up");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.down)
                            {
                                enterplayers[^1][^1].Add("Down");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.right || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.right / 2)
                            {
                                enterplayers[^1][^1].Add("Right");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.left || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.left / 2)
                            {
                                enterplayers[^1][^1].Add("Left");
                            }
                            if (enterplayers[^1][^1].Count == 4)
                            {
                                var temp = enterplayers[^1][^1][0];
                                enterplayers[^1][^1][0] = enterplayers[^1][^1][2];
                                enterplayers[^1][^1][2] = temp;
                            }
                        }
                        Debug.Log("Down3D");
                    }
                }
                else if (level.transform.localScale == new Vector3(1, -1, 1))
                {
                    RaycastHit2D up = Physics2D.Raycast(level.transform.position, Vector3.up, 1, blockingLayer, level.transform.position.z, level.transform.position.z);
                    RaycastHit2D right = Physics2D.Raycast(level.transform.position, Vector3.right, 0.5f, blockingLayer, level.transform.position.z, level.transform.position.z);
                    RaycastHit2D left = Physics2D.Raycast(level.transform.position, Vector3.left, 0.5f, blockingLayer, level.transform.position.z, level.transform.position.z);
                    if (up.collider != null && up.collider.gameObject.CompareTag("Player"))
                    {
                        enterplayers[^1].Add(new List<string>());
                        enterplayers[^1][^1].Add(up.collider.gameObject.name);
                        enterplayers[^1][^1].Add("Up");
                        playersToLevels.Add(up.collider.gameObject, level.transform.position);
                    }
                    if (right.collider != null && right.collider.gameObject.CompareTag("Player"))
                    {
                        enterplayers[^1].Add(new List<string>());
                        enterplayers[^1][^1].Add(right.collider.gameObject.name);
                        enterplayers[^1][^1].Add("Right");
                        playersToLevels.Add(right.collider.gameObject, level.transform.position);
                    }
                    if (left.collider != null && left.collider.gameObject.CompareTag("Player"))
                    {
                        enterplayers[^1].Add(new List<string>());
                        enterplayers[^1][^1].Add(left.collider.gameObject.name);
                        enterplayers[^1][^1].Add("Left");
                        playersToLevels.Add(left.collider.gameObject, level.transform.position);
                    }
                    if (Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z - 1, level.transform.position.z - 1) != null && Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z - 1, level.transform.position.z - 1).gameObject.CompareTag("Player"))
                    {
                        enterplayers[^1].Add(new List<string>());
                        enterplayers[^1][^1].Add(Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z - 1, level.transform.position.z - 1).gameObject.name);
                        enterplayers[^1][^1].Add("Up3D");
                        playersToLevels.Add(Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z - 1, level.transform.position.z - 1).gameObject, level.transform.position);
                        Debug.Log("Up3D");
                    }
                    if (Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z + 1, level.transform.position.z + 1) != null && Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z + 1, level.transform.position.z + 1).gameObject.CompareTag("Player"))
                    {
                        enterplayers[^1].Add(new List<string>());
                        enterplayers[^1][^1].Add(Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z + 1, level.transform.position.z + 1).gameObject.name);
                        enterplayers[^1][^1].Add("Down3D");
                        playersToLevels.Add(Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z + 1, level.transform.position.z + 1).gameObject, level.transform.position);
                        Debug.Log("Down3D");
                    }
                    if (up.collider != null && up.collider.gameObject.CompareTag("Rigidplayer"))
                    {
                        var player = up.collider.gameObject;
                        enterplayers[^1].Add(new List<string>());
                        enterplayers[^1][^1].Add(player.name);
                        enterplayers[^1][^1].Add("Up");
                        playersToLevels.Add(player.transform.parent.gameObject, level.transform.position);
                        if (player.transform.parent.GetComponent<RigidplayerMovement>() != null && player == player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1)
                        {
                            enterplayers[^1][^1].Add(player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.name);
                            if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.up)
                            {
                                enterplayers[^1][^1].Add("Up");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.down)
                            {
                                enterplayers[^1][^1].Add("Down");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.right || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.right / 2)
                            {
                                enterplayers[^1][^1].Add("Right");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.left || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.left / 2)
                            {
                                enterplayers[^1][^1].Add("Left");
                            }
                        }
                        else if (player.transform.parent.GetComponent<RigidplayerMovement>() != null && player == player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2)
                        {
                            enterplayers[^1][^1].Add(player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.name);
                            if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.up)
                            {
                                enterplayers[^1][^1].Add("Up");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.down)
                            {
                                enterplayers[^1][^1].Add("Down");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.right || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.right / 2)
                            {
                                enterplayers[^1][^1].Add("Right");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.left || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.left / 2)
                            {
                                enterplayers[^1][^1].Add("Left");
                            }
                            if (enterplayers[^1][^1].Count == 4)
                            {
                                var temp = enterplayers[^1][^1][0];
                                enterplayers[^1][^1][0] = enterplayers[^1][^1][2];
                                enterplayers[^1][^1][2] = temp;
                            }
                        }
                        Debug.Log("Up");
                    }
                    if (right.collider != null && right.collider.gameObject.CompareTag("Rigidplayer"))
                    {
                        var player = right.collider.gameObject;
                        enterplayers[^1].Add(new List<string>());
                        enterplayers[^1][^1].Add(right.collider.gameObject.name);
                        enterplayers[^1][^1].Add("Right");
                        playersToLevels.Add(player.transform.parent.gameObject, level.transform.position);
                        if (player.transform.parent.GetComponent<RigidplayerMovement>() != null && player == player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1)
                        {
                            enterplayers[^1][^1].Add(player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.name);
                            if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.up)
                            {
                                enterplayers[^1][^1].Add("Up");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.down)
                            {
                                enterplayers[^1][^1].Add("Down");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.right || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.right / 2)
                            {
                                enterplayers[^1][^1].Add("Right");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.left || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.left / 2)
                            {
                                enterplayers[^1][^1].Add("Left");
                            }
                        }
                        else if (player.transform.parent.GetComponent<RigidplayerMovement>() != null && player == player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2)
                        {
                            enterplayers[^1][^1].Add(player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.name);
                            if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.up)
                            {
                                enterplayers[^1][^1].Add("Up");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.down)
                            {
                                enterplayers[^1][^1].Add("Down");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.right || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.right / 2)
                            {
                                enterplayers[^1][^1].Add("Right");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.left || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.left / 2)
                            {
                                enterplayers[^1][^1].Add("Left");
                            }
                            if (enterplayers[^1][^1].Count == 4)
                            {
                                var temp = enterplayers[^1][^1][0];
                                enterplayers[^1][^1][0] = enterplayers[^1][^1][2];
                                enterplayers[^1][^1][2] = temp;
                            }
                        }
                        Debug.Log("Right");
                    }
                    if (left.collider != null && left.collider.gameObject.CompareTag("Rigidplayer"))
                    {
                        var player = left.collider.gameObject;
                        enterplayers[^1].Add(new List<string>());
                        enterplayers[^1][^1].Add(left.collider.gameObject.name);
                        enterplayers[^1][^1].Add("Left");
                        playersToLevels.Add(player.transform.parent.gameObject, level.transform.position);
                        if (player.transform.parent.GetComponent<RigidplayerMovement>() != null && player == player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1)
                        {
                            enterplayers[^1][^1].Add(player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.name);
                            if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.up)
                            {
                                enterplayers[^1][^1].Add("Up");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.down)
                            {
                                enterplayers[^1][^1].Add("Down");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.right || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.right / 2)
                            {
                                enterplayers[^1][^1].Add("Right");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.left || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.left / 2)
                            {
                                enterplayers[^1][^1].Add("Left");
                            }
                        }
                        else if (player.transform.parent.GetComponent<RigidplayerMovement>() != null && player == player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2)
                        {
                            enterplayers[^1][^1].Add(player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.name);
                            if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.up)
                            {
                                enterplayers[^1][^1].Add("Up");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.down)
                            {
                                enterplayers[^1][^1].Add("Down");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.right || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.right / 2)
                            {
                                enterplayers[^1][^1].Add("Right");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.left || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.left / 2)
                            {
                                enterplayers[^1][^1].Add("Left");
                            }
                            if (enterplayers[^1][^1].Count == 4)
                            {
                                var temp = enterplayers[^1][^1][0];
                                enterplayers[^1][^1][0] = enterplayers[^1][^1][2];
                                enterplayers[^1][^1][2] = temp;
                            }
                        }
                        Debug.Log("Left");
                    }
                    if (Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z - 1, level.transform.position.z - 1) != null && Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z - 1, level.transform.position.z - 1).gameObject.CompareTag("Rigidplayer"))
                    {
                        var player = Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z - 1, level.transform.position.z - 1).gameObject;
                        enterplayers[^1].Add(new List<string>());
                        enterplayers[^1][^1].Add(Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z - 1, level.transform.position.z - 1).gameObject.name);
                        enterplayers[^1][^1].Add("Up3D");
                        playersToLevels.Add(player.transform.parent.gameObject, level.transform.position);
                        if (player.transform.parent.GetComponent<RigidplayerMovement>() != null && player == player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1)
                        {
                            enterplayers[^1][^1].Add(player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.name);
                            if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.up)
                            {
                                enterplayers[^1][^1].Add("Up");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.down)
                            {
                                enterplayers[^1][^1].Add("Down");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.right || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.right / 2)
                            {
                                enterplayers[^1][^1].Add("Right");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.left || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.left / 2)
                            {
                                enterplayers[^1][^1].Add("Left");
                            }
                        }
                        else if (player.transform.parent.GetComponent<RigidplayerMovement>() != null && player == player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2)
                        {
                            enterplayers[^1][^1].Add(player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.name);
                            if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.up)
                            {
                                enterplayers[^1][^1].Add("Up");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.down)
                            {
                                enterplayers[^1][^1].Add("Down");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.right || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.right / 2)
                            {
                                enterplayers[^1][^1].Add("Right");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.left || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.left / 2)
                            {
                                enterplayers[^1][^1].Add("Left");
                            }
                            if (enterplayers[^1][^1].Count == 4)
                            {
                                var temp = enterplayers[^1][^1][0];
                                enterplayers[^1][^1][0] = enterplayers[^1][^1][2];
                                enterplayers[^1][^1][2] = temp;
                            }
                        }
                        Debug.Log("Up3D");
                    }
                    if (Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z + 1, level.transform.position.z + 1) != null && Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z + 1, level.transform.position.z + 1).gameObject.CompareTag("Rigidplayer"))
                    {
                        var player = Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z + 1, level.transform.position.z + 1).gameObject;
                        enterplayers[^1].Add(new List<string>());
                        enterplayers[^1][^1].Add(Physics2D.OverlapPoint(level.transform.position, blockingLayer, level.transform.position.z + 1, level.transform.position.z + 1).gameObject.name);
                        enterplayers[^1][^1].Add("Down3D");
                        playersToLevels.Add(player.transform.parent.gameObject, level.transform.position);
                        if (player.transform.parent.GetComponent<RigidplayerMovement>() != null && player == player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1)
                        {
                            enterplayers[^1][^1].Add(player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.name);
                            if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.up)
                            {
                                enterplayers[^1][^1].Add("Up");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.down)
                            {
                                enterplayers[^1][^1].Add("Down");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.right || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.right / 2)
                            {
                                enterplayers[^1][^1].Add("Right");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.left || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position - player.transform.position == Vector3.left / 2)
                            {
                                enterplayers[^1][^1].Add("Left");
                            }
                        }
                        else if (player.transform.parent.GetComponent<RigidplayerMovement>() != null && player == player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer2)
                        {
                            enterplayers[^1][^1].Add(player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.name);
                            if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.up)
                            {
                                enterplayers[^1][^1].Add("Up");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.down)
                            {
                                enterplayers[^1][^1].Add("Down");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.right || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.right / 2)
                            {
                                enterplayers[^1][^1].Add("Right");
                            }
                            else if (player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.left || player.transform.parent.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position - player.transform.position == Vector3.left / 2)
                            {
                                enterplayers[^1][^1].Add("Left");
                            }
                            if (enterplayers[^1][^1].Count == 4)
                            {
                                var temp = enterplayers[^1][^1][0];
                                enterplayers[^1][^1][0] = enterplayers[^1][^1][2];
                                enterplayers[^1][^1][2] = temp;
                            }
                        }
                        Debug.Log("Down3D");
                    }
                }
                if (level.shortcut)
                {
                    var levelparent = enterboxes[^1];
                    var num = scenes.Count - 1;
                    var num2 = enterboxes.Count - 1;
                    while (parentscenes[levelparent] != SceneManager.GetActiveScene().name)
                    {
                        levelparent = parentscenes[levelparent];
                        scenes.Insert(num2, parentscenes[levelparent]);
                        enterboxes.Insert(num2, levelparent);
                        enterplayers.Add(new List<List<String>>(enterplayers[^1]));
                    }
                }
            }
        }
        foreach (var list in enterplayers)
        {
            Debug.Log("list");
            foreach (var list_ in list)
            {
                Debug.Log("list_");
                foreach(var str in list_)
                {
                    Debug.Log(str);
                }
            }
        }
        
        if (indexes.Count != 0) 
        {
            foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (player.GetComponent<PlayerMovement>() != null && playersToLevels.ContainsKey(player))
                {
                    isMoving = true;
                    Debug.Log("Animation starting");
                    player.GetComponent<PlayerMovement>().StartCoroutine(player.GetComponent<PlayerMovement>().MoveToPosition(playersToLevels[player], 6 * Vector3.Distance(player.transform.position, playersToLevels[player]), playersToLevels[player], 6 * Vector3.Distance(player.transform.position, playersToLevels[player]), false, true, false));
                }
            }
            foreach (var rigidplayer in GameObject.FindObjectsByType<RigidplayerMovement>(sortMode: FindObjectsSortMode.None))
            {
                if (rigidplayer.GetComponent<RigidplayerMovement>() != null && playersToLevels.ContainsKey(rigidplayer.gameObject))
                {
                    isMoving = true;
                    Debug.Log("Animation starting");
                    rigidplayer.GetComponent<RigidplayerMovement>().StartCoroutine(rigidplayer.GetComponent<RigidplayerMovement>().MoveToPosition(playersToLevels[rigidplayer.gameObject], 6 * Vector3.Distance(rigidplayer.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position, playersToLevels[rigidplayer.gameObject]), playersToLevels[rigidplayer.gameObject], 6 * Vector3.Distance(rigidplayer.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position, playersToLevels[rigidplayer.gameObject]), false, true, false));
                }
            }
            StartCoroutine(LevelTransition(indexes));
        }
        Debug.Log(enterplayers.Count);
        Debug.Log(enterboxes.Count);
        var str_ = string.Empty;
        foreach (var box in enterboxes)
        {
            str_ += box;
            str_ += ", ";
        }
        Debug.Log(str_);
    }
    private IEnumerator LevelTransition(List<int> levelindexes)
    {
        yield return null;
        yield return null;
        yield return new WaitUntil(() => !isMoving);
        Debug.Log(isMoving);
        Debug.Log("Scene loaded");
        SceneManager.LoadScene(levelindexes.Min());
        dontSave = false;
        first = true;
        Debug.Log("Scene loaded");
    }
    public void ExitLevel()
    {
        if (scenes.Count != 0)
        {
            Debug.Log(enterplayers.Count);
            transitions.Add("Exit");
            SceneManager.LoadScene(scenes[^1]);
            exitscenes.Add(scenes[^1]);
            scenes.RemoveAt(scenes.Count - 1);
            exit = true;
        }
    }
    public static void CheckWin()
    {
        var boxes_on_goals = 0;
        var rigidboxes_on_goals = 0;
        var players_on_goals = 0;
        var players_ = GameObject.FindGameObjectsWithTag("Player");
        var boxes = GameObject.FindGameObjectsWithTag("Box");
        var rigidboxes = GameObject.FindGameObjectsWithTag("Rigidbox");
        var boxtargets = GameObject.FindGameObjectsWithTag("Box target");
        var playertargets = GameObject.FindGameObjectsWithTag("Player target");
        if (boxtargets.Length == 0 && playertargets.Length == 0)
        {
            return;
        }
        foreach (GameObject box in boxes)
        {
            var goal = Physics2D.OverlapPoint(box.transform.position, Goal, box.transform.position.z, box.transform.position.z);
            if (goal != null)
            {
                boxes_on_goals += 1;
            }
        }
        foreach (GameObject rigidbox in rigidboxes)
        {
            if (rigidbox.transform.parent != null)
            {
                if (Physics2D.OverlapPoint(rigidbox.transform.position, Goal, rigidbox.transform.position.z, rigidbox.transform.position.z) != null)
                {
                    rigidboxes_on_goals += 1;
                }
            }
        }
        foreach (GameObject player in players_)
        {   
            var goal = Physics2D.OverlapPoint(player.transform.position, Player_goal, player.transform.position.z, player.transform.position.z);
            if (goal != null)
            {
                players_on_goals += 1;
            }
        }
        if (boxes_on_goals + rigidboxes_on_goals == boxtargets.Length && players_on_goals == playertargets.Length)
        {
            won = true;
        }
    }



    public static void ColorCheck()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        objects = new();
        foreach (GameObject player_ in players)
        {
            objects.Add(player_);
        }
        var rigidplayers = GameObject.FindGameObjectsWithTag("Rigidplayer").ToList();
        var rigidplayers_ = new List<GameObject>();
        foreach (var rigidplayer in rigidplayers)
        {
            if (rigidplayer.transform.parent != null)
            {
                rigidplayers_.Add(rigidplayer);
            }
        }
        levels = new();
        indexes = new();
        foreach (GameObject player in objects)
        {
            if (player.transform.position.y < GridChange)
            {
                RaycastHit2D up = Physics2D.Raycast(player.transform.position, Vector3.up, 1, blockingLayer, player.transform.position.z, player.transform.position.z);
                RaycastHit2D down = Physics2D.Raycast(player.transform.position, Vector3.down, 1, blockingLayer, player.transform.position.z, player.transform.position.z);
                RaycastHit2D right = Physics2D.Raycast(player.transform.position, Vector3.right, 1, blockingLayer, player.transform.position.z, player.transform.position.z);
                RaycastHit2D left = Physics2D.Raycast(player.transform.position, Vector3.left, 1, blockingLayer, player.transform.position.z, player.transform.position.z);
                if (up.collider != null && up.collider.gameObject.GetComponent<LevelScript>() != null && (!up.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[up.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(up.collider.gameObject.GetComponent<LevelScript>());
                }
                if (down.collider != null && down.collider.gameObject.GetComponent<LevelScript>() != null && (!down.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[down.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(down.collider.gameObject.GetComponent<LevelScript>());
                }
                if (right.collider != null && right.collider.gameObject.GetComponent<LevelScript>() != null && (!right.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[right.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(right.collider.gameObject.GetComponent<LevelScript>());
                }
                if (left.collider != null && left.collider.gameObject.GetComponent<LevelScript>() != null && (!left.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[left.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(left.collider.gameObject.GetComponent<LevelScript>());
                }
                if (Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z - 1, player.transform.position.z - 1) != null && Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z - 1, player.transform.position.z - 1).gameObject.GetComponent<LevelScript>() != null && (!Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z - 1, player.transform.position.z - 1).gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z - 1, player.transform.position.z - 1).gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z - 1, player.transform.position.z - 1).gameObject.GetComponent<LevelScript>());
                }
                if (Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z + 1, player.transform.position.z + 1) != null && Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z + 1, player.transform.position.z + 1).gameObject.GetComponent<LevelScript>() != null && (!Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z + 1, player.transform.position.z + 1).gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z + 1, player.transform.position.z + 1).gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z + 1, player.transform.position.z + 1).gameObject.GetComponent<LevelScript>());
                }

            }
            else if (player.transform.localScale == Vector3.one)
            {
                RaycastHit2D down = Physics2D.Raycast(player.transform.position, Vector3.down, 1, blockingLayer, player.transform.position.z, player.transform.position.z);
                RaycastHit2D right = Physics2D.Raycast(player.transform.position, Vector3.right, 0.5f, blockingLayer, player.transform.position.z, player.transform.position.z);
                RaycastHit2D left = Physics2D.Raycast(player.transform.position, Vector3.left, 0.5f, blockingLayer, player.transform.position.z, player.transform.position.z);
                if (down.collider != null && down.collider.gameObject.GetComponent<LevelScript>() != null && (!down.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[down.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(down.collider.gameObject.GetComponent<LevelScript>());
                }
                if (right.collider != null && right.collider.gameObject.GetComponent<LevelScript>() != null && (!right.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[right.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(right.collider.gameObject.GetComponent<LevelScript>());
                }
                if (left.collider != null && left.collider.gameObject.GetComponent<LevelScript>() != null && (!left.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[left.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(left.collider.gameObject.GetComponent<LevelScript>());
                }
                if (Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z - 1, player.transform.position.z - 1) != null && Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z - 1, player.transform.position.z - 1).gameObject.GetComponent<LevelScript>() != null && (!Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z - 1, player.transform.position.z - 1).gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z - 1, player.transform.position.z - 1).gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z - 1, player.transform.position.z - 1).gameObject.GetComponent<LevelScript>());
                }
                if (Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z + 1, player.transform.position.z + 1) != null && Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z + 1, player.transform.position.z + 1).gameObject.GetComponent<LevelScript>() != null && (!Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z + 1, player.transform.position.z + 1).gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z + 1, player.transform.position.z + 1).gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z + 1, player.transform.position.z + 1).gameObject.GetComponent<LevelScript>());
                }
            }
            else if (player.transform.localScale == new Vector3(1, -1, 1))
            {
                RaycastHit2D up = Physics2D.Raycast(player.transform.position, Vector3.up, 1, blockingLayer, player.transform.position.z, player.transform.position.z);
                RaycastHit2D right = Physics2D.Raycast(player.transform.position, Vector3.right, 0.5f, blockingLayer, player.transform.position.z, player.transform.position.z);
                RaycastHit2D left = Physics2D.Raycast(player.transform.position, Vector3.left, 0.5f, blockingLayer, player.transform.position.z, player.transform.position.z);
                if (up.collider != null && up.collider.gameObject.GetComponent<LevelScript>() != null && (!up.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[up.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(up.collider.gameObject.GetComponent<LevelScript>());
                }
                if (right.collider != null && right.collider.gameObject.GetComponent<LevelScript>() != null && (!right.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[right.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(right.collider.gameObject.GetComponent<LevelScript>());
                }
                if (left.collider != null && left.collider.gameObject.GetComponent<LevelScript>() != null && (!left.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[left.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(left.collider.gameObject.GetComponent<LevelScript>());
                }
                if (Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z - 1, player.transform.position.z - 1) != null && Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z - 1, player.transform.position.z - 1).gameObject.GetComponent<LevelScript>() != null && (!Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z - 1, player.transform.position.z - 1).gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z - 1, player.transform.position.z - 1).gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z - 1, player.transform.position.z - 1).gameObject.GetComponent<LevelScript>());
                }
                if (Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z + 1, player.transform.position.z + 1) != null && Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z + 1, player.transform.position.z + 1).gameObject.GetComponent<LevelScript>() != null && (!Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z + 1, player.transform.position.z + 1).gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z + 1, player.transform.position.z + 1).gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(Physics2D.OverlapPoint(player.transform.position, blockingLayer, player.transform.position.z + 1, player.transform.position.z + 1).gameObject.GetComponent<LevelScript>());
                }
            }
        }
        foreach (GameObject rigidplayer in rigidplayers_)
        {
            Debug.Log(rigidplayer.name);
            if (rigidplayer.transform.position.y < GridChange)
            {
                RaycastHit2D up = Physics2D.Raycast(rigidplayer.transform.position, Vector3.up, 1, blockingLayer, rigidplayer.transform.position.z, rigidplayer.transform.position.z);
                RaycastHit2D down = Physics2D.Raycast(rigidplayer.transform.position, Vector3.down, 1, blockingLayer, rigidplayer.transform.position.z, rigidplayer.transform.position.z);
                RaycastHit2D right = Physics2D.Raycast(rigidplayer.transform.position, Vector3.right, 1, blockingLayer, rigidplayer.transform.position.z, rigidplayer.transform.position.z);
                RaycastHit2D left = Physics2D.Raycast(rigidplayer.transform.position, Vector3.left, 1, blockingLayer, rigidplayer.transform.position.z, rigidplayer.transform.position.z);
                if (up.collider != null && up.collider.gameObject.GetComponent<LevelScript>() != null && (!up.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[up.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(up.collider.gameObject.GetComponent<LevelScript>());
                }
                if (down.collider != null && down.collider.gameObject.GetComponent<LevelScript>() != null && (!down.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[down.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(down.collider.gameObject.GetComponent<LevelScript>());
                }
                if (right.collider != null && right.collider.gameObject.GetComponent<LevelScript>() != null && (!right.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[right.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(right.collider.gameObject.GetComponent<LevelScript>());
                }
                if (left.collider != null && left.collider.gameObject.GetComponent<LevelScript>() != null && (!left.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[left.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(left.collider.gameObject.GetComponent<LevelScript>());
                }
                if (Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z - 1, rigidplayer.transform.position.z - 1) != null && Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z - 1, rigidplayer.transform.position.z - 1).gameObject.GetComponent<LevelScript>() != null && (!Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z - 1, rigidplayer.transform.position.z - 1).gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z - 1, rigidplayer.transform.position.z - 1).gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z - 1, rigidplayer.transform.position.z - 1).gameObject.GetComponent<LevelScript>());
                }
                if (Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z + 1, rigidplayer.transform.position.z + 1) != null && Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z + 1, rigidplayer.transform.position.z + 1).gameObject.GetComponent<LevelScript>() != null && (!Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z + 1, rigidplayer.transform.position.z + 1).gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z + 1, rigidplayer.transform.position.z + 1).gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z + 1, rigidplayer.transform.position.z + 1).gameObject.GetComponent<LevelScript>());
                }

            }
            else if (rigidplayer.transform.localScale == Vector3.one)
            {
                RaycastHit2D down = Physics2D.Raycast(rigidplayer.transform.position, Vector3.down, 1, blockingLayer, rigidplayer.transform.position.z, rigidplayer.transform.position.z);
                RaycastHit2D right = Physics2D.Raycast(rigidplayer.transform.position, Vector3.right, 0.5f, blockingLayer, rigidplayer.transform.position.z, rigidplayer.transform.position.z);
                RaycastHit2D left = Physics2D.Raycast(rigidplayer.transform.position, Vector3.left, 0.5f, blockingLayer, rigidplayer.transform.position.z, rigidplayer.transform.position.z);
                if (down.collider != null && down.collider.gameObject.GetComponent<LevelScript>() != null && (!down.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[down.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(down.collider.gameObject.GetComponent<LevelScript>());
                }
                if (right.collider != null && right.collider.gameObject.GetComponent<LevelScript>() != null && (!right.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[right.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(right.collider.gameObject.GetComponent<LevelScript>());
                }
                if (left.collider != null && left.collider.gameObject.GetComponent<LevelScript>() != null && (!left.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[left.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(left.collider.gameObject.GetComponent<LevelScript>());
                }
                if (Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z - 1, rigidplayer.transform.position.z - 1) != null && Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z - 1, rigidplayer.transform.position.z - 1).gameObject.GetComponent<LevelScript>() != null && (!Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z - 1, rigidplayer.transform.position.z - 1).gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z - 1, rigidplayer.transform.position.z - 1).gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z - 1, rigidplayer.transform.position.z - 1).gameObject.GetComponent<LevelScript>());
                }
                if (Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z + 1, rigidplayer.transform.position.z + 1) != null && Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z + 1, rigidplayer.transform.position.z + 1).gameObject.GetComponent<LevelScript>() != null && (!Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z + 1, rigidplayer.transform.position.z + 1).gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z + 1, rigidplayer.transform.position.z + 1).gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z + 1, rigidplayer.transform.position.z + 1).gameObject.GetComponent<LevelScript>());
                }
            }
            else if (rigidplayer.transform.localScale == new Vector3(1, -1, 1))
            {
                RaycastHit2D up = Physics2D.Raycast(rigidplayer.transform.position, Vector3.up, 1, blockingLayer, rigidplayer.transform.position.z, rigidplayer.transform.position.z);
                RaycastHit2D right = Physics2D.Raycast(rigidplayer.transform.position, Vector3.right, 0.5f, blockingLayer, rigidplayer.transform.position.z, rigidplayer.transform.position.z);
                RaycastHit2D left = Physics2D.Raycast(rigidplayer.transform.position, Vector3.left, 0.5f, blockingLayer, rigidplayer.transform.position.z, rigidplayer.transform.position.z);
                if (up.collider != null && up.collider.gameObject.GetComponent<LevelScript>() != null && (!up.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[up.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(up.collider.gameObject.GetComponent<LevelScript>());
                }
                if (right.collider != null && right.collider.gameObject.GetComponent<LevelScript>() != null && (!right.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[right.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(right.collider.gameObject.GetComponent<LevelScript>());
                }
                if (left.collider != null && left.collider.gameObject.GetComponent<LevelScript>() != null && (!left.collider.gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[left.collider.gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(left.collider.gameObject.GetComponent<LevelScript>());
                }
                if (Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z - 1, rigidplayer.transform.position.z - 1) != null && Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z - 1, rigidplayer.transform.position.z - 1).gameObject.GetComponent<LevelScript>() != null && (!Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z - 1, rigidplayer.transform.position.z - 1).gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z - 1, rigidplayer.transform.position.z - 1).gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z - 1, rigidplayer.transform.position.z - 1).gameObject.GetComponent<LevelScript>());
                }
                if (Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z + 1, rigidplayer.transform.position.z + 1) != null && Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z + 1, rigidplayer.transform.position.z + 1).gameObject.GetComponent<LevelScript>() != null && (!Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z + 1, rigidplayer.transform.position.z + 1).gameObject.GetComponent<LevelScript>().shortcut || LevelsWon[Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z + 1, rigidplayer.transform.position.z + 1).gameObject.GetComponent<LevelScript>().SceneName]))
                {
                    levels.Add(Physics2D.OverlapPoint(rigidplayer.transform.position, blockingLayer, rigidplayer.transform.position.z + 1, rigidplayer.transform.position.z + 1).gameObject.GetComponent<LevelScript>());
                }
            }
        }
        foreach (var level in levels)
        {
            if (level.SceneBuildIndex(level.SceneName) != -1)
            {
                indexes.Add(level.SceneBuildIndex(level.SceneName));
            }
        }
        foreach (var level in levels)
        {
            if (level.SceneBuildIndex(level.SceneName) == indexes.Min())
            {
                level.GetComponentInChildren<TMP_Text>().color = new Color32(242, 103, 75, 255);
            }
            else if (LevelsWon[level.SceneName])
            {
                level.GetComponentInChildren<TMP_Text>().color = new Color32(90, 165, 240, 255);
            }
            else if(level.GetComponent<LevelScript>().shortcut && !LevelsWon[level.GetComponent<LevelScript>().SceneName])
            {
                level.GetComponentInChildren<TMP_Text>().color = Color.gray;
            }
            else
            {
                level.GetComponentInChildren<TMP_Text>().color = Color.black;
            }
        }
        foreach(var level in GameObject.FindObjectsByType<LevelScript>(FindObjectsSortMode.None))
        {
            if (!levels.Contains(level))
            {
                if (LevelsWon.ContainsKey(level.SceneName) && LevelsWon[level.SceneName])
                {
                    level.GetComponentInChildren<TMP_Text>().color = new Color32(90, 165, 240, 255);
                }
                else if (level.GetComponent<LevelScript>().shortcut && !LevelsWon[level.GetComponent<LevelScript>().SceneName])
                {
                    level.GetComponentInChildren<TMP_Text>().color = Color.gray;
                }
                else
                {
                    level.GetComponentInChildren<TMP_Text>().color = Color.black;
                }
            }
        }
    }
    public void LoadData(GameData data)
    {
        LevelsWon = data.LevelsCompleted;
        LevelsCompleted = data.LevelsCompleted;
    }
    public void SaveData(ref GameData data)
    {
        data.LevelsCompleted = this.LevelsCompleted;
    }
    public static Vector3 Movement(GameObject obj, string key)
    {
        var movement = Vector3.zero;
        var push = Vector3.zero;
        if (obj.transform.position.y < GridChange)
        {
            if (key == "W")
            {
                push = Vector3.up;
                if (obj.transform.position.y >= GridChange - 1)
                {
                    key = "W1";
                    movement = Vector3.left / 2;
                }
                else
                {
                    movement = Vector3.up;
                }
            }
            if (key == "A1")
            {
                key = "A";
            }
            if (key == "A")
            {
                movement = Vector3.left;
                push = movement;
            }
            if (key == "X1" || key == "Z1")
            {
                key = "S";
            }
            if (key == "S")
            {
                movement = Vector3.down;
                push = movement;
            }
            if (key == "D1")
            {
                key = "D";
            }
            if (key == "D")
            {
                movement = Vector3.right;
                push = movement;
            }
        }
        else
        {
            if (key == "W")
            {
                key = "W1";
            }
            if (key == "W1")
            {
                movement = Vector3.left / 2;
                push = Vector3.up;
            }
            if (key == "E1")
            {
                movement = Vector3.right / 2;
                push = Vector3.up;
            }
            if (key == "A")
            {
                key = "A1";
            }
            if (key == "A1")
            {
                movement = Vector3.left / 2;
                push = Vector3.left / 2;
            }
            if (key == "D")
            {
                key = "D1";
            }
            if (key == "D1")
            {
                movement = Vector3.right / 2;
                push = Vector3.right / 2;
            }
            if (key == "Z1")
            {
                movement = Vector3.down;
                push = Vector3.left / 2;
            }
            if (key == "X1")
            {
                movement = Vector3.down;
                push = Vector3.right / 2;
            }
            if (Mathf.Abs(obj.transform.position.x) % 1 == ((obj.transform.position.y - GridChange) % 2) / 2)
            {
                if (obj.transform.position.y <= GridChange)
                {
                    if (key == "Z1" || key == "X1")
                    {
                        key = "S";
                        push = Vector3.down;
                    }
                }
            }
            if (Mathf.Abs(obj.transform.position.x) % 1 == ((obj.transform.position.y - GridChange) % 2) / 2)
            {
                movement += push;
                push = movement - push;
                movement -= push;
            }
            if (obj.transform.position.y == GridChange && push == Vector3.down)
            {
                movement = Vector3.down;
            }
        }
        return movement;
    }
    public static Vector3 Push(GameObject obj, string key)
    {
        var movement = Vector3.zero;
        var push = Vector3.zero;
        if (obj.transform.position.y < GridChange)
        {
            if (key == "W")
            {
                push = Vector3.up;
                if (obj.transform.position.y >= GridChange - 1)
                {
                    key = "W1";
                    movement = Vector3.left / 2;
                }
                else
                {
                    movement = Vector3.up;
                }
            }
            if (key == "A1")
            {
                key = "A";
            }
            if (key == "A")
            {
                movement = Vector3.left;
                push = movement;
            }
            if (key == "X1" || key == "Z1")
            {
                key = "S";
            }
            if (key == "S")
            {
                movement = Vector3.down;
                push = movement;
            }
            if (key == "D1")
            {
                key = "D";
            }
            if (key == "D")
            {
                movement = Vector3.right;
                push = movement;
            }
        }
        else
        {
            if (key == "W")
            {
                key = "W1";
            }
            if (key == "W1")
            {
                movement = Vector3.left / 2;
                push = Vector3.up;
            }
            if (key == "E1")
            {
                movement = Vector3.right / 2;
                push = Vector3.up;
            }
            if (key == "A")
            {
                key = "A1";
            }
            if (key == "A1")
            {
                movement = Vector3.left / 2;
                push = Vector3.left / 2;
            }
            if (key == "D")
            {
                key = "D1";
            }
            if (key == "D1")
            {
                movement = Vector3.right / 2;
                push = Vector3.right / 2;
            }
            if (key == "Z1")
            {
                movement = Vector3.down;
                push = Vector3.left / 2;
            }
            if (key == "X1")
            {
                movement = Vector3.down;
                push = Vector3.right / 2;
            }
            if (Mathf.Abs(obj.transform.position.x) % 1 == ((obj.transform.position.y - GridChange) % 2) / 2)
            {
                if (obj.transform.position.y <= GridChange)
                {
                    if (key == "Z1" || key == "X1")
                    {
                        key = "S";
                        push = Vector3.down;
                    }
                }
            }
            if (Mathf.Abs(obj.transform.position.x) % 1 == ((obj.transform.position.y - GridChange) % 2) / 2)
            {
                movement += push;
                push = movement - push;
                movement -= push;
            }
            if (obj.transform.position.y == GridChange && push == Vector3.down)
            {
                movement = Vector3.down;
            }
        }
        return push;
    }
}
