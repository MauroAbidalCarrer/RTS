using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class agent : activeEntity//harmless agent
{
    public static float largestAgentRadius;
    //init
    [HideInInspector]
    public NavMeshAgent na;
    public override void init()
    {
        tStarters = new voidF[/*Etype*/,/*side*/]
        {   //terrain,player1,player2,ressource
            {sGoToD, sGoToS, sGoToS, sGoToS},//static
            {sGoToD, sGoToU, sGoToU, sGoToU}//units
        };
        na = GetComponent<NavMeshAgent>();
        AEtype = AentityType.orderReceiver;
        Etype = entityType.unit;
        largestAgentRadius = Mathf.Max(na.radius, largestAgentRadius);
        base.init();
    }

    //moving group
    public movingAgentGroup cMovingGroup;
    public delegate void clearingOfDirReceiver(agent a);
    public List<clearingOfDirReceiver> clearingOfDirReceivers = new List<clearingOfDirReceiver>();//moving agent groups
    public override void clearNextDs()
    {
        base.clearNextDs();
        while(clearingOfDirReceivers.Count > 0)
            clearingOfDirReceivers[0](this);
    }
    protected override void exeDirective(directive d)
    {
        cMovingGroup = d.g;
        base.exeDirective(d);
    }

    public const float watchOutDistance = 2;//used here as runwayDistance
    public override void takeDammage(int dammage, entity e)
    {
        if (cJob == doNothing &&  isOpp(e))
        {
            Vector3 pos = transform.position;
            Vector3 d = pos + (pos - e.transform.position).normalized * watchOutDistance;
            Debug.DrawLine(transform.position, d, Color.red, 1);
            movingAgentGroup group = new movingAgentGroup(new List<agent>() { this });//might cause error: not unsubcribing to cMovingGroup
            sWaitFor(0.2f, new directive(sGoToD, new sKit(d), group));//strange..........
        }
        base.takeDammage(dammage, e);
    }

    public override void receiveDeathSig(entity other)
    {
        for (int i = 0; i < nextDs.Count; i++)
        {
            if (nextDs[0].sk.e == other)
                nextDs.Remove(nextDs[0]);
        }
        if (target == other)
            exeNext();
        other.deathSubscribers.Remove(receiveDeathSig);
    }

    #region go to
    public void sGoToD()
    {
        na.isStopped = false;
        na.SetDestination(dest);
        cJob = doNothing;
        isJobDone = isArrivedToDest;
        debugSate = "goingToDest";
    }
    bool isArrivedToDest()
    {
        if (squaredDist(usefull.TTD(dest)) <= usefull.p2(na.stoppingDistance) 
            || cMovingGroup != null && cMovingGroup.arrivedByTouch(this))
        {
            cMovingGroup?.arrived.Add(this);
            return true;
        }
        return false;
    }

    public void sGoToS()
    {
        isInContactWithStaticEntity = false;
        na.isStopped = false;
        na.SetDestination(target.transform.position);
        cJob = doNothing;
        isJobDone = delegate() { return isInContactWithStaticEntity; } ;
        debugSate = "going To static entity " + target.name;
    }
    public bool isInContactWithStaticEntity;
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.GetComponent<entity>() == target)
            isInContactWithStaticEntity = true;
    }

    public void sGoToU()
    {
        if(target == this)
        {
            sRest();
            return;
        }
        na.isStopped = false;
        cJob = goToUnit;
        isJobDone = shouldNotChangeJob;
        debugSate = "going to unitg" + target.name;
    }
    void goToUnit()
    {
        na.SetDestination(target.transform.position);
        if (isOpp(target))
            Debug.DrawLine(transform.position, target.transform.position, Color.yellow);
    }
    #endregion

    //                                                                /*target starters*/
    ////enemmies
    //public virtual void sGoAttackE() => sGoToU();
    ////ressource
    //public virtual void sGoPickUp() => sGoToD();
    //public virtual void sGoToRessource() => sGoToS();
    ////allies
    //public virtual void sGoInteractB() => sGoToS();
                                                                    /*amove starters*/
    public virtual void sAmoveD() => sGoToD();
    public virtual void sAmoveE() => sGoToU();

    //rest
    public override void sRest()
    {
        base.sRest();
        na.isStopped = true;
        target = null;
        dest = Vector3.zero;
    }
}