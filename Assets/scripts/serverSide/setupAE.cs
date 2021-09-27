using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonTransformView))]
[RequireComponent(typeof(PhotonView))]
public class setupAE : MonoBehaviour
{
    [SerializeField] Behaviour originalBehaviour;
    private void Start() => setup();
    public void setup()//must be called befor init of ae
    {
        bool isMine = GetComponent<PhotonView>().IsMine;
        bool isP1 = PhotonNetwork.IsMasterClient ? isMine : !isMine;
        activeEntity ae = GetComponent<activeEntity>();
        entity e = GetComponent<entity>();
        e.Etype = GetComponent<building>() ? entity.entityType.Static : entity.entityType.unit;
        if (!isMine)
        {
            e = gameObject.AddComponent<entity>();
            Destroy(originalBehaviour);
        }
        e.side = isP1 ? entity.party.p1 : entity.party.p2;
        tag = isP1 ? "player1Party" : "player2Party";
        e.health = ae.obj.health;
        if (isMine)
        {
            originalBehaviour.enabled = true;
            ae.init();
        }
        Destroy(this);
    }
}
