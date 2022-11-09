using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomClasses
{
    public enum GameStatus
    {
        MainMenu,
        InGame,
        Pause,
        IsDead,
        GameDone,
        Leaderboard
    }

    public enum LogInState
    {
        Disconnected,
        Connected
    }

    public class Score
    {
        public string username;
        public int score;
        public float seedX;
        public float seedY;
        public int set_seed;
        public Score(string username, int score, float seedX, float seedY, int set_seed)
        {
            this.username = username;
            this.score = score;
            this.seedX = seedX;
            this.seedY = seedY;
            this.set_seed = set_seed;
        }
    }

    public static GameObject[] chunks = new GameObject[]
    {
        (GameObject)Resources.Load<GameObject>("Chunks/chunk0"),
        (GameObject)Resources.Load<GameObject>("Chunks/chunk1"),
        (GameObject)Resources.Load<GameObject>("Chunks/chunk2")
    };

    public static int currChunkId = -1;
    private static int GenerateNewChunkId()
    {
        currChunkId++;
        return currChunkId;
    }

    public class Chunk
    {
        public int id;
        public int type;
        public GameObject instance;

        private Vector3 offset = new Vector3(0, -4, 0);
        public const int width = 10;

        public float xMin { get; private set; }
        public float xMax { get; private set; }

        public Chunk(int _type, bool load = true)
        {
            type = _type;
            id = GenerateNewChunkId();
            instance = GameObject.Instantiate(
                    chunks[type],
                    offset + Vector3.right * (id * width),
                    Quaternion.identity
                    );
            xMin = width * id - width / 2f + 0.5f;//0.5f = player width
            xMax = xMin + width - 2 * 0.5f;
            instance.name = $"Chunk ID:{id} T:{type} min:{xMin} max:{xMax}";

            if (!load)
                Unload();
        }

        public void Load()
        {
            if (!instance) { return; }
            instance.SetActive(true);
        }

        public void Unload()
        {
            if (!instance) { return; }
            instance.SetActive(false);
        }
    }

    public static string GetUntilOrEmpty(string text, string stopAt = "(")
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            int charLocation = text.IndexOf(stopAt, System.StringComparison.Ordinal);

            if (charLocation > 0)
            {
                return text.Substring(0, charLocation);
            }
        }

        return string.Empty;
    }
}
