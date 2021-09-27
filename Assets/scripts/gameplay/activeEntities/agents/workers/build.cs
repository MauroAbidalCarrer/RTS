using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "buildSpell", menuName = "buildSpell")]
public class build : spell
{
    public activeEntityObj obj;
    Vector3 getExtents() => obj.prefab.GetComponent<activeEntity>().getExtents();
    public override bool canCast(activeEntity ae, activeEntity.sKit sk)
    {
        bool canPutHere = obj.prefab.GetComponent<building>().canBuildHere(sk.d + new Vector3(0, getExtents().y, 0));
        return canPutHere && ae.Player.canSpend(obj.costs);
    }
    public override directive getDirective(activeEntity ae, activeEntity.sKit sk, movingAgentGroup group)
    {
        activeEntity.sKit kit = new activeEntity.sKit(obj.prefab.GetComponent<entity>(), sk.d - new Vector3(0, 0, getExtents().z));
        movingAgentGroup g = new movingAgentGroup(ae.GetComponent<agent>());
        return new directive(ae.GetComponent<worker>().sGoBuild, kit, g);
    }
}
