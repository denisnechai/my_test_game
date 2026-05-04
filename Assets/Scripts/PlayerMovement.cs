using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class PlayerMovement : MonoBehaviour
{
    public float GridChange = 1;
    public SpriteRenderer spriteRenderer;
    public Vector3 camerapos;
    public Sprite Triangle;
    public Sprite Square;
    public Sprite Fallen_triangle;
    public Sprite Fallen_square;
    public bool isMoving = false;
    public bool noInput = false;
    public bool readyToMove = false;
    public bool isPushed = false;
    public bool ignoreIsMoving = false;
    public bool noHitLastTurn;
    public LayerMask blockingLayer;
    public LayerMask Hole;
    public LayerMask Ice;
    private string key;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camerapos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        if (readyToMove) return;
        if (isMoving) return;
        if(GameManager.isMoving) return;
        if(GameManager.noInput) return;
        GameManager.dontSave = false;
        SpriteUpdate();
        key = string.Empty;
        
        if (transform.position.y < GridChange)
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
        

        if (key != string.Empty && TryToMove(transform.position, key, true, false))
        {
            TryToMove(transform.position, key, false, false);
        }
    }
    public void SpriteUpdate()
    {
        if (transform.position.y < GridChange)
        {
            if (transform.position.z == 1f)
            {
                spriteRenderer.sprite = Fallen_square;
            }
            else if (transform.position.z == 0f)
            {
                spriteRenderer.sprite = Square;
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
                spriteRenderer.sprite = Triangle;
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
    public bool TryToMove(Vector3 pos, string key, bool check, bool slide)
    {
        if (readyToMove && !check && !ignoreIsMoving) return false;
        if (isMoving && !ignoreIsMoving) return false;
        if (GameManager.isMoving && !ignoreIsMoving) return false;
        var movement = Vector3.zero;
        var push = Vector3.zero;
        var MoveSpeed = 0f;
        var PushSpeed = 0f;
        var move = false;
        if (pos.y < GridChange)
        {
            if (key == "W")
            {
                movement = Vector3.up;
                if (pos.y >= GridChange - 1)
                {
                    push = Vector3.left / 2;
                }
                else
                {
                    push = Vector3.up;
                }
            }
            else if (key == "A")
            {
                movement = Vector3.left;
                push = movement;
            }
            else if (key == "S")
            {
                movement = Vector3.down;
                push = movement;
            }
            else if (key =="D")
            {
                movement = Vector3.right;
                push = movement;
            }
        }
        else
        {
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
            if (key == "A1")
            {
                movement = Vector3.left / 2;
                push = Vector3.left / 2;
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
            if (pos.y == GridChange)
            {
                if (Mathf.Abs(pos.x) % 1 == ((pos.y - GridChange) % 2) / 2)
                {
                    if (key == "Z1" || key == "X1")
                    {
                        push = Vector3.down;
                    }
                }
            }

            if (Mathf.Abs(pos.x) % 1 != ((pos.y - GridChange) % 2) / 2)
            {
                movement += push;
                push = movement - push;
                movement -= push;
            }
        }
        if (movement == Vector3.zero || push == Vector3.zero)
        {
            return false;
        }

        MoveSpeed = 6 * (Mathf.Sqrt((movement.x) * (movement.x) + (movement.y) * (movement.y)));
        PushSpeed = 6 * (Mathf.Sqrt((push.x) * (push.x) + (push.y) * (push.y)));


        var targetpos = pos + movement;

        if (pos.z == 1f)
        {
            RaycastHit2D hit_ = (Physics2D.Raycast(pos, movement, Mathf.Sqrt((movement.x) * (movement.x) + (movement.y) * (movement.y)), Hole, pos.z, pos.z));
            if (!hit_)
            {
                return false;
            }
        }
        if (pos.z == -1f)
        {
            RaycastHit2D hit_ = (Physics2D.Raycast(pos, movement, Mathf.Sqrt((movement.x) * (movement.x) + (movement.y) * (movement.y)), blockingLayer, pos.z, pos.z + 1));
            if (hit_ && (hit_.collider.gameObject.CompareTag("Wall")))
            {
                return false;
            }
        }

        RaycastHit2D hit = (Physics2D.Raycast(pos, movement, Mathf.Sqrt((movement.x) * (movement.x) + (movement.y) * (movement.y)), blockingLayer, pos.z, pos.z));
        if (!hit)
        {
            move = true;
        }
        else if (hit.collider.CompareTag("Box"))
        {
            var box = hit.collider.GetComponent<BoxMovement>();
            if (box != null && box.TryToPushBox(box.transform.position, key, true, false))
            {
                if (box != null && box.TryToPushBox(box.transform.position, key, check, false))
                {
                    move = true;
                }
            }
        }
        else if (hit.collider.CompareTag("Rigidbox"))
        {
            var rigidbox = hit.collider.transform.parent.GetComponent<RigidboxMovement>();

            if (rigidbox != null && rigidbox.TryToPushRigidbox(key, true))
            {
                if (rigidbox != null && rigidbox.TryToPushRigidbox(key, check))
                {
                    move = true;
                }
            }
        }
        else if (hit.collider.CompareTag("Player"))
        {
            var player = hit.collider.transform.GetComponent<PlayerMovement>();
            Debug.Log(player.gameObject.name);
            if (player.transform.rotation == transform.rotation)
            {
                if (pos.y == GridChange && player.transform.position.y == GridChange - 1 && player.TryToPushPlayer(Vector3.down, 0, Vector3.down, 0, key, true, false))
                {
                    if (player.TryToPushPlayer(Vector3.down, 0, Vector3.down, 0, key, check, false))
                    {
                        move = true;
                        Debug.Log(move);
                    }
                }
                else if ((Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.X)) && key == "S" && player.TryToPushPlayer(Vector3.down, 0, Vector3.down, 0, key, true, false))
                {
                    if (player.TryToPushPlayer(Vector3.down, 0, Vector3.down, 0, key, check, false))
                    {
                        move = true;
                        Debug.Log(move);
                    }
                }
                else if (player.TryToMove(player.transform.position, key, true, false) || player.TryToMove(player.transform.position, key += "1", true, false) || player.TryToMove(player.transform.position, key.Replace("1", string.Empty), true, false))
                {
                    move = true;
                    Debug.Log(move);
                }
            }
            else if (player.TryToPushPlayer(movement, MoveSpeed, push, PushSpeed, key, true, false))
            {
                if (player.TryToPushPlayer(movement, MoveSpeed, push, PushSpeed, key, check, false))
                {
                    move = true;
                    Debug.Log(move);
                }
            }
        }
        Debug.Log(transform.position);
        Debug.Log("move = " + move);
        Debug.Log("check = " + check);
        Debug.Log("noHitLastTurn = " + noHitLastTurn);
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
                StartCoroutine(MoveToPosition(targetpos, MoveSpeed, push, PushSpeed, true, false, false));
            }
            return true;
        }
        else
        {
            return false;
        }
        
    }
    public bool TryToPushPlayer(Vector3 chainpush, float ChainPushSpeed, Vector3 push, float PushSpeed, string key, bool check, bool slide)
    {
        if (isMoving) return false;
        if (GameManager.isMoving) return false;
        var move = false;
        var targetpos = transform.position + push;
        ChainPushSpeed = 6 * (Mathf.Sqrt((chainpush.x) * (chainpush.x) + (chainpush.y) * (chainpush.y)));
        PushSpeed = 6 * (Mathf.Sqrt((push.x) * (push.x) + (push.y) * (push.y)));
        if (transform.position.z == 1f)
        {
            RaycastHit2D[] hit_ = (Physics2D.RaycastAll(transform.position, chainpush, Mathf.Sqrt((push.x) * (push.x) + (push.y) * (push.y)), Hole, transform.position.z, transform.position.z));
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
                box.pushedByPushedPlayer = true;
                move = true;
            }
            else if (box != null && box.TryToMultipushBox(push, 0, push, 0, key, check))
            {
                box.pushedByPushedPlayer = true;
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
                rigidbox.pushedByPushedPlayer = true;
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
                StartCoroutine(MoveToPosition(targetpos, PushSpeed, chainpush, ChainPushSpeed, false, false, false));
            }
            return true;
        }
        else
        {
            return false;
        }
    }
    public IEnumerator MoveToPosition(Vector3 targetpos, float MoveSpeed, Vector3 push, float PushSpeed, bool stopIfPushed,  bool enteranimation, bool exitanimation)
    {
        readyToMove = true;
        yield return null;
        if (stopIfPushed)
        {
            if (isPushed)
            {
                readyToMove = false;
                isPushed = false;
                yield break;
            }
        }
        if (isMoving && !ignoreIsMoving) yield break;
        isMoving = true;
        var move = targetpos - transform.position;
        if (enteranimation)
        {
            Debug.Log("Animation started");
            Debug.Log(isMoving);
        }
        if (enteranimation)
        {
            GameManager.dontSave = true;
            spriteRenderer.sortingOrder = 1;
            while (Vector3.Distance(transform.position, targetpos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetpos, MoveSpeed / 2 * Time.deltaTime);
                transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, 6 * Time.deltaTime);
                yield return null;
            }
            Debug.Log("Animation ended");
            isMoving = false;
            yield break;
        }
        else if (exitanimation)
        {
            spriteRenderer.sortingOrder = 1;
            var scale = Vector3.zero;
            if (targetpos.y >= GridChange)
            {
                if (Mathf.Abs(targetpos.x) % 1 == ((targetpos.y - GridChange) % 2) / 2)
                {
                    scale = new Vector3(1, 1, 1);
                }
                else
                {
                    scale = new Vector3(1, -1, 1);
                }
            }
            else
            {
                scale = Vector3.one;
            }
            
            while (Vector3.Distance(transform.position, targetpos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetpos, MoveSpeed / 2 * Time.deltaTime);
                transform.localScale = Vector3.MoveTowards(transform.localScale, scale, 6 * Time.deltaTime);
                yield return null;
            }
            transform.position = targetpos;
            transform.localScale = Vector3.one;
            Debug.Log("Animation ended");
            SpriteUpdate();
            readyToMove = false;
            isMoving = false;
            GameManager.dontSave = false;
            yield break;
        }
        else
        {
            var avgmove = (targetpos - transform.position + push) / 2;
            var avgSpeed = (MoveSpeed + PushSpeed) / 2;
            var avgtarget = (transform.position + avgmove);
            bool square = false;
            Vector3 cameratarget;
            Debug.Log(transform.position);
            Debug.Log(avgSpeed);
            Debug.Log(avgtarget);
            if (transform.position.y < GridChange)
            {
                square = true;
                while (Vector3.Distance(transform.position, targetpos) > 0.01f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetpos, MoveSpeed * Time.deltaTime);
                    camerapos = Vector3.MoveTowards(camerapos, targetpos, MoveSpeed * Time.deltaTime);
                    yield return null;
                }
            }
            else
            {
                var pos = transform.position;
                var time = 0f;
                while (Vector3.Distance(transform.position, avgtarget) > 0.01f)
                {
                    time += Time.deltaTime;
                    transform.position = Vector3.MoveTowards(pos, avgtarget, avgSpeed * time);
                    cameratarget = Vector3.Lerp(targetpos, transform.position, 0.5f);
                    camerapos = Vector3.MoveTowards(camerapos, cameratarget, avgSpeed * time);
                    yield return null;
                }
            }

            if (!square)
            {
                camerapos = Vector3.Lerp(targetpos, avgtarget, 0.5f);
            }
        }
        transform.position = targetpos;
        
        if (transform.position.z == -1f)
        {
            if (!Physics2D.OverlapPoint(transform.position, blockingLayer, 0f, 0f))
            {
                transform.position += Vector3.forward;
            }
        }
        if (transform.position.z == 0f)
        {
            if(Physics2D.OverlapPoint(transform.position, Hole, 1f, 1f) && !Physics2D.OverlapPoint(transform.position, blockingLayer, 1f, 1f))
            {
                transform.position += Vector3.forward;
            }
        }
        SpriteUpdate();
        if (!noInput) yield return new WaitForSeconds(0.03f);
        readyToMove = false;
        isMoving = false;
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
        if (Physics2D.OverlapPoint(transform.position, Ice))
        {
            Debug.Log("On ice");
            noInput = true;
            yield return new WaitUntil(() => !GameManager.isMoving);
            yield return new WaitForSeconds(0.03f);
            yield return null;
            if (!isPushed && TryToMove(transform.position, key, true, true))
            {
                Debug.Log("Sliding");
                Debug.Log(isMoving);
                noInput = true;
                yield return null;
                TryToMove(transform.position, key, false, true);
            }
            else if (isPushed && TryToPushPlayer(move, 0, push, 0, key, true, true))
            {
                Debug.Log("Sliding");
                Debug.Log(isMoving);
                noInput = true;
                yield return null;
                TryToPushPlayer(move, 0, push, 0, key, false, true);
            }
            else
            {
                noInput = false;
                isPushed = false;
            }
        }
        else
        {
            noInput = false;
            isPushed = false;
        }
    }
}
