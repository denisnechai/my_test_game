using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LevelScript : MonoBehaviour
{
    public Scene scene;
    public string SceneName;
    public bool first = true;
    public int SceneBuildIndex(string sceneName)
    {
        return SceneUtility.GetBuildIndexByScenePath(SceneName);
    }
    public float GridChange = 1;
    public bool shortcut = false;
    public Sprite Square_up;
    public Sprite Square_down;
    public Sprite Square_left;
    public Sprite Square_right;
    public Sprite Triangle_down;
    public Sprite Triangle_left;
    public Sprite Triangle_right;
    public Sprite Square;
    public Sprite Triangle;
    public Sprite Fallen_square;
    public Sprite Fallen_triangle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (first)
        {
            GameManager.ColorCheck();
            if (gameObject.GetComponent<BoxMovement>() != null && GameManager.LevelsWon.ContainsKey(SceneName) && GameManager.LevelsWon[SceneName] == false && gameObject.GetComponent<BoxMovement>().won == true)
            {
                gameObject.GetComponent<BoxMovement>().won = false;
            }
            else if (gameObject.GetComponent<BoxMovement>() != null && GameManager.LevelsWon.ContainsKey(SceneName) && GameManager.LevelsWon[SceneName] == true)
            {
                gameObject.GetComponent<BoxMovement>().won = true;
                if (transform.position.y < GridChange)
                {
                    if (transform.position.z == 1f)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = Fallen_square;
                    }
                    else if (transform.position.z == 0f)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = Square;
                    }
                }
                else
                {
                    if (transform.position.z == 1f)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = Fallen_triangle;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = Triangle;
                    }
                    if (Mathf.Abs(transform.position.x) % 1 == ((transform.position.y - GridChange) % 2) / 2)
                    {
                        transform.localScale = new Vector3(1, 1, 1);
                    }
                    else
                    {
                        transform.localScale = new Vector3(1, -1, 1);
                    }
                }
            }
            else if (gameObject.transform.root.GetComponent<RigidboxMovement>() != null && gameObject == gameObject.transform.root.GetComponent<RigidboxMovement>().Rigidbox1 && GameManager.LevelsWon.ContainsKey(SceneName) && GameManager.LevelsWon[SceneName] == false && gameObject.transform.root.GetComponent<RigidboxMovement>().won1 == true)
            {
                gameObject.transform.root.GetComponent<RigidboxMovement>().won1 = false;
            }
            else if (gameObject.transform.root.GetComponent<RigidboxMovement>() != null && gameObject == gameObject.transform.root.GetComponent<RigidboxMovement>().Rigidbox1 && GameManager.LevelsWon.ContainsKey(SceneName) && GameManager.LevelsWon[SceneName] == true && gameObject.transform.root.GetComponent<RigidboxMovement>().won1 == false)
            {
                Debug.Log("Sprite set");
                gameObject.transform.root.GetComponent<RigidboxMovement>().won1 = true;
                if (gameObject.transform.position.y < GridChange && gameObject.transform.root.GetComponent<RigidboxMovement>().Rigidbox2.transform.position.y < GridChange)
                {
                    gameObject.transform.localScale = Vector3.one;
                    if (gameObject.transform.position.x < gameObject.transform.root.GetComponent<RigidboxMovement>().Rigidbox2.transform.position.x)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = Square_right;
                    }
                    else if (gameObject.transform.position.x > gameObject.transform.root.GetComponent<RigidboxMovement>().Rigidbox2.transform.position.x)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = Square_left;
                    }
                    else if (gameObject.transform.position.y < gameObject.transform.root.GetComponent<RigidboxMovement>().Rigidbox2.transform.position.y)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = Square_up;
                    }
                    else if (gameObject.transform.position.y > gameObject.transform.root.GetComponent<RigidboxMovement>().Rigidbox2.transform.position.y)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = Square_down;
                    }
                }
                else if (gameObject.transform.position.y >= GridChange && gameObject.transform.root.GetComponent<RigidboxMovement>().Rigidbox2.transform.position.y >= GridChange)
                {
                    if (gameObject.transform.position.y != gameObject.transform.root.GetComponent<RigidboxMovement>().Rigidbox2.transform.position.y)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = Triangle_down;
                    }
                    else if (gameObject.transform.position.x < gameObject.transform.root.GetComponent<RigidboxMovement>().Rigidbox2.transform.position.x)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = Triangle_right;
                    }
                    else if (gameObject.transform.position.x > gameObject.transform.root.GetComponent<RigidboxMovement>().Rigidbox2.transform.position.x)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = Triangle_left;
                    }
                    if (Mathf.Abs(gameObject.transform.position.x) % 1 == ((gameObject.transform.position.y - GridChange) % 2) / 2)
                    {
                        gameObject.transform.localScale = Vector3.one;
                    }
                    else
                    {
                        gameObject.transform.localScale = new Vector3(1, -1, 1);
                    }
                }
                else if (gameObject.transform.position.y >= GridChange || gameObject.transform.root.GetComponent<RigidboxMovement>().Rigidbox2.transform.position.y < GridChange)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = Triangle_down;
                    gameObject.transform.localScale = Vector3.one;
                }
                else if (gameObject.transform.position.y < GridChange || gameObject.transform.root.GetComponent<RigidboxMovement>().Rigidbox2.transform.position.y >= GridChange)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = Square_up;
                    gameObject.transform.localScale = Vector3.one;
                }
            }
            else if (gameObject.transform.root.GetComponent<RigidboxMovement>() != null && gameObject == gameObject.transform.root.GetComponent<RigidboxMovement>().Rigidbox2 && GameManager.LevelsWon.ContainsKey(SceneName) && GameManager.LevelsWon[SceneName] == false && gameObject.transform.root.GetComponent<RigidboxMovement>().won2 == true)
            {
                gameObject.transform.root.GetComponent<RigidboxMovement>().won2 = false;
            }
            else if (gameObject.transform.root.GetComponent<RigidboxMovement>() != null && gameObject == gameObject.transform.root.GetComponent<RigidboxMovement>().Rigidbox2 && GameManager.LevelsWon.ContainsKey(SceneName) && GameManager.LevelsWon[SceneName] == true && gameObject.transform.root.GetComponent<RigidboxMovement>().won2 == false)
            {
                gameObject.transform.root.GetComponent<RigidboxMovement>().won2 = true;
                if (gameObject.transform.root.GetComponent<RigidboxMovement>().Rigidbox1.transform.position.y < GridChange && gameObject.transform.position.y < GridChange)
                {
                    gameObject.transform.localScale = Vector3.one;
                    if (gameObject.transform.root.GetComponent<RigidboxMovement>().Rigidbox1.transform.position.x < gameObject.transform.position.x)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = Square_left;
                    }
                    else if (gameObject.transform.root.GetComponent<RigidboxMovement>().Rigidbox1.transform.position.x > gameObject.transform.position.x)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = Square_right;
                    }
                    else if (gameObject.transform.root.GetComponent<RigidboxMovement>().Rigidbox1.transform.position.y < gameObject.transform.position.y)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = Square_down;
                    }
                    else if (gameObject.transform.root.GetComponent<RigidboxMovement>().Rigidbox1.transform.position.y > gameObject.transform.position.y)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = Square_up;
                    }
                }
                else if (gameObject.transform.root.GetComponent<RigidboxMovement>().Rigidbox1.transform.position.y >= GridChange && gameObject.transform.position.y >= GridChange)
                {
                    if (gameObject.transform.root.GetComponent<RigidboxMovement>().Rigidbox1.transform.position.y != gameObject.transform.position.y)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = Triangle_down;
                    }
                    else if (gameObject.transform.root.GetComponent<RigidboxMovement>().Rigidbox1.transform.position.x < gameObject.transform.position.x)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = Triangle_left;
                    }
                    else if (gameObject.transform.root.GetComponent<RigidboxMovement>().Rigidbox1.transform.position.x > gameObject.transform.position.x)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sprite = Triangle_right;
                    }
                    if (Mathf.Abs(gameObject.transform.position.x) % 1 == ((gameObject.transform.position.y - GridChange) % 2) / 2)
                    {
                        gameObject.transform.localScale = Vector3.one;
                    }
                    else
                    {
                        gameObject.transform.localScale = new Vector3(1, -1, 1);
                    }
                }
                else if (gameObject.transform.root.GetComponent<RigidboxMovement>().Rigidbox1.transform.position.y >= GridChange && gameObject.transform.position.y < GridChange)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = Square_up;
                    gameObject.transform.localScale = Vector3.one;
                }
                else if (gameObject.transform.root.GetComponent<RigidboxMovement>().Rigidbox1.transform.position.y < GridChange && gameObject.transform.position.y >= GridChange)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = Triangle_down;
                    gameObject.transform.localScale = Vector3.one;
                }
            }
            first = false;
        }
    }
}
