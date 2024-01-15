using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; //Import Text Mesh Pro

public class Counter_Timer : MonoBehaviour
{
    //Declare variables

    //Initializes GameBehaviour
    [SerializeField] GameBehaviour gameManager;

    //Initializes MoveCharacter
    [SerializeField] MoveCharacter player;

    //Initializes the AllyNPC
    [SerializeField] AllyNPC allyNPC;

    //Initializes SoundControl
    [SerializeField] SoundControl sound;

    //For the timer
    //Initializes how much time the player has at the start of the game
    [SerializeField] float startTime;

    //Keeps track of how much time (in seconds) the player has left
    public float timeRemaining;

    //Initializes the timer to not running
    bool timerIsRunning = false;

    //Holds whether or not the player has won
    public bool won;

    //Holds whether the game is over or not
    public bool gameOver;

    //Holds whether the typeWriter is done typing or not
    public bool doneTyping;

    string line = "";

    //For the counter
    //Holds the maximum amount of items that can be collected for each charm
    [SerializeField] int[] maxItems;

    //Holds the amount of collected items for each charm
    [SerializeField] int[] itemsCollected;

    //Holds the amount of hidden items collected
    [SerializeField] int[] hiddenCounter;

    //Initalizes the space where the text is going to be put
    [SerializeField] TextMeshProUGUI counterTimerText, hiddenCounterText, announcementText, endMessageText, timeTakenText, foundStatsText, destroyedStatsText;

    //Initializes the box that holds the announcementText
    [SerializeField] GameObject interactBox;

    //Initializes the main and summary canvases
    [SerializeField] Canvas mainCanvas, endCanvas;

    //Sets and returns the number of winter charms collected
    public int winterItem
    {
        get { return itemsCollected[0]; }
        set { itemsCollected[0] = value; }
    }

    //Sets and returns the number of spring charms collected
    public int springItem
    {
        get { return itemsCollected[1]; }
        set { itemsCollected[1] = value; }
    }

    //Sets and returns the number of summer charms collected
    public int summerItem
    {
        get { return itemsCollected[2]; }
        set { itemsCollected[2] = value; }
    }

    //Sets and returns the number of autumn charms collected
    public int autumnItem
    {
        get { return itemsCollected[3]; }
        set { itemsCollected[3] = value; }
    }

    //Sets and returns the number of pestle and mortar charms collected
    public int mixItem
    {
        get { return itemsCollected[4]; }
        set { itemsCollected[4] = value; }
    }

    //Sets and returns the number of bottle charms collected
    public int bottleItem
    {
        get { return itemsCollected[5]; }
        set { itemsCollected[5] = value; }
    }

    //Sets and returns the number of waterballs collected
    public int waterball
    {
        get { return hiddenCounter[0]; }
        set { hiddenCounter[0] = value; }
    }

    //Sets and returns the number of sticks collected
    public int stick
    {
        get { return hiddenCounter[1]; }
        set { hiddenCounter[1] = value; }
    }

    //Sets and returns the number of firesticks collected
    public int firestick
    {
        get { return hiddenCounter[2]; }
        set { hiddenCounter[2] = value; }
    }

    //Sets and returns the number of herbs collected
    public int herb
    {
        get { return hiddenCounter[3]; }
        set { hiddenCounter[3] = value; }
    }

    //Start is called before the first frame update
    void Start()
    {
        //Gets the GameBehaviour script
        gameManager = GameObject.Find("GameManager").GetComponent<GameBehaviour>();

        //Gets the MoveCharacter script
        player = GameObject.Find("Player").GetComponent<MoveCharacter>();

        //Gets the SoundControl script
        sound = GameObject.Find("Main Canvas").GetComponent<SoundControl>();

        //Get the mainCanvas
        //mainCanvas = GameObject.Find("Main Canvas").GetComponent<Canvas>();

        //Get the endCanvas
        //endCanvas = GameObject.Find("End Canvas").GetComponent<Canvas>();

        //Initializes variables
        initialize();
    }

