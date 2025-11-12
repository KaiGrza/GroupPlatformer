using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSender : MonoBehaviour
{
    public void LoadLevel(int ID)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(ID);
    }
    public void LoadNextLevel()
    {
        LoadLevel(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1);
    }
}
