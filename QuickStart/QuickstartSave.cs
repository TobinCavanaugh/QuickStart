using System.Collections.Generic;
using System.Linq;

namespace QuickStart
{
    public class QuickstartSave
    {
        public List<QProgram> programs = new List<QProgram>();
        public bool invalidated = false;

        public bool GetByAlias(string target, out QProgram program)
        {
            QProgram found = null;
            programs.ForEach(p =>
            {
                p.aliases.ForEach((a) =>
                {
                    if (target == a)
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

        public bool AddAliasesPath(string path, string[] newAliases, out QProgram program)
        {
            if (GetByPath(path, true, out program))
            {
                program.aliases.AddUniqueRange(newAliases);
                invalidated = true;
            }
            else
            {
                var p = new QProgram();
                p.aliases = newAliases.ToList();
                p.Path = path;
                programs.Add(p);
            }

            return false;
        }

        public bool AddPath(string path, out QProgram program)
        {
            if (programs.Any(x => x.Path == path))
            {
                program = null;
                return false;
            }
            else
            {
                var q = new QProgram(){Path = path};
                program = q;
                programs.Add(q);
                return true;
            }
        }
    }
}