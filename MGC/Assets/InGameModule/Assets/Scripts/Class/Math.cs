using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Math
{
    public static bool IsPointInRect(Vector2 _vPoint, Vector2 _vRectPos, float _fRectWidthHalf, float _fRectHeightHalf)
    {
        if (_vPoint.x > (_vRectPos.x - _fRectWidthHalf) && _vPoint.x < (_vRectPos.x + _fRectWidthHalf) &&
            _vPoint.y > (_vRectPos.y - _fRectHeightHalf) && _vPoint.y < (_vRectPos.y + _fRectHeightHalf))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool GetIntersectPoint(Vector2 _vAP1, Vector2 _vAP2, Vector2 _vBP1, Vector2 _vBP2, ref Vector2 _IntersectPoint)
    {
        float fUnder = (_vBP2.y - _vBP1.y) * (_vAP2.x - _vAP1.x) - (_vBP2.x - _vBP1.x) * (_vAP2.y - _vAP1.y);

        if (fUnder.Equals(0))
            return false;

        float fT = ((_vBP2.x - _vBP1.x) * (_vAP1.y - _vBP1.y) - (_vBP2.y - _vBP1.y) * (_vAP1.x - _vBP1.x)) / fUnder;
        float fS = ((_vAP2.x - _vAP1.x) * (_vAP1.y - _vBP1.y) - (_vAP2.y - _vAP1.y) * (_vAP1.x - _vBP1.x)) / fUnder;

        if (fT < 0 || fT > 1 || fS < 0 || fS > 1)
            return false;

        _IntersectPoint.x = _vAP1.x + fT * (_vAP2.x - _vAP1.x);
        _IntersectPoint.y = _vAP1.y + fT * (_vAP2.y - _vAP1.y);

        return true;
    }
}
