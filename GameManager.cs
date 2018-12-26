using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public InputField billetera;
    public InputField contrasenaLogin;
    public InputField contrasenaRegister;
    public InputField coinfirmContrasenaRegister;
    public InputField nickname;
    public GameObject panelBilletera;
    public GameObject panelLogin;
    public GameObject panelRegister;

    public GameObject tomado;
    public GameObject availiable;
    public GameObject errorBilletera;
    public GameObject errorLogin;
    public GameObject errorRegistro;

    public string[] clasificados;
    public GameObject[] clasifTabla;
    public GameObject prefabClasif;
    public GameObject panelClasif;

    public Text posicionText;
    public Text posicionTextMuerto;

    private bool nick;
    private bool billeteraCheck;

    public static GameManager Instance { set; get; }

    private bool skipIntro;

    public int InvenciPower = 0;
    public int GoldenT = 0;

    private AudioSource audiSourc;
    private GameObject player;

    public bool isDead { set; get; }
    private bool isGameStarted = false;
    private bool iniciado = false;
    public static bool Once = false;
    private PlayerMotor motor;
    public Camera camara;
    public GameObject botonPlay, botonTienda, botonInvenci, CanvasWinSomething, tapPanel;

    [Header("Gifts Parameters")]
    public GameObject[] giftArray;
    public string[] giftTextArray;
    [Range(0, 1)]
    public float probabWinSomeThing;
    [Space(30)]

    public float pitch;
    public float pitchTimer;

    public AudioClip BotonMain, BotonTienda, Loop, Main;

    public AudioSource dragonAudio;

    string urlSaveAccount = "https://tronwebapi.azurewebsites.net/api/v1/Cuenta/SaveAccount/token";

    // UI and UI fields
    public Animator gameCanvas, menuAnim, CoinUIAnim, botonAnim, Dragon, CameraAnim, PlayerAnim;
    public Text scoreText, coinText, modifierText, WinSomeThingText;
    private float score, coinScore, modifierScore;
    private int lastScore;

    //Death menu
    public Animator deathMenuAnim;
    public Text deadScoreText, deadCoinText;

    private void Awake()
    {
        if (PlayerPrefs.GetInt("ConBilletera") == 0)
        {
            if (PlayerPrefs.HasKey("Billetera"))
            {
                panelLogin.SetActive(true);
            }
        }
        else
        {
            panelBilletera.SetActive(false);
            StartCoroutine(posicion());
        }
        GoldenT = PlayerPrefs.GetInt("Golden");
        CanvasWinSomething.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player");
        audiSourc = GetComponent<AudioSource>();
        audiSourc.clip = Main;
        audiSourc.Play();
        if (PlayerPrefs.GetInt("Replay") == 1)
        {
            Jugar();
        }
        PlayerPrefs.SetInt("Replay", 0);
        if (!PlayerPrefs.HasKey("MagCooldown"))
        {
            PlayerPrefs.SetInt("MagCooldown", 10);
        }
        if (!PlayerPrefs.HasKey("InvCooldown"))
        {
            PlayerPrefs.SetInt("InvCooldown", 10);
        }
        if (!PlayerPrefs.HasKey("x2Cooldown"))
        {
            PlayerPrefs.SetInt("x2Cooldown", 10);
        }

        Once = false;
        Instance = this;
        modifierScore = 1;

        Invencibilidad.powerInvenci = false;
        Magneto.powerMagneto = false;
        X2.x2 = 1;

        modifierText.text = "x" + modifierScore.ToString("0.0");
        scoreText.text = scoreText.text = score.ToString("0");


        motor = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>();

        botonAnim.SetTrigger("Iniciar");
    }
    private void Update()
    {
        if (pitch > 0)
        {
            pitchTimer -= Time.deltaTime;
            if (pitchTimer <= 0)
            {
                pitch = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            coinScore += 100;
        }

        if (iniciado == true)
        {
            if (MobileInput.Instance.Tap && skipIntro == true)
            {
                CancelInvoke("empezar");
                empezar();
                skipIntro = false;
            }

            if (MobileInput.Instance.Tap && !isGameStarted)
            {
                audiSourc.Stop();
                isGameStarted = true;
                tapPanel.SetActive(false);
                Dragon.SetTrigger("Iniciado");
                CameraAnim.SetTrigger("Iniciado");
                PlayerAnim.SetTrigger("Iniciado");
                dragonAudio.Play();
                Invoke("empezar", 9.5f);
                skipIntro = true;

            }
        }

        if (isGameStarted && !isDead)
        {
            if (player.transform.position.z > 0)
            {
                score = player.transform.position.z;
            }
            if (lastScore != (int)score)
            {
                lastScore = (int)score;
                scoreText.text = score.ToString("0");
            }
        }
    }

    public void GetCoin()
    {
        pitch += 0.1f;
        pitchTimer = 0.7f;
        coinScore += (1 * modifierScore) * X2.x2;
        coinText.text = coinScore.ToString("0");
    }

    public void Jugar()
    {
        botonPlay.GetComponent<AudioSource>().clip = BotonMain;
        botonPlay.GetComponent<AudioSource>().Play();
        iniciado = true;
        menuAnim.SetTrigger("Hide");
        tapPanel.SetActive(true);
        botonAnim.SetTrigger("Esconder");
    }

    public void Invenci()
    {
        if (InvenciPower > 0)
        {
            InvenciPower--;
            PlayerPrefs.SetInt("IntInvencibilidad", InvenciPower);

            if (Invencibilidad.powerInvenci == true)
            {
                PlayerMotor.Instance.slider.value = 0;
                PlayerMotor.Instance.speed -= 10;
                Invencibilidad.powerInvenci = false;
            }
            PlayerMotor.Instance.GetComponent<Animator>().SetTrigger("BigRunning");
            PlayerMotor.Instance.speed += 10;
            Invencibilidad.powerInvenci = true;
            botonInvenci.GetComponent<AudioSource>().clip = PlayerMotor.Instance.InvenciAudio;
            botonInvenci.GetComponent<AudioSource>().Play();
            Pasos.iniciadoPasos = true;
        }
    }

    public void UpdateModifier(float modifierAmount)
    {
        modifierScore += modifierAmount;
        modifierText.text = "x" + modifierScore.ToString("0.0");
    }

    public void HomePause()
    {
        Pausar();
        SceneManager.LoadScene("GameScene");
    }

    public void Home()
    {
        PlayerPrefs.SetInt("Replay", 0);
        SceneManager.LoadScene("GameScene");
    }
    public void Replay()
    {
        float i = Random.Range(0f, 1f);

        if (i <= probabWinSomeThing)
        {
            WinSomeThing();
        }
        else
        {
            PlayerPrefs.SetInt("Replay", 1);
            SceneManager.LoadScene("GameScene");
        }
    }
    public void WinSomeThing()
    {
        CanvasWinSomething.SetActive(true);
        int i = Random.Range(0, 3);
        giftArray[i].SetActive(true);
        WinSomeThingText.text = giftTextArray[i];
        switch (i + 1)
        {
            case 1:
                PlayerPrefs.SetInt("Score", (int)coinScore + 200);
                break;
            case 2:
                PlayerPrefs.SetInt("Golden", (int)GoldenT + 1);
                break;
            case 3:
                PlayerPrefs.SetInt("IntInvencibilidad", InvenciPower + 1);
                break;
        }
    }
    public void ReplayAfterWin()
    {
        PlayerPrefs.SetInt("Replay", 1);
        SceneManager.LoadScene("GameScene");
    }


    public void Pausar()
    {
        if (Time.timeScale == 1)
        {
            Pasos.pararPasos = true;
            Time.timeScale = 0;
        }
        else
        {
            Pasos.iniciadoPasos = true;
            Time.timeScale = 1;
        }
    }

    public void Tienda()
    {
        botonTienda.GetComponent<AudioSource>().clip = BotonTienda;
        botonTienda.GetComponent<AudioSource>().Play();
        SceneManager.LoadScene("Tienda");
    }

    public void OnDeath()
    {
        saveAccount();
        audiSourc.Stop();
        Pasos.pararPasos = true;
        isDead = true;
        FindObjectOfType<GlacierSpawner>().IsScrolling = false;
        deathMenuAnim.SetTrigger("Dead");
        gameCanvas.SetTrigger("Hide");

        
        StartCoroutine(SlidingNumbers());

        PlayerPrefs.SetInt("Score", (int)coinScore);

        if (score > PlayerPrefs.GetInt("Hiscore"))
        {
            float s = score;

            if (s % 1 == 0)
            {
                s += 1;
            }
            PlayerPrefs.SetInt("Hiscore", (int)s);
        }
        StartCoroutine(posicion());
        PlayerPrefs.SetInt("ConBilletera", 1);
    }

    private IEnumerator SlidingNumbers()
    {
        yield return new WaitForSeconds(1.5f);
        camara.GetComponent<SlidingNumber>().AddToNumber(score);
        camara.GetComponent<SlidingNumber>().AddToNumber2(coinScore);

        StartCoroutine(posicion());
    }

    public void empezar()
    {
        camara.GetComponent<Animator>().enabled = false;
        dragonAudio.Stop();
        audiSourc.clip = Loop;
        audiSourc.Play();
        Once = true;

        motor.StartRunning();
        FindObjectOfType<CamaraMotor>().IsMoving = true;
        gameCanvas.SetTrigger("Show");
        coinText.text = coinScore.ToString("0");

        InvenciPower = PlayerPrefs.GetInt("IntInvencibilidad");
    }

    public void InsertarBilletera()
    {
        StartCoroutine(enviarBilletera());
    }

    public void Login()
    {
        if(PlayerPrefs.HasKey("Contraseña" + PlayerPrefs.GetString("Billetera")))
        {
            if (contrasenaLogin.text == PlayerPrefs.GetString("Contraseña" + PlayerPrefs.GetString("Billetera")))
            {
                panelBilletera.SetActive(false);
                panelLogin.SetActive(false);
                PlayerPrefs.SetInt("ConBilletera", 1);
                StartCoroutine(posicion());
            }
            else
            {
                errorLogin.SetActive(true);
            }
        }
        else
        {
            errorLogin.SetActive(true);
        }
    }

    public void Register()
    {
        if (string.Equals(contrasenaRegister.text, coinfirmContrasenaRegister.text) && nickname.text != "")
        {
            StartCoroutine(nicknameGet());
        }
        else
        {
            errorRegistro.SetActive(true);
        }
    }

    IEnumerator nicknameGet()
    {
        WWW itemsData = new WWW("https://tronwebapi.azurewebsites.net/api/v1/Cuenta/ExistNickName?nickName=" + nickname.text);
        PlayerPrefs.SetString("Nick", nickname.text);
        yield return itemsData;
        string itemsDataString = itemsData.text;
        if(itemsDataString == "false")
        {
            nick = false;
        }
        else
        {
            nick = true;
        }

        if (nick == false)
        {
            availiable.SetActive(true);
            tomado.SetActive(false);
            PlayerPrefs.SetString("Contraseña" + billetera.text, contrasenaRegister.text);
            panelBilletera.SetActive(false);
            panelRegister.SetActive(false);
            PlayerPrefs.SetInt("ConBilletera", 1);
            StartCoroutine(posicion());
        }
        else
        {
            availiable.SetActive(false);
            tomado.SetActive(true);
        }
        print(itemsDataString);
    }

    IEnumerator enviarBilletera()
    {
        WWW itemsData = new WWW("https://tronwebapi.azurewebsites.net/api/v1/Cuenta/GetAccount?address=" + billetera.text);
        yield return itemsData;
        string itemsDataString = itemsData.text;
        print(itemsDataString);
        if (itemsDataString.Length > 40)
        {
            errorBilletera.SetActive(false);

            if (PlayerPrefs.HasKey("Billetera"))
            {
                billetera.interactable = false;
                panelLogin.SetActive(true);
            }
            else
            {
                PlayerPrefs.SetString("Billetera", billetera.text);
                billetera.interactable = false;
                panelRegister.SetActive(true);
            }
        }
        else
        {
            errorBilletera.SetActive(true);
        }
    }

    public void saveAccount()
    {
        WWWForm form = new WWWForm();
        form.AddField("CUE_NickName", PlayerPrefs.GetString("Nick"));
        form.AddField("CUE_ClavePublica", PlayerPrefs.GetString("Billetera"));
        form.AddField("CUE_Puntos", coinScore.ToString("0"));

        WWW www = new WWW(urlSaveAccount, form);
    }

    public void OnApplicationPause(bool pause)
    {
        PlayerPrefs.SetInt("ConBilletera", 0);
    }

    public void OnApplicationFocus(bool focus)
    {
        PlayerPrefs.SetInt("ConBilletera", 0);
    }

    public void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("ConBilletera", 0);
    }

    public void enviarClasif()
    {
        StartCoroutine(clasificacion());
    }

    IEnumerator clasificacion()
    {
        WWW itemsData = new WWW("https://tronwebapi.azurewebsites.net/api/v1/Cuenta/GetClasification?numeroMaximoRegistros=20");
        yield return itemsData;
        string itemsDataString = itemsData.text;
        print(itemsDataString);
        clasificados = itemsDataString.Split(',');
        print(clasificados.Length);
        for (int i = 0; i < (clasificados.Length/6); i++)
        {
            GameObject go = Instantiate (prefabClasif, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            go.transform.SetParent(panelClasif.transform, false);

            go.transform.GetChild(0).GetComponent<Text>().text = (i+1).ToString();
            go.transform.GetChild(1).GetComponent<Text>().text = clasificados[(i * 6) + 1].Substring(16, clasificados[(i * 6) + 1].Length - 17);
            go.transform.GetChild(2).GetComponent<Text>().text = clasificados[(i * 6) + 3].Substring(13, clasificados[(i * 6) + 3].Length - 13);
            resize.Instance.Resz();
        }
    }

    string GetDataValue(string data, string index)
    {
        string value = data.Substring(data.IndexOf(index) + index.Length);
        if (value.Contains("|")) value = value.Remove(value.IndexOf("|"));
        return value;
    }

    IEnumerator posicion()
    {
        WWW itemsData = new WWW("https://tronwebapi.azurewebsites.net/api/v1/Cuenta/GetUserPositionPoints?address=" + PlayerPrefs.GetString("Billetera"));
        yield return itemsData;
        string itemsDataString = itemsData.text;
        print(itemsDataString);
        clasificados = itemsDataString.Split(',');
        print(clasificados.Length);

        posicionText.text = "" + clasificados[0].Substring(10, clasificados[0].Length - 10);
        posicionTextMuerto.text = "" + clasificados[0].Substring(10, clasificados[0].Length - 10);
    }

    public void backClasif()
    {
        foreach (Transform child in panelClasif.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
