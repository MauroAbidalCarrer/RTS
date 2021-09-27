using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshObstacle))]
public class building : activeEntity
{
    public bool canBuildHere(Vector3 point, activeEntity other)
    {
        Vector3 extent = getExtents();
        usefull.drawCube(point, extent);
        Collider[] COs = Physics.OverlapBox(point + new Vector3(0, extent.y, 0), extent);
        return COs.Length == 0 || (COs.Length == 1 && COs[0].GetComponent<activeEntity>() && COs[0].GetComponent<activeEntity>() == other);
    }
    public bool canBuildHere(Vector3 point)
    {
        Vector3 extent = getExtents();
        usefull.drawCube(point, extent);
        return Physics.OverlapBox(point + new Vector3(0, extent.y, 0), extent).Length == 0;
    }
    public override void init()
    {
        base.init();
        Etype = entityType.Static;
    }

    public bool isBeingBuilt = true;
    public float constructionState;
    float healthAccumulator;
    public void build()//job for worker
    {
        constructionState += Time.deltaTime;
        healthAccumulator += Time.deltaTime;
        float frac = obj.timeToMake / (float)health;
        if (healthAccumulator >= frac)
        {
            health += (int)(healthAccumulator / frac);
            healthAccumulator -= (int)(healthAccumulator / frac);
        }
        if (constructionState >= obj.timeToMake)
            finishBuilding();
    }
    public virtual void finishBuilding() => isBeingBuilt = false;

    public override void update()
    {
        if (isBeingBuilt)
            doNothing();
        else
            base.update();
    }

    public override void exeDirectDir(directive d) => queueUpDir(d);
}