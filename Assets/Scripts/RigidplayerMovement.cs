using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

public class RigidplayerMovement : MonoBehaviour
{
    public GameObject Rigidplayer1;
    public GameObject Rigidplayer2;
    public bool isMoving1 = false;
    public bool isMoving2 = false;
    public bool noInput = false;
    public bool won1 = true;
    public bool won2 = true;
    public Vector3 camerapos1;
    public Vector3 camerapos2;
    private string key;
    public bool isPushed = false;
    public LayerMask blockingLayer;
    public LayerMask Hole;
    public float GridChange = 1;
    public SpriteRenderer spriterenderer1;
    public SpriteRenderer spriterenderer2;
    public Sprite Square_up;
    public Sprite Square_down;
    public Sprite Square_left;
    public Sprite Square_right;
    public Sprite Triangle_down;
    public Sprite Triangle_left;
    public Sprite Triangle_right;
    public Sprite Fallen_Square_up;
    public Sprite Fallen_Square_down;
    public Sprite Fallen_Square_left;
    public Sprite Fallen_Square_right;
    public Sprite Fallen_Triangle_down;
    public Sprite Fallen_Triangle_left;
    public Sprite Fallen_Triangle_right;
    public GameObject Player;
    private bool destroy = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Rigidplayer1.transform.rotation = Quaternion.identity;
        Rigidplayer2.transform.rotation = Quaternion.identity;
        camerapos1 = Rigidplayer1.transform.position;
        camerapos2 = Rigidplayer2.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (isMoving1 || isMoving2) return;
        if (!won1 || !won2) return;
        if (GameManager.isMoving) return;
        if ((Rigidplayer1.transform.position.y >= GridChange && Rigidplayer2.transform.position.y >= GridChange && ((Mathf.Abs(Rigidplayer1.transform.position.x) % 1 == ((Rigidplayer1.transform.position.y - GridChange) % 2) / 2 && Rigidplayer2.transform.position.y > Rigidplayer1.transform.position.y) || Mathf.Abs(Rigidplayer2.transform.position.x) % 1 == ((Rigidplayer2.transform.position.y - GridChange) % 2) / 2 && Rigidplayer1.transform.position.y > Rigidplayer2.transform.position.y || Mathf.Abs(Rigidplayer2.transform.position.x - Rigidplayer1.transform.position.x) > 0.5f)) || Mathf.Abs(Rigidplayer2.transform.position.x - Rigidplayer1.transform.position.x) + Mathf.Abs(Rigidplayer2.transform.position.y - Rigidplayer1.transform.position.y) > 1 && !GameManager.isMoving)
        {
            destroy = true;
            Debug.Log("Destroy");
        }
        if (destroy == true)
        {
            var player1 = Instantiate(Player, Rigidplayer1.transform.position, Rigidplayer1.transform.rotation);
            var player2 = Instantiate(Player, Rigidplayer2.transform.position, Rigidplayer2.transform.rotation);
            player1.name = Rigidplayer1.name;
            player2.name = Rigidplayer2.name;
            GameObject.Destroy(gameObject);
            isMoving1 = true;
            isMoving2 = true;
            destroy = false;
            return;
        }
        if ((Rigidplayer1.transform.position.z == 0f || Rigidplayer2.transform.position.z == 0f))
        {
            if (Physics2D.OverlapPoint(Rigidplayer1.transform.position, Hole, 1f, 1f) && !(Physics2D.OverlapPoint(Rigidplayer1.transform.position, blockingLayer, 1f, 1f)) && Physics2D.OverlapPoint(Rigidplayer2.transform.position, Hole, 1f, 1f) && !(Physics2D.OverlapPoint(Rigidplayer2.transform.position, blockingLayer, 1f, 1f)) && !GameManager.isMoving)
            {
                if (!(Physics2D.OverlapPoint(Rigidplayer1.transform.position, blockingLayer, 1f, 1f)) && !(Physics2D.OverlapPoint(Rigidplayer2.transform.position, blockingLayer, 1f, 1f)))
                {
                    var pos1 = new Vector3(Rigidplayer1.transform.position.x, Rigidplayer1.transform.position.y, 1f);
                    var pos2 = new Vector3(Rigidplayer2.transform.position.x, Rigidplayer2.transform.position.y, 1f);
                    Debug.Log(Rigidplayer1.transform.position);
                    Debug.Log(Rigidplayer2.transform.position);
                    Debug.Log(GameObject.FindGameObjectWithTag("Player").transform.position);
                    Rigidplayer1.transform.position = pos1;
                    Rigidplayer2.transform.position = pos2;
                    Debug.Log(GameManager.isMoving);
                    Debug.Log("Fall");
                }
            }
        }
        SpriteUpdate();
        key = string.Empty;
        if (Rigidplayer1.transform.position.y < GridChange && Rigidplayer2.transform.position.y < GridChange)
        {
            if (Input.GetKey(KeyCode.W))
            {
                key = "W";
            }
            if (Input.GetKey(KeyCode.A))
            {
                key = "A";
            }
            if (Input.GetKey(KeyCode.S))
            {
                key = "S";
            }
            if (Input.GetKey(KeyCode.D))
            {
                key = "D";
            }

        }
        else if ((Rigidplayer1.transform.position.y == GridChange && Rigidplayer2.transform.position.y < GridChange) || (Rigidplayer1.transform.position.y < GridChange && Rigidplayer2.transform.position.y == GridChange))
        {
            if (Input.GetKey(KeyCode.S))
            {
                key = "S";
            }
            else if ((Input.GetKey(KeyCode.W)))
            {
                key = "W1";
            }
            else if ((Input.GetKey(KeyCode.E)))
            {
                key = "E1";
            }
            else if ((Input.GetKey(KeyCode.A)))
            {
                key = "A1";
            }
            else if ((Input.GetKey(KeyCode.D)))
            {
                key = "D1";
            }
            else if ((Input.GetKey(KeyCode.Z)))
            {
                key = "Z1";
            }
            else if ((Input.GetKey(KeyCode.X)))
            {
                key = "X1";
            }
        }
        else
                {
            if ((Input.GetKey(KeyCode.W)))
            {
                key = "W1";
            }
            else if ((Input.GetKey(KeyCode.E)))
            {
                key = "E1";
            }
            else if ((Input.GetKey(KeyCode.A)))
            {
                key = "A1";
            }
            else if ((Input.GetKey(KeyCode.D)))
            {
                key = "D1";
            }
            else if ((Input.GetKey(KeyCode.Z)))
            {
                key = "Z1";
            }
            else if ((Input.GetKey(KeyCode.X)))
            {
                key = "X1";
            }

        }

