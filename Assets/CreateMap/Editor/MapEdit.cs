using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

public class MapEdit : EditorWindow
{
    List<RoomCreateInfo> RoomList = new List<RoomCreateInfo>();     //방 생성 정보

    string prefab_path = "Assets/04.Prefab/DungeonObj/Room";        //방 프리팹 위치
    int map_count = 0;          //맵 갯수
    bool isRoomObj;             //방 프리팹있는지 확인
    bool isRoomSettingFold;   //방 세팅 폴드 확인
    bool isPathFold;              //방 위치 폴드 확인
    GameObject DoorPrefab;  //문 프리팹

    Vector2 scrollPosition;      //스크롤


    [MenuItem("Window/MyCustom/Map")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(MapEdit));
    }

    public void OnInspectorUpdate()
    {
        this.Repaint();
    }

    private void OnGUI()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);     //스크롤

        DoorPrefab = EditorGUILayout.ObjectField("DoorPrefab", DoorPrefab, typeof(GameObject), false) as GameObject;        //방 프리팹

        //파일 위치 폴드
        isPathFold = EditorGUILayout.Foldout(isPathFold, "RoomPath");
        if (isPathFold)
        {
            //파일 폴더 읽기
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(prefab_path);
            System.IO.DirectoryInfo[] CInfo = di.GetDirectories("*", System.IO.SearchOption.AllDirectories);        //디렉토리 이름 배열

            foreach (System.IO.DirectoryInfo item in CInfo)
            {
                if (GUILayout.Button(item.Name))
                {
                    isRoomObj = GetRoom(prefab_path + "/" + item.Name);     //디렉토리에 방 프리팹이 있는지 확인
                }
            }
        }

        //룸 세팅 폴드
        isRoomSettingFold = EditorGUILayout.Foldout(isRoomSettingFold, "RoomSetting");
        if (isRoomSettingFold)
        {
            //룸세팅 GUI 설정
            foreach (RoomCreateInfo info in RoomList)
            {
                GUILayout.Label(info.Room.name);
                EditorGUILayout.BeginHorizontal();

                info.isCreate = EditorGUILayout.Toggle("Create", info.isCreate);
                info.onlyOne = EditorGUILayout.Toggle("OnlyOne", info.onlyOne);
                info.never = EditorGUILayout.Toggle("Never", info.never);

                EditorGUILayout.EndHorizontal();
            }
        }

        //맵 갯수
        map_count = EditorGUILayout.IntField("Map Count", map_count);

        //생성 버튼
        if(GUILayout.Button("Create"))
        {
            if (isRoomObj)
            {
                CreateDungeon.CreateMap(RoomList, map_count, DoorPrefab);
            }
        }
        GUILayout.EndScrollView();
    }

    /// <summary>
    /// 방 얻기
    /// </summary>
    /// <param name="_path">디렉토리 위치</param>
    /// <returns>있는지 확인</returns>
    bool GetRoom(string _path)
    {
        RoomList.Clear();

        if (System.IO.Directory.Exists(_path))
        {
            //DirectoryInfo 객체 생성
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(_path);

            //해당 폴더에 있는 프리팹 정보 읽기
            foreach (var item in di.GetFiles())
            {
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(_path +"/" + item.Name); //프리팹 가져오기
                if(obj != null)
                {
                    RoomCreateInfo roominfo = new RoomCreateInfo();
                    roominfo.Room = obj;        //프리팹 세팅
                    RoomList.Add(roominfo);     //리스트에 추가
                }
            }
        }

        return RoomList.Count > 0 ? true : false;       //프리팹이 있는지 확인
    }
}
