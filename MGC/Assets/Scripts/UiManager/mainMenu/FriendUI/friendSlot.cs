using PACKET;
using PROTOCOL;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class friendSlot : MonoBehaviour
{
    [SerializeField]
    Text                m_userNicknameText;

    public delegate void dClickEvent(string nickname);
    dClickEvent         m_dDeletClickEvent;
    dClickEvent         m_dWhisperClickEvent;
    S_FriendInfo        m_friendInfo;

    /// <summary>
    /// 삭제 버튼 클릭 이벤트 세팅
    /// </summary>
    public void setSlot(S_FriendInfo info, dClickEvent funcDelete, dClickEvent funcWhisper)
    {
        m_friendInfo = info;
        m_dDeletClickEvent = null;
        m_dDeletClickEvent += funcDelete;
        m_dWhisperClickEvent = null;
        m_dWhisperClickEvent += funcWhisper;
        m_userNicknameText.text = m_friendInfo.m_nickName;
    }

    public void onClickDelete()
    {
        if(m_dDeletClickEvent != null)
        {
            m_dDeletClickEvent(m_friendInfo.m_nickName);
        }
    }
    public void onClickWhisper()
    {
        if (m_dWhisperClickEvent != null)
        {
            m_dWhisperClickEvent(m_friendInfo.m_nickName);
        }
    }


    public int compareNickname(string nickname)
    {
        return m_friendInfo.m_nickName.CompareTo(nickname);
    }


}