        if (key != string.Empty && TryToMove(key, true))
        {
            TryToMove(key, false);
        }
    }
    public void SpriteUpdate()
    {
        if (Rigidplayer1.transform.position.z != 1 && Rigidplayer2.transform.position.z != 1)
        {
            if (Rigidplayer1.transform.position.y < GridChange && Rigidplayer2.transform.position.y < GridChange)
            {
                Rigidplayer1.transform.localScale = Vector3.one;
                Rigidplayer2.transform.localScale = Vector3.one;
                if (Rigidplayer1.transform.position.x < Rigidplayer2.transform.position.x)
                {
                    spriterenderer1.sprite = Square_right;
                    spriterenderer2.sprite = Square_left;
                }
                else if (Rigidplayer1.transform.position.x > Rigidplayer2.transform.position.x)
                {
                    spriterenderer1.sprite = Square_left;
                    spriterenderer2.sprite = Square_right;
                }
                else if (Rigidplayer1.transform.position.y < Rigidplayer2.transform.position.y)
                {
                    spriterenderer1.sprite = Square_up;
                    spriterenderer2.sprite = Square_down;
                }
                else if (Rigidplayer1.transform.position.y > Rigidplayer2.transform.position.y)
                {
                    spriterenderer1.sprite = Square_down;
                    spriterenderer2.sprite = Square_up;
                }
            }
            else if (Rigidplayer1.transform.position.y >= GridChange && Rigidplayer2.transform.position.y >= GridChange)
            {
                if (Rigidplayer1.transform.position.y != Rigidplayer2.transform.position.y)
                {
                    spriterenderer1.sprite = Triangle_down;
                    spriterenderer2.sprite = Triangle_down;
                }
                else if (Rigidplayer1.transform.position.x < Rigidplayer2.transform.position.x)
                {
                    spriterenderer1.sprite = Triangle_right;
                    spriterenderer2.sprite = Triangle_left;
                }
                else if (Rigidplayer1.transform.position.x > Rigidplayer2.transform.position.x)
                {
                    spriterenderer1.sprite = Triangle_left;
                    spriterenderer2.sprite = Triangle_right;
                }
                if (Mathf.Abs(Rigidplayer1.transform.position.x) % 1 == ((Rigidplayer1.transform.position.y - GridChange) % 2) / 2)
                {
                    Rigidplayer1.transform.localScale = Vector3.one;
                }
                else
                {
                    Rigidplayer1.transform.localScale = new Vector3(1, -1, 1);
                }
                if (Mathf.Abs(Rigidplayer2.transform.position.x) % 1 == ((Rigidplayer2.transform.position.y - GridChange) % 2) / 2)
                {
                    Rigidplayer2.transform.localScale = Vector3.one;
                }
                else
                {
                    Rigidplayer2.transform.localScale = new Vector3(1, -1, 1);
                }
            }
            else if (Rigidplayer1.transform.position.y >= GridChange || Rigidplayer2.transform.position.y < GridChange)
            {
                spriterenderer1.sprite = Triangle_down;
                spriterenderer2.sprite = Square_up;
                Rigidplayer1.transform.localScale = Vector3.one;
                Rigidplayer2.transform.localScale = Vector3.one;
            }
            else if (Rigidplayer1.transform.position.y < GridChange || Rigidplayer2.transform.position.y >= GridChange)
            {
                spriterenderer1.sprite = Square_up;
                spriterenderer2.sprite = Triangle_down;
                Rigidplayer1.transform.localScale = Vector3.one;
                Rigidplayer2.transform.localScale = Vector3.one;
            }
        }
        else
        {
            if (Rigidplayer1.transform.position.y < GridChange && Rigidplayer2.transform.position.y < GridChange)
            {
                Rigidplayer1.transform.localScale = Vector3.one;
                Rigidplayer2.transform.localScale = Vector3.one;
                if (Rigidplayer1.transform.position.x < Rigidplayer2.transform.position.x)
                {
                    spriterenderer1.sprite = Fallen_Square_right;
                    spriterenderer2.sprite = Fallen_Square_left;
                }
                else if (Rigidplayer1.transform.position.x > Rigidplayer2.transform.position.x)
                {
                    spriterenderer1.sprite = Fallen_Square_left;
                    spriterenderer2.sprite = Fallen_Square_right;
                }
                else if (Rigidplayer1.transform.position.y < Rigidplayer2.transform.position.y)
                {
                    spriterenderer1.sprite = Fallen_Square_up;
                    spriterenderer2.sprite = Fallen_Square_down;
                }
                else if (Rigidplayer1.transform.position.y > Rigidplayer2.transform.position.y)
                {
                    spriterenderer1.sprite = Fallen_Square_down;
                    spriterenderer2.sprite = Fallen_Square_up;
                }
            }
            else if (Rigidplayer1.transform.position.y >= GridChange && Rigidplayer2.transform.position.y >= GridChange)
            {
                if (Rigidplayer1.transform.position.y != Rigidplayer2.transform.position.y)
                {
                    spriterenderer1.sprite = Fallen_Triangle_down;
                    spriterenderer2.sprite = Fallen_Triangle_down;
                }
                else if (Rigidplayer1.transform.position.x < Rigidplayer2.transform.position.x)
                {
                    spriterenderer1.sprite = Fallen_Triangle_right;
                    spriterenderer2.sprite = Fallen_Triangle_left;
                }
                else if (Rigidplayer1.transform.position.x > Rigidplayer2.transform.position.x)
                {
                    spriterenderer1.sprite = Fallen_Triangle_left;
                    spriterenderer2.sprite = Fallen_Triangle_right;
                }
                if (Mathf.Abs(Rigidplayer1.transform.position.x) % 1 == ((Rigidplayer1.transform.position.y - GridChange) % 2) / 2)
                {
                    Rigidplayer1.transform.localScale = Vector3.one;
                }
                else
                {
                    Rigidplayer1.transform.localScale = new Vector3(1, -1, 1);
                }
                if (Mathf.Abs(Rigidplayer2.transform.position.x) % 1 == ((Rigidplayer2.transform.position.y - GridChange) % 2) / 2)
                {
                    Rigidplayer2.transform.localScale = Vector3.one;
                }
                else
                {
                    Rigidplayer2.transform.localScale = new Vector3(1, -1, 1);
                }
            }
            else if (Rigidplayer1.transform.position.y >= GridChange && Rigidplayer2.transform.position.y < GridChange)
            {
                spriterenderer1.sprite = Triangle_down;
                spriterenderer2.sprite = Square_up;
                Rigidplayer1.transform.localScale = Vector3.one;
                Rigidplayer2.transform.localScale = Vector3.one;
            }
            else if (Rigidplayer1.transform.position.y < GridChange && Rigidplayer2.transform.position.y >= GridChange)
            {
                spriterenderer1.sprite = Square_up;
                spriterenderer2.sprite = Triangle_down;
                Rigidplayer1.transform.localScale = Vector3.one;
                Rigidplayer2.transform.localScale = Vector3.one;
            }
        }
    }
    public bool TryToMove(string key, bool check)
    {
        if (isMoving1 || isMoving2) return true;
        if (!won1 || !won2) return false;
        var movement1 = Vector3.zero;
        var movement2 = Vector3.zero;
        var move1 = false;
        var move2 = false;
        if (Rigidplayer1.transform.position.y < GridChange && Rigidplayer2.transform.position.y < GridChange)
        {
            if (key == "W")
            {
                movement1 = Vector3.up;
                movement2 = Vector3.up;
            }
            else if (key == "A")
            {
                movement1 = Vector3.left;
                movement2 = Vector3.left;
            }
            else if (key == "S")
            {
                movement1 = Vector3.down;
                movement2 = Vector3.down;
            }
            else if (key == "D")
            {
                movement1 = Vector3.right;
                movement2 = Vector3.right;
            }

        }
        else if (Rigidplayer1.transform.position.y > GridChange || Rigidplayer2.transform.position.y > GridChange)
        {
            if (Rigidplayer1.transform.position.y != Rigidplayer2.transform.position.y)
            {
                if (key == "W1")
                {
                    movement1 = Vector3.up;
                    movement2 = Vector3.left / 2;
                }
                else if (key == "E1")
                {
                    movement1 = Vector3.up;
                    movement2 = Vector3.right / 2;
                }
                else if (key == "A1")
                {
                    movement1 = Vector3.left;
                    movement2 = Vector3.left;
                }
                else if (key == "D1")
                {
                    movement1 = Vector3.right;
                    movement2 = Vector3.right;
                }
                else if (key == "Z1")
                {
                    movement1 = Vector3.left / 2;
                    movement2 = Vector3.down;
                }
                else if (key == "X1")
                {
                    movement1 = Vector3.right / 2;
                    movement2 = Vector3.down;
                }
                if (Rigidplayer1.transform.position.y > Rigidplayer2.transform.position.y)
                {
                    movement1 += movement2;
                    movement2 = movement1 - movement2;
                    movement1 -= movement2;
                }
            }
            else if ((Mathf.Abs(Rigidplayer1.transform.position.x) % 1 == ((Rigidplayer1.transform.position.y - GridChange) % 2) / 2 & Rigidplayer2.transform.position.x > Rigidplayer1.transform.position.x) || (Mathf.Abs(Rigidplayer2.transform.position.x) % 1 == ((Rigidplayer2.transform.position.y - GridChange) % 2) / 2 & Rigidplayer1.transform.position.x > Rigidplayer2.transform.position.x))
            {
                if (key == "W1")
                {
                    movement1 = new Vector3(-0.5f, 1, 0);
                    movement2 = new Vector3(-0.5f, 1, 0);
                }
                else if (key == "E1")
                {
                    movement1 = Vector3.up;
                    movement2 = Vector3.right / 2;
                }
                else if (key == "A1")
                {
                    movement1 = Vector3.left / 2;
                    movement2 = Vector3.left / 2;
                }
                else if (key == "D1")
                {
                    movement1 = Vector3.right / 2;
                    movement2 = Vector3.right / 2;
                }
                else if (key == "Z1")
                {
                    movement1 = Vector3.left / 2;
                    movement2 = Vector3.down;
                }
                else if (key == "X1")
                {
                    movement1 = new Vector3(0.5f, -1, 0);
                    movement2 = new Vector3(0.5f, -1, 0);
                }
                if (Rigidplayer1.transform.position.x < Rigidplayer2.transform.position.x)
                {
                    movement1 += movement2;
                    movement2 = movement1 - movement2;
                    movement1 -= movement2;
                }
            }
            else if ((Mathf.Abs(Rigidplayer1.transform.position.x) % 1 == ((Rigidplayer1.transform.position.y - GridChange) % 2) / 2 && Rigidplayer2.transform.position.x < Rigidplayer1.transform.position.x) || (Mathf.Abs(Rigidplayer2.transform.position.x) % 1 == ((Rigidplayer2.transform.position.y - GridChange) % 2) / 2 && Rigidplayer1.transform.position.x < Rigidplayer2.transform.position.x))
            {
                if (key == "W1")
                {
                    movement1 = Vector3.up;
                    movement2 = Vector3.left / 2;
                }
                else if (key == "E1")
                {
                    movement1 = new Vector3(0.5f, 1, 0);
                    movement2 = new Vector3(0.5f, 1, 0);
                }
                else if (key == "A1")
                {
                    movement1 = Vector3.left / 2;
                    movement2 = Vector3.left / 2;
                }
                else if (key == "D1")
                {
                    movement1 = Vector3.right / 2;
                    movement2 = Vector3.right / 2;
                }
                else if (key == "Z1")
                {
                    movement1 = new Vector3(-0.5f, -1, 0);
                    movement2 = new Vector3(-0.5f, -1, 0);
                }
                else if (key == "X1")
                {
                    movement1 = Vector3.right / 2;
                    movement2 = Vector3.down;
                }
                if (Rigidplayer1.transform.position.x > Rigidplayer2.transform.position.x)
                {
                    movement1 += movement2;
                    movement2 = movement1 - movement2;
                    movement1 -= movement2;
                }
            }
        }
        else if ((Rigidplayer1.transform.position.y == GridChange && Rigidplayer2.transform.position.y < GridChange) || (Rigidplayer1.transform.position.y < GridChange && Rigidplayer2.transform.position.y == GridChange))
        {
            if (key == "W1")
            {
                movement1 = Vector3.left / 2;
                movement2 = Vector3.up;
            }
            if (key == "E1")
            {
                movement1 = Vector3.right / 2;
                movement2 = Vector3.up;
            }
            if (key == "A" || key == "A1")
            {
                movement1 = Vector3.left;
                movement2 = Vector3.left;
            }
            if (key == "D" || key == "D1")
            {
                movement1 = Vector3.right;
                movement2 = Vector3.right;
            }
            if (key == "Z1" || key == "X1" || key == "S")
            {
                movement1 = Vector3.down;
                movement2 = Vector3.down;
            }
            if (Rigidplayer1.transform.position.y < GridChange && Rigidplayer2.transform.position.y >= GridChange)
            {
                movement1 += movement2;
                movement2 = movement1 - movement2;
                movement1 -= movement2;

            }
        }
        else if (Rigidplayer1.transform.position.y == GridChange && Rigidplayer2.transform.position.y == GridChange)
        {
            if ((Mathf.Abs(Rigidplayer1.transform.position.x) % 1 == ((Rigidplayer1.transform.position.y - GridChange) % 2) / 2 && Rigidplayer2.transform.position.x > Rigidplayer1.transform.position.x) || (Mathf.Abs(Rigidplayer2.transform.position.x) % 1 == ((Rigidplayer2.transform.position.y - GridChange) % 2) / 2 & Rigidplayer1.transform.position.x > Rigidplayer2.transform.position.x))
            {
                if (key == "W")
                {
                    key = "W1";
                }
                if (key == "W1")
                {
                    movement1 = new Vector3(-0.5f, 1, 0);
                    movement2 = new Vector3(-0.5f, 1, 0);
                }
                if (key == "E1")
                {
                    movement1 = Vector3.up;
                    movement2 = Vector3.right / 2;
                }
                else if (key == "A1")
                {
                    movement1 = Vector3.left / 2;
                    movement2 = Vector3.left / 2;
                }
                else if (key == "D1")
                {
                    movement1 = Vector3.right / 2;
                    movement2 = Vector3.right / 2;

                }
                else if (key == "Z1")
                {
                    movement1 = Vector3.left / 2;
                    movement2 = Vector3.down;
                }
                else if (key == "X1")
                {
                    movement1 = new Vector3(0.5f, -1, 0);
                    movement2 = Vector3.down;
                }
                if (Rigidplayer1.transform.position.x < Rigidplayer2.transform.position.x)
                {
                    movement1 += movement2;
                    movement2 = movement1 - movement2;
                    movement1 -= movement2;
                }
            }
            else if ((Mathf.Abs(Rigidplayer1.transform.position.x) % 1 == ((Rigidplayer1.transform.position.y - GridChange) % 2) / 2 && Rigidplayer2.transform.position.x < Rigidplayer1.transform.position.x) || (Mathf.Abs(Rigidplayer2.transform.position.x) % 1 == ((Rigidplayer2.transform.position.y - GridChange) % 2) / 2 & Rigidplayer1.transform.position.x < Rigidplayer2.transform.position.x))
            {
                if (key == "W")
                {
                    key = "W1";
                }
                if (key == "W1")
                {
                    movement1 = Vector3.up;
                    movement2 = Vector3.left / 2;
                }
                else if (key == "E1")
                {
                    movement1 = new Vector3(0.5f, 1, 0);
                    movement2 = new Vector3(0.5f, 1, 0);
                }
                else if (key == "A1")
                {
                    movement1 = Vector3.left / 2;
                    movement2 = Vector3.left / 2;
                }
                else if (key == "D1")
                {
                    movement1 = Vector3.right / 2;
                    movement2 = Vector3.right / 2;
                }
                else if (key == "Z1")
                {
                    movement1 = new Vector3(-0.5f, -1, 0);
                    movement2 = Vector3.down;
                }
                else if (key == "X1")
                {
                    movement1 = Vector3.right / 2;
                    movement2 = Vector3.down;
                }
                if (Rigidplayer1.transform.position.x > Rigidplayer2.transform.position.x)
                {
                    movement1 += movement2;
                    movement2 = movement1 - movement2;
                    movement1 -= movement2;
                }
            }
        }

        var moveSpeed1 = 6 * (Mathf.Sqrt((movement1.x) * (movement1.x) + (movement1.y) * (movement1.y)));
        var moveSpeed2 = 6 * (Mathf.Sqrt((movement2.x) * (movement2.x) + (movement2.y) * (movement2.y)));

        var targetpos1 = Rigidplayer1.transform.position + movement1;
        var targetpos2 = Rigidplayer2.transform.position + movement2;

        if (Rigidplayer1.transform.position.z == 1f && Rigidplayer2.transform.position.z == 1f)
        {
            RaycastHit2D[] hit1_ = (Physics2D.RaycastAll(Rigidplayer1.transform.position, movement1, Mathf.Sqrt((movement1.x) * (movement1.x) + (movement1.y) * (movement1.y)), Hole, Rigidplayer1.transform.position.z, Rigidplayer1.transform.position.z));
            if (((((movement1 == Vector3.right || movement1 == Vector3.left) && Rigidplayer1.transform.position.y >= GridChange && movement1.magnitude == 1) || movement1.magnitude > 1) && hit1_.Length < 2) || hit1_.Length < 1)
            {
                return false;
            }
            RaycastHit2D[] hit2_ = (Physics2D.RaycastAll(Rigidplayer2.transform.position, movement2, Mathf.Sqrt((movement2.x) * (movement2.x) + (movement2.y) * (movement2.y)), Hole, Rigidplayer2.transform.position.z, Rigidplayer2.transform.position.z));
            if (((((movement2 == Vector3.right || movement2 == Vector3.left) && Rigidplayer2.transform.position.y >= GridChange && movement2.magnitude == 1) || movement2.magnitude > 1) && hit2_.Length < 2) || hit2_.Length < 1)
            {
                return false;
            }
        }
        RaycastHit2D hit1 = (Physics2D.Raycast(Rigidplayer1.transform.position, movement1, Mathf.Sqrt((movement1.x) * (movement1.x) + (movement1.y) * (movement1.y)), blockingLayer, Rigidplayer1.transform.position.z, Rigidplayer1.transform.position.z));
        RaycastHit2D hit2 = (Physics2D.Raycast(Rigidplayer2.transform.position, movement2, Mathf.Sqrt((movement2.x) * (movement2.x) + (movement2.y) * (movement2.y)), blockingLayer, Rigidplayer2.transform.position.z, Rigidplayer2.transform.position.z));


        if (!hit1 || hit1.collider.transform.root == transform)
        {
            move1 = true;
        }
        else if (Rigidplayer1.transform.position.y >= GridChange && Rigidplayer2.transform.position.y >= GridChange && movement1 == movement2 && movement2 != Vector3.left / 2 && movement2 != Vector3.right / 2 && Mathf.Abs(Mathf.Abs(Rigidplayer1.transform.position.x - hit1.collider.transform.position.x) * 2 + Mathf.Abs(Rigidplayer1.transform.position.y - hit1.collider.transform.position.y)) <= 1 && hit1.collider.transform.position.y >= GridChange && hit1.collider.CompareTag("Box"))
        {
            var box1 = hit1.collider.GetComponent<BoxMovement>();
            if (box1 != null && box1.TryToMultipushBox(movement1, 0, movement1, 0, key, true))
            {
                if (box1 != null && box1.TryToMultipushBox(movement1, 0, movement1, 0, key, check))
                {
                    move1 = true;
                }
            }
        }
        else if (Rigidplayer1.transform.position.y >= GridChange && Rigidplayer2.transform.position.y >= GridChange && movement1 == movement2 && movement2 != Vector3.left / 2 && movement2 != Vector3.right / 2 && Mathf.Abs(Mathf.Abs(Rigidplayer1.transform.position.x - hit1.collider.transform.position.x) * 2 + Mathf.Abs(Rigidplayer1.transform.position.y - hit1.collider.transform.position.y)) <= 1 && hit1.collider.transform.position.y >= GridChange && hit1.collider.CompareTag("Rigidbox"))
        {
            var rigidbox1 = hit1.collider.transform.parent.GetComponent<RigidboxMovement>();
            if (rigidbox1 != null && rigidbox1.TryToMultipushRigidbox(movement1, 0, movement1, 0, key, true))
            {
                if (rigidbox1 != null && rigidbox1.TryToMultipushRigidbox(movement1, 0, movement1, 0, key, check))
                {
                    move1 = true;
                }
            }
        }
        else if (hit1.collider.CompareTag("Box"))
        {
            var box1 = hit1.collider.GetComponent<BoxMovement>();
            if ((Rigidplayer1.transform.position.y == GridChange && Rigidplayer2.transform.position.y == GridChange) && ((Mathf.Abs(Rigidplayer1.transform.position.x) % 1 == ((Rigidplayer1.transform.position.y - GridChange) % 2) / 2 & Rigidplayer2.transform.position.x > Rigidplayer1.transform.position.x) || (Mathf.Abs(Rigidplayer2.transform.position.x) % 1 == ((Rigidplayer2.transform.position.y - GridChange) % 2) / 2 & Rigidplayer1.transform.position.x > Rigidplayer2.transform.position.x)) && hit1.collider.transform.position.y == GridChange && hit1.collider.transform.position.y >= GridChange && key == "X1")
            {
                if (box1 != null && box1.TryToPushBox(box1.transform.position, "D1", true, false))
                {
                    if (box1 != null && box1.TryToPushBox(box1.transform.position, "D1", check, false))
                    {
                        move1 = true;
                    }
                }
            }
            else if ((Rigidplayer1.transform.position.y == GridChange && Rigidplayer2.transform.position.y == GridChange) && ((Mathf.Abs(Rigidplayer1.transform.position.x) % 1 == ((Rigidplayer1.transform.position.y - GridChange) % 2) / 2 & Rigidplayer2.transform.position.x < Rigidplayer1.transform.position.x) || (Mathf.Abs(Rigidplayer2.transform.position.x) % 1 == ((Rigidplayer2.transform.position.y - GridChange) % 2) / 2 & Rigidplayer1.transform.position.x < Rigidplayer2.transform.position.x)) && hit1.collider.transform.position.y == GridChange && hit1.collider.transform.position.y >= GridChange && key == "Z1")
            {
                if (box1 != null && box1.TryToPushBox(box1.transform.position, "A1", true, false))
                {
                    if (box1 != null && box1.TryToPushBox(box1.transform.position, "A1", check, false))
                    {
                        move1 = true;
                    }
                }
            }
            else if (box1 != null && box1.TryToPushBox(box1.transform.position, key, true, false))
            {
                if (box1 != null && box1.TryToPushBox(box1.transform.position, key, check, false))
                {
                    move1 = true;
                }
            }
        }
        else if (hit1.collider.CompareTag("Rigidbox"))
        {
            var rigidbox1 = hit1.collider.transform.parent.GetComponent<RigidboxMovement>();
            if ((Rigidplayer1.transform.position.y == GridChange && Rigidplayer2.transform.position.y == GridChange) && ((Mathf.Abs(Rigidplayer1.transform.position.x) % 1 == ((Rigidplayer1.transform.position.y - GridChange) % 2) / 2 & Rigidplayer2.transform.position.x > Rigidplayer1.transform.position.x) || (Mathf.Abs(Rigidplayer2.transform.position.x) % 1 == ((Rigidplayer2.transform.position.y - GridChange) % 2) / 2 && Rigidplayer1.transform.position.x > Rigidplayer2.transform.position.x)) && hit1.collider.transform.position.y == GridChange && hit1.collider.transform.position.y >= GridChange && key == "X1")
            {
                if (rigidbox1 != null && rigidbox1.TryToPushRigidbox("D1", true))
                {
                    if (rigidbox1 != null && rigidbox1.TryToPushRigidbox("D1", check))
                    {
                        move1 = true;
                    }
                }
            }
            else if ((Rigidplayer1.transform.position.y == GridChange && Rigidplayer2.transform.position.y == GridChange) && ((Mathf.Abs(Rigidplayer1.transform.position.x) % 1 == ((Rigidplayer1.transform.position.y - GridChange) % 2) / 2 & Rigidplayer2.transform.position.x < Rigidplayer1.transform.position.x) || (Mathf.Abs(Rigidplayer2.transform.position.x) % 1 == ((Rigidplayer2.transform.position.y - GridChange) % 2) / 2 && Rigidplayer1.transform.position.x < Rigidplayer2.transform.position.x)) && hit1.collider.transform.position.y == GridChange && hit1.collider.transform.position.y >= GridChange && key == "Z1")
            {
                if (rigidbox1 != null && rigidbox1.TryToPushRigidbox("A1", true))
                {
                    if (rigidbox1 != null && rigidbox1.TryToPushRigidbox("A1", check))
                    {
                        move1 = true;
                    }
                }
            }

            else if (rigidbox1 != null && rigidbox1.TryToPushRigidbox(key, true))
            {
                if (rigidbox1 != null && rigidbox1.TryToPushRigidbox(key, check))
                {
                    move1 = true;
                }
            }
        }
        else if (Rigidplayer1.transform.position.y >= GridChange && Rigidplayer2.transform.position.y >= GridChange && movement1 == movement2 && movement2 != Vector3.left / 2 && movement2 != Vector3.right / 2 && Mathf.Abs(Mathf.Abs(Rigidplayer1.transform.position.x - hit1.collider.transform.position.x) * 2 + Mathf.Abs(Rigidplayer1.transform.position.y - hit1.collider.transform.position.y)) <= 1 && hit1.collider.transform.position.y >= GridChange && hit1.collider.CompareTag("Player"))
        {
            var player1 = hit1.collider.transform.GetComponent<PlayerMovement>();
            if (player1 != null && player1.TryToPushPlayer(movement1, 0, movement1, 0, key, true, false))
            {
                if (player1 != null && player1.TryToPushPlayer(movement1, 0, movement1, 0, key, check, false))
                {
                    player1.isPushed = true;
                    move1 = true;
                }
            }
        }
        else if (hit1.collider.CompareTag("Player"))
        {
            var player1 = hit1.collider.transform.GetComponent<PlayerMovement>();
            Debug.Log(player1.gameObject.name);
            if (Rigidplayer1.transform.position.y == GridChange && player1.transform.position.y == GridChange - 1 && player1.TryToPushPlayer(Vector3.down, 0, Vector3.down, 0, key, true, false))
            {
                if (player1.TryToPushPlayer(Vector3.down, 0, Vector3.down, 0, key, check, false))
                {
                    move1 = true;
                    Debug.Log(move1);
                }
            }
            else if ((Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.X)) && key == "S" && player1.TryToPushPlayer(Vector3.down, 0, Vector3.down, 0, key, true, false))
            {
                if (player1.TryToPushPlayer(Vector3.down, 0, Vector3.down, 0, key, check, false))
                {
                    move1 = true;
                    Debug.Log(move1);
                }
            }
            else if (player1.TryToMove(player1.transform.position, key, true, false) || player1.TryToMove(player1.transform.position, key += "1", true, false) || player1.TryToMove(player1.transform.position, key.Replace("1", string.Empty), true, false))
            {
                move1 = true;
                Debug.Log(move1);
            }
        }
        if (!hit2 || hit2.collider.transform.root == transform)
        {
            move2 = true;
        }
        else if (Rigidplayer1.transform.position.y >= GridChange && Rigidplayer2.transform.position.y >= GridChange && movement1 == movement2 && movement2 != Vector3.left / 2 && movement2 != Vector3.right / 2 && Mathf.Abs(Mathf.Abs(Rigidplayer2.transform.position.x - hit2.collider.transform.position.x) * 2 + Mathf.Abs(Rigidplayer2.transform.position.y - hit2.collider.transform.position.y)) <= 1 && hit2.collider.transform.position.y >= GridChange && hit2.collider.CompareTag("Box"))
        {
            var box2 = hit2.collider.GetComponent<BoxMovement>();
            if (box2 != null && box2.TryToMultipushBox(movement1, 0, movement1, 0, key, true))
            {
                if (box2 != null && box2.TryToMultipushBox(movement1, 0, movement1, 0, key, check))
                {
                    move2 = true;
                }
            }
        }
        else if (Rigidplayer1.transform.position.y >= GridChange && Rigidplayer2.transform.position.y >= GridChange && movement1 == movement2 && movement2 != Vector3.left / 2 && movement2 != Vector3.right / 2 && Mathf.Abs(Mathf.Abs(Rigidplayer2.transform.position.x - hit2.collider.transform.position.x) * 2 + Mathf.Abs(Rigidplayer2.transform.position.y - hit2.collider.transform.position.y)) <= 1 && hit2.collider.transform.position.y >= GridChange && hit2.collider.CompareTag("Rigidbox"))
        {
            var rigidbox2 = hit2.collider.transform.parent.GetComponent<RigidboxMovement>();
            if (rigidbox2 != null && rigidbox2.TryToMultipushRigidbox(movement1, 0, movement1, 0, key, true))
            {
                if (rigidbox2 != null && rigidbox2.TryToMultipushRigidbox(movement1, 0, movement1, 0, key, check))
                {
                    move2 = true;
                }
            }
        }

        else if (hit2.collider.CompareTag("Box"))
        {
            var box2 = hit2.collider.GetComponent<BoxMovement>();
            if ((Rigidplayer1.transform.position.y == GridChange && Rigidplayer2.transform.position.y == GridChange) && ((Mathf.Abs(Rigidplayer1.transform.position.x) % 1 == ((Rigidplayer1.transform.position.y - GridChange) % 2) / 2 & Rigidplayer2.transform.position.x > Rigidplayer1.transform.position.x) || (Mathf.Abs(Rigidplayer2.transform.position.x) % 1 == ((Rigidplayer2.transform.position.y - GridChange) % 2) / 2 & Rigidplayer1.transform.position.x > Rigidplayer2.transform.position.x)) && hit2.collider.transform.position.y == GridChange && key == "X1")
            {
                if (box2 != null && box2.TryToPushBox(box2.transform.position, "D1", true, false))
                {
                    if (box2 != null && box2.TryToPushBox(box2.transform.position, "D1", check, false))
                    {
                        move2 = true;
                    }
                }
            }
            else if ((Rigidplayer1.transform.position.y == GridChange && Rigidplayer2.transform.position.y == GridChange) && ((Mathf.Abs(Rigidplayer1.transform.position.x) % 1 == ((Rigidplayer1.transform.position.y - GridChange) % 2) / 2 & Rigidplayer2.transform.position.x < Rigidplayer1.transform.position.x) || (Mathf.Abs(Rigidplayer2.transform.position.x) % 1 == ((Rigidplayer2.transform.position.y - GridChange) % 2) / 2 & Rigidplayer1.transform.position.x < Rigidplayer2.transform.position.x)) && hit2.collider.transform.position.y == GridChange && key == "Z1")
            {
                if (box2 != null && box2.TryToPushBox(box2.transform.position, "A1", true, false))
                {
                    if (box2 != null && box2.TryToPushBox(box2.transform.position, "A1", check, false))
                    {
                        move2 = true;
                    }
                }
            }
            else if (box2 != null && box2.TryToPushBox(box2.transform.position, key, true, false))
            {
                if (box2 != null && box2.TryToPushBox(box2.transform.position, key, check, false))
                {
                    move2 = true;
                }
            }
        }
        else if (hit2.collider.CompareTag("Rigidbox"))
        {
            var rigidbox2 = hit2.collider.transform.parent.GetComponent<RigidboxMovement>();
            if ((Rigidplayer1.transform.position.y == GridChange && Rigidplayer2.transform.position.y == GridChange) && ((Mathf.Abs(Rigidplayer1.transform.position.x) % 1 == ((Rigidplayer1.transform.position.y - GridChange) % 2) / 2 && Rigidplayer2.transform.position.x > Rigidplayer1.transform.position.x) || (Mathf.Abs(Rigidplayer2.transform.position.x) % 1 == ((Rigidplayer2.transform.position.y - GridChange) % 2) / 2 & Rigidplayer1.transform.position.x > Rigidplayer2.transform.position.x)) && hit2.collider.transform.position.y >= GridChange && key == "X1")
            {
                if (rigidbox2 != null && rigidbox2.TryToPushRigidbox("D1", true))
                {
                    if (rigidbox2 != null && rigidbox2.TryToPushRigidbox("D1", check))
                    {
                        move2 = true;
                    }
                }
            }
            else if ((Rigidplayer1.transform.position.y == GridChange && Rigidplayer2.transform.position.y == GridChange) && ((Mathf.Abs(Rigidplayer1.transform.position.x) % 1 == ((Rigidplayer1.transform.position.y - GridChange) % 2) / 2 & Rigidplayer2.transform.position.x < Rigidplayer1.transform.position.x) || (Mathf.Abs(Rigidplayer2.transform.position.x) % 1 == ((Rigidplayer2.transform.position.y - GridChange) % 2) / 2 & Rigidplayer1.transform.position.x < Rigidplayer2.transform.position.x)) && hit2.collider.transform.position.y >= GridChange && key == "Z1")
            {
                if (rigidbox2 != null && rigidbox2.TryToPushRigidbox("A1", true))
                {
                    if (rigidbox2 != null && rigidbox2.TryToPushRigidbox("A1", check))
                    {
                        move2 = true;
                    }
                }
            }
            else if (rigidbox2 != null && rigidbox2.TryToPushRigidbox(key, true))
            {
                if (rigidbox2 != null && rigidbox2.TryToPushRigidbox(key, check))
                {
                    move2 = true;
                }
            }

        }
        else if (Rigidplayer1.transform.position.y >= GridChange && Rigidplayer2.transform.position.y >= GridChange && movement1 == movement2 && movement2 != Vector3.left / 2 && movement2 != Vector3.right / 2 && Mathf.Abs(Mathf.Abs(Rigidplayer2.transform.position.x - hit2.collider.transform.position.x) * 2 + Mathf.Abs(Rigidplayer2.transform.position.y - hit2.collider.transform.position.y)) <= 1 && hit2.collider.transform.position.y >= GridChange && hit2.collider.CompareTag("Player"))
        {
            var player2 = hit2.collider.transform.GetComponent<PlayerMovement>();
            Debug.Log("Pushing player");
            if (player2 != null && player2.TryToPushPlayer(movement1, 0, movement1, 0, key, true, false))
            {
                if (player2 != null && player2.TryToPushPlayer(movement1, 0, movement1, 0, key, check, false))
                {
                    player2.isPushed = true;
                    move2 = true;
                }
            }
        }
        else if (hit2.collider.CompareTag("Player"))
        {
            var player2 = hit2.collider.transform.GetComponent<PlayerMovement>();
            Debug.Log(player2.gameObject.name);

            if (Rigidplayer1.transform.position.y == GridChange && player2.transform.position.y == GridChange - 1 && player2.TryToPushPlayer(Vector3.down, 0, Vector3.down, 0, key, true, false))
            {
                if (player2.TryToPushPlayer(Vector3.down, 0, Vector3.down, 0, key, check, false))
                {
                    move2 = true;
                    Debug.Log(move2);
                }
            }
            else if ((Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.X)) && key == "S" && player2.TryToPushPlayer(Vector3.down, 0, Vector3.down, 0, key, true, false))
            {
                if (player2.TryToPushPlayer(Vector3.down, 0, Vector3.down, 0, key, check, false))
                {
                    move2 = true;
                    Debug.Log(move2);
                }
            }
            else if (player2.TryToMove(player2.transform.position, key, true, false) || player2.TryToMove(player2.transform.position, key += "1", true, false) || player2.TryToMove(player2.transform.position, key.Replace("1", string.Empty), true, false))
            {
                move2 = true;
                Debug.Log(move2);
            }
        }
        if (move1 && move2)
        {
            if (!check)
            {
                StartCoroutine(MoveToPosition(targetpos1, moveSpeed1, targetpos2, moveSpeed2, true, false, false));
            }
            return true;
        }

        return false;
    }
    public bool TryToPushRigidplayer(string key, bool check)
    {
        if (isMoving1 || isMoving2) return true;
        if (!won1 || !won2) return false;
        var push1 = Vector3.zero;
        var push2 = Vector3.zero;
        var move1 = false;
        var move2 = false;
        if (Rigidplayer1.transform.position.y < GridChange && Rigidplayer2.transform.position.y < GridChange)
        {
            if (key == "X1" || key == "Z1")
            {
                key = "S";
            }
            if (key == "A1")
            {
                key = "A";
            }
            if (key == "D1")
            {
                key = "D";
            }
            if (key == "W")
            {
                push1 = Vector3.up;
                push2 = Vector3.up;
            }
            else if (key == "A")
            {
                push1 = Vector3.left;
                push2 = Vector3.left;
            }
            else if (key == "S")
            {
                push1 = Vector3.down;
                push2 = Vector3.down;
            }
            else if (key == "D")
            {
                push1 = Vector3.right;
                push2 = Vector3.right;
            }

        }
        else if (Rigidplayer1.transform.position.y > GridChange || Rigidplayer2.transform.position.y > GridChange)
        {
            if (key == "W")
            {
                key = "W1";
            }
            if (key == "A")
            {
                key = "A1";
            }
            if (key == "D")
            {
                key = "D1";
            }
            if (Rigidplayer1.transform.position.y != Rigidplayer2.transform.position.y)
            {
                if (key == "W1")
                {
                    push1 = Vector3.up;
                    push2 = Vector3.left / 2;
                }
                else if (key == "E1")
                {
                    push1 = Vector3.up;
                    push2 = Vector3.right / 2;
                }
                else if (key == "A1")
                {
                    push1 = Vector3.left;
                    push2 = Vector3.left;
                }
                else if (key == "D1")
                {
                    push1 = Vector3.right;
                    push2 = Vector3.right;
                }
                else if (key == "Z1")
                {
                    push1 = Vector3.left / 2;
                    push2 = Vector3.down;
                }
                else if (key == "X1")
                {
                    push1 = Vector3.right / 2;
                    push2 = Vector3.down;
                }
                if (Rigidplayer1.transform.position.y > Rigidplayer2.transform.position.y)
                {
                    push1 += push2;
                    push2 = push1 - push2;
                    push1 -= push2;
                }
            }
            else if ((Mathf.Abs(Rigidplayer1.transform.position.x) % 1 == ((Rigidplayer1.transform.position.y - GridChange) % 2) / 2 & Rigidplayer2.transform.position.x > Rigidplayer1.transform.position.x) || (Mathf.Abs(Rigidplayer2.transform.position.x) % 1 == ((Rigidplayer2.transform.position.y - GridChange) % 2) / 2 & Rigidplayer1.transform.position.x > Rigidplayer2.transform.position.x))
            {
                if (key == "W1")
                {
                    push1 = new Vector3(-0.5f, 1, 0);
                    push2 = new Vector3(-0.5f, 1, 0);
                }
                else if (key == "E1")
                {
                    push1 = Vector3.up;
                    push2 = Vector3.right / 2;
                }
                else if (key == "A1")
                {
                    push1 = Vector3.left / 2;
                    push2 = Vector3.left / 2;
                }
                else if (key == "D1")
                {
                    push1 = Vector3.right / 2;
                    push2 = Vector3.right / 2;
                }
                else if (key == "Z1")
                {
                    push1 = Vector3.left / 2;
                    push2 = Vector3.down;
                }
                else if (key == "X1")
                {
                    push1 = new Vector3(0.5f, -1, 0);
                    push2 = new Vector3(0.5f, -1, 0);
                }
                if (Rigidplayer1.transform.position.x < Rigidplayer2.transform.position.x)
                {
                    push1 += push2;
                    push2 = push1 - push2;
                    push1 -= push2;
                }
            }
            else if ((Mathf.Abs(Rigidplayer1.transform.position.x) % 1 == ((Rigidplayer1.transform.position.y - GridChange) % 2) / 2 & Rigidplayer2.transform.position.x < Rigidplayer1.transform.position.x) || (Mathf.Abs(Rigidplayer2.transform.position.x) % 1 == ((Rigidplayer2.transform.position.y - GridChange) % 2) / 2 & Rigidplayer1.transform.position.x < Rigidplayer2.transform.position.x))
            {
                if (key == "W1")
                {
                    push1 = Vector3.up;
                    push2 = Vector3.left / 2;
                }
                else if (key == "E1")
                {
                    push1 = new Vector3(0.5f, 1, 0);
                    push2 = new Vector3(0.5f, 1, 0);
                }
                else if (key == "A1")
                {
                    push1 = Vector3.left / 2;
                    push2 = Vector3.left / 2;
                }
                else if (key == "D1")
                {
                    push1 = Vector3.right / 2;
                    push2 = Vector3.right / 2;
                }
                else if (key == "Z1")
                {
                    push1 = new Vector3(-0.5f, -1, 0);
                    push2 = new Vector3(-0.5f, -1, 0);
                }
                else if (key == "X1")
                {
                    push1 = Vector3.right / 2;
                    push2 = Vector3.down;
                }
                if (Rigidplayer1.transform.position.x > Rigidplayer2.transform.position.x)
                {
                    push1 += push2;
                    push2 = push1 - push2;
                    push1 -= push2;
                }
            }
        }
        else if ((Rigidplayer1.transform.position.y == GridChange && Rigidplayer2.transform.position.y < GridChange) || (Rigidplayer1.transform.position.y < GridChange && Rigidplayer2.transform.position.y == GridChange))
        {
            if (key == "W")
            {
                push1 = Vector3.left / 2;
                push2 = Vector3.up;
            }
            if (key == "A" || key == "A1")
            {
                push1 = Vector3.left;
                push2 = Vector3.left;
            }
            if (key == "D" || key == "D1")
            {
                push1 = Vector3.right;
                push2 = Vector3.right;
            }
            if (key == "Z1" || key == "X1")
            {
                push1 = Vector3.down;
                push2 = Vector3.down;
            }
            if (Rigidplayer1.transform.position.y < GridChange && Rigidplayer2.transform.position.y >= GridChange)
            {
                push1 += push2;
                push2 = push1 - push2;
                push1 -= push2;

            }
        }
        else if (Rigidplayer1.transform.position.y == GridChange && Rigidplayer2.transform.position.y == GridChange)
        {
            if ((Mathf.Abs(Rigidplayer1.transform.position.x) % 1 == ((Rigidplayer1.transform.position.y - GridChange) % 2) / 2 & Rigidplayer2.transform.position.x > Rigidplayer1.transform.position.x) || (Mathf.Abs(Rigidplayer2.transform.position.x) % 1 == ((Rigidplayer2.transform.position.y - GridChange) % 2) / 2 & Rigidplayer1.transform.position.x > Rigidplayer2.transform.position.x))
            {
                if (key == "W")
                {
                    key = "W1";
                }
                if (key == "W1")
                {
                    push1 = new Vector3(-0.5f, 1, 0);
                    push2 = new Vector3(-0.5f, 1, 0);
                }
                else if (key == "A1")
                {
                    push1 = Vector3.left / 2;
                    push2 = Vector3.left / 2;
                }
                else if (key == "D1")
                {
                    push1 = Vector3.right / 2;
                    push2 = Vector3.right / 2;

                }
                else if (key == "Z1")
                {
                    push1 = Vector3.left / 2;
                    push2 = Vector3.down;
                }
                else if (key == "X1")
                {
                    push1 = new Vector3(0.5f, -1, 0);
                    push2 = Vector3.down;
                }
                if (Rigidplayer1.transform.position.x < Rigidplayer2.transform.position.x)
                {
                    push1 += push2;
                    push2 = push1 - push2;
                    push1 -= push2;
                }
            }
            else if ((Mathf.Abs(Rigidplayer1.transform.position.x) % 1 == ((Rigidplayer1.transform.position.y - GridChange) % 2) / 2 && Rigidplayer2.transform.position.x < Rigidplayer1.transform.position.x) || (Mathf.Abs(Rigidplayer2.transform.position.x) % 1 == ((Rigidplayer2.transform.position.y - GridChange) % 2) / 2 & Rigidplayer1.transform.position.x < Rigidplayer2.transform.position.x))
            {
                if (key == "W")
                {
                    key = "W1";
                }
                if (key == "W1")
                {
                    push1 = Vector3.up;
                    push2 = Vector3.left / 2;
                }
                else if (key == "E1")
                {
                    push1 = new Vector3(0.5f, 1, 0);
                    push2 = new Vector3(0.5f, 1, 0);
                }
                else if (key == "A1")
                {
                    push1 = Vector3.left / 2;
                    push2 = Vector3.left / 2;
                }
                else if (key == "D1")
                {
                    push1 = Vector3.right / 2;
                    push2 = Vector3.right / 2;
                }
                else if (key == "Z1")
                {
                    push1 = new Vector3(-0.5f, -1, 0);
                    push2 = Vector3.down;
                }
                else if (key == "X1")
                {
                    push1 = Vector3.right / 2;
                    push2 = Vector3.down;
                }
                if (Rigidplayer1.transform.position.x > Rigidplayer2.transform.position.x)
                {
                    push1 += push2;
                    push2 = push1 - push2;
                    push1 -= push2;
                }
            }
        }

        var pushSpeed1 = 6 * (Mathf.Sqrt((push1.x) * (push1.x) + (push1.y) * (push1.y)));
        var pushSpeed2 = 6 * (Mathf.Sqrt((push2.x) * (push2.x) + (push2.y) * (push2.y)));

        var targetpos1 = Rigidplayer1.transform.position + push1;
        var targetpos2 = Rigidplayer2.transform.position + push2;

        if (Rigidplayer1.transform.position.z == 1f && Rigidplayer2.transform.position.z == 1f)
        {
            RaycastHit2D hit01 = (Physics2D.Raycast(Rigidplayer1.transform.position, push1, Mathf.Sqrt((push1.x) * (push1.x) + (push1.y) * (push1.y)), Hole, transform.position.z, Rigidplayer1.transform.position.z));
            RaycastHit2D hit02 = (Physics2D.Raycast(Rigidplayer2.transform.position, push2, Mathf.Sqrt((push2.x) * (push2.x) + (push2.y) * (push2.y)), Hole, transform.position.z, Rigidplayer2.transform.position.z));
            if (!hit01 || !hit02)
            {
                return false;
            }
        }

        RaycastHit2D hit1 = (Physics2D.Raycast(Rigidplayer1.transform.position, push1, Mathf.Sqrt((push1.x) * (push1.x) + (push1.y) * (push1.y)), blockingLayer, Rigidplayer1.transform.position.z, Rigidplayer1.transform.position.z));
        RaycastHit2D hit2 = (Physics2D.Raycast(Rigidplayer2.transform.position, push2, Mathf.Sqrt((push2.x) * (push2.x) + (push2.y) * (push2.y)), blockingLayer, Rigidplayer2.transform.position.z, Rigidplayer2.transform.position.z));


        if (!hit1 || hit1.collider.transform.root == transform)
        {
            move1 = true;
        }
        else if (Rigidplayer1.transform.position.y >= GridChange && Rigidplayer2.transform.position.y >= GridChange && push1 == push2 && push2 != Vector3.left / 2 && push2 != Vector3.right / 2 && Mathf.Abs(Mathf.Abs(Rigidplayer1.transform.position.x - hit1.collider.transform.position.x) * 2 + Mathf.Abs(Rigidplayer1.transform.position.y - hit1.collider.transform.position.y)) <= 1 && hit1.collider.transform.position.y >= GridChange && hit1.collider.CompareTag("Box"))
        {
            var box1 = hit1.collider.GetComponent<BoxMovement>();
            if (box1 != null && box1.TryToMultipushBox(push1, 0, push1, 0, key, check))
            {
                move1 = true;
            }
        }
        else if (Rigidplayer1.transform.position.y >= GridChange && Rigidplayer2.transform.position.y >= GridChange && push1 == push2 && push2 != Vector3.left / 2 && push2 != Vector3.right / 2 && Mathf.Abs(Mathf.Abs(Rigidplayer1.transform.position.x - hit1.collider.transform.position.x) * 2 + Mathf.Abs(Rigidplayer1.transform.position.y - hit1.collider.transform.position.y)) <= 1 && hit1.collider.transform.position.y >= GridChange && hit1.collider.CompareTag("Rigidbox"))
        {
            var rigidbox1 = hit1.collider.transform.parent.GetComponent<RigidboxMovement>();
            if (rigidbox1 != null && rigidbox1.TryToMultipushRigidbox(push1, 0, push1, 0, key, check))
            {
                move1 = true;
            }
        }
        else if (hit1.collider.CompareTag("Box"))
        {
            var box1 = hit1.collider.GetComponent<BoxMovement>();
            if ((Rigidplayer1.transform.position.y == GridChange && Rigidplayer2.transform.position.y == GridChange) && ((Mathf.Abs(Rigidplayer1.transform.position.x) % 1 == ((Rigidplayer1.transform.position.y - GridChange) % 2) / 2 & Rigidplayer2.transform.position.x > Rigidplayer1.transform.position.x) || (Mathf.Abs(Rigidplayer2.transform.position.x) % 1 == ((Rigidplayer2.transform.position.y - GridChange) % 2) / 2 & Rigidplayer1.transform.position.x > Rigidplayer2.transform.position.x)) && hit1.collider.transform.position.y == GridChange && hit1.collider.transform.position.y >= GridChange && key == "X1")
            {
                if (box1 != null && box1.TryToPushBox(box1.transform.position, "D1", check, false))
                {
                    move1 = true;
                }
            }
            else if ((Rigidplayer1.transform.position.y == GridChange && Rigidplayer2.transform.position.y == GridChange) && ((Mathf.Abs(Rigidplayer1.transform.position.x) % 1 == ((Rigidplayer1.transform.position.y - GridChange) % 2) / 2 & Rigidplayer2.transform.position.x < Rigidplayer1.transform.position.x) || (Mathf.Abs(Rigidplayer2.transform.position.x) % 1 == ((Rigidplayer2.transform.position.y - GridChange) % 2) / 2 & Rigidplayer1.transform.position.x < Rigidplayer2.transform.position.x)) && hit1.collider.transform.position.y == GridChange && hit1.collider.transform.position.y >= GridChange && key == "Z1")
            {
                if (box1 != null && box1.TryToPushBox(box1.transform.position, "A1", check, false))
                {
                    move1 = true;
                }
            }
            else if (box1 != null && box1.TryToPushBox(box1.transform.position, key, check, false))
            {
                move1 = true;
            }
        }
        else if (hit1.collider.CompareTag("Rigidbox"))
        {
            var rigidbox1 = hit1.collider.transform.parent.GetComponent<RigidboxMovement>();
            if ((Rigidplayer1.transform.position.y == GridChange && Rigidplayer2.transform.position.y == GridChange) && ((Mathf.Abs(Rigidplayer1.transform.position.x) % 1 == ((Rigidplayer1.transform.position.y - GridChange) % 2) / 2 & Rigidplayer2.transform.position.x > Rigidplayer1.transform.position.x) || (Mathf.Abs(Rigidplayer2.transform.position.x) % 1 == ((Rigidplayer2.transform.position.y - GridChange) % 2) / 2 & Rigidplayer1.transform.position.x > Rigidplayer2.transform.position.x)) && hit1.collider.transform.position.y == GridChange && hit1.collider.transform.position.y >= GridChange && key == "X1")
            {
                if (rigidbox1 != null && rigidbox1.TryToPushRigidbox("D1", check))
                {
                    move1 = true;
                }
            }
            else if ((Rigidplayer1.transform.position.y == GridChange && Rigidplayer2.transform.position.y == GridChange) && ((Mathf.Abs(Rigidplayer1.transform.position.x) % 1 == ((Rigidplayer1.transform.position.y - GridChange) % 2) / 2 & Rigidplayer2.transform.position.x < Rigidplayer1.transform.position.x) || (Mathf.Abs(Rigidplayer2.transform.position.x) % 1 == ((Rigidplayer2.transform.position.y - GridChange) % 2) / 2 & Rigidplayer1.transform.position.x < Rigidplayer2.transform.position.x)) && hit1.collider.transform.position.y == GridChange && hit1.collider.transform.position.y >= GridChange && key == "Z1")
            {
                if (rigidbox1 != null && rigidbox1.TryToPushRigidbox("A1", check))
                {
                    move1 = true;
                }
            }
            else if (rigidbox1 != null && rigidbox1.TryToPushRigidbox(key, check))
            {
                move1 = true;
            }
        }
        else if (Rigidplayer1.transform.position.y >= GridChange && Rigidplayer2.transform.position.y >= GridChange && push1 == push2 && push2 != Vector3.left / 2 && push2 != Vector3.right / 2 && Mathf.Abs(Mathf.Abs(Rigidplayer1.transform.position.x - hit1.collider.transform.position.x) * 2 + Mathf.Abs(Rigidplayer1.transform.position.y - hit1.collider.transform.position.y)) <= 1 && hit1.collider.transform.position.y >= GridChange && hit1.collider.CompareTag("Player"))
        {
            var player1 = hit1.collider.transform.GetComponent<PlayerMovement>();
            if (player1 != null && player1.TryToPushPlayer(push1, 0, push1, 0, key, check, false))
            {
                player1.isPushed = true;
                move1 = true;
            }
        }
        else if (hit1.collider.CompareTag("Player"))
        {
            var player1 = hit1.collider.transform.GetComponent<PlayerMovement>();
            Debug.Log(player1.gameObject.name);
            if (Rigidplayer1.transform.position.y == GridChange && player1.transform.position.y == GridChange - 1 && player1.TryToPushPlayer(Vector3.down, 0, Vector3.down, 0, key, check, false))
            {
                move1 = true;
                Debug.Log(move1);
            }
            else if ((Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.X)) && key == "S" && player1.TryToPushPlayer(Vector3.down, 0, Vector3.down, 0, key, check, false))
            {
                move1 = true;
                Debug.Log(move1);
            }
            else if (player1.TryToMove(player1.transform.position, key, true, false) || player1.TryToMove(player1.transform.position, key += "1", true, false) || player1.TryToMove(player1.transform.position, key.Replace("1", string.Empty), true, false))
            {
                move1 = true;
                Debug.Log(move1);
            }
        }
        if (!hit2 || hit2.collider.transform.root == transform)
        {
            move2 = true;
        }
        else if (Rigidplayer1.transform.position.y >= GridChange && Rigidplayer2.transform.position.y >= GridChange && push1 == push2 && push2 != Vector3.left / 2 && push2 != Vector3.right / 2 && Mathf.Abs(Mathf.Abs(Rigidplayer2.transform.position.x - hit2.collider.transform.position.x) * 2 + Mathf.Abs(Rigidplayer2.transform.position.y - hit2.collider.transform.position.y)) <= 1 && hit2.collider.transform.position.y >= GridChange && hit2.collider.CompareTag("Box"))
        {
            var box2 = hit2.collider.GetComponent<BoxMovement>();
            if (box2 != null && box2.TryToMultipushBox(push1, 0, push1, 0, key, check))
            {
                move2 = true;
            }
        }
        else if (Rigidplayer1.transform.position.y >= GridChange && Rigidplayer2.transform.position.y >= GridChange && push1 == push2 && push2 != Vector3.left / 2 && push2 != Vector3.right / 2 && Mathf.Abs(Mathf.Abs(Rigidplayer2.transform.position.x - hit2.collider.transform.position.x) * 2 + Mathf.Abs(Rigidplayer2.transform.position.y - hit2.collider.transform.position.y)) <= 1 && hit2.collider.transform.position.y >= GridChange && hit2.collider.CompareTag("Rigidbox"))
        {
            var rigidbox2 = hit2.collider.transform.parent.GetComponent<RigidboxMovement>();
            if (rigidbox2 != null && rigidbox2.TryToMultipushRigidbox(push1, 0, push1, 0, key, check))
            {
                move2 = true;
            }
        }

        else if (hit2.collider.CompareTag("Box"))
        {
            var box2 = hit2.collider.GetComponent<BoxMovement>();
            if ((Rigidplayer1.transform.position.y == GridChange && Rigidplayer2.transform.position.y == GridChange) && ((Mathf.Abs(Rigidplayer1.transform.position.x) % 1 == ((Rigidplayer1.transform.position.y - GridChange) % 2) / 2 & Rigidplayer2.transform.position.x > Rigidplayer1.transform.position.x) || (Mathf.Abs(Rigidplayer2.transform.position.x) % 1 == ((Rigidplayer2.transform.position.y - GridChange) % 2) / 2 & Rigidplayer1.transform.position.x > Rigidplayer2.transform.position.x)) && hit2.collider.transform.position.y == GridChange && key == "X1")
            {
                if (box2 != null && box2.TryToPushBox(box2.transform.position, "D1", check, false))
                {
                    move2 = true;
                }
            }
            else if ((Rigidplayer1.transform.position.y == GridChange && Rigidplayer2.transform.position.y == GridChange) && ((Mathf.Abs(Rigidplayer1.transform.position.x) % 1 == ((Rigidplayer1.transform.position.y - GridChange) % 2) / 2 & Rigidplayer2.transform.position.x < Rigidplayer1.transform.position.x) || (Mathf.Abs(Rigidplayer2.transform.position.x) % 1 == ((Rigidplayer2.transform.position.y - GridChange) % 2) / 2 & Rigidplayer1.transform.position.x < Rigidplayer2.transform.position.x)) && hit2.collider.transform.position.y == GridChange && key == "Z1")
            {
                if (box2 != null && box2.TryToPushBox(box2.transform.position, "A1", check, false))
                {
                    move2 = true;
                }
            }
            else if (box2 != null && box2.TryToPushBox(box2.transform.position, key, check, false))
            {
                move2 = true;
            }
        }
        else if (hit2.collider.CompareTag("Rigidbox"))
        {
            var rigidbox2 = hit2.collider.transform.parent.GetComponent<RigidboxMovement>();
            if ((Rigidplayer1.transform.position.y == GridChange && Rigidplayer2.transform.position.y == GridChange) && ((Mathf.Abs(Rigidplayer1.transform.position.x) % 1 == ((Rigidplayer1.transform.position.y - GridChange) % 2) / 2 && Rigidplayer2.transform.position.x > Rigidplayer1.transform.position.x) || (Mathf.Abs(Rigidplayer2.transform.position.x) % 1 == ((Rigidplayer2.transform.position.y - GridChange) % 2) / 2 & Rigidplayer1.transform.position.x > Rigidplayer2.transform.position.x)) && hit2.collider.transform.position.y >= GridChange && key == "X1")
            {
                if (rigidbox2 != null && rigidbox2.TryToPushRigidbox("D1", check))
                {
                    move2 = true;
                }
            }
            else if ((Rigidplayer1.transform.position.y == GridChange && Rigidplayer2.transform.position.y == GridChange) && ((Mathf.Abs(Rigidplayer1.transform.position.x) % 1 == ((Rigidplayer1.transform.position.y - GridChange) % 2) / 2 & Rigidplayer2.transform.position.x < Rigidplayer1.transform.position.x) || (Mathf.Abs(Rigidplayer2.transform.position.x) % 1 == ((Rigidplayer2.transform.position.y - GridChange) % 2) / 2 & Rigidplayer1.transform.position.x < Rigidplayer2.transform.position.x)) && hit2.collider.transform.position.y >= GridChange && key == "Z1")
            {
                if (rigidbox2 != null && rigidbox2.TryToPushRigidbox("A1", check))
                {
                    move2 = true;
                }
            }
            else if (rigidbox2 != null && rigidbox2.TryToPushRigidbox(key, check))
            {
                move2 = true;
            }

        }
        else if (Rigidplayer1.transform.position.y >= GridChange && Rigidplayer2.transform.position.y >= GridChange && push1 == push2 && push2 != Vector3.left / 2 && push2 != Vector3.right / 2 && Mathf.Abs(Mathf.Abs(Rigidplayer2.transform.position.x - hit2.collider.transform.position.x) * 2 + Mathf.Abs(Rigidplayer2.transform.position.y - hit2.collider.transform.position.y)) <= 1 && hit2.collider.transform.position.y >= GridChange && hit2.collider.CompareTag("Player"))
        {
            var player2 = hit2.collider.transform.GetComponent<PlayerMovement>();
            Debug.Log("Pushing player");
            if (player2 != null && player2.TryToPushPlayer(push1, 0, push1, 0, key, check, false))
            {
                player2.isPushed = true;
                move2 = true;
            }
        }
        else if (hit2.collider.CompareTag("Player"))
        {
            var player2 = hit2.collider.transform.GetComponent<PlayerMovement>();
            Debug.Log(player2.gameObject.name);
            
            if (Rigidplayer1.transform.position.y == GridChange && player2.transform.position.y == GridChange - 1 && player2.TryToPushPlayer(Vector3.down, 0, Vector3.down, 0, key, check, false))
            {
                move2 = true;
                Debug.Log(move2);
            }
            else if ((Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.X)) && key == "S" && player2.TryToPushPlayer(Vector3.down, 0, Vector3.down, 0, key, check, false))
            {
                move2 = true;
                Debug.Log(move2);
            }
            else if (player2.TryToMove(player2.transform.position, key, true, false) || player2.TryToMove(player2.transform.position, key += "1", true, false) || player2.TryToMove(player2.transform.position, key.Replace("1", string.Empty), true, false))
            {
                move2 = true;
                Debug.Log(move2);
            }
        }
        if (move1 && move2)
        {
            if (!check)
            {
                if (Rigidplayer1.transform.position.y == GridChange - 1 && Rigidplayer2.transform.position.y == GridChange - 1 && key == "W")
                {
                    destroy = true;
                }
                StartCoroutine(MoveToPosition(targetpos1, pushSpeed1, targetpos2, pushSpeed2, false, false, false));
            }
            return true;
        }

        return false;
    }
    public bool TryToMultipushRigidplayer(Vector3 movement, float MoveSpeed, Vector3 push, float PushSpeed, string key, bool check)
    {
        if (isMoving1 || isMoving2) return true;
        if (!won1 || !won2) return false;
        var move1 = false;
        var move2 = false;
        var push1 = push;
        var push2 = push;
        if (Rigidplayer1.transform.position.y == GridChange && Rigidplayer2.transform.position.y == GridChange && ((Mathf.Abs(Rigidplayer1.transform.position.x) % 1 == ((Rigidplayer1.transform.position.y - GridChange) % 2) / 2 && Rigidplayer1.transform.position.x > Rigidplayer2.transform.position.x) || (Mathf.Abs(Rigidplayer2.transform.position.x) % 1 == ((Rigidplayer2.transform.position.y - GridChange) % 2) / 2 && Rigidplayer2.transform.position.x > Rigidplayer1.transform.position.x)) && push == new Vector3(-0.5f, -1, 0))
        {
            push1 = Vector3.down;
            if (Rigidplayer1.transform.position.x < Rigidplayer2.transform.position.x)
            {
                push1 += push2;
                push2 = push1 - push2;
                push1 -= push2;
            }
        }
        else if (Rigidplayer1.transform.position.y == GridChange && Rigidplayer2.transform.position.y == GridChange && ((Mathf.Abs(Rigidplayer1.transform.position.x) % 1 == ((Rigidplayer1.transform.position.y - GridChange) % 2) / 2 && Rigidplayer1.transform.position.x > Rigidplayer2.transform.position.x) || (Mathf.Abs(Rigidplayer2.transform.position.x) % 1 == ((Rigidplayer2.transform.position.y - GridChange) % 2) / 2 && Rigidplayer2.transform.position.x > Rigidplayer1.transform.position.x)) && push == new Vector3(0.5f, -1, 0))
        {
            push1 = Vector3.down * 2;
            if (Rigidplayer1.transform.position.x < Rigidplayer2.transform.position.x)
            {
                push1 += push2;
                push2 = push1 - push2;
                push1 -= push2;
            }
        }
        else if (Rigidplayer1.transform.position.y == GridChange && Rigidplayer2.transform.position.y == GridChange && ((Mathf.Abs(Rigidplayer1.transform.position.x) % 1 == ((Rigidplayer1.transform.position.y - GridChange) % 2) / 2 && Rigidplayer1.transform.position.x < Rigidplayer2.transform.position.x) || (Mathf.Abs(Rigidplayer2.transform.position.x) % 1 == ((Rigidplayer2.transform.position.y - GridChange) % 2) / 2 && Rigidplayer2.transform.position.x < Rigidplayer1.transform.position.x)) && push == new Vector3(0.5f, -1, 0))
        {
            push1 = Vector3.down;
            if (Rigidplayer1.transform.position.x > Rigidplayer2.transform.position.x)
            {
                push1 += push2;
                push2 = push1 - push2;
                push1 -= push2;
            }
        }
        else if (Rigidplayer1.transform.position.y == GridChange && Rigidplayer2.transform.position.y == GridChange && ((Mathf.Abs(Rigidplayer1.transform.position.x) % 1 == ((Rigidplayer1.transform.position.y - GridChange) % 2) / 2 && Rigidplayer1.transform.position.x < Rigidplayer2.transform.position.x) || (Mathf.Abs(Rigidplayer2.transform.position.x) % 1 == ((Rigidplayer2.transform.position.y - GridChange) % 2) / 2 && Rigidplayer2.transform.position.x < Rigidplayer1.transform.position.x)) && push == new Vector3(-0.5f, -1, 0))
        {
            push1 = Vector3.down * 2;
            if (Rigidplayer1.transform.position.x > Rigidplayer2.transform.position.x)
            {
                push1 += push2;
                push2 = push1 - push2;
                push1 -= push2;
            }
        }
        var pushSpeed1 = 6 * (Mathf.Sqrt((push1.x) * (push1.x) + (push1.y) * (push1.y)));
        var pushSpeed2 = 6 * (Mathf.Sqrt((push2.x) * (push2.x) + (push2.y) * (push2.y)));

        var targetpos1 = Rigidplayer1.transform.position + push1;
        var targetpos2 = Rigidplayer2.transform.position + push2;


        if (Rigidplayer1.transform.position.z == 1f && Rigidplayer2.transform.position.z == 1f)
        {
            RaycastHit2D hit01 = (Physics2D.Raycast(Rigidplayer1.transform.position, push1, Mathf.Sqrt((push1.x) * (push1.x) + (push1.y) * (push1.y)), Hole, transform.position.z, transform.position.z));
            RaycastHit2D hit02 = (Physics2D.Raycast(Rigidplayer2.transform.position, push2, Mathf.Sqrt((push2.x) * (push2.x) + (push2.y) * (push2.y)), Hole, transform.position.z, transform.position.z));
            if (!hit01 || !hit02)
            {
                return false;
            }
        }

        RaycastHit2D hit1 = (Physics2D.Raycast(Rigidplayer1.transform.position, push1, Mathf.Sqrt((push1.x) * (push1.x) + (push1.y) * (push1.y)), blockingLayer, Rigidplayer1.transform.position.z, Rigidplayer1.transform.position.z));
        RaycastHit2D hit2 = (Physics2D.Raycast(Rigidplayer2.transform.position, push2, Mathf.Sqrt((push2.x) * (push2.x) + (push2.y) * (push2.y)), blockingLayer, Rigidplayer2.transform.position.z, Rigidplayer2.transform.position.z));


        if (!hit1 || hit1.collider.transform.root == transform)
        {
            move1 = true;
        }
        else if (Rigidplayer1.transform.position.y >= GridChange && Rigidplayer2.transform.position.y >= GridChange && Mathf.Abs(Mathf.Abs(Rigidplayer1.transform.position.x - hit1.collider.transform.position.x) * 2 - Mathf.Abs(Rigidplayer1.transform.position.y - hit1.collider.transform.position.y)) <= 1 && hit1.collider.CompareTag("Box"))
        {
            var box1 = hit1.collider.GetComponent<BoxMovement>();
            if (box1 != null && box1.TryToMultipushBox(push1, 0, push1, 0, key, check))
            {
                if (isPushed == true)
                {
                    box1.pushedByPushedPlayer = true;
                }
                move1 = true;
            }
        }
        else if (Rigidplayer1.transform.position.y >= GridChange && Rigidplayer2.transform.position.y >= GridChange && Mathf.Abs(Mathf.Abs(Rigidplayer1.transform.position.x - hit1.collider.transform.position.x) * 2 - Mathf.Abs(Rigidplayer1.transform.position.y - hit1.collider.transform.position.y)) <= 1 && hit1.collider.CompareTag("Rigidbox"))
        {
            var rigidbox1 = hit1.collider.transform.parent.GetComponent<RigidboxMovement>();
            if (rigidbox1 != null && rigidbox1.TryToMultipushRigidbox(push1, 0, push1, 0, key, check))
            {
                if (isPushed == true)
                {
                    rigidbox1.pushedByPushedPlayer = true;
                }
                move1 = true;
            }
        }
        else if (hit1.collider.CompareTag("Box"))
        {
            var box1 = hit1.collider.GetComponent<BoxMovement>();
            if (box1 != null && box1.transform.position.y == GridChange && Mathf.Abs(box1.transform.position.x) % 1 == ((box1.transform.position.y - GridChange) % 2) / 2 && box1.TryToMultipushBox(Vector3.down * 2, 0, Vector3.down * 2, 0, key, check))
            {
                if (isPushed == true)
                {
                    box1.pushedByPushedPlayer = true;
                }
                move1 = true;
            }
            else if (box1 != null && box1.TryToMultipushBox(push, 0, push, 0, key, check))
            {
                if (isPushed == true)
                {
                    box1.pushedByPushedPlayer = true;
                }
                move1 = true;
            }
        }
        else if (hit1.collider.CompareTag("Rigidbox"))
        {
            var rigidbox1 = hit1.collider.transform.parent.GetComponent<RigidboxMovement>();
            if (rigidbox1 != null && rigidbox1.TryToPushRigidbox(key, check))
            {
                move1 = true;
            }
        }
        else if (Rigidplayer1.transform.position.y >= GridChange && Rigidplayer2.transform.position.y >= GridChange && Mathf.Abs(Mathf.Abs(Rigidplayer1.transform.position.x - hit1.collider.transform.position.x) * 2 - Mathf.Abs(Rigidplayer1.transform.position.y - hit1.collider.transform.position.y)) <= 1 && hit1.collider.CompareTag("Player"))
        {
            var player1 = hit1.collider.transform.GetComponent<PlayerMovement>();
            if (player1 != null && player1.TryToPushPlayer(push1, 0, push1, 0, key, check, false))
            {
                player1.isPushed = true;
                move1 = true;
            }
        }
        else if (hit1.collider.CompareTag("Player"))
        {
            var player = hit1.collider.transform.GetComponent<PlayerMovement>();
            Debug.Log(player.gameObject.name);
            if (player.TryToMove(player.transform.position, key, true, false))
            {
                move1 = true;
                Debug.Log(move1);
            }

        }
        if (!hit2 || hit2.collider.transform.root == transform)
        {
            move2 = true;
        }
        else if (Rigidplayer1.transform.position.y >= GridChange && Rigidplayer2.transform.position.y >= GridChange && Mathf.Abs(Mathf.Abs(Rigidplayer2.transform.position.x - hit2.collider.transform.position.x) * 2 - Mathf.Abs(Rigidplayer2.transform.position.y - hit2.collider.transform.position.y)) <= 1 && hit2.collider.CompareTag("Box"))
        {
            var box2 = hit2.collider.GetComponent<BoxMovement>();
            if (box2 != null && box2.transform.position.y == GridChange && Mathf.Abs(box2.transform.position.x) % 1 == ((box2.transform.position.y - GridChange) % 2) / 2 && box2.TryToMultipushBox(Vector3.down * 2, 0, Vector3.down * 2, 0, key, check))
            {
                if (isPushed == true)
                {
                    box2.pushedByPushedPlayer = true;
                }
                move2 = true;
            }
            else if (box2 != null && box2.TryToMultipushBox(push, 0, push, 0, key, check))
            {
                if (isPushed == true)
                {
                    box2.pushedByPushedPlayer = true;
                }
                move2 = true;
            }
        }
        else if (Rigidplayer1.transform.position.y >= GridChange && Rigidplayer2.transform.position.y >= GridChange && Mathf.Abs(Mathf.Abs(Rigidplayer2.transform.position.x - hit2.collider.transform.position.x) * 2 - Mathf.Abs(Rigidplayer2.transform.position.y - hit2.collider.transform.position.y)) <= 1 && hit2.collider.CompareTag("Rigidbox"))
        {
            var rigidbox2 = hit2.collider.transform.parent.GetComponent<RigidboxMovement>();
            if (rigidbox2 != null && rigidbox2.TryToMultipushRigidbox(push2, 0, push2, 0, key, check))
            {
                if (isPushed == true)
                {
                    rigidbox2.pushedByPushedPlayer = true;
                }
                move2 = true;
            }
        }

        else if (hit2.collider.CompareTag("Box"))
        {
            var box2 = hit2.collider.GetComponent<BoxMovement>();
            if (box2 != null && box2.TryToPushBox(box2.transform.position, key, check, false))
            {
                move2 = true;
            }
        }
        else if (hit2.collider.CompareTag("Rigidbox"))
        {
            var rigidbox2 = hit2.collider.transform.parent.GetComponent<RigidboxMovement>();
            if (hit1 && hit1.collider.transform.root.GetComponent<RigidboxMovement>() == rigidbox2)
            {
                move2 = true;
            }
            else if (rigidbox2 != null && rigidbox2.TryToPushRigidbox(key, check))
            {
                move2 = true;
            }
        }
        else if (Rigidplayer1.transform.position.y >= GridChange && Rigidplayer2.transform.position.y >= GridChange && Mathf.Abs(Mathf.Abs(Rigidplayer2.transform.position.x - hit2.collider.transform.position.x) * 2 - Mathf.Abs(Rigidplayer2.transform.position.y - hit2.collider.transform.position.y)) <= 1 && hit2.collider.CompareTag("Player"))
        {
            var player2 = hit1.collider.transform.GetComponent<PlayerMovement>();
            if (player2 != null && player2.TryToPushPlayer(push2, 0, push2, 0, key, check, false))
            {
                player2.isPushed = true;
                move2 = true;
            }
        }
        else if (hit2.collider.CompareTag("Player"))
        {
            var player = hit2.collider.transform.GetComponent<PlayerMovement>();
            Debug.Log(player.gameObject.name);
            if (player.TryToMove(player.transform.position, key, true, false))
            {
                move2 = true;
                Debug.Log(move2);
            }

        }
        if (move1 && move2)
        {
            if (!check)
            {
                StartCoroutine(MoveToPosition(targetpos1, pushSpeed1, targetpos2, pushSpeed2, false, false, false));
            }
            return true;
        }
        return false;
    }
    public IEnumerator MoveToPosition(Vector3 targetpos1, float MoveSpeed1, Vector3 targetpos2, float MoveSpeed2, bool stopIfPushed, bool enteranimation, bool exitanimation)
    {
        yield return null;
        if (stopIfPushed)
        {
            if (isPushed)
            {
                isPushed = false;
                yield break;
            }
        }
        if (isMoving1 || isMoving2) yield break;
        isMoving1 = true;
        isMoving2 = true;
        if (enteranimation)
        {
            GameManager.dontSave = true;
            Rigidplayer1.GetComponent<SpriteRenderer>().sortingOrder = 1;
            Rigidplayer2.GetComponent<SpriteRenderer>().sortingOrder = 1;
            var scale1 = Rigidplayer1.transform.localScale;
            var scale2 = Rigidplayer2.transform.localScale;
            var startingdistance = Vector3.Distance(Rigidplayer1.transform.position, Rigidplayer2.transform.position);
            while (Vector3.Distance(Rigidplayer1.transform.position, targetpos1) > 0.01f && Vector3.Distance(Rigidplayer2.transform.position, targetpos2) > 0.01f)
            {
                Rigidplayer1.transform.position = Vector3.MoveTowards(Rigidplayer1.transform.position, targetpos1, MoveSpeed1 / 2 * Time.deltaTime);
                Rigidplayer2.transform.position = Vector3.MoveTowards(Rigidplayer2.transform.position, targetpos2, MoveSpeed2 / 2 * Time.deltaTime);
                Rigidplayer1.transform.localScale = scale1 / startingdistance * Vector3.Distance(Rigidplayer1.transform.position, Rigidplayer2.transform.position);
                Rigidplayer2.transform.localScale = scale2 / startingdistance * Vector3.Distance(Rigidplayer1.transform.position, Rigidplayer2.transform.position);
                yield return null;
            }
            Rigidplayer1.transform.position = targetpos1;
            Rigidplayer2.transform.position = targetpos2;
            Rigidplayer1.transform.localScale = Vector3.zero;
            Rigidplayer2.transform.localScale = Vector3.zero;
            Debug.Log("Animation ended");
            isMoving1 = false;
            isMoving2 = false;
            enabled = false;
            yield break;
        }
        else if (exitanimation)
        {
            GameManager.dontSave = true;
            Rigidplayer1.GetComponent<SpriteRenderer>().sortingOrder = 1;
            Rigidplayer2.GetComponent<SpriteRenderer>().sortingOrder = 1;
            var endingdistance = Vector3.Distance(targetpos1, targetpos2);
            var scale1 = Vector3.zero;
            var scale2 = Vector3.zero;
            if (targetpos1.y >= GridChange)
            {
                if (Mathf.Abs(targetpos1.x) % 1 == ((targetpos1.y - GridChange) % 2) / 2)
                {
                    scale1 = new Vector3(1, 1, 1);
                }
                else
                {
                    scale1 = new Vector3(1, -1, 1);
                }
            }
            else
            {
                scale1 = Vector3.one;
            }
            if (targetpos2.y >= GridChange)
            {
                if (Mathf.Abs(targetpos2.x) % 1 == ((targetpos2.y - GridChange) % 2) / 2)
                {
                    scale2 = new Vector3(1, 1, 1);
                }
                else
                {
                    scale2 = new Vector3(1, -1, 1);
                }
            }
            else
            {
                scale2 = Vector3.one;
            }
            Rigidplayer1.transform.localScale = Vector3.zero;
            Rigidplayer2.transform.localScale = Vector3.zero;
            while (Vector3.Distance(Rigidplayer1.transform.position, targetpos1) > 0.01f && Vector3.Distance(Rigidplayer2.transform.position, targetpos2) > 0.01f)
            {
                Rigidplayer1.transform.position = Vector3.MoveTowards(Rigidplayer1.transform.position, targetpos1, MoveSpeed1 / 2 * Time.deltaTime);
                Rigidplayer2.transform.position = Vector3.MoveTowards(Rigidplayer2.transform.position, targetpos2, MoveSpeed2 / 2 * Time.deltaTime);
                Rigidplayer1.transform.localScale = scale1 / endingdistance * Vector3.Distance(Rigidplayer1.transform.position, Rigidplayer2.transform.position);
                Rigidplayer2.transform.localScale = scale2 / endingdistance * Vector3.Distance(Rigidplayer1.transform.position, Rigidplayer2.transform.position);
                yield return null;
            }
            Rigidplayer1.transform.position = targetpos1;
            Rigidplayer2.transform.position = targetpos2;
            Rigidplayer1.transform.localScale = scale1;
            Rigidplayer2.transform.localScale = scale2;
            Debug.Log("Animation ended");
            isMoving1 = false;
            isMoving2 = false;
            GameManager.dontSave = false;
            yield break;
        }
        else
        {
            var avgmove = (targetpos1 - Rigidplayer1.transform.position + targetpos2 - Rigidplayer2.transform.position) / 2;
            var avgSpeed = (MoveSpeed1 + MoveSpeed2) / 2;
            var avgtarget1 = Rigidplayer1.transform.position + avgmove;
            var avgtarget2 = Rigidplayer2.transform.position + avgmove;
            var square = false;
            Vector3 cameratarget1;
            Vector3 cameratarget2;
            if (Rigidplayer1.transform.position.y < GridChange && Rigidplayer2.transform.position.y < GridChange)
            {
                square = true;
                while (Vector3.Distance(Rigidplayer1.transform.position, targetpos1) > 0.01f && Vector3.Distance(Rigidplayer2.transform.position, targetpos2) > 0.01f)
                {
                    Rigidplayer1.transform.position = Vector3.MoveTowards(Rigidplayer1.transform.position, targetpos1, MoveSpeed1 * Time.deltaTime);
                    Rigidplayer2.transform.position = Vector3.MoveTowards(Rigidplayer2.transform.position, targetpos2, MoveSpeed2 * Time.deltaTime);
                    camerapos1 = Vector3.MoveTowards(camerapos1, targetpos1, MoveSpeed1 * Time.deltaTime);
                    camerapos2 = Vector3.MoveTowards(camerapos2, targetpos2, MoveSpeed2 * Time.deltaTime);
                    yield return null;
                }
            }
            else
            {
                while (Vector3.Distance(Rigidplayer1.transform.position, avgtarget1) > 0.01f && Vector3.Distance(Rigidplayer2.transform.position, avgtarget2) > 0.01f)
                {
                    Rigidplayer1.transform.position = Vector3.MoveTowards(Rigidplayer1.transform.position, avgtarget1, avgSpeed * Time.deltaTime);
                    Rigidplayer2.transform.position = Vector3.MoveTowards(Rigidplayer2.transform.position, avgtarget2, avgSpeed * Time.deltaTime);
                    cameratarget1 = Vector3.Lerp(targetpos1, Rigidplayer1.transform.position, 0.5f);
                    cameratarget2 = Vector3.Lerp(targetpos2, Rigidplayer2.transform.position, 0.5f);
                    camerapos1 = Vector3.MoveTowards(camerapos1, cameratarget1, avgSpeed * Time.deltaTime);
                    camerapos2 = Vector3.MoveTowards(camerapos2, cameratarget2, avgSpeed * Time.deltaTime);
                    yield return null;
                }
            }

            if (!square)
            {
                camerapos1 = Vector3.Lerp(targetpos1, avgtarget1, 0.5f);
                camerapos2 = Vector3.Lerp(targetpos2, avgtarget2, 0.5f);
            }
        }
        Rigidplayer1.transform.position = targetpos1;
        Rigidplayer2.transform.position = targetpos2;
        if (destroy == true)
        {
            var player1 = Instantiate(Player, Rigidplayer1.transform.position, Rigidplayer1.transform.rotation);
            var player2 = Instantiate(Player, Rigidplayer2.transform.position, Rigidplayer2.transform.rotation);
            player1.name = Rigidplayer1.name;
            player2.name = Rigidplayer2.name;
            GameObject.Destroy(gameObject);
            destroy = false;
        }
        if ((Rigidplayer1.transform.position.z == 0f || Rigidplayer2.transform.position.z == 0f))
        {
            if (Physics2D.OverlapPoint(Rigidplayer1.transform.position, Hole, 1f, 1f) && !Physics2D.OverlapPoint(Rigidplayer1.transform.position, blockingLayer, 1f, 1f) && Physics2D.OverlapPoint(Rigidplayer2.transform.position, Hole, 1f, 1f) && !Physics2D.OverlapPoint(Rigidplayer2.transform.position, blockingLayer, 1f, 1f))
            {
                var pos1 = new Vector3(Rigidplayer1.transform.position.x, Rigidplayer1.transform.position.y, 1f);
                var pos2 = new Vector3(Rigidplayer2.transform.position.x, Rigidplayer2.transform.position.y, 1f);
                Rigidplayer1.transform.position = pos1;
                Rigidplayer2.transform.position = pos2;
            }
        }
        SpriteUpdate();
        yield return new WaitForSeconds(0.03f);
        isMoving1 = false;
        isMoving2 = false;
    }
}
    
