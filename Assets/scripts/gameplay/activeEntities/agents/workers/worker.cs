using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class worker : agent
{
    ressourceObj.ressourceType carriedRType;
    public int nbCarriedFrags;
    [SerializeField]
    ressource rTarget;

    public override void init()
    {
        base.init();
        tStarters[(int)party.ressource, 0] = sGoToRessource;
        tStarters[(int)side, (int)entityType.Static] = sGoInteractB;
    }

    #region mining
    void sGoToRessource()
    {
        //sould not be there but for some very unknown reasons, this gets called even when rTarget == null
        if (!target)
        {
            exeNext();
            print("called");
            return;
        }
        sGoToS();
        debugSate = "going to ressource " + target.name;
        rTarget = target.GetComponent<ressource>();
        insertDir(sTryMine);
        cJobJS = sGoToRessource;
    }
    void sTryMine()//short
    {
        //sould not be there but for some very unknown reasons, this gets called even when rTarget == null
        if (!rTarget)
        {
            exeNext();
            return;
        }
        debugSate = "tried to mine";
        if (rTarget.canBeMinned())
        {
            sWaitFor(rTarget.obj.timeToMine, sTakeFrag);
            rTarget.nbWorkersFollowers++;
        }
        else if (rTarget.patch.patch.Count > 1)
            exeDirective(new directive(sGoToRessource, rTarget.patch.getOtherRessource(rTarget)));
        else
        {
            sRest();
            isJobDone = rTarget.canBeMinned;
            insertDir(sTryMine);
        }
    }
    void sTakeFrag()//short
    {
        entity nextCollector = usefull.findClosest(transform.position, Player.collectors).GetComponent<entity>();
        if (nextCollector == null)
        {
            print("could not find a collector");
            exeDirective(new directive(sRest, new sKit()));
        }
        else
            exeDirective(new directive(sGoInteractB, new sKit(nextCollector)));
        carriedRType = rTarget.obj.RType;
        rTarget.nbWorkersFollowers--;
        //giveFrags must be at the end in case the ressoure dies when taking frag
        nbCarriedFrags = rTarget.giveFrags();
    }
    public override void receiveDeathSig(entity other)
    {
        if (other == rTarget)
        {
            Debug.DrawLine(transform.position, rTarget.transform.position, Color.blue, 3f);
            Debug.DrawLine(transform.position, na.destination, Color.red, 3f);
            //print(" state: " + state + " position= " + transform.position);
            rTarget = rTarget.getOther();
            if (rTarget && other == target)
            {
                exeDirective(new directive(sGoToRessource, rTarget.GetComponent<entity>()));
                Debug.DrawLine(transform.position, rTarget.transform.position, Color.blue, 3f);
            }
        }
        base.receiveDeathSig(other);
        //if (cJob == doNothing)
        //    print("doing nothing");
    }
    protected override void updateTarget(entity e)
    {
        if (!rTarget || target != rTarget)
            base.updateTarget(e);
        else
        {
            if (e)
                subscribeToEntity(e);
            target = e;
        }
    }
    #endregion

    #region briging ressources
    void sGoInteractB()
    {
        sGoToS();
        if (target.GetComponent<collector>() && obj.carryingCapacity > 0)
        {
            debugSate = "going to collector " + target.name;
            insertDir(sWaitToDrop);
        }
    }
    //short
    void sWaitToDrop()
    {
        debugSate = "waiting to drop";
        sWaitFor(target.GetComponent<collector>().timeToDrop, dropRessource);
    }
    void dropRessource()//short js
    {
        debugSate = "dropping";
        Player.nbFrags[(int)carriedRType] += nbCarriedFrags;
        nbCarriedFrags = 0;
        if (rTarget != null)
            exeDirective(new directive(sGoToRessource, rTarget.GetComponent<entity>()));
        else
            exeNext();
    }
    #endregion

    #region building
    public void sGoBuild()
    {
        sGoToD();
        insertDir(sBuild);
        debugSate = "going to build " + target.name;
        Vector3 extent = target.GetComponent<activeEntity>().getExtents();
        buildingSpot = dest + new Vector3(0, extent.y, extent.z);
    }
    Vector3 buildingSpot;
    void sBuild()
    {
        if (!target.GetComponent<building>().canBuildHere(buildingSpot, this))
        {
            print("cant put building anymore");
            Player.cashIn(target.GetComponent<activeEntity>().obj.costs);
            exeNext();
            return;
        }
        updateTarget(usefull.spawnAE(target.gameObject, buildingSpot));
        //target.GetComponent<building>().isBeingBuilt = true;
        target.GetComponent<activeEntity>().addToPlayer(Player);
        cJob = target.GetComponent<building>().build;
        isJobDone = delegate () { return !target.GetComponent<building>().isBeingBuilt; };
        na.isStopped = true;
        debugSate = "building " + target.name;
    }
    #endregion
}