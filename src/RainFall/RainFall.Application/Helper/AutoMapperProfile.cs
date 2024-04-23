using AutoMapper;
using RainFall.Domain.Models;

namespace RainFall.Application.Helper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Reading, RainfallReading>()
            .ForMember(d => d.AmountMeasured, o => o.MapFrom(s => s.Value))
            .ForMember(d => d.DateMeasured, o => o.MapFrom(s => s.DateTime));

    }
}