using UnityEngine;
using System.Collections;
using System.IO;
using MTUnity.Utils;
using UnityEngine.UI;
using MTUnity.Actions;
using MTXxtea;
using System;
using System.Collections.Generic;
using MTUnity;
//using Facebook.Unity;

public enum PlayState
{
    Normal,
    Vegas,
    Challenge

}

public enum SettingEnum
{
    PlayState,
    
    SoundControl,
    Hint,
    Draw3,
    VegasCumulative,
    WinningDeals,
    Orientation,

    CongratulationsScreen,
    TapMove,

    Time_Moves,
    RightHanded,
    PlayingLv,
    TutorLv,
    CumulativeNum,
    Installed,

    BgIndex,
    CardBackIndex,

    PlayAdNum,
    GroupId,

    ThemeOn,

    TestLvNum
   
}


public class LimitedQueue<T> : Queue<T>
{
    public int Limit { get; set; }

    public LimitedQueue(int limit) : base(limit)
    {
        Limit = limit;
    }

    public new void Enqueue(T item)
    {
        while (Count >= Limit)
        {
            Dequeue();
        }
        base.Enqueue(item);
    }
}


public class SettingMgr : MonoBehaviour {

    public static SettingMgr current;
    void Awake()
    {

        current = this;
        LoadFile();
        InitState();
        Register();
        StartCoroutine(FindPage());


    }



    public AdState _adState;

    void Register()
    {
        LoadDone += AdMgr.RegisterAllAd;
      
    }


    public Action LoadDone;

 
    void Start()
    {
       // _user = new UserModel();
        //FacebookMgr.current.InitFacebook();

        if (LoadDone != null)
        {
            LoadDone();
            FacebookMgr.current.InitFacebook();
            
        }
    }
    const string settingFileName = "setting.dt";
    public PlayState _state = PlayState.Normal;
    //public UserModel _user;
    public int SoundControl = 1; //bool
    public int Hint = 0;// 0 1 2
    public int Draw3 = 0;//bool
    public int VegasCumulative = 0;//bool
    public int AllWinning = 0;//int 100
    public int Orientation = 0;//int 0竖 1横 2自动 

    public int CongratulationsScreen = 1;//bool
    public int TapMove = 1;//int 0 1 2 on off auto

    public int TimeMode = 0;//bool
    public int RightHanded = 1;//bool

    int _playingIndex = 1;


    int _tLv = 0;
    public int TutorLv {
        set
        {
            _tLv = value;

        }
        get
        {
            return _tLv;
        }
            
    }


    public int CumulitiveNum = 0;

    public int Installed = 0;

    public int _themeOn = 0;

    public int testLv = 1;

   

    public int BgIndex = 1;
    int _cardBackIndex = 0;
    public int CardBackIndex {
        set
        {
            _cardBackIndex = value;

        }
        get
        {
            return _cardBackIndex;
        }
            
    }

    public void AddTestLv()
    {
        if(testLv < MaxSourceLv)
        {
            testLv++;
        }else
        {
            testLv = 1;
        }
        RefreshTestLv();

    }
    public void ReduceTestLv()
    {
        if(testLv >1)
        {
            testLv--;
        }else
        {
            testLv = MaxSourceLv;
        }
        RefreshTestLv();
    }

    public void LoadFile()
    {
        var filePath = GetPath();
        if (!File.Exists(filePath))
        {

            SaveToFile();
            
        }
        LoadSetting();
    }


    public Toggle sound;
    public Toggle Draw3Tog;
    public Toggle allwinning;
    public Toggle vegasmode;
    public Toggle vegascumulative;
    public Toggle timermode;
    public Toggle lefthanded;
    public Toggle autohint;



    public int MaxTutorLv = 20;
    int MaxSourceLv = 440;

    public int PlayNoAdNum = 3;

    const ulong groupAmount = 4;
    int ABTestGroupNum()
    {
       var uuid =  MTTracker.Instance.TrackId.id;

        int groupID = 0;
        if (string.IsNullOrEmpty(uuid))
        {
            groupID = 0;
        }
        else
        {
            groupID = (int)(Convert.ToUInt64(MTSecurity.Md5Sum(uuid).Substring(0, 16), 16) % (ulong)groupAmount);
        }
        return groupID;
    }

