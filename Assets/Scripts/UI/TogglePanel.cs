using UnityEngine;
using System.Collections;

public class TogglePanel : MonoBehaviour
{
    public void Toggle(GameObject panel)
    {
        panel.SetActive(!panel.activeSelf);
    }
}
