using UnityEngine;
using FMODUnity;

/// <summary>
/// Zarządza głośnością ścieżek audio poprzez FMOD VCAs.
/// </summary>
public class VCA : MonoBehaviour
{
    private FMOD.Studio.VCA globalVCA;
    private FMOD.Studio.VCA musicVCA;
    private FMOD.Studio.VCA tavernVCA;
    private FMOD.Studio.VCA outsideVCA;

    [SerializeField] private bool globalMuteActive = false;
    [SerializeField] private bool musicMuteActive = false;
    [SerializeField] private bool tavernMuteActive = false;
    [SerializeField] private bool outsideMuteActive = false;

    void Start()
    {
        globalVCA = RuntimeManager.GetVCA("vca:/Mute");
        musicVCA = RuntimeManager.GetVCA("vca:/Music");
        tavernVCA = RuntimeManager.GetVCA("vca:/Tavern_amb");
        outsideVCA = RuntimeManager.GetVCA("vca:/Outside_amb");

        // globalMuteActive = true;
        // globalVCA.setVolume(0.0f); 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            ToggleMute(globalVCA, ref globalMuteActive);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleMute(musicVCA, ref musicMuteActive);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            ToggleMute(tavernVCA, ref tavernMuteActive);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            ToggleMute(outsideVCA, ref outsideMuteActive);
        }
    }

    private void ToggleMute(FMOD.Studio.VCA vca, ref bool muteFlag)
    {
        muteFlag = !muteFlag;

        float volume = muteFlag ? 0.0f : 1.0f;

        vca.setVolume(volume);
    }
}