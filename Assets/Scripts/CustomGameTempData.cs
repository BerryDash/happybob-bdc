using UnityEngine;

public class CustomGameTempData : MonoBehaviour
{
    public float normalBerryChance;
    public float poisonBerryChance;
    public float slowBerryChance;
    public float ultraBerryChance;
    public float speedyBerryChance;
    public float randomBerryChance;
    public float antiBerryChance;
    public float nothingBerryChance;
    public float timeSlowBerryChance;
    public float purpleBerryChance;
    public float timeFreezeBerryChance;
    public float evilBerryChance;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
