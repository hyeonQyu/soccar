using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    [SerializeField]
    private Image _logoPanel;
    [SerializeField]
    private Image _mainScreenPanel;
    [SerializeField]
    private Image _pressAnyKey;

    public static bool IsLogoDestroyed { get; set; }
    public static bool IsMainDestroyed { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        Animator logoAnimator = _logoPanel.GetComponent<Animator>();
        logoAnimator.SetBool("isDestroy", true);
    }

    // Update is called once per frame
    void Update()
    {
        // 아무 키나 누르면 Main Screen이 사라지고 로그인 화면으로 이동
        if(IsLogoDestroyed && Input.anyKeyDown)
        {
            Destroy(_pressAnyKey.gameObject);
            Animator mainScreenAnimator = _mainScreenPanel.GetComponent<Animator>();
            mainScreenAnimator.SetBool("isDestroy", true);
        }

        if(IsMainDestroyed)
            SceneManager.LoadScene("Lobby Scene");
    }
}
