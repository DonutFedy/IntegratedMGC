using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using PACKET;
using PROTOCOL;
using UnityEngine;

public class inputKeyManager : management
{
    List<S_KeyInput>    m_keyInputList;
    StreamReader        m_keySettingFileReader;
    string              m_keySettingFilePath = "\\StreamingAssets\\SetFile\\SettingFile.txt";

    public override void init()
    {
        m_keyInputList = new List<S_KeyInput>();
        m_keySettingFileReader = new StreamReader(Application.dataPath + m_keySettingFilePath);
        int nCount = Int32.Parse(m_keySettingFileReader.ReadLine());

        while(nCount >0)
        {
            string curLine = m_keySettingFileReader.ReadLine();
            string[] splits = curLine.Split(' ');
            int nKeyCode = Int32.Parse(splits[0]);
            int nKeyType = Int32.Parse(splits[1]);
            m_keyInputList.Add(new S_KeyInput(nKeyCode, nKeyType));
            --nCount;
        }

        m_keySettingFileReader.Close();
    }

    public override void release()
    {
        m_keyInputList.Clear();
    }

    protected override void processEvent(C_BasePacket curEvent)
    {
    }

    public void checkInputKey()
    {

        foreach(S_KeyInput var in m_keyInputList)
        {
            if(Input.GetKeyDown((KeyCode)var.m_nKeyCode))
            {
                GameManager.m_Instance.inputKey(var.m_keyType);
            }
        }
    }

    public struct S_KeyInput
    {
        public enum KeyType
        {
            ENTER = 0,
            ESC = 1,
            TAP = 2,

        }
        public int m_nKeyCode;
        public KeyType m_keyType;
        public S_KeyInput(int nKeyCode, int nKeyType)
        {
            m_nKeyCode = nKeyCode;
            m_keyType = (KeyType)nKeyType;
        }
    }

}
