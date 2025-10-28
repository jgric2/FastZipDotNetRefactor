namespace Brutal_Zip
{
    internal static class Cli
    {
        internal enum CommandType { None, Open, ExtractHere, ExtractSmart, ExtractTo, Create }

        internal sealed class Command
        {
            public CommandType Type { get; set; }
            public string Archive { get; set; }
            public string TargetDir { get; set; }
            public string OutArchive { get; set; }
            public List<string> Inputs { get; set; } = new();
        }

        public static Command Parse(string[] args)
        {
            if (args == null || args.Length == 0) return new Command { Type = CommandType.None };

            string first = args[0];

            // stage: just open the UI and pre-stage given paths
            if (string.Equals(first, "--stage", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(first, "-stage", StringComparison.OrdinalIgnoreCase))
            {
                return new Command
                {
                    Type = CommandType.Open, // we’ll show Home + stage items
                    Inputs = args.Skip(1).ToList()
                };
            }

            if (string.Equals(first, "-c", StringComparison.OrdinalIgnoreCase))
            {
                if (args.Length < 3) return new Command { Type = CommandType.None };
                return new Command { Type = CommandType.Create, OutArchive = args[1], Inputs = args.Skip(2).ToList() };
            }

            if (string.Equals(first, "-x", StringComparison.OrdinalIgnoreCase) && args.Length >= 2)
                return new Command { Type = CommandType.ExtractSmart, Archive = args[1] };

            if (string.Equals(first, "-xh", StringComparison.OrdinalIgnoreCase) && args.Length >= 2)
                return new Command { Type = CommandType.ExtractHere, Archive = args[1] };

            if (string.Equals(first, "-xto", StringComparison.OrdinalIgnoreCase) && args.Length >= 3)
                return new Command { Type = CommandType.ExtractTo, TargetDir = args[1], Archive = args[2] };

            if (System.IO.Path.GetExtension(first).Equals(".zip", StringComparison.OrdinalIgnoreCase))
                return new Command { Type = CommandType.Open, Archive = first };

            return new Command { Type = CommandType.None };
        }
    }
}
