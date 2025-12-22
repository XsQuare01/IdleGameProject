using System.Collections.Generic;
using UnityEngine;

public class CSVImporter : MonoBehaviour
{
    public static List<Dictionary<string, object>> exp = new List<Dictionary<string, object>>(CSVReader.Read("EXP"));
}
