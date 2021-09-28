using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public interface Iunit
{
    int height { get; }
    Vector3 pos { get; }
    float viewRadius { get; }
}

public class fog5 : MonoBehaviour
{
    public bool restart;
    float worldToPix;
    Vector2 terrainSize;
    Dictionary<float, (List<Iunit> Ius, ComputeBuffer CB)> radiiToUnitGroup;
    [SerializeField] RenderTexture fogTex;
    [Header("rectangles")]
    public Transform terrainTransform;
    [SerializeField] Vector2[] Rposs;
    [SerializeField] int[] Rheights;
    [SerializeField] Vector2Int[] Rsizes;
    [Header("Compute Shader")]
    [SerializeField] ComputeShader fogCS;
    [SerializeField] ComputeShader clearCS;
    [SerializeField] ComputeShader writeRectangles;

    private void OnValidate()
    {
        if (restart)
        {
            restart = false;
            setup(fts.topDown(terrainTransform.localScale));
            updateFog(radiiToUnitGroup);
        }
    }

    #region setup
    void Start() => setup(fts.topDown(terrainTransform.localScale));
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
    void setup(Vector2 terrainSize)
    {
        if (terrainSize.x / terrainSize.y != (float)fogTex.width / (float)fogTex.height)
            throw new InvalidOperationException("terrain and fogTex proportion missmatche!");
        worldToPix = (float)fogTex.width / terrainSize.x; /*print("worldToPix= " + worldToPix);*/
        this.terrainSize = terrainSize;
        fogTex.enableRandomWrite = true;
        if (Rposs.Length != Rheights.Length || Rheights.Length != Rsizes.Length)
        {
            print("Not all rect arrays are of same length!");
            return;
        }
        if (Rposs.Length == 0)
        {
            print("no rectangles!");
        }
        else
        {
            Rectangle mkRect(Vector2 p, int h, Vector2Int s) => new Rectangle(p, h, s);
            var rects = fts.map(mkRect, Rposs, Rheights, Rsizes);
            var rectsCB = new ComputeBuffer(rects.Length, sizeof(float) * 2 + sizeof(int) * 3);
            rectsCB.SetData(rects);
            writeRectangles.SetBuffer(0, "rectangles", rectsCB);
            writeRectangles.SetTexture(0, "map", fogTex);
            writeRectangles.Dispatch(0, rects.Length, 1, 1);
            rectsCB.Dispose();
        }

        temporarySetup();
    }
    void temporarySetup()
    {
        //unit groups
        radiiToUnitGroup = new Dictionary<float, (List<Iunit>, ComputeBuffer)>();
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("unit"))
        {
            float rad = go.GetComponent<Iunit>().viewRadius;
            if (radiiToUnitGroup.ContainsKey(rad))
                radiiToUnitGroup[rad].Ius.Add(go.GetComponent<Iunit>());
            else
                radiiToUnitGroup.Add(rad, (new List<Iunit>() { go.GetComponent<Iunit>() }, getOffsetsCB(rad)));
        }
        print(radiiToUnitGroup.Keys.Count);
    }
    ComputeBuffer getOffsetsCB(float radius)
    {
        if (radius == 0)
            throw new ArgumentException("radius == 0");
        radius *= worldToPix;
        int sideLength = (int)fts.ceil(radius);
        Vector2[] offsets = new Vector2[sideLength * 4];
        Debug.DrawLine(new Vector2(-1, 1) * radius, new Vector2(1, 1) * radius, Color.green, 5);
        Debug.DrawLine(new Vector2(1, 1) * radius, new Vector2(1, -1) * radius, Color.green, 5);
        Debug.DrawLine(new Vector2(1, -1) * radius, new Vector2(-1, -1) * radius, Color.green, 5);
        Debug.DrawLine(new Vector2(-1, -1) * radius, new Vector2(-1, 1) * radius, Color.green, 5);
        for(int i = 0; i < sideLength * 4; i++)
        {
            offsets[i] = (Vector2.one * radius + new Vector2(0, -i))/*.normalized*/;
            offsets[i].Normalize();
            print("offsets[" + i + "]= " + offsets[i] + ", normalized= " + offsets[i].normalized);
            Debug.DrawLine(Vector3.zero, offsets[i], Color.red, 5);
            offsets[i + sideLength * 2] = (-Vector2.one * radius + new Vector2(0, i))/*.normalized*/;
            offsets[i + sideLength * 2].Normalize();
            Debug.DrawLine(Vector3.zero, offsets[i + sideLength * 2], Color.red, 5);
            offsets[i + sideLength] = (new Vector2(1, -1) * radius + new Vector2(-i, 0))/*.normalized*/;
            offsets[i + sideLength].Normalize();
            Debug.DrawLine(Vector3.zero, offsets[i + sideLength], Color.red, 5);
            offsets[i + sideLength * 3] = (new Vector2(-1, 1) * radius + new Vector2(i, 0))/*.normalized*/;
            offsets[i + sideLength * 3].Normalize();
            Debug.DrawLine(Vector3.zero, offsets[i + sideLength * 3], Color.red, 5);
        }
        var viewRaysBuffer =  new ComputeBuffer(offsets.Length, sizeof(float) * 2);
        for (int i = 0; i < offsets.Length; i++)
        {
            if (offsets[i] == Vector2.zero)
                print("null Vector at index [" + i + "]");
        }
        viewRaysBuffer.SetData(offsets);
        return viewRaysBuffer;
    }
    #endregion

    #region update
    [SerializeField, Range(10, 60)] float frequence = 30;
    float timmer;
    //void Update()
    //{
    //    timmer += Time.deltaTime;
    //    if(timmer >= 1f / frequence)
    //    {
    //        timmer = 0;
    //        updateFog(radiiToUnitGroup);
    //    }
    //}
    struct unit
    {
        public Vector2 pPos;
        public int height;
        public unit(Vector2 p, int h)
        {
            pPos = p;
            height = h;
        }
    }
    public void updateFog(Dictionary<float, (List<Iunit> Ius, ComputeBuffer CB)> radiiToUG)
    {
        clearCS.SetTexture(0, "map", fogTex);
        clearCS.Dispatch(0, fogTex.width / 8, fogTex.height/ 8, 1);
        foreach (float rad in radiiToUG.Keys)
        {
            bool inRAnge(Iunit iu) => fts.inRange(fts.topDown(iu.pos), fts.fV2(rad), terrainSize - fts.fV2(rad));
            unit toUnit (Iunit iu) => new unit(fts.topDown(iu.pos) * worldToPix, iu.height);
            var units = fts.mapif(toUnit, inRAnge, radiiToUG[rad].Ius);
            updateFogWithRadius(rad, units, radiiToUG[rad].CB);
        }
    }
    void updateFogWithRadius(float radius, List<unit> units, ComputeBuffer normedOffsCB)
    {
        if (units.Count == 0) return;
        var unitsCB = new ComputeBuffer(units.Count, sizeof(float) * 2 + sizeof(int));
        unitsCB.SetData(units);
        fogCS.SetBuffer(0, "units", unitsCB);
        fogCS.SetBuffer(0, "normedOffs", normedOffsCB);
        fogCS.SetTexture(0, "map", fogTex);
        fogCS.SetFloat("radius", radius * worldToPix);
        fogCS.Dispatch(0, unitsCB.count , normedOffsCB.count , 1);
        unitsCB.Dispose();
    }
    #endregion
}
