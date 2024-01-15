using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; //Import Text Mesh Pro

public class Loading : MonoBehaviour
{
    //Intitializes the loading text
    [SerializeField] TextMeshProUGUI Text;

    [SerializeField] bool called = false;

    //Start is called before the first frame update
    void Update()
    {
        if (!called)
        {
            called = true;
            StartCoroutine(loading(1f));
        }
        
    }

    private IEnumerator loading(float s)
    {
        //Makes 2 iterations of "Loading ... "
        for (int i = 0; i < 2; i++)
        {
            //Makes 1 iteration of "Loading ... "
            for (int j = 0; j < 4; j++)
            {
                //Add elipses
                if (inRange(1, 3, j))
                {
                    Text.text += ".";
                }
                //Make the word return to the orignal
                else
                {
                    Text.text = "Loading";
                }
                
                //Wait for 1 second
                yield return new WaitForSeconds(s);
            }
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");

    }

    //Returns if the number is in the specified range
    private bool inRange(float min, float max, float num)
    {
        return num >= min && num <= max;
    }
}
