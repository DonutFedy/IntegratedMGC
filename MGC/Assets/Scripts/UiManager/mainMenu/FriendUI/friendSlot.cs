using PACKET;
using PROTOCOL;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class friendSlot : MonoBehaviour
{
    [SerializeField]
    Button              m_whisperBTN;
    [SerializeField]
    Text                m_userNicknameText;
    [SerializeField]
    Text                m_DeleteBTNtext;

    public delegate void dClickEvent(string nickname);
    dClickEvent         m_dDeletClickEvent;
    dClickEvent         m_dWhisperClickEvent;
    S_FriendInfo        m_friendInfo;

    /// <summary>
    /// 삭제 버튼 클릭 이벤트 세팅
    /// </summary>
    public void setSlot(S_FriendInfo info, dClickEvent funcDelete, dClickEvent funcWhisper,bool bIsInviteUI =false)
    {
        m_friendInfo = info;

        if(bIsInviteUI)
        {
            m_DeleteBTNtext.text = "초 대";
            m_whisperBTN.gameObject.SetActive(false);
        }
        else
        {
            m_DeleteBTNtext.text = "삭 제";
            m_whisperBTN.gameObject.SetActive(true);
            m_whisperBTN.interactable = m_friendInfo.m_bIsOnLine;
        }

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
