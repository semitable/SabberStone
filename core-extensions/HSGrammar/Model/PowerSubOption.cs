﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSGrammar.Model
{
    class PowerSubOption : PowerLogEntry
    {
        public int Id { get; set; }

        public PowerEntity Entitiy { get; set; }

        public List<PowerTarget> Targets { get; set; } = new List<PowerTarget>();

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine($"{GetType().Name}: Id={Id}");
            return str.ToString();
        }

        public override void Process()
        {
            throw new NotImplementedException();
        }
    }
}
