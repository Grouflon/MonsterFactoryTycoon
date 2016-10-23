using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MissionPreparation : MonoBehaviour
{
    public ScrollRect assignableList;
    public Text assignableText;
    public ScrollRect assignedList;
    public Text assignedText;
    public Button startMissionButton;
    public Text missionText;

    public MissionPreparation()
    {
        m_assignableMonsters = new List<Monster>();
        m_assignedMonsters = new List<Monster>();
    }

    public void SetMission(Mission _value)
    {
        m_mission = _value;
        m_assignedMonsters.Clear();
        ResetMissionDesc();
    }

    public Mission GetMission()
    {
        return m_mission;
    }

    void Start ()
	{
        m_company = Company.Instance();
        m_UI = UIManager.Instance();

        foreach (Monster monster in m_company.GetMonsters())
        {
            if (m_assignedMonsters.BinarySearch(monster) != -1)
                continue;

            m_assignableMonsters.Add(monster);
        }

        ResetMissionDesc();

        m_company.OnMonsterHired += OnMonsterHired;
        m_company.OnMonsterHealed += OnMonsterHealed;
    }

    void OnDestroy()
    {
        m_company.OnMonsterHired -= OnMonsterHired;
        m_company.OnMonsterHealed -= OnMonsterHealed;
    }

    void OnMonsterHired(Monster _monster)
    {
        m_assignableMonsters.Add(_monster);
        ResetMissionDesc();
    }

    void OnMonsterHealed(Monster _monster)
    {
        if (m_assignableMonsters.Find(delegate (Monster m) { return _monster == m; }) == null)
            return;

        ResetMissionDesc();
    }
	
	void Update()
	{
        
    }

    void ResetMissionDesc()
    {
        UnityTools.DestroyAllChildren(assignableList.content);
        UnityTools.DestroyAllChildren(assignedList.content);

        assignableText.text = "Assignable monsters (" + m_assignedMonsters.Count + ")";
        foreach (Monster monster in m_assignableMonsters)
        {
            Button button = Instantiate(m_UI.listButtonPrefab);
            button.GetComponentInChildren<Text>().text = monster.GetName() + " (" + monster.GetCurrentStrength() + "/" + monster.GetMaxStrength() + ")";
            button.transform.SetParent(assignableList.content);
            button.interactable = m_assignedMonsters.Count < m_mission.maxMonstersCount;

            Monster m = monster;
            button.onClick.AddListener(() =>
            {
                m_assignableMonsters.Remove(m);
                m_assignedMonsters.Add(m);
                ResetMissionDesc();
            });

            if (monster.GetCurrentStrength() != monster.GetMaxStrength())
            {
                // TODO plug an event on heal
                button.interactable = false;
            }
        }

        assignedText.text = "Assigned monsters (" + m_assignedMonsters.Count + "/" + m_mission.maxMonstersCount + ")";
        foreach (Monster monster in m_assignedMonsters)
        {
            Button button = Instantiate(m_UI.listButtonPrefab);
            button.GetComponentInChildren<Text>().text = monster.GetName() + " (" + monster.GetCurrentStrength() + "/" + monster.GetMaxStrength() + ")";
            button.transform.SetParent(assignedList.content);

            Monster m = monster;
            button.onClick.AddListener(() =>
            {
                m_assignedMonsters.Remove(m);
                m_assignableMonsters.Add(m);
                ResetMissionDesc();
            });
        }

        startMissionButton.interactable = m_assignedMonsters.Count != 0;
        startMissionButton.onClick.RemoveAllListeners();
        startMissionButton.onClick.AddListener(() =>
        {
            m_company.StartMission(m_mission, m_assignedMonsters);
            UIWindowManager.Instance().DestroyWindow(transform.GetComponentInParent<UIWindow>());
        });

        missionText.text = "";
        missionText.text += "Obstacles count: " + m_mission.obstacles.Length + "\n";
        missionText.text += "Maximum monsters: " + m_mission.maxMonstersCount + "\n";
        missionText.text += "Reward: " + m_mission.reward + "$\n";
    }

    Mission m_mission;
    List<Monster> m_assignableMonsters;
    List<Monster> m_assignedMonsters;
    Company m_company;
    UIManager m_UI;
}
