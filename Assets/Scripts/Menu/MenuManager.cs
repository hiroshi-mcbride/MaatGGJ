using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Menu[] menuObjects;
    private Dictionary<MenuType, Menu> menus;
    private MenuType currentMenu;

    private static MenuManager instance;
    public static MenuManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MenuManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        menus = new Dictionary<MenuType, Menu>();
        foreach (Menu m in menuObjects)
        {
            menus.Add(m.menuType, m);
        }
        OpenMenu(MenuType.Main);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            GotoScene(1);
        }
    }

    public void OpenMenu(int menuType)
    {
        MenuType t = (MenuType)menuType;

        
        if (currentMenu == t || menus[t] == null)
        {
            return;
        }
        CloseMenu();
        menus[t].Open();
        currentMenu = t;
    }
    public void OpenMenu(MenuType menuType)
    {
        if (currentMenu == menuType || !menus.ContainsKey(menuType))
        {
            return;
        }
        CloseMenu();
        menus[menuType].Open();
        currentMenu = menuType;
    }

    public void CloseMenu()
    {

        if (!menus.ContainsKey(currentMenu))
        {
            return;
        }
        if (menus[currentMenu].isActiveAndEnabled)
        {
            menus[currentMenu].Close();
        }
        
    }
    public void GotoScene(int _scene) => SceneManager.LoadScene(_scene);
    public void QuitGame() => Application.Quit();
}