    void LoadSetting()
    {
        var bt = File.ReadAllBytes(GetPath());
        string content =MTXXTea.DecryptToString (bt, SKEY); //File.ReadAllText(GetPath());


        MTJSONObject setJs = MTJSON.Deserialize(content);
        if(setJs == null)
        {
            SaveToFile();
        }else
        {
            _state = (PlayState)setJs.GetInt(SettingEnum.PlayState.ToString());
            SoundControl = setJs.GetInt(SettingEnum.SoundControl.ToString());
            Hint = setJs.GetInt(SettingEnum.Hint.ToString());
            Draw3 = setJs.GetInt(SettingEnum.Draw3.ToString());
            VegasCumulative = setJs.GetInt(SettingEnum.VegasCumulative.ToString());
            AllWinning = setJs.GetInt(SettingEnum.WinningDeals.ToString());
            Orientation = setJs.GetInt(SettingEnum.Orientation.ToString());

            CongratulationsScreen = setJs.GetInt(SettingEnum.CongratulationsScreen.ToString());
            TapMove = setJs.GetInt(SettingEnum.TapMove.ToString());

            TimeMode = setJs.GetInt(SettingEnum.Time_Moves.ToString());
            RightHanded = setJs.GetInt(SettingEnum.RightHanded.ToString());
            _playingIndex = setJs.GetInt(SettingEnum.PlayingLv.ToString(),1);
            
            TutorLv = setJs.GetInt(SettingEnum.TutorLv.ToString(),0);

            Installed = setJs.GetInt(SettingEnum.Installed.ToString());

            PlayNoAdNum = setJs.GetInt(SettingEnum.PlayAdNum.ToString());
            CumulitiveNum = setJs.GetInt(SettingEnum.CumulativeNum.ToString());
            _groupId = setJs.GetInt(SettingEnum.GroupId.ToString(), -1);
            _themeOn = setJs.GetInt(SettingEnum.ThemeOn.ToString(), 0);

            if (_groupId == -1)
            {
                _groupId = ABTestGroupNum();
            }



            BgIndex = setJs.GetInt(SettingEnum.BgIndex.ToString(),1);
            CardBackIndex = setJs.GetInt(SettingEnum.CardBackIndex.ToString(),0);


            testLv = setJs.GetInt(SettingEnum.TestLvNum.ToString());
        }

        SetToggleState();

        LoadTrack();
        AddToggleListener();



        OnLoadFinish();
    


    }

    void InitState()
    {
        int adplan = 0;
        int gId = SettingMgr.current.GroupId % 4;
        if (gId == 0)
        {
            adplan = 13000;
            _adState = new AdPlanA();
        }
        else if (gId == 1)
        {
            adplan = 13000;
            _adState = new AdPlanA();
        }
        else if (gId == 2)
        {
            adplan = 14000;
            _adState = new AdPlanB();
        }
        else
        {
            adplan = 14000;
            _adState = new AdPlanB();
        }

        int lvaB = 10;
        if (GroupId % 2 == 0)
        {
            lvaB = 10;
        }
        else
        {
            lvaB = 20;
        }

        int confversion = lvaB + adplan;
        MTTracker.Instance.UpdateConfVersion(confversion);
    }

    int  _groupId = -1;
    public int GroupId{
        get
        {
            if (_groupId == -1)
            {
                _groupId = ABTestGroupNum();
            }
            return _groupId;
        }
        
   }

    #region ThemeOn
    public GameObject ThemeOnMenu;
    const string openThemeUrl = "https://s3-us-west-2.amazonaws.com/magic-solitaire/config/prod/config.json";
    public IEnumerator FindPage()
    {

        //WWW www = new WWW(openThemeUrl);
        //yield return www;
        //var s = www.text;
        //MTJSONObject obj = MTJSON.Deserialize(s);

        //var onffState = obj[back].s;

        //if (onffState.Equals("on"))
        //{
        //    _themeOn = 1;
        //}
        //else
        //{
        //    _themeOn = 0;
        //}
        _themeOn = 1;
        UpdateThemeOn();
        yield return null;

       
    }
    const string back = "background";

    void UpdateThemeOn()
    {

       ThemeOnMenu.SetActive(_themeOn == 1);

    }
    #endregion

    const string TRACK_FILE = "track.tr";

    public LimitedQueue<TrackData> _trackQueue = new LimitedQueue<TrackData>(3);
    void LoadTrack()
    {
        if (!SoFileMgr.Exists(TRACK_FILE))
        {
            for(int i = 0; i < 3; i ++)
            {
                _trackQueue.Enqueue(new TrackData());
            }
            SaveTrack();
        }else
        {
            var content = SoFileMgr.Load(TRACK_FILE);
            var mtJson = MTJSON.Deserialize(content);
            for(int i = 0; i < mtJson.count; i++)
            {
                var trackData = new TrackData();
                trackData.InitBy(mtJson[i]);
                _trackQueue.Enqueue(trackData);
            }
        }
       

    }



