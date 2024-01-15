using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    Camera camera;

    [SerializeField]
    GameObject projectile;

    //Initializes the timer and counter
    [SerializeField] Counter_Timer count;

    Rigidbody rb;

    // Update is called once per frame
    void Update()
    {
        //If left mouse is clicked ...
        if (Input.GetMouseButtonDown(0) && count.waterball > 0)
        {
            Vector3 point = new Vector3(camera.pixelWidth / 2, camera.pixelHeight / 2, 0);

            //Get where the mouse clicked
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            
            RaycastHit hit;

            //If the raycast hits an object
            if (Physics.Raycast(ray, out hit))
            {
                createSphere(hit.point);

            }
        }
    }

    private void createSphere(Vector3 pos)
    {
        GameObject sphere = Instantiate(projectile);

        Vector3 add = new Vector3(0f, 5f, 0f);
        sphere.transform.position = pos + add;

        count.waterball--;
    }

}
