using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class obstacleOBJ : MonoBehaviour
{
    protected int       m_nDirection;
    protected float     m_fSpeed;
    protected Vector3   m_initPos;
    float               m_fLimitX;
    float               m_fOffsetSpeed;

    public delegate void onHitPlayer();
    public onHitPlayer  m_func;

    public Collider2D   m_collider;

    public virtual void setOBJ(int nDirection, Vector3 pos, float LimitX, float fOffsetSpeed, onHitPlayer func)
    {
        
        m_nDirection = nDirection;
        m_fOffsetSpeed = fOffsetSpeed;
        resetSpeed();
        m_initPos = pos;
        transform.localPosition = m_initPos;
        m_fLimitX = LimitX;
        if(m_func == null)
            m_func = func;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && m_func != null)
            m_func();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && m_func != null)
            m_func();
    }
    

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && m_func != null)
            m_func();
    }


    public virtual float updateOBJ()
    {
        gameObject.transform.localPosition += Vector3.right * m_fSpeed * m_nDirection * Time.deltaTime;
        if ( (m_nDirection > 0 && transform.localPosition.x >= m_fLimitX ) ||
            (m_nDirection < 0 && transform.localPosition.x <= m_fLimitX))
        {
            resetPosX();
        }
        return transform.localPosition.x;
    }
    public void resetPosX()
    {
        m_initPos.y = transform.localPosition.y;
        transform.localPosition = m_initPos;
        resetSpeed();
    }

    void resetSpeed()
    {
        m_fSpeed = Random.Range(1, 3+ m_fOffsetSpeed);
    }

    public float moveDown(float pos)
    {
        Vector3 curPos = gameObject.transform.localPosition;
        curPos.y = pos;
        gameObject.transform.localPosition = curPos;
        return transform.localPosition.y;
    }
    public float getY()
    {
        return transform.localPosition.y;
    }
}
