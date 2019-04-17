namespace SCide.WPF.Helpers
{
    public class IdentifierMatch
    {
        internal IdentifierMatch(int line, string identifier)
        {
            this.Line = line;
            this.Identifier = identifier;
        }

        public string Identifier { get; set; }
        public int Line { get; set; }

        public override string ToString()
        {
            return Identifier;
        }
    }
}
