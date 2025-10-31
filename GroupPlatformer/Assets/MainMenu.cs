using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void LoadScene(int sceneId)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneId);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void SetPickupCount(int amt)
    {
        PlayerMovement.TotalPickups = (byte)amt;
    }
}
