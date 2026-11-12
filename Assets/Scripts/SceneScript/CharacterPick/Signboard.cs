using UnityEngine;
using UnityEngine.InputSystem;

public class Signboard : MonoBehaviour
{
    [SerializeField] GameObject[] _signboard = new GameObject[2];
    bool[] _isDisplaySignboard = new bool[2] { false, false };
    CharacterSelectManagerBase _characterSelectManager;
    AudioSource _audioSource;
    [SerializeField] AudioClip _audioClip;

    void Start()
    {
        _characterSelectManager = GetComponent<CharacterSelectManagerBase>();
        _audioSource = GetComponent<AudioSource>();
    }

    // 
    public void ToggleDoDisplaySignboard(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (_characterSelectManager.GetReady(Identification(ctx))) return;

        var id = Identification(ctx);
        _isDisplaySignboard[id] = !_signboard[id].activeSelf;
        _signboard[id].SetActive(_isDisplaySignboard[id]);
        _audioSource.PlayOneShot(_audioClip);
    }

    protected virtual int Identification(InputAction.CallbackContext ctx)
    {
        // Ž¯•Ê
        if (Gamepad.all[0] == ctx.control.device) return 0;
        else return 1;
    }

    public bool GetOption(int id) { return _isDisplaySignboard[id]; }

    class MoveSignboard
    {
        bool _isDisplaySignboard = false;
        Vector3 _defaultPosition;
        Vector3 _restartPosition;
        Vector3 _moveEndPosition;
    }
}