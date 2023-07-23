using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PtahBuilder.BuildSystem.Steps.Output.SimpleText;

public record SimpleTextOutput(string Name, string Extension, string Path, string Contents);