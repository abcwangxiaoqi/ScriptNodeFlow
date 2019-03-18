
using System;

namespace ScriptNodeFlow
{
    public abstract class SharedData : IDisposable
    {
        public string flowName;

        public virtual void Dispose()
        {

        }

        public virtual void reset()
        { }
    }
}
