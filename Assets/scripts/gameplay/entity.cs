using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class entity : MonoBehaviour//everything that does something in the game
                                   //EVERY GO THAT IS IN THE SELECT LAYERMUST HAVE THIS
{
    //party and type
    public enum party
    {
        terrain,
        p1,
        p2,
        ressource
    }
    //[HideInInspector]
    public party side;
    public enum entityType
    {
        Static,
        unit
    }
    [HideInInspector]
    public entityType Etype;
    protected bool isOpp(entity other) => usefull.isOpp(side, other.side);
    protected bool isOpp(party other) => usefull.isOpp(side, other);

    //death "event"
    public delegate void deathSignalReceiver(entity e);
    public virtual void receiveDeathSig(entity other) => print("reiceved death sig from: " + other.name);
    public List<deathSignalReceiver> deathSubscribers = new List<deathSignalReceiver>();
    protected List<List<deathSignalReceiver>> broadcasters = new List<List<deathSignalReceiver>>();
    protected void subscribeToEntity(entity e)
    {
        if (e.deathSubscribers.Contains(receiveDeathSig))
            return;
        e.deathSubscribers.Add(receiveDeathSig);
        broadcasters.Add(e.deathSubscribers);
    }
    protected void unSubscribeToEntity(entity e)
    {
        e.deathSubscribers.Remove(receiveDeathSig);
        broadcasters.Remove(e.deathSubscribers);
    }
    public virtual void Die()
    {
        while(deathSubscribers.Count > 0)
            deathSubscribers[0](this);
        foreach (List<deathSignalReceiver> l in broadcasters)
            l.Remove(receiveDeathSig);
        Destroy(gameObject);
    }

    //health
    public int health = 1;
    public virtual void takeDammage(int dammage, entity e)//should change
    {
        if(changeHealth(-dammage))
        {
            GetComponent<PhotonView>().RPC("RPC_Die", RpcTarget.All);
            Die();
        }
        else
            GetComponent<PhotonView>().RPC("RPC_takeDammage", RpcTarget.All, dammage);
    }
    [PunRPC]
    public void RPC_takeDammage(int dammage) => changeHealth(-dammage);
    [PunRPC]
    public void RPC_Die() => Die();
    bool changeHealth(int delta)
    {
        health += delta;
        return health <= 0;
    }

    //public void resetColor()
    //{
    //    Color c = Random.ColorHSV();
    //    GetComponent<SpriteRenderer>().color = c;
    //    GetComponent<PhotonView>().RPC("RPC_resetColor", RpcTarget.AllBuffered, c.r, c.g, c.b, c.a);
    //}
    //[PunRPC]
    //public void RPC_resetColor(float r, float g, float b, float a)
    //{ GetComponent<SpriteRenderer>().color = new Color(r, g, b, a); }
    //make
    public Vector3 getExtents()
    {return getBounds() + new Vector3(agent.largestAgentRadius, 0, agent.largestAgentRadius);}
    public Vector3 getBounds()
    {return GetComponent<MeshFilter>().sharedMesh.bounds.size / 2f;}

    //usefull
    public GameObject closest(GameObject[] gos) => usefull.findClosest(transform.position, gos);
    public GameObject closest(List<GameObject> gos) => usefull.findClosest(transform.position, gos);
    public bool inRangeFromThis(float range, Vector3 b) => usefull.inRange(range, transform.position, b);
    public float squaredDist(Vector2 b) => usefull.squaredDist(usefull.TTD(transform.position), b);
}