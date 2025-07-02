using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using static UnityEngine.GraphicsBuffer;

public class RestClient : MonoBehaviour
{
    public TMP_Text captionText; // Reference to the Text UI element
    public TMP_Text titleText; // Reference to the Text UI element
    string baseUrl = "http://localhost:5000";


    void Start()
    {
        Application.runInBackground = true;
        Application.targetFrameRate = 30;
        //DeleteAllContentsInStreamingAssets();

        SetSupportedScenes(); //Starts loop of setting scenes amd getting content with error cases built-in, will run forever
        StartCoroutine(GetAndRunEpisodes());
    }
    SupportedScenes FindSupportedScenes()
    {
        List<string> characters = new List<string>();
        foreach (Transform child in GameObject.Find("Characters").transform)
        {
            characters.Add(child.name);
        }

        //detect unity scenes
        //List<string> sceneNames = new List<string>();
        //for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        //{
        //    string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
        //    string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
        //    sceneNames.Add(sceneName);
        //}
        string sceneName = gameObject.scene.name;
        List<Location> locations = new List<Location>();
        //foreach (string sceneName in sceneNames)
        //{
        List<string> interactableObjects = new List<string>();
        foreach (GameObject sceneChild in gameObject.scene.GetRootGameObjects())
        {
            if (sceneChild.name == "InteractableObjects")
            {
                foreach (Transform interactableObject in sceneChild.transform)
                {
                    interactableObjects.Add(interactableObject.name);
                }
                interactableObjects.Add("Camera");
                continue;
            }
        }
        locations.Add(new Location(sceneName, interactableObjects));
        //}
        SupportedScenes scenes = new SupportedScenes(characters, locations);
        return scenes;
    }

    void SetSupportedScenes()
    {
        SupportedScenes supportedScenes = FindSupportedScenes();
        string jsonPayload = JsonUtility.ToJson(supportedScenes);
        string path = Application.streamingAssetsPath + "/supported_scenes.json";

        File.WriteAllText(path, jsonPayload);
        Debug.Log("Supported scenes saved to " + path);
    }

