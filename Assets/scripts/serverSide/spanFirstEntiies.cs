using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class spanFirstEntiies : MonoBehaviour
{
    [SerializeField] GameObject CollectorPrefab, workerPrefab;
    [SerializeField] player Player;
    [SerializeField] Transform masterSpawn, otherSpawn;
    private void Start()
    {
        bool isMaster = PhotonNetwork.IsMasterClient;
        //spawn first units
        Vector3 spawnPoint = (isMaster? masterSpawn : otherSpawn).position;
        CollectorPrefab.GetComponent<collector>().isBeingBuilt = false;
        usefull.spawnAE(CollectorPrefab, spawnPoint);
        CollectorPrefab.GetComponent<collector>().isBeingBuilt = true;
        usefull.spawnAE(workerPrefab, spawnPoint - Vector3.forward * 2);

        Player.side = isMaster ? entity.party.p1 : entity.party.p2;

        Destroy(gameObject);
        Destroy(otherSpawn.gameObject);
        Destroy(masterSpawn.gameObject);
    }
}
