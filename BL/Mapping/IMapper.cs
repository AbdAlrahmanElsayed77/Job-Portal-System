using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Mapping
{
    public interface IMapper
    {
        public TDestination Map<TSource, TDestination>();
    }
}