   public void SaveTrack()
    {
        MTJSONObject trackList = MTJSONObject.CreateList();
        if(_trackQueue.Count == 0)
        {
            for(int i = 0; i < 3; i ++)
            {
                _trackQueue.Enqueue(new TrackData());
            }
        }

        var trackArr = _trackQueue.ToArray();
        for(int i  = 0; i <trackArr.Length;i++)
        {
            var curData = trackArr[i];
            trackList.Add(curData.ToJson());

        }
        SoFileMgr.Save(TRACK_FILE, trackList.ToString());


    }

    public Text levelNumText;

    void OnLoadFinish()
    {
        RefreshTestLv();
    }

    public void RefreshTestLv()
    {
        levelNumText.text = "LoadLv" + testLv;
    }

    void AddToggleListener()
    {
        sound.onValueChanged.AddListener(OnsoundToggle);
        allwinning.onValueChanged.AddListener(OnallwinningToggle);
        vegasmode.onValueChanged.AddListener(OnvegasmodeToggle);
        vegascumulative.onValueChanged.AddListener(OnvegascumulativeToggle);
        timermode.onValueChanged.AddListener(OntimermodeToggle);

        autohint.onValueChanged.AddListener(OnautohintToggle);
      


    }

    void SetToggleState()
    {
        sound.isOn = SoundControl == 1;
        Draw3Tog.isOn = Draw3== 1;
        allwinning.isOn = AllWinning== 1;
        vegasmode.isOn = _state==PlayState.Vegas;
        vegascumulative.isOn = VegasCumulative== 1;
        timermode.isOn = TimeMode== 1;
        lefthanded.isOn = RightHanded== 0;
        autohint.isOn = Hint== 1;


        TimeOnlyForm.SetActive(TimeMode == 1);
        NormalForm.SetActive(TimeMode != 1);


    }

    void PlayToggleSound()
    {
        SoundManager.Current.Play_switch(0);
    }
    void OnsoundToggle(bool b)
    {
        //Debug.Log("OnsoundToggle" + b.ToString());
        PlayToggleSound(); 
        if (b)
        {
            SoundControl = 1;
        }else
        {
            SoundControl = 0;
        }
    }

    void OnallwinningToggle(bool b)
    {
        PlayToggleSound();
        // Debug.Log("OnallwinningToggle" + b.ToString());
        if (b)
        {
            AllWinning = 1;
        }
        else
        {
            AllWinning = 0;
        }
    }
    void OnvegasmodeToggle(bool b)
    {
        PlayToggleSound();
        //Debug.Log("OnvegasmodeToggle" + b.ToString());
        if (b)
        {
            _state = PlayState.Vegas;
            ShowSwitchVegas(VEGAS_SWITCH_CONTENT);
        }else
        {
            _state = PlayState.Normal;
            ShowSwitchVegas(NORMAL_SWITCH_CONTENT);

        }
    }
    void OnvegascumulativeToggle(bool b)
    {
        PlayToggleSound();
        // Debug.Log("OnvegascumulativeToggle" + b.ToString());
        if (b)
        {
            VegasCumulative = 1;
        }
        else
        {
            if (VegasCumulative == 1)
            {
                ShowVegasConfirmWindow();
               // gameObject.SetActive(false);
            }

            VegasCumulative = 0;
        }
    }
    void OntimermodeToggle(bool b)
    {
        PlayToggleSound();
        TimeOnlyForm.SetActive(b);
        NormalForm.SetActive(!b);
        //Debug.Log("OntimermodeToggle" + b.ToString());
        if (b)
        {
            TimeMode = 1;
           
        }
        else
        {
            TimeMode = 0;
        }
    }

    void OnautohintToggle(bool b)
    {
        PlayToggleSound();
        //Debug.Log("OnautohintToggle" + b.ToString());
        if (b)
        {
            Hint = 1;
        }
        else
        {
            Hint = 0;
        }
    }




    string GetPath()
    {
        return Application.persistentDataPath + "/" + settingFileName;
    }

