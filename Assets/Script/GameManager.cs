using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public void ToPhoto()
    {
        SceneManager.LoadScene("Photo");
    }

    public void ToMail()
    {
        SceneManager.LoadScene("Mail");
    }

    public void ToCamera()
    {
        SceneManager.LoadScene("Camera");
    }

    public void ToMap()
    {
        SceneManager.LoadScene("Map");
    }

    public void ToMainPage()
    {
        SceneManager.LoadScene("MainPage");
    }
}
