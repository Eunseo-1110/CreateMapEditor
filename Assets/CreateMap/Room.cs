using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 방
/// </summary>
public class Room : MonoBehaviour
{
    List<Door> m_DoorList;

    [SerializeField]
    CompositeCollider2D AreaCollider;

    bool isInit;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        if (isInit)
            return;

        m_DoorList = new List<Door>(GetComponentsInChildren<Door>());
        AreaCollider = GetComponent<CompositeCollider2D>();
    }

    /// <summary>
    /// 방 콜라이더 얻기
    /// </summary>
    /// <returns>방 콜라이더</returns>
    public CompositeCollider2D GetObjArea()
    {
        return AreaCollider;
    }
}
