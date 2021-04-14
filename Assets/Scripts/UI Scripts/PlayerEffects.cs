using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

using DigitalRuby.Tween;
using Mirror.Cloud.Examples.Pong;

public class PlayerEffects : NetworkBehaviour
{
    [SerializeField]
    private Canvas _canvas = null;
    [SerializeField]
    private GameObject _hitmarker = null;
    [SerializeField]
    private GameObject _damageTakenText = null;
    [SerializeField]
    private Image _killFeed = null;

    [SerializeField]
    private GameObject _killFeedPrefab = null;

    [SerializeField]
    private GameObject _blastedPrefab = null;

    [SerializeField]
    private GameObject _enemyKilled = null;

    private float _growSize = 0.75f;


    //Creates a hitmarker.
    //Run from PlayerHealth and tells client they hit someone
    [TargetRpc]
    public void CreateHitmarker(int DamageDealt)
    {
        GameObject Clone = Instantiate(_hitmarker, _canvas.transform);
        //Set damage text
        GameObject Dmg = Clone.transform.GetChild(0).gameObject;
        Dmg.GetComponent<Text>().text = DamageDealt.ToString();
        int Rot = Random.Range(-25, 25);
        if (Rot < 10 && Rot >= 0)
        {
            Rot = 10;
        } else if (Rot > -10 && Rot < 0)
        {
            Rot = -10;
        }
        Dmg.transform.rotation = Quaternion.Euler(new Vector3(0, Rot * 1.5f, Rot));
        Color tempColor = new Color(1, (1f / (DamageDealt/4)), (1f / (DamageDealt / 4)));
        Dmg.GetComponent<Text>().color = tempColor;
        StartCoroutine(MoveObject(Dmg, Dmg.transform.position + new Vector3(Rot * -1, Random.Range(10, 25), 0), 0.4f));
        StartCoroutine(HitMarkerEffect(Clone, 1, 0, 0.5f));
    }


    //Creates the kill marker and adds it to
    //the function
    [TargetRpc]
    public void CreateKillmarker()
    {
        GameObject Clone = Instantiate(_enemyKilled, _canvas.transform);
        StartCoroutine(HitMarkerEffect(Clone, 1, 0, 0.5f));
    }


    //Creates a death display that starts center of the screen and tweens up
    [TargetRpc]
    public void ShowDeathDisplay()
    {
        float TimeToDestroy = 9f;
        GameObject Clone = Instantiate(_blastedPrefab, _canvas.transform);
        Clone.transform.GetChild(1).gameObject.GetComponent<Text>().text = ReturnRandomHint();
        StartCoroutine(MoveObject(Clone, Clone.transform.position + new Vector3(0,600,0), TimeToDestroy));
    }

    //Returns a random hint
    //Can add more before release
    private string ReturnRandomHint()
    {
        string[] Hints = new string[] { 
            "Pay attention to the mini-map! It's the perfect way to find targets.", 
            "Make sure to use your weapons effectivley. Shotguns are better upclose while rifles are good in middle ground.", 
            "That looks like it hurt...", 
            "Watch out for crowded areas! Lots of blasters are in play!",};
        return Hints[Random.Range(0, Hints.Length)];
    }

    //Creates a pop up near health bar
    //Allows players to know how much damage they took
    public void DamageTakenText(int DamageDealt)
    {
        GameObject Clone = Instantiate(_damageTakenText, _canvas.transform);
        //Set damage text
        Clone.GetComponent<Text>().text = DamageDealt.ToString();
        int Rot = Random.Range(-10, 30);
        Clone.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -45 + Rot));
        Color tempColor = new Color(1, (1f / (DamageDealt / 6)), (1f / (DamageDealt / 6)));
        Clone.GetComponent<Text>().color = tempColor;
        StartCoroutine(MoveObject(Clone, Clone.transform.position + new Vector3(Random.Range(30, 50) + Rot, Random.Range(30, 50) + Rot, 0), 1.5f));
    }


    [TargetRpc]
    //Creates a kill feed object
    public void CreateKillFeed(string Player1, string Player2)
    {
        //Create kill effect and set the outputs
        GameObject Clone = Instantiate(_killFeedPrefab, _killFeed.transform);
        GameObject Shooting = Clone.transform.GetChild(0).gameObject;
        Shooting.GetComponent<Text>().text = Player1;

        GameObject How = Shooting.transform.GetChild(0).gameObject;
        GameObject Elimed = How.transform.GetChild(0).gameObject;
        Elimed.GetComponent<Text>().text = Player2;

        //Get preferred values
        //Find the value for the bar to be stretched
        float Val = (Shooting.GetComponent<Text>().preferredWidth +
            How.GetComponent<Text>().preferredWidth +
            Elimed.GetComponent<Text>().preferredWidth + 20);

        GameObject Bar = Clone.transform.GetChild(1).gameObject;
        Bar.GetComponent<RectTransform>().sizeDelta = new Vector2(Val, 40);
        Destroy(Clone, 10f);
    }

    /// <summary>
    /// Creates a tween effect that later destroys it after the lerp is complete
    /// </summary>
    /// <param name="go"> Game object to move </param>
    /// <param name="end"> End vector 3 position to go to</param>
    /// <param name="lerpTime"> Time it takes to lerp (move to point) </param>
    /// <returns></returns>
    public IEnumerator MoveObject(GameObject go, Vector3 end, float lerpTime)
    {
        float _timeStartedLerping = Time.time;
        float _timeSinceLast = Time.time - _timeStartedLerping;
        float _completedPercent = _timeSinceLast / lerpTime;
        Vector3 start = go.transform.position;

        while (true)
        {
            _timeSinceLast = Time.time - _timeStartedLerping;
            _completedPercent = _timeSinceLast / lerpTime;

            //Set the Image alpha (fade)
            Vector3 currVal = Vector3.Lerp(start, end, _completedPercent);
            go.transform.position = currVal;
            if (_completedPercent >= 1) break;

            yield return null;
        }
        Destroy(go);
    }


    //Creates a color changing X that is a hitmarker
    public IEnumerator HitMarkerEffect(GameObject go, float start, float end, float lerpTime)
    {
        float _timeStartedLerping = Time.time;
        float _timeSinceLast = Time.time - _timeStartedLerping;
        float _completedPercent = _timeSinceLast / lerpTime;

        while (true)
        {
            _timeSinceLast = Time.time - _timeStartedLerping;
            _completedPercent = _timeSinceLast / lerpTime;

            //Set the scale
            float currSizeVal = Mathf.Lerp(1, _growSize, _completedPercent);
            go.transform.localScale = new Vector3(currSizeVal, currSizeVal, currSizeVal);

            //Set the Image alpha (fade)
            float currVal = Mathf.Lerp(start, end, _completedPercent);
            Image newImg = go.GetComponent<Image>();
            if (newImg != null)
            {
                Color tempColor = newImg.color;
                tempColor.a = currVal;
                newImg.color = tempColor;
            }
            if (_completedPercent >= 1) break;

            yield return null;
        }
        Destroy(go);
    }
}
