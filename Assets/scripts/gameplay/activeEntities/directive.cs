using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct directive
{
    public activeEntity.voidF js;
    public activeEntity.sKit sk;
    public movingAgentGroup g;//optional
    public activeEntity.condition jobDone;//optiona
    public spell s;
    public directive(activeEntity.voidF js, activeEntity.sKit sk)
    {
        this.js = js;
        this.sk = sk;
        g = new movingAgentGroup();
        jobDone = null;
        s = null;
    }
    public directive(activeEntity.voidF js, entity e)
    {
        this.js = js;
        this.sk = new activeEntity.sKit(e);
        g = new movingAgentGroup();
        jobDone = null;
        s = null;
    }
    public directive(activeEntity.voidF js, activeEntity.sKit sk, movingAgentGroup g)
    {
        this.js = js;
        this.sk = sk;
        this.g = g;
        jobDone = null;
        s = null;
    }
    public directive(activeEntity.voidF js, activeEntity.sKit sk, movingAgentGroup g, activeEntity.condition jobDone)
    {
        this.js = js;
        this.sk = sk;
        this.g = g;
        this.jobDone = jobDone;
        s = null;
    }
    public directive(activeEntity.voidF js, activeEntity.sKit sk, activeEntity.condition jobDone)
    {
        this.js = js;
        this.sk = sk;
        this.g = null;
        this.jobDone = jobDone;
        s = null;
    }
    public directive(activeEntity.sKit sk, agent a)
    {
        if (sk.e)
            js = a.tStarters[(int)sk.e.Etype, (int)sk.e.side];
        else
            js = a.sGoToD;
        this.sk = sk;
        g = null;
        jobDone = null;
        s = null;
    }
}