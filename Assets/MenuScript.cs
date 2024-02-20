using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuScript : MonoBehaviour
{
    public TMP_InputField widthInputField;
    public TMP_InputField heightInputField;
    void Update()
    {
        SaveSize();
        
    }
    public void SaveSize()
    {
        int width = int.Parse(widthInputField.text);
        int height = int.Parse(heightInputField.text);
        PlayerPrefs.SetInt("Width", width);
        PlayerPrefs.SetInt("Height", height);
    }
}