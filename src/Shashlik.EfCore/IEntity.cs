using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlik.EfCore
{
    public interface IEntity
    {
    }
    public interface IEntity<TKey> : IEntity
    {
        TKey Id { get; set; }
    }
}
