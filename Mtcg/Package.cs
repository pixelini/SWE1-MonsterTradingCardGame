using System.Collections.Generic;
using NUnit.Framework;

namespace Mtcg
{
    public class Package
    {
        public float Price { get; set; }
        public List<ICard> Cards { get; set; }
    }
}