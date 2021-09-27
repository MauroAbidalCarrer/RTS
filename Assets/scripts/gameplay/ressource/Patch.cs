using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patch : MonoBehaviour
{
    public List<ressource> patch;
    public entity getOtherRessource(ressource re)
    {
        for (int i = 0; i < patch.Count; i++)
        {
            if (patch[i] != re)
            {
                moveDown(i);
                return patch[i].GetComponent<entity>();
            }
        }
        return null;
    }
    public ressource findClosest(Vector3 a)
    {
        float smallestDist = usefull.squaredDist(a, patch[0].transform.position);
        ressource closest = patch[0];
        int index = 0;
        for (int i = 1; i < patch.Count; i++)
        {
            if(smallestDist > usefull.squaredDist(a, patch[i].transform.position))
            {
                index = i;
                closest = patch[i];
            }
        }
        moveDown(index);
        return closest;
    }
    void moveDown(int i)
    {
        ressource r = patch[i];
        patch.RemoveAt(i);
        patch.Add(r);
    }
}