using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    GameObject player;
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        player.transform.position = gameObject.transform.position;
    }
}