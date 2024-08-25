using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class CSVImporter
{
    private static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    private static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    private static char[] TRIM_CHARS = { '\"' };

    public static List<Dictionary<string, object>> Read(string csvFile)
    {
        var list = new List<Dictionary<string, object>>();
        TextAsset csvData = Resources.Load(csvFile) as TextAsset;

        var csvLines = Regex.Split(csvData.text, LINE_SPLIT_RE);

        if (1 >= csvLines.Length)
            return list;

        var header = Regex.Split(csvLines[0], SPLIT_RE);

        int n;
        float f;
        string value;
        object finalvalue;

        for (int i = 1; i < csvLines.Length; ++i)
        {
            var csvValues = Regex.Split(csvLines[i], SPLIT_RE);

            if (0 == csvValues.Length)
                continue;

            var entry = new Dictionary<string, object>();

            for(int j = 0; (j < header.Length) && (j < csvValues.Length); ++j)
            {
                value = csvValues[j];

                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");

                finalvalue = value;

                if (int.TryParse(value, out n))
                    finalvalue = n;
                else if (float.TryParse(value, out f))
                    finalvalue = f;

                entry[header[j]] = finalvalue;
            }

            list.Add(entry);
        }

        return list;
    }
}