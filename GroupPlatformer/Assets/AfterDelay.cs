using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterDelay : MonoBehaviour
{
    public UnityEngine.Events.UnityEvent Action;
    public float delay;
    public void Invoke()
    {
        StartCoroutine(act(gameObject));
    }
    IEnumerator act(GameObject runner)
    {
        yield return new WaitForSeconds(delay);
        if(runner!=null)
        Action.Invoke();
    }
}
