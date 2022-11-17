using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

public class CSVParser{
    public static string[] ConvertCsv(string raw)
    {
        var a = raw.Split('\n');
        
        return a;
    }
}