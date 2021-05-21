using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScores : MonoBehaviour
{
    public Text ScoreText;
    public InputField nameText;
    private System.Random random = new System.Random();


    public int playerScore;
    public string playerName;

    // Start is called before the first frame update
    void Start()
    {
        playerScore = random.Next(0, 10);
        ScoreText.text = "Score: " + playerScore;
    }

    // Update is called once per frame
    public void onSubmit()
    {
        playerName = nameText.text ;
        postToDatabase();
    }


    private void postToDatabase()
    {
        User user = new User(playerName, playerScore);
        RestClient.Put("https://augmenttours-default-rtdb.firebaseio.com/"+playerName+".json",user);
    }

    public void getScore()

    {
        playerName = nameText.text;
        RestClient.Get<User>("https://augmenttours-default-rtdb.firebaseio.com/" + playerName + ".json").Then(response =>
            {
               User user = response;
                Debug.Log(playerName);
                Debug.Log(user.userName);
                Debug.Log(user.userScore.ToString());
               ScoreText.text = user.userScore.ToString();
            });

        
    }
    
}
