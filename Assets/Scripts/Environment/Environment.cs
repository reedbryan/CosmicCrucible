using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
    public GameObject empty;
    GameObject foregroundAnchor;
    GameObject backgroundAnchor;
    List<GameObject> backgroundObjects = new List<GameObject>();

    // pickup items
    [SerializeField] GameObject speedBoostPrefab;
    [SerializeField] GameObject healthPickup;
    GameObject[] pickUpsList = new GameObject[2];

    [SerializeField] float pickUpSpawnInterval;
    [SerializeField] float PUSI_Timer; // timer for pickUpSpawnInterval

    // Stars
    public GameObject starPrefab;
    public float foregroundStarCount;
    public float backgroundStarCount;

    public float starCount;
    [SerializeField] Vector2[] starLayout;
    [SerializeField] List<Vector2> allPostionsInView = new List<Vector2>();

    public Vector2 starArea;

    Camera mainCamera;

    private void Awake()
    {
        // Assignment
        PUSI_Timer = pickUpSpawnInterval;

        pickUpsList[0] = speedBoostPrefab;
        pickUpsList[1] = healthPickup;

        foregroundAnchor = Instantiate(empty, new Vector3(0, 0, -1), transform.rotation);
        foregroundAnchor.name = "Foreground Anchor";
    }

    private void Update()
    {
        placePickUps();
    }

    void placePickUps()
    {
        if (PUSI_Timer <= 0) // PUSI -> Pick Up Spawn Interval
        {
            // Get pickup
            GameObject toBeSpawned = pickUpsList[Random.Range(0, pickUpsList.Length)];

            // Get pos
            Vector2 pos = new Vector2(Random.Range(starArea.x * -1f, starArea.x), Random.Range(starArea.y * -1f, starArea.y));

            GameObject newPickup = Instantiate(toBeSpawned, pos, transform.rotation);
            newPickup.transform.parent = foregroundAnchor.transform;

            PUSI_Timer = pickUpSpawnInterval;
        }
        PUSI_Timer -= 1 * Time.deltaTime;
    }

    // - - - - - - - OLD single star gfx - - - - - - - - - - -

    /*
    private void Awake()
    {
        // Assignment
        PUSI_Timer = pickUpSpawnInterval;

        pickUpsList[0] = speedBoostPrefab;
        pickUpsList[1] = healthPickup;

        starLayout = new Vector2[(int)starCount];

        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    void Start()
    {
        foregroundAnchor = Instantiate(empty, new Vector3(0, 0, -1), transform.rotation);
        foregroundAnchor.name = "Foreground Anchor";

        backgroundAnchor = Instantiate(empty, new Vector3(0, 0, -1), transform.rotation);
        backgroundAnchor.name = "Background Anchor";

        placeStars();
    }

    private void Update()
    {
        QualitySettings.pixelLightCount = 0;

        placePickUps();
        //background();
    }

    void placePickUps()
    {
        if (PUSI_Timer <= 0) // PUSI -> Pick Up Spawn Interval
        {
            // Get pickup
            GameObject toBeSpawned = pickUpsList[Random.Range(0, pickUpsList.Length)];

            // Get pos
            Vector2 pos = new Vector2(Random.Range(starArea.x * -1f, starArea.x), Random.Range(starArea.y * -1f, starArea.y));

            GameObject newPickup = Instantiate(toBeSpawned, pos, transform.rotation);
            newPickup.transform.parent = foregroundAnchor.transform;

            PUSI_Timer = pickUpSpawnInterval;
        }
        PUSI_Timer -= 1 * Time.deltaTime;
    }

    void getStarLayout()
    {
        for (int i = 0; i < starCount; i++)
        {
            starLayout[i] = new Vector2(Random.Range(starArea.x * -1, starArea.x), Random.Range(starArea.y * -1, starArea.y));
        }
    }

    void placeStars()
    {
        foreach (var item in starLayout)
        {
            if (mainCamera.transform.position.y + mainCamera.orthographicSize < item.y &&
                mainCamera.transform.position.y - mainCamera.orthographicSize > item.y)
            {
                continue;
            }
            if (mainCamera.transform.position.x + mainCamera.orthographicSize * 2 < item.x &&
                mainCamera.transform.position.x - mainCamera.orthographicSize * 2 > item.x)
            {
                continue;
            }

            allPostionsInView.Add(item);
        }


        // Foreground stars
        for (int i = 0; i < foregroundStarCount; i++)
        {
            GameObject star = createStar(transform.position, starArea, 0.15f, foregroundAnchor);
            star.GetComponent<Star>().deapth = 0;

            star.GetComponent<SpriteRenderer>().color = getOffWhiteColor();
        }

        // Background stars
        for (int i = 0; i < backgroundStarCount; i++)
        {
            Vector2 range = new Vector2(mainCamera.orthographicSize * 2f, mainCamera.orthographicSize * 1f);
            GameObject star = createStar(transform.position, range, 0.08f, backgroundAnchor);
            star.GetComponent<Star>().deapth = Random.Range(0.3f, 0.6f);

            backgroundObjects.Add(star);
            star.GetComponent<SpriteRenderer>().color = getOffWhiteColor();
        }
    }

    GameObject createStar(Vector2 origin, Vector2 range, float size, GameObject parent)
    {
        Vector2 spawnPos = new Vector2(Random.Range(range.x * -1f, range.x), Random.Range(range.y * -1f, range.y)) + origin;

        GameObject star = Instantiate(starPrefab, spawnPos, transform.rotation);
        star.transform.eulerAngles += new Vector3(0, 0, Random.Range(-180, 180));
        star.transform.localScale = new Vector3(size, size, size);
        star.transform.parent = parent.transform;

        star.GetComponent<SpriteRenderer>().color = getOffWhiteColor();

        return star;
    }

    /// <summary>
    /// Returns RGBA color that is close to white.
    /// </summary>
    /// <returns></returns>
    Color getOffWhiteColor()
    {
        Color offWhite = new Color(
            Random.Range(0.6f, 1)
            , Random.Range(0.6f, 1)
            , Random.Range(0.6f, 1)
            , 1);
        return offWhite;
    }

    */
}
