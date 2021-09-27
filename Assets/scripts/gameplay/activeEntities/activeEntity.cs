using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class activeEntity : entity//player's entity
{
    //player Staff
    [HideInInspector]
    public player Player;
    public activeEntityObj obj;
    public enum AentityType
    {
        orderReceiver,
        other
    }
    [HideInInspector]
    public AentityType AEtype;
    public virtual void addToPlayer(player p)
    {
        p.alliedEntities.Add(this);
        deathSubscribers.Add(p.receiveDeathSig);
        Player = p;
    }
    //jobs and conditions 
    public entity target;
    public struct sKit
    {
        public entity e;
        public Vector3 d;
        public sKit(entity e) { this.e = e; this.d = new Vector3(); }
        public sKit(Vector3 d) { this.e = null; this.d = d; }
        public sKit(entity e, Vector3 d) { this.e = e; this.d = d; }
        public Vector3 toVector3() => e != null && e.side != party.terrain ? e.transform.position : d;
    }
    public delegate bool condition();
    //short is when a job starter does not have an associated job
    public delegate void voidF();
    public condition isJobDone;
    public voidF cJob;
    public voidF cJobJS;
    protected voidF JSWhenNothingToDO;
    public voidF[,] tStarters = new voidF[2, 4];
    public string debugSate;
    //basic jobs
    public virtual void sRest(sKit sk) => sRest();
    public virtual void sRest()
    {
        cJob = doNothing;
        isJobDone = hasSomethingToDoNext;
        debugSate = "resting";
    }
    public virtual void doNothing() { }
    public bool shouldNotChangeJob() => false;
    public bool hasSomethingToDoNext() => nextDs.Count > 0;
    //spells
    [SerializeField]
    bool showEnergy;
    public float energy;
    

    //init and update
    //private void Awake() => init();
    public virtual void init()
    {
        addToPlayer(GameObject.FindGameObjectWithTag("Player").GetComponent<player>());
        health = obj.health;
        JSWhenNothingToDO = JSWhenNothingToDO ?? sRest;
        JSWhenNothingToDO();
    }
    private void Update() => update();
    public virtual void update()
    {
        //main
        if (isJobDone())
            exeNext();
        cJob();
        //debug
        test = nextDs.Count;
        //if (debugNext)
        DebugNext();
    }

    //directives
    public List<directive> nextDs = new List<directive>();
    public int test;
    void insertDirAt(int index, directive d)
    {
        if (d.js == null)
            throw new System.Exception("werid, gave no js");
        if (d.sk.e)
            subscribeToEntity(d.sk.e);
        nextDs.Insert(index, d);
    }
    public void insertDir(voidF js) => insertDirAt(0, new directive(js, new sKit(target)));
    public void insertDir(directive d) => insertDirAt(0, d);
    public void queueUpDir(directive d) => insertDirAt(nextDs.Count, d);

    public virtual void exeNext()
    {
        if (nextDs.Count != 0)
        {
            directive d = nextDs[0];//to avoid stackOverflow
            nextDs.RemoveAt(0);
            exeDirective(d);
        }
        else
            JSWhenNothingToDO();
    }
    public virtual void exeDirectDir(directive d)
    {
        clearNextDs();
        exeDirective(d);
    }
    public virtual void clearNextDs()
    {
        foreach (directive i in nextDs)
        {
            if (i.sk.e)
                i.sk.e.deathSubscribers.Remove(receiveDeathSig);
        }
        nextDs.Clear();
    }

    public Vector3 dest;
    protected void exeDirective(voidF js) => exeDirective(new directive(js, new sKit()));
    protected virtual void exeDirective(directive d)
    {
        updateTarget(d.sk.e);
        dest = d.sk.d;
        d.js();
        if (d.jobDone == null)
            print("isJobDone null, eveything ok");
        isJobDone = d.jobDone ?? isJobDone;
        cJobJS = d.js;
    }
    protected virtual void updateTarget(entity e)//check for the worker override when changing
    {
        if (target)
        {
            bool shouldUnSub = true;
            foreach (directive di in nextDs)
            {
                if (di.sk.e == e)
                {
                    shouldUnSub = false;
                    break;
                }
            }
            if (shouldUnSub)
                target.deathSubscribers.Remove(receiveDeathSig);
        }
        if (e)
            subscribeToEntity(e);
        target = e;
    }
    float timer;
    protected void sWaitFor(float timeToWait, directive d)
    {
        sWaitFor(timeToWait);
        insertDir(d);
    }
    protected void sWaitFor(float timeToWait, voidF js)
    {
        sWaitFor(timeToWait);
        insertDir(js);
    }
    protected void sWaitFor(float timeToInteract)
    {
        timer = timeToInteract;
        cJob = delegate () { timer -= Time.deltaTime; };
        isJobDone = delegate() { return timer <= 0f; };
        debugSate = "waiting to interact with " + target?.name;
    }

    //debug
    public void DebugNext()
    {
        Vector3 actualDest = target != null && target.side != party.terrain ? target.transform.position : dest;
        if (cJob != doNothing)
            usefull.drawCircle(actualDest, 0.25f, Color.green, 20, Time.deltaTime);
        if (nextDs.Count > 0)
            Debug.DrawLine(actualDest, dToV(0), Color.green, Time.deltaTime);//first next
        for (int i = 1; i < nextDs.Count; i++)
            Debug.DrawLine(dToV(i - 1), dToV(i), Color.green, Time.deltaTime);//other nexts
    }
    Vector3 dToV(int i)
    {
        entity e = nextDs[i].sk.e;
        if (e != null && e.side != party.terrain)
            return e.transform.position;
        return nextDs[i].sk.d;
    }
}