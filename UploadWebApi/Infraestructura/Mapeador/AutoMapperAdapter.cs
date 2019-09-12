/*
 * Copyright © 2019 Fundación del Olivar
 * Todos los derechos reservados
 *
 * Autor: Miguel A. Romera  - miguel
 * Fecha: 12/09/2019 17:47:24
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using UploadWebApi.Applicacion.Mapeado;
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
                cfg.CreateMap<HuellaDto, GetHuellaDto>()
                .ForMember(dest => dest.FechaAnalisis, opt => opt.MapFrom(src => src.FechaHuella));

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