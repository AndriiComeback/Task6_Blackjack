using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public static class SaveSystem
{
    public static void SaveGame(List<Card> playerHand, List<Card> dealerHand, List<Card> deck, GameState gameState) {
        var formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/save.iva";
        var stream = new FileStream(path, FileMode.Create);

        var data = new SaveData(playerHand, dealerHand, deck, gameState);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static SaveData LoadData() {
        string path = Application.persistentDataPath + "/save.iva";
        if (File.Exists(path)) {
            var formatter = new BinaryFormatter();
            var stream = new FileStream(path, FileMode.Open);

            SaveData data = formatter.Deserialize(stream) as SaveData;
            stream.Close();

            return data;
        } else {
            return null;
        }
    }
}