    //Update is called once per frame
    void Update()
    {
        //If in autumn ...
        if (inSeason("Autumn", player.slimePosition) || inSeason("Maze", player.slimePosition)) {
            //Change text color to white
            counterTimerText.color = Color.white;
            hiddenCounterText.color = Color.white;
        }

        //If not in autumn ...
        else
        {
            //Change text color to black
            counterTimerText.color = Color.black;
            hiddenCounterText.color = Color.black;
        }

        
        //Checks to see if the timer is still running
        if (timerIsRunning)
        {
            //If time is still left ... 
            if (timeRemaining > 0)
            {
                //Subtract how much time has passed from timeRemaining
                timeRemaining -= Time.deltaTime;
            }

            //If time has run out ...
            else
            {
                //Set timeRemaining to 0, so there isn't negative time
                timeRemaining = 0;

                //Stop the timer from running
                timerIsRunning = false;

                //Game has ended
                gameOver = true;

                //Start end of game procedures
                GameEnd(timeRemaining);
            }
        }

        //Displays the counters and the time
        DisplayText(timeRemaining);

        //Checks to see if the game is over
        if (haveAllItems() && !gameOver)
        {
            //Game has ended
            gameOver = true;

            //Player won game
            won = true;

            //Start end of game procedures
            GameEnd(timeRemaining);
        }

        if (!allyNPC.interacting)
        {
            //Disable interaction box
            interactBox.SetActive(false);
        }

        else
        {
            //Enable interaction box
            interactBox.SetActive(true);
        }
    }

    public void typeText(string str)
    {
        //StartCoroutine(typeWriter(line, 0.01f, announcementText));
        //announcementText.text = line;
        line = str;
    }

    private IEnumerator typeWriter(string str, float delay, TextMeshProUGUI box)
    {
        box.text = "";

        for (int i = 0; i < str.Length; i++)
        {
            box.text += str.Substring(i, 1);
            yield return new WaitForSeconds(delay);
        }

        //allyNPC.typingIndex++;

        doneTyping = true;
    }

    void DisplayText(float timeToDisplay)
    {
        //Makes sure that the time is not negative
        if (timeToDisplay < 0)
        {
            timeToDisplay = 0;
        }

        //Gets the number of minutes
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);

        //Gets the number of seconds
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        
        //Formats and puts the counters and timer into the text
        counterTimerText.text = string.Format("Snowflakes Found: {0:0}/{1:0}\nFlowers Found: {2:0}/{3:0}\nCrystals Found: {4:0}/{5:0}\nFruits Found: {6:0}/{7:0}\nPestle & Mortar Found: {8:0}/{9:0}\nPotion Bottle Found: {10:0}/{11:0}\n\nTime Remaining: {12:00}:{13:00}", itemsCollected[0], maxItems[0], itemsCollected[1], maxItems[1], itemsCollected[2], maxItems[2], itemsCollected[3], maxItems[3], itemsCollected[4], maxItems[4], itemsCollected[5], maxItems[5], minutes, seconds);

        announcementText.text = line;

        string hidden0 = "";
        string hidden1 = "";
        string hidden2 = "";
        string hidden3 = "";
        string hidden4 = "";

        if (hiddenCounter[0] > 0 || hiddenCounter[1] > 0 || hiddenCounter[2] > 0 || hiddenCounter[3] > 0)
        {
            hidden0 = "Inventory: ";
        }

        if (hiddenCounter[0] > 0)
        {
            hidden1 = "Waterballs: " + hiddenCounter[0];
        }
        
        if (hiddenCounter[1] > 0)
        {
            hidden2 = "Sticks: " + hiddenCounter[1];
        }

        if (hiddenCounter[2] > 0)
        {
            hidden3 = "Fire Sticks: " + hiddenCounter[2];
        }

        if (hiddenCounter[3] > 0)
        {
            hidden4 = "Herbs: " + hiddenCounter[3];
        }

