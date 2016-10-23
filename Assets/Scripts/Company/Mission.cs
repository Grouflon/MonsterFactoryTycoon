using UnityEngine;
using System.Collections;

public class Obstacle
{
    public string name;
    public int strength;
}

public class Mission
{
    public Mission()
    {
        obstacles = new Obstacle[3];

        for (int i = 0; i < 3; ++i)
        {
            obstacles[i] = new Obstacle();
            obstacles[i].strength = 10;

            obstacles[i].name = "Obstacle " + (i + 1);
        }

        reward = 1000;
        maxMonstersCount = 3;
    }

    public Obstacle[] obstacles;
    public int maxMonstersCount;
    public int reward;
}
