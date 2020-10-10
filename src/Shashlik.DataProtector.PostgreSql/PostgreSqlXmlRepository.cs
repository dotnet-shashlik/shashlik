// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection.Repositories;

namespace Shashlik.DataProtector.PostgreSql
{
    /// <summary>
    /// An XML repository backed by a Redis list entry.
    /// </summary>
    public class PostgreSqlXmlRepository : IXmlRepository
    {
        public PostgreSqlXmlRepository(PostgreSqlDataProtectorOptions options)
        {
            Options = options;
        }

        private PostgreSqlDataProtectorOptions Options { get; }
    

        /// <inheritdoc />
        public IReadOnlyCollection<XElement> GetAllElements()
        {
            return GetAllElementsCore().ToList().AsReadOnly();
        }

        private IEnumerable<XElement> GetAllElementsCore()
        {
            return null;
        }

        /// <inheritdoc />
        public void StoreElement(XElement element, string friendlyName)
        {
            
        }
    }
}