        hiddenCounterText.text = hidden0 + "\n" + hidden1 + "\n" + hidden2 + "\n" + hidden3 + "\n" + hidden4;


    }

    //End of game procedures
    public void GameEnd(float timeToDisplay)
    {
        //Disable main canvas and enable the ending canvas
        mainCanvas.gameObject.SetActive(false);
        endCanvas.gameObject.SetActive(true);

        //Mute background music and sound effects
        sound.mute();

        //If player won ... 
        if (won)
        {
            //Play celebration music
            GameObject.Find("Win Music").GetComponent<AudioSource>().Play();

            //Display ending message
            endMessageText.text = string.Format("Congratulations! You have found all the charms and helped Serenade save the seasons!");
        }

        //If player did not win ...
        else
        {
            //Play sad music
            GameObject.Find("Lose Music").GetComponent<AudioSource>().Play();

            //Display ending message
            endMessageText.text = string.Format("Time's Up! Unfortunately, Roxie was able to destroy some of the charms.");
        }

        //Makes sure that the time is not negative
        if (timeToDisplay < 0)
        {
            timeToDisplay = 0;
        }

        //Gets the number of minutes
        float minutes = Mathf.FloorToInt((startTime - timeToDisplay) / 60);

        //Gets the number of seconds
        float seconds = Mathf.FloorToInt((startTime - timeToDisplay) % 60);

        //Display the time taken
        timeTakenText.text = string.Format("Time Taken: {0:00}:{1:00}", minutes, seconds);

        //Display the ending stats
        EndingStats();
    }

    //Displays the ending statistics
    void EndingStats()
    {
        //Display the number of charms that were found
        foundStatsText.text = string.Format("Charms Found:\nSnowflakes: {0:0}/2\nFlowers: {1:0}/2\nCrystals: {2:0}/2\nFruits: {3:0}/2\nPestle & Mortar: {4:0}/1\nPotion Bottle: {5:0}/1", itemsCollected[0], itemsCollected[1], itemsCollected[2], itemsCollected[3], itemsCollected[4], itemsCollected[5]);

        //Display the number of charms that were not found
        destroyedStatsText.text = string.Format("Charms Destroyed:\nSnowflakes: {0:0}/2\nFlowers: {1:0}/2\nCrystals: {2:0}/2\nFruits: {3:0}/2\nPestle & Mortar: {4:0}/1\nPotion Bottle: {5:0}/1", 2-itemsCollected[0], 2-itemsCollected[1], 2-itemsCollected[2], 2-itemsCollected[3], 1-itemsCollected[4], 1-itemsCollected[5]);

    }

    //Returns whether or not all the items were found
    bool haveAllItems()
    {
        for (int i = 0; i < itemsCollected.Length; i++)
        {
            if (itemsCollected[i] != maxItems[i])
                return false;
        }

        return true;
    }

    //Returns whether the position is in a season
    private bool inSeason(string season, Vector3 pos)
    {
        switch (season)
        {
            //Returns if position is in winter
            case "Winter": case "winter":
                return inRange(0, 990, pos.x) && inRange(-1000, -90, pos.z);

            //Returns if position is in spring
            case "Spring": case "spring":
                return inRange(0, 990, pos.x) && inRange(90, 1000, pos.z);

            //Returns if position is in summer
            case "Summer": case "summer":
                return inRange(1090, 2000, pos.x) && inRange(90, 1000, pos.z);

            //Returns if position is in autumn
            case "Autumn": case "autumn":
                return inRange(1090, 2000, pos.x) && inRange(-1000, -90, pos.z);
            case "Maze": case "maze":
                return inRange(3500, 4025, pos.x) && inRange(1680, 2200, pos.z);
        }

        return false;
    }

    //Return whether the number is within the minimum and maximum values
    private bool inRange(float min, float max, float num)
    {
        return num <= max && num >= min;
    }

    private void initialize()
    {
        //Set start time to 1800 seconds (30 minutes)
        startTime = 1800f;

        //Set the time remaining to the start time
        timeRemaining = startTime;

        //Starts the timer automatically
        timerIsRunning = true;

        //Enable mainCanvas
        mainCanvas.gameObject.SetActive(true);

        //Disable endCanvas
        endCanvas.gameObject.SetActive(false);

        //Disable announcementText
        interactBox.SetActive(false);

        //Player has not won yet
        won = false;

        //The game is not over yet
        gameOver = false;

        //Set the maximum number of charms that can be collected
        maxItems = new int[]{ 2, 2, 2, 2, 1, 1 };

        //Set the number of charms that have been collected
        itemsCollected = new int[]{ 0, 0, 0, 0, 0, 0 };

        //Set the number of hidden items that have been collected
        hiddenCounter = new int[] { 0, 0, 0, 0};

        //Not currently typing something
        doneTyping = false;
    }
}