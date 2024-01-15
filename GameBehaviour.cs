using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBehaviour : MonoBehaviour
{
    //Declare variables

    //Holds the hidden item prefab transforms
    [SerializeField] Transform water, sticky, herby;

    //Initialize the y coorinates for the charms
    [SerializeField]
    float winterYCoor = 160.2f, springYCoor = 143.85f, summerYCoor = 144.77f, autumnYCoor = 143.82f, mortarYCoor = 147.7f, bottleYCoor = 147.5f;

    //Get the prefabs that will be used for charms
    [SerializeField]
    GameObject winter, spring, summer, autumn, mix, bottle;

    //Declare the arrays that will hold the charms to be distributed in each season
    [SerializeField]
    GameObject[] winterList, springList, summerList, autumnList, mazeList;

    //Holds all possible ranges for charms to be in each season.
    //Vector3's x coordinate holds the mininum vale for each range of coordinates.
    //Vector3's z coordinate holds the maximum value for each range of coordinates.
    Vector3[] winterPosX, winterPosZ;
    Vector3[] springPosX, springPosZ;
    Vector3[] summerPosX, summerPosZ;
    Vector3[] autumnPosX, autumnPosZ;
    Vector3[] mazePos;
    Vector3[] waterPosX, waterPosZ;
    Vector3[] stickPosX, stickPosZ;
    Vector3[] herbPosX, herbPosZ;

    //Holds all indicies for ranges that have already been chosen for each season
    ArrayList chosenWinter = new ArrayList();
    ArrayList chosenSpring = new ArrayList();
    ArrayList chosenSummer = new ArrayList();
    ArrayList chosenAutumn = new ArrayList();
    ArrayList chosenMaze = new ArrayList();

    //Holds the lengths for each seasonal list of prefabs
    int[] lengths = { 2, 2, 2, 2, 2};

    //Holds the number of objects that were found in each season
    public ArrayList winterArrList = new ArrayList();
    public ArrayList springArrList = new ArrayList();
    public ArrayList summerArrList = new ArrayList();
    public ArrayList autumnArrList = new ArrayList();
    public ArrayList mazeArrList = new ArrayList();

    //Shares the number of items that can be found in each season with other scripts/classes
    public int foundWinter = 2;
    public int foundSpring = 2;
    public int foundSummer = 2;
    public int foundAutumn = 2;
    public int foundMaze = 2;

    // Start is called before the first frame update
    void Start()
    {
        //Initialize the range of positions that each charm can be found in
        InstantiateXZPos();

        initialize();
    }

    //Update is called once per frame
    void Update()
    {
        //Update the number of items found in each season
        foundWinter = winterList.Length - winterArrList.Count;
        foundSpring = springList.Length - springArrList.Count;
        foundSummer = summerList.Length - summerArrList.Count;
        foundAutumn = autumnList.Length - autumnArrList.Count;
        foundMaze = mazeList.Length - mazeArrList.Count;
    }

    //Distributes Winter charms
    void Winter()
    {
        for (int i = 0; i < winterList.Length; i++)
        {
            //Chooses the random ranges where the charm to be placed
            int index = chooseIndex(winterPosX, chosenWinter);

            //Seperate the x and z coordinates
            Vector3 x = winterPosX[index];
            Vector3 z = winterPosZ[index];

            //Specify the x and z coordinates
            float xCoor = getRandF(x.x, x.z);
            float zCoor = getRandF(z.x, z.z);

            winterList[i].transform.position = new Vector3(xCoor, winterYCoor, zCoor);

            //Add the chosen index into the used coordinates arraylist
            chosenWinter.Add(index);
        }
    }

    //Distributes Spring charms
    void Spring()
    {
        for (int i = 0; i < springList.Length; i++)
        {
            //Chooses the random ranges where the charm to be placed
            int index = chooseIndex(springPosX, chosenSpring);

            //Seperate the x and z coordinates
            Vector3 x = springPosX[index];
            Vector3 z = springPosZ[index];

            //Specify the x and z coordinates
            float xCoor = getRandF(x.x, x.z);
            float zCoor = getRandF(z.x, z.z);

            springList[i].transform.position = new Vector3(xCoor, springYCoor, zCoor);

            //Add the chosen index into the used coordinates arraylist
            chosenSpring.Add(index);
        }
    }

    //Distributes Summer charms
    void Summer()
    {
        for (int i = 0; i < summerList.Length; i++)
        {
            //Chooses the random ranges where the charm to be placed
            int index = chooseIndex(summerPosX, chosenSummer);

            //Seperate the x and z coordinates
            Vector3 x = summerPosX[index];
            Vector3 z = summerPosZ[index];

            //Specify the x and z coordinates
            float xCoor = getRandF(x.x, x.z);
            float zCoor = getRandF(z.x, z.z);

            summerList[i].transform.position = new Vector3(xCoor, summerYCoor, zCoor);

            //Add the chosen index into the used coordinates arraylist
            chosenSummer.Add(index);
        }
    }

    //Distributes Autumn charms
    void Autumn()
    {
        for (int i = 0; i < autumnList.Length; i++)
        {
            //Chooses the random ranges where the charm to be placed
            int index = chooseIndex(autumnPosX, chosenAutumn);

            //Seperate the x and z coordinates
            Vector3 x = autumnPosX[index];
            Vector3 z = autumnPosZ[index];

            //Specify the x and z coordinates
            float xCoor = getRandF(x.x, x.z);
            float zCoor = getRandF(z.x, z.z);

            //Place the charm on the coordinates
            autumnList[i].transform.position = new Vector3(xCoor, autumnYCoor, zCoor);

            //Add the chosen index into the used coordinates arraylist
            chosenAutumn.Add(index);
        }
    }

    //Distributes Maze charms
    void Maze()
    {
        for (int i = 0; i < mazeList.Length; i++)
        {
            //Chooses the random ranges where the charm to be placed
            int index = chooseIndex(mazePos, chosenMaze);

            //Get the position coordinates
            Vector3 pos = mazePos[index];

            //Change the y coordinate the the correct height
            if (i == 0)
            {
                //Use the pestle and mortar's y coordinate
                pos.y = mortarYCoor;
            }

            else
            {
                //Use the bottle's y coordinate
                pos.y = bottleYCoor;
            }

            //Place the charm on the coordinates
            mazeList[i].transform.position = pos;

            //Add the chosen index into the used coordinates arraylist
            chosenMaze.Add(index);
        }
    }

    //Distributes waterballs
    void waterBall()
    {
        int max = getRandI(120, 150);

        for (int i = 0; i < max; i++)
        {
            //Chooses the random ranges where the charm to be placed
            int index = getRandI(0, 4);

            //Seperate the x and z coordinates
            Vector3 x = waterPosX[index];
            Vector3 z = waterPosZ[index];

            //Specify the x and z coordinates
            float xCoor = getRandF(x.x, x.z);
            float zCoor = getRandF(z.x, z.z);

            //Place the charm on the coordinates
            Transform waterball = Instantiate(water);
            waterball.position = new Vector3(xCoor, 2.5f, zCoor);
        }
    }

    //Distributes sticks
    void stick()
    {
        int max = getRandI(5, 7);

        for (int i = 0; i < max; i++)
        {
            //Chooses the random ranges where the charm to be placed
            int index = getRandI(0, 4);

            //Seperate the x and z coordinates
            Vector3 x = stickPosX[index];
            Vector3 z = stickPosZ[index];

            //Specify the x and z coordinates
            float xCoor = getRandF(x.x, x.z);
            float zCoor = getRandF(z.x, z.z);

            //Place the charm on the coordinates
            Transform stick = Instantiate(sticky);
            stick.position = new Vector3(xCoor, 2.5f, zCoor);
        }
    }

    //Distributes herbs
    void herb()
    {
        int max = getRandI(50, 60);

        for (int i = 0; i < max; i++)
        {
            //Chooses the random ranges where the charm to be placed
            int index = getRandI(0, 4);

            //Seperate the x and z coordinates
            Vector3 x = herbPosX[index];
            Vector3 z = herbPosZ[index];

            //Specify the x and z coordinates
            float xCoor = getRandF(x.x, x.z);
            float zCoor = getRandF(z.x, z.z);

            //Place the charm on the coordinates
            Transform herb = Instantiate(herby);
            herb.position = new Vector3(xCoor, 2.5f, zCoor);
        }
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
        return (int) getRandF(min, max);
    }

    //Chooses a random index that has not already been chosen
    int chooseIndex(Vector3[] arr, ArrayList arrList)
    {
        int rand = 0;

        //Get a new number if the old number has already been used
        do
        {
            rand = getRandI(0, arr.Length - 1);
        }
        while (inList(rand, arrList));

        //Return the new number that hasn't been used before
        return rand;
    }

    //Returns whether or not the number is in the arraylist
    bool inList(int num, ArrayList arr)
    {
        return arr.Contains(num);
    }

    //Instantiates all the possible x and z values for each charm to be distributed in each season
    void InstantiateXZPos()
    {
        XZWinter();
        XZSpring();
        XZSummer();
        XZAutumn();
        XZMaze();
        XZWater();
        XZStick();
        XZHerb();
    }

    //Instantiates all the possible x and z values for each charm to be distributed in winter
    void XZWinter()
    {
        int len = 12;
        winterPosX = new Vector3[len];
        winterPosZ = new Vector3[len];

        winterPosX[0] = new Vector3(385f, 0f, 755f);
        winterPosZ[0] = new Vector3(-745f, 0f, -700f);

        winterPosX[1] = new Vector3(405f, 0f, 460f);
        winterPosZ[1] = new Vector3(-645f, 0f, -515f);

        winterPosX[2] = new Vector3(220f, 0f, 460f);
        winterPosZ[2] = new Vector3(-500f, 0f, -475f);

        winterPosX[3] = new Vector3(230f, 0f, 290f);
        winterPosZ[3] = new Vector3(-615f, 0f, -505f);

        winterPosX[4] = new Vector3(660f, 0f, 810f);
        winterPosZ[4] = new Vector3(-610f, 0f, -550f);

        winterPosX[5] = new Vector3(620f, 0f, 635f);
        winterPosZ[5] = new Vector3(-595f, 0f, -475f);

        winterPosX[6] = new Vector3(510f, 0f, 625f);
        winterPosZ[6] = new Vector3(-460f, 0f, -250f);

        winterPosX[7] = new Vector3(750f, 0f, 790f);
        winterPosZ[7] = new Vector3(-360f, 0f, -205f);

        winterPosX[8] = new Vector3(585f, 0f, 730f);
        winterPosZ[8] = new Vector3(-220f, 0f, -190f);

        winterPosX[9] = new Vector3(210f, 0f, 410f);
        winterPosZ[9] = new Vector3(-225f, 0f, -175f);

        winterPosX[10] = new Vector3(200f, 0f, 415f);
        winterPosZ[10] = new Vector3(-365f, 0f, -360f);

        winterPosX[11] = new Vector3(185f, 0f, 220f);
        winterPosZ[11] = new Vector3(-350f, 0f, -240f);
    }

    //Instantiates all the possible x and z values for each charm to be distributed in spring
    void XZSpring()
    {
        int len = 6;
        springPosX = new Vector3[len];
        springPosZ = new Vector3[len];

        springPosX[0] = new Vector3(425f, 0f, 555f);
        springPosZ[0] = new Vector3(370f, 0f, 480f);

        springPosX[1] = new Vector3(745f, 0f, 825f);
        springPosZ[1] = new Vector3(405f, 0f, 545f);

        springPosX[2] = new Vector3(450f, 0f, 570f);
        springPosZ[2] = new Vector3(210f, 0f, 315f);

        springPosX[3] = new Vector3(750f, 0f, 830f);
        springPosZ[3] = new Vector3(675f, 0f, 720f);

        springPosX[4] = new Vector3(560f, 0f, 645f);
        springPosZ[4] = new Vector3(630f, 0f, 720f);

        springPosX[5] = new Vector3(325f, 0f, 385f);
        springPosZ[5] = new Vector3(570f, 0f, 680f);
    }

    //Instantiates all the possible x and z values for each charm to be distributed in summer
    void XZSummer()
    {
        int len = 4;
        summerPosX = new Vector3[len];
        summerPosZ = new Vector3[len];
        
        summerPosX[0] = new Vector3(1650f, 0f, 1780f);
        summerPosZ[0] = new Vector3(380f, 0f, 500f);

        summerPosX[1] = new Vector3(1165f, 0f, 1310f);
        summerPosZ[1] = new Vector3(390f, 0f, 450f);

        summerPosX[2] = new Vector3(1125f, 0f, 1295f);
        summerPosZ[2] = new Vector3(640f, 0f, 785f);

        summerPosX[3] = new Vector3(1325f, 0f, 1650f);
        summerPosZ[3] = new Vector3(415f, 0f, 800f);
    }

    //Instantiates all the possible x and z values for each charm to be distributed in autumn
    void XZAutumn()
    {
        int len = 4;
        autumnPosX = new Vector3[len];
        autumnPosZ = new Vector3[len];

        autumnPosX[0] = new Vector3(1255f, 0f, 1355f);
        autumnPosZ[0] = new Vector3(-745f, 0f, -255f);

        autumnPosX[1] = new Vector3(1605f, 0f, 1680f);
        autumnPosZ[1] = new Vector3(-745f, 0f, -255f);

        autumnPosX[2] = new Vector3(1360f, 0f, 1600f);
        autumnPosZ[2] = new Vector3(-325f, 0f, -230f);

        autumnPosX[3] = new Vector3(1230f, 0f, 1600f);
        autumnPosZ[3] = new Vector3(-725f, 0f, -620f);
    }

    //Instantiates all the possible x and z values for each charm to be distributed in the maze
    void XZMaze()
    {
        int len = 12;
        mazePos = new Vector3[len];

        //(1, 17)
        mazePos[0] = new Vector3(3945f, 0f, 2170f);

        //(1, 19)
        mazePos[1] = new Vector3(4000f, 0f, 2170f);

        //(3, 13)
        mazePos[2] = new Vector3(3840f, 0f, 2120f);

        //(5, 9)
        mazePos[3] = new Vector3(3735f, 0f, 2065f);
        
        //(7, 3)
        mazePos[4] = new Vector3(3580f, 0f, 2015f);
        
        //(9, 9)
        mazePos[5] = new Vector3(3735f, 0f, 1960f);
        
        //(11, 1)
        mazePos[6] = new Vector3(3525f, 0f, 1910f);
        
        //(11, 15)
        mazePos[7] = new Vector3(3895f, 0f, 1910f);
        
        //(13, 11)
        mazePos[8] = new Vector3(3790f, 0f, 1855f);
        
        //(15, 7)
        mazePos[9] = new Vector3(3685f, 0f, 1805f);
        
        //(17, 3)
        mazePos[10] = new Vector3(3575f, 0f, 1750f);
        
        //(17, 17)
        mazePos[11] = new Vector3(3945f, 0f, 1750f);
    }

    void XZWater()
    {
        int len = 4;
        waterPosX = new Vector3[len];
        waterPosZ = new Vector3[len];

        waterPosX[0] = new Vector3(1043.77f, 0f, 1922f);
        waterPosZ[0] = new Vector3(-856f, 0f, -906.9869f);

        waterPosX[1] = new Vector3(1037f, 0f, 1896f);
        waterPosZ[1] = new Vector3(-33f, 0f, -83f);

        waterPosX[2] = new Vector3(1033f, 0f, 1074f);
        waterPosZ[2] = new Vector3(-51f, 0f, -890f);

        waterPosX[3] = new Vector3(1859f, 0f, 1914f);
        waterPosZ[3] = new Vector3(-33f, 0f, -912f);
    }

    void XZStick()
    {
        int len = 4;
        stickPosX = new Vector3[len];
        stickPosZ = new Vector3[len];

        stickPosX[0] = new Vector3(1880.4f, 0f, 1924.4f);
        stickPosZ[0] = new Vector3(55.4574f, 0f, 869f);

        stickPosX[1] = new Vector3(1031f, 0f, 1075.4f);
        stickPosZ[1] = new Vector3(67.4f, 0f, 869f);

        stickPosX[2] = new Vector3(1049.3f, 0f, 1909f);
        stickPosZ[2] = new Vector3(61f, 0f, 111.5f);

        stickPosX[3] = new Vector3(1041.6f, 0f, 1847.3f);
        stickPosZ[3] = new Vector3(874.3f, 0f, 914.2f);
    }

    void XZHerb()
    {
        int len = 4;
        herbPosX = new Vector3[len];
        herbPosZ = new Vector3[len];

        herbPosX[0] = new Vector3(907f, 0f, 952f);
        herbPosZ[0] = new Vector3(50.34634f, 0f, 889f);

        herbPosX[1] = new Vector3(73f, 0f, 120.2f);
        herbPosZ[1] = new Vector3(60f, 0f, 906.5f);

        herbPosX[2] = new Vector3(71.2f, 0f, 940f);
        herbPosZ[2] = new Vector3(42f, 0f, 96.6f);

        herbPosX[3] = new Vector3(95f, 0f, 956.9f);
        herbPosZ[3] = new Vector3(847.6f, 0f, 886f);
    }

    private void initialize()
    {
        
        //Initialize the prefab lists
        winterList = new GameObject[2];
        springList = new GameObject[2];
        summerList = new GameObject[2];
        autumnList = new GameObject[2];
        mazeList = new GameObject[2];

        //Instantiate winter charms and add them to winterList
        winterList[0] = Instantiate(winter);
        winterList[1] = Instantiate(winter);

        //Instantiate spring charms and add them to springList
        springList[0] = Instantiate(spring);
        springList[1] = Instantiate(spring);

        //Instantiate summer charms and add them to summerList
        summerList[0] = Instantiate(summer);
        summerList[1] = Instantiate(summer);

        //Instantiate autumn charms and add them to autumnList
        autumnList[0] = Instantiate(autumn);
        autumnList[1] = Instantiate(autumn);

        //Instantiate pestle & mortar and bottle charms and add them mazeList
        mazeList[0] = Instantiate(mix);
        mazeList[1] = Instantiate(bottle);
        

        //Randomly distribute the charms within their seasons
        Winter();
        Spring();
        Summer();
        Autumn();
        Maze();
        waterBall();
        stick();
        herb();
    }
}