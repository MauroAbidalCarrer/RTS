using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct icon
{
    public Sprite sprite;
    public KeyCode key;
    public const float sideLength = 1f;

    public icon(spell s)
    {
        this.sprite = s.sprite;
        this.key = s.key;
    }
    public icon(activeEntityObj obj)
    {
        this.sprite = obj.image;
        this.key = KeyCode.Exclaim;
    }
    public static icon[] toIcons(spell[] spells)
    {
        icon[] icons = new icon[spells.Length];
        for (int i = 0; i < spells.Length; i++)
            icons[i] = new icon(spells[i]);
        return icons;
    }
    public static icon[] toIcons(activeEntityObj[] obj)
    {
        icon[] icons = new icon[obj.Length];
        for (int i = 0; i < obj.Length; i++)
            icons[i] = new icon(obj[i]);
        return icons;
    }

}

public class UI : MonoBehaviour
{
    //player staff
    [SerializeField]
    player Player;
    selectionGroup cSelection;
    Dictionary<activeEntityObj, List<activeEntity>> cEntities;
    activeEntityObj cObj;
    void updateStaff()
    {
        cSelection = Player.cSelection;
        cEntities = cSelection.entities;
        cObj = cSelection.getKey();
    }
    [SerializeField]
    Text[] ressourceCounters = new Text[ressourceObj.nbRtypes];
    [SerializeField]
    //UI
    Transform SingleUnitElements, sPosUnitIcons, sPosSpellIcons;
    [SerializeField]
    Image unitImage;
    List<GameObject> unitIcons = new List<GameObject>();
    List<GameObject> spellsIcons = new List<GameObject>();
    [SerializeField]
    GameObject iconPrefab;
    private void Update()
    {
        //counters
        for (int i = 0; i < ressourceCounters.Length; i++)
            ressourceCounters[i].text = Player.nbFrags[i].ToString();
        //setup
        updateStaff();
        //directives
        foreach (activeEntity ae in cSelection.getEntities())
            ae.DebugNext();
        //clear Panels
        foreach (GameObject go in unitIcons)
            Destroy(go);
        unitIcons.Clear();
        foreach (GameObject go in spellsIcons)
            Destroy(go);
        spellsIcons.Clear();
        if (cObj == null)
            return;
        //spell panel
        showSpells(cObj.dSpells, delegate (spell s) { return delegate () { }; });
        showSpells(cObj.iSpells, delegate (spell s) { return delegate () { }; });
        //image panel
        unitImage.sprite = cObj.image;
        //main panel
        if (cEntities.Keys.Count == 1 && cEntities[cObj].Count == 1)//must be at the end 
        {
            activeEntity ae = cEntities[cObj][0];
            SingleUnitElements.gameObject.SetActive(true);
            SingleUnitElements.GetChild(0).GetComponent<Image>().sprite = cObj.icon;
            SingleUnitElements.GetChild(1).GetComponent<Text>().text = cObj.name;
            SingleUnitElements.GetChild(3).GetComponent<Text>().text = ae.health.ToString() + " / " + cObj.health.ToString();
            SingleUnitElements.GetChild(4).GetComponent<Text>().text = cObj.dammage.ToString();
            SingleUnitElements.GetChild(5).GetComponent<Text>().text = cObj.description;
        }
        else
        {
            int index = 0;
            SingleUnitElements.gameObject.SetActive(false);
            foreach(activeEntityObj obj in cEntities.Keys)
            {
                foreach(activeEntity ae in cEntities[obj])
                {
                    Vector3 offset = new Vector2((float)(index % 8), -(float)(index / 8)) * 50f;
                    unitIcons.Add(Instantiate(iconPrefab, sPosUnitIcons.position + offset, Quaternion.identity, transform));
                    unitIcons[index].GetComponent<Image>().sprite = obj.icon;
                    index++;
                    if (index >= 32)
                        return;
                }
            }
        }
    }
    delegate UnityEngine.Events.UnityAction yes(spell s);
    void showSpells(spell[] spells, yes f)
    {
        for (int i = 0; i < spells.Length; i++)
        {
            Vector3 offset = new Vector2((float)(i % 8), -(float)(i / 8)) * 50f;
            spellsIcons.Add(Instantiate(iconPrefab, sPosSpellIcons.position + offset, Quaternion.identity, transform));
            spellsIcons[i].GetComponent<Image>().sprite = spells[i].sprite;
            //spellsIcons[i].GetComponent<Button>().onClick.AddListener(f(spells[i]));
            spellsIcons[i].transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = spells[i].key.ToString();
        }
    }

    [SerializeField] GameObject line;
    List<GameObject> lines = new List<GameObject>();
    private void Start()
    {
        //line = Instantiate(line);
    }
    public void showDirectives()
    {
        //if (cSelection.getEntities().Count != 1)
        //    return;
        //activeEntity ae = cSelection.getEntities()[0];
        //Vector3 actualDest = ae.target != null && ae.target.side != entity.party.terrain ? ae.target.transform.position : ae.dest;
        //List<Vector3> points = new List<Vector3>();
        //if (ae.nextDs.Count > 0)
        //{
        //    points.Add(actualDest);
        //    points.Add(dToV(0, ae));
        //}
        //for (int i = 1; i < ae.nextDs.Count; i++)
        //{
        //    points.Add(dToV(i - 1, ae));
        //    points.Add(dToV(i, ae));
        //    Debug.DrawLine(dToV(i - 1, ae), dToV(i, ae), Color.green, Time.deltaTime, false);//other nexts
        //}
        //Vector3[] Apoints = points.ToArray();
        //for (int i = 0; i < points.Count - 1; i++)
        //    Debug.DrawLine(Apoints[i], Apoints[i + 1], Color.green, Time.deltaTime, false);
        //line.GetComponent<LineRenderer>().positionCount = Apoints.Length;
        //line.GetComponent<LineRenderer>().SetPositions(Apoints);
    }
    Vector3 dToV(int i, activeEntity ae)
    {
        entity e = ae.nextDs[i].sk.e;
        if (e != null && e.side != entity.party.terrain)
            return e.transform.position + new Vector3(0, 0.1f, 0);
        return ae.nextDs[i].sk.d + new Vector3(0, 0.1f, 0);
    }
    void RenderLines(List<Vector3> points, Color color, Material mat)
    {
        GL.Begin(GL.LINES);
        mat.SetPass(0);
        for (int i = 0; i < points.Count; i++)
        {
            GL.Color(color);
            GL.Vertex(points[i]);
        }
        GL.End();
    }
}
