using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new ressource obj", menuName ="ressoure object")]
public class ressourceObj : ScriptableObject
{
    public new string name;

    public int nbWrokerFollowersMax = 2;
    public float timeToMine = 0.2f;


    public enum ressourceType
    {
        mineral,
        gas
    }
    public const int nbRtypes = 2;//can't get enum.Length idk why
    public ressourceType RType;
    public int nbFragsPerTake = 2;
    public int initialNbFrags = 10;
}
