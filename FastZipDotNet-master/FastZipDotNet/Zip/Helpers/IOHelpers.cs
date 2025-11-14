namespace FastZipDotNet.Zip.Helpers
{
    public static class IOHelpers
    {


        /// <summary>
        /// Check if the file is locked
        /// </summary>
        /// <param name="pathFileName">File to check</param>
        /// <returns>True if the file is locked</returns>
        public static bool IsFileLocked(string pathFileName)
        {
            try
            {
                // File exists?
                if (!File.Exists(pathFileName))
                {
                    return false;
                }

                // If can open, it is not locked
                using (FileStream stream = File.Open(pathFileName, FileMode.Open, FileAccess.Write, FileShare.None))
                {
                    stream.Close();
                }

                return false;
            }
            catch
            {
                return true;
            }
        }

        public static string NormalizedFilename(string p)
        {
            if (string.IsNullOrEmpty(p)) return "";
            // normalize slashes
            p = p.Replace('\\', '/');
            
            // remove drive prefix "C:" etc, keep rest unchanged
            int pos = p.IndexOf(':');
            if (pos >= 0)
                p = p.Substring(pos + 1);

            // DO NOT trim trailing slash; just trim leading slashes
            p = p.TrimStart('/');

            // collapse duplicate slashes (optional)
            // while (p.Contains("//")) p = p.Replace("//", "/");

            return p;
        }

        //public static string NormalizedFilename(string _filename)
        //{
        //    try
        //    {
        //        string filename = _filename.Replace('\\', '/');
        //        int pos = filename.IndexOf(':');
        //        if (pos >= 0)
        //            filename = filename.Remove(0, pos + 1);

        //        return filename.Trim('/');
        //    }
        //    catch (Exception ex) { throw new Exception(ex.Message + "\r\nBrutalZip error in NormalizedFilename"); }
        //}

    }
}
