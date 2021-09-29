using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unitsOnMap : MonoBehaviour
{
    [SerializeField] ComputeShader fogCShader;

    #region main
    [SerializeField] bool execute;
    private void OnValidate()
    {
        if (execute)
        {
            execute = false;
            executeShader();
        }
    }
    void executeShader()
    {
        setupMap();
        setUpUnits(viewRadii);
        fogCShader.Dispatch(0, units.Length, 1, 1);
        unitsBuffer.GetData(units);
        unitsBuffer.Dispose();
    }
    #endregion

    #region units
    struct unit
    {
        public Vector2 UVpos;
        public int height;
        public int viewRadiusIndex;
    }
    struct offsetArrayInfo
    {
        public int index;
        public int size;
        public offsetArrayInfo(int index, int size)
        {
            this.index = index;
            this.size = size;
        }
    }
    unit[] units;
    public Vector2[] positions;
    public List<float> viewRadii;
    ComputeBuffer unitsBuffer, offsetArraysInfo, offsetArrays;
    void setUpUnits(List<float> viewRadiuses)
    {
        //units
        units = new unit[positions.Length];
        for (int i = 0; i < units.Length; i++)
        {
            units[i] = new unit();
            units[i].UVpos = positions[i];
        }
        unitsBuffer = new ComputeBuffer(units.Length, sizeof(float) * 2 + sizeof(int) * 2);
        unitsBuffer.SetData(units);
        fogCShader.SetBuffer(0, "units", unitsBuffer);
        fogCShader.SetInt("nbUnits", units.Length);

        //viewArrays
        List<offsetArrayInfo> infos = new List<offsetArrayInfo>();
        List<Vector2Int> arrays = new List<Vector2Int>();
        int cIndex = 0;
        foreach(float viewRadius in viewRadiuses)
        {
            List<Vector2Int> offsets = setOffsets(viewRadius);
            infos.Add(new offsetArrayInfo(cIndex, offsets.Count));
            arrays.AddRange(offsets);
            cIndex += offsets.Count;
        }
        offsetArraysInfo = new ComputeBuffer(infos.Count, sizeof(int) * 2);
        offsetArrays = new ComputeBuffer(arrays.Count, sizeof(int) * 2);
        offsetArraysInfo.SetData(infos.ToArray());
        offsetArrays.SetData(arrays.ToArray());
        fogCShader.SetBuffer(0, "offsetArraysInfo", offsetArraysInfo);
        fogCShader.SetBuffer(0, "offsetArrays", offsetArrays);
        fogCShader.SetInt("nbArray", infos.Count);
    }

    List<Vector2Int> setOffsets(float viewRadius)
    {
        List<Vector2Int> offsets = new List<Vector2Int>();
        for (int i = 0; i < 100; i++)
        {
            Vector2 yes = onCircle((float)i * 360f / 100f) * viewRadius;
            Vector2Int offset = new Vector2Int((int)yes.x, (int)yes.y);
            if (!offsets.Contains(offset))
                offsets.Add(offset);
        }
        return offsets;
    }
    Vector2 onCircle(float angle) => new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle));
    #endregion

    #region map
    [SerializeField] RenderTexture map;
    void setupMap()
    {
        map = new RenderTexture(256, 256, 24);
        map.enableRandomWrite = true;
        map.Create();
        fogCShader.SetTexture(0, "map", map);
        fogCShader.SetInt("width", map.width);
        fogCShader.SetInt("height", map.height);
    }
    #endregion
}
