using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenusHandler : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject settingsMenu;

    private void Start()
    {
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }
    public void OpenMainMenu()
    {
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }
    public void OpenSettingsMenu()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }
}
