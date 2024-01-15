using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Main_Menu : MonoBehaviour
{
    //Declare variables

    //Initializes the buttons
    [SerializeField] Button button1, button2;

    // Start is called before the first frame update
    void Start()
    {
        //Gets the buttons
        Button b1 = button1.GetComponent<Button>();
        Button b2 = button2.GetComponent<Button>();
    }

    //Loads game scene
    public void game_start()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
    
    //Loads start scene
    public void start_scene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Start");
    }
    
    //Loads instructions1 scene
    public void instructions_1()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Instructions1");
    }

    //Loads instructions2 scene
    public void instructions_2()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Instructions2");
    }

    //Loads instructions3 scene
    public void instructions_3()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Instructions3");
    }

    //Loads instructions4 scene
    public void instructions_4()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Instructions4");
    }

    //Loads instructions5 scene
    public void instructions_5()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Instructions5");
    }

    //Loads instructions6 scene
    public void instructions_6()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Instructions6");
    }

    //Loads instructions7 scene
    public void instructions_7()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Instructions7");
    }

    //Loads instructions8 scene
    public void instructions_8()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Instructions8");
    }

    //Loads instructions9 scene
    public void instructions_9()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Instructions9");
    }

    //Loads loading scene
    public void loading()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Loading");
    }
}