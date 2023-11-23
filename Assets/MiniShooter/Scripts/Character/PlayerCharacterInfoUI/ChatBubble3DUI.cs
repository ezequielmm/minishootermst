using MasterServerToolkit.UI;
using MasterServerToolkit.Utils;
using UnityEngine;

namespace MiniShooter
{
    public class ChatBubble3DUI : MonoBehaviour
    {
        #region INSPECTOR

        [SerializeField]
        private UILable chatBubbleText;

        #endregion

        private TweenerActionInfo showChatBubbleAction = new TweenerActionInfo();
        private TweenerActionInfo delayChatBubbleAction = new TweenerActionInfo();

        private void Start()
        {
            transform.localScale = Vector3.zero;
        }

        public void Show(string message, float showTime = 5f)
        {
            if (!gameObject.activeInHierarchy) return;

            chatBubbleText.Text = message;

            Tweener.Cancel(showChatBubbleAction);
            Tweener.Cancel(delayChatBubbleAction);

            showChatBubbleAction = Tweener.Tween(0f, 1f, 0.25f, (value) =>
            {
                transform.localScale = Vector3.one * value;
            }).OnComplete((id) =>
            {
                delayChatBubbleAction = Tweener.DelayedCall(showTime, () =>
                {
                    transform.localScale = Vector3.zero;
                });
            });
        }

        private void OnDestroy()
        {
            Tweener.Cancel(showChatBubbleAction);
            Tweener.Cancel(delayChatBubbleAction);
        }
    }
}