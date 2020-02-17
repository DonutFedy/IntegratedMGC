using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestZone : floor
{
    [SerializeField]
    List<SpriteRenderer>        m_arrowList = new List<SpriteRenderer>();
    Color                       m_curColor;
    [SerializeField]
    List<obstacleOBJ>           m_obstacleObjList = new List<obstacleOBJ>();
    int                         m_nActiveObstacleCount;

    private void OnEnable()
    {
        m_curColor = Color.white;
        m_nDirection = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_curFloor - singleGameManager.m_nCurMoveLength > 0) return;

        m_curColor.b += m_nDirection * Time.deltaTime;
        if ((m_nDirection <0 && m_curColor.b <= 0.5f)||(m_nDirection > 0 && m_curColor.b >= 1.0f))
            m_nDirection *= -1;
        for (int i = 0; i < m_arrowList.Count; ++i)
            m_arrowList[i].color = m_curColor;
    }

    public void setColorG(float g)
    {
        m_curColor.g = g;
        for (int i = 0; i < m_arrowList.Count; ++i)
            m_arrowList[i].color = m_curColor;
    }
    public void setFloor(int nFloor, obstacleOBJ.onHitPlayer func)
    {
        m_nActiveObstacleCount = 0;
        m_func = func;
        m_curFloor = nFloor;
        gameObject.transform.localPosition = Vector3.up* realFloor();
        for (int i = 0; i < m_obstacleObjList.Count; ++i)
        {
            m_obstacleObjList[i].gameObject.SetActive(false);
            m_obstacleObjList[i].setOBJ(0, m_obstacleObjList[i].transform.localPosition,0,0,m_func);
        }
        for (int i = 0; i < m_arrowList.Count; ++i)
            m_arrowList[i].gameObject.SetActive(false);
    }

    public void onFlag()
    {
        for (int i = 0; i < m_arrowList.Count; ++i)
            m_arrowList[i].gameObject.SetActive(true);
    }
    public void disableFlag()
    {
        for (int i = 0; i < m_arrowList.Count; ++i)
            m_arrowList[i].gameObject.SetActive(false);
    }

    int realFloor()
    {
        return m_curFloor - singleGameManager.m_nCurMoveLength;
    }

    public override void moveDown(float fRate)
    {
        gameObject.transform.localPosition = Vector3.up * (realFloor()-fRate);
        if (realFloor()- fRate <= C_obstacleFloor.fLimitDownY)
        {
            gameObject.SetActive(false);
        }
    }
    public void makeObstacleZone()
    {
        if (m_nActiveObstacleCount >= m_obstacleObjList.Count)
            return;
        int nTryCount = 0;
        while(nTryCount <10)
        {
            int nMakeIndex = UnityEngine.Random.Range(0,m_obstacleObjList.Count);
            if(m_obstacleObjList[nMakeIndex].gameObject.activeSelf == false)
            {
                ++m_nActiveObstacleCount;
                m_obstacleObjList[nMakeIndex].gameObject.SetActive(true);
                break;
            }
            ++nTryCount;
        }
    }
}
