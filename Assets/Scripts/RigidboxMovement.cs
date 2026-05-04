using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

public class RigidboxMovement : MonoBehaviour
{
    public GameObject Rigidbox1;
    public GameObject Rigidbox2;
    public bool isMoving1 = false;
    public bool isMoving2 = false;
    public bool noInput = false;
    public bool won1 = true;
    public bool won2 = true;
    public bool pushedByPushedPlayer = false;
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
    public GameObject Box;
    public GameObject Levelbox;
    private bool destroy = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Rigidbox1.transform.rotation = Quaternion.identity;
        Rigidbox2.transform.rotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving1 || isMoving2) return;
        if (!won1 || !won2) return;
        
        if (GameManager.isMoving) return;
        if ((Rigidbox1.transform.position.z == 0f || Rigidbox2.transform.position.z == 0f))
        {
            if (Physics2D.OverlapPoint(Rigidbox1.transform.position, Hole, 1f, 1f) && !Physics2D.OverlapPoint(Rigidbox1.transform.position, blockingLayer, 1f, 1f) && Physics2D.OverlapPoint(Rigidbox2.transform.position, Hole, 1f, 1f) && !Physics2D.OverlapPoint(Rigidbox2.transform.position, blockingLayer, 1f, 1f))
            {
                var pos1 = new Vector3(Rigidbox1.transform.position.x, Rigidbox1.transform.position.y, 1f);
                var pos2 = new Vector3(Rigidbox2.transform.position.x, Rigidbox2.transform.position.y, 1f);
                Rigidbox1.transform.position = pos1;
                Rigidbox2.transform.position = pos2;
            }
        }
        if (Rigidbox1.transform.position.z != 1 && Rigidbox2.transform.position.z != 1)
        {
            if (Rigidbox1.transform.position.y < GridChange && Rigidbox2.transform.position.y < GridChange)
            {
                Rigidbox1.transform.localScale = Vector3.one;
                Rigidbox2.transform.localScale = Vector3.one;
                if (Rigidbox1.transform.position.x < Rigidbox2.transform.position.x)
                {
                    spriterenderer1.sprite = Square_right;
                    spriterenderer2.sprite = Square_left;
                }
                else if (Rigidbox1.transform.position.x > Rigidbox2.transform.position.x)
                {
                    spriterenderer1.sprite = Square_left;
                    spriterenderer2.sprite = Square_right;
                }
                else if (Rigidbox1.transform.position.y < Rigidbox2.transform.position.y)
                {
                    spriterenderer1.sprite = Square_up;
                    spriterenderer2.sprite = Square_down;
                }
                else if (Rigidbox1.transform.position.y > Rigidbox2.transform.position.y)
                {
                    spriterenderer1.sprite = Square_down;
                    spriterenderer2.sprite = Square_up;
                }
            }
            else if (Rigidbox1.transform.position.y >= GridChange && Rigidbox2.transform.position.y >= GridChange)
            {
                if (Rigidbox1.transform.position.y != Rigidbox2.transform.position.y)
                {
                    spriterenderer1.sprite = Triangle_down;
                    spriterenderer2.sprite = Triangle_down;
                }
                else if (Rigidbox1.transform.position.x < Rigidbox2.transform.position.x)
                {
                    spriterenderer1.sprite = Triangle_right;
                    spriterenderer2.sprite = Triangle_left;
                }
                else if (Rigidbox1.transform.position.x > Rigidbox2.transform.position.x)
                {
                    spriterenderer1.sprite = Triangle_left;
                    spriterenderer2.sprite = Triangle_right;
                }
                if (Mathf.Abs(Rigidbox1.transform.position.x) % 1 == ((Rigidbox1.transform.position.y - GridChange) % 2) / 2)
                {
                    Rigidbox1.transform.localScale = Vector3.one;
                }
                else
                {
                    Rigidbox1.transform.localScale = new Vector3(1, -1, 1);
                }
                if (Mathf.Abs(Rigidbox2.transform.position.x) % 1 == ((Rigidbox2.transform.position.y - GridChange) % 2) / 2)
                {
                    Rigidbox2.transform.localScale = Vector3.one;
                }
                else
                {
                    Rigidbox2.transform.localScale = new Vector3(1, -1, 1);
                }
            }
            else if (Rigidbox1.transform.position.y >= GridChange || Rigidbox2.transform.position.y < GridChange)
            {
                spriterenderer1.sprite = Triangle_down;
                spriterenderer2.sprite = Square_up;
                Rigidbox1.transform.localScale = Vector3.one;
                Rigidbox2.transform.localScale = Vector3.one;
            }
            else if (Rigidbox1.transform.position.y < GridChange || Rigidbox2.transform.position.y >= GridChange)
            {
                spriterenderer1.sprite = Square_up;
                spriterenderer2.sprite = Triangle_down;
                Rigidbox1.transform.localScale = Vector3.one;
                Rigidbox2.transform.localScale = Vector3.one;
            }
        }
        else
        {
            if (Rigidbox1.transform.position.y < GridChange && Rigidbox2.transform.position.y < GridChange)
            {
                Rigidbox1.transform.localScale = Vector3.one;
                Rigidbox2.transform.localScale = Vector3.one;
                if (Rigidbox1.transform.position.x < Rigidbox2.transform.position.x)
                {
                    spriterenderer1.sprite = Fallen_Square_right;
                    spriterenderer2.sprite = Fallen_Square_left;
                }
                else if (Rigidbox1.transform.position.x > Rigidbox2.transform.position.x)
                {
                    spriterenderer1.sprite = Fallen_Square_left;
                    spriterenderer2.sprite = Fallen_Square_right;
                }
                else if (Rigidbox1.transform.position.y < Rigidbox2.transform.position.y)
                {
                    spriterenderer1.sprite = Fallen_Square_up;
                    spriterenderer2.sprite = Fallen_Square_down;
                }
                else if (Rigidbox1.transform.position.y > Rigidbox2.transform.position.y)
                {
                    spriterenderer1.sprite = Fallen_Square_down;
                    spriterenderer2.sprite = Fallen_Square_up;
                }
            }
            else if (Rigidbox1.transform.position.y >= GridChange && Rigidbox2.transform.position.y >= GridChange)
            {
                if (Rigidbox1.transform.position.y != Rigidbox2.transform.position.y)
                {
                    spriterenderer1.sprite = Fallen_Triangle_down;
                    spriterenderer2.sprite = Fallen_Triangle_down;
                }
                else if (Rigidbox1.transform.position.x < Rigidbox2.transform.position.x)
                {
                    spriterenderer1.sprite = Fallen_Triangle_right;
                    spriterenderer2.sprite = Fallen_Triangle_left;
                }
                else if (Rigidbox1.transform.position.x > Rigidbox2.transform.position.x)
                {
                    spriterenderer1.sprite = Fallen_Triangle_left;
                    spriterenderer2.sprite = Fallen_Triangle_right;
                }
                if (Mathf.Abs(Rigidbox1.transform.position.x) % 1 == ((Rigidbox1.transform.position.y - GridChange) % 2) / 2)
                {
                    Rigidbox1.transform.localScale = Vector3.one;
                }
                else
                {
                    Rigidbox1.transform.localScale = new Vector3(1, -1, 1);
                }
                if (Mathf.Abs(Rigidbox2.transform.position.x) % 1 == ((Rigidbox2.transform.position.y - GridChange) % 2) / 2)
                {
                    Rigidbox2.transform.localScale = Vector3.one;
                }
                else
                {
                    Rigidbox2.transform.localScale = new Vector3(1, -1, 1);
                }
            }
            else if (Rigidbox1.transform.position.y >= GridChange && Rigidbox2.transform.position.y < GridChange)
            {
                spriterenderer1.sprite = Triangle_down;
                spriterenderer2.sprite = Square_up;
                Rigidbox1.transform.localScale = Vector3.one;
                Rigidbox2.transform.localScale = Vector3.one;
            }
            else if (Rigidbox1.transform.position.y < GridChange && Rigidbox2.transform.position.y >= GridChange)
            {
                spriterenderer1.sprite = Square_up;
                spriterenderer2.sprite = Triangle_down;
                Rigidbox1.transform.localScale = Vector3.one;
                Rigidbox2.transform.localScale = Vector3.one;
            }
        }

    }
    public bool TryToPushRigidbox(string key, bool check)
    {
        if (isMoving1 || isMoving2) return true;
        if (!won1 || !won2) return false;
        var push1 = Vector3.zero;
        var push2 = Vector3.zero;
        var move1 = false;
        var move2 = false;
        if (Rigidbox1.transform.position.y < GridChange && Rigidbox2.transform.position.y < GridChange)
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
        else if (Rigidbox1.transform.position.y > GridChange || Rigidbox2.transform.position.y > GridChange)
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
            if (Rigidbox1.transform.position.y != Rigidbox2.transform.position.y)
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
                if (Rigidbox1.transform.position.y > Rigidbox2.transform.position.y)
                {
                    push1 += push2;
                    push2 = push1 - push2;
                    push1 -= push2;
                }
            }
            else if ((Mathf.Abs(Rigidbox1.transform.position.x) % 1 == ((Rigidbox1.transform.position.y - GridChange) % 2) / 2 & Rigidbox2.transform.position.x > Rigidbox1.transform.position.x) || (Mathf.Abs(Rigidbox2.transform.position.x) % 1 == ((Rigidbox2.transform.position.y - GridChange) % 2) / 2 & Rigidbox1.transform.position.x > Rigidbox2.transform.position.x))
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
                if (Rigidbox1.transform.position.x < Rigidbox2.transform.position.x)
                {
                    push1 += push2;
                    push2 = push1 - push2;
                    push1 -= push2;
                }
            }
            else if ((Mathf.Abs(Rigidbox1.transform.position.x) % 1 == ((Rigidbox1.transform.position.y - GridChange) % 2) / 2 & Rigidbox2.transform.position.x < Rigidbox1.transform.position.x) || (Mathf.Abs(Rigidbox2.transform.position.x) % 1 == ((Rigidbox2.transform.position.y - GridChange) % 2) / 2 & Rigidbox1.transform.position.x < Rigidbox2.transform.position.x))
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
                if (Rigidbox1.transform.position.x > Rigidbox2.transform.position.x)
                {
                    push1 += push2;
                    push2 = push1 - push2;
                    push1 -= push2;
                }
            }
        }
        else if ((Rigidbox1.transform.position.y == GridChange && Rigidbox2.transform.position.y < GridChange) || (Rigidbox1.transform.position.y < GridChange && Rigidbox2.transform.position.y == GridChange))
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
            if (Rigidbox1.transform.position.y < GridChange && Rigidbox2.transform.position.y >= GridChange)
            {
                push1 += push2;
                push2 = push1 - push2;
                push1 -= push2;

            }
        }
        else if (Rigidbox1.transform.position.y == GridChange && Rigidbox2.transform.position.y == GridChange)
        {
            if ((Mathf.Abs(Rigidbox1.transform.position.x) % 1 == ((Rigidbox1.transform.position.y - GridChange) % 2) / 2 & Rigidbox2.transform.position.x > Rigidbox1.transform.position.x) || (Mathf.Abs(Rigidbox2.transform.position.x) % 1 == ((Rigidbox2.transform.position.y - GridChange) % 2) / 2 & Rigidbox1.transform.position.x > Rigidbox2.transform.position.x))
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
                if (Rigidbox1.transform.position.x < Rigidbox2.transform.position.x)
                {
                    push1 += push2;
                    push2 = push1 - push2;
                    push1 -= push2;
                }
            }
            else if ((Mathf.Abs(Rigidbox1.transform.position.x) % 1 == ((Rigidbox1.transform.position.y - GridChange) % 2) / 2 && Rigidbox2.transform.position.x < Rigidbox1.transform.position.x) || (Mathf.Abs(Rigidbox2.transform.position.x) % 1 == ((Rigidbox2.transform.position.y - GridChange) % 2) / 2 & Rigidbox1.transform.position.x < Rigidbox2.transform.position.x))
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
                if (Rigidbox1.transform.position.x > Rigidbox2.transform.position.x)
                {
                    push1 += push2;
                    push2 = push1 - push2;
                    push1 -= push2;
                }
            }
        }

        var pushSpeed1 = 6 * (Mathf.Sqrt((push1.x) * (push1.x) + (push1.y) * (push1.y)));
        var pushSpeed2 = 6 * (Mathf.Sqrt((push2.x) * (push2.x) + (push2.y) * (push2.y)));

        var targetpos1 = Rigidbox1.transform.position + push1;
        var targetpos2 = Rigidbox2.transform.position + push2;

        if (Rigidbox1.transform.position.z == 1f && Rigidbox2.transform.position.z == 1f)
        {
            RaycastHit2D[] hit1_ = (Physics2D.RaycastAll(Rigidbox1.transform.position, push1, Mathf.Sqrt((push1.x) * (push1.x) + (push1.y) * (push1.y)), Hole, Rigidbox1.transform.position.z, Rigidbox1.transform.position.z));
            if (((((push1 == Vector3.right || push1 == Vector3.left) && Rigidbox1.transform.position.y >= GridChange && push1.magnitude == 1) || push1.magnitude > 1) && hit1_.Length < 2) || hit1_.Length < 1)
            {
                return false;
            }
            RaycastHit2D[] hit2_ = (Physics2D.RaycastAll(Rigidbox2.transform.position, push2, Mathf.Sqrt((push2.x) * (push2.x) + (push2.y) * (push2.y)), Hole, Rigidbox2.transform.position.z, Rigidbox2.transform.position.z));
            if (((((push2 == Vector3.right || push2 == Vector3.left) && Rigidbox2.transform.position.y >= GridChange && push2.magnitude == 1) || push2.magnitude > 1) && hit2_.Length < 2) || hit2_.Length < 1)
            {
                return false;
            }
        }

        RaycastHit2D hit1 = (Physics2D.Raycast(Rigidbox1.transform.position, push1, Mathf.Sqrt((push1.x) * (push1.x) + (push1.y) * (push1.y)), blockingLayer, Rigidbox1.transform.position.z, Rigidbox1.transform.position.z));
        RaycastHit2D hit2 = (Physics2D.Raycast(Rigidbox2.transform.position, push2, Mathf.Sqrt((push2.x) * (push2.x) + (push2.y) * (push2.y)), blockingLayer, Rigidbox2.transform.position.z, Rigidbox2.transform.position.z));


        if (!hit1 || hit1.collider.transform.root == transform)
        {
            move1 = true;
        }
        else if (Rigidbox1.transform.position.y >= GridChange && Rigidbox2.transform.position.y >= GridChange && push1 == push2 && push2 != Vector3.left / 2 && push2 != Vector3.right / 2 && Mathf.Abs(Mathf.Abs(Rigidbox1.transform.position.x - hit1.collider.transform.position.x) * 2 + Mathf.Abs(Rigidbox1.transform.position.y - hit1.collider.transform.position.y)) <= 1 && hit1.collider.transform.position.y >= GridChange && hit1.collider.CompareTag("Box"))
        {
            var box1 = hit1.collider.GetComponent<BoxMovement>();
            if (box1 != null && box1.TryToMultipushBox(push1, 0, push1, 0, key, check))
            {
                move1 = true;
            }
        }
        else if (Rigidbox1.transform.position.y >= GridChange && Rigidbox2.transform.position.y >= GridChange && push1 == push2 && push2 != Vector3.left / 2 && push2 != Vector3.right / 2 && Mathf.Abs(Mathf.Abs(Rigidbox1.transform.position.x - hit1.collider.transform.position.x) * 2 + Mathf.Abs(Rigidbox1.transform.position.y - hit1.collider.transform.position.y)) <= 1 && hit1.collider.transform.position.y >= GridChange && hit1.collider.CompareTag("Rigidbox"))
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
            if ((Rigidbox1.transform.position.y == GridChange && Rigidbox2.transform.position.y == GridChange) && ((Mathf.Abs(Rigidbox1.transform.position.x) % 1 == ((Rigidbox1.transform.position.y - GridChange) % 2) / 2 & Rigidbox2.transform.position.x > Rigidbox1.transform.position.x) || (Mathf.Abs(Rigidbox2.transform.position.x) % 1 == ((Rigidbox2.transform.position.y - GridChange) % 2) / 2 & Rigidbox1.transform.position.x > Rigidbox2.transform.position.x)) && hit1.collider.transform.position.y == GridChange && hit1.collider.transform.position.y >= GridChange && key == "X1")
            {
                if (box1 != null && box1.TryToPushBox(box1.transform.position, "D1", check, false))
                {
                    move1 = true;
                }
            }
            else if ((Rigidbox1.transform.position.y == GridChange && Rigidbox2.transform.position.y == GridChange) && ((Mathf.Abs(Rigidbox1.transform.position.x) % 1 == ((Rigidbox1.transform.position.y - GridChange) % 2) / 2 & Rigidbox2.transform.position.x < Rigidbox1.transform.position.x) || (Mathf.Abs(Rigidbox2.transform.position.x) % 1 == ((Rigidbox2.transform.position.y - GridChange) % 2) / 2 & Rigidbox1.transform.position.x < Rigidbox2.transform.position.x)) && hit1.collider.transform.position.y == GridChange && hit1.collider.transform.position.y >= GridChange && key == "Z1")
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
            if ((Rigidbox1.transform.position.y == GridChange && Rigidbox2.transform.position.y == GridChange) && ((Mathf.Abs(Rigidbox1.transform.position.x) % 1 == ((Rigidbox1.transform.position.y - GridChange) % 2) / 2 & Rigidbox2.transform.position.x > Rigidbox1.transform.position.x) || (Mathf.Abs(Rigidbox2.transform.position.x) % 1 == ((Rigidbox2.transform.position.y - GridChange) % 2) / 2 & Rigidbox1.transform.position.x > Rigidbox2.transform.position.x)) && hit1.collider.transform.position.y == GridChange && hit1.collider.transform.position.y >= GridChange && key == "X1")
            {
                if (rigidbox1 != null && rigidbox1.TryToPushRigidbox("D1", check))
                {
                    move1 = true;
                }
            }
            else if ((Rigidbox1.transform.position.y == GridChange && Rigidbox2.transform.position.y == GridChange) && ((Mathf.Abs(Rigidbox1.transform.position.x) % 1 == ((Rigidbox1.transform.position.y - GridChange) % 2) / 2 & Rigidbox2.transform.position.x < Rigidbox1.transform.position.x) || (Mathf.Abs(Rigidbox2.transform.position.x) % 1 == ((Rigidbox2.transform.position.y - GridChange) % 2) / 2 & Rigidbox1.transform.position.x < Rigidbox2.transform.position.x)) && hit1.collider.transform.position.y == GridChange && hit1.collider.transform.position.y >= GridChange && key == "Z1")
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
        else if (Rigidbox1.transform.position.y >= GridChange && Rigidbox2.transform.position.y >= GridChange && push1 == push2 && push2 != Vector3.left / 2 && push2 != Vector3.right / 2 && Mathf.Abs(Mathf.Abs(Rigidbox1.transform.position.x - hit1.collider.transform.position.x) * 2 + Mathf.Abs(Rigidbox1.transform.position.y - hit1.collider.transform.position.y)) <= 1 && hit1.collider.transform.position.y >= GridChange && hit1.collider.CompareTag("Player"))
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
            if (Rigidbox1.transform.position.y == GridChange && player1.transform.position.y == GridChange - 1 && player1.TryToPushPlayer(Vector3.down, 0, Vector3.down, 0, key, check, false))
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
        else if (Rigidbox1.transform.position.y >= GridChange && Rigidbox2.transform.position.y >= GridChange && push1 == push2 && push2 != Vector3.left / 2 && push2 != Vector3.right / 2 && Mathf.Abs(Mathf.Abs(Rigidbox2.transform.position.x - hit2.collider.transform.position.x) * 2 + Mathf.Abs(Rigidbox2.transform.position.y - hit2.collider.transform.position.y)) <= 1 && hit2.collider.transform.position.y >= GridChange && hit2.collider.CompareTag("Box"))
        {
            var box2 = hit2.collider.GetComponent<BoxMovement>();
            if (box2 != null && box2.TryToMultipushBox(push1, 0, push1, 0, key, check))
            {
                move2 = true;
            }
        }
        else if (Rigidbox1.transform.position.y >= GridChange && Rigidbox2.transform.position.y >= GridChange && push1 == push2 && push2 != Vector3.left / 2 && push2 != Vector3.right / 2 && Mathf.Abs(Mathf.Abs(Rigidbox2.transform.position.x - hit2.collider.transform.position.x) * 2 + Mathf.Abs(Rigidbox2.transform.position.y - hit2.collider.transform.position.y)) <= 1 && hit2.collider.transform.position.y >= GridChange && hit2.collider.CompareTag("Rigidbox"))
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
            if ((Rigidbox1.transform.position.y == GridChange && Rigidbox2.transform.position.y == GridChange) && ((Mathf.Abs(Rigidbox1.transform.position.x) % 1 == ((Rigidbox1.transform.position.y - GridChange) % 2) / 2 & Rigidbox2.transform.position.x > Rigidbox1.transform.position.x) || (Mathf.Abs(Rigidbox2.transform.position.x) % 1 == ((Rigidbox2.transform.position.y - GridChange) % 2) / 2 & Rigidbox1.transform.position.x > Rigidbox2.transform.position.x)) && hit2.collider.transform.position.y == GridChange && key == "X1")
            {
                if (box2 != null && box2.TryToPushBox(box2.transform.position, "D1", check, false))
                {
                    move2 = true;
                }
            }
            else if ((Rigidbox1.transform.position.y == GridChange && Rigidbox2.transform.position.y == GridChange) && ((Mathf.Abs(Rigidbox1.transform.position.x) % 1 == ((Rigidbox1.transform.position.y - GridChange) % 2) / 2 & Rigidbox2.transform.position.x < Rigidbox1.transform.position.x) || (Mathf.Abs(Rigidbox2.transform.position.x) % 1 == ((Rigidbox2.transform.position.y - GridChange) % 2) / 2 & Rigidbox1.transform.position.x < Rigidbox2.transform.position.x)) && hit2.collider.transform.position.y == GridChange && key == "Z1")
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
            if ((Rigidbox1.transform.position.y == GridChange && Rigidbox2.transform.position.y == GridChange) && ((Mathf.Abs(Rigidbox1.transform.position.x) % 1 == ((Rigidbox1.transform.position.y - GridChange) % 2) / 2 && Rigidbox2.transform.position.x > Rigidbox1.transform.position.x) || (Mathf.Abs(Rigidbox2.transform.position.x) % 1 == ((Rigidbox2.transform.position.y - GridChange) % 2) / 2 & Rigidbox1.transform.position.x > Rigidbox2.transform.position.x)) && hit2.collider.transform.position.y >= GridChange && key == "X1")
            {
                if (rigidbox2 != null && rigidbox2.TryToPushRigidbox("D1", check))
                {
                    move2 = true;
                }
            }
            else if ((Rigidbox1.transform.position.y == GridChange && Rigidbox2.transform.position.y == GridChange) && ((Mathf.Abs(Rigidbox1.transform.position.x) % 1 == ((Rigidbox1.transform.position.y - GridChange) % 2) / 2 & Rigidbox2.transform.position.x < Rigidbox1.transform.position.x) || (Mathf.Abs(Rigidbox2.transform.position.x) % 1 == ((Rigidbox2.transform.position.y - GridChange) % 2) / 2 & Rigidbox1.transform.position.x < Rigidbox2.transform.position.x)) && hit2.collider.transform.position.y >= GridChange && key == "Z1")
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
        else if (Rigidbox1.transform.position.y >= GridChange && Rigidbox2.transform.position.y >= GridChange && push1 == push2 && push2 != Vector3.left / 2 && push2 != Vector3.right / 2 && Mathf.Abs(Mathf.Abs(Rigidbox2.transform.position.x - hit2.collider.transform.position.x) * 2 + Mathf.Abs(Rigidbox2.transform.position.y - hit2.collider.transform.position.y)) <= 1 && hit2.collider.transform.position.y >= GridChange && hit2.collider.CompareTag("Player"))
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
            
            if (Rigidbox1.transform.position.y == GridChange && player2.transform.position.y == GridChange - 1 && player2.TryToPushPlayer(Vector3.down, 0, Vector3.down, 0, key, check, false))
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
                if (Rigidbox1.transform.position.y == GridChange - 1 && Rigidbox2.transform.position.y == GridChange - 1 && key == "W")
                {
                    destroy = true;
                }
                StartCoroutine(MoveToPosition(targetpos1, pushSpeed1, targetpos2, pushSpeed2, true));
            }
            return true;
        }

        return false;
    }
    public bool TryToMultipushRigidbox(Vector3 movement, float MoveSpeed, Vector3 push, float PushSpeed, string key, bool check)
    {
        if (isMoving1 || isMoving2) return true;
        if (!won1 || !won2) return false;
        var move1 = false;
        var move2 = false;
        var push1 = push;
        var push2 = push;
        if (Rigidbox1.transform.position.y == GridChange && Rigidbox2.transform.position.y == GridChange && ((Mathf.Abs(Rigidbox1.transform.position.x) % 1 == ((Rigidbox1.transform.position.y - GridChange) % 2) / 2 && Rigidbox1.transform.position.x > Rigidbox2.transform.position.x) || (Mathf.Abs(Rigidbox2.transform.position.x) % 1 == ((Rigidbox2.transform.position.y - GridChange) % 2) / 2 && Rigidbox2.transform.position.x > Rigidbox1.transform.position.x)) && push == new Vector3(-0.5f, -1, 0))
        {
            push1 = Vector3.down;
            if (Rigidbox1.transform.position.x < Rigidbox2.transform.position.x)
            {
                push1 += push2;
                push2 = push1 - push2;
                push1 -= push2;
            }
        }
        else if (Rigidbox1.transform.position.y == GridChange && Rigidbox2.transform.position.y == GridChange && ((Mathf.Abs(Rigidbox1.transform.position.x) % 1 == ((Rigidbox1.transform.position.y - GridChange) % 2) / 2 && Rigidbox1.transform.position.x > Rigidbox2.transform.position.x) || (Mathf.Abs(Rigidbox2.transform.position.x) % 1 == ((Rigidbox2.transform.position.y - GridChange) % 2) / 2 && Rigidbox2.transform.position.x > Rigidbox1.transform.position.x)) && push == new Vector3(0.5f, -1, 0))
        {
            push1 = Vector3.down * 2;
            if (Rigidbox1.transform.position.x < Rigidbox2.transform.position.x)
            {
                push1 += push2;
                push2 = push1 - push2;
                push1 -= push2;
            }
        }
        else if (Rigidbox1.transform.position.y == GridChange && Rigidbox2.transform.position.y == GridChange && ((Mathf.Abs(Rigidbox1.transform.position.x) % 1 == ((Rigidbox1.transform.position.y - GridChange) % 2) / 2 && Rigidbox1.transform.position.x < Rigidbox2.transform.position.x) || (Mathf.Abs(Rigidbox2.transform.position.x) % 1 == ((Rigidbox2.transform.position.y - GridChange) % 2) / 2 && Rigidbox2.transform.position.x < Rigidbox1.transform.position.x)) && push == new Vector3(0.5f, -1, 0))
        {
            push1 = Vector3.down;
            if (Rigidbox1.transform.position.x > Rigidbox2.transform.position.x)
            {
                push1 += push2;
                push2 = push1 - push2;
                push1 -= push2;
            }
        }
        else if (Rigidbox1.transform.position.y == GridChange && Rigidbox2.transform.position.y == GridChange && ((Mathf.Abs(Rigidbox1.transform.position.x) % 1 == ((Rigidbox1.transform.position.y - GridChange) % 2) / 2 && Rigidbox1.transform.position.x < Rigidbox2.transform.position.x) || (Mathf.Abs(Rigidbox2.transform.position.x) % 1 == ((Rigidbox2.transform.position.y - GridChange) % 2) / 2 && Rigidbox2.transform.position.x < Rigidbox1.transform.position.x)) && push == new Vector3(-0.5f, -1, 0))
        {
            push1 = Vector3.down * 2;
            if (Rigidbox1.transform.position.x > Rigidbox2.transform.position.x)
            {
                push1 += push2;
                push2 = push1 - push2;
                push1 -= push2;
            }
        }
        var pushSpeed1 = 6 * (Mathf.Sqrt((push1.x) * (push1.x) + (push1.y) * (push1.y)));
        var pushSpeed2 = 6 * (Mathf.Sqrt((push2.x) * (push2.x) + (push2.y) * (push2.y)));

        var targetpos1 = Rigidbox1.transform.position + push1;
        var targetpos2 = Rigidbox2.transform.position + push2;


        if (Rigidbox1.transform.position.z == 1f && Rigidbox2.transform.position.z == 1f)
        {
            RaycastHit2D[] hit1_ = (Physics2D.RaycastAll(Rigidbox1.transform.position, push1, Mathf.Sqrt((push1.x) * (push1.x) + (push1.y) * (push1.y)), Hole, Rigidbox1.transform.position.z, Rigidbox1.transform.position.z));
            if (((((push1 == Vector3.right || push1 == Vector3.left) && Rigidbox1.transform.position.y >= GridChange && push1.magnitude == 1) || push1.magnitude > 1) && hit1_.Length < 2) || hit1_.Length < 1)
            {
                return false;
            }
            RaycastHit2D[] hit2_ = (Physics2D.RaycastAll(Rigidbox2.transform.position, push2, Mathf.Sqrt((push2.x) * (push2.x) + (push2.y) * (push2.y)), Hole, Rigidbox2.transform.position.z, Rigidbox2.transform.position.z));
            if (((((push2 == Vector3.right || push2 == Vector3.left) && Rigidbox2.transform.position.y >= GridChange && push2.magnitude == 1) || push2.magnitude > 1) && hit2_.Length < 2) || hit2_.Length < 1)
            {
                return false;
            }
        }

        RaycastHit2D hit1 = (Physics2D.Raycast(Rigidbox1.transform.position, push1, Mathf.Sqrt((push1.x) * (push1.x) + (push1.y) * (push1.y)), blockingLayer, Rigidbox1.transform.position.z, Rigidbox1.transform.position.z));
        RaycastHit2D hit2 = (Physics2D.Raycast(Rigidbox2.transform.position, push2, Mathf.Sqrt((push2.x) * (push2.x) + (push2.y) * (push2.y)), blockingLayer, Rigidbox2.transform.position.z, Rigidbox2.transform.position.z));


        if (!hit1 || hit1.collider.transform.root == transform)
        {
            move1 = true;
        }
        else if (Rigidbox1.transform.position.y >= GridChange && Rigidbox2.transform.position.y >= GridChange && Mathf.Abs(Mathf.Abs(Rigidbox1.transform.position.x - hit1.collider.transform.position.x) * 2 - Mathf.Abs(Rigidbox1.transform.position.y - hit1.collider.transform.position.y)) <= 1 && hit1.collider.CompareTag("Box"))
        {
            var box1 = hit1.collider.GetComponent<BoxMovement>();
            if (box1 != null && box1.TryToMultipushBox(push1, 0, push1, 0, key, check))
            {
                if (pushedByPushedPlayer == true)
                {
                    box1.pushedByPushedPlayer = true;
                }
                move1 = true;
            }
        }
        else if (Rigidbox1.transform.position.y >= GridChange && Rigidbox2.transform.position.y >= GridChange && Mathf.Abs(Mathf.Abs(Rigidbox1.transform.position.x - hit1.collider.transform.position.x) * 2 - Mathf.Abs(Rigidbox1.transform.position.y - hit1.collider.transform.position.y)) <= 1 && hit1.collider.CompareTag("Rigidbox"))
        {
            var rigidbox1 = hit1.collider.transform.parent.GetComponent<RigidboxMovement>();
            if (rigidbox1 != null && rigidbox1.TryToMultipushRigidbox(push1, 0, push1, 0, key, check))
            {
                if (pushedByPushedPlayer == true)
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
                if (pushedByPushedPlayer == true)
                {
                    box1.pushedByPushedPlayer = true;
                }
                move1 = true;
            }
            else if (box1 != null && box1.TryToMultipushBox(push, 0, push, 0, key, check))
            {
                if (pushedByPushedPlayer == true)
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
        else if (Rigidbox1.transform.position.y >= GridChange && Rigidbox2.transform.position.y >= GridChange && Mathf.Abs(Mathf.Abs(Rigidbox1.transform.position.x - hit1.collider.transform.position.x) * 2 - Mathf.Abs(Rigidbox1.transform.position.y - hit1.collider.transform.position.y)) <= 1 && hit1.collider.CompareTag("Player"))
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
        else if (Rigidbox1.transform.position.y >= GridChange && Rigidbox2.transform.position.y >= GridChange && Mathf.Abs(Mathf.Abs(Rigidbox2.transform.position.x - hit2.collider.transform.position.x) * 2 - Mathf.Abs(Rigidbox2.transform.position.y - hit2.collider.transform.position.y)) <= 1 && hit2.collider.CompareTag("Box"))
        {
            var box2 = hit2.collider.GetComponent<BoxMovement>();
            if (box2 != null && box2.transform.position.y == GridChange && Mathf.Abs(box2.transform.position.x) % 1 == ((box2.transform.position.y - GridChange) % 2) / 2 && box2.TryToMultipushBox(Vector3.down * 2, 0, Vector3.down * 2, 0, key, check))
            {
                if (pushedByPushedPlayer == true)
                {
                    box2.pushedByPushedPlayer = true;
                }
                move2 = true;
            }
            else if (box2 != null && box2.TryToMultipushBox(push, 0, push, 0, key, check))
            {
                if (pushedByPushedPlayer == true)
                {
                    box2.pushedByPushedPlayer = true;
                }
                move2 = true;
            }
        }
        else if (Rigidbox1.transform.position.y >= GridChange && Rigidbox2.transform.position.y >= GridChange && Mathf.Abs(Mathf.Abs(Rigidbox2.transform.position.x - hit2.collider.transform.position.x) * 2 - Mathf.Abs(Rigidbox2.transform.position.y - hit2.collider.transform.position.y)) <= 1 && hit2.collider.CompareTag("Rigidbox"))
        {
            var rigidbox2 = hit2.collider.transform.parent.GetComponent<RigidboxMovement>();
            if (rigidbox2 != null && rigidbox2.TryToMultipushRigidbox(push2, 0, push2, 0, key, check))
            {
                if (pushedByPushedPlayer == true)
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
        else if (Rigidbox1.transform.position.y >= GridChange && Rigidbox2.transform.position.y >= GridChange && Mathf.Abs(Mathf.Abs(Rigidbox2.transform.position.x - hit2.collider.transform.position.x) * 2 - Mathf.Abs(Rigidbox2.transform.position.y - hit2.collider.transform.position.y)) <= 1 && hit2.collider.CompareTag("Player"))
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
                if (Rigidbox1.transform.position.y == GridChange - 1 && Rigidbox2.transform.position.y == GridChange - 1 && key == "W")
                {
                    destroy = true;
                }
                StartCoroutine(MoveToPosition(targetpos1, pushSpeed1, targetpos2, pushSpeed2, false));
            }
            return true;
        }
        return false;
    }
    private IEnumerator MoveToPosition(Vector3 targetpos1, float MoveSpeed1, Vector3 targetpos2, float MoveSpeed2, bool stopIfPushedByPushedPlayer)
    {
        yield return null;
        if (stopIfPushedByPushedPlayer)
        {
            if (pushedByPushedPlayer)
            {
                pushedByPushedPlayer = false;
                yield break;
            }
        }
        if (isMoving1 || isMoving2) yield break;
        isMoving1 = true;
        isMoving2 = true;
        var avg = (targetpos1 - Rigidbox1.transform.position + targetpos2 - Rigidbox2.transform.position) / 2;
        var avgSpeed = (MoveSpeed1 + MoveSpeed2) / 2;
        var avgtarget1 = Rigidbox1.transform.position + avg;
        var avgtarget2 = Rigidbox2.transform.position + avg;
        while (Vector3.Distance(Rigidbox1.transform.position, avgtarget1) > 0.01f && Vector3.Distance(Rigidbox2.transform.position, avgtarget2) > 0.01f)
        {
            Rigidbox1.transform.position = Vector3.MoveTowards(Rigidbox1.transform.position, avgtarget1, avgSpeed * Time.deltaTime);
            Rigidbox2.transform.position = Vector3.MoveTowards(Rigidbox2.transform.position, avgtarget2, avgSpeed * Time.deltaTime);
            yield return null;
        }
        Rigidbox1.transform.position = targetpos1;
        Rigidbox2.transform.position = targetpos2;
        if (destroy == true)
        {
            if (Rigidbox1.GetComponent<LevelScript>() != null)
            {
                var levelbox = Instantiate(Levelbox, Rigidbox1.transform.position, Rigidbox1.transform.rotation);
                levelbox.GetComponent<LevelScript>().SceneName = Rigidbox1.GetComponent<LevelScript>().SceneName;
                levelbox.GetComponentInChildren<TMP_Text>().SetText(Rigidbox1.GetComponentInChildren<TMP_Text>().text);
                levelbox.name = Rigidbox1.name.Replace("Rigidbox", "Box");
            }
            else
            {
                var box = Instantiate(Box, Rigidbox1.transform.position, Rigidbox1.transform.rotation);
                box.name = Rigidbox1.name.Replace("Rigidbox", "Box");
            }
            if (Rigidbox2.GetComponent<LevelScript>() != null)
            {
                var levelbox = Instantiate(Levelbox, Rigidbox2.transform.position, Rigidbox2.transform.rotation);
                levelbox.GetComponent<LevelScript>().SceneName = Rigidbox2.GetComponent<LevelScript>().SceneName;
                levelbox.GetComponentInChildren<TMP_Text>().SetText(Rigidbox2.GetComponentInChildren<TMP_Text>().text);
                levelbox.name = Rigidbox2.name.Replace("Rigidbox", "Box");
            }
            else
            {
                var box = Instantiate(Box, Rigidbox2.transform.position, Rigidbox2.transform.rotation);
                box.name = Rigidbox1.name.Replace("Rigidbox", "Box");
            }
            GameObject.Destroy(gameObject);
            destroy = false;
        }
        if ((Rigidbox1.transform.position.z == 0f || Rigidbox2.transform.position.z == 0f))
        {
            if (Physics2D.OverlapPoint(Rigidbox1.transform.position, Hole, 1f, 1f) && !Physics2D.OverlapPoint(Rigidbox1.transform.position, blockingLayer, 1f, 1f) && Physics2D.OverlapPoint(Rigidbox2.transform.position, Hole, 1f, 1f) && !Physics2D.OverlapPoint(Rigidbox2.transform.position, blockingLayer, 1f, 1f))
            {
                var pos1 = new Vector3(Rigidbox1.transform.position.x, Rigidbox1.transform.position.y, 1f);
                var pos2 = new Vector3(Rigidbox2.transform.position.x, Rigidbox2.transform.position.y, 1f);
                Rigidbox1.transform.position = pos1;
                Rigidbox2.transform.position = pos2;
            }
        }
        if (Rigidbox1.transform.position.z != 1 && Rigidbox2.transform.position.z != 1)
        {
            if (Rigidbox1.transform.position.y < GridChange && Rigidbox2.transform.position.y < GridChange)
            {
                Rigidbox1.transform.localScale = Vector3.one;
                Rigidbox2.transform.localScale = Vector3.one;
                if (Rigidbox1.transform.position.x < Rigidbox2.transform.position.x)
                {
                    spriterenderer1.sprite = Square_right;
                    spriterenderer2.sprite = Square_left;
                }
                else if (Rigidbox1.transform.position.x > Rigidbox2.transform.position.x)
                {
                    spriterenderer1.sprite = Square_left;
                    spriterenderer2.sprite = Square_right;
                }
                else if (Rigidbox1.transform.position.y < Rigidbox2.transform.position.y)
                {
                    spriterenderer1.sprite = Square_up;
                    spriterenderer2.sprite = Square_down;
                }
                else if (Rigidbox1.transform.position.y > Rigidbox2.transform.position.y)
                {
                    spriterenderer1.sprite = Square_down;
                    spriterenderer2.sprite = Square_up;
                }
            }
            else if (Rigidbox1.transform.position.y >= GridChange && Rigidbox2.transform.position.y >= GridChange)
            {
                if (Rigidbox1.transform.position.y != Rigidbox2.transform.position.y)
                {
                    spriterenderer1.sprite = Triangle_down;
                    spriterenderer2.sprite = Triangle_down;
                }
                else if (Rigidbox1.transform.position.x < Rigidbox2.transform.position.x)
                {
                    spriterenderer1.sprite = Triangle_right;
                    spriterenderer2.sprite = Triangle_left;
                }
                else if (Rigidbox1.transform.position.x > Rigidbox2.transform.position.x)
                {
                    spriterenderer1.sprite = Triangle_left;
                    spriterenderer2.sprite = Triangle_right;
                }
                if (Mathf.Abs(Rigidbox1.transform.position.x) % 1 == ((Rigidbox1.transform.position.y - GridChange) % 2) / 2)
                {
                    Rigidbox1.transform.localScale = Vector3.one;
                }
                else
                {
                    Rigidbox1.transform.localScale = new Vector3(1, -1, 1);
                }
                if (Mathf.Abs(Rigidbox2.transform.position.x) % 1 == ((Rigidbox2.transform.position.y - GridChange) % 2) / 2)
                {
                    Rigidbox2.transform.localScale = Vector3.one;
                }
                else
                {
                    Rigidbox2.transform.localScale = new Vector3(1, -1, 1);
                }
            }
            else if (Rigidbox1.transform.position.y >= GridChange || Rigidbox2.transform.position.y < GridChange)
            {
                spriterenderer1.sprite = Triangle_down;
                spriterenderer2.sprite = Square_up;
                Rigidbox1.transform.localScale = Vector3.one;
                Rigidbox2.transform.localScale = Vector3.one;
            }
            else if (Rigidbox1.transform.position.y < GridChange || Rigidbox2.transform.position.y >= GridChange)
            {
                spriterenderer1.sprite = Square_up;
                spriterenderer2.sprite = Triangle_down;
                Rigidbox1.transform.localScale = Vector3.one;
                Rigidbox2.transform.localScale = Vector3.one;
            }
        }
        else
        {
            if (Rigidbox1.transform.position.y < GridChange && Rigidbox2.transform.position.y < GridChange)
            {
                Rigidbox1.transform.localScale = Vector3.one;
                Rigidbox2.transform.localScale = Vector3.one;
                if (Rigidbox1.transform.position.x < Rigidbox2.transform.position.x)
                {
                    spriterenderer1.sprite = Fallen_Square_right;
                    spriterenderer2.sprite = Fallen_Square_left;
                }
                else if (Rigidbox1.transform.position.x > Rigidbox2.transform.position.x)
                {
                    spriterenderer1.sprite = Fallen_Square_left;
                    spriterenderer2.sprite = Fallen_Square_right;
                }
                else if (Rigidbox1.transform.position.y < Rigidbox2.transform.position.y)
                {
                    spriterenderer1.sprite = Fallen_Square_up;
                    spriterenderer2.sprite = Fallen_Square_down;
                }
                else if (Rigidbox1.transform.position.y > Rigidbox2.transform.position.y)
                {
                    spriterenderer1.sprite = Fallen_Square_down;
                    spriterenderer2.sprite = Fallen_Square_up;
                }
            }
            else if (Rigidbox1.transform.position.y >= GridChange && Rigidbox2.transform.position.y >= GridChange)
            {
                if (Rigidbox1.transform.position.y != Rigidbox2.transform.position.y)
                {
                    spriterenderer1.sprite = Fallen_Triangle_down;
                    spriterenderer2.sprite = Fallen_Triangle_down;
                }
                else if (Rigidbox1.transform.position.x < Rigidbox2.transform.position.x)
                {
                    spriterenderer1.sprite = Fallen_Triangle_right;
                    spriterenderer2.sprite = Fallen_Triangle_left;
                }
                else if (Rigidbox1.transform.position.x > Rigidbox2.transform.position.x)
                {
                    spriterenderer1.sprite = Fallen_Triangle_left;
                    spriterenderer2.sprite = Fallen_Triangle_right;
                }
                if (Mathf.Abs(Rigidbox1.transform.position.x) % 1 == ((Rigidbox1.transform.position.y - GridChange) % 2) / 2)
                {
                    Rigidbox1.transform.localScale = Vector3.one;
                }
                else
                {
                    Rigidbox1.transform.localScale = new Vector3(1, -1, 1);
                }
                if (Mathf.Abs(Rigidbox2.transform.position.x) % 1 == ((Rigidbox2.transform.position.y - GridChange) % 2) / 2)
                {
                    Rigidbox2.transform.localScale = Vector3.one;
                }
                else
                {
                    Rigidbox2.transform.localScale = new Vector3(1, -1, 1);
                }
            }
            else if (Rigidbox1.transform.position.y >= GridChange && Rigidbox2.transform.position.y < GridChange)
            {
                spriterenderer1.sprite = Triangle_down;
                spriterenderer2.sprite = Square_up;
                Rigidbox1.transform.localScale = Vector3.one;
                Rigidbox2.transform.localScale = Vector3.one;
            }
            else if (Rigidbox1.transform.position.y < GridChange && Rigidbox2.transform.position.y >= GridChange)
            {
                spriterenderer1.sprite = Square_up;
                spriterenderer2.sprite = Triangle_down;
                Rigidbox1.transform.localScale = Vector3.one;
                Rigidbox2.transform.localScale = Vector3.one;
            }
        }
        isMoving1 = false;
        isMoving2 = false;
    }
}
    
