using UnityEngine;
using UnityEngine.UI;

namespace VascoGames.MoreGames
{
    [RequireComponent(typeof(Button))]
    public class MoreGamesUIButton : MonoBehaviour
    {
        protected Button UIButton;  

        protected virtual void Awake()
        {
			UIButton = GetComponent<Button>();
            UIButton.onClick.AddListener(OnClick);           
        }

        public void OnClick()
        {
            MoreGamesManager.Instance.Show();
        }
    }
}