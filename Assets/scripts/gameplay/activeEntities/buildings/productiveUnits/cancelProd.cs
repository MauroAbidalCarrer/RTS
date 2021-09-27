using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class cancelProd : spell
{
    public override bool canCast(activeEntity ae, activeEntity.sKit sk) => true;
    public override directive getDirective(activeEntity ae, activeEntity.sKit sk, movingAgentGroup group)
    { 
        activeEntity.voidF js = ae.GetComponent<productiveBuilding>().cancelProd;
        return new directive(js, sk);
    }
}
