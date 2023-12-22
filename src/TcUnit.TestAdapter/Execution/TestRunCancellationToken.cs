using System;

namespace TcUnit.TestAdapter.Execution
{
    public class TestRunCancellationToken
    {
        private bool _canceled;
        private Action _registeredCallback;

        public bool Canceled
        {
            get => _canceled;

            private set
            {
                _canceled = value;
                if (_canceled)
                {
                    _registeredCallback?.Invoke();
                }
            }
        }

        public void Cancel()
        {
            Canceled = true;
        }

        public void Register(Action callback)
        {
            _ = callback ?? throw new ArgumentNullException(nameof(callback));
            _registeredCallback = callback;
        }

        public void Unregister()
        {
            _registeredCallback = null;
        }
    }
}
