using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCreateInfo
{
    public GameObject Room;
    public bool isCreate;           //�����ϴ���
    public bool onlyOne;           //1����?
    public bool never;    //������ ��������
    public int index;
}


public class MapInfo
{
    public RoomInfo[,] RoomArr;         //�� �迭
}