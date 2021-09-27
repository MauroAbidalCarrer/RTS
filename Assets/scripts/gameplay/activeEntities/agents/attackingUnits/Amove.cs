using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Amove", menuName = "Amove")]
public class Amove : spell
{
    public override bool canCast(activeEntity ae, activeEntity.sKit sk) => sk.e || sk.d != new Vector3();
    public override directive getDirective(activeEntity ae, activeEntity.sKit sk, movingAgentGroup group)
    {
        if (sk.e.side != entity.party.terrain)
            return new directive(ae.GetComponent<agent>().sAmoveE, sk, group);
        return new directive(ae.GetComponent<agent>().sAmoveD, sk, group);
    }
}
