using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeServerInfo : MonoBehaviour
{
    public InputField   m_loginIPIF;
    public InputField   m_loginPortIF;
    public InputField   m_mainIPIF;
    public InputField   m_mainPortIF;

    public void onClickChangeInfo()
    {
        GameManager.m_Instance.setLoginServerInfo(m_loginIPIF.text, Int32.Parse(m_loginPortIF.text));
        GameManager.m_Instance.setMainServerInfo(m_mainIPIF.text, Int32.Parse(m_mainPortIF.text));
    }
}
