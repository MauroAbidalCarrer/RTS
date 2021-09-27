using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class player : MonoBehaviour
{
    [Header("units")]
    public List<activeEntity> alliedEntities = new List<activeEntity>();
    public List<GameObject> collectors = new List<GameObject>();
    public entity.party side = entity.party.p1;
    public void receiveDeathSig(entity e)
    {
        alliedEntities.Remove(e.GetComponent<activeEntity>());
        e.deathSubscribers.Remove(receiveDeathSig);
    }
    [Header("ressources")] 
    public int[] nbFrags = new int[ressourceObj.nbRtypes];
    [Header("selection")]
    public selectionGroup cSelection = new selectionGroup();
    activeEntityObj cObj;
    //command
    enum playerState
    {
        nothing,
        slecting,
        castingISpell
    }
    [SerializeField]
    playerState Pstate;
    delegate bool inputD();
    delegate void voidF();
    voidF cJob;
    struct behaviour
    {
        public inputD input;
        public voidF[] outcomes;
        public behaviour(inputD input, voidF[] outcome)
        {
            this.input = input;
            this.outcomes = outcome;
        }
        public behaviour(inputD input, voidF outcome1, voidF outcome2, voidF outcome3)
        {
            this.input = input;
            this.outcomes = new voidF[] { outcome1, outcome2, outcome3 };
        }
        public behaviour(inputD input, voidF outcome)
        {
            this.input = input;
            this.outcomes = new voidF[] { outcome, outcome, outcome, outcome };
        }
    }
    behaviour[] behaviours;


    [Header("UI"), SerializeField]
    Image selectImg;
    [SerializeField]
    Image unitImg;
    [SerializeField]
    Transform productionTab;

    private void Start()
    {
        behaviours = new behaviour[]
        {
            new behaviour(dSpellCalled, tryCastDspell),
            new behaviour(keyD(KeyCode.Escape), doNothing, cancelSelect, cancelIcast),
            new behaviour(MouseD(1), target, cancelSelect, cancelIcast ),
            new behaviour(MouseD(0), sSelect, doNothing, fCastIspell),
            new behaviour(iSpellCalled, sCastIspell, doNothing, sCastIspell),
            new behaviour(delegate(){return Input.GetMouseButtonUp(0); }, doNothing, fSelect, doNothing),
            new behaviour(keyD(KeyCode.Tab), changeIndex, doNothing, doNothing)
        };
        cJob = doNothing;
        Cursor.SetCursor(defaultCursor, arrowOffset, CursorMode.Auto);
    }
    void changeIndex() => cSelection.changeIndex();
    inputD keyD(KeyCode k) => delegate () { return Input.GetKeyDown(k); };
    inputD MouseD(int i) => delegate () { return Input.GetMouseButtonDown(i); };
    Vector2 arrowOffset = new Vector2(45, 50);
    Vector2 targetOffset = new Vector2(45, 50);
    activeEntity.sKit cSK;
    private void Update()
    {
        //command
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 8);
        entity e = null;
        if (hit.collider?.GetComponent<entity>())
        {
            e = hit.collider.GetComponent<entity>();
            e = e.CompareTag("wall") ? null : e;
        }
        cSK = new activeEntity.sKit(e, hit.point);
        selectionGroup.cGD = selectionGroup.giveDirectDirective;
        if (Input.GetKey(KeyCode.LeftShift))
            selectionGroup.cGD = selectionGroup.queueUpDirective;
        foreach (behaviour b in behaviours)
        {
            if (b.input())
                b.outcomes[(int)Pstate]();
        }
        cJob();
        cObj = cSelection.getKey();
    }

    //                              behaviours
    bool dSpellCalled()
    {
        if (cObj)
            return getIndexOfCalledSpell(cObj.dSpells) != -1;
        return false;
    }
    public void tryCastDspell()
    {
        if (cSelection.castSpell(cSK, cSpell))
        {
            if (Pstate == playerState.slecting)
                cancelSelect();
            else if (Pstate == playerState.castingISpell)
                cancelIcast();
        }
    }
    void target()
    {
        if (cSK.e)
            cSelection.target(cSK);
    }
    void sSelect()
    {
        Pstate = playerState.slecting;
        cJob = select;
        selectImg.gameObject.SetActive(true);
        sMousPos = Input.mousePosition;
    }
    public float minimalDistanceSelect = 0.1f;
    void fSelect()
    {
        cSelection.dissolve();
        takeEntitiesInSelectBox();
        cancelSelect();
    }
    void takeEntitiesInSelectBox()
    {
        List<activeEntity> whatsIn = new List<activeEntity>();
        foreach (activeEntity ae in alliedEntities)
        {
            Vector2 sPos = Camera.main.
                WorldToScreenPoint(ae.transform.position);
            if (usefull.inBox(sMousPos, Input.mousePosition, sPos))
                whatsIn.Add(ae);
        }
        cSelection = new selectionGroup(whatsIn);
    }
    bool iSpellCalled()
    {
        if (cObj)
            return getIndexOfCalledSpell(cObj.iSpells) != -1;
        return false;
    }
    public Texture2D defaultCursor;
    public Texture2D targetCursor;
    public void sCastIspell()
    {
        Pstate = playerState.castingISpell;
        cJob = castingSpell;
        Cursor.SetCursor(targetCursor, targetOffset, CursorMode.Auto);
    }
    void fCastIspell()
    {
        cSelection.castSpell(cSK, cSpell);
        if(!Input.GetKey(KeyCode.LeftShift))
        {
            sDoN();
            Cursor.SetCursor(defaultCursor, arrowOffset, CursorMode.Auto);
        }
    }
    //                              jobs
    //nothing
    void sDoN()
    {
        Pstate = playerState.nothing;
        cJob = doNothing;
    }
    void doNothing() { }
    //selecting
    Vector2 sMousPos;
    void select()
    {
        if (Vector2.Distance(sMousPos, Input.mousePosition) < minimalDistanceSelect)
        {
            selectImg.gameObject.SetActive(false);
            return;
        }
        selectImg.gameObject.SetActive(true);
        Vector2 mousePos = Input.mousePosition;
        selectImg.rectTransform.anchoredPosition = sMousPos + (mousePos - sMousPos) / 2f;
        selectImg.rectTransform.sizeDelta = new Vector2(Mathf.Abs(mousePos.x - sMousPos.x), Mathf.Abs(mousePos.y - sMousPos.y));
    }
    void cancelSelect()
    {
        selectImg.gameObject.SetActive(false);
        sDoN();
    }
    //castingIspell
    [HideInInspector]
    public spell cSpell;
    void castingSpell() { }
    void cancelIcast()
    {
        cSpell = null;
        sDoN();
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }
    public bool canSpend(int[] costs)
    {
        for (int i = 0; i < nbFrags.Length; i++)
        {
            if (nbFrags[i] < costs[i])
                return false;
        }
        spend(costs);
        return true;
    }
    public void spend(int[] costs)
    {
        for (int i = 0; i < nbFrags.Length; i++)
            nbFrags[i] -= costs[i];
    }
    public void cashIn(int[] incomes)
    {
        for (int i = 0; i < nbFrags.Length; i++)
            nbFrags[i] += incomes[i];
    }
    int getIndexOfCalledSpell(spell[] spells)
    {
        for (int i = 0; i < spells.Length; i++)
        {
            if (Input.GetKeyDown(spells[i].key))
            {
                cSpell = spells[i];
                return i;
            }
        }
        return -1;
    }
}
