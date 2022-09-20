using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticHelpers
{

    public static int ChooseDir(Vector3 me, Vector3 they)
    {
        Vector3 newMe = new(me.x, me.y, 0);
        Vector3 newThey = new(they.x, they.y, 0);
        Vector3 normal = (newMe - newThey).normalized;
        float lowPoint = 0.4f;
        if (normal.x <= -lowPoint && normal.y >= lowPoint)
            return 1;
        else if (normal.x <= -lowPoint && (normal.y > -lowPoint && normal.y < lowPoint))
            return 2;
        else if (normal.x <= -lowPoint && normal.y <= -lowPoint)
            return 3;
        else if ((normal.x > -lowPoint && normal.x < lowPoint) && normal.y <= -lowPoint)
            return 4;
        else if (normal.x >= lowPoint && normal.y <= -lowPoint)
            return 5;
        else if (normal.x >= lowPoint && (normal.y > -lowPoint && normal.y < lowPoint))
            return 6;
        else if (normal.x >= lowPoint && normal.y >= lowPoint)
            return 7;
        else
            return 8;
    }

    public static bool CheckIfTargetIsInRange(Vector3Int targetPos, Vector3Int myPos, int range)
    {
        int xDistance = Math.Abs(myPos.x - targetPos.x);
        int yDistance = Math.Abs(myPos.y - targetPos.y);
        if (xDistance <= range && yDistance <= range)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