   public void SaveToFile()
    {
        MTJSONObject setJs = MTJSONObject.CreateDict();
        setJs.Set(SettingEnum.PlayState.ToString(), (int)_state);

        setJs.Set(SettingEnum.SoundControl.ToString(), SoundControl);
        setJs.Set(SettingEnum.Hint.ToString(), Hint);
        setJs.Set(SettingEnum.Draw3.ToString(), Draw3);
        setJs.Set(SettingEnum.VegasCumulative.ToString(), VegasCumulative);
        setJs.Set(SettingEnum.WinningDeals.ToString(), AllWinning);
        setJs.Set(SettingEnum.Orientation.ToString(), Orientation);

        setJs.Set(SettingEnum.CongratulationsScreen.ToString(), CongratulationsScreen);
        setJs.Set(SettingEnum.TapMove.ToString(), TapMove);

        setJs.Set(SettingEnum.Time_Moves.ToString(), TimeMode);
        setJs.Set(SettingEnum.RightHanded.ToString(), RightHanded);

        setJs.Set(SettingEnum.CumulativeNum.ToString(), CumulitiveNum);
        setJs.Set(SettingEnum.PlayAdNum.ToString(), PlayNoAdNum);
        setJs.Set(SettingEnum.TestLvNum.ToString(), testLv);
        setJs.Set(SettingEnum.PlayingLv.ToString(), _playingIndex);
        setJs.Set(SettingEnum.TutorLv.ToString(), TutorLv);
        setJs.Set(SettingEnum.Installed.ToString(), Installed);

        setJs.Set(SettingEnum.GroupId.ToString(), _groupId);

        setJs.Set(SettingEnum.BgIndex.ToString(), BgIndex);
        setJs.Set(SettingEnum.CardBackIndex.ToString(), CardBackIndex);
        setJs.Set(SettingEnum.ThemeOn.ToString(), _themeOn);

        var bt = MTXXTea.Encrypt(setJs.ToString(), SKEY);

        SaveTrack();
        File.WriteAllBytes(GetPath(), bt);
    }

    public static readonly string SKEY = "b8167365ee0a51e4dcc49";


    public void EraseVegasCumulitive()
    {
        CumulitiveNum = 0;
        HideVegasConfirmWindow();
    }

    public void SaveVegasCumulitive()
    {
        HideVegasConfirmWindow();
    }


    public GameObject VegasConfirmWindow;
    public void ShowVegasConfirmWindow()
    {
        VegasConfirmWindow.SetActive(true);
        SoundManager.Current.Play_ui_open(0);

    }

    public SlideIn _vegasConfirmSlid;
    public void HideVegasConfirmWindow()
    {
        if (VegasConfirmWindow.activeSelf)
        {
            SoundManager.Current.Play_ui_close(0);
        }
        var t = _vegasConfirmSlid.SlidOut();

        StartCoroutine(DelayHide(t,VegasConfirmWindow));

    }

    IEnumerator DelayHide(float t,GameObject g)
    {
        yield return new WaitForSeconds(t);
        g.SetActive(false);

    }

    public GameObject NormalForm;
    public GameObject TimeOnlyForm;
    public GameObject HowToPlay;

    public void ShowHowToPlay()
    {
        if(HowToPlay.activeSelf != true)
        {
            SoundManager.Current.Play_ui_open(0);
        }
        HowToPlay.SetActive(true);

       
    }
    public SlideIn _howToSlidein;
    public void HideHowToPlay()
    {
        if (HowToPlay.activeSelf)
        {
            SoundManager.Current.Play_ui_close(0);
        }
       var t=  _howToSlidein.SlidOut();
        StartCoroutine(DelayHide(t, HowToPlay));
        //HowToPlay.SetActive(false);
    }

    public GameObject NoWindealInVegas;
    public void HideVegasWindeal()
    {
        NoWindealInVegas.SetActive(false);
    }
    public void ShowVegasWindeal()
    {
        NoWindealInVegas.SetActive(true);

    }

    [Header("Theme")]
    public Image CardBack;
    public Image BgTheme;

    public GameObject SwitchVegasConfirm;
    public SlideIn _switchVegasSlid;
    public Text _switchContent;
    public void ShowSwitchVegas(string str)
    {
        if (SwitchVegasConfirm != null)
        {
            SwitchVegasConfirm.SetActive(true);
            _switchContent.text = str;


        }
    }

    public void HideSwitchVegas()
    {
        if (SwitchVegasConfirm.activeSelf)
        {
            SoundManager.Current.Play_ui_close(0);
        }
        var t = _switchVegasSlid.SlidOut();

        StartCoroutine(DelayHide(t, SwitchVegasConfirm));
    }

    const string VEGAS_SWITCH_CONTENT = "Set Vegas mode successfully! Please start a new game.";
    const string NORMAL_SWITCH_CONTENT = "Set Normal mode successfully! Please start a new game.";
    internal UserModel _user;
}
