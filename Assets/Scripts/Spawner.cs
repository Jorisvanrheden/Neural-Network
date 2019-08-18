using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Spawner : MonoBehaviour {

    public List<Player> players_generation;
    public List<Player> saved_players_generation;

    private const string filePath = @"C:\Users\Public\Documents\Unity Projects\Joris' Projects\Neural Network\Saves\brain";
    private const string mapPath = @"C:\Users\Public\Documents\Unity Projects\Joris' Projects\Neural Network\Maps\";
    private const string activeMap = "1.png";

    private const int PLAYERS = 100;

    public Player player_go;
    // Use this for initialization

    public GameObject tile;
    private Vector3 startLocation = new Vector3(0, 0, 0);

	void Start ()
    {
        Texture2D tex = null;
        byte[] fileData;
        string filePath = mapPath + activeMap;
        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }

        for (int i = 0; i < tex.width; i++)
        {
            for (int j = 0; j < tex.height;j++)
            {
                BoxCollider collider = tile.GetComponent<BoxCollider>();
                Vector3 pos = new Vector3(i * collider.size.x * collider.transform.localScale.x, 0, j * collider.size.z *collider.transform.localScale.z);

                Color c = tex.GetPixel(i, j);
                if (c.r == 0 && c.g == 0 && c.b == 0)
                {
                    Instantiate(tile, pos, Quaternion.identity);
                }

                if (c.r == 1 && c.g == 0 && c.b == 0)
                {
                    startLocation = pos;
                }
            }
        }


        InitializePlayers();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            //InitializePlayers();

            SaveBestPlayer();
        }
	}

    private void SaveBestPlayer()
    {
        int BEST_INDEX = 0;
        float BEST_SCORE = 0;

        //Calculate fitness
        float sum = 0;
        for (int i = 0; i < saved_players_generation.Count; i++)
        {
            sum += saved_players_generation[i].score;

            if (saved_players_generation[i].score > BEST_SCORE)
            {
                BEST_SCORE = saved_players_generation[i].score;
                BEST_INDEX = i;
            }
        }

        saved_players_generation[BEST_INDEX].Save(filePath);
    }

    private void InitializePlayers()
    {
        players_generation = new List<Player>();
        saved_players_generation = new List<Player>();

        for (int i = 0; i < PLAYERS; i++)
        {
            Vector3 position = startLocation;
            Player player = (Player)Instantiate(player_go, position, Quaternion.Euler(0, 0, 0));

            NeuralNetwork network = null;
            //network = new NeuralNetwork(filePath);

            player.Initialize(network);

            player.OnTrigger += Player_OnTrigger;

            players_generation.Add(player);
            saved_players_generation.Add(player);
        }
    }

    private void Player_OnTrigger(Player player)
    {
        players_generation.Remove(player);
        player.MakeInivisible();
        //player.Destroy();

        if (players_generation.Count == 0)
        {
            CalculateNextGeneration();
        }
    }

    private void CalculateNextGeneration()
    {
        int BEST_INDEX = 0;
        float BEST_SCORE = 0;

        //Calculate fitness
        float sum = 0;
        for (int i = 0; i < saved_players_generation.Count; i++)
        {
            sum += saved_players_generation[i].score;

            if (saved_players_generation[i].score > BEST_SCORE)
            {
                BEST_SCORE = saved_players_generation[i].score;
                BEST_INDEX = i;
            } 
        }
        for (int i = 0; i < saved_players_generation.Count; i++)
        {
            saved_players_generation[i].fitness = saved_players_generation[i].score/sum;
        }

        Debug.Log("Average score: " + sum / saved_players_generation.Count);

        List<NeuralNetwork> brains = new List<NeuralNetwork>();
        //Pick a normalized distribution thing?
        for (int i = 0; i < PLAYERS - 1; i++)
        {
            //brains.Add(GetDistributedBrain());
            NeuralNetwork brain = saved_players_generation[BEST_INDEX].GetBrain();
            brain.Mutate();
            brains.Add(brain);
        }
        brains.Add(saved_players_generation[BEST_INDEX].GetBrain());

        for (int i = 0; i < saved_players_generation.Count; i++)
        {
            saved_players_generation[i].Destroy();
        }
        players_generation.Clear();
        saved_players_generation.Clear();

        //Pick a normalized distribution thing?
        for (int i = 0; i < PLAYERS; i++)
        {
            Vector3 position = startLocation;
            Player player = (Player)Instantiate(player_go, position, Quaternion.Euler(0, 0, 0));
            player.Initialize(brains[i]);
            player.OnTrigger += Player_OnTrigger;
            players_generation.Add(player);
            saved_players_generation.Add(player);
        }
    }

    private NeuralNetwork GetDistributedBrain()
    {
        //List<float> values = new List<float>();
        //for (int i = 0; i < saved_players_generation.Count; i++)
        //{
        //    values.Add(saved_players_generation[i].fitness);
        //}
        //values.Sort();

        ////Get the top 30%
        //float threshold = values[(int)(values.Count * 0.3f)];
        //List<Player> potentials = new List<Player>();
        //for (int i = 0; i < saved_players_generation.Count; i++)
        //{
        //    if (saved_players_generation[i].fitness > threshold) potentials.Add(saved_players_generation[i]);
        //}

        //int randomIndex = (int)(potentials.Count * RandomGenerator.GetRandomNumber());
        //if (randomIndex >= potentials.Count) randomIndex = potentials.Count - 1;

        //NeuralNetwork brain = potentials[randomIndex].GetBrain();
        //brain.Mutate();

        //return brain;

        int index = 0;

        float r = RandomGenerator.GetRandomNumber();
        //float r = 0.5f + RandomGenerator.GetRandomNumber() / 2;
        while (r > 0)
        {
            r -= saved_players_generation[index].fitness;
            index++;
        }
        index--;

        if (index >= saved_players_generation.Count) index = saved_players_generation.Count - 1;
        if (index < 0) index = 0;

        NeuralNetwork brain = saved_players_generation[index].GetBrain();
        brain.Mutate();

        return brain;
    }
}
