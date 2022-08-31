using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using UnityEditor;
using UnityEngine;

namespace Assets._Scripts.GameManagement
{
    public static class DataManager
    {

        #region Properties and Backing Fields

        /// <summary>
        /// File path of the save file
        /// </summary>
        public static string FilePath => $"{Application.persistentDataPath}/savefile.json";

        /// <summary>
        /// Max number of high scores
        /// </summary>
        public static int MaxCount => 10;

        private static List<HighScore> _highScores;
        /// <summary>
        /// List of top <see cref="MaxCount"/> high scores
        /// </summary>
        public static List<HighScore> HighScores => _highScores ??= new List<HighScore>();

        #endregion

        #region Methods

        /// <summary>
        /// Load data stored in the save file
        /// </summary>
        public static void LoadData()
        {
            if (!File.Exists(FilePath))
                return;

            var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(FilePath));
            HighScores.AddRange(data.HighScores);
        }

        /// <summary>
        /// Save data to the save file
        /// </summary>
        public static void SaveData()
        {
            var data = new SaveData
            {
                HighScores = HighScores
            };

            File.WriteAllText(FilePath, JsonUtility.ToJson(data));
        }

        #endregion

        /// <summary>
        /// Attempt to add a new high score
        /// </summary>
        /// <param name="highScore"></param>
        /// <returns>true if new high score is in top scores; false if not</returns>
        public static bool AddHighScore(HighScore highScore)
        {
            var added = false;

            for (var i = 0; i < HighScores.Count; i++)
            {
                if (highScore.Points <= HighScores[i].Points) 
                    continue;

                HighScores.Insert(i, highScore);
                added = true;
                break;
            }

            if (!added && HighScores.Count < MaxCount)
            {
                HighScores.Add(highScore);
                added = true;
            }

            // if list is now larger than max, remove last score
            while (HighScores.Count > MaxCount)
            {
                HighScores.RemoveAt(HighScores.Count - 1);
            }

            return added;
        }

        public static bool AddHighScore(string name, int newScore)
        {
            return AddHighScore(new HighScore { Name = name, Points = newScore });
        }

    }

    /// <summary>
    /// Represents object to be saved to json save file
    /// </summary>
    [Serializable]
    public class SaveData
    {
        public List<HighScore> HighScores;
    }

    /// <summary>
    /// Represents a high score entry
    /// </summary>
    [Serializable]
    public class HighScore
    {
        public string Name;
        public int Points;
    }
}