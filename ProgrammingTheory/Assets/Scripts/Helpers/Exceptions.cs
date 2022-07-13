using System;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public partial class Exceptions : MonoBehaviour
    {
        public class DuplicateSingletonException : Exception { public DuplicateSingletonException(string m) : base(m) { } }


        public class GameManagerNotInitializedException : Exception { public GameManagerNotInitializedException(string m) : base(m) { } }

        public class LoadOperationNotContainedInListException : Exception { public LoadOperationNotContainedInListException(string m) : base(m) { } }
    }
}