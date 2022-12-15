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
        RestartQuestSave();
    }

    public void RestartQuestSave()
    {
        Quests.Clear();
        Quests.Add(1, 0); // first ghost apperance
        Quests.Add(2, 0); // lovers in distress
        Quests.Add(3, 0); // enginner
        Quests.Add(4, 0); // second ghost apperance
        Quests.Add(5, 0); // madman
        Quests.Add(6, 0); // dynamite
        Quests.Add(7, 0); // rebel cultist first talk
    }

    private void Awake()
    {
        Vector2 hotspot = new Vector2(mainCursor.width / 2, 0);
        Cursor.SetCursor(mainCursor, hotspot, CursorMode.ForceSoftware);

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
