using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attackingUnit : agent
{
    public override void init()
    {
        JSWhenNothingToDO = sWatchOut;
        base.init();
        tStarters[1 - (int)side, 0] = tStarters[1 - (int)side, 1] = sGoAttackE;
    }

    void sGoAttackE()
    {
        if (target.Etype == entityType.unit)
            sGoToU();
        else
            sGoToS();
        isJobDone = target.Etype == entityType.unit ? new condition(UTinAttackRange) : STinAttackRAnge;
        debugSate = "going to attack " + (target.Etype == entityType.unit ? "unit " : "static ") + target.name + " '";
        insertDir(new directive(sAttackE, new sKit(target)));
    }
    void sAttackE()
    {
        na.isStopped = true;
        cJob = target.Etype == entityType.unit ? new voidF(attackU) : attackE;
        isJobDone = delegate() { return target.health <= 0; } ;
        debugSate = "attacking '" + target.name + "'";
    }
    public void attackU()
    {
        if (UTinAttackRange())
            attackE();
        else
            sGoAttackE();
    }
    float attackTimmer;
    void attackE()
    {
        Debug.DrawLine(transform.position, target.transform.position, Color.red, 0.01f);
        if (attackTimmer >= obj.attackSpeed)
        {
            target.takeDammage(obj.dammage, this);
            attackTimmer = 0;
        }
        else
            attackTimmer += Time.deltaTime;
    }

    public override void sAmoveD()
    {
        sGoToD();
        cJob = watchOut;
        debugSate = "Amove dest";
    }
    public override void sAmoveE()
    {
        sGoAttackE();
        cJob = watchOut;
        debugSate = "Amove entity ' " + target.name + " '";
    }

    void sWatchOut()
    {
        na.isStopped = true;
        cJob = watchOut;
        isJobDone = hasSomethingToDoNext;
        debugSate = "watching out";
    }
    public void watchOut()
    {
        if (target != null && target.Etype == entityType.unit)//for sAmoveU
            na.SetDestination(target.transform.position);
        //for some very unknow reasons, the OverlapSphere also gives destroyed objects-_-
        foreach (Collider co in Physics.OverlapSphere(transform.position, watchOutDistance, 1 << 8))
        {
            entity e = co.GetComponent<entity>();
            if (e.health > 0 && isOpp(e))
            {
                insertDir(new directive(cJobJS, new sKit(target, dest), cMovingGroup, isJobDone));
                exeDirective(new directive(sGoAttackE, new sKit(e)));
                break;
            }
        }
        usefull.drawCircle(transform.position, watchOutDistance, Color.red);
    }

    bool STinAttackRAnge() => UTinAttackRange() || isInContactWithStaticEntity ;
    bool UTinAttackRange() => inAttackRange(target.transform.position);
    bool inAttackRange(Vector3 p) => inRangeFromThis(Mathf.Max(obj.attackRange, na.stoppingDistance), p);
}