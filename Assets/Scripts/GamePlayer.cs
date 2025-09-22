using System.Linq;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GamePlayer : MonoBehaviour
{
    public static GamePlayer instance;
    private readonly float spawnRate = 1f;
    private float nextSpawnTime;
    private BigInteger score;
    private BigInteger attempts;
    private BigInteger highscore;
    private BigInteger totalNormalBerries;
    private BigInteger totalPoisonBerries;
    private BigInteger totalSlowBerries;
    private BigInteger totalUltraBerries;
    private BigInteger totalSpeedyBerries;
    private BigInteger totalTimeSlowBerries;
    private BigInteger totalPurpleBerries;
    private BigInteger totalTimeFreezeBerries;
    private BigInteger totalEvilBerries;
    private BigInteger totalAttempts;
    private float boostLeft;
    private float timeslowLeft;
    private float timefreezeLeft;
    private float slownessLeft;
    private float speedyLeft;
    private float screenWidth;
    private bool isGrounded;
    public TMP_Text scoreText;
    public TMP_Text highScoreText;
    public TMP_Text boostText;
    public GameObject bird;
    public GameObject pausePanel;
    public Rigidbody2D rb;
    public AudioSource backgroundMusic;
    public TMP_Text fpsCounter;
    private float nextUpdate;
    private float fps;
    public SpriteRenderer overlayRender;
    private float lastMoveTime;
    public GameObject berryParent;

    public GameObject mobileButtons;
    public Button pauseButton;
    public Button restartButton;
    public Button jumpButton;
    public Button rightButton;
    public Button leftButton;

    void Start()
    {
        var backgroundColor = BazookaManager.Instance.GetColorSettingBackground();
        Camera.main.backgroundColor = new Color(
            int.Parse(backgroundColor[0].ToString()) / 255f,
            int.Parse(backgroundColor[1].ToString()) / 255f,
            int.Parse(backgroundColor[2].ToString()) / 255f
        );

        SpriteRenderer component = bird.GetComponent<SpriteRenderer>();
        var birdColor = BazookaManager.Instance.GetColorSettingIcon();
        var overlayColor = BazookaManager.Instance.GetColorSettingOverlay();
        bird.GetComponent<SpriteRenderer>().color = new Color(
            int.Parse(birdColor[0].ToString()) / 255f,
            int.Parse(birdColor[1].ToString()) / 255f,
            int.Parse(birdColor[2].ToString()) / 255f
        );
        bird.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(
            int.Parse(overlayColor[0].ToString()) / 255f,
            int.Parse(overlayColor[1].ToString()) / 255f,
            int.Parse(overlayColor[2].ToString()) / 255f
        );

        int num = BazookaManager.Instance.GetBirdIcon();
        int num2 = BazookaManager.Instance.GetBirdOverlay();
        if (num == 1)
        {
            component.sprite = Tools.GetIconForUser(BazookaManager.Instance.GetAccountID() ?? 0);
        }
        else
        {
            component.sprite = Resources.Load<Sprite>("Icons/Icons/bird_" + num);
        }
        if (num2 == 8)
        {
            overlayRender.sprite = Resources.Load<Sprite>("Icons/Overlays/overlay_" + num2);
            overlayRender.transform.localPosition = new UnityEngine.Vector3(-0.37f, 0.32f, 0f);
        }
        else if (num2 == 11)
        {
            overlayRender.sprite = Resources.Load<Sprite>("Icons/Overlays/overlay_" + num2);
            overlayRender.transform.localScale = new UnityEngine.Vector3(1.1f, 1.1f, 1.1f); //yea i didnt feel like doing it for all lmao
            overlayRender.transform.localPosition = new UnityEngine.Vector3(-0.3141809f, 0.4324968f, 0f);
        }
        else if (num2 == 13)
        {
            overlayRender.sprite = Resources.Load<Sprite>("Icons/Overlays/overlay_" + num2);
            overlayRender.transform.localPosition = new UnityEngine.Vector3(-0.3559977f, 0.3179995f, 0f);
        }
        else
        {
            overlayRender.sprite = Resources.Load<Sprite>("Icons/Overlays/overlay_" + num2);
        }
        if (component.sprite == null)
        {
            component.sprite = Resources.Load<Sprite>("Icons/Icons/bird_1");
            BazookaManager.Instance.SetBirdIcon(1);
        }
        if (overlayRender.sprite == null && num2 != 0)
        {
            overlayRender.sprite = Resources.Load<Sprite>("Icons/Overlays/overlay_1");
            BazookaManager.Instance.SetBirdOverlay(1);
        }

        lastMoveTime = Time.time;
        UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Enable();
        instance = this;
        highscore = BazookaManager.Instance.GetGameStoreHighScore();
        totalNormalBerries = BazookaManager.Instance.GetGameStoreTotalNormalBerries();
        totalPoisonBerries = BazookaManager.Instance.GetGameStoreTotalPoisonBerries();
        totalSlowBerries = BazookaManager.Instance.GetGameStoreTotalSlowBerries();
        totalUltraBerries = BazookaManager.Instance.GetGameStoreTotalUltraBerries();
        totalSpeedyBerries = BazookaManager.Instance.GetGameStoreTotalSpeedyBerries();
        totalTimeSlowBerries = BazookaManager.Instance.GetGameStoreTotalTimeSlowBerries();
        totalAttempts = BazookaManager.Instance.GetGameStoreTotalAttepts();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        backgroundMusic.volume = BazookaManager.Instance.GetSettingMusicVolume();
        screenWidth = Camera.main.orthographicSize * 2f * Camera.main.aspect;
        if (Application.isMobilePlatform) mobileButtons.SetActive(true);
        UpdateStats(0, 1);
    }

    void MoveBird()
    {
        float screenWidth = Camera.main.orthographicSize * 2f * Camera.main.aspect;
        float baseSpeed = 0.18f * (screenWidth / 20.19257f);
        bool doMoveRight = false;
        bool doMoveLeft = false;
        bool doJump = false;
        bool doRestart = false;
        bool doBack = false;
        float movespeed = baseSpeed;
        if (boostLeft > 0f || speedyLeft > 0f)
        {
            movespeed = baseSpeed * 1.39f;
        }
        else if (slownessLeft > 0f)
        {
            movespeed = baseSpeed * 0.56f;
        }
        CheckIfGrounded();
        bool controllerLeft = Gamepad.current != null && (Gamepad.current.leftStick.left.isPressed || Gamepad.current.dpad.left.isPressed || Gamepad.current.rightStick.left.isPressed);
        bool controllerRight = Gamepad.current != null && (Gamepad.current.leftStick.right.isPressed || Gamepad.current.dpad.right.isPressed || Gamepad.current.rightStick.right.isPressed);
        bool controllerJump = Gamepad.current != null && (Gamepad.current.leftStick.up.isPressed || Gamepad.current.leftStick.down.isPressed || Gamepad.current.dpad.up.isPressed || Gamepad.current.dpad.down.isPressed || Gamepad.current.rightStick.up.isPressed || Gamepad.current.rightStick.down.isPressed);
        if (!Application.isMobilePlatform)
        {
            if (controllerLeft || Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed || Keyboard.current.jKey.isPressed)
            {
                doMoveLeft = true;
            }
            if (controllerRight || Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed || Keyboard.current.lKey.isPressed)
            {
                doMoveRight = true;
            }
            if (controllerJump || Keyboard.current.spaceKey.isPressed || Keyboard.current.upArrowKey.isPressed || Keyboard.current.wKey.isPressed || Keyboard.current.downArrowKey.isPressed || Keyboard.current.sKey.isPressed || Keyboard.current.kKey.isPressed || Keyboard.current.iKey.isPressed || Mouse.current.leftButton.isPressed || (Gamepad.current != null && Gamepad.current.buttonSouth.isPressed))
            {
                doJump = true;
            }
            if (Keyboard.current.rKey.isPressed)
            {
                doRestart = true;
            }
        }
        else
        {
            var touches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;
            for (int i = 0; i < touches.Count; i++)
            {
                var pos = touches[i].screenPosition;
                UnityEngine.Vector3 clickPosition = Camera.main.ScreenToWorldPoint(new UnityEngine.Vector3(pos.x, pos.y, 0f));
                clickPosition.z = 0f;
                if (leftButton.GetComponent<HoldableButton>().isPressed) doMoveLeft = true;
                if (rightButton.GetComponent<HoldableButton>().isPressed) doMoveRight = true;
                if (jumpButton.GetComponent<HoldableButton>().isPressed) doJump = true;
                if (restartButton.GetComponent<HoldableButton>().isPressed) doRestart = true;
                if (pauseButton.GetComponent<HoldableButton>().isPressed) doBack = true;
            }
        }
        if (doMoveLeft && !doMoveRight)
        {
            lastMoveTime = Time.time;
            bird.transform.position += new UnityEngine.Vector3(-movespeed, 0f, 0f);
            ClampPosition(bird);
            bird.transform.localScale = new UnityEngine.Vector3(1.35f, 1.35f, 1.35f);
        }
        if (doMoveRight && !doMoveLeft)
        {
            lastMoveTime = Time.time;
            bird.transform.position += new UnityEngine.Vector3(movespeed, 0f, 0f);
            ClampPosition(bird);
            bird.transform.localScale = new UnityEngine.Vector3(-1.35f, 1.35f, 1.35f);
        }
        if (doJump && isGrounded)
        {
            lastMoveTime = Time.time;
            AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Jump"), Camera.main.transform.position, 0.75f * BazookaManager.Instance.GetSettingSFXVolume());
            if (boostLeft > 0f || speedyLeft > 0f)
            {
                rb.linearVelocity = UnityEngine.Vector2.up * 12f;
            }
            else if (slownessLeft > 0f)
            {
                rb.linearVelocity = UnityEngine.Vector2.up * 6f;
            }
            else
            {
                rb.linearVelocity = UnityEngine.Vector2.up * 9f;
            }
        }
        if (doBack)
        {
            TogglePause();
        }
        if (doRestart)
        {
            if (score != 0) Respawn();
        }
    }

    void ClampPosition(GameObject obj)
    {
        var cam = Camera.main;
        var pos = obj.transform.position;
        var bounds = obj.GetComponent<Renderer>().bounds.extents;

        float zDist = Mathf.Abs(cam.transform.position.z - pos.z);

        UnityEngine.Vector3 min = cam.ViewportToWorldPoint(new UnityEngine.Vector3(0, 0, zDist));
        UnityEngine.Vector3 max = cam.ViewportToWorldPoint(new UnityEngine.Vector3(1, 1, zDist));

        pos.x = Mathf.Clamp(pos.x, min.x + bounds.x, max.x - bounds.x);
        pos.y = Mathf.Clamp(pos.y, min.y + bounds.y, max.y - bounds.y);

        obj.transform.position = pos;
    }

    void FixedUpdate()
    {
        SpawnBerries();
        if (!pausePanel.activeSelf)
        {
            MoveBird();
            if (boostLeft > 0f)
            {
                boostLeft -= Time.deltaTime;
                boostText.text = "Boost expires in " + string.Format("{0:0.0}", boostLeft) + "s";
            }
            else if (slownessLeft > 0f)
            {
                slownessLeft -= Time.deltaTime;
                boostText.text = "Slowness expires in " + string.Format("{0:0.0}", slownessLeft) + "s";
            }
            else if (speedyLeft > 0f)
            {
                speedyLeft -= Time.deltaTime;
                boostText.text = "Speed expires in " + string.Format("{0:0.0}", speedyLeft) + "s";
            }
            else if (timeslowLeft > 0f)
            {
                timeslowLeft -= Time.deltaTime;
                boostText.text = "Time Slow expires in " + string.Format("{0:0.0}", timeslowLeft) + "s";
            }
            else if (timefreezeLeft > 0f)
            {
                timefreezeLeft -= Time.deltaTime;
                boostText.text = "Time Freeze expires in " + string.Format("{0:0.0}", timefreezeLeft) + "s";
            }
            else
            {
                boostText.text = "";
            }
        }
    }

    void SpawnBerries()
    {
        if (Time.time < nextSpawnTime)
        {
            return;
        }
        if (speedyLeft > 0)
        {
            nextSpawnTime = Time.time + 1f / (spawnRate * 1.875f);
        }
        else
        {
            nextSpawnTime = Time.time + 1f / spawnRate;
        }
        float spawnProbability = Random.value;
        if ((int)timefreezeLeft != 0) return;
        if (!pausePanel.activeSelf)
        {
            GameObject newBerry = new("Berry");
            newBerry.transform.SetParent(berryParent.transform);
            SpriteRenderer spriteRenderer = newBerry.AddComponent<SpriteRenderer>();

            if (spawnProbability <= 0.35f)
            {
                spriteRenderer.sprite = Resources.Load<Sprite>("Berries/Berry");
                newBerry.tag = "NormalBerry";
            }
            else if (spawnProbability <= 0.50f)
            {
                spriteRenderer.sprite = Resources.Load<Sprite>("Berries/PoisonBerry");
                newBerry.tag = "PoisonBerry";
            }
            else if (spawnProbability <= 0.60f)
            {
                spriteRenderer.sprite = Resources.Load<Sprite>("Berries/SlowBerry");
                newBerry.tag = "SlowBerry";
            }
            else if (spawnProbability <= 0.70f)
            {
                spriteRenderer.sprite = Resources.Load<Sprite>("Berries/UltraBerry");
                newBerry.tag = "UltraBerry";
            }
            else if (spawnProbability <= 0.80f)
            {
                spriteRenderer.sprite = Resources.Load<Sprite>("Berries/SpeedyBerry");
                newBerry.tag = "SpeedyBerry";
            }
            else if (spawnProbability <= 0.85f)
            {
                spriteRenderer.sprite = Resources.Load<Sprite>("Berries/PurpleBerry");
                newBerry.tag = "PurpleBerry";
            }
            else if (spawnProbability <= 0.90f)
            {
                spriteRenderer.sprite = Resources.Load<Sprite>("Berries/TimeSlowBerry");
                newBerry.tag = "TimeSlowBerry";
            }
            else if (spawnProbability <= 0.95f)
            {
                spriteRenderer.sprite = Resources.Load<Sprite>("Berries/EvilBerry");
                newBerry.tag = "EvilBerry";
            }
            else
            {
                spriteRenderer.sprite = Resources.Load<Sprite>("Berries/TimeFreezeBerry");
                newBerry.tag = "TimeFreezeBerry";
            }


            spriteRenderer.sortingOrder = -5;

            float screenWidth = Camera.main.orthographicSize * 2 * Camera.main.aspect;
            float spawnPositionX = Random.Range(-screenWidth / 2.17f, screenWidth / 2.17f);
            newBerry.transform.position = new UnityEngine.Vector3(spawnPositionX, Camera.main.orthographicSize + 1f, 0f);

            Rigidbody2D rb = newBerry.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.linearVelocity = new UnityEngine.Vector2(0f, -4f);
        }
    }

    void Update()
    {
        if (BazookaManager.Instance.GetSettingShowFPS() && Time.time > nextUpdate)
        {
            fps = 1f / Time.deltaTime;
            fpsCounter.text = "FPS: " + Mathf.Round(fps);
            nextUpdate = Time.time + 0.25f;
        }
        if (screenWidth != Camera.main.orthographicSize * 2f * Camera.main.aspect)
        {
            screenWidth = Camera.main.orthographicSize * 2f * Camera.main.aspect;
            ClampPosition(bird);
        }
        GameObject[] normalBerries = GameObject.FindGameObjectsWithTag("NormalBerry");
        GameObject[] poisonBerries = GameObject.FindGameObjectsWithTag("PoisonBerry");
        GameObject[] slowBerries = GameObject.FindGameObjectsWithTag("SlowBerry");
        GameObject[] ultraBerries = GameObject.FindGameObjectsWithTag("UltraBerry");
        GameObject[] speedyBerries = GameObject.FindGameObjectsWithTag("SpeedyBerry");
        GameObject[] timeslowBerries = GameObject.FindGameObjectsWithTag("TimeSlowBerry");
        GameObject[] purpleBerries = GameObject.FindGameObjectsWithTag("PurpleBerry");
        GameObject[] timefreezeBerries = GameObject.FindGameObjectsWithTag("TimeFreezeBerry");
        GameObject[] evilBerries = GameObject.FindGameObjectsWithTag("EvilBerry");

        if (!pausePanel.activeSelf)
        {
            if (Time.time - lastMoveTime > 20)
            {
                lastMoveTime = float.MaxValue;
                EnablePause();
            }
            CheckIfGrounded();
            foreach (GameObject normalBerry in normalBerries)
            {
                if (normalBerry.transform.position.y < 0f - Camera.main.orthographicSize - 1f)
                {
                    Destroy(normalBerry);
                }
                else if (UnityEngine.Vector3.Distance(bird.transform.position, normalBerry.transform.position) < 1.5f)
                {
                    AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Eat"), Camera.main.transform.position, 1.2f * BazookaManager.Instance.GetSettingSFXVolume());
                    Destroy(normalBerry);
                    totalNormalBerries++;
                    UpdateStats(1, 0);
                }

                if (timefreezeLeft > 0)
                {
                    normalBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, 0f);
                }
                else if (speedyLeft > 0)
                {
                    normalBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -7.5f);
                }
                else if (timeslowLeft > 0)
                {
                    normalBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -2f);
                }
                else
                {
                    normalBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -4f);
                }
            }

            foreach (GameObject poisonBerry in poisonBerries)
            {
                if (poisonBerry.transform.position.y < 0f - Camera.main.orthographicSize - 1f)
                {
                    Destroy(poisonBerry);
                }
                else if (UnityEngine.Vector3.Distance(bird.transform.position, poisonBerry.transform.position) < 1.5f)
                {
                    AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Death"), Camera.main.transform.position, 1.2f * BazookaManager.Instance.GetSettingSFXVolume());
                    Respawn();
                    totalPoisonBerries++;
                    UpdateStats(0, 0);
                }

                if (timefreezeLeft > 0)
                {
                    poisonBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, 0f);
                }
                else if (speedyLeft > 0)
                {
                    poisonBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -7.5f);
                }
                else if (timeslowLeft > 0)
                {
                    poisonBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -2f);
                }
                else
                {
                    poisonBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -4f);
                }
            }

            foreach (GameObject slowBerry in slowBerries)
            {
                if (slowBerry.transform.position.y < 0f - Camera.main.orthographicSize - 1f)
                {
                    Destroy(slowBerry);
                }
                else if (UnityEngine.Vector3.Distance(bird.transform.position, slowBerry.transform.position) < 1.5f)
                {
                    AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Downgrade"), Camera.main.transform.position, 0.35f * BazookaManager.Instance.GetSettingSFXVolume());
                    Destroy(slowBerry);
                    boostLeft = 0f;
                    slownessLeft = 10f;
                    speedyLeft = 0f;
                    totalSlowBerries++;
                    if (score > 0)
                    {
                        UpdateStats(-1, 0);
                    }
                }

                if (timefreezeLeft > 0)
                {
                    slowBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, 0f);
                }
                else if (speedyLeft > 0)
                {
                    slowBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -7.5f);
                }
                else if (timeslowLeft > 0)
                {
                    slowBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -2f);
                }
                else
                {
                    slowBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -4f);
                }
            }

            foreach (GameObject ultraBerry in ultraBerries)
            {
                if (ultraBerry.transform.position.y < 0f - Camera.main.orthographicSize - 1f)
                {
                    Destroy(ultraBerry);
                }
                else if (UnityEngine.Vector3.Distance(bird.transform.position, ultraBerry.transform.position) < 1.5f)
                {
                    AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Powerup"), Camera.main.transform.position, 0.35f * BazookaManager.Instance.GetSettingSFXVolume());
                    Destroy(ultraBerry);
                    totalUltraBerries++;
                    timeslowLeft = 0f;
                    speedyLeft = 0f;
                    if (slownessLeft > 0f)
                    {
                        slownessLeft = 0f;
                        UpdateStats(1, 0);
                    }
                    else
                    {
                        boostLeft += 10f;
                        UpdateStats(5, 0);
                    }
                }

                if (timefreezeLeft > 0)
                {
                    ultraBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, 0f);
                }
                else if (speedyLeft > 0)
                {
                    ultraBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -7.5f);
                }
                else if (timeslowLeft > 0)
                {
                    ultraBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -2f);
                }
                else
                {
                    ultraBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -4f);
                }
            }

            foreach (GameObject speedyBerry in speedyBerries)
            {
                if (speedyBerry.transform.position.y < 0f - Camera.main.orthographicSize - 1f)
                {
                    Destroy(speedyBerry);
                }
                else if (UnityEngine.Vector3.Distance(bird.transform.position, speedyBerry.transform.position) < 1.5f)
                {
                    AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/SpeedyPowerup"), Camera.main.transform.position, 0.35f * BazookaManager.Instance.GetSettingSFXVolume());
                    Destroy(speedyBerry);
                    boostLeft = 0f;
                    slownessLeft = 0f;
                    timeslowLeft = 0f;
                    speedyLeft = 10f;
                    totalSpeedyBerries++;
                    UpdateStats(10, 0);
                }

                if (timefreezeLeft > 0)
                {
                    speedyBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, 0f);
                }
                else if (speedyLeft > 0)
                {
                    speedyBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -7.5f);
                }
                else if (timeslowLeft > 0)
                {
                    speedyBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -2f);
                }
                else
                {
                    speedyBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -4f);
                }
            }

            foreach (GameObject timeslowBerry in timeslowBerries)
            {
                if (timeslowBerry.transform.position.y < 0f - Camera.main.orthographicSize - 1f)
                {
                    Destroy(timeslowBerry);
                }
                else if (UnityEngine.Vector3.Distance(bird.transform.position, timeslowBerry.transform.position) < 1.5f)
                {
                    AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Eat"), Camera.main.transform.position, 1.2f * BazookaManager.Instance.GetSettingSFXVolume());
                    Destroy(timeslowBerry);
                    boostLeft = 0f;
                    speedyLeft = 0f;
                    timeslowLeft = 10f;
                    totalTimeSlowBerries++;
                    UpdateStats(1, 0);
                }

                if (timefreezeLeft > 0)
                {
                    timeslowBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, 0f);
                }
                else if (speedyLeft > 0)
                {
                    timeslowBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -7.5f);
                }
                else if (timeslowLeft > 0)
                {
                    timeslowBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -2f);
                }
                else
                {
                    timeslowBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -4f);
                }
            }

            foreach (GameObject purpleBerry in purpleBerries)
            {
                if (purpleBerry.transform.position.y < 0f - Camera.main.orthographicSize - 1f)
                {
                    Destroy(purpleBerry);
                }
                else if (UnityEngine.Vector3.Distance(bird.transform.position, purpleBerry.transform.position) < 1.5f)
                {
                    AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Eat"), Camera.main.transform.position, 1.2f * BazookaManager.Instance.GetSettingSFXVolume());
                    Destroy(purpleBerry);
                    totalPurpleBerries++;
                    UpdateStats(15, 0);
                }

                if (timefreezeLeft > 0)
                {
                    purpleBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, 0f);
                }
                else if (speedyLeft > 0)
                {
                    purpleBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -7.5f);
                }
                else if (timeslowLeft > 0)
                {
                    purpleBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -2f);
                }
                else
                {
                    purpleBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -4f);
                }
            }

            foreach (GameObject timefreezeBerry in timefreezeBerries)
            {
                if (timefreezeBerry.transform.position.y < 0f - Camera.main.orthographicSize - 1f)
                {
                    Destroy(timefreezeBerry);
                }
                else if (UnityEngine.Vector3.Distance(bird.transform.position, timefreezeBerry.transform.position) < 1.5f)
                {
                    AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Eat"), Camera.main.transform.position, 1.2f * BazookaManager.Instance.GetSettingSFXVolume());
                    Destroy(timefreezeBerry);
                    boostLeft = 0f;
                    speedyLeft = 0f;
                    timeslowLeft = 0f;
                    timefreezeLeft = 5f;
                    totalTimeFreezeBerries++;
                }

                if (timefreezeLeft > 0)
                {
                    timefreezeBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, 0f);
                }
                else if (speedyLeft > 0)
                {
                    timefreezeBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -7.5f);
                }
                else if (timeslowLeft > 0)
                {
                    timefreezeBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -2f);
                }
                else
                {
                    timefreezeBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -4f);
                }
            }
            foreach (GameObject evilBerry in evilBerries)
            {
                if (evilBerry.transform.position.y < 0f - Camera.main.orthographicSize - 1f)
                {
                    Destroy(evilBerry);
                }
                else if (UnityEngine.Vector3.Distance(bird.transform.position, evilBerry.transform.position) < 1.5f)
                {
                    AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Eat"), Camera.main.transform.position, 1.2f * BazookaManager.Instance.GetSettingSFXVolume());
                    Destroy(evilBerry);
                    totalEvilBerries++;
                    if (score > 9)
                    {
                        UpdateStats(-10, 0);
                    }
                }

                if (timefreezeLeft > 0)
                {
                    evilBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, 0f);
                }
                else if (speedyLeft > 0)
                {
                    evilBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -7.5f);
                }
                else if (timeslowLeft > 0)
                {
                    evilBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -2f);
                }
                else
                {
                    evilBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -4f);
                }
            }

        }

        else
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = UnityEngine.Vector2.zero;
            GameObject[] allberries = normalBerries
                .Concat(poisonBerries)
                .Concat(slowBerries)
                .Concat(ultraBerries)
                .Concat(speedyBerries)
                .Concat(timeslowBerries)
                .Concat(purpleBerries)
                .Concat(timefreezeBerries)
                .Concat(evilBerries)
                .ToArray();
            foreach (GameObject berry in allberries)
            {
                berry.GetComponent<Rigidbody2D>().linearVelocity = UnityEngine.Vector2.zero;
            }
        }
        if ((Application.platform == RuntimePlatform.Android && Keyboard.current.escapeKey.wasPressedThisFrame) || !Application.isMobilePlatform && (Keyboard.current.escapeKey.wasPressedThisFrame || (Gamepad.current != null && (Gamepad.current.startButton.wasPressedThisFrame || Gamepad.current.buttonEast.wasPressedThisFrame)))) TogglePause();
    }

    void Respawn()
    {
        bird.transform.position = new UnityEngine.Vector3(0f, -4.3f, 0f);
        bird.transform.localScale = new UnityEngine.Vector3(1.35f, 1.35f, 1.35f);
        rb.gravityScale = 0f;
        rb.linearVelocity = UnityEngine.Vector2.zero;
        score = 0;
        boostLeft = 0f;
        timeslowLeft = 0f;
        timefreezeLeft = 0f;
        slownessLeft = 0f;
        speedyLeft = 0f;
        UpdateStats(0, 1);

        GameObject[] allberries = GameObject.FindGameObjectsWithTag("NormalBerry")
            .Concat(GameObject.FindGameObjectsWithTag("PoisonBerry"))
            .Concat(GameObject.FindGameObjectsWithTag("SlowBerry"))
            .Concat(GameObject.FindGameObjectsWithTag("UltraBerry"))
            .Concat(GameObject.FindGameObjectsWithTag("SpeedyBerry"))
            .Concat(GameObject.FindGameObjectsWithTag("TimeSlowBerry"))
            .Concat(GameObject.FindGameObjectsWithTag("PurpleBerry"))
            .Concat(GameObject.FindGameObjectsWithTag("TimeFreezeBerry"))
            .Concat(GameObject.FindGameObjectsWithTag("EvilBerry"))
            .ToArray();
        foreach (GameObject berry in allberries)
        {
            Destroy(berry);
        }
    }

    void UpdateStats(BigInteger scoreAddAmount, BigInteger attemptAddAmount)
    {
        score += scoreAddAmount;
        totalAttempts += attemptAddAmount;
        attempts += attemptAddAmount;
        if (score > highscore)
        {
            highscore = score;
        }
        BazookaManager.Instance.SetGameStoreHighScore(highscore);
        BazookaManager.Instance.SetGameStoreTotalNormalBerries(totalNormalBerries);
        BazookaManager.Instance.SetGameStoreTotalPoisonBerries(totalPoisonBerries);
        BazookaManager.Instance.SetGameStoreTotalSlowBerries(totalSlowBerries);
        BazookaManager.Instance.SetGameStoreTotalUltraBerries(totalUltraBerries);
        BazookaManager.Instance.SetGameStoreTotalSpeedyBerries(totalSpeedyBerries);
        BazookaManager.Instance.SetGameStoreTotalTimeSlowBerries(totalTimeSlowBerries);
        BazookaManager.Instance.SetGameStoreTotalPurpleBerries(totalPurpleBerries);
        BazookaManager.Instance.SetGameStoreTotalTimeFreezeBerries(totalTimeFreezeBerries);
        BazookaManager.Instance.SetGameStoreTotalEvilBerries(totalEvilBerries);
        BazookaManager.Instance.SetGameStoreTotalAttepts(totalAttempts);
        scoreText.text = $"Score: {Tools.FormatWithCommas(score)} \\u2022 Attempts: {Tools.FormatWithCommas(attempts)}";
        highScoreText.text = $"High Score: {Tools.FormatWithCommas(highscore)} \\u2022 Total Attempts: {Tools.FormatWithCommas(totalAttempts)}";
        if (Application.isMobilePlatform) restartButton.interactable = score != 0;
    }

    void CheckIfGrounded()
    {
        isGrounded = bird.transform.position.y <= -4.1299996f;

        rb.gravityScale = isGrounded ? 0f : 1.5f;

        if (bird.transform.position.y < -4.1359f)
        {
            bird.transform.position = new UnityEngine.Vector2(bird.transform.position.x, -4.1359f);
            rb.linearVelocity = new UnityEngine.Vector2(rb.linearVelocity.x, 0f);
        }
        if (Application.isMobilePlatform) jumpButton.transform.GetChild(0).GetComponent<TMP_Text>().color = isGrounded ? Color.white : Color.red;
    }

    internal void TogglePause()
    {
        if (pausePanel.activeSelf)
        {
            DisablePause();
        }
        else
        {
            EnablePause();
        }
    }

    internal void EnablePause()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        backgroundMusic.Pause();
        pausePanel.SetActive(true);
    }

    internal void DisablePause()
    {
        lastMoveTime = Time.time;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        backgroundMusic.Play();
        pausePanel.SetActive(false);
        if (GamePlayerPauseMenu.Instance.editingUI == true) GamePlayerPauseMenu.Instance.ToggleEditingUI();
    }

    void OnApplicationPause(bool pause)
    {
        if (pause) EnablePause();
    }

    void OnApplicationQuit()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}