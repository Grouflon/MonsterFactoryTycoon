using UnityEngine;
using System.Collections;

public class Balance : MonoBehaviour
{
    [Header("Time")]
    public float cycleDuration = 30.0f;

    [Header("Money")]
    public int startingMoney = 10000;
    public int staffHiringCost = 100;
    public int staffSalary = 100;
    public int denCost = 1000;
    public int portalCost = 1000;

    public static Balance Instance()
    {
        Balance instance = FindObjectOfType<Balance>();
        if (instance == null)
        {
            Debug.LogError("You need to have one instance of Balance in the scene.");
        }
        return instance;
    }
}
