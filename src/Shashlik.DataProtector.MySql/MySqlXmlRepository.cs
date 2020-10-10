// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection.Repositories;

namespace Shashlik.DataProtector.MySql
{
    /// <summary>
    /// An XML repository backed by a Redis list entry.
    /// </summary>
    public class MySqlXmlRepository : IXmlRepository
    {
        private MySqlDataProtectorOptions Option { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public MySqlXmlRepository(MySqlDataProtectorOptions option)
        {
            Option = option;
        }

        /// <inheritdoc />
        public IReadOnlyCollection<XElement> GetAllElements()
        {
            return GetAllElementsCore().ToList().AsReadOnly();
        }

        private IEnumerable<XElement> GetAllElementsCore()
        {
            // Note: Inability to read any value is considered a fatal error (since the file may contain
            // revocation information), and we'll fail the entire operation rather than return a partial
            // set of elements. If a value contains well-formed XML but its contents are meaningless, we
            // won't fail that operation here. The caller is responsible for failing as appropriate given
            // that scenario.
            return null;
        }

        /// <inheritdoc />
        public void StoreElement(XElement element, string friendlyName)
        {
            // var database = _databaseFactory();
            // database.ListRightPush();
        }
    }
}