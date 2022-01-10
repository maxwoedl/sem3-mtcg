using System;
using System.Collections.ObjectModel;

namespace MTCG.DAL
{
    public class PackageDTO
    {
        public Guid Id { get; set; }
        
        public int Price { get; set; }

        public Collection<Guid> Cards = new Collection<Guid>();
        
        public string Owner { get; set; }
    }
}