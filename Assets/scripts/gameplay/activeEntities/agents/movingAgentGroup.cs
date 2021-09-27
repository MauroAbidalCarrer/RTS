using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingAgentGroup
{
    public List<agent> agents;
    public List<agent> arrived;
    public void receiveDeathSig(entity e)
    {
        agents.Remove(e.GetComponent<agent>());
        arrived.Remove(e.GetComponent<agent>());
        e.deathSubscribers.Remove(receiveDeathSig);
    }
    public void receiveClearingOfDirSig(agent a)
    {
        agents.Remove(a);
        arrived.Remove(a);
        a.clearingOfDirReceivers.Remove(receiveClearingOfDirSig);
        if (agents.Count == 0)
            arrived.Clear();
    }
    public movingAgentGroup(agent a)
    {
        agents = new List<agent>() { a };
        arrived = new List<agent>();
    }
    public movingAgentGroup()
    {
        agents = null;
        arrived = null;
    }
    public movingAgentGroup(List<agent> agents)
    {
        this.agents = agents;
        arrived = new List<agent>();
        foreach (agent a in agents)
        {
            a.deathSubscribers.Add(receiveDeathSig);
            a.clearingOfDirReceivers.Add(receiveClearingOfDirSig);
        }
    }
    public bool arrivedByTouch(agent a)
    {
        foreach(agent other in arrived)
        {
            if (usefull.inRange(a.na.stoppingDistance, a.transform.position, other.transform.position))
            {
                Debug.DrawLine(a.transform.position, other.transform.position, Color.magenta, 0.1f, false);
                return true;
            }
        }
        return false;
    }
}
