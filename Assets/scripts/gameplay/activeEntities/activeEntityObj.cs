using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;

[CreateAssetMenu(fileName = "new AEobj", menuName ="AEobj")]
public class activeEntityObj : ScriptableObject
{
    //make
    public new string name = "oui";
    public string description;
    public GameObject prefab;
    public float timeToMake = 5f;
    public int[] costs = new int[ressourceObj.nbRtypes];

    //UI
    public Sprite image;
    public Sprite icon;

    //spell
    public spell[] dSpells;
    public spell[] iSpells;

    public int health = 1;

    //worker
    public int carryingCapacity;
    
    //attackingUnit
    public int dammage = 1;
    public float attackSpeed = 0.5f;
    public float attackRange;

}
