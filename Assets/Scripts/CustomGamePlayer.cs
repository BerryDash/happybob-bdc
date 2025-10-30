using System.Linq;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CustomGamePlayer : MonoBehaviour
{
    public static CustomGamePlayer instance;
    private readonly float spawnRate = 1f;
    private float nextSpawnTime;
    internal BigInteger score;
    private float boostLeft;
    private float slownessLeft;
    private float speedyLeft;
    private float antiLeft;
    private float timeSlowLeft;
    private float timeFreezeLeft;
    private float screenWidth;
    internal bool isGrounded;
    public TMP_Text scoreText;
    public TMP_Text boostText;
    public GameObject bird;
    public GameObject pausePanel;
    public Rigidbody2D rb;
    public AudioSource backgroundMusic;
    public SpriteRenderer overlayRender;
    private float lastMoveTime;
    public GameObject berryParent;

    public GameObject mobileButtons;
    public Button pauseButton;
    public Button restartButton;
    public Button jumpButton;
    public Button rightButton;
    public Button leftButton;
    
    private float normalBerryChance;
    private float poisonBerryChance;
    private float slowBerryChance;
    private float ultraBerryChance;
    private float speedyBerryChance;
    private float randomBerryChance;
    private float antiBerryChance;
    private float nothingBerryChance;
    private float timeSlowBerryChance;
    private float purpleBerryChance;
    private float timeFreezeBerryChance;
    private float evilBerryChance;

    void Start()
    {
        CustomGameTempData customGameTempData = FindObjectsByType<CustomGameTempData>(FindObjectsSortMode.None)[0];
        if (customGameTempData == null)
        {
            SceneManager.LoadScene("MainMenu");
            return;
        }
        normalBerryChance = customGameTempData.normalBerryChance;
        poisonBerryChance = customGameTempData.poisonBerryChance;
        slowBerryChance = customGameTempData.slowBerryChance;
        ultraBerryChance = customGameTempData.ultraBerryChance;
        speedyBerryChance = customGameTempData.speedyBerryChance;
        randomBerryChance = customGameTempData.randomBerryChance;
        antiBerryChance = customGameTempData.antiBerryChance;
        nothingBerryChance = customGameTempData.nothingBerryChance;
        timeSlowBerryChance = customGameTempData.timeSlowBerryChance;
        purpleBerryChance = customGameTempData.purpleBerryChance;
        timeFreezeBerryChance = customGameTempData.timeFreezeBerryChance;
        evilBerryChance = customGameTempData.evilBerryChance;
        Destroy(customGameTempData.gameObject);

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
        component.sprite = Resources.Load<Sprite>("Icons/Icons/bird_" + num);
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
        if (antiLeft > 0f)
        {
            string[] berryTags = { "NormalBerry", "PoisonBerry", "SlowBerry", "UltraBerry", "SpeedyBerry", "RandomBerry", "AntiBerry", "NothingBerry", "TimeSlowBerry", "PurpleBerry", "TimeFreezeBerry", "EvilBerry" };
            foreach (string tag in berryTags)
            {
                foreach (var berry in GameObject.FindGameObjectsWithTag(tag))
                {
                    UnityEngine.Vector3 dir = berry.transform.position - bird.transform.position;
                    if (dir.magnitude < 3f)
                    {
                        berry.GetComponent<Rigidbody2D>().linearVelocity = dir.normalized * 5f;
                        ClampPosition(berry, false);
                    }
                }
            }
        }
    }

    void ClampPosition(GameObject obj, bool modifyY = true)
    {
        var cam = Camera.main;
        var pos = obj.transform.position;
        var bounds = obj.GetComponent<Renderer>().bounds.extents;

        float zDist = Mathf.Abs(cam.transform.position.z - pos.z);

        UnityEngine.Vector3 min = cam.ViewportToWorldPoint(new UnityEngine.Vector3(0, 0, zDist));
        UnityEngine.Vector3 max = cam.ViewportToWorldPoint(new UnityEngine.Vector3(1, 1, zDist));

        pos.x = Mathf.Clamp(pos.x, min.x + bounds.x, max.x - bounds.x);
        if (modifyY) pos.y = Mathf.Clamp(pos.y, min.y + bounds.y, max.y - bounds.y);

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
            else if (antiLeft > 0f)
            {
                antiLeft -= Time.deltaTime;
                boostText.text = "Berry repellent expires in " + string.Format("{0:0.0}", antiLeft) + "s";
            }
            else if (timeSlowLeft > 0f)
            {
                timeSlowLeft -= Time.deltaTime;
                boostText.text = "Time Slow expires in " + string.Format("{0:0.0}", timeSlowLeft) + "s";
            }
            else if (timeFreezeLeft > 0f)
            {
                timeFreezeLeft -= Time.deltaTime;
                boostText.text = "Time Freeze expires in " + string.Format("{0:0.0}", timeFreezeLeft) + "s";
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
        if ((int)timeFreezeLeft != 0) return;
        if (!pausePanel.activeSelf)
        {
            float cumulative = 0f;
            GameObject newBerry = new("Berry");
            newBerry.transform.SetParent(berryParent.transform);
            SpriteRenderer spriteRenderer = newBerry.AddComponent<SpriteRenderer>();
            cumulative += normalBerryChance / 100f;
            if (spawnProbability <= cumulative)
            {
                spriteRenderer.sprite = Resources.Load<Sprite>("Berries/Berry");
                newBerry.tag = "NormalBerry";
                goto finish;
            }
            cumulative += poisonBerryChance / 100f;
            if (spawnProbability <= cumulative)
            {
                spriteRenderer.sprite = Resources.Load<Sprite>("Berries/PoisonBerry");
                newBerry.tag = "PoisonBerry";
                goto finish;
            }
            cumulative += slowBerryChance / 100f;
            if (spawnProbability <= cumulative)
            {
                spriteRenderer.sprite = Resources.Load<Sprite>("Berries/SlowBerry");
                newBerry.tag = "SlowBerry";
                goto finish;
            }
            cumulative += ultraBerryChance / 100f;
            if (spawnProbability <= cumulative)
            {
                spriteRenderer.sprite = Resources.Load<Sprite>("Berries/UltraBerry");
                newBerry.tag = "UltraBerry";
                goto finish;
            }
            cumulative += speedyBerryChance / 100f;
            if (spawnProbability <= cumulative)
            {
                spriteRenderer.sprite = Resources.Load<Sprite>("Berries/SpeedyBerry");
                newBerry.tag = "SpeedyBerry";
                goto finish;
            }
            cumulative += randomBerryChance / 100f;
            if (spawnProbability <= cumulative)
            {
                spriteRenderer.sprite = Resources.Load<Sprite>("Berries/BerryNoColor");
                newBerry.tag = "RandomBerry";
                RainbowSpriteRender randomBerryRainbowImage = newBerry.AddComponent<RainbowSpriteRender>();
                randomBerryRainbowImage.frequency = 5f;
                goto finish;
            }
            cumulative += antiBerryChance / 100f;
            if (spawnProbability <= cumulative)
            {
                spriteRenderer.sprite = Resources.Load<Sprite>("Berries/AntiBerry");
                newBerry.tag = "AntiBerry";
                goto finish;
            }
            cumulative += nothingBerryChance / 100f;
            if (spawnProbability <= cumulative)
            {
                spriteRenderer.sprite = Resources.Load<Sprite>("Berries/BerryNoColor");
                newBerry.tag = "NothingBerry";
                goto finish;
            }
            cumulative += timeSlowBerryChance / 100f;
            if (spawnProbability <= cumulative)
            {
                spriteRenderer.sprite = Resources.Load<Sprite>("Berries/TimeSlowBerry");
                newBerry.tag = "TimeSlowBerry";
                goto finish;
            }
            cumulative += purpleBerryChance / 100f;
            if (spawnProbability <= cumulative)
            {
                spriteRenderer.sprite = Resources.Load<Sprite>("Berries/PurpleBerry");
                newBerry.tag = "PurpleBerry";
                goto finish;
            }
            cumulative += timeFreezeBerryChance / 100f;
            if (spawnProbability <= cumulative)
            {
                spriteRenderer.sprite = Resources.Load<Sprite>("Berries/TimeFreezeBerry");
                newBerry.tag = "TimeFreezeBerry";
                goto finish;
            }
            cumulative += evilBerryChance / 100f;
            if (spawnProbability <= cumulative)
            {
                spriteRenderer.sprite = Resources.Load<Sprite>("Berries/EvilBerry");
                newBerry.tag = "EvilBerry";
                goto finish;
            }
        finish:
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
        foreach (AudioSource audio in FindObjectsByType<AudioSource>(FindObjectsSortMode.None))
        {
            audio.pitch = speedyLeft > 0f ? 1.3f : 1f;
        }
        if (screenWidth != Camera.main.orthographicSize * 2f * Camera.main.aspect)
        {
            screenWidth = Camera.main.orthographicSize * 2f * Camera.main.aspect;
            ClampPosition(bird);
            GameObject[] allberries = GameObject.FindGameObjectsWithTag("NormalBerry")
                .Concat(GameObject.FindGameObjectsWithTag("PoisonBerry"))
                .Concat(GameObject.FindGameObjectsWithTag("SlowBerry"))
                .Concat(GameObject.FindGameObjectsWithTag("UltraBerry"))
                .Concat(GameObject.FindGameObjectsWithTag("SpeedyBerry"))
                .Concat(GameObject.FindGameObjectsWithTag("RandomBerry"))
                .Concat(GameObject.FindGameObjectsWithTag("AntiBerry"))
                .Concat(GameObject.FindGameObjectsWithTag("TimeSlowBerry"))
                .Concat(GameObject.FindGameObjectsWithTag("PurpleBerry"))
                .Concat(GameObject.FindGameObjectsWithTag("TimeFreezeBerry"))
                .Concat(GameObject.FindGameObjectsWithTag("EvilBerry"))
                .ToArray();
            foreach (GameObject berry in allberries)
            {
                ClampPosition(berry, false);
            }
        }
        GameObject[] normalBerries = GameObject.FindGameObjectsWithTag("NormalBerry");
        GameObject[] poisonBerries = GameObject.FindGameObjectsWithTag("PoisonBerry");
        GameObject[] slowBerries = GameObject.FindGameObjectsWithTag("SlowBerry");
        GameObject[] ultraBerries = GameObject.FindGameObjectsWithTag("UltraBerry");
        GameObject[] speedyBerries = GameObject.FindGameObjectsWithTag("SpeedyBerry");
        GameObject[] randomBerries = GameObject.FindGameObjectsWithTag("RandomBerry");
        GameObject[] antiBerries = GameObject.FindGameObjectsWithTag("AntiBerry");
        GameObject[] nothingBerries = GameObject.FindGameObjectsWithTag("NothingBerry");
        GameObject[] timeSlowBerries = GameObject.FindGameObjectsWithTag("TimeSlowBerry");
        GameObject[] purpleBerries = GameObject.FindGameObjectsWithTag("PurpleBerry");
        GameObject[] timeFreezeBerries = GameObject.FindGameObjectsWithTag("TimeFreezeBerry");
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
                    DoNormalBerry(normalBerry);
                }
                if (timeFreezeLeft > 0)
                {
                    normalBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, 0f);
                }
                else if (timeSlowLeft > 0)
                {
                    normalBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -2f);
                }
                else if (speedyLeft > 0)
                {
                    normalBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -7.5f);
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
                    DoPoisonBerry();
                }
                if (timeFreezeLeft > 0)
                {
                    poisonBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, 0f);
                }
                else if (timeSlowLeft > 0)
                {
                    poisonBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -2f);
                }
                else if (speedyLeft > 0)
                {
                    poisonBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -7.5f);
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
                    DoSlowBerry(slowBerry);
                }
                if (timeFreezeLeft > 0)
                {
                    slowBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, 0f);
                }
                else if (timeSlowLeft > 0)
                {
                    slowBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -2f);
                }
                else if (speedyLeft > 0)
                {
                    slowBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -7.5f);
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
                    DoUltraBerry(ultraBerry);
                }
                if (timeFreezeLeft > 0)
                {
                    ultraBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, 0f);
                }
                else if (timeSlowLeft > 0)
                {
                    ultraBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -2f);
                }
                else if (speedyLeft > 0)
                {
                    ultraBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -7.5f);
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
                    DoSpeedyBerry(speedyBerry);
                }
                if (timeFreezeLeft > 0)
                {
                    speedyBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, 0f);
                }
                else if (timeSlowLeft > 0)
                {
                    speedyBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -2f);
                }
                else if (speedyLeft > 0)
                {
                    speedyBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -7.5f);
                }
                else
                {
                    speedyBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -4f);
                }
            }
            foreach (GameObject randomBerry in randomBerries)
            {
                if (randomBerry.transform.position.y < 0f - Camera.main.orthographicSize - 1f)
                {
                    Destroy(randomBerry);
                }
                else if (UnityEngine.Vector3.Distance(bird.transform.position, randomBerry.transform.position) < 1.5f)
                {
                    System.Action[] funcs = {
                        () => DoNormalBerry(randomBerry),
                        () => DoSlowBerry(randomBerry),
                        () => DoUltraBerry(randomBerry),
                        () => DoSpeedyBerry(randomBerry),
                        () => DoAntiBerry(randomBerry)
                    };
                    funcs[Random.Range(0, funcs.Length)]();
                }
                if (timeFreezeLeft > 0)
                {
                    randomBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, 0f);
                }
                else if (timeSlowLeft > 0)
                {
                    randomBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -2f);
                }
                else if (speedyLeft > 0)
                {
                    randomBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -7.5f);
                }
                else
                {
                    randomBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -4f);
                }
            }
            foreach (GameObject antiBerry in antiBerries)
            {
                if (antiBerry.transform.position.y < 0f - Camera.main.orthographicSize - 1f)
                {
                    Destroy(antiBerry);
                }
                else if (UnityEngine.Vector3.Distance(bird.transform.position, antiBerry.transform.position) < 1.5f)
                {
                    DoAntiBerry(antiBerry);
                }
                if (timeFreezeLeft > 0)
                {
                    antiBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, 0f);
                }
                else if (timeSlowLeft > 0)
                {
                    antiBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -2f);
                }
                else if (speedyLeft > 0)
                {
                    antiBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -7.5f);
                }
                else
                {
                    antiBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -4f);
                }
            }
            foreach (GameObject nothingBerry in nothingBerries)
            {
                if (nothingBerry.transform.position.y < 0f - Camera.main.orthographicSize - 1f)
                {
                    Destroy(nothingBerry);
                }
                else if (UnityEngine.Vector3.Distance(bird.transform.position, nothingBerry.transform.position) < 1.5f)
                {
                    DoNothingBerry(nothingBerry);
                }
                if (speedyLeft > 0)
                {
                    nothingBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -7.5f);
                }
                else
                {
                    nothingBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -4f);
                }
            }
            foreach (GameObject timeSlowBerry in timeSlowBerries)
            {
                if (timeSlowBerry.transform.position.y < 0f - Camera.main.orthographicSize - 1f)
                {
                    Destroy(timeSlowBerry);
                }
                else if (UnityEngine.Vector3.Distance(bird.transform.position, timeSlowBerry.transform.position) < 1.5f)
                {
                    DoTimeSlowBerry(timeSlowBerry);
                }
                if (timeFreezeLeft > 0)
                {
                    timeSlowBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, 0f);
                }
                else if (timeSlowLeft > 0)
                {
                    timeSlowBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -2f);
                }
                else if (speedyLeft > 0)
                {
                    timeSlowBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -7.5f);
                }
                else
                {
                    timeSlowBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -4f);
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
                    DoPurpleBerry(purpleBerry);
                }
                if (timeFreezeLeft > 0)
                {
                    purpleBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, 0f);
                }
                else if (timeSlowLeft > 0)
                {
                    purpleBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -2f);
                }
                else if (speedyLeft > 0)
                {
                    purpleBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -7.5f);
                }
                else
                {
                    purpleBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -4f);
                }
            }
            foreach (GameObject timeFreezeBerry in timeFreezeBerries)
            {
                if (timeFreezeBerry.transform.position.y < 0f - Camera.main.orthographicSize - 1f)
                {
                    Destroy(timeFreezeBerry);
                }
                else if (UnityEngine.Vector3.Distance(bird.transform.position, timeFreezeBerry.transform.position) < 1.5f)
                {
                    DoTimeFreezeBerry(timeFreezeBerry);
                }
                if (timeFreezeLeft > 0)
                {
                    timeFreezeBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, 0f);
                }
                else if (timeSlowLeft > 0)
                {
                    timeFreezeBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -2f);
                }
                else if (speedyLeft > 0)
                {
                    timeFreezeBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -7.5f);
                }
                else
                {
                    timeFreezeBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -4f);
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
                    DoEvilBerry(evilBerry);
                }
                if (timeFreezeLeft > 0)
                {
                    evilBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, 0f);
                }
                else if (timeSlowLeft > 0)
                {
                    evilBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -2f);
                }
                else if (speedyLeft > 0)
                {
                    evilBerry.GetComponent<Rigidbody2D>().linearVelocity = new UnityEngine.Vector2(0f, -7.5f);
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
                .Concat(randomBerries)
                .Concat(antiBerries)
                .Concat(nothingBerries)
                .Concat(timeSlowBerries)
                .Concat(purpleBerries)
                .Concat(timeFreezeBerries)
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
        slownessLeft = 0f;
        speedyLeft = 0f;
        antiLeft = 0f;
        timeSlowLeft = 0f;
        timeFreezeLeft = 0f;
        UpdateStats(0, 1);

        GameObject[] allberries = GameObject.FindGameObjectsWithTag("NormalBerry")
            .Concat(GameObject.FindGameObjectsWithTag("PoisonBerry"))
            .Concat(GameObject.FindGameObjectsWithTag("SlowBerry"))
            .Concat(GameObject.FindGameObjectsWithTag("UltraBerry"))
            .Concat(GameObject.FindGameObjectsWithTag("SpeedyBerry"))
            .Concat(GameObject.FindGameObjectsWithTag("RandomBerry"))
            .Concat(GameObject.FindGameObjectsWithTag("AntiBerry"))
            .Concat(GameObject.FindGameObjectsWithTag("NothingBerry"))
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
        scoreText.text = $"Score: {Tools.FormatWithCommas(score)}";
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
        backgroundMusic.GetComponent<GameMusicHandler>().PauseMusic();
        pausePanel.SetActive(true);
    }

    internal void DisablePause()
    {
        lastMoveTime = Time.time;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        backgroundMusic.GetComponent<GameMusicHandler>().ResumeMusic();
        pausePanel.SetActive(false);
        if (CustomGamePlayerPauseMenu.Instance.editingUI == true) CustomGamePlayerPauseMenu.Instance.ToggleEditingUI();
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

    void DoNormalBerry(GameObject berry)
    {
        AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Eat"), Camera.main.transform.position, 1.2f * BazookaManager.Instance.GetSettingSFXVolume());
        Destroy(berry);
        UpdateStats(1, 0);
    }

    void DoPoisonBerry()
    {
        AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Death"), Camera.main.transform.position, 1.2f * BazookaManager.Instance.GetSettingSFXVolume());
        Respawn();
        UpdateStats(0, 0);
    }

    void DoSlowBerry(GameObject berry)
    {
        AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Downgrade"), Camera.main.transform.position, 0.35f * BazookaManager.Instance.GetSettingSFXVolume());
        Destroy(berry);
        boostLeft = 0f;
        slownessLeft = 10f;
        speedyLeft = 0f;
        antiLeft = 0f;
        timeSlowLeft = 0f;
        timeFreezeLeft = 0f;
        if (score > 0)
        {
            UpdateStats(-1, 0);
        }
    }

    void DoUltraBerry(GameObject berry)
    {
        AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Powerup"), Camera.main.transform.position, 0.35f * BazookaManager.Instance.GetSettingSFXVolume());
        Destroy(berry);
        speedyLeft = 0f;
        antiLeft = 0f;
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
        timeSlowLeft = 0f;
        timeFreezeLeft = 0f;
    }

    void DoSpeedyBerry(GameObject berry)
    {
        AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/SpeedyPowerup"), Camera.main.transform.position, 0.35f * BazookaManager.Instance.GetSettingSFXVolume());
        Destroy(berry);
        boostLeft = 0f;
        slownessLeft = 0f;
        speedyLeft = 10f;
        antiLeft = 0f;
        timeSlowLeft = 0f;
        timeFreezeLeft = 0f;
        UpdateStats(10, 0);
    }

    void DoAntiBerry(GameObject berry)
    {
        AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Downgrade"), Camera.main.transform.position, 0.35f * BazookaManager.Instance.GetSettingSFXVolume());
        Destroy(berry);
        boostLeft = 0f;
        slownessLeft = 0f;
        speedyLeft = 0f;
        antiLeft = 10f;
        timeSlowLeft = 0f;
        timeFreezeLeft = 0f;
        UpdateStats(0, 0);
    }

    void DoNothingBerry(GameObject berry)
    {
        AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Eat"), Camera.main.transform.position, 1.2f * BazookaManager.Instance.GetSettingSFXVolume());
        Destroy(berry);
    }

    void DoTimeSlowBerry(GameObject berry)
    {
        AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/SlowMotion"), Camera.main.transform.position, 1.2f * BazookaManager.Instance.GetSettingSFXVolume());
        Destroy(berry);
        boostLeft = 0f;
        slownessLeft = 0f;
        speedyLeft = 0f;
        antiLeft = 0f;
        timeSlowLeft = 10f;
        timeFreezeLeft = 0f;
        UpdateStats(1, 0);
    }

    void DoPurpleBerry(GameObject berry)
    {
        AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/Eat"), Camera.main.transform.position, 1.2f * BazookaManager.Instance.GetSettingSFXVolume());
        Destroy(berry);
        UpdateStats(15, 0);
    }

    void DoTimeFreezeBerry(GameObject berry)
    {
        AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/TimeStop"), Camera.main.transform.position, 1.2f * BazookaManager.Instance.GetSettingSFXVolume());
        Destroy(berry);
        boostLeft = 0f;
        slownessLeft = 0f;
        speedyLeft = 0f;
        antiLeft = 0f;
        timeSlowLeft = 0f;
        timeFreezeLeft = 5f;
    }

    void DoEvilBerry(GameObject berry)
    {
        AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/EvilLaugh"), Camera.main.transform.position, 1.2f * BazookaManager.Instance.GetSettingSFXVolume());
        Destroy(berry);
        if (score >= 10)
        {
            UpdateStats(-10, 0);
        } else
        {
            score = 0;
        }
    }
}