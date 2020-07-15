using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterBall : MonoBehaviour
{
    GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            
            TeleportToBall(player.transform);
            Destroy(gameObject);
        }
    }

    public Transform TeleportToBall(Transform player)
    {
        player.transform.position = new Vector3(transform.position.x, player.transform.position.y, transform.position.z);
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        return transform;
    }
}
