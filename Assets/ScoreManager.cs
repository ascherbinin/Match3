using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{

    public static ScoreManager instance = null;
    private int _score = 0;

    private Text _scoreText = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void AddScore(int amount)
    {
        _score += amount;
        UpdateScore();
    }


	// Use this for initialization
	void Start ()
	{
	    _scoreText = GameObject.Find("Score").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void UpdateScore ()
	{
	    _scoreText.text = "SCORE: " + _score;
	}
}
