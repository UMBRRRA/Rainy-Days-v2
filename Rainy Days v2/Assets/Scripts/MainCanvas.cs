using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvas : MonoBehaviour
{
    public Dictionary<int, Encounter> FinishedEncounters { get; set; } = new();

    public Dictionary<int, int> Quests { get; set; } = new();

    public Texture2D mainCursor;

    private static MainCanvas singleton;
    private Camera cam;

    private void Start()
    {
        // add empty quests here
        Quests.Add(1, 0); // this is first ghost apperance
    }

    private void Awake()
    {

        Cursor.SetCursor(mainCursor, Vector2.zero, CursorMode.ForceSoftware);

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
