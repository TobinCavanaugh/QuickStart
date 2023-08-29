using System.Collections.Generic;
using System.Linq;

namespace QuickStart
{
    public class QuickstartSave
    {
        public List<QProgram> programs = new List<QProgram>();
        public bool invalidated = false;

        /// <summary>
        /// Gets the QProgram by alias, returns true if it succeeds, false if it fails (duh)
        /// </summary>
        /// <param name="target">The target alias to find, case insensitive</param>
        /// <param name="program">The out paramater of the resulting program</param>
        /// <returns></returns>
        public bool GetByAlias(string target, out QProgram program)
        {
            QProgram found = null;
            programs.ForEach(p =>
            {
                p.aliases.ForEach((a) =>
                {
                    if (target.ToLower() == a.ToLower())
                    {
                        found = p;
                    }
                });
            });

            if (found != null)
            {
                program = found;
                return true;
            }

            program = null;
            return false;
        }

        /// <summary>
        /// Gets the qProgram by path, creates a new one if getOnly is false
        /// </summary>
        /// <param name="path">The path to get</param>
        /// <param name="getOnly">False if you want it to create a path if one doesnt exist</param>
        /// <param name="program">The program the path has</param>
        /// <returns></returns>
        public bool GetByPath(string path, bool getOnly, out QProgram program)
        {
            foreach (var p in programs)
            {
                if (p.Path == path)
                {
                    program = p;
                    return true;
                }
            }

            if (getOnly)
            {
                program = null;
                return false;
            }


            AddPath(path, out program);
            return true;
        }

        
        public bool AddAliases(string oldAlias, string[] newAliases, out QProgram program)
        {
            if (GetByAlias(oldAlias, out program))
            {
                program.aliases.AddUniqueRange(newAliases);
                invalidated = true;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Add an array of aliases to a pre-existing path. Creates a new QProgram if one doesn't exist.
        /// </summary>
        /// <param name="path">The path</param>
        /// <param name="newAliases">The aliases</param>
        /// <param name="program">The out program parameter</param>
        /// <returns></returns>
        public bool AddAliasesPath(string path, string[] newAliases, out QProgram program)
        {
            if (GetByPath(path, true, out program))
            {
                program.aliases.AddUniqueRange(newAliases);
                invalidated = true;
                return true;
            }
            else
            {
                var p = new QProgram();
                p.aliases = newAliases.ToList();
                p.Path = path;
                programs.Add(p);
                return true;
            }
        }

        /// <summary>
        /// Adds a path if one doesn't exist
        /// </summary>
        /// <param name="path"></param>
        /// <param name="program"></param>
        /// <returns></returns>
        public bool AddPath(string path, out QProgram program)
        {
            if (programs.Any(x => x.Path == path))
            {
                program = null;
                return false;
            }
            else
            {
                var q = new QProgram() { Path = path };
                program = q;
                programs.Add(q);
                return true;
            }
        }
    }
}