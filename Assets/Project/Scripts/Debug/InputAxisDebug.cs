using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InputAxisDebug : MonoBehaviour
{
    [SerializeField] private string[] axisNames;

    private Text text;

    private void Start()
    {
        text = GetComponentInChildren<Text>();
    }

    private void Update()
    {
        string s = "";
        for (int i = 0; i < axisNames.Length; i++)
        {
            s += axisNames[i] + ": " + Input.GetAxis(axisNames[i]) + System.Environment.NewLine;
        }
        text.text = s;
    }
}
