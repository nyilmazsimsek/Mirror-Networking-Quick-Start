using Mirror;
using TMPro;
using UnityEngine;

public class PlayerScript : NetworkBehaviour
{
    public TextMeshPro _playerNameText;
    public GameObject _floatingInfo;

    private Material _playerMaterialClone;

    [SyncVar(hook = nameof(OnNameChanged))]
    public string _playerName;
    
    [SyncVar(hook = nameof(OnColorChanged))]
    public Color _playerColor = Color.white;

    private SceneScript _sceneScript;

    private int selectedWeaponLocal = 1;
    public GameObject[] weaponArray;

    [SyncVar(hook = nameof(OnWeaponChanged))]
    public int activeWeaponSynced = 1;
    private void Awake()
    {
        _sceneScript = GameObject.FindObjectOfType<SceneScript>();
        _sceneScript = GameObject.Find("SceneReference").GetComponent<SceneReference>().sceneScript;
        
        // disable all weapons
        foreach (var item in weaponArray)
        {
            if (item != null)
            { 
                item.SetActive(false); 
            }
        }
    }
    
    public override void OnStartLocalPlayer()
    {
        _sceneScript._playerScript = this;
        Camera.main.transform.SetParent(transform);
        Camera.main.transform.localPosition = new Vector3(0, 0, 0);
            
        _floatingInfo.transform.localPosition = new Vector3(0, -0.3f, 0.6f);
        _floatingInfo.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        string name = "Player" + Random.Range(100, 999);
        Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        CmdSetupPlayer(name, color);
    }
    
    void Update()
    {
        if (!isLocalPlayer)
        {
            // make non-local players run this
            _floatingInfo.transform.LookAt(Camera.main.transform);
            return;
        }

        float moveX = Input.GetAxis("Horizontal") * Time.deltaTime * 110.0f;
        float moveZ = Input.GetAxis("Vertical") * Time.deltaTime * 4f;

        transform.Rotate(0, moveX, 0);
        transform.Translate(0, 0, moveZ);
        
        if (Input.GetButtonDown("Fire2")) //Fire2 is mouse 2nd click and left alt
        {
            selectedWeaponLocal += 1;

            if (selectedWeaponLocal > weaponArray.Length) 
            {
                selectedWeaponLocal = 1; 
            }

            CmdChangeActiveWeapon(selectedWeaponLocal);
        }
    }

    [Command]
    public void CmdSetupPlayer(string _name, Color _col)
    {
        // player info sent to server, then server updates sync vars which handles it on all clients
        _playerName = _name;
        _playerColor = _col;
        _sceneScript._statusText = $"{_playerName} joined.";
    }
    
    [Command]
    public void CmdSendPlayerMessage()
    {
        if (_sceneScript) 
        { 
            _sceneScript._statusText = $"{_playerName} says hello {Random.Range(10, 99)}";
        }
    }
    
    [Command]
    public void CmdChangeActiveWeapon(int newIndex)
    {
        activeWeaponSynced = newIndex;
    }

   
    void OnNameChanged(string _Old, string _New)
    {
        _playerNameText.text = _playerName;
    }

    void OnColorChanged(Color _Old, Color _New)
    {
        _playerNameText.color = _New;
        _playerMaterialClone = new Material(GetComponent<Renderer>().material);
        _playerMaterialClone.color = _New;
        GetComponent<Renderer>().material = _playerMaterialClone;
    }
    
    void OnWeaponChanged(int _Old, int _New)
    {
        // disable old weapon
        // in range and not null
        if (0 < _Old && _Old < weaponArray.Length && weaponArray[_Old] != null)
        {
            weaponArray[_Old].SetActive(false);
        }
    
        // enable new weapon
        // in range and not null
        if (0 < _New && _New < weaponArray.Length && weaponArray[_New] != null)
        {
            weaponArray[_New].SetActive(true);
        }
    }

}
