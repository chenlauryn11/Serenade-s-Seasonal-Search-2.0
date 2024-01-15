using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCharacter : MonoBehaviour
{
    //Declare variables
    //Initializes the timer and counter
    [SerializeField] Counter_Timer count;

    //Initializes GameBehaviour
    [SerializeField] GameBehaviour gameManager;

    //Initializes AllyNPC
    [SerializeField] AllyNPC allyNPC;

    //Initializes Panel
    [SerializeField] GameObject waterPanel;

    //Initializes a place to hold the direction the player is moving
    [SerializeField] Vector3 direction;

    //Controls the faces on the slime
    [SerializeField] Face faces;

    //The slime's body
    [SerializeField] GameObject slimeBody;

    //The material for the face
    Material faceMaterial;

    //Holds the transform components of the ally npcs
    [SerializeField] Transform[] allyTransform;

    //Holds the tags for the all npcs
    [SerializeField] string[] allyTags;

    //Holds the two different cameras that are used
    [SerializeField] Camera cameraNormal, cameraMaze;

    //Allows the player to be moved or not
    public bool disabled;

    //Holds whether the player touched the flowers or not
    [SerializeField] bool touchedFlowers;

    //Holds whether or not the controls will be backwards
    [SerializeField] bool backward;

    //Holds whether self-created gravity is to be used
    [SerializeField] bool useGravity = true;

    //Holds whether system gravity is to be used
    [SerializeField] bool useSystemGravity = false;

    //The speed in which the player turns
    [SerializeField] float rotationSpeed = 3f;

    //Shows whether the player is in water or not
    [SerializeField] bool inWater;

    //Holds the arrow keyboard inputs
    [SerializeField] float horizontalInput;
    [SerializeField] float verticalInput;

    //Holds the speeds of the player in water
    [SerializeField] float waterSpeed; //Moving forward
    [SerializeField] float waterUp; //Moving up

    //Holds the distance of the raycast to see the NPCs
    private float fovDist = 60f;

    //Holds the angle of the raycast to see the NPCs
    private float fovAngle = 25f;

    //Holds the current health and the maximum health
    public int health, maxHealth;

    //Holds which index in the hearts array
    public int heartIndex;

    //Holds the HPBar prefab
    public Transform HPBar;

    //The HP Bar that is the child of the player
    GameObject HPBarChild;

    //Holds the individual hearts in the HP bar
    GameObject[] hearts = new GameObject[5];

    Vector3 originalPosition;
    Quaternion originalRotation;

    //Initializes the rigidbody and animator on the player
    private Rigidbody rb;
    private Animator anim;

    //Holds all of the illusion flowers
    GameObject[] flowers;

    //Shares the player's position to other scripts/classes
    public Vector3 slimePosition;

    //Start is called before the first frame update
    void Start()
    {
        //Get the rigidbody
        rb = GetComponent<Rigidbody>();

        //Get the animator
        anim = GetComponent<Animator>();
        
        //Gets the GameBehaviour script
        gameManager = GameObject.Find("GameManager").GetComponent<GameBehaviour>();

        //Holds the position of the player
        slimePosition = transform.position;

        //Gets the face of the slime
        faceMaterial = slimeBody.GetComponent<Renderer>().materials[1];

        //Get the slime's initial position
        originalPosition = transform.position;

        //Get the slime's intitial rotation
        originalRotation = transform.rotation;

        initialize();
    }

    //Update is called once per frame
    void Update()
    {
        //Make slimePosition the same as the player's position
        slimePosition = transform.position;

        //If player is in maze ...
        if (inSeason("Maze", slimePosition)) {
            //Enable maze camera
            cameraMaze.enabled = true;

            //Disable normal camera
            cameraNormal.enabled = false;
        }

        //If player is not in maze ...
        else
        {
            //Enable normal camera
            cameraNormal.enabled = true;

            //Disable maze camera
            cameraMaze.enabled = false;
        }

        //If slime is deep enough in the water ...
        if (slimePosition.y < 125)
        {
            //Make screen look blue
            waterPanel.SetActive(true);
        }

        //If player is not deep enough in water or is not in water ... 
        else
        {
            //Makes screen normal
            waterPanel.SetActive(false);

        }

        //If player has plants and heal key is pressed ...
        if (Input.GetKeyDown(KeyCode.R))
        {
            heal();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            int ind = -1;
            for (int i = 0; i < allyTransform.Length; i++)
            {
                if (canSeeNPC(allyTransform[i]))
                {
                    ind = i;
                    break;
                }
            }
            
            if (ind >= 0)
            {
                Debug.Log("Interacting with " + allyTags[ind]);
                allyNPC.interact(ind);
            }
        }

        //Set backward
        backward = inSeason("Spring", transform.position) && touchedFlowers;

        //If the player controls are not disabled ... 
        if (!disabled)
        {
            //Make the player jump
            if (Input.GetButtonDown("Jump"))
            {
                //Make animation for jumping
                anim.SetTrigger("Jump");
                rb.AddForce(transform.forward * 100);
            }

            //Get the horizontal component of movement
            horizontalInput = Input.GetAxis("Horizontal");

            //Get the vertical component of movement
            verticalInput = Input.GetAxis("Vertical");

            //If no input is given ...
            if (horizontalInput == 0 && verticalInput == 0 && !shiftKeyPressed())
            {
                //Keep player as is
                anim.SetFloat("Speed", 0);
                return;
            }

            //If up/w or down/s keys are pressed ...
            if (verticalInput == 0)
            {
                //Stop walking animation
                anim.SetFloat("Speed", 0);
            }

            //Declare the direction the player is moving in
            direction = new Vector3(horizontalInput, 0f, verticalInput);

            //Declare the angle the player will be facing in
            float targetAngle;
            
            //Makes the player's controls backwards
            if (backward)
            {
                //Make the angle the player is facing the opposite of the normal angle
                targetAngle = -horizontalInput * rotationSpeed;
            }

            //Makes the player's controls normal
            else
            {
                //Make the angle the player is facing the normal angle
                targetAngle = horizontalInput * rotationSpeed;
            }

            //Rotate the character based on the angle
            transform.Rotate(0f, targetAngle, 0f);

            //Ensure that the only vertical input being used is from the up arrow
            if (verticalInput > 0)
            {
                //Make the character move
                anim.SetFloat("Speed", verticalInput);
            }

            //Find out if player is in water
            inWater = isInWater();

            //If the player is in the water ...
            if (inWater)
            {
                //Make player swim
                anim.SetBool("Swim", true);

                //Ensure player does not walk
                anim.SetFloat("Speed", 0);

                //Holds the speed of the player in water moving forward when slowed
                float slowSpeed = 1000f;

                //Holds the speed of the player in water moving forward in normal speed
                float normalSpeed = 2000f;

                //Holds the speed of the player in water going up when slowed
                float slowUp = 1f;

                //Holds the speed of the player in water going up in normal speed
                float normalUp = 10f;

                //If player has fire from summer ...
                if (!inSeason("winter", slimePosition) || count.firestick > 0)
                {
                    //Speed in water is normal
                    waterSpeed = normalSpeed;
                    waterUp = normalUp;
                }

                //If fire does not have fire from summer ...
                else
                {
                    //Speed in water is slowed
                    waterSpeed = slowSpeed;
                    waterUp = slowUp;
                }

                //Ensure that the only vertical input being used is from the up arrow
                if (verticalInput > 0)
                {
                    //Move player forward
                    rb.AddForce(transform.forward * waterSpeed * verticalInput);
                }

                //Disable self-created gravity
                useGravity = false;

                //If shift key is being pressed ...
                if (shiftKeyPressed())
                {
                    //Disable system gravity
                    useSystemGravity = false;
                    //useSystemGravity = true;

                    //Make player move up in water
                    rb.AddForce(transform.up * waterSpeed);
                }

                //If shift key is not being pressed ...
                else
                {
                    //Enable system gravity
                    useSystemGravity = true;
                }

            }

            //If player is not touching water ...
            else
            {
                //Make player not swim
                anim.SetBool("Swim", false);

                //Enable self-created gravity
                useGravity = true;

                //Disable system gravity
                useSystemGravity = false;
            }
        }
    }

    //Called for physics updates
    void FixedUpdate()
    {
        //If gravity is being used ... 
        if (useGravity)
        {
            //Disable system gravity
            rb.useGravity = false;

            //Create own gravity
            rb.AddForce(Vector3.down * 100);
        }

        //If system gravity is being used ...
        if (useSystemGravity)
        {
            //Enable system gravity
            rb.useGravity = true;
        }

        //If system gravity is not being used ...
        else
        {
            //Disable system gravity
            rb.useGravity = false;
        }
    }

    //Collision detection using tags
    void OnCollisionEnter(Collision other)
    {
        //If player is touching winter charm ...
        if (other.gameObject.tag == "Winter Collect")
        {
            //Deactivate the charm
            other.gameObject.SetActive(false);

            //Add to charm counter
            count.winterItem++;

            //Play sound effects
            collectSFX();

            //Add item was found in winter
            gameManager.winterArrList.Add("");

            //Start collection animation
            StartCoroutine(collectionAnimation());
        }

        //If player is touching spring charm ...
        if (other.gameObject.tag == "Spring Collect")
        {
            //Deactivate the charm
            other.gameObject.SetActive(false);

            //Add to charm counter
            count.springItem++;

            //Play sound effects
            collectSFX();

            //Add item was found in spring
            gameManager.springArrList.Add("");

            //Start collection animation
            StartCoroutine(collectionAnimation());
        }

        //If player is touching summer charm ...
        if (other.gameObject.tag == "Summer Collect")
        {
            //Deactivate the charm
            other.gameObject.SetActive(false);

            //Add to charm counter
            count.summerItem++;

            //Play sound effects
            collectSFX();

            //Add item was found in summer
            gameManager.summerArrList.Add("");

            //Start collection animation
            StartCoroutine(collectionAnimation());
        }

        //If player is touching autumn charm ...
        if (other.gameObject.tag == "Autumn Collect")
        {
            //Deactivate the charm
            other.gameObject.SetActive(false);

            //Add to charm counter
            count.autumnItem++;

            //Play sound effects
            collectSFX();

            //Add item was found in autumn
            gameManager.autumnArrList.Add("");

            //Start collection animation
            StartCoroutine(collectionAnimation());
        }

        //If player is touching pestle and mortar ...
        if (other.gameObject.tag == "Mix Collect")
        {
            //Deactivate the charm
            other.gameObject.SetActive(false);

            //Add to charm counter
            count.mixItem++;

            //Play sound effects
            collectSFX();

            //Add item was found in maze
            gameManager.mazeArrList.Add("");
            
            //Start collection animation
            StartCoroutine(collectionAnimation());
        }

        //If player is touching potion bottle ...
        if (other.gameObject.tag == "Bottle Collect")
        {
            //Deactivate the charm
            other.gameObject.SetActive(false);

            //Add to charm counter
            count.bottleItem++;

            //Play sound effects
            collectSFX();

            //Add item was found in maze
            gameManager.mazeArrList.Add("");

            //Start collection animation
            StartCoroutine(collectionAnimation());
        }

        //If player is touching water ball ...
        if (other.gameObject.tag == "WaterBall")
        {
            //Destroy the object
            Destroy(other.gameObject);

            //Add to counter
            count.waterball++;

            //Play sound effects
            collectSFX();
        }

        //If player is touching stick ...
        if (other.gameObject.tag == "Stick")
        {
            //Destroy the object
            Destroy(other.gameObject);

            //Add to counter
            count.stick++;

            //Play sound effects
            collectSFX();
        }

        //If player is touching herb ...
        if (other.gameObject.tag == "Herb")
        {
            //Destroy the object
            Destroy(other.gameObject);

            //Add to counter
            count.herb++;

            //Play sound effects
            collectSFX();
        }

        //If player is touching illusion flowers ...
        if (other.gameObject.tag == "Illusion Flowers")
        {
            //Player has touched the flowers
            touchedFlowers = true;

            //Disable colliders on all illusion flowers
            for (int i = 0; i < flowers.Length; i++)
            {
                flowers[i].GetComponent<Collider>().enabled = false;
            }
        }

        //If player is touching the winter to spring portal ...
        if (other.gameObject.tag == "Winter2Spring Portal")
        {
            //Teleports the player in front of the spring to winter portal
            StartCoroutine(Teleport("Winter2Spring"));
        }

        //If player is touching the winter to autumn portal ...
        else if (other.gameObject.tag == "Winter2Autumn Portal")
        {
            //Teleports the player in front of the autumn to winter portal
            StartCoroutine(Teleport("Winter2Autumn"));
        }

        //If player is touching the spring to winter portal ... 
        else if (other.gameObject.tag == "Spring2Winter Portal")
        {
            //Teleports the player in front of the winter to spring portal
            StartCoroutine(Teleport("Spring2Winter"));
        }

        //If player is touching the spring to summer portal ...
        else if (other.gameObject.tag == "Spring2Summer Portal")
        {
            //Teleports player in front of the summer to spring portal
            StartCoroutine(Teleport("Spring2Summer"));
        }

        //If player is touching the summer to spring portal ...
        else if (other.gameObject.tag == "Summer2Spring Portal")
        {
            //Teleports player in front of the spring to summer portal
            StartCoroutine(Teleport("Summer2Spring"));
        }

        //If player is touching the summer to autumn portal ...
        else if (other.gameObject.tag == "Summer2Autumn Portal")
        {
            //Teleports the player in front of the autumn to summmer portal
            StartCoroutine(Teleport("Summer2Autumn"));
        }

        //If player is touching the autumn to winter portal ... 
        else if (other.gameObject.tag == "Autumn2Winter Portal")
        {
            //Teleports the player in front of the winter to autumn portal
            StartCoroutine(Teleport("Autumn2Winter"));
        }

        //If player is touching autumn to summer portal ...
        else if (other.gameObject.tag == "Autumn2Summer Portal")
        {
            //Teleports the player in front of the summer to autumn portal
            StartCoroutine(Teleport("Autumn2Summer"));
        }

        //If player is touching the maze to winter portal ...
        else if (other.gameObject.tag == "Maze2Winter Portal")
        {
            //Teleports the player in front of the winter to maze portal
            StartCoroutine(Teleport("Maze2Winter"));
        }
        
        //If player is touching the winter to maze portal ...
        else if (other.gameObject.tag == "Winter2Maze Portal")
        {
            //Teleports the player in front of the maze to winter portal
            StartCoroutine(Teleport("Winter2Maze"));
        }
    }

    //Particle collision detection using tags
    void OnParticleCollision(GameObject other)
    {
        //If player is touching fire ...
        if (other.tag == "Fire")
        {
            count.firestick += count.stick;
            count.stick = 0;
            //Teleport player to random location
            StartCoroutine(Teleport("Random"));
        }

        //If player is touched by enemy's flames
        else if (other.tag == "Flame Attack")
        {
            //Disable the flames
            other.gameObject.SetActive(false);

            //Player is hurt
            damage();
        }
    }

    //Change slime's face
    void SetFace(Texture tex)
    {
        faceMaterial.SetTexture("_MainTex", tex);
    }

    //Plays the sound of the charms being collected
    void collectSFX()
    {
        GetComponent<AudioSource>().Play();
    }

    //Teleports the player in front of one of the portals
    private IEnumerator Teleport(string location)
    {
        Vector3[] pos = { new Vector3(505f, 144.5f, 85f), new Vector3(1065f, 144.5f, -452f), new Vector3(505f, 144.5f, -85f), new Vector3(1058f, 144.5f, 460f), new Vector3(932f, 144.5f, 463.1f), new Vector3(1426f, 144.5f, -70f), new Vector3(890f, 144.5f, -452.2f), new Vector3(1426f, 144.5f, 100f), new Vector3(135f, 2.861f, -805f), new Vector3(3527.3f, 147.5f, 2166.3f) };
        int num;

        //Assigns each string the corresponding element in pos
        switch(location)
        {
            case "Winter2Spring":
                num = 0;
                break;
            case "Winter2Autumn":
                num = 1;
                break;
            case "Spring2Winter":
                num = 2;
                break;
            case "Spring2Summer":
                num = 3;
                break;
            case "Summer2Spring":
                num = 4;
                break;
            case "Summer2Autumn":
                num = 5;
                break;
            case "Autumn2Winter":
                num = 6;
                break;
            case "Autumn2Summer":
                num = 7;
                break;
            case "Maze2Winter":
                num = 8;
                break;
            case "Winter2Maze":
                num = 9;
                break;
            default:
                num = getRandI(0, pos.Length-1);
                break;

        }

        //Disable player movements
        disabled = true;

        //Wait for 0.01 seconds
        yield return new WaitForSeconds(0.01f);

        //Teleports the player to the chosen position
        gameObject.transform.position = pos[num];

        //Wait for 0.01 seconds
        yield return new WaitForSeconds(0.01f);

        //Re-enable player movements
        disabled = false;
    }

    //Player celebrates finding a charm
    private IEnumerator collectionAnimation()
    {/*
        //Disable player movements
        disabled = true;

        //Player has to look at camera
        bool look = true;


        if (look)
        {
            //Make player look at camera
            transform.LookAt(Camera.main.transform);
        }

        /*float yRotation;
        if (Mathf.Abs(transform.rotation.y) == 180)
        {
            yRotation = 180;
        }
        else
        {
            yRotation = 360-Mathf.Abs(transform.rotation.y);
        }

        transform.Rotate(0, yRotation, 0);*/
        /*yield return new WaitForSeconds(0.1f);

        SetFace(faces.WalkFace);
        anim.SetTrigger("Jump");

        yield return new WaitForSeconds(1f);
        SetFace(faces.Idleface);
        disabled = false;

        //Player can face away from camera
        look = false;*/
        yield return new WaitForSeconds(0.0000000000000000000000000001f);
    }

    //Takes 1 heart off the HP
    public void damage()
    {
        //End the game (player loses)
        if (health <= 0)
        {
            //Game has ended
            count.gameOver = true;

            //Player did not win
            count.won = false;

            //Start end of game procedures
            count.GameEnd(count.timeRemaining);
        }

        else
        {

            //Decrease the health by 1
            health--;

            //Change the color of the specified heart to gray
            changeColor(hearts[heartIndex], "gray");

            //Decrease the heartIndex by 1
            heartIndex--;

            //End the game (player loses)
            if (health == 0)
            {
                //Game has ended
                count.gameOver = true;

                //Player did not win
                count.won = false;

                //Start end of game procedures
                count.GameEnd(count.timeRemaining);
            }
        }
    }

    //Adds 1 heart to the HP
    private void heal()
    {
        //Don't do anything because the player is already at max health
        if (health >= maxHealth)
        {
            Debug.Log("You're already fully healed!");
            return;
        }

        else
        {
            //Increase the health by 1
            health++;

            //Increase the healthIndex by 1
            heartIndex++;

            //Change the color of the specified heart to red
            changeColor(hearts[heartIndex], "red");

            
            if (health == maxHealth)
            {
                Debug.Log("You're at max health!");
            }
        }

    }

    //Changes the color of the heart
    private void changeColor(GameObject heart, string color)
    {
        //Get the sprite renderer
        SpriteRenderer sr = heart.GetComponent<SpriteRenderer>();

        //Create the color gray
        Color g = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        //Create the color red
        Color r = new Color(255f, 0f, 0f, 255f);

        switch (color)
        {
            case "gray":
            case "grey":
                //Change the color of the heart to gray
                sr.color = g;
                break;
            default:
                //Change the color of the heart to red
                sr.color = r;
                break;
        }
    }

    bool canSeeNPC(Transform t)
    {
        Vector3 direction = t.position - this.transform.position;
        float angle = Vector3.Angle(direction, this.transform.forward);

        return direction.magnitude < fovDist && angle <= fovAngle;
    }

    //Returns if the shift key is pressed
    private bool shiftKeyPressed()
    {
        return Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift);
    }

    //Returns if the string is one of the seasonal npcs
    private bool isAllyNPC(string str)
    {
        
        for (int i = 0; i < allyTags.Length; i++)
        {
            if (allyTags[i] == str)
                return true;
        }

        return false;
    }
    
    //Returns a Random Number as a float in the Range 
    float getRandF(float min, float max)
    {
        //Return a random float
        return Random.Range(min, max);
    }

    //Returns a Random Number as an int in the Range
    int getRandI(float min, float max)
    {
        //Return a random int
        return (int)getRandF(min, max);
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

    //Returns whether the slime is in water or not
    private bool isInWater()
    {
        //If player's y position is less than 140, slime is in water. Else, slime is not in water
        return slimePosition.y < 140;
    }

    //Returns whether is not position in in a square of coordinates
    private bool inSquare(float minX, float maxX, float minZ, float maxZ, Vector3 pos)
    {
        return inRange(minX, maxX, pos.x) && inRange(minZ, maxZ, pos.z);
    }

    //Return whether the number is within the minimum and maximum values
    private bool inRange(float min, float max, float num)
    {
        return num <= max && num >= min;
    }

    private void initialize()
    {
        //Sets player's position to start position
        transform.position = originalPosition;

        //Sets player's rotation to start position
        transform.rotation = originalRotation;

        //Allows the player to move
        disabled = false;

        //Player has not touched flowers
        touchedFlowers = false;

        //Player is not movng backwards
        backward = false;

        //Get all of the illusion flowers
        flowers = GameObject.FindGameObjectsWithTag("Illusion Flowers");

        //Enable all colliders of flowers
        for (int i = 0; i < flowers.Length; i++)
        {
            flowers[i].GetComponent<Collider>().enabled = true;
        }

        //Player is not in water in start position
        inWater = false;

        //Disable water look
        waterPanel.SetActive(false);

        //Enable normal camera
        cameraNormal.enabled = true;

        //Disable maze camera
        cameraMaze.enabled = false;

        initializeHPBar();

        initializeAllyTransform();
    }

    private void initializeHPBar()
    {
        //Set the maximum health to 5
        maxHealth = 5;

        //Start the player on maximum health
        health = maxHealth;

        //Set the heartIndex to the right number
        heartIndex = health - 1;

        //Create the HP bar
        Transform healthBar = Instantiate(HPBar, new Vector3(0, 1f), Quaternion.identity);

        //Make the HP bar the child of the player
        healthBar.SetParent(transform, false);

        //Get the HP Bar attatched to the player
        HPBarChild = this.gameObject.transform.GetChild(4).gameObject;

        //Get each inidividual heart and put it in the array
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i] = HPBarChild.gameObject.transform.GetChild(i).gameObject;
        }
    }

    private void initializeAllyTransform()
    {
        allyTags = new string[5]{ "Winter NPC", "Spring NPC", "Summer NPC", "Autumn NPC", "Maze NPC"};

        allyTransform = new Transform[5];

        for (int i = 0; i < 5; i++)
        {
            allyTransform[i] = GameObject.Find(allyTags[i]).GetComponent<Transform>();
        }
    }

    //Restarts the game/resets all values
    public void restart()
    {
        initialize();
    }
}