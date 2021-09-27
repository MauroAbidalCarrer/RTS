using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class selectionGroup
{
    public int index;
    List<activeEntity> orderReceivers = new List<activeEntity>();
    List<agent> agents = new List<agent>();
    //List<activeEntityObj> keys = new List<activeEntityObj>();
    public Dictionary<activeEntityObj, List<activeEntity>> entities = new Dictionary<activeEntityObj, List<activeEntity>>();

    //constructors
    public selectionGroup() => addEntities(new List<activeEntity>());
    public selectionGroup(List<activeEntity> entities) => addEntities(entities);

    //ADD
    public void addEntities(List<activeEntity> entities) => entities.ForEach(addEntity);
    public void addEntity(activeEntity ae)
    {
        //check
        bool alreadyContains = false;
        foreach (List<activeEntity> l in entities.Values)
            alreadyContains = l.Contains(ae) ? true : alreadyContains;
        if (alreadyContains)
            return;
        //add
        if (ae.AEtype == activeEntity.AentityType.orderReceiver)
            orderReceivers.Add(ae);
        if (ae.GetComponent<agent>())
            agents.Add(ae.GetComponent<agent>());
        if (entities.ContainsKey(ae.obj))
            entities[ae.obj].Add(ae);
        else
            entities.Add(ae.obj, new List<activeEntity>() { ae });
        //deathSig
        ae.deathSubscribers.Add(receiveDeathSig);
        //Debug.Log(ae.obj.name);
        //Debug.Log(keys.Count);
    }
    void receiveDeathSig(entity e)
    {
        activeEntity ae = e.GetComponent<activeEntity>();
        if (ae.AEtype == activeEntity.AentityType.orderReceiver)
            orderReceivers.Remove(ae);
        if (e.Etype == entity.entityType.unit)
            agents.Remove(ae.GetComponent<agent>());
        entities[ae.obj].Remove(ae);
        if (entities[ae.obj].Count <= 0)
            entities.Remove(ae.obj);
        e.deathSubscribers.Remove(receiveDeathSig);
    }

    //GET
    public List<activeEntity> getEntities()
    {
        List<activeEntity> entities = new List<activeEntity>();
        foreach (List<activeEntity> l in this.entities.Values)
            entities.AddRange(l);
        return entities;
    }
    public activeEntityObj getKey()
    {
        if (entities.Keys.Count != 0)
            return entities.ElementAt(this.index).Key;
        return null;
    }
    public void changeIndex()
    {
        int count = entities.Keys.Count;
        if (count == 0) 
            return;
        index = (Input.GetKey(KeyCode.LeftShift) ? index + count - 1 : index + 1) % count;
    }
    //COMMAND
    public void target(activeEntity.sKit sk)
    {
        movingAgentGroup group = new movingAgentGroup(agents);
        foreach (activeEntity ae in orderReceivers)
            cGD(new directive(ae.tStarters[(int)sk.e.Etype, (int)sk.e.side], sk, group), ae);
    }
    public bool castSpell(activeEntity.sKit sk, spell s)
    {
        bool casted = false;
        movingAgentGroup group = new movingAgentGroup(agents);
        foreach (activeEntity ae in spellOwners(s))
        {
            if (s.canCast(ae, sk))
            {
                directive d = s.getDirective(ae, sk, group);
                d.s = s;
                cGD(d, ae);
                casted = true;
                if (s.Stype == spell.spellType.oneUnitAtATime)
                    return true;
            }
        }
        return casted;
    }
    List<activeEntity> spellOwners(spell s)
    {
        List<activeEntity> entities = new List<activeEntity>();
        foreach(activeEntityObj obj in this.entities.Keys)
        {
            if (obj.dSpells.Contains<spell>(s) || obj.iSpells.Contains<spell>(s))
                entities.AddRange(this.entities[obj]);
        }
        return entities;
    }
    public delegate void giveDirective(directive d, activeEntity ae);
    public static giveDirective cGD;
    public static void queueUpDirective(directive d, activeEntity ae) => ae.queueUpDir(d);
    public static void giveDirectDirective(directive d, activeEntity ae) => ae.exeDirectDir(d);
    
    public void dissolve()
    {
        foreach (List<activeEntity> l in entities.Values)
            l.ForEach(delegate (activeEntity ae) 
            { ae.deathSubscribers.Remove(receiveDeathSig); });
        agents.Clear();
        orderReceivers.Clear();
        entities.Clear();
    }


}
