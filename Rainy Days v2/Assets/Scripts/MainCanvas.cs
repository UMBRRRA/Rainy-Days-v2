using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvas : MonoBehaviour
{
    public Dictionary<int, Encounter> FinishedEncounters { get; set; } = new();

    private static MainCanvas singleton;
    private Camera cam;

    private void Awake()
    {

        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        StartCoroutine(FindCamera());

    }

    public IEnumerator FindCamera()
    {
        yield return new WaitUntil(() => (cam = FindObjectOfType<Camera>()) != null);
        GetComponent<Canvas>().worldCamera = cam;
    }

    public void ChangeLevel()
    {
        StartCoroutine(FindCamera());
    }

}
