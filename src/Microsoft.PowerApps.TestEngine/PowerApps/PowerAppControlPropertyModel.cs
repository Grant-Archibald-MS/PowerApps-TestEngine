﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.PowerFx.Core.Public.Types;
using Microsoft.PowerFx.Core.Public.Values;

namespace Microsoft.PowerApps.TestEngine.PowerApps
{
    /// <summary>
    /// IUntypedObject model for a property of a Power App Control
    /// </summary>
    public class PowerAppControlPropertyModel : IUntypedObject
    {
        public IUntypedObject this[int index] => throw new NotImplementedException();

        // TODO: need to set the type correctly for the property
        public FormulaType Type => ExternalType.String;

        public string Name { get; set; }
        public string Value { get; set; }

        public PowerAppControlPropertyModel(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public int GetArrayLength()
        {
            throw new NotImplementedException();
        }

        public bool GetBoolean()
        {
            return bool.Parse(Value);
        }

        public double GetDouble()
        {
            return double.Parse(Value);
        }

        public string GetString()
        {
            return Value;
        }

        public bool TryGetProperty(string value, out IUntypedObject result)
        {
            throw new NotImplementedException();
        }
    }
}