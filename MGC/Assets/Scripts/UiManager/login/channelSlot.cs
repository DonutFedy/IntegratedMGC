using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class channelSlot : MonoBehaviour
{
    [SerializeField]
    Text        m_channelText;
    int         m_nChannelIndex;
    [SerializeField]
    Button      m_button;
    string      m_channelName;

    public delegate void onButtonClicked(int nChannelIndex);
    public onButtonClicked m_clickEvent;

    public void setSlot(string channelName,int nIndex,int nCurManCount ,int nManCountLimit, onButtonClicked triggerEvent)
    {
        m_channelName = channelName;
        m_channelText.text = channelName + " :: "+ nCurManCount +"/"+nManCountLimit;
        m_nChannelIndex = nIndex;
        if(nCurManCount >= nManCountLimit)
        {
            m_button.interactable = false;
        }
        else
        {
            if (m_clickEvent != null)
                m_clickEvent = null;
            m_button.interactable = true;
            m_clickEvent = triggerEvent;
        }
    }

    public string getChannelName()
    {
        return m_channelName;
    }
    public void onClick()
    {
        if (m_clickEvent!=null)
            m_clickEvent(m_nChannelIndex);
    }
}
