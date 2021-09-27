using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "spawnSpell", menuName = "spawnSpell")]
public class spawnUnit : spell
{
    public activeEntityObj obj;
    public override bool canCast(activeEntity ae, activeEntity.sKit sk)
    { return ae.Player.canSpend(obj.costs); }
    public override directive getDirective(activeEntity ae, activeEntity.sKit sk, movingAgentGroup group)
    {
        entity e = obj.prefab.GetComponent<entity>();
        return new directive(ae.GetComponent<productiveBuilding>().sSpawnUnit, new activeEntity.sKit(e));
    }
}
