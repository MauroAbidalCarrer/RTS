using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//R:terrain G:viewed B:view
public class secondFog : MonoBehaviour
{
    [SerializeField] ComputeShader fogCShader;

    #region main
    [SerializeField] bool clear, update;
    private void OnValidate()
    {
        if (clear)
            map.Release();
        if (clear || update)
        {
            update = clear = false;
            setupMap();
            fogCShader.SetTexture(0, "map", map);
            setUpUnits();
            fogCShader.Dispatch(0, units.Length, 1, 1);
            unitsBuffer.GetData(units);
            unitsBuffer.Dispose();
        }
    }
    #endregion

    #region units
    struct unit
    {
        public Vector2 UVpos;
        public int height;
        public float viewRadius;
    }
    unit[] units;
    [SerializeField] Vector2[] positions;
    [SerializeField] int[] unitHeights;
    [SerializeField] float[] viewRadii;
    ComputeBuffer unitsBuffer;
    void setUpUnits()
    {
        //units
        units = new unit[positions.Length];
        for (int i = 0; i < units.Length; i++)
        {
            units[i] = new unit();
            units[i].UVpos = positions[i];
            units[i].height = unitHeights[i];
            units[i].viewRadius = viewRadii[i];
        }
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

    ComputeBuffer cbRects;
    void setupMap()
    {
        map.enableRandomWrite = true;
        if (poss.Length != heights.Length || heights.Length != sizes.Length)
            Debug.LogError("Not all arrays are of same length!");
        Rectangle[] rects = new Rectangle[poss.Length];
        for (int i = 0; i < poss.Length; i++)
            rects[i] = new Rectangle(poss[i], heights[i], sizes[i]);
        writeRectangles(rects);
        clearView();
    }
    public void writeRectangles(Rectangle[] rects)
    {
        cbRects = new ComputeBuffer(poss.Length, sizeof(float) * 2 + sizeof(int) * 3);
        cbRects.SetData(rects);
        writeSquares.SetBuffer(0, "rectangles", cbRects);
        writeSquares.SetTexture(0, "map", map);
        writeSquares.Dispatch(0, rects.Length, 1, 1);
        cbRects.Dispose();
    }
    [SerializeField] ComputeShader clearShader;
    void clearView()
    {
        clearShader.SetTexture(0, "map", map);
        clearShader.Dispatch(0, map.width / 8, map.height / 8, 1);
    }
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
