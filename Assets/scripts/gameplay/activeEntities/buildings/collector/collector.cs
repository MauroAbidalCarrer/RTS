using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collector : building
{
    public override void init()
    {
        base.init();
        AEtype = AentityType.other;
    }
    public float timeToDrop = 0.3f;
    public override void addToPlayer(player p)
    {
        base.addToPlayer(p);
        if(!isBeingBuilt)
            p.collectors.Add(gameObject);
    }
    public override void finishBuilding()
    {
        base.finishBuilding();
        Player.collectors.Add(gameObject);
    }
}
