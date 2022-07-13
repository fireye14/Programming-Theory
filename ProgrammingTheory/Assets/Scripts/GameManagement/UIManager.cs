using Assets.Scripts.GameManagement.BaseTypes;

namespace Assets.Scripts.GameManagement
{
    public class UIManager : SystemManagerBase<UIManager, GameManager>
    {
        protected override void AwakeSystemManager()
        {
            //throw new System.NotImplementedException();
            //var a = SystemManagerTypes;
            var a = GM;
        }

        protected override void OnDestroySystemManager()
        {
            throw new System.NotImplementedException();
        }
    }
}
