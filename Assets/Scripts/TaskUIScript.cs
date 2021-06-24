using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class TaskUIScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameController.onTaskChange += onTaskChange;
        GetComponent<Text>().DOFade(1f, 1f);
    }

    void onTaskChange(string task)
    {
        GetComponent<Text>().text = "Find " + task;
    }
}
