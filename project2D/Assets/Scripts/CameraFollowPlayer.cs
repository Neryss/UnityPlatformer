using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    public float smooth;
    public bool isFollowing;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(player && isFollowing)
        {
            Vector3 newPos = Vector3.Lerp(transform.position, player.position + offset, smooth * Time.deltaTime);
            transform.position = newPos;
        }
    }
}
