using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LexicomixNamespace;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

public class GameManager : MonoBehaviour
{

    [DllImport("__Internal")]
    private static extern void HelloString(string str);


    [DllImport("__Internal")]
    private static extern string getCookie(string cname);


    private MediaController mediaController;
    private GameStats gameStats;
    protected GameConfiguration gameConfig;
    private Localization local;


    private ImageController[] images;
    private ClueController[] clues;
    private TextInputController[] textInputs;
    private AudioController[] sounds;
    private WordSlotManager wSlotManager;
    private InputZoneController inputPanel;

    // The ordered list of words and syllables
    private int nextTestIndex;
    private List<string> testOrder;

    private float timePassed;

    // Loading assets
    public Image loadingBar;
    private float loadingOffset;
    public GameObject loadingCanvas;

    public Text timerText;

    // Add unstable features here
    private List<string> unstable = new List<string>();




    void Start()
    {
        // Getting all the references
        gameStats = new GameStats();
        mediaController = GetComponent<MediaController>();
        local = new Localization();
        gameConfig = new GameConfiguration();


        // Hide actual game screen
        loadingCanvas.SetActive(true);



#if UNITY_EDITOR
        // Unstable and untested features should be allowed only when running in the editor

#else
        // Do something only if build is running outside the unity editor

        // Don't allow unstable features(add to unstable list when running in the website)


        unstable.Add("SyllableTyping");
        unstable.Add("AlphabetTyping");


#endif

        // Get media objects
        GetMediaObjects();
        GetSyllablesObjects();
        GetTextInput();


        // Start needed objects
        mediaController.Load(this);
        gameConfig.Load();

        //testOrder = new List<int>(gameConfig.nbTests);

        timePassed = 0;

        //OrderTests();
        //nextTestIndex = 0;
        //ComputeNextTest();
    }


    public void UpdateLoadingBar(float addOffset)
    {
        // Add value to progress
        loadingOffset += addOffset;
    }

    public void StartGame(List<string> wordIDs)
    {
        // Everything loaded, show game screen
        loadingCanvas.SetActive(false);

        // Store the downloaded words and build initial order
        testOrder = wordIDs;
        OrderTests();

        // Add method calls for text input changes
        if(textInputs != null)
        {
            //textInputs[0].myTextInput.onValueChanged.AddListener(delegate { AlphabetTyping(Input.inputString); });
            //textInputs[0].myTextInput.onEndEdit.AddListener(delegate { OnCheckInput(textInputs[0].myTextInput.text); });
            //textInputs[0].myTextInput.onEndEdit.AddListener(delegate { OnCheckInput(inputPanel.GetWholeInputText()); });
        }


        // Set default game values and indexes
        nextTestIndex = 0;

        // Everything set, ready to receive player interactions
        ReadyToPlayStandBy();
    }

    void Update()
    {
        // Filling loading bar as all medias get loaded
        loadingBar.fillAmount = Mathf.Lerp(loadingBar.fillAmount, loadingOffset, 0.03f);


        // Start timer only when loading has ended
        if (loadingCanvas.activeInHierarchy)
            return;


        // Simple timer
        timePassed += Time.deltaTime;

        string minutes = Mathf.Floor(timePassed / 60).ToString("00");
        string seconds = Mathf.Floor(timePassed % 60).ToString("00");

        timerText.text = minutes + ":" + seconds;

    }


    void GetMediaObjects()
    {
        string image = "Image";
        GameObject[] imageObjects = GameObject.FindGameObjectsWithTag(image);
        if(imageObjects.Length > 0)
        {
            images = new ImageController[imageObjects.Length];
            foreach (GameObject obj in imageObjects)
            {
                // Extract index from image name
                int index = int.Parse(obj.name.Substring(image.Length));
                images[index] = obj.GetComponent<ImageController>();
                Debug.Log(images[index].name + " : " + obj.name);
            }
        }



        string sound = "Sound";
        GameObject[] soundObjects = GameObject.FindGameObjectsWithTag(sound);
        if(soundObjects.Length > 0)
        {
            sounds = new AudioController[soundObjects.Length];
            foreach (GameObject obj in soundObjects)
            {
                // Extract index from sound name
                int index = int.Parse(obj.name.Substring(sound.Length));
                sounds[index] = obj.GetComponent<AudioController>();
                Debug.Log(sounds[index].name + " : " + obj.name);
            }
        }




        string clue = "Clue";
        GameObject[] clueObjects = GameObject.FindGameObjectsWithTag(clue);
        if (clueObjects.Length > 0)
        {
            clues = new ClueController[clueObjects.Length];
            foreach (GameObject obj in clueObjects)
            {
                // Extract index from sound name
                int index = int.Parse(obj.name.Substring(clue.Length));
                clues[index] = obj.GetComponent<ClueController>();
                Debug.Log(clues[index].name + " : " + obj.name);
            }
        }

        string inputPanelString = "InputPanel";
        GameObject inputP = GameObject.FindGameObjectWithTag(inputPanelString);
        if (inputP != null)
        {
            inputPanel = inputP.GetComponent<InputZoneController>();
        }

    }

