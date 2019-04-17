using System.IO;
using System.Text.RegularExpressions;

namespace SCide.WPF.Helpers
{
    /// <summary>
    /// Purpose of this class is to get a list of subs and functions from a VBSCript
    /// </summary>
    public static class Parser
    {
        public static IdentifierMatches ParseVbScript(string content)
        {
            /* How to find the subs and functions?
             * We can use a regular expression
             * First of all, we'll read lines one by one
             * If it contains the string sub or function, we want to examine that match further.
             * 
             * The match:
             * - must be followed by the name of the routine or a _ character
             * - must not be preceded by a comment
             * - must not be part of a class
             * - can have leading blank space 
             * - can be preceded by a keyword such as private or public
             * 
             * The routine name:
             * - can have a trailing space
             * - can have a trailing ( bracket
             * 
             * In a perfect world scenario,
             * it must the be followed by (optional) text and 
             * an 'end sub' or 'end function' statement.
             * We do not check for that here
             */
            IdentifierMatches _matches = new IdentifierMatches();
            if (string.IsNullOrEmpty(content)) { return _matches; }

            using (StringReader sr = new StringReader(content))
            {
                // Loop over the lines in the string.
                int count = 0;
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    count++;
                    line = line.Trim();
                    if (!string.IsNullOrEmpty(line))
                    {
                        // find substring
                        //string exp = @"(sub|function)"; // too basic
                        string exp = @"(?<!(end|exit)\s+)(sub|function)"; // exclude any lines that have ' or end sub or exit sub
                        Regex regex = new Regex(exp, RegexOptions.IgnoreCase);
                        Match match = regex.Match(line);
                        if (match.Success)
                        {
                            // okay we have a line
                            if (line.StartsWith("'")) { continue; } // skip comment lines

                            // skip over classes
                            // TO DO

                            // theoretically, there can be a trailing _ between the sub/function keyword and the sub/functionname
                            // and even empty lines with just _ in them
                            // this would be valid scripting
                            // in that case, we just have to keep reading in lines
                            // until we get to the one that has the sub/functionname
                            if (line.EndsWith(" _")) // note the space; otherwise it can be part of an identifiername!
                            {
                                // while condition causes invalid parsing when there is an empty line
                                // not pretty, but in that case the script would be invalid anyway
                                while (!string.IsNullOrEmpty(line))
                                {
                                    line = sr.ReadLine().Trim();
                                    count++;
                                    if (line.Equals("_")) { continue; } // improbable but possible
                                    exp = @"^(\w+)"; // starts with word
                                    regex = new Regex(exp, RegexOptions.IgnoreCase);
                                    match = regex.Match(line);
                                    if (match.Success)
                                    {
                                        _matches.Add(new IdentifierMatch(count, match.Value));
                                        break; // found identifier, look no further but continue outer loop
                                    }
                                }
                            }
                            else
                            {
                                // inspect it closer
                                exp = @"(sub|function)\s+(\w+)"; // \s* allows for more than 1 whitespace
                                regex = new Regex(exp, RegexOptions.IgnoreCase);
                                match = regex.Match(line);
                                if (match.Success)
                                {
                                    _matches.Add(new IdentifierMatch(count, match.Groups[2].Value));
                                } // if
                            } // else
                        } // if
                    } // if
                } // if
            } // using
            return _matches;
        }
    }
}
