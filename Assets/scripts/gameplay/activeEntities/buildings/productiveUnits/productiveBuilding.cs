using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class productiveBuilding : building//building that makes units
{
    List<sKit> sks = new List<sKit>();
    public override void init()
    {
        base.init();
        AEtype = AentityType.orderReceiver;
    }
    public void sSpawnUnit()
    {
        sWaitFor(target.GetComponent<activeEntity>().obj.timeToMake);
        insertDir(spawnUnit);
    }
    void spawnUnit()//short
    {
        Vector3 spawnPos = transform.position + new Vector3(0, 0, -getExtents().z);
        if(Physics.OverlapBox(spawnPos, target.getBounds()).Length == 0)
        {
            print("no place to spawn entity");
            return;
        }
        agent a = usefull.spawnAE(target.gameObject, spawnPos).GetComponent<agent>();
        a.queueUpDir(new directive(new sKit(spawnPos - new Vector3(0, 0, 0.2f)), a));
        foreach (activeEntity.sKit sk in sks)
            a.queueUpDir(new directive(sk, a));
        exeNext();
    }
    public void cancelProd() => nextDs.RemoveAt(nextDs.Count - 1);
    public override void receiveDeathSig(entity other)
    {
        for(int i = 0; i < sks.Count; i++)
        {
            if (other == sks[i].e)
                sks.RemoveAt(i);
        }
        other.deathSubscribers.Remove(receiveDeathSig);
    }
    public override void exeDirectDir(directive d)
    {
        if (d.s)
        {
            if (d.js != cancelProd)
                queueUpDir(d);
            else
                cancelProd();
        }
        else
        {
            d.sk.e.deathSubscribers.Add(receiveDeathSig);
            sks.Add(d.sk);
        }
    }
}
