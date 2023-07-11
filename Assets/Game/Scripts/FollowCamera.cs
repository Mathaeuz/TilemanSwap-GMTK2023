using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Camera Camera;

    private Vector3 StartingPos; //This is starting position of the sprites.
    private Vector3 SizeOfSprite;    //This is the length of the sprites.
    public Vector3 AmountOfParallax;  //This is amount of parallax scroll. 

    // Start is called before the first frame update
    void Start()
    {
        //Getting the starting X position of sprite.
        StartingPos = Camera.transform.position;
        //Getting the length of the sprites.
        SizeOfSprite = GetComponentInChildren<SpriteRenderer>().bounds.size;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 Position = Camera.transform.position;
        var temp = Position;
        temp.Scale(Vector3.one - AmountOfParallax);
        var distance = Position;
        distance.Scale(AmountOfParallax);

        if (temp.x > StartingPos.x + (SizeOfSprite.x / 2))
        {
            StartingPos.x += SizeOfSprite.x;
        }
        else if (temp.x < StartingPos.x - (SizeOfSprite.x / 2))
        {
            StartingPos.x -= SizeOfSprite.x;
        }
        if (temp.y > StartingPos.y + (SizeOfSprite.y / 2))
        {
            StartingPos.y += SizeOfSprite.y;
        }
        else if (temp.y < StartingPos.y - (SizeOfSprite.y / 2))
        {
            StartingPos.y -= SizeOfSprite.y;
        }

        transform.position = StartingPos + distance;
    }
}
