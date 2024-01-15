using UnityEngine;
using System.Collections;

public class AnimatePrefabs : MonoBehaviour
{
    //Holds the speed at which the prefab will rotate
    [SerializeField] float rotationSpeed;

    //Holds the speed at which the prefab will move up and down
    [SerializeField] float moveSpeed;

    //Holds the displacement from the midpoint that the prefab will travel
    [SerializeField] float moveRadius;

    //Holds the starting y coordinate of the prefab
    [SerializeField] float yStart;

    //Start is called before the first frame update
    void Start()
    {
        //Set the rotation speed to 5
        rotationSpeed = 5f;

        //Set the move speed to 2
        moveSpeed = 2f;

        //Set the move radius to 0.1
        moveRadius = 0.1f;

        //If the game object is the winter collectible ...
        if (gameObject.tag == "Winter Collect")
        {
            yStart = 160.2f;
            moveRadius = 1f;
        }

        //If the game object is the spring question mark ...
        else if (gameObject.tag == "Spring?")
        {
            yStart = 1.1f;
        }

        //If the game object is the summer question mark ...
        else if (gameObject.tag == "Summer?")
        {
            yStart = 0.8f;
        }


        //If the game object is the autumn question mark ...
        else if (gameObject.tag == "Autumn?")
        {
            yStart = 1.2f;
        }

        //If the game object is the winter/maze question mark ...
        else
        {
            yStart = 0.925f;
        }
    }

    //Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;

        float time = Time.time;


        if (gameObject.tag == "Winter Collect")
        {
            transform.Rotate(0f, 0f, rotationSpeed);
            transform.localPosition = new Vector3(pos.x, Mathf.Sin(moveSpeed * time) * moveRadius + yStart, pos.z);

        }

        else
        {
            transform.Rotate(0f, rotationSpeed, 0f);
            transform.localPosition = new Vector3(0f, Mathf.Sin(moveSpeed * time) * moveRadius + yStart, 0f);

        }
    }
}
