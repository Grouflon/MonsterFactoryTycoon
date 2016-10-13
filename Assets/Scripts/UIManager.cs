using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("Prefabs")]
    public RectTransform emptyWindowContentPrefab;
    public Button listButtonPrefab;
    public Button buttonPrefab;
    public Text windowTextPrefab;

    [Header("Misc")]
    public Text moneyText;
    public Text timeText;

    [Header("Staff")]
    public ScrollRect staffListScrollRect;

    [Header("Monsters")]
    public ScrollRect monstersListScrollRect;

    [Header("Rooms")]
    public ScrollRect roomListScrollRect;
    public RectTransform roomWindowContentPrefab;


    public UIManager()
    {
        m_staffButtons = new Dictionary<Guid, Button>();
        m_monstersButtons = new Dictionary<Guid, Button>();
        
    }

	void Awake()
	{
        m_company = Company.Instance();

        m_company.OnStaffHired += OnStaffHired;
        m_company.OnStaffFired += OnStaffFired;

        m_company.OnMonsterHired += OnMonsterHired;

        m_company.OnRoomBuilt += onRoomBuilt;
	}

    void Start()
    {
        m_roomButtons = new Button[m_company.GetRooms().Length];
        for (int i = 0; i < m_roomButtons.Length; ++i)
        {
            Button button = Instantiate(buttonPrefab);
            m_roomButtons[i] = button;

            button.GetComponentInChildren<Text>().text = "Room " + i + "\nEmpty";
            button.transform.SetParent(roomListScrollRect.content);
            button.transform.SetAsLastSibling();

            int roomIndex = i;
            button.onClick.AddListener(() =>
            {
                UIWindow window = null;
                if (UIWindowManager.Instance().CreateWindow("Room " + roomIndex, out window))
                {
                    GameObject content = new GameObject();
                    content.name = "Empty Room Window Content";
                    content.AddComponent<VerticalLayoutGroup>();

                    for (int j = 0; j < Enum.GetNames(typeof(RoomType)).Length; ++j)
                    {
                        RoomType roomType = (RoomType)j;
                        Button roomButton = Instantiate(listButtonPrefab);
                        roomButton.GetComponentInChildren<Text>().text = "Build " + roomType.ToString();

                        roomButton.transform.SetParent(content.transform);

                        roomButton.onClick.AddListener(() =>
                        {
                            m_company.BuildRoom(roomType, roomIndex);
                            UIWindowManager.Instance().DestroyWindow(window);
                        });
                    }

                    window.SetContent(content.transform as RectTransform);
                }
            });
        }
    }
	
	void Update()
	{
        moneyText.text = m_company.GetMoney().ToString() + " $";
        timeText.text = m_company.GetCurrentTime().ToString("0.0") + " Cycles";
	}

    void OnStaffHired(Staff _staff)
    {
        Button button = Instantiate(listButtonPrefab);
        Text buttonText = button.GetComponentInChildren<Text>();
        buttonText.text = _staff.GetName();

        button.onClick.AddListener(() => {

            UIWindow window = null;
            if (UIWindowManager.Instance().CreateWindow(_staff.GetName(), out window))
            {
                RectTransform content = Instantiate(emptyWindowContentPrefab);
                window.SetContent(content);

                Button fireButton = Instantiate(buttonPrefab);
                fireButton.GetComponentInChildren<Text>().text = "Fire";
                fireButton.onClick.AddListener(() => {
                    m_company.FireStaff(_staff);
                });
                fireButton.transform.SetParent(content);
            }
        });

        (button.transform as RectTransform).SetParent(staffListScrollRect.content);
        m_staffButtons.Add(_staff.GetID(), button);
    }

    void OnStaffFired(Staff _staff)
    {
        Button button = null;
        if (m_staffButtons.TryGetValue(_staff.GetID(), out button))
        {
            UIWindowManager.Instance().DestroyWindow(_staff.GetName());

            Destroy(button.gameObject);
            m_staffButtons.Remove(_staff.GetID());
        }
    }

    void OnMonsterHired(Monster _monster)
    {
        Button button = Instantiate(listButtonPrefab);
        Text buttonText = button.GetComponentInChildren<Text>();
        buttonText.text = _monster.GetName();

        button.onClick.AddListener(() =>
        {
            UIWindow window = null;
            if (UIWindowManager.Instance().CreateWindow(_monster.GetName(), out window))
            {
                RectTransform content = Instantiate(emptyWindowContentPrefab);
                window.SetContent(content);

                Text text = Instantiate(windowTextPrefab);
                text.text = "Strength : " + _monster.GetStrength();
                text.transform.SetParent(content);
            }
        });

        (button.transform as RectTransform).SetParent(monstersListScrollRect.content);
        m_monstersButtons.Add(_monster.GetID(), button);
    }

    void onRoomBuilt(Room _room)
    {
        Button button = m_roomButtons[_room.GetPosition()];
        button.GetComponentInChildren<Text>().text = _room.GetName();
        button.onClick.RemoveAllListeners();

        button.onClick.AddListener(() =>
        {
            UIWindow window = null;
            if (UIWindowManager.Instance().CreateWindow(_room.GetName(), out window))
            {
                _room.FillWindowContent(window);
            }
        });
    }

    public static UIManager Instance()
    {
        UIManager instance = FindObjectOfType<UIManager>();
        if (instance == null)
        {
            Debug.LogError("You need to have one instance of UIManager in the scene.");
        }
        return instance;
    }

    private Dictionary<Guid, Button> m_staffButtons;
    private Dictionary<Guid, Button> m_monstersButtons;
    private Button[] m_roomButtons;

    private Company m_company;
}
