using UnityEngine;

public class TweetBlockManager : MonoBehaviour
{
    string tweetText;
    string pageURL;
    string hashTag;
    private AudioSource audioSource;
    private GameManager gameManager;

    void Start()
    {
        //各プロジェクトにあったテキストを入力
        tweetText = "全ての箱を落としました。";
        pageURL = "https://tsumuzh.github.io/games/hakootoshi2/index.html";
        hashTag = "";
        audioSource = GetComponent<AudioSource>();
        gameManager = GameObject.Find("Manager").GetComponent<GameManager>();
    }
    void Update()
    {
        if (transform.position.y < -7)
        {
            Tweet();
            gameManager.isPhaseTransitioning = false;
            Destroy(gameObject);
        }
    }
    public void Tweet()
    {
        var url = "https://twitter.com/intent/tweet?" + "text=" + tweetText + "&hashtags=" + hashTag + "&url=" + pageURL;

        Application.ExternalEval(string.Format("window.open('{0}','_blank')", url));
        Application.OpenURL(url);
    }
    private void OnCollisionEnter(Collision other)
    {
        GetComponent<Rigidbody>().useGravity = true;
        audioSource.PlayOneShot(audioSource.clip);
        gameManager.isPhaseTransitioning = true;
    }
}
