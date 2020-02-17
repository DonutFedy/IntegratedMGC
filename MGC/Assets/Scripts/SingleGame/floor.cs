using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class floor : MonoBehaviour
{
    public int                      m_curFloor;
    public int                      m_nDirection;
    public obstacleOBJ.onHitPlayer  m_func;

    public abstract void moveDown(float fRate);
}
