using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class usefull
{
    public static entity spawnAE(GameObject AEprefab, Vector3 pos)
    {
        GameObject go = PhotonNetwork.Instantiate(AEprefab.name, pos, Quaternion.identity);
        go.GetComponent<setupAE>().setup();
        return go.GetComponent<entity>();
    }

    public static void drawCube(Vector3 pos, Vector3 e) => drawCube(pos, e, Color.white);
    public static void drawCube(Vector3 pos, Vector3 e, Color color) => drawCube(pos, e, color, 5f);
    public static void drawCube(Vector3 pos, Vector3 e, Color color, float time)
    {
        drawQuad(pos + new Vector3(0, e.y, 0), TTD(e), color, time);
        drawQuad(pos - new Vector3(0, e.y, 0), TTD(e), color, time);
        Debug.DrawLine(pos + e, pos + new Vector3(e.x, -e.y, e.z), color, time, true);
        Debug.DrawLine(pos + new Vector3(-e.x, e.y, e.z), pos + new Vector3(-e.x, -e.y, e.z), color, time, true);
        Debug.DrawLine(pos + new Vector3(e.x, e.y, -e.z), pos + new Vector3(e.x, -e.y, -e.z), color, time, true);
        Debug.DrawLine(pos + new Vector3(-e.x, e.y, -e.z), pos - e, color, time, true);
    }
    public static void drawQuad(Vector3 pos, Vector2 e, Color color, float time)
    {
        Debug.DrawLine(pos + TTD(e), pos + new Vector3(-e.x, 0, e.y), color, time, true);
        Debug.DrawLine(pos + TTD(e), pos + new Vector3(e.x, 0, -e.y), color, time, true);
        Debug.DrawLine(pos - TTD(e), pos - new Vector3(-e.x, 0, e.y), color, time, true);
        Debug.DrawLine(pos - TTD(e), pos - new Vector3(e.x, 0, -e.y), color, time, true);
    }
    public static void drawCircle(Vector3 center, float radius) => drawCircle(center, radius, Color.white);
    public static void drawCircle(Vector3 center, float radius, float time) => drawCircle(center, radius, Color.white, 30, time);
    public static void drawCircle(Vector3 center, float radius, Color color) => drawCircle(center, radius, color, 30, 0f);
    public static void drawCircle(Vector3 center, float radius, Color color, int nbSeg, float time)
    {
        for (int i = 0; i < nbSeg; i++)
            Debug.DrawLine(center + TTD(angleToV(((float)i / (float)nbSeg)*360)) * radius, center + TTD(angleToV(((float)(1+i) / (float)nbSeg) * 360)) * radius, color, time, true);
    }
    public static Vector2 angleToV(float a) => new Vector2(Mathf.Cos(Mathf.Deg2Rad * a), Mathf.Sin(Mathf.Deg2Rad * a));

    public static GameObject findClosest(Vector3 a, GameObject[] GOs)
    {
        float smallestDist = float.PositiveInfinity;
        GameObject closest = null;
        foreach(GameObject go in GOs)
            closest = smallestDist > squaredDist(a, go.transform.position) ? go : closest;
        return closest;
    }
    public static GameObject findClosest(Vector3 a, List<GameObject> GOs)
    {
        float smallestDist = float.PositiveInfinity;
        GameObject closest = null;
        foreach (GameObject go in GOs)
            closest = smallestDist > squaredDist(a, go.transform.position) ? go : closest;
        return closest;
    }

    public static bool inRange(float range, Vector3 a, Vector3 b) => p2(range) >= squaredDist(TTD(a), TTD(b));

    public static float squaredDist(Vector3 a, Vector3 b) => squaredDist(TTD(a), TTD(b));
    public static float squaredDist(Vector2 a, Vector2 b)//dist from a to b
    {
        Vector2 diff = p2(a - b);
        return diff.x + diff.y;
    }

    public static float p2(float a) => a * a;
    public static Vector2 p2(Vector2 a) => new Vector2(p2(a.x), p2(a.y));

    public static Vector2 TTD(Vector3 v) => new Vector2(v.x, v.z);//toTopDown
    public static Vector3 TTD(Vector2 v) => new Vector3(v.x, 0, v.y);//toTopDown

    public static bool inBox(Vector2 s, Vector2 e, Vector2 p) => inBetween(s.x, e.x, p.x) && inBetween(s.y, e.y, p.y);
    static bool inBetween(float s, float e, float p) => (s >= p && p >= e) || (s <= p && p <= e);

    public static bool isOpp(entity.party a, entity.party b) => a != b && sideOfP(a) && sideOfP(b);
    public static bool sideOfP(entity.party p) => p == entity.party.p1 || p == entity.party.p2;//is on the side of a player
}