    void GetTextInput()
    {
        string textInput = "TextInput";
        GameObject[] textInputObjects = GameObject.FindGameObjectsWithTag(textInput);
        if (textInputObjects.Length > 0)
        {
            textInputs = new TextInputController[textInputObjects.Length];
            foreach (GameObject obj in textInputObjects)
            {
                // Extract index from sound name
                int index = int.Parse(obj.name.Substring(textInput.Length));
                textInputs[index] = obj.GetComponent<TextInputController>();
                Debug.Log(textInputs[index].name + " : " + obj.name);
            }
        }

    }


    // Getting the syllable slot manager
    void GetSyllablesObjects()
    {
        wSlotManager = GameObject.FindWithTag("WordSlotManager")?.GetComponent<WordSlotManager>();
    }

    public void ReadyToPlayStandBy()
    {

        if (images != null)
        {
            images[0].SetImage(mediaController.GetImage(testOrder[nextTestIndex], gameConfig.ImageMode), gameConfig.imageDelay);
        }

        if (clues != null)
        {
            clues[0].SetClues(mediaController.GetClues(testOrder[nextTestIndex]), gameConfig.clueDelay);

        }

        if(wSlotManager != null)
        {
            wSlotManager.GetSyllablesElements(mediaController.GetSyllablesPhono(testOrder[nextTestIndex]));
        }

        if(inputPanel != null)
        {
            inputPanel.BuildLayout(mediaController.GetSyllablesPhono(testOrder[nextTestIndex]), mediaController.GetClues(testOrder[nextTestIndex]));
        }

    }

    IEnumerator ComputeNextTest()
    {
        // Diaporama rules


        // Play current index sound
        if(sounds != null)
        {
            sounds[0].SetSound(mediaController.GetAudio(testOrder[nextTestIndex]), gameConfig.audioDelay);
        }



        // Update index for next image
        nextTestIndex = nextTestIndex >= testOrder.Count - 1 ? 0 : nextTestIndex + 1;



        // Wait 1 frame for sound source to start playing
        yield return null;


        yield return new WaitWhile(() => sounds[0].src.isPlaying);

        if (inputPanel != null)
        {
            inputPanel.BuildLayout(mediaController.GetSyllablesPhono(testOrder[nextTestIndex]), mediaController.GetClues(testOrder[nextTestIndex]));
        }

        // Update image and text clue
        if (images != null)
        {
            images[0].SetImage(mediaController.GetImage(testOrder[nextTestIndex], gameConfig.ImageMode), gameConfig.imageDelay);
        }
        

        if(clues != null)
        {
            clues[0].SetClues(mediaController.GetClues(testOrder[nextTestIndex]), gameConfig.clueDelay);

        }

        if (textInputs != null)
        {
            textInputs[0].ClearTextInput();
        }






    }

    IEnumerator ComputeNextSyllableTest()
    {

        // Wait 1 frame for sound source to start playing
        yield return null;


        yield return new WaitWhile(() => sounds[0].src.isPlaying);

        if(wSlotManager != null)
        {
            wSlotManager.GetSyllablesElements(mediaController.GetSyllables(testOrder[nextTestIndex]));
        }

    }


