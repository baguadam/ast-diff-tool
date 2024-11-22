using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ASTDiffTool.Shared
{
    public class CommandModifier
    {
        public List<JsonObject> ModifyCompileCommands(List<JsonObject> commands, string standard)
        {
            var modifiedCommands = new List<JsonObject>();

            foreach (var commandEntry in commands)
            {
                // deep copy of the current command entry
                var modifiedEntry = JsonNode.Parse(commandEntry.ToJsonString()).AsObject();

                if (modifiedEntry["command"] != null)
                {
                    string command = modifiedEntry["command"].ToString();

                    // replace or add the -std flag
                    if (command.Contains("-std=c++"))
                    {
                        command = Regex.Replace(command, @"-std=c\+\+\d{2}", $"-std={standard}");
                    }
                    else
                    {
                        command += $" -std={standard}";
                    }

                    modifiedEntry["command"] = command;
                }

                modifiedCommands.Add(modifiedEntry);
            }

            return modifiedCommands;
        }
    }
}
