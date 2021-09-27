using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct cube
{
    public Vector3 pos;
    public Color color;
}

public class yes : MonoBehaviour
{
    bool started;
    cube[] cubes;
    [SerializeField] ComputeShader Cshader;

    void Start()
    {
        started = true;
    }

    private void Update()
    {
        cubes = new cube[3];

        ComputeBuffer cBuffer = new ComputeBuffer(cubes.Length, sizeof(float) * 7);
        cBuffer.SetData(cubes);
        Cshader.SetBuffer(0, "cubes", cBuffer);
        Cshader.SetInt("nbCubes", cubes.Length);

        Cshader.Dispatch(0, cubes.Length, 1, 1);

        cBuffer.GetData(cubes);
        cBuffer.Dispose();
    }

    private void OnDrawGizmos()
    {
        if (!started)
            return;
        foreach(cube c in cubes)
        {
            Gizmos.color = c.color;
            print(c.color);
            Gizmos.DrawCube(c.pos, Vector3.one);
            Debug.DrawLine(transform.position, c.pos);
        }
    }
}
