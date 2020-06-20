using System;
using System.Collections.Generic;
using System.Text;

namespace InputsBuilder.Models
{
    /// <summary>
    /// A simple struct, which holds the contents which should be added to the configuration files.
    /// </summary>
    public struct ConfigurationData
    {
        public string Inputs;
        public string Indexes;
        public string Props;
        public string Transforms;
    }
}
