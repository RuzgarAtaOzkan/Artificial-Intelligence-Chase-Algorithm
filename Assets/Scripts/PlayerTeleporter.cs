using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleporter : MonoBehaviour
{
    [SerializeField] GameObject teleporter;
    
    int teleporterCount = 0;
    bool isTelInstantiated = false;
    bool isThrowable = true;

    private void Start()
    {
        teleporter = GameObject.Find("TeleporterBall"); // there has to be a gameobject called TeleporterBall
    }

    private void Update()
    {
        teleporter.transform.position = transform.position;

        if (isThrowable)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (teleporter == null)
                {
                    teleporter = Instantiate(teleporter, transform.position, Quaternion.identity);
                }
                GameObject currentTeleporter = Instantiate(teleporter, transform.position, Quaternion.identity);
                currentTeleporter.GetComponent<Rigidbody>().AddForce(transform.forward * 500f);
                currentTeleporter.transform.parent = null;
                teleporterCount++;
                if (teleporterCount > 666)
                {
                    isThrowable = false;
                }
            }
        }

    }
}
