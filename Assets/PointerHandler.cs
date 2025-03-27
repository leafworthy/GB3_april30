using UnityEngine;

public class PointerHandler : MonoBehaviour
{
    private Window_QuestPointer questPointer;
    public GameObject target;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        questPointer = GetComponentInChildren<Window_QuestPointer>(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(questPointer.transform.position, target.transform.position) <= 1)
        {
            questPointer.Hide();
        }else
        {
            questPointer.Show(new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z));
        }
    }
}
