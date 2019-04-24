using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour {

    [SerializeField] Transform player;

    private void Start()
    {
        if (!player)
        {
            player = FindObjectOfType<Character>().transform;
        }
    }

    void LateUpdate () {

        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;
    }
}
