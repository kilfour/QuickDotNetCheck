using System;

namespace QuickDotNetCheck.ShrinkingStrategies
{
    public interface IShrinkingStrategy
    {
        void Shrink(Func<bool> runFunc);
        bool Shrunk();
    }
}