using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class confirnFriendSlot : MonoBehaviour
{
    [SerializeField]
    Text                    m_destNameText;
    string                  m_destName;


    public delegate void answerConfirn(string destName, bool bPermission);
    public answerConfirn    m_onClickAnswer;


    public void setSlot(string destName, answerConfirn func)
    {
        m_destName = destName;
        m_destNameText.text = m_destName;
        m_onClickAnswer = func;
    }

    public string getSlotDestName()
    {
        return m_destName;
    }

    public void onClickPermissionBTN()
    {
        m_onClickAnswer?.Invoke(m_destName, true);
    }

    public void onClickRefusalBTN()
    {
        m_onClickAnswer?.Invoke(m_destName,false);
    }
}
