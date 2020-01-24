/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 12/09/2019 17:47:24
 *
 */

using System.Collections.Generic;

using AutoMapper;

using UploadWebApi.Aplicacion.Mapeado;
using UploadWebApi.Aplicacion.Modelo;
using UploadWebApi.Models;

namespace UploadWebApi.Infraestructura.Mapeador
{
    /// <summary>
    /// 
    /// </summary>
    public class AutoMapperAdapter : IMapperService
    {

        readonly IMapper _iMapper; 


        public AutoMapperAdapter()
        {
            _iMapper = new Mapper(GetConfiguracion());

        }

        IConfigurationProvider GetConfiguracion()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<HuellaAceite, GetRowHuellaDto>();
                //.ForMember(dest => dest.FechaAnalisis, opt => opt.MapFrom(src => src.FechaHuella));

                cfg.CreateMap<HuellaAceite, GetHuellaDto>()
                .ForMember(dest => dest.IdRegistrador, opt => opt.MapFrom(src => src.Propietario))
                .ForMember(dest => dest.NombreRegistrador, opt => opt.MapFrom(src => src.NombrePropietario));

                //configuration.AssertConfigurationIsValid();
            });
            return configuration;

        }


        public TDto Map<TDomain, TDto>(TDomain domain) where TDto : class
        {
            return _iMapper.Map<TDto>(domain);
        }

        public IEnumerable<TDto> Map<TDomain, TDto>(IEnumerable<TDomain> domain) where TDto : class
        {
            return _iMapper.Map<IEnumerable<TDto>>(domain);
        }
    }
}