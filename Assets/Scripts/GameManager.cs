using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using static CustomClasses;

public class GameManager : MonoBehaviour
{
    public GameStatus status = GameStatus.MainMenu;

    public string[] buttonsText = {
        "Jouer",
        "Classement",
        "Boutique",
        "Options"
    };

    // Toute ces valeures sont relatives à la taille de l'écran
    private float buttonHeight = 3 / 16f;
    private float buttonSpacing = 1 / 20f;
    private float buttonWidth = 1 / 3f;

    private static GameManager _instance;

    [SerializeField] private GameObject GM;
    private ProceduralGeneration PG;

    public GameObject player;

    private int currGenProgress = 0;
    private int lastUpdateChunkId = -1;

    [SerializeField] private float playerHealthDrainSpeed = 1f;// in health per seconds

    [SerializeField] private bool debug = true;

    private Vector3 playerStartPos;

    public LogInState logInState = LogInState.Disconnected;

    private string username = "_Unregistered_";
    string usernameField="";
    string passwordField="";

    private float buttonCooldown = 3f;
    private bool buttonActive = true;

    private bool showInfo = false;
    private string infoText = "";
    private IEnumerator showInfoInstance;

    List<Score> scores = new List<Score>();

    private float startTime = 0f;

    private Vector2 scrollPosition = Vector2.zero;

