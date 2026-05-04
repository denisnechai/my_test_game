using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraMovement : MonoBehaviour
{
    public float offset = -10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
    public void Move()
    {
        if (SceneManager.GetActiveScene().name.Contains("World"))
        {
            float minx = float.PositiveInfinity;
            float miny = float.PositiveInfinity;
            float maxx = float.NegativeInfinity;
            float maxy = float.NegativeInfinity;
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (player.GetComponent<PlayerMovement>().camerapos.x < minx)
                {
                    minx = player.GetComponent<PlayerMovement>().camerapos.x;
                }
                if (player.GetComponent<PlayerMovement>().camerapos.y < miny)
                {
                    miny = player.GetComponent<PlayerMovement>().camerapos.y;
                }
                if (player.GetComponent<PlayerMovement>().camerapos.x > maxx)
                {
                    maxx = player.GetComponent<PlayerMovement>().camerapos.x;
                }
                if (player.GetComponent<PlayerMovement>().camerapos.y > maxy)
                {
                    maxy = player.GetComponent<PlayerMovement>().camerapos.y;
                }
            }
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Rigidplayer"))
            {
                if (player.transform.parent == null)
                {
                    if (player.GetComponent<RigidplayerMovement>().camerapos1.x < minx)
                    {
                        minx = player.GetComponent<RigidplayerMovement>().camerapos1.x;
                    }
                    if (player.GetComponent<RigidplayerMovement>().camerapos1.y < miny)
                    {
                        miny = player.GetComponent<RigidplayerMovement>().camerapos1.y;
                    }
                    if (player.GetComponent<RigidplayerMovement>().camerapos1.x > maxx)
                    {
                        maxx = player.GetComponent<RigidplayerMovement>().camerapos1.x;
                    }
                    if (player.GetComponent<RigidplayerMovement>().camerapos1.y > maxy)
                    {
                        maxy = player.GetComponent<RigidplayerMovement>().camerapos1.y;
                    }
                    if (player.GetComponent<RigidplayerMovement>().camerapos2.x < minx)
                    {
                        minx = player.GetComponent<RigidplayerMovement>().camerapos2.x;
                    }
                    if (player.GetComponent<RigidplayerMovement>().camerapos2.y < miny)
                    {
                        miny = player.GetComponent<RigidplayerMovement>().camerapos2.y;
                    }
                    if (player.GetComponent<RigidplayerMovement>().camerapos2.x > maxx)
                    {
                        maxx = player.GetComponent<RigidplayerMovement>().camerapos2.x;
                    }
                    if (player.GetComponent<RigidplayerMovement>().camerapos2.y > maxy)
                    {
                        maxy = player.GetComponent<RigidplayerMovement>().camerapos2.y;
                    }
                }
            }
            transform.position = new Vector3((minx + maxx) / 2, (miny + maxy) / 2, offset);
            gameObject.GetComponent<Camera>().orthographicSize = MathF.Max(MathF.Max(5, (maxx - minx) / 4 + 1), (maxy - miny) / 2 + 1);
        }
    }
}
