using System;
using System.Threading;

namespace QuickDotNetCheck.Examples.BroadCaster
{
    public class Broadcast : Fixture
    {
        public static Exception ExceptionFromThread;

        public override bool CanAct()
        {
            return !BroadcasterSuite.ThreadSwitch;
        }

        protected override void Act()
        {
            BroadcasterSuite.ThreadSwitch = true;
            ExceptionFromThread = null;
            
            BroadcasterSuite.Thread = 
                new Thread(
                    () => ExceptionFromThread =
                        GetExceptionThrownBy(() => 
                            BroadcasterSuite.Broadcaster.Broadcast(null)));

            BroadcasterSuite.Thread.Start();
        }

        public Spec DoesNotThrow()
        {
            return new Spec(() => Ensure.Null(ExceptionFromThread));
        }

        private static Exception GetExceptionThrownBy(Action yourCode)
        {
            try { yourCode(); }
            catch (Exception e) { return e; }
            return null;
        }
    }
}