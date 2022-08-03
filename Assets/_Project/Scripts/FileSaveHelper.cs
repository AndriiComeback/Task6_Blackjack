using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class FileSaveHelper
{
    private static string _path = $"{Application.dataPath}/Stats.txt";
    public static void SaveStatsToTextFile(int wins, int loses, int ties, int blackjacks, int dealersBlackjacks) {
        CreateFileIfNotExists();
        File.WriteAllText(_path, string.Empty);
        StreamWriter sw = new StreamWriter(_path, true, Encoding.Default);
        sw.WriteLine(wins);
        sw.WriteLine(loses);
        sw.WriteLine(ties);
        sw.WriteLine(blackjacks);
        sw.WriteLine(dealersBlackjacks);
        sw.Close();
    }
    public static List<int> ReadStatsFromTextFile() {
        CreateFileIfNotExists();
        var result = new List<int>();
        using (StreamReader sr = File.OpenText(_path)) {
            string s = "";
            while ((s = sr.ReadLine()) != null) {
                if (int.TryParse(s, out int res)) {
                    result.Add(res);
                }
            }
        }
        return result;
    }
    private static void CreateFileIfNotExists() {
        if (!File.Exists(_path)) {
            using (StreamWriter sw = File.CreateText(_path)) {
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("0");
                sw.WriteLine("0");
            }
        }
    }
}
