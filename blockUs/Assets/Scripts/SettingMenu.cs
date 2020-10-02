using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using System;
using UnityEngine.UI;
using TMPro;

public class SettingMenu : MonoBehaviour
{
    [SerializeField] Slider MouseLookSpeed;
    [SerializeField] TMP_Text TextMouseLookSpeed;

    [SerializeField] Slider MouseLookSpeedScoope;
    [SerializeField] TMP_Text TextLookSpeedScoope;

    private void Start()
    {
        Load();
    }
    private void Update()
    {
        TextMouseLookSpeed.text = MouseLookSpeed.value.ToString();
        TextLookSpeedScoope.text = MouseLookSpeedScoope.value.ToString();
    }

    public void Safe()
    {
        SettingSafeLoad.MouseLookSpeed = MouseLookSpeed.value.ToString();
        SettingSafeLoad.MouseLookSpeedScope = MouseLookSpeedScoope.value.ToString();
        SettingSafeLoad.SafeAsync();
    }
    public void Load()
    {
        SettingSafeLoad.LoadAsync();
        StartCoroutine(LoadS());
    }
    IEnumerator LoadS()
    {
        yield return new WaitForSeconds(0.5f);
        MouseLookSpeed.value =float.Parse(SettingSafeLoad.MouseLookSpeed);
        MouseLookSpeedScoope.value =float.Parse(SettingSafeLoad.MouseLookSpeedScope);
    }
    public void Dest()
    {
        GetComponent<Canvas>().enabled = false;
    }
}

public static class SettingSafeLoad
{
    public static string MouseLookSpeed = "100";
    public static string MouseLookSpeedScope = "100";
    static string writePath = @"SettingCur.setting";

   

   public static async Task SafeAsync()
    {
        File.Delete(writePath);
            using (StreamWriter sw = new StreamWriter(writePath, true, System.Text.Encoding.Default))
            {
            
                await sw.WriteLineAsync(MouseLookSpeed.ToString());
                await sw.WriteLineAsync(MouseLookSpeedScope.ToString());
            }
       
    }
    public static async Task LoadAsync()
    {
        List<string> Lista = new List<string>();
        using (StreamReader sr = new StreamReader(writePath, System.Text.Encoding.Default))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                Lista.Add(line);
            }
            MouseLookSpeed = Lista[0];
            MouseLookSpeedScope = Lista[1];
            Lista.Clear();
        }
    }
}

