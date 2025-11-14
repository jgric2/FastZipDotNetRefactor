internal static class Cli
{
    internal enum CommandType
    {
        None,
        // Viewer / UI
        Open,
        Stage,

        // Extract
        ExtractHere,
        ExtractSmart,
        ExtractTo,
        ExtractEach,
        ExtractToDesktop,

        // Create
        Create,          // legacy: -c <out.zip> <inputs...>
        CreateQuick,
        CreateTo,
        CreateEach,
        CreateToDesktop,
        CreateAES,       // prompt password, AES-256
        CreateStore,     // force Store
        AddToExisting,   // append to a chosen .zip

        // Folder-special create
        CreateFolder,        // compress given folder path to parent\name.zip
        CreateFolderAES,     // same, AES-256

        // Tools on zip
        Test,
        Repair,
        Comment,
        Sfx
    }

    internal sealed class Command
    {
        public CommandType Type { get; set; } = CommandType.None;

        // Common inputs (files/folders to compress, or selection hints)
        public List<string> Inputs { get; set; } = new();

        // For single-archive actions
        public string Archive { get; set; }       // path to a .zip
        public string TargetDir { get; set; }     // extract-to
        public string OutArchive { get; set; }    // create legacy output

        // Modifiers
        public bool DeleteZip { get; set; }       // after extract
        public bool ForceStore { get; set; }      // create using Store
        public bool ForceAES { get; set; }        // create using AES-256
        public bool ToDesktop { get; set; }       // target is Desktop
        public bool CreateFolder { get; set; }    // special folder-create verb
    }

    public static Command Parse(string[] args)
    {
        var cmd = new Command { Type = CommandType.None };
        if (args == null || args.Length == 0)
            return cmd;

        // Helpers
        static bool Eq(string a, string b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        bool HasFlag(string flag) => args.Any(a => Eq(a, flag));

        // First token usually determines the verb
        string first = args[0];

        // Global modifiers (can appear after the verb)
        bool deleteZip = HasFlag("--delete-zip");

        // 1) Stage to UI
        if (Eq(first, "--stage") || Eq(first, "-stage"))
        {
            cmd.Type = CommandType.Stage;
            cmd.Inputs = args.Skip(1).Where(s => !Eq(s, "--delete-zip")).ToList();
            return cmd;
        }

        // 2) Legacy create
        if (Eq(first, "-c"))
        {
            if (args.Length >= 3)
            {
                cmd.Type = CommandType.Create;
                cmd.OutArchive = args[1];
                cmd.Inputs = args.Skip(2).ToList();
            }
            return cmd;
        }

        // 3) Legacy single-file extract
        if (Eq(first, "-x") && args.Length >= 2)
        {
            cmd.Type = CommandType.ExtractSmart;
            cmd.Archive = args[1];
            cmd.DeleteZip = deleteZip;
            return cmd;
        }
        if (Eq(first, "-xh") && args.Length >= 2)
        {
            cmd.Type = CommandType.ExtractHere;
            cmd.Archive = args[1];
            cmd.DeleteZip = deleteZip;
            return cmd;
        }
        if (Eq(first, "-xto") && args.Length >= 3)
        {
            cmd.Type = CommandType.ExtractTo;
            cmd.TargetDir = args[1];
            cmd.Archive = args[2];
            cmd.DeleteZip = deleteZip;
            return cmd;
        }

        // 4) Extract (selection-aware)
        if (Eq(first, "--extract-here"))
        {
            cmd.Type = CommandType.ExtractHere;
            cmd.Archive = args.Skip(1).FirstOrDefault(s => !Eq(s, "--delete-zip"));
            cmd.DeleteZip = deleteZip;
            return cmd;
        }
        if (Eq(first, "--extract-smart"))
        {
            cmd.Type = CommandType.ExtractSmart;
            cmd.Archive = args.Skip(1).FirstOrDefault(s => !Eq(s, "--delete-zip"));
            cmd.DeleteZip = deleteZip;
            return cmd;
        }
        if (Eq(first, "--extract-to"))
        {
            cmd.Type = CommandType.ExtractTo;
            cmd.Archive = args.Skip(1).FirstOrDefault(s => !Eq(s, "--delete-zip"));
            cmd.DeleteZip = deleteZip;
            return cmd;
        }
        if (Eq(first, "--extract-each"))
        {
            cmd.Type = CommandType.ExtractEach;
            cmd.Archive = args.Skip(1).FirstOrDefault(s => !Eq(s, "--delete-zip"));
            cmd.DeleteZip = deleteZip;
            return cmd;
        }
        if (Eq(first, "--extract-to-desktop"))
        {
            cmd.Type = CommandType.ExtractToDesktop;
            cmd.Archive = args.Skip(1).FirstOrDefault(s => !Eq(s, "--delete-zip"));
            cmd.ToDesktop = true;
            cmd.DeleteZip = deleteZip;
            return cmd;
        }

        // 5) Create (selection-aware)
        if (Eq(first, "--create-quick"))
        {
            cmd.Type = CommandType.CreateQuick;
            cmd.Inputs = args.Skip(1).ToList();
            return cmd;
        }
        if (Eq(first, "--create-to"))
        {
            cmd.Type = CommandType.CreateTo;
            cmd.Inputs = args.Skip(1).ToList();
            return cmd;
        }
        if (Eq(first, "--create-each"))
        {
            cmd.Type = CommandType.CreateEach;
            cmd.Inputs = args.Skip(1).ToList();
            return cmd;
        }
        if (Eq(first, "--create-to-desktop"))
        {
            cmd.Type = CommandType.CreateToDesktop;
            cmd.Inputs = args.Skip(1).ToList();
            cmd.ToDesktop = true;
            return cmd;
        }
        if (Eq(first, "--create-aes"))
        {
            cmd.Type = CommandType.CreateAES;
            cmd.Inputs = args.Skip(1).ToList();
            cmd.ForceAES = true;
            return cmd;
        }
        if (Eq(first, "--create-store"))
        {
            cmd.Type = CommandType.CreateStore;
            cmd.Inputs = args.Skip(1).ToList();
            cmd.ForceStore = true;
            return cmd;
        }
        if (Eq(first, "--add-to"))
        {
            cmd.Type = CommandType.AddToExisting;
            cmd.Inputs = args.Skip(1).ToList();
            return cmd;
        }

        // 6) Folder-special create
        if (Eq(first, "--create-folder"))
        {
            cmd.Type = CommandType.CreateFolder;
            cmd.Inputs = args.Skip(1).ToList(); // expects a single folder path (Directory / Background verbs)
            cmd.CreateFolder = true;
            return cmd;
        }
        if (Eq(first, "--create-folder-aes"))
        {
            cmd.Type = CommandType.CreateFolderAES;
            cmd.Inputs = args.Skip(1).ToList();
            cmd.CreateFolder = true;
            cmd.ForceAES = true;
            return cmd;
        }

        // 7) Zip tools
        if (Eq(first, "--test"))
        {
            cmd.Type = CommandType.Test;
            cmd.Archive = args.Skip(1).FirstOrDefault();
            return cmd;
        }
        if (Eq(first, "--repair"))
        {
            cmd.Type = CommandType.Repair;
            cmd.Archive = args.Skip(1).FirstOrDefault();
            return cmd;
        }
        if (Eq(first, "--comment"))
        {
            cmd.Type = CommandType.Comment;
            cmd.Archive = args.Skip(1).FirstOrDefault();
            return cmd;
        }
        if (Eq(first, "--sfx"))
        {
            cmd.Type = CommandType.Sfx;
            cmd.Archive = args.Skip(1).FirstOrDefault();
            return cmd;
        }

        // 8) Double-click a .zip (open viewer)
        try
        {
            if (Path.GetExtension(first).Equals(".zip", StringComparison.OrdinalIgnoreCase))
            {
                cmd.Type = CommandType.Open;
                cmd.Archive = first;
                return cmd;
            }
        }
        catch { }

        // 9) No match → None
        cmd.Type = CommandType.None;
        return cmd;
    }
}

//internal static class Cli
//{
//    internal enum CommandType
//    {
//        None, Open,
//        ExtractHere, ExtractSmart, ExtractTo, ExtractEach,
//        Create, CreateQuick, CreateTo, CreateEach
//    }

//    internal sealed class Command
//    {
//        public CommandType Type { get; set; }
//        public string Archive { get; set; }         // single archive (optional)
//        public string TargetDir { get; set; }       // for extract-to (optional)
//        public string OutArchive { get; set; }      // for old -c
//        public List<string> Inputs { get; set; } = new();
//    }

//    public static Command Parse(string[] args)
//    {
//        if (args == null || args.Length == 0) return new Command { Type = CommandType.None };

//        string first = args[0];

//        // Stage (existing)
//        if (Equals(first, "--stage") || Equals(first, "-stage"))
//            return new Command { Type = CommandType.Open, Inputs = args.Skip(1).ToList() };

//        // Old create CLI (existing)
//        if (Equals(first, "-c"))
//        {
//            if (args.Length < 3) return new Command { Type = CommandType.None };
//            return new Command { Type = CommandType.Create, OutArchive = args[1], Inputs = args.Skip(2).ToList() };
//        }

//        // Existing single-file extract switches
//        if (Equals(first, "-x") && args.Length >= 2)
//            return new Command { Type = CommandType.ExtractSmart, Archive = args[1] };
//        if (Equals(first, "-xh") && args.Length >= 2)
//            return new Command { Type = CommandType.ExtractHere, Archive = args[1] };
//        if (Equals(first, "-xto") && args.Length >= 3)
//            return new Command { Type = CommandType.ExtractTo, TargetDir = args[1], Archive = args[2] };

//        // NEW selection-aware switches (Explorer submenu pattern)
//        if (Equals(first, "--extract-here"))
//            return new Command { Type = CommandType.ExtractHere, Archive = args.Skip(1).FirstOrDefault() };
//        if (Equals(first, "--extract-smart"))
//            return new Command { Type = CommandType.ExtractSmart, Archive = args.Skip(1).FirstOrDefault() };
//        if (Equals(first, "--extract-to"))
//            return new Command { Type = CommandType.ExtractTo, Archive = args.Skip(1).FirstOrDefault() };
//        if (Equals(first, "--extract-each"))
//            return new Command { Type = CommandType.ExtractEach, Archive = args.Skip(1).FirstOrDefault() };

//        if (Equals(first, "--create-quick"))
//            return new Command { Type = CommandType.CreateQuick };
//        if (Equals(first, "--create-to"))
//            return new Command { Type = CommandType.CreateTo };
//        if (Equals(first, "--create-each"))
//            return new Command { Type = CommandType.CreateEach };

//        // Double-click .zip path (open viewer)
//        if (Path.GetExtension(first).Equals(".zip", StringComparison.OrdinalIgnoreCase))
//            return new Command { Type = CommandType.Open, Archive = first };

//        return new Command { Type = CommandType.None };
//    }

//    private static bool Equals(string a, string b) =>
//        string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
//}