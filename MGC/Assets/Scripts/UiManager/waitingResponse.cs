using PACKET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class waitingResponse : MonoBehaviour
{
    public RectTransform    m_imageTrans;
    public Text             m_text;

    public GameObject       m_okMsgObj;

    [SerializeField]
    public float    m_fRotationSpeed;
    public float    m_fAlphaColorSpeed;
    public float    m_fCurAlphaColorValue;
    int             m_nColorDirection;
    public string   m_basicText = "서버 연결 중";

    public float    m_fWaitingLimitTime = 5f;
    public float    m_fCurrentTime;

    public delegate void overTime();
    public overTime m_overTimeFunc;
    public delegate bool clearWaiting();
    public clearWaiting m_clearWaitingFUNC;

    private void OnEnable()
    {
        init();
    }
    private void OnDisable()
    {
        m_okMsgObj.SetActive(false);
    }

    public void closeOkMsgUI()
    {
        m_okMsgObj.SetActive(false);
        //
        if (m_clearWaitingFUNC == null || m_clearWaitingFUNC() == false)
        {
            return;
        }
        C_Anomaly anomaly = new C_Anomaly();
        GameManager.m_Instance.onErrorClient("Connection Error");
        m_overTimeFunc?.Invoke();

    }

    void init()
    {
        m_okMsgObj.SetActive(false);
        m_text.text = m_basicText;
        m_fCurAlphaColorValue = 1f;
        m_nColorDirection = -1;
        m_fCurrentTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_okMsgObj.activeSelf) return;

        m_imageTrans.Rotate(Vector3.forward*m_fRotationSpeed*Time.deltaTime);

        m_fCurrentTime += Time.deltaTime;
        if(m_fCurrentTime>= m_fWaitingLimitTime)
        {
            m_okMsgObj.SetActive(true);
            return;
        }

        m_fCurAlphaColorValue += m_nColorDirection * m_fAlphaColorSpeed * Time.deltaTime;
        if(m_fCurAlphaColorValue<=0.01f || m_fCurAlphaColorValue >= 1.0f)
        {
            m_nColorDirection *= -1;
        }
        Color curColor = m_text.color;
        curColor.a = m_fCurAlphaColorValue;
        m_text.color = curColor;
    }
}
