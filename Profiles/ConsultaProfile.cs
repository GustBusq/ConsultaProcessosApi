using AutoMapper;
using ConsultaProcessosApi.Data.Dtos;
using ConsultaProcessosApi.Models;

namespace ConsultaProcessosApi.Profiles;

public class ConsultaProfile: Profile
{
    public ConsultaProfile()
    {
        CreateMap<ConsultaProcessos, ReadConsultaDto>()
            .ForMember(dto => dto.Tribunal,
                opt => opt.MapFrom(consulta => consulta.Tribunais.Sigla));
    }
}
