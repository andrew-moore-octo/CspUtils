namespace CspUtils
{
    public static class Parser
    {
        private static readonly CspTokenizer CspTokenizer = new CspTokenizer();
        private static readonly CspParser CspParser = new CspParser();
        
        public static Csp Parse(string policy)
        {
            var tokens = CspTokenizer.Tokenize(policy);
            var model = CspParser.ParseOrThrow(tokens);

            return model;
        }
    }
}