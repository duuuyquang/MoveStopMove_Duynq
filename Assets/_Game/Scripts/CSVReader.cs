using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public static class CSVReader
{
    private const string DEFAULT_PATH = "Assets/_Game/Data/";

    private static List<Dictionary<string,string>> datas = new List<Dictionary<string, string>>();
    private static void Read(string fileName)
    {
        StreamReader streamReader = new StreamReader(DEFAULT_PATH + $"{fileName}.csv");
        bool isEndOfFile = false;
        string[] headLines = streamReader.ReadLine().Split(',');

        while (!isEndOfFile)
        {
            string eachLine = streamReader.ReadLine();
            if(eachLine == null)
            {
                isEndOfFile = true;
                break;
            }

            string[] datasInLine = eachLine.Split(',');

            Dictionary<string, string> temp = new Dictionary<string, string>();
            for (int i = 0; i < datasInLine.Length; i++) {
                temp.Add(headLines[i], datasInLine[i]);
            }
            datas.Add(temp);
        }
    }

    public static List<Dictionary<string, string>> GetData(string fileName)
    {
        Read(fileName);
        return datas;
    }
}
