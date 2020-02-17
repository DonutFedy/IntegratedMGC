using PROTOCOL;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class guildInfoSlot : MonoBehaviour
{
    [SerializeField]
    Text                m_guildNameText;
    [SerializeField]
    Text                m_guildRankText;
    //...guild info
    //S_GUILD_INFO        m_guildInfo;

    public delegate void dClickEvent(bool bIsSearchMode,object nickname);
    dClickEvent m_dClickEvent;

    /// <summary>
    /// 삭제 버튼 클릭 이벤트 세팅
    /// </summary>
    public void setSlot(object info, dClickEvent func)
    {
        //m_guildInfo = info;
        m_dClickEvent = null;
        m_dClickEvent += func;
        //m_guildRankText.text = info.m_rank+"위";
        //m_guildNameText.text = info.m_name;
    }

    public void onClick()
    {
        if (m_dClickEvent != null)
        {
            //m_dClickEvent(true,m_guildInfo);
        }
    }
}