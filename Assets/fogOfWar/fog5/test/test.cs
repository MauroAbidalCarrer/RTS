using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
	public Vector2 off = Vector2.right;
	public float radius = 20;
	float Nfloor(float a) { return fts.floor(a) == a ? a - 1 : fts.floor(a); }
	float Nceil(float a) { return fts.ceil(a) == a ? a + 1 : fts.ceil(a); }

	float nextDist(float off, float dir)
	{
		float lim = dir > 0 ? Nceil(off) : Nfloor(off);
		return (lim - off) / dir;
	}

	private void OnDrawGizmos()
    {
        Vector2 off = this.off.normalized;
        Vector2 curr = new Vector2();
        Vector2 sDir = fts.sign(off);
        float length = 0;
        while (length <= radius)
        {
            Gizmos.DrawCube(curr * 0.1f, Vector3.one * 0.1f);
            Vector2 next = fts.floor(curr) + sDir;
            Vector2 distToNext = (next - curr) / off;
            float delta = fts.min(distToNext.x, distToNext.y);
            curr += off * delta;
            length += delta;
        }

        //float dist = 0;
        //Vector2 curr = new Vector3();
        //Vector2 dir = off.normalized;
        //while (dist < radius)
        //{
        //          Gizmos.DrawCube(curr * 0.1f, Vector3.one * 0.1f);
        //          float off = fts.min(nextDist(curr.x, dir.x), nextDist(curr.y, dir.y));
        //	curr += dir * off;
        //	dist += off;
        //}
    }
}
