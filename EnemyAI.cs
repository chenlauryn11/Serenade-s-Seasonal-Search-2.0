using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    //Holds the flame prefab used to attack
    [SerializeField] Transform flames;

    //Holds the instantiated flame prefab
    [SerializeField] Transform flameAttack;

    //Holds when the player was last seen
    [SerializeField] float lastSeenPlayer;

    //Holds the previous state
    [SerializeField] string prevState;

    //Holds the current state
    [SerializeField] string currState;

    //Holds whether the enemy is attacking or not
    [SerializeField] bool attacking;

    //Holds whether the enemy has reached its destination
    [SerializeField] bool destReached;

    //Holds the ranges of x and z coordinates that the cells can be chosen from
    [SerializeField] int[] xRanges, zRanges;

    //Holds where the enemy's destination is (for wander only)
    [SerializeField] Vector3 pos;

    //Holds the player's transform (for the position)
    Transform playerPos;

    //Holds the agent that will be maneuvered
    NavMeshAgent agent;

    //Holds the script of the player
    MoveCharacter player;

    //Holds the coordinates for the centers of the maze cells
    Vector3[,] centers = new Vector3[10,10];

    //Holds the distance of the raycast to see player
    private float fovDist = 100f;

    //Holds the angle of the raycast to see player
    private float fovAngle = 90f;

    //Holds the distance of the raycast to attack
    private float attackDist = 50f;

    //Holds the angle of the raycast to attack
    private float attackAngle = 45f;

    //Holds the position of the enemy AI
    public Vector3 enemyPosition;

    //Holds the current health and the maximum health
    public int health, maxHealth;

    //Holds which index in the hearts array
    public int heartIndex;

    //Holds the HPBar prefab
    public Transform HPBar;

    //The HP Bar that is the child of the player
    GameObject HPBarChild;

    //Holds the individual hearts in the HP bar
    GameObject[] hearts = new GameObject[3];

    //Initialize animation
    Animation anim;

    //Holds the strings to call the enemy animations
    public const string IDLE = "Anim_Idle";
    public const string RUN = "Anim_Run";
    public const string ATTACK = "Anim_Attack";
    public const string DAMAGE = "Anim_Damage";
    public const string DEATH = "Anim_Death";

    // Start is called before the first frame update
    void Start()
    {
        //Get the animation 
        anim = GetComponent<Animation>();

        //Get the NavMeshAgent
        agent = GetComponent<NavMeshAgent>();

        //Get the player script
        player = GameObject.Find("Player").GetComponent<MoveCharacter>();

        //Get the player's transform
        playerPos = GameObject.Find("Player").GetComponent<Transform>();

        //Holds the position of the player
        enemyPosition = transform.position;

        //At first, destination has not been reached
        destReached = false;

        //Enemy is not attacking
        attacking = false;

        //Initialize pos
        pos = getRandCenter();

        //Initialize the enemy's HP bar
        initializeHPBar();

        //Initialize the centers of the maze cells
        initializeCenters();

        initializeFlames();
    }

    // Update is called once per frame
    void Update()
    {
        //If enemy doesn't have any life left ...
        if (health == 0)
        {
            //Enemy will die
            die();
            return;
        }

        //Make enemyPosition the same as the enemy's position
        enemyPosition = transform.position;

        //Holds whether the enemy's destination has been reached yet
        destReached = reachedDest();

        //Add the time to lastSeenPlayer
        lastSeenPlayer += Time.deltaTime;
        
        //Set the previous state to be the current state
        prevState = currState;

        //Get the next state
        currState = getNextState();

        //Switch the enemy's state
        switch (currState)
        {
            //If currState is idle ...
            case "idle":

                //Enemy will idle
                idle();

                //Exit case
                break;

            //If currState is wander ...
            case "wander":

                //Enemy will wander/patrol
                wander();

                //Exit case
                break;

            //If currState is chase
            case "chase":

                //Enemy will chase
                chase();

                //Exit case
                break;

            //If currState is attack
            case "attack":

                //Enemy will attack player
                attack();

                //Exit case
                break;

            //If currState is flee
            case "flee":

                //Enemy will run away
                flee();

                //Exit case
                break;
        }


    }

    //Returns the enemy's next state as a string
    string getNextState()
    {
        //Get a random number from 1-100
        float rand = getProb(); ;

        //If enemy can see the player ... 
        if (canSee(playerPos))
        {
            //Player has last been seen 0 seconds ago
            lastSeenPlayer = 0;

            //If player is in attack range ...
            if (canAttack(playerPos))
            {
                //If the previous state was running away ...
                if (prevState == "flee")
                {
                    //If the player's health is low ...
                    if (playerHealthLow())
                    {
                        //50% chance to attack
                        if (rand < 50)
                        {
                            return "attack";
                        }

                        //50% chance to run away
                        return "flee";
                    }

                    //If player's health is not low ...
                    else
                    {
                        //10% chance to attack
                        if (rand < 10)
                        {
                            return "attack";
                        }

                        //90% chance to run away
                        return "flee";
                    }
                }

                //If previous state was not running away ... attack
                return "attack";
            }

            //If player is not in attack range ... 
            else
            {
                //If the previous state was running away ...
                if (prevState == "flee")
                {
                    //If the player's health is low ...
                    if (playerHealthLow())
                    {
                        //70% chance to chase
                        if (rand < 70)
                        {
                            return "chase";
                        }

                        //30% chance to run away
                        return "flee";
                    }

                    //If player's health is not low ...
                    else
                    {
                        //20% chance to chase
                        if (rand < 20)
                        {
                            return "chase";
                        }

                        //80% chance to run away
                        return "flee";
                    }
                }

                //If previous state was not running away ... chase
                return "chase";
            }
            
        }

        //If the enemy is bored ... idle
        if (isBored(rand))
        {
            return "idle";
        }

        //If the enemy's health is low ...
        if (enemyHealthLow())
        {
            //90% chance of running away
            if (rand <= 90)
            {
                return "flee";
            }
        }

        //If none of the above conditions are met ... patrol
        return "wander";
    }

    //Idling because enemy is bored
    void idle()
    {
        //Enemy stops moving around
        agent.isStopped = true;

        //Enemy freezes
        anim.Stop();
    }
    
    //Patrols/wanders around the maze
    void wander()
    {
        //Make the animation the idle animation
        anim.CrossFade(IDLE);

        //If destination has been reached or pos has not been initialized ...
        if (destReached || noPos())
        {
            //Get a random maze cell center
            pos = getRandCenter();
        }

        //Make speed 20
        agent.speed = 20f;

        //Make agent move
        agent.isStopped = false;

        //Set the agent's destination to the chosen vector3
        agent.destination = pos;
    }
    
    //Chases the player
    void chase()
    {
        //Make animation the running animation
        anim.CrossFade(RUN);

        //Make speed 50
        agent.speed = 50f;

        //Make agent move
        agent.isStopped = false;

        //Set pos to player's position
        pos = playerPos.position;

        //Set the agent's destination to the player's position through pos
        agent.destination = pos;
    }
    
    //Attacks the player
    void attack()
    {
        //Make the animation the attacking animation
        //anim.CrossFade(RUN);
        anim.Stop();

        //Make speed 50
        agent.speed = 50f;

        //Make agent move
        agent.isStopped = false;

        //Set pos to player's position
        pos = playerPos.position;

        //Set the agent's destination to the player's position through pos
        //agent.destination = pos;

        //If the enemy is not already attacking ...
        if (!attacking)
        {
            //Enemy is attacking
            attacking = true;

            //Attack the player
            StartCoroutine(showFlames());
        }
    }
    
    //Runs away from the player
    void flee()
    {
        //Make animation run away
        anim.CrossFade(RUN);

        //Find opposite direction of player
        Vector3 direction = enemyPosition - playerPos.position;

        //Choose a random cell to run away to
        runAway(direction);
    }

    //Enemy dies
    void die()
    {
        //Make the animation the death animation
        anim.CrossFade(DEATH);

        //Wait some time before dying
        StartCoroutine(enemyDie(.75f));
    }

    //Chooses the place for enemy to run away to
    void runAway(Vector3 direction)
    {
        //If the x coor of direction is negative ...
        if (direction.x < 0)
        {
            //Find a cell that is to the left of the enemy's current position
            xRanges = findCenter("x", "more");
        }

        //If the x coor of direction is positive ...
        else
        {
            //Find a cell that is to the right of the enemy's current position
            xRanges = findCenter("x", "less");
        }

        //If the z coor of direction is negative ...
        if (direction.z < 0)
        {
            //Find a cell that is "below" the enemy's current position (when looking from a bird's eye-view)
            zRanges = findCenter("z", "less");
        }

        //If the z coord of direction is positive ...
        else
        {
            //Find a cell that is "above" the enemy's current position (when looking from a bird's eye-view)
            zRanges = findCenter("z", "more");
        }
        
        //Get a random index between the two ranges
        int r = getRandI(xRanges[0], xRanges[1]);

        //Get a random index between the two ranges
        int c = getRandI(zRanges[0], zRanges[1]);

        //If the destination has been reached or pos has not been instantiated ...
        if (destReached || noPos())
        {
            //Set pos to a randomly chosen cell
            pos = centers[r, c];
        }
        
        //Set the agent's destination to pos
        agent.destination = pos;
    }

    //Take 1 heart off of the enemy's HP
    public void damage()
    {
        //Make the animation the damage animation
        anim.CrossFade(DAMAGE);

        //Enemy is dead
        if (health <= 0)
        {
            die();
        }

        else
        {
            //Decrease the health by 1
            health--;

            //Change the color of the specified heart to gray
            changeColor(hearts[heartIndex], "gray");

            //Decrease the heartIndex by 1
            heartIndex--;

            //Enemy is dead
            if (health == 0)
            {
                die();
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

    private IEnumerator enemyDie(float delay)
    {
        //Wait some amount of time
        yield return new WaitForSeconds(delay);

        //Destroy the enemy
        Destroy(gameObject);
    }

    //Create the flames that attack the player
    private IEnumerator showFlames()
    {
        //Enable the flames
        flameAttack.gameObject.SetActive(true);
        
        //Wait for 2 seconds
        yield return new WaitForSeconds(2f);

        //Disable the flames
        flameAttack.gameObject.SetActive(false);

        //Wait some time before attacking again
        StartCoroutine(attackingInterval(1f));
    }

    //Waits a certain amount of time before attacking again
    private IEnumerator attackingInterval(float delay)
    {
        //Wait for 1 second
        yield return new WaitForSeconds(delay);

        //Enemy is no longer attacking
        attacking = false;
    }

    private int[] findCenter(string coor, string op)
    {
        //Holds the result that is to be returned
        int[] result = new int[2];

        //Assign all values of result to -1
        result[0] = -1;
        result[1] = -1;

        //If the coordinate to be found is the x coor ...
        if (coor == "x")
        {
            //If the x coor of direction is negative ...
            if (op == "less")
            {
                //The lower bound of the ranges is 0
                result[0] = 0;

                //Find the coordinate that is the biggest of all of those less than the enemy's position, starting from the biggest coordinates
                for (int i = 9; i >= 0; i--)
                {
                    //If such a coordinate is found ...
                    if (centers[0, i].x < enemyPosition.x)
                    {
                        //Assign the upper bound to that coordinate
                        result[1] = i;

                        //Exit the loop
                        break;
                    }
                }

                //If no such coordinate is found ...
                if (result[1] < 0)
                {
                    //Assign the upper bound to the smallest possible bound
                    result[1] = 0;
                }
                
            }

            //If the x coor of direction is positive ...
            else
            {
                //The upper bound of the ranges is 9
                result[1] = 9;

                //Find the coordinate that is the smallest of all of those greater than the enemy's position, starting from the smallest coordinates 
                for (int i = 0; i < 10; i++)
                {
                    //If such a coordinate is found ...
                    if (centers[0, i].x > enemyPosition.x)
                    {
                        //Assign the lower bound to that coordinate
                        result[0] = i;

                        //Exit the loop
                        break;
                    }

                    //If no such coordinate is found ...
                    if (result[0] < 0)
                    {
                        //Assign the lower bound to the greatest possible bound
                        result[0] = 9;
                    }
                }
            }
        }

        //If the coordinate to be found is the z coor ...
        else
        {
            //If the z coor of direction is negative ...
            if (op == "less")
            {
                //The lower bound of the ranges is 0
                result[0] = 0;

                //Find the coordinate that is the biggest of all of those less than the enemy's position, starting from the biggest coordinates
                for (int i = 9; i >= 0; i--)
                {
                    //If such a coordinate is found ...
                    if (centers[i, 0].z < enemyPosition.z)
                    {
                        //Assign the upper bound to that coordinate
                        result[1] = i;

                        //Exit the loop
                        break;
                    }
                }

                //If no such coordinate is found ...
                if (result[1] < 0)
                {
                    //Assign the upper bound to the smallest possible bound
                    result[1] = 0;
                }
            }

            //If the z coor of direction is positive ...
            else
            {
                //The upper bound of the ranges is 9
                result[1] = 9;

                //Find the coordinate that is the smallest of all those greater than the enemy's position, starting from the smallest coordinates
                for (int i = 0; i < 10; i++)
                {
                    //If such a coordinate is found ...
                    if (centers[i, 0].z > enemyPosition.z)
                    {
                        //Assign the lower bound to that coordinate
                        result[0] = i;

                        //Exit the loop
                        break;
                    }

                    //If no such coordinate is found ...
                    if (result[0] < 0)
                    {
                        //Assign the lower bound to the greatest possible bound
                        result[0] = 9;
                    }
                }
            }
        }

        return result;
    }

    private Vector3 getRandCenter()
    {
        int r = getRandI(0, 9);
        int c = getRandI(0, 9);

        return centers[r, c];
    }

    //Returns if the enemy is bored
    bool isBored(float rand)
    {
        //If the previous state is idle ...
        if (prevState == "idle")
        {
            //90% chance of being bored; 10% chance not being bored
            return rand <= 90;
        }

        //If the previous state is wander ...
        else if (prevState == "wander")
        {
            //If last saw player [60, 90) seconds ago ...
            if (inRange1(lastSeenPlayer, 60, 90))
            {
                //30% chance of being bored; 70% chance of not being bored
                return rand <= 30;
            }

            //If last saw player [90, 120) seconds ago ...
            else if (inRange1(lastSeenPlayer, 90, 120))
            {
                //60% chance of being bored; 40% chance of not being bored
                return rand <= 60;
            }

            //If last saw player [120, infinite) seconds ago ...
            else if (lastSeenPlayer >= 120)
            {
                //90% chance of being bored; 10% chance of being bored
                return rand <= 90;
            }
        }

        //Enemy is not bored
        return false;
    }

    bool canSee(Transform playerPos)
    {
        Vector3 direction = playerPos.position - this.transform.position;
        float angle = Vector3.Angle(direction, this.transform.forward);
        RaycastHit hit;

        //If the ray hits the player and the distance and angle are in the restrictions ...
        if (Physics.Raycast(this.transform.position, direction, out hit) && hit.collider.gameObject.tag == "Player" && direction.magnitude < fovDist && angle < fovAngle)
        {
            //Enemy has seen the player
            return true;
        }

        else
        {
            //Enemy has not seen the player
            return false;
        }
    }

    bool canAttack(Transform playerPos)
    {
        Vector3 direction = playerPos.position - this.transform.position;
        float angle = Vector3.Angle(direction, this.transform.forward);
        RaycastHit hit;

        //If the ray hits the player and the distance and angle are in the restrictions ...
        if (Physics.Raycast(this.transform.position, direction, out hit) && hit.collider.gameObject.tag == "Player" && direction.magnitude < attackDist && angle < attackDist)
        {

            //Enemy has seen the player
            return true;
        }

        else
        {
            //Enemy has not seen the player
            return false;

        }
    }

    //Returns if the player's health is low
    bool playerHealthLow()
    {
        return player.health <= 2;
    }

    //Returns is the enemy's health is low
    bool enemyHealthLow()
    {
        return health <= 1;
    }

    //Returns if pos has been initialized yet
    bool noPos()
    {
        return pos.x == 0 && pos.y == 0 && pos.z == 0;
    }

    //Returns if the enemy's destination has been reached yet
    bool reachedDest()
    {
        return enemyPosition.x == pos.x && enemyPosition.z == pos.z;
    }

    //Returns if the number is in the range of [min, max]
    bool inRange(float num, float min, float max)
    {
        return num >= min && num <= max;
    }

    //Returns if the number is in the range of [min, max)
    bool inRange1(float num, float min, float max)
    {
        return num >= min && num < max;
    }

    //Returns if the number is in the range of (min, max]
    bool inRange2(float num, float min, float max)
    {
        return num > min && num <= max;
    }

    //Gets a random number from 0 to 100
    float getProb()
    {
        return getRandF(0f, 100f);
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

    void initializeHPBar()
    {
        //Set the maximum health to 3
        maxHealth = 3;

        //Start the enemy on maximum health
        health = maxHealth;

        //Set the heartIndex to the right number
        heartIndex = health - 1;

        //Create the HP bar
        Transform healthBar = Instantiate(HPBar, new Vector3(0, 1.5f), Quaternion.identity);

        //Make the HP bar the child of the enemy
        healthBar.SetParent(transform, false);

        //Get the HP Bar attatched to the enemy
        HPBarChild = this.gameObject.transform.GetChild(2).gameObject;

        //Get each inidividual heart and put it in the array
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i] = HPBarChild.gameObject.transform.GetChild(i).gameObject;
        }
    }

    //Initializes the coordinates to the centers of all of the maze cells
    void initializeCenters()
    {
        //Holds the starting x coordinate
        float startX = 3525f;

        //Holds the x coordinate that is used to loop through the array
        float x = 3525f;

        //Holds the y coordinate that never changes
        float y = 162.5833f;

        //Holds the z coordinate that is used to loop through the array
        float z = 2170f;

        //Loop through all rows and columns in the maze
        for (int r = 0; r < 10; r++)
        {
            for (int c = 0; c < 10; c++)
            {
                //Initialize the center of the maze using the x, y, and z coordinates
                centers[r, c] = new Vector3(x, y, z);

                //Add 2 * (half of the wall's scale + half of the corner wall's scale)
                x += (2 * 26.25f);
            }

            //Loop x back to its starting position
            x = startX;

            //Subtract 2 * (half of the wall's scale + half of the corner wall's scale)
            z -= (2 * 26.25f);
        }
    }

    void initializeFlames()
    {
        //Instantiates the flames
        flameAttack = Instantiate(flames, new Vector3(0.045f, 0.33f), Quaternion.identity);

        //Make the HP bar the child of the enemy
        flameAttack.SetParent(transform, false);

        //Disable flame attack
        flameAttack.gameObject.SetActive(false);
    }
}