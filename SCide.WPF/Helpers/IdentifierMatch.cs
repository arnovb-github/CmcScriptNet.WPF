namespace SCide.WPF.Helpers
{
    public class IdentifierMatch
    {
        internal IdentifierMatch(int line, string identifier)
        {
            Line = line;
            Identifier = identifier;
        }

        public string Identifier { get; }
        public int Line { get; }

        public override string ToString()
        {
            return Identifier;
        }
    }
}
