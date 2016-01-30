namespace Assets.GGJ2016.Scripts
{
    public class StateChange<T>
    {
        public T Previous { get; private set; }
        public T Next { get; private set; }

        public StateChange(T previous, T next)
        {
            Previous = previous;
            Next = next;
        }

        public override string ToString()
        {
            return string.Format("(Previous: {0}, Next: {1})", Previous, Next);
        }
    }
}
