using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//R:terrain G:viewed B:view
public class fog4 : MonoBehaviour
{
    #region main
    [SerializeField] float updateRate = 0.1f;
    float timmer;
    private void Start()
    {
        if (map.width != map.height)
            Debug.LogError("map is not a square");
    }
    private void Update()
    {
        timmer += Time.deltaTime;
        if (timmer >= updateRate)
            updateFog();
    }
    [SerializeField] bool update;
    private void OnValidate() { if (update) updateFog(); }
    [Space]
    [SerializeField] ComputeShader fogCShader;
    private void updateFog()
    {
        update = false;
        unit[] units = setUpUnits();
        if (units.Length == 0)
        {
            print("aborting og update: units count equals 0");
            return ;
        }
        setupMap();
        setUPUnitBuffer(units);
        fogCShader.Dispatch(0, units.Length, 1, 1);
        unitsBuffer.Dispose();
    }
    #endregion

    #region units
    struct unit
    {
        public Vector2 pos;
        public int height;
        public float viewRadius;
    }
    ComputeBuffer unitsBuffer;
    public Vector2 terrainSize;
    unit GOtoUnit(GameObject go)
    {
        unit u = new unit()
        {
            pos = toFlatV2(go.transform.position) * (float)map.width / terrainSize,
            height = go.GetComponent<moveForward>()._height,
            viewRadius = go.GetComponent<moveForward>().viewRadius * 4 * (float)map.width / (float)terrainSize.x
        };
        float vr = u.viewRadius / 4f;
        bool outOfR(float a, float min, float max) =>  a < min || a > max;
        if (outOfR(u.pos.x, vr, map.width - vr) || outOfR(u.pos.y, vr, map.width - vr))
        {
            print("outside of range: " + go.name);
            print("unit: (pos= " + u.pos + ", height= " + u.height + ", viewRadius= " + u.viewRadius + ")");
            u.pos = -Vector2.one;
        }
        return u;
    }
    Vector2 toFlatV2(Vector3 a) => new Vector2(a.x, a.z);
    unit[] setUpUnits()
    {
        //units
        List<unit> units = new List<unit>();
        GameObject[] Gunits = GameObject.FindGameObjectsWithTag("unit");
        foreach(GameObject go in Gunits)
        {
            unit u = GOtoUnit(go);
            if (u.pos != -Vector2.one)
                units.Add(u);
        }
        return units.ToArray();
    }
    void setUPUnitBuffer(unit[] units)
    {
        unitsBuffer = new ComputeBuffer(units.Length, sizeof(float) * 3 + sizeof(int) * 1);
        unitsBuffer.SetData(units);
        fogCShader.SetBuffer(0, "units", unitsBuffer);
        fogCShader.SetInt("nbUnits", units.Length);
    }
    #endregion

    #region map
    [Space]
    [SerializeField] RenderTexture map;
    [Header("rectangles")]
    [SerializeField] ComputeShader writeSquares;
    [SerializeField] Vector2[] poss;
    [SerializeField] int[] heights;
    [SerializeField] Vector2Int[] sizes;

    void setupMap()
    {
        map.enableRandomWrite = true;
        clearShader.SetTexture(0, "map", map);
        clearShader.Dispatch(0, map.width / 8, map.height / 8, 1);
        if (poss.Length != heights.Length || heights.Length != sizes.Length)
            Debug.LogError("Not all arrays are of same length!");
        Rectangle[] rects = new Rectangle[poss.Length];
        for (int i = 0; i < poss.Length; i++)
            rects[i] = new Rectangle(poss[i], heights[i], sizes[i]);
        writeRectangles(rects);
        fogCShader.SetTexture(0, "map", map);
    }
    public void writeRectangles(Rectangle[] rects)
    {
        var cbRects = new ComputeBuffer(poss.Length, sizeof(float) * 2 + sizeof(int) * 3);
        cbRects.SetData(rects);
        writeSquares.SetBuffer(0, "rectangles", cbRects);
        writeSquares.SetTexture(0, "map", map);
        writeSquares.Dispatch(0, rects.Length, 1, 1);
        cbRects.Dispose();
    }
    [SerializeField] ComputeShader clearShader;
    public struct Rectangle
    {
        public Vector2 UVpos;
        public int height;
        public Vector2Int size;
        public Rectangle(Vector2 p, int h, Vector2Int s)
        {
            UVpos = p;
            height = h;
            size = s;
        }
    }
    #endregion
}
