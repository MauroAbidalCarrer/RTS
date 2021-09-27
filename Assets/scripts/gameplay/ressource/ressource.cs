using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
public class ressource : entity
{
    private void Awake() => init();
    public virtual void init()
    {
        Etype = entityType.Static;
        side = party.ressource;
        name = obj.name;
        remainingFrags = obj.initialNbFrags;
    }

    public ressourceObj obj;
    public int remainingFrags;
    public int nbWorkersFollowers;
    public int giveFrags()
    {
        if(obj.nbFragsPerTake >= remainingFrags)
        {
            Die();
            return remainingFrags;
        }
        else
        {
            remainingFrags -= obj.nbFragsPerTake;
            GetComponent<PhotonView>().RPC("RPC_giveFrag", RpcTarget.Others);
            return obj.nbFragsPerTake;
        }
    }
    [PunRPC]
    public void RPC_giveFrag()
    {
        print("called");
        remainingFrags -= obj.nbFragsPerTake;
    }
    public bool canBeMinned()
    {
        bool roomForWorker = nbWorkersFollowers < obj.nbWrokerFollowersMax;
        return roomForWorker && remainingFrags > obj.nbFragsPerTake * nbWorkersFollowers;
    }

    public Patch patch;
    public ressource getOther() => patch.getOtherRessource(this)?.GetComponent<ressource>();
    public override void Die()
    {
        patch.patch.Remove(this);
        base.Die();
    }
}
