internal static class Cli
{
    internal enum CommandType
    {
        None, Open,
        ExtractHere, ExtractSmart, ExtractTo, ExtractEach,
        Create, CreateQuick, CreateTo, CreateEach
    }

    internal sealed class Command
    {
        public CommandType Type { get; set; }
        public string Archive { get; set; }         // single archive (optional)
        public string TargetDir { get; set; }       // for extract-to (optional)
        public string OutArchive { get; set; }      // for old -c
        public List<string> Inputs { get; set; } = new();
    }

    public static Command Parse(string[] args)
    {
        if (args == null || args.Length == 0) return new Command { Type = CommandType.None };

        string first = args[0];

        // Stage (existing)
        if (Equals(first, "--stage") || Equals(first, "-stage"))
            return new Command { Type = CommandType.Open, Inputs = args.Skip(1).ToList() };

        // Old create CLI (existing)
        if (Equals(first, "-c"))
        {
            if (args.Length < 3) return new Command { Type = CommandType.None };
            return new Command { Type = CommandType.Create, OutArchive = args[1], Inputs = args.Skip(2).ToList() };
        }

        // Existing single-file extract switches
        if (Equals(first, "-x") && args.Length >= 2)
            return new Command { Type = CommandType.ExtractSmart, Archive = args[1] };
        if (Equals(first, "-xh") && args.Length >= 2)
            return new Command { Type = CommandType.ExtractHere, Archive = args[1] };
        if (Equals(first, "-xto") && args.Length >= 3)
            return new Command { Type = CommandType.ExtractTo, TargetDir = args[1], Archive = args[2] };

        // NEW selection-aware switches (Explorer submenu pattern)
        if (Equals(first, "--extract-here"))
            return new Command { Type = CommandType.ExtractHere, Archive = args.Skip(1).FirstOrDefault() };
        if (Equals(first, "--extract-smart"))
            return new Command { Type = CommandType.ExtractSmart, Archive = args.Skip(1).FirstOrDefault() };
        if (Equals(first, "--extract-to"))
            return new Command { Type = CommandType.ExtractTo, Archive = args.Skip(1).FirstOrDefault() };
        if (Equals(first, "--extract-each"))
            return new Command { Type = CommandType.ExtractEach, Archive = args.Skip(1).FirstOrDefault() };

        if (Equals(first, "--create-quick"))
            return new Command { Type = CommandType.CreateQuick };
        if (Equals(first, "--create-to"))
            return new Command { Type = CommandType.CreateTo };
        if (Equals(first, "--create-each"))
            return new Command { Type = CommandType.CreateEach };

        // Double-click .zip path (open viewer)
        if (Path.GetExtension(first).Equals(".zip", StringComparison.OrdinalIgnoreCase))
            return new Command { Type = CommandType.Open, Archive = first };

        return new Command { Type = CommandType.None };
    }

    private static bool Equals(string a, string b) =>
        string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
}