using System.Collections.Generic;
using NUnit.Framework;

namespace Mtcg
{
    // this class is only for deserialization of json
    public class Package
    {
        public string PackageID { get; set; }
        public List<Card> Cards { get; set; }

    }

    public class Card
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public float Damage { get; set; }
    }

}