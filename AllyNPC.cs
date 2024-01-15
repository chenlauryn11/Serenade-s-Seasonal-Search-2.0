using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyNPC : MonoBehaviour
{
    //The player
    [SerializeField] GameObject playerObj;

    //Initializes GameBehaviour
    [SerializeField] GameBehaviour gameManager;

    //Initializes the timer and counter
    [SerializeField] Counter_Timer count;

    //Initializes MoveCharacter
    [SerializeField] MoveCharacter player;
    
    //[SerializeField] 

    public int typingIndex = 0;

    int maxIndex;

    string[] lines;

    public string seasonSlime;

    public bool interacting = false;

    //Holds whether or not the player has interacted with the seasonal slime yet
    [SerializeField] bool vWinter, vSpring, vSummer, vAutumn, vMaze;

    //Holds whether all seasonal charms have been found yet
    [SerializeField] bool foundSeasonal;

    // Start is called before the first frame update
    void Start()
    {
        //seasonSlime = gameObject.tag;
        foundSeasonal = false;
        maxIndex = 0;

        GameObject.Find("Maze Portal Barrier").GetComponent<Collider>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Look at player
        //transform.LookAt(playerObj.transform);

        foundSeasonal = gameManager.foundWinter == 2 && gameManager.foundSpring == 2 && gameManager.foundSummer == 2 && gameManager.foundAutumn == 2;

        if (foundSeasonal)
        {
            GameObject.Find("Maze Portal Barrier").GetComponent<Collider>().enabled = false;
        }

        if (interacting && typingIndex < maxIndex)
        {
            count.typeText(lines[typingIndex]);

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                typingIndex++;
            }
        }

        else
        {
            interacting = false;

            //Enable player controls
            player.disabled = false;
        }
    }

    //Interaction with the player
    public void interact(int index)
    {
        //NPC is interacting
        interacting = true;

        //Start interacting
        npcInteract(index);
    }

    private void npcInteract(int index)
    {
        //Disable player controls
        player.disabled = true;

        //Get the lines that the npc will say
        lines = getLines(index);

        maxIndex = lines.Length;

        typingIndex = 0;
    }

    

    private string[] getLines(int index)
    {
        //First time meeting lines
        string[] meetWinter = new string[8] {"Hi! I'm Serenade's friend, Aurora of Winter!\nTo start off, press the up arrow to move forward.\n\n(Press enter to continue)", "The left arrow turns left and the right arrow turns right.\nPress space to jump.", "Also, if you decide to go swimming, press shift to move up in the water.", "There are portals on the mountain sides that can lead to the other seasons.", "You must find all of the charms before Roxie destroys them all!\nThe timer below the charm counter tells you how much time you have left.", "The charm you can find in winter is the Snowflake of the Wind.\nThis white snowflake is a crystallization of winter's joy.\nIt can summon winter's chilling arctic wind or a refreshing snowy breeze.\nA total of two can be found in the winter season!", "Unfortunately, I can only tell you about winter's charms and dangers.\nBut if you find our other friends, I'm sure they can tell you more about their respective seasons!", "Good luck on your journey!"};
        string[] meetSpring = new string[4] {"It's nice to meet you! I'm Kalina of Spring!", "The spring charm is the Flower of the Earth.\nThis purple flower grew from spring's hope.\nIt can summon stunning flora and precious fauna that can heal any wounds.\n", "The flowers growing in Spring are special illusion flowers.\nIf you touch them, you will pass through them and be enthralled in their illusion.\nSo you may mend up mixing up your directions while in Spring.", "Be careful!" };
        string[] meetSummer = new string[4] {"Celestia of Summer, it's a pleasure to meet you.", "The Crystal of Fire can be found in Summer.\nIt's an orange crystal that holds the power of the sun.\nIt can summon a gentle heat or a scorching flame.", "While you're looking for these two crystals, beware of summer's fire. It can teleport you to any random season.", "However, summer's fire is great since it can warm you up from winter's chilly waters." };
        string[] meetAutumn = new string[2] {"Hey, Serenade! It's your friend, Luna of Autumn!", "Autumn's charm is the Enchanted Fruit of Autumn, a red fruit filled with autumn's serenity.\nIt can summon foods-a-plenty for autumn's unique feasts and is often found on the floor under the trees it grows from.\nThere's two of these fruits to be found!" };
        string[] meetMaze = new string[6] {"I see you have all of the seasonal charms ... I normally wouldn't let you pass, but I guess I'll have to.", "But, please be careful!\nRoxie is unforgiving to those who dare to enter her lair.", "It's crawling with her minions.\nOnly enchanted water from autumn's rivers can defeat them./nClick the mouse where you want to shoot them.\nThey will appear above the place you shot at.", "Her minions can also shoot fire. You can heal yourself using the enchanted herbs from spring by pressing [r].", "I saw Roxie's minions bring your pestle and mortar as well as your special potion bottle in.\nYour pestle and mortar is the only tool capable of mixing a potion that can reunite the seasons,\nand your potion bottle is special object that can store the most potent of potions and magics, from revival potions to moonlight magic.", "There's only one of each object, so they're very valuable!\nTry to find them, but be careful!" };

        //Holds the lines to not allow player into maze
        string[] earlyMaze = new string[4] {"Serenade! What are you doing here?!", "You know it's dangerous to be around Roxie's lair!", "I can't let you pass.\nNot until you've proven that you won't be going risking your life in vain.", "I guess you can try gathering all of the seasonal charms to prove your strength."};

        //Holds the hint to look into the water
        string[] waterHint = new string[1] {"There may be something of interest if you dive in the water ... "};

        //How many items are to be found in each season
        string[] countWinter = new string[1];
        string[] countSpring = new string[1];
        string[] countSummer = new string[1];
        string[] countAutumn = new string[1];

        if (gameManager.foundWinter == 0)
        {
            countWinter[0] = string.Format("You have found all items in Winter!");
        }

        else if (gameManager.foundWinter == 1)
        {
            countWinter[0] = string.Format("You have {0:0} item left to find in Winter!", gameManager.foundWinter);
        }

        else
        {
            countWinter[0] = string.Format("You have {0:0} items left to find in Winter!", gameManager.foundWinter);
        }

        
        if (gameManager.foundSpring == 0)
        {
            countSpring[0] = string.Format("You have found all items in Spring!");
        }

        else if (gameManager.foundSpring == 1)
        {
            countSpring[0] = string.Format("You have {0:0} item left to find in Spring!", gameManager.foundSpring);
        }

        else
        {
            countSpring[0] = string.Format("You have {0:0} items left to find in Spring!", gameManager.foundSpring);
        }

        if (gameManager.foundSummer == 0)
        {
            countSummer[0] = string.Format("You have found all items in Summer!");
        }

        else if (gameManager.foundSummer == 1)
        {
            countSummer[0] = string.Format("You have {0:0} item left to find in Summer!", gameManager.foundSummer);
        }

        else
        {
            countSummer[0] = string.Format("You have {0:0} items left to find in Summer!", gameManager.foundSummer);
        }


        if (gameManager.foundAutumn == 0)
        {
            countAutumn[0] = string.Format("You have found all items in Autumn!");
        }

        else if (gameManager.foundAutumn == 1)
        {
            countAutumn[0] = string.Format("You have {0:0} item left to find in Autumn!", gameManager.foundAutumn);
        }

        else
        {
            countAutumn[0] = string.Format("You have {0:0} items left to find in Autumn!", gameManager.foundAutumn);
        }



        //Winter NPC
        if (index == 0)
        {
            if (!vWinter)
            {
                vWinter = true;
                return meetWinter;
            }

            else if (!foundSeasonal)
                return countWinter;
                
        }

        //Spring NPC
        else if (index == 1)
        {
            if (!vSpring)
            {
                vSpring = true;
                return meetSpring;
            }

            else if (!foundSeasonal)
                return countSpring;
        }

        //Summer NPC
        else if (index == 2)
        {
            if (!vSummer)
            {
                vSummer = true;
                return meetSummer;
            }

            else if (!foundSeasonal)
                return countSummer;
        }
        
        //Autumn NPC
        else if (index == 3)
        {
            if (!vAutumn)
            {
                vAutumn = true;
                return meetAutumn;
            }

            else if (!foundSeasonal)
                return countAutumn;
        }

        //MazeNPC
        else
        {
            if (!foundSeasonal)
            {
                return earlyMaze;
            }

            else if (!vMaze)
            {
                return meetMaze;
            }
        }

        return waterHint;
    }
}
