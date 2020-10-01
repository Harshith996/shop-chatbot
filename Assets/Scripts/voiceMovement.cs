using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class voiceMovement : MonoBehaviour
{
    public AudioSource welcomeAudioSource;
    public AudioSource welcomebackAudioSource;
    public AudioSource shirtIntroAudioSource;
    public AudioSource pantIntroAudioSource;
    public AudioSource sizeAudioSource;
    public AudioSource largeSizeAudioSource;
    public AudioSource mediumSizeAudioSource;
    public AudioSource shirtColourAudioSource;
    public AudioSource pantColourAudioSource;
    public AudioSource showOrdersAudioSource;
    public AudioSource showFinalOrder;
    public AudioSource showSameOrder;
    public AudioSource swapOfferAudioSource;

    public GameObject yellowShirtThumbnail;
    public GameObject blackShirtThumbnail;
    public GameObject yellowPantThumbnail;
    public GameObject blackPantThumbnail;

    public GameObject positionLeft;
    public GameObject positionRight;
    public GameObject positionCentre;
    
    private KeywordRecognizer keywordRecognizer;

    private Dictionary<string, Action> actions = new Dictionary<string, Action>();
    
    private int status;
    private String apparel;
    private String apparel_1;
    private String apparel_2;
    private String colour;
    private String colour_1;
    private String colour_2;
    private String size_1;
    private String size_2;
    private String size_1_check;

    String apparel_1_check;
    String apparel_2_check;

    [SerializeField] private Vector3 target = new Vector3(-3, 4, -1);
    [SerializeField] private float speed = 1;

    private void Start()
    {
        // welcomeAudioSource = GetComponent<AudioSource>();
        actions.Add("hello", Welcome);
        actions.Add("shirt", Shirt);
        actions.Add("yellow", Colour);
        actions.Add("black", Colour);
        actions.Add("sorry", null);
        actions.Add("trousers", Pant);
        actions.Add("yes", null);
        actions.Add("no", null);
        actions.Add("OK", SwapOffer);
        actions.Add("large", null);
        actions.Add("medium", null);


        keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecognisedSpeech;
        keywordRecognizer.Start();
    }

    private void RecognisedSpeech(PhraseRecognizedEventArgs speech)
    {
        Debug.Log(speech.text);
        if (speech.text == "hello" && status != 1)
        {
            actions[speech.text].Invoke();
            status = 1;
        }
        else if (speech.text == "shirt" && status == 1)
        {
            apparel_1 = "shirt";
            apparel = "shirt";
            size_1_check = PlayerPrefs.GetString("size_1");
            if(size_1_check == "large")
            {
                AskLargeSize();
            }
            else if(size_1_check == "medium")
            {
                AskMediumSize();
            }
            else
            {
                AskSize();
            }
            //actions[speech.text].Invoke();
            status = 2;
        }
        else if(speech.text == "trousers" && status == 1)
        {
            apparel_2 = "trousers";
            apparel = "trousers";
            actions[speech.text].Invoke();
            status = 2;
        }
        else if (speech.text == "sorry" && status == 2)
        {
            status = 1;
        }
        else if (speech.text == "yellow" || speech.text == "black" && status == 2)
        {
            colour = speech.text;
            if(apparel == "shirt")
            {
                colour_1 = speech.text;
            }
            else if (apparel == "trousers")
            {
                colour_2 = speech.text;
            }
            actions[speech.text].Invoke();
            status = 3;
        }
        else if (speech.text == "yes" && status == 3)
        {
            apparel_2 = "trousers";
            apparel = "trousers";
            Pant();
            status = 2;
        }
        else if (speech.text == "no" && status == 3)
        {
            showOrder();
            status = 4;
        }
        else if (speech.text == "OK")
        {
            actions[speech.text].Invoke();
            status = 10;
        }
        else if (speech.text == "yes" && status == 10)
        {
            colour_2 = "yellow";
            showOrder();
            status = 4;
        }
        else if (speech.text == "large")
        {
            size_1 = "large";
            PlayerPrefs.SetString("size_1", size_1);
            PlayerPrefs.Save();
            actions["shirt"].Invoke();
        }
        else if (speech.text == "medium")
        {
            size_1 = "medium";
            PlayerPrefs.SetString("size_1", size_1);
            PlayerPrefs.Save();
            actions["shirt"].Invoke();
        }

        
    }
    private void Welcome()
    {
        apparel_1_check = PlayerPrefs.GetString("shirt");
        apparel_2_check = PlayerPrefs.GetString("trousers");
        Debug.Log(apparel_1_check);
        Debug.Log(apparel_2_check);
        if(apparel_1_check == "" && PlayerPrefs.GetString(apparel_2) == "")
        {
            welcomeAudioSource.Play();
        }
        else
        {
            welcomebackAudioSource.Play();
        }
    }
    private void Shirt()
    {
        shirtIntroAudioSource.Play();
    }
    private void Pant()
    {
        pantIntroAudioSource.Play();
    }
    private void Colour()
    {
        if (apparel_1 != null && apparel_2 != null)
        {
            PlayerPrefs.SetString(apparel_1, colour_1);
            PlayerPrefs.SetString(apparel_2, colour_2);
            PlayerPrefs.Save();
            Debug.Log(apparel_1_check);
            Debug.Log(apparel_2_check);
            Debug.Log(colour_1);
            Debug.Log(colour_2);
            if (colour_1 == apparel_1_check && colour_2 == apparel_2_check)
            {
                showSameOrder.Play();
            }
            else
            {
                showOrdersAudioSource.Play();
            }
            if (colour_1 == "yellow")
            {
                yellowShirtThumbnail.transform.position = positionLeft.transform.position;
            }
            else if (colour_1 == "black")
            {
                blackShirtThumbnail.transform.position = positionLeft.transform.position;
            }
            if (colour_2 == "yellow")
            {
                yellowPantThumbnail.transform.position = positionRight.transform.position;
            }
            else if (colour_2 == "black")
            {
                blackPantThumbnail.transform.position = positionRight.transform.position;
            }
        }
        else if (apparel == "shirt")
        {
            shirtColourAudioSource.Play();
        }
        else if (apparel == "trousers")
        {
            pantColourAudioSource.Play();
        }
    }

    private void showOrder()
    {
        PlayerPrefs.SetString(apparel_1, colour_1);
        PlayerPrefs.SetString(apparel_2, colour_2);
        PlayerPrefs.Save();
        Debug.Log(apparel_1_check);
        Debug.Log(apparel_2_check);
        if (colour_1 == apparel_1_check && colour_2 == apparel_2_check)
        {
            showSameOrder.Play();
        }
        else
        {
            showOrdersAudioSource.Play();
        }
        if (colour_1 == "yellow")
        {
            yellowShirtThumbnail.transform.position = positionLeft.transform.position;
        }
        else if (colour_1 == "black")
        {
            blackShirtThumbnail.transform.position = positionLeft.transform.position;
        }
        if (colour_2 == "yellow")
        {
            yellowPantThumbnail.transform.position = positionRight.transform.position;
        }
        else if (colour_2 == "black")
        {
            blackPantThumbnail.transform.position = positionRight.transform.position;
        }
    }

    private void SwapOffer()
    {
        swapOfferAudioSource.Play();
    }
    private void AskSize()
    {
        sizeAudioSource.Play();
    }
    private void AskLargeSize()
    {
        largeSizeAudioSource.Play();
    }
    private void AskMediumSize()
    {
        mediumSizeAudioSource.Play();
    }

}