    IEnumerator GetAndRunEpisodes()
    {
        captionText.text = "...Generating";
        titleText.text = "...";


        yield return StartCoroutine(ChooseEpisodePath((chosenPath) =>
        {
            Debug.Log("Chosen Episode Path: " + chosenPath);
             StartCoroutine(PlayEpisode(chosenPath));
        }));
    }
    IEnumerator ChooseEpisodePath(Action<string> onEpisodeChosen)
    {
        string chosenPath = null;

        while (string.IsNullOrEmpty(chosenPath))
        {
            string[] prioritizedPaths = GetDirectories("prioritized_episodes");
            string[] unreleasedPaths = GetDirectories("unreleased_episodes");
            string[] releasedPaths = GetDirectories("released_episodes");

            // Try to pick from prioritized, else unreleased, else released
            chosenPath = prioritizedPaths.FirstOrDefault() ?? unreleasedPaths.FirstOrDefault() ?? releasedPaths.FirstOrDefault();

            chosenPath = chosenPath.Replace(@"\", "/");
            if (string.IsNullOrEmpty(chosenPath))
            {
                Debug.LogWarning("No episodes found in any folder. Retrying in 5 seconds...");
                captionText.text = "Waiting for episodes...";
                titleText.text = "...";

                // Wait for 5 seconds before retrying
                yield return new WaitForSeconds(5);
            }
        }

        onEpisodeChosen?.Invoke(chosenPath);
    }

    string[] GetDirectories(string folderName)
    {
        string path = Application.streamingAssetsPath + "/" + folderName;
        if (Directory.Exists(path))
        {
            return Directory.GetDirectories(path);
        }
        else
        {
            Debug.LogWarning($"Directory {path} does not exist.");
            return new string[0];
        }
    }




    public Episode LoadEpisodeFromJson(string jsonFilePath)
    {
        // Check if the file exists
        if (!File.Exists(jsonFilePath))
        {
            Debug.LogError("File not found: " + jsonFilePath);
            return null;
        }


        try
        {
            // Read the JSON content from the file
            string jsonContent = File.ReadAllText(jsonFilePath);

            // Deserialize the JSON content to an Episode object
            Episode episode = JsonUtility.FromJson<Episode>(jsonContent);
            Debug.Log("Found and loaded Episode: " + JsonUtility.ToJson(jsonContent));
            return episode;
        }
        catch (Exception ex)
        {
            Debug.LogError("Error reading the JSON file: " + ex.Message);
            return null;
        }
    }


    IEnumerator PlayEpisode(string episodePath)
    {
        titleText.text = Path.GetFileName(episodePath);
        Episode episode = LoadEpisodeFromJson(episodePath + "/script.json");
        Debug.Log(JsonUtility.ToJson(episode));
        int action_i = 0;
        foreach (Action action in episode.script)
        {
            var characterObject = GameObject.Find(action.character);
            if (!characterObject)
            {
                continue;
            }
            var characterAnimator = characterObject.GetComponentInChildren<Animator>();
            if (!characterAnimator)
            {
                continue;
            }
            var characterAudioSource = characterObject.GetComponentInChildren<AudioSource>();
            if (!characterAudioSource)
            {
                continue;
            }
            var characterNavMeshAgent = characterObject.GetComponentInChildren<NavMeshAgent>();
            if (!characterNavMeshAgent)
            {
                continue;
            }
            Debug.Log(JsonUtility.ToJson(action));

            var charactersObjects = GameObject.Find("Characters");
            foreach (var i_characterNavMeshAgent in charactersObjects.GetComponentsInChildren<NavMeshAgent>())
            {
                characterNavMeshAgent.transform.LookAt(characterObject.transform);
            }

            if (action.looking_at != "" || action.walking_to != "")
            {
                //StartCoroutine(LookAtTarget(characterObject, action.looking_at));

                GameObject target = GameObject.Find(action.looking_at);
                if (target != null)
                {
                    characterNavMeshAgent.transform.LookAt(target.transform);
                }
            }
            if (action.walking_to != "")
            {
                GameObject target = GameObject.Find(action.walking_to);
                if (target != null)
                {
                    characterAnimator.SetBool("isIdling", false);
                    characterAnimator.SetBool("isWalking", true);
                    characterNavMeshAgent.SetDestination(target.transform.position);
                    // Optionally, check if the character has reached the destination in Update() or another coroutine
                    // and set characterAnimator.SetBool("isWalking", false) accordingly
                }
            }
            if (action.voice_line != "")
            {
                if (!characterAnimator.GetBool("isWalking"))
                {
                    characterAnimator.SetBool("isIdling", false);
                    characterAnimator.SetBool("isTalking", true);
                }
                captionText.text = action.character + ": " + action.voice_line;

                string audioFilePath = $"{episodePath}/{action_i}_{action.character}.wav";
                yield return StartCoroutine(LoadAndPlayAudio(characterAudioSource, audioFilePath));
            }
            if (action.walking_to != "" || action.voice_line != "")
            {
                characterAnimator.SetBool("isIdling", true);
                yield return new WaitForSeconds(2);
            }

            yield return new WaitForSeconds(0.5f);
            characterAnimator.SetBool("isWalking", false);
            characterAnimator.SetBool("isTalking", false);
            characterAnimator.SetBool("isJumping", false);
            characterAnimator.SetBool("isIdling", true);
            action_i++;
        }
        try
        {
            Directory.Move(episodePath, Application.streamingAssetsPath + "/released_episodes/" + Path.GetFileName(episodePath));
            Directory.Move(episodePath + ".meta", Application.streamingAssetsPath + "/released_episodes/" + Path.GetFileName(episodePath) + ".meta");
        }
        catch (Exception e)
        {
            Directory.Delete(episodePath, true);
            Directory.Delete(episodePath + ".meta", true);
        }
        yield return StartCoroutine(GetAndRunEpisodes());
    }


    IEnumerator LoadAndPlayAudio(AudioSource audioSource, string path)
    {
        using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.WAV))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error when loading audio file: " + uwr.error);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(uwr);
                if (clip != null)
                {
                    audioSource.clip = clip;
                    audioSource.Play();

                    // Wait until the audio has finished playing
                    yield return new WaitWhile(() => audioSource.isPlaying);
                }
                else
                {
                    Debug.LogError("Audio file not found at: " + path);
                }
            }
        }
    }


    public static void DeleteAllContentsInStreamingAssets()
    {
        string streamingAssetsPath = Application.streamingAssetsPath;

        // Check if the directory exists
        if (Directory.Exists(streamingAssetsPath))
        {
            // Get all file names and delete each file
            string[] files = Directory.GetFiles(streamingAssetsPath);
            foreach (string file in files)
            {
                File.Delete(file);
            }

            // Get all directory names and delete each directory
            string[] directories = Directory.GetDirectories(streamingAssetsPath);
            foreach (string directory in directories)
            {
                Directory.Delete(directory, true); // true for recursive delete
            }
        }
        else
        {
            Debug.LogWarning("StreamingAssets directory does not exist or could not be found.");
        }
    }
}

[System.Serializable]
public class Action
{
    public string character;
    public string voice_line;
    public string looking_at;
    public string walking_to;
}


[System.Serializable]
public class Episode
{
    public string episode_title;
    public string location;
    public Action[] script;
}

[System.Serializable]
public class Location
{
    public string title;
    public List<string> interactableObjects;

    public Location(string title, List<string> interactableObjects)
    {
        this.title = title;
        this.interactableObjects = interactableObjects;
    }
}

[System.Serializable]
public class SupportedScenes
{
    public List<string> characters;
    public List<Location> locations;

    public SupportedScenes(List<string> characters, List<Location> locations)
    {
        this.characters = characters;
        this.locations = locations;
    }
}

[System.Serializable]
public class EpisodePathResponse
{
    public string episode_path;
}