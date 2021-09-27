using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class spell : ScriptableObject
{
    //UI
    public string description;
    public string errorMSG = "insufficient ressorces";
    public Sprite sprite;
    public virtual void GraphicsBeforeClick(activeEntity.sKit sk) { }

    public KeyCode key;

    //cast
    public virtual bool canCast(activeEntity ae, activeEntity.sKit sk) => true;
    public abstract directive getDirective(activeEntity ae, activeEntity.sKit sk, movingAgentGroup group);
    public enum spellType
    {
        forEveryUnit,
        oneUnitAtATime
    }
    public spellType Stype;
}