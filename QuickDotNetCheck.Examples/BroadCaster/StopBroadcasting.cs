namespace QuickDotNetCheck.Examples.BroadCaster
{
    public class StopBroadcasting : Fixture
    {
        public override bool CanAct()
        {
            return BroadcasterSuite.ThreadSwitch;
        }

        protected override void Act()
        {
            BroadcasterSuite.Thread.Join();
            BroadcasterSuite.Thread = null;
            BroadcasterSuite.ThreadSwitch = false;
        }

        //public Spec DoesNotThrow()
        //{
        //    return new Spec(() => Ensure.Null(Broadcast.ExceptionFromThread));
        //}
    }
}