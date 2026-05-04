using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Rendering.FilterWindow;
[Serializable]
public class GameState
{
    #region dataToSave
    public List<Vector3> playerPos;
    public List<Quaternion> playerRotation;
    public List<Vector3> playerCameraPos;
    public List<Dictionary<Vector3, List<string>>> boxPos;
    public List<Quaternion> boxRotation;
    public List<string> boxNames;
    public List<Dictionary<Vector3, List<string>>> rigidboxPos1;
    public List<Dictionary<Vector3, List<string>>> rigidboxPos2;
    public List<List<string>> rigidboxNames;
    public List<Dictionary<Vector3, List<string>>> rigidplayerPos1;
    public List<Dictionary<Vector3, List<string>>> rigidplayerPos2;
    public List<List<string>> rigidplayerNames;
    public List<Vector3> rigidplayerCameraPos1;
    public List<Vector3> rigidplayerCameraPos2;
    public List<string> portalOrientation;
    public Vector3 camerapos;

    public string SceneName;
    public GameObject rigidbox;
    public bool won;
    #endregion

    public GameObject MainCamera;

    public static GameState GetCurrentState()
    {
        GameState gameStateToSave = new GameState();
        SavedElement[] elementsToSaveOnScene = GameObject.FindObjectsByType<SavedElement>(FindObjectsSortMode.None);
        elementsToSaveOnScene = elementsToSaveOnScene.OrderBy(go => go.gameObject.name).ToArray();
        gameStateToSave.playerPos = new List<Vector3>();
        gameStateToSave.playerRotation = new List<Quaternion>();
        gameStateToSave.playerCameraPos = new List<Vector3>();
        gameStateToSave.boxPos = new List<Dictionary<Vector3, List<string>>>();
        gameStateToSave.boxNames = new List<string>();
        gameStateToSave.boxRotation = new List<Quaternion>();
        gameStateToSave.rigidboxPos1 = new List<Dictionary<Vector3, List<string>>>();
        gameStateToSave.rigidboxPos2 = new List<Dictionary<Vector3, List<string>>>();
        gameStateToSave.rigidboxNames = new List<List<string>>();
        gameStateToSave.rigidplayerPos1 = new List<Dictionary<Vector3, List<string>>>();
        gameStateToSave.rigidplayerPos2 = new List<Dictionary<Vector3, List<string>>>();
        gameStateToSave.rigidplayerNames = new List<List<string>>();
        gameStateToSave.rigidplayerCameraPos1 = new List<Vector3>();
        gameStateToSave.rigidplayerCameraPos2 = new List<Vector3>();
        gameStateToSave.portalOrientation = new List<string>();
        gameStateToSave.camerapos = GameObject.FindFirstObjectByType<CameraMovement>().transform.position;
        gameStateToSave.SceneName = SceneManager.GetActiveScene().name;
        gameStateToSave.won = GameManager.won;
        Debug.Log(gameStateToSave.won);
        foreach (SavedElement element in elementsToSaveOnScene)
        {
            if (element.type == SavedElement.Type.Player)
            {
                gameStateToSave.playerPos.Add(element.transform.position);
                gameStateToSave.playerRotation.Add(element.transform.rotation);
                gameStateToSave.playerCameraPos.Add(element.GetComponent<PlayerMovement>().camerapos);
            }
            else if (element.type == SavedElement.Type.Box)
            {
                gameStateToSave.boxNames.Add(element.transform.name);
                gameStateToSave.boxPos.Add(new Dictionary<Vector3, List<string>>());
                gameStateToSave.boxPos[^1].Add(element.transform.position, new List<string>());
                if (element.GetComponent<LevelScript>() != null)
                {
                    gameStateToSave.boxPos[^1][element.transform.position].Add(element.GetComponent<LevelScript>().SceneName);
                    gameStateToSave.boxPos[^1][element.transform.position].Add(element.GetComponentInChildren<TMP_Text>().text);
                }
                gameStateToSave.boxRotation.Add(element.transform.rotation);

            }
            else if (element.type == SavedElement.Type.Rigidbox)
            {
                gameStateToSave.rigidboxNames.Add(new List<string>());
                gameStateToSave.rigidboxNames[^1].Add(element.transform.name);
                gameStateToSave.rigidboxNames[^1].Add(element.GetComponent<RigidboxMovement>().Rigidbox1.transform.name);
                gameStateToSave.rigidboxNames[^1].Add(element.GetComponent<RigidboxMovement>().Rigidbox2.transform.name);
                gameStateToSave.rigidboxPos1.Add(new Dictionary<Vector3, List<string>>());
                gameStateToSave.rigidboxPos1[^1].Add(element.GetComponent<RigidboxMovement>().Rigidbox1.transform.position, new List<string>());
                if (element.GetComponent<RigidboxMovement>().Rigidbox1.GetComponent<LevelScript>() != null)
                {
                    gameStateToSave.rigidboxPos1[^1][element.GetComponent<RigidboxMovement>().Rigidbox1.transform.position].Add(element.GetComponent<RigidboxMovement>().Rigidbox1.GetComponent<LevelScript>().SceneName);
                    gameStateToSave.rigidboxPos1[^1][element.GetComponent<RigidboxMovement>().Rigidbox1.transform.position].Add(element.GetComponent<RigidboxMovement>().Rigidbox1.GetComponentInChildren<TMP_Text>().text);
                }
                gameStateToSave.rigidboxPos2.Add(new Dictionary<Vector3, List<string>>());
                gameStateToSave.rigidboxPos2[^1].Add(element.GetComponent<RigidboxMovement>().Rigidbox2.transform.position, new List<string>());
                if (element.GetComponent<RigidboxMovement>().Rigidbox2.GetComponent<LevelScript>() != null)
                {
                    gameStateToSave.rigidboxPos2[^1][element.GetComponent<RigidboxMovement>().Rigidbox2.transform.position].Add(element.GetComponent<RigidboxMovement>().Rigidbox2.GetComponent<LevelScript>().SceneName);
                    gameStateToSave.rigidboxPos2[^1][element.GetComponent<RigidboxMovement>().Rigidbox2.transform.position].Add(element.GetComponent<RigidboxMovement>().Rigidbox2.GetComponentInChildren<TMP_Text>().text);
                }
            }
            else if (element.type == SavedElement.Type.Rigidplayer)
            {
                gameStateToSave.rigidplayerNames.Add(new List<string>());
                gameStateToSave.rigidplayerNames[^1].Add(element.transform.name);
                gameStateToSave.rigidplayerNames[^1].Add(element.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.name);
                gameStateToSave.rigidplayerNames[^1].Add(element.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.name);
                gameStateToSave.rigidplayerPos1.Add(new Dictionary<Vector3, List<string>>());
                gameStateToSave.rigidplayerPos1[^1].Add(element.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position, new List<string>());
                gameStateToSave.rigidplayerCameraPos1.Add(element.GetComponent<RigidplayerMovement>().camerapos1);
                gameStateToSave.rigidplayerCameraPos2.Add(element.GetComponent<RigidplayerMovement>().camerapos2);
                if (element.GetComponent<RigidplayerMovement>().Rigidplayer1.GetComponent<LevelScript>() != null)
                {
                    gameStateToSave.rigidplayerPos1[^1][element.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position].Add(element.GetComponent<RigidplayerMovement>().Rigidplayer1.GetComponent<LevelScript>().SceneName);
                    gameStateToSave.rigidplayerPos1[^1][element.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position].Add(element.GetComponent<RigidplayerMovement>().Rigidplayer1.GetComponentInChildren<TMP_Text>().text);
                }
                gameStateToSave.rigidplayerPos2.Add(new Dictionary<Vector3, List<string>>());
                gameStateToSave.rigidplayerPos2[^1].Add(element.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position, new List<string>());
                if (element.GetComponent<RigidplayerMovement>().Rigidplayer2.GetComponent<LevelScript>() != null)
                {
                    gameStateToSave.rigidplayerPos2[^1][element.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position].Add(element.GetComponent<RigidplayerMovement>().Rigidplayer2.GetComponent<LevelScript>().SceneName);
                    gameStateToSave.rigidplayerPos2[^1][element.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position].Add(element.GetComponent<RigidplayerMovement>().Rigidplayer2.GetComponentInChildren<TMP_Text>().text);
                }
            }
        }
        return gameStateToSave;
    }
    public void LoadGameState()
    {
        SavedElement[] elementsToLoadOnScene = GameObject.FindObjectsByType<SavedElement>(FindObjectsSortMode.None);
        elementsToLoadOnScene = elementsToLoadOnScene.OrderBy(go => go.gameObject.name).ToArray();
        List<Vector3> remainingplayerPos = new List<Vector3>(playerPos);
        List<Quaternion> remainingplayerRotation = new List<Quaternion>(playerRotation);
        List <Vector3> remainingplayerCameraPos = new List<Vector3>(playerCameraPos);
        List<Dictionary<Vector3, List<string>>> remainingboxPos = new List<Dictionary<Vector3, List<string>>>(boxPos);
        List<Quaternion> remainingboxRotation = new List<Quaternion>(boxRotation);
        List<string> remainingboxNames = new List<string>(boxNames);
        List<Dictionary<Vector3, List<string>>> remainingrigidboxPos1 = new List<Dictionary<Vector3, List<string>>>(rigidboxPos1);
        List<Dictionary<Vector3, List<string>>> remainingrigidboxPos2 = new List<Dictionary<Vector3, List<string>>>(rigidboxPos2);
        List<List<string>> remainingrigidboxNames  = new List<List<string>>(rigidboxNames);
        List<Dictionary<Vector3, List<string>>> remainingrigidplayerPos1 = new List<Dictionary<Vector3, List<string>>>(rigidplayerPos1);
        List<Dictionary<Vector3, List<string>>> remainingrigidplayerPos2 = new List<Dictionary<Vector3, List<string>>>(rigidplayerPos2);
        List<List<string>> remainingrigidplayerNames = new List<List<string>>(rigidplayerNames);
        List<Vector3> remainingrigidplayerCameraPos1 = new List<Vector3>(rigidplayerCameraPos1);
        List<Vector3> remainingrigidplayerCameraPos2 = new List<Vector3>(rigidplayerCameraPos2);
        List<string> remainingportalOrientation = new List<string>(portalOrientation);
        GameManager.UndidWon = won;
        Debug.Log(won);
        Debug.Log(GameManager.UndidWon);
        foreach (SavedElement elementToLoad in elementsToLoadOnScene)
        {
            if (elementToLoad.type == SavedElement.Type.Player)
            {
                if (remainingplayerPos.Count == 0)
                {
                    GameObject.Destroy(elementToLoad.gameObject);
                }
                else
                {
                    elementToLoad.gameObject.transform.position = remainingplayerPos[0];
                    remainingplayerPos.RemoveAt(0);
                    elementToLoad.gameObject.transform.rotation = remainingplayerRotation[0];
                    remainingplayerRotation.RemoveAt(0);
                    elementToLoad.GetComponent<PlayerMovement>().camerapos = remainingplayerCameraPos[0];
                    remainingplayerCameraPos.RemoveAt(0);
                }
            }
            else if (elementToLoad.type == SavedElement.Type.Box)
            {
                var index = remainingboxNames.FindIndex(str => str == elementToLoad.transform.name);
                if (remainingboxPos.Count == 0 || index == -1)
                {
                    GameObject.Destroy(elementToLoad.gameObject);
                }
                else
                {
                    remainingboxNames.RemoveAt(index);
                    elementToLoad.gameObject.transform.position = remainingboxPos[index].Keys.ToArray()[0];
                    remainingboxPos.RemoveAt(index);
                    elementToLoad.gameObject.transform.rotation = remainingboxRotation[index];
                    remainingboxRotation.RemoveAt(index);
                }
            }
            else if (elementToLoad.type == SavedElement.Type.Rigidbox)
            {
                var index = remainingrigidboxNames.FindIndex(list => list.SequenceEqual(new List<string> { elementToLoad.name, elementToLoad.GetComponent<RigidboxMovement>().Rigidbox1.name, elementToLoad.GetComponent<RigidboxMovement>().Rigidbox2.name }));
                if (index != -1)
                {
                    elementToLoad.gameObject.GetComponent<RigidboxMovement>().Rigidbox1.transform.position = remainingrigidboxPos1[index].Keys.ToArray()[0];
                    elementToLoad.gameObject.GetComponent<RigidboxMovement>().Rigidbox2.transform.position = remainingrigidboxPos2[index].Keys.ToArray()[0];
                    remainingrigidboxNames.RemoveAt(index);
                    remainingrigidboxPos1.RemoveAt(index);
                    remainingrigidboxPos2.RemoveAt(index);
                }
                else
                {
                    GameObject.Destroy(elementToLoad.gameObject);
                }
            }
            else if (elementToLoad.type == SavedElement.Type.Rigidplayer)
            {
                elementToLoad.gameObject.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position = remainingrigidplayerPos1[0].Keys.ToArray()[0];
                elementToLoad.gameObject.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position = remainingrigidplayerPos2[0].Keys.ToArray()[0];
                elementToLoad.GetComponent<RigidplayerMovement>().camerapos1 = remainingrigidplayerCameraPos1[0];
                elementToLoad.GetComponent<RigidplayerMovement>().camerapos2 = remainingrigidplayerCameraPos2[0];
                remainingrigidplayerCameraPos1.RemoveAt(0);
                remainingrigidplayerCameraPos2.RemoveAt(0);
                remainingrigidplayerNames.RemoveAt(0);
                remainingrigidplayerPos1.RemoveAt(0);
                remainingrigidplayerPos2.RemoveAt(0);
            }
            if (remainingplayerPos.Count == 0 && remainingrigidplayerPos1.Count == 0 && remainingrigidplayerPos2.Count == 0)
            {
                MainCamera = GameObject.FindAnyObjectByType<CameraMovement>().gameObject;
                MainCamera.transform.position = camerapos;
            }
        }
        if (remainingboxPos.Count != 0)
        {
            while (remainingboxPos.Count != 0)
            {
                if (remainingboxPos[0].Values.ToArray()[0].Count != 0)
                {
                    var Instantiated = GameObject.Instantiate(Resources.Load<GameObject>("LevelBox"), Vector3.zero, remainingboxRotation[0]);
                    if (Instantiated != null)
                    {
                        Instantiated.transform.position = remainingboxPos[0].Keys.ToArray()[0];
                        if (remainingboxPos[0].Values.ToArray()[0].Count != 0)
                        {
                            Instantiated.GetComponent<LevelScript>().SceneName = remainingboxPos[0].Values.ToArray()[0][0];
                            Instantiated.GetComponentInChildren<TMP_Text>().text = remainingboxPos[0].Values.ToArray()[0][1];
                        }
                        else
                        {
                            GameObject.Destroy(Instantiated.GetComponent<LevelScript>());
                            Instantiated.GetComponentInChildren<Canvas>().enabled = false;
                        }
                        
                        Instantiated.transform.name = remainingboxNames[0];
                        remainingboxNames.RemoveAt(0);
                        remainingboxPos.RemoveAt(0);
                        remainingboxRotation.RemoveAt(0);
                    }
                }
                else
                {
                    var Instantiated = GameObject.Instantiate(Resources.Load<GameObject>("Box"), Vector3.zero, remainingboxRotation[0]);
                    if (Instantiated != null)
                    {
                        Instantiated.transform.name = remainingboxNames[0];
                        Instantiated.transform.position = remainingboxPos[0].Keys.ToArray()[0];
                        remainingboxNames.RemoveAt(0);
                        remainingboxPos.RemoveAt(0);
                        remainingboxRotation.RemoveAt(0);
                    }
                }
            }
        }
        if (remainingrigidboxPos1.Count != 0)
        {
            while (remainingrigidboxPos1.Count != 0)
            {
                if (remainingrigidboxPos1[0].Values.ToArray()[0].Count != 0 || remainingrigidboxPos2[0].Values.ToArray()[0].Count != 0)
                {
                    var Instantiated = GameObject.Instantiate(Resources.Load<GameObject>("LevelRigidbox"), Vector3.zero, Quaternion.Euler(Vector3.zero));
                    if (Instantiated != null)
                    {
                        Instantiated.GetComponent<RigidboxMovement>().Rigidbox1.transform.position = remainingrigidboxPos1[0].Keys.ToArray()[0];
                        if (remainingrigidboxPos1[0].Values.ToArray()[0].Count != 0)
                        {
                            Instantiated.GetComponent<RigidboxMovement>().Rigidbox1.GetComponent<LevelScript>().SceneName = remainingrigidboxPos1[0].Values.ToArray()[0][0];
                            Instantiated.GetComponent<RigidboxMovement>().Rigidbox1.GetComponentInChildren<TMP_Text>().text = remainingrigidboxPos1[0].Values.ToArray()[0][1];
                        }
                        else
                        {
                            GameObject.Destroy(Instantiated.GetComponent<RigidboxMovement>().Rigidbox1.GetComponent<LevelScript>());
                            Instantiated.GetComponent<RigidboxMovement>().Rigidbox1.GetComponentInChildren<Canvas>().enabled = false;
                        }
                        Instantiated.GetComponent<RigidboxMovement>().Rigidbox2.transform.position = remainingrigidboxPos2[0].Keys.ToArray()[0];
                        if (remainingrigidboxPos2[0].Values.ToArray()[0].Count != 0)
                        {
                            Instantiated.GetComponent<RigidboxMovement>().Rigidbox2.GetComponent<LevelScript>().SceneName = remainingrigidboxPos2[0].Values.ToArray()[0][0];
                            Instantiated.GetComponent<RigidboxMovement>().Rigidbox2.GetComponentInChildren<TMP_Text>().text = remainingrigidboxPos2[0].Values.ToArray()[0][1];
                        }
                        else
                        {
                            GameObject.Destroy(Instantiated.GetComponent<RigidboxMovement>().Rigidbox2.GetComponent<LevelScript>());
                            Instantiated.GetComponent<RigidboxMovement>().Rigidbox2.GetComponentInChildren<Canvas>().enabled =false;
                        }
                        Instantiated.transform.name = remainingrigidboxNames[0][0];
                        Instantiated.GetComponent<RigidboxMovement>().Rigidbox1.name = remainingrigidboxNames[0][1];
                        Instantiated.GetComponent<RigidboxMovement>().Rigidbox2.name = remainingrigidboxNames[0][2];
                        remainingrigidboxNames.RemoveAt(0);
                        remainingrigidboxPos1.RemoveAt(0);
                        remainingrigidboxPos2.RemoveAt(0);
                    }
                }
                else
                {
                    var Instantiated = GameObject.Instantiate(Resources.Load<GameObject>("Rigidbox"), Vector3.zero, Quaternion.Euler(Vector3.zero));
                    if (Instantiated != null)
                    {
                        Instantiated.transform.name = remainingrigidboxNames[0][0];
                        Instantiated.GetComponent<RigidboxMovement>().Rigidbox1.name = remainingrigidboxNames[0][1];
                        Instantiated.GetComponent<RigidboxMovement>().Rigidbox2.name = remainingrigidboxNames[0][2];
                        Instantiated.GetComponent<RigidboxMovement>().Rigidbox1.transform.position = remainingrigidboxPos1[0].Keys.ToArray()[0];
                        Instantiated.GetComponent<RigidboxMovement>().Rigidbox2.transform.position = remainingrigidboxPos2[0].Keys.ToArray()[0];
                        remainingrigidboxNames.RemoveAt(0);
                        remainingrigidboxPos1.RemoveAt(0);
                        remainingrigidboxPos2.RemoveAt(0);
                    }
                }
            }

        }
        if (remainingrigidplayerPos1.Count != 0)
        {
            while (remainingrigidplayerPos1.Count != 0)
            {
                    var Instantiated = GameObject.Instantiate(Resources.Load<GameObject>("Rigidplayer"), Vector3.zero, Quaternion.Euler(Vector3.zero));
                    if (Instantiated != null)
                    {
                        Instantiated.GetComponent<RigidplayerMovement>().Rigidplayer1.transform.position = remainingrigidplayerPos1[0].Keys.ToArray()[0];
                        Instantiated.GetComponent<RigidplayerMovement>().Rigidplayer2.transform.position = remainingrigidplayerPos2[0].Keys.ToArray()[0];
                        remainingrigidplayerPos1.RemoveAt(0);
                        remainingrigidplayerPos2.RemoveAt(0);
                    }
            }

        }
    }
}