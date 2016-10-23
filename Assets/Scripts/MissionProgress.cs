using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MissionProgress : MonoBehaviour
{
    public Button obstacleButtonPrefab;

    public RectTransform progressBar;
    public ScrollRect monstersList;
    public Text obstacleDescription;

    public MissionProgress()
    {
    }

    public void SetMissionInstance(MissionInstance _mission)
    {
        m_mission = _mission;
    }

	void Start ()
	{
        m_company = Company.Instance();
        UpdateContent();
    }

    void UpdateContent()
    {
        UnityTools.DestroyAllChildren(monstersList.content);
        foreach (Monster monster in m_mission.GetMonsters())
        {
            Button button = Instantiate(UIManager.Instance().listButtonPrefab);
            button.GetComponentInChildren<Text>().text = monster.GetName() + " (" + monster.GetCurrentStrength() + "/" + monster.GetMaxStrength() + ")"; 

            button.transform.SetParent(monstersList.content);

            Monster m = monster;
            button.onClick.AddListener(() =>
            {
                m_mission.CommitMonster(m);

                UpdateContent();
            });
        }

        int obstacleIndex = 0;
        UnityTools.DestroyAllChildren(progressBar);
        foreach (Obstacle obstacle in m_mission.GetMission().obstacles)
        {
            Button button = Instantiate(obstacleButtonPrefab);
            ColorBlock color = button.colors;
            color.normalColor = obstacleIndex == m_mission.GetCurrentObstacle() ? Color.white : Color.gray;
            color.disabledColor = obstacleIndex == m_mission.GetCurrentObstacle() ? Color.white : Color.gray;
            button.colors = color;
            //button.GetComponent<Image>().color = obstacleIndex == m_mission.GetCurrentObstacle() ? Color.white : Color.gray;
            button.transform.SetParent(progressBar);
            ++obstacleIndex;
        }

        UpdateObstacleDesc(m_mission.GetCurrentObstacle());
        UpdateObstacleSelection(m_mission.GetCurrentObstacle());
    }

    void UpdateObstacleDesc(int _obstacleIndex)
    {
        obstacleDescription.text = "";

        if (_obstacleIndex >= m_mission.GetMission().obstacles.Length)
            return;

        Obstacle obstacle = m_mission.GetMission().obstacles[_obstacleIndex];
        obstacleDescription.text += obstacle.name + "\n";
        obstacleDescription.text += "Strength: " + m_mission.GetCurrentObstacleStrength() + "/" + obstacle.strength;
    }

    void UpdateObstacleSelection(int _obstacleIndex)
    {
        int i = 0;
        foreach (Transform child in progressBar)
        {
            if (i == _obstacleIndex)
            {
                child.GetComponent<Image>().color = Color.white;
            }
            else
            {
                child.GetComponent<Image>().color = Color.gray;
            }
            ++i;
        }
    }
	
	void Update ()
	{
	
	}

    Company m_company;
    MissionInstance m_mission;
}
