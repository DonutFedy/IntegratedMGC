using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    public int m_iSendPointCount;      // 임시
    private List<Vector2> m_listPoints = null;
    private float m_fMinDistance;

    public LineRenderer m_lineRenderer;

    private void Start()
    {
        m_fMinDistance = 1f;
    }

    // 안에서 밖으로 나갈 때 교점 연산
    public void SetIntersectionPoint(Vector2 _vAP1, Vector2 _vAP2, Vector2 _vBP1, int _iIdx, int _iLineColor)
    {
        Vector2 vBP2 = m_listPoints.Last();
        Vector2 vIntersectionPoint = new Vector2();

        if(Math.GetIntersectPoint(_vAP1, _vAP2, _vBP1, vBP2, ref vIntersectionPoint))
        {
            if (m_listPoints == null)
            {
                m_listPoints = new List<Vector2>();
                SendPoint(vIntersectionPoint, _iIdx, _iLineColor);
                SetPoint(vIntersectionPoint);

                return;
            }

            SendPoint(vIntersectionPoint, _iIdx, _iLineColor);
            SetPoint(vIntersectionPoint);
        }
        else
        {
            Debug.Log("Fail : Can't draw line with intersection");
        }
    }

    // 밖에서 안으로 들어올 때 교점 연산
    public void SetIntersectionPoint(Vector2 _vAP1, Vector2 _vAP2, Vector2 _vBP1, Vector2 _vBP2, int _iIdx, int _iLineColor)
    {
        Vector2 vIntersectionPoint = new Vector2();

        if (Math.GetIntersectPoint(_vAP1, _vAP2, _vBP1, _vBP2, ref vIntersectionPoint))
        {
            if (m_listPoints == null)
            {
                m_listPoints = new List<Vector2>();
                SendPoint(vIntersectionPoint, _iIdx, _iLineColor);
                SetPoint(vIntersectionPoint);

                return;
            }

            SendPoint(vIntersectionPoint, _iIdx, _iLineColor);
            SetPoint(vIntersectionPoint);
        }
        else
        {
            Debug.Log("Fail : Can't draw line with intersection");
        }
    }

    public void UpdateLine(Vector2 _vMousePos, int _iIdx, int _iLineColor)
    {
        //1// 첫 점이면 거리 계산 없이 찍음
        if (m_listPoints == null)
        {
            m_listPoints = new List<Vector2>();
            SendPoint(_vMousePos, _iIdx, _iLineColor);
            SetPoint(_vMousePos);

            return;
        }
        //1//

        //2// 마우스 상 거리가 m_fMinDistance보다 크면 점을 찍음
        if (Vector2.Distance(m_listPoints.Last(), _vMousePos) > m_fMinDistance)
        {
            SendPoint(_vMousePos, _iIdx, _iLineColor);
            SetPoint(_vMousePos);
        }
        //2//
    }

    private void SendPoint(Vector2 _vPoint, int _iIdx, int _iLineColor)
    {
        GameData.SendPointPacket point = new GameData.SendPointPacket();
        point.byteGameType = (byte)GameData.EnumGameType.CATCH;
        point.byteStructType = (byte)GameData.EnumGameCatchStructType.SEND_POINT_PACKET;
        point.iColor = _iLineColor;
        point.iLineIdx = _iIdx;
        point.fX = _vPoint.x;
        point.fY = _vPoint.y;

        byte[] packet = Serializer.StructureToByte(point);

        IngameManager.m_Instance.GetClient().SendPacket(packet);

        Debug.Log("Send Point Packet, _vPoint : " + _vPoint);

        ++m_iSendPointCount;
    }

    public void SetPoint(Vector2 _vPoint)
    {
        if (m_listPoints == null)
            m_listPoints = new List<Vector2>();

        m_listPoints.Add(_vPoint);

        m_lineRenderer.positionCount = m_listPoints.Count;
        m_lineRenderer.SetPosition(m_listPoints.Count - 1, _vPoint);
    }
}
