using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEditor.PlayerSettings;
using static UnityEngine.Rendering.DebugUI;

public class BoxMovement : MonoBehaviour
{
    public LayerMask blockingLayer;
    public LayerMask Hole;
    public LayerMask Ice;
    public bool rotational = false;
    public Dictionary<string, string> RotateRight = new Dictionary<string, string>();
    public Dictionary<string, string> RotateLeft = new Dictionary<string, string>();
    public bool isMoving = false;
    public bool noInput = false;
    public bool won = true;
    public string pushed = string.Empty;
    public string sliding = string.Empty;
    public bool noHitLastTurn;
    public bool pushedByPushedPlayer = false;
    public bool ignoreIsMoving = false;
    public float GridChange = 1;
    public SpriteRenderer spriteRenderer;
    public Sprite Triangle;
    public Sprite Square;
    public Sprite Rotationalsquare;
    public Sprite Rotationaltriangle;
    public Sprite Fallen_square;
    public Sprite Fallen_triangle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RotateRight.Add("W1", "E1");
        RotateRight.Add("E1", "D1");
        RotateRight.Add("D1", "X1");
        RotateRight.Add("X1", "Z1");
        RotateRight.Add("Z1", "A1");
        RotateRight.Add("A1", "W1");
        RotateLeft.Add("W1", "A1");
        RotateLeft.Add("E1", "W1");
        RotateLeft.Add("D1", "E1");
        RotateLeft.Add("X1", "D1");
        RotateLeft.Add("Z1", "X1");
        RotateLeft.Add("A1", "Z1");
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving) return;
        if (!won) return;
        GameManager.dontSave = false;
        if (transform.position.y < GridChange)
        {
            if (transform.position.z == 1f)
            {
                spriteRenderer.sprite = Fallen_square;
            }
            else if (transform.position.z == 0f)
            {
                if (rotational)
                {
                    spriteRenderer.sprite = Rotationalsquare;
                }
                else
                {
                    spriteRenderer.sprite = Square;
                }
            }
        }
        else
        {
            if (transform.position.z == 1f)
            {
                spriteRenderer.sprite = Fallen_triangle;
            }
            else
            {
                if (rotational)
                {
                    spriteRenderer.sprite = Rotationaltriangle;
                }
                else
                {
                    spriteRenderer.sprite = Triangle;
                }
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
    public bool TryToPushBox(Vector3 pos, string key, bool check, bool slide)
    {
        if (!won) return false;
        if (isMoving && !ignoreIsMoving) return true;
        if (!slide)
        {
            pushed = key;
            /*Debug.Log("Pushed = " + pushed);
            Debug.Log("Sliding = " + sliding);
            Debug.Log("Check = " + check);*/
            if (sliding != string.Empty && TryToPushBox(pos, sliding, true, true) && !check)
            {
                return true;
            }
        } 
        else  
        {
            sliding = key;
            /*Debug.Log("Sliding = " + sliding);
            Debug.Log("Pushed = " + pushed);
            Debug.Log("Check = " + check);*/
        }
        

        var push = Vector3.zero;
        var chainpush = Vector3.zero;
        var PushSpeed = 0f;
        var ChainPushSpeed = 0f;
        var angle = 0f;
        var move = false;

        if (pos.y < GridChange)
        {

            if (key == "W")
            {
                push = Vector3.up;
                if (pos.y >= GridChange - 1)
                {
                    key = "W1";
                    chainpush = Vector3.left / 2;
                }
                else
                {
                    chainpush = Vector3.up;
                }
            }
            if (key == "A1")
            {
                key = "A";
            }
            if (key == "A")
            {
                chainpush = Vector3.left;
                push = chainpush;
            }
            if (key == "X1" || key == "Z1")
            {
                key = "S";
            }
            if (key == "S")
            {
                chainpush = Vector3.down;
                push = chainpush;
            }
            if (key == "D1")
            {
                key = "D";
            }
            if (key == "D")
            {
                chainpush = Vector3.right;
                push = chainpush;
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
                chainpush = Vector3.left / 2;
                push = Vector3.up;
            }
            if (key == "E1")
            {
                chainpush = Vector3.right / 2;
                push = Vector3.up;
            }
            if (key == "A")
            {
                key = "A1";
            }
            if (key == "A1")
            {
                chainpush = Vector3.left / 2;
                push = Vector3.left / 2;
            }
            if (key == "D")
            {
                key = "D1";
            }
            if (key == "D1")
            {
                chainpush = Vector3.right / 2;
                push = Vector3.right / 2;
            }
            if (key == "Z1")
            {
                chainpush = Vector3.down;
                push = Vector3.left / 2;
            }
            if (key == "X1")
            {
                chainpush = Vector3.down;
                push = Vector3.right / 2;
            }
            if (Mathf.Abs(pos.x) % 1 == ((pos.y - GridChange) % 2) / 2)
            {
                if (pos.y <= GridChange)
                {
                    if (key == "Z1" || key == "X1")
                    {
                        key = "S";
                        push = Vector3.down;
                    }
                }
            }
            if (Mathf.Abs(pos.x) % 1 == ((pos.y - GridChange) % 2) / 2)
            {
                chainpush += push;
                push = chainpush - push;
                chainpush -= push;
            }
            if (pos.y == GridChange && push == Vector3.down)
            {
                chainpush = Vector3.down;
            }
        }
        ChainPushSpeed = 6 * (Mathf.Sqrt((chainpush.x) * (chainpush.x) + (chainpush.y) * (chainpush.y)));
        PushSpeed = 6 * (Mathf.Sqrt((push.x) * (push.x) + (push.y) * (push.y)));

        var targetpos = pos + push;
        var rotationPoint = Vector3.positiveInfinity;

        if (rotational && transform.position.y > GridChange)
        {
            rotationPoint = DetermineRotationPoint(pos, key);
            angle = DetermineAngle(pos, key);
            if (angle == 60)
            {
                key = RotateRight[key];
            }
            else if (angle == -60)
            {
                key = RotateLeft[key];
            }
            Debug.Log(key);
        }

        if (pos.z == 1f)
        {
            RaycastHit2D hit1 = (Physics2D.Raycast(pos, push, Mathf.Sqrt((push.x) * (push.x) + (push.y) * (push.y)), Hole, pos.z, pos.z));
            if (!hit1)
            {
                return false;
            }
        }

        RaycastHit2D hit = (Physics2D.Raycast(pos, push, Mathf.Sqrt((push.x) * (push.x) + (push.y) * (push.y)), blockingLayer, pos.z, pos.z));
        if (!hit)
        {
            move = true;
        }
        else if (hit.collider.CompareTag("Box"))
        {
            var box = hit.collider.GetComponent<BoxMovement>();
            if (box != null && box.TryToPushBox(box.transform.position, key, check, false))
            {
                move = true;
            }
        }
        else if (hit.collider.CompareTag("Rigidbox"))
        {
            var rigidbox = hit.collider.transform.parent.GetComponent<RigidboxMovement>();
            if (rigidbox != null && rigidbox.TryToPushRigidbox(key, check))
            {
                move = true;
            }
        }
        else if (hit.collider.CompareTag("Player"))
        {
            var player = hit.collider.transform.GetComponent<PlayerMovement>();
            Debug.Log(player.gameObject.name);
            if (transform.position.y == GridChange && player.transform.position.y == GridChange - 1 && player.TryToPushPlayer(Vector3.down, 0, Vector3.down, 0, key, check, false))
            {
                move = true;
                Debug.Log(move);
            }
            else if ((Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.X)) && key == "S" && player.TryToPushPlayer(Vector3.down, 0, Vector3.down, 0, key, check, false))
            {
                move = true;
                Debug.Log(move);
            }
            else if (GameManager.noInput == true && player.TryToPushPlayer(push, 0, chainpush, 0, key, check, false))
            {
                player.isPushed = true;
                move = true;
                Debug.Log(move);
            }
            else if (player.TryToMove(player.transform.position, key, true, false) || player.TryToMove(player.transform.position, key += "1", true, false) || player.TryToMove(player.transform.position, key.Replace("1", string.Empty), true, false))
            {
                move = true;
                Debug.Log(move);
            }
        }
        if (!check || !move)
        {
            if (slide)
            {
                sliding = string.Empty;
            }
            else
            {
                pushed = string.Empty;
            }
        }
        if (slide && hit && noHitLastTurn && !check)
        {
            GameManager.dontSave = true;
            move = false;
            noInput = false;
        }
        if (!check)
        {
            if (!hit)
            {
                noHitLastTurn = true;
            }
            else
            {
                noHitLastTurn = false;
            }
        }

        if (move)
        {
            if (!check)
            {
                StartCoroutine(MoveToPosition(key, targetpos, PushSpeed, chainpush, ChainPushSpeed, rotationPoint, angle, true));   
            }
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool TryToMultipushBox(Vector3 chainpush, float ChainPushSpeed, Vector3 push, float PushSpeed, string key, bool check)
    {
        if (!won) return false;
        if (isMoving) return true;
        var move = false;
        ChainPushSpeed = 6 * (Mathf.Sqrt((chainpush.x) * (chainpush.x) + (chainpush.y) * (chainpush.y)));
        PushSpeed = 6 * (Mathf.Sqrt((push.x) * (push.x) + (push.y) * (push.y)));

        var targetpos = transform.position + push;

        if (transform.position.z == 1f)
        {
            RaycastHit2D[] hit_ = (Physics2D.RaycastAll(transform.position, push, Mathf.Sqrt((push.x) * (push.x) + (push.y) * (push.y)), Hole, transform.position.z, transform.position.z));
            if (((((push == Vector3.right || push == Vector3.left) && transform.position.y >= GridChange && push.magnitude == 1) || push.magnitude > 1) && hit_.Length < 2) || hit_.Length < 1)
            {
                return false;
            }
        }

        RaycastHit2D hit = (Physics2D.Raycast(transform.position, push, Mathf.Sqrt((push.x) * (push.x) + (push.y) * (push.y)), blockingLayer, transform.position.z, transform.position.z));

        if (!hit)
        {
            move = true;
        }
        else if (transform.position.y >= GridChange && Mathf.Abs(Mathf.Abs(transform.position.x - hit.collider.transform.position.x) * 2 + Mathf.Abs(transform.position.y - hit.collider.transform.position.y)) <= 1 && hit.collider.CompareTag("Box"))
        {
            var box = hit.collider.GetComponent<BoxMovement>();
            if (box != null && box.transform.position.y == GridChange && Mathf.Abs(box.transform.position.x) % 1 == ((box.transform.position.y - GridChange) % 2) / 2 && box.TryToMultipushBox(Vector3.down * 2, 0, Vector3.down * 2, 0, key, check))
            {
                if (pushedByPushedPlayer == true)
                {
                    box.pushedByPushedPlayer = true;
                }
                move = true;
            }
            else if (box != null && box.TryToMultipushBox(push, 0, push, 0, key, check))
            {
                if (pushedByPushedPlayer == true)
                {
                    box.pushedByPushedPlayer = true;
                }
                move = true;
            }
            else
            {
                return false;
            }
        }
        else if (transform.position.y >= GridChange && Mathf.Abs(Mathf.Abs(transform.position.x - hit.collider.transform.position.x) * 2 + Mathf.Abs(transform.position.y - hit.collider.transform.position.y)) <= 1 && hit.collider.CompareTag("Rigidbox"))
        {
            var rigidbox = hit.collider.transform.parent.GetComponent<RigidboxMovement>();
            if (rigidbox != null && rigidbox.TryToMultipushRigidbox(push, 0, push, 0, key, check))
            {
                if (pushedByPushedPlayer == true)
                {
                    rigidbox.pushedByPushedPlayer = true;
                }
                move = true;
            }
            else 
            { 
                return false; 
            }
        }
        else if (transform.position.y >= GridChange && Mathf.Abs(Mathf.Abs(transform.position.x - hit.collider.transform.position.x) * 2 + Mathf.Abs(transform.position.y - hit.collider.transform.position.y)) <= 1 && hit.collider.CompareTag("Player"))
        {
            var player = hit.collider.GetComponent<PlayerMovement>();
            if (player != null && player.transform.position.y == GridChange && Mathf.Abs(player.transform.position.x) % 1 == ((player.transform.position.y - GridChange) % 2) / 2 && player.TryToPushPlayer(Vector3.down * 2, 0, Vector3.down * 2, 0, key, check, false))
            {
                player.isPushed = true;
                move = true;
            }
            else if (player != null && player.TryToPushPlayer(push, 0, push, 0, key, check, false))
            {
                player.isPushed = true;
                move = true;
            }
            else
            {
                return false;
            }
        }
        else if (hit.collider.CompareTag("Box"))
        {
            var box = hit.collider.GetComponent<BoxMovement>();
            if (box != null && box.TryToPushBox(box.transform.position, key, check, false))
            {
                move = true;
            }
            else
            {
                return false;
            }
        }
        else if (hit.collider.CompareTag("Rigidbox"))
        {
            var rigidbox = hit.collider.transform.parent.GetComponent<RigidboxMovement>();
            if (rigidbox != null && rigidbox.TryToPushRigidbox(key, check))
            {
                move = true;
            }
            else
            {
                return false;
            }
        }
        else if (hit.collider.CompareTag("Player"))
        {
            var player = hit.collider.transform.GetComponent<PlayerMovement>();
            Debug.Log(player.gameObject.name);
            if (player.TryToMove(player.transform.position, key, true, false))
            {
                move = true;
                Debug.Log(move);
            }

        }
        if (move)
        {
            if (!check)
            {
                StartCoroutine(MoveToPosition(string.Empty, targetpos, PushSpeed, chainpush, ChainPushSpeed, Vector3.positiveInfinity, 0, false));
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    private Vector3 DetermineRotationPoint(Vector3 pos, string key)
    {
        if (Mathf.Abs(pos.x) % 1 == ((pos.y - GridChange) % 2) / 2)
        {
            if (key == "W1" || key == "X1")
            {
                return pos + new Vector3(-0.5f, -0.5f, 0);
            }
            else if (key == "E1" || key == "Z1")
            {
                return pos + new Vector3(0.5f, -0.5f, 0);
            }
            else if (key == "A1" || key == "D1")
            {
                return pos + Vector3.up / 2;
            }
            else return Vector3.positiveInfinity;
        }
        else
        {
            if (key == "W1" || key == "X1")
            {
                return pos + new Vector3(0.5f, 0.5f, 0);
            }
            else if (key == "E1" || key == "Z1")
            {
                return pos + new Vector3(-0.5f, 0.5f, 0);
            }
            else if (key == "A1" || key == "D1")
            {
                return pos + Vector3.down / 2;
            }
            else return Vector3.positiveInfinity;
        }
    }
    private float DetermineAngle(Vector3 pos, string key)
    {
        if (Mathf.Abs(pos.x) % 1 == ((pos.y - GridChange) % 2) / 2)
        {
            if (key == "W1" || key == "D1" || key == "Z1")
            {
                return -60;
            }
            else if (key == "E1" || key == "X1" || key == "A1")
            {
                return 60;
            }
            else { return 0; }
        }
        else
        {
            if (key == "W1" || key == "D1" || key == "Z1")
            {
                return 60;
            }
            else if (key == "E1" || key == "X1" || key == "A1")
            {
                return -60;
            }
            else { return 0; }
        }
    }

    private IEnumerator MoveToPosition(string key, Vector3 targetpos, float PushSpeed, Vector3 chainpush, float ChainPushSpeed, Vector3 rotationPoint, float rotationAngle, bool stopIfPushedByPushedPlayer)
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
        if (isMoving && !ignoreIsMoving) yield break;
        isMoving = true;
        if (rotational)
        {
            var angle = 0f;
            while (Mathf.Abs(angle) < Mathf.Abs(rotationAngle))
            {
                angle += 6 * rotationAngle * Time.deltaTime;
                transform.RotateAround(rotationPoint, Vector3.back, 6 * rotationAngle * Time.deltaTime);
                yield return null;
            }
        }
        else
        {
            var avg = (targetpos - transform.position + chainpush) / 2;
            var avgSpeed = (PushSpeed + ChainPushSpeed) / 2;
            var avgtarget = (transform.position + avg);
            var pos = transform.position;
            var time = 0f;
            while (Vector3.Distance(transform.position, avgtarget) > 0.01f)
            {
                time += Time.deltaTime;
                transform.position = Vector3.MoveTowards(pos, avgtarget, avgSpeed * time);
                yield return null;
            }
        }
        transform.rotation = Quaternion.identity;
        transform.position = targetpos;
        Debug.Log("Moved");
        if (transform.position.z == 0f)
        {
            if (Physics2D.OverlapPoint(transform.position, Hole, 1f, 1f) && !Physics2D.OverlapPoint(transform.position, blockingLayer, 1f, 1f))
            {
                transform.position += Vector3.forward;
            }
        }
        if (transform.position.y < GridChange)
        {
            if (transform.position.z == 1f)
            {
                spriteRenderer.sprite = Fallen_square;
            }
            else if (transform.position.z == 0f)
            {
                if (rotational)
                {
                    spriteRenderer.sprite = Rotationalsquare;
                }
                else
                {
                    spriteRenderer.sprite = Square;
                }
            }
        }
        else
        {
            if (transform.position.z == 1f)
            {
                spriteRenderer.sprite = Fallen_triangle;
            }
            else
            {
                if (rotational)
                {
                    spriteRenderer.sprite = Rotationaltriangle;
                }
                else
                {
                    spriteRenderer.sprite = Triangle;
                }
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
            
        Debug.Log(isMoving);
        Debug.Log(GameManager.isMoving);
        isMoving = false;
        if (Physics2D.OverlapPoint(transform.position, Ice))
        {
            noInput = true;
            yield return new WaitUntil(() => !GameManager.isMoving);
            yield return new WaitForSeconds(0.03f);
            yield return null;
            if (TryToPushBox(transform.position, key, true, true))
            {
                noInput = true;
                sliding = key;
                yield return null;
                TryToPushBox(transform.position, key, false, true);
            }
            else
            {
                noInput = false;
            }
        }
        else
        {
            noInput = false;
        }
        
    }
}


