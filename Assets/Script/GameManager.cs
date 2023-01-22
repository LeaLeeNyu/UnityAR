using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
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
        SceneManager.LoadScene("HomePage");
    }

    public void ToLockLevel()
    {
        SceneManager.LoadScene("LoveLock_Test");
    }

    public void ToParkLevel()
    {
        SceneManager.LoadScene("Park_Test");
    }

    public void ToChristmasLevel()
    {
        SceneManager.LoadScene("Christmas_Test");
    }

    public void ToLockAlbum()
    {
        SceneManager.LoadScene("PhotoDetail_Bridge");
    }
    public void ToParkAlbum()
    {
        SceneManager.LoadScene("PhotoDetail_Park");
    }

    public void ToChristmasAlbum()
    {
        SceneManager.LoadScene("PhotoDetail_Tree");
    }

    public void ToFinalAlbum()
    {
        SceneManager.LoadScene("PhotoDetail_Final");
    }
}