    public static GameManager instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("GameManager");
                go.AddComponent<GameManager>();
            }

            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        PG = this.GetComponent<ProceduralGeneration>();
        playerStartPos = player.transform.position;

        showInfoInstance = ShowInfo();
    }

    private void Update()
    {
        int chunkId = GetCurrChunkId();
        if (Input.GetKeyDown(KeyCode.H))
        {
            SpawnGM(Vector3.zero);
        }

        if(chunkId / 5 > currGenProgress)
        {
            currGenProgress++;
            PG.GenerateWorld(5, false);
            SpawnGM(new Vector3(player.transform.position.x + 50, 0, player.transform.position.z));
        }

        if(lastUpdateChunkId < chunkId)
        {
            PG.LoadChunkFromId(chunkId + 5);
            PG.UnloadChunkFromId(chunkId - 6);
            lastUpdateChunkId = chunkId;
        }
        else if (lastUpdateChunkId > chunkId)
        {
            PG.UnloadChunkFromId(chunkId + 6);
            PG.LoadChunkFromId(chunkId - 5);
            lastUpdateChunkId = chunkId;
        }
    }

    private void FixedUpdate()//called every tick which by default is set to 0.02s
    {
        if(status == GameStatus.InGame)
        {
            player.GetComponent<Player>().ChangeHealth(-playerHealthDrainSpeed * Time.fixedDeltaTime * (1+(Time.time - startTime)/100f), true);
        }
    }

    private void OnGUI()
    {
        switch (status)
        {
            case GameStatus.MainMenu:
                // 1/4 * 3/4 de la hauteur de l'ecran pour chaque boutons
                // 1/5 * 1/4 de la hauteur de l'ecran pour chaques espaces
                // 1/3 de la largeur de l'écran pour tout
                for (int i = 0; i < 4; i++)
                {
                    if (GUI.Button(new Rect(Screen.width*buttonWidth, (Screen.height * (buttonHeight*i + buttonSpacing*(i+1))), Screen.width*buttonWidth, Screen.height*buttonHeight), buttonsText[i]))
                    {
                        switch (i)
                        {
                            case 0:
                                //Setup game here
                                StartGame();
                                break;
                            case 1:
                                //display leaderboard
                                StartCoroutine(GetLeaderboard());
                                status = GameStatus.Leaderboard;
                                break;
                            default:

                                break;
                        }
                    }
                }

                //login/register interface
                GUI.Box(new Rect(Screen.width * (2 / 3f + 1 / 32f), 30, Screen.width * (1 / 3f - 2 / 32f), Screen.height - 60), "");

                if(logInState == LogInState.Disconnected)
                {
                    Regex rgx = new Regex("[^a-zA-Z0-9 -]");

                    GUI.Label(new Rect(Screen.width * (2 / 3f + 2 / 32f), (Screen.height * (buttonHeight * 1 + buttonSpacing * (1 + 1))), Screen.width * (1 / 3f - 4 / 32f), Screen.height * buttonHeight / 2f), "Username:");
                    usernameField = GUI.TextField(new Rect(Screen.width * (2 / 3f + 2 / 32f), (Screen.height * (buttonHeight * (1 + 1 / 2f) + buttonSpacing * (1 + 1))), Screen.width * (1 / 3f - 4 / 32f), Screen.height * buttonHeight / 2f), usernameField, 15);
                    usernameField = rgx.Replace(usernameField, "");

                    GUI.Label(new Rect(Screen.width * (2 / 3f + 2 / 32f), (Screen.height * (buttonHeight * 2 + buttonSpacing * (2 + 1))), Screen.width * (1 / 3f - 4 / 32f), Screen.height * buttonHeight / 2f), "Password:");
                    passwordField = GUI.PasswordField(new Rect(Screen.width * (2 / 3f + 2 / 32f), (Screen.height * (buttonHeight * (2 + 1 / 2f) + buttonSpacing * (2 + 1))), Screen.width * (1 / 3f - 4 / 32f), Screen.height * buttonHeight / 2f), passwordField, '*', 15);
                    passwordField = rgx.Replace(passwordField, "");

                    if (buttonActive)
                    {
                        if (GUI.Button(new Rect(Screen.width * (2 / 3f + 2 / 32f), (Screen.height * (buttonHeight * (3 - 1 / 8f) + buttonSpacing * (3 + 1))), Screen.width * (1 / 3f - 4 / 32f), Screen.height * buttonHeight / 2f), "Log In"))
                        {
                            StartCoroutine(LogIn(usernameField, passwordField));
                            StartCoroutine(LockButton());
                        }
                        if (GUI.Button(new Rect(Screen.width * (2 / 3f + 2 / 32f), (Screen.height * (buttonHeight * (3 + 1 / 2f) + buttonSpacing * (3 + 1))), Screen.width * (1 / 3f - 4 / 32f), Screen.height * buttonHeight / 2f), "Register"))
                        {
                            StartCoroutine(Register(usernameField, passwordField));
                            StartCoroutine(LockButton());
                        }
                    }
                    else
                    {
                        GUI.Box(new Rect(Screen.width * (2 / 3f + 2 / 32f), (Screen.height * (buttonHeight * (3 + 1 / 4f) + buttonSpacing * (3 + 1))), Screen.width * (1 / 3f - 4 / 32f), Screen.height * buttonHeight / 2f), "Please wait...");
                    }
                }
                else
                {

                }

                break;
            case GameStatus.InGame:
                GUI.Box(new Rect(10, 10, 300, 75), $"Health: {Mathf.RoundToInt(player.GetComponent<Player>().health)}\nScore:{Mathf.FloorToInt(player.transform.position.x)}");
                break;
            case GameStatus.Pause:

                break;
            case GameStatus.GameDone:
                //Display score
                //Display leaderboard at player's position
                //start another game button
                //try again on same seed button
                //Go to main menu button
                //Mathf.RoundToInt(player.GetComponent<Player>().health)

                GUI.Box(new Rect(Screen.width * buttonWidth, (Screen.height * (buttonHeight * 0 + buttonSpacing * (0 + 1))), Screen.width * buttonWidth, Screen.height * buttonHeight), $"Score:{Mathf.FloorToInt(player.transform.position.x)}");

                    //debug
                if (GUI.Button(new Rect(Screen.width * buttonWidth, (Screen.height * (buttonHeight * 1 + buttonSpacing * (1 + 1))), Screen.width * buttonWidth, Screen.height * buttonHeight), $"Rejouer sur une nouvelle seed"))
                {
                    ResetGame(false);
                }

                if (GUI.Button(new Rect(Screen.width * buttonWidth, (Screen.height * (buttonHeight * 2 + buttonSpacing * (2 + 1))), Screen.width * buttonWidth, Screen.height * buttonHeight), $"Rejouer sur la même seed"))
                {
                    ResetGame(true);
                }

                if (GUI.Button(new Rect(Screen.width * buttonWidth, (Screen.height * (buttonHeight * 3 + buttonSpacing * (3 + 1))), Screen.width * buttonWidth, Screen.height * buttonHeight), $"Retourner au menu"))
                {
                    status = GameStatus.MainMenu;
                }
                break;
            case GameStatus.IsDead:

                break;
            case GameStatus.Leaderboard:

                Rect position = new Rect(Screen.width * buttonWidth, 10, Screen.width * buttonWidth+30, Screen.height - 60);
                scrollPosition = GUI.BeginScrollView(position, scrollPosition, new Rect(0, 0, Screen.width * buttonWidth, (60) * scores.Count));

                for (int i = 0; i < scores.Count; i++)
                {


                    GUI.Box(new Rect(0, ((60 * i)), Screen.width * buttonWidth, 50), $"{(i + 1)}: {scores[i].username} | {scores[i].score} pts | seed:({scores[i].seedX},{scores[i].seedY}) | set seed: {(scores[i].set_seed == 1 ? "yes" : "no")}");
                    
                }

                GUI.EndScrollView();
                if(GUI.Button(new Rect(10, 10, 200, 100), "Menu"))
                {
                    status = GameStatus.MainMenu;
                }

                break;
        }

        //debug
        if (debug)
        {
            GUILayout.Box($"Health: {Mathf.RoundToInt(player.GetComponent<Player>().health * 100) / 100f}" +
                $"\nPosition: {player.transform.position}" +
                $"\nGameStatus: {status.ToString()}" +
                $"\nSeed: {PG.seedX}, {PG.seedY}");
        }

        if (showInfo)
        {
            GUI.Box(new Rect(Screen.width * (0 / 3f + 1 / 32f), 30, Screen.width * (1 / 3f - 2 / 32f), Screen.height/3f-30), infoText);
        }
    }

    private void StartGame()
    {
        Debug.Log($"Initialized world generation at: {Time.time}");

        player.transform.position = playerStartPos;
        currGenProgress = 0;

        PG.ResetWorld();
        player.GetComponent<Rigidbody2D>().WakeUp();
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        player.GetComponent<Player>().health = 100f;

        startTime = Time.time;
        Debug.Log($"Game started at: {startTime}");
        status = GameStatus.InGame;
    }

    private void ResetGame(bool sameSeed = false)
    {
        Debug.Log($"Game was reset at: {Time.time}");

        player.transform.position = playerStartPos;
        currGenProgress = 0;

        PG.ResetWorld(sameSeed);
        player.GetComponent<Rigidbody2D>().WakeUp();
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        player.GetComponent<Player>().health = 100f;

        startTime = Time.time;
        Debug.Log($"Game started at: {startTime}");
        status = GameStatus.InGame;

    }

    public void EndGame()
    {
        Debug.Log($"Game ended at: {Time.time}");

        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

        //sendscore etc....
        StartCoroutine(SendScore(Mathf.FloorToInt(player.transform.position.x)));
        //then
        status = GameStatus.GameDone;
    }

    private void SpawnGM(Vector3 pos)
    {
        GameObject temp = Instantiate(GM, pos, Quaternion.identity);

        Quaternion rot = Quaternion.Euler(player.transform.position - pos);
        temp.transform.rotation = rot;

        temp.GetComponent<GuidedMissile>().StartChase();
    }

    public int GetCurrChunkId()
    {
        return Mathf.RoundToInt((player.transform.position.x - 0.5f + Chunk.width / 2f) / Chunk.width);
    }

    private string secretKey = "DeTouteFaconINTv_C_Overrated890138647460aaa";
    private string addScoreURL = "http://vylax.free.fr/unity/jeu_blaze/sendscore.php";
    private string registerURL = "http://vylax.free.fr/unity/jeu_blaze/register.php";
    private string logInURL = "http://vylax.free.fr/unity/jeu_blaze/connexion.php";
    private string leaderboardURL = "http://vylax.free.fr/unity/jeu_blaze/displayscore.php";

    IEnumerator SendScore(int score)
    {

        string post_url = $"{addScoreURL}?username={username}&score={score}&seedX={PG.seedX}&seedY={PG.seedY}&set_seed={(PG.setSeed? 1 : 0)}&key={secretKey}";

        // Post the URL to the site and create a download object to get the result.
        WWW hs_post = new WWW(post_url);
        yield return hs_post; // Wait until the download is done

        if (!hs_post.text.Contains("Les donn"))
        {
            Debug.LogWarning("There was an error sending score: " + hs_post.text);
        }
        else
        {
            Debug.Log("score sent successfully");
        }
    }

    IEnumerator GetLeaderboard()
    {

        string post_url = $"{leaderboardURL}?key={secretKey}";

        WWW hs_post = new WWW(post_url);
        yield return hs_post; // Wait until the download is done

        if (hs_post.text.Contains("la clef"))
        {
            infoText = "There was an error while loading the leaderboard";
            Notification();
            Debug.LogWarning(infoText);
        }
        else
        {
            Debug.Log("leaderboard loaded successfully");
            scores = new List<Score>();
            //infoText = "";

            string rawData = hs_post.text;
            while (rawData.Length > 0)
            {
                string temp = GetUntilOrEmpty(rawData);
                rawData = rawData.Remove(0, temp.Length+1);
                string[] temp2 = temp.Split(';');
                scores.Add(new Score(temp2[0], int.Parse(temp2[1]), float.Parse(temp2[2]), float.Parse(temp2[3]), int.Parse(temp2[4])));
                /*for (int i = 0; i < temp2.Length; i++)
                {
                    infoText += temp2[i];
                }
                infoText += "\n";*/
            }
            //Notification();
        }
    }

    IEnumerator Register(string username, string password)
    {

        string post_url = $"{registerURL}?username={username}&password={password}";

        // Post the URL to the site and create a download object to get the result.
        WWW hs_post = new WWW(post_url);
        yield return hs_post; // Wait until the download is done

        if (!hs_post.text.Contains("compte"))
        {
            infoText = "There was an error: " + hs_post.text;
            Notification();
            Debug.LogWarning(infoText);
        }
        else
        {
            infoText = "Registered successfully";
            Notification();
            Debug.Log(infoText);
            logInState = LogInState.Disconnected;
        }
    }

    IEnumerator LogIn(string username, string password)
    {

        string post_url = $"{logInURL}?username={username}&password={password}";

        // Post the URL to the site and create a download object to get the result.
        WWW hs_post = new WWW(post_url);
        yield return hs_post; // Wait until the download is done

        if (!hs_post.text.Contains("compte"))
        {
            infoText = "There was an error: " + hs_post.text;
            Notification();
            Debug.LogWarning(infoText);
        }
        else
        {
            infoText = "Logged In successfully";
            Notification();
            Debug.Log(infoText);
            logInState = LogInState.Connected;
            this.username = username;
        }
    }

    private IEnumerator LockButton()
    {
        if (buttonActive)
        {
            buttonActive = false;
            yield return new WaitForSeconds(buttonCooldown);
            buttonActive = true;
        }
        yield return null;
    }

    private void Notification()
    {
        StopCoroutine(showInfoInstance);
        showInfoInstance = ShowInfo();
        StartCoroutine(showInfoInstance);
    }

    private IEnumerator ShowInfo()
    {
        showInfo = true;
        yield return new WaitForSeconds(5f);
        showInfo = false;
    }
}
