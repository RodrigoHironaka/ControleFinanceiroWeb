using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControlFinWeb.App.AutoMapper
{
    // T origem
    // G destino
    public class AutoMapperConfig<T, G>
        where T : class 
        where G : class
    {
        public static G Mapear(T origem)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<T, G>());
            var mapper = new Mapper(config);
            return mapper.Map<G>(origem);
        }
    }
}