    void BrowseTests(string way)
    {
        if(way == "next")
        {

            if(textInputs != null)
            {
                textInputs[0].ClearTextInput();
            }


            nextTestIndex = nextTestIndex >= testOrder.Count - 1 ? 0 : nextTestIndex + 1;

            if (images != null)
            {
                images[0].SetImage(mediaController.GetImage(testOrder[nextTestIndex], gameConfig.ImageMode), gameConfig.imageDelay);
            }

            if (clues != null)
            {
                clues[0].SetClues(mediaController.GetClues(testOrder[nextTestIndex]), gameConfig.clueDelay);

            }

            if (inputPanel != null)
            {
                inputPanel.BuildLayout(mediaController.GetSyllablesPhono(testOrder[nextTestIndex]), mediaController.GetClues(testOrder[nextTestIndex]));
            }


            // Testing syllables
            if (wSlotManager != null)
            {
                wSlotManager.GetSyllablesElements(mediaController.GetSyllables(testOrder[nextTestIndex]));
            }
        }
        else
        {
            if (nextTestIndex > 0)
            {
                if (textInputs != null)
                {
                    textInputs[0].ClearTextInput();
                }


                nextTestIndex = nextTestIndex > testOrder.Count - 1 ? 0 : nextTestIndex - 1;

                if (images != null)
                {
                    images[0].SetImage(mediaController.GetImage(testOrder[nextTestIndex], gameConfig.ImageMode), gameConfig.imageDelay);
                }

                if (clues != null)
                {
                    clues[0].SetClues(mediaController.GetClues(testOrder[nextTestIndex]), gameConfig.clueDelay);

                }

                if (inputPanel != null)
                {
                    inputPanel.BuildLayout(mediaController.GetSyllablesPhono(testOrder[nextTestIndex]), mediaController.GetClues(testOrder[nextTestIndex]));
                }


                if (wSlotManager != null)
                {
                    wSlotManager.GetSyllablesElements(mediaController.GetSyllables(testOrder[nextTestIndex]));
                }

            }
            else if (nextTestIndex == 0)
            {
                if (textInputs != null)
                {
                    textInputs[0].ClearTextInput();
                }


                nextTestIndex = testOrder.Count - 1;

                if (images != null)
                {
                    images[0].SetImage(mediaController.GetImage(testOrder[nextTestIndex], gameConfig.ImageMode), gameConfig.imageDelay);
                }

                if (clues != null)
                {
                    clues[0].SetClues(mediaController.GetClues(testOrder[nextTestIndex]), gameConfig.clueDelay);

                }

                if (inputPanel != null)
                {
                    inputPanel.BuildLayout(mediaController.GetSyllablesPhono(testOrder[nextTestIndex]), mediaController.GetClues(testOrder[nextTestIndex]));
                }


                if (wSlotManager != null)
                {
                    wSlotManager.GetSyllablesElements(mediaController.GetSyllables(testOrder[nextTestIndex]));
                }

            }

        }

    }

    void OrderTests()
    {
        //for(int i = nextTestIndex;i<gameConfig.nbTests;i++)
        //{
        //    testOrder[i] = i % mediaController.GetNbMedias();

        //}

    }


    public void OnImageClick()
    {
        // Store user action in GameStats
        if(sounds[0].canClick)
        {
            StartCoroutine(ComputeNextTest());
            StartCoroutine(ComputeNextSyllableTest());

        }

    }


    public void OnButtonClick(string whichWay)
    {
        BrowseTests(whichWay);
    }

    public void OnSyllableClickPlay(SyllableElement syllable)
    {
        if(sounds != null)
        {
            sounds[1].SetSound(mediaController.GetSyllableAudio(syllable), 0);
        }

    }

    public void OnPlayClick()
    {
        //Store user action in GameStats

        if (sounds != null)
        {
            sounds[0].SetSound(mediaController.GetAudio(testOrder[nextTestIndex]), 0);
        }

    }

    public void AlphabetTyping(string lastInput)
    {
        if (unstable.Contains(GetCurrentMethod()))
            return;

        string key = lastInput.ToLower();
        //Debug.Log("Key pressed: " + key);

        if(sounds != null)
        {
            sounds[1].PlayLetterAudio(mediaController.GetAlphabetAudio(key));
        }


    }


    public void OnCheckInput(string text)
    {
        // Check user string

        // Store user action in GameStats


        string lexicomixWord = mediaController.GetClues(testOrder[nextTestIndex]).ToLower();
        text = text.ToLower();

        if (text == lexicomixWord)
        {
            StartCoroutine(ComputeNextTest());
            return;
        }

        text = StringTreatment.RemoveDiacritics(text);
        lexicomixWord = StringTreatment.RemoveDiacritics(mediaController.GetClues(testOrder[nextTestIndex]));
        lexicomixWord = lexicomixWord.Replace("-", "").Replace(" ", "");

        if (text == lexicomixWord)
        {
            StartCoroutine(ComputeNextTest());
        }


    }






    [MethodImpl(MethodImplOptions.NoInlining)]
    public string GetCurrentMethod()
    {
        var st = new System.Diagnostics.StackTrace();
        var sf = st.GetFrame(1);

        return sf.GetMethod().Name;
    }



}
